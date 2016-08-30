using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;
//using Iata.IS.Model.Pax.Enums;


namespace Iata.IS.Model.SandBox
{
  public class SandboxTransactionGroupModel
  {
    public string TransactionGroupDescription { get; set; } 

    public TransactionGroup TransactionGroup { get; set; }

    public TransactionType TransactionType { get; set; }

    public int MinimumTransactionCount { get; set; }

    public string MinimumTransactionDescription { get; set; }

    public int ReceivedTransactionCount { get; set; }

    public string ReceivedTransactionDescription { get; set; }

    public bool TransactionStatus { get; set; }

    public string SubType1ReceivedDescription { get; set; }

    public string SubType1MinimumDescription { get; set; }

    public int SubType1MinimumCount { get; set; }

    public int SubType1ReceivedCount { get; set; }

    public bool SubType1Status { get; set; }

    public string SubType2ReceivedDescription { get; set; }

    public string SubType2MinimumDescription { get; set; }

    public int SubType2MinimumCount { get; set; }

    public int SubType2ReceivedCount { get; set; }

    public bool SubType2Status { get; set; }

    public List<SandboxTransactionModel> SandboxTransactionModelList { get; set; }
    
  }
}
