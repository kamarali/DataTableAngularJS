using System;
using System.Collections.Generic;
using Iata.IS.Core;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using SupportingAttachmentStatus = Iata.IS.Model.Enums.SupportingAttachmentStatus;

namespace Iata.IS.Model.Base
{
  public abstract class InvoiceBase : EntityBase<Guid>
  {
    private BillingPeriod _invoiceBillingPeriod;

    /// <summary>
    /// Gets or sets the legal text.
    /// </summary>
    /// <value>The legal text.</value>
    public string LegalText { get; set; }

    /// <summary>
    /// Gets or sets the payment details.
    /// </summary>
    /// <value>The payment details.</value>
    public PaymentDetail PaymentDetail { get; set; }

    public string InvoiceNumber { get; set; }

    /// <summary>
    /// Gets or sets the invoice summary.
    /// </summary>
    /// <value>The invoice summary.</value>
    public InvoiceSummary InvoiceSummary { get; set; }

    public DateTime InvoiceDate { get; set; }

    public InvoiceTotal InvoiceTotalRecord { get; set; }

    public SamplingFormEDetail SamplingFormEDetails { get; set; }

    public DigitalSignatureRequired DigitalSignatureRequired
    {
      get
      {
        return (DigitalSignatureRequired)DigitalSignatureRequiredId;
      }
      set
      {
        DigitalSignatureRequiredId = Convert.ToInt32(value);
      }
    }

    public int DigitalSignatureRequiredId { get; set; }

    public DigitalSignatureStatus DigitalSignatureStatus
    {
      get
      {
        return (DigitalSignatureStatus)DigitalSignatureStatusId;
      }
      set
      {
        DigitalSignatureStatusId = Convert.ToInt32(value);
      }
    }

    public int DigitalSignatureStatusId { get; set; }

    public DateTime DigitalSignatureDate { get; set; }

    public bool SuspendedInvoiceFlag { get; set; }

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

    public string SettlementMethodDisplayText { get; set; }

    public string BilledMemberLocationCode { get; set; }

    public string BillingMemberLocationCode { get; set; }

    public int BilledMemberId { get; set; }

    public int BillingMemberId { get; set; }

    public int? BilledParentMemberId { get; set; }

    public int? BillingParentMemberId { get; set; }

    public Member BilledMember { get; set; }

    public Member BillingMember { get; set; }

    /// <summary>
    /// Added for UI purpose.
    /// </summary>
    /// <value>The billed member text.</value>
    public string BilledMemberText
    {
      get
      {
        return BilledMember != null ? string.Format("{0}-{1}-{2}", BilledMember.MemberCodeAlpha, BilledMember.MemberCodeNumeric, BilledMember.CommercialName) : string.Empty;
      }
      set
      {
        //To be used in parsing to store the input value to be added in validation exception
      }
    }

    private string _billingMemberText;

    /// <summary>
    /// Added for UI purpose.
    /// </summary>
    /// <value>The billing member text.</value>
    public string BillingMemberText
    {
      get
      {
        return string.IsNullOrEmpty(_billingMemberText) ? BillingMember != null ? string.Format("{0}-{1}-{2}", BillingMember.MemberCodeAlpha, BillingMember.MemberCodeNumeric, BillingMember.CommercialName) : _billingMemberText : string.Empty;
      }
      set
      {
        _billingMemberText = value;
      }
    }

    public Currency ListingCurrency { get; set; }

    public string ListingCurrencyDisplayText
    {
      get
      {
        return ListingCurrency != null ? ListingCurrency.Code : string.Empty;
      }
    }

    public int? ListingCurrencyId { get; set; }

    public BillingCurrency? BillingCurrency
    {
      get
      {
        if (BillingCurrencyId != null)
        {
          return (BillingCurrency)BillingCurrencyId;
        }

        return null;
      }
      set
      {
        BillingCurrencyId = Convert.ToInt32(value);
      }
    }

    public string BillingCurrencyDisplayText
    {
      get
      {
        return BillingCurrency.HasValue ? EnumList.GetBillingCurrencyDisplayValue(BillingCurrency.Value) : string.Empty;
      }
    }

    public int? BillingCurrencyId { get; set; }

