using System;
using System.Collections.Generic;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using System.Linq;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Base;

namespace Iata.IS.Business.Pax
{
    public interface INonSamplingInvoiceManager : IInvoiceManager
    {
        /// <summary>
        /// Gets the list of Prime Billing Coupon records 
        /// </summary>
        /// <param name="invoiceId">string of the invoice whose prime billing records are to be fetched</param>
        /// <returns>List of Coupon Records</returns>
        /* SCP ID : 71798 - US Form D Entry - F&F
        Date: 25-02-2013
        Desc: Method return type is changed from IList<> to IQueryable<> as part of code optimazation. 
        IList<> actually executes the query on database; On the contrary IQueryable<> differe the query execution using entity framework.
        Differed query execution is advantageous as it select only required number of rows for binding to grid.
        */
        IQueryable<PrimeCoupon> GetPrimeBillingCouponList(string invoiceId);

        /// <summary>
        /// Get the details of a particular coupon record
        /// </summary>
        /// <param name="couponRecordId">string of the coupon record</param>
        /// <returns>Details of a coupon record</returns>
        PrimeCoupon GetCouponRecordDetails(string couponRecordId);

        /// <summary>
        /// Adds a coupon record to the database
        /// </summary>
        /// <param name="couponRecord">Coupon Record to add</param>
        /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
        /// <returns>Added coupon record</returns>
        PrimeCoupon AddCouponRecord(PrimeCoupon couponRecord, out string duplicateCouponErrorMessage);

        /// <summary>
        /// Updates a coupon record in the database
        /// </summary>
        /// <param name="couponRecord">Coupon Record to update</param>
        /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
        /// <returns>Updated coupon record</returns>
        PrimeCoupon UpdateCouponRecord(PrimeCoupon couponRecord, out string duplicateCouponErrorMessage, int submissionMethod = 0);

        /// <summary>
        /// Deletes a coupon record from the database
        /// </summary>
        /// <param name="couponRecordId">string of the coupon record to delete</param>
        /// <returns>True if successfully deleted, false otherwise</returns>
        bool DeleteCouponRecord(string couponRecordId);

        /// <summary>
        /// To get the list of billing memo records
        /// </summary>
        /// <param name="invoiceId">string if the invoice</param>
        /// <returns>list of the billing memo records</returns>
        IList<BillingMemo> GetBillingMemoList(string invoiceId);

        /// <summary>
        /// To get the billing memo record details
        /// </summary>
        /// <param name="billingMemoRecordId">string if the billing memo record</param>
        /// <returns>Details of the billing memo record matching with billingMemoRecordId</returns>
        BillingMemo GetBillingMemoRecordDetails(string billingMemoRecordId);

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      /// <summary>
      /// To add the billing memo record to the database
      /// </summary>
      /// <param name="billingMemo">Details of the billing memo record</param>
      /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
      /// <returns>Added billing record</returns>
      BillingMemo AddBillingMemoRecord(BillingMemo billingMemo, bool isNullCorrRefNo = false);

      /// <summary>
      /// To update the billing memo record
      /// </summary>
      /// <param name="billingMemo">Details of the billing memo record</param>
      /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
      /// <returns>Updated billing memo record</returns>
      BillingMemo UpdateBillingMemoRecord(BillingMemo billingMemo, bool isNullCorrRefNo = false);

        /// <summary>
        /// To delete the billing memo record from database
        /// </summary>
        /// <param name="billingMemoRecordId">string of the billing memo record</param>
        /// <returns>True if successfully deleted,false otherwise</returns>
        bool DeleteBillingMemoRecord(string billingMemoRecordId);

        /// <summary>
        /// Get billing memo attachment details 
        /// </summary>
        /// <param name="attachmentId">attachment Id</param>
        /// <returns></returns>
        BillingMemoAttachment GetBillingMemoAttachmentDetails(string attachmentId);

        /// <summary>
        /// Insert billing memo attachment record
        /// </summary>
        /// <param name="attach">attachment record</param>
        /// <returns></returns>
        BillingMemoAttachment AddBillingMemoAttachment(BillingMemoAttachment attach);

        /// <summary>
        /// Update billing memo attachment record parent id
        /// </summary>
        /// <param name="attachments">list of attachment</param>
        /// <param name="parentId">billing memo Id</param>
        /// <returns></returns>
        IList<BillingMemoAttachment> UpdateBillingMemoAttachment(IList<Guid> attachments, Guid parentId);

