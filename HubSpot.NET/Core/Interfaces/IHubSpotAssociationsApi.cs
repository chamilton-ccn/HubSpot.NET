using System.Collections.Generic;
using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotAssociationsApi
    {
        // TODO - [Batch] [Delete] Batch delete associations for objects
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/archive
        
        // TODO - [Batch] [Create Default Associations] Create the default (most generic) association type between two object types
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/associate/default
        List<AssociationBatchResultModel> BatchCreateAssociations<T>(AssociationListHubSpotModel<T> associationList) where T : AssociationHubSpotModel, new();
        
        AssociationListHubSpotModel<T> BatchDeleteAssociationLabels<T>(AssociationListHubSpotModel<T> association)
            where T : AssociationHubSpotModel, new();
        
        // TODO - [Batch] [Read] Batch read associations for objects to a specific object type
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/read        
        
        // TODO - [Basic] [Create Default] Create the default (most generic) association type between two object types
        
        AssociationListHubSpotModel<T> ListAssociationsByObjectType<T>(string fromObjectType,
            long fromObjectId, string toObjectType, int limit = 500) where T : AssociationHubSpotModel, new();
        
        T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new();
        
        T DeleteAllAssociations<T>(T association) where T : AssociationHubSpotModel, new();
        
        AssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(T associationType = null, 
            string fromObjectType = null, string toObjectType = null) where T : AssociationTypeHubSpotModel, new();

        AssociationTypeHubSpotModel UpdateCustomAssociationType<T>(T associationType,
            int? associationTypeId = null, string fromObjectType = null, string toObjectType = null)
            where T : AssociationTypeHubSpotModel, new();

        void DeleteAssociationType(string fromObjectType, string toObjectType, long associationTypeId);

        AssociationTypeListHubSpotModel<T> ListAssociationTypes<T>(string fromObjectType, string toObjectType) 
            where T : AssociationTypeHubSpotModel, new();
    }
}