using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Common
{
    public interface IConcurrencyManager
    {
        /// <summary>
        /// Check invoice is restricted or not.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="correspondenceNo"> Correspondence Number </param>
        /// <param name="correspondenceStage">correspondence Stage </param>
        /// <returns>
        /// CMP 400 change when invoice deleted. User not able to perform any transaction on it.
        /// 0 = invoice not able to edit.
        /// 1 = invoice can be edit.
        /// 2 = invoice has been deleted.
        /// </returns>
        int IsInvoiceRestricted(Guid? invoiceId, Guid? transactionId, long? correspondenceNo, int? correspondenceStage, string tableName);
    }
}
