using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleSimpleCount
{
    public class Calculator
    {
        public AnalysisModule.SimpleCalculationResult Calculate(List<InternalData> data)
        {
            return new AnalysisModule.SimpleCalculationResult() { 
                AnalysisName="SimpleCalculation",
                Result = data.Count(d => !Double.IsInfinity(d.Height) && !Double.IsNaN(d.Height) && !Double.IsNegativeInfinity(d.Height) && !Double.IsPositiveInfinity(d.Height)) 
            };
        }
    }
}
