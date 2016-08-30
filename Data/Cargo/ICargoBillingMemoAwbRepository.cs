using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
    public interface ICargoBillingMemoAwbRepository : IRepository<CargoBillingMemoAwb>
    {
        long GetBMAwbRecordDuplicateCount(int awbSerialNumber,  string awbIssuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth, int awbBillingCode);

        /// <summary>
        /// Loadstrategy method overload of Single
        /// </summary>
        /// <param name="awbRecordId">AwbRecord Id</param>
        /// <returns></returns>
        CargoBillingMemoAwb Single(Guid awbRecordId);

        /// <summary>
        /// Updates the Awb Record Invoice total.
        /// </summary>
        /// <param name="invoiceId">The Invoice id.</param>
        /// <param name="userId">The user id.</param>
        void UpdateBMAwbInvoiceTotal(Guid invoiceId, int userId);

      CargoBillingMemoAwb GetBillingMemoWithAwb(Guid bmAwbId);
    }
}
