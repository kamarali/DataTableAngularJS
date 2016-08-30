using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class MaxAcceptableAmount : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the transaction type id.
    /// </summary>
    /// <value>The transaction type id.</value>
    public int TransactionTypeId { get; set; }

    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    /// <value>The max amount.</value>
    public double Amount { get; set; }

    /// <summary>
    /// Gets or sets the clearing house.
    /// </summary>
    /// <value>The clearing house.</value>
    public string ClearingHouse { get; set; }

    /// <summary>
    /// Gets or sets the type of the transaction.
    /// </summary>
    /// <value>The type of the transaction.</value>
    public TransactionType TransactionType { get; set; }

    public string TransactionTypeName
    {
        get { return TransactionType.Name; }
    }

    /// <summary>
    /// Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective from period.</value>
    public DateTime EffectiveFromPeriod { get; set; }

    /// <summary>
    /// Gets or sets the effective to period.
    /// </summary>
    /// <value>The effective to period.</value>
    public DateTime EffectiveToPeriod { get; set; }
   
  }
}