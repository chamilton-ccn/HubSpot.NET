using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.PropertiesHistory;
namespace HubSpot.NET.Api.Contact.Dto
{
    [DataContract]
    public class ContactPropertiesHistoryModel
    {
        // TODO - Need to write a test that checks to ensure each of the Properties on the ContactPropertiesModel exist in this class.
        [DataMember(Name = "email")]
        public IList<PropertiesHistoryModelItem> Email { get; set; }
        
        [DataMember(Name = "firstname")]
        public IList<PropertiesHistoryModelItem> FirstName { get; set; }
        
        [DataMember(Name = "lastname")]
        public IList<PropertiesHistoryModelItem> LastName { get; set; }
        
        [DataMember(Name = "website")]
        public IList<PropertiesHistoryModelItem> Website { get; set; }
        
        [DataMember(Name = "hs_email_domain")]
        public IList<PropertiesHistoryModelItem> EmailDomain { get; set; }
        
        [DataMember(Name = "company")]
        public IList<PropertiesHistoryModelItem> Company { get; set; }
        
        [DataMember(Name = "phone")]
        public IList<PropertiesHistoryModelItem> Phone { get; set; }
        
        [DataMember(Name = "address")]
        public IList<PropertiesHistoryModelItem> Address { get; set; }
        
        [DataMember(Name = "city")]
        public IList<PropertiesHistoryModelItem> City { get; set; }
        
        [DataMember(Name = "state")]
        public IList<PropertiesHistoryModelItem> State { get; set; }
        
        [DataMember(Name = "zip")]
        public IList<PropertiesHistoryModelItem> ZipCode { get; set; }
    }
}