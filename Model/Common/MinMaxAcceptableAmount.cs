using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class MinMaxAcceptableAmount : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the transaction type id.
    /// </summary>
    /// <value>The transaction type id.</value>
    public int TransactionTypeId { get; set; }

    /// <summary>
    /// Gets or sets the min.
    /// </summary>
    /// <value>The min.</value>
    public double Min { get; set; }

    /// <summary>
    /// Gets or sets the max.
    /// </summary>
    /// <value>The max.</value>
    public double Max { get; set; }

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
        get { return this.TransactionType.Name; }
    }

  }
}