using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core;

namespace HubSpot.NET.Api
{
    /// <summary>
    /// Search request filter; by default, this will filter for records created within the last 7 days (inclusive).
    /// </summary>
    [DataContract]
    public class SearchRequestFilter
    {
        [DataMember(Name = "propertyName")]
        public string PropertyName { get; set; } = "createdate";

        [DataMember(Name = "operator")]
        public SearchRequestFilterOperatorType Operator { get; set; } = 
            SearchRequestFilterOperatorType.GreaterThanOrEqualTo;

        /// <summary>
        /// By default, we set Value to 7 days ago.
        /// Note: All timestamp values returned from the API are UTC.
        /// </summary>
        /// <remarks>
        /// This has to be a string because it has to accept strings, numbers, timestamps, etc. All will eventually be
        /// serialized to strings anyway, so there is no benefit to be gained by using any other data type.
        /// </remarks>
        [DataMember(Name = "value")]
        public string Value { get; set; } = ((DateTimeOffset)DateTime.Today.AddDays(-7))
            .ToUnixTimeMilliseconds().ToString();

        /// <summary>
        /// HighValue is only used when the Operator is "between", otherwise it is ignored by HubSpot.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#sorting:~:text=Not%20equal%20to-,BETWEEN,-Within%20the%20specified">Reference.</a>
        /// </summary>
        [DataMember(Name = "highValue")]
        public string HighValue { get; set; }
    }
}