using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.Reports.ConfirmationDetails
{
    public class ConfirmationDetailReportModel
    {
        public string BillingAirline { get; set; }        
        public string BillingAirlineNumber { get; set; }
        public int BillingPeriod { get; set; }
        public int ClearanceMonth { get; set; }
        public int ClearanceYear { get; set; }
        public string InvoiceNumber { get; set; }
        public string BilledAirline { get; set; }
        public string BilledAirlineNumber { get; set; }
        public string MonthofSales { get; set; }
        public string YearofSales { get; set; }
        public string IssuingAirline { get; set; }
        public int DocumentNumber { get; set; }
        public int CouponNumber { get; set; }
        public string OriginalPMI { get; set; }
        public string ValidatedPMI { get; set; }
        public string AgreementIndicatorSupplied { get; set; }
        public string AgreementIndicatorValidated { get; set; }
        public string ProrateMethodologySupplied { get; set; }
        public string ProrateMethodologyValidated { get; set; }
        public string NFPReasonCodeSupplied { get; set; }
        public string NFPReasonCodeValidated { get; set; }
        public decimal BilledAmountUSD { get; set; }
        public decimal ProrateAmountasperATPCO { get; set; }
        public string ProrateAmountBaseSupplied { get; set; }
        public string ProrateAmountBaseATPCO { get; set; }
        public decimal BilledTotalTaxAmountUSD { get; set; }
        public decimal TotalTaxAmountasperATPCO { get; set; }
        public string PublishedTaxAmountCurrency1 { get; set; }
        public string PublishedTaxAmountCurrency2 { get; set; }
        public string PublishedTaxAmountCurrency3 { get; set; }
        public string PublishedTaxAmountCurrency4 { get; set; }
        public decimal BilledISCPer { get; set; }
        public double ISCFeePer { get; set; }
        public decimal BilledHandlingFeeAmountUSD { get; set; }
        public decimal HandlingFeeAmount { get; set; }
        public string HandlingFeeBaseCurrency { get; set; }
        public decimal BilledUATPPercentage { get; set; }
        public double UATPPercentage { get; set; }
        public string ATPCOReasonCode { get; set; }

        /// <summary>
        /// property to get and set time on report
        /// </summary>
        public string ReportGeneratedDate { get; set; }
    }
}