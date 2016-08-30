using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax
{
  /// <summary>
  /// This class is used by billingMemoCouponBreakdownRecord, CouponCreditBreakdownRecord, CouponRejectionBreakdownRecord, 
  /// CouponRecord classes
  /// </summary>
  public class CouponBase : EntityBase<Guid>
  {
    public long TicketDocOrFimNumber { get; set; }

    public int TicketOrFimCouponNumber { get; set; }

    public string TicketOrFimIssuingAirline { get; set; }

    public string OriginalPmi { get; set; }

    public string ValidatedPmi { get; set; }

    public string SettlementAuthorizationCode { get; set; }

    public string NfpReasonCode { get; set; }

    public string AgreementIndicatorSupplied { get; set; }

    public string AgreementIndicatorValidated { get; set; }

    public string ISValidationFlag { get; set; }

    public string FromAirportOfCoupon { get; set; }

    public string ToAirportOfCoupon { get; set; }

    //Read-only property added for grid display purpose.
    public string FromToAirport
    {
      get
      {
        return !string.IsNullOrEmpty(FromAirportOfCoupon) || !string.IsNullOrEmpty(ToAirportOfCoupon)
          ? string.Format("{0}-{1}", !string.IsNullOrEmpty(FromAirportOfCoupon) ? FromAirportOfCoupon.ToUpper() : string.Empty, !string.IsNullOrEmpty(ToAirportOfCoupon) ? ToAirportOfCoupon.ToUpper() : string.Empty) : string.Empty;
      }
    }

    public int AttachmentIndicatorOriginal { get; set; }

    public bool? AttachmentIndicatorValidated { get; set; }

    public int? NumberOfAttachments { get; set; }

    public int CheckDigit { get; set; }
    
  }
}
