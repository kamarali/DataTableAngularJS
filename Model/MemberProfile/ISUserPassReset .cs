using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MemberProfile
{
  public class ISUserPassReset
  {
    public int IsUserId { get; set; }

    public Guid LinkId { get; set; }

    public int Used { get; set; }

    public DateTime CreatedOn { get; set; }
  }
}
