using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Util
{
  public class SourceCodeComparer : IEqualityComparer<SourceCode>
  {
    public bool Equals(SourceCode x, SourceCode y)
    {
      if (x.SourceCodeIdentifier == y.SourceCodeIdentifier && x.SourceCodeDescription == y.SourceCodeDescription)
        return true;
      return false;
    }

    public int GetHashCode(SourceCode a)
    {
      string z = a.SourceCodeIdentifier + a.SourceCodeDescription;
      return (z.GetHashCode());
    }
  }
}
