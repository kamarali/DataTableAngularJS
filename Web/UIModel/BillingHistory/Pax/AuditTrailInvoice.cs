using System;

namespace Iata.IS.Web.UIModel.BillingHistory.Pax
{
  public class AuditTrailInvoice
  {
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public string RedirectUrl { get; set; }
  }
}