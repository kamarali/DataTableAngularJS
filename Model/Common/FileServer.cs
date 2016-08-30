using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class FileServer : MasterBase<int>, ICacheable
  {
    public int ServerId { get; set; }
    public string BasePath { get; set; }
    public int Status { get; set; }
    public string ServerType { get; set; }
  }
}
