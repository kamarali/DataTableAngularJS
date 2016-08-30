using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class CsvUploadQueueMessage
  {
    public string InputFilePath { get; set; }
    public int SenderRecieverId { get; set; }
    public string OutPutCsvFilePath { get; set; }
    public string BillingCategory { get; set; }
    public int BillingPeriod { get; set; }
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }
    public int BillingCategoryId { get; set; }
    public DateTime FileDateTime { get; set; }
    public int FileFormatId { get; set; }
    public long FileSize { get; set; }
  }
}
