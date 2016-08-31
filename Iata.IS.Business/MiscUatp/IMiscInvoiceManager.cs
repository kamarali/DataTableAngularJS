
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;
using System;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Business.MiscUatp
{
    /// <summary>
    /// Miscellaneous Invoice manager interface.
    /// </summary>
  public interface IMiscInvoiceManager : IMiscUatpInvoiceManager
  {
    /// <summary>
    /// Get Billing History Search Result
    /// </summary>
    /// <returns></returns>
    IQueryable<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria, int billingCategoryId);

    IQueryable<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrCriteria, int billingCategoryId);

    /* SCP 250695: Correspondence Invoice raised is in Ready for Billing status and is visible to both the airline on Audit-trail. 
    * Description: Sending memberId to identify if its a payable view/receivable view
    */

    List<MiscUatpInvoice> GetBillingHistoryAuditTrail(string invoiceId, int memberId = 0);

    /// <summary>
    /// Method to check whether LineItem exists for given original Invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail object in which Original invoice exists</param>
    /// <param name="lineItem">Line item to check</param>
    /// <returns>True if Lineitem exixts else false</returns>
    bool ChechWhetherLineItemExistsForOriginalInvoice(AuditTrailPdf auditTrail, LineItem lineItem);

    /// <summary>
    /// Method to check whether LineItem exists for given rejection Invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">Rejection invoice model</param>
    /// <param name="lineItem">Line item to check</param>
    /// <returns>True if Lineitem exixts else false</returns>
    bool ChechWhetherLineItemExistsForRejectionInvoice(MiscUatpInvoice rejectionInvoice, LineItem lineItem);

    /// <summary>
    /// Method to get line item charge amount from Original invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail object in which Original invoice exists</param>
    /// <param name="lineItem">Lineitem model</param>
    /// <returns>line item charge amount</returns>
    decimal GetLineItemChargeAmountFromOriginalInvoice(AuditTrailPdf auditTrail, LineItem lineItem);

    /// <summary>
    /// Method to get line item charge amount from Rejection invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">rejection Invoice object</param>
    /// <param name="lineItem">Lineitem model</param>
    /// <returns>line item charge amount</returns>
    decimal GetLineItemChargeAmountFromRejectionInvoice(MiscUatpInvoice rejectionInvoice, LineItem lineItem);

    /// <summary>
    /// Method to get Ordered LineItem list. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">Invoice in which Lineitem exists</param>
    /// <returns>Ordered Line Item list</returns>
    List<LineItem> GetOrderedLineItemList(MiscUatpInvoice rejectionInvoice);

    /// <summary>
    /// Method to get Correspondence count. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail model in which Correspondence exists</param>
    /// <returns>Correspondence count</returns>
    int GetCorrespondenceCount(AuditTrailPdf auditTrail);

    /// <summary>
    /// Update invoices
    /// </summary>
    /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
    /// <returns></returns>
    //IList<MiscUatpInvoice> UpdateInvoiceTransactionStatuses(List<string> invoiceIdList);

    /// <summary>
    /// Update invoices
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MiscUatpInvoice UpdateInvoiceTransactionStatus(string invoiceId);

    /// <summary>
    /// Update multiple invoices status
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="miscLocationCode">The misc location code.</param>
    void UpdateInvoiceStatus(int billingYear, int billingMonth, int billingPeriod, int billedMemberId, int billingCategory, string miscLocationCode = null);


    /// <summary>
    /// Validates the correspondence time limit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="correspondenceStatusId">The correspondence status id.</param>
    /// <param name="authorityToBill">if set to true [authority to bill].</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <returns>
    /// true if [is valid correspondence time limit] [the specified invoice id]; otherwise, false.
    /// </returns>
    bool IsCorrespondenceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate);

    /// <summary>
    /// Clears transaction data from MISC invoice.
    /// </summary>
    /// <param name="miscUatpInvoice"></param>
    void ClearInvoiceTransationData(MiscUatpInvoice miscUatpInvoice);

    /// <summary>
    /// Returns the Exception codes for the given Billing category
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="billingCategoryTypeId"></param>
    /// <returns></returns>
    string GetExceptionCodeList(string filter, int billingCategoryTypeId);

    /// <summary>
    /// Updates the multiple invoices inclusion status and Generation date.
    /// </summary>
    /// <param name="invoiceIdList">The invoice ids.</param>
    /// <param name="inclusionStatusId"></param>
    /// <param name="isUpdateGenerationDate"></param>
    void UpdateInclusionStatus(List<Guid> invoiceIdList, int inclusionStatusId, bool isUpdateGenerationDate);

  }
}
