using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LightsOut.Models;

namespace LightsOut;

/// <summary>
/// Provides an easy dotnet api wrapper for
/// EskomSePush api, typically found at
/// https://developer.sepush.co.za/business/2.0
/// Requests require an api token, which can be
/// obtained from https://eskomsepush.gumroad.com/l/api
/// - a free token can be obtained for personal use,
///   restricted (at time of writing) to 50 api requests
///   per day, which should be enough to query every 1/2 an
///   hour for a single area's projected load-shedding
/// </summary>
public interface IApi : IDisposable
{
    /// <summary>
    /// The token this instance is using to perform queries
    /// </summary>
    string Token { get; }
    
    /// <summary>
    /// Search for an area by name. Will only return the
    /// first 10 matches (server-side restriction)
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    Task<Area[]> SearchAreas(string search);
    
    /// <summary>
    /// Fetch the overall load-shedding status
    /// (national &amp; Cape Town current stage)
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<string, StatusItem>> FetchStatus();

    /// <summary>
    /// Fetch the schedule for an area by the area id obtained
    /// from a SearchAreas call
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    Task<AreaSchedule> FetchAreaSchedule(
        string areaId
    );
}

/// <inheritdoc />
public class Api : IApi
{
    /// <summary>
    /// The default base url to query against
    /// </summary>
    public const string DEFAULT_BASE_URL = "https://developer.sepush.co.za/business/2.0/";
    private HttpClient _httpClient;

    /// <inheritdoc />
    public string Token { get; }

    /// <inheritdoc />
    public Api() : this(
        Environment.GetEnvironmentVariable("ESP_API_TOKEN")
        ?? throw new InvalidOperationException(
            "Please set the ESP_API_TOKEN environment variable to  your ESP token"
        )
    )
    {
    }

    /// <inheritdoc />
    public Api(string token)
        : this(token, DEFAULT_BASE_URL)
    {
    }

    /// <summary>
    /// Creates an instance of the api with the provided token and base url
    /// </summary>
    /// <param name="token"></param>
    /// <param name="baseUrl"></param>
    /// <exception cref="ArgumentException"></exception>
    public Api(string token, string baseUrl)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token is required", nameof(token));
        }

        Token = token;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(baseUrl),
            DefaultRequestHeaders =
            {
                { "Token", token }
            }
        };
    }

    /// <inheritdoc />
    public async Task<Area[]> SearchAreas(string search)
    {
        var response = await _httpClient.GetAsync<AreasSearchResponse>(
            RelativeUrlBuilder.Create()
                .WithPath("areas_search")
#if DEBUG
                .WithParameter("test", "1")
#endif
                .WithParameter("text", search)
                .Build()
        );
        return response.Areas;
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, StatusItem>> FetchStatus()
    {
        var result = await _httpClient.GetAsync<StatusResponse>("status");
        return result.Status;
    }

    /// <inheritdoc />
    public async Task<AreaSchedule> FetchAreaSchedule(
        string areaId
    )
    {
        return await _httpClient.GetAsync<AreaSchedule>(
            RelativeUrlBuilder.Create()
                .WithPath("area")
                .WithParameter("id", areaId)
                .Build()
        );
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _httpClient?.Dispose();
        _httpClient = null;
    }
}