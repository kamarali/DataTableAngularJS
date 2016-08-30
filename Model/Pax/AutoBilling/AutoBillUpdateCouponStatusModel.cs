using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Pax.AutoBilling
{
  public class AutoBillUpdateCouponStatusModel : EntityBase<Guid>
  {
    public int BillingAirlineId { get; set; }

    public long TicketDocumentNumber { get; set; }
    
    public string TicketIssuingAirline { get; set; }

    public int CouponNumber { get; set; }

    public int CouponStatusId { get; set; }

    public string CouponStatusDescription { get; set; }

    public string ResponseFileName { get; set; }

    public DateTime ResponseDate { get; set; }

    public bool IncludedInIrregularityReport { get; set; }
  }
}
