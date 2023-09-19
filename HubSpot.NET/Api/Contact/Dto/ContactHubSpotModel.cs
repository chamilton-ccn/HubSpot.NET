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
        [DataMember(Name = "id", EmitDefaultValue = false)]
        //[IgnoreDataMember]
        public long Id { get; set; }
        
        /*[DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }
        [DataMember(Name = "lastname")]
        public string LastName { get; set; }
        [DataMember(Name = "website")]
        public string Website { get; set; }
        [DataMember(Name = "company")]
        public string Company { get; set; }
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        [DataMember(Name = "address")]
        public string Address { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "state")]
        public string State { get; set; }
        [DataMember(Name = "zip")]
        public string ZipCode { get; set; }*/

        [DataMember(Name = "properties")]
        private ContactPropertiesModel Properties { get; set; } = new ContactPropertiesModel();
        
        // Experimental
        
        
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
        
        // END
        
        
        [DataMember(Name="associatedcompanyid")]
        public long? AssociatedCompanyId { get;set; }

        [DataMember(Name="hubspot_owner_id")]
        public long? OwnerId { get;set; }

        //[IgnoreDataMember]
        [DataMember(Name = "createdAt", EmitDefaultValue = false)]
        public DateTime? CreatedAt { get; set; }

        //[IgnoreDataMember]
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
