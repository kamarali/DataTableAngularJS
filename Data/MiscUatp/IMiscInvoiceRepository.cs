using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;

namespace Iata.IS.Data.MiscUatp
{
  public interface IMiscInvoiceRepository : IRepositoryEx<MiscUatpInvoice, InvoiceBase>
  {
    /// <summary>
    /// Get billling history for invoice
    /// </summary>
    /// <param name="invoiceSearchCriteria">Invoice search criteria</param>
    /// <param name="billingCategoryId">Billing category</param>
    /// <returns>List of billing history</returns>
    List<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceSearchCriteria, int billingCategoryId);

    /// <summary>
    /// Get billling history for correspondence
    /// </summary>
    /// <param name="corrSearchCriteria">Correspodence search criteria</param>
    /// <param name="billingCategoryId">Biling category</param>
    /// <returns>List of billing history</returns>
    List<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrSearchCriteria, int billingCategoryId);

    /// <summary>
    /// This method Gets correspondences fro either Misc or Uatp depending on billing category Id
    /// This is to be used in Corr Trail Report for Misc and Uatp
    /// </summary>
    /// <param name="corrSearchCriteria"></param>
    /// <param name="billingCategoryId"></param>
    /// <returns></returns>
    List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(
      CorrespondenceTrailSearchCriteria corrSearchCriteria, int billingCategoryId);
    /// <summary>
    /// To get matching Misc invoices.
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    List<MiscUatpInvoice> GetMiscInvoices(Expression<Func<MiscUatpInvoice, bool>> where);

    void UpdateInvoiceTotal(Guid invoiceId, Guid lineItemId, int rollupValue);

    /// <summary>
    /// This function is used to update invoice total based on invoice header data.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="totalTaxAmount"></param>
    /// <param name="totalVatAmount"></param>
    /// <param name="totalAddOnAmount"></param>
    //SCP324672: Wrong amount invoice
    void UpdateMUInvoiceSummary(Guid invoiceId, decimal? totalTaxAmount, decimal? totalVatAmount, decimal? totalAddOnAmount);

    void UpdateLineItemNumber(Guid invoiceId, Guid lineItemId, int serialNumber, bool isLineItemNumber);

    IQueryable<MiscAuditTrail> GetBillingHistoryAuditTrail(string invoiceId);

    MiscUatpInvoice GetSingleInvoiceTrail(Expression<Func<MiscUatpInvoice, bool>> where);
    /// <summary>
    /// Gets the line item detail navigation ids for populating First, Previous, Next and Last buttons.
    /// </summary>
    /// <param name="currentLineItemNumber">The current line item number.</param>
    /// <param name="lineItemId">The line item id.</param>
    /// <param name="isOnCreate">The is on create.</param>
    /// <returns></returns>
    IList<NavigationDetails> GetLineItemDetailNavigation(Guid currentLineItemNumber, Guid lineItemId, int isOnCreate);


    /// <summary>
    /// Updates the Misc/UATP file invoice status.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="isBadFileExists"></param>
    /// <param name="processId"></param>
    /// <param name="laFlag"></param>
    void UpdateInvoiceAndFileStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag);

    IQueryable<MiscUatpInvoice> GetAllForPayableSearch();

    /// <summary>
    /// Gets the header details.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    MiscUatpInvoice GetInvoiceHeader(Guid invoiceId, bool includeBillingBilled = false);

    /// <summary>
    /// Load Strategy overload of GetInvoiceHeaderInformation
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MiscUatpInvoice GetLsInvoiceHeaderInformation(Guid invoiceId);

    /// <summary>
    /// Updates the BH invoice.
    /// </summary>
    /// <param name="new_Invoice_Id">The new_ invoice_ id.</param>
    /// <param name="old_Invoice_Id">The old_ invoice_ id.</param>
    /// <param name="lineItemIds">The line item ids.</param>
    /// <param name="rejReasonCode">The rej reason code.</param>
    //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
    void UpdateBHInvoice(Guid new_Invoice_Id, Guid old_Invoice_Id, string lineItemIds,string rejReasonCode);

    /// <summary>
    /// Validates the Misc Uatp invoice location.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    int ValidateMiscUatpInvoiceLocation(Guid invoiceId);

    /// <summary>
    /// Singles the specified results of search criteria.
    /// </summary>
    /// <param name="invoiceId">invoiceId</param>
    /// <param name="invoiceNumber">invoiceNumber</param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId">billedMemberId</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="invoiceStatusId">invoiceStatusId</param>
    /// <param name="billingYear">billingYear</param>
    /// <param name="billingCategoryId"></param>
    /// <param name="rejectionStage"></param>
    /// <returns>MiscUatpInvoice</returns>
    MiscUatpInvoice Single(Guid? invoiceId = null,
                                  string invoiceNumber = null,
                                  int? billingMemberId = null,
                                  int? billedMemberId = null,
                                  int? billingPeriod = null,
                                  int? billingMonth = null,
                                  int? invoiceStatusId = null,
                                  int? billingYear = null,
                                  int? billingCategoryId = null,
                                  int? rejectionStage = null);

    /// <summary>
    /// Gets the M ulinked invoice header.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <returns></returns>
    MiscUatpInvoice GetMUlinkedInvoiceHeader(Guid? invoiceId = null,
                                             string invoiceNumber = null,
                                             int? billingMemberId = null,
                                             int? billedMemberId = null,
                                             int? billingPeriod = null,
                                             int? billingMonth = null,
                                             int? invoiceStatusId = null,
                                             int? billingYear = null,
                                             int? billingCategoryId = null,
                                             int? rejectionStage = null);
    

    /// <summary>
    /// LoadStrategy overload of GetSingleInvoiceTrail method
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    MiscUatpInvoice GetSingleInvoiceTrail(Guid invoiceId);

    /// <summary>
    /// Gets the derived vat details for an Invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details for the Invoice.</returns>
    IList<MiscDerivedVatDetails> GetDerivedVatDetails(Guid invoiceId);

    // CMP #529: Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// This is a loadstrategy method overload of GetMiscInvoices
    /// </summary>
    /// <param name="billedMemberId">billedMemberId</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="invoiceStatusIds">invoiceStatusIds</param>
    /// <param name="billingYear">billingYear</param>
    /// <param name="billingCategoryId">billingCategoryId</param>
    /// <param name="chargeCategoryId">The charge category id.</param>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="inclusionStatus">The inclusion status.</param>
    /// <param name="isWebGenerationDate">The is web generation date.</param>
    /// <param name="submissionMethodId">The submission method id.</param>
    /// <param name="onBehalfTransmitterCode">The on behalf transmitter code.</param>
    /// <param name="dailyDeliveryStatus">The daily delivery status.</param>
    /// <param name="targetDate">The target date.</param>
    /// <param name="outputType">Type of the output.</param>
    /// <returns>
    /// list of MiscUatpInvoice
    /// </returns>
    //CMP#622 : Add output type parameter to select invoices 
    List<MiscUatpInvoice> GetMiscUatpInvoices(int? billedMemberId = null,
                                              int? billingMemberId = null,
                                              int? billingPeriod = null,
                                              int? billingMonth = null,
                                              string invoiceStatusIds = null,
                                              int? billingYear = null,
                                              int? billingCategoryId = null,
                                              int? chargeCategoryId = null,
                                              int? chargeCodeId = null,
                                              string invoiceId = null, int? inclusionStatus = null,
                                              DateTime? isWebGenerationDate = null, int? submissionMethodId = null,
                                              string onBehalfTransmitterCode = null, int? dailyDeliveryStatus = null,
                                              DateTime? targetDate = null, int? outputType = null, string locationId = null);


    //List<MiscUatpInvoice> GetMiscUatpInvoices(string invoiceId);

      /// <summary>
      /// This is a loadstrategy method overload of GetMiscInvoices 
      /// </summary>
      /// <param name="billingMemberId"></param>
      /// <param name="invoiceStatusIds">invoiceStatusId</param>
      /// <param name="billingCategoryId">billingCategoryId</param>
      /// <param name="isWebGenerationDate">isWebGenerationDate</param>
      /// <param name="isReprocessing">isReprocessing</param>
      /// <param name="outputType">outputType</param>
      /// <param name="locationId">locationId</param>
      /// <returns>list of Misc Is Web Invoice</returns>
      //CMP#622: Add output type parameter
     List<MiscUatpInvoice> GetMiscIsWebInvoices(int? billingMemberId = null, string invoiceStatusIds = null,
                                                int? billingCategoryId = null, DateTime? isWebGenerationDate = null,
                                                int? isReprocessing = null, int? outputType = null,
                                                string locationId = null);
    /// <summary>
    /// Gets the invoice for listing report.
    /// </summary>
    /// <param name="invoiceId">String representation of the invoice Guid.</param>
    /// <returns></returns>
    MiscUatpInvoice GetInvoiceToGenerateOfflineCollectionWithISWebAttachments(string invoiceId = null, string invoiceNumber = null);

    /// <summary>
    /// Returns a flag indicating if line item detail is expected within an invoice, but not present.
    /// Also, returns the line item number for which line item detail is expected but not present.
    /// This check is required while validating an invoice.
    /// </summary>
    /// <param name="invoiceId">Id of the invoice to be validated.</param>
    /// <param name="isLineItemDetailExpected">Flag indicating line item detail present or not.</param>
    /// <param name="lineItemNumber">Line Item number for which expected line item detail is not present.</param>
    void IsLineItemDetailExpected(Guid invoiceId,int billingCategoryId, out bool isLineItemDetailExpected, out int lineItemNumber);

    /// <summary>
    /// Deletes a line item with the given id.
    /// </summary>
    /// <param name="lineItemId">Id of the line item to delete.</param>
    void DeleteLineItem(Guid lineItemId);

    /// <summary>
    /// Deletes a line item detail with the given id.
    /// </summary>
    /// <param name="lineItemDetailId">Id of the line item detail to delete.</param>
    void DeleteLineItemDetail(Guid lineItemDetailId);

    /// <summary>
    /// Gets the list of Exception codes for the specified category
    /// Used for Autocomplete textbox
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="billingCategoryTypeId"></param>
    /// <returns></returns>
    string GetExceptionCodeList(string filter, int billingCategoryTypeId);

    /// <summary>
    /// Updates the multiple invoices inclusion status and Generation date.
    /// </summary>
    /// <param name="invoiceIds">The invoice ids.</param>
    /// <param name="inclusionStatusId"></param>
    /// <param name="isUpdateGenerationDate"></param>
    void UpdateInclusionStatus(string invoiceIds, int inclusionStatusId, bool isUpdateGenerationDate);

      //void UpdateInvoiceAndSetLaParameters(Guid invoiceId);

    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod);


    /// <summary>
    /// To generate Atcan report for Receivables
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    void GenerateAtcanStatmentForReceivable(int billingperiod, int billingMonth, int billingYear);

    string ValidateMiscInvoiceBreakDownCaptured(Guid transactionId, int transactionType, decimal totalTaxAmount,
                                                decimal sumTaxBrDown, decimal totalVatAmount,
                                                decimal sumVatBrDown, decimal totalAddOnCharge,
                                                decimal sumAddonChargeBrDown);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    string ValidateMiscInvoiceTotalAndBreakdownAmount(Guid invoiceId);

    /// <summary>
    /// SCP85039
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageNo"></param>
    /// <param name="sortColumn"></param>
    /// <param name="sortOrder"></param>
    /// <returns></returns>
      List<MiscInvoiceSearchDetails> GetMiscManageInvoices(MiscSearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder);

      /// <summary>
      /// CMP288
      /// get invoice type by invoice descriptions
      /// </summary>
      /// <param name="invoiceDesc">invoice description</param>
      /// <returns>invoice type</returns>
      string LookupTemplateType(string invoiceDesc);

      /// <summary>
      /// Update invoice status for duplicate BM
      /// </summary>
      /// <param name="isFileLogId"></param>
      void UpdateInvoiceStatusForDuplicateBM(Guid isFileLogId);

  		/// <summary>
  		/// Update invoice status for duplicate RM
  		/// </summary>
  		/// <param name="isFileLogId"></param>
  		void UpdateInvoiceStatusForDuplicateRM(Guid isFileLogId);

      /// <summary>
      /// This function is used for fetch data from database based on search criteria.
      /// </summary>
      /// <param name="offlineReportSearchCriteria"></param>
      /// <returns></returns>
      //SCP382334: Daily Bilateral screen is not loading
      List<MUDailyPayableResultData> SearchDailyPayableInvoices(MiscSearchCriteria searchCriteria);

      /// <summary>
      ///  SCP#425230 - PAYABLES OPTION
      /// </summary>
      /// <param name="searchCriteria"></param>
      /// <param name="isPayableScreen"></param>
      /// <returns></returns>
      List<MiscInvoiceSearch> SearchMiscInvoiceRecords(MiscSearchCriteria searchCriteria, bool isPayableScreen);

  }
}
