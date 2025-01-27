﻿using System;
using System.Linq;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Paging;
using HubSpot.NET.Core.Search;
using System.Collections.Generic;
using HubSpot.NET.Core.Interfaces;
using System.Runtime.Serialization;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Contact.Dto
{
    [DataContract]
    public class ContactListHubSpotModel<T> : IHubSpotModel, IHubSpotModelList<T> where T: ContactHubSpotModel, new()
    {
        /// <summary>
        /// The list of contacts
        /// </summary>
        [IgnoreDataMember]
        public IList<T> Contacts { get; set; } = new List<T>();
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// The list of contacts. Serialized as "inputs" in batch requests.
        /// </value>
        [DataMember(Name = "inputs")]
        public IList<T> Inputs 
        {
            get => Contacts;
            set => Contacts = value;
        }
        
        /// <summary>
        /// Gets or sets the list of contacts.
        /// </summary>
        /// <value>
        /// The list of contacts. Deserialized from "results" in batch responses.
        /// </value>        
        [DataMember(Name = "results")]
        public IList<T> Results
        {
            get => Contacts;
            set => Contacts = value;
        }
        public bool ShouldSerializeResults() => false;
        
        /// <summary>
        /// List request status
        /// </summary>
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }
        
        /// <summary>
        /// Total results; typically only populated during search operations.
        /// </summary>
        [DataMember(Name = "total", EmitDefaultValue = false)]
        public long? Total { get; set; }
       
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

        public bool ShouldSerializeErrors() => false;
        
        [DataMember(Name = "requestedAt", EmitDefaultValue = false)]
        public DateTime? RequestedAt { get; set; }
        
        [DataMember(Name = "startedAt", EmitDefaultValue = false)]
        public DateTime? StartedAt { get; set; }
        
        [DataMember(Name = "completedAt", EmitDefaultValue = false)]
        public DateTime? CompletedAt { get; set; }
        
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
        public SearchRequestOptions SearchRequestOptions { get; set; } = new SearchRequestOptions();

        [DataMember(Name = "idProperty", EmitDefaultValue = false)]
        public string IdProperty  => SearchRequestOptions.IdProperty;
        
        // TODO - Needs a unit test
        [DataMember(Name = "properties")]
        public IList<string> PropertiesToInclude => SearchRequestOptions.PropertiesToInclude;
        public bool ShouldSerializePropertiesToInclude() => PropertiesToInclude.Any(); 
        
        [DataMember(Name = "propertiesWithHistory", EmitDefaultValue = false)]
        public IList<string> PropertiesWithHistory => SearchRequestOptions.PropertiesWithHistory;
        public bool ShouldSerializePropertiesWithHistory() => PropertiesWithHistory.Any();
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "contacts";
        
        [IgnoreDataMember]
        public string RouteBasePath => $"/crm/v3/objects/{HubSpotObjectType}";
    }
}
