using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// This entity is required for mapping the Records searched for Duplicate InvoiceNumber.
  /// </summary>
  public class InvoiceDuplicateRecord
  {
    public DateTime FileSubmissionDate { get; set; }
    public string InvoiceNo { get; set; }
    public string FileName { get; set; }
    public int BatchSeqNo { get; set; }
    public int RecordSeqNo { get; set; }
    public string BillingMemberCode { get; set; }
    public string BilledMemberCode { get; set; }
    public int BillingMemberId { get; set; }
    public int BillingCategoryId { get; set; }
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }
    public int BillingPeriod { get; set; }
  }
}
