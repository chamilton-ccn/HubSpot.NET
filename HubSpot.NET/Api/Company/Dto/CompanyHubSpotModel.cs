using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Company.Dto
{

    /// <summary>
    /// Models a Company entity within HubSpot. Default properties are included here
    /// with the intention that you'd extend this class with properties specific to 
    /// your HubSpot account.
    /// </summary>
    [DataContract]
    public class CompanyHubSpotModel : IHubSpotModel
    {
        /// <summary>
        /// Company unique ID in HubSpot
        /// </summary>
        /// <remarks>
        /// If this value is 0L (default value for long) then we don't want it serialized at all
        /// </remarks>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        //[IgnoreDataMember] // TODO - What was the point of this again?
        public long Id { get; set; }
        
        [DataMember(Name = "properties")]
        private CompanyPropertiesModel _properties { get; set; } = new CompanyPropertiesModel();

        [IgnoreDataMember]
        public string Name
        {
            get => _properties.Name;
            set => _properties.Name = value;
        }
        
        [IgnoreDataMember]
        public string Domain
        {
            get => _properties.Domain;
            set => _properties.Domain = value;
        }

        [IgnoreDataMember]
        public string Website
        {
            get => _properties.Website;
            set => _properties.Website = value;
        }

        [IgnoreDataMember]
        public string Description
        {
            get => _properties.Description;
            set => _properties.Description = value;
        }

        [IgnoreDataMember]
        public string About
        {
            get => _properties.About;
            set => _properties.About = value;
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

        [IgnoreDataMember]
        public string Country
        {
            get => _properties.Country;
            set => _properties.Country = value;
        }
        
        [DataMember(Name = "createdAt")]
        [IgnoreDataMember]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        [IgnoreDataMember]
        public DateTime? UpdatedAt { get; set; }

        [IgnoreDataMember]
        public string HubSpotObjectTypeId => "company";
        
        [IgnoreDataMember]
        public string HubSpotObjectTypeIdPlural => "companies";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectTypeIdPlural}";
    }
}
