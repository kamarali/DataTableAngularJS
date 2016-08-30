using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
    public class DuplicateFormCRecordLog : EntityBase<Guid>
    {
        public Guid FormCId { get; set; }
        public string TicketIssuingAirline { get; set; }
        public int CouponNo { get; set; }
        public int TicketDocNo { get; set; }
        public string ProvisionalInvoiceNo { get; set; }
        public int ProvisionalYear { get; set; }
        public int ProvisionalMonth { get; set; }
        public string FromMemberNumeric { get; set; }
        public string ProvMemberNumeric { get; set; }
        public int SourceCode { get; set; }
        public int BatchSeqNo { get; set; }
        public int BatchRecSeqNo { get; set; }
        public Guid IsFileLogId { get; set; }
        public string CsvProcessId { get; set; }
    }
}
