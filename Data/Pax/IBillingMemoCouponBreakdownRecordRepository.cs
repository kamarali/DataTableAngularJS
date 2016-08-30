using System;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax
{
  public interface IBillingMemoCouponBreakdownRecordRepository : IRepository<BMCoupon>
  {
      /// <summary>
      /// Get Single Billing Memo Coupon record
      /// </summary>
      /// <param name="billingMemoCouponId">The Billing Memo Coupon Id</param>
      /// <returns>Single record of Billing Memo Coupon</returns>
      BMCoupon Single(Guid billingMemoCouponId);


      /// <summary>
      /// Gets the billing memo coupon duplicate count.
      /// </summary>
      /// <param name="ticketCouponNumber">The ticket coupon number.</param>
      /// <param name="ticketDocNumber">The ticket doc number.</param>
      /// <param name="issuingAirline">The issuing airline.</param>
      /// <param name="billingMemberId">The billing member id.</param>
      /// <param name="billedMemberId">The billed member id.</param>
      /// <param name="billingYear">The billing year.</param>
      /// <param name="billingMonth">The billing month.</param>
      /// <returns></returns>
      long GetBillingMemoCouponDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth);

    /// <summary>
    /// Get Single Billing Memo Coupon record having Billing memo record within it
    /// </summary>
    /// <param name="billingMemoCouponId">The Billing Memo Coupon Id</param>
    /// <returns>Single record of Billing Memo Coupon</returns>
    BMCoupon GetBillingMemoWithCoupon(Guid billingMemoCouponId);
  }
}
