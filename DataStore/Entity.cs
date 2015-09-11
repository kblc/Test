using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public abstract class Entity
    {
        public long Id { get; set; }

        public string Tag { get; set; }
    }
}
