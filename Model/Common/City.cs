using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents a city.
  /// </summary>
   [Serializable]
  public class City : EntityBase<int>, ICacheable
  {
    /// <summary>
    /// Name of the city.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Code of the city.
    /// </summary>
    public string Code { get; set; }
    
    /// <summary>
    /// Country id to which the city belongs.
    /// </summary>
    public string CountryId { get; set; }

    /// <summary>
    /// Navigation property for country.
    /// </summary>
    public Country Country { get; set; }
  }
}
