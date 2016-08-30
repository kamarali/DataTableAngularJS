using Iata.IS.Model.MiscUatp.Base;

namespace Iata.IS.Model.MiscUatp
{
  public class InvoiceAddOnCharge : AddOnCharge
  {
    /// <summary>
    /// Gets or sets the charge for line item number.
    /// </summary>
    /// <value>The charge for line item number.</value>
    public string ChargeForLineItemNumber { get; set; }

    /// <summary>
    /// Navigation property for <see cref="InvoiceSummary"/>.
    /// </summary>
    /// <value>The invoice summary.</value>
    public InvoiceSummary InvoiceSummary { get; set; }
  }
}