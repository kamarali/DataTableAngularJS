using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
  public enum ErrorStatus
  {
    /// <summary>
    /// Sanity check error
    /// </summary>
    Z = 1,

    /// <summary>
    /// Error Non-Correctable
    /// </summary>
    X = 2,

    /// <summary>
    /// Error Correctable
    /// </summary>
    C = 3,

    /// <summary>
    /// Warning
    /// </summary>
    W = 4,

    /// <summary>
    /// Validated Successfully
    /// </summary>
    V = 5
  };
}
