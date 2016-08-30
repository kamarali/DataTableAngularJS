using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Model.BroadcastMessages
{
  public class ISMessageRecipients : EntityBase<Guid>
  {
    public int? MemberId { get; set;}

    public string MemberCategory { get; set; }

    public bool AllSuperUsers { get; set; }

    public bool AllUsers { get; set; }

    public string ContactTypeId { get; set; }

    public ISMessagesAlerts IsMessagesAlerts { get; set; }

    public Guid MessageId { get; set; }

    public Member Member { get; set; }
  }
  
}
