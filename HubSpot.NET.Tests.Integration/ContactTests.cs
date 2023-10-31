using System;
using System.Linq;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Search;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Core.Utilities;
using System.Collections.Generic;
using HubSpot.NET.Api.Contact.Dto;
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
		/// Also tests BatchRead archived/un-archived operations.
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
				.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
				"Found contact records with invalid id properties");
			var batchArchiveResult = contactApi.BatchArchive(batchCreateResult);
			Utilities.Sleep();
			// Archived contact records should have a Status of "ARCHIVED"
			Assert.IsTrue(batchArchiveResult.Status == "ARCHIVED",
				"Status is not 'ARCHIVED'"); // TODO - Should this be an enum?
			var batchReadResult = contactApi.BatchRead(batchArchiveResult);
			// Contacts should be empty after an archive operation.
			Assert.AreEqual(0, batchReadResult.Contacts.Count,
				"'Contacts' should be empty");
			// But if we set "Archived = true" in our SearchRequestOptions object ...
			batchArchiveResult.SearchRequestOptions.Archived = true;
			// ... then request the batch again ...
			var batchReadArchivedResult = contactApi.BatchRead(batchArchiveResult);
			// ... we should have 20 "archived" contact records.
			Assert.AreEqual(20, batchReadArchivedResult.Contacts.Count,
				$"Unexpected number of contacts: {batchReadArchivedResult.Contacts.Count}; expected: 20 ");
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
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
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
		/// Also tests BatchCreate operations.
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
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
				var batchReadResult = contactApi.BatchRead(batchCreateResult);
				// Retrieved contact records should have an Id property that is a long type.
				Assert.IsFalse(batchReadResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
				
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
					.All(c => (c.Id is null | c.Id == 0L | !(c.Id is long))),
					"Found contact records with invalid id properties");
				
				// TODO - Test "Archived = true" in search options. Currently this doesn't work even though the documentation says it should. I've emailed HubSpot about it; stay tuned!

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
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName} (UPDATED)";
				var batchUpdateResult = contactApi.BatchUpdate(batchCreateResult);
				Utilities.Sleep();
				var batchReadResult = contactApi.BatchRead(batchUpdateResult);
				// Updated contact records should have a FirstName property that contains "(UPDATED)".
				Assert.IsTrue(batchReadResult.Contacts.All(c => c.FirstName.Contains("(UPDATED)")),
					"'FirstName' property does not contain '(UPDATED)'");
			}
			finally
			{
				contactApi.BatchArchive(batchCreateResult);
			}
		}

		/// <summary>
		/// Test List operations.
		/// </summary>
		/// <remarks>
		/// Also tests contact property history and SearchRequestOptions (Limit, PropertiesWithHistory).
		/// </remarks>
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
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse(batchCreateResult.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
				
				// Update our newly-created models so we can generate some property history.
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName} (UPDATED)";
				contactApi.BatchUpdate(batchCreateResult);
				
				Utilities.Sleep();
				
				// Update (again) our recently updated models so we can generate some more property history.
				foreach (var contact in batchCreateResult.Contacts)
					contact.FirstName = $"{contact.FirstName} (UPDATED AGAIN)";
				
				contactApi.BatchUpdate(batchCreateResult);
				
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
						"firstname"
					}
				};

				batchCreateResult.SearchRequestOptions = searchOptions;
				
				/*
				 * By this point, every contact object in batchCreateResult should have had its FirstName property
				 * updated twice, so there should be three property history items (created, updated, updated again) for
				 * the FirstName property of each contact object in the list.
				 */
				foreach (var contact in contactApi.BatchRead(batchCreateResult, searchOptions).Contacts)
				{
					Assert.IsTrue(contact.PropertiesWithHistory.FirstName.Count == 3,
						$"Unexpected number of 'FirstName' property history items: " +
						$"{contact.PropertiesWithHistory.FirstName.Count}; expected: 3");
				}
				
				/*
				 * 20 contacts were created previously, so we should have no trouble listing 10 of them and there should
				 * be a paging object in the results.
				 */
				searchOptions.Limit = 10;
				var list10Contacts = contactApi.List<ContactHubSpotModel>(searchOptions);
				Assert.AreEqual(10, list10Contacts.Contacts.Count,
					$"Unexpected number of contacts: {list10Contacts.Contacts.Count}; expected: 10");
				Assert.IsNotNull(list10Contacts.Paging, "Paging object was null");
				Assert.IsTrue(list10Contacts.MoreResultsAvailable, "More results were expected");
				
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
				contactApi.BatchArchive(batchCreateResult);
			}
			
		}
		
		/// <summary>
		/// Test Create operations.
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
				Assert.IsFalse((contact.Id is null | contact.Id == 0L) | !(contact.Id is long),
					"Found contact records with invalid id properties");
			}
			finally
			{
				Utilities.Sleep();
				contactApi.Delete(contact);
			}
		}
		
		/// <summary>
		/// Test GetByUniqueId operations.
		/// </summary>
		/// <remarks>
		/// Also tests retrieval of archived contact records.
		/// </remarks>
		[TestMethod]
		public void GetByUniqueId_Contact()
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

			// We're going to keep a record of all created contacts to we can ensure they're removed later.
			var createdContacts = new ContactListHubSpotModel<ContactHubSpotModel>
			{
				Contacts = new List<ContactHubSpotModel> { contact }
			};

			Utilities.Sleep();
			
			try
			{
				// Created contact records should have an Id property that is a long type.
				Assert.IsFalse((contact.Id is null | contact.Id == 0L) | !(contact.Id is long),
					"Found contact records with invalid id properties");

				// Test retrieving a contact record via the "email" attribute.
				var searchOptions = new SearchRequestOptions
				{
					IdProperty = "email"
				};
				var getContactByEmail = contactApi
					.GetByUniqueId<ContactHubSpotModel>("test@communityclosing.com", searchOptions);
				Assert.AreEqual("test@communityclosing.com", getContactByEmail.Email, 
					$"Unexpected value for 'Email': {getContactByEmail.Email}; " +
					$"expected: 'test@communityclosing.com'");
				
				/*
				 * A word on archived contact records and email addresses: Even though an email address is a "unique"
				 * property, if you create and delete a contact that has the same email address multiple times, you
				 * will no longer be able to use GetByUniqueId to retrieve the archived record because there will be
				 * multiple archived records with that same email address. Further complicating matters (to the extent
				 * we need to retrieve archived items) is the fact that search will never return archived records, so
				 * you can't search for them, nor can you use the BatchRead method to retrieve them. To summarize: once
				 * a record has been archived (deleted), you cannot reliably retrieve them via any unique property other
				 * than the record id. This is demonstrated in the test that follows.
				 */
				var oldContactId = contact.Id;
				contactApi.Delete(contact);
				Utilities.Sleep();
				contact = contactApi.Create(contact);
				createdContacts.Contacts.Add(contact);
				Assert.AreNotEqual(oldContactId, contact.Id,
					"Old contact id and new contact id are the same but they shouldn't be");

				oldContactId = contact.Id;
				contactApi.Delete(contact);
				Utilities.Sleep();
				contact = contactApi.Create(contact);
				createdContacts.Contacts.Add(contact);
				Assert.AreNotEqual(oldContactId, contact.Id,
					"Old contact id and new contact id are the same but they shouldn't be");
				
				searchOptions.Archived = true;
				
				// GetByUniqueId fails if using the email address as the unique id ...
				Assert.ThrowsException<HubSpotException>(() => contactApi
					.GetByUniqueId<ContactHubSpotModel>(contact.Email, searchOptions),
					"A 'HubSpotException' was expected but not thrown");
				
				// ... And BatchRead fails as well.
				var batchReadArchivedContactsViaEmail = new ContactListHubSpotModel<ContactHubSpotModel>();
				batchReadArchivedContactsViaEmail.Contacts.Add(new ContactHubSpotModel {Id = contact.Email});
				batchReadArchivedContactsViaEmail.SearchRequestOptions = searchOptions;
				Assert.ThrowsException<HubSpotException>(() => contactApi.BatchRead(batchReadArchivedContactsViaEmail),
					"A 'HubSpotException' was expected but not thrown");
				
				/*
				 * However, both GetByUniqueId and BatchRead will work if you know the id of the record(s) you want to
				 * retrieve. This is demonstrated in the test that follows.
				 */
				searchOptions.IdProperty = null;
				var getArchivedContactById = contactApi
					.GetByUniqueId<ContactHubSpotModel>(contact.Id, searchOptions);
				Assert.AreEqual(contact.Id, getArchivedContactById.Id, "Contact ids do not match");

				var getArchivedContactsById = contactApi
					.BatchRead(createdContacts, searchOptions);
				Assert.IsFalse(getArchivedContactsById.Contacts
					.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
			}
			finally
			{
				contactApi.BatchArchive(createdContacts);
			}
		}
		
		/// <summary>
		/// Test Update operations.
		/// </summary>
		[TestMethod]
		public void Update_Contact()
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
				Assert.IsFalse((contact.Id is null | contact.Id == 0L) | !(contact.Id is long),
					"Found contact records with invalid id properties");

				contact.Email = $"UPDATED-{contact.Email}";
				contact.FirstName = $"{contact.FirstName} (UPDATED)";
				contact.LastName = $"{contact.LastName} (UPDATED)";
				contact.Phone = "2408675309";
				contact.Company = $"{contact.Company} (UPDATED)";

				contact = contactApi.Update(contact);
				
				/*
				 * Updated contact properties should match what we updated them to with one minor exception: email
				 * addresses will always be normalized to lowercase.
				 */
				Assert.AreNotEqual("UPDATED-test@communityclosing.com", contact.Email,
					$"Unexpected value for 'Email': '{contact.Email}'; expected: " +
					$"'updated-test@communityclosing.com'");
				Assert.AreEqual("updated-test@communityclosing.com", contact.Email,
					$"Unexpected value for 'Email': '{contact.Email}'; expected: " +
					$"'updated-test@communityclosing.com'");
				Assert.AreEqual("Testy (UPDATED)", contact.FirstName,
					$"Unexpected value for 'FirstName': '{contact.FirstName}'; expected: " +
					$"'Testy (UPDATED)'");
				Assert.AreEqual("Testerson (UPDATED)", contact.LastName,
					$"Unexpected value for 'LastName': '{contact.LastName}'; expected: " +
					$"'Testerson (UPDATED)'");
				Assert.AreEqual("2408675309", contact.Phone,
					$"Unexpected value for 'Phone': '{contact.Phone}'; expected: " +
					$"'2408675309'");
				Assert.AreEqual("Community Closing Network, LLC (UPDATED)", contact.Company,
					$"Unexpected value for 'Company': '{contact.Company}'; expected: " +
					$"'Community Closing Network, LLC (UPDATED)'");
			}
			finally
			{
				contactApi.Delete(contact);
			}
		}
		
		/// <summary>
		/// Test Delete (archive) operations.
		/// </summary>
		/// <remarks>
		/// It might feel a bit silly at this point, since we've created and deleted contact records multiple times
		/// already, but for completeness sake, here's the test :-) 
		/// </remarks>
		[TestMethod]
		public void Delete_Contact()
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

			// Created contact records should have an Id property that is a long type.
			Assert.IsFalse((contact.Id is null | contact.Id == 0L) | !(contact.Id is long),
				"Found contact records with invalid id properties");
			
			Utilities.Sleep(5);
			
			// Delete the contact
			contactApi.Delete(contact);
			
			// Attempt to retrieve the contact; the result will be null because we did not request archived records.
			var shouldBeNull = contactApi.GetByUniqueId<ContactHubSpotModel>(contact.Id);
			Assert.IsNull(shouldBeNull, "Retrieving a deleted (archived) contact by id without specifying " +
			                            "'Archived = true' should return null");
			
			// Attempt to retrieve the contact; this time around we're explicitly requesting archived records.
			var deletedContact = contactApi
				.GetByUniqueId<ContactHubSpotModel>(contact.Id, new SearchRequestOptions { Archived = true });
			Assert.AreEqual(contact.Id, deletedContact.Id,
				$"Unexpected value for 'Id': '{contact.Id}'; expected: '{deletedContact.Id}'");
		}
		
		/// <summary>
		/// Test Search operations.
		/// </summary>
		[TestMethod]
		public void Search_Contacts()
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
						.All(c => (c.Id is null | c.Id == 0L) | !(c.Id is long)),
					"Found contact records with invalid id properties");
				
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
					Value = "-test@communityclosing.com",
					PropertyName = "email"
				});
				
				batchCreateResult.SearchRequestOptions.FilterGroups[0].Filters.Add(new SearchRequestFilter
				{
					Operator = SearchRequestFilterOperatorType.EqualTo,
					Value = $"Testerson {timestamp}",
					PropertyName = "lastname"
				});
				
				batchCreateResult.SearchRequestOptions.Limit = 5;

				var searchResult = contactApi
					.Search<ContactHubSpotModel>(batchCreateResult.SearchRequestOptions);
				
				// We should only have 5 records ...
				Assert.AreEqual(5, searchResult.Contacts.Count, 
					$"Unexpected number of contacts: {searchResult.Contacts.Count}; expected: 5");
				
				// ... But there should be a total of 20 records
				Assert.AreEqual(20, searchResult.Total,
					$"Unexpected number of contacts: {searchResult.Total}; expected: 20");
				
				// ... And more results should be available
				Assert.IsTrue(searchResult.MoreResultsAvailable, 
					$"Invalid value for 'MoreResultsAvailable': '{searchResult.MoreResultsAvailable}'; " +
					$"expected: 'true'");

				/*
				 * By default, searches are sorted by 'createdate', 'descending', so if we reverse the sort direction,
				 * and search again, the previous first and last records should now be equal to the last and first
				 * records in the list, respectively. But there's a catch! it's possible, even likely in this scenario
				 * for multiple contact records to share the same 'createdate' value, which makes it impossible to
				 * reliably sort by createdate, so we'll switch to sorting by 'id' instead. But there's another catch!
				 * 'id' isn't actually a property that you can sort by, however 'hs_object_id' is, so we'll use that.
				 */
				searchResult.SearchRequestOptions.Offset = null;
				searchResult.SearchRequestOptions.Limit = 20;
				searchResult.SearchRequestOptions.SortBy = "hs_object_id";
				searchResult = contactApi.Search<ContactHubSpotModel>(searchResult.SearchRequestOptions);
				
				var firstRecordId = searchResult.Contacts.First().Id;
				var lastRecordId = searchResult.Contacts.Last().Id;
				
				searchResult.SearchRequestOptions.SortDirection = SearchRequestSortType.Ascending;
				searchResult = contactApi.Search<ContactHubSpotModel>(searchResult.SearchRequestOptions);
				Assert.IsTrue(lastRecordId == searchResult.Contacts.First().Id, 
					$"Unexpected 'id' value for the first contact in the list: '{searchResult.Contacts.First().Id}'; " +
					$"expected: '{lastRecordId}'");
				Assert.IsTrue((firstRecordId == searchResult.Contacts.Last().Id), 
					$"Unexpected 'id' value for the last contact in the list: '{searchResult.Contacts.Last().Id}; " +
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
				contactApi.BatchArchive(batchCreateResult);
			}
		}

		/// <summary>
		/// Tests the default values of properties for ContactHubSpotModel and ContactListHubSpotModel.
		/// </summary>
		[TestMethod]
		public void Default_Property_Values()
		{
			var contactHubSpotModel = new ContactHubSpotModel();
			
			Assert.IsNull(contactHubSpotModel.Id,
				$"Unexpected value for 'Id': '{contactHubSpotModel.Id}'; expected: 'null'");
			Assert.IsTrue(contactHubSpotModel.SerializeProperties,
				$"Unexpected value for 'SerializeProperties': '{contactHubSpotModel.SerializeProperties}'; " +
				$"expected: 'true'");
			var shouldSerializeProperties = contactHubSpotModel.ShouldSerializeProperties(); 
			Assert.IsTrue(shouldSerializeProperties,
				$"Unexpected value returned by 'ShouldSerializeProperties': " +
				$"'{shouldSerializeProperties}'; expected: 'true'");
			Assert.AreEqual(0, contactHubSpotModel.Associations.Count,
				$"Unexpected number of items in 'Associations': {contactHubSpotModel.Associations.Count}; " +
				$"expected: 0");
			Assert.IsNull(contactHubSpotModel.SerializeAssociations,
				$"Unexpected value for 'SerializeAssociations': '{contactHubSpotModel.Associations}'; " +
				$"expected: 'null'");
			var shouldSerializeAssociations = contactHubSpotModel.ShouldSerializeAssociations(); 
			Assert.IsFalse(shouldSerializeAssociations,
				$"Unexpected value for 'ShouldSerializeAssociations': '{shouldSerializeAssociations}'; " +
				$"expected: 'false'");
			Assert.IsNull(contactHubSpotModel.Email,
				$"Unexpected value for 'Email': '{contactHubSpotModel.Email}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.FirstName,
				$"Unexpected value for 'FirstName': '{contactHubSpotModel.FirstName}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.LastName,
				$"Unexpected value for 'LastName': '{contactHubSpotModel.LastName}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.Website,
				$"Unexpected value for 'Website': '{contactHubSpotModel.Website}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.EmailDomain,
				$"Unexpected value for 'EmailDomain': '{contactHubSpotModel.EmailDomain}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.Company,
				$"Unexpected value for 'Company': '{contactHubSpotModel.Company}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.Phone,
				$"Unexpected value for 'Phone': '{contactHubSpotModel.Phone}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.Address,
				$"Unexpected value for 'Address': '{contactHubSpotModel.Address}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.City,
				$"Unexpected value for 'City': '{contactHubSpotModel.City}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.State,
				$"Unexpected value for 'State': '{contactHubSpotModel.State}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.ZipCode,
				$"Unexpected value for 'ZipCode': '{contactHubSpotModel.ZipCode}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.PropertiesWithHistory,
				$"Unexpected value for 'PropertiesWithHistory': '{contactHubSpotModel.PropertiesWithHistory}'; " +
				$"expected: 'null'");
			Assert.IsNull(contactHubSpotModel.CreatedAt,
				$"Unexpected value for 'CreatedAt': '{contactHubSpotModel.CreatedAt}'; expected: 'null'");
			Assert.IsNull(contactHubSpotModel.UpdatedAt,
				$"Unexpected value for 'UpdatedAt': '{contactHubSpotModel.UpdatedAt}'; expected: 'null'");
			Assert.AreEqual("contacts", contactHubSpotModel.HubSpotObjectType,
				$"Unexpected value for 'HubSpotObjectType': '{contactHubSpotModel.HubSpotObjectType}'; " +
				$"expected: 'contacts'");
			Assert.AreEqual("/crm/v3/objects/contacts",contactHubSpotModel.RouteBasePath,
				$"Unexpected value for 'RouteBasePath': '{contactHubSpotModel.RouteBasePath}'; " +
				$"expected: '/crm/v3/objects/contacts'");
			
			var contactListHubSpotModel = new ContactListHubSpotModel<ContactHubSpotModel>();
			
			Assert.AreEqual(0, contactListHubSpotModel.Contacts.Count,
				$"Unexpected number of items in 'Contacts': {contactListHubSpotModel.Contacts.Count}; " +
				$"expected: 0");
			Assert.AreEqual(0, contactListHubSpotModel.Inputs.Count,
				$"Unexpected number of items in 'Inputs': {contactListHubSpotModel.Inputs.Count}; expected: 0");
			Assert.AreEqual(0, contactListHubSpotModel.Results.Count,
				$"Unexpected number of items in 'Results': '{contactListHubSpotModel.Results.Count}'; expected: 0");
			var shouldSerializeResults = contactListHubSpotModel.ShouldSerializeResults();
			Assert.IsFalse(shouldSerializeResults,
				$"Unexpected value for 'ShouldSerializeResults': '{shouldSerializeResults}'; expected: 'false'");
			Assert.IsNull(contactListHubSpotModel.Status,
				$"Unexpected value for 'Status': '{contactListHubSpotModel.Status}'; expected: 'null'");
			Assert.IsNull(contactListHubSpotModel.Total,
				$"Unexpected value for 'Total': '{contactListHubSpotModel.Total}'; expected: 'null'");
			Assert.IsNull(contactListHubSpotModel.TotalErrors,
				$"Unexpected value for 'TotalErrors': '{contactListHubSpotModel.TotalErrors}'; " +
				$"expected: 'null'");
			Assert.AreEqual(0, contactListHubSpotModel.Errors.Count,
				$"Unexpected number of items in 'Errors': {contactListHubSpotModel.Errors.Count}; expected: 0");
			var shouldSerializeErrors = contactListHubSpotModel.ShouldSerializeErrors();
			Assert.IsFalse(shouldSerializeErrors,
				$"Unexpected value for 'ShouldSerializeErrors': '{shouldSerializeErrors}'; expected: 'false'");
			Assert.IsNull(contactListHubSpotModel.RequestedAt,
				$"Unexpected value for 'RequestedAt': '{contactListHubSpotModel.RequestedAt}'; expected: 'null'");
			Assert.IsNull(contactListHubSpotModel.StartedAt,
				$"Unexpected value for 'StartedAt': '{contactListHubSpotModel.StartedAt}'; expected: 'null'");
			Assert.IsNull(contactListHubSpotModel.CompletedAt,
				$"Unexpected value for 'CompletedAt': '{contactListHubSpotModel.CompletedAt}'; expected: 'null'");
			Assert.IsFalse(contactListHubSpotModel.MoreResultsAvailable,
				$"Unexpected value for 'MoreResultsAvailable': '{contactListHubSpotModel.MoreResultsAvailable}'; " +
				$"expected: 'false'");
			Assert.IsNull(contactListHubSpotModel.Offset,
				$"Unexpected value for 'Offset': '{contactListHubSpotModel.Offset}'; expected: 'null'");
			Assert.IsNull(contactListHubSpotModel.Paging,
				$"Unexpected value for 'Paging': '{contactListHubSpotModel.Paging}'; expected: 'null'");
			// TODO: SearchRequestOptions is not tested here; instead it will be tested in Search unit tests
			Assert.IsNull(contactListHubSpotModel.IdProperty,
				$"Unexpected value for 'IdProperty': '{contactListHubSpotModel.IdProperty}'; expected: 'null'");
			Assert.AreEqual(0, contactListHubSpotModel.PropertiesWithHistory.Count,
				$"Unexpected number of items 'PropertiesWithHistory': " +
				$"{contactListHubSpotModel.PropertiesWithHistory.Count}; expected: 0");
			Assert.AreEqual("contacts", contactListHubSpotModel.HubSpotObjectType,
				$"Unexpected value for 'HubSpotObjectType': '{contactListHubSpotModel.HubSpotObjectType}'; " +
				$"expected: 'contacts'");
			Assert.AreEqual("/crm/v3/objects/contacts",contactListHubSpotModel.RouteBasePath,
				$"Unexpected value for 'RouteBasePath': '{contactListHubSpotModel.RouteBasePath}'; " +
				$"expected: '/crm/v3/objects/contacts'");
		}
	}
}