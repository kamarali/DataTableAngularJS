using System;

namespace Iata.IS.Core
{
  public static class GuidExtensions
  {
    /// <summary>
    /// Returns an unformatted numeric value for a GUID.
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static string Value(this Guid guid)
    {
      return guid.ToString("N");
    }

    /// <summary>
    /// Return Guid for string
    /// </summary>
    /// <param name="guidValue"></param>
    /// <returns></returns>
    public static Guid ToGuid(this string guidValue)
    {
      return Guid.Parse(guidValue);
    }

  }
}