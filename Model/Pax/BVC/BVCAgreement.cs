using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Model.Pax
{
    /// <summary>
    /// Object to hold bvc agreement.
    /// </summary>
    public class BvcAgreement : EntityBase<int>
    {
        /// <summary>
        /// Billing Member and billed Member mapping id
        /// </summary>
        public int BvcMappingId { get; set; }

        /// <summary>
        /// Billed Member ID
        /// </summary>
        public int BilledMemberId { set; get; }

        /// <summary>
        /// Billed Member Name
        /// </summary>
        public string BilledMemberText { set; get; }
        
        /// <summary>
        /// Billing Member ID
        /// </summary>
        public int BillingMemberId { set; get; }

        /// <summary>
        /// Billing Member Name
        /// </summary>
        public string BillingMemberText { set; get; }

        /// <summary>
        /// Shows if member is Active
        /// </summary>
        public bool IsActive { get; set; }
    }
}