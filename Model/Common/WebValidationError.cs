using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class WebValidationError : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>The error code.</value>
    public string ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the error description.
    /// </summary>
    /// <value>The error description.</value>
    public string ErrorDescription { get; set; }

    /// <summary>
    /// Gets or sets the invoice id.
    /// </summary>
    /// <value>The invoice id.</value>
    public Guid InvoiceId { get; set; }
  }
}