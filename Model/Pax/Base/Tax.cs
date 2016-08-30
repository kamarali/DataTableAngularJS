using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Base
{
  public abstract class Tax : EntityBase<Guid>
  {
    public string TaxCode { get; set; }

    public int TaxCodeId { get; set; }

    public double Amount { get; set; }

    public Guid ParentId { get; set; }
  }
}
