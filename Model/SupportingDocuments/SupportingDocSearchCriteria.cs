using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SupportingDocuments.Enums;


namespace Iata.IS.Model.SupportingDocuments
{
  public class SupportingDocSearchCriteria
  {
    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public SupportingDocType Type { get; set; }

    public int SupportingDocumentTypeId { get; set; }

    public int BilledMemberId { get; set; }

    // Used in AutoPopulate
    public string BilledMemberText { set; get; }

    // Used in AutoPopulate
    public string BillingMemberText { set; get; }

    public int BillingMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int? SourceCodeId { get; set; }

    public Iata.IS.Model.Cargo.Enums.BillingCode DisplayBillingCode { get; set; }

    public int BillingCode { get; set; }

   // public int BillingCodeId { get; set; }

    public int? BatchSequenceNumber { get; set; }

    public int? RecordSequenceWithinBatch { get; set; }

    public string RMBMCMNumber { get; set; }

    public int? AWBSerialNumber { get; set; }

    public int? CheckDigit { get; set; }

    public long? TicketDocNumber { get; set; }

    public int? CouponNumber { get; set; }

    public int AttachmentIndicatorOriginal { get; set; }

    public int AttachmentIndicatorValidated { get; set; }

    public int AttachmentNumber { get; set; }

    public bool IsMismatchCases { get; set; }

    public string CutOffDateEventName { get; set; }

    public SupportingDocSearchCriteria(string cutOffDateEventName)
    {
      CutOffDateEventName = cutOffDateEventName;
    }

    public int ChargeCategoryId { get; set; }

    public SupportingDocSearchCriteria()
    {
    }

    public bool IsUatp { get; set; }
  }
}
