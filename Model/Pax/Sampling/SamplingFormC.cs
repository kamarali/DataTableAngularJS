using System;
using System.Collections.Generic;
using Iata.IS.Core;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormC : EntityBase<Guid>
  {
    private BillingPeriod _provisionalBillingPeriod;

    public int ProvisionalBillingMonth { get; set; }

    public int ProvisionalBillingYear { get; set; }

    public string NilFormCIndicator { get; set; }

    public Currency ListingCurrency { get; set; }

    public int? ListingCurrencyId { get; set; }

    public IsInputFile IsInputFile { get; set; }

    public Guid? InputFileId { get; set; }

    public Member ProvisionalBillingMember { get; set; }

    public int ProvisionalBillingMemberId { get; set; }

    public Member FromMember { get; set; }

    public int FromMemberId { get; set; }

    public string FromMemberText
    {
      get
      {
        if (FromMember != null && !string.IsNullOrEmpty(FromMember.MemberCodeAlpha))
          return string.Format("{0}-{1}-{2}", FromMember.MemberCodeAlpha, FromMember.MemberCodeNumeric, FromMember.CommercialName);

        return string.Empty;
      }
    }

    public string ToMemberText { get; set; }

    public int InvoiceStatusId { get; set; }

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

    public string IsInputFileDisplayId
    {
      get
      {
        return InputFileId.HasValue ? InputFileId.Value.Value() : string.Empty;
      }
    }

    public List<SamplingFormCSourceCodeTotal> SamplingFormCSourceCodeTotals { get; private set; }

    public List<SamplingFormCRecord> SamplingFormCDetails { get; private set; }

    public SamplingFormC()
    {
      SamplingFormCDetails = new List<SamplingFormCRecord>();
      SamplingFormCSourceCodeTotals = new List<SamplingFormCSourceCodeTotal>();
      ValidationErrors = new List<WebValidationError>();
      ValidationExceptionSummary = new List<ValidationExceptionSummary>();
    }
    
    public string ProvisionalBillingMemberText
    {
      get
      {
        if (ProvisionalBillingMember != null && !string.IsNullOrEmpty(ProvisionalBillingMember.MemberCodeAlpha))
          return string.Format("{0}-{1}-{2}", ProvisionalBillingMember.MemberCodeAlpha, ProvisionalBillingMember.MemberCodeNumeric, ProvisionalBillingMember.CommercialName);

        return string.Empty;
      }
    }

    public double TotalGrossAmount { get; set; }

    /// <summary>
    /// Gets or sets the number of records.
    /// </summary>
    /// <value>The number of records.</value>
    public long NumberOfRecords { get; set; }

    /// <summary>
    /// Gets or sets the submission method id.
    /// </summary>
    /// <value>The submission method id.</value>
    public int SubmissionMethodId { get; set; }

    /// <summary>
    /// Gets or sets the submission method.
    /// </summary>
    /// <value>The submission method.</value>
    /// <remarks>
    /// Wrapper over <see cref="SubmissionMethodId"/> property.
    /// </remarks>
    public SubmissionMethod SubmissionMethod
    {
      get
      {
        return (SubmissionMethod)SubmissionMethodId;
      }
      set
      {
        SubmissionMethodId = Convert.ToInt32(value);
      }
    }

    public int ValidationStatus { get; set; }
    public DateTime ValidationStatusDate { get; set; }
    public int PresentedStatus { get; set; }
    public DateTime PresentedStatusDate { get; set; }
    public bool IsSuspended { get; set; }
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Flag indicating if linking of Form C is successful at the header level.
    /// </summary>
    public bool IsLinkingSuccessful { get; set; }

    /// <summary>
    /// Web validation errors.
    /// </summary>
    public List<WebValidationError> ValidationErrors { get; set; }


    public IsValidationExceptionSummary isValidationExceptionSummary { get; set; }

    public List<ValidationExceptionSummary> ValidationExceptionSummary { get; set; }

    /// <summary>
    /// Read-only parameter that returns a <see cref="BillingPeriod"/> instance from the individual billing year, month and period periods.
    /// </summary>
    public BillingPeriod ProvisionalBillingPeriod
    {
      get
      {
        _provisionalBillingPeriod.Year = ProvisionalBillingYear;
        _provisionalBillingPeriod.Month = ProvisionalBillingMonth;
        _provisionalBillingPeriod.Period = 1;

        return _provisionalBillingPeriod;
      }
    }

    public DateTime ProcessingCompletedOn { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }

    /// <summary>
    ///  These Time limits are used while validating parsed Sampling Form C Records.
    /// </summary>
    /// <value>The valid Time Limits.</value>
    public List<TimeLimit> ValidTimeLimits { get; set; }

    /// <summary>
    /// FormCOwner
    /// </summary>
    /// CMP #659: SIS IS-WEB Usage Report
    public int? FormCOwner { get; set; }
  }
}
