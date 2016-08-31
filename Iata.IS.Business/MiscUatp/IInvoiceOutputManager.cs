using System;
using System.Collections.Generic;

namespace Iata.IS.Business.MiscUatp
{
  /// <summary>
  /// Misc and Uatp invoice output interface
  /// </summary>
  public interface IInvoiceOutputManager
  {
    // Download the invoice details and return the base folder name
    string DownloadInvoice(Guid invoiceId, bool isReceivable, List<string> options, string outputZipFileName);

    // En-queue the download request for background processing service to download the invoice.
    bool EnqueueDownloadRequest(IDictionary<string, string> messages);

    // Send the mail to the recipients
    bool SendInvoiceDownloadNotificationEMail(int userId, string outputZipFileName);
  }
}
