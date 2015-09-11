using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTaskMainProject
{
    class Program
    {
        static void Main(string[] args)
        {
            DataStore.MainInterface dataStore = new DataStore.MainInterface();

            FillDataStore.Filler dataGenerator = new FillDataStore.Filler(dataStore);

            dataGenerator.DoIt(20);

            AnalysisModuleSimpleCount.Implementation impl = new AnalysisModuleSimpleCount.Implementation();


            
            impl.FetchData(dataStore);
            impl.Calculate();

            Console.ReadKey();
        }
    }
}
