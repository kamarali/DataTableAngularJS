using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Cargo
{
    public class RMBMCMSummaryCargoReceivableReport
    {
        public string BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public int PeriodNo { get; set; }
        public string SettlementMethod { get; set; }
        public string AirlineCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string MemoType { get; set; }
        public string MemoNumber { get; set; }
        public int Stage { get; set; }
        public string ReasonCode { get; set; }
        public string CurrencyCode { get; set; }
        public decimal WeightCharges { get; set; }
        public decimal ValuationCharges { get; set; }
        public decimal OtherChargeAmount { get; set; }
        public decimal IscAmount { get; set; }
        public decimal VatAmount { get; set; }
        public decimal NetAmount { get; set; }
        public int NumberofLinkedAwb { get; set; }
        public string AttachmentIndicatorOrig { get; set; }
        public int AttachmentIndicator { get; set; }
    }
}
