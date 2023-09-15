using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    [DataContract]
    public class SearchRequestFilterGroup
    {
        [DataMember(Name = "filters")]
        public IList<SearchRequestFilter> Filters { get; set; } = new List<SearchRequestFilter>();
    }
}