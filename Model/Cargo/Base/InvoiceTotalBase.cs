using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo.Base
{
  public abstract class InvoiceTotalBase : EntityBase<Guid>
  {
    public int NoOfBillingRecords { get; set; }

    public decimal TotalNetAmountWithoutVat { get; set; }

    public decimal NetBillingAmount { get; set; }

    public decimal TotalVatAmount { get; set; }

    public decimal NetTotal { get; set; }
  }
}
