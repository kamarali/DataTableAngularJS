using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax
{
    /// <summary>
    /// SCP - 85037: Provides method to get data of pax invoices based on parameters 
    /// </summary>
    public interface IManagePaxInvoiceRepository
    {
        List<PaxInvoiceSearchDetails> GetPaxManageInvoices(SearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder);
    }
}
