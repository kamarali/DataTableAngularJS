using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Cargo
{
    public class CargoInvoiceSearchDetails
    {
        public Guid Id { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public int IsLegalPdfGenerated { get; set; }

        public int DigitalSignatureStatusId { get; set; }

        public int BillingMonth { get; set; }

        public int BillingPeriod { get; set; }

        public int BillingYear { get; set; }

        public string DisplayBillingMonthYear { get; set; }

        public int BilledMemberId { get; set; }

        public int BillingMemberId { get; set; }

        public string BilledMemberText { get; set; }

        public string BillingMemberText { get; set; }

        public int InvoiceOwnerId { get; set; }

        public string InvoiceOwnerDisplayText { get; set; }

        public int BillingCodeId { get; set; }

        //public string DisplayBillingCode { get; set; }

        public string InvoiceNumber { get; set; }

        public int InvoiceStatusId { set; get; }

        public string InvoiceStatusDisplayText { set; get; }

        public int SettlementMethodId { get; set; }

        public string SettlementMethodDisplayText { get; set; }

        public string ListingCurrencyDisplayText { get; set; }

        public string BillingCurrencyDisplayText { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal ListingAmount { get; set; }

        public decimal BillingAmount { get; set; }

        public int SubmissionMethodId { get; set; }

        public string SubmissionMethodDisplayText { get; set; }

        public string InputFileNameDisplayText { get; set; }

        public int InputFileStatusId { get; set; }

        public int TotalRows { get; set; }

        public int RecordNo { get; set; }

        /// <summary>
        /// SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions [Cargo Receivables]
        /// </summary>
        public int InvoiceTypeId { get; set; }
    }
}
