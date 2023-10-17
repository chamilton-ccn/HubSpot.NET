using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationHubSpotModel : IHubSpotModel
    {
        [IgnoreDataMember]
        public IList<AssociationTypeHubSpotModel> AssociationTypes = new List<AssociationTypeHubSpotModel>();

        [DataMember(Name = "types", EmitDefaultValue = false)]
        private IList<AssociationTypeHubSpotModel> _types
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        
        [DataMember(Name = "associationTypes", EmitDefaultValue = false)]
        private IList<AssociationTypeHubSpotModel> _associationTypes
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        
        public bool ShouldSerializeAssociationTypes() => false; // TODO - not sure if we need to keep this
        
        [DataMember(Name = "from", EmitDefaultValue = false)]
        public AssociationObjectIdModel FromObject { get; set; } = new AssociationObjectIdModel();
        
        [IgnoreDataMember]
        public AssociationObjectIdModel ToObject { get; set; } = new AssociationObjectIdModel();
        
        [DataMember(Name = "to", EmitDefaultValue = false)]
        private AssociationObjectIdModel _to
        {
            get => ToObject;
            set => ToObject = value;
        }

        [DataMember(Name = "toObjectId", EmitDefaultValue = false)]
        private long _toObject
        {
            get => ToObject.Id;
            set => ToObject.Id = value;
        }
        
        [IgnoreDataMember]
        public AssociationResultModel Result { get; set; } = new AssociationResultModel();
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}