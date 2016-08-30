using System;
using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Data.Isr
{
  public interface IValueCouponRequestRepository : IRepository<Record50LiftRequest>
  {
    /// <summary>
    /// To check the existance of requested coupon in database.
    /// </summary>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    int GetValueRequestCouponCount(string ticketDocumentNo, int couponNumber, string issuingAirline,int billingMemberId);

    /// <summary>
    /// To Update the Usage Data.
    /// </summary>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="tktIssuingAirline"></param>
    /// <param name="billingMeberId"></param>
    /// <param name="couponStatusId"></param>
    /// <param name="couponStatusdescription"></param>
    /// <param name="filename"></param>
    /// <param name="responceDate"></param>
    /// <param name="inclideInIrrRepoer"></param>
    /// <returns></returns>
    int UpdateValueRequestCoupon(string ticketDocumentNo, int couponNumber, string tktIssuingAirline, int billingMeberId, int couponStatusId, string couponStatusdescription, string filename, DateTime responceDate, int inclideInIrrRepoer);

    /// <summary>
    /// To check wheather responce file needs to be send to airline or not.
    /// </summary>
    /// <param name="responcefilename"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    bool IsIsrFileUpdationRequried(string responcefilename, int billingMemberId);

    /// <summary>
    /// To validate Isr coupon for Duplicate entry in DB.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="tktIssuingAirline"></param>
    /// <param name="billingmonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="submissionMethosId"></param>
    /// <returns></returns>
    bool IsIsrCouponDuplicate(int billingMemberId, string ticketDocumentNo, int couponNumber, string tktIssuingAirline, int billingmonth, int billingYear, int submissionMethosId);

    /// <summary>
    /// To Update the flags for "Included in Irregularity Report" requested and unrequested Coupons.
    /// </summary>
    /// <param name="unRequestedCouponIds"></param>
    /// <param name="requestedCouponIds"></param>
    void UpdateIncludedInIRReportFlag(string unRequestedCouponIds, string requestedCouponIds);

    /// <summary>
    /// To Update the AutoBillingCoupon Status.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="isBadFileExists"></param>
    void UpdateAutbillingCouponDetails(string fileName, bool isBadFileExists);
  }
}
