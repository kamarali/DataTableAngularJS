using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileHelpers;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Data.Impl;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class ExchangeRateManager : IExchangeRateManager 
    {
        //public IRepository<ExchangeRate> ExchangeRateRepository { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate repository.
        /// </summary>
        /// <value>
        /// The exchange rate repository.
        /// </value>
        public IExchangeRateRepository ExchangeRateRepository { get; set; }

        /// <summary>
        /// Gets or sets the reference manager.
        /// </summary>
        /// <value>
        /// The reference manager.
        /// </value>
        public IReferenceManager ReferenceManager { get; set; }

        /// <summary>
        /// Adds the exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        public ExchangeRate AddExchangeRate(ExchangeRate exchangeRate)
        {
            var exchangeRateData = ExchangeRateRepository.Single(type => type.CurrencyId == exchangeRate.CurrencyId && type.EffectiveFromDate == exchangeRate.EffectiveFromDate && type.EffectiveToDate == exchangeRate.EffectiveToDate);
            //If ExchangeRate Code already exists, throw exception
            if (exchangeRateData != null)
            {
                throw new ISBusinessException(ErrorCodes.ExchangeRateAlreadyExists);
            }
            //Call repository method for adding exchangeRate
            ExchangeRateRepository.Add(exchangeRate);
            UnitOfWork.CommitDefault();
            return exchangeRate;
        }

        /// <summary>
        /// Updates the exchange rate.
        /// </summary>
        /// <param name="exchangeRate">The exchange rate.</param>
        /// <returns></returns>
        public ExchangeRate UpdateExchangeRate(ExchangeRate exchangeRate)
        {
            var exchangeRateData = ExchangeRateRepository.Single(type => type.Id != exchangeRate.Id && type.CurrencyId == exchangeRate.CurrencyId && type.EffectiveFromDate == exchangeRate.EffectiveFromDate && type.EffectiveToDate == exchangeRate.EffectiveToDate);
            //If ExchangeRate Code already exists, throw exception
            if (exchangeRateData != null)
            {
                throw new ISBusinessException(ErrorCodes.ExchangeRateAlreadyExists);
            }
            exchangeRateData = ExchangeRateRepository.Single(type => type.Id == exchangeRate.Id);
            var updatedexchangeRate = ExchangeRateRepository.Update(exchangeRate);
            UnitOfWork.CommitDefault();
            return updatedexchangeRate;
        }

        /// <summary>
        /// Deletes the exchange rate.
        /// </summary>
        /// <param name="ExchangeRateId">The exchange rate id.</param>
        /// <returns></returns>
        public bool DeleteExchangeRate(int ExchangeRateId)
        {
            bool delete = false;
            var exchangeRateData = ExchangeRateRepository.Single(type => type.Id == ExchangeRateId);
            if (exchangeRateData != null)
            {
                exchangeRateData.IsActive = !(exchangeRateData.IsActive);
                var updatedcountry = ExchangeRateRepository.Update(exchangeRateData);
                delete = true;
                UnitOfWork.CommitDefault();
            }
            return delete;
        }

        /// <summary>
        /// Gets the exchange rate details.
        /// </summary>
        /// <param name="ExchangeRateId">The exchange rate id.</param>
        /// <returns></returns>
        public ExchangeRate GetExchangeRateDetails(int ExchangeRateId)
        {
            var exchangeRate = ExchangeRateRepository.Single(type => type.Id == ExchangeRateId);
            return exchangeRate;
        }

        /// <summary>
        /// Gets all exchange rate list.
        /// </summary>
        /// <returns></returns>
        public List<ExchangeRate> GetAllExchangeRateList()
        {
            var exchangeRateList = ExchangeRateRepository.GetAllExchangeRate();
            return exchangeRateList.ToList();
        }

        /// <summary>
        /// Gets the exchange rate list.
        /// </summary>
        /// <param name="CurrencyId">The currency id.</param>
        /// <param name="Fromdate">The fromdate.</param>
        /// <param name="Todate">The todate.</param>
        /// <returns></returns>
        public List<ExchangeRate> GetExchangeRateList(int CurrencyId, DateTime? Fromdate, DateTime? Todate)
        {
            var exchangeRateList = new List<ExchangeRate>();
            exchangeRateList = ExchangeRateRepository.GetAllExchangeRate().ToList();
            if (CurrencyId>0)
            {
                exchangeRateList = exchangeRateList.Where(cl => cl.CurrencyId == CurrencyId).ToList();
            }
            if (Fromdate != null && Fromdate>(new DateTime()))
            {
                exchangeRateList = exchangeRateList.Where(cl => cl.EffectiveFromDate >= Convert.ToDateTime(Fromdate)).ToList();
            }
            if (Todate != null && Todate > (new DateTime()))
            {
                exchangeRateList =
                    exchangeRateList.Where(
                        (cl =>
                         Convert.ToDateTime(cl.EffectiveFromDate.ToShortDateString()) <= Convert.ToDateTime(Convert.ToDateTime(Todate).ToShortDateString()))).
                        ToList();
               // exchangeRateList = exchangeRateList.Where(cl => String.Format("{0:MM/dd/yyyy}",cl.EffectiveToDate) <= String.Format("{0:MM/dd/yyyy}",Convert.ToDateTime(Todate)).ToList());
            }
            return exchangeRateList.ToList();
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
        /// <param name="currentInvExchangeRate">The current inv exchange rate.</param>
        /// <param name="prevAmount">The prev amount.</param>
        /// <returns></returns>
        public double GetCurrencyConvertedAmount(int prevInvSmi, int currentInvSmi, int prevInvBillingCurrency, int prevInvListingCurrency,
         int currentInvListingCurrency, int prevInvBillingMonth, int prevInvBillingYear, int currentInvBillingMonth, int currentInvBillingYear, double prevInvExchangeRate, double currentInvExchangeRate, double prevAmount)
        {
            var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
            double conversionfactor = 1;
            try
            {
                conversionfactor = exchangeRateRepository.GetCurrencyConvertedAmount(prevInvSmi,
                                                                   currentInvSmi,
                                                                   prevInvBillingCurrency,
                                                                   prevInvListingCurrency,
                                                                   currentInvListingCurrency,
                                                                   prevInvBillingMonth,
                                                                   prevInvBillingYear,
                                                                   currentInvBillingMonth,
                                                                   currentInvBillingYear,
                                                                   prevInvExchangeRate,prevAmount);
            }
            catch (Exception ex)
            {
                conversionfactor = 1;
            }
            return conversionfactor;
        }

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
            var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
            bool isRmAmountValidationRequired = false;
            try
            {
                isRmAmountValidationRequired = exchangeRateRepository.IsRmAmountValidationRequired(prevInvSmi,
                                                                   currentInvSmi,
                                                                   prevInvBillingCurrency,
                                                                   prevInvListingCurrency,
                                                                   currentInvListingCurrency,
                                                                   prevInvBillingMonth,
                                                                   prevInvBillingYear,
                                                                   currentInvBillingMonth,
                                                                   currentInvBillingYear,
                                                                   prevInvExchangeRate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isRmAmountValidationRequired;
        }

        /// <summary>
        /// CMP#459 : Determines whether [is valid attribute of rejected invoice] [the specified smi].
        /// </summary>
        /// <param name="smi">The smi.</param>
        /// <param name="listingCurrencyId">The listing currency id.</param>
        /// <param name="billingCurrencyId">The billing currency id.</param>
        /// <param name="invExchangeRate">The inv exchange rate.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="previnvExchangeRateDb">The previnv exchange rate db.</param>
        /// <returns>
        ///   <c>true</c> if [is valid attribute of rejected invoice] [the specified smi]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidAttributeOfRejectedInvoice(int smi, int listingCurrencyId, int billingCurrencyId, double invExchangeRate, int billingYear, int billingMonth, ExchangeRate previnvExchangeRateDb)
        {
            var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
            bool isValid = false;
            // CMP#624 : Change#3 - Conditional validation of PAX/CGO Billed/Allowed amounts 
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if (smi == (int)SettlementMethodValues.IchSpecialAgreement || smi == (int)SettlementMethodValues.Ich || smi == (int)SettlementMethodValues.Ach || smi == (int)SettlementMethodValues.AchUsingIATARules)
            {
                isValid = true;
            }
            else if ((smi == (int)SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(smi, false)))
            {
                if (billingCurrencyId == (int)BillingCurrency.USD || billingCurrencyId == (int)BillingCurrency.GBP || billingCurrencyId == (int)BillingCurrency.EUR)
                {
                    if (previnvExchangeRateDb != null)
                    {
                        /* SCP# 305858 - FW: 350-YB Tax Rejections Which Failed In SIS Validation 
                         * Desc: Instead of exact comparison between exchange rates, common code in compare utility is used.
                         * Newly added code compares two exchange rates with 0 tollerance till 5 decimal places only.
                         * Because of some unknown reasons - GBP exchange rate for currency ID 949 for the month of June 2014 was retrievinge incorrect value -
                         * DB has EUR FDR value as 2.87499, but .Net gets 2.8749900000000000004 causing the issue.
                         */
                        if (billingCurrencyId == (int)BillingCurrency.USD)
                        {
                            if (CompareUtil.Compare(invExchangeRate, previnvExchangeRateDb.FiveDayRateUsd, 0D, Constants.ExchangeRateDecimalPlaces)) isValid = true;
                        }
                        if (billingCurrencyId == (int)BillingCurrency.GBP)
                        {
                            if (CompareUtil.Compare(invExchangeRate, previnvExchangeRateDb.FiveDayRateGbp, 0D, Constants.ExchangeRateDecimalPlaces)) isValid = true;
                        }
                        if (billingCurrencyId == (int)BillingCurrency.EUR)
                        {
                            if (CompareUtil.Compare(invExchangeRate, previnvExchangeRateDb.FiveDayRateEur, 0D, Constants.ExchangeRateDecimalPlaces)) isValid = true;
                        }
                    }
                }
                else
                {
                    isValid = true;
                }
            }
            return isValid;
        }
    }
}
