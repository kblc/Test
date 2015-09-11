using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStore
{
    public class MainInterface
    {
        public List<Entity> Entities = new List<Entity>();
        public List<Tuple<Entity, Entity>> Relations = new List<Tuple<Entity, Entity>>();
        public List<HeightComponent> HeightComponents = new List<HeightComponent>();
        public List<AgeComponent> AgeComponents = new List<AgeComponent>();
        public List<TimestampComponent> TimestampComponents = new List<TimestampComponent>();
    }
}
