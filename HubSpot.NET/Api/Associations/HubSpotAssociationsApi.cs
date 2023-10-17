using System;
using System.Collections.Generic;
using System.Linq;
using HubSpot.NET.Api.Associations.Dto;
using HubSpot.NET.Core;
using HubSpot.NET.Core.Interfaces;
using RestSharp;

namespace HubSpot.NET.Api.Associations
{
    public class HubSpotAssociationsApi : IHubSpotAssociationsApi
    {
        private readonly IHubSpotClient _client;

        public HubSpotAssociationsApi(IHubSpotClient client)
        {
            _client = client;
        }
        
        // TODO - [Batch] [Delete] Batch delete associations for objects
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/archive
        
        // TODO - [Batch] [Create Default Associations] Create the default (most generic) association type between two object types
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/associate/default
        
        /// <summary>
        /// Batch create associations for objects
        /// </summary>
        /// <param name="associationList">An AssociationListHubSpotModel instance</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>AssociationBatchResultModel</returns>
        public List<AssociationBatchResultModel> BatchCreateAssociations<T> (AssociationListHubSpotModel<T> associationList) 
            where T : AssociationHubSpotModel, new()
        {
            var objectTypePairs = new List<(string, string)> {};
            objectTypePairs.AddRange(associationList.Associations
                .Select(associationType => (associationType.FromObject.HubSpotObjectType,
                    associationType.ToObject.HubSpotObjectType)).Distinct());

            var results = new List<AssociationBatchResultModel>();
            foreach (var objectTypePair in objectTypePairs)
            {
                /*var associations = associationList.Associations
                    .Where(a => (a.FromObject.HubSpotObjectType == objectTypePair.Item1) 
                                && (a.ToObject.HubSpotObjectType == objectTypePair.Item2));*/
                var filteredAssociationsList = new AssociationListHubSpotModel<T>
                {
                    Associations =  associationList.Associations
                        .Where(a => (a.FromObject.HubSpotObjectType == objectTypePair.Item1) 
                                    && (a.ToObject.HubSpotObjectType == objectTypePair.Item2)).ToList()
                };
                var path = $"{associationList.RouteBasePath}/associations/{objectTypePair.Item1}/" +
                           $"{objectTypePair.Item2}/batch/create";
                // TODO - remove debugging
                Console.WriteLine($"### PATH: {path}");
                // TODO - remove serialisationType parameter
                var result = _client.Execute<AssociationBatchResultModel>(path, filteredAssociationsList, Method.Post, 
                    SerialisationType.PropertyBag);
                results.Add(result);
            }
            return results;
        }
        
        /// <summary>
        /// Deletes specified association labels between two objects
        /// </summary>
        /// <param name="associationList">An AssociationListHubSpotModel instance</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>The provided AssociationListHubSpotModel instance</returns>
        /// <remarks>
        /// Deleting an unlabeled association will also delete all labeled associations between two objects
        /// </remarks>
        public AssociationListHubSpotModel<T> BatchDeleteAssociationLabels<T>
            (AssociationListHubSpotModel<T> associationList) where T : AssociationHubSpotModel, new()
        {
            var objectRelationshipsInList = new List<(string, string)> {};
            objectRelationshipsInList.AddRange(associationList.Associations
                .Select(associationType => (associationType.FromObject.HubSpotObjectType,
                    associationType.ToObject.HubSpotObjectType)).Distinct());
            
            foreach (var i in objectRelationshipsInList)
            {
                var path = $"{associationList.RouteBasePath}/associations/{i.Item1}/" +
                           $"{i.Item2}/batch/labels/archive";
                // TODO - remove debugging
                Console.WriteLine($"### PATH: {path}");
                // TODO - remove serialisationType parameter
                // TODO - associationList needs to be filtered to ensure only the objects matching the to/from object types are in the request.
                _client.Execute<AssociationListHubSpotModel<T>>(path, associationList, Method.Post, 
                    SerialisationType.PropertyBag);
            }
            return associationList;
        }
        
        // TODO - [Batch] [Read] Batch read associations for objects to a specific object type
        // Example URL: POST /crm/v4/associations/{fromObjectType}/{toObjectType}/batch/read        
        
        // TODO - [Basic] [Create Default] Create the default (most generic) association type between two object types
        
        
        /// <summary>
        /// List all associations of an object by object type. Limit 500 per call.
        /// </summary>
        /// <param name="fromObjectType">The source object type</param>
        /// <param name="fromObjectId">The numeric ID of the source object instance</param>
        /// <param name="toObjectType">The destination object type</param>
        /// <param name="limit">The number of records to return (500 maximum)</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>AssociationListHubSpotModel</returns>
        public AssociationListHubSpotModel<T> ListAssociationsByObjectType<T>(string fromObjectType, 
            long fromObjectId, string toObjectType, int limit = 500) where T : AssociationHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/objects/{fromObjectType}/{fromObjectId}/associations/{toObjectType}" +
                       $"?limit={limit}";
            // TODO - remove debugging
            Console.WriteLine(path);
            // TODO - remove serialisationType parameter
            return _client.Execute<AssociationListHubSpotModel<T>>(path, null, Method.Get,
                SerialisationType.PropertyBag);
        }
        
