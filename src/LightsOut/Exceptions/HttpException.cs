using System;
using System.Net.Http;

namespace LightsOut.Exceptions
{
    /// <summary>
    /// The generic exception thrown for http errors
    /// </summary>
    public class HttpException : Exception
    {
        /// <summary>
        /// The received response
        /// </summary>
        public HttpResponseMessage Response { get; }

        /// <inheritdoc />
        public HttpException(
            HttpResponseMessage response
        ) : base($"Error: {response.StatusCode} {response.ReasonPhrase}")
        {
            Response = response;
        }
    }
}