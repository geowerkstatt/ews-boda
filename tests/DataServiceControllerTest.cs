using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetTopologySuite.Geometries;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace EWS;

[TestClass]
public class DataServiceTest
{
    private DataService dataService;

    [TestInitialize]
    public void Initialize()
    {
        var mockHttp = new MockHttpMessageHandler();

        // Gemeinde Langendorf
        mockHttp
            .When("*ch.so.agi.gemeindegrenzen.data/?bbox=2605532,1229554,2605532,1229554")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfJson);

        mockHttp
            .When("*ch.so.agi.gemeindegrenzen.data/?bbox=2605531,1229553,2605531,1229553")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfJson);

        mockHttp
            .When("*ch.so.agi.gemeindegrenzen.data/?bbox=2605591,1229592,2605591,1229592")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfJson);

        mockHttp
            .When("*ch.so.agi.gemeindegrenzen.data/?bbox=2605590,1229590,2605590,1229590")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfJson);

        mockHttp
            .When("*ch.so.agi.av.grundstuecke.rechtskraeftig.data/?bbox=2605532,1229554,2605532,1229554")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfGrundbuch1950Json);

        mockHttp
            .When("*ch.so.agi.av.grundstuecke.rechtskraeftig.data/?bbox=2605531,1229553,2605531,1229553")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfGrundbuch1950Json);

        mockHttp
            .When("*ch.so.agi.av.grundstuecke.rechtskraeftig.data/?bbox=2605591,1229592,2605591,1229592")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfGrundbuch1002Json);

        mockHttp
            .When("*ch.so.agi.av.grundstuecke.rechtskraeftig.data/?bbox=2605590,1229590,2605590,1229590")
            .Respond("application/json", DataServiceApiResponseMocks.GemeindeLangendorfGrundbuch1002Json);

        // Empty feature collection for unknown location.
        mockHttp
            .When("*ch.so.agi.gemeindegrenzen.data/?bbox=2147483647,-2147483648,2147483647,-2147483648")
            .Respond("application/json", DataServiceApiResponseMocks.EmptyFeatureCollectionJson);

        mockHttp
            .When("*ch.so.agi.av.grundstuecke.rechtskraeftig.data/?bbox=2147483647,-2147483648,2147483647,-2147483648")
            .Respond("application/json", DataServiceApiResponseMocks.EmptyFeatureCollectionJson);

        var mockHttpClient = new HttpClient(mockHttp);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(x => x.CreateClient("DataService")).Returns(mockHttpClient);
        dataService = new DataService(httpClientFactoryMock.Object, new Mock<ILogger<DataService>>().Object);
    }

    [TestMethod]
    public async Task GetAsyncForSinglePoint()
    {
        var points = new List<Point> { new(2605532, 1229554) };
        var result = await dataService.GetAsync(points);

        Assert.AreEqual("Langendorf", result.Gemeinde);
        Assert.AreEqual("1950", result.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncForMultiplePoints()
    {
        var points = new List<Point> { new(2605532, 1229554), new(2605590, 1229590) };
        var result = await dataService.GetAsync(points);

        Assert.AreEqual("Langendorf", result.Gemeinde);
        Assert.AreEqual("1950,1002", result.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncForMultiplePointsInDifferentGemeindenFails()
    {
        var points = new List<Point> { new(2605532, 1229554), new(2629600, 1246418) };
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await dataService.GetAsync(points));
    }

    [TestMethod]
    public async Task GetAsyncReturnsEmptyForUnknownLocation()
    {
        var points = new List<Point> { new(int.MaxValue, int.MinValue) };
        var result = await dataService.GetAsync(points);

        Assert.AreEqual(string.Empty, result.Gemeinde);
        Assert.AreEqual(string.Empty, result.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncReturnsEmptyForNoPoints()
    {
        var result = await dataService.GetAsync(new());

        Assert.AreEqual(string.Empty, result.Gemeinde);
        Assert.AreEqual(string.Empty, result.Grundbuchnummer);
    }

    [TestMethod]
    public async Task GetAsyncFailsOnInvalidJsonResponse()
    {
        using var mockHttp = new MockHttpMessageHandler();
        mockHttp.When("https://geo.so.ch/api/*").Respond("application/json", "{'error' : 'Unit test: Invalid JSON format'}");

        using var mockHttpClient = new HttpClient(mockHttp);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(x => x.CreateClient("DataService")).Returns(mockHttpClient);
        dataService = new DataService(httpClientFactoryMock.Object, new Mock<ILogger<DataService>>().Object);

        var points = new List<Point> { new(int.MaxValue, int.MinValue) };
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await dataService.GetAsync(points));
    }
}
