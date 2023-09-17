﻿using System.ComponentModel;

namespace HubSpot.NET.Api.Contact
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization;
    using HubSpot.NET.Api.Contact.Dto;
    using HubSpot.NET.Core;
    using HubSpot.NET.Core.Extensions;
    using HubSpot.NET.Core.Interfaces;
    using RestSharp;

   public class HubSpotContactApi : IHubSpotContactApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotContactApi(IHubSpotClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Creates a contact entity
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public T Create<T>(T entity) where T : ContactHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}";
            return _client.Execute<T>(path, entity, Method.Post, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Updates a given contact
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contact">The contact entity</param>
        public T Update<T>(T contact) where T : ContactHubSpotModel, new()
        {
            var path = contact.Id != 0L
                ? $"{contact.RouteBasePath}/{contact.Id}"
                : (contact.Email != null 
                    ? $"{contact.RouteBasePath}/{contact.Email}".SetQueryParam("idProperty", "email")
                    : throw new ArgumentException("Contact entity must have an id or email set!"));
            return _client.Execute<T>(path, contact, Method.Patch, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Deletes (archives) a given contact
        /// </summary>
        /// <param name="contactId">The ID of the contact</param>
        public void Delete(long contactId)
        {
            var path = $"{new ContactHubSpotModel().RouteBasePath}/{contactId}";
            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }

        /// <summary>
        /// Creates or Updates a contact entity based on the Entity Email
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        public T CreateOrUpdate<T>(T entity) where T : ContactHubSpotModel, new()
        {
            try
            {
                return Create(entity);
            }
            catch (HubSpotException e)
            {
                return Update(entity);
            }
        }

        /// <summary>
        /// Gets a single contact by ID from hubspot
        /// </summary>
        /// <param name="contactId">ID of the contact</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetById<T>(long contactId) where T : ContactHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/{contactId}";

            try
            {
                T data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Gets a contact by their email address
        /// </summary>
        /// <param name="email">Email address to search for</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetByEmail<T>(string email) where T : ContactHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/{email}"
                .SetQueryParam("idProperty", "email");
            try
            {
                T data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// Gets a contact by their user token
        /// </summary>
        /// <param name="userToken">User token to search for from hubspotutk cookie</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new()
        {
            var path = $"/contacts/v1/contact/utk/{userToken}/profile";

            try
            {
                T data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>
        /// List all available contacts 
        /// </summary>
        /// <param name="properties">List of properties to fetch for each contact</param>
        /// <param name="opts">Request options - used for pagination etc.</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>A list of contacts</returns>
        public ContactListHubSpotModel<T> List<T>(ListRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRequestOptions();

            var path = $"{new ContactHubSpotModel().RouteBasePath}"
                .SetQueryParam("limit", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("property", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("after", opts.Offset);

            ContactListHubSpotModel<T> data = _client.ExecuteList<ContactListHubSpotModel<T>>(
                path, convertToPropertiesSchema: true);

            return data;
        }

        /// <summary>
        /// Update or create a set of contacts, this is the preferred method when creating/updating in bulk.
        /// Best performance is with a maximum of 250 contacts. This method will determine whether a contact in the
        /// batch needs to be updated or created, and in the latter case, it will try to create them as a batch, but if
        /// that fails, it will execute CreateOrUpdate for each contact in the batch. 
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contacts">The set of contacts to update/create</param>
        public ContactListHubSpotModel<T> Batch<T>(List<T> contacts) where T : ContactHubSpotModel, new()
        {
            var createPath = $"{new T().RouteBasePath}/batch/create";
            var updatePath = $"{new T().RouteBasePath}/batch/update";
            
            var contactsWithId = new List<T>();
            var contactsWithEmail = new List<T>();

            foreach (var contact in contacts)
            {
                if (contact.Id != null)
                {
                    contactsWithId.Add(contact);
                }
                else if (contact.Email != null && contact.Id == null)
                {
                    contactsWithEmail.Add(contact);
                }
            }

            var contactsResults = new ContactListHubSpotModel<T>();
            
            // If the contacts in our batch have Id values, we assume this is an update operation.
            if (contactsWithId.Count != 0)
            {
                foreach (var _contact in _client.ExecuteBatch<ContactListHubSpotModel<T>>(
                    updatePath, contactsWithId.Select(c => (object)c).ToList(), Method.Post,
                    serialisationType: SerialisationType.BatchUpdateSchema).Contacts)
                    contactsResults.Contacts.Add(_contact);
            }
            // If the contacts in our batch only have an Email address, we don't know whether or not they need to be
            // created or updated so we try to create the entire batch first, and if it fails (any single contact in the
            // batch can cause the entire operation to fail) we try to CreateOrUpdate each contact in the batch
            // individually.
            if (contactsWithEmail.Count != 0)
            {
                try
                {
                    foreach (var _contact in _client.ExecuteBatch<ContactListHubSpotModel<T>>(
                        createPath, contactsWithEmail.Select(c => (object)c).ToList(), Method.Post,
                        serialisationType: SerialisationType.BatchCreationSchema).Contacts)
                        contactsResults.Contacts.Add(_contact);
                }
                catch (HubSpotException e)
                {
                    foreach (var contactWithEmail in contactsWithEmail)
                    {
                        contactsResults.Contacts.Add(CreateOrUpdate(contactWithEmail));
                    }
                }
            }
            return contactsResults;
        }
        
        public ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                return RecentlyCreated<T>();
            var path = $"{new ContactHubSpotModel().RouteBasePath}/search";
            ContactListHubSpotModel<T> data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, Method.Post, convertToPropertiesSchema: true);
            data.SearchRequestOptions = opts;
            return data;
        }

        public ContactListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
            {
                // By default, search options will sort by "createdate" in descending order
                opts = new ContactListHubSpotModel<T>().SearchRequestOptions;
                // 
                opts.Limit = 100;
            }
            return Search<T>(opts);
        }
        
        public ContactListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
            {
                // By default, search options will sort by "createdate" in descending order
                opts = new ContactListHubSpotModel<T>().SearchRequestOptions;
                opts.SortBy = "lastmodifieddate";
            }
            return Search<T>(opts);
        }
        
        /// <summary>
        /// Get recently updated (or created) contacts
        /// </summary>
        /*public ContactListHubSpotModel<T> RecentlyUpdated<T>(ListRecentRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRecentRequestOptions();

            var path = $"{new ContactHubSpotModel().RouteBasePath}/lists/recently_updated/contacts/recent"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("property", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("vidOffset", opts.Offset);

            if (!string.IsNullOrEmpty(opts.TimeOffset))
                path = path.SetQueryParam("timeOffset", opts.TimeOffset);
            
            path = path.SetQueryParam("propertyMode", opts.PropertyMode);
            
            path = path.SetQueryParam("formSubmissionMode", opts.FormSubmissionMode);
            
            path = path.SetQueryParam("showListMemberships", opts.ShowListMemberships);

            ContactListHubSpotModel<T> data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, convertToPropertiesSchema: true);

            return data;
        }*/

        // TODO - Convert to V3 API
        /*public ContactSearchHubSpotModel<T> Search<T>(ContactSearchRequestOptions opts = null)
            where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new ContactSearchRequestOptions();

            var path = $"{new T().RouteBasePath}/search/query"
                .SetQueryParam("q", opts.Query)
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("property", opts.PropertiesToInclude);

            if (opts.Offset != null)
                path = path.SetQueryParam("offset", opts.Offset);

            if (!string.IsNullOrWhiteSpace(opts.SortBy))
            {
                path = path.SetQueryParam("sort", opts.SortBy);

                Type enumType = typeof(SortingOrderType);
                MemberInfo[] memberInfos = enumType.GetMember(opts.Order.ToString());
                MemberInfo enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                object[] valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false);
                string description = ((EnumMemberAttribute)valueAttributes[0]).Value;

                path = path.SetQueryParam("order", description);
            }

            ContactSearchHubSpotModel<T> data = _client.ExecuteList<ContactSearchHubSpotModel<T>>(path, convertToPropertiesSchema: true);

            return data;
        }*/

        /// <summary>
        /// Get a list of recently created contacts
        /// </summary>
        // TODO - Convert to V3 API
        /*public ContactListHubSpotModel<T> RecentlyCreated<T>(ListRecentRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRecentRequestOptions();

            var path = $"{new ContactHubSpotModel().RouteBasePath}/lists/all/contacts/recent"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("property", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("vidOffset", opts.Offset);

            if (!string.IsNullOrEmpty(opts.TimeOffset))
                path = path.SetQueryParam("timeOffset", opts.TimeOffset);
            
            path = path.SetQueryParam("propertyMode", opts.PropertyMode);
            
            path = path.SetQueryParam("formSubmissionMode", opts.FormSubmissionMode);
            
            path = path.SetQueryParam("showListMemberships", opts.ShowListMemberships);

            ContactListHubSpotModel<T> data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, convertToPropertiesSchema: true);

            return data;
        }*/
    }
}
