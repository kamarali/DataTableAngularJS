using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Cargo
{
    public class RMBMCMDetailsModel
    {
        public string ClearanceMonth { get; set; }
        public int PeriodNo { get; set; }
        public string BillingType { get; set; }
        public string SettlementMethod { get; set; }
        public string MemoType { get; set; }
        public string DataSource { get; set; }
        public string AirlineCode { get; set; }
        public string AirlineId { get; set; }
        public int InvoiceNumber { get; set; }
        public string Output { get; set; }
        public int RMBMCMNumber { get; set; }
        public int billingYear { get; set; }
    }
}
