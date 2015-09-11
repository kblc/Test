using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    internal class Calculator
    {
        internal AnalysisModule.SimpleCalculationResult[] Calculate(List<InternalData> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var res = data
                    .GroupBy(i => new { i.DoctorId, i.PacientId })
                    .Select(g => new
                    {
                        g.FirstOrDefault().DoctorId,
                        g.FirstOrDefault().PacientId,
                        MaxHeight = !g.FirstOrDefault().PacientId.HasValue || g.Where(i => i.Height.HasValue).Count() == 0 ? 0 : g.Where(i => i.Height.HasValue).Select(i => i.Height.Value).Where(i => i >= 0).Max(),
                        MinHeight = !g.FirstOrDefault().PacientId.HasValue || g.Where(i => i.Height.HasValue).Count() == 0 ? 0 : g.Where(i => i.Height.HasValue).Select(i => i.Height.Value).Where(i => i >= 0).Min(),
                        Count = !g.FirstOrDefault().PacientId.HasValue ? 0 : g.Count(i => i.Height.HasValue)
                    })
                    .Select(i => new { i.DoctorId, i.PacientId, HeightAvgInc = i.Count == 0 ? 0 : (i.MaxHeight - i.MinHeight) / (double)i.Count }) //Get increment by measurement count per docror and patient
                    .GroupBy(i => i.DoctorId)
                    .Select(g => new { g.FirstOrDefault().DoctorId, HeightAvgInc = g.Average(i => i.HeightAvgInc) }) //Get average increment by measurement count per doctor
                    .Select(i => new AnalysisModule.SimpleCalculationResult() 
                        { 
                            AnalysisName = string.Format("TestXCalculation for doctor (id:{0})", i.DoctorId), 
                            Result = i.HeightAvgInc 
                        })
                    .ToArray();

            var res2 = res.Select(i => i.Result).ToArray();

            return res
                .Union(new AnalysisModule.SimpleCalculationResult[] 
                { 
                    new AnalysisModule.SimpleCalculationResult 
                    { 
                        AnalysisName = "TestXCalculation (total average)", 
                        Result = res2.Any() ? res2.Average() : 0 
                    } 
                })
                .ToArray();
        }
    }
}
