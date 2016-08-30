using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
 public class SystemParameter :  EntityBase<int>
  {
   public string Version { get; set; }
   public string ConfigXml { get; set; }
   public DateTime UpdatedOn { get; set; }
   // SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
   public int UserId { get; set; }
   public int ProxyUserId { get; set; }
  }
}
