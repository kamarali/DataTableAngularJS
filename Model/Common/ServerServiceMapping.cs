using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class ServerServiceMapping : MasterBase<int>
  {
    public string ServerName { get; set; }
    public string ServiceName { get; set; }
    public string ServiceLocation { get; set; }
  
  }
}
