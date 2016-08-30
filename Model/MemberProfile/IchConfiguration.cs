using System;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class IchConfiguration : ProfileEntity
  {
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.IchMemberShipStatusDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public int IchMemberShipStatusId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public int? IchMemberShipStatusIdFutureValue { get; set; }

    public string IchMemberShipStatusIdFuturePeriod { get; set; }

    public string IchMemberShipStatusIdDisplayValue { get; set; }

    public string IchMemberShipStatusIdFutureDisplayValue { get; set; }
    //End Addition

    public IchMemberShipStatus IchMemberShipStatus
    {
      get
      {
        return (IchMemberShipStatus)IchMemberShipStatusId;
      }
      set
      {
        IchMemberShipStatusId = Convert.ToInt32(value);
      }
    }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public DateTime? EntryDate { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DatePicker, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public DateTime? TerminationDate { get; set; }

    [ProfilePermission(IsMandatory = false, ControlType = ControlType.SimpleTextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public DateTime? ReinstatementPeriod { get; set; }

    public IchZoneType IchZone
    {
      get
      {
        return (IchZoneType)IchZoneId;
      }
      set
      {
        IchZoneId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "ZONE_ID", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.IchZoneDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public int IchZoneId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IchZoneIdFuturePeriod { get; set; }

    public int? IchZoneIdFutureValue { get; set; }

    public string IchZoneIdDisplayValue { get; set; }

    public string IchZoneIdFutureDisplayValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "ICH_MEMBER_CATEGORY_ID", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(IsFutureField = true, IsMandatory = true, ControlType = ControlType.IchCategoryDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public int IchCategoryId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IchCategoryIdFuturePeriod { get; set; }

    public int? IchCategoryIdFutureValue { get; set; }

    public string IchCategoryIdDisplayValue { get; set; }

    public string IchCategoryIdFutureDisplayValue { get; set; }
    //End Addition

    public IchCategory IchCategory
    {
      get
      {
        return (IchCategory)IchCategoryId;
      }
      set
      {
        IchCategoryId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "IS_EARLY_CALL_DAY", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool IsEarlyCallDay { get; set; }

    //CMP #625: New Fields in ICH Member Profile Update XML.
    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "ICH_ACCOUNT_ID", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public string IchAccountId { get; set; }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "IS_AGGREGATOR", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool IsAggregator { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string IsAggregatorFuturePeriod { get; set; }

    public bool? IsAggregatorFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "AGGREGATED_BY", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    public int? AggregatedById { get; set; }
    
    //Added code in get method .The value of field will be determined by Aggregated member object
    private string _aggregatedByText;
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.None)]
    public string AggregatedByText
    {
      get
      {
        return !string.IsNullOrEmpty(_aggregatedByText) ? _aggregatedByText : AggregatedBy != null ? string.Format("{0}-{1}-{2}", AggregatedBy.MemberCodeAlpha, AggregatedBy.MemberCodeNumeric, AggregatedBy.CommercialName) : string.Empty;
      }
      set
      {
        _aggregatedByText = value;
      }
    }

    //Added for storing future sponsored by value*/
    public int? AggregatedByTextFutureValueId { get; set; }

    public string AggregatedByTextFuturePeriod { get; set; }
    
    public Member AggregatedByFuture { get; set; }

    //Added code in get method .The value of field will be determined by sponsored member object

    private string _aggregatedByTextFutureValue;

    public string AggregatedByTextFutureValue
    {
      get
      {
        return !string.IsNullOrEmpty(_aggregatedByTextFutureValue) ? _aggregatedByTextFutureValue : AggregatedByFuture != null ? string.Format("{0}-{1}-{2}", AggregatedByFuture.MemberCodeAlpha, AggregatedByFuture.MemberCodeNumeric, AggregatedByFuture.CommercialName) : string.Empty;
      }
      set
      {
        _aggregatedByTextFutureValue = value;
      }
    }

    public Member AggregatedBy { get; set; }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "AGGREGATED_TYPE", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true, IgnoreValue = 0)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.AggregatedTypeDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public int? AggregatedTypeId { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string AggregatedTypeIdFuturePeriod { get; set; }

    public int? AggregatedTypeIdFutureValue { get; set; }

    public string AggregatedTypeIdDisplayValue { get; set; }

    public string AggregatedTypeIdFutureDisplayValue { get; set; }
    //End Addition

    public AggregatedType? AggregatedType
    {

      get
      {
        //return (AggregatedType)AggregatedTypeId;
        AggregatedType? aggregatedTypeValue = null;
        if (AggregatedTypeId.HasValue) aggregatedTypeValue = (AggregatedType)AggregatedTypeId;
        return aggregatedTypeValue;
      }
      set
      {
        AggregatedTypeId = Convert.ToInt32(value);
      }
    }

    public Member SponsoredBy { get; set; }

    public Member SponsoredByFuture { get; set; }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "SPONSORED_BY", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod)]
    public int? SponsoredById { get; set; }
    
    private string _sponseredByText;
    //Added code in get method .The value of field will be determined by sponsored member object
    [ProfilePermission(ControlType = ControlType.TextBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.None)]
    public string SponseredByText
    {
      get
      {
        return !string.IsNullOrEmpty(_sponseredByText) ? _sponseredByText : SponsoredBy != null ? string.Format("{0}-{1}-{2}", SponsoredBy.MemberCodeAlpha, SponsoredBy.MemberCodeNumeric, SponsoredBy.CommercialName) : string.Empty;
      }
      set
      {
        _sponseredByText = value;
      }
    }
  

    //Added for storing future sponsored by value*/
    public int? SponseredByTextFutureValueId { get; set; }

    public string SponseredByTextFuturePeriod { get; set; }

    //Added code in get method .The value of field will be determined by sponsored member object

    private string _sponseredByTextFutureValue;

    public string SponseredByTextFutureValue
    {
      get
      {
        return !string.IsNullOrEmpty(_sponseredByTextFutureValue) ? _sponseredByTextFutureValue : SponsoredByFuture != null ? string.Format("{0}-{1}-{2}", SponsoredByFuture.MemberCodeAlpha, SponsoredByFuture.MemberCodeNumeric, SponsoredByFuture.CommercialName) : string.Empty;
      }
      set
      {
        _sponseredByTextFutureValue = value;
      }
    }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "ICH_WEB_REPORT_OPTIONS", UpdateFlavor = UpdateFlavor.ImmediateUpdate, IgnoreValue = 0)]
    [ProfilePermission(IsMandatory = true, ControlType = ControlType.ICHWebReportOptionsDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public int IchWebReportOptionsId { get; set; }

    public IchWebReportOptions IchWebReportOptions
    {
      get
      {
        return (IchWebReportOptions)IchWebReportOptionsId;
      }
      set
      {
        IchWebReportOptionsId = Convert.ToInt32(value);
      }
    }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH",ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "ICH_OPS_COMMENTS", UpdateFlavor = UpdateFlavor.ImmediateUpdate)]
    [ProfilePermission(ControlType = ControlType.TextArea, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public string IchOpsComments { get; set; }

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "CAN_SUBMIT_PAX_IN_F12_FILES", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool CanSubmitPaxInF12Files { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CanSubmitPaxInF12FilesFuturePeriod { get; set; }

    public bool? CanSubmitPaxInF12FilesFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "CAN_SUBMIT_CGO_IN_F12_FILES", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool CanSubmitCargoInF12Files { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CanSubmitCargoInF12FilesFuturePeriod { get; set; }

    public bool? CanSubmitCargoInF12FilesFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "CAN_SUBMIT_MSC_IN_F12_FILES", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool CanSubmitMiscInF12Files { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CanSubmitMiscInF12FilesFuturePeriod { get; set; }

    public bool? CanSubmitMiscInF12FilesFutureValue { get; set; }
    //End Addition

    [Audit(ElementGroup = ElementGroupType.Ich, ElementGroupDisplayName = "ICH", ElementTable = "MEM_ICH_CONFIGURATION", ElementName = "CAN_SUBMIT_UATP_IN_F12_FILES", UpdateFlavor = UpdateFlavor.FutureUpdatePeriod, IncludeDisplayNames = true)]
    [ProfilePermission(IsFutureField = true, ControlType = ControlType.CheckBox, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public bool CanSubmitUatpinF12Files { get; set; }

    //Fields added for setting future dated updates
    //These fields are included only in model and they are not present in database
    public string CanSubmitUatpinF12FilesFuturePeriod { get; set; }

    public bool? CanSubmitUatpinF12FilesFutureValue { get; set; }
    //End Addition

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public string DisplayMemberText { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.SuspensionDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public DateTime? StatusChangedDate { get; set; }

    [ProfilePermission(IsMandatory = true, ControlType = ControlType.DefaultSuspensionDropdown, ReadAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps | AccessFlags.Member, WriteAccessFlags = AccessFlags.SisOps | AccessFlags.IchOps)]
    public DateTime? DefaultSuspensionDate { get; set; }

    public string ContactList { get; set; }

    public string CommercialName { get; set; }

    //Denotes future period for setting sponsoror values
    public string SponsororFuturePeriod { get; set; }
    public string SponsororFutureValue { get; set; }
    public string SponsororAddList { get; set; }
    public string SponsororDeleteList { get; set; }

    //Denotes future period for setting aggregator values
    public string AggregatorFuturePeriod { get; set; }
    public string AggregatorFutureValue { get; set; }
    public string AggregatorAddList { get; set; }
    public string AggregatorDeleteList { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool ISSponsororMember { get; set; }

    //Denotes whether member is a sponsoror for any other members
    public bool ISAggregatorMember { get; set; }

    //
    public string AggregatorAddCount = "0";

    //
    public string AggrAvailMembersDisabled = "-1";

    public IchConfiguration()
    {
        IchMemberShipStatus = IchMemberShipStatus.NotAMember;
    }


    /* SCP88742: ICH Member Profile XB-B41 - Not a Member status issue 
       Date: 02-Mar-2013
       Desc: New variable added to model to keep track of change in member status.
    */
    public int IchMemberShipStatusIdSelectedOnUi { get; set; }

    public IchMemberShipStatus IchMemberShipStatusSelectedOnUi
    {
        get
        {
            return (IchMemberShipStatus)IchMemberShipStatusIdSelectedOnUi;
        }
        set
        {
            IchMemberShipStatusIdSelectedOnUi = Convert.ToInt32(value);
        }
    }


    // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV
    public string MemberNameFutureValue { get; set; }

    public string MemberNameChangePeriodFrom { get; set; }
 }

}
