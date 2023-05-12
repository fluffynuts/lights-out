using System;

namespace LightsOut.Cached
{
    /// <summary>
    /// Provides the Like string extension method, which
    /// searches for a comparison string within another string
    /// on a case-insensitive, ordinal basis
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Search for compare in str, case-insensitive, returning
        /// true if found
        /// </summary>
        /// <param name="str"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool Like(
            this string str,
            string compare
        )
        {
            if (str is null && compare is null)
            {
                return true;
            }

            if (str is null || compare is null)
            {
                return false;
            }

            return str.IndexOf(
                compare,
                StringComparison.OrdinalIgnoreCase
            ) > -1;
        }
    }
}