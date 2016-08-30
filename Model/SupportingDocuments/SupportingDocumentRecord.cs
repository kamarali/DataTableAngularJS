using System;

namespace Iata.IS.Model.SupportingDocuments
{
  /// <summary>
  /// This entity is required for mapping the Records searched for supporting documents/attachments
  /// </summary>
  public class SupportingDocumentRecord
  {

    public Guid RecordId { get; set; }
    public string RecordType { get; set; }
    public Guid AttachmentId { get; set; }
    public string AttachmentFileName { get; set; }
    public int BillingYear { get; set; }
    public int BillingMonth { get; set; }
    public int BillingPeriod { get; set; }
    public int BilledMemberId { get; set; }
    public string InvoiceNumber { get; set; }
    public int BatchNumber { get; set; }
    public int SequenceNumber { get; set; }
    public int BreakdownSerialNumber { get; set; }
    public string BilledMemberCommercialName { get; set; }
    public string IsFormC { get; set; }
    public string BilledMemberName { get; set; }

    /// <summary>
    /// Gets or sets the charge category.
    /// </summary>
    /// <value>
    /// The charge category.
    /// </value>
    public string ChargeCategory { get; set; }
    public string FormDInvoiceNumber { get; set; }

    public string BillingYearMonth
    {
      get
      {
        if (BillingMonth != 0 && BillingYear != 0)
        {
          return Convert.ToDateTime(BillingMonth + "-" + BillingYear).ToString("yyyy-MMM");
        }
        else
        {
          return string.Empty;
        }
      }
    }

  }
}
