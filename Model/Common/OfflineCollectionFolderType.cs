using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// represents the offline collection folder type
  /// </summary>
  [Serializable]
  public class OfflineCollectionFolderType:MasterBase<int>, ICacheable
  {
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }
  }
}