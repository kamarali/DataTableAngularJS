using System;

namespace Iata.IS.Model.MemberProfile.Enums
{
  /// <summary>
  /// Defines the different billing output formats.
  /// </summary>
  [Flags]
  public enum BillingOutputFormats
  {
    /// <summary>
    /// IS-IDEC output format.
    /// </summary>
    ISIdec = 1,

    /// <summary>
    /// IS-XML output format.
    /// </summary>
    ISXml = 2
  }
}
