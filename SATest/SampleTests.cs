using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SettingsHelper;
using Moq;
using Samples;
using System.Reflection;

namespace SATest
{
    [TestClass]
    public class SampleTests
    {
        //use explicit public constructor to moq Static connection string returning from SettingsHelper
        public SampleTests()
        {
            ConnectionStringGiver.GetValidConnectionString = (string s) => { return @"Data Source=(localdb)\mssqllocaldb;AttachDbFilename=e:\Downloads\svpp\KSR\ChemicalAnalyses\ChemicalAnalyses.mdf;Initial Catalog=ChemicalAnalyses;Integrated Security=True"; };
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleLabNumberPositive()
        {
            //Get ready
            Sample smpl = Mock.Of<Sample>(p => p.LabNumber == "123");
            //Check
            Assert.AreEqual(smpl.LabNumber, "123");
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleLabNumberTooLongNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<Sample>(d => d.LabNumber == new String('1', 16));
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                Sample smpl = Mock.Of<Sample>(d => d.LabNumber == new String('1', 16));
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
                "Неверный формат номера!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleLabNumberTooShortNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<Sample>(d => d.LabNumber == new String('1', 1));
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                Sample smpl = Mock.Of<Sample>(d => d.LabNumber == new String('1', 1));
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
                "Неверный формат номера!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleDescriptionPositive()
        {
            //Get ready
            Sample smpl = Mock.Of<Sample>(p => p.Description== "123");
            //Check
            Assert.AreEqual(smpl.Description, "123");
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleDescriptionTooLongNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<Sample>(d => d.Description == new String('1', 201));
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                Sample smpl = Mock.Of<Sample>(d => d.Description == new String('1', 201));
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
                "Слишком длинная строка!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSampleSamplingDateNegative()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(-1));
            Action ac = () => Mock.Of<Sample>(d => d.SamplingDate == dt);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                Sample lc = Mock.Of<Sample>(d => d.SamplingDate == dt);
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
                "Дата отбора не может лежать в будущем!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSampleSamplingDatePositive()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(1));
            Sample mock = Mock.Of<Sample>(p => p.SamplingDate == dt);
            //Check
            Assert.AreEqual(mock.SamplingDate, dt);
        }
    }
}