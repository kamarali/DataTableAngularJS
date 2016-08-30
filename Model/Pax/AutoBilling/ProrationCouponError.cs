namespace Iata.IS.Model.Pax.AutoBilling
{
  public class ProrationCouponError
  {
    public int RecordCount { get; set; }

    public long TicketDocumentNumber { get; set; }

    public string TicketIssuingAirline { get; set; }

    public int TicketDocumentNumberChkDigit { get; set; }

    public string TransmissionControlNumber { get; set; }

    public int TransmissionControlNumberChkDigit { get; set; }

    public string ErrorCode1 { get; set; }

    public string ErrorCode2 { get; set; }

    public string SourceOfData { get; set; }

    public int CpeIndicator { get; set; }

    public int InternalUseData { get; set; }

    public int UnplannedLiftIndicator { get; set; }
  }
}
