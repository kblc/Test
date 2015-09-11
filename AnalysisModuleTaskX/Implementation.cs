using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    public class Implementation : AnalysisModule.IDataPreparator
    {
        private List<InternalData> data = null;
        public void FetchData(DataStore.MainInterface dataStore)
        {
            if (dataStore == null)
                throw new ArgumentNullException("dataStore");

            #region Comparison methods
            var isPacient = new Func<DataStore.Entity, bool>((ent) => ent != null && string.Compare(ent.Tag, "person", true) == 0);
            var isDoctor = new Func<DataStore.Entity, bool>((ent) => ent != null && string.Compare(ent.Tag, "doctor", true) == 0);
            var isMeasurement = new Func<DataStore.Entity, bool>((ent) => ent != null && string.Compare(ent.Tag, "measurement", true) == 0);
            #endregion

            data = new List<InternalData>(
                dataStore.Entities.Where(e => isDoctor(e))
                .Select(i => new { Doctor = i })
                .LeftOuterJoin(dataStore.Relations.Where(t => isDoctor(t.Item1) && isPacient(t.Item2)), i => i.Doctor, r => r.Item1, (i, r) => new { i.Doctor, Pacient = r == null ? null : r.Item2 })
                .LeftOuterJoin(dataStore.Relations.Where(t => isPacient(t.Item1) && isMeasurement(t.Item2)), i => i.Pacient, r => r.Item1, (i, r) => new { i.Doctor, i.Pacient, Measurement = r == null ? null : r.Item2 })
                .LeftOuterJoin(dataStore.HeightComponents.Where(hc => !hc.IsMissing), i => i.Measurement == null ? -1 : i.Measurement.Id, hc => hc.EntityId, (i, hc) => new { i.Measurement, i.Doctor, i.Pacient, Height = hc == null ? (Nullable<double>)null : (Nullable<double>)(hc.Unit == DataStore.LengthUnit.Inch ? hc.Value : hc.Value * 39.37) }) //Meters to Inch conversation
                .LeftOuterJoin(dataStore.TimestampComponents.Where(ts => !ts.IsMissing), i => i.Measurement == null ? -1 : i.Measurement.Id, hc => hc.EntityId, (i, ts) => new { i.Measurement, i.Doctor, i.Pacient, Height = ts == null ? (Nullable<double>)null : i.Height }) //If timestamp corrupt then exclude height measurement
                .Select(i => new InternalData
                {
                    DoctorId = i.Doctor.Id,
                    PacientId = i.Pacient == null ? (Nullable<long>)null : i.Pacient.Id,
                    Height = i.Height
                }));
        }

        private Calculator calc = null;
        public void Calculate()
        {
            if (data == null)
                throw new ArgumentNullException("data", "Need execute FetchData() first.");
            var result = (calc ?? (calc = new Calculator())).Calculate(data);
            foreach (var res in result)
                AnalysisModule.MessageBus.Instance.PublishResult(res);
        }
    }
}
