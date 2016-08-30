using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Common;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class Contact : ProfileEntity //: EntityBase<int> 
  {
    public Contact()
    {
      ContactTypes = new List<ContactType>();
    }

    private const int PleaseSelectId = 0;
    
    //Link to user
    public int IsUserId { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "STAFF_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string StaffId { get; set; }

    //Contact status
    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "IS_ACTIVE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public bool IsActive { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "SALUTATION_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.SalutationDropdown, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public int? SalutationId { get; set; }
    public string SalutationIdDisplayValue { get; set; }
    public string SalutationIdFutureDisplayValue { get; set; }


    public Salutation? Salutation
    {
      get
      {
        if (SalutationId != null)
        {
          return (Salutation)SalutationId;
        }
        else
        {
          return null;
        }
      }
      set
      {
        //SalutationId = Convert.ToInt32(value);
        SalutationId = value > 0 ? Convert.ToInt32(value) : (int?) null;
      }
    }

    //This field is added for sending salutation as a string to ICH
    public string DisplaySalutation { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "FIRST_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory=true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string FirstName { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "LAST_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string LastName { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "EMAIL_ADDRESS", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string EmailAddress { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "POSITION_OR_TITLE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string PositionOrTitle { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "DIVISION", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string Division { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "DEPARTMENT", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string Department { get; set; }

    public Location Location { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "LOCATION_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.LocationDropdown, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public int LocationId { get; set; }
    public string LocationIdDisplayValue { get; set; }
    public string LocationIdFutureDisplayValue { get; set; }

    public Country Country { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "COUNTRY_CODE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(ControlType = ControlType.CountryDropdown, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string CountryId { get; set; }
    public string CountryIdDisplayValue { get; set; }
    public string CountryIdFutureDisplayValue { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "ADDRESS_LINE1", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string AddressLine1 { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "ADDRESS_LINE2", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string AddressLine2 { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "ADDRESS_LINE3", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string AddressLine3 { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "CITY_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string CityName { get; set; }

    public string SubDivisionCode { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "SUB_DIVISION_NAME", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string SubDivisionName { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "POSTAL_CODE", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string PostalCode { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "PHONE_NUMBER1", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string PhoneNumber1 { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "PHONE_NUMBER2", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string PhoneNumber2 { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "MOBILE_NUMBER", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string MobileNumber { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "FAX_NUMBER", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string FaxNumber { get; set; }

    [Audit(ElementGroup = ElementGroupType.Contacts, ElementGroupDisplayName = "Contacts",ElementTable = "MEM_CONTACT", ElementName = "SITA_ADRESS", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeRelationId = true)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.All, WriteAccessFlags = AccessFlags.All)]
    public string SitaAddress { get; set; }

    public string Name
    {
      get
      {
        return FirstName + " " + LastName;
      }
    }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<ContactType> ContactTypes { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string ContactList { get; set; }

    public bool PrimaryContact { get; set; }

    public bool AdviceContact { get; set; }

    public bool ClaimConfirmationContact { get; set; }

    public bool ClearanceInitializationContact { get; set; }

    public bool FinancialContact { get; set; }

    public bool IsContactIsUser { get; set;}
  }
}
