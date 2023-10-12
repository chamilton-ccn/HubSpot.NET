using HubSpot.NET.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Paging;
using HubSpot.NET.Core.Search;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Company.Dto
{
    [DataContract]
    public class CompanyListHubSpotModel<T> : IHubSpotModel where T: CompanyHubSpotModel, new()
    {
        /// <summary>
        /// List request status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }
        
        /// <summary>
        /// Total number of companies in the Companies list. This is not always populated! For example: batch operations
        /// do not return a total but search operations do.
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public long? Total { get; set; }
        
        /// <summary>
        /// This is a backing property for both Companies and Results
        /// </summary>
        [IgnoreDataMember]
        private IList<T> _companiesList { get; set; } = new List<T>();
        
        /// <summary>
        /// Gets or sets the list of companies.
        /// </summary>
        /// <value>
        /// The list of companies. Serialized as "inputs" in batch requests.
        /// </value>
        [DataMember(Name = "inputs")]
        public IList<T> Companies
        {
            get => _companiesList;
            set => _companiesList = value;
        }
        
        /// <summary>
        /// Gets or sets the list of companies.
        /// </summary>
        /// <value>
        /// Also the list of companies. Serialized as "results" in batch responses.
        /// </value>        
        [DataMember(Name = "results")]
        public IList<T> Results
        {
            get => _companiesList;
            set => _companiesList = value;
        }
        
        /// <summary>
        /// A count of errors returned in the response
        /// </summary>
        [DataMember(Name = "numErrors", EmitDefaultValue = false)]
        public long? TotalErrors { get; set; }

        /// <summary>
        /// A list of errors returned in the response
        /// </summary>
        [DataMember(Name = "errors")] 
        public IList<ErrorsListItem> Errors { get; set; } = new List<ErrorsListItem>();
        
        public bool ShouldSerializeErrors() => Errors.Count > 0;
        
        [DataMember(Name = "startedAt", EmitDefaultValue = false)]
        public DateTime StartedAt { get; set; }
        
        [DataMember(Name = "completedAt", EmitDefaultValue = false)]
        public DateTime CompletedAt { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether more results are available.
        /// </summary>
        /// <value>
        /// <c>true</c> if [more results available]; otherwise, <c>false</c>.
        /// </value>
        [IgnoreDataMember]
        public bool MoreResultsAvailable => Paging != null;
        
        /// <summary>
        /// Gets the continuation offset.
        /// </summary>
        /// <value>
        /// The continuation offset.
        /// </value>
        /// <remarks>
        /// If the company list is the result of a search and the SearchRequestOptions member has been populated, set
        /// the SearchRequestOptions offset to match the Paging offset.
        /// </remarks>
        [IgnoreDataMember]
        public long? Offset {
            get
            {
                try
                {
                    if (SearchRequestOptions != null)
                        SearchRequestOptions.Offset = Paging.Next.After;
                    return Paging.Next.After;
                }
                catch (NullReferenceException)
                {
                    return null;
                }
            }
        }
        
        [DataMember(Name = "paging")]
        public PagingModel Paging { get; set; }
        
        /// <summary>
        /// Set the default search behavior.
        /// </summary>
        [IgnoreDataMember]
        private SearchRequestOptions _searchRequestOptions = null;
        
        [IgnoreDataMember]
        private readonly SearchRequestOptions _defaultSearchRequestOptions = new SearchRequestOptions();
        
        [IgnoreDataMember]
        public SearchRequestOptions SearchRequestOptions {
            get => _searchRequestOptions ?? _defaultSearchRequestOptions;
            set => _searchRequestOptions = value;
        }        
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "companies";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectType}";
    }
}
