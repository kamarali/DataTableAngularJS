using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.AutoBillingClosure
{
    public class EmailAutoBillingInvoices
    {
        public string InvoiceNo { get; set; }

        public string PeriodNo { get; set; }

        public string BillingMonth { get; set; }

        public int BillingYear { get; set; }

        public int BillingMemberId { get; set; }

        public string BillingMemberAlphaCode { get; set; }

        public string BillingMemberNumericCode { get; set; }

        public MemberStatus BillingMemberIsMembershipStatus { get; set; }

        public string BilledMemberAlphaCode { get; set; }

        public string BilledMemberNumericCode { get; set; }

        public MemberStatus BilledMemberIsMembershipStatus { get; set; }

        public string SisOpsEmailId { get; set; }

        

    }
}
