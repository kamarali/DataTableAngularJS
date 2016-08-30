using Iata.IS.Model.Base;
using Iata.IS.Model.Pax;
using System;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class TimeLimit : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the limit.
    /// </summary>
    /// <value>The limit.</value>
    public int Limit { get; set; }

   

    /// <summary>
    /// Gets or sets the transaction type id.
    /// </summary>
    /// <value>The transaction type id.</value>
    public int TransactionTypeId { get; set; }
    /// <summary>
    /// Gets or sets Transaction Type
    /// </summary>
    /// <value>The transaction type.</value>  
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Gets the name of the transaction type.
    /// </summary>
    /// <value>
    /// The name of the transaction type.
    /// </value>
    public string TransactionTypeName
    {
        get { return TransactionType!=null?this.TransactionType.Name:string.Empty; }
    }

    /// <summary>
    /// Gets or sets the calculation method.
    /// </summary>
    /// <value>The calculation method.</value>
    public string CalculationMethod { get; set; }

    public int SettlementMethodId { get; set; }
    /// <summary>
    /// Gets or sets Transaction Type
    /// </summary>
    /// <value>The transaction type.</value>  
    public SettlementMethod SettlementMethod { get; set; }

    /// <summary>
    /// Gets or sets the clearing house.
    /// </summary>
    /// <value>The clearing house.</value>
    public string ClearingHouse
    {
        get { return SettlementMethod != null ? this.SettlementMethod.Name : string.Empty; }
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 02-12-2011
    /// Purpose: Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective from period.</value>
    public DateTime EffectiveFromPeriod  { get; set; }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 02-12-2011
    /// Purpose: Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective to period.</value>
    public DateTime EffectiveToPeriod { get; set; }
  }
}
