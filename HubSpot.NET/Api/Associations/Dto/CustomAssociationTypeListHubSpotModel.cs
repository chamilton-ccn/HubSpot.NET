using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class CustomAssociationTypeListHubSpotModel<T> : AssociationTypeListHubSpotModel<T> 
        where T : CustomAssociationTypeHubSpotModel 
    {
        /// <summary>
        /// Whenever a new label is created, HubSpot returns two label objects: The one with the
        /// lowest `typeId` represents the relationship between the source object and the destination object, and the
        /// other one represents the reverse of that relationship. The following methods are intended to help
        /// differentiate between the two.
        /// </summary>
        // [IgnoreDataMember]
        // public override IList<T> SortedByTypeId => AssociationTypes
        //     .OrderBy(label => label.AssociationTypeId).ToList();
        //
        // [IgnoreDataMember]
        // public override T GetSourceToDestLabel => SortedByTypeId[0];
        //
        // [IgnoreDataMember]
        // public override T GetDestToSourceLabel => SortedByTypeId[1];
    }
}