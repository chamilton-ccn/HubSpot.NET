using System.Collections.Generic;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Contact;
using HubSpot.NET.Api.Contact.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotContactApi
    {
        T Create<T>(T entity) where T : ContactHubSpotModel, new();
        T Update<T>(T contact) where T : ContactHubSpotModel, new();
        void Delete(long contactId);
        void Delete(ContactHubSpotModel contact);
        T CreateOrUpdate<T>(T entity) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchCreateOrUpdate<T>(ContactListHubSpotModel<T> entities) where T : ContactHubSpotModel, new();
        T GetByEmail<T>(string email, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        T GetById<T>(long contactId) where T : ContactHubSpotModel, new();
        T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        
        ContactListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        }
}
