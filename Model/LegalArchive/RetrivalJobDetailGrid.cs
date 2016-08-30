using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    public class RetrivalJobDetailGrid :EntityBase<Guid>
    {
        public string Type { get; set; }
        public string MemberText { get; set; }
        public int MemberId { get; set; }
        public int BillingCategoryId { get; set; }
        public string BillingCategoryText { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public int BillingYear { get; set; }
        public int BillingMonthId { get; set; }
        public string BillingMonthText { get; set; }
        public int BillingPeriod { get; set; }
        public string BillingLocation { get; set; }
        public string BilledLocation { get; set; }
        public int SettlementId { get; set; }
        public string SettlementText { get; set; }
        public string ZipFile { get; set; }
        public string RetrievalJobId { get; set; }
        public string Action { get; set; }
        public bool IsFileExist { get; set; } //SCP442581 - legal archive in SIS
        //CMP #666: MISC Legal Archiving Per Location ID
        public string MiscLocationCode { get; set; }
    }
}
