using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Reflection;
using System.Collections.Generic;
using SA_EF;
using SA_EF.Interfaces;

namespace SATest
{
    [TestClass]
    public class SaltAnalysisDataTests
    {
        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataanalysisDateNegative()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(-1));
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.AnalysisDate == dt);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.AnalysisDate == dt);
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
                "Дата анализа не может лежать в будущем!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSamplingDatePositive()
        {
            // Arrange
            DateTime dt = DateTime.Today.Subtract(TimeSpan.FromDays(1));
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.AnalysisDate == dt);
            //Check
            Assert.AreEqual(mock.AnalysisDate, dt);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumAliquotePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.BromumAliquote == 1);
            //Check
            Assert.AreEqual(mock.BromumAliquote, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumAliquoteNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.BromumAliquote == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.BromumAliquote == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumBlankPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.BromumBlank == 1);
            //Check
            Assert.AreEqual(mock.BromumBlank, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumBlankNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.BromumBlank == -1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.BromumBlank == -1);
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
                "Значение не может быть отрицательным!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumStandardTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.BromumStandardTitre == 1);
            //Check
            Assert.AreEqual(mock.BromumStandardTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumStandardTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.BromumStandardTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.BromumStandardTitre == 0);
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
                "Значение не может быть отрицательным!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.BromumTitre == 1);
            //Check
            Assert.AreEqual(mock.BromumTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataBromumTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.BromumTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.BromumTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumAliquotePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.CalciumAliquote == 1);
            //Check
            Assert.AreEqual(mock.CalciumAliquote, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumAliquoteNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.CalciumAliquote == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.CalciumAliquote == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.CalciumTitre == 1);
            //Check
            Assert.AreEqual(mock.CalciumTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.CalciumTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.CalciumTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumTrilonTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.CalciumTrilonTitre == 1);
            //Check
            Assert.AreEqual(mock.CalciumTrilonTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCalciumTrilonTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.CalciumTrilonTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.CalciumTrilonTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCarbonatesTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.CarbonatesTitre == 1);
            //Check
            Assert.AreEqual(mock.CarbonatesTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataCarbonatesTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.CarbonatesTitre == -1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.CarbonatesTitre == -1);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataChlorumAliquotePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.ChlorumAliquote == 1);
            //Check
            Assert.AreEqual(mock.ChlorumAliquote, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataChlorumAliquoteNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.ChlorumAliquote == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.ChlorumAliquote == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataChlorumTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.ChlorumTitre == 1);
            //Check
            Assert.AreEqual(mock.ChlorumTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataChlorumTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.ChlorumTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.ChlorumTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHgCoefficientPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HgCoefficient == 1);
            //Check
            Assert.AreEqual(mock.HgCoefficient, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHgCoefficientTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HgCoefficient == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HgCoefficient == 0);
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
                "Значение параметра не может быть отрицательным либо равным 0!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleEmptyWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HumidityCrucibleEmptyWeight == 1);
            //Check
            Assert.AreEqual(mock.HumidityCrucibleEmptyWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleEmptyWeightNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 0);
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
                "Значение веса пустого тигля не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SaltAnalysisDataHumidityCrucibleWetSampleWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HumidityCrucibleEmptyWeight == 1
                && p.HumidityCrucibleWetSampleWeight == 2);
            //Check
            Assert.AreEqual(mock.HumidityCrucibleEmptyWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleWetSampleWeightNegative_Zero()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleWetSampleWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleWetSampleWeight == 0);
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
                "Значение веса тигля с сырой навеской не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SaltAnalysisDataHumidityCrucibleWetSampleWeightNegative_LessThanEmpty()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 2
                && d.HumidityCrucibleWetSampleWeight == 1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 2
                    && d.HumidityCrucibleWetSampleWeight == 1);
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
                "Значение веса тигля с сырой навеской не может быть меньшим или равным весу пустого тигля!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SaltAnalysisDataHumidityCrucibleDry110SampleWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HumidityCrucibleEmptyWeight == 1
                && p.HumidityCrucibleWetSampleWeight == 3
                && p.HumidityCrucibleDry110SampleWeight == 2);
            //Check
            Assert.AreEqual(mock.HumidityCrucibleDry110SampleWeight, 2);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry110SampleWeightNegative_Zero()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleDry110SampleWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleDry110SampleWeight == 0);
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
                "Значение веса тигля с сухой (110) навеской не может быть равным нулю или отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry110SampleWeightNegative_EqualToEmpty()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                && d.HumidityCrucibleDry110SampleWeight == 1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                    && d.HumidityCrucibleDry110SampleWeight == 1);
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
                "Значение веса тигля с сухой (110) навеской не может быть меньшим или равным весу пустого тигля!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry110SampleWeightNegative_GTWet()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                && d.HumidityCrucibleWetSampleWeight == 2
                && d.HumidityCrucibleDry110SampleWeight == 3);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                    && d.HumidityCrucibleWetSampleWeight == 2
                    && d.HumidityCrucibleDry110SampleWeight == 3);
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
                "Значение веса тигля с сухой (110) навеской не может быть большим веса тигля с сырой навеской!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry180SampleWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HumidityCrucibleEmptyWeight == 1
                && p.HumidityCrucibleWetSampleWeight == 4
                && p.HumidityCrucibleDry110SampleWeight == 3
                && p.HumidityCrucibleDry180SampleWeight == 2);
            //Check
            Assert.AreEqual(mock.HumidityCrucibleDry180SampleWeight, 2);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry180SampleWeightNegative_LessThanZero()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleDry180SampleWeight == -1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleDry180SampleWeight == -1);
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
                "Значение веса тигля с сухой (180) навеской не может быть равным нулю или отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry180SampleWeightNegative_LessThanEmpty()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 2
                && d.HumidityCrucibleDry180SampleWeight == 1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 2
                    && d.HumidityCrucibleDry180SampleWeight == 1);
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
                "Значение веса тигля с сухой (180) навеской не может быть меньшим или равным весу пустого тигля!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry180SampleWeightNegative_GTWet()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                && d.HumidityCrucibleWetSampleWeight == 2
                && d.HumidityCrucibleDry180SampleWeight == 3);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                    && d.HumidityCrucibleWetSampleWeight == 2
                    && d.HumidityCrucibleDry180SampleWeight == 3);
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
                "Значение веса тигля с сухой (180) навеской не может быть большим веса тигля с сырой навеской!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHumidityCrucibleDry180SampleWeightNegative_GT110()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                && d.HumidityCrucibleWetSampleWeight == 4
                && d.HumidityCrucibleDry110SampleWeight == 2
                && d.HumidityCrucibleDry180SampleWeight == 3);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HumidityCrucibleEmptyWeight == 1
                && d.HumidityCrucibleWetSampleWeight == 4
                && d.HumidityCrucibleDry110SampleWeight == 2
                && d.HumidityCrucibleDry180SampleWeight == 3);
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
                "Значение веса тигля с сухой (180) навеской не может быть большим веса тигля с навеской при 110!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHydrocarbonatesTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.HydrocarbonatesTitre == 1);
            //Check
            Assert.AreEqual(mock.HydrocarbonatesTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataHydrocarbonatesTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.HydrocarbonatesTitre == -1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.HydrocarbonatesTitre == -1);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumDiapasonPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.KaliumDiapason == 1);
            //Check
            Assert.AreEqual(mock.KaliumDiapason, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumDiapasonNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.KaliumDiapason == 3);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.KaliumDiapason == 3);
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
                "Значение диапазона 1 или 2"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumValuePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.KaliumValue == 1);
            //Check
            Assert.AreEqual(mock.KaliumValue, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumValueNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.KaliumValue == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.KaliumValue == 0);
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
                "Значение показаний не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumVolumPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.KaliumVolume == 1);
            //Check
            Assert.AreEqual(mock.KaliumVolume, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataKaliumVolumeNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.KaliumVolume == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.KaliumVolume == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgAliquotePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.MagnesiumAliquote == 1);
            //Check
            Assert.AreEqual(mock.MagnesiumAliquote, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgAliquoteNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.MagnesiumAliquote == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.MagnesiumAliquote == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.MagnesiumTitre == 1);
            //Check
            Assert.AreEqual(mock.MagnesiumTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.MagnesiumTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.MagnesiumTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgTrilonTitrePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.MagnesiumTrilonTitre == 1);
            //Check
            Assert.AreEqual(mock.MagnesiumTrilonTitre, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataMgTrilonTitreNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.MagnesiumTrilonTitre == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.MagnesiumTrilonTitre == 0);
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
                "Значение титра не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataResiduumCrucibleEmptyWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.ResiduumCrucibleEmptyWeight == 1);
            //Check
            Assert.AreEqual(mock.ResiduumCrucibleEmptyWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataResiduumCrucibleEmptyWeightNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleEmptyWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleEmptyWeight == 0);
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
                "Значение веса пустого бюкса не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataResiduumCrucibleFullWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.ResiduumCrucibleEmptyWeight == 1
                && p.ResiduumCrucibleFullWeight == 2);
            //Check
            Assert.AreEqual(mock.ResiduumCrucibleEmptyWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataResiduumCrucibleFullWeightNegative_Zero()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleFullWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleFullWeight == 0);
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
                "Значение веса тигля с осадком не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataResiduumCrucibleFullWeightNegative_LETEmpty()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleEmptyWeight == 1
                && d.ResiduumCrucibleFullWeight == 1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.ResiduumCrucibleEmptyWeight == 1
                    && d.ResiduumCrucibleFullWeight == 1);
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
                "Значение веса тигля с осадком не может быть меньше или равно весу пустого тигля!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesAliquotePositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.SulfatesAliquote == 1);
            //Check
            Assert.AreEqual(mock.SulfatesAliquote, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesAliquoteNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.SulfatesAliquote == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.SulfatesAliquote == 0);
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
                "Значение аликвоты не может быть отрицательным числом"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesBlankPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.SulfatesBlank == 1);
            //Check
            Assert.AreEqual(mock.SulfatesBlank, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesBlankNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.SulfatesBlank == -1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.SulfatesBlank == -1);
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
                "Значение не может быть отрицательным!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesCrucibleEmptyWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.SulfatesCrucibleEmptyWeight == 1);
            //Check
            Assert.AreEqual(mock.SulfatesCrucibleEmptyWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesCrucibleEmptyWeightNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.SulfatesCrucibleEmptyWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.SulfatesCrucibleEmptyWeight == 0);
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
                "Значение веса пустого тигля не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesCrucibleFullWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.SulfatesCrucibleEmptyWeight == 1
                                                                && p.SulfatesCrucibleFullWeight == 2);
            //Check
            Assert.AreEqual(mock.SulfatesCrucibleFullWeight, 2);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataSulfatesCrucibleFullWeightNegative_Zero()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.SulfatesCrucibleFullWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(d => d.SulfatesCrucibleFullWeight == 0);
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
                "Значение веса тигля с осадком не может быть отрицательным числом!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SaltAnalysisDataSulfatesCrucibleFullWeightNegative_LETEmpty()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(p => p.SulfatesCrucibleEmptyWeight == 1
                                                          && p.SulfatesCrucibleFullWeight == 1);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData lc = Mock.Of<SaltAnalysisData>(p => p.SulfatesCrucibleEmptyWeight == 1
                                                                  && p.SulfatesCrucibleFullWeight == 1);
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
                "Значение веса тигля с осадком не может быть меньшим или равным весу пустого тигля!"));
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataWetWeightPositive()
        {
            // Arrange
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.WetWeight == 1);
            //Check
            Assert.AreEqual(mock.WetWeight, 1);
        }

        [TestMethod, Owner("ZVV 60325-2")]
        public void SimpleSaltAnalysisDataWetEmptyWeightNegative()
        {
            // Arrange
            Action ac = () => Mock.Of<SaltAnalysisData>(d => d.WetWeight == 0);
            //Check if an exception is thrown
            Assert.ThrowsException<TargetInvocationException>(ac);

            Exception except = null;
            try
            {
                SaltAnalysisData sa = Mock.Of<SaltAnalysisData>(d => d.WetWeight == 0);
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
                "Значение сырой навески не может быть отрицательным числом!"));
        }

        [TestMethod]
        public void SaltAnalysisDataCalcValues_CalcScheme_Chloride()
        {
            SaltAnalysisData mock = Mock.Of<SaltAnalysisData>(p => p.WetWeight == 4.454M
            && p.MagnesiumTitre == 2.25M
            && p.MagnesiumTrilonTitre == 1.084M
            && p.CalciumTitre == 1.62M
            && p.CalciumTrilonTitre == 1.084M
            && p.HumidityCrucibleEmptyWeight == 12.5431M
            && p.HumidityCrucibleWetSampleWeight == 15.676M
            && p.HumidityCrucibleDry110SampleWeight == 15.6609M
            && p.HumidityCrucibleDry180SampleWeight == 15.6557M
            && p.ChlorumTitre == 6.65M
            && p.HgCoefficient == 1
            && p.BromumTitre == 11.27M
            && p.BromumBlank == 9.6M
            && p.ResiduumCrucibleEmptyWeight == 48.6832M
            && p.ResiduumCrucibleFullWeight == 48.863M
            && p.SulfatesCrucibleEmptyWeight == 14.97M
            && p.SulfatesCrucibleFullWeight == 14.9859M
            && p.SulfatesBlank == 0.0008M
            && p.RecommendedCalculationScheme == SaltCalculationSchemes.Chloride);
            //Calculate values
            mock.CalcDryValues();
            //Calculate recommended scheme
            Assert.AreEqual(SaltCalculationSchemes.Chloride, mock.CalcRecommendedScheme());
            //Set tolerance for rounding in 0.5%
            decimal threshold = 0.005M;
            //Check if calculated values fall into allowable range
            Assert.IsTrue(Math.Abs(mock.SampleCorrectedDryWeight / 4.4434M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.MgDry / 0.00093M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.CaDry / 0.00396M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.HumidityContent / 0.0065M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.ClDry / 0.53M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.BrDry / 0.0005M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.ResiduumDry / 0.0404M - 1) < threshold);
            Assert.IsTrue(Math.Abs(mock.SulfatesDry / 0.00699M - 1) < threshold);
        }

        [TestMethod]
        public void SaltAnalysisDataCalcK_Chloride()
        {
            //LinearCalibration lc = new LinearCalibration(new List<Tuple<int, decimal, decimal>> {
            //    { new Tuple<int, decimal, decimal>(0,0.0005M,7.5M)},
            //    { new Tuple<int, decimal, decimal>(0,0.001M,12.5M)},
            //    { new Tuple<int, decimal, decimal>(0,0.0015M,18)},
            //    { new Tuple<int, decimal, decimal>(0,0.002M,23)},
            //    { new Tuple<int, decimal, decimal>(0,0.0025M,29)},
            //    { new Tuple<int, decimal, decimal>(0,0.005M,53)},
            //    { new Tuple<int, decimal, decimal>(1,0.005M,6.5M)},
            //    { new Tuple<int, decimal, decimal>(1,0.01M,12.5M)},
            //    { new Tuple<int, decimal, decimal>(1,0.015M,18)},
            //    { new Tuple<int, decimal, decimal>(1,0.02M,23.5M)},
            //    { new Tuple<int, decimal, decimal>(1,0.025M,28.5M)},
            //    { new Tuple<int, decimal, decimal>(1,0.03M,33.5M)},
            //    { new Tuple<int, decimal, decimal>(1,0.035M,38)},
            //    { new Tuple<int, decimal, decimal>(1,0.04M,44)},
            //    { new Tuple<int, decimal, decimal>(1,0.05M,51)}});
            //lc.CalibrationID = 1;
            var lc = new Mock<ILinearCalibration>();
            lc.Setup(p => p.Intercept).Returns(new decimal[2] { 2.71311M, 2.78387M });
            lc.Setup(p => p.Slope).Returns(new decimal[2] { 10137.7M, 157.377M });
            lc.Setup(p => p.CalibrationID).Returns(1);
            lc.Setup(p => p.ValueToConcentration(It.IsAny<decimal>(), It.IsAny<int>())).Returns(0.0152M);

            SaltAnalysisData sa = new SaltAnalysisData(new Dictionary<int, ILinearCalibration>() {
                { lc.Object.CalibrationID, lc.Object } });
            sa.WetWeight = 4.454M;
            sa.MagnesiumTitre = 2.25M;
            sa.MagnesiumTrilonTitre = 1.084M;
            sa.CalciumTitre = 1.62M;
            sa.CalciumTrilonTitre = 1.084M;
            sa.HumidityCrucibleEmptyWeight = 12.5431M;
            sa.HumidityCrucibleWetSampleWeight = 15.676M;
            sa.HumidityCrucibleDry110SampleWeight = 15.6609M;
            sa.HumidityCrucibleDry180SampleWeight = 15.6557M;
            sa.KaliumVolume = 100;
            sa.RecommendedCalculationScheme = SaltCalculationSchemes.Chloride;
            sa.CalcDryValues();
            sa.KaliumCalibration = lc.Object.CalibrationID;
            sa.KDry = sa.CalcKaliumValue();
            Assert.AreEqual(0.17M, Math.Round(sa.KDry, 2));
        }
    }
}