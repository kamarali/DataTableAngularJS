using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.ParsingModel;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Business.Pax
{
  public interface IOutputIdecGeneratorManager
  {
    /// <summary>
    /// Gets PAX invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria">Search Criteria</param>
    /// <returns>Invoices matching the search criteria.</returns>
    List<PaxInvoice> GetPaxInvoicesForXml(Model.Pax.SearchCriteria searchCriteria);

    /// <summary>
    /// Gets sampling form c list matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns>Sampling form Cs matching the search criteria.</returns>
    List<SamplingFormC> GetSamplingFormCList(Model.Pax.SearchCriteria searchCriteria);

    /// <summary>
    /// Gets all invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <param name="isOutput">if set to <c>true</c> [is output].</param>
    /// <returns></returns>
    InvoiceModelList GetAllInvoicesForIdecAndSamlingFormC(Model.Pax.SearchCriteria searchCriteria, bool isOutput = false);

    /// <summary>
    /// Gets AutoBilling invoices matching the specified search criteria
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <param name="couponSearchCriteriaString">SCP215457: Daily RRF Query. [To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.]</param>
    /// <returns></returns>
    InvoiceModelList GetAutoBillingInvoices(Model.Pax.SearchCriteria searchCriteria, string couponSearchCriteriaString = null);

    /// <summary>
    ///  Gets all invoices matching the specified search criteria for Old Idec
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    InvoiceModelList GetInvoicesForOldIdec(SearchCriteria searchCriteria);

    /// <summary>
    /// Gets data for Consolidated Provisional Invoice file
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    InvoiceModelList GetConsolidatedProvisionalInvoice(SearchCriteria searchCriteria);

    /// <summary>
    /// This function will select and return invoices to write in output file 
    /// UC 3950 : point 0.7
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    InvoiceModelList GetFilteredInvoices(InvoiceModelList invoiceModelList, int billedMemberId);

    /// <summary>
    /// This will return data for Consolidated Provisional Invoice file
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    InvoiceModelList GetConsolidatedProvisionalInvoice(BillingPeriod billingPeriod, int billedMemberId);

    InvoiceModelList SystemMonitorGetConsolidatedProvisionalInvoice(BillingPeriod billingPeriod, int billedMemberId, string invoiceStatusIds);

    InvoiceModelList SystemMonitorGetAllInvoicesForIdecAndSamlingFormC(Model.Pax.SearchCriteria searchCriteria);

    /// <summary>
    /// This will form and return the appropriate error message
    /// </summary>
    /// <param name="paxinvoiceBases">paxinvoiceBases</param>
    /// <returns>error message</returns>
    string GetErrorMessage(IEnumerable<InvoiceBase> paxinvoiceBases);

    void DetachPaxInvoices(List<InvoiceBase> paxinvoiceBases);
  }
}
