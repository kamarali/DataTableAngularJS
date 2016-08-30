using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsOutputProcessLog : EntityBase<Guid>
  {
    public int OfflineCollectionStatus { get; set; }

    public DateTime OfflineCollectionDate { get; set; }

    public int OfflineCollectionRetryCount { get; set; }
  }
}
