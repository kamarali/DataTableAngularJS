using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface ICgoRMReasonAcceptableDiffManager
    {
        /// <summary>
        /// Adds the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgo RM reason acceptable diff.</param>
        /// <returns></returns>
        CgoRMReasonAcceptableDiff AddCgoRMReasonAcceptableDiff(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff);

        /// <summary>
        /// Updates the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiff">The cgo RM reason acceptable diff.</param>
        /// <returns></returns>
        CgoRMReasonAcceptableDiff UpdateCgoRMReasonAcceptableDiff(CgoRMReasonAcceptableDiff cgoRMReasonAcceptableDiff);

        /// <summary>
        /// Deletes the cgo RM reason acceptable diff.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiffId">The cgo RM reason acceptable diff id.</param>
        /// <returns></returns>
        bool DeleteCgoRMReasonAcceptableDiff(int cgoRMReasonAcceptableDiffId);

        /// <summary>
        /// Gets the cgo RM reason acceptable diff details.
        /// </summary>
        /// <param name="cgoRMReasonAcceptableDiffId">The cgo RM reason acceptable diff id.</param>
        /// <returns></returns>
        CgoRMReasonAcceptableDiff GetCgoRMReasonAcceptableDiffDetails(int cgoRMReasonAcceptableDiffId);

        /// <summary>
        /// Gets all cgo RM reason acceptable diff list.
        /// </summary>
        /// <returns></returns>
        List<CgoRMReasonAcceptableDiff> GetAllCgoRMReasonAcceptableDiffList();

        /// <summary>
        /// Gets the cgo RM reason acceptable diff list.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <param name="effectiveFrom">The effective from.</param>
        /// <param name="effectiveTo">The effective to.</param>
        /// <returns></returns>
        List<CgoRMReasonAcceptableDiff> GetCgoRMReasonAcceptableDiffList(int reasonCodeId,int TransactionTypeId, string effectiveFrom, string effectiveTo);

    }
}