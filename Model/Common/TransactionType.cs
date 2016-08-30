using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
namespace Iata.IS.Model.Common
{
  [Serializable]
  public class TransactionType : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryCode { get; set; }
    public string BillingCategory
    {
        get
        {
            return ((BillingCategoryType)BillingCategoryCode).ToString();
        }
    }
  }
}