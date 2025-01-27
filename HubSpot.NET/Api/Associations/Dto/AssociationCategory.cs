﻿using System.Runtime.Serialization;

namespace HubSpot.NET.Api.Associations.Dto
{
    public enum AssociationCategory
    {
        [EnumMember(Value = "HUBSPOT_DEFINED")]
        HubSpotDefined,

        [EnumMember(Value = "USER_DEFINED")]
        UserDefined,
        
        [EnumMember(Value = "INTEGRATOR_DEFINED")]
        IntegratorDefined
    }
}