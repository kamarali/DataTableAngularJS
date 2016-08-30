using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Model.Pax.BillingHistory
{
  public class PaxAuditTrail:EntityBase<Guid>
  {
    public List<PaxInvoice> Invoices { get;  set; }
    public List<SamplingFormC> SamplingFormC { get;  set; }
  }
}