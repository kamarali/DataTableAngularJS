using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsEmail : EntityBase<int>
  {
    public string FromEmailAddress { get; set; }

    public string ToEmailAddress { get; set; }

    public string BccEmailAddress { get; set; }

    public string CcEmailAddress { get; set;}

    public string Subject { get; set; }

    public string Body { get; set; }

    public bool IsBodyHtml { get; set; }

    public int RetryCount { get; set; }

    public DateTime LastAttemptOn { get; set; }

    public string Status { get; set; }

    public List<IsEmailAttachment> Attachments { get; set; }

  }
}
