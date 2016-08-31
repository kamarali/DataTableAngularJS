using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ICountryManager
    {

        /// <summary>
        /// Adds the country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
        Country AddCountry(Country country);


        /// <summary>
        /// Updates the country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
        Country UpdateCountry(Country country);


        /// <summary>
        /// Deletes the country.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <returns></returns>
        bool DeleteCountry(string countryId);


        /// <summary>
        /// Gets the country details.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <returns></returns>
        Country GetCountryDetails(string countryId);

        /// <summary>
        /// Gets all country list.
        /// </summary>
        /// <returns></returns>
        List<Country> GetAllCountryList();

        /// <summary>
        /// Gets the country list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="CountryCodeIcao">The country code icao.</param>
        /// <returns></returns>
        List<Country> GetCountryList(string Name, string CountryCode);



    }
}
