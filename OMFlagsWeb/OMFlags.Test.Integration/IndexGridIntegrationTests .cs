using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OMFlags.UI.Services;
using OMFlags.UI.Pages;     
using RichardSzalay.MockHttp;
using System.Net.Http;
using Xunit;

public class IndexGridIntegrationTests : TestContext
{
    [Fact]
    public void Index_Renders_Flag_Cards_From_API()
    {
        var apiBase = "https://localhost:7021";
        var mock = new MockHttpMessageHandler();
        mock.When($"{apiBase}/api/Countries/GetCountries")
            .Respond("application/json", """
            [
              {"id":1,"code":"","name":"Albania","flagUrl":"https://flagcdn.com/al.png"},
              {"id":2,"code":"","name":"Zimbabwe","flagUrl":"https://flagcdn.com/zw.png"}
            ]
            """);

        var http = new HttpClient(mock) { BaseAddress = new Uri(apiBase) };
        Services.AddScoped<HttpClient>(_ => http);
        Services.AddScoped<BackendClient>();

        var cut = RenderComponent<OMFlags.UI.Pages.Index>();

        cut.Markup.Should().Contain("Albania");
        cut.Markup.Should().Contain("Zimbabwe");
        cut.FindAll("button.card").Should().HaveCount(2);
        cut.FindAll("img").Should().HaveCount(2);
    }
}
