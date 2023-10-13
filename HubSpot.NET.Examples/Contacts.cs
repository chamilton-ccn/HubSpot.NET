﻿using System;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Api.Engagement.Dto;
using HubSpot.NET.Api.Files.Dto;
using HubSpot.NET.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core.Search;
using HubSpot.NET.Core.Utilities;

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
            Console.WriteLine($"-> Contact updated! {contact.FirstName} {contact.LastName} {contact.Phone}");
            
            /*
             * Wait for HubSpot to catch up
             */
            Utilities.Sleep(15);

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
            }
            
            /*
             * Batch create or update contacts
             */
            Console.WriteLine($"* Creating a batch of contacts ...");
            var batchContacts = new ContactListHubSpotModel<ContactHubSpotModel>();
            foreach (var i in Enumerable.Range(1, 10))
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
            foreach (var updateContact in search.Contacts)
            {
                updateContact.LastName += " UPDATE ME!";
                batchContacts.Contacts.Add(updateContact);
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
             * Wait for HubSpot to catch up
             */
            Utilities.Sleep(30);
            
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
                    recent = api.Contact.RecentlyCreated<ContactHubSpotModel>(recent.SearchRequestOptions);
            }
            
            /*
             * Associate a contact with a company
             */
            Console.WriteLine("* Creating a company so we can associate a contact with it ...");
            var company = api.Company.Create(new CompanyHubSpotModel()
            {
                Domain = "squaredup.com",
                Name = "Squared Up"
            });
            Console.WriteLine($"-> Company created: {company.Name} ...");
            /*
             * Wait for HubSpot to catch up
             */
            Utilities.Sleep(15);
            
            Console.WriteLine($"* Randomly selecting a contact from previously created contacts ...");
            var randomNumber = new Random();
            var randomContact = recent.Contacts[randomNumber.Next(0, recent.Contacts.Count)];
            Console.WriteLine($"-> Randomly selected contact: {randomContact.FirstName} {randomContact.LastName} " +
                              $"<{randomContact.Email}>");
            Console.WriteLine("* Creating a single custom association label: 'TEST LABEL #1'...");
            var firstCustomAssociationLabel = api.Associations
                .CreateCustomAssociationType(
                    randomContact.HubSpotObjectType, 
                    company.HubSpotObjectType, 
                    new CustomAssociationTypeHubSpotModel
                    {
                        Name = "TEST LABEL #1", 
                        Label = "TEST LABEL #1"
                    }).GetSourceToDestLabel;
            
            Console.WriteLine($"-> Association label created! Name: '{firstCustomAssociationLabel.Name}' " +
                              $"Label: {firstCustomAssociationLabel.Label}, " +
                              $"TypeID: {firstCustomAssociationLabel.AssociationTypeId}");
            
            Console.WriteLine($"* Creating an association with multiple labels ({firstCustomAssociationLabel.Name}, " +
                              $"ContactToCompany) between company: '{company.Name}' and '{randomContact.FirstName} " +
                              $"{randomContact.LastName}' ...");

            var singleAssociation = new AssociationHubSpotModel
            {
                AssociationTypes = new List<AssociationTypeHubSpotModel>
                {
                    firstCustomAssociationLabel,
                    new AssociationTypeHubSpotModel
                    {
                        AssociationTypeId = AssociationType.ContactToCompany
                    }
                },
                FromObject = new AssociationObjectIdModel
                {
                    Id = randomContact.Id,
                    HubSpotObjectType = randomContact.HubSpotObjectType
                },
                ToObject = new AssociationObjectIdModel
                {
                    Id = company.Id,
                    HubSpotObjectType = company.HubSpotObjectType
                }
            };
            
            var association = api.Associations.CreateAssociation(singleAssociation);
            
            Console.WriteLine($"-> Association created! The following custom labels were applied to the association " +
                              $"between '{randomContact.FirstName} {randomContact.LastName}' and '{company.Name}': " +
                              $"{string.Join(", ", association.Result.Labels)}");
            
            Console.WriteLine("* Creating another custom association label: 'TEST LABEL #2'...");
            var secondCustomAssociationLabel = api.Associations
                .CreateCustomAssociationType(
                    randomContact.HubSpotObjectType, 
                    company.HubSpotObjectType, 
                    new CustomAssociationTypeHubSpotModel
                    {
                        Name = "TEST LABEL #2",
                        Label = "TEST LABEL #2"
                    }).GetSourceToDestLabel;
            
            Console.WriteLine($"-> Association label created! Name: '{secondCustomAssociationLabel.Name}' " +
                              $"Label: {secondCustomAssociationLabel.Label}, " +
                              $"TypeID: {secondCustomAssociationLabel.AssociationTypeId}");
            Console.WriteLine($"* Creating another association ({secondCustomAssociationLabel.Name}, " +
                              $"ContactToCompany) between company: '{company.Name}' and '{randomContact.FirstName} " +
                              $"{randomContact.LastName}' ...");
            
            singleAssociation.AssociationTypes.Add(secondCustomAssociationLabel);

            association = api.Associations.CreateAssociation(singleAssociation);
            
            Console.WriteLine($"-> Association created! The following custom labels were applied to the association " +
                              $"between '{randomContact.FirstName} {randomContact.LastName}' and '{company.Name}': " +
                              $"{string.Join(", ", association.Result.Labels)}");            
            
            // Wait for HubSpot to catch up
            Utilities.Sleep(15);
            
           
            /*
             * Batch delete specific associations (labeled or unlabeled)
             */
            Console.WriteLine($"* Batch deleting previously created associations by explicitly specifying which to " +
                              $"delete");
            var associationList = new AssociationListHubSpotModel<AssociationHubSpotModel>
            {
                Associations = new List<AssociationHubSpotModel>
                {
                    new AssociationHubSpotModel
                    {
                        AssociationTypes = new List<AssociationTypeHubSpotModel>
                        {
                            firstCustomAssociationLabel,
                            secondCustomAssociationLabel
                        },
                        FromObject = new AssociationObjectIdModel
                        {
                            Id = randomContact.Id,
                            HubSpotObjectType = randomContact.HubSpotObjectType
                        },
                        ToObject = new AssociationObjectIdModel
                        {
                            Id = company.Id,
                            HubSpotObjectType = company.HubSpotObjectType
                        }                        
                    }
                }
            };
            var batchDeletedAssociations = api.Associations
                .BatchDeleteAssociations(associationList);
            
            /*
             * Delete all associations between the previously created contact & company
             */
            Console.WriteLine($"* Deleting all associations between '{randomContact.FirstName} " +
                              $"{randomContact.LastName}' and '{company.Name}'");
            var deletedAssociation = api.Associations.DeleteAllAssociations(association);
            Console.WriteLine($"-> All associations deleted between: '{deletedAssociation.FromObject.Id}' and " +
                              $"'{deletedAssociation.ToObject.Id}'");
            
            /*
             * Delete the previously created association types
             */
            Console.WriteLine($"* Deleting previously created association types: '{firstCustomAssociationLabel.Label}' " +
                              $"and '{secondCustomAssociationLabel.Label}'");
            api.Associations.DeleteAssociationType(randomContact.HubSpotObjectType, 
                company.HubSpotObjectType, firstCustomAssociationLabel.AssociationTypeId);
            
            api.Associations.DeleteAssociationType(randomContact.HubSpotObjectType, 
                company.HubSpotObjectType, secondCustomAssociationLabel.AssociationTypeId);
            
            
            /*
             * Delete the previously created company (cleanup)
             */
            Console.WriteLine($"* Deleting company with ID: {company.Id}");
            api.Company.Delete(company.Id);
            
            // TODO - add the ability to delete labels
            
            
            /*
             * Delete recently created contacts
             */
            Console.WriteLine($"* Deleting recently created contacts ...");
            foreach (var recentContact in recent.Contacts)
            {
                Console.WriteLine($"-> Deleting: {recentContact.FirstName} {recentContact.LastName} " +
                                  $"<{recentContact.Email}>");
                api.Contact.Delete(recentContact);
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
