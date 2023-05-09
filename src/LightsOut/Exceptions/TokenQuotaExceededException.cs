using System;

namespace LightsOut.Exceptions
{
    /// <summary>
    /// The exception thrown when the quota for your api token has been exceeded
    /// (remote api returns 429)
    /// </summary>
    public class TokenQuotaExceededException
        : Exception
    {
        /// <inheritdoc />
        public TokenQuotaExceededException() : base(
            "The quota for your ESP API token has been exceeded. Either upgrade, or wait until tomorrow.")
        {
        }
    }
}