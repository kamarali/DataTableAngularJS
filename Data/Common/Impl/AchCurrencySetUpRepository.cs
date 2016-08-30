using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Master;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Data.Common.Impl
{
    public class AchCurrencySetUpRepository :Repository<AchCurrencySetUp>, IAchCurrencySetUpRepository
    {
        /// <summary>
        /// This function is used to get ach currency data based on currency code 
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <param name="getOnlyActiveCurrencies"></param>
        /// <returns></returns>
        //CMP #553: ACH Requirement for Multiple Currency Handling
        public List<AchCurrencySearchData> GetAchCurrencyData(string currencyCode, int getOnlyActiveCurrencies = 0)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter("CURRENCY_CODE_I", typeof(string)) { Value = currencyCode };
            parameters[1] = new ObjectParameter("GET_ONLY_ACTIVE_CURRENCIES_I", typeof(bool)) { Value = getOnlyActiveCurrencies };

            //Execute stored procedure and fetch data based on criteria.
            var achCurrencyData = ExecuteStoredFunction<AchCurrencySearchData>("GetAchCurrencyData", parameters);

            return achCurrencyData.ToList();
        }
    }
}
