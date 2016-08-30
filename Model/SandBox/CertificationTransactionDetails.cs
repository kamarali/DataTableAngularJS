using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SandBox
{
  public class CertificationTransactionDetails : EntityBase<int>
  {

    public int TransactionHeaderId { get; set; }

    public int TransactionGroupId { get; set; }

    public int CertificationParameterId { get; set; }

    public int MinimumTransactionCount { get; set; }

    public int ReceivedTransactionCount { get; set; }

    public string GroupStatus { get; set; }

    public string SubType1Description { get; set; }

    public int SubType1MinimumCount { get; set; }

    public int SubType1ReceivedCount { get; set; }

    public string SubType1Status { get; set; }

    public string SubType2Description { get; set; }

    public int SubType2MinimumCount { get; set; }

    public int SubType2ReceivedCount { get; set; }

    public string SubType2Status { get; set; }
    public string SubType3Description { get; set; }

    public int SubType3MinimumCount { get; set; }

    public int SubType3ReceivedCount { get; set; }

    public string SubType3Status { get; set; }
    public string SubType4Description { get; set; }

    public int SubType4MinimumCount { get; set; }

    public int SubType4ReceivedCount { get; set; }

    public string SubType4Status { get; set; }

    public string SubType5Description { get; set; }

    public int SubType5MinimumCount { get; set; }

    public int SubType5ReceivedCount { get; set; }

    public string SubType5Status { get; set; }

    public string SubType6Description { get; set; }

    public int SubType6MinimumCount { get; set; }

    public int SubType6ReceivedCount { get; set; }

    public string SubType6Status { get; set; }

    public string SubType7Description { get; set; }

    public int SubType7MinimumCount { get; set; }

    public int SubType7ReceivedCount { get; set; }

    public string SubType7Status { get; set; }

    public CertificationTransactionGroup TransactionGroup { get; set; }

    public CertificationTransactionHeader TransactionHeader { get; set; }
  }
}
