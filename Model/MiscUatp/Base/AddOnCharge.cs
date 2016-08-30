using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Base
{
  public abstract class AddOnCharge : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    public string Code { get; set; }

    /// <summary>
    /// Gets or sets the percentage.
    /// </summary>
    /// <value>The percentage.</value>
    public double? Percentage { get; set; }

    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    /// <value>The amount.</value>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the chargeable amount.
    /// </summary>
    /// <value>The chargeable amount.</value>
    public decimal? ChargeableAmount { get; set; }

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid ParentId { get; set; }
  }
}