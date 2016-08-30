using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    /// <summary>
    /// Represents Misc Codes in the system.
    /// </summary>

    [Serializable]
    public class MiscCode : MasterBase<int>
    {
        /// <summary>
        /// Get or Set Misc Group
        /// </summary>
        /// <value>The Misc Group </value>
        public int Group { get; set; }

        /// <summary>
        /// Get or Set Misc Code Description
        /// </summary>
        /// <value>The Misc Code Description</value>
        public string Description { get; set; }

        /// <summary>
        /// Get or Set Misc Code' Parent Group, if any 
        /// </summary>
        /// <value>The Misc Code' parent group</value>
        public string ParentGroup { get; set; }

        /// <summary>
        /// Get or Set Misc Code' Parent Code, if any
        /// </summary>
        /// <value>The Misc Code' parent code</value>
        public string ParentCode { get; set; }

        /// <summary>
        /// Get or Set Misc Code' IS SYS code
        /// </summary>
        /// <value>The Misc Code' IS SYS code</value>
        public string ISSYSCode { get; set; }

        /// <summary>
        /// Gets or sets the misc code group.
        /// </summary>
        /// <value>
        /// The misc code group.
        /// </value>
        public MiscCodeGroup MiscCodeGroup { get; set; }

        private string _miscCodeGroupName;

        /// <summary>
        /// Gets or sets the name of the misc code group.
        /// </summary>
        /// <value>
        /// The name of the misc code group.
        /// </value>
        public string MiscCodeGroupName
        {
            get
            {
                return !string.IsNullOrEmpty(_miscCodeGroupName) ? _miscCodeGroupName : MiscCodeGroup != null ? MiscCodeGroup.MiscGroup : string.Empty;
            }
            set
            {
                _miscCodeGroupName = value;
            } 
        }

    }
}
