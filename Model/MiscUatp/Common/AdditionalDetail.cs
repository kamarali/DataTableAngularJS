using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Model.MiscUatp.Common
{
  public class AdditionalDetail : MasterBase<string>
  {
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public int TypeId { get; set; }

    /// <summary>
    /// Gets or sets the level id.
    /// </summary>
    /// <value>The level id.</value>
    public int LevelId { get; set; }

    /// <summary>
    /// Gets or sets the type of the additional detail.
    /// </summary>
    /// <value>The type of the additional detail.</value>
    public AdditionalDetailType AdditionalDetailType
    {
      get
      {
        return (AdditionalDetailType)TypeId;
      }
      set
      {
        TypeId = Convert.ToInt32(value);
      }
    }

    /// <summary>
    /// Gets or sets the type of the additional detail.
    /// </summary>
    /// <value>The type of the additional detail.</value>
    public AdditionalDetailLevel AdditionalDetailLevel
    {
      get
      {
        return (AdditionalDetailLevel)LevelId;
      }
      set
      {
        LevelId = Convert.ToInt32(value);
      }
    }
  }
}