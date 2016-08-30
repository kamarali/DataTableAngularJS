namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Billing types in the system.
  /// </summary>
  public enum BillingType
  {
    /// <summary>
    /// Payables - from a billed member's perspective.
    /// </summary>
    Payables = 1,

    /// <summary>
    /// Receivables - from a billing member's perspective.
    /// </summary>
    Receivables = 2
  }
}
