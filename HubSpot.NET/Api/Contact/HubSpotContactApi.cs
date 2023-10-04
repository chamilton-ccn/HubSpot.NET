using System;
using System.Linq;
using System.Net;
using RestSharp;
using System.Collections.Generic;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Api.Contact
{
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
        public T Create<T>(T entity) where T : ContactHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}";
            // TODO - remove serialisationType parameter
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
            // TODO - remove serialisationType parameter 
            return _client.Execute<T>(path, contact, Method.Patch, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Deletes (archives) a given contact
        /// </summary>
        /// <param name="contactId">The ID of the contact</param>
        public void Delete(long contactId)
        {
            var path = $"{new ContactHubSpotModel().RouteBasePath}/{contactId}";
            // TODO - remove convertToPropertiesSchema parameter
            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }
        
        /// <summary>
        /// Deletes (archives) a given contact
        /// </summary>
        /// <param name="contact">
        /// A ContactHubSpotModel instance
        /// </param>
        public void Delete(ContactHubSpotModel contact)
        {
            if (contact.Id == 0L)
                throw new ArgumentException("You must specify a contact ID to delete");
            Delete(contact.Id);
        }

        /// <summary>
        /// Creates or Updates a contact entity based on the Entity Email
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contact">The contact object to create or update</param>
        /// <returns>The created contact (with ID set)</returns>
        public T CreateOrUpdate<T>(T contact) where T : ContactHubSpotModel, new()
        {
            try
            {
                return Create(contact);
            }
            catch (HubSpotException e)
            {
                return Update(contact);
            }
        }

        /// <summary>
        /// Gets a single contact by ID from hubspot
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contactId">The ID of the contact</param>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetById<T>(long contactId) where T : ContactHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/{contactId}";

            try
            {
                // TODO - remove convertToPropertiesSchema parameter
                var data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException e)
            {
                if (e.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        /// <summary>Gets a contact by their email address</summary>
        /// <param name="email">Email address to search for</param>
        /// <param name="opts">
        /// Request options - used to specify properties to return, limiting results, pagination, etc.
        /// </param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetByEmail<T>(string email, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new SearchRequestOptions();
            
            var path = $"{new T().RouteBasePath}/{email}"
                .SetQueryParam("idProperty", "email");
            
            if (opts.PropertiesToInclude.Any())
                path = path.SetPropertiesListQueryParams(opts.PropertiesToInclude);
            
            try
            {
                // TODO - remove convertToPropertiesSchema parameter
                var data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException e)
            {
                if (e.ReturnedError.StatusCode == HttpStatusCode.NotFound)
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
                // TODO - remove convertToPropertiesSchema parameter
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
        /// Creates a batch of Contact objects
        /// </summary>
        /// <param name="contacts"></param>
        /// <typeparam name="T">A ContactHubSpotModel instance</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        public ContactListHubSpotModel<T> BatchCreate<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/batch/create";
            return _client.ExecuteBatch<ContactListHubSpotModel<T>>(path, contacts, Method.Post,
                serialisationType: SerialisationType.Raw); // TODO remove serialisationType parameter
        }

        /// <summary>
        /// Update or create a set of contacts, this is the preferred method when creating/updating in bulk.
        /// Batch operations are <see href="https://developers.hubspot.com/docs/api/crm/contacts#limits">limited to 100
        /// records per batch</see>. This method will determine whether a contact in the batch needs to be updated or
        /// created.
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contacts">The set of contacts to update/create</param>
        /// <returns>A list of contacts that were either updated or created</returns>
        public ContactListHubSpotModel<T> BatchCreateOrUpdate<T>(ContactListHubSpotModel<T> contacts) where T : ContactHubSpotModel, new()
        {
            var createPath = $"{new T().RouteBasePath}/batch/create";
            var updatePath = $"{new T().RouteBasePath}/batch/update";

            var contactsWithId = new ContactListHubSpotModel<T>();
            var contactsWithEmail = new ContactListHubSpotModel<T>();
            var statuses = new List<string>();
            
            foreach (var contact in contacts.Contacts)
            {
                // If contact.Id isn't the default value for long, add it to the list of contacts with id values
                if (contact.Id != 0L)
                {
                    contactsWithId.Contacts.Add(contact);
                }
                else if (contact.Email != null)
                {
                    contactsWithEmail.Contacts.Add(contact);
                }
            }

            var contactsResults = new ContactListHubSpotModel<T>();
            
            // If the contacts in our batch have Id values, we assume this is an update operation.
            if (contactsWithId.Contacts.Count != 0)
            {
                // TODO at this point there is no difference between this invocation of ExecuteBatch and Execute (below)
                //return _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post, serialisationType: SerialisationType.BatchCreationSchema);
                var data = _client.ExecuteBatch<ContactListHubSpotModel<T>>(updatePath, contactsWithId, Method.Post,
                    serialisationType: SerialisationType.Raw); // TODO remove serialisationType parameter
                foreach (var error in data.Errors)
                    contactsResults.Errors.Add(error);
                foreach (var contact in data.Contacts)
                    contactsResults.Contacts.Add(contact);
                statuses.Add(data.Status += $" ({data.Contacts.Count} contacts updated)");
                contactsResults.Total = data.Contacts.Count;
            }
            // If the contacts in our batch do not have Id values, we assume this is a create operation.
            if (contactsWithEmail.Contacts.Count != 0)
            {
                // TODO at this point there is no difference between this invocation of ExecuteBatch and Execute (below)
                //return _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post, serialisationType: SerialisationType.BatchCreationSchema);
                // TODO - We should use BatchCreate here instead of _client.ExecuteBatch
                var data = _client.ExecuteBatch<ContactListHubSpotModel<T>>(createPath, contactsWithEmail,
                    Method.Post, serialisationType: SerialisationType.Raw); // TODO remove serialisationType parameter
                foreach (var error in data.Errors)
                    contactsResults.Errors.Add(error);
                foreach (var contact in data.Contacts) 
                    contactsResults.Contacts.Add(contact);
                statuses.Add(data.Status += $" ({data.Contacts.Count} contacts created)");
                contactsResults.Total += data.Contacts.Count;
            }
            contactsResults.Status = string.Join(",", statuses);
            return contactsResults;
        }
        
        /// <summary>
        /// List all available contacts (basically "search" but with no filter criteria. Nearly identical to
        /// </summary>
        /// <param name="opts">Request options - used for pagination etc.</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>A list of contacts</returns>
        public ContactListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new SearchRequestOptions();

            var path = $"{new T().RouteBasePath}"
                .SetQueryParam("limit", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetPropertiesListQueryParams(opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("after", opts.Offset);
            
            var data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, Method.Get, convertToPropertiesSchema: true);
            /*
             * Update the Offset in opts to match the Offset returned from our request (data.Offset), then set the
             * SearchRequestOptions in our data object to the value of opts (we don't want to lose anything that may
             * have been passed in) so that it can be passed back into this method on the next iteration (assuming there
             * is one).
             */
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }
        
        public ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            /*
             * If no search criteria is supplied, default to "RecentlyCreated".
             */
            if (opts == null) return RecentlyCreated<T>();
            var path = $"{new T().RouteBasePath}/search";
            // TODO - remove convertToPropertiesSchema parameter
            var data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, Method.Post, convertToPropertiesSchema: true);
            /*
             * Update the Offset in opts to match the Offset returned from our request (data.Offset), then set the
             * SearchRequestOptions in our data object to the value of opts (we don't want to lose anything that may
             * have been passed in) so that it can be passed back into this method on the next iteration (assuming there
             * is one).
             */
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }

        public ContactListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts != null) return Search<T>(opts);
            // TODO - this 
            opts = new ContactListHubSpotModel<T>().SearchRequestOptions;
            var searchRequestFilterGroup = new SearchRequestFilterGroup();
            /*
             * SearchRequestFilter defaults to "createdate GreaterThanOrEqualTo 7 days ago", which seems to be a
             * reasonable default for "RecentlyCreated".
             */
            var searchRequestFilter = new SearchRequestFilter();
            searchRequestFilterGroup.Filters.Add(searchRequestFilter);
            opts.FilterGroups.Add(searchRequestFilterGroup);
            return Search<T>(opts);
        }
        
        public ContactListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts != null) return Search<T>(opts);
            opts = new ContactListHubSpotModel<T>().SearchRequestOptions;
            opts.Limit = 100;
            opts.SortBy = "lastmodifieddate";
            var searchRequestFilterGroup = new SearchRequestFilterGroup();
            var searchRequestFilter = new SearchRequestFilter
            {
                PropertyName = "lastmodifieddate"
            };
            searchRequestFilterGroup.Filters.Add(searchRequestFilter);
            opts.FilterGroups.Add(searchRequestFilterGroup);
            return Search<T>(opts);
        }
    }
}
