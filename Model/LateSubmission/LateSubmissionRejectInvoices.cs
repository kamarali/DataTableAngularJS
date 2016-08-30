using System;

namespace Iata.IS.Model.LateSubmission
{
  public class LateSubmissionRejectInvoices
  {
   
    public string InvoiceNo { get; set; }

    public string Category { get; set; }

    public string BilledAirline { get; set; }

    public string Currency { get; set; }

    public int BillingMemberId { get; set; }
  
    public string BillingPeriod { get; set; }

    public int CategoryId { get; set; }

    public int LateSubRequestStatus { get; set; }
    
  }
}
