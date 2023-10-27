using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Search;
using RestSharp;

namespace HubSpot.NET.Api.Company
{
    public class HubSpotCompanyApi : IHubSpotCompanyApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotCompanyApi(IHubSpotClient client)
        {
            _client = client;
        }
        
        /// <summary>
        /// Archive a batch of companies by ID
        /// </summary>
        /// <param name="companies">CompanyListHubSpotModel</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public CompanyListHubSpotModel<T> BatchArchive<T>(CompanyListHubSpotModel<T> companies) 
            where T : CompanyHubSpotModel, new()
        {
            var path = $"{companies.RouteBasePath}/batch/archive";
            
            foreach (var contact in companies.Companies)
            {
                contact.SerializeAssociations = false;
                contact.SerializeProperties = false;
            }
            
            // TODO - remove SerializationType parameter
            _client.Execute<CompanyListHubSpotModel<T>>(path, companies, Method.Post,
                SerialisationType.PropertyBag);
            companies.Status = "ARCHIVED"; // TODO - Should this be an enum?
            return companies;
        }
        
        /// <summary>
        /// Create a batch of companies
        /// </summary>
        /// <param name="companies">CompanyListHubSpotModel</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public CompanyListHubSpotModel<T> BatchCreate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new()
        {
            var path = $"{companies.RouteBasePath}/batch/create";
            // TODO remove serialisationType parameter
            return _client.Execute<CompanyListHubSpotModel<T>>(path, companies, Method.Post,
                serialisationType: SerialisationType.Raw); 
        }
        
