using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Data.Pax
{
  public interface IAutoBillingCouponRecordRepository : IRepository<AutoBillingPrimeCoupon>
  {
    /// <summary>
    /// To Update the prime Coupon status and AutoBill primeCoupon status and Is_included in daily report falg.
    /// </summary>
    /// <param name="primeCouponIds"></param>
    /// <param name="autoBillprimeCouponIds"></param>
    void UpdateAutoBillCouponStatus(string primeCouponIds, string autoBillprimeCouponIds);
  }
}
