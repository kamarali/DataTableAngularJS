using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.MiscUatp
{
  public class DynamicValidationError : EntityBase<Guid>
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
    /// Gets or sets the line item detail id.
    /// </summary>
    /// <value>The line item detail id.</value>
    public Guid LineItemDetailId { get; set; }

    public ErrorStatus ErrorStatus { get; set; }

    public string RecordId { get; set; }

    public string FieldValue { get; set; }
  }
}