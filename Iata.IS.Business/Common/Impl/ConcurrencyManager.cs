using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.Data.Common;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
    public class ConcurrencyManager : IConcurrencyManager
    {
        public IConcurrencyData ConcurrencyData  { get; set; }

        /// <summary>
        /// Check invoice is restricted or not.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="correspondenceNo"> Correspondence Number </param>
        /// <param name="correspondenceStage">Correspondence Stage</param>
        /// <returns>
        /// CMP 400 change when invoice deleted. User not able to perform any transaction on it.
        /// 0 = invoice not able to edit.
        /// 1 = invoice can be edit.
        /// 2 = invoice has been deleted.
        /// </returns>
        public int IsInvoiceRestricted(Guid? invoiceId, Guid? transactionId, long? correspondenceNo, int? correspondenceStage, string tableName)
        {
            return ConcurrencyData.IsInvoiceRestricted(invoiceId, transactionId, correspondenceNo, correspondenceStage, tableName);
        }

    }
}
