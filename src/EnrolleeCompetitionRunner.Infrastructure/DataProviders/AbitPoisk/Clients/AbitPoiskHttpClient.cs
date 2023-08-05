using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Responses;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net;
using System.Net.Http.Headers;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.AbitPoisk.Clients;
public sealed class AbitPoiskHttpClient
{
    private readonly HttpClient _internalClient;
    private readonly ILogger<AbitPoiskHttpClient> _logger;
    private readonly IAsyncPolicy<HttpResponseMessage> _abitPoiskRetryPolicy;

    public AbitPoiskHttpClient(HttpClient internalClient, ILogger<AbitPoiskHttpClient> logger)
    {
        _internalClient = internalClient;
        _logger = logger;
        _abitPoiskRetryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                5,
                n => TimeSpan.FromSeconds(n * 2),
                onRetry: (e, t) => _logger.LogWarning("Request to AbitPoisk {Source} failed (Status code: {StatusCode}) {Exception}", e.Source, e.Source, e))
            .AsAsyncPolicy<HttpResponseMessage>();
    }

    // Currently just for quotes.
    public async Task<RawHtmlResponse?> GetOfferInfoAsync(string offerCode, int year)
    {
        _internalClient.DefaultRequestHeaders.Accept.Clear();
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var uri = $"https://abit-poisk.org.ua/rate{year}/direction/{offerCode}";
        var result = await _abitPoiskRetryPolicy.ExecuteAsync(async () =>
        {
            var result = await _internalClient.GetAsync(uri);

            return (result.StatusCode == HttpStatusCode.NotFound ? null :  result.EnsureSuccessStatusCode())!;
        });

        if (result is null)
        {
            return null;
        }
        return new RawHtmlResponse
        {
            Html = await result.Content.ReadAsStringAsync()
        };
    }
}
