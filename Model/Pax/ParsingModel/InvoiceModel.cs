using Iata.IS.Model.Pax.Sampling;
namespace Iata.IS.Model.Pax.ParsingModel
{
  public class InvoiceModel
  {
    /// <summary>
    /// Gets or sets the invoice.
    /// </summary>
    /// <value>The invoice.</value>
    public PaxInvoice Invoice { get; set; }

    /// <summary>
    /// Gets or sets the sampling form C.
    /// </summary>
    /// <value>The sampling form C.</value>
    public SamplingFormC SamplingFormC { get; set; }
  }
}