using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
  public enum MemberConfigParameter
  {
    PaxRejectionOnValidationFailure = 1,
    MiscRejectionOnValidationFailure = 2,
    PaxValidFileExtensions = 3,
    MiscValidFileExtensions = 4,
    CargoRejectionOnValidationFailure = 5,
    CargoValidFileExtensions = 6,
  }
}
