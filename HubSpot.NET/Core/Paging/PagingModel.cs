using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Paging
{
	[DataContract]
    public class PagingModel
    {
        [DataMember(Name = "next")]
        public NextModel Next { get; set; }
    }
}
