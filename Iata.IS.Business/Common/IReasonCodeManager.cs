using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Common
{
    public interface IReasonCodeManager
    {
        /// <summary>
        /// Adds the reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns></returns>
        ReasonCode AddReasonCode(ReasonCode reasonCode);

        /// <summary>
        /// Updates the reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <returns></returns>
        ReasonCode UpdateReasonCode(ReasonCode reasonCode);

        /// <summary>
        /// Deletes the reason code.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <returns></returns>
        bool DeleteReasonCode(int reasonCodeId);

        /// <summary>
        /// Gets the reason code details.
        /// </summary>
        /// <param name="reasonCodeId">The reason code id.</param>
        /// <returns></returns>
        ReasonCode GetReasonCodeDetails(int reasonCodeId);

        /// <summary>
        /// Gets all reason code list.
        /// </summary>
        /// <returns></returns>
        List<ReasonCode> GetAllReasonCodeList();

        /// <summary>
        /// Gets the reason code list.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        List<ReasonCode> GetReasonCodeList(string code, int transactionTypeId);

        /// <summary>
        /// Gets the rejection reason code list.
        /// </summary>
        /// <param name="transactionTypeId">The transaction type id.</param>
        /// <returns></returns>
        List<ReasonCode> GetRejectionReasonCodeList(int transactionTypeId);
    }
}
