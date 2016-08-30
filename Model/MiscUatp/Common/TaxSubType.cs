using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.Common
{
  [Serializable]
  public class TaxSubType : MasterBase<int>
  {
    /// <summary>
    /// Gets or sets the type of the sub.
    /// </summary>
    /// <value>The type of the sub.</value>
    public string SubType { get; set; }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string Type { get; set; }
  }
}
