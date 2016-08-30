using System;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Model.Base
{
  public class CorrespondenceBase : EntityBase<Guid>
  {
    public long? CorrespondenceNumber { get; set; }

    public DateTime CorrespondenceDate { get; set; }

    public int CorrespondenceStage { get; set; }

    public User CorrespondenceOwner
    {
      get;
      set;
    }

    public int CorrespondenceOwnerId { get; set; }

    private string _correspondenceOwnerName;
    public string CorrespondenceOwnerName
    {
      get
      {
        return CorrespondenceOwner != null ? string.Format("{0} {1}", CorrespondenceOwner.FirstName, CorrespondenceOwner.LastName) : _correspondenceOwnerName;
      }
      set
      {
        _correspondenceOwnerName = value;
      }
    }
    /// <summary>
    /// For Navigation to <see cref="Member"/>. 
    /// </summary>
    public int FromMemberId { get; set; }

    private string _fromMemberText;

    public string FromMemberText
    {
      get
      {
        return FromMember != null ? FromMember.CommercialName : _fromMemberText;
      }
      set
      {
        _fromMemberText = value;
      }
    }

    public Member FromMember { get; set; }

    /// <summary>
    /// For Navigation to <see cref="Member"/>. 
    /// </summary>
    public int ToMemberId { get; set; }

    private string _toMemberText;

    public string ToMemberText
    {
      get
      {
        return ToMember != null ? ToMember.CommercialName : _toMemberText;
      }
      set
      {
        _toMemberText = value;
      }
    }

    public Member ToMember { get; set; }

    public string FromEmailId { get; set; }

    public string ToEmailId { get; set; }

    /* CMP#657: Retention of Additional Email Addresses in Correspondences
       Adding code to get email ids from initiator and non-initiator*/
    public string AdditionalEmailInitiator { get; set; }

    public string AdditionalEmailNonInitiator { get; set; }

    public decimal AmountToBeSettled { get; set; }

    public string OurReference { get; set; }

    public string YourReference { get; set; }

    public CorrespondenceStatus CorrespondenceStatus
    {
      get
      {
        return (CorrespondenceStatus)CorrespondenceStatusId;
      }
      set
      {
        CorrespondenceStatusId = Convert.ToInt32(value);
      }
    }

    public int CorrespondenceStatusId { get; set; }

    public string Subject { get; set; }

    //public int CorrespondenceSubStatus { get; set; }
    public CorrespondenceSubStatus CorrespondenceSubStatus
    {
      get
      {
        return (CorrespondenceSubStatus)CorrespondenceSubStatusId;
      }
      set
      {
        CorrespondenceSubStatusId = Convert.ToInt32(value);
      }
    }

    public int CorrespondenceSubStatusId { get; set; }

    public bool AuthorityToBill { get; set; }

    public int NumberOfDaysToExpire { get; set; }

    public string CorrespondenceDetails { get; set; }

    public int? CurrencyId { get; set; }

    public Currency Currency { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime CorrespondenceSentOnDate { get; set; }

    public DateTime? ExpiryDatePeriod { get; set; }

    public DateTime? BMExpiryPeriod { get; set; }

    public string RejectionMemoIds { get; set; }

    public string AcceptanceComment { get; set; }

    public string AcceptanceUserName { get; set; }
    
    public DateTime AcceptanceDateTime { get; set; }

  }
}
