using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common
{
  public class Rfisc : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the rfic id.
    /// </summary>
    /// <value>The rfic id.</value>
    public string RficId { get; set; }

    /// <summary>
    /// Gets or sets the rfic.
    /// </summary>
    /// <value>The rfic.</value>
    public Rfic Rfic { get; set; }

    /// <summary>
    /// Gets or sets the name of the group.
    /// </summary>
    /// <value>The name of the group.</value>
    public string GroupName { get; set; }

    /// <summary>
    /// Gets or sets the name of the commercial.
    /// </summary>
    /// <value>The name of the commercial.</value>
    public string CommercialName { get; set; }
  }
}
