using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.ParsingModel;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Pax
{
  public interface IInvoiceRepository : IRepositoryEx<PaxInvoice, InvoiceBase>
  {

    /// <summary>
    /// Gets the derived vat details for an Invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details for the Invoice.</returns>
    IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId);

    /// <summary>
    /// Gets the non applied vat details.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of non-applied vat details for the Invoice.</returns>
    IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId);

    /// <summary>
    /// Updates the Prime Billing Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="sourceId">The Source id.</param>
    /// <param name="userId">The user id.</param>
    void UpdatePrimeInvoiceTotal(Guid invoiceId, int sourceId, int userId);

    /// <summary>
    /// Updates the Rejection memo Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="sourceId">The Source id.</param>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    void UpdateRMInvoiceTotal(Guid invoiceId, int sourceId, Guid rejectionMemoId, int userId, bool isCouponDelete = false);

    void InsertToCorrReport(string filePath);

    /// <summary>
    /// Updates the Billing Memo Invoice total.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="sourceId">The source id.</param>
    /// <param name="billingMemoId">The Billing Memo id.</param>
    /// <param name="isCouponDelete"></param>
    void UpdateBMInvoiceTotal(Guid invoiceId, int sourceId, Guid billingMemoId, int userId, bool isCouponDelete = false);

    /// <summary>
    /// Updates the Credit Memo Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="sourceId">The Source id.</param>
    /// <param name="CreditMemoId">The Credit Memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    void UpdateCMInvoiceTotal(Guid invoiceId, int sourceId, Guid CreditMemoId, int userId, bool isCouponDelete = false);

    /// <summary>
    /// Determines whether invoice exists for the specified invoice number.
    /// </summary>
    /// <param name="yourInvoiceNumber">Your Invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    int IsExistingInvoice(string yourInvoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int invoiceStatusId);


    /// <summary>
    /// Determines whether is reference correspondence exists for the specified correspondence number.
    /// </summary>
    /// <param name="correspondenceNumber">The correspondence number.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <returns></returns>
    int IsRefCorrespondenceNumberExists(long correspondenceNumber, int billingMemberId, int billedMemberId);

    /// <summary>
    /// Determines whether invoice number exists for given input parameters.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns>
    /// Count of Invoice matched against the input parameters
    /// </returns>
    long IsInvoiceNumberExists(string invoiceNumber, int billingYear, int billingMemberId);


    /// <summary>
    /// Gets the invoice with RM.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    PaxInvoice GetInvoiceWithCoupons(Expression<Func<PaxInvoice, bool>> where);

    /// <summary>
    /// Gets if Invoice contains coupons where attachement ind org is "Y"
    /// </summary>
    /// <param name="invoiceId">InvoiceId</param>
    /// <returns>bool</returns>
    bool GetAttachmentIndOrgForInvoice(Guid invoiceId);

    /// <summary>
    /// Gets the derived vat details for an Form D/E invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of derived vat details.</returns>
    IList<DerivedVatDetails> GetFormDDerivedVatDetails(Guid invoiceId);

    /// <summary>
    /// Gets the non applied vat details for Form D/E invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of non-applied vat details.</returns>
    IList<NonAppliedVatDetails> GetFormDNonAppliedVatDetails(Guid invoiceId);
    /// <summary>
    /// Get all Payables
    /// </summary>
    /// <returns></returns>
    IQueryable<PaxInvoice> GetAllPayables();
    /// <summary>
    /// Populates the Invoice object with its child model
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoiceHierarchy(Expression<Func<PaxInvoice, bool>> where);

    /// <summary>
    /// Get Claim Failed settelment Status Invoice(s)
    /// </summary>
    /// <returns></returns>
    List<ProcessingDashboardInvoiceActionStatus> GetClaimFailedInvoices();

    /// <summary>
    /// Get Invoice Hierarchy
    /// </summary>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="invoiceStatusIds">The invoice status ids.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="submissionMethodId">The submission method id.</param>
    /// <param name="couponSearchCriteriaString">SCP215457: Daily RRF Query. To include only those coupons in DRR report that haveing 'INCLUDE_IN_DAILY_REV_RECOGN' flag is set to zero.</param>
    /// <param name="isOutput">if set to <c>true</c> [is output].</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, string invoiceStatusIds = null, int? billingCode = null, string invoiceId = null, int? billingMemberId = null, int? submissionMethodId = null, string couponSearchCriteriaString = null, bool isOutput = false);

    List<PaxInvoice> GetInvoiceHierarchy(string invoiceId);

    //List<PaxInvoice> GetInvoiceHierarchy(string invoiceId);

    /// <summary>
    /// This function is used for Old IDEC output file generation. 
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    List<PaxInvoice> GetOldIdecInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, int? invoiceStatusId = null);

    /// <summary>
    /// Gets the invoice using LS.
    /// </summary>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invoiceStatusIds">The invoice status id.</param>
    /// <param name="couponSearchCriteriaString">To load only selected coupon</param>
    /// <param name="submissionMethodId"></param>
    /// <param name="rejectionMemoNumber">Added the new parameter for SCP51931: File stuck in Production. If value provided then data would be fetched for the provided RM only.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoiceLS(LoadStrategy loadStrategy, string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string invoiceId = null, string invoiceStatusIds = null, string couponSearchCriteriaString = null, int? submissionMethodId = null,string rejectionMemoNumber = null);

    List<PaxInvoice> GetInvoicesLS(SearchCriteria criteria, LoadStrategy loadStrategy);

    /// <summary>
    /// Gets the pax old idec invoice LS.
    /// </summary>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns></returns>
    List<PaxInvoice> GetPaxOldIdecInvoiceLS(LoadStrategy loadStrategy, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? checkValueConfurmation = null);

    /// <summary>
    /// Gets the member location information for a particular invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="isBillingMember">
    /// Set to true if Location information for BillingMember is to fetched.
    /// Otherwise, false.
    /// </param>
    /// <returns></returns>
    List<MemberLocationInformation> GetInvoiceMemberLocationInformation(Guid invoiceId, bool isBillingMember);

    /// <summary>
    /// Gets the member location  details in Member location information format.
    /// </summary>
    /// <param name="locationId">The location id.</param>
    /// <returns></returns>
    List<MemberLocationInformation> GetMemberLocationInformation(int locationId);

    /// <summary>
    /// Gets the invoices with coupons.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    IQueryable<PaxInvoice> GetInvoicesWithCoupons(Expression<Func<PaxInvoice, bool>> where);

    /// <summary>
    /// Gets the invoices with coupons.
    /// </summary>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithCoupons(int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null);


    /// <summary>
    /// Gets the invoices with invoice total record.
    /// </summary>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithTotal(int? billingMonth = null, int? billingYear = null, int? billingPeriod = null,
                                          int? billingMemberId = null, int? billedMemberId = null,
                                          int? billingCode = null);
    

    /// <summary>
    /// Gets the invoices with rejection memo record.
    /// </summary>
    /// <param name="where">The where.</param>
    /// <returns></returns>
    IQueryable<PaxInvoice> GetInvoicesWithRejectionMemoRecord(Expression<Func<PaxInvoice, bool>> where);

    /// <summary>
    /// Gets the invoice with rejection memo record.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithRejectionMemoRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null);

    /// <summary>
    /// Updates the file log and invoice status depending on Validation Exception details.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemberId"></param>
    /// <param name="isBadFileExists"></param>
    /// <param name="processId"></param>
    /// <param name="laFlag"></param>
    void UpdateFileInvoiceStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag);

    /// <summary>
    /// Deletes/truncates the table partitions for PAX Staging tables.
    /// </summary>
    /// <param name="processId"></param>
    void DeleteFileInvoiceStats(string processId);

    /// <summary>
    /// To update the senderReceiver and fileProcessStartDate in is_file_log
    /// </summary>
    /// <param name="isFileLogId"></param>
    /// <param name="senderReceiver"></param>
    /// <param name="fileProcessStartDate"></param>
    /// <param name="fileStatusId"></param>
    /// <param name="lastUpdatedBy"></param>
    void UpdateIsFileLog(Guid isFileLogId, int senderReceiver, DateTime? fileProcessStartDate = null, int fileStatusId = 0, int lastUpdatedBy = 0);

    /// <summary>
    /// Gets the invoices with Form D Record
    /// </summary>
    /// <param name="where"></param>
    /// <returns></returns>
    PaxInvoice GetInvoiceWithFormDRecord(Expression<Func<PaxInvoice, bool>> where);

    /// <summary>
    /// Gets the invoices with Form D Record (using Load strategy)
    /// </summary>
    /// <param name="invoiceNumber"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="billingCode"></param>
    /// <returns></returns>
    PaxInvoice GetInvoiceWithFormDRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null);
    
    //SCP210204: IS-WEB Outage (done Changes to improve performance)
    /// <summary>
    /// Get invoice using load strategy to be used in readonly header
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="includeBillingBilled">if set to <c>true</c> [include billing billed].</param>
    /// <returns></returns>
    PaxInvoice GetInvoiceHeader(Guid invoiceId,bool includeBillingBilled=false);

    IList<PaxBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria, CorrespondenceSearchCriteria corrCriteria, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1);

    List<PaxBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1);

    List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria);

    /// <summary>
    /// Singles the specified invoice number.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <param name="invoiceStatusId"></param>
    /// <returns></returns>
    PaxInvoice Single(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, Guid? id = null, int? invoiceStatusId = null);

    PaxAuditTrail AuditSingle(Guid transactionId, string transactionType = null);

    /// <summary>
    /// Updates the multiple invoice status.
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="miscLocationCode">The misc location code.</param>
    void UpdateInvoiceStatus(int billingYear, int billingMonth, int billingPeriod, int billedMemberId, int billingCategory, string miscLocationCode = null);

    /// <summary>
    /// Get form DE Invoice Details.
    /// </summary>
    /// <param name="where"> This will add the condition to linq.</param>
    /// <returns>Invoice.</returns>
    PaxInvoice GetFormDEInvoice(Expression<Func<PaxInvoice, bool>> where);
    /// <summary>
    /// This method will return list of processed invoice details for given criteria.
    /// </summary>
    /// <param name="memberId">ID of member who is creating invoices</param>
    /// <param name="clearanceMonth">clearance month</param>
    /// <param name="period">period</param>
    /// <returns>list of processed invoice details</returns>
    List<ProcessedInvoiceDetail> GetProcessedInvoiceDetails(int memberId, string clearanceMonth, int period);

    /// <summary>
    /// Get the sampling constatnt of the linked form F
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <returns></returns>
    SamplingConstantDetails GetFormFSamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear);


    /// <summary>
    /// Gets the invoices with rejection memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithRejectionMemoRecordForReport(string invoiceNumber = null,
                                                                 int? billingMonth = null,
                                                                 int? billingYear = null,
                                                                 int? billingPeriod = null,
                                                                 int? billingMemberId = null,
                                                                 int? billedMemberId = null,
                                                                 int? billingCode = null,
                                                                 string id = null);

    /// <summary>
    /// Gets the invoices with credit memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithCreditMemoRecordForReport(string invoiceNumber = null,
                                                              int? billingMonth = null,
                                                              int? billingYear = null,
                                                              int? billingPeriod = null,
                                                              int? billingMemberId = null,
                                                              int? billedMemberId = null,
                                                              int? billingCode = null,
                                                              string id = null);


    /// <summary>
    /// Gets the invoices with billing memo record for report.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    List<PaxInvoice> GetInvoicesWithBillingMemoRecordForReport(string invoiceNumber = null,
                                                              int? billingMonth = null,
                                                              int? billingYear = null,
                                                              int? billingPeriod = null,
                                                              int? billingMemberId = null,
                                                              int? billedMemberId = null,
                                                              int? billingCode = null,
                                                              string id = null);

    /// <summary>
    /// Gets the invoice offline collection data.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <returns></returns>
    PaxInvoice GetInvoiceOfflineCollectionDataWithISWebAttachments(string invoiceId = null, string invoiceNumber = null);

    List<Transaction> GetRejectedTransactionDetails(string memoId, string couponIds);
    /// <summary>
    /// Gets  Pax invoices from InvoiceModelList
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <returns></returns>
    List<PaxInvoice> GetPaxInvoicesFromModel(IEnumerable<InvoiceModel> invoiceModelList);

    /// <summary>
    /// Gets  Pax Sampling Form C List from InvoiceModelList
    /// </summary>
    /// <param name="invoiceModelList"></param>
    /// <returns></returns>
    List<SamplingFormC> GetSamplingFormCListFromModel(IEnumerable<InvoiceModel> invoiceModelList);

    /// <summary>
    /// Checks whether invoices are blocked due to some pending processes
    /// </summary>
    /// <param name="paxInvoiceBases"></param>
    /// <returns></returns>
    bool ValidatePaxInvoices(IEnumerable<InvoiceBase> paxInvoiceBases);

    /// <summary>
    /// Validates the rejection memo.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingCode"></param>
    /// <returns></returns>
    string ValidateMemo(Guid invoiceId, int billingCode);

    /// <summary>
    /// Gets billing memos created for a correspondence.
    /// </summary>
    /// <param name="correspondenceNumber">Correspondence Reference Number</param>
    /// <returns></returns>
    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    //Desc: More details about existing BM are added, hence returned cursor is changed.
    List<ExistingBMTransaction> GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId);


    /// <summary>
    /// Updates the source code total vat.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    void UpdateSourceCodeTotalVat(Guid invoiceId);

    /// <summary>
    /// Get Invoice Legal PDF file Path
    /// </summary>
    /// <param name="invoiceId"> Invoice Number </param>
    /// <returns> string of InvoiceLegalPdf</returns>
    string GetInvoiceLegalPdfPath(Guid invoiceId);


    /// <summary>
    /// This function is used to set invoice status, validation status, is future submission, clearing house, total amount currency. 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="invoiceStatus"></param>
    /// <param name="validationStatus"></param>
    /// <param name="isFutureSubmission"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="totalAmountInCurrency"></param>
    /// <param name="billingCategory"></param>
    /// <param name="exchangeRate"></param>
    void SetInvoiceAndValidationStatus(Guid invoiceId, int invoiceStatus, int validationStatus, bool isFutureSubmission = false, string clearingHouse = "", decimal? totalAmountInCurrency = null, int billingCategory = 0, decimal? exchangeRate = null);

    /// <summary>
    /// Insert Web validation error entry in database
    /// </summary>
    /// <param name="webValidationError"></param>
    void AddWebValiadtionErrorEntry(WebValidationError webValidationError);

      /// <summary>
      /// delete web validation error 
      /// </summary>
      /// <param name="webValidationErrorId"></param>
    void DeleteWebValiadtionError(Guid webValidationErrorId);
      
    /// <summary>
    /// Finalization of Supporting Document
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    /// <param name="createNilFileForMiscLocation">The create nil file for misc location.</param>
    //CMP#622: MISC Outputs Split as per Location ID
    void FinalizeSupportingDocument(int billingperiod, int billingMonth, int billingYear, bool createNilFileForMiscLocation = false);

    //SCP 170146: Proposed improvement in Supporting Doc Linking Finalization Process
    /// <summary>
    /// 
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    void FinalizeSuppDocLinking(int billingYear, int billingMonth, int billingperiod);

    void UpdateInvoiceOnReadyForBilling(Guid invoiceId, int billingCatId, int billingMemberId, int billedMemberId,
                                  int billingCodeId);
    /// <summary>
    /// Determines whether [is valid batch sequence no] [the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="batchRecordSequenceNo">The batch record sequence no.</param>
    /// <param name="batchSequenceNo">The batch sequence no.</param>
    /// <param name="memoId">TransactionId</param>
    /// <returns>
    ///   <c>true</c> if [is valid batch sequence no] [the specified invoice id]; otherwise, <c>false</c>.
    /// </returns>
    int IsValidBatchSequenceNo(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo, Guid memoId, int sourceCodeId);

    /// <summary>
    /// This method is added to fix issue of IS_BILLING_MEMBER of InvoiceLocationInfo was set to 0 for both entries.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="isLateSubmission"></param>
    /// <param name="dsRequiredBy"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="sponsoredBy"></param>
    /// <param name="isValidBillingPeriod"></param>
    void SubmitMiscInvoice(Guid invoiceId, bool isLateSubmission, string dsRequiredBy, string clearingHouse,
                           int? sponsoredBy, bool isValidBillingPeriod);

    /// <summary>
    /// Add file log entry into is_file_log.
    /// </summary>
    /// <param name="isInputFile">file record</param>
    /// <param name="isConsolidatedFile"></param>
    /// <param name="usadaDataExpRespHours"></param>
    void AddFileLogEntry(IsInputFile isInputFile, bool isConsolidatedFile = false, int usadaDataExpRespHours = 0);

    /// <summary>
    /// Method to add/update file processing time to IS_FILE_LOG_DETAIL table for the given file and for the given process name.
    /// </summary>
    /// <param name="isInputFileId"> File Id</param>
    /// <param name="processName">Process Name</param>
    /// <param name="fileName">File Name</param>
    /// <param name="billingCategory">Billing Category</param>
    /// <param name="fileSize">File Size</param>
    /// <param name="invoiceCount">Invoice Count</param>
    /// <param name="primecouponCount">Prime Coupon count</param>
    /// <param name="primeCouponTaxCount">Coupon Tax Breakdown count</param>
    /// <param name="rejectionMemocount">Rejection Memo count</param>
    /// <param name="rmCouponBdnCount">Rejection Memo Coupon Breakdown count</param>
    void PopulateFileProcessingStats(Guid isInputFileId, string processName, string fileName, string billingCategory, string fileSize, int invoiceCount, int primecouponCount,
                                     int primeCouponTaxCount, int rejectionMemocount, int rmCouponBdnCount, int lineItemCnt=0, int lineItemDetCnt=0, int fieldValueCnt=0);
    
    //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    ///// <summary>
    ///// Updates the expiry date period.
    ///// </summary>
    ///// <param name="transactionId">The transaction id.</param>
    ///// <param name="transactionTypeId">The transaction type id.</param>
    ///// <param name="expiryPeriod">The expiry period.</param>
    //void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod);

    //void UpdateInvoiceAndSetLaParameters(Guid invoiceId);


    List<PaxOldIdecBillingMember> GetPaxOldIdecBillingMember(int? billingMemberId, int billingYear, int billingMonth);

    List<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceReportData(int logInMemberid,int entityId, int currencyId,
                                                                                       int clearanceMonth,
                                                                                       int clearanceYear);

    /// <summary>
    /// To update the IsPurged status of IsFileLogId and unlinked supporting documents for Specified Ids.
    /// </summary>
    /// <param name="fileLogIds"></param>
    /// <param name="purgedStatus"></param>
    /// <param name="isFileLogPurged"></param>
    void UpdateFileLogPurgedStatus(string fileLogIds, int purgedStatus, int isFileLogPurged);

    /// <summary>
    /// Deletes the RM coupon, re-sequences the breakdown serial numbers of the subsequent coupons and updates the invoice total.
    /// </summary>
    /// <param name="rmCouponId">The RM coupon id.</param>
    void DeleteRejectionMemoCoupon(Guid rmCouponId);

    /// <summary>
    /// Deletes the BM coupon, re-sequences the breakdown serial numbers of the subsequent coupons and updates the invoice total.
    /// </summary>
    /// <param name="bmCouponId">The BM coupon id.</param>
    void DeleteBillingMemoCoupon(Guid bmCouponId);

    /// <summary>
    /// Deletes the CM coupon, re-sequences the breakdown serial numbers of the subsequent coupons and updates the invoice total.
    /// </summary>
    /// <param name="cmCouponId">The CM coupon id.</param>
    void DeleteCreditMemoCoupon(Guid cmCouponId);

    /// <summary>
    /// Get next sequence number for Auto Billing Invoice
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns>
    /// Invoice Sequence Number
    /// </returns>
     int GetAutoBillingInvoiceNumberSeq(int billingMemberId, int billingYear);

    /// <summary>
    /// Get Invoice Deleteion Audit Details for Invoice Deletion Report
    /// </summary>
    /// <param name="auditDeletedInvoice"></param>
    /// <returns>
    /// List of InvoiceDeletionAuditReport
    /// </returns>
    List<InvoiceDeletionAuditReport> GetInvoiceDeletionAuditDetails(AuditDeletedInvoice auditDeletedInvoice);

    /// <summary>
    /// Update Duplicate Coupon as DU mark. Ref.SCP ID : 94742 - coupons marked as DU duplicate billing
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    void UpdateDuplicateCouponByInvoiceId(Guid invoiceId, int billingMemberId);

    /// <summary>
    /// SCP85837: PAX CGO Sequence Number
    /// </summary>
    /// <param name="invoiceId"></param>
    bool UpdateTransSeqNoWithInBatch(Guid invoiceId);

    /// <summary>
    /// SCP149711 - Incorrect Form E UA to 3M
    /// </summary>
    /// <param name="invoiceId"></param>
    bool RecalculateFormE(Guid invoiceId);

    /// <summary>
    /// Gets the ach invoice count.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="clearanceHouse">The clearance house.</param>
    /// <returns></returns>
    int GetAchInvoiceCount(int billingMemberId, int billingCategoryId, int billingYear, int billingMonth, int billingPeriod, int settlementMethodId, string clearanceHouse);

    //SCP 152109: as discussed
    //Desc: False alert was generated for correspondence to raise BM even when BM was already raised. 
    //Problem identified to be because of future invoices not calling the SP to close the respective correspondences when marked RFB by the Job.
    //Date: 24-July-2013
    void CloseCorrespondenceOnInvoiceReadyForBilling(Guid invoiceId, int billingCatId);

    /// <summary>
    /// Finalize supporting documents for Daily output i.e. update TargetDate, AttachmentIndVal and No.OfAttachment for each invoice to be included in daily output. 
    /// En-queue members who have opted for misc daily output to Daily Output Gen queues.
    /// </summary>
    /// <param name="targetDate"></param>
    /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
    List<InvoicePendingForDailyDelivery> FinalizeSupportingDocumentForDailyOutput(DateTime targetDate);

    /// <summary>
    /// Method will populate report related stats details for given invoiceid.
    /// </summary>
    /// <param name="invoiceId">InvoiceId for which to populate report related stats.</param>
    void PopulateInvoiceReportStats(Guid invoiceId);

    /// <summary>
    /// This function is used to get linked correspondence rejection memo list.
    /// </summary>
    /// <param name="CorrespondenceRefNo"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    List<PaxLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId);

    /// <summary>
    /// This function is used to validate source code for rejection memo using stored procedure.
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    //CMP614: Source Code Validation for PAX RMs.
    String ValidateRMSourceCode(RMSourceCodeValidationCriteria criteria);

    /// <summary>
    /// SCP391025 : Validation and Loader process segregation based on billing category
    /// Enqueue message for CSV to Upload into database, Queue will be chosen based on billing category.
    /// This method will be used to call "PROC_ENQUEUE_LOADER_FILE" stored procedure.
    /// </summary>
    /// <param name="message">message object.</param>
    /// <returns>ture if successfully enqueued/false in case of error</returns>
    bool Enqueue(CsvUploadQueueMessage message, Guid isFileLogId);

    /// <summary>
    /// This function is used to check csv loader, given file has been processed or not
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="senderReceiverId"></param>
    /// <returns></returns>
    //SCP390945: Error in prime file validation - WestJet.
    int CheckCSVLoaderInProcess(string filePath, int senderReceiverId);

      /// <summary>
      /// This function is used to en-queue message for offline collection download
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="billingPeriod"></param>
      /// <param name="billingMonth"></param>
      /// <param name="billingYear"></param>
      /// <param name="invoiceStatusId"></param>
      /// <param name="billingCategory"></param>
      /// <param name="delays"></param>
      void EnqueueOfflineCollectionDownload(int memberId, int billingPeriod, int billingMonth, int billingYear, int invoiceStatusId, string billingCategory, int delays);

      /// <summary>
      /// Method to update FILE_PROCESS_END_TIME in IS_FILE_LOG table.
      /// SCP#432666: SRM: Long time for loading the file
      /// </summary>
      /// <param name="inputIsFileLogId">IS_FILE_LOG_ID of input file</param>
      void UpdateFileProcessEndTimeInIsFileLog(Guid inputIsFileLogId);
  }

}
