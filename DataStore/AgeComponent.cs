using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public enum AgeUnit {
        Years,
        Month,
        Days,
        Minutes
    }

    public class AgeComponent : Component
    {
        public double Value { get; set; }
        public bool IsMissing { get; set; }
        public AgeUnit Unit { get;set;}
    }
}
