using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationResultModel : IHubSpotModel
    {
        [DataMember(Name = "fromObjectTypeId")]
        public string FromObjectTypeId { get; set; }

        [DataMember(Name = "toObjectTypeId")]
        public string ToObjectTypeId { get; set; }

        [DataMember(Name = "fromObjectId")]
        public string FromObjectId { get; set; }

        [DataMember(Name = "toObjectId")]
        public string ToObjectId { get; set; }

        [DataMember(Name = "labels")] 
        public IList<string> Labels { get; set; } = new List<string>();

        [IgnoreDataMember]
        public string HubSpotObjectTypeId => throw new NotImplementedException();
        
        [IgnoreDataMember]
        public string HubSpotObjectTypeIdPlural => throw new NotImplementedException();
        
        [IgnoreDataMember]
        public string RouteBasePath => throw new NotImplementedException();
    }
}