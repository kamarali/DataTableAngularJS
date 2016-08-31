using System;
using System.Collections.Generic;
using Iata.IS.Data.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IMinAcceptableAmountManager
    {
        //IMinAcceptableAmountRepository MinAcceptableAmountRepository { get; set; }

        //IReasonCodeRepository RejectionReasonCodeRepository { get; set; }
        /// <summary>
        /// Add minimum acceptable ammount
        /// </summary>
        /// <param name="minAcceptableAmount">Object of MinimumAcceptableAmount</param>
        /// <returns></returns>
        MinAcceptableAmount AddMinAcceptableAmount(MinAcceptableAmount minAcceptableAmount);

        /// <summary>
        /// Update minimum acceptable ammount
        /// </summary>
        /// <param name="minAcceptableAmount">Object of MinimumAcceptableAmount</param>
        /// <returns></returns>
        MinAcceptableAmount UpdateMinAcceptableAmount(MinAcceptableAmount minAcceptableAmount);

        /// <summary>
        /// Delete minimum acceptable amount
        /// </summary>
        /// <param name="minAcceptableAmountId">The min acceptable amount Id</param>
        /// <returns></returns>
        bool DeleteMinAcceptableAmount(int minAcceptableAmountId);

        /// <summary>
        /// Gets the min acceptable amount details.
        /// </summary>
        /// <param name="minAcceptableAmountId">The min acceptable amount id.</param>
        /// <returns></returns>
        MinAcceptableAmount GetMinAcceptableAmountDetails(int minAcceptableAmountId);

        /// <summary>
        /// Gets all min acceptable amount list.
        /// </summary>
        /// <returns></returns>
        List<MinAcceptableAmount> GetAllMinAcceptableAmountList();

        /// <summary>
        /// Gets the min acceptable amount list.
        /// </summary>
        /// <param name="applicableField">Applicable Amount Field</param>
        /// <param name="effectiveFromPeriod">Effective From Period</param>
        /// <param name="effectiveToPeriod">Effective To Period</param>
        /// <param name="reasonCode">Rejection Reasoncode </param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <param name="clearinghouse">The clearinghouse.</param>
        /// <param name="amount">The minimum amount.</param>
        /// <returns></returns>
        List<MinAcceptableAmount> GetMinAcceptableAmountList(DateTime effectiveFromPeriod, DateTime effectiveToPeriod, int applicableField, string reasonCode, int transactionTypeId, string clearinghouse, double amount);

        /// <summary>
        /// Get the list of rejection reasoncode list
        /// </summary>
        /// <param name="transactionTypeId"></param>
        /// <returns></returns>
        List<ReasonCode> GetRejectionReasonCodeList(int transactionTypeId);
    }
}
