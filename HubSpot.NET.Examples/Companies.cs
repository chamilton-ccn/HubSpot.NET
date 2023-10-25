using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Company.Dto;
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
             * Creating a reusable search filter
             */
            var reusableSearchFilter = new SearchRequestOptions()
            {
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter()
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "domain", "name", "website"
                }
            };
            
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
            Utilities.Sleep();
            
            /*
             * Search for recently created companies
             */
            Console.WriteLine("* Searching for recently created companies ...");
            
            reusableSearchFilter.FilterGroups[0].Filters[0].PropertyName = "createdate";
            reusableSearchFilter.FilterGroups[0].Filters[0].Operator = SearchRequestFilterOperatorType
                .GreaterThanOrEqualTo;
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = ((DateTimeOffset)DateTime.Today.AddDays(-7))
                .ToUnixTimeMilliseconds().ToString();
            
            var recentlyCreated = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);
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
                        api.Company.Search<CompanyHubSpotModel>(recentlyCreated.SearchRequestOptions);
            }
            
            /*
             * Update a company
             */
            Console.WriteLine("* Updating a company ...");
            company.Description = "Data Visualization for Enterprise IT";
            company = api.Company.Update(company);
            Console.WriteLine($"-> Company description updated: {company.Name} {company.Description}...");
            
            // Wait for HubSpot to catch up
            Utilities.Sleep();
            
            /*
             * Search for recently updated companies
             */
            Console.WriteLine("* Searching for recently updated companies ...");
            
            reusableSearchFilter.FilterGroups[0].Filters[0].PropertyName = "hs_lastmodifieddate";
            reusableSearchFilter.SortBy = "hs_lastmodifieddate";
            
            var recentlyUpdated = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);
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
                        api.Company.Search<CompanyHubSpotModel>(recentlyUpdated.SearchRequestOptions);
            }
            
            /*
             * Get all companies with domain name "squaredup.com"
             */
            Console.WriteLine("* Searching for companies with the domain: 'squaredup.com' ...");
            reusableSearchFilter.FilterGroups[0].Filters[0].PropertyName = "domain";
            reusableSearchFilter.FilterGroups[0].Filters[0].Operator = SearchRequestFilterOperatorType.EqualTo;
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = "squaredup.com";
            
            var companiesByDomain = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);
            
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
                        api.Company.Search<CompanyHubSpotModel>(companiesByDomain.SearchRequestOptions);
            }
            
            /*
             * Search for a company by name
             */
            Console.WriteLine($"* Searching for a company by name: {company.Name} ...");
            
            reusableSearchFilter.FilterGroups[0].Filters[0].PropertyName = "name";
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = company.Name;
            
            var searchedCompany = api.Company.Search<CompanyHubSpotModel>(reusableSearchFilter);
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
            var batchCreateCompanies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
            Console.WriteLine("* Batch creating multiple companies to demonstrate search & paging ...");
            for (var i = 1; i <= 33; i++)
            {
                batchCreateCompanies.Companies.Add(new CompanyHubSpotModel()
                {
                    Domain = "squaredup.com",
                    Name = $"Squared Up {i:N0}"
                });
            }
            batchCreateCompanies = api.Company.BatchCreate(batchCreateCompanies);
            
            // Wait for HubSpot to catch up
            Utilities.Sleep();
            
            Console.WriteLine($"* Searching for companies containing 'Squared Up' in the name ...");

            reusableSearchFilter.FilterGroups[0].Filters[0].Operator = SearchRequestFilterOperatorType.ContainsAToken;
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = "Squared Up";
            
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
             * Batch update companies
             */
            Console.WriteLine($"* Updating a batch of companies ...");
            foreach (var c in batchCreateCompanies.Companies)
                c.Name += " UPDATE ME!";

            var batchUpdateCompanies = api.Company.BatchUpdate(batchCreateCompanies);
            Console.WriteLine($"-> Total: {batchUpdateCompanies.Total}");
            Console.WriteLine($"-> Status: {batchUpdateCompanies.Status}");
            
            // Wait for HubSpot to catch up
            Utilities.Sleep();
            
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
            reusableSearchFilter.FilterGroups[0].Filters[0].Operator = SearchRequestFilterOperatorType.EqualTo;
            reusableSearchFilter.FilterGroups[0].Filters[0].Value = "squaredup.com";
            searchedCompanies = api.Company
                .Search<CompanyHubSpotModel>(reusableSearchFilter);

            var deleteTest = api.Company.BatchArchive(searchedCompanies);
            Console.WriteLine($">>>>>>> {deleteTest.Status}");
            foreach (var shit in deleteTest.Companies)
                Console.WriteLine($"shitty shit shit {shit.Name}");
            
            moreResults = true;
            while (moreResults)
            {
                moreResults = searchedCompanies.MoreResultsAvailable;
                foreach (var searchResult in searchedCompanies.Companies)
                {
                    //Console.WriteLine($"-> Deleting: {searchResult.Name}");
                    //api.Company.Delete(searchResult);
                    if (api.Company.GetByUniqueId<CompanyHubSpotModel>(searchResult.Id) == null)
                        Console.WriteLine($"{searchResult.Name} has been deleted!");
                }
                if (moreResults)
                    searchedCompanies = api.Company.Search<CompanyHubSpotModel>(searchedCompany.SearchRequestOptions);
            }
        }
    }
}
