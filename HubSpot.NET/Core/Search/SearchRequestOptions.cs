﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Utilities;

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
        public IList<SearchRequestFilterGroup> FilterGroups = new LimitedList<SearchRequestFilterGroup>(3);

        /// <summary>
        /// Gets the SearchRequestSort object, which determines how to return the results. By default, we'll sort by
        /// "createdate", "descending". This member is never interacted with directly.
        /// </summary>
        [DataMember(Name = "sorts")]
        private IList<SearchRequestSort> Sort =>
            new LimitedList<SearchRequestSort>(1)
            {
                new SearchRequestSort
                {
                    SortBy = SortBy,
                    SortDirection = SortDirection
                }
            };

        // TODO - [SortBy] XML Documentation
        [IgnoreDataMember]
        public string SortBy { get; set; } = "createdate";

        // TODO [SortDirection] XML Documentation
        [IgnoreDataMember]
        public SearchRequestSortType SortDirection { get; set; } = SearchRequestSortType.Descending;

        /// <summary>
        /// This is a backing field for Limit
        /// </summary>
        [IgnoreDataMember]
        private int _limit { get; set; }

        /// <summary>
        /// Gets or sets the number of items to return.
        /// </summary>
        /// <remarks>
        /// Defaults to 100; 50 if requesting property history.
        /// </remarks>
        /// <value>
        /// The number of items to return for any single request.
        /// </value>
        [DataMember(Name = "limit")]
        public int Limit
        {
            get => (_limit == 0 & PropertiesWithHistory.Any())
                ? 50
                : _limit == 0
                    ? 100
                    : _limit;
            set
            {
                if ((value > 100) | (value > 50 & PropertiesWithHistory.Any()))
                {
                    throw new ArgumentException($"{value} exceeds the maximum limit of 100 records (or 50 if " +
                                                $"including property history) per request");
                }

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
        /// See <a href="https://developers.hubspot.com/docs/api/crm/search#crm-objects">this</a> for a list of default
        /// values returned by each object type.
        /// </summary>
        // TODO - XML Documentation: Explain why ShouldSerializePropertiesToInclude is necessary. 
        [DataMember(Name = "properties")]
        public IList<string> PropertiesToInclude { get; set; } = new List<string>();
        public bool ShouldSerializePropertiesToInclude() => PropertiesToInclude.Any();

        /// <summary>
        /// <a href="https://tinyurl.com/2pzss22h">
        /// Archived CRM objects won't appear in any search results.
        /// </a> However, SearchRequestOptions is a multi-purpose object used by some methods to tailor the output
        /// returned from HubSpot's API. So while the Archived property below is never used/ignored by Search requests,
        /// it is used by records retrieval requests.
        /// </summary>
        [IgnoreDataMember]
        public bool Archived { get; set; }

        // TODO - [IdProperty] XML Documentation
        [IgnoreDataMember]
        public string IdProperty { get; set; }

        // TODO - [PropertiesWithHistory] XML Documentation
        [IgnoreDataMember]
        public IList<string> PropertiesWithHistory { get; set; } = new List<string>();
        public bool ShouldSerializePropertiesWithHistory() => PropertiesWithHistory.Any();
    }
}