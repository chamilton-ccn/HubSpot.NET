using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Paging;
using HubSpot.NET.Core.Search;

// ReSharper disable InconsistentNaming
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
        /// Total number of contacts in the Contacts list. This is not always populated! For example: batch operations
        /// do not return a total but search operations do. 
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public long? Total { get; set; }

        /// <summary>
        /// The list of contacts
        /// </summary>
        [IgnoreDataMember]
        public IList<T> Contacts { get; set; } = new List<T>();
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// This is a backing field for Contacts. Serialized as "inputs" in batch requests.
        /// </value>
        [DataMember(Name = "inputs")]
        private IList<T> _inputs 
        {
            get => Contacts;
            set => Contacts = value;
        }
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// This is a backing field for Contacts. Deserialized as "results" in batch responses.
        /// </value>        
        [DataMember(Name = "results")]
        private IList<T> _results
        {
            get => Contacts;
            set => Contacts = value;
        }
        public bool ShouldSerializeResults() => false;
        
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

        [IgnoreDataMember]
        public SearchRequestOptions SearchRequestOptions { get; set; }

        [DataMember(Name = "idProperty", EmitDefaultValue = false)]
        private string _idProperty => SearchRequestOptions?.IdProperty;
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "contacts";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectType}";
    }
}
