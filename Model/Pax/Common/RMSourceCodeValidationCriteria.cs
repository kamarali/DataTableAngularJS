using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Pax.Common
{
  public class RMSourceCodeValidationCriteria
  {
    public int SourceCode { get; set; }

    public int BillingMemberId { get; set; }

    public int BilledMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public int BillingYear { get; set; }

    public int BillingMonth { get; set; }

    public int BillingPeriod { get; set; }

    public string RejectionMemoNumber { get; set; }

    public string FimBMCMNumber { get; set; }

    public int? FimCouponNumber { get; set; }

    public int RejectionStage { get; set; }

    public int IgnoreValidationOnRMSourceCodes { get; set; } 

    public RMSourceCodeValidationCriteria()
    {
    }
  }
}
