using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Company.Dto
{
    /// <summary>
    /// A word or two regarding model properties: All possible properties are not present on most models. The reason
    /// being, there are simply too many, and that's before you start counting potential custom properties. For example,
    /// at the time of this writing, there are over 300 available properties for a single Contact object. So, if you
    /// need a property that isn't available using the the predefined models, you will need to inherit from an existing
    /// model and add your own properties.
    /// </summary>
    [DataContract]
    public class CompanyPropertiesModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "domain")]
        public string Domain { get; set; }

        [DataMember(Name = "website")]
        public string Website { get; set; }
        
        [DataMember(Name = "phone")]
        public string Phone { get; set; }

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