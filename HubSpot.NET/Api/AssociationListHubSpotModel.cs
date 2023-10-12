using System;
using HubSpot.NET.Core.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    //TODO - marked for removal
    [Obsolete("This will be replaced via the new Associations models & API")]
    public class AssociationListHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "hasMore")]
        public bool HasMore { get; set; }

        [DataMember(Name = "offset")]
        public long? Offset { get; set; }

        [DataMember(Name = "toObjectId")]
        public long? ToObjectId { get; set; }

        [DataMember(Name = "results")]
        public List<AssociationResult> Results { get; set; }
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => throw new System.NotImplementedException();
    }
}
