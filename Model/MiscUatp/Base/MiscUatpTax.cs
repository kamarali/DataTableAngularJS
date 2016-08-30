using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Model.MiscUatp.Base
{
  public class MiscUatpTax : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the type of the tax.
    /// </summary>
    /// <value>The type of the tax.</value>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the sub type of the tax.
    /// </summary>
    /// <value>The type of the tax sub.</value>
    public string SubType { get; set; }

    /// <summary>
    /// Gets or sets the registration id.
    /// </summary>
    /// <value>The registration id.</value>
    public string RegistrationId { get; set; }

    /// <summary>
    /// Gets or sets the tax percentage.
    /// </summary>
    /// <value>The percentage.</value>
    public double? Percentage { get; set; }

    /// <summary>
    /// Gets or sets the tax calculated amount.
    /// </summary>
    /// <value>The calculated amount.</value>
    public decimal? CalculatedAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax category code.
    /// </summary>
    /// <value>The category code.</value>
    public string CategoryCode { get; set; }

    /// <summary>
    /// Gets or sets the tax category id.
    /// </summary>
    /// <value>The category id.</value>
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the tax description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the country id.
    /// </summary>
    /// <value>The country id.</value>
    public string CountryId { get; set; }

    /// <summary>
    /// Navigation property for country id. 
    /// </summary>
    /// <value>The country.</value>
    public Country Country { get; set; }


    /// <summary>
    /// Gets or sets the country code ICAO.
    /// </summary>
    /// <value>The country code ICAO.</value>
    public string CountryCodeIcao { get; set; }

    /// <summary>
    /// Gets or sets the sub division code.
    /// </summary>
    /// <value>The sub division code.</value>
    public string SubdivisionCode { get; set; }

    /// <summary>
    /// Gets or sets the Base Amount.
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// Navigation property.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid ParentId { get; set; }
  }
}