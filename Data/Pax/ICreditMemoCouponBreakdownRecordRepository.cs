using System;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax
{
  public interface ICreditMemoCouponBreakdownRecordRepository : IRepository<CMCoupon>
  {
    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="cmCouponId">CM coupon breakdown id</param>
    /// <returns>CMCoupon</returns>
    CMCoupon Single(Guid cmCouponId);



    /// <summary>
    /// Gets the credit memo coupon duplicate count.
    /// </summary>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <returns></returns>
    long GetCreditMemoCouponDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth);

    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="cmCouponId">CMCouponBreakdown Id</param>
    /// <returns>CMCoupon object</returns>
    CMCoupon GetCmCouponWithCreditMemoObject(Guid cmCouponId);

  }
}
