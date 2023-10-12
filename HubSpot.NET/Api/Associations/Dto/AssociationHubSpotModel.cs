using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "types", EmitDefaultValue = false)]
        public IList<AssociationTypeHubSpotModel> AssociationTypes = new List<AssociationTypeHubSpotModel>();
        
        [DataMember(Name = "from", EmitDefaultValue = false)]
        public AssociationObjectIdModel FromObject { get; set; } = new AssociationObjectIdModel();
        
        [DataMember(Name = "to", EmitDefaultValue = false)]
        public AssociationObjectIdModel ToObject { get; set; } = new AssociationObjectIdModel();

        [IgnoreDataMember]
        public AssociationResultModel Result { get; set; } = new AssociationResultModel();
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}