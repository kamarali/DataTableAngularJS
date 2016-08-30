using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.MiscUatp
{
  public class MiscDerivedVatDetails
  {
    public int SerialNo { get; set; }

    /// <summary>
    /// Gets or sets the sub type of the tax.
    /// </summary>
    /// <value>The type of the tax sub.</value>
    public string SubType { get; set; }

    /// <summary>
    /// Gets or sets the tax percentage.
    /// </summary>
    /// <value>The percentage.</value>
    public double Percentage { get; set; }

    /// <summary>
    /// Gets or sets the tax calculated amount.
    /// </summary>
    /// <value>The calculated amount.</value>
    public decimal CalculatedAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax category code.
    /// </summary>
    /// <value>The category code.</value>
    public string CategoryCode { get; set; }
        
    /// <summary>
    /// Gets or sets the tax description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the Base Amount.
    /// </summary>
    public decimal Amount { get; set; }
  }
}
