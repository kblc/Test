using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    internal class InternalDataPacient
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

    internal class InternalDataDoctor
    {
        /// <summary>
        /// Doctor identifier
        /// </summary>
        public long DoctorId { get; set; }
    }

    internal class InternalDataTimestamp
    {
        public long MeasurementId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    internal class InternalDataHeighComponent
    {
        public long MeasurementId { get; set; }
        public double Height { get; set; }
    }

    internal class InternalDataMeasurement
    {
        public long MeasurementId { get; set; }
        public long PacientId { get; set; }
    }

    internal class InternalData
    {
        /// <summary>
        /// Doctors
        /// </summary>
        //public IEnumerable<InternalDataDoctor> Doctors { get; set; }

        public IEnumerable<InternalDataDoctor> Doctors { get; set; }
        public IEnumerable<InternalDataPacient> Pacients { get; set; }
        public IEnumerable<InternalDataMeasurement> Measurements { get; set; }
        public IEnumerable<InternalDataTimestamp> Timestamps { get; set; }
        public IEnumerable<InternalDataHeighComponent> HeighComponent { get; set; }
    }
}
