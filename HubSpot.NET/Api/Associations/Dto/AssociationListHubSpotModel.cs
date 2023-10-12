using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationListHubSpotModel<T> : IHubSpotModel where T : AssociationHubSpotModel
    {
        [DataMember(Name = "inputs")]
        public IList<T> Associations { get; set; } = new List<T>();
            
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v4"; 
        
    }
}