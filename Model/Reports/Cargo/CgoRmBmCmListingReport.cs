using System.ComponentModel;

namespace Iata.IS.Model.Reports.Cargo
{
    public class CgoRmBmCmListingReport
    {
        [DisplayName("Sr No")]
        public string SerialNo { get; set; }

        [DisplayName("Billing Airline Code")]
        public string BillingAirlineCode { get; set; }

        [DisplayName("Billed Airline Code")]
        public string BilledAirlineCode { get; set; }

        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }

        [DisplayName("Billing Month/Year")]
        public string BillingMonthYear { get; set; }

        [DisplayName("Period No")]
        public string PeriodNo { get; set; }

        [DisplayName("Billing Code")]
        public string BillingCode { get; set; }

        [DisplayName("Batch Number")]
        public string BatchNo { get; set; }

        [DisplayName("Seq No")]
        public string SequenceNo { get; set; }

        [DisplayName("Rejection Memo/Billing Memo/Credit Memo")]
        public string MemoNumber { get; set; }

        [DisplayName("Stage No")]
        public string StageNo { get; set; }

        [DisplayName("Currency of Listing")]
        public string CurrencyOfListing { get; set; }

        [DisplayName("Weight Charges")]
        public string WeightChargeAmount { get; set; }

        [DisplayName("Valuation Charges")]
        public string ValuationChargeAmount { get; set; }

        [DisplayName("Other Charges")]
        public string OtherCommissionAmount { get; set; }

        [DisplayName("ISC Amount")]
        public string IscAmount { get; set; }

        [DisplayName("VAT Amount")]
        public string VatAmount { get; set; }

        [DisplayName("Net Amount")]
        public string NetRejectCreditAmount { get; set; }

        [DisplayName("Reason Code")]
        public string ReasonCode { get; set; }

        [DisplayName("Reason Description")]
        public string ReasonDescription { get; set; }


    }
}