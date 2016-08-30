using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.UIModel.FileViaWeb
{
  public class DownloadView
  {
    public DateTime StatusDateFrom;
    public DateTime StatusDateTo;
    public int FileStatus;
    public DateTime BillingPeriodFrom;
    public DateTime BillingPeriodTo;
  }
}