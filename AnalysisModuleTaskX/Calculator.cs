using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace AnalysisModuleTaskX
{
    public enum CalculationType
    {
        ByPacient = 0,
        ByDoctor
    }

    public enum CalculationTimeType
    {
        [Description("year")]
        PerYear = 0,
        [Description("month")]
        PerMonth,
        [Description("week")]
        PerWeek,
        [Description("day")]
        PerDay
    }

    internal class Calculator
    {
        /// <summary>
        /// Return time part for totalDays with selected calculation time type
        /// </summary>
        /// <param name="totalDays">Total days</param>
        /// <param name="calcTime">Calculation time type</param>
        /// <returns>Double value in selected time part</returns>
        private static double GetTimePart(double totalDays, CalculationTimeType calcTime)
        {
            double dec = 1d;
            switch (calcTime)
            {
                case CalculationTimeType.PerYear:
                    dec = 365.25d;
                    break;
                case CalculationTimeType.PerMonth:
                    dec = ((365.25d / 12d));
                    break;
                case CalculationTimeType.PerWeek:
                    dec = 7d;
                    break;
                case CalculationTimeType.PerDay:
                    dec = 1d;
                    break;
            }
            return totalDays / dec; 
        }

        /// <summary>
        /// Calculation task name
        /// </summary>
        private const string CalcTaskName = "TestXCalculation";
        /// <summary>
        /// Calculate stored data
        /// </summary>
        /// <param name="data">Internal data storage</param>
        /// <param name="calcType">Calculate average data by each doctor or by each doctor's pacient</param>
        /// <param name="calcTimeType">Calculate results by any part of year (year/month/week/day)</param>
        /// <param name="excludeDoctorsWithotPacients">Excludes from result doctors with no one pacient</param>
        /// <param name="excludePacientsWithoutMeasurements">Excludes from result pacients with one or no one measuremtns</param>
        /// <returns>Calculation result</returns>
        internal AnalysisModule.SimpleCalculationResult[] Calculate(
            InternalData data,
            CalculationType calcType,
            CalculationTimeType calcTimeType,
            bool excludeDoctorsWithotPacients,
            bool excludePacientsWithoutMeasurements)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var res = new List<AnalysisModule.SimpleCalculationResult>();

            var bigTableData =
                data.Doctors
                .LeftOuterJoin(data.Pacients, d => d.DoctorId, p => p.DoctorId, (d, p) => new { DoctorId = d.DoctorId, PacientId = p == null ? (long?)null : p.PacientId })
                .LeftOuterJoin(data.Measurements, i => i.PacientId, m => m.PacientId, (i, m) => new { i.DoctorId, i.PacientId, MeasurementId = m == null ? (long?)null : m.MeasurementId })
                .LeftOuterJoin(data.Timestamps, i => i.MeasurementId, t => t.MeasurementId, (i, t) => new { i.DoctorId, i.PacientId, i.MeasurementId, Timestamp = t == null ? (DateTime?)null : t.Timestamp })
                .LeftOuterJoin(data.HeighComponent
                    .Where(h => h.Height > 0 
                        && !double.IsInfinity(h.Height) 
                        && !double.IsNegativeInfinity(h.Height)
                        && !double.IsPositiveInfinity(h.Height)
                        && !double.IsNaN(h.Height)
                        ), 
                    i => i.MeasurementId, hc => hc.MeasurementId, (i, hc) => new { i.DoctorId, i.PacientId, i.MeasurementId, i.Timestamp, Heigh = hc == null ? (double?)null : hc.Height })
                .ToArray();

            //Table generated. Group data by Doctor, Pacient and Measurement
            var doctors = bigTableData
                .GroupBy(i => i.DoctorId)
                .Select(g => new
                {
                    g.FirstOrDefault().DoctorId,
                    //Group data by pacient
                    Pacients = g.Where(p => p != null)
                        .GroupBy(g2 => g2.PacientId)
                        .Select(g2 => new
                        {
                            g2.FirstOrDefault().PacientId,
                            Measurements = g2
                                .Where(i => i.Heigh != null && i.Timestamp != null)
                                .OrderBy(m => m.Timestamp)
                                .ToArray()
                        })
                        .Select(g2 => new
                        {
                            g2.PacientId,
                            MeasurementsExists = g2.Measurements.Count() > 1,
                            Measurements = g2.Measurements.Count() > 1
                                ? Enumerable.Range(0, g2.Measurements.Count() - 1)
                                    .Select(i => new //Get measurement pairs for pacient
                                    {
                                        Start = g2.Measurements.ElementAt(i),
                                        End = g2.Measurements.ElementAt(i + 1)
                                    })
                                    .Select(i => new //Calc start and end measure data
                                    {
                                        HeightStart = i.Start.Heigh.Value,
                                        HeightEnd = i.End.Heigh.Value,
                                        TimestampStart = i.Start.Timestamp.Value,
                                        TimestampEnd = i.End.Timestamp.Value,
                                    })
                                    .Select(i => new //get height change by part of time
                                    {
                                        HeightChange = i.HeightEnd - i.HeightStart,
                                        TimeParts = GetTimePart((i.TimestampEnd - i.TimestampStart).TotalDays, calcTimeType),
                                    })
                                    .Select(i => i.HeightChange / i.TimeParts) //calc growth per week
                                    .ToArray()
                                : Enumerable.Empty<double>().ToArray() //return empty array if no one or one only measurement
                        })
                        .Select(g2 => new 
                            { 
                                g2.PacientId,
                                g2.MeasurementsExists,
                                //g2.Measurements //!!! enable to debug
                                MeanRateGrowhPerTimePart = g2.MeasurementsExists ? g2.Measurements.Average() : 0 
                            })
                })
                //Exclude from data pacients with one or less measurements (is setted)
                .Select(i => new
                    {
                        i.DoctorId,
                        Pacients = i.Pacients.Where(n => !excludePacientsWithoutMeasurements || n.MeasurementsExists)
                        .ToArray()
                    })
                //Exclude from data doctors without pacients (is setted)
                .Where(i => !excludeDoctorsWithotPacients || i.Pacients.Any())
                .ToArray()
                ;

            //Get enum description attribute
            var perName = calcTimeType.ToString();
            var memInfo = calcTimeType.GetType().GetMember(perName).FirstOrDefault();
            if (memInfo != null)
            { 
                var descrAttr = memInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                if (descrAttr != null)
                    perName = "per " + descrAttr.Description;
            }

            //generate result from data
            foreach (var doc in doctors)
            {
                switch (calcType)
                {
                    case CalculationType.ByPacient:
                        res.AddRange(
                            doc.Pacients.Select(i => new AnalysisModule.SimpleCalculationResult()
                            {
                                Result = i.MeanRateGrowhPerTimePart,
                                AnalysisName = string.Format("{0} {3} for pacient (id:{1}) of doctor (id:{2})", CalcTaskName, i.PacientId, doc.DoctorId, perName)
                            }));
                        break;

                    case CalculationType.ByDoctor:
                        res.Add(new AnalysisModule.SimpleCalculationResult()
                        {
                            Result = doc.Pacients.Any() ? doc.Pacients.Average(p => p.MeanRateGrowhPerTimePart) : 0,
                            AnalysisName = string.Format("{0} {2} for doctor (id:{1})", CalcTaskName, doc.DoctorId, perName)
                        });
                        break;
                }
            }
            return res.ToArray();
        }
    }
}
