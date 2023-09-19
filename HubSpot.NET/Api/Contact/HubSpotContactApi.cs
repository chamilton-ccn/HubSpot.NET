namespace HubSpot.NET.Api.Contact
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Dto;
    using Core;
    using Core.Extensions;
    using Core.Interfaces;
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
            // TODO - remove debugging
            Console.WriteLine($"Creating... {path}");
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
        /// Deletes (archives) a given contact
        /// </summary>
        /// <param name="contact">
        /// A ContactHubSpotModel instance
        /// </param>
        public void Delete(ContactHubSpotModel contact)
        {
            Delete(contact.Id);
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
        /// <param name="opts">Request options - used for pagination etc.</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        public T GetByEmail<T>(string email, ListRequestOptionsV3 opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts == null)
                opts = new ListRequestOptionsV3(); // TODO - ListRequestOptions -or- ListRequestOptionsV3; one has to go!
            
            var path = $"{new T().RouteBasePath}/{email}"
                .SetQueryParam("idProperty", "email");
            
            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParams("properties", opts.PropertiesToInclude);
            
            Console.WriteLine(path);
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

            var data = _client.ExecuteList<ContactListHubSpotModel<T>>(
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
        /// <returns>A list of contacts that were either updated or created</returns>
        /// TODO - Add an "errors" property to ContactListHubSpotModel and ensure this function populates it correctly
        public ContactListHubSpotModel<T> Batch<T>(List<T> contacts) where T : ContactHubSpotModel, new()
        {
            var createPath = $"{new T().RouteBasePath}/batch/create";
            var updatePath = $"{new T().RouteBasePath}/batch/update";
            
            var contactsWithId = new List<T>();
            var contactsWithEmail = new List<T>();

            foreach (var contact in contacts)
            {
                // If contact.Id isn't the default value for long, add it to the list of contacts with a valid Id
                if (contact.Id != 0L) 
                {
                    contactsWithId.Add(contact);
                }
                else if (contact.Email != null)
                {
                    contactsWithEmail.Add(contact);
                }
            }

            var contactsResults = new ContactListHubSpotModel<T>();
            
            // If the contacts in our batch have Id values, we assume this is an update operation.
            if (contactsWithId.Count != 0)
            {
                foreach (var contact in _client.ExecuteBatch<ContactListHubSpotModel<T>>(
                    updatePath, contactsWithId.Select(c => (object)c).ToList(), Method.Post,
                    serialisationType: SerialisationType.BatchUpdateSchema).Contacts)
                    contactsResults.Contacts.Add(contact);
            }
            // If the contacts in our batch only have an Email address, we don't know whether or not they need to be
            // created or updated so we try to create the entire batch first, and if it fails (any single contact in the
            // batch can cause the entire operation to fail) we try to CreateOrUpdate each contact in the batch
            // individually.
            if (contactsWithEmail.Count != 0)
            {
                try
                {
                    foreach (var contact in _client.ExecuteBatch<ContactListHubSpotModel<T>>(
                        createPath, contactsWithEmail.Select(c => (object)c).ToList(), Method.Post,
                        serialisationType: SerialisationType.BatchCreationSchema).Contacts)
                        contactsResults.Contacts.Add(contact);
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
            var data = _client.ExecuteList<ContactListHubSpotModel<T>>(path, opts, Method.Post, convertToPropertiesSchema: true);
            data.SearchRequestOptions = opts;
            return data;
        }

        public ContactListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            if (opts != null) return Search<T>(opts);
            opts = new ContactListHubSpotModel<T>().SearchRequestOptions;
            opts.Limit = 100;
            var searchRequestFilterGroup = new SearchRequestFilterGroup();
            // SearchRequestFilter defaults to "createdate GreaterThanOrEqualTo 7 days ago"
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
            var searchRequestFilter = new SearchRequestFilter();
            searchRequestFilter.PropertyName = "lastmodifieddate";
            searchRequestFilterGroup.Filters.Add(searchRequestFilter);
            opts.FilterGroups.Add(searchRequestFilterGroup);
            return Search<T>(opts);
        }
    }
}
