using System;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingRMLinkingCriteria
  {
  
    public string ReasonCode { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public Guid RejectingInvoiceId { get; set; }

    public int ProvBillingMonth { get; set; }

    public int ProvBillingYear { get; set; }

    public string RejectionMemoNumber { get; set; }

    public bool IgnoreValidationOnMigrationPeriod { get; set; }
  
  }
}
