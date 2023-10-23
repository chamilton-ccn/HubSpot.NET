using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Core.Search
{
    /// <summary>
    /// Options used when querying for a list matching the query term
    /// </summary>
    [DataContract]
    public class SearchRequestOptions
    {
        /// <summary>
        /// Gets or set the query term to use when searching. Limited to three groups of three filters.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#filter-search-results">Reference</a>
        /// </summary>
        [DataMember(Name = "filterGroups")]
        public IList<SearchRequestFilterGroup> FilterGroups { get; set; } = new List<SearchRequestFilterGroup>(3);
        
        
        
        /// <summary>
        /// Gets the SearchRequestSort object, which determines how to return the results. By default, we'll sort by
        /// "createdate", "descending". This member is never interacted with directly.
        /// </summary>
        [DataMember(Name = "sorts")]
        private IList<SearchRequestSort> Sort =>
            new List<SearchRequestSort>(1)
            {
                new SearchRequestSort
                {
                    SortBy = SortBy,
                    SortDirection = SortDirection
                }
            };
        
        [IgnoreDataMember]
        public string SortBy { get; set; } = "createdate";
        
        [IgnoreDataMember]
        public SearchRequestSortType SortDirection { get; set; } = SearchRequestSortType.Descending;
        
        /// <summary>
        /// This is a backing field for Limit
        /// </summary>
        [IgnoreDataMember]
        private int _limit { get; set; } = 100;
        
        /// <summary>
        /// Gets or sets the number of items to return.
        /// </summary>
        /// <remarks>
        /// Defaults to 100. We're assuming 100 is the maximum value because  
        /// <see href="https://developers.hubspot.com/docs/api/crm/contacts#limits">
        /// that is the maximum value for Contacts objects</see>. The documentation for Companies does not list a
        /// maximum value and 100 seems reasonable anyway.
        /// </remarks>
        /// <value>
        /// The number of items to return.
        /// </value>
        [DataMember(Name = "limit")]
        public int Limit
        {
            get => _limit;
            set
            {
                if (value > 100)
                    throw new ArgumentException($"{value} exceeds the maximum limit of 100 records per request");
                _limit = value;
            }
        }
        
        /// <summary>
        /// Get or set the continuation offset when calling list many times to enumerate all your items
        /// </summary>
        /// <remarks>
        /// The return DTO from List contains the current "offset" that you can inject into your next list call 
        /// to continue the listing process
        /// </remarks>
        [DataMember(Name = "after", EmitDefaultValue = false)]
        public long? Offset { get; set; }

        /// <summary>
        /// Specifies the properties that should be returned by the search.
        /// See <a href="https://developers.hubspot.com/docs/api/crm/search#crm-objects">this page</a> for a list of
        /// default values returned by each object type.
        /// </summary>
        [DataMember(Name = "properties")]
        public virtual IList<string> PropertiesToInclude { get; set; } = new List<string>(); // TODO - Why is this virtual?

        /// <summary>
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#crm-objects:~:text=Archived%20CRM%20objects%20won%E2%80%99t%20appear%20in%20any%20search%20results">
        /// Archived CRM objects won't appear in any search results.
        /// </a> However, SearchRequestOptions is a multi-purpose object used by some methods to tailor the output
        /// returned from HubSpot's API. So while the Archived property below is never used/ignored by Search requests,
        /// it is used by records retrieval requests.
        /// </summary>
        [IgnoreDataMember]
        public bool Archived { get; set; } = false;

        [IgnoreDataMember]
        public string IdProperty { get; set; }
        
        [IgnoreDataMember]
        public IList<string> PropertiesWithHistory { get; set; }
        
    }
}