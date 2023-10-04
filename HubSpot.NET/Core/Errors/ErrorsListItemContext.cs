using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Core.Errors
{
    public class ErrorsListItemContext
    {
        [DataMember(Name = "ids")]
        public IList<string> Ids { get; set; } = new List<string>();
        
        // This must be manually populated!
        [IgnoreDataMember]
        public IList<IHubSpotModel> Objects { get; set; }
    }
}