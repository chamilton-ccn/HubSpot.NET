using System.Runtime.Serialization;
using HubSpot.NET.Api;

namespace HubSpot.NET.Core
{
	[DataContract]
    public class PagingModel
    {
        [DataMember(Name = "next")]
        public NextModel Next { get; set; }
    }
}
