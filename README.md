# OMFlags.Web (Blazor WebAssembly UI)

Blazor WebAssembly frontend for ""OMFlags"".  
Shows all country flags in a responsive grid; clicking a flag opens a ""popup modal"" with full country details pulled from ""OMFlags.API"".

This project follows OOP/DRY and a Clean‑Architecture-aligned separation: the UI never calls third‑party APIs directly — it only talks to ""OMFlags.API"".

---

## Table of Contents
- [Features](#features)
- [Requirements](#requirements)
- [Getting Started](#getting-started)
  - [Configure API Base URL](#configure-api-base-url)
  - [Run the UI](#run-the-ui)
- [How It Works](#how-it-works)
  - [Data Flow](#data-flow)
  - [Controls](#controls)
  - [Popup Modal](#popup-modal)
- [Project Structure](#project-structure)
- [Testing](#testing)
- [Logging](#logging)
- [Troubleshooting](#troubleshooting)
- [Appendix: Sample Code](#appendix-sample-code)

---

## Features
- ""Flags Grid"": pulls "/api/Countries/GetCountries" from OMFlags.API and renders a responsive card grid (image + name).
- ""Details Modal"": clicking a card opens a modal and loads "/api/Countries/GetCountryDetailsAsync?name=...".
- ""Strict Backend-Only"": UI does not talk to "restcountries.com" directly (keeps CORS/security consistent).
- ""Typed Models"": "CountryModel" (list) and "CountryDetailModel" (modal) mirror API JSON.
- ""Composable Modal"": reusable component with overlay, backdrop click, and Esc-to-close.

---

## Requirements
- .NET 8 SDK
- OMFlags.API running locally:
  - HTTP:  "http://localhost:7020"
  - HTTPS: "https://localhost:7021"

> For best results, run API over ""HTTPS"" and configure CORS to allow the UI origin (typically "https://localhost:7245").

---

## Getting Started

### Configure API Base URL

"wwwroot/appsettings.json":
"""json
{
  "ApiBaseUrl": "https://localhost:7021"
}
"""

"Program.cs":
"""csharp
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7021";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBase) });

builder.Services.AddScoped<OMFlags.Web.Services.BackendClient>();
"""

> If you change ports or scheme, update both ""ApiBaseUrl"" and API CORS settings.

### Run the UI
"""bash
cd OMFlags.Web
dotnet restore
dotnet run
# Usually serves at https://localhost:7245
"""

Open the browser at "https://localhost:7245".

---

## How It Works

### Data Flow
1. "Pages/Index.razor" uses "BackendClient" to call ""OMFlags.API"".
2. The API responds with a list of countries ("id, code, name, flagUrl").
3. The grid renders each country as a ""card"" with an "<img>" bound to "flagUrl".
4. Clicking a card opens a ""modal"", which then loads full details by name.
5. The modal displays capital, population, area, timezones, languages, currencies, and map links.

### Controls
- ""Grid"": responsive CSS grid with card hover effect.
- ""Card"": "<button>" semantics for accessibility; no page navigation.
- ""Modal"": overlay with backdrop, "position: fixed", high "z-index", and keyboard handling (Esc).

### Popup Modal
Modal is a reusable component ("Shared/Modal.razor") with inline fallback CSS so it always overlays even if isolation CSS isn’t loaded.

Usage in "Index.razor":
"""razor
<Modal Show="@_showModal" Title="@_modalTitle" OnClose="CloseModal">
    @if (_detailLoading) { <p>Loading…</p> }
    else if (!string.IsNullOrEmpty(_detailError)) { <p style="color:#b91c1c;">@_detailError</p> }
    else if (_detail is not null)
    {
        <!-- Render CountryDetailModel fields -->
    }
</Modal>
"""

---

## Project Structure

"""
OMFlags.Web/
  wwwroot/
    appsettings.json
  Models/
    CountryModel.cs          // { id, code, name, flagUrl }
    CountryDetailModel.cs    // { name, flagPng, capital, population, area, timezones, languages, currencies[], googleMaps, openStreetMaps }
  Services/
    BackendClient.cs         // Calls OMFlags.API using HttpClient
  Shared/
    Modal.razor              // Reusable popup component (overlay, Esc, backdrop close)
  Pages/
    Index.razor              // Flags grid + modal
  Program.cs                 // DI & HttpClient base address
"""

---

## Testing

### Unit tests (bUnit recommended)
- Test that "Index.razor":
  - Renders "n" cards for "n" countries.
  - Opens modal on card click.
  - Shows ""loading"" and ""error"" states for details.

### Integration/E2E (Playwright or Selenium)
- Click a flag → assert modal shows country name, capital, and population.
- Close the modal via ""×"", backdrop, and ""Esc"".

---

## Logging

- ""Browser Console"": "Console.WriteLine()" appears in DevTools → Console when running in WASM.
- ""ILogger"" (recommended):
  """csharp
  @inject ILogger<Index> Logger

  protected override async Task OnInitializedAsync()
  {
      try { /" load data "/ }
      catch (Exception ex) { Logger.LogError(ex, "Failed to load countries"); }
  }
  """

---

## Troubleshooting

- ""No flags, only names""  
  - UI must bind to "flagUrl" (grid), not "flagPng".  
  - Ensure API returns ""HTTPS"" image URLs or proxy via API to avoid mixed content.

- ""CORS blocked""  
  - API must allow the UI origin (e.g., "https://localhost:7245").  
  - In API, call "UseCors" ""before"" "MapControllers".

- ""“Modal shows at bottom”""  
  - Use "Shared/Modal.razor" (has inline styles with "position: fixed; z-index").  
  - Ensure details are rendered ""only inside"" the "<Modal>" component.  
  - Use "<Modal ...>" (not "<Shared.Modal ...>").

- ""Details don’t load""  
  - Check Network tab: "GET /api/Countries/GetCountryDetailsAsync?name=..." must return 200 with JSON.  
  - Verify "BackendClient" uses the configured "ApiBaseUrl" and the name is URL-encoded.

---

## Appendix: Sample Code

### "Models/CountryModel.cs"
"""csharp
using System.Text.Json.Serialization;

namespace OMFlags.Web.Models
{
    public sealed class CountryModel
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("flagUrl")] public string FlagUrl { get; set; } = string.Empty;
    }
}
"""

### "Models/CountryDetailModel.cs"
"""csharp
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace OMFlags.Web.Models
{
    public sealed class CurrencyModel
    {
        [JsonPropertyName("code")] public string? Code { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("symbol")] public string? Symbol { get; set; }
    }

    public sealed class CountryDetailModel
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
        [JsonPropertyName("flagPng")] public string? FlagPng { get; set; }
        [JsonPropertyName("capital")] public string? Capital { get; set; }
        [JsonPropertyName("population")] public long Population { get; set; }
        [JsonPropertyName("area")] public long Area { get; set; }
        [JsonPropertyName("timezones")] public List<string> Timezones { get; set; } = new();
        [JsonPropertyName("languages")] public List<string> Languages { get; set; } = new();
        [JsonPropertyName("currencies")] public List<CurrencyModel> Currencies { get; set; } = new();
        [JsonPropertyName("googleMaps")] public string? GoogleMaps { get; set; }
        [JsonPropertyName("openStreetMaps")] public string? OpenStreetMaps { get; set; }
    }
}
"""

### "Services/BackendClient.cs"
"""csharp
using System.Net.Http.Json;
using System.Text.Json;
using OMFlags.Web.Models;

namespace OMFlags.Web.Services
{
    public sealed class BackendClient
    {
        private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };
        private readonly HttpClient _http;
        public BackendClient(HttpClient http) => _http = http;

        public Task<List<CountryModel>?> GetCountriesAsync(CancellationToken ct = default)
            => _http.GetFromJsonAsync<List<CountryModel>>("/api/Countries/GetCountries", JsonOpts, ct);

        public Task<CountryDetailModel?> GetCountryDetailsAsync(string name, CancellationToken ct = default)
            => _http.GetFromJsonAsync<CountryDetailModel>(
                $"/api/Countries/GetCountryDetailsAsync?name={Uri.EscapeDataString(name)}", JsonOpts, ct);
    }
}
"""

### "Shared/Modal.razor" (self-contained)
"""razor
@using Microsoft.AspNetCore.Components.Web

<style>
  .__ms_backdrop { position: fixed; inset: 0; background: rgba(0,0,0,.45); z-index: 10000; }
  .__ms_card { position: fixed; top:50%; left:50%; transform: translate(-50%, -50%);
               width: min(620px, 92vw); max-height: 80vh; background: #fff; border-radius: 12px;
               box-shadow: 0 20px 50px rgba(0,0,0,.25); z-index: 10001; outline: none;
               display: flex; flex-direction: column; }
  .__ms_header { display:flex; align-items:center; justify-content:space-between;
                 padding:.9rem 1rem; border-bottom:1px solid #eee; }
  .__ms_body { padding: 1rem; overflow: auto; }
  .__ms_close { background:transparent; border:none; font-size:1.5rem; line-height:1;
                cursor:pointer; padding:.25rem .5rem; }
</style>

@if (Show)
{
    <div class="__ms_backdrop" @onclick="HandleBackdropClick"></div>

    <div class="__ms_card" role="dialog" aria-modal="true" aria-labelledby="modal-title" tabindex="0" @onkeydown="HandleKeyDown">
        <div class="__ms_header">
            <h4 id="modal-title">@Title</h4>
            <button class="__ms_close" type="button" @onclick="Close" aria-label="Close">×</button>
        </div>
        <div class="__ms_body">
            @ChildContent
        </div>
    </div>
}

@code {
    [Parameter] public bool Show { get; set; }
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public bool CloseOnBackdrop { get; set; } = true;

    private Task Close() => OnClose.InvokeAsync();
    private Task HandleBackdropClick(MouseEventArgs _) => CloseOnBackdrop ? Close() : Task.CompletedTask;
    private async Task HandleKeyDown(KeyboardEventArgs e) { if (e.Key is "Escape" or "Esc") await Close(); }
}
"""

---

Happy building! 🎉
