using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisModuleSimpleCountTests
{
    [TestClass]
    public class TestImplementation
    {
        DataStore.MainInterface dataStore;

        [TestInitialize]
        public void Setup()
        {
            dataStore = new DataStore.MainInterface();
            FillDataStore.Filler dataGenerator = new FillDataStore.Filler(dataStore);
            dataGenerator.DoIt(20);
        }

        [TestMethod]
        public void Implementation_FetchData_NoDataAlteration()
        {
            var hash = dataStore.GetHashCode();
            var impl = new AnalysisModuleTaskX.Implementation();
            impl.FetchData(dataStore);
            Assert.AreEqual(hash, dataStore.GetHashCode());
        }

        [TestMethod]
        public void Implementation_Calculate_NoDataAlteration()
        {
            var hash = dataStore.GetHashCode();

            var impl = new AnalysisModuleTaskX.Implementation();
            impl.FetchData(dataStore);
            impl.Calculate();
            Assert.AreEqual(hash, dataStore.GetHashCode());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Implementation_Calculate_DoesNotRunWithoutFetchingFirst()
        {
            var impl = new AnalysisModuleTaskX.Implementation();
            impl.Calculate();
        }
    }
}
