using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationTypeListHubSpotModel<T> : IHubSpotModel where T : AssociationTypeHubSpotModel
    {
        [IgnoreDataMember]
        public IList<T> AssociationTypes { get; set; } = new List<T>();

        /// <summary>
        /// Whenever a new custom ("USER_DEFINED") association type is created, HubSpot returns two type objects: The
        /// one with the lowest `typeId` represents the relationship between the source (from) object and the
        /// destination (to) object, and the other one represents the reverse of that relationship. For non-custom
        /// association types, the AssociationTypeId of an AssociationTypeHubSpotModel instance is a predetermined enum
        /// value that corresponds to a standard, unlabeled, HubSpot association type. See:
        /// <a href="https://developers.hubspot.com/docs/api/crm/associations#association-type-id-values"> for more
        /// information</a>. We never have to create those types manually, which means we never have to worry about
        /// grabbing the right one after creation, but for custom types, we often need to create it then use it directly
        /// afterwards. The following three properties are intended to assist with that.
        /// </summary>
        [IgnoreDataMember]
        public IList<T> SortedByTypeId => AssociationTypes
            .OrderBy(label => (int)label.AssociationTypeId).ToList();

        [IgnoreDataMember]
        public T GetSourceToDestLabel => SortedByTypeId[0];
        
        [IgnoreDataMember]
        public T GetDestToSourceLabel => SortedByTypeId[1];
        
        [DataMember(Name = "results")]
        private IList<T> _results
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }

        public bool ShouldSerialize_results() => false;
        
        [DataMember(Name = "types")]
        private IList<T> _types
        { 
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}