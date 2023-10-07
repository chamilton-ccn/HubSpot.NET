using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Search;
using HubSpot.NET.Core.Utilities;

namespace HubSpot.NET.Examples
{
    public class Companies
    {
        public static void Example(HubSpotApi api)
        {
            /*
             * Create a company
             */
            Console.WriteLine("* Creating a company ...");
            var company = api.Company.Create(new CompanyHubSpotModel()
            {
                Domain = "squaredup.com",
                Name = "Squared Up"
            });
            Console.WriteLine($"-> Company created: {company.Name} ...");
            
            // Wait for HubSpot to catch up
            Utilities.Sleep(15);
            
            /*
             * Search for recently created companies
             */
            Console.WriteLine("* Searching for recently created companies ...");
            var recentlyCreated = api.Company
                .RecentlyCreated<CompanyHubSpotModel>();
            var moreResults = true;
            while (moreResults)
            {
                moreResults = recentlyCreated.MoreResultsAvailable;
                foreach (var recentResult in recentlyCreated.Companies)
                {
                    Console.WriteLine($"-> Found recently created company: {recentResult.Name}");
                }
                if (moreResults)
                    recentlyCreated =
                        api.Company.RecentlyUpdated<CompanyHubSpotModel>(recentlyCreated.SearchRequestOptions);
            }

            /*
             * Update a company's property
             */
            Console.WriteLine("* Updating a company ...");
            company.Description = "Data Visualization for Enterprise IT";
            company = api.Company.Update(company);
            Console.WriteLine($"-> Company description updated: {company.Name} {company.Description}...");
            
            // Wait for HubSpot to catch up
            Utilities.Sleep(10);
            
            /*
             * Search for recently updated companies
             */
            Console.WriteLine("* Searching for recently updated companies ...");
            var recentlyUpdated = api.Company
                .RecentlyUpdated<CompanyHubSpotModel>();
            moreResults = true;
            while (moreResults)
            {
                moreResults = recentlyUpdated.MoreResultsAvailable;
                foreach (var recentResult in recentlyUpdated.Companies)
                {
                    Console.WriteLine($"-> Found recently updated company: {recentResult.Name}");
                }
                if (moreResults)
                    recentlyUpdated =
                        api.Company.RecentlyUpdated<CompanyHubSpotModel>(recentlyUpdated.SearchRequestOptions);
            }
            
            /*
             * Get all companies with domain name "squaredup.com"
             */
            var companiesByDomain = api.Company
                .GetByDomain<CompanyHubSpotModel>("squaredup.com");
            moreResults = true;
            while (moreResults)
            {
                moreResults = companiesByDomain.MoreResultsAvailable;
                foreach (var domainSearchResult in companiesByDomain.Companies)
                {
                    Console.WriteLine($"-> Found company with domain: {domainSearchResult.Domain}");
                }

                if (moreResults)
                    companiesByDomain =
                        api.Company.GetByDomain<CompanyHubSpotModel>("squaredup.com",
                            companiesByDomain.SearchRequestOptions);
            }
            
            /*
             * Search for a company by name
             */
            Console.WriteLine($"* Searching for a company by name: {company.Name} ...");
            var searchedCompany = api.Company.Search<CompanyHubSpotModel>(new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                PropertyName = "name",
                                Operator = SearchRequestFilterOperatorType.EqualTo,
                                Value = company.Name
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            });
            foreach (var c in searchedCompany.Companies)
            {
                Console.WriteLine($"-> Found: {c.Name}");
            }
            
            /*
             * Delete a company
             */
            Console.WriteLine($"* Deleting company with ID: {company.Id}");
            api.Company.Delete(company.Id);

            /*
             * Batch create multiple companies and test searching/paging
             */
            var companiesBatch = new CompanyListHubSpotModel<CompanyHubSpotModel>();
            Console.WriteLine("* Batch creating multiple companies to demonstrate search & paging ...");
            for (var i = 1; i <= 33; i++)
            {
                companiesBatch.Companies.Add(new CompanyHubSpotModel()
                {
                    Domain = "squaredup.com",
                    Name = $"Squared Up {i:N0}"
                });
            }
            companiesBatch = api.Company.BatchCreate(companiesBatch);
            
            // Wait for HubSpot to catch up
            Utilities.Sleep(20);
            
            Console.WriteLine($"* Searching for companies containing 'Squared Up' in the name ...");
            
            var reusableSearchFilter = new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                PropertyName = "name",
                                Operator = SearchRequestFilterOperatorType.ContainsAToken,
                                Value = "Squared Up"
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            };
            
            var searchedCompanies = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);
            moreResults = true;
            while (moreResults)
            {
                moreResults = searchedCompanies.MoreResultsAvailable;
                foreach (var searchResult in searchedCompanies.Companies)
                {
                    Console.WriteLine($"-> Found: {searchResult.Name}");
                }
                if (moreResults)
                    searchedCompanies = api.Company.Search<CompanyHubSpotModel>(searchedCompany.SearchRequestOptions);
            }
            
            /*
             * Batch create -or- update multiple companies
             */
            foreach (var c in companiesBatch.Companies)
                c.Name += " UPDATE ME!";
            foreach (var i in Enumerable.Range(34, 66))
            {
                companiesBatch.Companies.Add(new CompanyHubSpotModel()
                {
                    Domain = "squaredup.com",
                    Name = $"Squared Up {i:N0} - NEW COMPANY!"
                });
            }
            
            companiesBatch = api.Company.BatchCreateOrUpdate(companiesBatch);
            Console.WriteLine($"-> {companiesBatch.Total} Companies were created or updated");
            Console.WriteLine($"-> Status: {companiesBatch.Status}");
            
            // Wait for HubSpot to catch up
            Utilities.Sleep(20);
            
            /*
             * List companies (using default search options)
             */
            Console.WriteLine($"* Listing all companies using the default sorting options (sort by createdate, " +
                              $"descending) ...");
            var companiesList = api.Company.List<CompanyHubSpotModel>();
            moreResults = true;
            while (moreResults)
            {
                moreResults = companiesList.MoreResultsAvailable;
                foreach (var listResult in companiesList.Companies)
                    Console.WriteLine($"-> Found: {listResult.Name} - Created: {listResult.CreatedAt}");
                if (moreResults)
                    companiesList = api.Company.List<CompanyHubSpotModel>(companiesList.SearchRequestOptions);
            }
            
            Console.WriteLine($"* Deleting all previously created companies ...");
            reusableSearchFilter.FilterGroups[0].Filters[0].PropertyName = "domain";
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = "squaredup.com";
            searchedCompanies = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);            
            moreResults = true;
            while (moreResults)
            {
                moreResults = searchedCompanies.MoreResultsAvailable;
                foreach (var searchResult in searchedCompanies.Companies)
                {
                    Console.WriteLine($"-> Deleting: {searchResult.Name}");
                    api.Company.Delete(searchResult);
                }
                if (moreResults)
                    searchedCompanies = api.Company.Search<CompanyHubSpotModel>(searchedCompany.SearchRequestOptions);
            }
        }
    }
}
