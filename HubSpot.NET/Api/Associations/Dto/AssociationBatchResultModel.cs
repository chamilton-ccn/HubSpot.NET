using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationBatchResultModel : IHubSpotModel
    {
        [DataMember(Name = "completedAt")]
        public DateTime CompletedAt { get; set; }
        
        [DataMember(Name = "requestedAt")]
        public DateTime RequestedAt { get; set; }
        
        [DataMember(Name = "startedAt")]
        public DateTime StartedAt { get; set; }
        
        [DataMember(Name = "links")]
        public object Links { get; set; }

        [DataMember(Name = "results")]
        public IList<AssociationResultModel> Results { get; set; }
        
        [DataMember(Name = "status")]
        public string Status { get; set; }
        
        // TODO - needed?
        [DataMember(Name = "errors")]
        public IList<ErrorsListItem> Errors { get; set; }
        
        // TODO - needed?
        [DataMember(Name = "numErrors")]
        public int TotalErrors { get; set; }

        [IgnoreDataMember]
        public string HubSpotObjectType => throw new NotImplementedException();
        
        [IgnoreDataMember]
        public string RouteBasePath => throw new NotImplementedException();
    }
}