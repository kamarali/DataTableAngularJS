
using System.ComponentModel;

namespace Iata.IS.Model.ExternalInterfaces.IATAInterface
{
    /// <summary>
    /// SisIsWebUsageData 
    /// </summary>
    /// CMP #659: SIS IS-WEB Usage Report.
    public class SisIsWebUsageRptData
    {
        [DisplayName("Member Code")]
        public string MemberCode { get; set; }

        [DisplayName("Billing Year/Month")]
        public int YearAndMonth { get; set; }

        [DisplayName("Billing Period")]
        public int PeriodNo { get; set; }

        [DisplayName("User")]
        public string UserName { get; set; }

        [DisplayName("Username / Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("PAX Invoices Submitted in IS-WEB")]
        public int PaxInvoices { get; set; }

        [DisplayName("CGO Invoices Submitted in IS-WEB")]
        public int CgoInvoices { get; set; }

        [DisplayName("MISC Invoices Submitted in IS-WEB")]
        public int MiscInvoices { get; set; }

        [DisplayName("Prime Coupons (PAX)")]
        public int PaxPrimeCoupons { get; set; }

        [DisplayName("Billing Memos (PAX)")]
        public int PaxBillingMemos { get; set; }

        [DisplayName("Credit Memos (PAX)")]
        public int PaxCreditMemos { get; set; }

        [DisplayName("Sampling UAF Coupons (PAX)")]
        public int PaxSamplingUafCoupons { get; set; }

        [DisplayName("Sampling Digit Eval. Coupons (PAX)")]
        public int PaxSamplingDigitEvalCoupons { get; set; }

        [DisplayName("Rejection Memos (Incl Sampling) (PAX)")]
        public int PaxRejectionMemosInclSampling { get; set; }

        [DisplayName("Original Billing (AWBs) (CGO)")]
        public int CgoOriginalBillingAwbs { get; set; }

        [DisplayName("Rejection Memos (CGO)")]
        public int CgoRejectionMemos { get; set; }

        [DisplayName("Billing Memos (CGO)")]
        public int CgoBillingMemos { get; set; }

        [DisplayName("Credit Memos (CGO)")]
        public int CgoCreditMemos { get; set; }

        [DisplayName("Correspondences (PAX)")]
        public int PaxCorrespondences { get; set; }

        [DisplayName("Correspondences (CGO)")]
        public int CgoCorrespondences { get; set; }

        [DisplayName("Correspondences (MISC)")]
        public int MiscCorrespondences { get; set; }

        [DisplayName("Correspondences (UATP)")]
        public int UatpCorrespondences { get; set; }

        [DisplayName("PAX (Supp. Doc in kb)")]
        public int PaxSupportingDocs { get; set; }

        [DisplayName("CGO (Supp. Doc in kb)")]
        public int CgoSupportingDocs { get; set; }

        [DisplayName("MISC (Supp. Doc in kb)")]
        public int MiscSupportingDocs { get; set; }

        [DisplayName("UATP (Supp. Doc in kb)")]
        public int UatpSupportingDocs { get; set; }

        [DisplayName("PAX (Digi.Sign inv. sent)")]
        public int PaxDigiSignValidSent { get; set; }

        [DisplayName("CGO (Digi.Sign inv. sent)")]
        public int CgoDigiSignValidSent { get; set; }

        [DisplayName("MISC (Digi.Sign inv. sent)")]
        public int MiscDigiSignValidSent { get; set; }

        [DisplayName("PAX (e-Archive Count)")]
        public int PaxEArchiving { get; set; }

        [DisplayName("CGO (e-Archive Count)")]
        public int CgoEArchiving { get; set; }

        [DisplayName("MISC (e-Archive Count)")]
        public int MiscEArchiving { get; set; }

        [DisplayName("PAX (e-Archive Size in KBs)")]
        public long PaxTotalSizeEArchiving { get; set; }

        [DisplayName("CGO (e-Archive Size in KBs)")]
        public long CgoTotalSizeEArchiving { get; set; }

        [DisplayName("MISC (e-Archive Size in KBs)")]
        public long MiscTotalSizeEArchiving { get; set; }
    }
}
