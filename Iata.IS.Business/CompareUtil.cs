using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using log4net;

namespace Iata.IS.Business
{
  /// <summary>
  /// Caparison utility.
  /// </summary>
  public static class CompareUtil
  {

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static readonly IReferenceManager ReferenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

    /// <summary>
    /// Check whether the new value is changed or not.
    /// </summary>
    /// <typeparam name="T">Type of values.</typeparam>
    /// <param name="newValue">New Value to be checked for changes.</param>
    /// <param name="oldValue">old value.</param>
    /// <returns></returns>
    public static bool IsDirty<T>(T newValue, T oldValue)
    {
      if (typeof(T) == typeof(string))
      {
        if (string.Compare(Convert.ToString(newValue), Convert.ToString(oldValue)) == 0)
        {
          return false;
        }

        return true;
      }
      return !newValue.Equals(oldValue);
    }

    /// <summary>
    /// Compares the specified value1.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="tolerance">The tolerance.</param>
    /// <param name="decimalPlaces">The decimal places.</param>
    /// <returns></returns>
    public static bool Compare(double value1, double value2, double tolerance, int decimalPlaces)
    {
      var difference = Math.Abs(ConvertUtil.Round(value1, decimalPlaces) - ConvertUtil.Round(value2, decimalPlaces));
      return ConvertUtil.Round(difference, decimalPlaces) <= ConvertUtil.Round(tolerance, decimalPlaces);
    }

    /// <summary>
    /// Compares the specified value1.
    /// </summary>
    /// <param name="value1">The value1.</param>
    /// <param name="value2">The value2.</param>
    /// <param name="tolerance">The tolerance.</param>
    /// <param name="decimalPlaces">The decimal places.</param>
    /// <returns></returns>
    public static bool Compare(decimal value1, decimal value2, double tolerance, int decimalPlaces)
    {
      var difference = Math.Abs(ConvertUtil.Round(value1, decimalPlaces) - ConvertUtil.Round(value2, decimalPlaces));
      return ConvertUtil.Round(difference, decimalPlaces) <= ConvertUtil.Round(Convert.ToDecimal(tolerance), decimalPlaces);
    }

    /// <summary>
    /// Used for Rounding Tolerance
    /// </summary>
    /// <param name="value1">value1</param>
    /// <param name="value2">value2</param>
    /// <param name="tolerance">Rounding tolerance</param>
    /// <param name="decimalPlaces">The decimal places.</param>
    /// <returns></returns>
    public static bool Compare(decimal value1, decimal value2, long tolerance, int decimalPlaces)
    {
      var difference = Convert.ToInt64(Math.Abs(value1 - value2));
      return difference <= tolerance;
    }

    /// <summary>
    /// Get clearing house from settlement method
    /// </summary>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    public static string GetClearingHouse(int settlementMethodId)
    {
      string clearingHouse = string.Empty;
      if (settlementMethodId == (int)SMI.Ach)
      {
        clearingHouse = "A";
      }
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      else if (settlementMethodId == (int)SMI.Ich || settlementMethodId == (int)SMI.IchSpecialAgreement || settlementMethodId == (int)SMI.AchUsingIataRules || ReferenceManager.IsSmiLikeBilateral(settlementMethodId, false) || settlementMethodId == (int)SMI.AdjustmentDueToProtest)
      {
        clearingHouse = "I";
      }
      return clearingHouse;
    }

    /// <summary>
    /// Gets the tolerance.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="currencyId">The currency id.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="allowedDecimalPlaces">The allowed decimal places.</param>
    /// <param name="validationExceptionDetail">The validation exception detail.</param>
    /// <param name="exceptionlist">The exceptionlist.</param>
    /// <returns></returns>
    public static Tolerance GetTolerance(BillingCategoryType billingCategory, int currencyId, InvoiceBase invoice, int allowedDecimalPlaces, IsValidationExceptionDetail validationExceptionDetail = null, IList<IsValidationExceptionDetail> exceptionlist = null)
    {
      if (invoice == null)
      {
        throw new ArgumentNullException("invoice");
      }

      var toleranceRepository = Ioc.Resolve<IRepository<Tolerance>>();
      var exchangRateRepository = Ioc.Resolve<IRepository<ExchangeRate>>();

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";
      DateTime billingDate;

      if (invoice.BillingYear == 0 || invoice.BillingMonth == 0)
      {
        return null;
      }

      // To search exchange rate for the billing month.
      var conversionSuccessful = DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString("00"), "01"), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      if (!conversionSuccessful)
      {
        return null;
      }

