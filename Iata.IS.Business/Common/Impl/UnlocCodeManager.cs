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
    public class UnlocCodeManager:IUnlocCodeManager
    {
        /// <summary>
        /// Gets or sets the unloc code repository.
        /// </summary>
        /// <value>
        /// The unloc code repository.
        /// </value>
        public IRepository<UnlocCode> UnlocCodeRepository { get; set; }

        public IRepository<Country> CountryRepository { get; set; }

        /// <summary>
        /// Adds the unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <returns></returns>
        public UnlocCode AddUnlocCode(UnlocCode unlocCode)
        {
            var countryCode = unlocCode.Id.Substring(0, 2);
            var country = CountryRepository.Get(c => c.Id == countryCode).FirstOrDefault();
            if(country==null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCountryCodeofUnLocation);
            }
            var unlocCodeData = UnlocCodeRepository.Single(type => type.Id == unlocCode.Id);
            //If BillingCategory Code already exists, throw exception
            if (unlocCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidUNLocationCode);
            }
            //Call repository method for adding unlocCode
            UnlocCodeRepository.Add(unlocCode);
            UnitOfWork.CommitDefault();
            return unlocCode;
        }

        /// <summary>
        /// Updates the unloc code.
        /// </summary>
        /// <param name="unlocCode">The unloc code.</param>
        /// <returns></returns>
        public UnlocCode UpdateUnlocCode(UnlocCode unlocCode)
        {
            var unlocCodeData = UnlocCodeRepository.Single(type => type.Id == unlocCode.Id);
            //If BillingCategory Code already exists, throw exception
            if (unlocCodeData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidUNLocationCode);
            } 
            var updatedbillingCategory = UnlocCodeRepository.Update(unlocCode);
            UnitOfWork.CommitDefault();
            return updatedbillingCategory;
        }

        /// <summary>
        /// Deletes the unloc code.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public bool DeleteUnlocCode(string Id)
        {
            bool delete = false;
            var unlocCodeData = UnlocCodeRepository.Single(type => type.Id == Id);
            if (unlocCodeData != null)
            {
                unlocCodeData.IsActive = !(unlocCodeData.IsActive);
                var updatedcountry = UnlocCodeRepository.Update(unlocCodeData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the unloc code details.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public UnlocCode GetUnlocCodeDetails(string Id)
        {
            var unlocCode = UnlocCodeRepository.Single(type => type.Id == Id);
            return unlocCode;
        }

        /// <summary>
        /// Gets all unloc code list.
        /// </summary>
        /// <returns></returns>
        public List<UnlocCode> GetAllUnlocCodeList()
        {
          var unlocCodeList = UnlocCodeRepository.Get(unlockCode => unlockCode.IsActive);
          return unlocCodeList.ToList();
        }
        
        /// <summary>
        /// Check if Unloc code exists in database.
        /// </summary>
        /// <param name="unlocCode"></param>
        /// <returns></returns>
        public bool IsValidUnlocCode(string unlocCode)
        {
          return UnlocCodeRepository.GetCount(unlocRecord => unlocRecord.IsActive && unlocRecord.Id == unlocCode) > 0;
        }

        /// <summary>
        /// Gets the unloc code list.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public List<UnlocCode> GetUnlocCodeList(string Id, string Name)
        {
            var cityAirportList = new List<UnlocCode>();
            cityAirportList = UnlocCodeRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Id))
            {
                cityAirportList = cityAirportList.Where(cA => (cA.Id.ToLower().Contains(Id.ToLower()))).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                cityAirportList = cityAirportList.Where(cA => (cA.Name.ToLower().Contains(Name.ToLower()))).ToList();
            }
            return cityAirportList.ToList();
        }
    }
}
