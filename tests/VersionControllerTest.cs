using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EWS;

[TestClass]
public class VersionControllerTest
{
    [TestMethod]
    public void Get()
    {
        var controller = new VersionController();
        StringAssert.StartsWith(controller.Get(), "1.0");
    }
}
