using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Cargo
{
    public class CgoAwbListingReport
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

        [DisplayName("Batch No")]
        public string BatchNo { get; set; }

        [DisplayName("Sequence No")]
        public string SequenceNo { get; set; }


        [DisplayName("AWB Issue Date")]
        public string AwbIssueDate { get; set; }

        [DisplayName("Issue Airline")]
        public string IssueAirline { get; set; }

        [DisplayName("AWB Serial No")]
        public string AwbSerialNo { get; set; }

        [DisplayName("Check Digit")]
        public string CheckDigit { get; set; }

        [DisplayName("Consignment Orig")]
        public string ConsignmentOrig { get; set; }

        [DisplayName("Consignment Dest")]
        public string ConsignmentDest { get; set; }

        [DisplayName("Carriage From")]
        public string CarriageFrom { get; set; }

        [DisplayName("Carriage To")]
        public string CarriageTo { get; set; }

        [DisplayName("Date of Carriage")]
        public string CarriageDate { get; set; }

        [DisplayName("Currency of Listing")]
        public string ListingCurrency { get; set; }

        [DisplayName("Weight Charges")]
        public string WeightCharges { get; set; }

        [DisplayName("Valuation Charges")]
        public string ValuationCharges { get; set; }

        [DisplayName("Other Charges")]
        public string OtherCharges { get; set; }

        [DisplayName("Amount Sub to ISC")]
        public string WtChargesSubIS { get; set; }

        [DisplayName("ISC Rate")]
        public string IscRate { get; set; }

        [DisplayName("ISC Amount")]
        public string IscAmount { get; set; }

        [DisplayName("VAT Amount")]
        public string VatAmount { get; set; }

        [DisplayName("AWB Total Amount")]
        public string AwbTotalAmount { get; set; }

        [DisplayName("Curr Adjustment Ind")]
        public string CurrAdjustmentInd { get; set; }

        [DisplayName("Billed Weight")]
        public string BilledWeight { get; set; }

        [DisplayName("KG/LB Ind")]
        public string KgLbInd { get; set; }

        [DisplayName("Part Shipment")]
        public string PartShipment { get; set; }

        [DisplayName("Proviso/Req/SPA  Ind")]
        public string ProvisoReqSpaInd { get; set; }

        [DisplayName("CCA Ind")]
        public string CcaInd { get; set; }

        [DisplayName("Prorate%")]
        public string ProratePercent { get; set; }
    }
}
