namespace Iata.IS.Model.MemberProfile.Enums
{
  /// <summary>
  /// Type of the digital signature service.
  /// </summary>
  public enum DigitalSignature
  {
    /// <summary>
    /// Digital signature not required.
    /// </summary>
    NotRequired = 0,

    /// <summary>
    /// Digital signature required only for incoming invoices.
    /// </summary>
    IncomingInvoices = 1,

    /// <summary>
    /// Digital signature required only for outgoing invoices.
    /// </summary>
    OutgoingInvoices = 2,

    /// <summary>
    /// Required for both incoming and outgoing.
    /// </summary>
    Both = 3
  }
}
