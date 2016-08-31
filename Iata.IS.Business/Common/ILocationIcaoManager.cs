using System.Collections.Generic;
using Iata.IS.Model.Common;
namespace Iata.IS.Business.Common
{
    public interface ILocationIcaoManager
    {
        /// <summary>
        /// Adds the location icao.
        /// </summary>
        /// <param name="locationIcao">The location icao.</param>
        /// <returns></returns>
        LocationIcao AddLocationIcao(LocationIcao locationIcao);

        /// <summary>
        /// Updates the location icao.
        /// </summary>
        /// <param name="locationIcao">The location icao.</param>
        /// <returns></returns>
        LocationIcao UpdateLocationIcao(LocationIcao locationIcao);

        /// <summary>
        /// Deletes the location icao.
        /// </summary>
        /// <param name="locationIcaoId">The location icao id.</param>
        /// <returns></returns>
        bool DeleteLocationIcao(string locationIcaoId);

        /// <summary>
        /// Gets the location icao details.
        /// </summary>
        /// <param name="locationIcaoId">The location icao id.</param>
        /// <returns></returns>
        LocationIcao GetLocationIcaoDetails(string locationIcaoId);

        /// <summary>
        /// Gets all location icao list.
        /// </summary>
        /// <returns></returns>
        List<LocationIcao> GetAllLocationIcaoList();

        /// <summary>
        /// Gets the location icao list.
        /// </summary>
        /// <param name="locationIcaoCode">The location icao code.</param>
        /// <param name="description">The description.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns></returns>
        List<LocationIcao> GetLocationIcaoList(string locationIcaoCode, string countryCode, string description);

    }
}
