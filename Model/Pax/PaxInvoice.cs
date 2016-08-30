using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.Pax
{
  public class PaxInvoice : InvoiceBase
  {
    public int ProvisionalBillingMonth { get; set; }

    public int ProvisionalBillingYear { get; set; }

    public string DisplayProvisionalBillingMonthYear
    {
      get
      {
        if (ProvisionalBillingMonth > 0 && ProvisionalBillingYear > 0 )
          return string.Format("{0}-{1}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(ProvisionalBillingMonth).ToUpper(), ProvisionalBillingYear);
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets the listing amount.
    /// </summary>
    /// <value>The listing amount.</value>
    /// <remarks>
    /// The Listing Amount is derived from NetTotal property of <see cref="InvoiceBase.InvoiceTotalRecord"/>.
    /// Since, this property is not stored in database, no setter method is provided.
    /// </remarks>
    public decimal ListingAmount
    {
      get
      {
        if (BillingCode == (int)Enums.BillingCode.SamplingFormDE)
        {
          return SamplingFormEDetails == null ? 0.0M : SamplingFormEDetails.NetAmountDue;
        }
        return InvoiceTotalRecord == null ? 0.0M : InvoiceTotalRecord.NetTotal;
      }
    }

    /// <summary>
    /// Gets the billing amount.
    /// </summary>
    /// <value>The billing amount.</value>
    /// <remarks>
    /// The billing Amount is derived from NetBillingAmount property of <see cref="InvoiceBase.InvoiceTotalRecord"/>.
    /// Since, this property is not stored in database, no setter method is provided.
    /// </remarks>
    public decimal BillingAmount
    {
      get
      {
        if (BillingCode == (int)Enums.BillingCode.SamplingFormDE)
        {
          return SamplingFormEDetails == null ? 0.0M : SamplingFormEDetails.NetAmountDueInCurrencyOfBilling;
        }
        return InvoiceTotalRecord == null ? 0.0M : InvoiceTotalRecord.NetBillingAmount;
      }
    }

    public bool SendIncremental { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public InvoiceType InvoiceType
    {
      get
      {
        return (InvoiceType)InvoiceTypeId;
      }
      set
      {
        InvoiceTypeId = Convert.ToInt32(value);
      }
    }

    public int InvoiceTypeId { get; set; }

    public List<SourceCodeTotal> SourceCodeTotal { get; private set; }

    public List<ValidationExceptionSummary> ValidationExceptionSummary { get; set; }

    public List<PrimeCoupon> CouponDataRecord { get; private set; }

    //For Auto Billing invoices - Added by Priya R.
    public List<AutoBillingPrimeCoupon> AutoBillingCouponDataRecord { get; set; }

    public List<RejectionMemo> RejectionMemoRecord { get; private set; }

    public List<BillingMemo> BillingMemoRecord { get; private set; }

    public List<CreditMemo> CreditMemoRecord { get; private set; }

    public List<SamplingFormDRecord> SamplingFormDRecord { get; private set; }

    

    public List<ProvisionalInvoiceRecordDetail> ProvisionalInvoiceRecordDetails { get; set; }
   
    public Guid InvoiceTotalRecordId { get; set; }

    public List<InvoiceVat> InvoiceTotalVat { get; private set; }

    public List<SamplingFormEDetailVat> SamplingFormEDetailVat { get; private set; }

    //Display Properties

    public string DisplayBillingCode
    {
      get
      {
        return EnumList.GetBillingCodeDisplayValue((BillingCode)BillingCode);
      }
    }

    public string DisplayInvoiceStatus { get; set; }

    //public string DisplayInvoiceStatus
    //{
    //  get
    //  {
    //    return EnumList.GetInvoiceStatusDisplayValue((InvoiceStatus)(InvoiceStatusId));
    //  }
    //}

    /// <summary>
    /// Gets or sets the sampling constant.
    /// </summary>
    /// <value>The sampling constant.</value>
    public double SamplingConstant { get; set; }

    public IsValidationExceptionSummary isValidationExceptionSummary { get; set;}

    public PaxInvoice()
    {
      ValidationExceptionSummary = new List<ValidationExceptionSummary>();
      CouponDataRecord = new List<PrimeCoupon>();
      RejectionMemoRecord = new List<RejectionMemo>();
      BillingMemoRecord = new List<BillingMemo>();
      CreditMemoRecord = new List<CreditMemo>();
      SamplingFormDRecord = new List<SamplingFormDRecord>();
      InvoiceTotalVat = new List<InvoiceVat>();
      SourceCodeTotal = new List<SourceCodeTotal>();
      ProvisionalInvoiceRecordDetails = new List<ProvisionalInvoiceRecordDetail>();
      SamplingFormEDetailVat = new List<SamplingFormEDetailVat>();
      AutoBillingCouponDataRecord = new List<AutoBillingPrimeCoupon>();
    }

    /// <summary>
    /// Used to indicate if Form A/B by provisional billing airline Via IS. Used in Form DE linking.
    /// </summary>
    public bool IsFormABViaIS { get; set; }

    /// <summary>
    /// Used to indicate if Form C by provisional billed airline Via IS. Used in Form DE linking.
    /// </summary>
    public bool IsFormCViaIS { get; set; }

    /// <summary>
    /// Used to indicate if the profile of the billed member indicates that migration of Form D/E was done on/before the provisional billing month for which the Form F is being created. OR
    /// Form D/E for the provisional billing month exists in the system.
    /// </summary>
    public bool IsFormDEViaIS { get; set; }

    /// <summary>
    /// Used to indicate if the profile of the billed member indicates that migration of Form F was done on/before the provisional billing month for which the Form XF is being created. OR
    /// Form F for the provisional billing month exists in the system.
    /// </summary>
    public bool IsFormFViaIS { get; set; }

    /// <summary>
    /// Gets or sets the correspondence ref number.
    /// </summary>
    /// <value>The correspondence ref number.</value>
    public long? CorrespondenceRefNo { get; set; }

    /// <summary>
    /// Gets or sets the rejected or correspondence invoice number.
    /// </summary>
    /// <value>The rejected invoice number.</value>
    public string RejectedInvoiceNumber { get; set; }

  }
}
