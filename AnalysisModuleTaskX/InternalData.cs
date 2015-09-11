using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    internal class InternalData
    {
        /// <summary>
        /// Doctor entity identifier
        /// </summary>
        public long DoctorId { get; set; }

        /// <summary>
        /// Pacient entity identifier
        /// </summary>
        public Nullable<long> PacientId { get; set; }
        
        /// <summary>
        /// Doctor height measurenment (If null then it is missing measurement)
        /// </summary>
        public Nullable<double> Height { get; set; }
    }
}
