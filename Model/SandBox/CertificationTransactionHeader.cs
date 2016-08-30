using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Model.SandBox
{
  public class CertificationTransactionHeader : EntityBase<int>
  {
    public Guid FileId { get; set; }

    public string RequestType { get; set; }

    public string FileStatus { get; set; }

    public IsInputFile IsInputFiles { get; set; } 

  }
}
