using System;

namespace Iata.IS.Model.Pax
{
  public class AutoBillingSearchCriteria
  {
    public int BilledMemberId { set; get; }

    public string BilledMemberText { set; get; }

    public int BillingMemberId { set; get; }
    
    public DateTime? DailyRevenueRecognitionFileDate { get; set; }

    public string InvoiceNumber { get; set; }

    public int SourceCode { get; set; }

    public string ProrateMethodology { get; set; }

    public string TicketIssuingAirline { get; set; }

    public long TicketDocNumber { get; set; }

    public int CouponNumber { get; set; }
  }
}
