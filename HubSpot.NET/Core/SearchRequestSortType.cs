using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    public enum SearchRequestSortType
    {
        [EnumMember(Value = "ASCENDING")]
        Ascending,

        [EnumMember(Value = "DESCENDING")]
        Descending
    }
}