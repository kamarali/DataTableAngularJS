using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Business.Common
{
    public interface ICurrencyManager
    {
        /// <summary>
        /// Adds the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        Currency AddCurrency(Currency currency);

        /// <summary>
        /// Updates the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        Currency UpdateCurrency(Currency currency);

        /// <summary>
        /// Deletes the currency.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <returns></returns>
        bool DeleteCurrency(int currencyId);

        /// <summary>
        /// Gets the currency details.
        /// </summary>
        /// <param name="currencyId">The currency id.</param>
        /// <returns></returns>
        Currency GetCurrencyDetails(int currencyId);

        /// <summary>
        /// Gets all currency list.
        /// </summary>
        /// <returns></returns>
        List<Currency> GetAllCurrencyList();

        /// <summary>
        /// Gets the currency list.
        /// </summary>
        /// <param name="Code">The code.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        List<Currency> GetCurrencyList(string Code,string Name);

      /// <summary>
      /// CMP#642: Show Appropriate Currency Decimals in MISC PDFs
      /// Get Listing Currency and Billing Currency Precision.
      /// </summary>
      /// <param name="currencyCode">Listing Currency Code.</param>
      /// <returns>Precision</returns>
      int GetClearanceCurrencyPrecision(string currencyCode);

      /// <summary>
      /// CMP#642: Show Appropriate Currency Decimals in MISC PDFs
      /// Get Listing Currency and Billing Currency Precision.
      /// </summary>
      /// <param name="currencyCode">Listing Currency Code.</param>
      /// <returns>Precision</returns>
      int GetBillingCurrencyPrecision(string currencyCode);
    }
}
