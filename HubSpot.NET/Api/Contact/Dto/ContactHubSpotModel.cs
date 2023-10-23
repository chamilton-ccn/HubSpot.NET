using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Core.Interfaces;
using Microsoft.CSharp.RuntimeBinder;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Contact.Dto
{
    [DataContract]
    public class ContactHubSpotModel : IHubSpotModel
    {
        /// <summary>
        /// This property can be either a `long` or a `string`. By default, the Id (`id`) property refers to the numeric
        /// HubSpot ID of a record, but it can also refer to any unique value of a given record by populating the
        /// IdProperty (`idProperty`) property of a SearchRequestOptions object with the HubSpot "system" name of a
        /// unique field that exists on the record. For example, we might want to retrieve a Contact by some custom
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
                if (_idLong != 0L && _idString == null)
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
        public long _idLong { get; set; }
        
        [IgnoreDataMember]
        public string _idString { get; set; }
        
        [DataMember(Name = "properties")]
        private ContactPropertiesModel Properties { get; set; } = new ContactPropertiesModel();
        
        [DataMember(Name = "associations")]
        public IList<AssociationHubSpotModel> Associations { get; set; } = new List<AssociationHubSpotModel>();
        public bool SerializeAssociations { get; set; } = true;
        public bool ShouldSerializeAssociations() => SerializeAssociations;
        
        [IgnoreDataMember]
        public string Email
        {
            get => Properties.Email;
            set => Properties.Email = value;
        }

        [IgnoreDataMember]
        public string FirstName
        {
            get => Properties.FirstName;
            set => Properties.FirstName = value;
        }
        
        [IgnoreDataMember]
        public string LastName
        {
            get => Properties.LastName;
            set => Properties.LastName = value;
        }
        
        [IgnoreDataMember]
        public string Website
        {
            get => Properties.Website;
            set => Properties.Website = value;
        }
        
        [IgnoreDataMember]
        public string EmailDomain
        {
            get => Properties.EmailDomain;
            set => Properties.EmailDomain = value;
        }        
        
        [IgnoreDataMember]
        public string Company
        {
            get => Properties.Company;
            set => Properties.Company = value;
        }
        
        [IgnoreDataMember]
        public string Phone
        {
            get => Properties.Phone;
            set => Properties.Phone = value;
        }
        
        [IgnoreDataMember]
        public string Address
        {
            get => Properties.Address;
            set => Properties.Address = value;
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
        
        // TODO - does this "fit" anymore?
        [DataMember(Name="associatedcompanyid")]
        public long? AssociatedCompanyId { get;set; }

        // TODO - does this "fit" anymore?
        [DataMember(Name="hubspot_owner_id")]
        public long? OwnerId { get;set; }

        [DataMember(Name = "createdAt", EmitDefaultValue = false)]
        public DateTime? CreatedAt { get; set; }
        
        [DataMember(Name = "updatedAt", EmitDefaultValue = false)]
        public DateTime? UpdatedAt { get; set; }

        [IgnoreDataMember]
        public string HubSpotObjectType => "contacts";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectType}";
    }
}
