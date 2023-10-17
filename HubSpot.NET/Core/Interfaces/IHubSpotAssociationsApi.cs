using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        //CustomAssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(string fromObjectType, 
        //    string toObjectType, T label = null) where T : CustomAssociationTypeHubSpotModel, new();
        
        AssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(string fromObjectType, 
            string toObjectType, T label = null) where T : AssociationTypeHubSpotModel, new();

        void DeleteAssociationType(string fromObjectType, string toObjectType, long associationTypeId);

        AssociationTypeListHubSpotModel<T> ListAssociationTypes<T>(string fromObjectType, string toObjectType) 
            where T : AssociationTypeHubSpotModel, new();

        AssociationListHubSpotModel<T> ListAssociationsByObjectType<T>(string fromObjectType,
            long fromObjectId, string toObjectType, int limit = 500) where T : AssociationHubSpotModel, new();

        T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new();

        T DeleteAllAssociations<T>(T association) where T : AssociationHubSpotModel, new();
        AssociationListHubSpotModel<T> BatchDeleteAssociations<T>(AssociationListHubSpotModel<T> association)
            where T : AssociationHubSpotModel, new();

    }
}