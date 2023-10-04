﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Search
{
    [DataContract]
    public class SearchRequestFilterGroup
    {
        [DataMember(Name = "filters")]
        public IList<SearchRequestFilter> Filters { get; set; } = new List<SearchRequestFilter>();
    }
}