using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationObjectIdModel
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        
        [IgnoreDataMember]
        public string HubSpotObjectType { get; set; }
    }
}