using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core.Search;
using HubSpot.NET.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class CompanyTests
	{
		/// <summary>
		/// Test BatchArchive operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchRead archived/unarchived operations.
		/// </remarks>
		[TestMethod]
		public void BatchArchive_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();
			// Created company records should have an Id property that is a long type.
			Assert.IsFalse(batchCreateResult.Companies
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
				"Found company records with invalid id properties");
			var batchArchiveResult = companyApi.BatchArchive(batchCreateResult);
			Utilities.Sleep();
			// Archived company records should have a Status of "ARCHIVED"
			Assert.IsTrue(batchArchiveResult.Status == "ARCHIVED",
				"Status is not 'ARCHIVED'"); // TODO - Should this be an enum?
			var batchReadResult = companyApi.BatchRead(batchArchiveResult);
			// Companies should be empty after an archive operation.
			Assert.AreEqual(0, batchReadResult.Companies.Count,
				"'Companies' should be empty");
			// But if we set "Archived = true" in our SearchRequestOptions object ...
			batchArchiveResult.SearchRequestOptions.Archived = true;
			// ... then request the batch again ...
			var batchReadArchivedResult = companyApi.BatchRead(batchArchiveResult);
			// ... we should have 20 "archived" company records.
			Assert.AreEqual(20, batchReadArchivedResult.Companies.Count,
				$"Unexpected number of companies: {batchReadArchivedResult.Companies.Count}; expected: 20");
		}
		
		/// <summary>
		/// Test BatchCreate operations.
		/// </summary>
		[TestMethod]
		public void BatchCreate_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();
			
			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
			}
			finally
			{
				companyApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Test BatchRead operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchCreate operations.
		/// </remarks>
		[TestMethod]
		public void BatchRead_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();
			
			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
				var batchReadResult = companyApi.BatchRead(batchCreateResult);
				
				// Retrieved company records should have an Id property that is a long type.
				Assert.IsFalse(batchReadResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");

				/*
				 * Even though HubSpot says domain names are the "primary unique identifier" for companies:
				 * https://tinyurl.com/3xnkkxr6 You still *cannot* retrieve a company by its domain name:
				 * https://tinyurl.com/ytr95847 Until this changes (assuming that it will ever change) the test below
				 * should pass (i.e., reading a batch of companies using the domain name as the unique id will return
				 * no records. Why test for this? Well, it feels like this is something that should eventually be
				 * rectified; this test will keep an eye on that :-)
				 */
				foreach (var company in batchReadResult.Companies)
				{
					company.Id = company.Domain;
				}
				var searchOptions = new SearchRequestOptions
				{
					IdProperty = "domain",
					PropertiesToInclude = new List<string>
					{
						"name",
						"phone",
						"domain"
					}
				};
				batchReadResult = companyApi.BatchRead(batchReadResult, searchOptions);
				Assert.AreEqual(0, batchReadResult.Companies.Count);
			}
			finally
			{
				companyApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Test BatchUpdate operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchCreate and BatchRead operations. 
		/// </remarks>
		[TestMethod]
		public void BatchUpdate_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();

			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
				foreach (var company in batchCreateResult.Companies)
				{
					company.Name = $"{company.Name} (UPDATED)";
				}
				var batchUpdateResult = companyApi.BatchUpdate(batchCreateResult);
				Utilities.Sleep();
				var batchReadResult = companyApi.BatchRead(batchUpdateResult);
				// Updated company records should have a Name property that contains "(UPDATED)".
				Assert.IsTrue(batchReadResult.Companies.All(c => c.Name.Contains("(UPDATED)")),
					$"'Name' property does not end with '(UPDATED)'");
			}
			finally
			{
				companyApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Test List operations.
		/// </summary>
		/// <remarks>
		/// Also tests company property history and SearchRequestOptions (Limit, PropertiesWithHistory).
		/// </remarks>
		[TestMethod]
		public void List_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();

			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
				
				// Update our newly-created models so we can generate some property history.
				foreach (var company in batchCreateResult.Companies)
					company.Name = $"{company.Name} (UPDATED)";
				companyApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();
				
				// Update (again) our recently updated models so we can generate some more property history.
				foreach (var company in batchCreateResult.Companies)
					company.Name = $"{company.Name} (UPDATED AGAIN)";

				companyApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();

				var searchOptions = new SearchRequestOptions
				{
					PropertiesToInclude = new List<string>
					{
						"name",
						"phone",
						"domain"
					},
					PropertiesWithHistory = new List<string>
					{
						"name"
					}
				};
				
				batchCreateResult.SearchRequestOptions = searchOptions;

				/*
				 * By this point, every company object in batchCreateResult should have had its Name property
				 * updated twice, so there should be three property history items (created, updated, updated again) for
				 * the Name property of each company object in the list.
				 */
				foreach (var company in companyApi.BatchRead(batchCreateResult, searchOptions).Companies)
				{
					Assert.IsTrue(company.PropertiesWithHistory.Name.Count == 3,
						$"Unexpected number of 'Name' property history items: " +
						$"{company.PropertiesWithHistory.Name.Count}; expected: 3");
				}
				
				/*
				 * 20 company objects were created previously, so we should have no trouble listing 10 of them and there
				 * should be a paging object in the results.
				 */
				searchOptions.Limit = 10;
				var list10Companies = companyApi.List<CompanyHubSpotModel>(searchOptions);
				Assert.AreEqual(10, list10Companies.Companies.Count,
					$"Unexpected number of companies: {list10Companies.Companies.Count}; expected: 10");
				Assert.IsNotNull(list10Companies.Paging, "Paging object was null");
				Assert.IsTrue(list10Companies.MoreResultsAvailable, "More results were expected");
				
				/*
				 * Under any circumstances, if we attempt to set a per-request limit higher than 100, an
				 * ArgumentException will be thrown.
				 */
				Assert.ThrowsException<ArgumentException>(() => searchOptions.Limit = 101, 
					"Setting the limit > 100 should throw an ArgumentException");
				
				/*
				 * Testing the default value for Limit: If the per-request limit is 0 (default/undefined), and
				 * PropertiesWithHistory is populated, the limit will be 50.
				 */
				searchOptions.Limit = 0;
				Assert.AreEqual(50, searchOptions.Limit,
					"If there is no limit specified (or if it is 0), and 'PropertiesWithHistory' is not " +
					"empty, limit should default to 50");
				
				/*
				 * Testing the default value for Limit: If the per-request limit is 0 (default/undefined), and
				 * PropertiesWithHistory is not populated, the limit will be 100.
				 */
				searchOptions.PropertiesWithHistory.Clear();
				Assert.AreEqual(100, searchOptions.Limit,
					"If there is no limit specified (or if it is 0), and 'PropertiesWithHistory' is " +
					"empty, limit should default to 100");
			}
			finally
			{
				companyApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Test Create operations.
		/// </summary>
		[TestMethod]
		public void Create_Company()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var company = companyApi.Create(new CompanyHubSpotModel
			{
				Name = $"Community Closing Network, LLC",
				Phone = "3018675309",
				Domain = $"communityclosing.com"
			});

			try
			{
				// Created  company records should have an Id property that is a long type.
				Assert.IsFalse((company.Id is null | company.Id == 0L) | !(company.Id is long),
					"Found company records with invalid id properties");
			}
			finally
			{
				Utilities.Sleep();
				companyApi.Delete(company);
			}
		}
		
		/// <summary>
		/// Test GetByUniqueId operations.
		/// </summary>
		/// <remarks>
		/// Also tests retrieval of archived company records.
		/// </remarks>
		[TestMethod]
		public void GetByUniqueId_Company()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var company = companyApi.Create(new CompanyHubSpotModel
			{
				Name = "Community Closing Network, LLC",
				Phone = "3018675309",
				Domain = "communityclosing.com"
			});
			
			// We're going to keep a record of all created companies to we can ensure they're removed later.
			var createdCompanies = new CompanyListHubSpotModel<CompanyHubSpotModel>
			{
				Companies = new List<CompanyHubSpotModel> { company }
			};

			Utilities.Sleep();

			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse((company.Id is null | company.Id == 0L) | !(company.Id is long),
					"Found company records with invalid id properties");
				
				/*
				 * At the time of this writing, the only truly unique, non-custom property that can be used to retrieve
				 * company objects is the "id" attribute, but we'll set up this test to be easily modifiable should
				 * HubSpot ever add another unique, non-custom property to the company object. If and when HubSpot ever
				 * makes the "domain" property truly unique, see the GetByUniqueId_Company test for an example of how
				 * this test should be modified.
				 */
				var getCompanyById = companyApi
					.GetByUniqueId<CompanyHubSpotModel>(company.Id);
				Assert.AreEqual(company.Id, getCompanyById.Id, 
					$"Unexpected value for 'Id': {getCompanyById.Id}; expected: '{company.Id}'");
				
				/*
				 * Next we'll create and delete a couple of company objects to demonstrate retrieving archived records
				 * by id.
				 */
				var oldCompanyId = company.Id;
				companyApi.Delete(company);
				Utilities.Sleep();
				company = companyApi.Create(company);
				createdCompanies.Companies.Add(company);
				Assert.AreNotEqual(oldCompanyId, company.Id,
					"Old company id and new company id are the same but they shouldn't be");

				oldCompanyId = company.Id;
				companyApi.Delete(company);
				Utilities.Sleep();
				company = companyApi.Create(company);
				createdCompanies.Companies.Add(company);
				Assert.AreNotEqual(oldCompanyId, company.Id,
					"Old company id and new company id are the same but they shouldn't be");
				
				var searchOptions = new SearchRequestOptions
				{
					Archived = true
				};

				var getArchivedCompanyById = companyApi
					.GetByUniqueId<CompanyHubSpotModel>(company.Id, searchOptions);
				Assert.AreEqual(company.Id, getArchivedCompanyById.Id,
					"Company ids do not match");

				var getArchivedCompaniesById = companyApi
					.BatchRead(createdCompanies, searchOptions);
				Assert.IsFalse(getArchivedCompaniesById.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
			}
			finally
			{
				companyApi.BatchArchive(createdCompanies);
			}
		}
		
		/// <summary>
		/// Test Update operations.
		/// </summary>
		[TestMethod]
		public void Update_Company()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var company = companyApi.Create(new CompanyHubSpotModel
			{
				Name = "Community Closing Network, LLC",
				Phone = "3018675309",
				Domain = "communityclosing.com"
			});
			
			Utilities.Sleep();

			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse((company.Id is null | company.Id == 0L) | !(company.Id is long),
					"Found company records with invalid id properties");

				company.Name = $"{company.Name} (UPDATED)";
				company.Phone = "2408675309";
				company.Domain = $"UPDATED-{company.Domain}";

				company = companyApi.Update(company);
				
				/*
				 * Updated company properties should match what we changed them to.
				 */
				Assert.AreEqual("Community Closing Network, LLC (UPDATED)", company.Name,
					$"Unexpected value for 'Name': '{company.Name}'; expected: 'Community Closing Network, " +
					$"LLC (UPDATED)'");
				Assert.AreEqual("2408675309", company.Phone,
					$"Unexpected value for 'Phone': '{company.Phone}'; expected: '2408675309'");
				Assert.AreEqual("UPDATED-communityclosing.com", company.Domain,
					$"Unexpected value for 'Domain': {company.Domain}; expected: 'UPDATED-communityclosing.com'");
			}
			finally
			{
				companyApi.Delete(company);
			}
			
		}
		
		/// <summary>
		/// Test Delete (archive) operations.
		/// </summary>
		/// <remarks>
		/// It might feel a bit silly at this point, since we've created and deleted company records multiple times
		/// already, but for completeness sake, here's the test :-)
		/// </remarks>
		[TestMethod]
		public void Delete_Company()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var company = companyApi.Create(new CompanyHubSpotModel
			{
				Name = "Community Closing Network, LLC",
				Phone = "3018675309",
				Domain = "communityclosing.com"
			});
			
			// Created company records should have an Id property that is a long type.
			Assert.IsFalse((company.Id is null | company.Id == 0L) | !(company.Id is long),
				"Found company records with invalid id properties");
			
			Utilities.Sleep(5);
			
			// Delete the company
			companyApi.Delete(company);
			
			// Attempt to retrieve the company; the result will be null because we did not request archived records.
			var shouldBeNull = companyApi.GetByUniqueId<CompanyHubSpotModel>(company.Id);
			Assert.IsNull(shouldBeNull, "Retrieving a deleted (archived) company by id without specifying " +
			                            "'Archived = true' should return null");
			
			// Attempt to retrieve the company; this time around we're explicitly requesting archived records.
			var deletedCompany = companyApi
				.GetByUniqueId<CompanyHubSpotModel>(company.Id, new SearchRequestOptions { Archived = true });
			Assert.AreEqual(company.Id, deletedCompany.Id,
				$"Unexpected value for 'Id': '{company.Id}'; expected: '{deletedCompany.Id}'");
		}
		
		/// <summary>
		/// Test Search operations.
		/// </summary>
		[TestMethod]
		public void Search_Companies()
		{
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var companies = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				companies.Companies.Add(new CompanyHubSpotModel
				{
					Name = $"{i:N0} Community Closing Network, LLC ({timestamp})",
					Phone = "3018675309",
					Domain = $"{i:N0}-{timestamp}-communityclosing.com"
				});
			}
			var batchCreateResult = companyApi.BatchCreate(companies);
			Utilities.Sleep();

			try
			{
				// Created company records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Companies
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found company records with invalid id properties");
				
				batchCreateResult.SearchRequestOptions.FilterGroups.Add(new SearchRequestFilterGroup());
				
				batchCreateResult.SearchRequestOptions.FilterGroups[0].Filters.Add(new SearchRequestFilter
				{
					Operator = SearchRequestFilterOperatorType.GreaterThanOrEqualTo,
					Value = timestamp,
					PropertyName = "createdate"
				});
				
				batchCreateResult.SearchRequestOptions.FilterGroups[0].Filters.Add(new SearchRequestFilter
				{
					Operator = SearchRequestFilterOperatorType.ContainsAToken,
					Value = $"-{timestamp}-communityclosing.com",
					PropertyName = "domain"
				});
				
				batchCreateResult.SearchRequestOptions.FilterGroups[0].Filters.Add(new SearchRequestFilter
				{
					Operator = SearchRequestFilterOperatorType.ContainsAToken,
					Value = $"Community Closing Network, LLC ({timestamp})",
					PropertyName = "name"
				});
				
				batchCreateResult.SearchRequestOptions.Limit = 5;

				var searchResult = companyApi
					.Search<CompanyHubSpotModel>(batchCreateResult.SearchRequestOptions);
				
				// We should only have 5 records ...
				Assert.AreEqual(5, searchResult.Companies.Count, 
					$"Unexpected number of companies: {searchResult.Companies.Count}; expected: 5");
				
				// ... But there should be a total of 20 records
				Assert.AreEqual(20, searchResult.Total,
					$"Unexpected number of companies: {searchResult.Total}; expected: 20");
				
				// ... And more results should be available
				Assert.IsTrue(searchResult.MoreResultsAvailable, 
					$"Invalid value for 'MoreResultsAvailable': '{searchResult.MoreResultsAvailable}'; " +
					$"expected: 'true'");
				
				/*
				 * By default, searches are sorted by 'createdate', 'descending', so if we reverse the sort direction,
				 * and search again, the previous first and last records should now be equal to the last and first
				 * records in the list, respectively. But there's a catch! it's possible, even likely in this scenario
				 * for multiple company records to share the same 'createdate' value, which makes it impossible to
				 * reliably sort by createdate, so we'll switch to sorting by 'id' instead. But there's another catch!
				 * 'id' isn't actually a property that you can sort by, however 'hs_object_id' is, so we'll use that.
				 */
				searchResult.SearchRequestOptions.Offset = null;
				searchResult.SearchRequestOptions.Limit = 20;
				searchResult.SearchRequestOptions.SortBy = "hs_object_id";
				searchResult = companyApi.Search<CompanyHubSpotModel>(searchResult.SearchRequestOptions);
				
				var firstRecordId = searchResult.Companies.First().Id;
				var lastRecordId = searchResult.Companies.Last().Id;
				
				searchResult.SearchRequestOptions.SortDirection = SearchRequestSortType.Ascending;
				searchResult = companyApi.Search<CompanyHubSpotModel>(searchResult.SearchRequestOptions);
				Assert.IsTrue(lastRecordId == searchResult.Companies.First().Id, 
					$"Unexpected 'id' value for the first company in the list: '{searchResult.Companies.First().Id}'; " +
					$"expected: '{lastRecordId}'");
				Assert.IsTrue((firstRecordId == searchResult.Companies.Last().Id), 
					$"Unexpected 'id' value for the last company in the list: '{searchResult.Companies.Last().Id}; " +
					$"expected: '{firstRecordId}''");
				
				/*
				 * Searches are allowed up to three filter groups, each containing three filters maximum
				 * See: https://developers.hubspot.com/docs/api/crm/search#filter-search-results for more details.
				 */
				batchCreateResult.SearchRequestOptions.FilterGroups.Add(new SearchRequestFilterGroup());
				batchCreateResult.SearchRequestOptions.FilterGroups.Add(new SearchRequestFilterGroup());
				
				Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
						batchCreateResult.SearchRequestOptions.FilterGroups.Add(new SearchRequestFilterGroup()),
					"FilterGroups is allowing more than 3 groups when it should be limited to 3");
				
				Assert.ThrowsException<ArgumentOutOfRangeException>(() => batchCreateResult
						.SearchRequestOptions.FilterGroups[0].Filters.Add(new SearchRequestFilter()),
					"The maximum number of filters should be 3");
			}
			finally
			{
				companyApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Tests the default values of properties for CompanyHubSpotModel and CompanyListHubSpotModel.
		/// </summary>
		[TestMethod]
		public void Default_Property_Values()
		{
			var companyHubSpotModel = new CompanyHubSpotModel();
			
			Assert.IsNull(companyHubSpotModel.Id,
				$"Unexpected value for 'Id': '{companyHubSpotModel.Id}'; expected: 'null'");
			Assert.IsTrue(companyHubSpotModel.SerializeProperties,
				$"Unexpected value for 'SerializeProperties': '{companyHubSpotModel.SerializeProperties}'; " +
				$"expected: 'true'");
			var shouldSerializeProperties = companyHubSpotModel.ShouldSerializeProperties(); 
			Assert.IsTrue(shouldSerializeProperties,
				$"Unexpected value for 'ShouldSerializeProperties': '{shouldSerializeProperties}'; " +
				$"expected: 'true'");
			Assert.AreEqual(0, companyHubSpotModel.Associations.Count,
				$"Unexpected number of items in 'Associations': {companyHubSpotModel.Associations.Count}; " +
				$"expected: 0");
			Assert.IsNull(companyHubSpotModel.SerializeAssociations,
				$"Unexpected value for 'SerializeAssociations': '{companyHubSpotModel.Associations}'; " +
				$"expected: 'null'");
			var shouldSerializeAssociations = companyHubSpotModel.ShouldSerializeAssociations(); 
			Assert.IsFalse(shouldSerializeAssociations,
				$"Unexpected value for 'ShouldSerializeAssociations': " +
				$"'{shouldSerializeAssociations}'; expected: 'false'");
			Assert.IsNull(companyHubSpotModel.Name,
				$"Unexpected value for 'Name': '{companyHubSpotModel.Name}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.Domain,
				$"Unexpected value for 'Domain': '{companyHubSpotModel.Domain}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.Website,
				$"Unexpected value for 'Website': '{companyHubSpotModel.Website}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.Description,
				$"Unexpected value for 'Description': '{companyHubSpotModel.Description}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.About,
				$"Unexpected value for 'About': '{companyHubSpotModel.About}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.City,
				$"Unexpected value for 'City': '{companyHubSpotModel.City}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.State,
				$"Unexpected value for 'State': '{companyHubSpotModel.State}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.ZipCode,
				$"Unexpected value for 'ZipCode': '{companyHubSpotModel.ZipCode}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.Country,
				$"Unexpected value for 'Country': '{companyHubSpotModel.Country}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.PropertiesWithHistory,
				$"Unexpected value for 'PropertiesWithHistory': '{companyHubSpotModel.PropertiesWithHistory}'; " +
				$"expected: 'null'");
			Assert.IsNull(companyHubSpotModel.CreatedAt,
				$"Unexpected value for 'CreatedAt': '{companyHubSpotModel.CreatedAt}'; expected: 'null'");
			Assert.IsNull(companyHubSpotModel.UpdatedAt,
				$"Unexpected value for 'UpdatedAt': '{companyHubSpotModel.UpdatedAt}'; expected: 'null'");
			Assert.AreEqual("companies", companyHubSpotModel.HubSpotObjectType,
				$"Unexpected value for 'HubSpotObjectType': '{companyHubSpotModel.HubSpotObjectType}'; " +
				$"expected: 'companies'");
			Assert.AreEqual("/crm/v3/objects/companies",companyHubSpotModel.RouteBasePath,
				$"Unexpected value for 'RouteBasePath': '{companyHubSpotModel.RouteBasePath}'; " +
				$"expected: '/crm/v3/objects/companies'");
			
			var companyListHubSpotModel = new CompanyListHubSpotModel<CompanyHubSpotModel>();
			
			Assert.AreEqual(0, companyListHubSpotModel.Companies.Count,
				$"Unexpected number of items in 'Companies': {companyListHubSpotModel.Companies.Count}; " +
				$"expected: 0");
			Assert.AreEqual(0, companyListHubSpotModel.Inputs.Count,
				$"Unexpected number of items in 'Inputs': {companyListHubSpotModel.Inputs.Count}; expected: 0");
			Assert.AreEqual(0, companyListHubSpotModel.Results.Count,
				$"Unexpected number of items in 'Results': '{companyListHubSpotModel.Results.Count}'; expected: 0");
			var shouldSerializeResults = companyListHubSpotModel.ShouldSerializeResults();
			Assert.IsFalse(shouldSerializeResults,
				$"Unexpected value for 'ShouldSerializeResults': '{shouldSerializeResults}'; expected: 'false'");
			Assert.IsNull(companyListHubSpotModel.Status,
				$"Unexpected value for 'Status': '{companyListHubSpotModel.Status}'; expected: 'null'");
			Assert.IsNull(companyListHubSpotModel.Total,
				$"Unexpected value for 'Total': '{companyListHubSpotModel.Total}'; expected: 'null'");
			Assert.IsNull(companyListHubSpotModel.TotalErrors,
				$"Unexpected value for 'TotalErrors': '{companyListHubSpotModel.TotalErrors}'; " +
				$"expected: 'null'");
			Assert.AreEqual(0, companyListHubSpotModel.Errors.Count,
				$"Unexpected number of items in 'Errors': {companyListHubSpotModel.Errors.Count}; expected: 0");
			var shouldSerializeErrors = companyListHubSpotModel.ShouldSerializeErrors();
			Assert.IsFalse(shouldSerializeErrors,
				$"Unexpected value for 'ShouldSerializeErrors': '{shouldSerializeErrors}'; expected: 'false'");
			Assert.IsNull(companyListHubSpotModel.RequestedAt,
				$"Unexpected value for 'RequestedAt': '{companyListHubSpotModel.RequestedAt}'; expected: 'null'");
			Assert.IsNull(companyListHubSpotModel.StartedAt,
				$"Unexpected value for 'StartedAt': '{companyListHubSpotModel.StartedAt}'; expected: 'null'");
			Assert.IsNull(companyListHubSpotModel.CompletedAt,
				$"Unexpected value for 'CompletedAt': '{companyListHubSpotModel.CompletedAt}'; expected: 'null'");
			Assert.IsFalse(companyListHubSpotModel.MoreResultsAvailable,
				$"Unexpected value for 'MoreResultsAvailable': '{companyListHubSpotModel.MoreResultsAvailable}'; " +
				$"expected: 'false'");
			Assert.IsNull(companyListHubSpotModel.Offset,
				$"Unexpected value for 'Offset': '{companyListHubSpotModel.Offset}'; expected: 'null'");
			Assert.IsNull(companyListHubSpotModel.Paging,
				$"Unexpected value for 'Paging': '{companyListHubSpotModel.Paging}'; expected: 'null'");
			// TODO: SearchRequestOptions is not tested here; instead it will be tested in Search unit tests
			Assert.IsNull(companyListHubSpotModel.IdProperty,
				$"Unexpected value for 'IdProperty': '{companyListHubSpotModel.IdProperty}'; expected: 'null'");
			Assert.AreEqual(0, companyListHubSpotModel.PropertiesWithHistory.Count,
				$"Unexpected number of items 'PropertiesWithHistory': " +
				$"{companyListHubSpotModel.PropertiesWithHistory.Count}; expected: 0");
			Assert.AreEqual("companies", companyListHubSpotModel.HubSpotObjectType,
				$"Unexpected value for 'HubSpotObjectType': '{companyListHubSpotModel.HubSpotObjectType}'; " +
				$"expected: 'companies'");
			Assert.AreEqual("/crm/v3/objects/companies",companyListHubSpotModel.RouteBasePath,
				$"Unexpected value for 'RouteBasePath': '{companyListHubSpotModel.RouteBasePath}'; " +
				$"expected: '/crm/v3/objects/companies'");
		}



		// ### ORIGINAL TESTS FOLLOW ### //
		/*private object apiCompany;

		[TestMethod]
		public void Create_SampleDetails_IdProeprtyIsSet()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Created Company",
				Domain = "sampledomain.com"
			};

			// Act
			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			try
			{
				// Assert
				Assert.IsNotNull(company.Id, "The Id was not set and returned.");
				Assert.AreEqual(sampleCompany.Domain, company.Domain);
				Assert.AreEqual(sampleCompany.Name, company.Name);
			}
			finally
			{
				// Clean-up
				companyApi.Delete(company.Id);
			}
		}

		[TestMethod]
		public void Update_SampleDetails_PropertiesAreUpdated()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Updated Company",
				Domain = "sampledomain.com"
			};

			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			company.Domain = "sampledomain2.com";
			company.Name = "Updated Company #1";

			// Act
			companyApi.Update(company);

			try
			{
				// Assert
				Assert.AreNotEqual(sampleCompany.Domain, company.Domain);
				Assert.AreNotEqual(sampleCompany.Name, company.Name);
				Assert.AreEqual("sampledomain2.com", company.Domain);
				Assert.AreEqual("Updated Company #1", company.Name);

				// Second Act
				company = companyApi.GetByUniqueId<CompanyHubSpotModel>(company.Id);

				// Second Assert
				Assert.AreNotEqual(sampleCompany.Domain, company.Domain);
				Assert.AreNotEqual(sampleCompany.Name, company.Name);
				Assert.AreEqual("sampledomain2.com", company.Domain);
				Assert.AreEqual("Updated Company #1", company.Name);
			}
			finally
			{
				// Clean-up
				companyApi.Delete(company.Id);
			}
		}

		[TestMethod]
		public void Delete_SampleCompany_CompanyIsDeleted()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			var sampleCompany = new CompanyHubSpotModel
			{
				Name = "New Deleted Company",
				Domain = "sampledomain.com"
			};

			CompanyHubSpotModel company = companyApi.Create(sampleCompany);

			// Act
			companyApi.Delete(company.Id);

			// Assert
			company = companyApi.GetByUniqueId<CompanyHubSpotModel>(company.Id);
			Assert.IsNull(company, "The company was searchable and not deleted.");
		}

		[TestMethod]
		public void GetByDomain_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			IList<CompanyHubSpotModel> sampleCompanys = new List<CompanyHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var company = new CompanyHubSpotModel()
				{
					Name = $"New Sample Company #{i:N0}",
					Domain = $"sample{i:N0}domain.com"
				};

				if (i % 2 == 1)
					company.Domain = "sampledomain.com";

				company = companyApi.Create(company);

				sampleCompanys.Add(company);
			}

			try
			{
				var searchOptions = new SearchRequestOptions()
				{
					Limit = 2
				};

				// Act
				var domainSearch = new SearchRequestOptions();
				domainSearch.FilterGroups[0].Filters[0] = new SearchRequestFilter
				{
					Value = "sampledomain.com",
					PropertyName = "domain",
					Operator = SearchRequestFilterOperatorType.EqualTo
				};
				var results = companyApi.Search<CompanyHubSpotModel>(domainSearch);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(2, results.Results.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNotNull(results.Offset);
				//TODO - take a closer look at this.
				//Assert.AreNotEqual(0, results.ContinuationOffset.CompanyId); 

				// Second Act
				searchOptions.Offset = results.Offset;
				var results2 = companyApi.Search<CompanyHubSpotModel>(domainSearch);

				Assert.IsFalse(results2.MoreResultsAvailable, "Did not identify at the end of results.");
				Assert.AreEqual(1, results2.Results.Count, "Did not return 1 of the 5 results.");
				Assert.AreEqual(false, results2.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleCompanys.Count; i++)
				{
					companyApi.Delete(sampleCompanys[i].Id);
				}
			}
		}

		[TestMethod]
		public void Search_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var companyApi = new HubSpotCompanyApi(TestSetUp.Client);
			IList<CompanyHubSpotModel> sampleCompanys = new List<CompanyHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var company = new CompanyHubSpotModel()
				{
					Name = $"New Sample Company #{i:N0}",
					Domain = $"sample{i:N0}domain.com",
					Website = $"http://www.sample{i:N0}domain.com"
				};

				if (i % 2 == 0)
					company.Name = $"Something Else #{i:N0}";

				company = companyApi.Create(company);

				sampleCompanys.Add(company);
			}

			// HubSpot is rather slow to update... wait 15 seconds to allow it to catch up
			System.Threading.Thread.Sleep(15 * 1000);

			try
			{
				string searchValue = "New Sample Company";
				SearchRequestFilterOperatorType searchOperator = SearchRequestFilterOperatorType.ContainsAToken;
				var searchOptions = new SearchRequestOptions
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
									Operator = searchOperator,
									Value = searchValue
								}
							}
						}
					},
					PropertiesToInclude = new List<string>
					{
						"domain", "name", "website"
					},
					Limit = 2
				};

				// Act
				CompanyListHubSpotModel<CompanyHubSpotModel> results = companyApi.Search<CompanyHubSpotModel>(searchOptions);

				// Assert
				Assert.AreEqual(2, results.Results.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNotNull(results.Paging);
				Assert.IsNotNull(results.Paging.Next);
				Assert.IsFalse(string.IsNullOrWhiteSpace(results.Paging.Next.After.ToString()), "Paging did not deserialize correctly");
				Assert.AreEqual(2, results.Paging.Next.After);

				// Second Act
				// TODO - fixme (Cannot convert source type 'string' to target type 'System.Nullable<long>')
				searchOptions.Offset = results.Paging.Next.After; 
				var results2 = companyApi.Search<CompanyHubSpotModel>(searchOptions);

				Assert.AreEqual(1, results2.Results.Count, "Did not return 1 of the 5 results.");
				Assert.AreEqual(false, results2.Results.Any(c => string.IsNullOrWhiteSpace(c.Name)), "Some companies do not have a name.");
				Assert.IsNull(results2.Paging);
			}
			finally
			{
				// Clean-up
				for (int i = 0; i < sampleCompanys.Count; i++)
				{
					companyApi.Delete(sampleCompanys[i].Id);
				}
			}
		}*/
	}
}