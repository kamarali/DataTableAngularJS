using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Common;

namespace Iata.IS.Model.MiscUatp
{
  public class PaymentDetail : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the type of the payment terms.
    /// </summary>
    /// <value>The type of the payment terms.</value>
    public string PaymentTermsType { get; set; }

    /// <summary>
    /// Gets or sets description for payment terms.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the basis date for payment terms.
    /// </summary>
    /// <value>The date basis.</value>
    public string DateBasis { get; set; }

    /// <summary>
    /// Gets or sets the discount percent.
    /// </summary>
    /// <value>The discount percent.</value>
    public double? DiscountPercent { get; set; }

    /// <summary>
    /// Gets or sets the discount due date.
    /// </summary>
    /// <value>The discount due date.</value>
    public DateTime? DiscountDueDate { get; set; }

    /// <summary>
    /// Gets or sets the discount due days.
    /// </summary>
    /// <value>The discount due days.</value>
    public int? DiscountDueDays { get; set; }

    /// <summary>
    /// Gets or sets the net due date.
    /// </summary>
    /// <value>The net due date.</value>
    public DateTime? NetDueDate { get; set; }

    /// <summary>
    /// Gets or sets Number of days till net due payment term
    /// </summary>
    /// <value>The net due days.</value>
    public int? NetDueDays { get; set; }

    /// <summary>
    /// Gets or sets the parent invoice id.
    /// </summary>
    /// <value>The invoice id.</value>
    public Guid InvoiceId { get; set; }

    /// <summary>
    /// Navigation property of <see cref="MiscUatpInvoice"/>.
    /// </summary>
    /// <value>The misc invoice.</value>
    public InvoiceBase MiscUatpInvoice { get; set; }

    public List<PaymentTermsAdditionalDetail> PaymentTermsAdditionalDetails { get; private set; }

    public PaymentDetail()
    {
      PaymentTermsAdditionalDetails = new List<PaymentTermsAdditionalDetail>();
    }

  }
}