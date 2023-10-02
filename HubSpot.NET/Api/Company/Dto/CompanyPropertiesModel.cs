using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Company.Dto
{
    [DataContract]
    public class CompanyPropertiesModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "website")]
        public string Website { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
        
        [DataMember(Name = "about_us")]
        public string About { get; set; }
        
        [DataMember(Name = "city")]
        public string City { get; set; }
        
        [DataMember(Name = "state")]
        public string State { get; set; }
        
        [DataMember(Name = "zip")]
        public string ZipCode { get; set; }
        
        [DataMember(Name = "country")]
        public string Country { get; set; }
    }
}