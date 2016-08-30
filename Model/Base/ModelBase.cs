using System;

namespace Iata.IS.Model.Base
{
  /// <summary>
  /// Class that acts as an abstract root for all the model classes.
  /// </summary>
  [Serializable]
  public abstract class ModelBase
  {
    /// <summary>
    /// Entity Modification date. Must be UTC date and time.
    /// </summary>
    public DateTime LastUpdatedOn { get; set; }

    /// <summary>
    /// Entity Modification By.
    /// </summary>
    public int LastUpdatedBy { get; set; }
  }
}