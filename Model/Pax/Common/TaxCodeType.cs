using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common
{
  [Serializable]
  public class TaxCodeType : EntityBase<int>, ICacheable
  {
    public string CodeType { get; set; }
    public string Description { get; set; }

  }
}
