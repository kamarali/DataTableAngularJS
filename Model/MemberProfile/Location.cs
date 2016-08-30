using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class Location : ProfileEntity
  {
    private const int PleaseSelectId = 0;

    public string LocationCode { get; set; }
    
    //CMP597: make field future update field 
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations", ElementTable = "MEM_LOCATION", ElementName = "MEMBER_LEGAL_NAME", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string MemberLegalName { get; set; }
    public string MemberLegalNameFuturePeriod { get; set; }
    public string MemberLegalNameFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "MEMBER_COMMERCIAL_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    public string MemberCommercialName { get; set; }

    //CMP597: make field future update field 
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations", ElementTable = "MEM_LOCATION", ElementName = "IS_ACTIVE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public bool IsActive { get; set; }
    public string IsActiveFuturePeriod { get; set; }
    public string IsActiveFutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "REGISTRATION_ID", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string RegistrationId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string RegistrationIdFuturePeriod { get; set; }

    public string RegistrationIdFutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "TAX_VAT_REGISTRATION_NUMBER", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string TaxVatRegistrationNumber { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string TaxVatRegistrationNumberFuturePeriod { get; set; }

    public string TaxVatRegistrationNumberFutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "ADD_TAX_VAT_REGISTRATION_NUM", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string AdditionalTaxVatRegistrationNumber { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string AdditionalTaxVatRegistrationNumberFuturePeriod { get; set; }

    public string AdditionalTaxVatRegistrationNumberFutureValue { get; set; }
    //End Addition

    public Country Country { get; set; }

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations", ElementTable = "MEM_LOCATION", ElementName = "COUNTRY_CODE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory = true, IsFutureField = true, ControlType = ControlType.CountryDropdown, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string CountryId { get; set; }
    public string CountryIdDisplayValue { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CountryIdFuturePeriod { get; set; }
    public string CountryIdFutureValue { get; set; }
    public string CountryIdFutureDisplayValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "ADDRESS_LINE1", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory = true, IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string AddressLine1 { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string AddressLine1FuturePeriod { get; set; }

    public string AddressLine1FutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "ADDRESS_LINE2", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string AddressLine2 { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string AddressLine2FuturePeriod { get; set; }

    public string AddressLine2FutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "ADDRESS_LINE3", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string AddressLine3 { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string AddressLine3FuturePeriod { get; set; }

    public string AddressLine3FutureValue { get; set; }
    //End Addition

    //public City City { get; set; }

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "CITY_NAME", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory = true, IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string CityName { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CityNameFuturePeriod { get; set; }

    public string CityNameFutureValue { get; set; }
    //End Addition

    //[Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "SUB_DIVISION_CODE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    public string SubDivisionCode { get; set; }

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "SUB_DIVISION_NAME", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string SubDivisionName { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string SubDivisionNameFuturePeriod { get; set; }

    public string SubDivisionNameFutureValue { get; set; }
    //End Addition

    //CMP 653: Increased Visibility of Member Profile for ICH and ACH Operations Users
    //  Added AccessFlags for AchOps & IchOps
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "POSTAL_CODE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string PostalCode { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string PostalCodeFuturePeriod { get; set; }

    public string PostalCodeFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "LEGAL_TEXT", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeRelationId = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.TextArea, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.Member | AccessFlags.SisOps)]
    public string LegalText { get; set; }
    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string LegalTextFuturePeriod { get; set; }

    public string LegalTextFutureValue { get; set; }
    //End Addition
    /// <summary>
    /// Bank details
    /// </summary>
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "IBAN", UpdateFlavor = UpdateFlavor.ImmediateUpdate,IncludeRelationId=true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string Iban { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "SWIFT", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string Swift { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "BANK_CODE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string BankCode { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "BRANCH_CODE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string BranchCode { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "BANK_ACCOUNT_NUMBER", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string BankAccountNumber { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "BANK_ACCOUNT_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string BankAccountName { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "BANK_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public string BankName { get; set; }

    public Currency Currency { get; set; }

    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Locations",ElementTable = "MEM_LOCATION", ElementName = "CURRENCY_CODE_NUM", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.CurrencyDropdown, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public int? CurrencyId { get; set; }

    public string CurrencyIdDisplayValue { get; set; }

    public string CurrencyIdFutureDisplayValue { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps)]
    public bool IsUatpLocation { get; set; }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 18-06-2012
    /// Issue: SCP ID : 23155 - Changing locations becomes adding location 
    /// Reason: This property is used to check, whether clicked on 'Add Loction' button of the Location tab on Member profile screen.
    /// </summary>
    public int LocationIdFlag { get; set; }

    //CMP#622: MISC Outputs Split as per Location IDs
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.Member | AccessFlags.SisOps, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Location", ElementTable = "MEM_LOCATION", ElementName = "FILE_SPECIFIC_LOC_REQ", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    public bool FileSpecificLocReq { get; set; }

    //CMP#622: MISC Outputs Split as per Location IDs
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.Member)]
    [Audit(ElementGroup = ElementGroupType.Locations, ElementGroupDisplayName = "Location", ElementTable = "MEM_LOCATION", ElementName = "LOC_IINET_ACCOUNTID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    public string LociiNetAccountId { get; set; }
  }
}
