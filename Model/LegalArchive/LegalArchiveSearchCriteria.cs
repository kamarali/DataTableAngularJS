using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    [Serializable]
    public class LegalArchiveSearchCriteria : EntityBase<Guid>
    {
        public LegalArchiveSearchCriteria()
        {
        }

        // Used in AutoPopulate
        public string MemberText { set; get; }
        public int MemberId { get; set; }
        public string InvoiceNumber { get; set; }
        public int BillingYear { set; get; }
        public int BillingMonth { set; get; }
        public int BillingPeriod { set; get; }
        public int BillingCategoryId { get; set; }
        public int SettlementMethodId { get; set; }
        public string BillingCountryCode { get; set; }
        public string BilledCountryCode { get; set; }
        public int Type { get; set; }
        public string ArchivalLocationId { get; set; }
    }
}
