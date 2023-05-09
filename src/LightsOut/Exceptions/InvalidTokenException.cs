using System;

namespace LightsOut.Exceptions
{
    /// <summary>
    /// The exception thrown when the api was provided an invalid token
    /// (remote api will return a 403)
    /// </summary>
    public class InvalidTokenException
        : Exception
    {
        /// <inheritdoc />
        public InvalidTokenException() 
            : base("Your ESP API token is invalid (rejected 403). Please get a new one!")
        {
        }
    }
}