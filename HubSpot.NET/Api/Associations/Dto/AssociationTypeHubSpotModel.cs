using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
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
        /// This is the human-readable name of the association type.
        /// </summary>
        [DataMember(Name = "label")]
        public string Label { get; set; }

        /// <summary>
        /// This is the internal, unique name of the association type.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name => AssociationCategory != AssociationCategory.HubSpotDefined
            ? NonAlphanumeric.Replace($"{Label}_{FromObjectType}_{ToObjectType}".ToLower(), "_")
            : null;
        
        public bool ShouldSerializeName() => Name != null;

        private static Regex NonAlphanumeric => new Regex(@"[^a-zA-Z0-9]|\s+", RegexOptions.Compiled);

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