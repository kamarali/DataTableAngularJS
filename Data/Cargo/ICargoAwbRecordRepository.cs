using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
 public interface ICargoAwbRecordRepository: IRepository<AwbRecord>
  {
    //AwbRecord GetCouponWithVatList(Guid couponRecordId);
   // AwbRecord GetCouponWithTaxList(Guid couponRecordId);
   // AwbRecord GetCouponWithAllDetails(Guid couponRecordId);

   /// <summary>
   /// 
   /// </summary>
   /// <param name="awbSerialNumber"></param>
   /// <param name="awbIssueingAirline"></param>
   /// <param name="billingMemberId"></param>
   /// <param name="carriageFromId"></param>
   /// <param name="carriageToId"></param>
   /// <param name="awbDate"></param>
   /// <param name="awbBillingCode"></param>
   /// <returns></returns>
   long GetAwbRecordDuplicateCount(int awbSerialNumber, string awbIssueingAirline, int billingMemberId,
                                   string carriageFromId, string carriageToId, DateTime? awbDate, int awbBillingCode);

    /// <summary>
    /// Loadstrategy method overload of Single
    /// </summary>
    /// <param name="awbRecordId">AwbRecord Id</param>
    /// <returns></returns>
    AwbRecord Single(Guid awbRecordId);

    /// <summary>
    /// Updates the Awb Record Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="userId">The user id.</param>
   void UpdateAwbInvoiceTotal(Guid invoiceId, int userId);

   int GetAwbRecordSeqNumber(int batchSeqNo, string invoiceNo);
  }
}

