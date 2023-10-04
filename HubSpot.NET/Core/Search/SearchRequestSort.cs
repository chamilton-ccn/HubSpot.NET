using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Search
{
    [DataContract]
    public class SearchRequestSort
    {
        [DataMember(Name = "propertyName")]
        public string SortBy { get; set; } = "createdate";

        [DataMember(Name = "direction")]
        public SearchRequestSortType SortDirection { get; set; } = SearchRequestSortType.Descending;
    }
}