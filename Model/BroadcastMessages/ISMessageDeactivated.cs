using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.BroadcastMessages
{
  public class ISMessageDeactivated
  {
    public Guid MessageId { get; set; }

    public int UserId { get; set; }
  }
}
