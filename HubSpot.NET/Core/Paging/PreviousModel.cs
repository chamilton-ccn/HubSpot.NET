using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Paging
{
    [DataContract]
    public class PreviousModel
    {
        [DataMember(Name = "before")]
        public long Before { get; set; }
        
        [DataMember(Name = "link")]
        public string Link { get; set; }        
    }
}