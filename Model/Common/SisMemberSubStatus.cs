using System;
using Iata.IS.Model.Base;
namespace Iata.IS.Model.Common
{
    [Serializable]
    public class SisMemberSubStatus : MasterBase<int>
    {
  

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        public SisMemberStatus SisMemberStatus { get; set; }

        public string BeforeUpdateMemberSubStatus { get; set; }

        #region CMP #665: Added four property in the class SisMemberSubStatus.

        /// <summary>
        /// Gets or sets a value indicating whether this instance is SuppressOtpEmail.
        /// </summary>
        public bool SuppressOtpEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is RedirectUponLogin.
        /// </summary>
        public bool RedirectUponLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is LimitedMemProfileAccess.
        /// </summary>
        public bool LimitedMemProfileAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is DisableUserProfileUpdates.
        /// </summary>
        public bool DisableUserProfileUpdates { get; set; }

        #endregion

       //public string MemberStatus { get { return this.SisMemberStatus.MemberStatus; } }
    }
}