    public Currency InvoiceBillingCurrency { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate.
    /// </summary>
    /// <value>The exchange rate.</value>
    /// CMP#648: Exchange rate should contains null.
    public decimal? ExchangeRate { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate.
    /// </summary>
    /// <value>The exchange rate.</value>
    /// CMP#648: Exchange rate should contains null.
    public decimal? ListingToBillingRate
    {
      get { return ExchangeRate.HasValue ? ExchangeRate.Value : (decimal?) null; }
      set { ExchangeRate = value; }
    }

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

    public string InvoiceStatusDisplayText { set; get; }

    public IsInputFile IsInputFile { get; set; }

    public Guid? IsInputFileId { get; set; }

    public string IsInputFileDisplayId
    {
      get
      {
        return IsInputFileId.HasValue ? IsInputFileId.Value.Value() : string.Empty;
      }
    }

    public string InputFileNameDisplayText
    {
      get
      {
        return IsInputFile != null ? IsInputFile.FileName : string.Empty;
      }
    }

    public int OriginalBillingMonth { get; set; }

    public int OriginalPeriod { get; set; }

    public int SuspensionMonth { get; set; }

    public int SuspensionPeriod { get; set; }

    public int ReinstatementMonth { get; set; }

    public int ReinstatementPeriod { get; set; }

    public ResubmissionStatus FileStatus
    {
      get
      {
        return ResubmissionStatusId != null ? (ResubmissionStatus)ResubmissionStatusId : ResubmissionStatus.NotSet;
      }
      set
      {
        ResubmissionStatusId = value == ResubmissionStatus.NotSet ? (int?)null : Convert.ToInt32(value);
      }
    }

    public int? ResubmissionStatusId { get; set; }

    public int ResubmissionBillingMonth { get; set; }

    public int ResubmissionPeriod { get; set; }

    public string ResubmissionRemarks { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public int BillingYear { get; set; }

    public int SubmissionMethodId { get; set; }

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

    public string SubmissionMethodDisplayText { get; set; }

    public string DisplayBillingPeriod
    {
      get
      {
        //IMP : Check whether BillingMonth is valid
        if (BillingMonth > 0 && BillingMonth < 13)
          return string.Format("{0} {1} P{2}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillingMonth), BillingYear, BillingPeriod);
        return string.Empty;
      }
    }

    public string DisplayBillingMonthYear
    {
      get
      {
        if (BillingMonth > 0 && BillingMonth < 13)
          return string.Format("{0}-{1}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(BillingMonth).ToUpper(), BillingYear);
        return string.Empty;
      }
    }

    /// <summary>
    /// Read-only parameter that returns a <see cref="BillingPeriod"/> instance from the individual billing year, month and period periods.
    /// </summary>
    public BillingPeriod InvoiceBillingPeriod
    {
      get
      {
        _invoiceBillingPeriod.Year = BillingYear;
        _invoiceBillingPeriod.Month = BillingMonth;
        _invoiceBillingPeriod.Period = BillingPeriod;

        return _invoiceBillingPeriod;
      }
    }

    /// <summary>
    /// TODO: Need to remove as it should be in child classes.
    /// </summary>
    public int BillingCode { get; set; }

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

    public bool IsLateSubmitted { get; set; }

    public InvoiceValidationStatus ValidationStatus
    {
      get
      {
        return (InvoiceValidationStatus)ValidationStatusId;
      }
      set
      {
        ValidationStatusId = Convert.ToInt32(value);
      }
    }

    public int ValidationStatusId { get; set; }

    public DateTime ValidationDate { get; set; }

    public ValueConfirmationStatus ValueConfirmationStatus
    {
      get
      {
        if (ValueConfirmationStatusId.HasValue)
        {
          return (ValueConfirmationStatus)ValueConfirmationStatusId;
        }
        return ValueConfirmationStatus.None;
      }
      set
      {
        if (value != ValueConfirmationStatus.None)
        {
          ValueConfirmationStatusId = Convert.ToInt32(value);
        }
        else
        {
          ValueConfirmationStatusId = null;
        }
      }
    }

    public int? ValueConfirmationStatusId { get; set; }

    public DateTime ValueConfirmationDate { get; set; }

    public InvoiceProcessStatus SettlementFileStatus
    {
      get
      {
          return SettlementFileStatusId != null ? (InvoiceProcessStatus)SettlementFileStatusId : InvoiceProcessStatus.NotSet;
       // return (InvoiceProcessStatus)SettlementFileStatusId;
      }
      set
      {
         SettlementFileStatusId = value == InvoiceProcessStatus.NotSet ? (int?)null : Convert.ToInt32(value);
        //SettlementFileStatusId = Convert.ToInt32(value);
      }
    }

    public int? SettlementFileStatusId { get; set; }

    public DateTime SettlementFileSentDate { get; set; }

    public InvoiceProcessStatus PresentedStatus
    {
      get
      {
        return (InvoiceProcessStatus)PresentedStatusId;
      }
      set
      {
        PresentedStatusId = Convert.ToInt32(value);
      }
    }

    public int PresentedStatusId { get; set; }

    public DateTime PresentedStatusDate { get; set; }

    public List<MemberLocationInformation> MemberLocationInformation { get; set; }

    public List<WebValidationError> ValidationErrors { get; set; }

    /// <summary>
    /// Gets or sets the tolerance.
    /// </summary>
    /// <value>The tolerance.</value>
    public Tolerance Tolerance { get; set; }

    /// <summary>
    /// Gets or sets the billing reference data source id.
    /// </summary>
    /// <value>The billing reference data source id.</value>
    public int BillingReferenceDataSourceId { get; set; }

    /// <summary>
    /// Gets or sets the billed reference data source id.
    /// </summary>
    /// <value>The billed reference data source id.</value>
    public int BilledReferenceDataSourceId { get; set; }

    public int InvoiceOwnerId { get; set; }

    public User InvoiceOwner { get; set; }


    public string InvoiceOwnerDisplayText
    {
      get
      {
        return InvoiceOwner != null ? string.Format("{0} {1}", InvoiceOwner.FirstName, InvoiceOwner.LastName) : string.Empty;
      }
    }

    public SupportingAttachmentStatus SupportingAttachmentStatus
    {
      get
      {
        return (SupportingAttachmentStatus)SupportingAttachmentStatusId;
      }
      set
      {
        SupportingAttachmentStatusId = Convert.ToInt32(value);
      }
    }

    public int SupportingAttachmentStatusId { get; set; }

    public bool IsFutureSubmission { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string DsRequirdBy { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string DsStatus { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ClearingHouse { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Member SponsoredBy { get; set; }

    public int? SponsoredById { get; set; }

    public int LateSubmissionRequestStatusId { get; set; }

    public int OperatingUserId { get; set; }

    /// <summary>
    /// Gets Valid Tax Codes in the form of key as Tax Code, this collection will be used for validating tax code in tax records.
    /// </summary>
    public IDictionary<string, bool> ValidTaxCodes { get; set; }

    /// <summary>
    /// Gets Valid Currency Codes in the form of key as Currency Code and value isActive or not.
    /// </summary>
    public IDictionary<string, int> ValidCurrencyCodes { get; set; }

    /// <summary>
    /// Gets Valid Country Codes in the form of key as Country Code and value is IsActive
    /// </summary>
    public IDictionary<string, bool> ValidCountryCodes { get; set; }

    /// <summary>
    /// These source codes are used while validating parsed invoice source codes.
    /// </summary>
    /// <value>The valid source codes.</value>
    public IList<SourceCode> ValidSourceCodes { get; set; }

    /// <summary>
    /// These reason codes are used while validating parsed invoice reason codes.
    /// </summary>
    /// <value>The valid reason codes.</value>
    public IList<ReasonCode> ValidReasonCodes { get; set; }

    /// <summary>
    ///  These Time limits are used while validating parsed invoice reason codes.
    /// </summary>
    /// <value>The valid Time Limits.</value>
    public List<TimeLimit> ValidTimeLimits { get; set; }

    /// <summary>
    /// Lead periods used to determine the expiry date period of transaction.
    /// </summary>
    public List<LeadPeriod> ValidLeadPeriods { get; set; }

    /// <summary>
    /// These RM reason code acceptable difference are used while validating parsed invoice amount.
    /// </summary>
    /// <value>The valid reason codes.</value>
    public IList<RMReasonAcceptableDiff> ValidRMReasonAcceptableDiff { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for RM1.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodRm1 { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for Coupon.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodCoupon { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for Form D.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodFormAb { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for Form D.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodFormD { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for Form F.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodFormF { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for BM.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodBm { get; set; }

    /// <summary>
    /// Get or Set ignore the validation in migration period for CM.
    /// </summary>
    public bool IgnoreValidationInMigrationPeriodCm { get; set; }

    public LateSubmissionRequestStatus LateSubmissionRequestStatus
    {
      get
      {
        return (LateSubmissionRequestStatus)LateSubmissionRequestStatusId;
      }
      set
      {
        LateSubmissionRequestStatusId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the legal PDF location for the Invoice.
    /// </summary>
    /// <value>The legal PDF location.</value>
    public string LegalPdfLocation { get; set; }

    /// <summary>
    /// Gets or sets the legal XML location for the Invoice.
    /// </summary>
    /// <value>The legal Xml location.</value>
    public string LegalXmlLocation { get; set; }

    /// <summary>
    /// Gets or sets the Xml Signature Location for the Invoice.
    /// </summary>
    /// <value>The legal Xml Signature Location.</value>
    public string XmlSignatureLocation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <value>The Xml Verification Log location.</value>
    public string XmlVerificationLogLocation { get; set; }


    /// <summary>
    /// Gets a value indicating whether Legal PDF has been generated for the Invoice.
    /// </summary>
    /// <value>
    /// 	<c>1</c> if this instance is legal PDF generated; otherwise, <c>0</c>.
    /// </value>
    /// <remarks>
    /// It uses <see cref="LegalPdfLocation"/> property to check whether PDF has been generated.
    /// If the property is null or empty, then it assumes that PDF is not generated
    /// </remarks>
    public int IsLegalPdfGenerated
    {
      get
      {
        return string.IsNullOrEmpty(LegalPdfLocation) ? 0 : 1;
      }
    }
   
    public string InvTemplateLanguage { get; set; }

    /// <summary>
    /// Stored the Min Max amounts.
    /// </summary>
    public List<MinAcceptableAmount> ValidMinAcceptableAmounts { get; set; }

    /// <summary>
    /// Stored the Max amounts.
    /// </summary>
    public List<MaxAcceptableAmount> ValidMaxAcceptableAmounts { get; set; }

    public int? IchSettlementStatus { get; set; }

    public string RecapsheetProcessStatus { get; set; }

    public int? PdfInProcess { get; set; }

    public int? ArcReqAsBL { get; set; }

    public int? ArcReqAsBD { get; set; }

    public string DepositStatusBL { get; set; }

    public string DepositStatusBD { get; set; }

    // SCP85837: PAX CGO Sequence Number
    public RecordSequence IsRecordSequenceArranged;

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public int DailyDeliveryStatusId { get; set; }

    //CMP602: View Inter-Clearance Invoices in ICH / ACH Dashboards
    public string ViewableByClearingHouse { get; set; }

    public DateTime? TargetDate { get; set; }

    //SCP149711 - Incorrect Form E UA to 3M
    public RecalculateFormE IsRecalculatedFormE;

    #region CMP #624: ICH Rewrite-New SMI X

    /* Description: Added new fields at invoice level */
    public string ChAgreementIndicator { get; set; }

    public DateTime? ChDueDate { get; set; }

    public string ChValidationResult { get; set; }
    public string CurrencyRateIndicator { get; set; }

    private bool _smiXPhase1ValidationStatus = true;
    public bool GetSmiXPhase1ValidationStatus()
    {
        return _smiXPhase1ValidationStatus;
    }
    public void SetSmiXPhase1ValidationStatus(bool newStatus)
    {
        if (!newStatus)
        {
            _smiXPhase1ValidationStatus = false;//newStatus
        }
    }

    #endregion

    //CMP622 : MISC Outputs Split as per Location IDs
    public string MiscBillingMemberLocCode { get; set; }
    public string MiscBilledMemberLocCode { get; set; }

    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery
    public int DailyDeliveryOutputFileTypeId { get; set; }

    protected InvoiceBase()
    {
      MemberLocationInformation = new List<MemberLocationInformation>();
      ValidationErrors = new List<WebValidationError>();
      SupportingAttachmentStatus = SupportingAttachmentStatus.NotProcessed;
      ValueConfirmationStatus = ValueConfirmationStatus.None;
      SettlementFileStatus = InvoiceProcessStatus.Pending;
      ValidationStatus = InvoiceValidationStatus.Pending;
      PresentedStatus = InvoiceProcessStatus.Pending;
      DigitalSignatureStatus = DigitalSignatureStatus.NotRequired;
    }
  }
}
