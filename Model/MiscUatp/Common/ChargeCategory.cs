using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class ChargeCategory : MasterBase<int>, ICacheable
  {
    /// <summary>
    /// Gets or sets the charge codes.
    /// </summary>
    /// <value>The charge codes.</value>
    public List<ChargeCode> ChargeCodes { get; set; }

    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    public ChargeCategory()
    {
        ChargeCodes = new List<ChargeCode>();
    }
  }
}