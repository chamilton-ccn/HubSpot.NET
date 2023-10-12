using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        AssociationTypeListHubSpotModel<T> CreateLabel<T>(T label, string fromObjectType, string toObjectType) 
            where T : AssociationTypeHubSpotModel, new();

        T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new();

        T DeleteAllAssociations<T>(T association) where T : AssociationHubSpotModel, new();
        AssociationListHubSpotModel<T> BatchDeleteAssociations<T>(AssociationListHubSpotModel<T> association)
            where T : AssociationHubSpotModel, new();

    }
}