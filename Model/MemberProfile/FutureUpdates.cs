using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class FutureUpdates : EntityBase<int>
  {
    public Member BilateralMember { get; set; }

    //Marked field as nullable since bilateral member functionality is not present currently
    public int? BilateralMemberId { get; set; }

    public int ElementGroupTypeId { get; set; }

    public string ElementName { get; set; }

    public int ActionTypeId { get; set; }

    public ActionType ActionType
    {
      get
      {
        return (ActionType)ActionTypeId;
      }
      set
      {
        ActionTypeId = Convert.ToInt32(value);
      }
    }

    public string OldVAlue { get; set; }

    public string NewVAlue { get; set; }

    //Field is marked as nullable since user can set either ChangeEffectivePeriod or ChangeEffectiveOn
    public DateTime? ChangeEffectiveOn { get; set; }

    public DateTime ModifiedOn { get; set; }

    public int UserId { get; set; }

    //In case of bilateral element
    public int ChangeApprovedByUserId { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    //Field is marked as nullable since user can set either ChangeEffectivePeriod or ChangeEffectiveOn
    public DateTime? ChangeEffectivePeriod { get; set; }

    public string DisplayChangeEffectivePeriod
    {
      get
      {
        string period = String.Empty;
        //string ChangeEffectivePeriodFormat = String.Empty;
        //if (ChangeEffectivePeriod != null)
        //{
        period = (ChangeEffectivePeriod != null ? ChangeEffectivePeriod.Value.ToString("dd/MMM/yyyy") : string.Empty);
          
          if (period == "01-01-0001")
          {
            period = String.Empty;
          }
          else if (period.Length != 0)
          {
            period = period.Substring(3, 3) + " " + period.Substring(7, 4) + " " + "P" + period.Substring(1, 1);
           
          }
   
        return period;
      }

    }

    public string DisplayChangeEffectiveDate
    {
      get
      {
        string displayDate = String.Empty;

        displayDate = (ChangeEffectiveOn != null ? ChangeEffectiveOn.Value.ToString("dd/MMM/yyyy") : string.Empty);


        return displayDate;
      }

    }

    public bool IsChangeApplied { get; set; }

    public string DisplayActionType
    {
        get
        {
            return EnumList.GetActionTypeDisplayValue((ActionType)(ActionTypeId));
        }
    }

    public string DisplayGroup { get; set; }

    public string DisplayBilateralMember
    {
        get
        {
            return BilateralMember != null ? BilateralMember.CommercialName : string.Empty;
        }
    }

    //public int? ContactId { get; set; }
    //public Contact Contact { get; set; }

    //public Location Location { get; set; }
    //public int? LocationId { get; set; }

    public int? RelationId { get; set; }

    public string RelationIdDisplayName{ get; set; }
   
    public string TableName { get; set; }

    public string OldValueDisplayName { get; set; }

    public string NewValueDisplayName { get; set; }

      
  }
}
