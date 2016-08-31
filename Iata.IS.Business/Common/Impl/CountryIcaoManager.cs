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
    public class CountryIcaoManager : ICountryIcaoManager
    {
        /// <summary>
        /// Gets or sets the country icao repository.
        /// </summary>
        /// <value>
        /// The country icao repository.
        /// </value>
        public IRepository<CountryIcao> CountryIcaoRepository { get; set; }

        /// <summary>
        /// Adds the country icao.
        /// </summary>
        /// <param name="countryIcao">The country icao.</param>
        /// <returns></returns>
        public CountryIcao AddCountryIcao(CountryIcao countryIcao)
        {
            var countryIcaoData = CountryIcaoRepository.Single(type => type.CountryCodeIcao.ToLower() == countryIcao.CountryCodeIcao.ToLower());
            //If CountryIcao Code already exists, throw exception
            if (countryIcaoData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCountryIcaoCode);
            }
            //Call repository method for adding countryIcao
            CountryIcaoRepository.Add(countryIcao);
            UnitOfWork.CommitDefault();
            return countryIcao;
        }

        /// <summary>
        /// Updates the country icao.
        /// </summary>
        /// <param name="countryIcao">The country icao.</param>
        /// <returns></returns>
        public CountryIcao UpdateCountryIcao(CountryIcao countryIcao)
        {
            var countryIcaoData = CountryIcaoRepository.Single(type => type.Id != countryIcao.Id && type.CountryCodeIcao.ToLower() == countryIcao.CountryCodeIcao.ToLower() );
            if (countryIcaoData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCountryIcaoCode);
            }
            countryIcaoData = CountryIcaoRepository.Single(type => type.Id == countryIcao.Id);
            var updatedcountryIcao = CountryIcaoRepository.Update(countryIcao);
            UnitOfWork.CommitDefault();
            return updatedcountryIcao;
        }

        /// <summary>
        /// Deletes the country icao.
        /// </summary>
        /// <param name="countryIcaoId">The country icao id.</param>
        /// <returns></returns>
        public bool DeleteCountryIcao(int countryIcaoId)
        {
            bool delete = false;
            var countryIcaoData = CountryIcaoRepository.Single(type => type.Id == countryIcaoId);
            if (countryIcaoData != null)
            {
                countryIcaoData.IsActive = !(countryIcaoData.IsActive);
                var updatedcountry = CountryIcaoRepository.Update(countryIcaoData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the country icao details.
        /// </summary>
        /// <param name="countryIcaoId">The country icao id.</param>
        /// <returns></returns>
        public CountryIcao GetCountryIcaoDetails(int countryIcaoId)
        {
            var countryIcao = CountryIcaoRepository.Single(type => type.Id == countryIcaoId);
            return countryIcao;
        }

        /// <summary>
        /// Gets all country icao list.
        /// </summary>
        /// <returns></returns>
        public List<CountryIcao> GetAllCountryIcaoList()
        {
            var countryIcaoList = CountryIcaoRepository.GetAll();
            return countryIcaoList.ToList();
        }

        /// <summary>
        /// Gets the country icao list.
        /// </summary>
        /// <param name="countryIcaoCode"></param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public List<CountryIcao> GetCountryIcaoList(string countryIcaoCode, string name)
        {
            var countryIcaoList = new List<CountryIcao>();
            countryIcaoList = CountryIcaoRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(countryIcaoCode))
            {
                countryIcaoList = countryIcaoList.Where(cl => cl.CountryCodeIcao.ToLower().Contains(countryIcaoCode.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(name))
            {
                countryIcaoList = countryIcaoList.Where(cl => cl.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            return countryIcaoList.ToList();
        }
    }
}
