using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

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
        private ContactPropertiesModel Properties { get; set; } = new ContactPropertiesModel();
        
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
        
        public string Email
        {
            get => Properties.Email;
            set => Properties.Email = value;
        }

        public string FirstName
        {
            get => Properties.FirstName;
            set => Properties.FirstName = value;
        }

        public string LastName
        {
            get => Properties.LastName;
            set => Properties.LastName = value;
        }
        
        public string Website
        {
            get => Properties.Website;
            set => Properties.Website = value;
        }
        
        public string EmailDomain
        {
            get => Properties.EmailDomain;
            set => Properties.EmailDomain = value;
        }        
        
        public string Company
        {
            get => Properties.Company;
            set => Properties.Company = value;
        }
        
        public string Phone
        {
            get => Properties.Phone;
            set => Properties.Phone = value;
        }
        
        public string Address
        {
            get => Properties.Address;
            set => Properties.Address = value;
        }
        
        
        public string City
        {
            get => Properties.City;
            set => Properties.City = value;
        }
        
        public string State
        {
            get => Properties.State;
            set => Properties.State = value;
        }
        
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

        public string RouteBasePath => "/crm/v3/objects/contacts";
        
        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
