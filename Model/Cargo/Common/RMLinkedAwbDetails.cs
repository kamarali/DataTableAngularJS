using System;
using System.Collections.Generic;

namespace Iata.IS.Model.Cargo.Common
{
  public class RMLinkedAwbDetails
  {

    public List<RMLinkedAwb> RMLinkedAwbRecords { get; set; }
    public RMAwb Details { get; set; }
    public string ErrorMessage { get; set; }
    public RMLinkedAwbDetails()
    {
      RMLinkedAwbRecords = new List<RMLinkedAwb>();
    }
  }
  public class RMLinkedAwb
  {
    public int BatchNumber { get; set; }
    public int RecordSeqNumber { get; set; }
    public int BreakdownSerialNo { get; set; }
    public Guid RMAwbId { get; set; }
  }
}
