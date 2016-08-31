using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
  public interface IAirportManager
  {
    /// <summary>
    /// Adds the airport.
    /// </summary>
    /// <param name="airport">The airport.</param>
    /// <returns></returns>
    Airport AddAirport(Airport airport);

    /// <summary>
    /// Updates the airport.
    /// </summary>
    /// <param name="airport">The airport.</param>
    /// <returns></returns>
    Airport UpdateAirport(Airport airport);

    /// <summary>
    /// Deletes the airport.
    /// </summary>
    /// <param name="airportId">The airport id.</param>
    /// <returns></returns>
    bool DeleteAirport(string airportId);

    /// <summary>
    /// Gets the airport details.
    /// </summary>
    /// <param name="airportId">The airport id.</param>
    /// <returns></returns>
    Airport GetAirportDetails(string airportId);

    /// <summary>
    /// Gets all airport list.
    /// </summary>
    /// <returns></returns>
    List<Airport> GetAllAirportList();

    /// <summary>
    /// Gets the airport list.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <param name="name">The name.</param>
    /// <param name="countryCode">The country code.</param>
    /// <returns></returns>
    List<Airport> GetAirportList(string id, string name, string countryCode);
  }
}
