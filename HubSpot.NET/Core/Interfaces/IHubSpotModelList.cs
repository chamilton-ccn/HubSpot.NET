// TODO - Experimental! Attempting to standardize some functionality in Models & ModelLists. Might delete later.

using System;
using System.Collections.Generic;
using HubSpot.NET.Core.Errors;
using HubSpot.NET.Core.Paging;
using HubSpot.NET.Core.Search;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotModelList<T> where T : IHubSpotModel, new()
    {
        IList<T> Inputs { get; set; }
        
        IList<T> Results { get; set; }
        
        bool ShouldSerializeResults();
        
        string Status { get; }
        
        long? Total { get; }
        
        long? TotalErrors { get; }
        
        IList<ErrorsListItem> Errors { get; }

        bool ShouldSerializeErrors();
        
        DateTime RequestedAt { get; }
        
        DateTime StartedAt { get; }
        
        DateTime CompletedAt { get; }
        
        bool MoreResultsAvailable { get; }
        
        long? Offset { get; }
        
        PagingModel Paging { get; }
        
        SearchRequestOptions SearchRequestOptions { get; }
        
        string IdProperty { get; }
        
        IList<string> PropertiesWithHistory { get; }
        
        string HubSpotObjectType { get; }
        
        string RouteBasePath { get; }
        
    }
}