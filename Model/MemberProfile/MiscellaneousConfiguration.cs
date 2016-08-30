using System;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class MiscellaneousConfiguration : ProfileEntity
  {
    public MiscellaneousConfiguration()
    {
      RejectionOnValidationFailure = RejectionOnValidationFailure.RejectFileInError;
      BillingIsXmlMigrationStatus = MigrationStatus.NotTested;
    }

    private const int PleaseSelectId = 0;

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "REJ_ON_VALIDATION_FAILURE_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.RejectionOnValidatonFailureDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int RejectionOnValidationFailureId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database

    public int? RejectionOnValidationFailureIdFutureValue { get; set; }

    public string RejectionOnValidationFailureIdDisplayValue { get; set; }
    public string RejectionOnValidationFailureIdFutureDisplayValue { get; set; }
   
    //End Addition

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
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_ONLINE_CORRECTION_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsOnlineCorrectionAllowed { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsOnlineCorrectionAllowedFutureDate { get; set; }
    public bool? IsOnlineCorrectionAllowedFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "ALLOWED_FILE_TYPE_FOR_SUPP_DOC", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string MiscAllowedFileTypesForSupportingDocuments { get; set; }
    
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "BILLING_XML_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingXmlOutput { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string BillingXmlOutputFuturePeriod { get; set; }

    public bool? BillingXmlOutputFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_BILLNG_DATA_BY_3RDPARTY_REQ", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsBillingDataSubmittedByThirdPartiesRequired { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod { get; set; }

    public bool? IsBillingDataSubmittedByThirdPartiesRequiredFutureValue { get; set; }
    //End Addition
    
    public string IsXmlOutputFileVersion { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsXmlOutputFileVersionFuturePeriod { get; set; }

    public string IsXmlOutputFileVersionFutureValue { get; set; }
    //End Addition

    //Migration

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "BILLING_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
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

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "BILLING_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BillingIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "BILLING_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BillingIsXmlMigrationDate { set; get; }
      /// <summary>
      /// Updated By Upendra Yadav
      /// Added for IS-WEB
      /// </summary>
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "BILLING_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
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

    //Additional Outputs Fields

    //Other outputs as billed entity

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_PDF_AS_OTHER_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DS_FIL_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDsFileAsOtherOutputAsBilledEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDsFileAsOtherOutputAsBilledEntityFutureValue { get; set; }

    public string IsDsFileAsOtherOutputAsBilledEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_SUPP_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
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
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_PDF_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsPdfAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsPdfAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBillingEntity { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public bool? IsDetailListingAsOtherOutputAsBillingEntityFutureValue { get; set; }

    public string IsDetailListingAsOtherOutputAsBillingEntityFutureDate { get; set; }
    /*End Addition*/

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous",ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DS_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate, IncludeDisplayNames = true)]
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

    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DAILY_XML_REQUIRED", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDailyXmlRequired { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsDailyXmlRequiredFuturePeriod { get; set; }

    public bool? IsDailyXmlRequiredValue { get; set; }

    public bool? IsDailyXmlRequiredFutureValue { get; set; }
    //End Addition    

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0 1-KP 2
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DAILY_PAYABLE_IS_WEB_REQ", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDailyPayableIsWebRequired { get; set; }

    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0 1-KP 2
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DAILY_PAYABLE_OAR_REQ", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDailyPayableOARRequired { get; set; }

    //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0 1-KP 2
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_DAILY_PAYABLE_XML_REQ", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDailyPayableXmlRequired { get; set; }

    //CMP626
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "IS_FUTURE_BILLING_SUB_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsFutureBillingSubmissionsAllowed { get; set; }

    //CMP#622: MISC Outputs Split as per Location IDs
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    [Audit(ElementGroup = ElementGroupType.Miscellaneous, ElementGroupDisplayName = "Miscellaneous", ElementTable = "MEM_MSC_CONFIGURATION", ElementName = "REC_LOCSPEC_MISCOUTPUT_ATMAIN", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IncludeRelationId = true)]
    public bool RecCopyOfLocSpecificMISCOutputsAtMain { get; set; }

  }
}
