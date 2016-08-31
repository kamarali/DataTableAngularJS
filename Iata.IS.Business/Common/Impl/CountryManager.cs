using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
   public class CountryManager:ICountryManager
    {
        /// <summary>
        /// Gets or sets the country repository.
        /// </summary>
        /// <value>
        /// The country repository.
        /// </value>
       public IRepository<Country> CountryRepository { get; set; }

       /// <summary>
       /// Adds the country.
       /// </summary>
       /// <param name="country">The country.</param>
       /// <returns></returns>
       public Country AddCountry(Country country)
       {
            var countryData = CountryRepository.Single(type => type.Id.ToLower() == country.Id.ToLower());
            //If Country Code already exists, throw exception
            if (countryData != null)
            {
                throw new ISBusinessException(ErrorCodes.CountryCodeAlreadyExists);
            }
            else
            {
                //Call repository method for adding country
                CountryRepository.Add(country);
                UnitOfWork.CommitDefault();
            }
           return country;
        }

        /// <summary>
        /// Updates the country.
        /// </summary>
        /// <param name="country">The country.</param>
        /// <returns></returns>
       public Country UpdateCountry(Country country)
        {
            var countryData = CountryRepository.Single(type => type.Id == country.Id);
            //If Country Code already exists, throw exception
            if (countryData != null)
            {
                country = CountryRepository.Update(country);
                UnitOfWork.CommitDefault(); 
            }
            else
            {
                throw new ISBusinessException(ErrorCodes.CountryCodeAlreadyExists);
            }
            return country;
        }

        /// <summary>
        /// Deletes the country.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <returns></returns>
        public bool DeleteCountry(string countryId)
        {
            bool delete = false;
            var countryData = CountryRepository.Single(type => type.Id.Trim() == countryId.Trim());
            if (countryData != null)
            {
                countryData.IsActive = !(countryData.IsActive);
                var updatedcountry = CountryRepository.Update(countryData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the country details.
        /// </summary>
        /// <param name="countryId">The country id.</param>
        /// <returns></returns>
        public Country GetCountryDetails(string countryId)
        {
            var country = CountryRepository.Single(type => type.Id.Trim() == countryId.Trim());
            return country;
        }

        /// <summary>
        /// Gets all country list.
        /// </summary>
        /// <returns></returns>
        public List<Country> GetAllCountryList()
        {
            var countryList = CountryRepository.GetAll();

            return countryList.ToList();
        }

        /// <summary>
        /// Gets the country list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="CountryCodeIcao">The country code icao.</param>
        /// <returns></returns>
        public List<Country> GetCountryList(string Name, string CountryCode)
        {
            var countryList = new List<Country>();
            countryList = CountryRepository.GetAll().ToList();
            
            if ((!string.IsNullOrEmpty(Name)))
            {
                countryList = countryList.Where(cl => cl.Name != null && cl.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(CountryCode))
            {
                countryList = countryList.Where(cl => cl.Id != null && cl.Id.ToLower().Contains(CountryCode.ToLower())).ToList();
            }
           
            return countryList.ToList();
        }
    }
}
