using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormDRecord : EntityBase<Guid>
  {
    public SamplingFormDRecord()
    {
      TaxBreakdown = new List<SamplingFormDTax>();
      VatBreakdown = new List<SamplingFormDVat>();
      Attachments = new List<SamplingFormDRecordAttachment>();
    }

    public string ProvisionalInvoiceNumber { get; set; }

    public int BatchNumberOfProvisionalInvoice { get; set; }

    public int RecordSeqNumberOfProvisionalInvoice { get; set; }

    public string TicketIssuingAirline { get; set; }

    public int CouponNumber { get; set; }

    public long TicketDocNumber { get; set; }

    public double ProvisionalGrossAlfAmount { get; set; }

    public double EvaluatedGrossAmount { get; set; }

    public double EvaluatedNetAmount { get; set; }

    public string ProrateMethodology { get; set; }

    public string NfpReasonCode { get; set; }

    public string AgreementIndicatorSupplied { get; set; }

    public string AgreementIndicatorValidated { get; set; }

    public string OriginalPmi { get; set; }

    public string ValidatedPmi { get; set; }

    public double IscPercent { get; set; }

    public double IscAmount { get; set; }

    public double HandlingFeeAmount { get; set; }

    //public string PmiBillingCarrier { get; set; }

    public int ProvisionalBillingMonth { get; set; }

    public int SourceCodeId { get; set; }

    public double OtherCommissionPercent { get; set; }

    public double OtherCommissionAmount { get; set; }

    public double UatpPercent { get; set; }

    public double UatpAmount { get; set; }

    public int AttachmentIndicatorOriginal { get; set; }

    public bool? AttachmentIndicatorValidated { get; set; }

    public int? NumberOfAttachments { get; set; }

    public string ReferenceField1 { get; set; }

    public string ReferenceField2 { get; set; }

    public string ReferenceField3 { get; set; }

    public string ReferenceField4 { get; set; }

    public string ReferenceField5 { get; set; }

    public string AirlineOwnUse { get; set; }

    public string ProrateSlipDetails { get; set; }

    public List<SamplingFormDRecordAttachment> Attachments { get; set; }

    public List<SamplingFormDTax> TaxBreakdown { get; private set; }

    public List<SamplingFormDVat> VatBreakdown { get; private set; }

    public double VatAmount { get; set; }

    public double TaxAmount { get; set; }

    public PaxInvoice Invoice { get; set; }

    public Guid InvoiceId { get; set; }

    public string ReasonCode { get; set; }

    /// <summary>
    /// Property added to display data in grid
    /// </summary>
    public string AirlineTicketDocNumber
    {
      get
      {
        return string.Format("{0:D3}", TicketIssuingAirline) + TicketDocNumber;
      }
    }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
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

    /// <summary>
    /// Expiry period for purging.
    /// </summary>
    public DateTime? ExpiryDatePeriod { get; set; }
  }
}
