using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class Member : ProfileEntity //EntityBase<int> 
  {
    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "MEMBER_CODE_NUMERIC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(IsMandatory=true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public string MemberCodeNumeric { get; set; }

    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "MEMBER_CODE_ALPHA", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public string MemberCodeAlpha { get; set; }


    // CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login
    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "MEMBER_LEGAL_NAME", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public string LegalName { get; set; }
    public string LegalNameFuturePeriod { get; set; }
    public string LegalNameFutureValue { get; set; }

    //SCP276172 - IS-WEB Member Profile Mandatory Field validation issue
    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "MEMBER_COMMERCIAL_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string CommercialName { get; set; }

    // This property will be used for displaying searched member details
    public string DisplayCommercialName
    {
      get
      {
        return !string.IsNullOrEmpty(CommercialName) ? string.Format("{0}-{1}-{2}", MemberCodeAlpha, MemberCodeNumeric, CommercialName) : string.Empty;
      }
    }

    public string ContactList { get; set; }

    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.MemberStatusDropdown, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public int IsMembershipStatusId { get; set; }
    public int? IsMembershipStatusIdFutureValue { get; set; }
    public string IsMembershipStatusIdFuturePeriod { get; set; }
    public string IsMembershipStatusIdDisplayValue { get; set; }
    public string IsMembershipStatusIdFutureDisplayValue { get; set; }

    public MemberStatus IsMembershipStatus
    {
      get
      {
        return (MemberStatus)IsMembershipStatusId;
      }
      set
      {
        IsMembershipStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_MEMBERSHIP_SUB_STATUS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.MemberSubStatusDropdown, IsMandatory = true, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public int? IsMembershipSubStatusId { get; set; }

    public string IsMembershipSubStatusIdDisplayValue { get; set; }

    public string IsMembershipSubStatusIdFutureDisplayValue { get; set; }

    public int? IsMigrationStatusId { get; set; }

    public IsMigrationStatus? IsMigrationStatus
    {
      get
      {
        IsMigrationStatus? migrationStatus = null;
        if (IsMigrationStatusId.HasValue)
        {
          migrationStatus = (IsMigrationStatus)IsMigrationStatusId;
        }

        return migrationStatus;
      }
      set
      {
        IsMigrationStatusId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IATA_MEMBERSHIP_STATUS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IataMemberStatus { get; set; }

    // It will be derived from ACH and ICH configuration.
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.None)]
    public bool IchMemberStatus { get; set; }

    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.None)]
    public bool AchMemberStatus { get; set; }

    // Not there in member profile 18.0 excel sheet
    public bool DualMemberStatus { get; set; }

    [Audit(ElementGroup = ElementGroupType.MemberDetails, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_OPS_COMMENTS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextArea, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.AchOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string IsOpsComments { get; set; }

    public Location DefaultLocation { get; set; }

    public PassengerConfiguration PassengerConfiguration { get; set; }

    public MiscellaneousConfiguration MiscellaneousConfiguration { get; set; }

    public CargoConfiguration CargoConfiguration { get; set; }

    public UatpConfiguration UatpConfiguration { get; set; }

    public IchConfiguration IchConfiguration { get; set; }

    public AchConfiguration AchConfiguration { get; set; }

    public EBillingConfiguration EBillingConfiguration { get; set; }

    public TechnicalConfiguration TechnicalConfiguration { get; set; }

    public bool IsPartillyCreated { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? EntryDate { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps)]
    public DateTime? TerminationDate { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_SAMP_MEM_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableSampMemInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_SAMP_SC_MEM_INFO_CONT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableSampScMemInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_SAMP_QSMART_INFO_CONT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableSampQSmartInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_OLD_IDEC_SC_INFO_CONT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableOldIdecScInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_FIRST_N_FINAL_INFO_CONT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableFirstNFinalInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_FNF_ASG_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableFnfAsgInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_FNF_AIA_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableFnfAiaInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_RAWG_MEM_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableRawgMemInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_IAWG_MEM_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableIawgMemInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_ICH_PANEL_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableIchPanelInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_E_INV_WG_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableEInvWgInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ENABLE_SIS_SC_INFO_CONTACT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool EnableSisScInfoContact { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "ALLOW_CONTACT_DETAILS_DOWNLOAD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool AllowContactDetailsDownload { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_PARTICIPATE_IN_VALUE_DETERM", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsParticipateInValueDetermination { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_PARTICIPATE_IN_VALUE_CONFIR", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsParticipateInValueConfirmation { get; set; }

    // This property is added for sending concatenated member commercial name and member numeric code to ICH web service.
    public string MemberCode { get; set; }

    public int IchMemberStatusId { get; set; }
    public int AchMemberStatusId { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "DIGITAL_SIGN_APPLICATION", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField=true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool DigitalSignApplication { get; set; }

    public bool? DigitalSignApplicationFutureValue { get; set; }
    public string DigitalSignApplicationFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "DIGITAL_SIGN_VERIFICATION", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool DigitalSignVerification { get; set; }

    public bool? DigitalSignVerificationFutureValue { get; set; }
    public string DigitalSignVerificationFuturePeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_LEGAL_ARCHIVING_REQUIRED", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool LegalArchivingRequired { get; set; }
    
      
    // Added for CMP-409 By Upendra Yadav

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_MERGED", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsMerged { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsMergedFuturePeriod { get; set; }
    public bool? IsMergedFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "MERGER_DATE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true , ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string ActualMergerDate { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string ActualMergerDateFuturePeriod { get; set; }
    public string ActualMergerDateFutureValue { get; set; }
    //End Addition

    public Member ParentMember { get; set; }
    private string _parentMemberName;
    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "PARENT_MEMBER_ID", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod,IncludeDisplayNames=true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public int ParentMemberId { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string ParentMemberIdFuturePeriod { get; set; }
    public int? ParentMemberIdFutureValue { get; set; }
    public string ParentMemberIdDisplayValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_parentMemberName))
                _parentMemberName = ParentMember == null ? string.Empty : string.Format("{0}-{1}-{2}", ParentMember.MemberCodeAlpha, ParentMember.MemberCodeNumeric,
                                             ParentMember.CommercialName);
            return _parentMemberName;
        }
        set { _parentMemberName = value; }

    }
    public string ParentMemberIdFutureDisplayValue { get; set; }
    //End Addition

    // end for CMP-409 

    
    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "CDC_COMPARTMENTID_FOR_INV", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string CdcCompartmentIDforInv { get; set; }
      

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "IS_UATP_INV_HANDLED_BY_ATCAN", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField=true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool UatpInvoiceHandledbyAtcan { get; set; }
    public string UatpInvoiceHandledbyAtcanFuturePeriod { get; set; }
    public bool? UatpInvoiceHandledbyAtcanFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "PAX_OLD_IDEC_MEMBER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool PaxOldIdecMember { get; set; }

    [Audit(ElementGroup = ElementGroupType.SISOperations, ElementGroupDisplayName = "Member Details", ElementTable = "MEMBER_DETAILS", ElementName = "CGO_OLD_IDEC_MEMBER", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool CgoOldIdecMember { get; set; }

    public Member()
    {
      IsMembershipStatus = MemberStatus.Basic;
    }
  }
}
