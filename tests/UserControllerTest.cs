using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class UserControllerTest
{
    private EwsContext context;
    private UserController controller;

    [TestInitialize]
    public void TestInitialize()
    {
        context = ContextFactory.CreateContext();
        controller = new UserController(context);
    }

    [TestCleanup]
    public void TestCleanup() => context.Dispose();

    [TestMethod]
    public async Task GetUsers()
    {
        var users = await controller.GetAsync().ConfigureAwait(false);

        Assert.IsTrue(users.Count() > 50);

        var userId = 80037;
        var userToTest = users.Single(b => b.Id == userId);

        Assert.AreEqual("Christoph_Schaefer71", userToTest.Name);
        Assert.AreEqual(UserRole.Administrator, userToTest.Role);
        Assert.AreEqual("Christoph_Schaefer71", userToTest.UserErstellung);
        Assert.AreEqual("Christoph Schäfer", userToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 1, 9).Date, userToTest.Erstellungsdatum.Date);
        Assert.AreEqual(new DateTime(2021, 10, 12).Date, userToTest.Mutationsdatum!.Value.Date);

    }
}
