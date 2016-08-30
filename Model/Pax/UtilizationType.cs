using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax
{
  [Serializable]
  public class UtilizationType : EntityBase<int>, ICacheable
  {
    public string Code { get; set; }

    public string Name { get; set; }
  }
}
