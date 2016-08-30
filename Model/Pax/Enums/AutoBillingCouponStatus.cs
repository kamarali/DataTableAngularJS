namespace Iata.IS.Model.Pax.Enums
{
  public enum AutoBillingCouponStatus
  {
    /// <summary>
    /// Used when Billed Member is terminated.
    /// </summary>
    MEM = 1,

    /// <summary>
    /// Used for showing Record 50 error.
    /// </summary>
    ERR = 2,

    /// <summary>
    /// Used to validate MonthOfIataFiveDayRate.
    /// </summary>
    RNA = 3,

    /// <summary>
    /// Used when request Type is V.
    /// </summary>
    RER = 4,

    /// <summary>
    /// Used for FlightDate validation.
    /// </summary>
    RNC = 7,

    /// <summary>
    /// Used for New Coupon.
    /// </summary>
    PEN = 6,

    /// <summary>
    /// Used when Coupon is Duplicate.
    /// </summary>
    RNB = 5,

    /// <summary>
    /// Used when Coupon Successfully Validated.
    /// </summary>
    RRB = 8,

    /// <summary>
    /// If no Invoice number is avaliable in the range.
    /// </summary>
    RND = 9,

    /// <summary>
    /// Validation error in the Coupon.
    /// </summary>
    VER = 10
   
  }
}
