using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationObjectIdModel
    {
        [IgnoreDataMember]
        public long Id { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        private long _id
        {
            get => Id;
            set => Id = value;
        }
        
        [DataMember(Name = "toObjectId", EmitDefaultValue = false)]
        private long _toObjectId
        {
            get => Id;
            set => Id = value;
        }

        public bool ShouldSerialize_toObjectId() => false;

        [IgnoreDataMember]
        public string HubSpotObjectType { get; set; }
    }
}