using System;
using System.Data.Objects;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Impl
{
  public class RemoveInvoiceDupCheck : Repository<InvoiceBase>, IRemoveInvoiceDupCheck
  {

    /// <summary>
    /// Remove Invoice data from DUP_INVOICE_LOG and DUP_INVOICE_TEMP tables
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    public void  RemoveDupCheckForInvoice(Guid invoiceId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(RemoveInvoiceDupCheckConstants.InvoiceId, invoiceId);
      ExecuteStoredProcedure(RemoveInvoiceDupCheckConstants.RemoveDupCheckOfInvoice, parameters);
    }
  }
}
