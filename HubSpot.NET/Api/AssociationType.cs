using System;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api
{
    // TODO - marked for removal
    [Obsolete("This will be replaced via the new Associations models & API")]
    public class AssociationType
    {
        [DataMember(Name = "category")]
        public string Category { get; set; }
        [DataMember(Name = "typeId")]
        public long TypeId { get; set; }
        [DataMember(Name = "label")]
        public string Label { get; set; }
    }
}
