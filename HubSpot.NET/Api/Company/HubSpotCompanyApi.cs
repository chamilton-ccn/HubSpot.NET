using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Extensions;
using HubSpot.NET.Core.Interfaces;
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
        /// Creates a Company entity
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="entity">The entity</param>
        /// <returns>The created entity (with ID set)</returns>
        public T Create<T>(T entity) where T : CompanyHubSpotModel, new()
        {
            var path = $"{entity.RouteBasePath}";
            // TODO - remove serialisationType parameter
            return _client.Execute<T>(path, entity, Method.Post, SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Updates a given company entity, any changed properties are updated
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="company">The company entity</param>
        /// <returns>The updated company entity</returns>
        public T Update<T>(T company) where T : CompanyHubSpotModel, new()
        {
            var path = company.Id != 0L
                ? $"{company.RouteBasePath}/{company.Id}"
                : throw new ArgumentException("Company entity must have an id set!");

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
            if (company.Id == 0L)
                throw new ArgumentException("Company entity must have an id.");
            Delete(company.Id);
        }

        /// <summary>
        /// Creates or Updates a company entity
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="company">The company object to create or update</param>
        /// <returns>The created company (with ID set)</returns>
        public T CreateOrUpdate<T>(T company) where T : CompanyHubSpotModel, new()
        {
            try
            {
                return Create(company);
            }
            catch (HubSpotException e)
            {
                return Update(company);
            }
        }
        
        /// <summary>
        /// Creates a batch of Company objects
        /// </summary>
        /// <param name="companies"></param>
        /// <typeparam name="T">A CompanyHubSpotModel instance</typeparam>
        /// <returns>CompanyListHubSpotModel</returns>
        public CompanyListHubSpotModel<T> BatchCreate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/batch/create";
            return _client.ExecuteBatch<CompanyListHubSpotModel<T>>(path, companies, Method.Post,
                serialisationType: SerialisationType.Raw); // TODO - remove serializationType parameter
        }
        
        // TODO - Add a BatchDelete method
        
        /// <summary>
        /// Update or create a set of companies, this is the preferred method when creating/updating in bulk.
        /// This method will determine whether a company in the batch needs to be updated or created.
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="companies">The set of companies to update/create</param>
        /// <returns>A list of companies that were either updated or created</returns>
        public CompanyListHubSpotModel<T> BatchCreateOrUpdate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new()
        {
            var updatePath = $"{new T().RouteBasePath}/batch/update";
            
            var companiesWithId = new CompanyListHubSpotModel<T>();
            var companiesWithOutId = new CompanyListHubSpotModel<T>();
            var statuses = new List<string>();
            
            foreach (var company in companies.Companies)
            {
                // If company.Id isn't the default value for long, add it to the list of companies with id values
                if (company.Id != 0L)
                {
                    companiesWithId.Companies.Add(company);
                }
                else
                {
                    companiesWithOutId.Companies.Add(company);
                }
            }
            var companiesResults = new CompanyListHubSpotModel<T>();
            
            // If the companies in our batch have Id values, we assume this is an update operation.
            if (companiesWithId.Companies.Count != 0)
            {
                // TODO at this point there is no difference between this invocation of ExecuteBatch and Execute (below)
                //return _client.Execute<CompanyListHubSpotModel<T>>(path, companies, Method.Post, serialisationType: SerialisationType.BatchCreationSchema);
                var data = _client.ExecuteBatch<CompanyListHubSpotModel<T>>(updatePath, companiesWithId, Method.Post,
                    serialisationType: SerialisationType.Raw); // TODO remove serialisationType parameter
                foreach (var error in data.Errors)
                    companiesResults.Errors.Add(error);
                foreach (var company in data.Companies)
                    companiesResults.Companies.Add(company);
                statuses.Add(data.Status += " (batch update)");
            }
            // If the companies in our batch do not have Id values, we assume this is a create operation.
            if (companiesWithOutId.Companies.Count != 0)
            {
                var data = BatchCreate(companiesWithOutId);
                foreach (var error in data.Errors)
                    companiesResults.Errors.Add(error);
                foreach (var company in data.Companies)
                    companiesResults.Companies.Add(company);
                statuses.Add(data.Status += " (batch create)");
            }
            companiesResults.Status = string.Join(",", statuses);
            return companiesResults;
        }

        /// <summary>
        /// Gets a single company by ID from hubspot
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="companyId">The ID of the company</param>
        /// <returns>The company entity or null if the company does not exist</returns>
        public T GetById<T>(long companyId) where T : CompanyHubSpotModel, new()
        {
            var path =  $"{new T().RouteBasePath}/{companyId}";

            try
            {
                // TODO - remove convertToPropertiesSchema parameter
                return _client.Execute<T>(path, Method.Get, convertToPropertiesSchema: true);
            }
            catch (HubSpotException e)
            {
                if (e.ReturnedError.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        //TODO - refactor
        public CompanyListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T: CompanyHubSpotModel, new()
        {
            if (opts == null)
                opts = new SearchRequestOptions();

            var path = $"{new CompanyHubSpotModel().RouteBasePath}/companies/paged"
                .SetQueryParam("count", opts.Limit);

            if (opts.PropertiesToInclude.Any())
                path = path.SetQueryParam("properties", opts.PropertiesToInclude);

            if (opts.Offset.HasValue)
                path = path.SetQueryParam("offset", opts.Offset);

			CompanyListHubSpotModel<T> data = _client.ExecuteList<CompanyListHubSpotModel<T>>(path, convertToPropertiesSchema: true);

            return data;
        }

        public CompanyListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            /*
             * If no search criteria is supplied, default to "RecentlyCreated".
             */
            if (opts == null) return RecentlyCreated<T>();
            var path = $"{new T().RouteBasePath}/search";
            // TODO - remove convertToPropertiesSchema parameter
            var data = _client.ExecuteList<CompanyListHubSpotModel<T>>(path, opts, Method.Post,
                convertToPropertiesSchema: true);
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

        
        /// <summary>
        /// Gets a list of companies by domain name
        /// </summary>
        /// <typeparam name="T">Implementation of CompanyHubSpotModel</typeparam>
        /// <param name="domain">Domain name to search for</param>
        /// <param name="opts">
        /// Set of search options - domain name will be AND'ed to this if it is specified
        /// </param>
        /// <returns>
        /// A list of companies whose domain name matches
        /// </returns>
        /// <remarks>
        /// This exists for backward compatibility.
        /// </remarks>
        [Obsolete("This functionality will be removed in favor of the general purpose Search method")]
        public CompanyListHubSpotModel<T> GetByDomain<T>(string domain, SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            var domainFilter = new SearchRequestFilterGroup
            {
                Filters = new List<SearchRequestFilter>
                {
                    new SearchRequestFilter
                    {
                        PropertyName = "domain",
                        Operator = SearchRequestFilterOperatorType.EqualTo,
                        Value = domain
                    }
                }
            };
            
            if (opts == null)
            {
                opts = new SearchRequestOptions()
                {
                    FilterGroups = new List<SearchRequestFilterGroup> { domainFilter },
                };
            }
            else
            {
                opts.FilterGroups.Add(domainFilter);
            }
            if (!opts.PropertiesToInclude.Contains("domain"))
                opts.PropertiesToInclude.Add("domain");
            return Search<T>(opts);
        }
        
        public CompanyListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            if (opts != null) return Search<T>(opts);
            opts = new CompanyListHubSpotModel<T>().SearchRequestOptions;
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
        
        public CompanyListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new()
        {
            if (opts != null) return Search<T>(opts);
            opts = new CompanyListHubSpotModel<T>().SearchRequestOptions;
            opts.Limit = 100;
            opts.SortBy = "hs_lastmodifieddate";
            var searchRequestFilterGroup = new SearchRequestFilterGroup();
            var searchRequestFilter = new SearchRequestFilter
            {
                PropertyName = "hs_lastmodifieddate"
            };
            searchRequestFilterGroup.Filters.Add(searchRequestFilter);
            opts.FilterGroups.Add(searchRequestFilterGroup);
            return Search<T>(opts);
        }        

        /// <summary>
        /// Gets a list of associations for a given deal
        /// </summary>
        /// <typeparam name="T">Implementation of <see cref="CompanyHubSpotModel"/></typeparam>
        /// <param name="entity">The deal to get associations for</param>
        // TODO - refactor
        public T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new()
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
        }
    }
}