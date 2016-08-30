using System;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class UatpConfiguration : ProfileEntity
  {
    public UatpConfiguration()
    {
      RejectionOnValidationFailure = RejectionOnValidationFailure.RejectFileInError;
      BillingIsXmlMigrationStatus = MigrationStatus.NotTested;
    }
    private const int PleaseSelectId = 0;

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "REJ_ON_VALIDATION_FAILURE_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.RejectionOnValidatonFailureDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int RejectionOnValidationFailureId { get; set; }

    public int? RejectionOnValidationFailureIdFutureValue { get; set; }

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

    //New in 18.0
    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP", ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_ONLINE_CORRECTION_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsOnlineCorrectionAllowed { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsOnlineCorrectionAllowedFutureDate { get; set; }

    public bool? IsOnlineCorrectionAllowedFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "ALLOWED_FILE_TYPE_FOR_SUPP_DOC", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string UatpAllowedFileTypesForSupportingDocuments { get; set; }
    

    public bool UatpInvoiceHandledbyAtcan { get; set; }
    public bool? UatpInvoiceHandledbyAtcanFutureValue { get; set; }
    public string UatpInvoiceHandledbyAtcanFuturePeriod { get; set; }
    public string UatpInvoiceHandledbyAtcanDisplay { get; set;}

    //Billing output
    public bool IsOutputFormatInXml { get; set; }

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_BILLNG_DATA_BY_3RDPARTY_REQ", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsBillingDataSubmittedByThirdPartiesRequired { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod { get; set; }

    public bool? IsBillingDataSubmittedByThirdPartiesRequiredFutureValue { get; set; }
    //End Addition

    public string IsXmlOutputFileVersion { get; set; }

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "BILLING_XML_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingxmlOutput { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string BillingxmlOutputFuturePeriod { get; set; }

    public bool? BillingxmlOutputFutureValue { get; set; }
    //End Addition

    //Migration

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "BILLING_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int BillingIsXmlMigrationStatusId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string BillingIsXmlMigrationStatusIdFuturePeriod { get; set; }

    public int? BillingIsXmlMigrationStatusIdFutureValue { get; set; }

    public string BillingIsXmlMigrationStatusIdDisplayValue { get; set; }

    public string BillingIsXmlMigrationStatusIdFutureDisplayValue { get; set; }
    //End Addition

    public MigrationStatus BillingIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)BillingIsXmlMigrationStatusId;
      }
      set
      {
        BillingIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "BILLING_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BillingIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "BILLING_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BillingIsXmlMigrationDate { set; get; }
    
     /// <summary>
    /// added by upendra yadav for IS-WEB
    /// </summary>
    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP", ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "BILLING_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BillingIswebMigrationDate { set; get; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string BillingIsXmlMigrationDateFuturePeriod { get; set; }

    public DateTime? BillingIsXmlMigrationDateFutureValue { get; set; }
    //End Addition


    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string ContactList { get; set; }

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_UATP_INV_IGNORE_FROM_DSPROC", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField =true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool ISUatpInvIgnoreFromDsproc { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string ISUatpInvIgnoreFromDsprocFuturePeriod { get; set; }

    public bool? ISUatpInvIgnoreFromDsprocFutureValue { get; set; }
    //End Addition

    public bool IsDigitalSignatureRequired { get; set; }

    public bool IsDigitalSignatureRequiredFutureValue { get; set; }

    //Additional Outputs Fields
    //Other outputs as billed entity

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_PDF_AS_OTHER_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_DS_FIL_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDsFileAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDsFileAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDsFileAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_SUPP_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsSuppDocAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsSuppDocAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsSuppDocAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsMemoAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsMemoAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsMemoAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    //Other outputs as billing entity

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_PDF_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Uatp, ElementGroupDisplayName = "UATP",ElementTable = "MEM_UATP_CONFIGURATION", ElementName = "IS_DS_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDsFileAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDsFileAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsDsFileAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    public bool IsMemoAsOtherOutputAsBillingEntity { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsMemoAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsMemoAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/


  }
}
