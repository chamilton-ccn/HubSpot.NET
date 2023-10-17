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
        /// TODO - needs cleanup
        //public CustomAssociationTypeListHubSpotModel<T> CreateCustomAssociationType<T>(string fromObjectType, 
        //string toObjectType, T associationType = null) where T : CustomAssociationTypeHubSpotModel, new()
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
        /// <param name="fromObjectType"></param>
        /// <param name="toObjectType"></param>
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
        
        
        // TODO - List all associations for a given object instance to another object type
        // Example URL: GET /crm/v4/objects/{fromObjectType}/{fromObject.Id}/associations/{toObjectType}?limit=500
        
        

        /// <summary>
        /// Creates a single association with potentially multiple labels/association types between two objects
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
        /// Deletes specified associations between two objects
        /// </summary>
        /// <param name="associationList">An AssociationListHubSpotModel</param>
        /// <typeparam name="T">AssociationHubSpotModel</typeparam>
        /// <returns>The provided AssociationListHubSpotModel instance</returns>
        public AssociationListHubSpotModel<T> BatchDeleteAssociations<T>(AssociationListHubSpotModel<T> associationList)
            where T : AssociationHubSpotModel, new()
        {
            var objectRelationshipsInList = new List<(string, string)> {};
            objectRelationshipsInList.AddRange(associationList.Associations
                .Select(associationType => (associationType.FromObject.HubSpotObjectType,
                    associationType.ToObject.HubSpotObjectType)).Distinct());
            
            foreach (var i in objectRelationshipsInList)
            {
                // TODO - remove debugging
                Console.WriteLine($"The following object types with associations have been found:");
                Console.WriteLine($"{i.Item1} -> {i.Item2}");
                var path = $"{associationList.RouteBasePath}/associations/{i.Item1}/" +
                           $"{i.Item2}/batch/labels/archive";
                // TODO - remove debugging
                Console.WriteLine($"### PATH: {path}");
                // TODO - remove serialisationType parameter
                _client.Execute<AssociationListHubSpotModel<T>>(path, associationList, Method.Post, 
                    SerialisationType.PropertyBag);
            }
            return associationList;
        }        
        
        
    }
}