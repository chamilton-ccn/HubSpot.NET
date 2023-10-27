using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Utilities;
namespace HubSpot.NET.Core.Search
{
    [DataContract]
    public class SearchRequestFilterGroup
    {
        [DataMember(Name = "filters")]
        public IList<SearchRequestFilter> Filters { get; set; } = new LimitedList<SearchRequestFilter>(3);
    }
}