        /// <summary>
        /// Set association labels between two records.
        /// </summary>
        /// <param name="association">An AssociationHubSpotModel instance</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>The provided AssociationHubSpotModel instance, updated to include the result of the operation</returns>
        public T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new()
        {
            var path = $"{association.RouteBasePath}/objects/{association.FromObject.HubSpotObjectType}/" +
                       $"{association.FromObject.Id}/{association.HubSpotObjectType}/" +
                       $"{association.ToObject.HubSpotObjectType}/{association.ToObject.Id}";
            // TODO - remove debugging
            Console.WriteLine($"### PATH: {path}");
            // TODO - remove serialisationType parameter
            var data = _client.Execute<AssociationResultModel>(path, association.AssociationTypes, Method.Put,
                SerialisationType.PropertyBag);
            association.Result = data;
            return association;
        }
        
        /// <summary>
        /// Deletes all associations between two objects
        /// </summary>
        /// <param name="association"></param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>The provided AssociationHubSpotModel instance</returns>
        public T DeleteAllAssociations<T>(T association) where T : AssociationHubSpotModel, new()
        {
            var path = $"{association.RouteBasePath}/objects/{association.FromObject.HubSpotObjectType}/" +
                       $"{association.FromObject.Id}/{association.HubSpotObjectType}/" +
                       $"{association.ToObject.HubSpotObjectType}/{association.ToObject.Id}";
            // TODO - remove serialisationType parameter
            _client.Execute(path, null, Method.Delete, SerialisationType.PropertyBag);
            return association;
        }
        
        /// <summary>
        /// Creates a single custom association type/label
        /// </summary>
        /// <param name="fromObjectType">The source object type</param>
        /// <param name="toObjectType">The destination object type</param>
        /// <param name="associationType">The name of the label</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>An AssociationTypeListHubSpotModel instance</returns>
        /// <remarks>This is used to create a custom association type/label that can be used when creating the actual
        /// association between two objects. It does not create an association on its own.</remarks>
        public AssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(string fromObjectType, 
            string toObjectType, T associationType = null) where T : AssociationTypeHubSpotModel, new()
        {
            associationType = associationType ?? new T();
            var path = $"{associationType.RouteBasePath}/associations/{fromObjectType}/{toObjectType}/labels";
            // TODO - remove debugging
            Console.WriteLine($"### PATH: {path}");
            // TODO - remove serialisationType parameter
            return _client.Execute<AssociationTypeListHubSpotModel<T>>(path, associationType, Method.Post, 
                SerialisationType.PropertyBag);
        }

        /// <summary>
        /// Deletes a single custom association type/label
        /// </summary>
        /// <param name="fromObjectType">The source object type</param>
        /// <param name="toObjectType">The destination object type</param>
        /// <param name="associationTypeId">The numeric ID of the label to be deleted</param>
        /// <remarks>This is used to delete a custom association type/label.</remarks>
        public void DeleteAssociationType(string fromObjectType, string toObjectType, long associationTypeId)
        {
            var path = $"{new AssociationTypeHubSpotModel().RouteBasePath}/associations/{fromObjectType}/" +
                       $"{toObjectType}/labels/{associationTypeId}";
            // TODO - remove serialisationType parameter
            _client.Execute(path, null, Method.Delete, SerialisationType.PropertyBag);
        }

        /// <summary>
        /// List association types between two object types.
        /// <a href="https://developers.hubspot.com/docs/api/crm/associations#:~:text=Retrieve%20association%20types">
        /// See the documentation
        /// </a> for more details.
        /// </summary>
        /// <param name="fromObjectType">The source object type</param>
        /// <param name="toObjectType">The destination object type</param>
        /// <typeparam name="T">AssociationTypeHubSpotModel</typeparam>
        /// <returns>AssociationTypeListHubSpotModel</returns>
        public AssociationTypeListHubSpotModel<T> ListAssociationTypes<T>(string fromObjectType, string toObjectType) 
            where T : AssociationTypeHubSpotModel, new()
        {
            var path = $"{new T().RouteBasePath}/associations/{fromObjectType}/{toObjectType}/labels";
            // TODO - remove debugging
            Console.WriteLine(path);
            // TODO - remove serialisationType parameter
            return _client.Execute<AssociationTypeListHubSpotModel<T>>(path, null, Method.Get,
                SerialisationType.PropertyBag);
        }
    }
}