        /// <summary>
        /// Check for duplicate file name for billing memo attachment
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="billingMemoId">billing memo id</param>
        /// <returns></returns>
        bool IsDuplicateBillingMemoAttachmentFileName(string fileName, Guid billingMemoId);

        /// <summary>
        /// To get the Coupon breakdown record list for billing memo
        /// </summary>
        /// <param name="memoRecordId">string of the memo record</param>
        /// <returns>List of the coupon records</returns>
        IList<BMCoupon> GetBillingMemoCouponList(string memoRecordId);

        /// <summary>
        /// To get the coupon details corresponding to a coupon record.
        /// </summary>
        /// <param name="couponBreakdownRecordId">string of the coupon breakdown list</param>
        /// <returns></returns>
        BMCoupon GetBillingMemoCouponDetails(string couponBreakdownRecordId);

        /// <summary>
        /// To add coupon record to specific billing memo
        /// </summary>
        /// <param name="billingMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="duplicateErrorMessage">The duplicate error message.</param>
        /// <returns>Add coupon breakdown record</returns>
        BMCoupon AddBillingMemoCouponDetails(BMCoupon billingMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage);

        /// <summary>
        /// To update coupon record of a specific billing memo
        /// </summary>
        /// <param name="billingMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="duplicateErrorMessage">The duplicate error message.</param>
        /// <returns>Updated coupon breakdown record</returns>
        BMCoupon UpdateBillingMemoCouponDetails(BMCoupon billingMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage);

        List<CargoBillingCodeSubTotalVat> GetBillingCodeVatTotal(string billingCodeSubVatTotalId);

        /// <summary>
        /// To delete coupon record corresponding to a billing memo
        /// </summary>
        /// <param name="couponBreakdownRecordId">string if the coupon breakdown record</param>
        /// <returns>True if successfully deleted,false otherwise</returns>
        bool DeleteBillingMemoCouponRecord(string couponBreakdownRecordId, out Guid billingMemoId, out Guid invoiceId);

        /// <summary>
        /// Get billing memo coupon record attachment details
        /// </summary>
        /// <param name="attachmentId">attachment id</param>
        /// <returns></returns>
        BMCouponAttachment GetBillingMemoCouponAttachmentDetails(string attachmentId);

        /// <summary>
        /// Add billing memo coupon attachment record
        /// </summary>
        /// <param name="attach">attachment record</param>
        /// <returns></returns>
        BMCouponAttachment AddBillingMemoCouponAttachment(BMCouponAttachment attach);

        /// <summary>
        /// Update parent id of attachment records with given list of GUID
        /// </summary>
        /// <param name="attachments">list of GUID of attachment</param>
        /// <param name="parentId">billing memo coupon id</param>
        /// <returns></returns>
        IList<BMCouponAttachment> UpdateBillingMemoCouponAttachment(IList<Guid> attachments, Guid parentId);

        /// <summary>
        /// Check for duplicate file name for Billing memo coupon attachment
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="billingMemoCouponId">billing Memo Coupon Id</param>
        /// <returns></returns>
        bool IsDuplicateBillingMemoCouponAttachmentFileName(string fileName, Guid billingMemoCouponId);

        /// <summary>
        /// Gets the list of rejection memo records corresponding to an invoice number
        /// </summary>
        /// <param name="invoiceId">invoice number of invoice for which rejection memo records need to be fetched</param>
        /// <returns>collection of rejection memo records</returns>
        IList<RejectionMemo> GetRejectionMemoList(string invoiceId);

        /// <summary>
        /// Gets details of rejection memo record corresponding to passed rejection memo record Id
        /// </summary>
        /// <param name="rejectionMemoRecordId">string of the rejection Memo record</param>
        /// <returns>Details of rejection memo record</returns>
        RejectionMemo GetRejectionMemoRecordDetails(string rejectionMemoRecordId);

        /// <summary>
        /// To add rejection memo record to the database
        /// </summary>
        /// <param name="rejectionMemoRecord">Details of the rejection memo record</param>
        /// <returns>Added rejection memo record</returns>
        RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord);

