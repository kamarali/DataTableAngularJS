using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IAircraftTypeIcaoManager
    {
        /// <summary>
        /// Adds the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraft type icao.</param>
        /// <returns></returns>
        AircraftTypeIcao AddAircraftTypeIcao(AircraftTypeIcao aircraftTypeIcao);

        /// <summary>
        /// Updates the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraft type icao.</param>
        /// <returns></returns>
        AircraftTypeIcao UpdateAircraftTypeIcao(AircraftTypeIcao aircraftTypeIcao);

        /// <summary>
        /// Deletes the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcaoId">The aircraft type icao id.</param>
        /// <returns></returns>
        bool DeleteAircraftTypeIcao(string aircraftTypeIcaoId);

        /// <summary>
        /// Gets the aircraft type icao details.
        /// </summary>
        /// <param name="aircraftTypeIcaoId">The aircraft type icao id.</param>
        /// <returns></returns>
        AircraftTypeIcao GetAircraftTypeIcaoDetails(string aircraftTypeIcaoId);

        /// <summary>
        /// Gets all aircraft type icao list.
        /// </summary>
        /// <returns></returns>
        List<AircraftTypeIcao> GetAllAircraftTypeIcaoList();

        /// <summary>
        /// Gets the aircraft type icao list.
        /// </summary>
        /// <param name="aircraftTypeIcaoCode">The aircraft type icao code.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        List<AircraftTypeIcao> GetAircraftTypeIcaoList(string aircraftTypeIcaoCode, string description);

    }
}
