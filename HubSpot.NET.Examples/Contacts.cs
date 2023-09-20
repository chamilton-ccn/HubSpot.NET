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
            var contact = api.Contact.Create(new ContactHubSpotModel()
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
            Console.WriteLine($"* Updating a contact's phone number: '{contact.Phone}' to '111111 11111' ...");
            contact.Phone = "111111 11111";
            api.Contact.Update(contact);
            Console.WriteLine($"-> Contact updated! {contact.FirstName} {contact.LastName} <{contact.Phone}>");
            
            /*
             * Search for a contact
             */
            Console.WriteLine($"* Searching for email addresses matching: '*squaredup*' ...");
            var searchOptions = new SearchRequestOptions
            {
                Limit = 1,
                SortBy = "createdate",
                SortDirection = SearchRequestSortType.Descending
            };
            var filterGroup = new SearchRequestFilterGroup();
            
            var filter = new SearchRequestFilter
            {
                Operator = SearchRequestFilterOperatorType.EqualTo,
                Value = "*squaredup*",
                PropertyName = "email"
            };
            filterGroup.Filters.Add(filter);
            searchOptions.FilterGroups.Add(filterGroup);
            var search = api.Contact.Search<ContactHubSpotModel>(searchOptions);
            var moreResults = true;
            while (moreResults)
            {
                moreResults = search.MoreResultsAvailable;
                foreach (var searchResult in search.Contacts)
                {
                    Console.WriteLine(
                        $"-> Search Result: {searchResult.FirstName} {searchResult.LastName} <{searchResult.Email}> " +
                        $"Created: {searchResult.CreatedAt} Modified: {searchResult.UpdatedAt}");
                }
                if (moreResults)
                    search = api.Contact.Search<ContactHubSpotModel>(search.SearchRequestOptions);
                    /* This works too:
                     * search = api.Contact.Search<ContactHubSpotModel>(searchOptions);
                     */
                    // TODO - There should be no difference between the two; might want to demonstrate that in the examples for documentation sake. 
                    
            }
            
            // TODO - Examples have been refactored up to this line.




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
