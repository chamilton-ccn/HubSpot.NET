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
        
        [IgnoreDataMember]
        public IList<T> SortedByTypeId => AssociationLabels.OrderBy(type => type.AssociationTypeId).ToList();
        
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
        public string HubSpotObjectTypeId => "associations";
        
        [IgnoreDataMember]
        public string HubSpotObjectTypeIdPlural => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}