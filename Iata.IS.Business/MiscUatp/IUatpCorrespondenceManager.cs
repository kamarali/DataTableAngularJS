using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Business.MiscUatp
{
    public interface IUatpCorrespondenceManager
    {
        /// <summary>
        /// Creates the Uatp. Correspondence.
        /// </summary>
        /// <param name="miscCorrespondence">The Uatp correspondence.</param>
        /// <returns></returns>
        MiscCorrespondence AddCorrespondence(MiscCorrespondence miscCorrespondence);

        /// <summary>
        /// Updates the  Uatp. Correspondence.
        /// </summary>
        /// <param name="miscCorrespondence">The Uatp Correspondence.</param>
        /// <returns></returns>
        MiscCorrespondence UpdateCorrespondence(MiscCorrespondence miscCorrespondence);

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
        MiscCorrespondence GetCorrespondenceDetails(string correspondenceId);

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id
        /// </summary>
        /// <param name="correspondenceNumber">correspondence id To Be fetched..</param>
        /// <returns></returns>
        MiscCorrespondence GetCorrespondenceDetails(long? correspondenceNumber);

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
        List<MiscUatpCorrespondenceAttachment> GetMiscCorrespondenceAttachments(List<Guid> attachmentIds);

        /// <summary>
        /// Add Misc Correspondence Attachment record
        /// </summary>
        /// <param name="attach">Uatp Correspondence Attachment record</param>
        /// <returns></returns>
        MiscUatpCorrespondenceAttachment AddRejectionMemoAttachment(MiscUatpCorrespondenceAttachment attach);

        /// <summary>
        /// Updates the correspondence attachment.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        IList<MiscUatpCorrespondenceAttachment> UpdateMiscCorrespondenceAttachment(IList<Guid> attachments, Guid parentId);

        /// <summary>
        /// Gets the correspondence attachment detail.
        /// </summary>
        /// <param name="attachmentId">The attachment id.</param>
        /// <returns></returns>
        MiscUatpCorrespondenceAttachment GetMiscCorrespondenceAttachmentDetail(string attachmentId);


        /// <summary>
        /// Determines whether specified file name already exists for given correspondence.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="correspondenceId">The correspondence id.</param>
        /// <returns>
        /// true if specified file name found in repository; otherwise, false.
        /// </returns>
        bool IsDuplicateMiscCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId);

        /// <summary>
        /// Validates the correspondence.
        /// </summary>
        /// <param name="correspondence">The correspondence Id.</param>
        /// <returns></returns>
        bool ValidateCorrespondence(MiscCorrespondence correspondence);

        /// <summary>
        /// Retrieves Correspondence History List
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId);

        /// <summary>
        /// Retrieve Correspondence Rejection List
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        IList<MiscUatpInvoice> GetCorrespondenceRejectionList(string invoiceId);

        /// <summary>
        /// Function to retrieve invoice details of the given correspondence id
        /// </summary>
        /// <param name="invoiceId">invoice id To Be fetched..</param>
        /// <returns></returns>
        MiscUatpInvoice GetInvoiceDetail(string invoiceId);

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

        /// <summary>
        /// Function to send correspondence email
        /// </summary>
        /// <returns> bool </returns>
        bool SendCorrespondenceMail(string correspondPageUrl, string to, string subject);

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
        MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId);

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
        MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId, int billingMemberId);

        /// <summary>
        /// Retrieve Correspondence History List
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="billingMemberId"></param>
        /// <returns></returns>
        IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId, int billingMemberId);

        /// <summary>
        /// Function to retrieve correspondence number of the given member id
        /// </summary>
        /// <param name="memberId">Member id .</param>
        /// <returns></returns>
        bool IsFirstCorrespondence(int memberId);

        List<ExpiredCorrespondence> UpdateCorrespondenceStatus();

        string CreateCorrespondenceFormatPdf(string correspondenceId);
        MiscCorrespondence GetFirstCorrespondenceDetails(string invoiceId);

        string IsCorrespondenceInvoiceExistsForCorrespondence(int billingMemberId, long correspondenceRefNumber);

        bool IsCorrespondenceOutSideTimeLimit(string invoiceId);
    }
}
