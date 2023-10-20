using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Search
{
    /// <summary>
    /// Options used when querying for a list matching the query term
    /// </summary>
    [DataContract]
    public class SearchRequestOptions
    {
        /// <summary>
        /// Gets or set the query term to use when searching. Limited to three groups.
        /// <a href="https://developers.hubspot.com/docs/api/crm/search#filter-search-results">Reference</a>
        /// </summary>
        [DataMember(Name = "filterGroups")]
        public IList<SearchRequestFilterGroup> FilterGroups { get; set; } = new List<SearchRequestFilterGroup>();

        /// <summary>
        /// If limit isn't specified, it to the <see href="https://developers.hubspot.com/docs/api/crm/contacts#limits">
        /// maximum allowable number</see>
        /// </summary>        
        //private int _limit = 100;
        //private readonly int _upperLimit;
        
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
        public virtual int Limit { get; set; }
        /*{
            get => _limit;
            set
            {
                if (value < 1 || value > _upperLimit)
                {
                    throw new ArgumentException($"Number of items to return must be a positive integer " +
                                                $"greater than 0, and less than {_upperLimit} (you provided {value}).");
                }
                _limit = value;
            }
        }*/

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HubSpot.NET.Core.Search.SearchRequestOptions"/> class.
        /// </summary>
        /// <param name="upperLimit">Upper limit for the amount of items to request for the list.</param>
        /*public SearchRequestOptions(int upperLimit)
        {
            _upperLimit = upperLimit;
        }*/

        /// <summary>
        /// Sets the upper limit to 100, which is the
        /// <see href="https://developers.hubspot.com/docs/api/crm/contacts#limits">maximum per page</see>. 
        /// </summary>
        //public SearchRequestOptions() : this(100) { }

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

        // TODO - Test whether this is a viable alternative (for consistency) for methods like GetByProperty, GetByEmail, etc.
        [IgnoreDataMember]
        public string IdProperty { get; set; }
        
        // TODO - Test whether this is a viable alternative (for consistency) for methods like GetByProperty, GetByEmail, etc.
        [IgnoreDataMember]
        public IList<string> PropertiesWithHistory { get; set; }
        
    }
}