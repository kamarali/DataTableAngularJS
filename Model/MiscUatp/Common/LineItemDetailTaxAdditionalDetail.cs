using System;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class LineItemDetailTaxAdditionalDetail : AdditionalDetailBase
  {
    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid LineItemDetailTaxId { get; set; }
  }
}