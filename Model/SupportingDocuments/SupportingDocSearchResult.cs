using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Base;
using Iata.IS.Model.SupportingDocuments.Enums;

namespace Iata.IS.Model.SupportingDocuments
{
  public class SupportingDocSearchResult : EntityBase<Guid>
  {

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public int BillingYear { get; set; }

    public string DisplayBillingMonthYear { get; set; }

    public int SupportingDocTypeId { get; set; }

    public SupportingDocType Type 
    {
      get
      {
        return (SupportingDocType)SupportingDocTypeId;
      }
    }

    public string SupportingDocTypeText { get; set; }

    public int BilledMemberId { get; set; }

    public string BilledMemberText { set; get; }

    public string BillingMemberText { set; get; }

    public int BillingMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int SourceCodeId { get; set; }

    public Iata.IS.Model.Cargo.Enums.BillingCode DisplayBillingCode { get; set; }

    public string BillingCode { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public string RMBMCMNumber { get; set; }

    public string AWBSerialNumber { get; set; }

    public int? CheckDigit { get; set; }

    public long TicketDocNumber { get; set; }

    public int CouponNumber { get; set; }

    public int AttachmentIndicatorOriginal { get; set; }

    public string AttachmentIndicatorOriginalText { get; set; }

    public bool AttachmentIndicatorValidated { get; set; }

    public string AttachmentIndicatorValidatedText
    {
      get
      {
        if (AttachmentIndicatorValidated)
          return SupportingDocAttachmentIndicator.Yes.ToString();
        else
          return SupportingDocAttachmentIndicator.No.ToString();
      }
    }

    public int AttachmentNumber { get; set; }

    public Guid InvoiceId { get; set; }

    public int RecordType { get; set; }

    public string ChargeCategory { get; set; }

    public string CutOffDateEventName { get; set; }

    public int IsFormCAttachmentAllowed { get; set; }

    public SupportingDocRecordType TypeofRecord
    {
      get
      {
        return (SupportingDocRecordType)RecordType;
      }
    }
  }
}
