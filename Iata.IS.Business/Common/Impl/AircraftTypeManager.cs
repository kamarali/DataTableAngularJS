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
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class AircraftTypeManager : IAircraftTypeManager
    {
        /// <summary>
        /// Gets or sets the aircraftType repository.
        /// </summary>
        /// <value>
        /// The aircraftType repository.
        /// </value>
        public IRepository<AircraftType> AircraftTypeRepository { get; set; }

        /// <summary>
        /// Adds the aircraftType.
        /// </summary>
        /// <param name="aircraftType">The aircraftType.</param>
        /// <returns></returns>
        public AircraftType AddAircraftType(AircraftType aircraftType)
        {
            var aircraftTypeData = AircraftTypeRepository.Single(type => type.Id.ToLower() == aircraftType.Id.ToLower());
            //If AircraftType Code already exists, throw exception
            if (aircraftTypeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidAircraftTypeCode);
            }
            //Call repository method for adding aircraftType
            AircraftTypeRepository.Add(aircraftType);
            UnitOfWork.CommitDefault();
            return aircraftType;
        }

        /// <summary>
        /// Updates the aircraftType.
        /// </summary>
        /// <param name="aircraftType">The aircraftType.</param>
        /// <returns></returns>
        public AircraftType UpdateAircraftType(AircraftType aircraftType)
        {
            var aircraftTypeData = AircraftTypeRepository.Single(type => type.Id == aircraftType.Id);
            if (aircraftTypeData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidAircraftTypeCode);
            }
            var updatedaircraftType = AircraftTypeRepository.Update(aircraftType);
            UnitOfWork.CommitDefault();
            return updatedaircraftType;
        }

        /// <summary>
        /// Deletes the aircraftType.
        /// </summary>
        /// <param name="aircraftTypeId">The aircraftType id.</param>
        /// <returns></returns>
        public bool DeleteAircraftType(string aircraftTypeId)
        {
            bool delete = false;
            var aircraftTypeData = AircraftTypeRepository.Single(type => type.Id == aircraftTypeId);
            if (aircraftTypeData != null)
            {
                aircraftTypeData.IsActive = !(aircraftTypeData.IsActive);
                var updatedaircraftType = AircraftTypeRepository.Update(aircraftTypeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the aircraftType details.
        /// </summary>
        /// <param name="aircraftTypeId">The aircraftType id.</param>
        /// <returns></returns>
        public AircraftType GetAircraftTypeDetails(string aircraftTypeId)
        {
            var aircraftType = AircraftTypeRepository.Single(type => type.Id == aircraftTypeId);
            return aircraftType;
        }

        /// <summary>
        /// Gets all aircraftType list.
        /// </summary>
        /// <returns></returns>
        public List<AircraftType> GetAllAircraftTypeList()
        {
            var aircraftTypeList = AircraftTypeRepository.GetAll();

            return aircraftTypeList.ToList();
        }

        /// <summary>
        /// Gets the aircraftType list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="AircraftTypeCodeIcao">The aircraftType code icao.</param>
        /// <returns></returns>
        public List<AircraftType> GetAircraftTypeList(string id, string description)
        {
            var aircraftTypeList = new List<AircraftType>();
            aircraftTypeList = AircraftTypeRepository.GetAll().ToList();

            if ((!string.IsNullOrEmpty(description)))
            {
                aircraftTypeList = aircraftTypeList.Where(cl => cl.Description != null && cl.Description.ToLower().Contains(description.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(id))
            {
                aircraftTypeList = aircraftTypeList.Where(cl => cl.Id != null && cl.Id.ToLower().Contains(id.ToLower())).ToList();
            }

            return aircraftTypeList.ToList();
        }
    }
}
