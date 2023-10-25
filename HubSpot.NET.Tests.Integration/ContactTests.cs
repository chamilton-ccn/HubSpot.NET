using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core.Search;
using HubSpot.NET.Core.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HubSpot.NET.Tests.Integration
{
	[TestClass]
	public class ContactTests
	{
		/// <summary>
		/// Test BatchArchive operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchRead, archived/un-archived operations.
		/// </remarks>
		[TestMethod]
		public void BatchArchive_Contacts()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contacts = new ContactListHubSpotModel<ContactHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				contacts.Contacts.Add(new ContactHubSpotModel
				{
					FirstName = $"{i:N0} Testy",
					LastName = $"Testerson {timestamp}",
					Email = $"{i:N0}-test@communityclosing.com",
					Phone = "3018675309",
					Company = $"Community Closing Network, LLC"
				});
			}
			var batchCreateResult = contactApi.BatchCreate(contacts);
			Utilities.Sleep();
			// Created contact records should have an Id property that is a long type.
			Assert.IsFalse(batchCreateResult.Contacts
				.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
			var batchArchiveResult = contactApi.BatchArchive(batchCreateResult);
			Utilities.Sleep();
			// Archived contact records should have a Status of "ARCHIVED"
			Assert.IsTrue(batchArchiveResult.Status == "ARCHIVED"); // TODO - Should this be an enum?
			var batchReadResult = contactApi.BatchRead(batchArchiveResult);
			// Contacts should be empty after an archive operation.
			Assert.AreEqual(0, batchReadResult.Contacts.Count);
			// But if we set "Archived = true" in our SearchRequestOptions object ...
			batchArchiveResult.SearchRequestOptions.Archived = true;
			// ... then request the batch again ...
			var batchReadArchivedResult = contactApi.BatchRead(batchArchiveResult);
			// ... we should have 20 "archived" contact records.
			Assert.AreEqual(20, batchReadArchivedResult.Contacts.Count);
		}
		
		/// <summary>
		/// Test BatchCreate operations.
		/// </summary>
		[TestMethod]
		public void BatchCreate_Contacts()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contacts = new ContactListHubSpotModel<ContactHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				contacts.Contacts.Add(new ContactHubSpotModel
				{
					FirstName = $"{i:N0} Testy",
					LastName = $"Testerson {timestamp}",
					Email = $"{i:N0}-test@communityclosing.com",
					Phone = "3018675309",
					Company = $"Community Closing Network, LLC"
				});
			}
			var batchCreateResult = contactApi.BatchCreate(contacts);
			Utilities.Sleep();
			try
			{
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
			}
			finally
			{
				contactApi.BatchArchive(batchCreateResult);
			}
		}
		
		/// <summary>
		/// Test BatchRead operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchCreate and BatchRead operations.
		/// </remarks>
		[TestMethod]
		public void BatchRead_Contacts()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contacts = new ContactListHubSpotModel<ContactHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				contacts.Contacts.Add(new ContactHubSpotModel
				{
					FirstName = $"{i:N0} Testy",
					LastName = $"Testerson {timestamp}",
					Email = $"{i:N0}-test@communityclosing.com",
					Phone = "3018675309",
					Company = $"Community Closing Network, LLC"
				});
			}
			var batchCreateResult = contactApi.BatchCreate(contacts);
			Utilities.Sleep();
			try
			{
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
				var batchReadResult = contactApi.BatchRead(batchCreateResult);
				// Retrieved contact records should have an Id property that is a long type.
				Assert.IsFalse(batchReadResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
				
				foreach (var contact in batchReadResult.Contacts)
				{
					contact.Id = contact.Email;
				}
				var searchOptions = new SearchRequestOptions
				{
					IdProperty = "email"
				};
				batchReadResult = contactApi.BatchRead(batchReadResult, searchOptions);
				// Retrieved contact records should have an Id property that is a long type.
				Assert.IsFalse(batchReadResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L | !(c.Id is long))));
			}
			finally
			{
				contactApi.BatchArchive(batchCreateResult);
			}
		}

		/// <summary>
		/// Test BatchUpdate operations.
		/// </summary>
		/// <remarks>
		/// Also tests BatchCreate and BatchRead operations. 
		/// </remarks>
		[TestMethod]
		public void BatchUpdate_Contacts()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contacts = new ContactListHubSpotModel<ContactHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				contacts.Contacts.Add(new ContactHubSpotModel
				{
					FirstName = $"{i:N0} Testy",
					LastName = $"Testerson {timestamp}",
					Email = $"{i:N0}-test@communityclosing.com",
					Phone = "3018675309",
					Company = $"Community Closing Network, LLC"
				});
			}
			var batchCreateResult = contactApi.BatchCreate(contacts);
			Utilities.Sleep();

			try
			{
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName}-UPDATED";
				var batchUpdateResult = contactApi.BatchUpdate(batchCreateResult);
				Utilities.Sleep();
				var batchReadResult = contactApi.BatchRead(batchUpdateResult);
				// Updated contact records should have a FirstName property that ends with "-UPDATED".
				Assert.IsTrue(batchReadResult.Contacts.All(c => c.FirstName.Contains("-UPDATED")));
			}
			finally
			{
				contactApi.BatchArchive(batchCreateResult);
			}
		}

		
		[TestMethod]
		public void List_Contacts()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contacts = new ContactListHubSpotModel<ContactHubSpotModel>();
			var timestamp = ((DateTimeOffset)DateTime.Today).ToUnixTimeMilliseconds().ToString();
			foreach (var i in Enumerable.Range(1, 20))
			{
				contacts.Contacts.Add(new ContactHubSpotModel
				{
					FirstName = $"{i:N0} Testy",
					LastName = $"Testerson {timestamp}",
					Email = $"{i:N0}-test@communityclosing.com",
					Phone = "3018675309",
					Company = $"Community Closing Network, LLC"
				});
			}
			var batchCreateResult = contactApi.BatchCreate(contacts);
			Utilities.Sleep();

			try
			{
				// TODO - INCOMPLETE
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)));
				
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName}-UPDATED";
				var batchUpdateResult = contactApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();
				
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName}-UPDATED2";
				batchUpdateResult = contactApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();
				
				var searchOptions = new SearchRequestOptions
				{
					PropertiesToInclude = new List<string> {
						"firstname", 
						"lastname",
						"email",
						"company",
						"phone"
					},
					PropertiesWithHistory = new List<string>
					{
						"firstname", 
						"lastname",
						"email",
						"company",
						"phone"
					}
				};
				// TODO - "test" / temporary stuff follows
				var listContacts = contactApi.List<ContactHubSpotModel>(searchOptions);
				var moreResults = true;
				while (moreResults)
				{
					moreResults = listContacts.MoreResultsAvailable;
					foreach (var contact in listContacts.Contacts)
					{
						Console.WriteLine($"{contact.FirstName} {contact.LastName}");
					}
					if (moreResults)
						listContacts = contactApi.List<ContactHubSpotModel>(listContacts.SearchRequestOptions);
						
				}

			}
			finally
			{
				contactApi.BatchArchive(batchCreateResult);
			}
			
		}
		
		/// <summary>
		/// Test Create operations
		/// </summary>
		[TestMethod]
		public void Create_Contact()
		{
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var contact = contactApi.Create(new ContactHubSpotModel
			{
				Email = "test@communityclosing.com",
				FirstName = "Testy",
				LastName = "Testerson",
				Phone = "3018675309",
				Company = "Community Closing Network, LLC"
			});
			try
			{
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse((contact.Id is null | contact.Id == 0L) | !(contact.Id is long));
			}
			finally
			{
				Utilities.Sleep();
				contactApi.Delete(contact);
				contactApi.GetByUniqueId<ContactHubSpotModel>(contact.Id); 
			}
		}
		
		[TestMethod]
		public void GetByUniqueId_Contact()
		{
			
		}
		
		[TestMethod]
		public void Update_Contact()
		{
			
		}
		
		[TestMethod]
		public void Delete_Contact()
		{
			
		}
		
		[TestMethod]
		public void Search_Contacts()
		{
			
		}
		
	
		
		
		
		// *** ORIGINAL TESTS FOLLOW *** //
		/*[TestMethod]
		public void Search_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			IList<ContactHubSpotModel> sampleContacts = new List<ContactHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var contact = contactApi.Create(new ContactHubSpotModel()
				{
					FirstName = "Test",
					LastName = $"User {i:N0}",
					Email = $"Test.User.{i:N0}@sampledomain.com"
				});
				sampleContacts.Add(contact);
			}
			
			// HubSpot is rather slow to update... wait 20 seconds to allow it to catch up
			System.Threading.Thread.Sleep(20 * 1000);
			
			try
			{
				var searchOptions = new SearchRequestOptions
				{
					Limit = 3,
					SortBy = "lastname",
					SortDirection = SearchRequestSortType.Ascending
				};
				var filterGroup = new SearchRequestFilterGroup();
				var filter = new SearchRequestFilter
				{
					Operator = SearchRequestFilterOperatorType.EqualTo,
					Value = "sampledomain.com",
					PropertyName = "hs_email_domain"
				};
				filterGroup.Filters.Add(filter);
				searchOptions.FilterGroups.Add(filterGroup);
				searchOptions.PropertiesToInclude = new List<string>
				{
					"firstname",
					"lastname",
					"email",
					"hs_email_domain",
					"createdate",
					"lastmodifieddate"
				};
                
				// Act
				var results = contactApi.Search<ContactHubSpotModel>(searchOptions);

				// Assert
				Assert.AreEqual(5, results.Total, "Did not identify a total of 5 results.");
				Assert.AreEqual(3, results.Contacts.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Contacts.Any(c => string.IsNullOrWhiteSpace(c.Email)), "Some contacts do not have email addresses.");
				Assert.AreEqual($"User 1", results.Contacts[0].LastName, $"Last Name '{results.Contacts[0].LastName}' did not match User 1.");
				Assert.AreEqual($"User 2", results.Contacts[1].LastName, $"Last Name '{results.Contacts[1].LastName}' did not match User 2.");
				Assert.AreEqual($"User 3", results.Contacts[2].LastName, $"Last Name '{results.Contacts[2].LastName}' did not match User 3.");
				Assert.AreNotEqual(0, results.Offset);

				// Second Act
				searchOptions.Offset = results.Offset;
				results = contactApi.Search<ContactHubSpotModel>(searchOptions);

				Assert.AreEqual(5, results.Total, "Did not identify a total of 5 results.");
				Assert.AreEqual(2, results.Contacts.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results.Contacts.Any(c => string.IsNullOrWhiteSpace(c.Email)), "Some contacts do not have email addresses.");
				Assert.AreEqual($"User 4", results.Contacts[0].LastName, $"Last Name '{results.Contacts[0].LastName}' did not match User 4.");
				Assert.AreEqual($"User 5", results.Contacts[1].LastName, $"Last Name '{results.Contacts[1].LastName}' did not match User 5.");
			}
			finally
			{
				// Clean-up
				foreach (var contact in sampleContacts)
				{
					contactApi.Delete(contact.Id);
				}
					
			}
		}

		[TestMethod]
		public void RecentlyCreated_5SamplesLimitedTo3WitContinuations_ReturnsCollectionWith3ItemsWithContinuationDetails()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			IList<ContactHubSpotModel> sampleContacts = new List<ContactHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var contact = contactApi.Create(new ContactHubSpotModel()
				{
					FirstName = "Created Test",
					LastName = $"User {i:N0}",
					Email = $"Test.User.{i:N0}@sampledomain.com"
				});
				sampleContacts.Add(contact);
			}
            
			// HubSpot is rather slow to update the list... wait 20 seconds to allow it to catch up
			System.Threading.Thread.Sleep(20 * 1000);
            
			try
			{
				var searchOptions = new SearchRequestOptions
				{
					Limit = 3,
					FilterGroups = new List<SearchRequestFilterGroup>
					{
						new SearchRequestFilterGroup
						{
							Filters = new List<SearchRequestFilter>
							{
								new SearchRequestFilter
								{
									PropertyName = "createdate",
									Operator = SearchRequestFilterOperatorType.GreaterThanOrEqualTo,
									Value = ((DateTimeOffset)sampleContacts.First().CreatedAt).AddSeconds(-10)
										.ToUnixTimeMilliseconds().ToString()
								}
							}
						}
					}
				};
				
				// Act
				var results = contactApi.Search<ContactHubSpotModel>(searchOptions);
                
				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(3, results.Contacts.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Contacts.Any(c => string.IsNullOrWhiteSpace(c.Email)), "Some contacts do not have email addresses.");
				Assert.AreNotEqual(0, results.Offset);
				
				// Second Act
				searchOptions.Offset = results.Offset;
				var results2 = contactApi.Search<ContactHubSpotModel>(searchOptions);
                
				Assert.IsFalse(results2.MoreResultsAvailable, "Did not identify at the end of results.");
				Assert.AreEqual(2, results2.Contacts.Count, "Did not return 2 of the 5 results.");
				Assert.AreEqual(false, results2.Contacts.Any(c => string.IsNullOrWhiteSpace(c.Email)), "Some contacts do not have email addresses.");
			}
			finally
			{
				// Clean-up
				foreach (var contact in sampleContacts)
                    contactApi.Delete(contact.Id);
			}
		}

		[TestMethod]
		public void RecentlyUpdated_3SamplesLimitedTo2WitContinuations_ReturnsCollectionWith2ItemsWithContinuationDetails()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			IList<ContactHubSpotModel> sampleContacts = new List<ContactHubSpotModel>();
			for (int i = 1; i <= 5; i++)
			{
				var contact = contactApi.Create(new ContactHubSpotModel()
				{
					FirstName = "Created Test",
					LastName = $"User {i:N0}",
					Email = $"Test.User.{i:N0}@sampledomain.com"
				});
				sampleContacts.Add(contact);
			}

			for (int i = 0; i < sampleContacts.Count; i++)
			{
				ContactHubSpotModel contact = sampleContacts[i];
				contact.FirstName = $"Updated Test";
				contactApi.Update(contact);
				// This is intentional to skip to every odd item
				i++;
			}

			// HubSpot is rather slow to update the list... wait 20 seconds to allow it to catch up
			System.Threading.Thread.Sleep(20 * 1000);

			try
			{
				var searchOptions = new SearchRequestOptions
				{
					Limit = 2,
					FilterGroups = new List<SearchRequestFilterGroup>
					{
						new SearchRequestFilterGroup
						{
							Filters = new List<SearchRequestFilter>
							{
								new SearchRequestFilter
								{
									PropertyName = "lastmodifieddate",
									Operator = SearchRequestFilterOperatorType.GreaterThanOrEqualTo,
									Value = ((DateTimeOffset)sampleContacts.First().CreatedAt).AddSeconds(-10)
										.ToUnixTimeMilliseconds().ToString()
								}
							}
						}
					}
				};

				// Act
				ContactListHubSpotModel<ContactHubSpotModel> results = contactApi
					.Search<ContactHubSpotModel>(searchOptions);

				// Assert
				Assert.IsTrue(results.MoreResultsAvailable, "Did not identify more results are available.");
				Assert.AreEqual(2, results.Contacts.Count, "Did not return 3 of the 5 results.");
				Assert.AreEqual(false, results.Contacts.Any(c => string
					.IsNullOrWhiteSpace(c.Email)), "Some contacts do not have email addresses.");
				Assert.AreNotEqual(0, results.Offset);

				// Cannot actually test recently updated as recently created pollutes the results.
				// TODO - test recently updated
			}
			finally
			{
				// Clean-up
				foreach (var contact in sampleContacts)
					contactApi.Delete(contact.Id);
			}
		}

		[TestMethod]
		public void Create_SampleDetails_IdProeprtyIsSet()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var sampleContact = new ContactHubSpotModel
			{
				FirstName = "Test",
				LastName = $"User Create",
				Email = "Test.User.Create@sampledomain.com",
				Phone = "123-456-789",
				Company = "Sample Company"
			};

			// Act
			ContactHubSpotModel contact = contactApi.Create(sampleContact);

			try
			{
				// Assert
				Assert.IsNotNull(contact.Id, "The Id was not set and returned.");
				Assert.AreEqual(sampleContact.FirstName, contact.FirstName);
				Assert.AreEqual(sampleContact.LastName, contact.LastName);
				// HubSpot stores all email address in lowercase
				Assert.AreEqual(sampleContact.Email?.ToLowerInvariant(), contact.Email);
				Assert.AreEqual(sampleContact.Phone, contact.Phone);
				Assert.AreEqual(sampleContact.Company, contact.Company);
			}
			finally
			{
                // Clean-up
				contactApi.Delete(contact.Id);
			}
		}*/

		/*[TestMethod]
		public void Update_SampleDetails_PropertiesAreUpdated()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var sampleContact = new ContactHubSpotModel
			{
				FirstName = "Test",
				LastName = $"User Update",
				Email = "Test.User.Update@sampledomain.com",
				Phone = "123-456-789",
				Company = "Sample Company"
			};

			ContactHubSpotModel contact = contactApi.Create(sampleContact);

			contact.Phone = "1234-5678";
			contact.Company = "Second Sample Company";

			// Act
			contactApi.Update(contact);
			
			// HubSpot is rather slow to update the list... wait 20 seconds to allow it to catch up
			System.Threading.Thread.Sleep(20 * 1000);
            
			try
			{
				// Assert
				Assert.AreNotEqual(sampleContact.Phone, contact.Phone);
				Assert.AreNotEqual(sampleContact.Company, contact.Company);
				Assert.AreEqual("1234-5678", contact.Phone);
				Assert.AreEqual("Second Sample Company", contact.Company);

				// Second Act
				contact = contactApi.GetByEmail<ContactHubSpotModel>(sampleContact.Email, new SearchRequestOptions
				{
					PropertiesToInclude = new List<string> {"phone", "email", "company"}
				});
                
				// Second Assert
				Assert.AreNotEqual(sampleContact.Phone, contact.Phone);
				Assert.AreNotEqual(sampleContact.Company, contact.Company);
				Assert.AreEqual("1234-5678", contact.Phone);
				Assert.AreEqual("Second Sample Company", contact.Company);
			}
			finally
			{
				// Clean-up
				contactApi.Delete(contact.Id);
			}
		}*/

		/*[TestMethod]
		public void Delete_SampleContact_ContactIsDeleted()
		{
			// Arrange
			var contactApi = new HubSpotContactApi(TestSetUp.Client);
			var sampleContact = new ContactHubSpotModel
			{
				FirstName = "Test",
				LastName = $"User Delete",
				Email = "Test.User.Delete@sampledomain.com",
				Phone = "123-456-789",
				Company = "Sample Company"
			};

			ContactHubSpotModel contact = contactApi.Create(sampleContact);

			// Act
			contactApi.Delete(contact.Id);

			// Assert
			contact = contactApi.GetByEmail<ContactHubSpotModel>(sampleContact.Email);
			Console.WriteLine();
			Assert.IsNull(contact, "The contact was searchable and not deleted.");
		}*/
	}
}