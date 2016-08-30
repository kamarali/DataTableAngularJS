using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
    /// <summary>
    /// SCP - 85039:
    /// </summary>
    public interface IManageCargoInvoiceRepository
    {
        List<CargoInvoiceSearchDetails> GetCargoManageInvoices(SearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder);
    }
}
