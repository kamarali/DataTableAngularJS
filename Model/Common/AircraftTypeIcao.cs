using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    [Serializable]
    public class AircraftTypeIcao : MasterBase<string>
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}
