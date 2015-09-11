using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AnalysisModuleTaskX;
using System.Linq;

// git repository: https://github.com/kblc/Test
namespace AnalysisModuleTaskXTests
{
    [TestClass]
    public class TestCalculator
    {
        public Calculator Calc { get; set; }

        [TestInitialize]
        public void Initialization()
        {
            Calc = new Calculator();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Calculator_Calculate_ErrorOnNullArgument()
        {
            Calc.Calculate(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Calculator_Calculate_ErrorOnEmptyListOfValues()
        {
            Calc.Calculate(new InternalData());
        }

        [TestMethod]
        public void Calculator_Calculate_OneDoctorWithoutPacients()
        {
            var data = new InternalData();
            var doc = new InternalDataDoctor() { DoctorId = 1 };

            data.Doctors.Add(doc);
            var res0 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(1, res0.Count(), "res0 must contains 1 result");
            Assert.AreEqual(0d, res0.First().Result);

            var res1 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: true, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(0, res1.Count(), "res1 must contains no result");

            data.Doctors.Add(new InternalDataDoctor() { DoctorId = 2 });
            var res2 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(2, res2.Count(), "res2 must contains 2 result");

            var res3 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: true, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(0, res3.Count(), "res3 must contains 0 result");
        }

        [TestMethod]
        public void Calculator_Calculate_OneDoctorWithPacients()
        {
            var data = new InternalData();
            var doc = new InternalDataDoctor() { DoctorId = 1 };
            var pac0 = new InternalDataPacient() { DoctorId = doc.DoctorId, PacientId = 1 };
            var pac1 = new InternalDataPacient() { DoctorId = doc.DoctorId, PacientId = 2 };

            data.Doctors.Add(doc);
            data.Pacients.Add(pac0);
            data.Pacients.Add(pac1);

            var res0 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(1, res0.Count(), "res0 must contains 1 result");
            Assert.AreEqual(0d, res0.First().Result, "result for res0 must equals 0");

            var res1 = Calc.Calculate(data, CalculationType.ByPacient, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(2, res1.Count(), "res1 must contains 2 result");
            Assert.AreEqual(0d, res1.Distinct().First().Result, "result for res1 must equals 0");

            var res2 = Calc.Calculate(data, CalculationType.ByPacient, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: true);
            Assert.AreEqual(0, res2.Count(), "res2 must contains 0 result");

            var res3 = Calc.Calculate(data, CalculationType.ByPacient, excludeDoctorsWithoutPacients: true, excludePacientsWithoutMeasurements: true);
            Assert.AreEqual(0, res3.Count(), "res3 must contains 2 result");

            data.Doctors.Add(new InternalDataDoctor() { DoctorId = 2 });
            var res4 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(2, res4.Count(), "res4 must contains 2 result");

            var res5 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: true, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(1, res5.Count(), "res5 must contains 1 result");
        }

        [TestMethod]
        public void Calculator_Calculate_OneDoctorWithTwoPacientsAndMeasurementsForOnePacient()
        {
            var data = new InternalData();
            var doc = new InternalDataDoctor() { DoctorId = 1 };
            var pac0 = new InternalDataPacient() { DoctorId = doc.DoctorId, PacientId = 1 };
            var pac1 = new InternalDataPacient() { DoctorId = doc.DoctorId, PacientId = 2 };
            var me00 = new InternalDataMeasurement() { PacientId = pac0.PacientId, MeasurementId = 1 };
            var me01 = new InternalDataMeasurement() { PacientId = pac0.PacientId, MeasurementId = 2 };

            data.Doctors.Add(doc);
            data.Pacients.Add(pac0);
            data.Measurements.Add(me00);
            data.Measurements.Add(me01);

            var res0 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: true);
            //pacient 0 with bad measurement (result should equals 0)
            Assert.AreEqual(1, res0.Count(), "res0 must contains 1 result");
            Assert.AreEqual(0d, res0.First().Result, "result for res0 must equals 0");

            var hc00 = new InternalDataHeighComponent() { MeasurementId = me00.MeasurementId, Height = 100 };
            var hc01 = new InternalDataHeighComponent() { MeasurementId = me01.MeasurementId, Height = 200 };
            var dt = DateTime.Now;
            var ts00 = new InternalDataTimestamp() { MeasurementId = me00.MeasurementId, Timestamp = dt };
            var ts01 = new InternalDataTimestamp() { MeasurementId = me01.MeasurementId, Timestamp = dt.AddDays(7) };

            data.HeighComponent.Add(hc00);
            data.HeighComponent.Add(hc01);
            data.Timestamps.Add(ts00);
            data.Timestamps.Add(ts01);

            var res1 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: true);
            //pacient 0 with bad measurement (result should equals 0)
            Assert.AreEqual(1, res1.Count(), "res1 must contains 1 result");
            Assert.AreEqual(100d, res1.First().Result, "result for res1 must equals 100");

            //add one pacient without measurement
            data.Pacients.Add(pac1);
            var res2 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: true);
            Assert.AreEqual(1, res2.Count(), "res2 must contains 1 result");
            Assert.AreEqual(100d, res2.First().Result, "result for res2 must equals 100");

            //if we calc 2 pacients (one without measurements) then result must be (100 - 0) / 2 = 50
            var res3 = Calc.Calculate(data, CalculationType.ByDoctor, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(1, res3.Count(), "res3 must contains 1 result");
            Assert.AreEqual(50d, res3.First().Result, "result for res3 must equals 50");
        }

        [TestMethod]
        public void Calculator_Calculate_OneDoctorWithOnePacientsAndMeasurements()
        {
            var data = new InternalData();
            var doc = new InternalDataDoctor() { DoctorId = 1 };
            var pac0 = new InternalDataPacient() { DoctorId = doc.DoctorId, PacientId = 1 };

            data.Doctors.Add(doc);
            data.Pacients.Add(pac0);

            double start_height = 100;
            double height = start_height;
            var dt = DateTime.Now;
            for (int i=0; i<100; i++)
            {
                var me = new InternalDataMeasurement() { PacientId = pac0.PacientId, MeasurementId = i };
                var hc = new InternalDataHeighComponent() { MeasurementId = me.MeasurementId, Height = height };
                var ts = new InternalDataTimestamp() { MeasurementId = me.MeasurementId, Timestamp = dt };

                data.Measurements.Add(me);
                data.HeighComponent.Add(hc);
                data.Timestamps.Add(ts);

                height += start_height;
                dt = dt.AddDays(7);
            }

            var res0 = Calc.Calculate(data, CalculationType.ByDoctor, CalculationTimeType.PerWeek, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(start_height, res0.First().Result, string.Format("result for res0 must equals {0} (per week)", start_height));

            var res1 = Calc.Calculate(data, CalculationType.ByDoctor, CalculationTimeType.PerDay, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(start_height / 7d, res1.First().Result.Value, 0.01, string.Format("result for res1 must equals {0}/7 (per day)", start_height));

            var res2 = Calc.Calculate(data, CalculationType.ByDoctor, CalculationTimeType.PerYear, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(start_height / 7d * 365.25, res2.First().Result.Value, 0.01, string.Format("result for res2 must equals {0}/7*365.25 (per year)", start_height));

            var res3 = Calc.Calculate(data, CalculationType.ByDoctor, CalculationTimeType.PerMonth, excludeDoctorsWithoutPacients: false, excludePacientsWithoutMeasurements: false);
            Assert.AreEqual(start_height / 7d * 365.25 / 12d, res3.First().Result.Value, 0.01, string.Format("result for res3 must equals {0}/7*365.25/12 (per month)", start_height));
        }
    }
}
