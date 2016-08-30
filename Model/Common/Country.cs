using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents a country.
  /// </summary>

  [Serializable]
  public class Country : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the country code ICAO.
    /// </summary>
    /// <value>The country code ICAO.</value>
    public string CountryCodeIcao { get; set; }

    /// <summary>
    /// Gets or sets the Digital Signature format.
    /// </summary>
    /// <value>The Digital Signature format.</value>
    public string DsFormat { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether digital signature supported by ATOS.
    /// </summary>
    /// <value>true if digital signature supported by ATOS; otherwise, false.</value>
    public bool DsSupportedByAtos { get; set; }

  }
}
