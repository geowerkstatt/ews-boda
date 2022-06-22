using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EWS;

[TestClass]
public class VersionControllerTest : TestBase
{
    [TestMethod]
    public void Get()
    {
        var controller = new VersionController();
        Assert.AreEqual("0.1.0", controller.Get());
    }
}
