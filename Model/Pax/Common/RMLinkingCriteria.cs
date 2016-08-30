using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Pax.Common
{
  public class RMLinkingCriteria
  {
    public string SourceCode { get; set; }

    public string ReasonCode { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public string RejectionMemoNumber { get; set; }

    public string FimBMCMNumber { get; set; }

    public int? FimCouponNumber { get; set; }

    public int FimBmCmIndicatorId { get; set; }

    public int RejectionStage { get; set; }

    public Guid MemoId { get; set; }

    public Guid RejectedInvoiceId { get; set; }

    public FIMBMCMIndicator FIMBMCMIndicator
    {
      get
      {
        return (FIMBMCMIndicator)FimBmCmIndicatorId;
      }
    }

    public bool IgnoreValidationOnMigrationPeriod { get; set; }

    public RMLinkingCriteria()
    {
    }

  }
}
