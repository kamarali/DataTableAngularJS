using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common
{
  public class BvcMatrix : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the validated pmi.
    /// </summary>
    /// <value>The validated pmi.</value>
    public string ValidatedPmi { get; set; }

    /// <summary>
    /// Gets or sets the effective from.
    /// </summary>
    /// <value>The effective from.</value>
    public string EffectiveFrom { get; set; }

    /// <summary>
    /// Gets or sets the effective to.
    /// </summary>
    /// <value>The effective to.</value>
    public string EffectiveTo { get; set; }

    /// <summary>
    /// Gets or sets the fare amount.
    /// </summary>
    /// <value>The fare amount.</value>
    public bool IsFareAmount { get; set; }

    /// <summary>
    /// Gets or sets the hf amount.
    /// </summary>
    /// <value>The hf amount.</value>
    public bool IsHfAmount { get; set; }

    /// <summary>
    /// Gets or sets the tax amount.
    /// </summary>
    /// <value>The tax amount.</value>
    public bool IsTaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the isc percentage.
    /// </summary>
    /// <value>The isc percentage.</value>
    public bool IsIscPercentage { get; set; }

    /// <summary>
    /// Gets or sets the uatp percentage.
    /// </summary>
    /// <value>The uatp percentage.</value>
    public bool IsUatpPercentage { get; set; }
  }
}
