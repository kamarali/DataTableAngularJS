using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
namespace Iata.IS.Model.Common
{
  public class Tolerance : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the billing category id.
    /// </summary>
    /// <value>The billing category id.</value>
    public int BillingCategoryId { get; set; }

    /// <summary>
    /// Gets or sets the rounding tolerance.
    /// </summary>
    /// <value>The rounding tolerance.</value>
    public double RoundingTolerance { get; set; }

    /// <summary>
    /// Gets or sets the clearing house.
    /// </summary>
    /// <value>The clearing house.</value>
    public string ClearingHouse { get; set; }

    /// <summary>
    /// Gets or sets the summation tolerance.
    /// </summary>
    /// <value>The summation tolerance.</value>
    public double SummationTolerance { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }

    public string BillingCategory
    {
        get
        {
            return ((BillingCategoryType)BillingCategoryId).ToString();
        }
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 02-12-2011
    /// Purpose: Gets or sets the effective from period.
    /// </summary>
    /// <value>The effective from period.</value>
    public DateTime EffectiveFromPeriod { get; set; }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 02-12-2011
    /// Purpose: Gets or sets the effective to period.
    /// </summary>
    /// <value>The effective to period.</value>
    public DateTime EffectiveToPeriod { get; set; }
  }
}
