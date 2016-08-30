using System;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class CorrespondenceSearchCriteria
  {
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public int CorrBillingMemberId { get; set; }

    public int CorrBilledMemberId { get; set; }

    public string CorrBilledMemberText { get; set; }

    public long? CorrespondenceNumber { get; set; }

    public int CorrespondenceStatusId { get; set; }

    public int CorrespondenceSubStatusId { get; set; }

    public bool AuthorityToBill { get; set; }

    public int? NoOfDaysToExpiry { get; set; }

    public int? CorrespondenceOwnerId { get; set; }

    public int? InitiatingMember { get; set; }

    //CMP526 - Passenger Correspondence Identifiable by Source Code
    public int? SourceCode { get; set; }
  }
}