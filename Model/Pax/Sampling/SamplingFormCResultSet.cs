using System;

namespace Iata.IS.Model.Pax.Sampling
{
  /// <summary>
  /// This class is use to return the search results for Form C search query.
  /// The class is used to display list of records in Search Result grid.
  /// </summary>
  public class SamplingFormCResultSet
  {    
    public int ProvisionalBillingMemberId { get; set; }

    public string ProvisionalBillingMember { get; set; }

    public int ProvisionalBillingMonth { get; set; }

    public int ProvisionalBillingYear { get; set; }

    public int InvoiceStatusId { get; set; }

    public int? ListingCurrencyId { get; set; }

    public string ListingCurrencyCode { get; set; }

    public int FromMemberId { get; set; }

    public string NilFormCIndicator { get; set; }

    public long TotalRecords { get; set; }

    public double TotalGrossAmountAlf { get; set; }

    // Properties for display purposes
    public string ProvisionalInvoiceBillingYearMonth
    {
      get
      {
        return string.Format("{0}-{1}", ProvisionalBillingYear, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(ProvisionalBillingMonth));
      }
    }

    public string InvoiceStatus { get; set; }
    //public string InvoiceStatus
    //{
    //  get
    //  {
    //    return EnumList.GetFormCInvoiceStatusDisplayValue((Enums.InvoiceStatusType)InvoiceStatusId);
    //  }
    //}

    public string UniqueKey
    {
      get{
        return string.Format("{0}-{1}-{2}-{3}-{4}-{5}", ProvisionalBillingYear, ProvisionalBillingMonth, ProvisionalBillingMemberId, FromMemberId, ListingCurrencyId, InvoiceStatusId);
      }
    }

    public int SubmissionMethodId { get; set; }

    public int FileStatusId { get; set; }

    // SCP155930: FORM C APRIL and MAY 2013
    // Added property for the FormC Id
    public Guid? FormCId { get; set; }
  }
}
