using System;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

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
        public CompanyHubSpotModel()
        {
            Associations = new CompanyHubSpotAssociations();
        }

        /// <summary>
        /// Company unique ID in HubSpot
        /// </summary>
        /// <remarks>
        /// If this value is 0L (default value for long) then we don't want it serialized at all
        /// </remarks>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [IgnoreDataMember]
        public long Id { get; set; }
        
        [DataMember(Name = "properties")]
        private CompanyPropertiesModel Properties { get; set; } = new CompanyPropertiesModel();

        public string Name
        {
            get => Properties.Name;
            set => Properties.Name = value;
        }

        public string Domain
        {
            get => Properties.Domain;
            set => Properties.Domain = value;
        }

        public string Website
        {
            get => Properties.Website;
            set => Properties.Website = value;
        }

        public string Description
        {
            get => Properties.Description;
            set => Properties.Description = value;
        }

        public string About
        {
            get => Properties.About;
            set => Properties.About = value;
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

        public string Country
        {
            get => Properties.Country;
            set => Properties.Country = value;
        }
        
        [DataMember(Name = "createdAt")]
        [IgnoreDataMember]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        [IgnoreDataMember]
        public DateTime? UpdatedAt { get; set; }

        public string RouteBasePath => "/crm/v3/objects/companies";
        public bool IsNameValue => true;

        [IgnoreDataMember]
        public CompanyHubSpotAssociations Associations { get; }
        
        
        // TODO - not sure if this is going to be necessary anymore
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
