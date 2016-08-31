using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Pax
{
  /// <summary>
  /// A credit note  is a type of invoice that can only have credit memos.
  /// 
  /// </summary>
  public interface INonSamplingCreditNoteManager : IInvoiceManager
  {
    /// <summary>
    /// To get the list of credit memo records
    /// </summary>
    /// <param name="creditNoteId">string of the credit Note</param>
    /// <returns>List of credit memo records </returns>
    IList<CreditMemo> GetCreditMemoList(string creditNoteId);

    /// <summary>
    /// To get the details of credit memo record
    /// </summary>
    /// <param name="creditMemoId">string if the credit memo record</param>
    /// <returns>Details of the credit memo record</returns>
    CreditMemo GetCreditMemoRecordDetails(string creditMemoId);

    /// <summary>
    /// To add credit memo record to the database
    /// </summary>
    /// <param name="creditMemoRecord">credit memo details</param>
    /// <returns>Details of the added credit memo record</returns>
    CreditMemo AddCreditMemoRecord(CreditMemo creditMemoRecord);

    /// <summary>
    /// To update the credit memo record
    /// </summary>
    /// <param name="creditMemoRecord">Details of the credit memo record</param>
    /// <returns>Details of the updated credit memo record</returns>
    CreditMemo UpdateCreditMemoRecord(CreditMemo creditMemoRecord);

    /// <summary>
    /// To delete the credit memo record from the database
    /// </summary>
    /// <param name="creditMemoId">string of the credit memo</param>
    /// <returns>True id deleted successfully, false otherwise</returns>
    bool DeleteCreditMemoRecord(string creditMemoId);

    /// <summary>
    /// Get credit memo attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    CreditMemoAttachment GetCreditMemoAttachmentDetails(string attachmentId);

    /// <summary>
    /// Add credit memo attachment
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    CreditMemoAttachment AddCreditMemoAttachment(CreditMemoAttachment attach);

    /// <summary>
    /// Update parent id for credit memo attachment record with given Guid
    /// </summary>
    /// <param name="attachments">list of Guid of attachment records</param>
    /// <param name="parentId">credit memo id</param>
    /// <returns></returns>
    IList<CreditMemoAttachment> UpdateCreditMemoAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Check for duplicate filename for credit memo attachment reocrds
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="creditMemoId">credit memo id</param>
    /// <returns></returns>
    bool IsDuplicateCreditMemoAttachmentFileName(string fileName, Guid creditMemoId);

    /// <summary>
    /// To get the Coupon breakdown record list for rejection memo
    /// </summary>
    /// <param name="memoRecordId">string of the memo record</param>
    /// <returns>List of the coupon records</returns>
    IList<CMCoupon> GetCreditMemoCouponBreakdownList(string memoRecordId);

    /// <summary>
    /// To get the coupon details to a coupon record
    /// </summary>
    /// <param name="couponBreakdownRecordId">string of the coupon breakdown list</param>
    /// <returns></returns>
    CMCoupon GetCreditMemoCouponDetails(string couponBreakdownRecordId);

    /// <summary>
    /// To add coupon record to specific Credit memo
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Added coupon breakdown record</returns>
    CMCoupon AddCreditMemoCouponRecord(CMCoupon creditMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage);

    /// <summary>
    /// To update coupon record of a specific Credit memo
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Updated coupon breakdown record</returns>
    CMCoupon UpdateCreditMemoCouponRecord(CMCoupon creditMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage);

    /// <summary>
    /// To delete coupon record corresponding to a Credit memo
    /// </summary>
    /// <param name="couponBreakdownRecordId">string if the coupon breakdown record</param>
    /// <returns>True if successfully deleted, false otherwise</returns>
    bool DeleteCreditMemoCouponRecord(string couponBreakdownRecordId, ref Guid creditMemoId, ref Guid invoiceId);

    /// <summary>
    /// Get credit memo Coupon attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    CMCouponAttachment GetCreditMemoCouponAttachmentDetails(string attachmentId);

    /// <summary>
    /// Add credit memo attachment
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    CMCouponAttachment AddCreditMemoCouponAttachment(CMCouponAttachment attach);

    /// <summary>
    /// Update parent id for credit memo Coupon attachment record with given Guid
    /// </summary>
    /// <param name="attachments">list of Guid of attachment records</param>
    /// <param name="parentId">credit memo id</param>
    /// <returns></returns>
    IList<CMCouponAttachment> UpdateCreditMemoCouponAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Check for duplicate filename for credit memo Coupon attachment reocrds
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="creditMemoCouponId">credit Memo Coupon Id</param>
    /// <returns></returns>
    bool IsDuplicateCreditMemoCouponAttachmentFileName(string fileName, Guid creditMemoCouponId);

    /// <summary>
    /// Gets the credit memo coupon breakdown count.
    /// </summary>
    /// <param name="memoRecordId">The memo record id.</param>
    /// <returns></returns>
    long GetCreditMemoCouponBreakdownCount(string memoRecordId);

    /// <summary>
    /// Determines whether [is credit memo exists] [the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if [is credit memo exists] [the specified invoice id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsCreditMemoExists(string invoiceId);

    /// <summary>
    /// Validates the parsed credit memo record.
    /// </summary>
    /// <param name="creditMemoRecord">The credit memo record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="airlineFlightDesignator"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="exchangeRate"></param>
    /// <returns></returns>
    bool ValidateParsedCreditMemoRecord(CreditMemo creditMemoRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> airlineFlightDesignator, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate, ExchangeRate exchangeRate, MaxAcceptableAmount maxAcceptableAmount);

    /// <summary>
    /// Gets the credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CreditMemoAttachment> GetCreditMemoAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Gets the credit memo coupon attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CMCouponAttachment> GetCreditMemoCouponAttachments(List<Guid> attachmentIds);
 
  }

}