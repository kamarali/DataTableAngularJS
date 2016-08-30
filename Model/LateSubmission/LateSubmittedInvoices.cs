using System;

namespace Iata.IS.Model.LateSubmission
{
   public class LateSubmittedInvoices
  {
     public Guid InvoiceId { get; set; }

     public string InvoiceNo { get; set; }

     public string Category { get; set; }

     public string BilledAirline { get; set; }

     public  decimal Amount { get; set; }

     public string Currency { get; set; }

     public string Status { get; set; }

     public int CategoryId { get; set; }

  }
}
