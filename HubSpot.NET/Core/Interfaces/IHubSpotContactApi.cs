using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotContactApi
    {
        T Create<T>(T entity) where T : ContactHubSpotModel, new();
        T Update<T>(T contact) where T : ContactHubSpotModel, new();
        void Delete(long contactId);
        void Delete(ContactHubSpotModel contact);
        T CreateOrUpdate<T>(T company) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchCreate<T>(ContactListHubSpotModel<T> contacts) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchCreateOrUpdate<T>(ContactListHubSpotModel<T> contacts) where T : ContactHubSpotModel, new();
        T GetByEmail<T>(string email, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        T GetById<T>(long contactId) where T : ContactHubSpotModel, new();
        T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        }
}
