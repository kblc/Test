using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class TimestampComponent : Component
    {
        public DateTime Timestamp { get; set; }
        public bool IsMissing { get; set; }
    }
}
