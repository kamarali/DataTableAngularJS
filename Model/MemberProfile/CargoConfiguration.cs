using System;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class CargoConfiguration : ProfileEntity
  {
    public CargoConfiguration()
    {
      RejectionOnValidationFailure = RejectionOnValidationFailure.RejectFileInError;
      PrimeBillingIsIdecMigrationStatus = MigrationStatus.NotTested;
      PrimeBillingIsxmlMigrationStatus = MigrationStatus.NotTested;
      RmIsIdecMigrationStatus = MigrationStatus.NotTested;
      RmIsXmlMigrationStatus = MigrationStatus.NotTested;
      BmIsIdecMigrationStatus = MigrationStatus.NotTested;
      BmIsXmlMigrationStatus = MigrationStatus.NotTested;
      CmIsIdecMigrationStatus = MigrationStatus.NotTested;
      CmIsXmlMigrationStatus = MigrationStatus.NotTested;
    }
    private const int PleaseSelectId = 0;

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "REJ_ON_VALIDATION_FAILURE_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
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

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo", ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_ONLINE_CORRECTION_ALLOWED", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = false, IgnoreValue = 0)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsOnlineCorrectionAllowed { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsOnlineCorrectionAllowedFutureDate { get; set; }
    public bool? IsOnlineCorrectionAllowedFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "ALLOWED_FILE_TYPE_FOR_SUPP_DOC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string CgoAllowedFileTypesForSupportingDocuments { get; set; }
    
    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BILLING_INVOICE_IDEC_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingInvoiceIdecOutput { get; set; }
    public string BillingInvoiceIdecOutputFuturePeriod { get; set; }
    public bool? BillingInvoiceIdecOutputFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BILLING_INVOICE_XML_OUTPUT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool BillingInvoiceXmlOutput { get; set; }
    public string BillingInvoiceXmlOutputFuturePeriod { get; set; }
    public bool? BillingInvoiceXmlOutputFutureValue { get; set; }

    public string IsIdecOutputVersion { get; set; }

    public string IsXmlOutputVersion { get; set; }

    //// Control 
    //[Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "ENABLE_OLD_IDEC_PASSENGER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    //public bool IsEnableOldIdecPassengerBilling { get; set; }

    // Migration

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int PrimeBillingIsIdecMigrationStatusId { get; set; }
    public string PrimeBillingIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? PrimeBillingIsIdecMigrationStatusIdFutureValue { get; set; }
    public string PrimeBillingIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string PrimeBillingIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus PrimeBillingIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)PrimeBillingIsIdecMigrationStatusId;
      }
      set
      {
        PrimeBillingIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? PrimeBillingIsIdecMigratedDate { set; get; }
    public DateTime? PrimeBillingIsIdecMigratedDateFutureValue { get; set; }
    public string PrimeBillingIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? PrimeBillingIsIdecCertifiedOn { set; get; }
     
    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int PrimeBillingIsxmlMigrationStatusId { get; set; }
    public string PrimeBillingIsxmlMigrationStatusIdFuturePeriod { get; set; }
    public int? PrimeBillingIsxmlMigrationStatusIdFutureValue { get; set; }
    public string PrimeBillingIsxmlMigrationStatusIdDisplayValue { get; set; }
    public string PrimeBillingIsxmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus PrimeBillingIsxmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)PrimeBillingIsxmlMigrationStatusId;
      }
      set
      {
        PrimeBillingIsxmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? PrimeBillingIsxmlMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? PrimeBillingIsxmlCertifiedOn { set; get; }
    public DateTime? PrimeBillingIsxmlMigratedDateFutureValue { get; set; }
    public string PrimeBillingIsxmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int RmIsIdecMigrationStatusId { get; set; }
    public string RmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? RmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string RmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string RmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus RmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)RmIsIdecMigrationStatusId;
      }
      set
      {
        RmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? RmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? RmIsIdecMigratedDate { set; get; }
    public DateTime? RmIsIdecMigratedDateFutureValue { get; set; }
    public string RmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int BmIsIdecMigrationStatusId { get; set; }
    public string BmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? BmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string BmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string BmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus BmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)BmIsIdecMigrationStatusId;
      }
      set
      {
        BmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BmIsIdecMigratedDate { set; get; }
    public DateTime? BmIsIdecMigratedDateFutureValue { get; set; }
    public string BmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISIDEC_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int CmIsIdecMigrationStatusId { get; set; }
    public string CmIsIdecMigrationStatusIdFuturePeriod { get; set; }
    public int? CmIsIdecMigrationStatusIdFutureValue { get; set; }
    public string CmIsIdecMigrationStatusIdDisplayValue { get; set; }
    public string CmIsIdecMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus CmIsIdecMigrationStatus
    {
      get
      {
        return (MigrationStatus)CmIsIdecMigrationStatusId;
      }
      set
      {
        CmIsIdecMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISIDEC_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? CmIsIdecCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISIDEC_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? CmIsIdecMigratedDate { set; get; }
    public DateTime? CmIsIdecMigratedDateFutureValue { get; set; }
    public string CmIsIdecMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int RmIsXmlMigrationStatusId { get; set; }
    public string RmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? RmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string RmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string RmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus RmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)RmIsXmlMigrationStatusId;
      }
      set
      {
        RmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? RmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? RmIsXmlMigratedDate { set; get; }
    public DateTime? RmIsXmlMigratedDateFutureValue { get; set; }
    public string RmIsXmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int BmIsXmlMigrationStatusId { get; set; }
    public string BmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? BmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string BmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string BmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus BmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)BmIsXmlMigrationStatusId;
      }
      set
      {
        BmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BmIsXmlMigratedDate { set; get; }
    public DateTime? BmIsXmlMigratedDateFutureValue { get; set; }
    public string BmIsXmlMigratedDateFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISXML_MIG_STAT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.MigrationStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public int CmIsXmlMigrationStatusId { get; set; }
    public string CmIsXmlMigrationStatusIdFuturePeriod { get; set; }
    public int? CmIsXmlMigrationStatusIdFutureValue { get; set; }
    public string CmIsXmlMigrationStatusIdDisplayValue { get; set; }
    public string CmIsXmlMigrationStatusIdFutureDisplayValue { get; set; }

    public MigrationStatus CmIsXmlMigrationStatus
    {
      get
      {
        return (MigrationStatus)CmIsXmlMigrationStatusId;
      }
      set
      {
        CmIsXmlMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISXML_CERT_ON", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? CmIsXmlCertifiedOn { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISXML_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? CmIsXmlMigratedDate { set; get; }
    public DateTime? CmIsXmlMigratedDateFutureValue { get; set; }
    public string CmIsXmlMigratedDateFuturePeriod { get; set; }

    /// <summary>
    /// Update By Upendra Yadav
    /// This is added for IS-WEB Migration Period
    /// </summary>

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo", ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "PRIME_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? PrimeBillingIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo", ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "RM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? RmIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo", ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "BM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? BmIswebMigratedDate { set; get; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo", ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "CM_ISWEB_MIG_DATE", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? CmIswebMigratedDate { set; get; }





    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string ContactList { get; set; }

    // Additional Outputs Fields
    // Other outputs as billed entity
    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_PDF_AS_OTHER_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsPdfAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsPdfAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsDetailListingAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsDetailListingAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_DS_FIL_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDsFileAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsDsFileAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsDsFileAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_SUPP_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsSuppDocAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsSuppDocAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsSuppDocAsOtherOutputAsBilledEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_MEMO_AS_OTH_OUT_AS_BILLED", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsMemoAsOtherOutputAsBilledEntity { get; set; }
    public bool? IsMemoAsOtherOutputAsBilledEntityFutureValue { get; set; }
    public string IsMemoAsOtherOutputAsBilledEntityFutureDate { get; set; }

    // Other outputs as billing entity
    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_PDF_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPdfAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsPdfAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsPdfAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_LIST_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDetailListingAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsDetailListingAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsDetailListingAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_DS_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsDsFileAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsDsFileAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsDsFileAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "IS_MEMO_AS_OTH_OUT_AS_BILLING", UpdateFlavor = UpdateFlavor.FutureUpdateDate)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsMemoAsOtherOutputAsBillingEntity { get; set; }
    public bool? IsMemoAsOtherOutputAsBillingEntityFutureValue { get; set; }
    public string IsMemoAsOtherOutputAsBillingEntityFutureDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.Cgo, ElementGroupDisplayName = "Cargo",ElementTable = "MEM_CGO_CONFIGURATION", ElementName = "DOWN_CONVERT_ISTRAN_OLDIDEC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool DownConvertISTranToOldIdeccgo { set; get; }

    public bool CgoOldIdecMember { get; set; }

  }
}
