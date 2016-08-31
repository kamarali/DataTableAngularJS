using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Calendar;
using System;
using Iata.IS.Model.Pax;
//using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Common;
using InvoiceSearchCriteria = Iata.IS.Model.Pax.BillingHistory.InvoiceSearchCriteria;
using RejectionMemo = Iata.IS.Model.Cargo.CargoRejectionMemo;

namespace Iata.IS.Business.Cargo
{
  /// <summary>
  /// Responsible for invoice search, validating and submitting invoices.

  /// </summary>
  public interface ICargoInvoiceManager
  {
    /// <summary>
    /// Creates an invoice header
    /// </summary>
    /// <param name="invoiceHeader">Invoice Header to be created</param>
    /// <returns>Created Invoice Header</returns>
    CargoInvoice CreateInvoice(CargoInvoice invoiceHeader);

      //28.11.2011=============================================


    /// <summary>
    /// Generate the pax Invoces old IDEC file.
    /// </summary>
    /// <param name="billingMemberId">The billing member id.</param>
    void GenerateCargoOldIdec(BillingPeriod billingPeriod);

    /// <summary>
    /// Generate the pax Invoces old IDEC file, based on Billing Period
    /// </summary>
    /// <param name="lastBillingMonthPeriod"></param>
    void GenerateCargoOldIdecInternal(BillingPeriod lastBillingMonthPeriod, int regenerateFlag = 0, int billingMemberId = 0);

      //=====================================================

    /// <summary>
    /// Gets details of an invoice header. The containing objects for this invoice - like RMs, BMs, etc. will not be populated.
    /// </summary>
    /// <param name="invoiceId">string of the invoice</param>
    /// <returns>Invoice Header Details</returns>
    CargoInvoice GetInvoiceHeaderDetails(string invoiceId);

    /// <summary>
    /// Gets details of an invoice header. The containing objects for this invoice - like RMs, BMs, etc. will not be populated.
    /// </summary>
    /// <param name="invoiceId">string of the invoice</param>
    /// <returns>Invoice  Details</returns>
    CargoInvoice GetInvoiceDetails(string invoiceId);


    /// <summary>
    /// Gets details of rejection memo record corresponding to passed rejection memo record Id
    /// </summary>
    /// <param name="rejectionMemoRecordId">string of the rejection Memo record</param>
    /// <returns>Details of rejection memo record</returns>
    RejectionMemo GetRejectionMemoRecordDetails(string rejectionMemoRecordId);


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
    /// <returns>True if successfully validated, false otherwise</returns>
    CargoInvoice ValidateInvoice(string invoiceId);

    /// <summary>
    /// Validates an invoice
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <returns>True if successfully validated, false otherwise</returns>
    CargoInvoice Validate(string invoiceId);

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
    IList<CargoInvoice> SubmitInvoices(List<string> invoiceIdList);

    MemberLocationInformation GetMemberReferenceData(string invoiceId, bool isBillingMember, string locationCode);


    CargoInvoice SubmitInvoice(string invoiceId);
    /// <summary>
    /// Get Invoice Legal PDF path 
    /// </summary>
    /// <param name="invoiceId">Invoice Number </param>
    /// <returns> PDF location path </returns>
    string GetInvoiceLegalPfdPath(Guid invoiceId);


    /// <summary>
    /// Determines whether transaction exists for the specified invoice id
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    bool IsTransactionExists(string invoiceId);

    /// <summary>
    /// Set BilledMember property of invoice when there is business exception
    /// </summary>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <returns></returns>
    Member GetBilledMember(int billedMemberId);

    #region Billing Memo

    // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
    /// <summary>
    /// Adds the billing memo record.
    /// </summary>
    /// <param name="billingMemo">The billing memo.</param>
    /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
    /// <returns></returns>
    CargoBillingMemo AddBillingMemoRecord(CargoBillingMemo billingMemo, bool isNullCorrRefNo = false);

    /// <summary>
    /// Updates the billing memo attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    IList<CargoBillingMemoAttachment> UpdateBillingMemoAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Determines whether [is duplicate billing memo attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate billing memo attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    bool IsDuplicateBillingMemoAttachmentFileName(string fileName, Guid billingMemoId);

    /// <summary>
    /// Adds the billing memo attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    CargoBillingMemoAttachment AddBillingMemoAttachment(CargoBillingMemoAttachment attach);

