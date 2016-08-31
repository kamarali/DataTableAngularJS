using System.Text;
using Iata.IS.Model.MiscUatp;
using log4net;

namespace Iata.IS.Business.Common
{
  /// <summary>
  /// Interface for generating misc listing reports
  /// </summary>
  public interface IMiscListingReportGenerator
  {
    /// <summary>
    /// Creates the misc listing.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="billingCurrencyPrecision">Invoice Billing Currency Precision.</param>
    void CreateMuListing(MiscUatpInvoice miscUatpInvoice, string listingPath, StringBuilder errors, ILog logger, int billingCurrencyPrecision);
  }
}