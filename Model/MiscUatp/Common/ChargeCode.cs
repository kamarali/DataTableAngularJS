using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class ChargeCode : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the charge code types.
    /// </summary>
    /// <value>The charge code types.</value>
    public List<ChargeCodeType> ChargeCodeTypes { get; set; }

    /// <summary>
    /// Gets or sets the charge category id.
    /// </summary>
    /// <value>The charge category id.</value>
    public int ChargeCategoryId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether location code required for invoice.
    /// </summary>
    /// <value>true if location required for invoice; otherwise, false.</value>
    public bool IsLocationRequiredForInvoice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether location code required for line item.
    /// </summary>
    /// <value>true if [location required for line item]; otherwise, false.</value>
    public bool IsLocationRequiredForLineItem { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Charge Code type required for line item.
    /// </summary>
    /// <value>true if [Charge Code type required for line item]; otherwise, false.</value>
    public bool? IsChargeCodeTypeRequired { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether Charge Code type required for line item.
    /// </summary>
    /// <value>true if [Charge Code type required for line item]; otherwise, false.</value>
    public bool IsActiveChargeCodeType { get; set; }

    public ChargeCode()
    {
        ChargeCodeTypes = new List<ChargeCodeType>();
    }
  }
}