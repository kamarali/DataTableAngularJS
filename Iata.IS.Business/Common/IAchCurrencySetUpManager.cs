using System.Collections.Generic;
using Iata.IS.Model.Master;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Business.Common
{
    /// <summary>
    /// This interface is used for ACH Currency setup.
    /// CMP #553: ACH Requirement for Multiple Currency Handling.
    /// </summary>
    public interface IAchCurrencySetUpManager
    {
        void AddAchCurrencySetUp(AchCurrencySetUp achCurrencySetUp);

        void DeleteAchCurrencySetUp(int currencyCode);

        List<AchCurrencySearchData> GetAchCurrencySetUpList(string currencyCode, int getOnlyActiveCurrencies = 0);


        /// <summary>
        /// Determines whether [is valid ach currency code] [the specified ach currency code].
        /// </summary>
        /// <param name = "currencyId">The currency id.</param>
        /// <returns>
        /// <c>true</c> if [is valid currency code] [the specified currency code]; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidAchCurrency(int? currencyId);
    }
}
