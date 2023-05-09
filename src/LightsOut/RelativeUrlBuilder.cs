using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LightsOut
{
    internal class RelativeUrlBuilder
    {
        private string _path;

        private readonly Dictionary<string, string> _parameters
            = new(StringComparer.OrdinalIgnoreCase);

        public static RelativeUrlBuilder Create()
        {
            return new RelativeUrlBuilder();
        }

        public RelativeUrlBuilder WithPath(string path)
        {
            _path = HttpUtility.UrlEncode(path);
            return this;
        }

        public RelativeUrlBuilder WithParameter(
            string name,
            string value
        )
        {
            _parameters[name] = value;
            return this;
        }

        public string Build()
        {
            if (string.IsNullOrWhiteSpace(_path))
            {
                throw new ArgumentException("Path not set");
            }

            if (!_parameters.Any())
            {
                return _path;
            }

            return $@"{_path}?{string.Join(
                "&",
                _parameters.Select(
                    kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"
                )
            )}";
        }
    }
}