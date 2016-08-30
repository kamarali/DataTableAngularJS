using System;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class CorrespondenceTrailSearchCriteria
  {
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int CorrBillingMemberId { get; set; }

    public int CorrBilledMemberId { get; set; }

    public string CorrBilledMemberText { get; set; }

    //public long? CorrespondenceNumber { get; set; }

    public int CorrespondenceStatusId { get; set; }

    public int CorrespondenceSubStatusId { get; set; }

    //public int? CorrespondenceOwnerId { get; set; }

    public int? InitiatingMember { get; set; }
   
  }
}