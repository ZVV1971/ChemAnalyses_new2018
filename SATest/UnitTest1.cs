using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Samples;

namespace SATest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void IndexViewModelNotNull()
        {
            // Arrange
            var mock = Mock.Of<Sample>(p=>p.Description=="1");
            Sample smpl = Mock.Of<Sample>(d => d.IDSample == 1);
            
        }
    }
}
