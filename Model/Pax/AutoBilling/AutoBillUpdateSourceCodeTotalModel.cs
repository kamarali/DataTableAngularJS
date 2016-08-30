using System;
using Iata.IS.Core;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class AutoBillUpdateSourceCodeTotalModel : EntityBase<Guid>
  {
    public Guid InvoiceId { get; set; }

    public string InvoiceDisplayId
    {
      get
      {
        return InvoiceId.Value();
      }
    }

    public string ResponseFileName { get; set; }
  }
}
