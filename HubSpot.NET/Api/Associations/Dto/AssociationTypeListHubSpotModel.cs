using System;
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
        /// Whenever a new custom label is created, HubSpot returns two label objects: The one with the lowest `typeId`
        /// represents the relationship between the source object and the destination object, and the other one
        /// represents the reverse of that relationship. But! For non-CustomAssociationType* models, the
        /// AssociationTypeId of an AssociationTypeHubSpotModel instance is a predetermined enum value that corresponds
        /// to a standard, unlabeled, HubSpot association type. See:
        /// <a href="https://developers.hubspot.com/docs/api/crm/associations#association-type-id-values"> for more
        /// information</a>. We never have to create those types manually, which means we never have to worry about
        /// grabbing the right one after creation, so the following three properties are intended to be overridden in
        /// in the CustomAssociationTypeListHubSpotModel class.
        /// </summary>
        [IgnoreDataMember]
        public virtual IList<T> SortedByTypeId => throw new NotImplementedException(
            "This is only intended for use with instances of CustomAssociationTypeListHubSpotModel.");

        [IgnoreDataMember]
        public virtual T GetSourceToDestLabel => throw new NotImplementedException(
            "This is only intended for use with instances of CustomAssociationTypeListHubSpotModel.");
        
        [IgnoreDataMember]
        public virtual T GetDestToSourceLabel => throw new NotImplementedException(
            "This is only intended for use with instances of CustomAssociationTypeListHubSpotModel.");
        
        
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