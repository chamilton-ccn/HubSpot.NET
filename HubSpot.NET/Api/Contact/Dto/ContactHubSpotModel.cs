using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable once InconsistentNaming

namespace HubSpot.NET.Api.Contact.Dto
{
    /// <summary>
    /// Models a Contact entity within HubSpot. Default properties are included here
    /// with the intention that you'd extend this class with properties specific to 
    /// your HubSpot account.
    /// </summary>
    [DataContract]
    public class ContactHubSpotModel : IHubSpotModel
    {
        /// <summary>
        /// Contacts unique ID in HubSpot
        /// </summary>
        /// <remarks>
        /// If this value is 0L (default value for long) then we don't want it serialized at all
        /// </remarks>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long Id { get; set; }
        
        [DataMember(Name = "properties")]
        private ContactPropertiesModel _properties { get; set; } = new ContactPropertiesModel();
        
        // TODO - Associations List
        // See: https://developers.hubspot.com/docs/api/crm/contacts (Create a batch of contacts)
        // Example: (it resides on the same level as "properties")
        // "associations": [
        // {
        //     "to": {
        //         "id": "company"
        //     },
        //     "types": [
        //     {
        //         "associationCategory": "USER_DEFINED",
        //         "associationTypeId": 123
        //     }
        //     ]
        
        [IgnoreDataMember]
        public string Email
        {
            get => _properties.Email;
            set => _properties.Email = value;
        }

        [IgnoreDataMember]
        public string FirstName
        {
            get => _properties.FirstName;
            set => _properties.FirstName = value;
        }
        
        [IgnoreDataMember]
        public string LastName
        {
            get => _properties.LastName;
            set => _properties.LastName = value;
        }
        
        [IgnoreDataMember]
        public string Website
        {
            get => _properties.Website;
            set => _properties.Website = value;
        }
        
        [IgnoreDataMember]
        public string EmailDomain
        {
            get => _properties.EmailDomain;
            set => _properties.EmailDomain = value;
        }        
        
        [IgnoreDataMember]
        public string Company
        {
            get => _properties.Company;
            set => _properties.Company = value;
        }
        
        [IgnoreDataMember]
        public string Phone
        {
            get => _properties.Phone;
            set => _properties.Phone = value;
        }
        
        [IgnoreDataMember]
        public string Address
        {
            get => _properties.Address;
            set => _properties.Address = value;
        }
        
        [IgnoreDataMember]
        public string City
        {
            get => _properties.City;
            set => _properties.City = value;
        }
        
        [IgnoreDataMember]
        public string State
        {
            get => _properties.State;
            set => _properties.State = value;
        }
        
        [IgnoreDataMember]
        public string ZipCode
        {
            get => _properties.ZipCode;
            set => _properties.ZipCode = value;
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
