using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Iata.IS.Business.Reports.MiscUatp;
using Iata.IS.Business.Reports.MiscUatp.Impl;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class MiscListingReportGenerator : IMiscListingReportGenerator
  {
    //Fixed issue UCG1040:View and download:Naming Convention for the Listing PDF 
    private const string MiscBillingCategory = "M";
    private const string UatpBillingCategory = "U";

    #region Implementation of IMiscListingReportGenerator

    /// <summary>
    /// Creates the misc listing.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="listingPath">The listing path.</param>
    /// <param name="errors">The errors.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="billingCurrencyPrecision">Invoice Billing Currency Precision.</param>
   public void CreateMuListing(MiscUatpInvoice miscUatpInvoice, string listingPath, StringBuilder errors, ILog logger, int billingCurrencyPrecision)
    {
      // Create ordered collection of LineItem
      logger.InfoFormat("Creating ordered collection of LineItem...");
      var orderedLineItems = from li in miscUatpInvoice.LineItems
                             orderby li.LineItemNumber ascending
                             select li;
      logger.InfoFormat("Creating MiscUatpReportManager object...");
      IMiscUatpReportManager miscUatpReportManager = new MiscUatpReportManager();
      logger.InfoFormat("Generating Misc Listing Report...");
      miscUatpReportManager.BuildListingReport(miscUatpInvoice,
                                               orderedLineItems,
                                               Path.Combine(listingPath, string.Format("{0}DETLST-{1}.PDF", miscUatpInvoice.BillingCategory == BillingCategoryType.Misc ? MiscBillingCategory : UatpBillingCategory, miscUatpInvoice.InvoiceNumber)).ToUpper(), billingCurrencyPrecision);
      logger.InfoFormat("Misc Listing report generated.");
      orderedLineItems = null;
    }

    #endregion
  }
}
