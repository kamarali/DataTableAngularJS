using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.LegalArchive
{
    [Serializable]
    public class LegalArchiveSearch : EntityBase<Guid>
    {
        public string MemberText { set; get; }
        public int MemberId { get; set; }
        public int CrBillingMemeberIsId { get; set; }
        public int DbBilledMemeberIsId { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceType { get; set; }
        public string InvoiceDate { get; set; }
        public int BillingYear { set; get; }
        public int BillingMonthId { set; get; }
        public string BillingMonthText { set; get; }
        public int BillingPeriod { set; get; }
        public int BillingCategoryId { get; set; }
        public string BillingCategoryText { get; set; }
        public string SettlementMethodText { get; set; }
        public  int SettlementMethodId { get; set; }
        public string BillingCountryCode { get; set; }
        public string BilledCountryCode { get; set; }
        public string ReceivablePayableIndicator { get; set; }
        public string LegalArchivalLocation { get; set; }
    }
}