      var clearingHouse = GetClearingHouse(invoice.SettlementMethodId);

      Tolerance toleranceValueCache;

      var toleranceValue = new Tolerance();
      var effectiveBillingPeriod = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      if (ValidationCache.Instance != null && ValidationCache.Instance.ToleranceList != null)
        toleranceValueCache = ValidationCache.Instance.ToleranceList.Find(rec => rec.IsActive && rec.BillingCategoryId == (int)billingCategory && rec.ClearingHouse == clearingHouse && rec.Type.ToUpper() == "A" && effectiveBillingPeriod >= rec.EffectiveFromPeriod && effectiveBillingPeriod <= rec.EffectiveToPeriod);
      else
        toleranceValueCache = toleranceRepository.Single(rec => rec.IsActive && rec.BillingCategoryId == (int)billingCategory && rec.ClearingHouse == clearingHouse && rec.Type.ToUpper() == "A" && effectiveBillingPeriod >= rec.EffectiveFromPeriod && effectiveBillingPeriod <= rec.EffectiveToPeriod);

      if (toleranceValueCache != null)
      {
        // Get exchange rate query.
        var exchangeRates = exchangRateRepository.Get(rec => rec.IsActive && rec.CurrencyId == currencyId && rec.EffectiveFromDate <= billingDate && rec.EffectiveToDate >= billingDate);

        // Get Five Day exchange rate only.
        var exchangeRateList = exchangeRates.Select(rate => new { rate.FiveDayRateUsd }).ToList();
        if (exchangeRateList.Count > 0)
        {
          var exchangeRate = exchangeRateList[0];
          toleranceValue.SummationTolerance = ConvertUtil.Round(toleranceValueCache.SummationTolerance * exchangeRate.FiveDayRateUsd, allowedDecimalPlaces);
          toleranceValue.RoundingTolerance = ConvertUtil.Round(toleranceValueCache.RoundingTolerance * exchangeRate.FiveDayRateUsd, allowedDecimalPlaces);
        }
        else
        {
          //SCP#140863 : Rounding tolerance issue 
          if (validationExceptionDetail != null && exceptionlist != null)
          {
            validationExceptionDetail.ErrorDescription = string.Format(validationExceptionDetail.ErrorDescription, billingDate.ToString("MMM-yyyy"));
            exceptionlist.Add(validationExceptionDetail);
          }
          Logger.Info("ExchangeRate is null for billing date = " + billingDate.ToString());
        }
      }
      else
      {
        Logger.Info(string.Format("{0}{1}", "Tolerance is NULL for invoice", invoice.InvoiceNumber));
      }
      return toleranceValue;
    }
    
    /// <summary>
    /// Determines whether the specified value is numeric.
    /// </summary>
    /// <param name="literal">The val.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is numeric; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNumeric(string literal)
    {
      Double result;
      return Double.TryParse(literal, out result);
    }

    /// <summary>
    /// Determines whether the specified value is alpha.
    /// </summary>
    /// <param name="literal">The val.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is alpha; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAlpha(string literal)
    {
      var objAlphaPattern = new Regex("^[a-zA-Z][a-zA-Z\\s]+$");
      return objAlphaPattern.IsMatch(literal);
    }

    /// <summary>
    /// Determines whether the specified value is date.
    /// </summary>
    /// <param name="literal">The val.</param>
    /// <returns>
    /// 	<c>true</c> if the specified value is date; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDate(string literal)
    {
      DateTime result;
      return DateTime.TryParse(literal, out result);
    }
  }
}
