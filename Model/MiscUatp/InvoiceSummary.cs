using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp
{
  public class InvoiceSummary : EntityBase<Guid>
  {
    //CMP#648: Clearance Information in MISC Invoice PDFs.
    private decimal? _totalAmountInClearanceCurrency = null;

    /// <summary>
    /// Gets or sets the line item count.
    /// </summary>
    /// <value>The line item count.</value>
    public int LineItemCount { get; set; }

    /// <summary>
    /// Gets or sets the total line item amount.
    /// </summary>
    /// <value>The total line item amount.</value>
    public decimal TotalLineItemAmount { get; set; }

    /// <summary>
    /// Gets or sets the total add on charge amount.
    /// </summary>
    /// <value>The total add on charge amount.</value>
    public decimal? TotalAddOnChargeAmount { get; set; }

    /// <summary>
    /// Gets or sets the total tax amount.
    /// </summary>
    /// <value>The total tax amount.</value>
    public decimal? TotalTaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the total vat amount.
    /// </summary>
    /// <value>The total vat amount.</value>
    public decimal? TotalVatAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount without vat.
    /// </summary>
    /// <value>The total amount without vat.</value>
    public decimal TotalAmountWithoutVat { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    /// <value>The total amount.</value>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount in clearance currency.
    /// </summary>
    /// <value>The total amount in clearance currency.</value>
   //CMP#648: Clearance Information in MISC Invoice PDFs. 
    public decimal? TotalAmountInClearanceCurrency
    {
        get { return _totalAmountInClearanceCurrency; }
        //set { _totalAmountInClearanceCurrency = value.HasValue ? value.Value == 0.0M ? (decimal?)null : value.Value : null; }
        /* SCP 390702 - KAL: Issue with Clearance Amount.
         * Desc: Threating input setter value 0 as is and not as null value. */
        set { _totalAmountInClearanceCurrency = value.HasValue ? value.Value : (decimal?)null; }
    }

    /// <summary>
    /// Gets or sets the attachment count.
    /// </summary>
    /// <value>The attachment count.</value>
    public int AttachmentCount { get; set; }

    /// <summary>
    /// Gets or sets the misc invoice id.
    /// </summary>
    /// <value>The misc invoice id.</value>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Navigational property.
    /// </summary>
    /// <value>The misc invoice.</value>
    public InvoiceBase MiscUatpInvoice { get; set; }

  }
}
