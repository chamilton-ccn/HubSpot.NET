using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HubSpot.NET.Core.Errors
{
    public class ErrorsListItem
    {
        [DataMember(Name = "id")]
        public long Id { get; set; }
        
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }
        
        [DataMember(Name = "subCategory")]
        public dynamic SubCategory { get; set; }
        
        [DataMember(Name = "message")]
        public string Message { get; set; }
        
        [DataMember(Name = "correlationId")]
        public string CorrelationId { get; set; }
        
        [DataMember(Name = "code")]
        public string Code { get; set; }
        
        [DataMember(Name = "context")]
        public ErrorsListItemContext Context { get; set; } = new ErrorsListItemContext();
        
        [DataMember(Name = "links")]
        public object Links { get; set; }
        
        [DataMember(Name = "errors")]
        public IList<ErrorsListItem> Errors { get; set; }
    }
}