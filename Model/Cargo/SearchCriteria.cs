using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using BillingCode = Iata.IS.Model.Cargo.Enums.BillingCode;

namespace Iata.IS.Model.Cargo
{
  /// <summary>
  /// Cargo Invoice Search Criteria. This is not an entity - so, does not derive from <see cref="EntityBase{PK}"/>.
  /// There will be elements that are specific to receivables search and some specific to payables search.
  /// Note: Need to determine if payables and receivables need to be separated.
  /// </summary>
  public class SearchCriteria
  {
    //Receivables
    public string InvoiceNumber { get; set; }

    public int? BillingCode { get; set; }

    public BillingCode DisplayBillingCode { get; set; }


    public string BillingCategory { get; set; }

    public int BillingCategoryId { get; set; }

    public int SettlementMethodId { get; set; }

    public SMI InvoiceSmi
    {
      get
      {
        return (SMI) SettlementMethodId;
      }
      set
      {
        SettlementMethodId = Convert.ToInt32(value);
      }
    }

    // Review: What is this property used for?
    public string SubMethod { set; get; }

    public string FileName { set; get; }

    public FileStatusType FileStatus { set; get; }

    public FileFormatType FileFormat { set; get; }

    public int BillingYear { set; get; }

    public int BillingMonth { set; get; }

    public string ClearanceType { set; get; }

    public int BillingPeriod { set; get; }

    /// <summary>
    /// Gets or sets the billed member id.
    /// </summary>
    /// <value>The billed member id.</value>
    /// <remarks>This will be considered as Provisional Billing Member Id in Form C Search.</remarks>
    public int BilledMemberId { set; get; }

    // Used in AutoPopulate
    public string BilledMemberText { set; get; }

    public InvoiceStatusType InvoiceStatus
    {
      get
      {
        return (InvoiceStatusType) InvoiceStatusId;
      }
      set
      {
        InvoiceStatusId = Convert.ToInt32(value);
      }
    }

    public int InvoiceStatusId { set; get; }

    public string InvoiceStatusIds { set; get; }
    //Payables
    public string SettlementMethodIndicator { set; get; }

    public int BillingMemberId { set; get; }

    // Sampling FormC
    public string ProvisionalBillingMonth { set; get; }

    public string AirlineCode { set; get; }

    public string FileStatusForProcessingDashboard { get; set; }

    public string FileFormatForProcessingDashboard { get; set; }

    public string ChargeCategory { set; get; }


    public int ChargeCategoryId { set; get; }


    public int ChargeCodeId { set; get; }


    public string Carrier { set; get; }

    public DateTime ReceivedByIs { get; set; }

    public string FormCFromAirline { get; set; }

    /// <summary>
    /// Form C To Airline
    /// </summary>   
    public int FormCToAirline { get; set; }

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
        return (SubmissionMethod) SubmissionMethodId;
      }
    }

    public int SubmissionMethodId { set; get; }

    /// <summary>
    /// Invoice Owner
    /// </summary>
    public string InvoiceOwner { set; get; }

    // Review: Needs to be moved to a separate search criteria for supporting documents/attachments.
    public int BatchSequenceNumber { get; set; }

    public long TicketDocNumber { get; set; }

    public int CouponNumber { get; set; }

    public int SourceCode { get; set; }

    public bool AttachmentIndicatorOriginal { get; set; }

    /// <summary>
    /// Resubmission Status
    /// </summary>
    public string ResubmissionStatus { set; get; }
      /// <summary>
      /// Billing member text fro pax payable search 
      /// </summary>
    public string BillingMemberText { get; set; }


    /// <summary>
    /// Gets or sets the Invoice Owner id.
    /// </summary>
    /// <value>The owner id.</value>
    public int OwnerId { get; set; }
  }
}