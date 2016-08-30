using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Model.Pax.ParsingModel
{
  public class SourceCodeTotalModel
  {
    /// <summary>
    /// Gets or sets the source code total.
    /// </summary>
    /// <value>The source code total.</value>
    public SourceCodeTotal SourceCodeTotal { get; set; }

    /// <summary>
    /// Gets or sets the sampling form C source code total.
    /// </summary>
    /// <value>The sampling form C source code total.</value>
    public SamplingFormCSourceCodeTotal SamplingFormCSourceCodeTotal { get; set; }
  }
}
