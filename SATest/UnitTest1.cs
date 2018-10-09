using Microsoft.VisualStudio.TestTools.UnitTesting;
using SA_EF;

namespace SATest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void EncrytptionTest()
        {
            string path = "";
            X509EncDec xCert = new X509EncDec("KML-SERVER");
            string text = xCert.EncryptRsa(path);
        }
    }
}
