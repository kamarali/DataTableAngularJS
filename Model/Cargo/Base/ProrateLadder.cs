using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Cargo.Base
{
  public class ProrateLadder : EntityBase<Guid>
  {
    public string FromSector { get; set; }

    public string ToSector { get; set; }

    public string CarrierPrefix { get; set; }

    public string ProvisoReqSpa { get; set; }

    public string ProrateCalCurrencyId { get; set; }

    public Guid ParentId { get; set; }

    public double TotalAmount { get; set; }

    public int ProrateFactor { get; set; }

    public double PercentShare { get; set; }

    public double Amount { get; set; }
  }
}
