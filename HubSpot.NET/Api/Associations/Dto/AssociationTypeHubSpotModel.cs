using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationTypeHubSpotModel : IHubSpotModel
    {
        [IgnoreDataMember]
        public dynamic AssociationTypeId { get; set; }

        [DataMember(Name = "associationTypeId", EmitDefaultValue = false)]
        private dynamic _labelAssociationTypeId
        {
            get
            {
                if (AssociationCategory == AssociationCategory.UserDefined)
                    return (int?)AssociationTypeId;
                return (AssociationType)AssociationTypeId;
            }
            set
            {
                if (AssociationCategory == AssociationCategory.UserDefined)
                    AssociationTypeId = (int?)value;
                else
                    AssociationTypeId = (AssociationType)value;
            } 
        }
        
        [DataMember(Name = "typeId", EmitDefaultValue = false)]
        private dynamic _labelTypeId
        {
            get
            {
                if (AssociationCategory == AssociationCategory.UserDefined)
                    return (int?)AssociationTypeId;
                return (AssociationType)AssociationTypeId;
            }
            set
            {
                if (AssociationCategory == AssociationCategory.UserDefined)
                    AssociationTypeId = (int?)value;
                else
                    AssociationTypeId = (AssociationType)value;
            } 
        }

        /// <summary>
        /// It is possible to create an unlabeled association type that has a name. It is also possible to create a
        /// labeled association type that does not have a name. But, just because you *can* do something, doesn't mean
        /// you *should*. Custom association types are most useful when they are both named and labeled. If you find
        /// yourself needing unlabeled custom associations, simply do not populate the Name or Label properties.
        /// Specifying a value for either one, will specify the same value for both. 
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }
        
        /// <summary>
        /// If this association type is defined in the AssociationType enum, then it is a HubSpot defined type and it
        /// won't have a name so there's no point in setting a value here.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name => Enum.IsDefined(typeof(AssociationType), AssociationTypeId) 
            ? null 
            : $"{Label}_{FromObjectType}_{ToObjectType}".ToLower();

        [IgnoreDataMember]
        public AssociationCategory AssociationCategory { get; set; } = AssociationCategory.HubSpotDefined;

        [DataMember(Name = "category")]
        private AssociationCategory _category
        {
            get => AssociationCategory;
            set => AssociationCategory = value;
        }
        
        [DataMember(Name = "associationCategory")]
        private AssociationCategory _associationCategory
        {
            get => AssociationCategory;
            set => AssociationCategory = value;
        }
        
        [IgnoreDataMember]
        public string FromObjectType { get; set; }
        
        [IgnoreDataMember]
        public string ToObjectType { get; set; }
       
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}