        /// <summary>
        /// Add RejectionMemo Record. Used to save RM to display error message while adding RM Coupon
        /// </summary>
        /// <param name="rejectionMemoRecord">RejectionMemoRecord to be added.</param>
        /// <param name="linkingErrorMessage">Error message in RM coupon linking</param>
        /// <param name="warningMessage">Warning message in RM coupon linking</param>
        /// <returns></returns>
        RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string linkingErrorMessage, out string warningMessage);

        /// <summary>
        /// To update rejection memo record
        /// </summary>
        /// <param name="rejectionMemoRecord">Details of the rejection memo record</param>
        /// <param name="warningMessage">Warning messsage for ignoring linking validation errors.</param>
        /// <returns>Updated rejection memo record</returns>
        RejectionMemo UpdateRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage);

        /// <summary>
        /// To delete rejection memo record from the database
        /// </summary>
        /// <param name="rejectionMemoRecordId">string of the rejection memo record</param>
        /// <returns>True is record is successfully added, false otherwise</returns>
        bool DeleteRejectionMemoRecord(string rejectionMemoRecordId);

        /// <summary>
        /// Get rejection memo attachment details
        /// </summary>
        /// <param name="attachmentId">attachment Id</param>
        /// <returns></returns>
        RejectionMemoAttachment GetRejectionMemoAttachmentDetails(string attachmentId);

        /// <summary>
        /// Add rejection memo attachment record
        /// </summary>
        /// <param name="attach">rejection meo attachment record</param>
        /// <returns></returns>
        RejectionMemoAttachment AddRejectionMemoAttachment(RejectionMemoAttachment attach);

        /// <summary>
        /// Update parent id of rejection memo attachment record for given Guids
        /// </summary>
        /// <param name="attachments">list of Guid of rejection memo attachment record</param>
        /// <param name="parentId">rejection memo id</param>
        /// <returns></returns>
        IList<RejectionMemoAttachment> UpdateRejectionMemoAttachment(IList<Guid> attachments, Guid parentId);

        /// <summary>
        /// Check for duplicate file name of rejection memo attachment
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="rejectionMemoId">rejection Memo Id</param>
        /// <returns></returns>
        bool IsDuplicateRejectionMemoAttachmentFileName(string fileName, Guid rejectionMemoId);

        /// <summary>
        /// To get the Coupon breakdown record list for rejection memo
        /// </summary>
        /// <param name="memoRecordId">string of the memo record</param>
        /// <returns>List of the coupon records</returns>
        IList<RMCoupon> GetRejectionMemoCouponBreakdownList(string memoRecordId);

        /// <summary>
        /// To get the coupon details to a coupon record
        /// </summary>
        /// <param name="couponBreakdownRecordId">string of the coupon breakdown list</param>
        /// <returns></returns>
        RMCoupon GetRejectionMemoCouponDetails(string couponBreakdownRecordId);

        /// <summary>
        /// To add coupon record to specific rejection memo
        /// </summary>
        /// <param name="rejectionMemoCouponBreakdownRecord">Add the coupon breakdown records</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="duplicateErrorMessage">The duplicate error message.</param>
        /// <param name="isFromBillingHistory">Indicates whether method is called when creating RM from billing history screen.</param>
        /// <returns>Add coupon breakdown record</returns>
        RMCoupon AddRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage, bool isFromBillingHistory = false);

        /// <summary>
        /// To update coupon record of a specific rejection memo
        /// </summary>
        /// <param name="rejectionMemoCouponBreakdownRecord">Update the coupon breakdown record</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="duplicateErrorMessage">The duplicate error message.</param>
        /// <returns>Updated coupon breakdown record</returns>
        RMCoupon UpdateRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage);

        /// <summary>
        /// To delete coupon record corresponding to a rejection memo
        /// </summary>
        /// <param name="couponBreakdownRecordId">string if the coupon breakdown record</param>
        /// <param name="rejectionMemoId"></param>
        /// <param name="invoiceId"></param>
        /// <returns>True if successfully deleted,false otherwise</returns>
        bool DeleteRejectionMemoCouponRecord(string couponBreakdownRecordId, out Guid rejectionMemoId, out Guid invoiceId);

        /// <summary>
        /// Get rejection memo Coupon attachment details
        /// </summary>
        /// <param name="attachmentId">attachment Id</param>
        /// <returns></returns>
        RMCouponAttachment GetRejectionMemoCouponAttachmentDetails(string attachmentId);

        /// <summary>
        /// Add rejection memo Coupon attachment record
        /// </summary>
        /// <param name="attach">rejection memo Coupon attachment record</param>
        /// <returns></returns>
        RMCouponAttachment AddRejectionMemoCouponAttachment(RMCouponAttachment attach);

        /// <summary>
        /// Update parent id of rejection memo Coupon attachment record for given Guids
        /// </summary>
        /// <param name="attachments">list of Guid of rejection memo Coupon attachment record</param>
        /// <param name="parentId">rejection memo id</param>
        /// <returns></returns>
        IList<RMCouponAttachment> UpdateRejectionMemoCouponAttachment(IList<Guid> attachments,
                                                                          Guid parentId);

        /// <summary>
        /// Check for duplicate file name of rejection memo Coupon attachment
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="rejectionMemoCouponId">rejection Memo Coupon Id</param>
        /// <returns></returns>
        bool IsDuplicateRejectionMemoCouponAttachmentFileName(string fileName, Guid rejectionMemoCouponId);

        /// <summary>
        /// Function to retrieve Attachment details
        /// </summary>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        PrimeCouponAttachment GetCouponLevelAttachmentDetails(string attachmentId);

        /// <summary>
        /// To add coupon level Attachment 
        /// </summary>
        /// <param name="attach">Coupon level Attachment details</param>
        /// <returns>Details of the added Attachment record</returns>
        PrimeCouponAttachment AddCouponLevelAttachment(PrimeCouponAttachment attach);

        /// <summary>
        /// Update attachment record Parent Id. Assign parent id after coupon record is created
        /// </summary>
        /// <param name="attachments">Attachment List</param>
        /// <param name="parentId">coupon record Id</param>
        /// <returns></returns>
        IList<PrimeCouponAttachment> UpdateCouponRecordAttachment(IList<Guid> attachments, Guid parentId);

        /// <summary>
        /// Check whether coupon record attachment file name is duplicate
        /// </summary>
        /// <param name="fileName">File Name</param>
        /// <param name="couponId">Coupon Id</param>
        /// <returns></returns>
        bool IsDuplicateCouponAttachmentFileName(string fileName, Guid couponId);

        //Rejection memo coupon level methods

        /// <summary>
        /// Gets the rejection memo coupon breakdown count.
        /// </summary>
        /// <param name="memoRecordId">The memo record id.</param>
        /// <returns></returns>
        long GetRejectionMemoCouponBreakdownCount(string memoRecordId);

        /// <summary>
        /// Gets the billing memo coupon breakdown count.
        /// </summary>
        /// <param name="memoRecordId">The memo record id.</param>
        /// <returns></returns>
        long GetBillingMemoCouponCount(string memoRecordId);

        /// <summary>
        /// Determines whether [is transaction exists] [the specified invoice id].
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// 	<c>true</c> if [is transaction exists] [the specified invoice id]; otherwise, <c>false</c>.
        /// </returns>
        bool IsTransactionExists(string invoiceId);

        /// <summary>
        /// Gets the credit memo attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        List<PrimeCouponAttachment> GetCouponRecordAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Gets the credit memo attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        List<BillingMemoAttachment> GetBillingMemoAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Gets the credit memo coupon attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        List<BMCouponAttachment> GetBillingMemoCouponAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Gets the credit memo coupon attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        List<RejectionMemoAttachment> GetRejectionMemoAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Gets the credit memo coupon attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        List<RMCouponAttachment> GetRejectionMemoCouponAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Get linking details for rejection memo
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        RMLinkingResultDetails GetRejectionMemoLinkingDetails(RMLinkingCriteria criteria);

        /// <summary>
        /// Get linking details for rejection memo when multiple records are found for rejected entity then as per user selection fetch data for selected memo
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria);

        IList<PrimeCoupon> GetPrimeBillingCouponList(string[] rejectionIdList);

        /// <summary>
        /// get RM Coupon break down record details
        /// </summary>
        RMLinkedCouponDetails GetRMCouponBreakdownRecordDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId);

        /// <summary>
        /// Get the single record details from the list of RM coupon
        /// </summary>
        RMLinkedCouponDetails GetRMCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId);

        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        ExistingBMTransactionDetails GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId);

        bool IsBillingMemoInvoiceOutSideTimeLimit(string correspondenceRefNumber, bool authorityToBill, int correspondenceStatusId, DateTime correspondenceDate);

        /// <summary>
        /// Following method is used to create VelocityContext for creating AuditTrail.html file, from which we create AuditTrail.pdf file.
        /// </summary>
        /// <param name="auditTrail">AuditTrail details</param>
        /// <returns>AuditTrail.html file</returns>
        string GeneratePaxBillingHistoryAuditTrailPdf(PaxAuditTrail auditTrail);

        /// <summary>
        /// Following method is executed from .vm file to check whether RejectionMemo is already displayed while creating .pdf file.  
        /// </summary>
        /// <param name="rejectionMemoString">String of RejectionMemo Id's</param>
        /// <param name="rejectionMemoId">RejectionMemo Id, to check whether it is displayed</param>
        /// <returns>Returns "Yes" if RejectionMemo is already displayed, else returns "No"</returns>
        string IsRejectionMemoDisplayed(string rejectionMemoString, Guid rejectionMemoId);

        /// <summary>
        /// Following method retrieves Stage2 RejectionMemo details
        /// </summary>
        /// <param name="memoList">RejectionMemo List</param>
        /// <param name="rejectionMemo">RejectionMemo to find in list</param>
        /// <returns>Stage2 RejectionMemo</returns>
        RejectionMemo GetRejectionStage2MemoDetails(List<RejectionMemo> memoList, RejectionMemo rejectionMemo);

        /// <summary>
        /// Following method retrieves Stage1 RejectionMemo details
        /// </summary>
        /// <param name="memoList">RejectionMemo List</param>
        /// <param name="stage2RM">Stage2 RejectionMemo to find in list</param>
        /// <returns>Stage1 RejectionMemo</returns>
        RejectionMemo GetRejectionStage1MemoDetails(List<RejectionMemo> memoList, RejectionMemo stage2RM);

        /// <summary>
        /// Following method is executed from PaxbillingHistoryAuditTrail.vm file to return Month name from month number
        /// </summary>
        /// <param name="monthNumber">Month number whose name is to be retrieved</param>
        /// <returns>Month name</returns>
        string GetAbbreviatedMonthName(int monthNumber);

        /// <summary>
        /// Following method sorts Correspondence details in descending order depending on stage. Executed from .vm file 
        /// </summary>
        /// <param name="rejectionMemo">Rejection memo object</param>
        /// <returns>Rejection memo with correspondence in sorted in descending order</returns>
        RejectionMemo GetCorrespondenceDetails(RejectionMemo rejectionMemo);

        /// <summary>
        /// Get the reason code list for transaction tyes - BillingMemo, PasNsBillingMemoDueToAuthorityToBill, PasNsBillingMemoDueToExpiry.
        /// </summary>
        /// <returns></returns>
        IList<ReasonCode> GetReasonCodeListForBillingMemo();

        /// <summary>
        /// Following method is used to break Reason remarks text int string of 80 characters. Used in Pax pdf generation 
        /// </summary>
        /// <param name="reasonRemarks">reason remarks text</param>
        /// <returns>String broken into 80 characters</returns>
        string CreateReasonRemarksString(string reasonRemarks);
        
        /// <summary>
        /// This method returns auto billing coupon objects which match search criteria specified in SearchCriteria object passed as input parameter.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        IQueryable<PrimeCoupon> GetAutoBillingPrimeCouponList(AutoBillingSearchCriteria searchCriteria);

        /// <summary>
        /// This method will be used to track ISR response from AIA.
        /// </summary>
        void TrackIsrResponse();

        /// <summary>
        /// To be used in Pax Auto Billing performance report
        /// </summary>
        /// <param name="members"></param>
        /// <param name="currency"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <returns></returns>
        List<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceData(List<Member> members, Currency currency, int billingMonth, int billingYear);

        //CMP508:Audit Trail Download with Supporting Documents
        /// <summary>
        /// Returns Html string for audit trail with supporting docs assigned with their folder numbers
        /// </summary>
        /// <param name="auditTrail">audit trail for which html is to be genereated</param>
        /// <param name="suppDocs">out parameter for Supp Docs</param>
        /// <returns>Html for audit trail</returns>
        string GeneratePaxBillingHistoryAuditTrailPackage(PaxAuditTrail auditTrail,
                                                          out Dictionary<Attachment, int> suppDocs);


        /// <summary>
        /// This function is used to validate RM Source Codes
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        //CMP614: Source Code Validation for PAX RMs.
        String ValidateRMSourceCode(RMSourceCodeValidationCriteria criteria);
    }
}
