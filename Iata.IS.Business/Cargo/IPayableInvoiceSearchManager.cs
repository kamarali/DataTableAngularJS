using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Business.Cargo
{
    public interface IPayableInvoiceSearchManager
    {
        /// <summary>
        /// Gets invoices matching the specified search criteria
        /// </summary>
        /// <param name="searchCriteria">Search Criteria</param>
        /// <returns>Invoices matching the search criteria.</returns>
       // IQueryable<CargoInvoiceSearch> GetInvoices(PayableSearch searchCriteria);
        IQueryable<CargoInvoiceSearch> GetInvoices(SearchCriteria searchCriteria);
        
        /// <summary>
        /// Get all payables invoices matching the specified search criteria
        /// </summary>
        /// <param name="searchCriteria">Search Criteria</param>
        /// <returns>Invoices matching the search criteria.</returns>
        //IQueryable<CargoInvoiceSearch> GetAllPayables(PayableSearch searchCriteria);
        IQueryable<CargoInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria);
    }
}
