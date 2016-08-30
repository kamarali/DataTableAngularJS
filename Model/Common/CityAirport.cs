using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class CityAirport : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the main city.
    /// </summary>
    /// <value>The main city.</value>
    public string MainCity { get; set; }

    /// <summary>
    /// Gets or sets the city code numeric.
    /// </summary>
    /// <value>The city code numeric.</value>
    public int CityCodeNumeric { get; set; }

    /// <summary>
    /// Gets or sets the Country id from mst_Country.
    /// </summary>
    /// <value>The Country id.</value>
    public string CountryId { get; set; }
    
    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    /// <value>The country.</value>
    public Country Country { get; set; }

    public string CityAirportCodeDisplayName
    {
        get { return string.IsNullOrEmpty(Id) ? Id : Id.Replace("_1", string.Empty); }
    }

  }
}