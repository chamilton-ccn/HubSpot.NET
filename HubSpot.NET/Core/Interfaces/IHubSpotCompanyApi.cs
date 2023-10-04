using HubSpot.NET.Api;
using HubSpot.NET.Api.Company;
using HubSpot.NET.Api.Company.Dto;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotCompanyApi
    {
        T Create<T>(T entity) where T : CompanyHubSpotModel, new();
        T Update<T>(T entity) where T : CompanyHubSpotModel, new();
        void Delete(long companyId);
        void Delete(CompanyHubSpotModel company);
        T CreateOrUpdate<T>(T company) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> BatchCreate<T>(CompanyListHubSpotModel<T> companies) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> BatchCreateOrUpdate<T>(CompanyListHubSpotModel<T> companies) where T : CompanyHubSpotModel, new();        
        T GetById<T>(long companyId) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> List<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> Search<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> GetByDomain<T>(string domain, SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> RecentlyCreated<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        CompanyListHubSpotModel<T> RecentlyUpdated<T>(SearchRequestOptions opts = null) where T : CompanyHubSpotModel, new();
        // TODO - Decouple Associations
        //T GetAssociations<T>(T entity) where T : CompanyHubSpotModel, new();
    }
}