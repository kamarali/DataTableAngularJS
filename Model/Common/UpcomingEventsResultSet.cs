using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class UpcomingEventsResultSet : MasterBase<int>
  {
    public string Period { get; set; }

    public string EventDescription { get; set; }

    public DateTime YmqDateTime { get; set; }

    public DateTime LocalDateTime { get; set; }

  }
}
