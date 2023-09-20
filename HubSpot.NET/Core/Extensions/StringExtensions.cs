using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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


        /// <summary>
        /// Appends a "properties" query parameter, which is a comma-separated list of HubSpot fields, to a URL/string 
        /// </summary>
        /// <param name="url">The URL string</param>
        /// <param name="properties">An enumerable containing values that will be appended to the URL</param>
        /// <returns>A URL with a "properties" query parameter appended.</returns>
        /// <example>
        /// <code>
        /// var properties = new List&lt;string&gt; { "firstname", "lastname", "email" };
        /// "http://example.tld".SetPropertiesListQueryParams(properties)
        /// </code>
        /// The above returns: http://example.tld?properties=firstname%2clastname%2cemail
        /// </example>
        public static string SetPropertiesListQueryParams(this string url, IEnumerable<string> properties)
        {
            var newUrl = new StringBuilder(url);
            if (url.IndexOf("?", StringComparison.Ordinal) == -1)
                newUrl.Append("?");
            else if (!url.EndsWith("&"))
                newUrl.Append("&");
            newUrl.Append($"properties={WebUtility.UrlEncode(string.Join(",", properties))}");
            return newUrl.ToString();
        }

        public static string SetQueryParam(this string url, string name, string value)
        {
            var newUrl = new StringBuilder(url);
            if (url.IndexOf("?", StringComparison.Ordinal) == -1)
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
