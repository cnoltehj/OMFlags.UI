using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OMFlags.UI.Pages;
using OMFlags.UI.Services;
using RichardSzalay.MockHttp;
using System.Net.Http;
using Xunit;

public class IndexModalIntegrationTests : TestContext
{
    [Fact]
    public void Clicking_Card_Opens_DetailModal_From_API()
    {
        var apiBase = "https://localhost:7021";
        var mock = new MockHttpMessageHandler();

        mock.When($"{apiBase}/api/Countries/GetCountries")
            .Respond("application/json", """
            [{ "id": 1, "code": "", "name": "South Africa", "flagUrl": "https://flagcdn.com/za.png" }]
            """);

        mock.When($"{apiBase}/api/Countries/GetCountryDetailsAsync?name=South%20Africa")
            .Respond("application/json", """
            {
              "name": "South Africa",
              "flagPng": null,
              "capital": "Pretoria",
              "population": 59308690,
              "area": 1221037,
              "timezones": ["UTC+02:00"],
              "languages": ["English","Zulu"],
              "currencies": [{ "code":"ZAR","name":"South African rand","symbol":"R" }],
              "googleMaps": "https://goo.gl/maps/x",
              "openStreetMaps": "https://osm.org/x"
            }
            """);

        var http = new HttpClient(mock) { BaseAddress = new Uri(apiBase) };
        Services.AddScoped<HttpClient>(_ => http);
        Services.AddScoped<BackendClient>();

        var cut = RenderComponent<OMFlags.UI.Pages.Index>();

        // click first flag card
        cut.Find("button.card").Click();

        // detail fields appear in the modal
        cut.Markup.Should().Contain("South Africa");
        cut.Markup.Should().Contain("Pretoria");
        cut.Markup.Should().Contain("ZAR");

        // close modal
        cut.Find("button.__ms_close").Click();
        cut.Markup.Should().NotContain("__ms_backdrop");
    }
}
