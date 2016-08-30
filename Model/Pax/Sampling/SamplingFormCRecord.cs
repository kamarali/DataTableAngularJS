using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormCRecord : EntityBase<Guid>
  {
    public SamplingFormC SamplingFormC { get; set; }

    public Guid SamplingFormCId { get; set; }

    public long DocumentNumber { get; set; }

    public int CouponNumber { get; set; }

    public string IssueAirline { get; set; }

    public string ReasonCode { get; set; }

    public int SourceCodeId { get; set; }

    public string ProvisionalInvoiceNumber { get; set; }

    public int BatchNumberOfProvisionalInvoice { get; set; }

    public int RecordSeqNumberOfProvisionalInvoice { get; set; }

    public double GrossAmountAlf { get; set; }

    public string TicketIssuingAirline { get; set; }

    public bool ElectronicTicketIndicator { get; set; }

    public string NfpReasonCode { get; set; }

    public string AgreementIndicatorSupplied { get; set; }

    public string AgreementIndicatorValidated { get; set; }

    public string OriginalPmi { get; set; }

    public string ValidatedPmi { get; set; }

    public int AttachmentIndicatorOriginal { get; set; }

    public bool? AttachmentIndicatorValidated { get; set; }

    public int? NumberOfAttachments { get; set; }

    public string Remarks { get; set; }

    public List<SamplingFormCRecordAttachment> Attachments { get; set; }

    public SamplingFormCRecord()
    {
      Attachments = new List<SamplingFormCRecordAttachment>();
    }

    public string ETicketIndicator
    {
      get
      {
        return ElectronicTicketIndicator ? "E" : string.Empty;
      }
    }

    public string ReasonCodeDescription { get; set; }

    //Number of child records required in case of IDEC validations.
    public long NumberOfChildRecords { get; set; }

    /// <summary>
    /// ErrorCorrectable = 1, ErrorNonCorrectable = 2,Validated = 3
    /// </summary>
    public int TransactionStatusId { set; get; }

    public TransactionStatus TransactionStatus
    {
      get
      {
        return (TransactionStatus)TransactionStatusId;
      }
      set
      {
        TransactionStatusId = Convert.ToInt32(value);
      }
    }
  }
}
