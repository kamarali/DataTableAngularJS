using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class AchConfiguration : ProfileEntity
  {
    private const int PleaseSelectId = 0;
    /// <summary>
    /// Stores the ACH membership status.
    /// </summary>
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.IchMemberShipStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public int AchMembershipStatusId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public int? AchMembershipStatusIdFutureValue { get; set; }

    public string AchMembershipStatusIdFuturePeriod { get; set; }

    public string AchMembershipStatusIdDisplayValue { get; set; }

    public string AchMembershipStatusIdFutureDisplayValue { get; set; }
    //End Addition

    /// <summary>
    /// Wraps the database membership status into a meaningful enumeration.
    /// </summary>
    public AchMembershipStatus AchMembershipStatus
    {
      get
      {
        return (AchMembershipStatus)AchMembershipStatusId;
      }
      set
      {
        AchMembershipStatusId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Stores the ACH category.
    /// </summary>

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH",ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_MEMBER_CATEGORY_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true, IgnoreValue = PleaseSelectId)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.AchCategoryDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public int AchCategoryId { get; set; }

    public string AchCategoryIdDisplayValue { get; set; }

    public string AchCategoryIdFutureDisplayValue { get; set; }

    /// <summary>
    /// Wraps the ACH category into a meaningful enumeration.
    /// </summary>
    public AchCategory AchCategory
    {
      get
      {
        return (AchCategory)AchCategoryId;
      }
      set
      {
        AchCategoryId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Entry date.
    /// </summary>
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public DateTime? EntryDate { get; set; }

    /// <summary>
    /// Termination date.
    /// </summary>
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    /// Reinstatement Period
    /// </summary>
    [ProfilePermission(IsMandatory = false, ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public DateTime? ReinstatementPeriod { get; set; }
    /// <summary>
    /// Comments from the ACH operations users.
    /// </summary>

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH",ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_OPS_COMMENTS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextArea, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public string AchOpsComments { get; set; }

    //Flag for 1111 - Period 1, Period 2, Period 3, Period 4

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_INV_SUB_PATTERN_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int AchClearanceInvoiceSubmissionPatternPaxId { get; set; }

    public string AchClearanceInvoiceSubmissionPatternPaxIdDisplayValue { get; set; }

    public string AchClearanceInvoiceSubmissionPatternPaxIdFutureDisplayValue { get; set; }

    public InvoicePeriod AchClearanceInvoiceSubmissionPatternPax
    {
      get
      {
         
        return (InvoicePeriod)AchClearanceInvoiceSubmissionPatternPaxId;
      }
      set
      {
        AchClearanceInvoiceSubmissionPatternPaxId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_INV_SUB_PATTERN_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int AchClearanceInvoiceSubmissionPatternCgoId { get; set; }

    public string AchClearanceInvoiceSubmissionPatternCgoIdDisplayValue { get; set; }

    public string AchClearanceInvoiceSubmissionPatternCgoIdFutureDisplayValue { get; set; }

    public InvoicePeriod AchClearanceInvoiceSubmissionPatternCgo
    {
      get
      {
        return (InvoicePeriod)AchClearanceInvoiceSubmissionPatternCgoId;
      }
      set
      {
        AchClearanceInvoiceSubmissionPatternCgoId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_INV_SUB_PATTERN_MSC", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int AchClearanceInvoiceSubmissionPatternMiscId { get; set; }

    public string AchClearanceInvoiceSubmissionPatternMiscIdDisplayValue { get; set; }

    public string AchClearanceInvoiceSubmissionPatternMiscIdFutureDisplayValue { get; set; }
    public InvoicePeriod AchClearanceInvoiceSubmissionPatternMisc
    {
      get
      {
        return (InvoicePeriod)AchClearanceInvoiceSubmissionPatternMiscId;
      }
      set
      {
        AchClearanceInvoiceSubmissionPatternMiscId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "ACH_INV_SUB_PATTERN_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int AchClearanceInvoiceSubmissionPatternUatpId { get; set; }

    public string AchClearanceInvoiceSubmissionPatternUatpIdDisplayValue { get; set; }

    public string AchClearanceInvoiceSubmissionPatternUatpIdFutureDisplayValue { get; set; }
    public InvoicePeriod AchClearanceInvoiceSubmissionPatternUatp
    {
      get
      {
        return (InvoicePeriod)AchClearanceInvoiceSubmissionPatternUatpId;
      }
      set
      {
        AchClearanceInvoiceSubmissionPatternUatpId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "INTRCLRNC_INV_SUB_PATTERN_PAX", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int InterClearanceInvoiceSubmissionPatternPaxId { get; set; }

    public string InterClearanceInvoiceSubmissionPatternPaxIdDisplayValue { get; set; }

    public string InterClearanceInvoiceSubmissionPatternPaxIdFutureDisplayValue { get; set; }
    public InvoicePeriod InterClearanceInvoiceSubmissionPatternPax
    {
      get
      {
        return (InvoicePeriod)InterClearanceInvoiceSubmissionPatternPaxId;
      }
      set
      {
        InterClearanceInvoiceSubmissionPatternPaxId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "INTRCLRNC_INV_SUB_PATTERN_CGO", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]

    public int InterClearanceInvoiceSubmissionPatternCgoId { get; set; }
    public string InterClearanceInvoiceSubmissionPatternCgoIdDisplayValue { get; set; }
    public string InterClearanceInvoiceSubmissionPatternCgoIdFutureDisplayValue { get; set; }

    public InvoicePeriod InterClearanceInvoiceSubmissionPatternCgo
    {
      get
      {
        return (InvoicePeriod)InterClearanceInvoiceSubmissionPatternCgoId;
      }
      set
      {
        InterClearanceInvoiceSubmissionPatternCgoId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "INTRCLRNC_INV_SUB_PATTERN_MSC", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]
    public int InterClearanceInvoiceSubmissionPatternMiscId { get; set; }
    public string InterClearanceInvoiceSubmissionPatternMiscIdDisplayValue { get; set; }
    public string InterClearanceInvoiceSubmissionPatternMiscIdFutureDisplayValue { get; set; }
    public InvoicePeriod InterClearanceInvoiceSubmissionPatternMisc
    {
      get
      {
        return (InvoicePeriod)InterClearanceInvoiceSubmissionPatternMiscId;
      }
      set
      {
        InterClearanceInvoiceSubmissionPatternMiscId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.AchConfiguration, ElementGroupDisplayName = "ACH", ElementTable = "MEM_ACH_CONFIGURATION", ElementName = "INTRCLRNC_INV_SUB_PATTERN_UATP", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IncludeDisplayNames = true)]

    public int InterClearanceInvoiceSubmissionPatternUatpId { get; set; }
    public string InterClearanceInvoiceSubmissionPatternUatpIdDisplayValue { get; set; }
    public string InterClearanceInvoiceSubmissionPatternUatpIdFutureDisplayValue { get; set; }

    public InvoicePeriod InterClearanceInvoiceSubmissionPatternUatp
    {
      get
      {
        return (InvoicePeriod)InterClearanceInvoiceSubmissionPatternUatpId;
      }
      set
      {
        InterClearanceInvoiceSubmissionPatternUatpId = Convert.ToInt32(value);
      }
    }

    public bool IsSettlementViaIchPax { get; set; }

    public bool IsSettlementViaIchCgo { get; set; }

    public bool IsSettlementViaIchMisc { get; set; }

    public bool IsSettlementViaIchUatp { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string DiaplayMemberText { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.SuspensionDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public DateTime? StatusChangedDate { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DefaultSuspensionDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.AchOps)]
    public DateTime? DefaultSuspensionDate { get; set; }

    public string ContactList { get; set; }

    //Following fields are used for saving exception details.They are not present in database
    public string PaxExceptionMemberAddList { get; set; }

    public string PaxExceptionMemberDeleteList { get; set; }

    public string CgoExceptionMemberAddList { get; set; }

    public string CgoExceptionMemberDeleteList { get; set; }

    public string MiscExceptionMemberAddList { get; set; }

    public string MiscExceptionMemberDeleteList { get; set; }

    public string UatpExceptionMemberAddList { get; set; }

    public string UatpExceptionMemberDeleteList { get; set; }
    //End Addition

    //Denotes future period for setting pax exception values
    public string PaxExceptionFuturePeriod { get; set; }
    public string PaxExceptionFutureValue { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool HasPaxExceptionMembers { get; set; }


    //Denotes future period for setting pax exception values
    public string CgoExceptionFuturePeriod { get; set; }
    public string CgoExceptionFutureValue { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool HasCgoExceptionMembers { get; set; }

    //Denotes future period for setting pax exception values
    public string MiscExceptionFuturePeriod { get; set; }
    public string MiscExceptionFutureValue { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool HasMiscExceptionMembers { get; set; }


    //Denotes future period for setting pax exception values
    public string UatpExceptionFuturePeriod { get; set; }
    public string UatpExceptionFutureValue { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool HasUatpExceptionMembers { get; set; }

    public AchConfiguration()
    {
      AchMembershipStatus = AchMembershipStatus.NotAMember;
    }

    // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV
    public string MemberNameFutureValue { get; set; }

    public string MemberNameChangePeriodFrom { get; set; }
  }
}
