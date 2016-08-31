using System.Linq;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Business.Cargo
{
/// <summary>
/// This interface is specific to invoice search functionality
/// </summary>
  public interface ISearchInvoiceManager
  {
    /// <summary>
    /// Gets invoices matching the specified search criteria
    /// SCP85037: IS Web Performance Feedback - Invoice Search
    /// </summary>
    /// <param name="searchCriteria">Search Criteria</param>
    /// <returns>Invoices matching the search criteria.</returns>
      IQueryable<CargoInvoiceSearchDetails> GetInvoices(SearchCriteria searchCriteria, int pageNo, int pageSize, string sortColumn, string sortOrder);
    /// <summary>
    /// Get all payables invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria">Search Criteria</param>
    /// <returns>Invoices matching the search criteria.</returns>
      IQueryable<CargoInvoiceSearch> GetAllPayables(SearchCriteria searchCriteria);
  }
}
