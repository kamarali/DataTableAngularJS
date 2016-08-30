using System.ComponentModel;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class UatpAtcanDetails
    {
        public int BillingMemberId { get; set; }

        public int BilledMemberId { get; set; }
        
        [DisplayName("Record Type")]
        public string RecordType { get; set; }

        [DisplayName("Clearance Month")]
        public string ClearanceMonth { get; set; }

        [DisplayName("Period")]
        public string Period { get; set; }
        
        [DisplayName("Airline/Supplier Code")]
        public string EntityCode { get; set; }

        [DisplayName("Airline/Supplier Name")]
        public string EntityName { get; set; }

        [DisplayName("Zone")]
        public string Zone { get; set; }

        [DisplayName("Billing Type")]
        public string BillingType { get; set; }

        [DisplayName("Settlement Method Indicator")]
        public string SettlementMethod { get; set; }

        [DisplayName("Invoice Number")]
        public string InvoiceNumber { get; set; }
        
        [DisplayName("Invoice Date")]
        public string InvoiceDate { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }
        
        [DisplayName("Original Invoice Number")] 
        public string OriginaInvoiceNumber { get; set; }

        [DisplayName("BSP/Original Invoice Date")]
        public string OriginalInvoiceDate { get; set; }

        [DisplayName("Daily Exchange Rate")]
        public decimal DailyExchangeRate { get; set; }

        [DisplayName("Signed For Currency")]
        public string SignedCurrencyCode { get; set; }

        [DisplayName("Signed For Currency Amount")]
        public decimal SignedAmount { get; set; }

        [DisplayName("Billing Currency")]
        public string ClearanceCurrency { get; set; }

        [DisplayName("Billing Currency Amount")]
        public decimal ClearanceAmount { get; set; }

        
    }
}