    /// <summary>
    /// Gets the billing memo attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    CargoBillingMemoAttachment GetBillingMemoAttachmentDetails(string attachmentId);

    /// <summary>
    /// Gets the billing memo record details.
    /// </summary>
    /// <param name="billingMemoRecordId">The billing memo record id.</param>
    /// <returns></returns>
    CargoBillingMemo GetBillingMemoRecordDetails(string billingMemoRecordId);

    // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
    /// <summary>
    /// Updates the billing memo record.
    /// </summary>
    /// <param name="billingMemo">The billing memo.</param>
    /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
    /// <returns></returns>
    CargoBillingMemo UpdateBillingMemoRecord(CargoBillingMemo billingMemo, bool isNullCorrRefNo = false);

    /// <summary>
    /// Gets the billing memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CargoBillingMemoAttachment> GetBillingMemoAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Gets the billing memo awb count.
    /// </summary>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <returns></returns>
    long GetBillingMemoAwbCount(string billingMemoId);

    /// <summary>
    /// Gets the billing memo list.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IList<CargoBillingMemo> GetBillingMemoList(string invoiceId);

    /// <summary>
      /// Deletes the billing memo record.
      /// </summary>
      /// <param name="billingMemoRecordId">The billing memo record id.</param>
      /// <returns></returns>
    bool DeleteBillingMemoRecord(string billingMemoRecordId);

    /// <summary>
      /// Gets the BM awb list.
      /// </summary>
      /// <param name="billingMemoId">The billing memo id.</param>
      /// <returns></returns>
    IList<CargoBillingMemoAwb> GetBMAwbList(string billingMemoId);


    /// <summary>
    /// Deletes the billing memo awb record.
    /// </summary>
    /// <param name="awbRecordId">The awb record id.</param>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    bool DeleteBillingMemoAwbRecord(string awbRecordId, out Guid billingMemoId, out Guid invoiceId);

    /// <summary>
    /// Gets the BM awb attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    BMAwbAttachment GetBMAwbAttachmentDetails(string attachmentId);

    /// <summary>
    /// Adds the BM awb attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    BMAwbAttachment AddBMAwbAttachment(BMAwbAttachment attach);

    /// <summary>
    /// Gets the BM awb attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<BMAwbAttachment> GetBMAwbAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Determines whether [is duplicate BM awb attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate BM awb attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    bool IsDuplicateBMAwbAttachmentFileName(string fileName, Guid bmAwbId);


    /// <summary>
    /// Adds the BM awb record.
    /// </summary>
    /// <param name="bmAwb">The bm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    CargoBillingMemoAwb AddBMAwbRecord(CargoBillingMemoAwb bmAwb, string invoiceId, out string duplicateErrorMessage);

    /// <summary>
    /// Updates the BM awb record.
    /// </summary>
    /// <param name="bmAwb">The bm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    CargoBillingMemoAwb UpdateBMAwbRecord(CargoBillingMemoAwb bmAwb, string invoiceId, out string duplicateErrorMessage);

      /// <summary>
      /// Updates the BM awb attachment.
      /// </summary>
      /// <param name="attachments">The attachments.</param>
      /// <param name="parentId">The parent id.</param>
      /// <returns></returns>
      IList<BMAwbAttachment> UpdateBMAwbAttachment(IList<Guid> attachments, Guid parentId);

      /// <summary>
      /// Gets the B memo awb record details.
      /// </summary>
      /// <param name="bmAwbRecordId">The bm awb record id.</param>
      /// <returns></returns>
      CargoBillingMemoAwb GetBMemoAwbRecordDetails(string bmAwbRecordId);

      /// <summary>
      /// Gets the BM awb prorate ladder detail list.
      /// </summary>
      /// <param name="bmAwbProrateLadderDetailId">The bm awb prorate ladder detail id.</param>
      /// <returns></returns>
      IList<BMAwbProrateLadderDetail> GetBMAwbProrateLadderDetailList(Guid bmAwbProrateLadderDetailId);

