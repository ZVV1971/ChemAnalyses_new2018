using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA_EF;
using SA_EF.Interfaces;
using ChemicalAnalyses.Alumni;
using Moq;
using System.Collections.Generic;

namespace SATest
{
    [TestClass]
    public class ClSchemePrintingGridTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<ISaltAnalysisCalcResults> lst = new List<ISaltAnalysisCalcResults>()
            {
                Mock.Of<ISaltAnalysisCalcResults>()
            };
            SchemesPrintingGrid pgrd = new SchemesPrintingGrid(lst);
        }
    }
}
