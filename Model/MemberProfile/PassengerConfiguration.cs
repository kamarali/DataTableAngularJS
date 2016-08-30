using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Enums;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class PassengerConfiguration : ProfileEntity
  {
    public PassengerConfiguration()
    {
      RejectionOnValidationFailure = RejectionOnValidationFailure.RejectFileInError;
      SamplingCareerType = SamplingCarrierType.NotASamplingCarrier;
      NonSamplePrimeBillingIsIdecMigrationStatus = MigrationStatus.NotTested;
      NonSamplePrimeBillingIsxmlMigrationStatus = MigrationStatus.NotTested;
      SamplingProvIsIdecMigrationStatus = MigrationStatus.NotTested;
      SamplingProvIsxmlMigrationStatus = MigrationStatus.NotTested;
      NonSampleRmIsIdecMigrationStatus = MigrationStatus.NotTested;
      NonSampleBmIsIdecMigrationStatus = MigrationStatus.NotTested;
      NonSampleCmIsIdecMigrationStatus = MigrationStatus.NotTested;
      NonSampleRmIsXmlMigrationStatus = MigrationStatus.NotTested;
      NonSampleBmIsXmlMigrationStatus = MigrationStatus.NotTested;
      NonSampleCmIsXmlMigrationStatus = MigrationStatus.NotTested;
      SampleFormCIsIdecMigrationStatus = MigrationStatus.NotTested;
      SampleFormCIsxmlMigrationStatus = MigrationStatus.NotTested;
      SampleFormDeIsIdecMigrationStatus = MigrationStatus.NotTested;
      SampleFormFxfIsIdecMigrationStatus = MigrationStatus.NotTested;
    }

    private const int PleaseSelectId = 0;

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_FUTURE_BILLING_SUBMISSIONS_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsFutureBillingSubmissionsAllowed { get; set; }
    

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "REJ_ON_VALIDATION_FAILURE_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.RejectionOnValidatonFailureDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int RejectionOnValidationFailureId { get; set; }
    public string RejectionOnValidationFailureIdDisplayValue { get; set; }
    public string RejectionOnValidationFailureIdFutureDisplayValue { get; set; }
    
    public RejectionOnValidationFailure RejectionOnValidationFailure
    {
      get
      {
        return (RejectionOnValidationFailure)RejectionOnValidationFailureId;
      }
      set
      {
        RejectionOnValidationFailureId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_ONLINE_CORRECTION_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsOnlineCorrectionAllowed { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsOnlineCorrectionAllowedFutureDate { get; set; }
    public bool? IsOnlineCorrectionAllowedFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "ALLOWED_FILE_TYPE_FOR_SUPP_DOC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string PaxAllowedFileTypesForSupportingDocuments { get; set; }
    
    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "SAMPLING_CARRIER_TYPE_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.SamplingCareerTypeDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int SamplingCareerTypeId { get; set; }
    public string SamplingCareerTypeIdFuturePeriod { get; set; }
    public int? SamplingCareerTypeIdFutureValue { get; set; }
    public string SamplingCareerTypeIdDisplayValue { get; set; }
    public string SamplingCareerTypeIdFutureDisplayValue { get; set; }

    public SamplingCarrierType SamplingCareerType
    {
      get
      {
        return (SamplingCarrierType)SamplingCareerTypeId;
      }
      set
      {
        SamplingCareerTypeId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_CONS_PROV_BILLING_FILE_REQ", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField=true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsConsolidatedProvisionalBillingFileRequired { get; set; }
    public string IsConsolidatedProvisionalBillingFileRequiredFuturePeriod { get; set; }
    public bool? IsConsolidatedProvisionalBillingFileRequiredFutureValue { get; set; }
    
    public bool IsParticipateInValueDetermination { get; set; }
    public string IsParticipateInValueDeterminationDisplay { get; set; }
    public bool IsParticipateInValueConfirmation { get; set; }
    public string IsParticipateInValueConfirmationDisplay { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_AUTOMATED_VC_DET_RPT_REQ", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsAutomatedVcDetailsReportRequired { get; set; }
    public string IsAutomatedVcDetailsReportRequiredFuturePeriod { get; set; }
    public bool? IsAutomatedVcDetailsReportRequiredFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_PARTICIPATE_IN_AUTO_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField=true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsParticipateInAutoBilling { get; set; }
    public string IsParticipateInAutoBillingFutureDate { get; set; }
    public bool? IsParticipateInAutoBillingFutureValue { get; set; }

    // Auto billing configurations 

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "INVOICE_NO_RANGE_PREFIX", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string InvoiceNumberRangePrefix { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "INVOICE_NO_RANGE_FROM", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public long? InvoiceNumberRangeFrom { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "INVOICE_NO_RANGE_TO", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public long? InvoiceNumberRangeTo { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_ISR_FILE_REQUIRED", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField=true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsIsrFileRequired { get; set; }
    public string IsIsrFileRequiredFuturePeriod { get; set; }
    public bool? IsIsrFileRequiredFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "CUT_OFF_TIME", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int CutOffTime { get; set; }
    public string CutOffTimeFuturePeriod { get; set; }
    public int? CutOffTimeFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "LISTING_CURRENCY_CODE_NUM", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IgnoreValue = 0, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CurrencyDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int? ListingCurrencyId { get; set; }
    public Currency ListingCurrency { get; set; }
    public string ListingCurrencyIdFuturePeriod { get; set; }
    public int? ListingCurrencyIdFutureValue { get; set; }
    public string ListingCurrencyIdDisplayValue { get; set; }
    public string ListingCurrencyIdFutureDisplayValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "BILLING_INVOICE_IDEC_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingInvoiceIdecOutput { get; set; }
    public string BillingInvoiceIdecOutputFuturePeriod { get; set; }
    public bool? BillingInvoiceIdecOutputFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "BILLING_INVOICE_XML_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingInvoiceXmlOutput { get; set; }
    public string BillingInvoiceXmlOutputFuturePeriod { get; set; }
    public bool? BillingInvoiceXmlOutputFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "ISIDEC_OUTPUT_FILE_VERSION", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    public string IsIdecOutputVersion { get; set; }
    public string IsIdecOutputVersionFutureValue { get; set; }
    public string IsIdecOutputVersionFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "ISXML_OUTPUT_FILE_VERSION", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    public string IsXmlOutputVersion { get; set; }
    public string IsXmlOutputVersionFutureValue { get; set; }
    public string IsXmlOutputVersionFutureDate { get; set; }

    // Control 
    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "ENABLE_OLD_IDEC_PASSENGER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    public bool IsEnableOldIdecPassengerBilling { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSamplePrimeBillingIsIdecMigrationStatusId { get; set; }
    public string NonSamplePrimeBillingIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSamplePrimeBillingIsIdecMigrationStatusIdFutureValue { get; set; }
    public string NonSamplePrimeBillingIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string NonSamplePrimeBillingIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSamplePrimeBillingIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSamplePrimeBillingIsIdecMigrationStatusId;
      }
      set
      {
        NonSamplePrimeBillingIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSamplePrimeBillingIsIdecCertifiedOn { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSamplePrimeBillingIsIdecMigratedDate { set; get; }
    public DateTime? NonSamplePrimeBillingIsIdecMigratedDateFutureValue { get; set; }
    public string NonSamplePrimeBillingIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSamplePrimeBillingIsxmlMigrationStatusId { get; set; }
    public string NonSamplePrimeBillingIsxmlMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSamplePrimeBillingIsxmlMigrationStatusIdFutureValue { get; set; }
    public string NonSamplePrimeBillingIsxmlMigrationStatusIdDisplayValue { get; set; }
    public string NonSamplePrimeBillingIsxmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSamplePrimeBillingIsxmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSamplePrimeBillingIsxmlMigrationStatusId;
      }
      set
      {
        NonSamplePrimeBillingIsxmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSamplePrimeBillingIsxmlCertifiedOn { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSamplePrimeBillingIsxmlMigratedDate { set; get; }
    public DateTime? NonSamplePrimeBillingIsxmlMigratedDateFutureValue { get; set; }
    public string NonSamplePrimeBillingIsxmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SamplingProvIsIdecMigrationStatusId { get; set; }
    public string SamplingProvIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? SamplingProvIsIdecMigrationStatusIdFutureValue { get; set; }
    public string SamplingProvIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string SamplingProvIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SamplingProvIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)SamplingProvIsIdecMigrationStatusId;
      }
      set
      {
        SamplingProvIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SamplingProvIsIdecCerfifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SamplingProvIsIdecMigratedDate { set; get; }
    public DateTime? SamplingProvIsIdecMigratedDateFutureValue { get; set; }
    public string SamplingProvIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SamplingProvIsxmlMigrationStatusId { get; set; }
    public string SamplingProvIsxmlMigrationStatusIdFuturePeriod { get; set; }
    public int? SamplingProvIsxmlMigrationStatusIdFutureValue { get; set; }
    public string SamplingProvIsxmlMigrationStatusIdDisplayValue { get; set; }
    public string SamplingProvIsxmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SamplingProvIsxmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)SamplingProvIsxmlMigrationStatusId;
      }
      set
      {
        SamplingProvIsxmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SamplingProvIsxmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SamplingProvIsxmlMigratedDate { set; get; }
    public DateTime? SamplingProvIsxmlMigratedDateFutureValue { get; set; }
    public string SamplingProvIsxmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleRmIsIdecMigrationStatusId { get; set; }
    public string NonSampleRmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleRmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string NonSampleRmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleRmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleRmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleRmIsIdecMigrationStatusId;
      }
      set
      {
        NonSampleRmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleRmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleRmIsIdecMigratedDate { set; get; }
    public DateTime? NonSampleRmIsIdecMigratedDateFutureValue { get; set; }
    public string NonSampleRmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleBmIsIdecMigrationStatusId { get; set; }
    public string NonSampleBmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleBmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string NonSampleBmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleBmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleBmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleBmIsIdecMigrationStatusId;
      }
      set
      {
        NonSampleBmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleBmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleBmIsIdecMigratedDate { set; get; }
    public DateTime? NonSampleBmIsIdecMigratedDateFutureValue { get; set; }
    public string NonSampleBmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleCmIsIdecMigrationStatusId { get; set; }
    public string NonSampleCmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleCmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string NonSampleCmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleCmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleCmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleCmIsIdecMigrationStatusId;
      }
      set
      {
        NonSampleCmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleCmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleCmIsIdecMigratedDate { set; get; }
    public DateTime? NonSampleCmIsIdecMigratedDateFutureValue { get; set; }
    public string NonSampleCmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleRmIsXmlMigrationStatusId { get; set; }
    public string NonSampleRmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleRmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string NonSampleRmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleRmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleRmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleRmIsXmlMigrationStatusId;
      }
      set
      {
        NonSampleRmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleRmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleRmIsXmlMigratedDate { set; get; }
    public DateTime? NonSampleRmIsXmlMigratedDateFutureValue { get; set; }
    public string NonSampleRmIsXmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleBmIsXmlMigrationStatusId { get; set; }
    public string NonSampleBmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleBmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string NonSampleBmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleBmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleBmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleBmIsXmlMigrationStatusId;
      }
      set
      {
        NonSampleBmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleBmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleBmIsXmlMigratedDate { set; get; }
    public DateTime? NonSampleBmIsXmlMigratedDateFutureValue { get; set; }
    public string NonSampleBmIsXmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int NonSampleCmIsXmlMigrationStatusId { get; set; }
    public string NonSampleCmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? NonSampleCmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string NonSampleCmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string NonSampleCmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus NonSampleCmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)NonSampleCmIsXmlMigrationStatusId;
      }
      set
      {
        NonSampleCmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleCmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleCmIsXmlMigratedDate { set; get; }
    public DateTime? NonSampleCmIsXmlMigratedDateFutureValue { get; set; }
    public string NonSampleCmIsXmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormCIsIdecMigrationStatusId { get; set; }
    public string SampleFormCIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? SampleFormCIsIdecMigrationStatusIdFutureValue { get; set; }
    public string SampleFormCIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string SampleFormCIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormCIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)SampleFormCIsIdecMigrationStatusId;
      }
      set
      {
        SampleFormCIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormCIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormCIsIdecMigratedDate { set; get; }
    public DateTime? SampleFormCIsIdecMigratedDateFutureValue { get; set; }
    public string SampleFormCIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormCIsxmlMigrationStatusId { get; set; }
    public string SampleFormCIsxmlMigrationStatusIdFuturePeriod { get; set; }
    public int? SampleFormCIsxmlMigrationStatusIdFutureValue { get; set; }
    public string SampleFormCIsxmlMigrationStatusIdDisplayValue { get; set; }
    public string SampleFormCIsxmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormCIsxmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)SampleFormCIsxmlMigrationStatusId;
      }
      set
      {
        SampleFormCIsxmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormCIsxmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormCIsxmlMigratedDate { set; get; }
    public DateTime? SampleFormCIsxmlMigratedDateFutureValue { get; set; }
    public string SampleFormCIsxmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormDeIsIdecMigrationStatusId { get; set; }
    public string SampleFormDeIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? SampleFormDeIsIdecMigrationStatusIdFutureValue { get; set; }
    public string SampleFormDeIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string SampleFormDeIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormDeIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)SampleFormDeIsIdecMigrationStatusId;
      }
      set
      {
        SampleFormDeIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormDeIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormDeIsIdecMigratedDate { set; get; }
    public DateTime? SampleFormDeIsIdecMigratedDateFutureValue { get; set; }
    public string SampleFormDeIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormDEisxmlMigrationStatusId { get; set; }
    public string SampleFormDEisxmlMigrationStatusIdFuturePeriod { get; set; }
    public int? SampleFormDEisxmlMigrationStatusIdFutureValue { get; set; }
    public string SampleFormDEisxmlMigrationStatusIdDisplayValue { get; set; }
    public string SampleFormDEisxmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormDEisxmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)SampleFormDEisxmlMigrationStatusId;
      }
      set
      {
        SampleFormDEisxmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormDeIsxmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormDeIsxmlMigratedDate { set; get; }
    public DateTime? SampleFormDeIsxmlMigratedDateFutureValue { get; set; }
    public string SampleFormDeIsxmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormFxfIsIdecMigrationStatusId { get; set; }
    public string SampleFormFxfIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? SampleFormFxfIsIdecMigrationStatusIdFutureValue { get; set; }
    public string SampleFormFxfIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string SampleFormFxfIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormFxfIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)SampleFormFxfIsIdecMigrationStatusId;
      }
      set
      {
        SampleFormFxfIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormFxfIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormFxfIsIdecMigratedDate { set; get; }
    public DateTime? SampleFormFxfIsIdecMigratedDateFutureValue { get; set; }
    public string SampleFormFxfIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int SampleFormFxfIsxmlMigratedStatusId { get; set; }
    public string SampleFormFxfIsxmlMigratedStatusIdFuturePeriod { get; set; }
    public int? SampleFormFxfIsxmlMigratedStatusIdFutureValue { get; set; }
    public string SampleFormFxfIsxmlMigratedStatusIdDisplayValue { get; set; }
    public string SampleFormFxfIsxmlMigratedStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus SampleFormFxfIsxmlMigratedStatus
    {
      get
      {
        return (MigrationStatus)SampleFormFxfIsxmlMigratedStatusId;
      }
      set
      {
        SampleFormFxfIsxmlMigratedStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormFxfIsxmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormFxfIsxmlMigratedDate { set; get; }
    public DateTime? SampleFormFxfIsxmlMigratedDateFutureValue { get; set; }
    public string SampleFormFxfIsxmlMigratedDateFuturePeriod { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string ContactList { get; set; }
    /// <summary>
    /// Update By Upendra Yadav
    /// This is added for IS-WEB Migration Period
    /// </summary>
    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_PRIME_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSamplePrimeBillingIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_PRO_PRIME_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SamplingProvIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_RM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleRmIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_BM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleBmIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "NON_S_CM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? NonSampleCmIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_C_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormCIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_DE_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormDeIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "S_FORM_FXF_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? SampleFormFxfIswebMigratedDate { set; get; }


    // Additional Outputs Fields

    // Other outputs as billed entity
    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_PDF_AS_OTHER_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsPdfAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsPdfAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsPdfAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsDetailListingAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsDetailListingAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsDetailListingAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_DS_FIL_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsDsFileAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsDsFileAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsDsFileAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_SUPP_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsSuppDocAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsSuppDocAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsSuppDocAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_MEMO_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsMemoAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsMemoAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsMemoAsOtherOutputAsBilledEntityFutureDate { get; set; }

    // Other outputs as billing entity
    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_PDF_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsPdfAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsPdfAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsPdfAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsDetailListingAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsDetailListingAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsDetailListingAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_DS_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsDsFileAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsDsFileAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsDsFileAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "IS_MEMO_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsMemoAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsMemoAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsMemoAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Pax, ElementGroupDisplayName = "Passenger", ElementTable = "MEM_PAX_CONFIGURATION", ElementName = "DOWN_CONVERT_ISTRAN_OLDIDEC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool DownConvertISTranToOldIdec { set; get; }
  
    public bool PaxOldIdecMember { get; set; }
  }
}
