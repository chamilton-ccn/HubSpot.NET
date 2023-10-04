using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Search
{
    public enum SearchRequestSortType
    {
        [EnumMember(Value = "ASCENDING")]
        Ascending,

        [EnumMember(Value = "DESCENDING")]
        Descending
    }
}