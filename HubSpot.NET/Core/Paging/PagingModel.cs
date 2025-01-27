﻿using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Paging
{
	[DataContract]
    public class PagingModel
    {
        [DataMember(Name = "next")]
        public NextModel Next { get; set; }
        
        [DataMember(Name = "prev")]
        public PreviousModel Previous { get; set; }
    }
}
