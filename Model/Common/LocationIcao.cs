using System;
using Iata.IS.Model.Base;


namespace Iata.IS.Model.Common
{
    [Serializable]
    public class LocationIcao : MasterBase<string>
    {
        /// <summary>
        /// Gets or sets the country code.
        /// </summary>
        /// <value>
        /// The country code.
        /// </value>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}

