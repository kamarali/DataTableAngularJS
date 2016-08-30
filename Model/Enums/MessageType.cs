using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Enums
{
  public enum MessageType
  {
    Announcement = 1,
    Message = 2,
    Alert = 3
  }

  public enum RAGIndicator
  {
    Red = 1,
    Amber = 2,
    Green = 3
  }
}
