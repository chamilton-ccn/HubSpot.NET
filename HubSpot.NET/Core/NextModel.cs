﻿using System.Runtime.Serialization;

namespace HubSpot.NET.Core
{
	[DataContract]
    public class NextModel
    {
        [DataMember(Name = "after")]
        public long After { get; set; }
        
        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}
