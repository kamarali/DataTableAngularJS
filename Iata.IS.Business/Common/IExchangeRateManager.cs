using System.Collections.Generic;
using Iata.IS.Model.Common;
using System;
namespace Iata.IS.Business.Common
{
    public interface IExchangeRateManager
    {
        /// <summary>
        /// Adds the exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        ExchangeRate AddExchangeRate(ExchangeRate exchangeRate);

        /// <summary>
        /// Updates the exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        ExchangeRate UpdateExchangeRate(ExchangeRate exchangeRate);

        /// <summary>
        /// Deletes the exchange rate.
        /// </summary>
        /// <param name="ExchangeRateId">The exchange rate id.</param>
        /// <returns></returns>
        bool DeleteExchangeRate(int ExchangeRateId);

        /// <summary>
        /// Gets the exchange rate details.
        /// </summary>
        /// <param name="ExchangeRateId">The exchange rate id.</param>
        /// <returns></returns>
        ExchangeRate GetExchangeRateDetails(int ExchangeRateId);

        /// <summary>
        /// Gets all exchange rate list.
        /// </summary>
        /// <returns></returns>
        List<ExchangeRate> GetAllExchangeRateList();

        /// <summary>
        /// Gets the exchange rate list.
        /// </summary>
        /// <param name="CurrencyId">The currency id.</param>
        /// <param name="Fromdate">The fromdate.</param>
        /// <param name="Todate">The todate.</param>
        /// <returns></returns>
        List<ExchangeRate> GetExchangeRateList(int CurrencyId, DateTime? Fromdate, DateTime? Todate);

        /// <summary>
        /// CMP#459 : Gets the currency converted amount.
        /// </summary>
        /// <param name="prevInvSmi">The prev inv smi.</param>
        /// <param name="currentInvSmi">The current inv smi.</param>
        /// <param name="prevInvBillingCurrency">The prev inv billing currency.</param>
        /// <param name="prevInvListingCurrency">The prev inv listing currency.</param>
        /// <param name="currentInvListingCurrency">The current inv listing currency.</param>
        /// <param name="prevInvBillingMonth">The prev inv billing month.</param>
        /// <param name="prevInvBillingYear">The prev inv billing year.</param>
        /// <param name="currentInvBillingMonth">The current inv billing month.</param>
        /// <param name="currentInvBillingYear">The current inv billing year.</param>
        /// <param name="prevInvExchangeRate">The prev inv exchange rate.</param>
        /// <param name="currentInvExchangeRate">The current inv exchange rate.</param>
        /// <param name="prevAmount">The prev amount.</param>
        /// <returns></returns>
        double GetCurrencyConvertedAmount(int prevInvSmi, int currentInvSmi, int prevInvBillingCurrency, int prevInvListingCurrency,
                                         int currentInvListingCurrency, int prevInvBillingMonth, int prevInvBillingYear,
                                         int currentInvBillingMonth, int currentInvBillingYear, double prevInvExchangeRate, double currentInvExchangeRate ,double prevAmount);

        /// <summary>
        /// CMP#459 : Determines whether [is rm amount validation required] [the specified prev inv smi].
        /// </summary>
        /// <param name="prevInvSmi">The prev inv smi.</param>
        /// <param name="currentInvSmi">The current inv smi.</param>
        /// <param name="prevInvBillingCurrency">The prev inv billing currency.</param>
        /// <param name="prevInvListingCurrency">The prev inv listing currency.</param>
        /// <param name="currentInvListingCurrency">The current inv listing currency.</param>
        /// <param name="prevInvBillingMonth">The prev inv billing month.</param>
        /// <param name="prevInvBillingYear">The prev inv billing year.</param>
        /// <param name="currentInvBillingMonth">The current inv billing month.</param>
        /// <param name="currentInvBillingYear">The current inv billing year.</param>
        /// <param name="prevInvExchangeRate">The prev inv exchange rate.</param>
        /// <returns>
        ///   <c>true</c> if [is rm amount validation required] [the specified prev inv smi]; otherwise, <c>false</c>.
        /// </returns>
        bool IsRmAmountValidationRequired(int prevInvSmi,
                                          int currentInvSmi,
                                          int prevInvBillingCurrency,
                                          int prevInvListingCurrency,
                                          int currentInvListingCurrency,
                                          int prevInvBillingMonth,
                                          int prevInvBillingYear,
                                          int currentInvBillingMonth,
                                          int currentInvBillingYear,
                                          double prevInvExchangeRate);

        /// <summary>
        /// CMP#459 : Determines whether [is valid attribute of rejected invoice] [the specified smi].
        /// </summary>
        /// <param name="smi">The smi.</param>
        /// <param name="listingCurrency">The listing currency.</param>
        /// <param name="billingCurrencyId">The billing currency id.</param>
        /// <param name="invExchangeRate">The inv exchange rate.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="previnvExchangeRateD">The previnv exchange rate D.</param>
        /// <returns>
        ///   <c>true</c> if [is valid attribute of rejected invoice] [the specified smi]; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidAttributeOfRejectedInvoice(int smi, int listingCurrency, int billingCurrencyId, double invExchangeRate, int billingYear, int billingMonth, ExchangeRate previnvExchangeRateD);
    }
}
