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
        public IList<T> AssociationLabels { get; set; } = new List<T>();

        /// <summary>
        /// So here's the thing: Whenever a new label is created, HubSpot returns two label objects: The one with the
        /// lowest `typeId` represents the relationship between the source object and the destination object, and the
        /// other one represents the reverse of that relationship. The following methods are intended to help
        /// differentiate between the two.
        /// </summary>
        [IgnoreDataMember]
        public IList<T> SortedByTypeId => AssociationLabels.OrderBy(label => label.AssociationTypeId).ToList();

        [IgnoreDataMember]
        public T GetSourceToDestLabel => SortedByTypeId[0];
        
        [IgnoreDataMember]
        public T GetDestToSourceLabel => SortedByTypeId[1];
        
        
        [DataMember(Name = "results")]
        private IList<T> _results
        {
            get => AssociationLabels;
            set => AssociationLabels = value;
        }

        public bool ShouldSerialize_results() => false;
        
        [DataMember(Name = "types")]
        private IList<T> _types
        { 
            get => AssociationLabels;
            set => AssociationLabels = value;
        }
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}