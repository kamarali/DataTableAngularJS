using System;
namespace Iata.IS.Model.Pax
{
  /// <summary>
  /// This class is used by billingMemoCouponBreakdownRecord and CouponCreditBreakdownRecord classes
  /// </summary>
  public abstract class MemoCouponBase : CouponBase
  {
    public int SerialNo { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }

    public int? CurrencyId { get; set; }

    public bool ElectronicTicketIndicator { get; set; }

    public string AirlineFlightDesignator { get; set; }

    public int? FlightNumber { get; set; }

    /// <summary>
    /// Format - 00MMYY
    /// </summary>

    public DateTime? FlightDate { get; set; }

    public string CabinClass { get; set; }

    public string ProrateMethodology { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

  }
}
