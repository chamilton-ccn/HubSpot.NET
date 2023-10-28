using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Utilities;
namespace HubSpot.NET.Core.Search
{
    /// <summary>
    /// Defines a group of <c>SearchFilter</c> objects, used to filter search results.
    /// </summary>
    /// <remarks>
    /// <c>SearchRequestFilterGroup</c> objects are limited to three filters maximum. All of the filters in a given
    /// group are AND'ed together. See
    /// <a href="https://developers.hubspot.com/docs/api/crm/search#filter-search-results">the documentation</a> for
    /// further details.
    /// </remarks>
    [DataContract]
    public class SearchRequestFilterGroup
    {
        [DataMember(Name = "filters")]
        public IList<SearchRequestFilter> Filters { get; set; } = new LimitedList<SearchRequestFilter>(3);
    }
}