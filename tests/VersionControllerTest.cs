using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EWS;

[TestClass]
public class VersionControllerTest
{
    [TestMethod]
    public void Get()
    {
        var controller = new VersionController();
        Assert.AreEqual("1.0", controller.Get());
    }
}
