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
using InvoiceSearchCriteria = Iata.IS.Model.Cargo.BillingHistory.InvoiceSearchCriteria;
using RejectionMemo = Iata.IS.Model.Cargo.CargoRejectionMemo;

namespace Iata.IS.Business.Cargo
{
  /// <summary>
  /// Responsible for invoice search, validating and submitting invoices.

  /// </summary>
  public interface ICargoCreditNoteManager : ICargoInvoiceManager
  {
    /// <summary>
    /// Determines whether [is credit memo exists] [the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if [is credit memo exists] [the specified invoice id]; otherwise, <c>false</c>.
    /// </returns>
    bool IsCreditMemoExists(string invoiceId);

    /// <summary>
    /// Gets the credit memo attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    CargoCreditMemoAttachment GetCreditMemoAttachmentDetails(string attachmentId);

    /// <summary>
    /// Adds the credit memo attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    CargoCreditMemoAttachment AddCreditMemoAttachment(CargoCreditMemoAttachment attach);

    /// <summary>
    /// Determines whether [is duplicate credit memo attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingMemoId">Credit memo id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate credit memo attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    bool IsDuplicateCreditMemoAttachmentFileName(string fileName, Guid creditMemoId);

    /// <summary>
    /// Adds credit memo record.
    /// </summary>
    /// <param name="creditMemo">Credit memo</param>
    /// <returns></returns>
    CargoCreditMemo AddCreditMemoRecord(CargoCreditMemo creditMemo);

    /// <summary>
    /// Updates the credit memo attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    IList<CargoCreditMemoAttachment> UpdateCreditMemoAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Gets credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CargoCreditMemoAttachment> GetCreditMemoAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// To update the credit memo record
    /// </summary>
    /// <param name="creditMemoRecord">Details of the credit memo record</param>
    /// <returns>Details of the updated credit memo record</returns>
    CargoCreditMemo UpdateCreditMemoRecord(CargoCreditMemo creditMemoRecord);

    /// <summary>
    /// Gets credit memo record details.
    /// </summary>
    /// <param name="creditMemoRecordId">credit memo record id.</param>
    /// <returns></returns>
    CargoCreditMemo GetCreditMemoRecordDetails(string creditMemoRecordId);

    /// <summary>
    /// To get the list of credit memo records
    /// </summary>
    /// <param name="creditNoteId">string of the credit Note</param>
    /// <returns>List of credit memo records </returns>
    IList<CargoCreditMemo> GetCreditMemoList(string creditNoteId);

    /// <summary>
    /// To delete the credit memo record from the database
    /// </summary>
    /// <param name="creditMemoId">string of the credit memo</param>
    /// <returns>True id deleted successfully, false otherwise</returns>
    bool DeleteCreditMemoRecord(string creditMemoId);

    /// <summary>
    /// Adds the CM awb record.
    /// </summary>
    /// <param name="bmAwb">The bm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    CMAirWayBill AddCMAwbRecord(CMAirWayBill bmAwb, string invoiceId, out string duplicateErrorMessage);

    /// <summary>
    /// Updates the CM awb attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    IList<CMAwbAttachment> UpdateCMAwbAttachment(IList<Guid> attachments, Guid parentId);

    /// <summary>
    /// Gets the CM awb attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    List<CMAwbAttachment> GetCMAwbAttachments(List<Guid> attachmentIds);

    /// <summary>
    /// Adds CM awb attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    CMAwbAttachment AddCMAwbAttachment(CMAwbAttachment attach);

    /// <summary>
    /// Gets Credit memo awb record details.
    /// </summary>
    /// <param name="cmAwbRecordId">The cm awb record id.</param>
    /// <returns></returns>
    CMAirWayBill GetCreditMemoAwbRecordDetails(string cmAwbRecordId);

    /// <summary>
    /// Gets the CM awb prorate ladder detail list.
    /// </summary>
    /// <param name="cmAwbProrateLadderDetailId">The cm awb prorate ladder detail id.</param>
    /// <returns></returns>
    IList<CMAwbProrateLadderDetail> GetCMAwbProrateLadderDetailList(Guid cmAwbProrateLadderDetailId);


    /// <summary>
    /// Adds the awb prorate ladder detail.
    /// </summary>
    /// <param name="cmAwbProrateLadderDetail">The cm awb prorate ladder detail.</param>
    /// <returns></returns>
    CMAwbProrateLadderDetail AddCreditMemoAwbProrateLadderDetail(CMAwbProrateLadderDetail cmAwbProrateLadderDetail);

    /// <summary>
    /// Updates CM awb record.
    /// </summary>
    /// <param name="cmAwb">The cm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    CMAirWayBill UpdateCMAwbRecord(CMAirWayBill cmAwb, string invoiceId, out string duplicateErrorMessage);

    /// <summary>
    /// Gets the CM awb list.
    /// </summary>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <returns></returns>
    IList<CMAirWayBill> GetCMAwbList(string creditMemoId);

    /// <summary>
    /// Deletes the credit memo awb record.
    /// </summary>
    /// <param name="awbRecordId">The awb record id.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    bool DeleteCreditMemoAwbRecord(string awbRecordId, out Guid creditMemoId, out Guid invoiceId);

    /// <summary>
    /// Gets CM awb attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    CMAwbAttachment GetCMAwbAttachmentDetails(string attachmentId);

    /// <summary>
    /// Gets the credit memo awb breakdown count.
    /// </summary>
    /// <param name="memoRecordId">The memo record id.</param>
    /// <returns></returns>
    long GetCreditMemoAwbBreakdownCount(string memoRecordId);
  }
}
