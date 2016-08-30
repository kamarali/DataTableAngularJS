using System;

namespace Iata.IS.Web.UIModel.BillingHistory.Cargo
{
  public class CargoAuditTrailInvoice
  {
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; }
    public string RedirectUrl { get; set; }
  }
}