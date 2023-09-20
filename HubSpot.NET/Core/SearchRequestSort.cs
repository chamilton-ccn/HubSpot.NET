using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    [DataContract]
    public class SearchRequestSort
    {
        [DataMember(Name = "propertyName")]
        public string SortOn { get; set; } = "createdate";

        [DataMember(Name = "direction")]
        public SearchRequestSortType Direction { get; set; } = SearchRequestSortType.Descending;
    }
}