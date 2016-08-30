using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SandBox
{
  public class CertificationTransactionGroup : EntityBase<int>
  {
    public string TransactionGroupName { get; set; }

    public string TransactionGroupDescription { get; set; }
  }
}
