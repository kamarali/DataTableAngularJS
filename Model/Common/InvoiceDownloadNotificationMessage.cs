using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Common
{
  public class InvoiceDownloadNotificationMessage
  {
    public string RecipientName { get; set; }
    public string DownloadableInvoicePath { get; set; }
  }
}