      ///// <summary>
      ///// Adds the awb prorate ladder.
      ///// </summary>
      ///// <param name="bmAwbProrateLadder">The bm awb prorate ladder.</param>
      ///// <param name="bmAwbProrateLadderDetail">The bm awb prorate ladder detail.</param>
      ///// <returns></returns>
      //BMAwbProrateLadder AddAwbProrateLadder(BMAwbProrateLadder bmAwbProrateLadder, BMAwbProrateLadderDetail bmAwbProrateLadderDetail);

      /// <summary>
      /// Adds the awb prorate ladder detail.
      /// </summary>
      /// <param name="bmAwbProrateLadderDetail">The bm awb prorate ladder detail.</param>
      /// <returns></returns>
      BMAwbProrateLadderDetail AddAwbProrateLadderDetail(BMAwbProrateLadderDetail bmAwbProrateLadderDetail);

      ///// <summary>
      ///// Deletes the awb prorate ladder record.
      ///// </summary>
      ///// <param name="prorateLadderId">The prorate ladder id.</param>
      ///// <returns></returns>
      //bool DeleteAwbProrateLadderRecord(string prorateLadderId, out Guid couponId);

      /// <summary>
      /// Deletes the awb prorate ladder detail record.
      /// </summary>
      /// <param name="prorateLadderDetailId">The prorate ladder detail id.</param>
      /// <returns></returns>
      bool DeleteAwbProrateLadderDetailRecord(string prorateLadderDetailId, out bool isParentdelete, out Guid couponId);

      ///// <summary>
      ///// Updates the awb prorate ladder.
      ///// </summary>
      ///// <param name="bmAwbProrateLadder">The bm awb prorate ladder.</param>
      ///// <returns></returns>
      //BMAwbProrateLadder UpdateBMAwbProrateLadder(BMAwbProrateLadder bmAwbProrateLadder);

      ///// <summary>
      ///// Gets the BM awb prorate ladder record.
      ///// </summary>
      ///// <param name="prorateLadderId">The prorate ladder id.</param>
      ///// <returns></returns>
      //BMAwbProrateLadder GetBMAwbProrateLadderRecord(string prorateLadderId);

    #endregion

    /// <summary>
    /// Get linking details for rejection memo
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
  //  RMLinkingResultDetails GetRejectionMemoLinkingDetails(RMLinkingCriteria criteria);


    /// <summary>
    /// Add RejectionMemo Record. Used to save RM to display error message while adding RM Coupon
    /// </summary>
    /// <param name="rejectionMemoRecord">RejectionMemoRecord to be added.</param>
    /// <param name="linkingErrorMessage">Error message in RM coupon linking</param>
    /// <param name="warningMessage">Warning message in RM coupon linking</param>
    /// <returns></returns>
    RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string linkingErrorMessage, out string warningMessage);

