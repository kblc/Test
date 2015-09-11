using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    public class InternalDataPacient
    {
        /// <summary>
        /// Pacient identifier
        /// </summary>
        public long PacientId { get; set; }
        /// <summary>
        /// Doctor identifier
        /// </summary>
        public long DoctorId { get; set; }
    }

    public class InternalDataDoctor
    {
        /// <summary>
        /// Doctor identifier
        /// </summary>
        public long DoctorId { get; set; }
    }

    public class InternalDataTimestamp
    {
        public long MeasurementId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class InternalDataHeighComponent
    {
        public long MeasurementId { get; set; }
        public double Height { get; set; }
    }

    public class InternalDataMeasurement
    {
        public long MeasurementId { get; set; }
        public long PacientId { get; set; }
    }

    public class InternalData
    {
        public InternalData()
        {
            Doctors = new List<InternalDataDoctor>();
            Pacients = new List<InternalDataPacient>();
            Measurements = new List<InternalDataMeasurement>();
            Timestamps = new List<InternalDataTimestamp>();
            HeighComponent = new List<InternalDataHeighComponent>();
        }

        public IList<InternalDataDoctor> Doctors { get; set; }
        public IList<InternalDataPacient> Pacients { get; set; }
        public IList<InternalDataMeasurement> Measurements { get; set; }
        public IList<InternalDataTimestamp> Timestamps { get; set; }
        public IList<InternalDataHeighComponent> HeighComponent { get; set; }
    }
}
