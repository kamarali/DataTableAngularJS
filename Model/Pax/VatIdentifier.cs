using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
namespace Iata.IS.Model.Pax
{
  [Serializable]
  public class VatIdentifier : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public string Identifier { get; set; }

    /// <summary>
    /// Gets or sets the identifier code.
    /// </summary>
    /// <value>The identifier code.</value>
    public string IdentifierCode { get; set; }

    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryCode { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    public string BillingCategory
    {
        get
        {
            return ((BillingCategoryType)BillingCategoryCode).ToString();
        }
    }
  }
}
