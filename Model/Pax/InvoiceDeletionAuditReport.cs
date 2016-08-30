using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Pax
{
    public class InvoiceDeletionAuditReport
    {
        
        public string BillingCategory { get; set; }

        public string BillingMember { get; set; }

        public string BilledMember { get; set; }

        public int BillingYear { get; set; }

        public string BillingMonth { get; set; }

        public int BillingPeriod { get; set; }

        public string InvoiceNo { get; set; }

        public string FileName { get; set; }

        public string DeletedBy { get; set; }

        public DateTime DeteledOn { get; set; }
    }
}
