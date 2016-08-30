using System.Collections.Generic;
using Iata.IS.Model.Master;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Data.Common
{
   public interface IAchCurrencySetUpRepository : IRepository<AchCurrencySetUp>
   {
       /// <summary>
       /// This function is used to get ach currency data based on currency code 
       /// </summary>
       /// <param name="currencyCode"></param>
       /// <param name="getOnlyActiveCurrencies"></param>
       /// <returns></returns>
       //CMP #553: ACH Requirement for Multiple Currency Handling
       List<AchCurrencySearchData> GetAchCurrencyData(string currencyCode, int getOnlyActiveCurrencies = 0);
   }
}
