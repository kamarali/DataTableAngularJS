namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Indicates whether a digital signature is required by a member.
  /// </summary>
  public enum DigitalSignatureRequired
  {
    /// <summary>
    /// Indicates that an invoice must have a digital signature.
    /// </summary>
    Yes = 1,

    /// <summary>
    /// Indicates that an invoice should not have a digital signature.
    /// </summary>
    No = 2,
    /// <summary>
    /// Default
    /// </summary>
    Default = 3
  };
}