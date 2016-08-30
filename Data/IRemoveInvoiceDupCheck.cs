using System;

namespace Iata.IS.Data
{
  public interface IRemoveInvoiceDupCheck
  {

    /// <summary>
    /// Remove Invoice data from DUP_INVOICE_LOG and DUP_INVOICE_TEMP tables
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    void RemoveDupCheckForInvoice(Guid invoiceId);
  }
}
