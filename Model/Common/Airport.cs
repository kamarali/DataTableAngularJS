using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents an airport in the system.
  /// </summary>
  
  [Serializable]
  public class Airport : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the country code.
    /// </summary>
    /// <value>The country code.</value>
    public string CountryCode { get; set; }
    
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>The country.</value>
    public Country Country { get; set; }
  }
}