    RejectionMemo UpdateRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage);

    /// <summary>
    /// Update parent id of rejection memo attachment record for given Guids
    /// </summary>
    /// <param name="attachments">list of Guid of rejection memo attachment record</param>
    /// <param name="parentId">rejection memo id</param>
    /// <returns></returns>
    IList<CgoRejectionMemoAttachment> UpdateRejectionMemoAttachment(IList<Guid> attachments, Guid parentId);

    //IList<PrimeCoupon> GetPrimeBillingCouponList(string[] rejectionIdList);

    /// <summary>
    /// New Awb record to be added
    /// </summary>
    /// <param name="awbRecord">The Awb record.</param>
    /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
    /// <returns>Added Awb record</returns>
    AwbRecord AddAwbRecord(AwbRecord awbRecord, out string duplicateCouponErrorMessage);

    /// <summary>
    /// Gets the Awb attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<AwbAttachment> GetAwbRecordAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Updates Invoice
    /// </summary>
    /// <param name="invoiceHeader">Invoice Id.</param>
    /// <returns></returns>
    CargoInvoice UpdateInvoice(CargoInvoice invoiceHeader);

    /// <summary>
    /// Gets the awb attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    AwbAttachment GetAwbAttachmentDetails(string attachmentId);

    /// <summary>
    /// Add awb attachment.
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    AwbAttachment AddAwbAttachment(AwbAttachment attach);
    /// <summary>
    /// Check for duplicate awb Attachment file.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="awbRecordId"></param>
    /// <returns></returns>
    bool IsDuplicateAwbAttachmentFileName(string fileName, Guid awbRecordId);
    /// <summary>
    /// Updates Awb Attachment.
    /// </summary>
    /// <param name="attachments"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    IList<AwbAttachment> UpdateAwbAttachment(IList<Guid> attachments, Guid parentId);
    
    /// <summary>
    /// Check for duplicate file name of rejection memo attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="rejectionMemoId">rejection Memo Id</param>
    /// <returns></returns>
    bool IsDuplicateRejectionMemoAttachmentFileName(string fileName, Guid rejectionMemoId);

    CgoRejectionMemoAttachment AddRejectionMemoAttachment(CgoRejectionMemoAttachment attach);

    List<CgoRejectionMemoAttachment> GetRejectionMemoAttachments(List<Guid> attachmentIds);

    long GetRejectionMemoAwbCount(string memoRecordId);

    IList<RejectionMemo> GetRejectionMemoList(string invoiceId);

    bool DeleteRejectionMemoRecord(string rejectionMemoRecordId);

    CgoRMLinkingResultDetails GetRejectionMemoLinkingDetails(CgoRMLinkingCriteria criteria);

    /// <summary>
    /// Following method is used to retrieve Billing history Invoice data
    /// </summary>
    /// <param name="searchCriteria">Invoice Search criteria</param>
    /// <param name="correspondenceSearchCriteria">Correspondence Search criteria</param>
    /// <returns>Billing History Invoice search list</returns>
    IQueryable<CargoBillingHistorySearchResult> GetBillingHistorySearchResult(Model.Cargo.BillingHistory.InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria);

    /// <summary>
    /// Following method is used to retrieve Billing history Correspondence data
    /// </summary>
    /// <param name="correspondenceSearchCriteria">Correspondence Search criteria</param>
    /// <returns>Billing History Correspondence search list</returns>
    IQueryable<CargoBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria);

    /// <summary>
    /// Gets the billing memos for a correspondence.
    /// </summary>
    /// <param name="correspondenceNumber">Correspondence Number</param>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <returns></returns>
    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    CargoExistingBMTransactionDetails GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId);

    CargoAuditTrail GetBillingHistoryAuditTrail(string transactionId, string transactionType);

    AwbRecord UpdateAwbRecord(AwbRecord awbRecord, out string duplicateCouponErrorMessage);

    IList<AwbRecord> GetAwbPrepaidBillingRecordList(string invoiceId);

    bool DeleteAwbRecord(string awbRecordId);

    AwbRecord GetAwbRecordDetails(string awbRecordId);

    CgoRejectionMemoAttachment GetRejectionMemoAttachmentDetails(string attachmentId);

    RMAwb AddRejectionMemoAwbDetails(RMAwb rmAwbRecord, string invoiceId, out string duplicateErrorMessage, bool isFromBillingHistory = false, Guid? logRefId = null);

    RMAwbAttachment GetRejectionMemoAwbAttachmentDetails(string attachmentId);

    RMAwbAttachment AddRejectionMemoAwbAttachment(RMAwbAttachment attach);
    
    IList<AwbRecord> GetAwbChargeCollectBillingRecordList(string invoiceId);
    
    IList<RMAwb> GetRejectionMemoAwbList(string memoRecordId);

    bool DeleteRejectionMemoAwbRecord(string awbRecordId, out Guid rejectionMemoId, out Guid invoiceId);

    BMAwbAttachment GetBillingMemoAwbAttachmentDetails(string attachmentId);

    CargoCreditMemoAttachment GetCreditMemoAttachmentDetails(string attachmentId);

    CMAwbAttachment GetCreditMemoAwbAttachmentDetails(string attachmentId);

    /// <summary>
    /// Add invoice level Vat
    /// </summary>
    /// <param name="vatList">The invoice total vat.</param>
    /// <returns></returns>
    CargoInvoiceTotalVat AddInvoiceLevelVat(CargoInvoiceTotalVat vatList);

    /// <summary>
    /// Delete cargo invoice level Vat
    /// </summary>
    /// <param name="vatId"></param>
    /// <returns></returns>
    bool DeleteInvoiceLevelVat(string vatId);

    /// <summary>
    /// To get Invoice level VAT list
    /// </summary>
    /// <param name="invoiceId">invoice id for which vat list to be retrieved..</param>
    /// <returns>List of the invoice level Vat</returns>
    IList<CargoInvoiceTotalVat> GetInvoiceLevelVatList(string invoiceId);

    IList<DerivedVatDetails> GetInvoiceLevelDerivedVatList(string invoiceId);

    IList<NonAppliedVatDetails> GetNonAppliedVatList(string invoiceId);

    RMAwb GetRejectionMemoAwbDetails(string rmAwbId);

    RMAwb UpdateRejectionMemoAwbDetails(RMAwb rmAwb, string invoiceId, out string duplicateErrorMessage);

    List<RMAwbAttachment> GetRejectionMemoAwbAttachments(List<Guid> attachmentIds);

    IList<RMAwbAttachment> UpdateRejectionMemoAwbAttachment(IList<Guid> attachments, Guid parentId);

    TransactionDetails GetRejectedTransactionDetails(string transactionIds);

    IQueryable<CargoInvoice> GetInvoicesForBillingHistory(int billingCode, int billedMemberId, int billingMemberId, int settlementMethodId);

    /// <summary>
    /// Validate AwbSerialNumber 
    /// (if AwbSerialNumber Mod 7 == Checkdigit) then valid
    /// else in valid
    /// </summary>
    /// <param name="awbSerialNumber">AwbSerialNumber</param>
    /// <param name="checkDigit">checkDigit</param>
    /// <returns></returns>
    bool ValidateAwbSerialNumber(int awbSerialNumber, int checkDigit);

    /// <summary>
    /// Retrieve invoice subtotal
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    IList<BillingCodeSubTotal> GetSubTotalList(string invoiceId);

    /// <summary>
    /// Checks whether invoices are blocked due to some pending processes
    /// </summary>
    /// <param name="cgoInvoiceBases"></param>
    /// <returns></returns>
    bool ValidateCgoInvoices(IEnumerable<InvoiceBase> cgoInvoiceBases);

    RMLinkedAwbDetails GetRMAwbBreakdownRecordDetails(string issuingAirline, int serialNo, Guid rmId, int billingMemberId, int billedMemberId, int awbBillingCode);

    CgoRMLinkingResultDetails GetLinkedMemoAmountDetails(CgoRMLinkingCriteria criteria);

    List<CargoBillingCodeSubTotalVat> GetBillingCodeVatTotal(string billingCodeSubVatTotalId);

    //List<CgoBillingCodeSubTotalVat> FetchBillingCodeVatTotal(string billingCodeSubVatTotalId);

    RMLinkedAwbDetails GetRMAwbBreakdownSingleRecordDetails(Guid awbId, Guid rejectionMemoId);

    bool IsBillingMemoInvoiceOutSideTimeLimit(string correspondenceRefNumber, bool authorityToBill, int correspondenceStatusId, DateTime correspondenceDate);


    /// <summary>
    /// Gets the RM awb prorate ladder detail list.
    /// </summary>
    /// <param name="bmAwbProrateLadderDetailId">The bm awb prorate ladder detail id.</param>
    /// <returns></returns>
    IList<RMAwbProrateLadderDetail> GetRMAwbProrateLadderDetailList(Guid bmAwbProrateLadderDetailId);

    ///// <summary>
    ///// Adds the RM awb prorate ladder.
    ///// </summary>
    ///// <param name="bmAwbProrateLadder">The bm awb prorate ladder.</param>
    ///// <param name="bmAwbProrateLadderDetail">The bm awb prorate ladder detail.</param>
    ///// <returns></returns>
    //RMAwbProrateLadder AddRMAwbProrateLadder(RMAwbProrateLadder bmAwbProrateLadder, RMAwbProrateLadderDetail rmAwbProrateLadderDetail);

    /// <summary>
    /// Adds the awb RM prorate ladder detail.
    /// </summary>
    /// <param name="bmAwbProrateLadderDetail">The bm awb prorate ladder detail.</param>
    /// <returns></returns>
    RMAwbProrateLadderDetail AddRMAwbProrateLadderDetail(RMAwbProrateLadderDetail bmAwbProrateLadderDetail);

    ///// <summary>
    ///// Deletes the RM awb prorate ladder record.
    ///// </summary>
    ///// <param name="prorateLadderId">The prorate ladder id.</param>
    ///// <returns></returns>
    //bool DeleteRMAwbProrateLadderRecord(string prorateLadderId, out Guid couponId);

    /// <summary>
    /// Deletes the RM awb prorate ladder detail record.
    /// </summary>
    /// <param name="prorateLadderDetailId">The prorate ladder detail id.</param>
    /// <returns></returns>
    bool DeleteRMAwbProrateLadderDetailRecord(string prorateLadderDetailId, out bool isParentdelete, out Guid couponId);

    ///// <summary>
    ///// Updates the RM awb prorate ladder.
    ///// </summary>
    ///// <param name="bmAwbProrateLadder">The bm awb prorate ladder.</param>
    ///// <returns></returns>
    //RMAwbProrateLadder UpdateRMAwbProrateLadder(RMAwbProrateLadder bmAwbProrateLadder);

    ///// <summary>
    ///// Gets the RM awb prorate ladder record.
    ///// </summary>
    ///// <param name="prorateLadderId">The prorate ladder id.</param>
    ///// <returns></returns>
    //RMAwbProrateLadder GetRMAwbProrateLadderRecord(string prorateLadderId);

    void ValidateCorrespondenceReference(CargoBillingMemo billingMemo, bool isUpdateOperation,
                                         CargoInvoice billingMemoInvoice);

    IList<ReasonCode> GetReasonCodeListForBillingMemo();

    int GetAwbBatchRecSeqNumber(int batchSeqNo, string invoiceNumber);

    string GenerateCargoBillingHistoryAuditTrailPdf(CargoAuditTrail auditTrail);

    /// <summary>
    /// Following method is executed from CargoBillingHistoryAuditTrail.vm file to return Month name from month number
    /// </summary>
    /// <param name="monthNumber">Month number whose name is to be retireved</param>
    /// <returns>Month name</returns>
    string GetAbbreviatedMonthName(int monthNumber);

    /// <summary>
    /// Following method is used to break Reason remarks text int string of 80 characters. Used in Pax pdf generation 
    /// </summary>
    /// <param name="reasonRemarks">reason remarks text</param>
    /// <returns>String broken into 80 characters</returns>
    string CreateReasonRemarksString(string reasonRemarks);

    /// <summary>
    /// To validate error correction
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorLevel"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    int ValidateForErrorCorrection(string newValue, string exceptionCode,string errorLevel,Guid? entityId = null);

    /// <summary>
    /// Following method sorts Correspondence details in descending order depending on stage. Executed from Audit trail pdf .vm file 
    /// </summary>
    /// <param name="rejectionMemo">Rejection memo object</param>
    /// <returns>Rejection memo with correspondence in sorted in descending order</returns>
    CargoRejectionMemo GetCorrespondenceDetails(CargoRejectionMemo rejectionMemo);

    /// <summary>
    /// Following method retrieves Stage2 RejectionMemo details
    /// </summary>
    /// <param name="memoList">RejectionMemo List</param>
    /// <param name="rejectionMemo">RejectionMemo to find in list</param>
    /// <returns>Stage2 RejectionMemo</returns>
    CargoRejectionMemo GetRejectionStage2MemoDetails(List<CargoRejectionMemo> memoList, CargoRejectionMemo rejectionMemo);

    /// <summary>
    /// Following method retrieves Stage1 RejectionMemo details
    /// </summary>
    /// <param name="memoList">RejectionMemo List</param>
    /// <param name="stage2RM">Stage2 RejectionMemo to find in list</param>
    /// <returns>Stage1 RejectionMemo</returns>
    CargoRejectionMemo GetRejectionStage1MemoDetails(List<CargoRejectionMemo> memoList, CargoRejectionMemo stage2RM);

    /// <summary>
    /// Following method returns Credit memo. it will be executed from cargo audit trail pdf .vm file 
    /// </summary>
    /// <param name="invoice">cargo invoice</param>
    /// <param name="stage1RM">rejection memo</param>
    /// <returns>Cargo credit memo</returns>
    CargoCreditMemo GetCreditMemoRecord(CargoInvoice invoice, CargoRejectionMemo stage1RM);

    /// <summary>
    /// Gets the invoice with RM coupons.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="couponSearchCriteriaString">The coupon search criteria string.</param>
    /// <returns></returns>
    CargoInvoice GetInvoiceWithRMCoupons(string invoiceNumber,
                                           int billingMonth,
                                           int billingYear,
                                           int billingPeriod,
                                           int billingMemberId,
                                           int billedMemberId,
                                           int? billingCode = null,
                                           string couponSearchCriteriaString = null);

    /// <summary>
    /// Following method is executed from .vm file to check whether RejectionMemo is already displayed while creating .pdf file.  
    /// </summary>
    /// <param name="rejectionMemoString">String of RejectionMemo Id's</param>
    /// <param name="rejectionMemoId">RejectionMemo Id, to check whether it is displayed</param>
    /// <returns>Returns "Yes" if RejectionMemo is already displayed, else returns "No"</returns>
    string IsRejectionMemoDisplayed(string rejectionMemoString, Guid rejectionMemoId);

    /// <summary>
    /// Retrieves Batch Number and Sequence number which will be pre populated while creating BM, CM, RM and AWB records.
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionTypeId">Transaction Type Id</param>
    /// <param name="batchNumber">Batch number</param>
    /// <param name="sequenceNumber">Sequence number</param>
    /// <returns>Retrieves Batch Number and Sequence number which will be pre populated.</returns>
    void GetBatchAndSequenceNumber(Guid invoiceId, int transactionTypeId, out int batchNumber, out int sequenceNumber);

    IList<CargoInvoice> ProcessingCompleteInvoices(List<string> invoiceIdList);

    IList<CargoInvoice> PresentInvoices(List<string> invoiceIdList);

    IList<AwbRecord> GetAwbRecordList(string[] couponIdList);

    /// <summary>
    /// Gets Correspondences for correspondence Trail Search Criteria
    /// </summary>
    /// <param name="correspondenceTrailSearchCriteria"></param>
    /// <returns></returns>
    IQueryable<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(
      CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria);

      /// <summary>
      /// CMP#459 : Validates the original billing amount in rm.
      /// </summary>
      /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
      /// <param name="currentRejectionMemoRecord">The current rejection memo record.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="currentInvoice">The current invoice.</param>
      /// <param name="yourInvoice">Your invoice.</param>
      /// <param name="currentRmAwbRecord">The current rm awb record.</param>
      /// <param name="primeAwbRecords">The prime awb records.</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="isIsWeb">if set to <c>true</c> [is is web].</param>
      /// <param name="isBillingHistory">if set to <c>true</c> [is billing history].</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <returns></returns>
      bool ValidateOriginalBillingAmountInRm(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                             ExchangeRate prevExchangeRate,
                                             ExchangeRate currentExchangeRate,
                                             CargoRejectionMemo currentRejectionMemoRecord,
                                             IList<IsValidationExceptionDetail> exceptionDetailsList,
                                             CargoInvoice currentInvoice,
                                             CargoInvoice yourInvoice,
                                             RMAwb currentRmAwbRecord,
                                             IList<AwbRecord> primeAwbRecords,
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

      //CMP508:Audit Trail Download with Supporting Documents
      /// <summary>
      /// Returns Html string for audit trail with supporting docs assigned with their folder numbers
      /// </summary>
      /// <param name="auditTrail">audit trail for which html is to be genereated</param>
      /// <param name="suppDocs">out parameter for Supp Docs</param>
      /// <returns>Html for audit trail</returns>
      string GenerateCargoBillingHistoryAuditTrailPackage(CargoAuditTrail auditTrail, out Dictionary<Attachment, int> suppDocs);

      /// <summary>
      /// Get all stage 3 rejection memo linked with correspondence.
      /// </summary>
      /// <param name="corrRefNo"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      List<CgoLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId);

      /// <summary>
      /// This function is used for create Cargo rejection audit trail pdf.
      /// </summary>
      /// <param name="request"></param>
      /// <param name="memberFtpRejectionPath"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      string CreateRejectionAuditTrailPdf(ReportDownloadRequestMessage request, string memberFtpRejectionPath,int ProcessingUnitNumber);

      /// <summary>
      /// CMP#650 Conditional Validations on Reason Codes for CGO Rejections
      /// This Function is used for validating reason code for BM/RM
      /// </summary>
      /// <param name="rmCurrentRecord"></param>
      /// <param name="yourReasonCode"></param>
      /// <param name="isIsWeb"></param>
      /// <param name="isValidationCorrection"></param>
      /// <returns></returns>
      string ValidateCargoReasonCode(CargoRejectionMemo rmCurrentRecord, string yourReasonCode, bool isIsWeb = false, bool isValidationCorrection = false);

  }
}
