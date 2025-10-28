using Microsoft.Extensions.DependencyInjection;
using OMFlags.Application.Abstractions;
using OMFlags.Infrastructure.Adapters;
using System.Net;

namespace OMFlags.Infrastructure
{
        public static class DependencyInjection
        {
            public static IServiceCollection AddCountries(this IServiceCollection services)
            {
                services.AddHttpClient<ICountryApi, RestCountriesApiAdapter>(c =>
                {
                    c.BaseAddress = new Uri("https://restcountries.com");
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                    c.Timeout = TimeSpan.FromSeconds(5); // client-side timeout
                })
                .AddHttpMessageHandler(() => new ResilienceHandler(maxAttempts: 3, baseDelay: TimeSpan.FromMilliseconds(200)));

                return services;
            }
        }

        /// <summary>
        /// Lightweight retry with exponential backoff + jitter for transient HTTP errors.
        /// No external packages required.
        /// </summary>
        internal sealed class ResilienceHandler : DelegatingHandler
        {
            private readonly int _maxAttempts;
            private readonly TimeSpan _baseDelay;
            private readonly Random _jitter = new();

            public ResilienceHandler(int maxAttempts = 3, TimeSpan? baseDelay = null)
            {
                _maxAttempts = Math.Max(1, maxAttempts);
                _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(200);
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            {
                for (var attempt = 1; ; attempt++)
                {
                    HttpResponseMessage? response = null;
                    Exception? lastEx = null;

                    try
                    {
                        response = await base.SendAsync(request, ct).ConfigureAwait(false);
                        if (!IsTransient(response.StatusCode))
                            return response;
                    }
                    catch (HttpRequestException ex) { lastEx = ex; }
                    catch (TaskCanceledException ex) when (!ct.IsCancellationRequested) { lastEx = ex; } // timeout

                    if (attempt >= _maxAttempts)
                    {
                        if (response is not null) return response;
                        throw lastEx ?? new HttpRequestException("Request failed after retries.");
                    }

                    var delay = TimeSpan.FromMilliseconds(_baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1))
                               + TimeSpan.FromMilliseconds(_jitter.Next(0, 100));
                    await Task.Delay(delay, ct).ConfigureAwait(false);
                }
            }

            private static bool IsTransient(HttpStatusCode code) =>
                code == HttpStatusCode.TooManyRequests ||
                code == HttpStatusCode.RequestTimeout ||
                code == HttpStatusCode.BadGateway ||
                code == HttpStatusCode.ServiceUnavailable ||
                code == HttpStatusCode.GatewayTimeout ||
                (int)code >= 500;
        }
    }

