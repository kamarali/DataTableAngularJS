using System;
using Iata.IS.Model.Cargo.Enums;

namespace Iata.IS.Model.Cargo.Common
{
  public class CgoRMLinkingCriteria
  {
    public string ReasonCode { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public string RejectionMemoNumber { get; set; }

    public int BMCMIndicatorId { get; set; }

    public int RejectionStage { get; set; }

    public Guid MemoId { get; set; }

    public Guid RejectedInvoiceId { get; set; }

    public string YourBillingMemoNumber { get; set; }

    public BMCMIndicator BMCMIndicator
    {
      get
      {
        return (BMCMIndicator)BMCMIndicatorId;
      }
    }

    public bool IgnoreValidationOnMigrationPeriod { get; set; }

      // CMP #650
    public string TransactionType { get; set; }
  }
}
