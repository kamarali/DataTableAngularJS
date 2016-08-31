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
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class ChargeCategoryManager :IChargeCategoryManager
    {
        /// <summary>
        /// Gets or sets the charge category repository.
        /// </summary>
        /// <value>
        /// The charge category repository.
        /// </value>
        public IRepository<ChargeCategory> ChargeCategoryRepository { get; set; }

        /// <summary>
        /// Adds the charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <returns></returns>
        public ChargeCategory AddChargeCategory(ChargeCategory chargeCategory)
        {
            var ChargeCategoryData = ChargeCategoryRepository.Single(type => type.Id == chargeCategory.Id);
            //If ChargeCategory Code already exists, throw exception
            if (ChargeCategoryData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCountryCode);
            }
            ChargeCategoryRepository.Add(chargeCategory);
            UnitOfWork.CommitDefault();
            return chargeCategory;
        }

        /// <summary>
        /// Updates the charge category.
        /// </summary>
        /// <param name="chargeCategory">The charge category.</param>
        /// <returns></returns>
        public ChargeCategory UpdateChargeCategory(ChargeCategory chargeCategory)
        {
            var ChargeCategoryData = ChargeCategoryRepository.Single(type => type.Id == chargeCategory.Id);
            var updatedChargeCategory = ChargeCategoryRepository.Update(chargeCategory);
            UnitOfWork.CommitDefault();
            return updatedChargeCategory;
        }

        /// <summary>
        /// Deletes the charge category.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public bool DeleteChargeCategory(int Id)
        {
            bool delete = false;
            var airportData = ChargeCategoryRepository.Single(type => type.Id ==Id);
            if (airportData != null)
            {
                airportData.IsActive = !(airportData.IsActive);
                var updatedcountry = ChargeCategoryRepository.Update(airportData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the charge category details.
        /// </summary>
        /// <param name="Id">The id.</param>
        /// <returns></returns>
        public ChargeCategory GetChargeCategoryDetails(int Id)
        {
            var ChargeCategory = ChargeCategoryRepository.Single(type => type.Id == Id);
            return ChargeCategory;
        }

        /// <summary>
        /// Gets all charge category list.
        /// </summary>
        /// <returns></returns>
        public List<ChargeCategory> GetAllChargeCategoryList()
        {
            var ChargeCategoryList = ChargeCategoryRepository.GetAll();

            return ChargeCategoryList.ToList();
        }

        /// <summary>
        /// Gets the charge category list.
        /// </summary>
        /// <param name="Name">The name.</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <returns></returns>
        public List<ChargeCategory> GetChargeCategoryList(string Name, int billingCategoryId)
        {
            var ChargeCategoryList = new List<ChargeCategory>();
            ChargeCategoryList = ChargeCategoryRepository.GetAll().ToList();

            if ((!string.IsNullOrEmpty(Name)))
            {
                ChargeCategoryList = ChargeCategoryList.Where(cl => (cl.Name.ToLower().Contains(Name.ToLower()))).ToList();
            }
            if (billingCategoryId>0)
            {
                ChargeCategoryList = ChargeCategoryList.Where(cl => cl.BillingCategoryId == billingCategoryId).ToList();
            }
           
            return ChargeCategoryList.ToList();
        }
    }
}
