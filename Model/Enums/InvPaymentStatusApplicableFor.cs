using System.ComponentModel;

namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Billing types in the system.
  /// </summary>
  public enum InvPaymentStatusApplicableFor
  {
    /// <summary>
    /// Billing Member
    /// </summary>
    [Description("Billing Member")]
    BillingMember = 1,

    /// <summary>
    /// Billed Member
    /// </summary>
    [Description("Billed Member")]
    BilledMember = 2
  }
}
