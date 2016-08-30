

using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class DefaultBatchNumber : MasterBase<int>
  {
    public int BillingCategoryId { get; set; }

    public int TransactionTypeId { get; set; }

    public int DefaultBatchSequenceNumber { get; set; }
  }
}
