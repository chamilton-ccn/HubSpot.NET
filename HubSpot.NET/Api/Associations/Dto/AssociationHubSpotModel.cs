﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.Paging;

// ReSharper disable InconsistentNaming

namespace HubSpot.NET.Api.Associations.Dto
{
    [DataContract]
    public class AssociationHubSpotModel : IHubSpotModel
    {
        [IgnoreDataMember]
        public IList<AssociationTypeHubSpotModel> AssociationTypes { get; set; }
        public bool ShouldSerializeAssociationTypes() => false; // TODO - not sure if we need to keep this

        [DataMember(Name = "types", EmitDefaultValue = false)]
        private IList<AssociationTypeHubSpotModel> _types
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        
        [DataMember(Name = "associationTypes", EmitDefaultValue = false)]
        private IList<AssociationTypeHubSpotModel> _associationTypes
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        public bool ShouldSerialize_associationTypes() => false;
        
        [DataMember(Name = "associationSpec", EmitDefaultValue = false)]
        private IList<AssociationTypeHubSpotModel> _associationSpec
        {
            get => AssociationTypes;
            set => AssociationTypes = value;
        }
        public bool ShouldSerialize_associationSpec() => false;
        
        // TODO - consider implementing a property that accepts a dynamic object that will be used to create the AssociationObjectIdModel
        [DataMember(Name = "from", EmitDefaultValue = false)]
        public AssociationObjectIdModel FromObject { get; set; } = new AssociationObjectIdModel();
        
        [IgnoreDataMember]
        public AssociationObjectIdModel ToObject { get; set; } = new AssociationObjectIdModel();
        
        [DataMember(Name = "to", EmitDefaultValue = false)]
        private AssociationObjectIdModel _to
        {
            get => ToObject;
            set => ToObject = value;
        }

        [DataMember(Name = "toObjectId", EmitDefaultValue = false)]
        private long _toObject
        {
            get => ToObject.Id;
            set => ToObject.Id = value;
        }
        
        [DataMember(Name = "paging", EmitDefaultValue = false)]
        public PagingModel Paging { get; set; }
        public bool ShouldSerializePaging() => false;
        
        
        [IgnoreDataMember]
        public AssociationResultModel Result { get; set; } = new AssociationResultModel();
        
        [IgnoreDataMember]
        public string HubSpotObjectType => "associations";
        
        [IgnoreDataMember]
        public string RouteBasePath => "/crm/v4";
    }
}