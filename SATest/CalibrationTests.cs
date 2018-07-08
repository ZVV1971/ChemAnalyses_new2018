using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Calibration;
using SettingsHelper;
using System;
using System.Reflection;
using System.Collections.ObjectModel;

namespace SATest
{
    [TestClass, TestCategory("Calibration")]
    public class CalibrationTests
    {
        //use implicit public constructor to moq Static connection string returning from SettingsHelper
        public CalibrationTests()
        {
            ConnectionStringGiver.GetValidConnectionString = (string s) => { return @"Data Source=(localdb)\mssqllocaldb;AttachDbFilename=E:\IIT\Projects\СВПП\KSR\ChemicalAnalyses\ChemicalAnalyses.mdf;Initial Catalog=ChemicalAnalyses;Integrated Security=True"; };
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleCalibrationDescriptionPositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => p.Description == "desc");
            //Check
            Assert.AreEqual(mock.Description, "desc");
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleCalibrationDescriptionNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<LinearCalibration>(d => d.Description == "");
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                LinearCalibration lc = Mock.Of<LinearCalibration>(d => d.Description == "");
            }
            catch (Exception ex)
            {
                except = ex;
            }
            //Check the real type of the inner exception
            Assert.IsInstanceOfType(except.InnerException, typeof(ArgumentNullException));
            // and its text
            Assert.IsTrue(String.Equals(except.InnerException.Message.Substring(0,
                except.InnerException.Message.IndexOf("\r\n")),
                "Введите описание!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleCalibrationCalibrationDatePositive()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(1));
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => p.CalibrationDate == dt);
            //Check
            Assert.AreEqual(mock.CalibrationDate, dt);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleCalibrationCalibrationDateNegative()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(-1));
            Action ac = () => Mock.Of<LinearCalibration>(d => d.CalibrationDate == dt);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                LinearCalibration lc = Mock.Of<LinearCalibration>(d => d.CalibrationDate == dt);
            }
            catch (Exception ex)
            {
                except = ex;
            }
            //Check the real type of the inner exception
            Assert.IsInstanceOfType(except.InnerException, typeof(ArgumentOutOfRangeException));
            // and its text
            Assert.IsTrue(String.Equals(except.InnerException.Message.Substring(0,
                except.InnerException.Message.IndexOf("\r\n")),
                "Дата калибровки не может лежать в будущем!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ContainsEqualDataPointsDiapasonCheckPositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>();
            //Check
            Assert.IsFalse(mock.ContainsEqualDataPoints(0));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ContainsEqualDataPointsDiapasonCheckNegative()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>();
            //Check
            Assert.IsTrue(mock.ContainsEqualDataPoints(3));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ContainsEqualDataPointsIsFalsePositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => (p.LinearCalibrationData) == 
            new ObservableCollection<DataPoint>[2] {
                new ObservableCollection<DataPoint> {
                    new DataPoint(1,1),
                    new DataPoint(2,2)},
                new ObservableCollection<DataPoint> {}});
            //Check
            Assert.IsFalse(mock.ContainsEqualDataPoints(0));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ContainsEqualDataPointsIsTruePositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => (p.LinearCalibrationData) ==
            new ObservableCollection<DataPoint>[2] {
                new ObservableCollection<DataPoint> {
                    new DataPoint(2,2),
                    new DataPoint(2,2)},
                new ObservableCollection<DataPoint> {}});
            //Check
            Assert.IsTrue(mock.ContainsEqualDataPoints(0));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void GetLinearCoefficientsPositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => (p.LinearCalibrationData) ==
            new ObservableCollection<DataPoint>[2] {
                new ObservableCollection<DataPoint> {
                    new DataPoint(2,2),
                    new DataPoint(3,3)},
                new ObservableCollection<DataPoint> {
                    new DataPoint(1,2),
                    new DataPoint(2,4)} });
            mock.GetLinearCoefficients();
            //Check
            Assert.IsTrue(mock.Intercept[0] == 0);
            Assert.IsTrue(mock.Intercept[1] == 0);
            Assert.IsTrue(mock.Slope[0] == 1);
            Assert.IsTrue(mock.Slope[1] == 2);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void GetLinearCoefficientsNegative()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => (p.LinearCalibrationData) ==
            new ObservableCollection<DataPoint>[2] {
                new ObservableCollection<DataPoint> {
                    new DataPoint(2,2),
                    new DataPoint(2,2)},
                new ObservableCollection<DataPoint> {
                    new DataPoint(1,2),
                    new DataPoint(2,2)} });
            mock.GetLinearCoefficients();
            //Check
            Assert.IsTrue(mock.Intercept[0] == decimal.MaxValue);
            Assert.IsTrue(mock.Slope[0] == decimal.MaxValue);
            Assert.IsTrue(mock.RSquared[0] == 0);
            Assert.IsTrue(mock.RSquared[1] == 0);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ValueToConcentrationDiapasonNegative()
        {
            // Arrange
            LinearCalibration lc = Mock.Of<LinearCalibration>();
           
            Exception except = null;
            try
            {
                lc.ValueToConcentration(1, 2);
            }
            catch (Exception ex)
            {
                except = ex;
            }
            //Check the real type of the inner exception
            Assert.IsInstanceOfType(except, typeof(ArgumentOutOfRangeException));
            // and its text
            Assert.IsTrue(String.Equals(except.Message.Substring(0,
                except.Message.IndexOf("\r\n")),
                "Недопустимый номер диапазаона или неверное значение показателя"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void ValueToConcentrationPositive()
        {
            // Arrange
            LinearCalibration mock = Mock.Of<LinearCalibration>(p => (p.LinearCalibrationData) ==
            new ObservableCollection<DataPoint>[2] {
                new ObservableCollection<DataPoint> {
                    new DataPoint(2,2),
                    new DataPoint(3,3)},
                new ObservableCollection<DataPoint> {
                    new DataPoint(1,2),
                    new DataPoint(2,4)} });
            mock.GetLinearCoefficients();
            Assert.AreEqual(mock.ValueToConcentration(2, 0), 2);
            Assert.AreEqual(mock.ValueToConcentration(2, 1), 1);
        }
    }
}