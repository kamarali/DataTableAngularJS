using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class MinAcceptableAmount : MasterBase<int>
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

    /// <summary>
    /// Gets or sets Effective from date
    /// </summary>
    public DateTime EffectiveFromPeriod { get; set; }
      
    /// <summary>
    /// Gets or sets Effective to date
    /// </summary>
    public DateTime EffectiveToPeriod { get; set; }

    /// <summary>
    /// Gets or sets Rejection reason code
    /// </summary>
    public string RejectionReasonCode { get; set; }

    /// <summary>
    /// Gets or sets Rejection reason code Id
    /// </summary>
    public int RejectionReasonCodeId { get; set; }

    /// <summary>
    ///  Gets or sets ApplicableMinimumField
    /// </summary>
    public ApplicableMinimumField ApplicableMinimumField
    {
      get { return (ApplicableMinimumField) ApplicableMinimumFieldId; }
      set { ApplicableMinimumFieldId = (int)value; }
    }

    /// <summary>
    /// Gets teh Apllicable minimum field
    /// </summary>
    public string ApplicableMinimumFields
    {
        get { return (ApplicableMinimumField.ToString()); }
    }
    /// <summary>
    ///  Gets or sets the Applicable Minimum Field Id.
    /// </summary>
    public int ApplicableMinimumFieldId
    { get; set; }

    public string TransactionTypeName
    {
        get { return (TransactionType.Name); }
    }
      
  }
}
