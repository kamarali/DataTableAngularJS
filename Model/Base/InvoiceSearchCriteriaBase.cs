using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Base
{
  [Serializable]
  public class InvoiceSearchCriteriaBase
  {
    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public BillingType BillingType { get; set; }

    public int BillingTypeId { get; set; }

    public string BilledMemberCode { get; set; }

    public int BilledMemberId { get; set; }

    public int BillingMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public TransactionStatus TransactionStatus { get; set; }

    public int TransactionStatusId { get; set; }

    public string OnBehalfOfMember { get; set; }

    public Member Member { get; set; }

    public RejectionStage? RejectionStage { get; set; }

    public int? RejectionStageId { get; set; }
  }
}
