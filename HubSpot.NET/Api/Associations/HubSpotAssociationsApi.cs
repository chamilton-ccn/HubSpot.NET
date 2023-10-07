using System;
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

        public AssociationTypeListHubSpotModel<T> CreateLabel<T>(T label, string fromObjectTypeId, 
            string toObjectTypeId) where T : AssociationTypeHubSpotModel, new()
        {
            var path = $"{label.RouteBasePath}/associations/{fromObjectTypeId}/{toObjectTypeId}/labels";
            // TODO - remove debugging
            Console.WriteLine($"### PATH: {path}");
            // TODO - remove serialisationType parameter
            return _client.Execute<AssociationTypeListHubSpotModel<T>>(path, label, Method.Post, SerialisationType.PropertyBag);
        }

        public T CreateAssociation<T>(T association) where T : AssociationHubSpotModel, new()
        {
            var path = $"{association.RouteBasePath}/objects/{association.FromObject.HubSpotObjectType}/" +
                       $"{association.FromObject.Id}/{association.HubSpotObjectTypeIdPlural}/" +
                       $"{association.ToObject.HubSpotObjectType}/{association.ToObject.Id}";
            // TODO - remove debugging
            Console.WriteLine($"### PATH: {path}");
            // TODO - remove serialisationType parameter
            var data = _client.Execute<AssociationResultModel>(path, association.AssociationTypes, Method.Put,
                SerialisationType.PropertyBag);
            association.Result = data;
            return association;
        }
        
        
    }
}