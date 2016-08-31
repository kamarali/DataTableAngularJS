using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ICountryIcaoManager
    {
        /// <summary>
        /// Adds the country icao.
        /// </summary>
        /// <param name="countryIcao">The country icao.</param>
        /// <returns></returns>
        CountryIcao AddCountryIcao(CountryIcao countryIcao);

        /// <summary>
        /// Updates the country icao.
        /// </summary>
        /// <param name="countryIcao">The country icao.</param>
        /// <returns></returns>
        CountryIcao UpdateCountryIcao(CountryIcao countryIcao);

        /// <summary>
        /// Deletes the country icao.
        /// </summary>
        /// <param name="countryIcaoId">The country icao id.</param>
        /// <returns></returns>
        bool DeleteCountryIcao(int countryIcaoId);

        /// <summary>
        /// Gets the country icao details.
        /// </summary>
        /// <param name="countryIcaoId">The country icao id.</param>
        /// <returns></returns>
        CountryIcao GetCountryIcaoDetails(int countryIcaoId);

        /// <summary>
        /// Gets all country icao list.
        /// </summary>
        /// <returns></returns>
        List<CountryIcao> GetAllCountryIcaoList();

        /// <summary>
        /// Gets the country icao list.
        /// </summary>
        /// <param name="countryIcao">The country icao.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        List<CountryIcao> GetCountryIcaoList(string countryIcaoCode, string name);

    }
}
