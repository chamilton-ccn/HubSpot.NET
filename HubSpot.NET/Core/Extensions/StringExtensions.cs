using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace HubSpot.NET.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        // TODO - this is nowhere near complete. We need a generalized version for V3 query parameters. Format:
        // TODO - ?properties=property1,property2,property3
        public static string SetQueryParams(this string url, string name, List<string> parameters)
        {
            var urlParams = new StringBuilder();
            var newUrl = new StringBuilder(url);
            newUrl.Append("&");
            newUrl.Append($"{name}=");
            urlParams.Append(WebUtility.UrlEncode(string.Join(",", parameters)));
            newUrl.Append(urlParams);
            return newUrl.ToString();
        }

        public static string SetQueryParam(this string url, string name, string value)
        {
            var newUrl = new StringBuilder(url);
            if (url.IndexOf("?") == -1)
                newUrl.Append("?");
            else if (!url.EndsWith("&"))
                newUrl.Append("&");

            newUrl.Append($"{name}={WebUtility.UrlEncode(value)}");

            return newUrl.ToString();
        }

        public static string SetQueryParam(this string url, string name, IEnumerable<string> values)
        {
            string _url = url;

            // return SetQueryParam(_url, name, string.Join(",", values));

            foreach (string value in values)
                _url = SetQueryParam(_url, name, value);
            return _url;
        }

        public static string SetQueryParam(this string url, string name, bool value)
        {
            return SetQueryParam(url, name, value.ToString().ToLowerInvariant());
        }

        public static string SetQueryParam(this string url, string name, short value)
        {
            return SetQueryParam(url, name, value.ToString());
        }

        public static string SetQueryParam(this string url, string name, int value)
        {
            return SetQueryParam(url, name, value.ToString());
        }

        public static string SetQueryParam(this string url, string name, long value)
        {
            return SetQueryParam(url, name, value.ToString());
        }

        public static string SetQueryParam(this string url, string name, short? value)
        {
            return SetQueryParam(url, name, value.Value);
        }

        public static string SetQueryParam(this string url, string name, int? value)
        {
            return SetQueryParam(url, name, value.Value);
        }

        public static string SetQueryParam(this string url, string name, long? value)
        {
            return SetQueryParam(url, name, value.Value);
        }
    }
}
