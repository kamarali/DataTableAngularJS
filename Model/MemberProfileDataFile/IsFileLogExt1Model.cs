using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfileDataFile
{
  public class IsFileLogExt1Model : EntityBase<Guid>
  {
      public Guid IsFileLogId { get; set; }
      public string MemberIdLIst { get; set; }
      public string AccountIdLIst { get; set; }
      public string ContactIdLIst { get; set; }
      public string IinetUploadStatus { get; set; }
     	
  }
}
