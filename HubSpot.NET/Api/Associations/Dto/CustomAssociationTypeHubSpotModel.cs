using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class CustomAssociationTypeHubSpotModel : AssociationTypeHubSpotModel
    {
        [IgnoreDataMember]
        public new long AssociationTypeId { get; set; }

        [DataMember(Name = "associationTypeId", EmitDefaultValue = false)]
        private new long TypeAssociationTypeId
        {
            get => AssociationTypeId;
            set => AssociationTypeId = value;
        }
        
        [DataMember(Name = "typeId", EmitDefaultValue = false)]
        private new long TypeId
        {
            get => AssociationTypeId;
            set => AssociationTypeId = value;
        }
        
        [IgnoreDataMember]
        public new AssociationCategory AssociationCategory { get; set; } = AssociationCategory.UserDefined;
        
    }
}