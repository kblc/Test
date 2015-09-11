using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnalysisModuleSimpleCountTests
{
    [TestClass]
    public class TestCalculator
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Calculator_Calculate_ErrorOnNullArgument()
        {
            var calc = new AnalysisModuleSimpleCount.Calculator();
            calc.Calculate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Calculator_Calculate_ErrorOnEmptyListOfValues()
        {
            var calc = new AnalysisModuleSimpleCount.Calculator();
            calc.Calculate(new List<AnalysisModuleSimpleCount.InternalData>());
        }
    }
}
