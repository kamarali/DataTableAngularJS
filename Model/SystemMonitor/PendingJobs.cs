using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.SystemMonitor
{
  public class PendingJobs
  {

    public Guid FileId { get; set; }

    public string FileName { get; set; }

    public DateTime FileTime { get; set; }

    public string FormatedFileTime
    {
      get
      {
        string formatedDate = string.Empty;
        formatedDate = FileTime.ToString("dd MMM yyyy HH:mm");
        return formatedDate;
      }
    }

    public string Status { get; set; }

    //CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Added new column
    public int FileProgressStatus { get; set; }

   }
}
