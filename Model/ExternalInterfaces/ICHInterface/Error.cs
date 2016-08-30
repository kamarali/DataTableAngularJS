using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.ExternalInterfaces.ICHInterface
{
  public class Error
  {
    //CMP #624: ICH Rewrite-New SMI X

    public string ErrorCode { get; set; }

    public string ErrorDescription { get; set; }
  }
}
