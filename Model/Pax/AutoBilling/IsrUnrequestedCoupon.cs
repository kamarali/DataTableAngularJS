using System;
using Iata.IS.Core;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Common;
using System.ComponentModel;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class IsrUnrequestedCoupon : EntityBase<Guid>
  {
    [DisplayName("Issuing Airline")]
    public string TicketIssuingAirline { get; set; }

    [DisplayName("Document Number")]
    public long TicketDocumentNumber { get; set; }

    [DisplayName("Coupon Number")]
    public int? CouponNumber { get; set; }

    public int BillingMemberId { get; set; }

    [DisplayName("Prorate Results Filename")]
    public string ResponseFileName { get; set; }

    public DateTime ResponceDate { get; set; }

    public bool IsRequriedInDailyAutoBilling { get; set; }

    public Guid IsInputFileId { get; set; }

    public IsInputFile IsInputFile { get; set; }

    public string IsInputFileDisplayId
    {
      get
      {
        return IsInputFileId.Value();
      }
    }
  }
}
