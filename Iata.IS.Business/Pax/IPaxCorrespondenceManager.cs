using System;
using System.Collections.Generic;
using Iata.IS.Business.Common;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Pax
{
  public interface IPaxCorrespondenceManager : ICorrespondenceManager
  {
    /// <summary>
    /// Creates the misc. Correspondence.
    /// </summary>
    /// <param name="miscCorrespondence">The misc correspondence.</param>
    /// <returns></returns>
    Correspondence AddCorrespondence(Correspondence miscCorrespondence);

    /// <summary>
    /// Updates the  misc. Correspondence.
    /// </summary>
    /// <param name="miscCorrespondence">The misc Correspondence.</param>
    /// <returns></returns>
    Correspondence UpdateCorrespondence(Correspondence miscCorrespondence);

    /// <summary>
    /// Deletes the correspondence.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    bool DeleteCorrespondence(string correspondenceId);

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceId">correspondence id To Be fetched..</param>
    /// <returns></returns>
    Correspondence GetCorrespondenceDetails(string correspondenceId);

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
    /// </summary>
    /// <param name="correspondenceId">correspondence id To Be fetched..</param>
    /// <returns></returns>
    Correspondence GetCorrespondenceDetailsForSaveAndSend(string correspondenceId);

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceNumber">correspondence id To Be fetched..</param>
    /// <returns></returns>
    Correspondence GetCorrespondenceDetails(long? correspondenceNumber);

    /// <summary>
    /// Determines whether transaction exists for the specified correspondence id
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    bool IsTransactionExists(string correspondenceId);

    /// <summary>
    /// Gets the correspondence attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CorrespondenceAttachment> GetCorrespondenceAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Gets the pax correspondence attachments.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    List<CorrespondenceAttachment> GetPaxCorrespondenceAttachments(Guid correspondenceId);
    /// <summary>
    /// Add Misc Correspondence Attachment record
    /// </summary>
    /// <param name="attach">Misc Correspondence Attachment record</param>
    /// <returns></returns>
    CorrespondenceAttachment AddCorrespondenceAttachment(CorrespondenceAttachment attach);

    /// <summary>
    /// Updates the correspondence attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    //IList<MiscUatpCorrespondenceAttachment> UpdateMiscCorrespondenceAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Gets the correspondence attachment detail.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    CorrespondenceAttachment GetCorrespondenceAttachmentDetail(string attachmentId);

    /// <summary>
    /// Determines whether specified file name already exists for given correspondence.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// true if specified file name found in repository; otherwise, false.
    /// </returns>
    bool IsDuplicatePaxCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId);

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="correspondence">The correspondence Id.</param>
    /// <returns></returns>
    bool ValidateCorrespondence(Correspondence correspondence);

    /// <summary>
    /// Retrieves Correspondence History List
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IList<Correspondence> GetCorrespondenceHistoryList(string invoiceId);

    /// <summary>
    /// Retrieve Correspondence Rejection List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    IList<RejectionMemo> GetCorrespondenceRejectionList(string invoiceId);

    /// <summary>
    /// Function to retrieve invoice details of the given correspondence id
    /// </summary>
    /// <param name="invoiceId">invoice id To Be fetched..</param>
    /// <returns></returns>
    PaxInvoice GetInvoiceDetail(string invoiceId);


      /// <summary>
      /// Function to retrieve invoice details of the given invoice id
      /// </summary>
      /// <param name="invoiceId">invoice id To Be fetched..</param>
      /// <returns></returns>
      PaxInvoice GetInvoiceDetailFromSp(string invoiceId);

    /// <summary>
    /// Get the member of provided member Id .
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <returns></returns>
    Member GetMember(int memberId);

    /// <summary>
    /// Function to retrieve correspondence number of the given member id
    /// </summary>
    /// <param name="memberId">Member id .</param>
    /// <returns></returns>
    long GetCorrespondenceNumber(int memberId);

    long GetInitialCorrespondenceNumber(int memberId);

    // CMP#657: Retention of Additional Email Addressed in Correspondences.
    ///// <summary>
    ///// Function to send correspondence email
    ///// </summary>
    //bool SendCorrespondenceMail(string correspondPageUrl, string to, string subject);

    /// <summary>
    /// Get the charge codes for provided invoice Id .
    /// </summary>
    /// <param name="invoiceId">The invoice Id.</param>
    /// <returns></returns>
    string GetChargeCodes(string invoiceId);

    /// <summary>
    /// Retrieve recent Correspondence Id for provided invoice id. 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    Correspondence GetRecentCorrespondenceDetails(string invoiceId);

    /// <summary>
    /// Get toemailIds of correspondence contact of the to member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="processingContact"></param>
    /// <returns></returns>
    string GetToEmailIds(int memberId, ProcessingContactType processingContact);

    Contact GetContactDetails(int memberId, ProcessingContactType processingContact);

    int GetNumericMemberCode(string memberCode);

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="toEmailIds"></param>
    /// <returns></returns>
    bool ValidateToEmailIds(string toEmailIds);

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id - If billing member is To Member of the correspondence then he should not allow to view the Saved and Ready to Submit correspondences of the other member.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    Correspondence GetRecentCorrespondenceDetails(string invoiceId, int billingMemberId);

    /// <summary>
    /// Retrieve Correspondence History List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    IList<Correspondence> GetCorrespondenceHistoryList(string invoiceId, int billingMemberId);

    /// <summary>
    /// Function to retrieve correspondence number of the given member id
    /// </summary>
    /// <param name="memberId">Member id .</param>
    /// <returns></returns>
    bool IsFirstCorrespondence(int memberId);

    List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold);

    string CreateCorrespondenceFormatPdf(string correspondenceId);
    Correspondence GetFirstCorrespondenceDetails(string correspondenceId, bool withoutLoadStrategy = false);
    Correspondence GetLastCorrespondenceDetails(long? correspondenceRefNo);
    RejectionMemo GetRejectedMemoDetails(string rejectionMemoId);
    void UpdateRejectedMemo(RejectionMemo rejectionMemo);

    IList<CorrespondenceAttachment> UpdatePaxCorrespondenceAttachment(List<Guid> correspondenceAttachmentIds, Guid correspondenceId);
    //SCP210204: IS-WEB Outage
    PaxInvoice GetCorrespondenceRelatedInvoice(string correspondenceId,Correspondence correspondence=null);
    Correspondence GetOriginalCorrespondenceDetails(long? correspondenceNumber);
    Correspondence GetRecentCorrespondenceDetails(long? correspondenceNumber, bool getLastCorr = false);
    bool IsCorrespondenceOutsideTimeLimit(string invoiceId, ref bool isTimeLimitRecordFound);

    string CreatePaxCorrespondenceTrailPdf(ReportDownloadRequestMessage message, string basePath);

    //CMP508: Audit Trail Download with Supporting Documents
    //void RequestPaxCorrespondenceTrailPdf(TransactionTrailReportRequestMessage message);

    /* SCP106534: ISWEB No-02350000768 
    Desc: Added support for operation status parameter
    Date: 20/06/2013*/
      Correspondence AddCorrespondenceAndUpdateRejection(Correspondence paxCorrespondence,
                                                         List<Guid> correspondenceAttachmentIds, string rejectionMemoIds,
                                                         ref int operationStatusIndicator);

    void UpdateExpiryDate(Correspondence correspondence, PaxInvoice invoice);

      /// <summary>
      /// Gets Only Correspondence from database Using Load Strategy. No other details like attachment, from member/ to member etc.. will be loaded.
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <returns></returns>
      Correspondence GetOnlyCorrespondenceUsingLoadStrategy(string correspondenceId);

      Correspondence GetLastRespondedCorrespondene(long correspondenceNumber);
  }
}
