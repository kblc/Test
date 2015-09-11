using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    //internal class InternalDataMeasurement
    //{
    //    /// <summary>
    //    /// Measurement identifier
    //    /// </summary>
    //    public long MeasurementId { get; set; }
    //    /// <summary>
    //    /// Measurement of growth
    //    /// </summary>
    //    public double Height { get; set; }
    //    /// <summary>
    //    /// Measurement date
    //    /// </summary>
    //    public DateTime Timestamp { get; set; }
    //    /// <summary>
    //    /// Is any data missing
    //    /// </summary>
    //    public bool IsMissing { get; set; }
    //}

    //internal class InternalDataPacient
    //{
    //    /// <summary>
    //    /// Pacient identifier
    //    /// </summary>
    //    public long PacientId { get; set; }
    //    /// <summary>
    //    /// Pacient measurements
    //    /// </summary>
    //    public IEnumerable<InternalDataMeasurement> Measurements { get; set; }
    //}

    //internal class InternalDataDoctor
    //{
    //    /// <summary>
    //    /// Doctor identifier
    //    /// </summary>
    //    public long DoctorId { get; set; }
    //    /// <summary>
    //    /// Pacients
    //    /// </summary>
    //    public IEnumerable<InternalDataPacient> Pacient { get; set; }
    //}

    internal class InternalDataTimestamp
    {
        public long MeasurementId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class InternalDataMeasurement
    {
        public long MeasurementId { get; set; }
        public long PacientId { get; set; }
        public long Heigh { get; set; }
    }

    internal class InternalData
    {
        /// <summary>
        /// Doctors
        /// </summary>
        //public IEnumerable<InternalDataDoctor> Doctors { get; set; }

        public IEnumerable<long> Doctors { get; set; }
        public IEnumerable<long> Pacients { get; set; }
        public IEnumerable<InternalDataMeasurement> Measurements { get; set; }
        public IEnumerable<InternalDataTimestamp> Timestamps { get; set; }

    }
}
