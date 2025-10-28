using System.Net.Http;
using System.Text.Json;
using FluentAssertions;
using OMFlags.UI.Models;
using OMFlags.UI.Services;
using RichardSzalay.MockHttp;
using Xunit;

public class BackendClientUnitTests
{
    [Fact]
    public async Task GetCountriesAsync_Returns_List()
    {
        var apiBase = "https://localhost:7021";
        var mock = new MockHttpMessageHandler();
        mock.When($"{apiBase}/api/Countries/GetCountries")
            .Respond("application/json", """
            [
              {"id":1,"code":"","name":"Albania","flagUrl":"https://flagcdn.com/al.png"}
            ]
            """);

        var http = new HttpClient(mock) { BaseAddress = new Uri(apiBase) };
        var svc = new BackendClient(http);

        var list = await svc.GetCountriesAsync();

        list.Should().NotBeNull();
        list!.Should().ContainSingle(x => x.Name == "Albania" && x.FlagUrl.EndsWith("/al.png"));
    }

    [Fact]
    public async Task GetCountryDetailsAsync_UrlEncodes_Name()
    {
        var apiBase = "https://localhost:7021";
        var mock = new MockHttpMessageHandler();
        mock.When($"{apiBase}/api/Countries/GetCountryDetailsAsync?name=South%20Africa")
            .Respond("application/json", """{ "name": "South Africa", "population": 1, "area": 2, "timezones": [], "languages": [], "currencies": [] }""");

        var http = new HttpClient(mock) { BaseAddress = new Uri(apiBase) };
        var svc = new BackendClient(http);

        var dto = await svc.GetCountryDetailsAsync("South Africa");
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("South Africa");
    }
}