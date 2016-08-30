using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class DailyOutputFileDownloadSearch
  {
    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public DateTime? DeliveryDateFrom { get; set; }

    public DateTime? DeliveryDateTo { get; set; }

    public int FileFormatId { set; get; }

    //CMP#622: MISC Outputs Split as per Location ID
    public string MiscLocCode { get; set; }
  }
}
