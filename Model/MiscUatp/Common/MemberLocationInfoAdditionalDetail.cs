using System;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class MemberLocationInfoAdditionalDetail : AdditionalDetailBase
  {
    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid MemberLocationId { get; set; }

    /// <summary>
    /// Navigational property for <see cref="LineItem"/>
    /// </summary>
    /// <value>The line item.</value>
    public MemberLocationInformation MemberLocationInformation { get; set; }
  }
}