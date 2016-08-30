using System;
using Iata.IS.Model.Base;


namespace Iata.IS.Model.Common
{
  /// <summary>
  ///  File Watcher Model class to holdes file watcher component properties
  /// </summary>
  public class FileWatcher : EntityBase<int>
  {
    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string SourceId { get; set; }

    public string FileIo { get; set; }

    public string Status { get; set; }

    public DateTime FileModifiedDateTime { get; set; }

  }
}
