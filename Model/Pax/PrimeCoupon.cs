using System;
using System.Collections.Generic;
using Iata.IS.Core;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Model.Pax
{
  public class PrimeCoupon : CouponBase
  {
    public Guid InvoiceId { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public double CouponGrossValueOrApplicableLocalFare { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }
  
    public bool ElectronicTicketIndicator { get; set; }

    public string AirlineFlightDesignator { get; set; }

    public int? FlightNumber { get; set; }

    public DateTime? FlightDate { get; set; }    

    public double CouponTotalAmount { get; set; }

    public string CabinClass { get; set; }

    public string ProrateMethodology { get; set; }

    public DateTime ProrationDate { get; set; }
    
    public double SurchargeAmount { get; set; }

    public double VatAmount { get; set; }

    public double TaxAmount { get; set; }

    public string FilingReference { get; set; }

    public string ReasonCode { get; set; }

    public int SourceCodeId { get; set; }

    public SourceCode SourceCode { get; set; }

    public double IscPercent { get; set; }

    public double IscAmount { get; set; }

    public double HandlingFeeAmount { get; set; }

    public string HandlingFeeTypeId { get; set; }

    public double OtherCommissionPercent { get; set; }

    public double OtherCommissionAmount { get; set; }

    public double UatpPercent { get; set; }

    public double UatpAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField2 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField3 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField4 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField5 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string AirlineOwnUse { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

    public PaxInvoice Invoice { get; set; }

    public string ETicketIndicator
    {
      get
      {
        if(ElectronicTicketIndicator)
          return "E";
        else
          return string.Empty;
      }
    }
    public string AirlineTicketDocOrFimNumber
    {
      get
      {
        return string.Format("{0:D3}", TicketOrFimIssuingAirline) + " " + TicketDocOrFimNumber;
      }
    }

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

    // Filels requried during AutoBilling ISR.

    public string MonthOfIataFiveDayRate { set; get; }

    public string SettlementCouponProrationType { set; get; }

    public string SettlementCouponCurrencyType { set; get; }

    public string HandlingFeeCurrency { set; get; }

    public string HandlingFeeAgreementType { set; get; }

    public double HandlingFeeChargePercentage { set; get; }

    public List<PrimeCouponTax> TaxBreakdown { get; set; }

    public List<PrimeCouponVat> VatBreakdown { get; set; }

    public List<PrimeCouponAttachment> Attachments { get; set; }

    public PrimeCouponMarketingDetails PrimeCouponMarketingDetailRecord { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }

    public PrimeCoupon()
    {
        TaxBreakdown = new List<PrimeCouponTax>();
        VatBreakdown = new List<PrimeCouponVat>();
        Attachments = new List<PrimeCouponAttachment>();
    }

      // For Auto Billing Invoices - Added by Priya R.

    public bool IncludeInDailyRevenueRecogn { set; get; }

    public int AutoBillingCouponStatus { set; get; }

    public Guid? IsInputFileId { get; set; }

    public string IsInputFileDisplayId
    {
      get
      {
        return IsInputFileId.HasValue ? IsInputFileId.Value.Value() : string.Empty;
      }
    }

  }
}