using System;
using Iata.IS.Model.Base;
using TransactionType = Iata.IS.Model.Common.TransactionType;

namespace Iata.IS.Model.Cargo.BillingHistory
{
  [Serializable]
  public class InvoiceSearchCriteria : InvoiceSearchCriteriaBase
  {
    public TransactionType MemoType { get; set; }
    public int MemoTypeId { get; set; }
    public string MemoNumber { get; set; }
    public int? SourceCodeId { get; set; }
    public string ReasonCodeId { get; set; }
    public string IssuingAirline { get; set; }
    public int? IssuingAirlineId { get; set; }
    public int AwbSerialNumber { get; set; }
    public long? DocumentNumber { get; set; }
  }
}