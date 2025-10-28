using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Xunit;

public class ModalUnitTests : TestContext
{
    [Fact]
    public void Modal_Renders_Title_And_ChildContent_And_Closes()
    {
        var cut = RenderComponent<OMFlags.UI.Pages.Modal>(ps => ps
            .Add(p => p.Show, true)
            .Add(p => p.Title, "Country Details")
            .AddChildContent("<p>Body here</p>")
        );

        cut.Markup.Should().Contain("Country Details");
        cut.Markup.Should().Contain("Body here");

        // close by clicking ×
        cut.Find("button.__ms_close").Click();
        cut.Markup.Should().NotContain("__ms_backdrop");
    }
}
