using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// Represents ICH Zone.
    /// </summary>

    [Serializable]
    public class IchZone : MasterBase<int>, ICacheable
    {

        /// <summary>
        /// Gets or sets the Zone.
        /// </summary>
        /// <value>The ICH Zone.</value>
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the Clearance Currency.
        /// </summary>
        /// <value>The Clearance Currency.</value>
        public string ClearanceCurrency { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        /// <value>The ICH Zone Description.</value>
        public string Description { get; set; }
    }
}
