using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo
{
  public class AwbRecord : AWBBase
  {
    public Guid InvoiceId { get; set; }

    //public string ISValidationFlag { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    //public int RecordSequenceNumber { get; set; }

    public int BillingAirline { get; set; }

    public int BilledAirline { get; set; }

    public DateTime? DateOfCarriage { get; set; }

    public double? WeightCharges { get; set; }

    public double OtherCharges { get; set; }

    public double AmountSubjectToIsc { get; set; }

    public double IscPer { get; set; }

    public int? BilledWeight { get; set; }

    public string ProvisoReqSpa { get; set; }

    public int? ProratePer { get; set; }

    //public double AWBGrossValueOrApplicableLocalFare { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }

    public string PartShipmentIndicator { get; set; }

    public double? ValuationCharges { get; set; }

    public string KgLbIndicator { get; set; }

    //public string ProrateMethodology { get; set; }

    public DateTime ProrationDate { get; set; }

    public double? VatAmount { get; set; }

    //public double TaxAmount { get; set; }

    public int BillingCodeId { get; set; }

    public double IscAmount { get; set; }

    public double? AwbTotalAmount { get; set; }

    public bool CcaIndicator { get; set; }

    public string OurReference { get; set; }
    
    public int IsValidation { get; set; }

    public CargoInvoice Invoice { get; set; }


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

    public double OtherChargeVatSumAmount
    {
      get
      {
        return Convert.ToDouble(OtherChargeBreakdown.Sum(oc => oc.OtherChargeVatCalculatedAmount));
      }
    }

    public int OtherChargeCount
    {
      get
      {
        return Convert.ToInt32(OtherChargeBreakdown.Count());
      }
    }

    public List<AwbTax> TaxBreakdown { get; private set; }

    public List<AwbVat> VatBreakdown { get; private set; }

    public List<AwbAttachment> Attachments { get; set; }
    public List<AwbOtherCharge> OtherChargeBreakdown { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }

    public AwbRecord()
    {
      TaxBreakdown = new List<AwbTax>();
      VatBreakdown = new List<AwbVat>();
      Attachments = new List<AwbAttachment>();
      OtherChargeBreakdown = new List<AwbOtherCharge>();
    }
  }
}

