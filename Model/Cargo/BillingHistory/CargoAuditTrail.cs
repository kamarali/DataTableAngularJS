using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo.BillingHistory
{
  public class CargoAuditTrail:EntityBase<Guid>
  {
    public List<CargoInvoice> Invoices { get;  set; }
  }
}