using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class CorrespondenceTrailSearchResult
  {
    private string _transactionNumber;
    public Guid TransactionId { get; set; }
    public string TransactionType { get; set; }
    public int BillingMemberId { get; set; }
    public string MemberCodeAlpha { get; set; }
    public string MemberCodeNumeric { get; set; }

    public int CorrInitiatingMember { get; set; }
   

    public string MemberCode
    {
      get
      {
        return string.Format("{0}-{1}", MemberCodeAlpha, MemberCodeNumeric);
      }
    }

    public string TransactionNumber
    {
      get
      {
        return string.Format("{0:00000000000}", Convert.ToInt64(_transactionNumber));
      }
      set
      {
        _transactionNumber = value;
      }
    }
   
    
    public string TransactionDate { get; set; }
    public string TotalNetAmount { get; set; }
    public long? CorrespondenceNumber { get; set; }
    public int CorrespondenceStatusId { get; set; }
    public CorrespondenceStatus CorrespondenceStatus { get; set; }
    public int CorrespondenceSubStatusId { get; set; }
    public CorrespondenceSubStatus CorrespondenceSubStatus { get; set; }
    public bool? AuthorityToBill { get; set; }
    public int? NoOfDaysToExpire { get; set; }

    public string DisplayCorrespondenceStatus
    {
      get
      {
        return EnumList.GetCorrespondenceStatusDisplayValue((CorrespondenceStatus)CorrespondenceStatusId);
      }
    }

    public string DisplayCorrespondenceSubStatus
    {
      get
      {
        return EnumList.GetCorrespondenceSubStatusDisplayValue((CorrespondenceSubStatus)CorrespondenceSubStatusId);
      }
    }
    
  }
}
