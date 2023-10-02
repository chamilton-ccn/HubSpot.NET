using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global

namespace HubSpot.NET.Api.Contact.Dto
{
    /// <summary>
    /// Models a set of results returned when querying for sets of contacts
    /// </summary>
    [DataContract]
    public class ContactListHubSpotModel<T> : IHubSpotModel where T: ContactHubSpotModel, new()
    {
        /// <summary>
        /// List request status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }
        
        /// <summary>
        /// Total number of contacts in the Contacts list.
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public long? Total { get; set; }

        /// <summary>
        /// This is a backing property for both Contacts and Results
        /// </summary>
        private IList<T> ContactsList { get; set; } = new List<T>();
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// The list of contacts. Serialized as "inputs" in batch requests.
        /// </value>
        [DataMember(Name = "inputs")]
        public IList<T> Contacts
        {
            get => ContactsList;
            set => ContactsList = value;
        }
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// Also the list of contacts. Serialized as "results" in batch responses.
        /// </value>        
        [DataMember(Name = "results")]
        public IList<T> Results
        {
            get => ContactsList;
            set => ContactsList = value;
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
        /// If the contact list is the result of a search and the SearchRequestOptions member has been populated, set
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
        private SearchRequestOptions _searchRequestOptions = null;
        private readonly SearchRequestOptions _defaultSearchRequestOptions = new SearchRequestOptions();
        [IgnoreDataMember]
        public SearchRequestOptions SearchRequestOptions {
            get => _searchRequestOptions ?? _defaultSearchRequestOptions;
            set => _searchRequestOptions = value;
        }
        
        public string RouteBasePath => "/crm/v3/objects/contacts";
        
        public bool IsNameValue => false;
        public virtual void ToHubSpotDataEntity(ref dynamic converted)
        {
        }

        public virtual void FromHubSpotDataEntity(dynamic hubspotData)
        {
        }
    }
}
