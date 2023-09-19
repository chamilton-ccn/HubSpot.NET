using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    public class ErrorsListItemContext
    {
        [DataMember(Name = "ids")]
        public IList<string> Ids { get; set; } = new List<string>();
    }
}