using EWS.Authentication;
using EWS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
        var response = await controller.GetAsync().ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(ActionResult<IEnumerable<User>>));

        var users = response.Value;
        Assert.IsNotNull(users);
        Assert.IsTrue(users.Count() > 50);

        var userId = 80037;
        var userToTest = users.Single(b => b.Id == userId);

        Assert.AreEqual("Christoph_Schaefer71", userToTest.Name);
        Assert.AreEqual(UserRole.Administrator, userToTest.Role);
        Assert.AreEqual("Christoph_Schaefer71", userToTest.UserErstellung);
        Assert.AreEqual("Christoph Schäfer", userToTest.UserMutation);
        Assert.AreEqual(new DateTime(2021, 1, 9).Date, userToTest.Erstellungsdatum!.Value.Date);
        Assert.AreEqual(new DateTime(2021, 10, 12).Date, userToTest.Mutationsdatum!.Value.Date);
    }

    [TestMethod]
    public async Task DeleteUser()
    {
        var addedUser = context.Users.Add(new User
        {
            Name = "STRANGEBOUNCE",
            Role = UserRole.Administrator,
            UserErstellung = "WAFFLEAUTO",
            Erstellungsdatum = DateTime.Now,
        }).Entity;
        context.SaveChanges();

        Assert.IsNotNull(addedUser);
        Assert.AreNotEqual(default, addedUser.Id);
        Assert.AreEqual("STRANGEBOUNCE", addedUser.Name);
        Assert.AreEqual(UserRole.Administrator, addedUser.Role);

        var response = await controller.DeleteAsync(addedUser.Id).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(OkResult));
        Assert.IsNull(context.Users.SingleOrDefault(s => s.Id == addedUser.Id));
    }

    [TestMethod]
    public async Task DeleteUserForNotFound()
    {
        var response = await controller.DeleteAsync(1234567).ConfigureAwait(false);

        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
        Assert.AreEqual(6000, context.Standorte.Count());
    }

    [TestMethod]
    public async Task EditUser()
    {
        await CreateAndAssertUser(UserRole.Administrator, async user =>
        {
            Assert.AreEqual("STRANGEBOUNCE", user.Name);
            Assert.AreEqual(UserRole.Administrator, user.Role);

            user.Name = "LOUDCHIPMUNK";
            user.Role = UserRole.Extern;
            var response = await controller.EditAsync(user).ConfigureAwait(false);
            Assert.AreEqual(StatusCodes.Status200OK, (response as OkResult)?.StatusCode);
            var updatedUser = await context.FindAsync<User>(user.Id).ConfigureAwait(false);
            Assert.AreEqual("LOUDCHIPMUNK", updatedUser.Name);
            Assert.AreEqual(UserRole.Extern, updatedUser.Role);
        });
    }

    [TestMethod]
    public async Task EditUserForNotFound()
    {
        var inexistentUser = new User { Id = 1234567 };
        var response = await controller.EditAsync(inexistentUser).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task AddUserForNotAllowed()
    {
        var response = await controller.CreateAsync(new User { Id = 1234567 }).ConfigureAwait(false);
        Assert.IsInstanceOfType(response, typeof(BadRequestObjectResult));
        Assert.AreEqual("Creating new users is not supported.", ((BadRequestObjectResult)response).Value);
    }

    private async Task CreateAndAssertUser(UserRole role, Func<User, Task> assert)
    {
        var user = context.Users.Add(new User
        {
            Name = "STRANGEBOUNCE",
            Role = role,
        }).Entity;

        await context.SaveChangesAsync().ConfigureAwait(false);

        var untrackedUser = context.Users.AsNoTracking().Where(x => x.Id == user.Id).First();
        await assert(untrackedUser);

        // Cleanup
        var unserToDelete = await context.FindAsync<User>(untrackedUser.Id).ConfigureAwait(false);
        if (unserToDelete != null)
        {
            context.Remove(unserToDelete);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
