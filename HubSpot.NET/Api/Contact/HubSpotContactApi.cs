using System;
using System.Linq;
using System.Net;
using RestSharp;
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
        /// Archive a batch of contacts by ID
        /// </summary>
        /// <param name="contacts">ContactListHubSpotModel</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public ContactListHubSpotModel<T> BatchArchive<T>(ContactListHubSpotModel<T> contacts) 
            where T : ContactHubSpotModel, new()
        {
            var path = $"{contacts.RouteBasePath}/batch/archive";
            
            foreach (var contact in contacts.Contacts)
            {
                contact.SerializeAssociations = false;
                contact.SerializeProperties = false;
            }
            
            // TODO - remove SerializationType parameter
            _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post,
                SerialisationType.PropertyBag);
            contacts.Status = "ARCHIVED"; // TODO - Should this be an enum?
            return contacts;
        }
        
        /// <summary>
        /// Create a batch of contacts
        /// </summary>
        /// <param name="contacts">ContactListHubSpotModel</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public ContactListHubSpotModel<T> BatchCreate<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new()
        {
            var path = $"{contacts.RouteBasePath}/batch/create";
            // TODO remove serialisationType parameter
            return _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post,
                serialisationType: SerialisationType.Raw); 
        }

        /// <summary>
        /// Read a batch of contacts by internal ID, or unique property values
        /// </summary>
        /// <param name="contacts">ContactListHubSpotModel</param>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public ContactListHubSpotModel<T> BatchRead<T>(ContactListHubSpotModel<T> contacts,
            SearchRequestOptions opts = null) where T : ContactHubSpotModel, new()
        {
            opts = opts != null 
                ? contacts.SearchRequestOptions = opts 
                : contacts.SearchRequestOptions;
            
            var path = $"{contacts.RouteBasePath}/batch/read";

            path = opts.Archived
                ? path.SetQueryParam("archived", true)
                : path;
            
            foreach (var contact in contacts.Contacts)
            {
                contact.SerializeAssociations = false;
                contact.SerializeProperties = false;
            }

            // TODO - remove SerializationType parameter
            return _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post,
                SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Update a batch of contacts
        /// </summary>
        /// <param name="contacts">ContactListHubSpotModel</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public ContactListHubSpotModel<T> BatchUpdate<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new()
        {
            var path = $"{contacts.RouteBasePath}/batch/update";
            // TODO - remove SerializationType parameter
            return _client.Execute<ContactListHubSpotModel<T>>(path, contacts, Method.Post,
                SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Read a page of contacts. Control what is returned via the properties query param.
        /// </summary>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>ContactListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public ContactListHubSpotModel<T> List<T>(SearchRequestOptions opts = null)
            where T : ContactHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();

            var path = $"{new T().RouteBasePath}";

            path = opts.PropertiesToInclude.Any()
                ? path.SetPropertiesListQueryParams(opts.PropertiesToInclude)
                : path;

            path = opts.PropertiesWithHistory.Any()
                ? path.SetPropertiesListQueryParams(opts.PropertiesWithHistory, "propertiesWithHistory")
                : path;
            
            path = path.SetQueryParam("limit", opts.Limit);
            
            path = opts.Archived 
                ? path.SetQueryParam("archived", true)
                : path;

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("after", opts.Offset);
            
            // TODO - remove SerializationType parameter
            var data = _client.Execute<ContactListHubSpotModel<T>>(path, opts, Method.Get,
                SerialisationType.PropertyBag);
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }
        
        /// <summary>
        /// Create a contact with the given properties and return a copy of the object, including the ID
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contact">A ContactHubSpotModel instance</param>
        /// <returns>A ContactHubSpotModel instance with the Id field populated</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public T Create<T>(T contact) where T : ContactHubSpotModel, new()
        {
            var path = $"{contact.RouteBasePath}";
            // TODO - remove serialisationType parameter
            return _client.Execute<T>(path, contact, Method.Post, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Get a single contact by unique Id
        /// </summary>
        /// <param name="uniqueId">The unique ID of the contact</param>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>A ContactHubSpotModel instance or null if one cannot be found</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public T GetByUniqueId<T>(string uniqueId, SearchRequestOptions opts = null) 
            where T : ContactHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            
            var path = $"{new T().RouteBasePath}/{uniqueId}";

            path = opts.IdProperty != null
                ? path.SetQueryParam("idProperty", opts.IdProperty)
                : path;
            
            path = opts.PropertiesToInclude.Any()
                ? path.SetPropertiesListQueryParams(opts.PropertiesToInclude)
                : path;
            
            path = opts.PropertiesWithHistory.Any()
                ? path.SetPropertiesListQueryParams(opts.PropertiesWithHistory, "propertiesWithHistory")
                : path;
            
            path = opts.Archived 
                ? path.SetQueryParam("archived", true)
                : path;
            
            try
            {
                // TODO - Remove SerializationType parameter
                return _client.Execute<T>(path, Method.Get, SerialisationType.PropertyBag);
            }
            catch (HubSpotException e)
            {
                if (e.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }
        public T GetByUniqueId<T>(long uniqueId, SearchRequestOptions opts = null)
            where T : ContactHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            return GetByUniqueId<T>(uniqueId.ToString(), opts);
        }
        public T GetByUniqueId<T>(int uniqueId, SearchRequestOptions opts = null)
            where T : ContactHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            return GetByUniqueId<T>(uniqueId.ToString(), opts);
        }
        
        /// <summary>
        /// Perform a partial update of a contact identified by Id
        /// </summary>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <param name="contact">ContactHubSpotModel</param>
        /// <param name="idProperty">The name of the unique property value to use as the unique id</param>
        /// <exception cref="ArgumentException">Id property of the contact object cannot be null</exception>
        /// <remarks>
        /// The Id field can contain the value of any valid unique identifier as long as
        /// that identifier is specified by the idProperty parameter.
        /// </remarks>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/contacts"/>
        public T Update<T>(T contact, string idProperty = null) where T : ContactHubSpotModel, new()
        {
            if (contact.Id == null)
                throw new ArgumentException("Id property cannot be null!");
            
            var path = $"{contact.RouteBasePath}/{contact.Id}";
            
            path = idProperty != null
                ? path.SetQueryParam("idProperty", idProperty)
                : path;
            
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
            if (contact.Id == null)
                throw new ArgumentException("Unable to delete without specifying an id.");
            Delete(contact.Id);
        }
        
        //  TODO [PUBLIC_OBJECT] [MERGE]
        //  TODO [GDPR] [GDPR DELETE]
        
        /// <summary>
        /// Gets a contact by their user token
        /// </summary>
        /// <param name="userToken">User token to search for from hubspotutk cookie</param>
        /// <typeparam name="T">Implementation of ContactHubSpotModel</typeparam>
        /// <returns>The contact entity or null if the contact does not exist</returns>
        /// TODO - Needs updating or removal
        [Obsolete("Conditionally obsolete: Needs documentation for > v1 API")]
        public T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new()
        {
            var path = $"/contacts/v1/contact/utk/{userToken}/profile";

            try
            {
                // TODO - remove convertToPropertiesSchema parameter
                var data = _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
                return data;
            }
            catch (HubSpotException exception)
            {
                if (exception.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) 
            where T : ContactHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            var path = $"{new T().RouteBasePath}/search";
            // TODO - remove SerializationType parameter
            var data = _client.Execute<ContactListHubSpotModel<T>>(path, opts, Method.Post, 
                SerialisationType.PropertyBag);
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }
    }
}
