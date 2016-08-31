using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Pax
{
    /// <summary>
    /// This interface is specific to invoice search functionality
    /// </summary>
    public interface ISearchInvoiceManager
    {
        /// <summary>
        ///Gets invoices matching the specified search criteria 
        ///SCP - 85037: Return type changed to PaxInvoiceSearchDetails from PaxInvoiceSearch
        /// </summary>
        /// <param name="searchCriteria">Search Criteria</param>
        /// <param name="currentPage"></param>
        /// <param name="noOfRecords"></param>
        /// <returns>Invoices matching the search criteria.</returns>
        List<PaxInvoiceSearchDetails> GetInvoices(SearchCriteria searchCriteria, int currentPage, int noOfRecords, string sortColumn, string sortOrder);
        /// <summary>
        /// Get all payables invoices matching the specified search criteria
        /// </summary>
        /// <param name="searchCriteria">Search Criteria</param>
        /// <returns>Invoices matching the search criteria.</returns>
        IQueryable<PaxInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria);
    }
}
