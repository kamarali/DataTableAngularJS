using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class InvoicePendingForDailyDelivery
  {
    public string InvoiceNumber { get; set; }

    public string BillingMember { get; set; }

    public string BilledMember { get; set; }

    public string BillingPeriod { get; set; }
  }
}
