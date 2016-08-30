using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp
{
    /// <summary>
    /// SCP85039
    /// </summary>
    public class MiscInvoiceSearchDetails
    {
        public Guid Id { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int IsLegalPdfGenerated { get; set; }

        public int DigitalSignatureStatusId { get; set; }

        public int BillingMonth { get; set; }

        public int BillingPeriod { get; set; }

        public int BillingYear { get; set; }

        public string DisplayBillingPeriod { get; set; }

        public int BilledMemberId { get; set; }

        public int BillingMemberId { get; set; }

        public string BilledMemberText { get; set; }

        public string BillingMemberText { get; set; }

        public int InvoiceOwnerId { get; set; }

        public string InvoiceOwnerDisplayText { get; set; }

        public string InvoiceNumber { get; set; }

        public int InvoiceStatusId { set; get; }

        public string InvoiceStatusDisplayText { set; get; }

        public string ChargeCategoryDisplayName { get; set; }

        public int SettlementMethodId { get; set; }

        public string SettlementMethodDisplayText { get; set; }

        public string ListingCurrencyDisplayText { get; set; }

        public string BillingCurrencyDisplayText { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal BillingAmount { get; set; }

        public int SubmissionMethodId { get; set; }

        public string SubmissionMethodDisplayText { get; set; }

        public string InputFileNameDisplayText { get; set; }

        public int InvoiceTypeId { get; set; }

        public string InvoiceTypeDisplayText { get; set; }

        //SCP#390702 - KAL: Issue with Clearance Amount. Desc: Clearance Amount is made nullable.
        public decimal? ClearanceAmount { get; set; }

        public int ChargeCategoryId { get; set; }

        public string LocationCode { get; set; }

        /// <summary>
        /// Gets or sets the billing category id.
        /// </summary>
        /// <value>
        /// The billing category id.
        /// </value>
        public int BillingCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the rejection stage.
        /// </summary>
        /// <value>The rejection stage.</value>
        public int RejectionStage { get; set; }

        public int InputFileStatusId { get; set; }

        public int TotalRows { get; set; }

        public int RecordNo { get; set; }

        //CMP #655: IS-WEB Display per Location ID
        public string BillingMemLocationCode { get; set; }

    }
}
