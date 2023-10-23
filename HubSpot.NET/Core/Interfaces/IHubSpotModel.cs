using System.Collections.Generic;
using HubSpot.NET.Api.Associations.Dto;

namespace HubSpot.NET.Core.Interfaces
{
    /// <summary>
    /// The base model for all HubSpot entities
    /// </summary>
    public interface IHubSpotModel
    {
        // TODO - Re-implement Id (see CompanyHubSpotModel). Also: This can't be used until after Model(s) and ModelList objects no longer share an interface
        //dynamic Id { get; set; }
        
        // TODO - Should "Properties" models have their own (empty) interface so we can ensure the Properties member is always implemented on IHubSpotModel(s)?
        //IHubSpotModelPropertiesModel Properties { get; set; }
        
        // TODO - Associations
        //IList<AssociationHubSpotModel> Associations { get; set; }
        //bool SerializeAssociations { get; set; }
        //bool ShouldSerializeAssociations();
        
        //TODO - DateTimeStamps
        //DateTime? CreatedAt {get; set;}
        //DateTime? UpdatedAt {get; set;}
        
        
        string RouteBasePath { get; }

        string HubSpotObjectType { get; }
    }
}
