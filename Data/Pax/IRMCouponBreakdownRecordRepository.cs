using System;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.Pax
{
  public interface IRMCouponBreakdownRecordRepository : IRepository<RMCoupon>
  {
    /// <summary>
    /// Gets the RM coupon duplicate count.
    /// </summary>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <param name="ticketIssuingAirline">The ticket issuing airline.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="couponNumber">The coupon number.</param>
    /// <param name="billingmember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="yourInvoiceNumber">Your invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <returns>
    /// Count of records matching the specific input values.
    /// </returns>
    long GetRMCouponDuplicateCount(int rejectionStage, string ticketIssuingAirline, long ticketDocNumber, int couponNumber, int billingmember, int billedMember, string yourInvoiceNumber, int billingMonth, int billingYear);

    
    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="rmCouponId">RMCouponBreakdown Id</param>
    /// <returns>RMCoupon object</returns>
    RMCoupon Single(Guid rmCouponId);    

    /// <summary>
    /// Get RM Coupon linking Details
    /// </summary>
    /// <param name="issuingAirline"></param>
    /// <param name="couponNo"></param>
    /// <param name="ticketDocNo"></param>
    /// <param name="rmId"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    RMLinkedCouponDetails GetRMCouponLinkingDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId);
    /// <summary>
    /// Get Sampling Coupon linking Details.
    /// </summary>
    /// <param name="issuingAirline"> Issuing Airling Number.</param>
    /// <param name="couponNo">Coupon Number.</param>
    /// <param name="ticketDocNo">Ticket Document Number.</param>
    /// <param name="rmId">Rejection memo ID.</param>
    /// <param name="billingMemberId">Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <returns></returns>
    RMLinkedCouponDetails GetSamplingCouponLinkingDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId);

    /// <summary>
    /// Get linking details for rejection memo Coupon when multiple records are found in rejected enity then as per user selection fetch data for selected coupon
    /// </summary>
    /// <param name="couponId"></param>
    /// <param name="rmId"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    RMLinkedCouponDetails GetLinkedCouponAmountDetails(Guid couponId, Guid rmId, int billingMemberId, int billedMemberId);

    /// <summary>
    /// Get linking details for the sampling rejection coupon on the base of selection of specified coupon.
    /// </summary>
    /// <param name="couponId">Coupon Number.</param>
    /// <param name="rmId">Rejection memo ID.</param>
    /// <param name="billingMemberId">Billing Member ID.</param>
    /// <param name="billedMemberId">Billed member ID.</param>
    /// <returns></returns>
    RMLinkedCouponDetails GetSamplingLinkedCouponAmountDetails(Guid couponId, Guid rmId, int billingMemberId, int billedMemberId);

    RMCoupon GetRmCouponWithRejectionMemoObject(Guid rmCouponId);
  }
}
