using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface ICouponRecordRepository : IRepository<PrimeCoupon>
  {
    PrimeCoupon GetCouponWithVatList(Guid couponRecordId);
    PrimeCoupon GetCouponWithTaxList(Guid couponRecordId);
    PrimeCoupon GetCouponWithAllDetails(Guid couponRecordId);

    /// <summary>
    /// Gets the coupon record duplicate count.
    /// </summary>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="SourceCodeId"></param>
    /// <param name="InvoiceId"></param>
    /// <returns></returns>
    long GetCouponRecordDuplicateCount(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int billingMemberId, int billedMemberId, int billingYear, int billingMonth, int SourceCodeId, Guid InvoiceId);


    /// <summary>
    /// Gets the form D linked coupon details.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <returns></returns>
    List<LinkedCoupon> GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long ticketDocNumber, string issuingAirline);

    /// <summary>
    /// Gets the linked coupons for form C.
    /// </summary>
    /// <param name="ticketCouponNumber">The ticket coupon number.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromBilledMemberId">From billed member id.</param>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <returns></returns>
    List<LinkedCoupon> GetLinkedCouponsForFormC(int ticketCouponNumber, long ticketDocNumber, string issuingAirline, int provisionalBillingMemberId, int fromBilledMemberId, int provisionalBillingMonth, int provisionalBillingYear);

    /// <summary>
    /// Loadstrategy method overload of Single
    /// </summary>
    /// <param name="couponId">coupon Id</param>
    /// <returns>SamplingFormDRecord</returns>
    PrimeCoupon Single(Guid couponId);
  }
}
