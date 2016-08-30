using System;
using System.Collections.Generic;

namespace Iata.IS.Model.Pax.Common
{
  public class RMLinkedCouponDetails
  {

    public List<RMLinkedCoupon> RMLinkedCoupons { get; set; }
    public RMCoupon Details { get; set; }
    public string ErrorMessage { get; set; }
    public RMLinkedCouponDetails()
    {
      RMLinkedCoupons = new List<RMLinkedCoupon>();
    }
  }
  public class RMLinkedCoupon
  {
    public int BatchNumber { get; set; }
    public int RecordSeqNumber { get; set; }
    public int BreakdownSerialNo { get; set; }
    public Guid CouponId { get; set; }
  }
}
