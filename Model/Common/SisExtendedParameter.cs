using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class SisExtendedParameter: EntityBase<Guid>
  {
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }
  }
}
