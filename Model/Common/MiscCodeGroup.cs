using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MiscCodeGroup : MasterBase<int>, ICacheable
    {

        /// <summary>
        /// Gets or sets the misc group.
        /// </summary>
        /// <value>
        /// The misc group.
        /// </value>
        public string MiscGroup { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

    }
}
