using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents airlines in the system. E.g. Issuing airline.
  /// </summary>
  public class Airline : EntityBase<int>
  {
    /// <summary>
    /// Numeric code of the airline.
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// Name of the airline.
    /// </summary>
    public string Name { get; set; }
  }
}
