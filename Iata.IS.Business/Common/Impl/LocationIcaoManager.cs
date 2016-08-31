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
    public class LocationIcaoManager : ILocationIcaoManager
    {
        /// <summary>
        /// Gets or sets the location icao repository.
        /// </summary>
        /// <value>
        /// The location icao repository.
        /// </value>
        public IRepository<LocationIcao> LocationIcaoRepository { get; set; }

        /// <summary>
        /// Adds the location icao.
        /// </summary>
        /// <param name="locationIcao">The location icao.</param>
        /// <returns></returns>
        public LocationIcao AddLocationIcao(LocationIcao locationIcao)
        {
            var sisMemberStatusData = LocationIcaoRepository.Single(type => type.Id == locationIcao.Id);
            //If LocationIcao Code already exists, throw exception
            if (sisMemberStatusData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidLocationIcaoCode);
            }
            //Call repository method for adding sisMemberStatus
            LocationIcaoRepository.Add(locationIcao);
            UnitOfWork.CommitDefault();
            return locationIcao;
        }

        /// <summary>
        /// Updates the location icao.
        /// </summary>
        /// <param name="locationIcao">The location icao.</param>
        /// <returns></returns>
        public LocationIcao UpdateLocationIcao(LocationIcao locationIcao)
        {
            var sisMemberStatusData = LocationIcaoRepository.Single(type => type.Id == locationIcao.Id );
            if (sisMemberStatusData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidLocationIcaoCode);
            }
            var updatedsisMemberStatus = LocationIcaoRepository.Update(locationIcao);
            UnitOfWork.CommitDefault();
            return updatedsisMemberStatus;
        }

        /// <summary>
        /// Deletes the location icao.
        /// </summary>
        /// <param name="locationIcaoCode">The location icao code.</param>
        /// <returns></returns>
        public bool DeleteLocationIcao(string locationIcaoCode)
        {
            bool delete = false;
            var sisMemberStatusData = LocationIcaoRepository.Single(type => type.Id == locationIcaoCode);
            if (sisMemberStatusData != null)
            {
                sisMemberStatusData.IsActive = !(sisMemberStatusData.IsActive);
                var updatedcountry = LocationIcaoRepository.Update(sisMemberStatusData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the location icao details.
        /// </summary>
        /// <param name="locationIcaoCode">The location icao code.</param>
        /// <returns></returns>
        public LocationIcao GetLocationIcaoDetails(string locationIcaoCode)
        {
            var sisMemberStatus = LocationIcaoRepository.Single(type => type.Id == locationIcaoCode);
            return sisMemberStatus;
        }

        /// <summary>
        /// Gets all location icao list.
        /// </summary>
        /// <returns></returns>
        public List<LocationIcao> GetAllLocationIcaoList()
        {
            var sisMemberStatusList = LocationIcaoRepository.GetAll();
            return sisMemberStatusList.ToList();
        }

        /// <summary>
        /// Gets the location icao list.
        /// </summary>
        /// <param name="locationIcaoCode">The location icao code.</param>
        /// <param name="description">The description.</param>
        /// <param name="countryCode">The country code.</param>
        /// <returns></returns>
        public List<LocationIcao> GetLocationIcaoList(string locationIcaoCode, string countryCode, string description)
        {
            var sisMemberStatusList = new List<LocationIcao>();
            sisMemberStatusList = LocationIcaoRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(locationIcaoCode))
            {
                sisMemberStatusList = sisMemberStatusList.Where(cl => cl.Id.ToLower().Contains(locationIcaoCode.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(description))
            {
                sisMemberStatusList = sisMemberStatusList.Where(cl => !string.IsNullOrEmpty(cl.Description) && cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(countryCode))
            {
                sisMemberStatusList = sisMemberStatusList.Where(cl => cl.CountryCode.ToLower().Contains(countryCode.ToLower())).ToList();
            }
            return sisMemberStatusList.ToList();
        }
    }
}
