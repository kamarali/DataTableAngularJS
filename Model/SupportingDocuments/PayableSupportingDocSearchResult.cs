using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Base;
using Iata.IS.Model.SupportingDocuments.Enums;

namespace Iata.IS.Model.SupportingDocuments
{
  public class PayableSupportingDocSearchResult : EntityBase<Guid>
  {
      public int BillingCodeId { get; set; }
      public Cargo.Enums.BillingCode DisplayBillingCode
      {
          get
          {
              return (Cargo.Enums.BillingCode)BillingCodeId;
          }
      }

      public int? AWBSerialNumber { get; set; }

      public int IsFormCAttachmentAllowed { get; set; }

    public string BillingPeriod { get; set; }

    public int SupportingDocTypeId { get; set; }

    public SupportingDocType Type 
    {
      get
      {
        return (SupportingDocType)SupportingDocTypeId;
      }
    }

    public string SupportingDocTypeText
    {
      get
      {
        if (Type == SupportingDocType.MiscInvoice || Type == SupportingDocType.UatpInvoice)
        {
          return Enum.GetName(typeof (SupportingDocType), Type);  
        }
        
        return EnumList.SupportingDocTypeValues[Type];
        
        //return Type.ToString();
      }
    }

    public int BilledMemberId { get; set; }

    //// Used in AutoPopulate
    public string BilledMemberText { set; get; }

    public int BillingMemberId { get; set; }

    // Used in AutoPopulate
    public string BillingMemberText { set; get; }

    public string InvoiceNumber { get; set; }

    public int SourceCodeId { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public string RMBMCMNumber { get; set; }

    public long TicketDocNumber { get; set; }

    public int CouponNumber { get; set; }

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

    public Guid RecordId { get; set; }

    public Guid InvoiceId { get; set; }

    public int RecordType { get; set; }

    public string ChargeCategory { get; set; }

    public SupportingDocRecordType TypeofRecord
    {
      get
      {
        return (SupportingDocRecordType)RecordType;
      }
    }
  }
}
