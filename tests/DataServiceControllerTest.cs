using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetTopologySuite.Geometries;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class DataServiceControllerTest
{
    private HttpClient httpClient;
    private DataServiceController controller;
    private EwsContext context;

    [TestInitialize]
    public void Initialize()
    {
        httpClient = new HttpClient();
        context = ContextFactory.CreateContext();
        controller = new DataServiceController(httpClient, new Mock<ILogger<DataServiceController>>().Object, context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        httpClient.Dispose();
        context.Dispose();
    }

    [TestMethod]
    public async Task GetAsyncForSinglePoint()
    {
        var points = new List<Point> { new(2605532, 1229554) };
        var result = await controller.GetAsync(points);

        Assert.AreEqual("Langendorf", result.Value.Gemeinde);
        Assert.AreEqual("1950", result.Value.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncForMultiplePoints()
    {
        var points = new List<Point> { new(2605532, 1229554), new(2605590, 1229590) };
        var result = await controller.GetAsync(points);

        Assert.AreEqual("Langendorf", result.Value.Gemeinde);
        Assert.AreEqual("1950,1002", result.Value.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncForMultiplePointsInDifferentGemeindenFails()
    {
        var points = new List<Point> { new(2605532, 1229554), new(2629600, 1246418) };
        var result = await controller.GetAsync(points);
        AssertErrorResult(result);
    }

    [TestMethod]
    public async Task GetAsyncReturnsEmptyForUnknownLocation()
    {
        var points = new List<Point> { new(int.MaxValue, int.MinValue) };
        var result = await controller.GetAsync(points);

        Assert.AreEqual(string.Empty, result.Value.Gemeinde);
        Assert.AreEqual(string.Empty, result.Value.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncReturnsEmptyForNoPoints()
    {
        var result = await controller.GetAsync(new());

        Assert.AreEqual(string.Empty, result.Value.Gemeinde);
        Assert.AreEqual(string.Empty, result.Value.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncFailsOnInvalidJsonResponse()
    {
        using var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://geo.so.ch/api/*").Respond("application/json", "{'error' : 'Unit test: Invalid JSON format'}");

        using var mockHttpClient = new HttpClient(mockHttp);
        controller = new DataServiceController(mockHttpClient, new Mock<ILogger<DataServiceController>>().Object, context);

        var points = new List<Point> { new(int.MaxValue, int.MinValue) };
        var result = await controller.GetAsync(points);
        AssertErrorResult(result);
    }

    [TestMethod]
    public async Task MigrateGemeinden()
    {
        await controller.MigrateGemeinden();
    }

    private static void AssertErrorResult(ActionResult<DataServiceResponse> result)
    {
        var objectResult = (ObjectResult)result.Result!;

        Assert.AreEqual(500, objectResult.StatusCode);
        Assert.AreEqual(null, result.Value);
    }
}
