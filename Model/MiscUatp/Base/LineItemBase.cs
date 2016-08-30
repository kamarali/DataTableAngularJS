using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Model.MiscUatp.Base
{
  public abstract class LineItemBase : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the line item number.
    /// </summary>
    /// <value>The line item number.</value>
    public int LineItemNumber { get; set; }

    /// <summary>
    /// Gets or sets the product id.
    /// </summary>
    /// <value>The product id.</value>
    public string ProductId { get; set; }

    /// <summary>
    /// Gets or sets the start date.
    /// </summary>
    /// <value>The start date.</value>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Gets or sets the end date.
    /// </summary>
    /// <value>The end date.</value>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [minimum quantity flag].
    /// </summary>
    /// <value>true if [minimum quantity flag]; otherwise, false.</value>
    public bool MinimumQuantityFlag { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    /// <value>The quantity.</value>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Gets or sets the UOM code id.
    /// </summary>
    /// <value>The UOM code id.</value>
    public string UomCodeId { get; set; }

    /// <summary>
    /// Navigation property for UomCode.
    /// </summary>
    /// <value>The UOM code.</value>
    public UomCode UomCode { get; set; }

    public string UomCodeNameDisplayText
    {
      get
      {
        return UomCode != null ? UomCode.Description : string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the unit price.
    /// </summary>
    /// <value>The unit price.</value>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the scaling factor.
    /// </summary>
    /// <value>The scaling factor.</value>
    public int? ScalingFactor { get; set; }

    /// <summary>
    /// Gets or sets the total net amount.
    /// </summary>
    /// <value>The total net amount.</value>
    public decimal TotalNetAmount { get; set; }

    /// <summary>
    /// Gets or sets the total tax amount.
    /// </summary>
    /// <value>The total tax amount.</value>
    public decimal? TotalTaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the total VAT amount.
    /// </summary>
    /// <value>The total VAT amount.</value>
    public decimal? TotalVatAmount { get; set; }

    /// <summary>
    /// Gets or sets the total add on charge amount.
    /// </summary>
    /// <value>The total add on charge amount.</value>
    public decimal? TotalAddOnChargeAmount { get; set; }

    /// <summary>
    /// This will be derived as Quantity * Unit Price / Scaling Factor.
    /// </summary>
    /// <value>The total.</value>
    ///<remarks>
    /// This wont be used any more...
    /// </remarks>
    public decimal Total
    {
      get
      {
        // if Minimum Quantity Flag set to true then return total as Total Net Amount instead of calculated amount.
        //if (MinimumQuantityFlag)
        //{
          return ChargeAmount;
        //}
        //if (ScalingFactor != null && ScalingFactor != 0)
        //  return Quantity * UnitPrice / ScalingFactor.Value;
        //return Quantity * UnitPrice;

      }
    }

    /// <summary>
    /// Gets or sets the charge amount.
    /// </summary>
    /// <value>The charge amount.</value>
    public decimal ChargeAmount { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    protected LineItemBase()
    {
      ScalingFactor = 1;
    }
  }
}
