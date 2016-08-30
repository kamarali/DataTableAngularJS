using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class ExchangeRateRepository : Repository<ExchangeRate>, IExchangeRateRepository
    {
        public const string GetCurrencyConversionFactorProcName = "GetCurrencyConvertedAmount";
        public const string CheckIsRmAmountValidationRequiredProcName = "IsRmAmountValidationRequired";
        public const string PrevInvSmiParamName = "PREV_INV_SMI_I";
        public const string CurrentInvSmiParamName = "CURRENT_INV_SMI_I";
        public const string PrevInvBillingCurrencyParamName = "PREV_BILLING_CURRENCY_I";
        public const string PrevInvListingCurrencyParamName = "PREV_LISTING_CURRENCY_I";
        public const string CurrentInvListingCurrencyParamName = "CURRENT_LISTING_CURRENCY_I";
        public const string PrevInvBillingMonthParamName = "PREV_BILLING_MONTH_I";
        public const string CurrentInvBillingMonthParamName = "CURRENT_BILLING_MONTH_I";
        public const string PrevInvBillingYearParamName = "PREV_BILLING_YEAR_I";
        public const string CurrentInvBillingYearParamName = "CURRENT_BILLING_YEAR_I";
        public const string CurrencyConversionFactorParamName = "CURR_CONVERSN_FACTOR_O";
        public const string PrevInvExchangeRateParamName = "PREV_INV_EXCH_RATE_I";
        public const string PrevAmountParamName = "PREV_AMOUNT_I";
        public const string CurrencyConvertedAmountParamName = "CONVERTED_AMOUNT_O";
        public const string CurrencyConversionrequiredParamName = "CONVERSN_REQUIRED_O";

        /// <summary>
        /// Gets all exchange rate.
        /// </summary>
        /// <returns></returns>
        public IQueryable<ExchangeRate> GetAllExchangeRate()
        {
            var exchangeRateList = EntityObjectSet.Include("Currency");

            return exchangeRateList;
        }

        /// <summary>
        /// CMP#459 : Gets the currency converted Amount.
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
        public double GetCurrencyConvertedAmount(int prevInvSmi,
                                           int currentInvSmi,
                                           int prevInvBillingCurrency,
                                           int prevInvListingCurrency,
                                           int currentInvListingCurrency,
                                           int prevInvBillingMonth,
                                           int prevInvBillingYear,
                                           int currentInvBillingMonth,
                                           int currentInvBillingYear,
                                           double prevInvExchangeRate,double prevAmount)
        {
            var parameters = new ObjectParameter[12];
            parameters[0] = new ObjectParameter(PrevInvSmiParamName, typeof(int)) { Value = prevInvSmi };
            parameters[1] = new ObjectParameter(CurrentInvSmiParamName, typeof(int)) { Value = currentInvSmi };
            parameters[2] = new ObjectParameter(PrevInvBillingCurrencyParamName, typeof(int)) { Value = prevInvBillingCurrency };
            parameters[3] = new ObjectParameter(PrevInvListingCurrencyParamName, typeof(int)) { Value = prevInvListingCurrency };
            parameters[4] = new ObjectParameter(CurrentInvListingCurrencyParamName, typeof(int)) { Value = currentInvListingCurrency };
            parameters[5] = new ObjectParameter(PrevInvBillingMonthParamName, typeof(int)) { Value = prevInvBillingMonth };
            parameters[6] = new ObjectParameter(CurrentInvBillingMonthParamName, typeof(int)) { Value = currentInvBillingMonth };
            parameters[7] = new ObjectParameter(PrevInvBillingYearParamName, typeof(int)) { Value = prevInvBillingYear };
            parameters[8] = new ObjectParameter(CurrentInvBillingYearParamName, typeof(int)) { Value = currentInvBillingYear };
            parameters[9] = new ObjectParameter(PrevInvExchangeRateParamName, typeof(double)) { Value = prevInvExchangeRate };
            parameters[10] = new ObjectParameter(PrevAmountParamName, typeof(double)) { Value = prevAmount };
            parameters[11] = new ObjectParameter(CurrencyConvertedAmountParamName, typeof(double));
            ExecuteStoredProcedure(GetCurrencyConversionFactorProcName, parameters);
            return Convert.ToDouble(parameters[11].Value);
        }

       public bool IsRmAmountValidationRequired(int prevInvSmi,
                                           int currentInvSmi,
                                           int prevInvBillingCurrency,
                                           int prevInvListingCurrency,
                                           int currentInvListingCurrency,
                                           int prevInvBillingMonth,
                                           int prevInvBillingYear,
                                           int currentInvBillingMonth,
                                           int currentInvBillingYear,
                                           double prevInvExchangeRate)
        {
            var parameters = new ObjectParameter[11];
            parameters[0] = new ObjectParameter(PrevInvSmiParamName, typeof(int)) { Value = prevInvSmi };
            parameters[1] = new ObjectParameter(CurrentInvSmiParamName, typeof(int)) { Value = currentInvSmi };
            parameters[2] = new ObjectParameter(PrevInvBillingCurrencyParamName, typeof(int)) { Value = prevInvBillingCurrency };
            parameters[3] = new ObjectParameter(PrevInvListingCurrencyParamName, typeof(int)) { Value = prevInvListingCurrency };
            parameters[4] = new ObjectParameter(CurrentInvListingCurrencyParamName, typeof(int)) { Value = currentInvListingCurrency };
            parameters[5] = new ObjectParameter(PrevInvBillingMonthParamName, typeof(int)) { Value = prevInvBillingMonth };
            parameters[6] = new ObjectParameter(CurrentInvBillingMonthParamName, typeof(int)) { Value = currentInvBillingMonth };
            parameters[7] = new ObjectParameter(PrevInvBillingYearParamName, typeof(int)) { Value = prevInvBillingYear };
            parameters[8] = new ObjectParameter(CurrentInvBillingYearParamName, typeof(int)) { Value = currentInvBillingYear };
            parameters[9] = new ObjectParameter(PrevInvExchangeRateParamName, typeof(double)) { Value = prevInvExchangeRate };
            parameters[10] = new ObjectParameter(CurrencyConversionrequiredParamName, typeof(int));
            ExecuteStoredProcedure(CheckIsRmAmountValidationRequiredProcName, parameters);
            return Convert.ToBoolean(parameters[10].Value);
        }
    }
}
