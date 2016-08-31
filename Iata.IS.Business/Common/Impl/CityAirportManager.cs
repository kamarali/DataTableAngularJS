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
    public class CityAirportManager:ICityAirportManager
    {
        /// <summary>
        /// Gets or sets the city airport repository.
        /// </summary>
        /// <value>
        /// The city airport repository.
        /// </value>
        public IRepository<CityAirport> CityAirportRepository { get; set; }

        /// <summary>
        /// Adds the city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <returns></returns>
        public CityAirport AddCityAirport(CityAirport cityAirport)
        {
            var cityAirportData = CityAirportRepository.Single(type => type.Id == cityAirport.Id);
            if (cityAirportData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCityCodeAlpha);
            }
            else
            {
                //cityAirportData = CityAirportRepository.Single(type => type.CityCodeNumeric == cityAirport.CityCodeNumeric);
                //if (cityAirportData != null)
                //{
                //    throw new ISBusinessException(ErrorCodes.InvalidCityCodeNum);
                //}
                CityAirportRepository.Add(cityAirport);
                UnitOfWork.CommitDefault();
            }
            return cityAirport;
        }

        /// <summary>
        /// Updates the city airport.
        /// </summary>
        /// <param name="cityAirport">The city airport.</param>
        /// <returns></returns>
        public CityAirport UpdateCityAirport(CityAirport cityAirport)
        {
            var cityAirportData = CityAirportRepository.Single(type => type.Id == cityAirport.Id);
            if (cityAirportData != null)
            {
                cityAirport = CityAirportRepository.Update(cityAirport);
                UnitOfWork.CommitDefault();
            }
            else
            {
                throw new ISBusinessException(ErrorCodes.InvalidCityCodeAlpha);
            }

            return cityAirport;
        }

        /// <summary>
        /// Deletes the city airport.
        /// </summary>
        /// <param name="CityAirportId">The city airport id.</param>
        /// <returns></returns>
        public bool DeleteCityAirport(string  CityAirportId)
        {
            bool delete = false;
            var cityAirportData = CityAirportRepository.Single(type => type.Id == CityAirportId);
            if (cityAirportData != null)
            {
                cityAirportData.IsActive = !(cityAirportData.IsActive);
                var updatedcountry = CityAirportRepository.Update(cityAirportData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the city airport details.
        /// </summary>
        /// <param name="CityAirportId">The city airport id.</param>
        /// <returns></returns>
        public CityAirport GetCityAirportDetails(string CityAirportId)
        {
            var cityAirport = CityAirportRepository.Single(type => type.Id == CityAirportId);
            return cityAirport;
        }

        /// <summary>
        /// Gets all city airport list.
        /// </summary>
        /// <returns></returns>
        public List<CityAirport> GetAllCityAirportList()
        {
            var billingCategoryList = CityAirportRepository.GetAll();
            return billingCategoryList.ToList();
        }

        /// <summary>
        /// Gets the city airport list.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="CityAirportName">Name of the city airport.</param>
        /// <param name="CountryCode">The country code.</param>
        /// <returns></returns>
        public List<CityAirport> GetCityAirportList(string Id,string CityAirportName,string CountryCode)
        {
            var cityAirportList = new List<CityAirport>();
            cityAirportList = CityAirportRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Id))
            {
                cityAirportList = cityAirportList.Where(cA => (cA.Id.ToLower().Contains(Id.ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(CityAirportName))
            {
                cityAirportList = cityAirportList.Where(cA => (cA.Name.ToLower().Contains(CityAirportName.ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(CountryCode))
            {
                cityAirportList = cityAirportList.Where(cA => (cA.CountryId.ToLower().Contains(CountryCode.ToLower()))).ToList();
            }
            return cityAirportList.ToList();
        }
    }
}
