using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Paging;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationListHubSpotModel<T> : IHubSpotModel where T : AssociationHubSpotModel
    {
        [IgnoreDataMember]
        public IList<T> Associations { get; set; } = new List<T>();
        
        [DataMember(Name = "associationTypes")]
        public IList<AssociationTypeHubSpotModel> AssociationTypes { get; set; }

        public bool ShouldSerializeAssociationTypes() => false; // TODO - not sure if we need to keep this
        
        [DataMember(Name = "inputs")]
        private IList<T> _inputs
        {
            get => Associations;
            set => Associations = value;
        }
        
        [DataMember(Name = "results")]
        private IList<T> _results
        {
            get => Associations;
            set => Associations = value;
        }
        
        public bool ShouldSerialize_results() => false;
        
        [DataMember(Name = "toObjectId")]
        public AssociationObjectIdModel ToObject { get; set; }
        
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }
        
        [IgnoreDataMember]
        public AssociationBatchResultModel BatchResults { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether more results are available.
        /// </summary>
        /// <value>
        /// <c>true</c> if [more results available]; otherwise, <c>false</c>.
        /// </value>
        [IgnoreDataMember]
        public bool MoreResultsAvailable => Paging != null;
            
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v4"; 
        
    }
}