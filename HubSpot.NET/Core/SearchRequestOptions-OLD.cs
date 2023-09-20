using System;
using System.Collections.Generic;

namespace HubSpot.NET.Core
{
    /// <summary>
    /// Options used when querying for lists of items.
    /// </summary>
    public class SearchRequestOptions
    {
        /// <summary>
        /// If limit isn't specified, it to the maximum allowable number of results per page.
        /// </summary>
        private int _limit = 100;
        private readonly int _upperLimit;

        /// <summary>
        /// Gets or sets the number of items to return.
        /// </summary>
        /// <remarks>
        /// Defaults to 20 which is also the HubSpot API default. Max value is 100
        /// </remarks>
        /// <value>
        /// The number of items to return.
        /// </value>
        public virtual int Limit
        {
            get => _limit;
            set
            {
                if (value < 1 || value > _upperLimit)
                    throw new ArgumentException($"Number of items to return must be a positive integer greater " +
                                                $"than 0, and less than {_upperLimit} (you provided {value}).");
                _limit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HubSpot.NET.Core.SearchRequestOptions"/> class.
        /// </summary>
        /// <param name="upperLimit">Upper limit for the amount of items to request for the list.</param>
        public SearchRequestOptions(int upperLimit)
        {
            _upperLimit = upperLimit;
            if (_limit > upperLimit)
                _limit = upperLimit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:HubSpot.NET.Core.SearchRequestOptions"/> class.
        /// Sets the upper limit to 100.
        /// <see href="https://developers.hubspot.com/docs/api/crm/contacts#limits">
        /// Hubspot only allows a maximum of 100 results per page.
        /// </see> 
        /// </summary>
        public SearchRequestOptions() : this(100) {}

        /// <summary>
        /// Get or set the continuation offset when calling list many times to enumerate all your items
        /// </summary>
        /// <remarks>
        /// The return DTO from List contains the current "offset" that you can inject into your next list call 
        /// to continue the listing process
        /// </remarks>
        public virtual long? Offset { get; set; } = null;

        public virtual List<string> PropertiesToInclude { get; set; } = new List<string>();
    }
}
