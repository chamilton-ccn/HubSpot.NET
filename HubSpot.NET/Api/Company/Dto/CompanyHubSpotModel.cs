using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Company.Dto
{
    [DataContract]
    public class CompanyHubSpotModel : IHubSpotModel
    {
        /// <summary>
        /// This property can be either a `long` or a `string`. By default, the Id (`id`) property refers to the numeric
        /// HubSpot ID of a record, but it can also refer to any unique value of a given record by populating the
        /// IdProperty (`idProperty`) property of a SearchRequestOptions object with the HubSpot "system" name of a
        /// unique field that exists on the record. For example, we might want to retrieve a Company by some custom
        /// unique attribute: "my_custom_unique_property". In a batch request, we can populate the Id property below
        /// with a value from "my_custom_unique_property" (which could be a number or a string), then we can set the
        /// `IdProperty` of the SearchRequestOptions object to "my_custom_unique_property". This way HubSpot
        /// knows to use that as the unique identifier instead of the unique numeric Id. 
        /// </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public dynamic Id
        {
            get
            {
                if (_idLong != null && _idString == null)
                    return _idLong;
                return _idString;
            }
            set
            {
                if (value is long | value is int)
                {
                    _idLong = (long)value;
                }
                else
                {
                    try
                    {
                        _idLong = long.Parse(value);
                    }
                    catch (FormatException)
                    {
                        _idString = (string)value;
                    }
                }
            }
        }

        [IgnoreDataMember]
        private long? _idLong { get; set; }
        
        [IgnoreDataMember]
        private string _idString { get; set; }
        
        [DataMember(Name = "properties")]
        private CompanyPropertiesModel Properties { get; set; } = new CompanyPropertiesModel();
        public bool SerializeProperties { get; set; } = true;
        public bool ShouldSerializeProperties() => SerializeProperties;
        
        [DataMember(Name = "associations")]
        public IList<AssociationHubSpotModel> Associations { get; set; } = new List<AssociationHubSpotModel>();
        //public bool SerializeAssociations { get; set; } = true;
        //public bool ShouldSerializeAssociations() => SerializeAssociations;
        
        public bool? SerializeAssociations { get; set; }
        public bool ShouldSerializeAssociations() => SerializeAssociations ?? Associations.Any();
        
        // TODO - EXPERIMENTAL / NEEDS TESTING: We want the value of SerializeAssociations to always override
        // TODO - Associations.Any() unless it is null, then we fall back on Associations.Any(). Example:
        // TODO -    public bool? SerializeAssociations { get; set; }
        // TODO -    public bool ShouldSerializeAssociations() => SerializeAssociations ?? Associations.Any();
        // TODO - Maybe do the same with SerializeProperties/ShouldSerializeProperties ?
        
        
        [IgnoreDataMember]
        public string Name
        {
            get => Properties.Name;
            set => Properties.Name = value;
        }
        
        [IgnoreDataMember]
        public string Domain
        {
            get => Properties.Domain;
            set => Properties.Domain = value;
        }

        [IgnoreDataMember]
        public string Website
        {
            get => Properties.Website;
            set => Properties.Website = value;
        }

        [IgnoreDataMember]
        public string Description
        {
            get => Properties.Description;
            set => Properties.Description = value;
        }

        [IgnoreDataMember]
        public string About
        {
            get => Properties.About;
            set => Properties.About = value;
        }

        [IgnoreDataMember]
        public string City
        {
            get => Properties.City;
            set => Properties.City = value;
        }

        [IgnoreDataMember]
        public string State
        {
            get => Properties.State;
            set => Properties.State = value;
        }

        [IgnoreDataMember]
        public string ZipCode
        {
            get => Properties.ZipCode;
            set => Properties.ZipCode = value;
        }

        [IgnoreDataMember]
        public string Country
        {
            get => Properties.Country;
            set => Properties.Country = value;
        }
        
        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [IgnoreDataMember]
        public string HubSpotObjectType => "companies";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectType}";
    }
}
