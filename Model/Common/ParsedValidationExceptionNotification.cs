using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class ParsedValidationExceptionNotification
  {
    public string RecipientName { get; set; }
    public string ParsedFileName { get; set; }
    public string FileLocation { get; set; }
  }
}
