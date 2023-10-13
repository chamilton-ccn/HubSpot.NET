using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        CustomAssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(string fromObjectType, 
            string toObjectType, T label = null) where T : CustomAssociationTypeHubSpotModel, new();

        void DeleteAssociationType(string fromObjectType, string toObjectType, long associationTypeId);

        T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new();

        T DeleteAllAssociations<T>(T association) where T : AssociationHubSpotModel, new();
        AssociationListHubSpotModel<T> BatchDeleteAssociations<T>(AssociationListHubSpotModel<T> association)
            where T : AssociationHubSpotModel, new();

    }
}