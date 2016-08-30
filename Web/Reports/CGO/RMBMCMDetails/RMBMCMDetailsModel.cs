using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;



namespace Iata.IS.Web.Reports.CGO.RMBMCMDetails
{
    public class RMBMCMDetailsModel
    {
        public string ClearanceMonth { get; set; }
        public int PeriodNo { get; set; }
        public string BillingType { get; set; }
        public string SettlementMethod { get; set; }
        public string AirlineCode { get; set; }
        public string InvoiceNumber { get; set; }
        public string MemoType { get; set; }
        public int AwbNumber { get; set; }
        public string MemoNumber { get; set; }
        public int Stage { get; set; }
        public string ReasonCode { get; set; }
        public string CurrencyCode { get; set; }
        public string WeightChargesBilled { get; set; }
        public string WeightChargesAccepted { get; set; }
        public string WeightCharges { get; set; }
        public string ValuationChargesBilled { get; set; }
        public string ValuationChargesAccepted { get; set; }
        public string ValuationCharges { get; set; }
        public string OtherChargeAmountBilled { get; set; }
        public string OtherChargeAmountAccepted { get; set; }
        public string OtherChargeAmount { get; set; }
        public string IscAmountAllowed { get; set; }
        public string IscAmountAccepted { get; set; }
        public string IscAmount { get; set; }
        public string VatAmountBilled { get; set; }
        public string VatAmountAccepted { get; set; }
        public string VatAmount { get; set; }
        public string NetAmount { get; set; }
        public int NumberofLinkedAwb { get; set; }
        public int AttachmentIndicatorOrig { get; set; }
        public string AttachmentIndicator { get; set; }
        
    }
}