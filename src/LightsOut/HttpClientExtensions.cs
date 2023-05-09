using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LightsOut.Exceptions;

// ReSharper disable MemberCanBePrivate.Global

namespace LightsOut
{
    internal static class HttpClientExtensions
    {
        public const int DEFAULT_RETRIES = 3;
        public const int DEFAULT_RETRY_DELAY_SECONDS = 3;

        public static Task<T> GetAsync<T>(
            this HttpClient client,
            string url
        )
        {
            return client.GetAsync<T>(url, DEFAULT_RETRIES, DEFAULT_RETRY_DELAY_SECONDS);
        }

        public static async Task<T> GetAsync<T>(
            this HttpClient client,
            string url,
            int maxRetries,
            int retryDelaySeconds
        )
        {
            var response = await client.GetAsync(url);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return await ParseResult<T>(response);
#if NET70 || NET60
                case HttpStatusCode.TooManyRequests:
                    throw new TokenQuotaExceededException();
#endif
                case HttpStatusCode.Forbidden:
                    throw new InvalidTokenException();
                case HttpStatusCode.RequestTimeout:
                    if (maxRetries < 1)
                    {
                        if ((int) response.StatusCode == 429)
                        {
                            throw new TokenQuotaExceededException();
                        }

                        throw new HttpException(response);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(retryDelaySeconds));
                    return await client.GetAsync<T>(url, --maxRetries, retryDelaySeconds);
                default:
                    throw new HttpException(response);
            }
        }

        private static async Task<T> ParseResult<T>(HttpResponseMessage response)
        {
#if NET70 || NET60
            await using var stream = await response.Content.ReadAsStreamAsync();
#else
            using var stream = await response.Content.ReadAsStreamAsync();
#endif
            return JsonSerializer.Deserialize<T>(stream, SerializerOptions);
        }

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new TimeRangeJsonConverter()
            }
        };
    }
}