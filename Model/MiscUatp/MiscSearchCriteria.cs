using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MiscUatp
{
  /// <summary>
  /// Search Criteria for Miscellaneous and UATP invoices.
  /// </summary>
  public class MiscSearchCriteria
  {
    public string InvoiceNumber { get; set; }

    public int BillingCategoryId { get; set; }

    public BillingCategoryType BillingCategory
    {
      get
      {
        return (BillingCategoryType)BillingCategoryId;
      }
      set
      {
        BillingCategoryId = Convert.ToInt32(value);
      }
    }

    public int BilledMemberId { set; get; }

    // Used in AutoPopulate
    public string BilledMemberText { set; get; }
    
    public int BillingYear { set; get; }

    public int BillingMonth { set; get; }
    
    public int BillingPeriod { set; get; }
    
    public int ChargeCategoryId { get; set; }

    public int SettlementMethodId { get; set; }

    public SMI InvoiceSmi
    {
      get
      {
        return (SMI)SettlementMethodId;
      }
      set
      {
        SettlementMethodId = Convert.ToInt32(value);
      }
    }

    public string FileName { set; get; }

    /// <summary>
    /// Submission method
    /// </summary>
    /// <remarks>
    /// Wrapper property over SubmissionMethod Id.
    /// </remarks>
    public SubmissionMethod SubmissionMethod
    {
      set
      {
        SubmissionMethodId = Convert.ToInt32(value);
      }
      get
      {
        return (SubmissionMethod)SubmissionMethodId;
      }
    }

    public int SubmissionMethodId { set; get; }

    public InvoiceStatusType InvoiceStatus
    {
      get
      {
        return (InvoiceStatusType)InvoiceStatusId;
      }
      set
      {
        InvoiceStatusId = Convert.ToInt32(value);
      }
    }

    public int InvoiceStatusId { set; get; }

    public int BillingMemberId { set; get; }

    /// <summary>
    /// Gets or sets the location code.
    /// </summary>
    /// <value>The location code.</value>
    public string LocationCode { get; set; }

    /// <summary>
    /// Gets or sets the Invoice Owner id.
    /// </summary>
    /// <value>The owner id.</value>
    public int OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the invoice type id.
    /// </summary>
    /// <value>The invoice type id.</value>
    public int InvoiceTypeId
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the type of the invoice.
    /// </summary>
    /// <value>The type of the invoice.</value>
    public InvoiceType InvoiceType
    {
      get { return (InvoiceType)InvoiceTypeId; }
      set { InvoiceTypeId = Convert.ToInt32(value); }
    }

    public int? DailyDeliveryStatusId { get; set; }

    public DateTime? TargetDate { get; set; }

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public DateTime? DeliveryDateFrom { get; set; }
    public DateTime? DeliveryDateTo { get; set; }

    //CMP #655: IS-WEB Display per Location ID
    public string BillingMemberLoc { get; set; }
    
    // CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.8 Conditional Redirection of users upon login in is-web]
    //Added new property, this property will use when use will redirect to MISC Daily  Bilateral Invoices page.
    public bool IsRedirectUponLogin { get; set; }

  }
}