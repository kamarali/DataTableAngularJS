using Iata.IS.Model.MemberProfile.Enums;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class EBillingConfiguration : ProfileEntity
  {
    [ProfilePermission(ControlType = ControlType.Label, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.None)]
    public bool IsDigitalSignatureRequired { get; set; }
    public string IsDigitalSignatureRequiredDisplay { get; set; }

    [ProfilePermission(ControlType = ControlType.Label, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.None)]
    public bool IsDsVerificationRequired { get; set; }
    public string IsDsVerificationRequiredDisplay { get; set; }
   
    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing",ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_TEXT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField =true, ControlType = ControlType.TextArea, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string LegalText { get; set; }
    public string LegalTextFuturePeriod { get; set; }
    public string LegalTextFutureValue { get; set; }

    [ProfilePermission(ControlType = ControlType.Label, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.None)]
    public bool IsLegalArchievingRequired { get; set; }
    public string IsLegalArchievingRequiredDisplay { get; set; }

    
    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "IS_PAYABLE_LEGAL_ARCHIVE_OPT", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsPayableLegalArchievingOptional { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "PAYABLE_LEGAL_ARCHIVING_PERIOD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int? PayableLegalArchievingPeriod { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "IS_REC_LEGAL_ARCHIVE_OPTIONAL", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsRecievableLegalArchievingOptional { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "REC_LEGAL_ARCHIVING_PERIOD", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public int? RecievableLegalArchievingPeriod { get; set; }

    //Start Update by Upendra Yadav

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_PAX_REC_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforPaxRecInv { get; set; }
    public string LegalArchRequiredforPaxRecInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforPaxRecInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_PAX_PAY_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforPaxPayInv { get; set; }
    public string LegalArchRequiredforPaxPayInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforPaxPayInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_CGO_REC_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforCgoRecInv { get; set; }
    public string LegalArchRequiredforCgoRecInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforCgoRecInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_CGO_PAY_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforCgoPayInv { get; set; }
    public string LegalArchRequiredforCgoPayInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforCgoPayInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_MISC_REC_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforMiscRecInv { get; set; }
    public string LegalArchRequiredforMiscRecInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforMiscRecInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_MISC_PAY_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforMiscPayInv { get; set; }
    public string LegalArchRequiredforMiscPayInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforMiscPayInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_UATP_REC_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforUatpRecInv { get; set; }
    public string LegalArchRequiredforUatpRecInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforUatpRecInvFutureValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "LEGAL_ARC_REQ_UATP_PAY_INV", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool LegalArchRequiredforUatpPayInv { get; set; }
    public string LegalArchRequiredforUatpPayInvFuturePeriod { get; set; }
    public bool? LegalArchRequiredforUatpPayInvFutureValue { get; set; }
    
   //
    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_PAX_REC_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsPaxRecArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_PAX_PAY_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsPaxPayArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_CGO_REC_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsCgoRecArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_CGO_PAY_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsCgoPayArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_MISC_REC_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsMiscRecArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_MISC_PAY_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsMiscPayArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_UATP_REC_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsUatpRecArch { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "INCLUDE_LISTINGS_UATP_PAY_ARC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IncludeListingsUatpPayArch { get; set; }

      
      //

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing",ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "HIDE_USERNAMES_IN_AUDIT_TRAILS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool IsHideUserNameInAuditTrail { get; set; }

    //***************** Start CMP597 ( Weekly Reference Data and Contact Data )  Changes  *******************
    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "CHANGE_INFO_REF_DATA_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool ChangeInfoRefDataPax { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "CHANGE_INFO_REF_DATA_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool ChangeInfoRefDataCgo { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "CHANGE_INFO_REF_DATA_MISC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool ChangeInfoRefDataMisc { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "CHANGE_INFO_REF_DATA_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool ChangeInfoRefDataUatp { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_REF_DATA_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteRefDataPax { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_REF_DATA_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteRefDataCgo { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_REF_DATA_MISC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteRefDataMisc { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_REF_DATA_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteRefDataUatp { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_CONTACTS_DATA_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteContactsDataPax { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_CONTACTS_DATA_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteContactsDataCgo { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_CONTACTS_DATA_MISC", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteContactsDataMisc { get; set; }

    [Audit(ElementGroup = ElementGroupType.EBilling, ElementGroupDisplayName = "e-Billing", ElementTable = "MEM_E_BILLING_CONFIGURATION", ElementName = "COMPLETE_CONTACTS_DATA_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public bool CompleteContactsDataUatp { get; set; }

    public string IinetAccountIdPax { get; set; }
    public string IinetAccountIdCgo { get; set; }
    public string IinetAccountIdMisc { get; set; }
    public string IinetAccountIdUatp { get; set; }
    //*****************End CMP597 Changes*******************

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string ContactList { get; set; }

    public string DSReqCountriesAsBillingFuturePeriod { get; set; }

    public string DSReqCountriesAsBillingFutureValue { get; set; }

    /// <summary>
    /// Denotes whether member ds Countries set already for current period.
    /// </summary>
    public bool HasDSReqCountriesAsBilling { get; set; }
    
    public string DSReqCountriesAsBilledFuturePeriod { get; set; }

    public string DSReqCountriesAsBilledFutureValue { get; set; }

    /// <summary>
    /// Denotes whether member ds Countries set already for current period.
    /// </summary>
    public bool HasDSReqCountriesAsBilled { get; set; }

    /// <summary>
    /// Used for passing countrylist from UI layer to business layer.
    /// </summary>
    public string CountryList { get; set; }

    public string BilledCountiesToAdd { get; set; }
    public string BilledCountiesToRemove { get; set; }
    public string BillingCountiesToAdd { get; set; }
    public string BillingCountiesToRemove { get; set; }

    public bool ISUatpInvIgnoreFromDsproc { get; set; }

    //CMP #666: MISC Legal Archiving Per Location ID
    public string MiscRecArchivingLocs { get; set; }

    public string MiscPayArchivingLocs { get; set; }

    public int RecAssociationType { get; set; }

    public int PayAssociationType { get; set; }

  }
}
