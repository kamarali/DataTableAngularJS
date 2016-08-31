using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Calendar;
using System;
using Iata.IS.Model.Pax.Sampling;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Business.Pax
{
  /// <summary>
  /// Responsible for invoice search, validating and submitting invoices.
  /// Same for sampling and non-sampling invoices, credit notes (credit notes are a type of invoice).
  /// </summary>
  public interface IInvoiceManager
  {
    /// <summary>
    /// Creates an invoice header
    /// </summary>
    /// <param name="invoiceHeader">Invoice Header to be created</param>
    /// <returns>Created Invoice Header</returns>
    PaxInvoice CreateInvoice(PaxInvoice invoiceHeader);

    /// <summary>
    /// Updates invoice header information
    /// </summary>
    /// <param name="invoiceHeader">Invoice details to be updated</param>
    /// <returns>Updated Invoice object</returns>
    PaxInvoice UpdateInvoice(PaxInvoice invoiceHeader);

    /// <summary>
    /// Gets details of an invoice header. The containing objects for this invoice - like RMs, BMs, etc. will not be populated.
    /// </summary>
    /// <param name="invoiceId">string of the invoice</param>
    /// <returns>Invoice Header Details</returns>
    PaxInvoice GetInvoiceHeaderDetails(string invoiceId);

    /// <summary>
    /// Gets details of an invoice header.
    /// </summary>
    /// <param name="invoiceId">string of the invoice</param>
    /// <returns>Invoice Header Details</returns>
    PaxInvoice GetInvoiceDetailForFileUpload(string invoiceId);

    /// <summary>
    /// Deletes an invoice.
    /// </summary>
    /// <param name="invoiceId">string of invoice to be deleted</param>
    /// <returns>True if successfully deleted, false otherwise</returns>
    bool DeleteInvoice(string invoiceId);

    /// <summary>
    /// Validates an invoice
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <param name="isFutureSubmission"></param>
    /// <returns>True if successfully validated, false otherwise</returns>
    PaxInvoice ValidateInvoice(string invoiceId, out bool isFutureSubmission, int UserId = 0, string logRefId = null);

    /// <summary>
    /// Check Submit Invoice Permission of user.
    ///  ID : 296572 - Submission and Assign permission to user doesn't match !
    /// </summary>
    /// <param name="invIdList">Invoice id list </param>
    /// <param name="userId">user id</param>
    /// <returns></returns>
    List<string> ChkInvSubmitPermission(List<string> invIdList, int userId);

    /// <summary>
    /// Submits invoices
    /// </summary>
    /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
    /// <returns></returns>
    IList<PaxInvoice> SubmitInvoices(List<string> invoiceIdList);

    InvoiceBase UpdateInvoiceBase(IRepository<InvoiceBase> invoiceBaseResository, InvoiceBase invoiceBase);

    /// <summary>
    /// Submits invoice
    /// </summary>
    /// <param name="invoiceId">invoice id to be submitted</param>
    /// <returns></returns>
    PaxInvoice SubmitInvoice(string invoiceId);

    /// <summary>
    /// Marks the status of all invoices specified in the list to Processing Complete.
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<PaxInvoice> ProcessingCompleteInvoices(List<string> invoiceIdList);

    /// <summary>
    /// Marks the specified invoice status to Processing Complete.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    PaxInvoice ProcessingCompleteInvoice(string invoiceId);

    /// <summary>
    /// Marks the status of all invoices specified in the list to Presented.
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<PaxInvoice> PresentInvoices(List<string> invoiceIdList);

    /// <summary>
    /// Marks the specified invoice status to Presented.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    PaxInvoice PresentInvoice(string invoiceId);

    /// <summary>
    /// To get Billed/Billing Member's reference data for an invoice
    /// </summary>
    /// <param name="invoiceId">Id of invoice for which reference data should be fetched.</param>
    /// <param name="isBillingMember">True, indicates reference data for billing member should be retrieved
    /// false, indicates reference data for billed member should be retrieved</param>
    /// <param name="locationCode">The location code.</param>
    /// <returns>MemberLocationInformation class object</returns>
    MemberLocationInformation GetMemberReferenceData(string invoiceId, bool isBillingMember, string locationCode);

    /// <summary>
    /// To get Invoice level VAT list
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of the invoice level Vat</returns>
    IList<InvoiceVat> GetInvoiceLevelVatList(string invoiceId);

    /// <summary>
    /// To get the derived invoice level VAT list
    /// DerivedVATDetails class is used temporarily.It will be replaced by InvoiceTotalVat class,
    /// once InvoiceTotalVat class is used in Repository.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>List of the derived invoice level Vat</returns>
    IList<DerivedVatDetails> GetInvoiceLevelDerivedVatList(string invoiceId);

    /// <summary>
    /// Following method is used to retrieve SourceCode Vat Total which will be displayed on Popup
    /// </summary>
    /// <param name="sourceCodeVatTotalId">SourceCode Vat Breakdown Id</param>
    /// <returns>SourceCode Vat record</returns>
    List<SourceCodeVat> GetSourceCodeVatTotal(string sourceCodeVatTotalId);

    /// <summary>
    /// Retrieves list of VAT types which are not applied in the invoice
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IList<NonAppliedVatDetails> GetNonAppliedVatList(string invoiceId);

    /// <summary>
    /// To add invoice level VAT 
    /// </summary>
    /// <param name="vatList">Invoice level VAT details</param>
    /// <returns>Details of the added VAT record</returns>
    InvoiceVat AddInvoiceLevelVat(InvoiceVat vatList);

    /// <summary>
    /// To delete VAT record from database
    /// </summary>
    /// <param name="vatId">string for VAT</param>
    /// <returns>True if successfully deleted , false otherwise</returns>
    bool DeleteInvoiceLevelVat(string vatId);

    /// <summary>
    /// To get Invoice Total record
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <returns>Invoice total record </returns>
    InvoiceTotal GetInvoiceTotal(string invoiceId);

    /// <summary>
    /// Gets source code total list for an invoice
    /// </summary>
    /// <param name="invoiceId">Guid of the invoice</param>
    /// <returns>List of source code totals</returns>
    IList<SourceCodeTotal> GetSourceCodeList(string invoiceId);

    /// <summary>
    /// Return default value for settlement indicator for given combination of billing and billed members
    /// </summary>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId);

    /// <summary>
    /// Set BilledMember property of invoice when there is business exception
    /// </summary>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <returns></returns>
    Member GetBilledMember(int billedMemberId);

    /// <summary>
    /// Determines whether billed member migrated for specified billing month and period in invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <returns>
    /// 	True if member migrated for the specified invoice header; otherwise, false.
    /// </returns>
    bool IsMemberMigrated(PaxInvoice invoiceHeader);

    PaxInvoice GetInvoiceWithCoupons(string invoiceId);
    List<PaxInvoice> GetInvoicesWithCoupons(SearchCriteria criteria);


    /// <summary>
    /// Updates the member location information.
    /// </summary>
    /// <param name="memberLocationInformation">The member location information.</param>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="isBillingMember">if set to <c>true</c> [is billing member].</param>
    /// <param name="commitChanges">commit changes</param>
    /// <returns></returns>
    MemberLocationInformation UpdateMemberLocationInformation(MemberLocationInformation memberLocationInformation, InvoiceBase invoiceHeader, bool isBillingMember, bool? commitChanges = null);

    /// <summary>
    /// Update invoices
    /// </summary>
    /// <param name="invoiceIdList"></param>
    /// <returns></returns>
    IList<PaxInvoice> UpdateInvoiceStatuses(List<Guid> invoiceIdList);

    void UpdateInvoiceDetailsForLateSubmission(InvoiceBase invoice, Member billingMember, Member billedMember);

    /// <summary>
    /// Update invoice.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    PaxInvoice UpdateInvoiceStatus(Guid invoiceId);


    IQueryable<PaxBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageNo = null, int? pageSize = null, string sortColumn = null, string sortOrder = null,int rowCountRequired=1);

    IQueryable<PaxBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageNo = null, int? pageSize = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1);

    IQueryable<PaxInvoice> GetInvoicesForBillingHistory(int billingCode, int billedMemberId, int billingMemberId, int settlementMethodId);

    IQueryable<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria);

    /// <summary>
    /// Get invoice using load strategy to be used in read-only header
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    PaxInvoice GetInvoiceHeader(string invoiceId);

    /// <summary>
    /// Gets the invoice with prime and RM coupons.
    /// </summary>
    /// <returns></returns>
    PaxInvoice GetInvoiceWithPrimeAndRMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null);

    /// <summary>
    /// To mark suspended member's invoices as suspended.
    /// </summary>
    /// <param name="memberId">Member Id.</param>
    /// <param name="defaultSuspensionDate">From Suspension Date.</param>
    /// <param name="fromSuspensionDate">Default Suspension Date.</param>
    /// <param name="clearingHouse">Clearing House.</param>
    /// <returns></returns>
    bool UpdateSuspendedBillingMemberInvoices(int memberId, DateTime defaultSuspensionDate, DateTime fromSuspensionDate, string clearingHouse);

    /// <summary>
    /// To mark suspended billed member's invoices as suspended.
    /// </summary>
    /// <param name="memberId">Member Id.</param>
    /// <param name="defaultSuspensionDate">Default Suspension Date.</param>
    /// <param name="fromSuspensionDate">From Suspension Date.</param>
    /// <param name="clearingHouse">Clearing House.</param>
    /// <returns></returns>
    List<InvoiceBase> UpdateSuspendedBilledMemberInvoices(int memberId, DateTime defaultSuspensionDate,
                                                          DateTime fromSuspensionDate, string clearingHouse);

    /// <summary>
    /// Validates the acceptable differences.
    /// </summary>
    /// <param name="reasonCode">The reason code.</param>
    /// <param name="transactionType">Type of the transaction.</param>
    /// <param name="fareAmountDifference">The gross amount difference.</param>
    /// <param name="taxAmountDifference">The Tax amount difference.</param>
    /// <param name="vatAmountDifference">The VAT amount difference.</param>
    /// <param name="iscAmountDifference">The ISC amount difference.</param>
    /// <param name="uatpAmountDifference">The UATP amount difference.</param>
    /// <param name="handlingFeeAmountDifference">The handling fee amount difference.</param>
    /// <param name="otherCommissionAmountDifference">The other commission amount difference.</param>
    /// <returns></returns>
    string ValidateAcceptableDifferences(string reasonCode, TransactionType transactionType,
                                         double fareAmountDifference, double taxAmountDifference,
                                         double vatAmountDifference, double iscAmountDifference,
                                         double uatpAmountDifference,
                                         double handlingFeeAmountDifference, double otherCommissionAmountDifference,
                                         IList<ReasonCode> validReasonCodes = null,
                                         IList<RMReasonAcceptableDiff> validRmReasonAcceptableDiff = null);


    /// <summary>
    /// Gets the default currency.
    /// </summary>
    /// <param name="settlementMethodId">The settlement method id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <returns></returns>
    int GetDefaultCurrency(int settlementMethodId, int billingMemberId, int billedMemberId);

    PaxAuditTrail GetbillingHistoryAuditTrail(string transactionId, string transactionType);

    ///<summary>
    /// Update invoice status to 'Processing Complete'
    /// </summary>
    bool UpdateInvoiceStatusToProcessingComplete();

    IQueryable<PaxInvoice> GetInvoicesForSamplingBillingHistory(int billingCode, int billedMemberId, int billingMemberId, int provisionalBillingMonth, int provisionalBillingYear, int settlementMethodId);

    TransactionDetails GetRejectedTransactionDetails(string transactionIds);

    /// <summary>
    /// Generate the pax Invoces old IDEC file.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    void GeneratePaxOldIdec(BillingPeriod billingPeriod);

    /// <summary>
    /// Generate the pax Invoces old IDEC file, based on Billing Period
    /// </summary>
    /// <param name="lastBillingMonthPeriod"></param>
    void GeneratePaxOldIdecInternal(BillingPeriod lastBillingMonthPeriod, int regenerateFlag = 0, int billingMemberId = 0);

    /// <summary>
    /// Get Invoice Legal PDF path 
    /// </summary>
    /// <param name="invoiceId">Invoice Number </param>
    /// <returns> PDF location path </returns>
    string GetInvoiceLegalPfdPath(Guid invoiceId);

    bool IgnoreValidationInMigrationPeriod(PaxInvoice invoice, RejectionMemo rejectionMemoRecord, TransactionType transactionType, PassengerConfiguration passengerConfig = null);

    /// <summary>
    /// This function will only return the invalid Invoices,
    /// which were not marked as processing complete by supporting
    /// document finalization process.
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    List<InvoiceBase> GetInvalidInvoicesForOutputfileGeneration(SearchCriteria searchCriteria);

    /// <summary>
    /// For system monitor, this function will only return the Invoice bases to be generated in output file
    /// </summary>
    /// <param name="searchCriteria"></param>
    /// <returns></returns>
    List<InvoiceBase> SystemMonitorGetAllInvoiceBasesForOutputfileGeneration(SearchCriteria searchCriteria);

    /// <summary>
    /// Gets the invoice with RM coupons.
    /// </summary>
    /// <param name="invoiceNumber"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="billingCode"></param>
    /// <param name="couponSearchCriteriaString"></param>
    /// <param name="rejectionMemoNumber">Added the new parameter for SCP51931: File stuck in Production. If value provided then data would be fetched for the provided RM only.</param> 
    /// <returns></returns>
    PaxInvoice GetInvoiceWithRMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null, string rejectionMemoNumber = null);

    /// <summary>
    /// Gets the invoice with prime for validation of RM coupons.
    /// </summary>
    /// <param name="invoiceNumber"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="billingCode"></param>
    /// <param name="couponSearchCriteriaString"></param>
    /// <param name="fetchCoupon"></param>
    /// <returns></returns>
    PaxInvoice GetInvoiceWithCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null, bool fetchCoupon = true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileLogId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    List<InvoiceBase> GetSuspendedMembersInvoicesForIsFile(string fileLogId, int billedMemberId,
                                                                  int billingMemberId);

    InvoiceBase GetInvoice(Guid invoiceId);

    /// <summary>
    /// Used to update member location information for submitted invoices. 
    /// While creating/updating invoices, member location information table stores entries only for blank location codes.
    /// </summary>
    /// <param name="updatedInvoice"></param>
    void UpdateMemberLocationInformationForLateSub(InvoiceBase updatedInvoice);

    /// <summary>
    /// This will validate the errors on the error correction screen
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    int ValidateForErrorCorrection(string newValue, string exceptionCode, Guid? entityId = null);

    /// <summary>
    /// Validate Correspondence for Pax
    /// </summary>
    /// <param name="billingMemo"></param>
    /// <param name="isUpdateOperation"></param>
    /// <param name="billingMemoInvoice"></param>
    void ValidateCorrespondenceReference(BillingMemo billingMemo, bool isUpdateOperation, PaxInvoice billingMemoInvoice);

    IQueryable<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceReportSearchResult( int logInMemberid,
    int entityId, int currencyId, int clearanceMonth, int clearanceYear);

    /// <summary>
    /// Get next sequence number for Auto Billing Invoice
    /// </summary>
    /// <returns>Next Invoice Sequnce Number</returns>
    int GetAutoBillingInvoiceNumberSeq(int billingMemberId, int billingYear);

      
      /// <summary>
      /// CMP#400- Audit Trail Report for Deleted Invoices
      /// </summary>
      /// <param name="auditDeletedInvoice">object of Audit Trail Deleted invoice</param>
      /// <returns></returns>
      bool AuditDeletedInvoice(AuditDeletedInvoice auditDeletedInvoice);

      /// <summary>
      /// CMP#400- Audit Trail Report for Deleted Invoices
      /// </summary>
      /// <param name="auditDeletedInvoice"></param>
      /// <returns></returns>
      List<InvoiceDeletionAuditReport> GetInvoiceDeletionAuditDetails(AuditDeletedInvoice auditDeletedInvoice);

      /// <summary>
      /// CMP#459 : Validates the amounts in RM on memo level.
      /// </summary>
      /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="rejectionMemoRecord">The rejection memo record.</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <returns></returns>
      bool ValidateAmountsInRMonMemoLevel(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                          IList<IsValidationExceptionDetail> exceptionDetailsList,
                                          RejectionMemo rejectionMemoRecord,
                                          bool isErrorCorrection = false);

      /// <summary>
      /// CMP#459 : Validates the amounts in RM on coupon level.
      /// </summary>
      /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="rejectionMemoRecord">The rejection memo record.</param>
      /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
      /// <param name="isBillingHistory">if set to <c>true</c> [is billing history].</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <returns></returns>
      bool ValidateAmountsInRMonCouponLevel(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                            IList<IsValidationExceptionDetail> exceptionDetailsList,
                                            RejectionMemo rejectionMemoRecord,
                                            RMCoupon rejectionMemoCouponBreakdownRecord, bool isBillingHistory = true, bool isErrorCorrection = false);

      /// <summary>
      /// CMP#459 : Validates the original billing amount in rm.
      /// </summary>
      /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
      /// <param name="prevExchangeRate">The prev exchange rate.</param>
      /// <param name="currentExchangeRate">The current exchange rate.</param>
      /// <param name="currentRejectionMemoRecord">The current rejection memo record.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="currentInvoice">The current invoice.</param>
      /// <param name="yourInvoice">Your invoice.</param>
      /// <param name="currentRmCoupon">The current rm coupon.</param>
      /// <param name="yourPrimeCouponRecords">Your prime coupon records.</param>
      /// <param name="samplingFormDRecords">The sampling form D records.</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="isIsWeb">if set to <c>true</c> [is is web].</param>
      /// <param name="isBillingHistory">if set to <c>true</c> [is billing history].</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <returns></returns>
      bool ValidateOriginalBillingAmountInRm(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                             ExchangeRate prevExchangeRate,
                                             ExchangeRate currentExchangeRate,
                                             RejectionMemo currentRejectionMemoRecord,
                                             IList<IsValidationExceptionDetail> exceptionDetailsList,
                                             PaxInvoice currentInvoice,
                                             PaxInvoice yourInvoice,
                                             RMCoupon currentRmCoupon,
                                             IList<PrimeCoupon> yourPrimeCouponRecords,
                                             IList<SamplingFormDRecord> samplingFormDRecords,
                                             string fileName,
                                             DateTime fileSubmissionDate,
                                             bool isIsWeb = false,
                                             bool isBillingHistory = true,
                                             bool isErrorCorrection = false);

      /// <summary>
      /// CMP#459 : Validates the amountof rm on validation error correction.
      /// </summary>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="errorCorrection">The error correction.</param>
      /// <returns></returns>
      bool ValidateAmountofRmOnValidationErrorCorrection(IList<IsValidationExceptionDetail> exceptionDetailsList, ValidationErrorCorrection errorCorrection);

    /// <summary>
    /// Delete Invoice from Manahe Invoice Screen
    /// </summary>
    /// <param name="invoiceIdStringInOracleFormat"></param>
    /// <param name="dummyMemberId"></param>
    /// <param name="userId"></param>
    /// <param name="isIsWebInvoice"></param>
    /// <returns></returns>
    bool DeleteInvoice(string invoiceIdStringInOracleFormat, int dummyMemberId, int userId, int isIsWebInvoice);

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

      void CheckDuplicateBillingMemoForCorr(BillingMemo billingMemo, PaxInvoice billingMemoInvoice,
                                            bool isUpdateOperation);

      /// <summary>
      /// This function is used to get linked correspondence rejection memo list.
      /// </summary>
      /// <param name="corrRefNo"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      List<PaxLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId);

      /// <summary>
      /// This function is used for create rejection audit trail pdf.
      /// </summary>
      /// <param name="request"></param>
      /// <param name="memberFtpRMAuditTrailPath"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      String CreateRejectionAuditTrailPdf(ReportDownloadRequestMessage request, string memberFtpPath, int ProcessingUnitNumber);

      //CMP602
      void SetViewableByClearingHouseForAutoBilling(InvoiceBase invoice);

      //SCP304880 - SRM: Invoices not presented in UAT
      bool CheckIfSettlementIsBilateral(Member billingMember, Member billedMember);

      //ID : 325377 - File Loading & Web Response Stats ViewInvoice CargoManageInvoice
      PaxInvoice GetInvoiceDetails(string invoiceId);

      /// <summary>
      /// CMP #596: Length of Member Accounting Code to be Increased to 12.
      /// Desc: This is a New auto-complete logic #MW1, Validation logic #MW2
      /// For more refer FRS Section 3.4 Point 20, 21.
      /// </summary>
      /// <param name="memberCodeNumeric">Member Numeric Code</param>
      /// <returns></returns>
      bool IsTypeBMember(string memberCodeNumeric);

      //CMP #641: Time Limit Validation on Third Stage PAX Rejections

      /// <summary>
      /// Validates the pax stage three rm for time limit.
      /// </summary>
      /// <param name="transactionType">Type of the transaction.</param>
      /// <param name="settlementMethodId">The settlement method id.</param>
      /// <param name="rejectionMemo">The rejection memo.</param>
      /// <param name="currentInvoice">The current invoice.</param>
      /// <param name="isIsWeb">if set to <c>true</c> [is is web].</param>
      /// <param name="isManageInvoice">if set to <c>true</c> [is manage invoice].</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="errorCorrection">The error correction.</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <param name="isBillingHistory">if set to <c>true</c> [is billing history].</param>
      /// <returns></returns>
      bool ValidatePaxStageThreeRmForTimeLimit(TransactionType transactionType,
                                               int settlementMethodId,
                                               RejectionMemo rejectionMemo,
                                               PaxInvoice currentInvoice,
                                               bool isIsWeb = false,
                                               bool isManageInvoice = false,
                                               string fileName = null,
                                               DateTime? fileSubmissionDate = null,
                                               IList<IsValidationExceptionDetail> exceptionDetailsList = null,
                                               ValidationErrorCorrection errorCorrection = null,
                                               bool isErrorCorrection = false,
                                               bool isBillingHistory = false);

      /// <summary>
      /// This function is used to check billing and billed member is ach or dual member.
      /// CMP #553: ACH Requirement for Multiple Currency Handling
      /// </summary>
      /// <param name="billingMemberId"></param>
      /// <param name="billedMemberId"></param>
      /// <returns></returns>
      bool IsBillingAndBilledAchOrDualMember(int billingMemberId, int billedMemberId);

  }
}
