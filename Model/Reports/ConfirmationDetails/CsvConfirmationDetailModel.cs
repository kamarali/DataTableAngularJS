using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ConfirmationDetails
{

    public class CsvConfirmationDetailModel
    {
        [DisplayName("Billing Airline")]
        public string BillingAirline { get; set; }

        [DisplayName("Billing Airline Number")]
        public string BillingAirlineNumber { get; set; }

        [DisplayName("Period No")]
        public int BillingPeriod { get; set; }
        // public int PeriodNo { get; set; }

        [DisplayName("Clearance Month")]
        public int ClearanceMonth { get; set; }

        [DisplayName("Clearance Year")]
        public int ClearanceYear { get; set; }

        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }

        [DisplayName("Billed Airline")]
        public string BilledAirline { get; set; }

        [DisplayName("Billed Airline Number")]
        public string BilledAirlineNumber { get; set; }

        [DisplayName("Month Of Sale")]
        public string MonthOfSales { get; set; }

        [DisplayName("Year Of Sale")]
        public string YearofSales { get; set; }

        [DisplayName("Issuing Airline")]
        public string IssuingAirline { get; set; }

        [DisplayName("Document Number")]
        public Int64 DocumentNumber { get; set; }

        [DisplayName("Coupon Number")]
        public int CouponNumber { get; set; }

        [DisplayName("Original PMI")]
        public string OriginalPMI { get; set; }

        [DisplayName("Validated PMI")]
        public string ValidatedPMI { get; set; }

        [DisplayName("Agreement Indicator Supplied")]
        public string AgreementIndicatorSupplied { get; set; }

        [DisplayName("Agreement Indicator Validated")]
        public string AgreementIndicatorValidated { get; set; }

        [DisplayName("Prorate Methodology Supplied")]
        public string ProrateMethodologySupplied { get; set; }

        [DisplayName("Prorate Methodology Validated")]
        public string ProrateMethodologyValidated { get; set; }

        [DisplayName("NFP Reason Code Supplied")]
        public string NFPReasonCodeSupplied { get; set; }

        [DisplayName("NFP Reason Code Validated")]
        public string NFPReasonCodeValidated { get; set; }

        [DisplayName("Billed Amount (USD)")]
        public double BilledAmountUSD { get; set; }

        [DisplayName("Prorate Amount as per ATPCO Database (USD)")]
        public double ProrateAmountasperATPCO { get; set; }

        [DisplayName("Prorate Amount Base Currency Supplied")]
        public string ProrateAmountBaseSupplied { get; set; }

        [DisplayName("Prorate Amount Base Currency as per ATPCO Database")]
        public string ProrateAmountBaseATPCO { get; set; }

        [DisplayName("Billed Total Tax Amount (USD)")]
        public double BilledTotalTaxAmountUSD { get; set; }

        [DisplayName("Total Tax Amount as per ATPCO Database (USD)")]
        public double TotalTaxAmountasperATPCO { get; set; }

        [DisplayName("Published Tax Amount Currency 1 as per ATPCO Database")]
        public string PublishedTaxAmountCurrency1 { get; set; }

        [DisplayName("Published Tax Amount Currency 2 as per ATPCO Database")]
        public string PublishedTaxAmountCurrency2 { get; set; }

        [DisplayName("Published Tax Amount Currency 3 as per ATPCO Database")]
        public string PublishedTaxAmountCurrency3 { get; set; }

        [DisplayName("Published Tax Amount Currency 4 as per ATPCO Database")]
        public string PublishedTaxAmountCurrency4 { get; set; }

        [DisplayName("Billed ISC %")]
        public double BilledISCPer { get; set; }

        [DisplayName("ISC Fee % as per ATPCO Database")]
        public double ISCFeePer { get; set; }

        [DisplayName("Billed Handling Fee Amount (USD)")]
        public double BilledHandlingFeeAmountUSD { get; set; }

        [DisplayName("Handling Fee Amount as per ATPCO Database (USD)")]
        public double HandlingFeeAmount { get; set; }

        [DisplayName("Handling Fee Base Currency as per ATPCO Database")]
        public string HandlingFeeBaseCurrency { get; set; }

        [DisplayName("Billed UATP %")]
        public double BilledUATPPercentage { get; set; }

        [DisplayName("UATP % as per ATPCO Database")]
        public double UATPPercentage { get; set; }

        [DisplayName("ATPCO Reason Code")]
        public string ATPCOReasonCode { get; set; }


    }
}
