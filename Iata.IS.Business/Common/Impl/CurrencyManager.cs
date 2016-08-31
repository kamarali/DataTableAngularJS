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
using Iata.IS.Model.MiscUatp;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
   public class CurrencyManager:ICurrencyManager 
    {
        /// <summary>
        /// Gets or sets the currency repository.
        /// </summary>
        /// <value>
        /// The currency repository.
        /// </value>
       public IRepository<Currency> CurrencyRepository { get; set; }

       /// <summary>
       /// Adds the currency.
       /// </summary>
       /// <param name="currency">The currency.</param>
       /// <returns></returns>
        public Currency AddCurrency(Currency currency)
        {
            var currencyData = CurrencyRepository.Single(type => type.Id == currency.Id);
            //If Currency Code already exists, throw exception
            if (currencyData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidNumericCurrencyCode);
            }
            currencyData = CurrencyRepository.Single(type => type.Code == currency.Code);
            //If Currency Code already exists, throw exception
            if (currencyData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCurrencyCodeAlpha);
            }
            //Call repository method for adding currency
            CurrencyRepository.Add(currency);
            UnitOfWork.CommitDefault();
            return currency;
        }

        /// <summary>
        /// Updates the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public Currency UpdateCurrency(Currency currency)
        {
            var currencyData = CurrencyRepository.Single(type =>  type.Id == currency.Id);
            if (currencyData == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidNumericCurrencyCode);
            }
            currencyData = CurrencyRepository.Single(type => type.Code == currency.Code && type.Id != currency.Id);
            if (currencyData != null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidCurrencyCodeAlpha);
            }
            var updatedcurrency = CurrencyRepository.Update(currency);
            UnitOfWork.CommitDefault();
            return updatedcurrency;
        }

        /// <summary>
        /// Deletes the currency.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <returns></returns>
        public bool DeleteCurrency(int currencyId)
        {
            bool delete = false;
            var currencyData = CurrencyRepository.Single(type => type.Id == currencyId);
            if (currencyData != null)
            {
                currencyData.IsActive = !(currencyData.IsActive);
                var updatedcountry = CurrencyRepository.Update(currencyData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the currency details.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <returns></returns>
        public Currency GetCurrencyDetails(int currencyId)
        {
            var currency = CurrencyRepository.Single(type => type.Id == currencyId);
            return currency;
        }

        /// <summary>
        /// Gets all currency list.
        /// </summary>
        /// <returns></returns>
        public List<Currency> GetAllCurrencyList()
        {
            var currencyList = CurrencyRepository.GetAll();
            return currencyList.ToList();
        }

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public List<Currency> GetCurrencyList(string Code, string Name)
        {
            var currencyList = new List<Currency>();
            currencyList = CurrencyRepository.GetAll().ToList();
            if (!string.IsNullOrEmpty(Code))
            {
                currencyList = currencyList.Where(cl => cl.Code.ToLower().Contains(Code.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                currencyList = currencyList.Where(cl => cl.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            return currencyList.ToList();
        }

        /// <summary>
        /// CMP#642: Show Appropriate Currency Decimals in MISC PDFs
        /// Get Listing Currency and Billing Currency Precision.
        /// </summary>
        /// <param name="currencyCode">Listing Currency Code.</param>
        /// <returns>Precision</returns>
        public int GetBillingCurrencyPrecision(string currencyCode)
        {
          var precision =
            CurrencyRepository.Get(l => l.Code.ToUpper() == currencyCode.ToUpper() && l.IsActive).FirstOrDefault();
          return precision != null ? precision.Precision : 0;
        }

        /// <summary>
        /// CMP#642: Show Appropriate Currency Decimals in MISC PDFs
        /// Get Listing Currency and Billing Currency Precision.
        /// </summary>
        /// <param name="currencyCode">Listing Currency Code.</param>
        /// <returns>Precision</returns>
        public int GetClearanceCurrencyPrecision(string currencyCode)
        {
          var precision =
            CurrencyRepository.Get(l => l.Code.ToUpper() == currencyCode.ToUpper() && l.IsActive).FirstOrDefault();
          return precision != null ? precision.Precision : 0;
        }
    }
}
