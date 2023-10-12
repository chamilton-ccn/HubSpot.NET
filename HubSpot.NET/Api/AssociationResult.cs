using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HubSpot.NET.Api
{
    // TODO - marked for removal
    [Obsolete("This will be replaced via the new Associations models & API")]
    public class AssociationResult
    {
        [DataMember(Name = "toObjectId")]
        public long? ToObjectId { get; set; }

        [DataMember(Name = "associationTypes")]
        public List<AssociationType> AssociationTypes { get; set; }
    }
}
