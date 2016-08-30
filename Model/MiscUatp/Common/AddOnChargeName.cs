using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using System;
namespace Iata.IS.Model.MiscUatp.Common
{
  public class AddOnChargeName : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryId { get; set; }

    public string BillingCategory
    {
        get
        {
            return ((BillingCategoryType)BillingCategoryId).ToString();
        }
    }
  }
}
