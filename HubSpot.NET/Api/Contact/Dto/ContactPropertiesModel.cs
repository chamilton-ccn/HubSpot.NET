using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Contact.Dto
{
    /// <summary>
    /// A word or two regarding model properties: All possible properties are not present on most models. The reason
    /// being, there are simply too many, and that's before you start counting potential custom properties. For example,
    /// at the time of this writing, there are over 300 available properties for a single Contact object. So, if you
    /// need a property that isn't available using the the predefined models, you will need to inherit from an existing
    /// model and add your own properties.
    /// </summary>
    [DataContract]
    public class ContactPropertiesModel
    {
        [DataMember(Name = "email")]
        public string Email { get; set; }
        
        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }
        
        [DataMember(Name = "lastname")]
        public string LastName { get; set; }
        
        [DataMember(Name = "website")]
        public string Website { get; set; }
        
        [DataMember(Name = "hs_email_domain")]
        public string EmailDomain { get; set; }
        
        /// <summary>
        /// Disables serialization of the EmailDomain property. Since this is a calculated, read-only field, we need to
        /// ensure it is not sent to HubSpot in request body JSON.
        /// </summary>
        /// <see href="https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_Serialization_JsonProperty_ShouldSerialize.htm">ShouldDeserialize</see>
        /// <returns>
        /// false
        /// </returns>
        public static bool ShouldSerializeEmailDomain() => false;
        
        /// <summary>
        /// Enables deserialization of the EmailDomain field. This is necessary to ensure the EmailDomain property
        /// is available (deserialized) *IF* it appears in JSON responses.
        /// </summary>
        /// <see href="https://www.newtonsoft.com/json/help/html/P_Newtonsoft_Json_Serialization_JsonProperty_ShouldDeserialize.htm">ShouldDeserialize</see>
        /// <returns>
        /// true
        /// </returns>
        public static bool ShouldDeserializeEmailDomain() => true;
        
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
        public string ZipCode { get; set; }
    }
}