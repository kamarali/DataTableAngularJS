using System;

namespace Iata.IS.Model.Base
{
  [Serializable]
  public class MasterBase<PK> : EntityBase<PK>
  {
    /// <summary>
    /// Gets or sets a value indicating whether this instance is active.
    /// </summary>
    /// <value>true if this instance is active; otherwise, false.</value>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
  }
}