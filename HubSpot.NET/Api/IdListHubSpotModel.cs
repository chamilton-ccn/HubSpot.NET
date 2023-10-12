using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Paging;

namespace HubSpot.NET.Api
{
    // TODO - marked for removal
    [Obsolete("This will be replaced soon")]
    public class IdListHubSpotModel : IHubSpotModel
    {

        [DataMember(Name = "total")]
        public long Total { get; set; }

        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }

        /// <summary>
        /// Gets or sets the <typeparamref name="T"/>.
        /// </summary>
        /// <value>
        /// The <typeparamref name="T"/>.
        /// </value>
        [DataMember(Name = "results")]
        public IList<long> Results { get; set; } = new List<long>();

        public string HubSpotObjectType => throw new NotImplementedException();
        
        public string RouteBasePath => "/crm/v3/objects";

        public bool IsNameValue => false;

        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
