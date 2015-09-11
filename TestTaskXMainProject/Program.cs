using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestTaskXMainProject
{
    class Program
    {
        static void Main(string[] args)
        {
            DataStore.MainInterface dataStore = new DataStore.MainInterface();

            FillDataStore.Filler dataGenerator = new FillDataStore.Filler(dataStore);

            dataGenerator.DoIt(20);

            AnalysisModuleTaskX.Implementation impl = new AnalysisModuleTaskX.Implementation();

            impl.FetchData(dataStore);
            impl.Calculate();

            Console.ReadKey();
        }
    }
}
