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
    public class AircraftTypeIcaoManager : IAircraftTypeIcaoManager
    {
        /// <summary>
        /// Gets or sets the aircraft type icao repository.
        /// </summary>
        /// <value>
        /// The aircraft type icao repository.
        /// </value>
        public IRepository<AircraftTypeIcao> AircraftTypeIcaoRepository { get; set; }

        /// <summary>
        /// Adds the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraft type icao.</param>
        /// <returns></returns>
        public AircraftTypeIcao AddAircraftTypeIcao(AircraftTypeIcao aircraftTypeIcao)
        {
            var aircraftTypeIcaoData = AircraftTypeIcaoRepository.Single(type => type.Id == aircraftTypeIcao.Id);
            //If AircraftTypeIcao Code already exists, throw exception
            if (aircraftTypeIcaoData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidAircraftTypeIcaoCode);
            }
            //Call repository method for adding aircraftTypeIcao
            AircraftTypeIcaoRepository.Add(aircraftTypeIcao);
            UnitOfWork.CommitDefault();
            return aircraftTypeIcao;
        }

        /// <summary>
        /// Updates the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcao">The aircraft type icao.</param>
        /// <returns></returns>
        public AircraftTypeIcao UpdateAircraftTypeIcao(AircraftTypeIcao aircraftTypeIcao)
        {
            var aircraftTypeIcaoData = AircraftTypeIcaoRepository.Single(type => type.Id == aircraftTypeIcao.Id );
            if (aircraftTypeIcaoData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidAircraftTypeIcaoCode);
            }
            var updatedaircraftTypeIcao = AircraftTypeIcaoRepository.Update(aircraftTypeIcao);
            UnitOfWork.CommitDefault();
            return updatedaircraftTypeIcao;
        }

        /// <summary>
        /// Deletes the aircraft type icao.
        /// </summary>
        /// <param name="aircraftTypeIcaoId">The aircraft type icao id.</param>
        /// <returns></returns>
        public bool DeleteAircraftTypeIcao(string aircraftTypeIcaoId)
        {
            bool delete = false;
            var aircraftTypeIcaoData = AircraftTypeIcaoRepository.Single(type => type.Id == aircraftTypeIcaoId);
            if (aircraftTypeIcaoData != null)
            {
                aircraftTypeIcaoData.IsActive = !(aircraftTypeIcaoData.IsActive);
                var updatedcountry = AircraftTypeIcaoRepository.Update(aircraftTypeIcaoData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the aircraft type icao details.
        /// </summary>
        /// <param name="aircraftTypeIcaoId">The aircraft type icao id.</param>
        /// <returns></returns>
        public AircraftTypeIcao GetAircraftTypeIcaoDetails(string aircraftTypeIcaoId)
        {
            var aircraftTypeIcao = AircraftTypeIcaoRepository.Single(type => type.Id == aircraftTypeIcaoId);
            return aircraftTypeIcao;
        }

        /// <summary>
        /// Gets all aircraft type icao list.
        /// </summary>
        /// <returns></returns>
        public List<AircraftTypeIcao> GetAllAircraftTypeIcaoList()
        {
            var aircraftTypeIcaoList = AircraftTypeIcaoRepository.GetAll();
            return aircraftTypeIcaoList.ToList();
        }

        /// <summary>
        /// Gets the aircraft type icao list.
        /// </summary>
        /// <param name="aircraftTypeIcaoCode">The aircraft type icao code.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public List<AircraftTypeIcao> GetAircraftTypeIcaoList(string aircraftTypeIcaoCode, string description)
        {
            var aircraftTypeIcaoList = new List<AircraftTypeIcao>();
            aircraftTypeIcaoList = AircraftTypeIcaoRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(aircraftTypeIcaoCode))
            {
                aircraftTypeIcaoList = aircraftTypeIcaoList.Where(cl => cl.Id.ToLower().Contains(aircraftTypeIcaoCode.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(description))
            {
                aircraftTypeIcaoList = aircraftTypeIcaoList.Where(cl => !string.IsNullOrEmpty(cl.Description) && cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }
            return aircraftTypeIcaoList.ToList();
        }
    }
}
