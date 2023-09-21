using System;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.Engagement.Dto;
using HubSpot.NET.Api.Files.Dto;
using HubSpot.NET.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HubSpot.NET.Api;

namespace HubSpot.NET.Examples
{
    public class Contacts
    {
        public static void Example(HubSpotApi api)
        {
            /*
             * Create a contact
             */
            Console.WriteLine("* Creating a contact ...");
            var contact = api.Contact.CreateOrUpdate(new ContactHubSpotModel()
            {
                Email = "john@squaredup.com",
                FirstName = "John",
                LastName = "Smith",
                Phone = "00000 000000",
                Company = "Squared Up Ltd."
            });
            Console.WriteLine($"-> Contact created! {contact.FirstName} {contact.LastName} <{contact.Email}>");

            /*
             * Update a contact
             */
            Console.WriteLine($"* Updating contact's phone number: '{contact.Phone}' to '111111 11111' ...");
            contact.Phone = "111111 11111";
            api.Contact.Update(contact);
            Console.WriteLine($"-> Contact updated! {contact.FirstName} {contact.LastName} <{contact.Phone}>");

            /*
             * Search for a contact
             */
            Console.WriteLine($"* Searching for email addresses matching: '*squaredup*' ...");
            var search = api.Contact.Search<ContactHubSpotModel>(new SearchRequestOptions
            {
                Limit = 1,
                SortBy = "createdate",
                SortDirection = SearchRequestSortType.Descending,
                FilterGroups = new List<SearchRequestFilterGroup>
                {
                    new SearchRequestFilterGroup
                    {
                        Filters = new List<SearchRequestFilter>
                        {
                            new SearchRequestFilter
                            {
                                Operator = SearchRequestFilterOperatorType.EqualTo,
                                Value = "*squaredup*",
                                PropertyName = "email"
                            }
                        }
                    }
                },
                PropertiesToInclude = new List<string>
                {
                    "firstname", "lastname", "email", "phone"
                }
            });
            var moreResults = true;
            while (moreResults)
            {
                moreResults = search.MoreResultsAvailable;
                foreach (var searchResult in search.Contacts)
                {
                    Console.WriteLine(
                        $"-> Search Result: #{searchResult.Id} {searchResult.FirstName} {searchResult.LastName} " +
                        $"<{searchResult.Email}> Created: {searchResult.CreatedAt} Modified: {searchResult.UpdatedAt}");
                }
                if (moreResults)
                    search = api.Contact.Search<ContactHubSpotModel>(search.SearchRequestOptions);
                    /* This works too:
                     * search = api.Contact.Search<ContactHubSpotModel>(searchOptions);
                     */
                    // TODO - There should be no difference between the two; might want to demonstrate that in the examples for documentation sake.
            }
            
            /*
             * Batch create or update contacts
             */
            Console.WriteLine($"* Creating a batch of contacts ...");
            var batchContacts = new ContactListHubSpotModel<ContactHubSpotModel>();
            foreach (var i in Enumerable.Range(1, 5))
            {
                var batchContact = new ContactHubSpotModel
                {
                    FirstName = $"FirstName{i:N0}",
                    LastName = $"LastName{i:N0}",
                    Email = $"test.user{i:N0}@example{i:N0}.com",
                    Phone = $"{i:N0}{i:N0}{i:N0}-{i:N0}{i:N0}{i:N0}-{i:N0}{i:N0}{i:N0}{i:N0}",
                    Company = $"Test Company {i:N0}"
                };
                batchContacts.Contacts.Add(batchContact);
            }
            batchContacts.Contacts[2].Id = 999999999999; // This ID does not (should not) exist
            batchContacts.Contacts[4].Id = 888888888888; // This ID does not (should not) exist
            var batchResults = api.Contact.BatchCreateOrUpdate(batchContacts);
            Console.WriteLine($"-> Status: {batchResults.Status}");
            Console.WriteLine($"-> Errors:");
            foreach (var error in batchResults.Errors)
            {
                Console.WriteLine($"\tStatus: {error.Status}");
                Console.WriteLine($"\tCategory: {error.Category}");
                Console.WriteLine($"\tMessage: {error.Message}");
                if (error.Context.Ids.Count <= 0) continue;
                Console.WriteLine($"\tProblematic Objects:");
                foreach (var id in error.Context.Ids)
                {
                    Console.WriteLine($"\t\t* id: {id}");
                }

            }
            
            /*
             * Wait a few seconds for HubSpot to update
             */
            System.Threading.Thread.Sleep(15 * 1000);
            
            /*
             * Get recently created contacts
             */
            Console.WriteLine($"* Retrieving recently created contacts ...");
            var recent = api.Contact.RecentlyCreated<ContactHubSpotModel>();
            var recentResults = true;
            while (recentResults)
            {
                recentResults = recent.MoreResultsAvailable;
                foreach (var searchResult in recent.Contacts)
                {
                    Console.WriteLine(
                        $"-> Search Result: #{searchResult.Id} {searchResult.FirstName} {searchResult.LastName} " +
                        $"<{searchResult.Email}> Created: {searchResult.CreatedAt} Modified: {searchResult.UpdatedAt}");
                }
                if (recentResults)
                    recent = api.Contact.Search<ContactHubSpotModel>(recent.SearchRequestOptions);
            }
            
            // TODO - Examples have been refactored up to this line.
            System.Environment.Exit(0);




            /*
             * Upload a file (to attach to a contact)
             */
            var file = new FileHubSpotModel()
            {
                File = File.ReadAllBytes("MY FILE PATH"),
                Name = "File.png",
                Hidden = true, //set to true for engagements
            };

            var uploaded = api.File.Upload<FileHubSpotModel>(file);
            var fileId = uploaded.Objects.First().Id;

            /*
             * Add a Note engagement to a contact with a file attachment
             */
            api.Engagement.Create(new EngagementHubSpotModel()
            {
                Engagement = new EngagementHubSpotEngagementModel()
                {
                    Type = "NOTE" //used for file attachments
                },
                Metadata = new
                {
                    body = "This is an example note"
                },
                Associations = new EngagementHubSpotAssociationsModel()
                {
                    ContactIds = new List<long>() { contact.Id } //use the ID of the created contact from above
                },
                Attachments = new List<EngagementHubSpotAttachmentModel>() {
                    new EngagementHubSpotAttachmentModel()
                    {
                        Id = fileId
                    }
                }
            });

            /*
             * Delete a contact
             */
            api.Contact.Delete(contact.Id);

            /*
             * Get all contacts with specific properties
             * By default only a few properties are returned
             */
            var contacts = api.Contact.List<ContactHubSpotModel>(
                new SearchRequestOptions { PropertiesToInclude = new List<string> { "firstname", "lastname", "email" } });

            /*
             * Get the most recently updated contacts, limited to 10
             */
            var recentlyUpdated = api.Contact.RecentlyUpdated<ContactHubSpotModel>(new SearchRequestOptions()
            {
                Limit = 10
            });

            /*
             * Get the most recently created contacts, limited to 10
             */
            var recentlyCreated = api.Contact.RecentlyCreated<ContactHubSpotModel>(new SearchRequestOptions()
            {
                Limit = 10
            });

          
        }
    }
}
