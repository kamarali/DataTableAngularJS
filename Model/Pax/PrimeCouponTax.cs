using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Pax
{
  /// <summary>
  /// To match with data model to create Entity Framework Context object 
  /// </summary>
  public class PrimeCouponTax : Tax
  {
    // Filels requried during AutoBilling ISR.

    public string TicketIssuingAirline { set; get; }

    public long TicketDocumentNumber { set; get; }

    public int CouponNumber { set; get; }

    public string TaxFeeCurrencyCode { set; get; }
  }
}
