using System;
using System.Collections.Generic;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Pax
{
  public interface ISamplingRejectionManager : IInvoiceManager
  {

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
    /// <param name="warningMessage">Warning message</param>
    /// <returns>Added rejection memo record</returns>
    RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage);

    /// <summary>
    /// To update rejection memo record
    /// </summary>
    /// <param name="rejectionMemoRecord">Details of the rejection memo record</param>
    /// <param name="warningMessage">Warning message</param>
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
    RejectionMemoAttachment GetRejectionMemoAttachmentDetails(string attachmentId);

    /// <summary>
    /// Add rejection memo attachment record
    /// </summary>
    /// <param name="attach">rejection memo attachment record</param>
    RejectionMemoAttachment AddRejectionMemoAttachment(RejectionMemoAttachment attach);

    /// <summary>
    /// Update parent id of rejection memo attachment record for given Guids
    /// </summary>
    /// <param name="attachments">list of Guid of rejection memo attachment record</param>
    /// <param name="parentId">rejection memo id</param>
    /// <returns></returns>
    IList<RejectionMemoAttachment> UpdateRejectionMemoAttachment(List<Guid> attachments, Guid parentId);

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
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Add coupon breakdown record</returns>
    RMCoupon AddRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, out string duplicateErrorMessage);

    /// <summary>
    /// To update coupon record of a specific rejection memo
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">Update the coupon breakdown record</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Updated coupon breakdown record</returns>
    RMCoupon UpdateRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, out string duplicateErrorMessage);

    /// <summary>
    /// To delete coupon record corresponding to a rejection memo
    /// </summary>
    /// <param name="couponBreakdownRecordId">string if the coupon breakdown record</param>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// True if successfully deleted,false otherwise
    /// </returns>
    bool DeleteRejectionMemoCouponRecord(string couponBreakdownRecordId, ref Guid rejectionMemoId, ref Guid invoiceId);

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
    IList<RMCouponAttachment> UpdateRejectionMemoCouponAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Deletes the rejection memo coupon attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    bool DeleteRejectionMemoCouponAttachment(string attachmentId);

    /// <summary>
    /// Check for duplicate file name of rejection memo Coupon attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="rejectionMemoCouponId">rejection Memo Coupon Id</param>
    /// <returns></returns>
    bool IsDuplicateRejectionMemoCouponAttachmentFileName(string fileName, Guid rejectionMemoCouponId);

    /// <summary>
    /// Determines whether transaction exists for the specified rejection memo id.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// True if transaction exists for the specified rejection memo id; otherwise, false
    /// </returns>
    bool IsTransactionExists(string invoiceId);

    /// <summary>
    /// Determines whether [is coupon exists] [the specified rejection memo id].
    /// </summary>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <returns>
    /// 	True if coupon exists for the specified rejection memo id; otherwise, false.
    /// </returns>
    bool IsCouponExists(string rejectionMemoId);

    /// <summary>
    /// Gets the rejection memo attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<RejectionMemoAttachment> GetRejectionMemoAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Gets the rejection memo coupon attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<RMCouponAttachment> GetRejectionMemoCouponAttachments(List<Guid> attachmentIds);
  }
}
