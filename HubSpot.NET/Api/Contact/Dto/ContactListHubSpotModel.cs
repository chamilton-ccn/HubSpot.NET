using System;
using System.Collections.Generic;
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
        /// DEPRECATED! Gets or sets a value indicating whether more results are available.
        /// </summary>
        /// <value>
        /// <c>true</c> if [more results available]; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This is here for backward compatibility
        /// </remarks>
        [IgnoreDataMember]
        [Obsolete(
            "DEPRECATED. In the v3 API, 'list' responses will always have a paging object containing an 'after'" +
            " offset, therefore `MoreResultsAvailable` will always be true.")]
        public bool MoreResultsAvailable = true;

        /// <summary>
        /// DEPRECATED! Gets the continuation offset.
        /// </summary>
        /// <value>
        /// The continuation offset.
        /// </value>
        /// <remarks>
        /// This is here for backward compatibility
        /// </remarks>
        [IgnoreDataMember]
        [Obsolete("DEPRECATED. In the v3 API, 'list' responses will always have a paging object containing an 'after'" +
                  " offset, so this value will always be the same as `Paging.Next.After`")]
        public long ContinuationOffset => int.Parse(Paging.Next.After);
        
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
