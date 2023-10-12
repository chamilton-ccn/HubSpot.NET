namespace HubSpot.NET.Core.Interfaces
{
    /// <summary>
    /// The base model for all HubSpot entities
    /// </summary>
    public interface IHubSpotModel
    {
        string RouteBasePath { get; }

        string HubSpotObjectType { get; }
    }
}
