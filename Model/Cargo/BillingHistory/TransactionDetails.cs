using System.Collections.Generic;

namespace Iata.IS.Model.Cargo.BillingHistory
{
  public class TransactionDetails
  {
    public List<CargoTransaction> Transactions { get; set; }
    public bool IsTransactionOutsideTimeLimit { get; set; }
  }

  public class CargoTransaction
  {
    /// <summary>
    /// Memo number in which this transaction was rejected.
    /// </summary>
    public string MemoNumber { get; set; }
    public string InvoiceNumber { get; set; }
  }
}