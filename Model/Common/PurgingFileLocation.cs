using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class PurgingFileLocation : MasterBase<int>
  {
    public string ServerName { get; set; }
    public string ServiceName { get; set; }
    public string FileLocation { get; set; }
    public int PurgingFileTypeId { get; set; }
    public PurgingFileType PurgingFileType
    {
      get { return (PurgingFileType)PurgingFileTypeId; }
      set { PurgingFileTypeId = (int)value; }
    }
  }
}
