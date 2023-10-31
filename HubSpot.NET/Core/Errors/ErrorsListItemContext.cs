using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Core.Errors
{
    // TODO - Needs unit test
    public class ErrorsListItemContext
    {
        [DataMember(Name = "ids")]
        public IList<string> Ids { get; set; } = new List<string>();
        
        [DataMember(Name = "missingScopes")]
        public IList<string> MissingScopes { get; set; }
        
        [DataMember(Name = "additionalProp1")]
        public IList<string> AdditionalProperty1 { get; set; }
        
        [DataMember(Name = "additionalProp2")]
        public IList<string> AdditionalProperty2 { get; set; }
        
        [DataMember(Name = "additionalProp3")]
        public IList<string> AdditionalProperty3 { get; set; }
        
        // This must be manually populated!
        [IgnoreDataMember]
        public IList<IHubSpotModel> Objects { get; set; }
    }
}