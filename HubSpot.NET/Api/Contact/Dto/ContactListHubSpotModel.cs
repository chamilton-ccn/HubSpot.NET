using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;

namespace HubSpot.NET.Api.Contact.Dto
{
    /// <summary>
    /// Models a set of results returned when querying for sets of contacts
    /// </summary>
    [DataContract]
    public class ContactListHubSpotModel<T> : IHubSpotModel where T: ContactHubSpotModel, new()
    {
        /// <summary>
        /// Gets or sets the contacts.
        /// </summary>
        /// <value>
        /// The contacts.
        /// </value>
        [DataMember(Name = "results")]
        public IList<T> Contacts { get; set; } = new List<T>();

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
        [IgnoreDataMember]
        public long? Offset {
            get
            {
                try
                {
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
