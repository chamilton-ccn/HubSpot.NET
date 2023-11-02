using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.PropertiesHistory;
namespace HubSpot.NET.Api.Company.Dto
{
    [DataContract]
    public class CompanyPropertiesHistoryModel
    {
        [DataMember(Name = "name")]
        public IList<PropertiesHistoryModelItem> Name { get; set; }

        [DataMember(Name = "domain")]
        public IList<PropertiesHistoryModelItem> Domain { get; set; }

        [DataMember(Name = "website")]
        public IList<PropertiesHistoryModelItem> Website { get; set; }
        
        [DataMember(Name = "phone")]
        public IList<PropertiesHistoryModelItem> Phone { get; set; }

        [DataMember(Name = "description")]
        public IList<PropertiesHistoryModelItem> Description { get; set; }
        
        [DataMember(Name = "address")]
        public IList<PropertiesHistoryModelItem> StreetAddress1 { get; set; }
        
        [DataMember(Name = "address2")]
        public IList<PropertiesHistoryModelItem> StreetAddress2 { get; set; }
        
        [DataMember(Name = "about_us")]
        public IList<PropertiesHistoryModelItem> About { get; set; }
        
        [DataMember(Name = "city")]
        public IList<PropertiesHistoryModelItem> City { get; set; }
        
        [DataMember(Name = "state")]
        public IList<PropertiesHistoryModelItem> State { get; set; }
        
        [DataMember(Name = "zip")]
        public IList<PropertiesHistoryModelItem> ZipCode { get; set; }
        
        [DataMember(Name = "country")]
        public IList<PropertiesHistoryModelItem> Country { get; set; }
    }
}