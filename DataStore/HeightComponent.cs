using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public enum LengthUnit
    {
        Inch,
        Meter
    }

    public class HeightComponent : Component
    {
        public double Value { get; set; }
        public bool IsMissing { get; set; }
        public LengthUnit Unit { get; set; }
    }
}
