using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Model.Pax.Base
{
  public abstract class MemoBase : EntityBase<Guid>
  {
    private BillingPeriod _yourBillingPeriod;

    public Guid InvoiceId { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public string YourInvoiceNumber { get; set; }

    public int YourInvoiceBillingYear { get; set; }

    public int YourInvoiceBillingMonth { get; set; }

    public int YourInvoiceBillingPeriod { get; set; }

    /// <summary>
    /// Read-only parameter that returns a <see cref="BillingPeriod"/> instance from the individual billing year, month and period periods.
    /// </summary>
    public BillingPeriod YourBillingPeriod
    {
      get
      {
        _yourBillingPeriod.Year = YourInvoiceBillingYear;
        _yourBillingPeriod.Month = YourInvoiceBillingMonth;
        _yourBillingPeriod.Period = YourInvoiceBillingPeriod;

        return _yourBillingPeriod;
      }
    }

    public string DisplayYourInvoiceBillingMonthYear
    {
      get
      {
        if (YourInvoiceBillingMonth > 0)
          return string.Format("{0}-{1}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(YourInvoiceBillingMonth).ToUpper(), YourInvoiceBillingYear);
        return string.Empty;
      }
    }

    public int AttachmentIndicatorOriginal { get; set; }

    public bool? AttachmentIndicatorValidated { get; set; }

    public int? NumberOfAttachments { get; set; }

    public string AirlineOwnUse { get; set; }

    public string ISValidationFlag { get; set; }

    public string ReasonRemarks { get; set; }

    public PaxInvoice Invoice { get; set; }

    /// <summary>
    /// Our reference 
    /// </summary>
    public string OurRef { get; set; }

    public int? FimCouponNumber { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }
    
  }
}
