using System;

namespace Iata.IS.Model.Base
{
  /// <summary>
  /// Acts as the base class of all entities.
  /// </summary>
  [Serializable]
  public abstract class EntityBase<PK> : ModelBase
  {
    /// <summary>
    /// Identifier for the entity.
    /// </summary>
    public PK Id { get; set; }
  }
}