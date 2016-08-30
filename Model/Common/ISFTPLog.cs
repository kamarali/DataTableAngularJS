using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsftpLog : EntityBase<int>
  {
    public Guid IsfileLogId { get; set; }
    public string LogText { get; set; }
    public string TdfFileContent { get; set; }
    
  }
}
