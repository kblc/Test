using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FillDataStore
{
    public class Filler
    {
        DataStore.MainInterface dataStore=null;
        List<DateTime> dates = new List<DateTime>();
        int entityIdCounter = 0;

        Random rng = new Random(33);


        public Filler( DataStore.MainInterface storeToFill  ) {
            dataStore = storeToFill;

            DateTime basis = DateTime.UtcNow;
            Enumerable.Range(0, 100).ToList().ForEach(_ =>
            {
                dates.Add(basis);
                basis = basis.Add(new TimeSpan(1, rng.Next(0, 3), rng.Next(0, 2), rng.Next(0, 2)));
            });
        }



        public void DoIt(int numberOfDoctors) {
            if (dataStore == null)
            {
                throw new Exception("Filler class was not initialized correctly.");
            }

            for (int i = 0; i < numberOfDoctors; i++)
            {
                CreateSite(100);
            }

        }

        private void CreateSite(int countPatients)
        {
            DataStore.Doctor doc = new DataStore.Doctor()
            {
                Id = entityIdCounter++,
                Tag="doctor"
            };

            dataStore.Entities.Add(doc);

            for(int i=0; i<countPatients; i++) {
                CreatePatient(doc);
            }
        }

        private void CreatePatient(DataStore.Doctor doc)
        {
            DataStore.Person person = new DataStore.Person()
            {
                Id = entityIdCounter++,
                Tag = "person"
            };
            dataStore.Entities.Add(person);
            dataStore.Relations.Add(new Tuple<DataStore.Entity, DataStore.Entity>(doc, person));

            bool missing = rng.Next(0, 100) == 1;

            DataStore.AgeComponent age = new DataStore.AgeComponent()
            {
                EntityId = person.Id,
                Unit = rng.Next(0, 10000) >= 10000 - 1 ? DataStore.AgeUnit.Years : DataStore.AgeUnit.Month,
                IsMissing = missing,
                Value = missing ? rng.NextDouble() : rng.Next(30, 70)
            };

            dataStore.AgeComponents.Add(age);

            foreach (var visitDate in dates)
            {
                DataStore.Measurement meas = new DataStore.Measurement()
                {
                    Id=entityIdCounter++,
                    Tag="measurement"
                };
                dataStore.Entities.Add(meas);
                dataStore.Relations.Add(new Tuple<DataStore.Entity, DataStore.Entity>(person, meas));

                bool missing2 = rng.Next(0, 100) == 1;

                DataStore.HeightComponent height = new DataStore.HeightComponent()
                {
                    EntityId = meas.Id,
                    Unit = rng.Next(0, 10000) >= 10000 - 1 ? DataStore.LengthUnit.Inch: DataStore.LengthUnit.Meter,
                    IsMissing = missing2,
                    Value = missing2 ? rng.NextDouble() : (rng.NextDouble()+1)
                };

                bool missing3 = rng.Next(0, 100) == 1;
                DataStore.TimestampComponent timestamp = new DataStore.TimestampComponent()
                {
                    EntityId=meas.Id,
                    IsMissing=missing3,
                    Timestamp= missing3?DateTime.MinValue:visitDate
                };

                dataStore.TimestampComponents.Add(timestamp);
                dataStore.HeightComponents.Add(height);
            }
        }
      
    }
}
