using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  /// <summary>
  /// represents IS HTTP Download Link
  /// </summary>
  public class IsHttpDownloadLink : EntityBase<Guid>
  {
    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    /// <value>The file path.</value>
    public string FilePath { get; set; }
  }
}