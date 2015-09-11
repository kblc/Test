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
            CalculationType = CalculationType.ByDoctor;
            CalculationTimeType = CalculationTimeType.PerWeek;
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
        /// <summary>
        /// Calculate results by any part of year (year/month/week/day)
        /// </summary>
        public CalculationTimeType CalculationTimeType { get; set; }

        private InternalData data = null;
        public void FetchData(DataStore.MainInterface dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException("dataStore");

            var d = new InternalData();

            d.Doctors = dataStore.Entities
                .Where(e => IsDoctor(e))
                .Select(e => new InternalDataDoctor() { DoctorId = e.Id })
                .ToArray();
            d.Pacients = dataStore.Relations
                .Where(r => IsDoctor(r.Item1) && IsPacient(r.Item2))
                .Select(r => new InternalDataPacient() { DoctorId = r.Item1.Id, PacientId = r.Item2.Id })
                .ToArray();
            d.Measurements = dataStore.Relations
                .Where(r => IsPacient(r.Item1) && IsMeasurement(r.Item2))
                .Select(r => new InternalDataMeasurement() { PacientId = r.Item1.Id, MeasurementId = r.Item2.Id })
                .ToArray();
            var dmIds = d.Measurements.Select(m => m.MeasurementId).ToList();

            d.Timestamps = dataStore.TimestampComponents
                .Where(ts => !ts.IsMissing)
                .Join(dmIds, tc => tc.EntityId, i => i, (ts, i) => ts) //Join too fast then [array].Contains()
                //.Where(ts => dmIds.Contains(ts.EntityId))
                .Select(ts => new InternalDataTimestamp() { MeasurementId = ts.EntityId, Timestamp = ts.Timestamp })
                .ToArray();
            d.HeighComponent = dataStore.HeightComponents
                .Where(h => !h.IsMissing)
                .Join(dmIds, hc => hc.EntityId, i => i, (hs, i) => hs) //Join too fast then [array].Contains()
                //.Where(h => dmIds.Contains(h.EntityId))
                .Select(h => new InternalDataHeighComponent() { MeasurementId = h.EntityId, Height = (h.Unit == DataStore.LengthUnit.Inch ? h.Value : h.Value * 39.37) })
                .ToArray();

            data = d;
        }

        private Calculator calc = null;
        public void Calculate()
        {
            if (data == null)
                throw new ArgumentNullException("data", "Need execute FetchData() first.");
            
            var result = (calc ?? (calc = new Calculator()))
                .Calculate(data, CalculationType, CalculationTimeType, ExcludeDoctorsWithotPacients, ExcludePacientsWithoutMeasurements);
            
            foreach (var res in result)
                AnalysisModule.MessageBus.Instance.PublishResult(res);
        }
    }
}
