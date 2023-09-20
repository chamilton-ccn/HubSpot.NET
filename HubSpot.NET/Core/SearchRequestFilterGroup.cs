using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Api;

namespace HubSpot.NET.Core
{
    [DataContract]
    public class SearchRequestFilterGroup
    {
        [DataMember(Name = "filters")]
        public IList<SearchRequestFilter> Filters { get; set; } = new List<SearchRequestFilter>();
    }
}