using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;
//using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.SandBox
{
  public class SandboxTransactionModel
  {
    public TransactionType TransactionType { get; set; }

    public int MinimumTransactionCount { get; set; }

    public int ReceivedTransactionCount { get; set; }

    public bool TransactionStatus { get; set; }

    public string SubType1Description { get; set; }

    public int SubType1MinimumCount { get; set; }

    public int SubType1ReceivedCount { get; set; }

    public bool SubType1Status { get; set; }

    public string SubType2Description { get; set; }

    public int SubType2MinimumCount { get; set; }

    public int SubType2ReceivedCount { get; set; }

    public bool SubType2Status { get; set; }
  }
}
