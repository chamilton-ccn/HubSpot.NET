﻿using HubSpot.NET.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HubSpot.NET.Core.Search;

namespace HubSpot.NET.Api.Deal.Dto
{
    public class DealRecentRequestOptions : SearchRequestOptions
    {
        /// <summary>
        /// Used to specify the oldest timestamp to use to retrieve deals
        /// </summary>
        public string Since { get; set; }

        /// <summary>
        /// Specififes if the current value for a property should be fetched or all historical values
        /// </summary>
        public bool IncludePropertyVersion { get; set; } = false;
    }
}
