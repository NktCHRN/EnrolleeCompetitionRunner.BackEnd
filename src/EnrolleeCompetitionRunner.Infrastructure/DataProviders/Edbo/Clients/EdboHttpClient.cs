using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Utilities;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Common.Responses;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Abstractions;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Requests;
using EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Responses;
using Microsoft.Extensions.Logging;
using Polly;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;

namespace EnrolleeCompetitionRunner.Infrastructure.DataProviders.Edbo.Clients;
public sealed class EdboHttpClient
{
    private readonly HttpClient _internalClient;

    private readonly ILogger<EdboHttpClient> _logger;

    private readonly IAsyncPolicy<HttpResponseMessage> _edboRetryPolicy;

    public EdboHttpClient(HttpClient internalClient, ILogger<EdboHttpClient> logger)
    {
        _internalClient = internalClient;
        _logger = logger;
        _edboRetryPolicy =  Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                7, 
                n => TimeSpan.FromSeconds(n * 2), 
                onRetry: (e, t) => _logger.LogWarning("Request to EDBO {Source} failed (Status code: {StatusCode}) {Exception}", e.Source, e.Source, e))
            .AsAsyncPolicy<HttpResponseMessage>();
    }

    public async Task<RawHtmlResponse> GetSupercompetitionInfoAsync()
    {
        _internalClient.DefaultRequestHeaders.Accept.Clear();
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

        var uri = "https://vstup.edbo.gov.ua/statistics/global-order/";
        var result = await _edboRetryPolicy.ExecuteAsync(async () =>
        {
            var result = await _internalClient.GetAsync(uri);
            return result.EnsureSuccessStatusCode();
        });
        return new RawHtmlResponse
        {
            Html = await result.Content.ReadAsStringAsync()
        };
    }

    public async Task<SearchUniversitiesResponse> SearchUniversitiesAsync(SearchUniversitiesRequest request)
    {
        _internalClient.DefaultRequestHeaders.Accept.Clear();
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var searchParameters = FormDataHelperMethods.ToFormDictionary(request);

        var uriBuilder = new UriBuilder($"https://vstup.edbo.gov.ua/offers/");
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        foreach (var param in searchParameters.Where(kvp => !string.IsNullOrEmpty(kvp.Value)))
        {
            query[param.Key] = param.Value;
        }
        uriBuilder.Query = query.ToString();
        _internalClient.DefaultRequestHeaders.Referrer = uriBuilder.Uri;

        var requestAsFormData = new FormUrlEncodedContent(searchParameters);

        var uri = "https://vstup.edbo.gov.ua/offers-universities/";
        var result = await _edboRetryPolicy.ExecuteAsync(async () =>
        {
            var result = await _internalClient.PostAsync(uri, requestAsFormData);
            return result.EnsureSuccessStatusCode();
        });

        return (await result.Content.ReadFromJsonAsync<SearchUniversitiesResponse>())!;
    }

    public async Task<GetOffersListResponse> GetOffersInfoAsync(IEnumerable<string> offerCodes, SearchUniversitiesRequest searchUniversitiesRequest)
    {
        _internalClient.DefaultRequestHeaders.Accept.Clear();
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var searchParameters = FormDataHelperMethods.ToFormDictionary(searchUniversitiesRequest);

        var uriBuilder = new UriBuilder($"https://vstup.edbo.gov.ua/offers/");
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        foreach (var param in searchParameters.Where(kvp => !string.IsNullOrEmpty(kvp.Value)))
        {
            query[param.Key] = param.Value;
        }
        uriBuilder.Query = query.ToString();
        _internalClient.DefaultRequestHeaders.Referrer = uriBuilder.Uri;

        var uri = "https://vstup.edbo.gov.ua/offers-list/";
        var request = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["ids"] = string.Join(',', offerCodes)
        });
        var result = await _edboRetryPolicy.ExecuteAsync(async () =>
        {
            var result = await _internalClient.PostAsync(uri, request);
            return result.EnsureSuccessStatusCode();
        });
        return (await result.Content.ReadFromJsonAsync<GetOffersListResponse>())!;
    }

    public async Task<GetOfferEnrolleesResponse> GetOfferEnrolleesAsync(string offerCode, int lastEnrolleePosition = 0)
    {
        _internalClient.DefaultRequestHeaders.Accept.Clear();
        _internalClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _internalClient.DefaultRequestHeaders.Referrer = new Uri($"https://vstup.edbo.gov.ua/offer/{offerCode}");

        var uri = "https://vstup.edbo.gov.ua/offer-requests/";
        var request = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["id"] = offerCode,
            ["last"] = lastEnrolleePosition.ToString()
        });
        var result = await _edboRetryPolicy.ExecuteAsync(async () =>
        {
            var result = await _internalClient.PostAsync(uri, request);
            return result.EnsureSuccessStatusCode();
        });

        return (await result.Content.ReadFromJsonAsync<GetOfferEnrolleesResponse>())!;
    }
}
