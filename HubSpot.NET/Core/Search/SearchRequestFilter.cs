using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Core.Search
{
    /// <summary>
    /// Defines a filter to be used when searching for records. See
    /// <a href="https://developers.hubspot.com/docs/api/crm/search#filter-search-results">the
    /// documentation</a> for further details.
    /// </summary>
    [DataContract]
    public class SearchRequestFilter
    {
        /// <summary>
        /// The name of the property that will be searched, for the value or values contained within <c>Value</c> or
        /// <c>Values</c>.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#search-default-searchable-properties"> See the
        /// documentation</a> for a list of default searchable properties.</summary>
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
        
        /// <summary>
        /// A list of values to search for.
        /// </summary>
        /// <remarks>
        /// Each item in the list must be a string.
        /// </remarks>
        /// TODO - A unit test has not been written for this.
        [DataMember(Name = "values", EmitDefaultValue = false)]
        public IList<string> Values { get; set; }

        /// <summary>
        /// HighValue is only used when the Operator is <c>BETWEEN</c>, otherwise it is ignored by HubSpot.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#sorting:~:text=Not%20equal%20to-,BETWEEN,-Within%20the%20specified">Reference.</a>
        /// </summary>
        /// TODO - A unit test has not been written for this.
        [DataMember(Name = "highValue")]
        public string HighValue { get; set; }
    }
}