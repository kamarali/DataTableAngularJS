using System;
using System.Collections.Generic;
using Iata.IS.Business.Common;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Business.Cargo
{
  public interface ICargoCorrespondenceManager: ICorrespondenceManager
  {
    /// <summary>
    /// Creates the misc. Correspondence.
    /// </summary>
    /// <param name="cargoCorrespondence">The misc correspondence.</param>
    /// <returns></returns>
    CargoCorrespondence AddCorrespondence(CargoCorrespondence cargoCorrespondence);

    /// <summary>
    /// Updates the  misc. Correspondence.
    /// </summary>
    /// <param name="cargoCorrespondence">The misc Correspondence.</param>
    /// <returns></returns>
    CargoCorrespondence UpdateCorrespondence(CargoCorrespondence cargoCorrespondence);

    /* /// <summary>
    /// Deletes the correspondence.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    bool DeleteCorrespondence(string correspondenceId); */

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceId">correspondence id To Be fetched..</param>
    /// <returns></returns>
    CargoCorrespondence GetCorrespondenceDetails(string correspondenceId);


    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceId">correspondence id To Be fetched..</param>
    /// <returns></returns>
    CargoCorrespondence GetCorrespondenceHeaderDetails(string correspondenceId);

   
    /* /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceNumber">correspondence id To Be fetched..</param>
    /// <returns></returns>
    CargoCorrespondence GetCorrespondenceDetails(long? correspondenceNumber);

    /// <summary>
    /// Determines whether transaction exists for the specified correspondence id
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    bool IsTransactionExists(string correspondenceId);*/

    /// <summary>
    /// Gets the correspondence attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CargoCorrespondenceAttachment> GetCorrespondenceAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Add Misc Correspondence Attachment record
    /// </summary>
    /// <param name="attach">Misc Correspondence Attachment record</param>
    /// <returns></returns>
    CargoCorrespondenceAttachment AddCorrespondenceAttachment(CargoCorrespondenceAttachment attach);

    /*/// <summary>
    /// Updates the correspondence attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    //IList<MiscUatpCorrespondenceAttachment> UpdateMiscCorrespondenceAttachment(IList<Guid> attachments, Guid parentId); */

    /// <summary>
    /// Gets the correspondence attachment detail.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    CargoCorrespondenceAttachment GetCorrespondenceAttachmentDetail(string attachmentId);

    /// <summary>
    /// Determines whether specified file name already exists for given correspondence.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// true if specified file name found in repository; otherwise, false.
    /// </returns>
    bool IsDuplicateCargoCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId);

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="correspondence">The correspondence Id.</param>
    /// <returns></returns>
    bool ValidateCorrespondence(CargoCorrespondence correspondence);

    /* /// <summary>
    /// Retrieves Correspondence History List
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    IList<CargoCorrespondence> GetCorrespondenceHistoryList(string invoiceId); */

    /// <summary>
    /// Retrieve Correspondence Rejection List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    IList<CargoRejectionMemo> GetCorrespondenceRejectionList(string invoiceId);

    /// <summary>
    /// Function to retrieve invoice details of the given correspondence id
    /// </summary>
    /// <param name="invoiceId">invoice id To Be fetched..</param>
    /// <returns></returns>
    CargoInvoice GetInvoiceDetail(string invoiceId);

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

    // CMP#657: Retention of Additional Email Addresses in Correspondences.
    // Logic Moved to common location. i.e in corresponceManager.cs
    ///// <summary>
    ///// Function to send correspondence email
    ///// </summary>
    //bool SendCorrespondenceMail(string correspondPageUrl, string to, string subject);
    
    /* /// <summary>
    /// Get the charge codes for provided invoice Id .
    /// </summary>
    /// <param name="invoiceId">The invoice Id.</param>
    /// <returns></returns>
    string GetChargeCodes(string invoiceId); */

    /// <summary>
    /// Retrieve recent Correspondence Id for provided invoice id. 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    CargoCorrespondence GetRecentCorrespondenceDetails(string invoiceId);

    /// <summary>
    /// Get toemailIds of correspondence contact of the to member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="processingContact"></param>
    /// <returns></returns>
    string GetToEmailIds(int memberId, ProcessingContactType processingContact);

    /* Contact GetContactDetails(int memberId, ProcessingContactType processingContact); */

    int GetNumericMemberCode(string memberCode);

    /* /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="toEmailIds"></param>
    /// <returns></returns>
    bool ValidateToEmailIds(string toEmailIds); */

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id - If billing member is To Member of the correspondence then he should not allow to view the Saved and Ready to Submit correspondences of the other member.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    CargoCorrespondence GetRecentCorrespondenceDetails(string invoiceId, int billingMemberId);

    /// <summary>
    /// Retrieve Correspondence History List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    IList<CargoCorrespondence> GetCorrespondenceHistoryList(string invoiceId, int billingMemberId);

    /// <summary>
    /// Function to retrieve correspondence number of the given member id
    /// </summary>
    /// <param name="memberId">Member id .</param>
    /// <returns></returns>
    bool IsFirstCorrespondence(int memberId);

    List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold);

    string CreateCorrespondenceFormatPdf(string correspondenceId);
    CargoCorrespondence GetFirstCorrespondenceDetails(string correspondenceId);
    CargoCorrespondence GetLastCorrespondenceDetails(long? correspondenceRefNo);
    CargoRejectionMemo GetRejectedMemoDetails(string rejectionMemoId);
    void UpdateRejectedMemo(CargoRejectionMemo rejectionMemo);

    IList<CargoCorrespondenceAttachment> UpdateCargoCorrespondenceAttachment(List<Guid> correspondenceAttachmentIds, Guid correspondenceId);
    CargoInvoice GetCorrespondenceRelatedInvoice(string correspondenceId);
    CargoCorrespondence GetOriginalCorrespondenceDetails(long? correspondenceNumber);
    // IList<CargoCorrespondence> GetCorrespondenceHistoryList(string correspondenceId, int billingMemberId);
    CargoCorrespondence GetRecentCorrespondenceDetails(long? correspondenceNumber);
    /// <summary>
    /// Fix bug no 8881 for CMP 527
    /// Get Correpondence details including correspondence whose sub status is closed by initiator.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    CargoCorrespondence GetRecentCorrespondenceDetailWithClosedStatus(long? correspondenceNumber);
    bool IsCorrespondenceOutsideTimeLimit(string invoiceId, ref bool isTimeLimitRecordFound);


    /// <summary>
    /// This method creates Pdf report for Cargo Correspondences
    /// Will be called from service
    /// </summary>
    /// <param name="message"></param>
    /// <param name="basePath"></param>
    string CreateCgoCorrespondenceTrailPdf(ReportDownloadRequestMessage message, string basePath);

    /// <summary>
    /// This method creates a messages in queue for correspondence trail report
    /// Will be called from Web
    /// </summary>
    /// <param name="message"></param>
    //CMP508: Audit Trail Download with Supporting Documents
    //void RequestCgoCorrespondenceTrailPdf(TransactionTrailReportRequestMessage message);

    /// <summary>
    /// Following method is used to Add Correspondence and update respective Rejection Memo
    /// </summary>
    /// <param name="cargoCorrespondence">Correspondence object</param>
    /// <param name="correspondenceAttachmentIds">Attachment list</param>
    /// <param name="rejectionMemoIds">Rejection memo id's string</param>
    /// <returns>Cargo correspondence object</returns>

    /* SCP106534: ISWEB No-02350000768 
        Desc: Added support for operation status parameter
        Date: 20/06/2013*/
      CargoCorrespondence AddCorrespondenceAndUpdateRejection(CargoCorrespondence cgoCorrespondence,
                                                              List<Guid> correspondenceAttachmentIds,
                                                              string rejectionMemoIds, ref int operationStatusIndicator);
    void UpdateExpiryDate(CargoCorrespondence correspondence, CargoInvoice invoice);

      /// <summary>
      /// Gets Only Correspondence from database Using Load Strategy. No other details like attachment, from member/ to member etc.. will be loaded.
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <returns></returns>
      CargoCorrespondence GetOnlyCorrespondenceUsingLoadStrategy(string correspondenceId);

      /// <summary>
      /// Gets a last responded correspondence from database. Useful for Close functionality in order to send emails 
      /// to all email Ids in previously responded state.
      /// </summary>
      /// <param name="correspondenceNumber"></param>
      /// <returns></returns>
      CargoCorrespondence GetLastRespondedCorrespondene(long correspondenceNumber);

  }
}
