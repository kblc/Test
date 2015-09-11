using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    public class Implementation : AnalysisModule.IDataPreparator
    {
        #region Comparison methods

        private const string DoctorEntityName = "doctor";
        private const string PacientEntityName = "person";
        private const string MeasurementEntityName = "measurement";
        private static bool IsItRightEntity(DataStore.Entity ent, string name)
        {
            return ent != null && string.Compare(ent.Tag, name, true) == 0;
        }
        private static bool IsPacient(DataStore.Entity ent) { return IsItRightEntity(ent, PacientEntityName); }
        private static bool IsMeasurement(DataStore.Entity ent) { return IsItRightEntity(ent, MeasurementEntityName); }
        private static bool IsDoctor(DataStore.Entity ent) { return IsItRightEntity(ent, DoctorEntityName); }

        #endregion

        public Implementation()
        {
            CalculationType = CalculationType.ByPacient;
            ExcludeDoctorsWithotPacients = false;
            ExcludePacientsWithoutMeasurements = true;
        }

        /// <summary>
        /// Excludes from result doctors with no one pacient
        /// </summary>
        public bool ExcludeDoctorsWithotPacients { get; set; }
        /// <summary>
        /// Excludes from result pacients with one or no one measuremtns
        /// </summary>
        public bool ExcludePacientsWithoutMeasurements { get; set; }
        /// <summary>
        /// Calculate average data by each doctor or by each doctor's pacient
        /// </summary>
        public CalculationType CalculationType { get; set; }

        private InternalData data = null;
        public void FetchData(DataStore.MainInterface dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException("dataStore");

            data = new InternalData()
            {
                //Get all doctors to data store
                Doctors = dataStore.Entities
                            .Where(e => IsDoctor(e))
                            .Select(e => new InternalDataDoctor() { DoctorId = e.Id })
                            .ToArray()
            };

            //Get pacient for each doctor (Prepare array for quick selection after)
            var allPacientsForAllDoctors = dataStore.Relations
                    .Where(r => IsDoctor(r.Item1) && IsPacient(r.Item2))
                    .Select(r => new { DoctorId = r.Item1.Id, PacientId = r.Item2.Id })
                    .ToArray();
            //Fill pacients for each doctor in data store
            foreach(var d in data.Doctors)
            {
                d.Pacient = allPacientsForAllDoctors
                    .Where(i => i.DoctorId == d.DoctorId)
                    .Select(i => new InternalDataPacient() { PacientId = i.PacientId })
                    .ToArray();
            }

            //Get all measurements for each pacient for each doctor
            var allMeasurementForAllPacients = dataStore.Relations
                    .Where(r => IsPacient(r.Item1) && IsMeasurement(r.Item2))
                    .Select(r => new 
                        { 
                            PacientId = r.Item1.Id, 
                            MeasurementId = r.Item2.Id
                        })
                    //For increase speed use some joins
                    .LeftOuterJoin(dataStore.HeightComponents, i => i.MeasurementId, hc => hc.EntityId, (i, hc) => new { i.PacientId, i.MeasurementId, HeightComponent = hc })
                    .LeftOuterJoin(dataStore.TimestampComponents, i => i.MeasurementId, tc => tc.EntityId, (i, tc) => new { i.PacientId, i.MeasurementId, i.HeightComponent, TimestampComponent = tc })
                    .ToArray();
            //Fill measurements for each pacient id data store
            foreach (var d in data.Doctors)
                foreach(var p in d.Pacient)
                {
                    p.Measurements = allMeasurementForAllPacients
                        .Where(i => i.PacientId == p.PacientId)
                        .Select(i => new InternalDataMeasurement()
                        {
                            MeasurementId = i.MeasurementId,
                            IsMissing = i.HeightComponent == null || i.TimestampComponent == null || i.HeightComponent.IsMissing || i.TimestampComponent.IsMissing,
                            Timestamp = i.TimestampComponent != null ? i.TimestampComponent.Timestamp : DateTime.MinValue,
                            Height = i.HeightComponent != null ? (i.HeightComponent.Unit == DataStore.LengthUnit.Inch ? i.HeightComponent.Value : i.HeightComponent.Value * 39.37) : 0,
                        })
                        .ToArray();

                    //##################### WARNING #####################
                    // We may use this code without LeftOuterJoin(), but it is to slow!!!
                    //###################################################
                    //foreach(var m in p.Measurements)
                    //{
                    //    var hComponent = dataStore.HeightComponents.FirstOrDefault(hc => hc.EntityId == m.MeasurementId);
                    //    if (hComponent != null)
                    //    {
                    //        m.IsMissing &= hComponent.IsMissing;
                    //        m.Height = (hComponent.Unit == DataStore.LengthUnit.Inch ? hComponent.Value : hComponent.Value * 39.37);
                    //    }
                    //    else
                    //        m.IsMissing = true;

                    //    var dComponent = dataStore.TimestampComponents.FirstOrDefault(dc => dc.EntityId == m.MeasurementId);
                    //    if (dComponent != null)
                    //    {
                    //        m.IsMissing &= dComponent.IsMissing;
                    //        m.Timestamp = dComponent.Timestamp;
                    //    }
                    //    else
                    //        m.IsMissing = true;
                    //}
                }

            //all needed data stored
        }

        private Calculator calc = null;
        public void Calculate()
        {
            if (data == null)
                throw new ArgumentNullException("data", "Need execute FetchData() first.");
            
            var result = (calc ?? (calc = new Calculator()))
                .Calculate(data, CalculationType, ExcludeDoctorsWithotPacients, ExcludePacientsWithoutMeasurements);
            
            foreach (var res in result)
                AnalysisModule.MessageBus.Instance.PublishResult(res);
        }
    }
}
