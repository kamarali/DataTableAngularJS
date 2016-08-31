using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public  interface IMinMaxAcceptableAmountManager
    {
        /// <summary>
        /// Adds the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <returns></returns>
        MinMaxAcceptableAmount AddMinMaxAcceptableAmount(MinMaxAcceptableAmount minMaxAcceptableAmount);

        /// <summary>
        /// Updates the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmount">The min max acceptable amount.</param>
        /// <returns></returns>
        MinMaxAcceptableAmount UpdateMinMaxAcceptableAmount(MinMaxAcceptableAmount minMaxAcceptableAmount);

        /// <summary>
        /// Deletes the min max acceptable amount.
        /// </summary>
        /// <param name="minMaxAcceptableAmountId">The min max acceptable amount id.</param>
        /// <returns></returns>
        bool DeleteMinMaxAcceptableAmount(int minMaxAcceptableAmountId);

        /// <summary>
        /// Gets the min max acceptable amount details.
        /// </summary>
        /// <param name="minMaxAcceptableAmountId">The min max acceptable amount id.</param>
        /// <returns></returns>
        MinMaxAcceptableAmount GetMinMaxAcceptableAmountDetails(int minMaxAcceptableAmountId);

        /// <summary>
        /// Gets all min max acceptable amount list.
        /// </summary>
        /// <returns></returns>
        List<MinMaxAcceptableAmount> GetAllMinMaxAcceptableAmountList();

        /// <summary>
        /// Gets the min max acceptable amount list.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        List<MinMaxAcceptableAmount> GetMinMaxAcceptableAmountList(int transactionTypeId, string clearinghouse, double min, double max);
    }
}
