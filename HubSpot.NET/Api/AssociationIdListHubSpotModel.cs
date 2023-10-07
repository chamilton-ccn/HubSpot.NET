using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api
{
    public class AssociationIdListHubSpotModel : IHubSpotModel
    {
        [DataMember(Name = "hasMore")]
        public bool HasMore { get; set; }

        [DataMember(Name = "offset")]
        public long? Offset { get; set; }

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/>.
        /// </summary>
        /// <value>
        /// The <typeparamref name="T"/>.
        /// </value>
        [DataMember(Name = "results")]
        public IList<long> Results { get; set; } = new List<long>();

        public string HubSpotObjectTypeId => "associations";
        public string HubSpotObjectTypeIdPlural => "associations";
        //public string RouteBasePath => "/crm/v4";
        public string RouteBasePath => "/crm/v3/objects";
    }
}