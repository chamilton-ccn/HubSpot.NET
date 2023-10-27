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
					Domain = $"{i:N0}-communityclosing.com"
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
		
		
		[TestMethod]
		public void BatchCreate_Companies()
		{
			
		}
		
		[TestMethod]
		public void BatchRead_Companies()
		{
			
		}
		
		[TestMethod]
		public void BatchUpdate_Companies()
		{
			
		}
		
		[TestMethod]
		public void List_Companies()
		{
			
		}
		
		[TestMethod]
		public void Create_Company()
		{
			
		}
		
		[TestMethod]
		public void GetByUniqueId_Company()
		{
			
		}
		
		[TestMethod]
		public void Update_Company()
		{
			
		}
		
		[TestMethod]
		public void Delete_Company()
		{
			
		}
		
		[TestMethod]
		public void Search_Companies()
		{
			
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