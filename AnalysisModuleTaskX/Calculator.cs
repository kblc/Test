using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModuleTaskX
{
    public enum CalculationType
    {
        ByPacient = 0,
        ByDoctor
    }

    internal class Calculator
    {
        /// <summary>
        /// Calculation task name
        /// </summary>
        private const string CalcTaskName = "TestXCalculation";
        /// <summary>
        /// Calculate stored data
        /// </summary>
        /// <param name="data">Internal data storage</param>
        /// <param name="calcType">Calculate average data by each doctor or by each doctor's pacient</param>
        /// <param name="excludeDoctorsWithotPacients">Excludes from result doctors with no one pacient</param>
        /// <param name="excludePacientsWithoutMeasurements">Excludes from result pacients with one or no one measuremtns</param>
        /// <returns>Calculation result</returns>
        internal AnalysisModule.SimpleCalculationResult[] Calculate(
            InternalData data,
            CalculationType calcType,
            bool excludeDoctorsWithotPacients,
            bool excludePacientsWithoutMeasurements)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var res = new List<AnalysisModule.SimpleCalculationResult>();
            //Exclude from data doctors without pacients (is setted)
            var doctors = data.Doctors
                        .Where(d => !excludeDoctorsWithotPacients || d.Pacient.Any());
            //Exclude from data pacients with one or less measurements (is setted)
            var pacientsWithDoctors = doctors
                            .SelectMany(d => d.Pacient
                            .Where(p => !excludePacientsWithoutMeasurements || p.Measurements.Count(m => !m.IsMissing) > 1)
                            .Select(p => new { Pacinent = p, Doctor = d } ));
            
            //Prepare data with calculated results for each doctor and each pacient
            var preparedData = pacientsWithDoctors
                .Select(p => new
                {
                    PacientId = p.Pacinent.PacientId,
                    DoctorId = p.Doctor.DoctorId,
                    Measurements = p.Pacinent.Measurements.Where(m => !m.IsMissing).OrderBy(m => m.Timestamp),
                    Count = p.Pacinent.Measurements.Count(m => !m.IsMissing),
                })
                .Select(p => new
                {
                    p.PacientId,
                    p.DoctorId,
                    GrowthMeasurements = p.Count > 1
                        ? Enumerable.Range(0, p.Count - 1)
                            .Select(i => new //Get measurement pairs for pacient
                            {
                                Start = p.Measurements.ElementAt(i),
                                End = p.Measurements.ElementAt(i + 1)
                            })
                            .Select(i => ((double)(i.End.Height - i.Start.Height) / ((i.End.Timestamp - i.Start.Timestamp).TotalDays / (double)7)) ) //calc growth per week
                            .ToArray()
                        : new double[] {},
                })
                .Select(p => new
                {
                    p.PacientId,
                    p.DoctorId,
                    MeanRateGrowhPerWeek = p.GrowthMeasurements.Any() ? p.GrowthMeasurements.Average() : (double)0,
                })
                .ToArray();

            foreach(var doc in doctors)
            {
                var dt = preparedData.Where(pd => pd.DoctorId == doc.DoctorId);
                switch (calcType)
                {
                    case CalculationType.ByPacient:
                        res.AddRange(
                            dt.Select(i => new AnalysisModule.SimpleCalculationResult()
                            {
                                Result = i.MeanRateGrowhPerWeek,
                                AnalysisName = string.Format("{0} for pacient (id:{1}) of doctor (id:{2})", CalcTaskName, i.PacientId, doc.DoctorId)
                            })
                            );
                        break;

                    case CalculationType.ByDoctor:
                        res.Add(new AnalysisModule.SimpleCalculationResult()
                        {
                            Result = dt.Any() ? dt.Average(i => i.MeanRateGrowhPerWeek) : 0,
                            AnalysisName = string.Format("{0} for doctor (id:{1})", CalcTaskName, doc.DoctorId)
                        });
                        break;
                }
            }
            return res.ToArray();
        }
    }
}
