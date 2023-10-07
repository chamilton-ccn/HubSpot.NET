using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Api.Company.Dto;


namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        AssociationTypeListHubSpotModel<T> CreateLabel<T>(T label, string fromObjectTypeId, string toObjectTypeId) 
            where T : AssociationTypeHubSpotModel, new();

        T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new();

    }
}