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
				$"20 company records were expected, but instead we received " +
				$"{batchReadArchivedResult.Companies.Count}");
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
					company.Name = $"{company.Name} UPDATED";
				}
				var batchUpdateResult = companyApi.BatchUpdate(batchCreateResult);
				Utilities.Sleep();
				var batchReadResult = companyApi.BatchRead(batchUpdateResult);
				// Updated company records should have a FirstName property that ends with "-UPDATED".
				Assert.IsTrue(batchReadResult.Companies.All(c => c.Name.Contains("UPDATED")),
					"'Name' property is not what was expected");
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
					company.Name = $"{company.Name} UPDATED";
				companyApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();
				
				// Update (again) our recently updated models so we can generate some more property history.
				foreach (var company in batchCreateResult.Companies)
					company.Name = $"{company.Name} UPDATED2";

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
						$"{company.PropertiesWithHistory.Name.Count}");
				}
				
				/*
				 * 20 company objects were created previously, so we should have no trouble listing 10 of them and there
				 * should be a paging object in the results.
				 */
				searchOptions.Limit = 10;
				var list10Companies = companyApi.List<CompanyHubSpotModel>(searchOptions);
				Assert.AreEqual(10, list10Companies.Companies.Count,
					$"10 companies were expected, instead we received: {list10Companies.Companies.Count}");
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
		
		// TODO - [XML Documentation] Update_Company
		[TestMethod]
		public void Update_Company()
		{
			// TODO - [TEST] Update_Company
		}
		
		// TODO - [XML Documentation] Delete_Company 
		[TestMethod]
		public void Delete_Company()
		{
			// TODO - [TEST] Delete_Company 
		}
		
		// TODO - [XML Documentation] Search_Companies
		[TestMethod]
		public void Search_Companies()
		{
			// TODO - [TEST] Search_Companies
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