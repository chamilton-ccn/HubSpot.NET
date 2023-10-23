using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Core.Search
{
    /// <summary>
    /// Search request filter; by default, this will filter for records created within the last 7 days (inclusive).
    /// </summary>
    [DataContract]
    public class SearchRequestFilter
    {
        [DataMember(Name = "propertyName", EmitDefaultValue = false)]
        public string PropertyName { get; set; }

        [DataMember(Name = "operator", EmitDefaultValue = false)]
        public SearchRequestFilterOperatorType Operator { get; set; }

        /// <summary>
        /// A single value to search for
        /// </summary>
        /// <remarks>
        /// This has to accept strings, numbers, timestamps, etc.
        /// Note: All timestamp values returned from the API are UTC. 
        /// </remarks>
        [DataMember(Name = "value", EmitDefaultValue = false)]
        public string Value { get; set; }
        
        [DataMember(Name = "values", EmitDefaultValue = false)]
        public IList<string> Values { get; set; }

        /// <summary>
        /// HighValue is only used when the Operator is "between", otherwise it is ignored by HubSpot.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#sorting:~:text=Not%20equal%20to-,BETWEEN,-Within%20the%20specified">Reference.</a>
        /// </summary>
        [DataMember(Name = "highValue")]
        public string HighValue { get; set; }
    }
}