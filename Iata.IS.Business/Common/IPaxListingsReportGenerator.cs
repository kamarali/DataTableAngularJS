using System.Collections.Generic;
using System.Text;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using log4net;

namespace Iata.IS.Business.Common
{
  public interface IPaxListingsReportGenerator
  {
    /// <summary>
    /// Creates the listings.
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="paxInvoiceCoupons"></param>
    /// <returns></returns>
    void CreatePaxListings(PaxInvoice paxInvoice, string listingPath, StringBuilder errors, ILog logger, IList<PrimeCoupon> paxInvoiceCoupons);

    /// <summary>
    /// Create the invoice details pdf
    /// </summary>
    /// <param name="invoiceId">invoice id</param>
    /// <param name="listingPath">the listing path</param>
    /// <param name="errors">the errors</param>
    /// <param name="logger">the loggers</param>
    /// <param name="paxInvoiceCoupons"></param>
    void CreatePaxDetailsPdf(string invoiceId, string listingPath, StringBuilder errors, ILog logger, PaxInvoice paxInvoice);

    /// <summary>
    /// Creates the form C listing.
    /// </summary>
    /// <param name="samplingFormCs">The List of sampling form C grouped by Billing Member & Billed Member.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <returns></returns>
    void CreateFormCListing(List<SamplingFormC> samplingFormCs, string listingPath, StringBuilder errors, ILog logger);

    
  }
}