using System.Collections.Generic;

namespace Iata.IS.Model.Pax.BillingHistory
{
  public class TransactionDetails
  {
    public List<Transaction> Transactions { get; set; }
    public bool IsTransactionOutsideTimeLimit { get; set; }
  }

  public class Transaction
  {
    /// <summary>
    /// Memo number in which this transaction was rejected.
    /// </summary>
    public string MemoNumber { get; set; }
    public string InvoiceNumber { get; set; }
  }
}