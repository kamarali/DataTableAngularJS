using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Data.MiscUatp
{
    /// <summary>
    /// SCP - 85039:
    /// </summary>
    public interface IManageMiscInvoiceRepository
    {
        List<MiscInvoiceSearchDetails> GetCargoManageInvoices(MiscSearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder);
    }
}
