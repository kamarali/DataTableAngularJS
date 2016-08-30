using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    public class RetrivalJobSummaryGrid : EntityBase<Guid>
    {
        public string RequestedBy { get; set; }
        public string RequestedOn { get; set;}
        public string JobId { get; set; }
        public string JobStatus { get; set; }
        public string InvoiceNumber { get; set; }
        public string Type { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonthId { get; set; }
        public string BillingMonthText { get; set; }
        public int BillingPeriod { get; set; }
        public string BillingPeriodText { get; set;}
        public string Member { get; set; }
        public int BillingCategoryId { get; set; }
        public string BillingCategoryText { get; set;}
        public string BilledingLocation { get; set; }
        public string BillingLocation { get; set; }
        public int SettlementId { get; set; }
        public string SettlementText { get; set; }
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscLocationCodes { get; set; }
    }
}
