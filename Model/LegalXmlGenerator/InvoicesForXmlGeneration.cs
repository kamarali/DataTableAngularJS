using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.LegalXmlGenerator
{
    public class InvoicesForXmlGeneration
    {
        public Guid InvoiceId { get; set; }

         public string BilledMemLocCode { get; set; }

        public string BillingMemLocCode { get; set; }

        public int DsRequiredId { get; set; }

        public string DsRequiredBy { get; set; }

        public string DsStatus { get; set; }

        public int BilledmemberId { get; set; }

        public int DigitalSignStatus { get; set; }

        public int BillingCtaegoryId { get; set; }

        public int BillingCodeId { get; set; }

        public string InvoiceNo { get; set; }

        public string MemberCodeAlpha { get; set; }

        public string MemberCodeNumeric { get; set; }

        public string BilledMemberAlphaCode { get; set; }

        public string BilledMemberNumCode { get; set; }

         public int BillingPeriod { get; set; }

         public int BillingMonth { get; set; }

         public int BillingYear { get; set; }

         public string InvTemplateLanguage { get; set; }

    }
}
