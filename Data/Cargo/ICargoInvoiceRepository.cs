using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.Common;
//using Iata.IS.Model.Pax;
//using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Cargo.Enums;
//using Iata.IS.Model.Pax.ParsingModel;
//using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Pax;
using SearchCriteria = Iata.IS.Model.Cargo.SearchCriteria;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoInvoiceRepository : IRepositoryEx<CargoInvoice, InvoiceBase>
  {

    ///// <summary>
    ///// Gets the derived vat details for an Invoice.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <returns>List of derived vat details for the Invoice.</returns>
    //IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId);

    ///// <summary>
    ///// Gets the non applied vat details.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <returns>List of non-applied vat details for the Invoice.</returns>
    //IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId);

    ///// <summary>
    ///// Updates the Prime Billing Invoice total.
    ///// </summary>
    ///// <param name="invoiceId">The Invoice id.</param>
    ///// <param name="sourceId">The Source id.</param>
    ///// <param name="userId">The user id.</param>
    //void UpdatePrimeInvoiceTotal(Guid invoiceId, int sourceId, int userId);

    ///// <summary>
    ///// Updates the Rejection memo Invoice total.
    ///// </summary>
    ///// <param name="invoiceId">The Invoice id.</param>
    ///// <param name="sourceId">The Source id.</param>
    ///// <param name="rejectionMemoId">The rejection memo id.</param>
    ///// <param name="userId">The user id.</param>
    ///// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    //void UpdateRMInvoiceTotal(Guid invoiceId, int sourceId, Guid rejectionMemoId, int userId, bool isCouponDelete = false);

    ///// <summary>
    ///// Updates the Billing Memo Invoice total.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <param name="sourceId">The source id.</param>
    ///// <param name="billingMemoId">The Billing Memo id.</param>
    ///// <param name="isCouponDelete"></param>
    //void UpdateBMInvoiceTotal(Guid invoiceId, int sourceId, Guid billingMemoId, int userId, bool isCouponDelete = false);

    ///// <summary>
    ///// Updates the Credit Memo Invoice total.
    ///// </summary>
    ///// <param name="invoiceId">The Invoice id.</param>
    ///// <param name="sourceId">The Source id.</param>
    ///// <param name="CreditMemoId">The Credit Memo id.</param>
    ///// <param name="userId">The user id.</param>
    ///// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    //void UpdateCMInvoiceTotal(Guid invoiceId, int sourceId, Guid CreditMemoId, int userId, bool isCouponDelete = false);

    ///// <summary>
    ///// Determines whether invoice exists for the specified invoice number.
    ///// </summary>
    ///// <param name="yourInvoiceNumber">Your Invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="invoiceStatusId"></param>
    ///// <returns></returns>
    //int IsExistingInvoice(string yourInvoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int invoiceStatusId);


    ///// <summary>
    ///// Determines whether is reference correspondence exists for the specified correspondence number.
    ///// </summary>
    ///// <param name="correspondenceNumber">The correspondence number.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <returns></returns>
    //int IsRefCorrespondenceNumberExists(long correspondenceNumber, int billingMemberId, int billedMemberId);

    ///// <summary>
    ///// Determines whether invoice number exists for given input parameters.
    ///// </summary>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <returns>
    ///// Count of Invoice matched against the input parameters
    ///// </returns>
    //long IsInvoiceNumberExists(string invoiceNumber, int billingYear, int billingMemberId);


    ///// <summary>
    ///// Gets the invoice with RM.
    ///// </summary>
    ///// <param name="where">The where.</param>
    ///// <returns></returns>
    //CargoInvoice GetInvoiceWithCoupons(Expression<Func<CargoInvoice, bool>> where);

    ///// <summary>
    ///// Gets the derived vat details for an Form D/E invoice.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <returns>List of derived vat details.</returns>
    //IList<DerivedVatDetails> GetFormDDerivedVatDetails(Guid invoiceId);

    ///// <summary>
    ///// Gets the non applied vat details for Form D/E invoice.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <returns>List of non-applied vat details.</returns>
    //IList<NonAppliedVatDetails> GetFormDNonAppliedVatDetails(Guid invoiceId);
    /// <summary>
    /// Get all Payables
    /// </summary>
    /// <returns></returns>
    IQueryable<CargoInvoice> GetAllPayables();
    ///// <summary>
    ///// Populates the Invoice object with its child model
    ///// </summary>
    ///// <param name="where"></param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoiceHierarchy(Expression<Func<CargoInvoice, bool>> where);

    //List<CargoInvoice> GetInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, string invoiceStatusIds = null, int? billingCode = null, string invoiceId = null, int? billingMemberId = null);

    ////List<CargoInvoice> GetInvoiceHierarchy(string invoiceId);

    ///// <summary>
    ///// This function is used for Old IDEC output file generation. 
    ///// </summary>
    ///// <param name="billedMemberId"></param>
    ///// <param name="billingPeriod"></param>
    ///// <param name="billingMonth"></param>
    ///// <param name="billingYear"></param>
    ///// <param name="invoiceStatusId"></param>
    ///// <returns></returns>
    //List<CargoInvoice> GetOldIdecInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null, int? billingMonth = null, int? billingYear = null, int? invoiceStatusId = null);

    ///// <summary>
    ///// Gets the invoice using LS.
    ///// </summary>
    ///// <param name="loadStrategy">The load strategy.</param>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <param name="invoiceStatusIds">The invoice status id.</param>
    ///// <param name="couponSearchCriteriaString">To load only selected coupon</param>
    ///// <returns></returns>
    List<CargoInvoice> GetInvoiceLS(LoadStrategy loadStrategy, string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string invoiceId = null, string invoiceStatusIds = null, string couponSearchCriteriaString = null);


    /// <summary>
    /// Gets the Cargo old idec invoice LS.
    /// </summary>
    /// <param name="loadStrategy">The load strategy.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns></returns>
    List<CargoInvoice> GetCargoOldIdecInvoiceLS(LoadStrategy loadStrategy, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? checkValueConfurmation = null);


    //List<CargoInvoice> GetInvoicesLS(SearchCriteria criteria, LoadStrategy loadStrategy);

    ///// <summary>
    ///// Gets the pax old idec invoice LS.
    ///// </summary>
    ///// <param name="loadStrategy">The load strategy.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetPaxOldIdecInvoiceLS(LoadStrategy loadStrategy, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? checkValueConfurmation = null);

    ///// <summary>
    ///// Gets the member location information for a particular invoice.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    ///// <param name="isBillingMember">
    ///// Set to true if Location information for BillingMember is to fetched.
    ///// Otherwise, false.
    ///// </param>
    ///// <returns></returns>
    //List<MemberLocationInformation> GetInvoiceMemberLocationInformation(Guid invoiceId, bool isBillingMember);

    ///// <summary>
    ///// Gets the member location  details in Member location information format.
    ///// </summary>
    ///// <param name="locationId">The location id.</param>
    ///// <returns></returns>
    //List<MemberLocationInformation> GetMemberLocationInformation(int locationId);

    ///// <summary>
    ///// Gets the invoices with coupons.
    ///// </summary>
    ///// <param name="where">The where.</param>
    ///// <returns></returns>
    //IQueryable<CargoInvoice> GetInvoicesWithCoupons(Expression<Func<CargoInvoice, bool>> where);

    ///// <summary>
    ///// Gets the invoices with coupons.
    ///// </summary>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoicesWithCoupons(int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null);

    ///// <summary>
    ///// Gets the invoices with rejection memo record.
    ///// </summary>
    ///// <param name="where">The where.</param>
    ///// <returns></returns>
    //IQueryable<CargoInvoice> GetInvoicesWithRejectionMemoRecord(Expression<Func<CargoInvoice, bool>> where);

    ///// <summary>
    ///// Gets the invoice with rejection memo record.
    ///// </summary>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <param name="id">The id.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoicesWithRejectionMemoRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, string id = null);

    ///// <summary>
    ///// Updates the file log and invoice status depending on Validation Exception details.
    ///// </summary>
    ///// <param name="fileName">Name of the file.</param>
    ///// <param name="billingMemberId"></param>
    ///// <param name="isBadFileExists"></param>
    ///// <param name="processId"></param>
    //List<InvoiceDuplicateRecord> UpdateFileInvoiceStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId);

    ///// <summary>
    ///// To update the senderReceiver and fileProcessStartDate in is_file_log
    ///// </summary>
    ///// <param name="isFileLogId"></param>
    ///// <param name="senderReceiver"></param>
    ///// <param name="fileProcessStartDate"></param>
    //void UpdateIsFileLog(Guid isFileLogId, int senderReceiver, DateTime fileProcessStartDate);

    ///// <summary>
    ///// Gets the invoices with Form D Record
    ///// </summary>
    ///// <param name="where"></param>
    ///// <returns></returns>
    //CargoInvoice GetInvoiceWithFormDRecord(Expression<Func<CargoInvoice, bool>> where);

    ///// <summary>
    ///// Gets the invoices with Form D Record (using Load strategy)
    ///// </summary>
    ///// <param name="invoiceNumber"></param>
    ///// <param name="billingMonth"></param>
    ///// <param name="billingYear"></param>
    ///// <param name="billingPeriod"></param>
    ///// <param name="billingMemberId"></param>
    ///// <param name="billedMemberId"></param>
    ///// <param name="billingCode"></param>
    ///// <returns></returns>
    //CargoInvoice GetInvoiceWithFormDRecord(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null);

    /// <summary>
    /// Get invoice using load strategy to be used in readonly header
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    CargoInvoice GetInvoiceHeader(Guid invoiceId, bool includeBillingBilled = false);

    //IList<PaxBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria, CorrespondenceSearchCriteria corrCriteria);

    //List<PaxBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria);

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
    CargoInvoice Single(string invoiceNumber = null, int? billingMonth = null, int? billingYear = null, int? billingPeriod = null, int? billingMemberId = null, int? billedMemberId = null, int? billingCode = null, Guid? id = null, int? invoiceStatusId = null);

    CargoAuditTrail AuditSingle(Guid transactionId, string transactionType = null);

    ///// <summary>
    ///// Updates the multiple invoice status.
    ///// </summary>
    ///// <param name="invoiceIds"></param>
    ///// <param name="invoiceStatus"></param>
    //void UpdateInvoiceStatus(string invoiceIds, InvoiceStatusType invoiceStatus);


    ///// <summary>
    ///// Get form DE Invoice Details.
    ///// </summary>
    ///// <param name="where"> This will add the condition to linq.</param>
    ///// <returns>Invoice.</returns>
    //CargoInvoice GetFormDEInvoice(Expression<Func<CargoInvoice, bool>> where);
    ///// <summary>
    ///// This method will return list of processed invoice details for given criteria.
    ///// </summary>
    ///// <param name="memberId">ID of member who is creating invoices</param>
    ///// <param name="clearanceMonth">clearance month</param>
    ///// <param name="period">period</param>
    ///// <returns>list of processed invoice details</returns>
    //List<ProcessedInvoiceDetail> GetProcessedInvoiceDetails(int memberId, string clearanceMonth, int period);

    ///// <summary>
    ///// Get the sampling constatnt of the linked form F
    ///// </summary>
    ///// <param name="billingMemberId"></param>
    ///// <param name="billedMemberId"></param>
    ///// <param name="provisionalBillingMonth"></param>
    ///// <param name="provisionalBillingYear"></param>
    ///// <returns></returns>
    //SamplingConstantDetails GetFormFSamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear);


    ///// <summary>
    ///// Gets the invoices with rejection memo record for report.
    ///// </summary>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <param name="id">The id.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoicesWithRejectionMemoRecordForReport(string invoiceNumber = null,
    //                                                             int? billingMonth = null,
    //                                                             int? billingYear = null,
    //                                                             int? billingPeriod = null,
    //                                                             int? billingMemberId = null,
    //                                                             int? billedMemberId = null,
    //                                                             int? billingCode = null,
    //                                                             string id = null);

    ///// <summary>
    ///// Gets the invoices with credit memo record for report.
    ///// </summary>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <param name="id">The id.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoicesWithCreditMemoRecordForReport(string invoiceNumber = null,
    //                                                          int? billingMonth = null,
    //                                                          int? billingYear = null,
    //                                                          int? billingPeriod = null,
    //                                                          int? billingMemberId = null,
    //                                                          int? billedMemberId = null,
    //                                                          int? billingCode = null,
    //                                                          string id = null);


    ///// <summary>
    ///// Gets the invoices with billing memo record for report.
    ///// </summary>
    ///// <param name="invoiceNumber">The invoice number.</param>
    ///// <param name="billingMonth">The billing month.</param>
    ///// <param name="billingYear">The billing year.</param>
    ///// <param name="billingPeriod">The billing period.</param>
    ///// <param name="billingMemberId">The billing member id.</param>
    ///// <param name="billedMemberId">The billed member id.</param>
    ///// <param name="billingCode">The billing code.</param>
    ///// <param name="id">The id.</param>
    ///// <returns></returns>
    //List<CargoInvoice> GetInvoicesWithBillingMemoRecordForReport(string invoiceNumber = null,
    //                                                          int? billingMonth = null,
    //                                                          int? billingYear = null,
    //                                                          int? billingPeriod = null,
    //                                                          int? billingMemberId = null,
    //                                                          int? billedMemberId = null,
    //                                                          int? billingCode = null,
    //                                                          string id = null);


    //List<Transaction> GetRejectedTransactionDetails(string memoId, string couponIds);
    ///// <summary>
    ///// Gets  Pax invoices from InvoiceModelList
    ///// </summary>
    ///// <param name="invoiceModelList"></param>
    ///// <returns></returns>
    //List<CargoInvoice> GetCargoInvoicesFromModel(IEnumerable<InvoiceModel> invoiceModelList);

    ///// <summary>
    ///// Gets  Pax Sampling Form C List from InvoiceModelList
    ///// </summary>
    ///// <param name="invoiceModelList"></param>
    ///// <returns></returns>
    //List<SamplingFormC> GetSamplingFormCListFromModel(IEnumerable<InvoiceModel> invoiceModelList);

    ///// <summary>
    ///// Checks whether invoices are blocked due to some pending processes
    ///// </summary>
    ///// <param name="CargoInvoiceBases"></param>
    ///// <returns></returns>
    //bool ValidateCargoInvoices(IEnumerable<InvoiceBase> CargoInvoiceBases);

    /// <summary>
    /// Validates the rejection memo.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingCode"></param>
    /// <returns></returns>
    string ValidateMemo(Guid invoiceId);

    ///// <summary>
    ///// Gets billing memos created for a correspondence.
    ///// </summary>
    ///// <param name="correspondenceNumber">Correspondence Reference Number</param>
    ///// <returns></returns>
    //List<Transaction> GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId);


    ///// <summary>
    ///// Updates the source code total vat.
    ///// </summary>
    ///// <param name="invoiceId">The invoice id.</param>
    //void UpdateSourceCodeTotalVat(Guid invoiceId);

    /// <summary>
    /// Get Invoice Legal PDF file Path
    /// </summary>
    /// <param name="invoiceId"> Invoice Number </param>
    /// <returns> string of InvoiceLegalPdf</returns>
    string GetInvoiceLegalPdfPath(Guid invoiceId);

    /// <summary>
    /// Finalization of Supporting Document
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    //void FinalizeSupportingDocument(int billingperiod, int billingMonth, int billingYear);

    /// <summary>
    /// Finalization of Supporting Document
    /// </summary>
    /// <param name="billingperiod">billingperiod</param>
    /// <param name="billingMonth">billingMonth</param>
    /// <param name="billingYear">billingYear</param>
    void UpdateInvoiceOnReadyForBilling(Guid invoiceId, int billingCatId, int billingMemberId, int billedMemberId, int billingCodeId);

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
    /// SCP85837: PAX CGO Sequence No
    int IsValidBatchSequenceNo(Guid invoiceId, int batchRecordSequenceNo, int batchSequenceNo,int billing_code, Guid memoId);

    //  /// <summary>
    ///// This method is added to fix issue of IS_BILLING_MEMBER of InvoiceLocationInfo was set to 0 for both entries.
    //  /// </summary>
    //  /// <param name="invoiceId"></param>
    //  /// <param name="isLateSubmission"></param>
    //  /// <param name="dsRequiredBy"></param>
    //  /// <param name="clearingHouse"></param>
    //  /// <param name="sponsoredBy"></param>
    //  /// <param name="isValidBillingPeriod"></param>
    //  void SubmitMiscInvoice(Guid invoiceId, bool isLateSubmission, string dsRequiredBy, string clearingHouse,
    //                         int? sponsoredBy, bool isValidBillingPeriod);


    //  void AddFileLogEntry(IsInputFile isInputFile);

    #region Billing History

    IList<CargoBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceSearchCriteria, CorrespondenceSearchCriteria corrSearchCriteria);

    List<CargoBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria);

    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    //Desc: More details about existing BM are added, hence returned cursor is changed.
    List<CargoExistingBMTransaction> GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId);
    #endregion


    #region Correspondence Trail Report
    /// <summary>
    /// This method get lits of correspondences .Used for Corr Trail Report
    /// </summary>
    /// <param name="correspondenceTrailSearchCriteria"></param>
    /// <returns></returns>
    List<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(
      CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria);
    #endregion


    /// <summary>
    /// This will return list of Cargo invoices along with child objects
    /// </summary>
    /// <param name="billedMemberId">billed Member Id</param>
    /// <param name="billingPeriod">billing Period</param>
    /// <param name="billingMonth">billing Month</param>
    /// <param name="billingYear">billing Year</param>
    /// <param name="invoiceStatusIds">invoiceStatusIds : Comma seperated list of invoice statuses</param>
    /// <param name="billingCode">billing Code</param>
    /// <param name="invoiceId">invoiceId</param>
    /// <param name="billingMemberId">billing Member Id</param>
    /// <returns>list of Cargo invoices along with child objects</returns>
    List<CargoInvoice> GetCargoInvoiceHierarchy(int? billedMemberId = null, int? billingPeriod = null,
                                                int? billingMonth = null, int? billingYear = null,
                                                string invoiceStatusIds = null, int? billingCode = null,
                                                string invoiceId = null, int? billingMemberId = null);


    /// <summary>
    /// Updates the Prime Billing Invoice total.
    /// </summary>
    /// <param name="invoiceId">The Invoice id.</param>
    /// <param name="billingCodeId">The billingCode Id.</param>
    /// <param name="batchSeqNumber">The batchSeq Number.</param>
    /// <param name="reqSeqNumber">The reqSeq Number.</param>
    /// <param name="userId">The user id.</param>
    void UpdateAwbInvoiceTotal(Guid invoiceId, int userId, int billingCodeId, int batchSeqNumber, int reqSeqNumber);

    IList<DerivedVatDetails> GetDerivedVatDetails(Guid invoiceId);

    IList<NonAppliedVatDetails> GetNonAppliedVatDetails(Guid invoiceId);

    List<CargoTransaction> GetRejectedTransactionDetails(string memoId, string couponIds);

    /// <summary>
    /// Updates the cargo BM invoice total.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingCodeId">The billing code id.</param>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isAwbDelete">if set to <c>true</c> [is awb delete].</param>
    void UpdateCargoBMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid billingMemoId, int userId, bool isAwbDelete = false);

    /// <summary>
    /// Update Cargo Invoice file status.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="isBadFileExists"></param>
    /// <param name="processId"></param>
    /// <param name="laFlag"></param>
    void UpdateCargoInvoiceStatus(string fileName, int billingMemberId, bool isBadFileExists, string processId, bool laFlag);

    void UpdateCargoRMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid rejectionMemoId, int userId, bool isAwbDelete = false);

    void UpdateBillingCodeTotalVat(Guid invoiceId);
    /// <summary>
    /// Updates the cargo Credit memo invoice total.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingCodeId">The billing code id.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <param name="userId">The user id.</param>
    /// <param name="isAwbDelete">if set to <c>true</c> [is awb delete].</param>
    void UpdateCargoCMInvoiceTotal(Guid invoiceId, int billingCodeId, Guid creditMemoId, int userId, bool isAwbDelete = false);

    /// <summary>
    /// Executes "PROC_CGO_IS_BAT_SEQ_NO_DUP" stored procedure which will return unique Max Batch and Sequence number
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionTypeId">Transaction type. eg. BM, CM, RM, AWB</param>
    /// <param name="batchNumber">Retrieved Batch number</param>
    /// <param name="sequenceNumber">Retrieved Sequence number</param>
    void GetBatchAndSequenceNumber(Guid invoiceId, int transactionTypeId, out int batchNumber, out int sequenceNumber);

    //void UpdateInvoiceAndSetLaParameters(Guid invoiceId);
    
    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    ///// <summary>
    ///// Updates the expiry date period.
    ///// </summary>
    ///// <param name="transactionId">The transaction id.</param>
    ///// <param name="transactionTypeId">The transaction type id.</param>
    ///// <param name="expiryPeriod">The expiry period.</param>
    //void UpdateExpiryDatePeriod(Guid transactionId, int transactionTypeId, DateTime expiryPeriod);

    /// <summary>
    /// Deletes the RM AWB, re-sequences the breakdown serial numbers of the subsequent AWBs and updates the invoice total.
    /// </summary>
    /// <param name="rmAwbId">The RM AWB id.</param>
    void DeleteRejectionMemoAwb(Guid rmAwbId);

    /// <summary>
    /// Deletes the BM AWB, re-sequences the breakdown serial numbers of the subsequent AWBs and updates the invoice total.
    /// </summary>
    /// <param name="bmAwbId">The BM AWB id.</param>
    void DeleteBillingMemoAwb(Guid bmAwbId);

    /// <summary>
    /// Deletes the CM AWB, re-sequences the breakdown serial numbers of the subsequent AWBs and updates the invoice total.
    /// </summary>
    /// <param name="cmAwbId">The CM AWB id.</param>
    void DeleteCreditMemoAwb(Guid cmAwbId);

    /// <summary>
    /// SCP - 85039:
    /// </summary>
    List<CargoInvoiceSearchDetails> GetCargoManageInvoices(SearchCriteria searchCriteria, int pageSize, int pageNo, string sortColumn, string sortOrder);

    /// <summary>
    /// SCP - 85837 SF-313420- PAX CGO Sequence Number
    /// </summary>
    /// <param name="invoiceId"></param>
    bool UpdateTransSeqNoWithInBatch(Guid invoiceId);

    /// <summary>
    /// This function is used to get linked correspondence rejection memo list.
    /// </summary>
    /// <param name="CorrespondenceRefNo"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    List<CgoLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId);

  }
}
