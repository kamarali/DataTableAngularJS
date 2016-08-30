using System;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Reports
{
    public class ProcessingDashboardSearchEntity
    {
        // Property to get and set Billing category
        public int BillingCategory { get; set; }

        // Property to get and set Settlement Method
        public int SettlementMethod { get; set; }

        // Property to get and set Billing Year
        public int? BillingYear { set; get; }

        // Property to get and set Billing Month
        public int BillingMonth { set; get; }

        // Property to get and set Billing Period
        public int BillingPeriod { set; get; }

        // Property to get and set Billed Member
        public string BilledMember { set; get; }

        // Property to get and set Billing Member
        public string BillingMember { set; get; }

        public int? BillingMemberId { set; get; }

        public int? BilledMemberId { set; get; }

        // Property to get and set Invoice Status
        public int InvoiceStatus { set; get; }

        // Property to get and set File Status
        public int? FileStatus { set; get; }

        // Property to get and set File Format
        public int? FileFormat { set; get; }

        // Property to get and set File Name
        public string FileName { get; set; }

        // Property to get and set File Format
        public string DisableBillingTextBox { set; get; }

        // Property to get and set IS User Id
        public int IsUserId { set; get; }

        // Property to set sort coulmn names (used in file status tab)
        public string SortCoulmns { set; get; }

        // Property to set sort columns values (asc,desc,none) (used in file status tab)
        public string SortColumnsValues { set; get; }

        public bool IncludeProcessingDatesTimestamp { set; get; }

        public DateTime? FileGeneratedDateFrom { get; set; }

        public DateTime? FileGeneratedDateTo { get; set; }

        // Property to get and set Invoice/Form C No
        public string InvoiceNo { get; set; }

        // Property to get and set Unique Invoice No
        public string UniqueInvoiceNo { get; set; }

        // Property to get and set Claim Failed Only Invoices
        public bool IsShowClaimFailed { set; get; }

        //CMP559 : Add Submission Method Column to Processing Dashboard
        public int SubmissionMethodId { get; set; }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        public int DailyDeliverystatusId { get; set; }

    }// end ProcessingDashboardSearchEntity class
}// end namespace
