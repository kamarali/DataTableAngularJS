using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// Represents Format of file.
    /// </summary>

    [Serializable]
    public class BillingCategory : MasterBase<int>, ICacheable
    {

        /// <summary>
        /// Gets or sets the Billing category code ISXML
        /// </summary>
        /// <value>The Billing category code ISXML.</value>
        public string CodeIsxml { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The Billing category Description.</value>
        public string Description { get; set; }

        
    }
}
