using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable ReturnValueOfPureMethodIsNotUsed

namespace HubSpot.NET.Api.Contact.Dto
{
    /// <summary>
    /// A word or two regarding model properties: All possible properties are not present on most models. The reason
    /// being, there are simply too many, and that's before you start counting potential custom properties. For example,
    /// at the time of this writing, there are over 300 available properties for a single Contact object. So, if you
    /// need a property that isn't available using the predefined models, you will need to inherit from an existing
    /// model and add your own properties.
    /// </summary>
    [DataContract]
    public class ContactPropertiesModel
    {
        [IgnoreDataMember]
        private string _email { get; set; }
        
        /// <summary>
        /// HubSpot <i>automatically</i> normalizes email addresses to lowercase, but we might need to compare email
        /// address values before retrieving records from HubSpot, so we enforce lowercase email addresses at the object
        /// level. If you are comparing email addresses from another source to the value below, ensure that you
        /// normalize your source value(s) to lowercase.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }
        
        [DataMember(Name = "firstname")]
        public string FirstName { get; set; }
        
        [DataMember(Name = "lastname")]
        public string LastName { get; set; }
        
        [DataMember(Name = "website")]
        public string Website { get; set; }
        
        /// <summary>
        /// Since EmailDomain is a calculated, read-only field (on HubSpot), if you need to compare email domain values
        /// prior to retrieving records from HubSpot, you must use the domain part of the email address. Do not rely
        /// on this field before it has been populated by HubSpot.
        /// </summary>
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
        
        /// <summary>
        /// This is a read-only property containing any secondary emails a contact record might have. This field is
        /// usually populated by merging two contact objects referring to the same person (i.e., an otherwise duplicate
        /// contact) but having different (primary) email addresses. 
        /// </summary>
        [DataMember(Name = "hs_additional_emails")]
        public dynamic AdditionalEmailAddressesList
        {
            get => _additionalEmailAddressesList.ToList();
            set
            {
                switch (value)
                {
                    case string _:
                        _additionalEmailAddressesList.Add(value);
                        break;
                    case IList<string> list:
                        _additionalEmailAddressesList.Union(list);
                        break;
                }
            }
        }

        [IgnoreDataMember]
        private List<string> _additionalEmailAddressesList { get; } = new List<string>();
        public static bool ShouldSerializeAdditionalEmailAddressesList() => false;
        public static bool ShouldDeserializeAdditionalEmailAddressesList() => false;
        
        
        [DataMember(Name = "company")]
        public string Company { get; set; }
        
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        
        [DataMember(Name = "mobilephone")]
        public string  MobilePhone { get; set; }
        
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