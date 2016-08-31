using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ICityAirportManager
    {
        /// <summary>
        /// Adds the city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <returns></returns>
        CityAirport AddCityAirport(CityAirport cityAirport);

        /// <summary>
        /// Updates the city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <returns></returns>
        CityAirport UpdateCityAirport(CityAirport cityAirport);

        /// <summary>
        /// Deletes the city airport.
        /// </summary>
        /// <param name="CityAirportId">The city airport id.</param>
        /// <returns></returns>
        bool DeleteCityAirport(string CityAirportId);

        /// <summary>
        /// Gets the city airport details.
        /// </summary>
        /// <param name="CityAirportId">The city airport id.</param>
        /// <returns></returns>
        CityAirport GetCityAirportDetails(string CityAirportId);

        /// <summary>
        /// Gets all city airport list.
        /// </summary>
        /// <returns></returns>
        List<CityAirport> GetAllCityAirportList();

        /// <summary>
        /// Gets the city airport list.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="CityAirportName">Name of the city airport.</param>
        /// <param name="CountryCode">The country code.</param>
        /// <returns></returns>
        List<CityAirport> GetCityAirportList(string Id, string CityAirportName, string CountryCode);
    }
}
