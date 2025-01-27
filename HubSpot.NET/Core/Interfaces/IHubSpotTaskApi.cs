﻿using HubSpot.NET.Api.Task.Dto;
using System.Collections.Generic;
using HubSpot.NET.Api;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Core.Interfaces
{
    public interface IHubSpotTaskApi
    {
        T Create<T>(T entity) where T : TaskHubSpotModel, new();
        void Delete(long dealId);
        T GetById<T>(long dealId, List<string> propertiesToInclude = null) where T : TaskHubSpotModel, new();
        T Update<T>(T entity) where T : TaskHubSpotModel, new();

        TaskListHubSpotModel<T> List<T>(SearchRequestOptions opts = null)
            where T : TaskHubSpotModel, new();
    }
}