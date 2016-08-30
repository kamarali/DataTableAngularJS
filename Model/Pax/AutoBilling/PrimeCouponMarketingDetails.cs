using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class PrimeCouponMarketingDetails : EntityBase<Guid>
  {
    public long TicketDocumentNumber { get; set; }

    public int CouponNumber { get; set; }

    public string TicketIssuingAirline { get; set; }

    public string CouponMarketingCarrier { get; set; }

    public string CouponMarketingFlightNumber { get; set; }

    public string CouponTicketingClassofService { get; set; }

    public string CouponFareBasisTktDesignator { get; set; }

    public PrimeCoupon Coupon { get; set; }

  }
}
