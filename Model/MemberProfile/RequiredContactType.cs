using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
  public  class RequiredContactType
    {
      public int ContactTypeId { get; set; }

      public string ContactTypeText { get; set; }

      public int ContactCount { get; set; }
    }
}