        /// <summary>
        /// Read a batch of companies by internal ID, or unique property values
        /// </summary>
        /// <param name="companies">CompanyListHubSpotModel</param>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public CompanyListHubSpotModel<T> BatchRead<T>(CompanyListHubSpotModel<T> companies,
            SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {   
            opts = opts != null 
                ? companies.SearchRequestOptions = opts 
                : companies.SearchRequestOptions;
            
            var path = $"{companies.RouteBasePath}/batch/read";

            path = opts.Archived
                ? path.SetQueryParam("archived", true)
                : path;

            foreach (var contact in companies.Companies)
            {   
                contact.SerializeAssociations = false;
                contact.SerializeProperties = false;
            }   

            // TODO - remove SerializationType parameter
            return _client.Execute<CompanyListHubSpotModel<T>>(path, companies, Method.Post,
                SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Update a batch of companies
        /// </summary>
        /// <param name="companies">CompanyListHubSpotModel</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public CompanyListHubSpotModel<T> BatchUpdate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new()
        {
            var path = $"{companies.RouteBasePath}/batch/update";
            // TODO - remove SerializationType parameter
            return _client.Execute<CompanyListHubSpotModel<T>>(path, companies, Method.Post,
                SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Read a page of companies. Control what is returned via the properties query param.
        /// </summary>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public CompanyListHubSpotModel<T> List<T>(SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new()
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
            var data = _client.Execute<CompanyListHubSpotModel<T>>(path, opts, Method.Get,
                SerialisationType.PropertyBag);
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }
        
        /// <summary>
        /// Create a company with the given properties and return a copy of the object, including the ID
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="company">A CompanyHubSpotModel instance</param>
        /// <returns>A CompanyHubSpotModel instance with the Id field populated</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public T Create<T>(T company) where T : CompanyHubSpotModel, new()
        {
            var path = $"{company.RouteBasePath}";
            // TODO - remove serialisationType parameter
            return _client.Execute<T>(path, company, Method.Post, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Get a single company by unique Id
        /// </summary>
        /// <param name="uniqueId">The unique ID of the company</param>
        /// <param name="opts">SearchRequestOptions</param>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <returns>A CompanyHubSpotModel instance or null if one cannot be found</returns>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public T GetByUniqueId<T>(string uniqueId, SearchRequestOptions opts = null) 
            where T : CompanyHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            
            var path = $"{new T().RouteBasePath}/{uniqueId}";

            path = opts.IdProperty != null
                ? path.SetQueryParam("idProperty", opts.IdProperty)
                : path;
            
            path = opts.PropertiesToInclude.Any()
                ? path.SetPropertiesListQueryParams(opts.PropertiesToInclude)
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
            where T : CompanyHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            return GetByUniqueId<T>(uniqueId.ToString(), opts);
        }
        public T GetByUniqueId<T>(int uniqueId, SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            return GetByUniqueId<T>(uniqueId.ToString(), opts);
        }
        
        /// <summary>
        /// Perform a partial update of a company identified by Id
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="company">CompanyHubSpotModel</param>
        /// <param name="idProperty">The name of the unique property value to use as the unique id</param>
        /// <exception cref="ArgumentException">Id property of the company object cannot be null</exception>
        /// <remarks>
        /// The Id field can contain the value of any valid unique identifier as long as
        /// that identifier is specified by the idProperty parameter.
        /// </remarks>
        /// <seealso href="https://developers.hubspot.com/docs/api/crm/companies"/>
        public T Update<T>(T company, string idProperty = null) where T : CompanyHubSpotModel, new()
        {
            if (company.Id == null)
                throw new ArgumentException("Id property cannot be null!");
            
            var path = $"{company.RouteBasePath}/{company.Id}";
            
            path = idProperty != null
                ? path.SetQueryParam("idProperty", idProperty)
                : path;
            
            // TODO - remove serialisationType parameter 
            return _client.Execute<T>(path, company, Method.Patch, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Deletes (archives) the given company
        /// </summary>
        /// <param name="companyId">The ID of the company</param>
        public void Delete(long companyId)
        {
            var path = $"{new CompanyHubSpotModel().RouteBasePath}/{companyId}";
            // TODO - remove convertToPropertiesSchema parameter
            _client.Execute(path, method: Method.Delete, convertToPropertiesSchema: true);
        }
        
        /// <summary>
        /// Deletes (archives) a given company
        /// </summary>
        /// <param name="company">
        /// A CompanyHubSpotModel instance
        /// </param>
        public void Delete(CompanyHubSpotModel company)
        {
            if (company.Id == null)
                throw new ArgumentException("Unable to delete without specifying an id.");
            Delete(company.Id);
        }
        
        // TODO [PUBLIC_OBJECT] [MERGE]
        // TODO [GDPR] [GDPR DELETE]
        
        public CompanyListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            opts ??= new SearchRequestOptions();
            var path = $"{new T().RouteBasePath}/search";
            // TODO - remove SerializationType parameter
            var data = _client.Execute<CompanyListHubSpotModel<T>>(path, opts, Method.Post, SerialisationType.PropertiesSchema);
            opts.Offset = data.Offset;
            data.SearchRequestOptions = opts;
            return data;
        }
        
        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="CompanyHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        // TODO - refactor
        /*public T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var companyPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/6";
            long? offSet = null;

            var dealResults = new List<long>();
            do
            {
                var dealAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", companyPath, offSet == null ? null : "&offset=" + offSet), convertToPropertiesSchema: false);
                if (dealAssociations.Results.Any())
                    dealResults.AddRange(dealAssociations.Results);
                if (dealAssociations.HasMore)
                    offSet = dealAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);
            if (dealResults.Any())
                entity.Associations.AssociatedDeals = dealResults.ToArray();
            else
                entity.Associations.AssociatedDeals = null;

            // see https://legacydocs.hubspot.com/docs/methods/crm-associations/crm-associations-overview
            var contactPath = $"/crm-associations/v1/associations/{entity.Id}/HUBSPOT_DEFINED/2";

            var contactResults = new List<long>();
            do
            {
                var contactAssociations = _client.ExecuteList<AssociationIdListHubSpotModel>(string.Format("{0}?limit=100{1}", contactPath, offSet == null ? null : "&offset=" + offSet), convertToPropertiesSchema: false);
                if (contactAssociations.Results.Any())
                    contactResults.AddRange(contactAssociations.Results);
                if (contactAssociations.HasMore)
                    offSet = contactAssociations.Offset;
                else
                    offSet = null;
            } while (offSet != null);
            if (contactResults.Any())
                entity.Associations.AssociatedContacts = contactResults.ToArray();
            else
                entity.Associations.AssociatedContacts = null;

            return entity;
        }*/
    }
}