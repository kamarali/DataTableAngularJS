using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
namespace Iata.IS.Data.Common
{
    public interface IExchangeRateRepository : IRepository<ExchangeRate>
    {
        /// <summary>
        /// Gets all exchange rate.
        /// </summary>
        /// <returns></returns>
        IQueryable<ExchangeRate> GetAllExchangeRate();

        /// <summary>
        /// CMP#459 : Gets the currency convered Amount.
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
        /// <param name="prevAmount">The prev amount.</param>
        /// <returns></returns>
        double GetCurrencyConvertedAmount(int prevInvSmi,
                                           int currentInvSmi,
                                           int prevInvBillingCurrency,
                                           int prevInvListingCurrency,
                                           int currentInvListingCurrency,
                                           int prevInvBillingMonth,
                                           int prevInvBillingYear,
                                           int currentInvBillingMonth,
                                           int currentInvBillingYear,
                                           double prevInvExchangeRate, 
                                           double prevAmount);
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
    }
}
