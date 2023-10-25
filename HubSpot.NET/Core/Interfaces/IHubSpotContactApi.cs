using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotContactApi
    {
        ContactListHubSpotModel<T> BatchArchive<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchCreate<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchRead<T>(ContactListHubSpotModel<T> contacts,
            SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> BatchUpdate<T>(ContactListHubSpotModel<T> contacts)
            where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        public T Create<T>(T contact) where T : ContactHubSpotModel, new();
        T GetByUniqueId<T>(string uniqueId, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        T GetByUniqueId<T>(long uniqueId, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        T GetByUniqueId<T>(int uniqueId, SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
        T Update<T>(T contact, string idProperty = null) where T : ContactHubSpotModel, new();
        void Delete(long contactId);
        void Delete(ContactHubSpotModel contact);
        T GetByUserToken<T>(string userToken) where T : ContactHubSpotModel, new();
        ContactListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : ContactHubSpotModel, new();
    }
}
