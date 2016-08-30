using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class LinkedRejectionMemo
  {
    public string Id { get; set; }

    public string InvoiceId { get; set; }

    public string BillingMemberText { get; set; }

    public string DisplayBillingPeriod { get; set; }

    public string InvoiceNumber { get; set; }

    public string RejectionMemoNumber { get; set; }

    public int BillingCode { get; set; }
  }
}
