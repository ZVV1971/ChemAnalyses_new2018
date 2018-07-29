using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChemicalAnalyses.Dialogs;

namespace SATest
{
    [TestClass]
    public class SADialogsTests
    {
        [TestMethod]
        public void SADescriptionPositive()
        {
            SADescriptionDialog sad = new SADescriptionDialog() { Description = new string('a', 100) }; 
        }

        [TestMethod]
        public void SADescriptionNegative()
        {
            Action ac = () => new SADescriptionDialog() { Description = new string('a', 101) };
            Assert.ThrowsException<ArgumentException>(ac);
        }
    }
}