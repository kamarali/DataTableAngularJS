using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class AircraftType : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the icao code.
    /// </summary>
    /// <value>The icao code.</value>
    public string IcaoCode { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }
  }
}
