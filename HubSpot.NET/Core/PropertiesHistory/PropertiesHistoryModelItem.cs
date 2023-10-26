using System;
using System.Runtime.Serialization;

namespace HubSpot.NET.Core.PropertiesHistory
{
    [DataContract]
    public class PropertiesHistoryModelItem
    {
        [DataMember(Name = "value")]
        public string Value { get; set; }
        
        [DataMember(Name = "timestamp")]
        public DateTime TimeStamp { get; set; }
        
        [DataMember(Name = "sourceType")]
        public string SourceType { get; set; } // TODO - This should be an enum; example values: INTEGRATION, IMPORT, API... https://knowledge.hubspot.com/contacts/understand-source-properties
        
        [DataMember(Name = "sourceId")]
        public string SourceId { get; set; }
    }
}