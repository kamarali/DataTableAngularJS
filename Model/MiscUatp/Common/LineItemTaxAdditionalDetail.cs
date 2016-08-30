using System;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class LineItemTaxAdditionalDetail : AdditionalDetailBase
  {

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid LineItemTaxId { get; set; }
  }
}