using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// Represents an object that holds the conversion rates for a currency.
  /// </summary>
  [Serializable]
  public class ExchangeRate : MasterBase<int>
  {
    /// <summary>
    /// The numeric id for the currency.
    /// </summary>
    public int CurrencyId { get; set; }

    /// <summary>
    /// Navigation property for currency.
    /// </summary>
    public Currency Currency { get; set; }

    /// <summary>
    /// The date from which this currency conversion rate is effective from.
    /// </summary>
    public DateTime EffectiveFromDate { get; set; }

    /// <summary>
    /// The date till which this currency conversion rate is effective.
    /// </summary>
    public DateTime EffectiveToDate { get; set; }

    /// <summary>
    /// The conversion rate from the currency to USD.
    /// </summary>
    public double FiveDayRateUsd { get; set; }

    /// <summary>
    /// The conversion rate from the currency to GBP.
    /// </summary>
    public double FiveDayRateGbp { get; set; }

    /// <summary>
    /// The conversion rate from the currency to EUR.
    /// </summary>
    public double FiveDayRateEur { get; set; }

    public string CurrencyCode
    {
        get { return this.Currency.Code; }
    }
  }
}