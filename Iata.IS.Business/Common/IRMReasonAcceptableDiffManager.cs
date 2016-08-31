using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IRMReasonAcceptableDiffManager
    {

        /// <summary>
        /// Adds the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <returns></returns>
        RMReasonAcceptableDiff AddRMReasonAcceptableDiff(RMReasonAcceptableDiff rmReasonAcceptableDiff);

        /// <summary>
        /// Updates the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiff">The rm reason acceptable diff.</param>
        /// <returns></returns>
        RMReasonAcceptableDiff UpdateRMReasonAcceptableDiff(RMReasonAcceptableDiff rmReasonAcceptableDiff);

        /// <summary>
        /// Deletes the RM reason acceptable diff.
        /// </summary>
        /// <param name="rmReasonAcceptableDiffId">The rm reason acceptable diff id.</param>
        /// <returns></returns>
        bool DeleteRMReasonAcceptableDiff(int rmReasonAcceptableDiffId);

        /// <summary>
        /// Gets the RM reason acceptable diff details.
        /// </summary>
        /// <param name="rmReasonAcceptableDiffId">The rm reason acceptable diff id.</param>
        /// <returns></returns>
        RMReasonAcceptableDiff GetRMReasonAcceptableDiffDetails(int rmReasonAcceptableDiffId);

        /// <summary>
        /// Gets all RM reason acceptable diff list.
        /// </summary>
        /// <returns></returns>
        List<RMReasonAcceptableDiff> GetAllRMReasonAcceptableDiffList();

        /// <summary>
        /// Gets the RM reason acceptable diff list.
        /// </summary>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        List<RMReasonAcceptableDiff> GetRMReasonAcceptableDiffList(int reasonCodeId,int transactionTypeId,string effectiveFrom ,string effectiveTo);
    }
}
