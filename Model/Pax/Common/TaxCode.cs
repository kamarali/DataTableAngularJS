using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Common
{
  [Serializable]
  public class TaxCode : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the type of the tax code.
    /// </summary>
    /// <value>The type of the tax code.</value>
    public TaxCodeType TaxCodeType { get; set; }

    /// <summary>
    /// Gets or sets the tax code type id.
    /// </summary>
    /// <value>The tax code type id.</value>
    public string TaxCodeTypeId { get; set; }
  }
}
