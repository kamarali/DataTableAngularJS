using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class MiscUatpInvoiceTaxAdditionalDetail : AdditionalDetailBase
  {

    /// <summary>
    /// Gets or sets the parent id.
    /// </summary>
    /// <value>The parent id.</value>
    public Guid MiscInvoiceTaxId { get; set; }

    /// <summary>
    /// Navigational property for <see cref="LineItem"/>
    /// </summary>
    /// <value>The line item.</value>
    public MiscUatpInvoiceTax MiscUatpInvoiceTax { get; set; }
  }
}