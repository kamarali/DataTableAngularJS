using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Reports
{
  public interface IProcessingDashboardInvoiceStatusRepository
  {
    List<ProcessingDashboardInvoiceStatusResultSet> GetInvoiceStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchcriteria);

    ProcessingDashboardInvoiceDetail GetInvoiceDetailsForProcDashBrd(Guid invoiceId);

    void UpdatePurgingExpiryPeriod(Guid invoiceId);
  }
}
