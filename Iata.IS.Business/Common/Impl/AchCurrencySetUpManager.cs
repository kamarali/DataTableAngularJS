using System.Collections.Generic;
using System.Linq;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Master;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Data;

namespace Iata.IS.Business.Common.Impl
{
    /// <summary>
    /// Ach Currency set up manager
    /// CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1
    /// </summary>
    public class AchCurrencySetUpManager : IAchCurrencySetUpManager
    {
        public IAchCurrencySetUpRepository AchCurrencyRepository { get; set; }

        /// <summary>
        /// Adds the AchCurrencySetUp.
        /// </summary>
        /// <param name="achCurrencySetUp">The AchCurrencySetUp.</param>
        /// <returns></returns>
        public void AddAchCurrencySetUp(AchCurrencySetUp achCurrencySetUp)
        {

            var currencyCodeData = AchCurrencyRepository.Single(type => type.Id == achCurrencySetUp.Id);

            //If AircraftType Code already exists, throw exception
            if (currencyCodeData != null)
            {
                throw new ISBusinessException(ErrorCodes.DuplicateAchCurrency);
            }

            //Call repository method for adding ach currency
            AchCurrencyRepository.Add(achCurrencySetUp);

            UnitOfWork.CommitDefault();
        }

        /// <summary>
        /// Deletes the AchCurrencySetUp.
        /// </summary>
        /// <param name="currencyCodeNum"></param>
        /// <returns></returns>
        public void DeleteAchCurrencySetUp(int currencyCodeNum)
        {
            var aircraftTypeData = AchCurrencyRepository.Single(type => type.Id == currencyCodeNum);
            if (aircraftTypeData != null)
            {
                aircraftTypeData.IsActive = !(aircraftTypeData.IsActive);
                AchCurrencyRepository.Update(aircraftTypeData);
                UnitOfWork.CommitDefault();
            }
        }

        /// <summary>
        /// Gets the AchCurrencySetUp list.
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="getOnlyActiveCurrencies"></param>
        /// <returns></returns>
        public List<AchCurrencySearchData> GetAchCurrencySetUpList(string currencyCode, int getOnlyActiveCurrencies = 0)
        {
            List<AchCurrencySearchData> achCurrencySetUpList = AchCurrencyRepository.GetAchCurrencyData(currencyCode, getOnlyActiveCurrencies);

            return achCurrencySetUpList.ToList();
        }

        /// <summary>
        /// Determines whether [is valid ach currency code] [the specified ach currency code].
        /// </summary>
        /// <param name = "currencyId">The currency id.</param>
        /// <returns>
        /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAchCurrency(int? currencyId)
        {
            if (currencyId == null)
            {
                return false;
            }
            return AchCurrencyRepository.GetCount(currency => currency.IsActive && currency.Id == currencyId) > 0;
        }
    }
}
