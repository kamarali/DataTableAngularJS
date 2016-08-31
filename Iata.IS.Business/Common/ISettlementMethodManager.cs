using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ISettlementMethodManager
    {
        IRepository<SettlementMethod> settlementMethodRepository { get; set; }

        /// <summary>
        /// Adds the settlement method
        /// </summary>
        /// <param name="settlementMethod">The settlement method.</param>
        /// <returns></returns>
        SettlementMethod AddSettlementMethod(SettlementMethod settlementMethod);

        /// <summary>
        /// Updates the settlement methodd.
        /// </summary>
        /// <param name="settlementMethod">The settlement method.</param>
        /// <returns></returns>
        SettlementMethod UpdateSettlementMethod(SettlementMethod settlementMethod);

        /// <summary>
        /// Deletes the settlement method.
        /// </summary>
        /// <param name="settlementMethodId">The settlement  method id.</param>
        /// <returns></returns>
        bool DeleteSettlementMethod(int settlementMethodId);

        /// <summary>
        /// Gets the settlement method details.
        /// </summary>
        /// <param name="settlementMethodId">The settlement method id.</param>
        /// <returns></returns>
        SettlementMethod GetSettlementMethodDetails(int settlementMethodId);

        /// <summary>
        /// Gets all settlement method list.
        /// </summary>
        /// <returns></returns>
        List<SettlementMethod> GetAllSettlementMethodList();

        /// <summary>
        /// Gets the settlement method list.
        /// </summary>
        ///<param name="settlementMethodName">Name of the settlement  method</param>
        /// <returns></returns>
        List<SettlementMethod> GetSettlementMethodList(string settlementMethodName);

        string GetSettlementMethodDescription(string settlementMethodName);
    }
}
