using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotCompanyApi
    {
        CompanyListHubSpotModel<T> BatchArchive<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> BatchCreate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> BatchRead<T>(CompanyListHubSpotModel<T> companies,
            SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> BatchUpdate<T>(CompanyListHubSpotModel<T> companies)
            where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> List<T>(SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new();
        T Create<T>(T company) where T : CompanyHubSpotModel, new();
        T GetByUniqueId<T>(string uniqueId, SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new();
        T GetByUniqueId<T>(long uniqueId, SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new();
        T GetByUniqueId<T>(int uniqueId, SearchRequestOptions opts = null)
            where T : CompanyHubSpotModel, new();
        T Update<T>(T company, string idProperty = null) where T : CompanyHubSpotModel, new();
        void Delete(long companyId);
        void Delete(CompanyHubSpotModel company);
        CompanyListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        
        // TODO - Decouple Associations
        //T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new();
    }
}