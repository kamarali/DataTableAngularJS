using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsEmailAttachment : EntityBase<int>
  {
    public int IsEmailId { get; set; }

    public string AttachmentFilePath { get; set; }

    public IsEmail Email { get; set; }

  }
}
