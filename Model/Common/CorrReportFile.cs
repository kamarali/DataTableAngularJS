using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class CorrReportFile : EntityBase<Guid>
  {
    public string FilePath { get; set; }
    public DateTime FileDate { get; set; }
    public bool IsPurged { get; set; }
   

  }
}
