using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IMaxAcceptableAmountManager
    {
        //IMaxAcceptableAmountRepository MaxAcceptableAmountRepository { get; set; }

        /// <summary>
        /// Add maximum acceptable ammount
        /// </summary>
        /// <param name="maxAcceptableAmount">Object of MaxAcceptableAmount</param>
        /// <returns></returns>
        MaxAcceptableAmount AddMaxAcceptableAmount(MaxAcceptableAmount maxAcceptableAmount);

        /// <summary>
        /// Update maximum acceptable ammount
        /// </summary>
        /// <param name="maxAcceptableAmount">Object of MaxAcceptableAmount</param>
        /// <returns></returns>
        MaxAcceptableAmount UpdateMaxAcceptableAmount(MaxAcceptableAmount maxAcceptableAmount);

        /// <summary>
        /// Delete maximum acceptable amount
        /// </summary>
        /// <param name="maxAcceptableAmountId">The max acceptable amount Id</param>
        /// <returns></returns>
        bool DeleteMaxAcceptableAmount(int maxAcceptableAmountId);

        /// <summary>
        /// Gets the max acceptable amount details.
        /// </summary>
        /// <param name="maxAcceptableAmountId">The max acceptable amount id.</param>
        /// <returns></returns>
        MaxAcceptableAmount GetMaxAcceptableAmountDetails(int maxAcceptableAmountId);

        /// <summary>
        /// Gets all max acceptable amount list.
        /// </summary>
        /// <returns></returns>
        List<MaxAcceptableAmount> GetAllMaxAcceptableAmountList();

        /// <summary>
        /// Gets the max acceptable amount list.
        /// </summary>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To  period</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        List<MaxAcceptableAmount> GetMaxAcceptableAmountList(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int transactionTypeId, string clearinghouse, double max);
    }
}
