﻿using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
	[DataContract]
    public class NextModel
    {
        [DataMember(Name = "after")]
        public string After { get; set; }
        
        [DataMember(Name = "link")]
        public string Link { get; set; }
    }
}
