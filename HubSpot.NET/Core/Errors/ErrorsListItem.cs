using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Errors
{
    public class ErrorsListItem
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
        
        [DataMember(Name = "message")]
        public string Message { get; set; }
        
        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; set; }

        [DataMember(Name = "context")]
        public ErrorsListItemContext Context { get; set; } = new ErrorsListItemContext();
    }
}