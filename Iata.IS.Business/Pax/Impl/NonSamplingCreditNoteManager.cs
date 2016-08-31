using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Enums;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Business.Pax.Impl
{
  class NonSamplingCreditNoteManager : InvoiceManager, INonSamplingCreditNoteManager
  {
    /// <summary>
    /// CreditMemo Repository, will be injected by the container.
    /// </summary>
    /// <value>The credit memo repository.</value>
    public ICreditMemoRecordRepository CreditMemoRepository { get; set; }

    /// <summary>
    /// MemberLocationInformation Repository, will be injected by the container.
    /// </summary>
    /// <value>The member loc info repository.</value>
    public IRepository<MemberLocationInformation> MemberLocInfoRepository { get; set; }

    /// <summary>
    /// CreditMemoCouponBreakdownRecordRepository Repository, will be injected by the container.
    /// </summary>
    /// <value>The credit memo coupon breakdown record.</value>
    public ICreditMemoCouponBreakdownRecordRepository CreditMemoCouponBreakdownRecordRepository { get; set; }

    /// <summary>
    /// CreditMemoCouponTaxBreakdownRepository Repository.
    /// </summary>
    public IRepository<CMCouponTax> CreditMemoCouponTaxBreakdownRepository { get; set; }

    /// <summary>
    /// Credit Memo Coupon Vat Breakdown Repository.
    /// </summary>
    public IRepository<CMCouponVat> CreditMemoCouponVatBreakdownRepository { get; set; }

    /// <summary>
    /// Credit tMemo Coupon Attachment Repository.
    /// </summary>
    public ICreditMemoCouponAttachmentRepository CreditMemoCouponAttachmentRepository { get; set; }

    /// <summary>
    /// Credit Memo Vat Repository.
    /// </summary>
    public IRepository<CreditMemoVat> CreditMemoVatRepository { get; set; }

    /// <summary>
    /// Credit Memo Attachment Repository.
    /// </summary>
    public ICreditMemoAttachmentRepository CreditMemoAttachmentRepository { get; set; }

    private const int MaxReasonRemarkCharLength = 4000;

    /// <summary>
    /// Function to retrieve credit memo. 
    /// </summary>
    /// <param name="creditNoteId">Credit Memo Id.</param>
    /// <returns>creditMemoList</returns>
    public IList<CreditMemo> GetCreditMemoList(string creditNoteId)
    {
      var creditNoteGuid = creditNoteId.ToGuid();
      var creditMemoList = CreditMemoRepository.Get(cm => cm.InvoiceId == creditNoteGuid);

      var reasonCodes = creditMemoList.Select(creditMemoRecord => creditMemoRecord.ReasonCode.ToUpper());
      var reasonCodesfromDb = ReasonCodeRepository.Get(reasonCode => reasonCodes.Contains(reasonCode.Code.ToUpper())).ToList();

      if (reasonCodesfromDb.Count() > 0)
      {
        foreach (var creditMemoRecord in creditMemoList)
        {
          var record = creditMemoRecord;
          var reasonCodeObj = reasonCodesfromDb.Single(rCode => rCode.Code.ToUpper() == record.ReasonCode.ToUpper() && rCode.TransactionTypeId == (int)TransactionType.CreditMemo);

          creditMemoRecord.ReasonCodeDescription = reasonCodeObj != null ? reasonCodeObj.Description : string.Empty;
        }
      }

      return creditMemoList.ToList();
    }

    /// <summary>
    /// Function to Retrieve credit memo details
    /// </summary>
    /// <param name="creditMemoId">Credit Memo Id.</param>
    /// <returns>creditMemo</returns>
    public CreditMemo GetCreditMemoRecordDetails(string creditMemoId)
    {
      var creditMemoGuid = creditMemoId.ToGuid();
      //LoadStrategy call
      var creditMemo = CreditMemoRepository.Single(creditMemoGuid);
      // var creditMemo = CreditMemoRepository.Single(cm => cm.Id == creditMemoGuid);

      return creditMemo;
    }

    /// <summary>
    /// Function to add Credit Memo 
    /// </summary>
    /// <param name="creditMemoRecord">Credit Memo Id.</param>
    /// <returns></returns>
    public CreditMemo AddCreditMemoRecord(CreditMemo creditMemoRecord)
    {
      creditMemoRecord.ReasonCode = creditMemoRecord.ReasonCode.ToUpper();
      ValidateCreditMemo(creditMemoRecord, null);
      CreditMemoRepository.Add(creditMemoRecord);
      UnitOfWork.CommitDefault();
      InvoiceRepository.UpdateCMInvoiceTotal(creditMemoRecord.InvoiceId, creditMemoRecord.SourceCodeId, creditMemoRecord.Id, creditMemoRecord.LastUpdatedBy);

     // UpdateExpiryDate(creditMemoRecord, creditMemoRecord.InvoiceId, creditMemoRecord.Id);

      return creditMemoRecord;
    }

    /// <summary>
    ///  Updates expiry date of Credit memo for purging.
    /// </summary>
    /// <param name="creditMemo"></param>
    /// <param name="invoiceId"></param>
    /// <param name="creditMemoId"></param>
    //private void UpdateExpiryDate(CreditMemo creditMemo, Guid invoiceId, Guid creditMemoId)
    //{
    //  PaxInvoice creditMemoInvoice;
    //  if(creditMemo == null || creditMemo.Invoice == null)
    //    creditMemoInvoice = InvoiceRepository.Single(id: invoiceId);
    //  else
    //  {
    //    creditMemoInvoice = creditMemo.Invoice;
    //  }

    //  // Get expiry period.
    //  DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, creditMemoInvoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

    //  // Update it in database.
    //  InvoiceRepository.UpdateExpiryDatePeriod(creditMemoId, (int)TransactionType.CreditMemo, expiryPeriod);
    //}

    /// <summary>
    /// Function to update credit memo
    /// </summary>
    /// <param name="creditMemoRecord">credit memo to be updated.</param>
    /// <returns></returns>
    public CreditMemo UpdateCreditMemoRecord(CreditMemo creditMemoRecord)
    {
      // LoadStrategy call
      var creditMemoRecordInDb = CreditMemoRepository.Single(creditMemoRecord.Id);
      //var creditMemoRecordInDb = CreditMemoRepository.Single(cm => cm.Id == creditMemoRecord.Id);
      creditMemoRecord.ReasonCode = creditMemoRecord.ReasonCode.ToUpper();
      ValidateCreditMemo(creditMemoRecord, creditMemoRecordInDb);
      var updatedCreditMemo = CreditMemoRepository.Update(creditMemoRecord);

      //Vat List Update along with Credit Memo Record.
      var listToDeleteVat = creditMemoRecordInDb.VatBreakdown.Where(vat => creditMemoRecord.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in creditMemoRecord.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        vat.ParentId = creditMemoRecord.Id;
        CreditMemoVatRepository.Add(vat);
      }

      foreach (var billingMemoVat in listToDeleteVat)
      {
        CreditMemoVatRepository.Delete(billingMemoVat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = creditMemoRecordInDb.Attachments.Where(attachment => creditMemoRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in creditMemoRecord.Attachments
                              where creditMemoRecordInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var creditMemoAttachmentInDb = CreditMemoAttachmentRepository.Get(cmAttachment => attachmentIdList.Contains(cmAttachment.Id));
      foreach (var recordAttachment in creditMemoAttachmentInDb)
      {
        if (IsDuplicateCreditMemoAttachmentFileName(recordAttachment.OriginalFileName, creditMemoRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = creditMemoRecord.Id;
        CreditMemoAttachmentRepository.Update(recordAttachment);
      }

      foreach (var couponRecordAttachment in listToDeleteAttachment)
      {
        CreditMemoAttachmentRepository.Delete(couponRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      InvoiceRepository.UpdateCMInvoiceTotal(creditMemoRecordInDb.InvoiceId, creditMemoRecordInDb.SourceCodeId, creditMemoRecordInDb.Id, creditMemoRecord.LastUpdatedBy);
      //UpdateExpiryDate(creditMemoRecord, creditMemoRecord.InvoiceId, creditMemoRecord.Id);
      return updatedCreditMemo;
    }

    /// <summary>
    /// Delete Credit MemoRecord.
    /// </summary>
    /// <param name="creditMemoId">Credit Memo Id. </param>
    /// <returns></returns>
    public bool DeleteCreditMemoRecord(string creditMemoId)
    {
      var creditMemoGuid = creditMemoId.ToGuid();
      //LoadStrategy call
      var creditMemo = CreditMemoRepository.Single(creditMemoGuid);
      // var creditMemo = CreditMemoRepository.Single(cm => cm.Id == creditMemoGuid);
      if (creditMemo == null) return false;
      CreditMemoRepository.Delete(creditMemo);

      UnitOfWork.CommitDefault();

      InvoiceRepository.UpdateCMInvoiceTotal(creditMemo.InvoiceId, creditMemo.SourceCodeId, creditMemo.Id, creditMemo.LastUpdatedBy);

      return true;
    }

    /// <summary>
    /// Get credit memo attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    public CreditMemoAttachment GetCreditMemoAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = CreditMemoAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Add credit memo attachment
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    public CreditMemoAttachment AddCreditMemoAttachment(CreditMemoAttachment attach)
    {

      CreditMemoAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //Commented below code. Just return the object which is passed to function.
     // attach = CreditMemoAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update parent id for credit memo attachment record with given Guid
    /// </summary>
    /// <param name="attachments">list of Guid of attachment records</param>
    /// <param name="parentId">credit memo id</param>
    /// <returns></returns>
    public IList<CreditMemoAttachment> UpdateCreditMemoAttachment(IList<Guid> attachments, Guid parentId)
    {
      var creditMemoAttachmentInDb = CreditMemoAttachmentRepository.Get(cmAttachment => attachments.Contains(cmAttachment.Id));
      foreach (var recordAttachment in creditMemoAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        CreditMemoAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return creditMemoAttachmentInDb.ToList();
    }

    /// <summary>
    /// Check for duplicate filename for credit memo attachment records
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="creditMemoId">credit memo id</param>
    /// <returns></returns>
    public bool IsDuplicateCreditMemoAttachmentFileName(string fileName, Guid creditMemoId)
    {
      return CreditMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == creditMemoId && attachment.OriginalFileName.Equals(fileName, StringComparison.OrdinalIgnoreCase)) > 0;
    }

    /// <summary>
    /// List of Credit Memo Records.
    /// </summary>
    /// <param name="memoRecordId">Credit Memo Id.</param>
    /// <returns></returns>
    public IList<CMCoupon> GetCreditMemoCouponBreakdownList(string memoRecordId)
    {
      var memoRecordGuid = memoRecordId.ToGuid();
      var creditMemoList = CreditMemoCouponBreakdownRecordRepository.Get(cmCb => cmCb.CreditMemoId == memoRecordGuid);

      return creditMemoList.ToList();
    }

    /// <summary>
    /// Gives the details of specific Credit Memo Coupon Breakdowns Details
    /// </summary>
    /// <param name="couponBreakdownRecordId">Credit coupon Breakdown Record Id</param>
    /// <returns></returns>
    public CMCoupon GetCreditMemoCouponDetails(string couponBreakdownRecordId)
    {
      var couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();
      var creditMemoCouponBreakdown = CreditMemoCouponBreakdownRecordRepository.Single(couponBreakdownRecordGuid);

      return creditMemoCouponBreakdown;
    }

    /// <summary>
    /// Adds new record in the list of Credit Memo Coupon Breakdown
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">Credit coupon Breakdown Record.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Added coupon breakdown record</returns>
    public CMCoupon AddCreditMemoCouponRecord(CMCoupon creditMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage)
    {
      // Validate
      duplicateErrorMessage = ValidateCreditMemoCoupon(creditMemoCouponBreakdownRecord, null, invoiceId);
      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        creditMemoCouponBreakdownRecord.ISValidationFlag = "DU";
      }

      creditMemoCouponBreakdownRecord.SerialNo = GetCMCouponSerialNo(creditMemoCouponBreakdownRecord.CreditMemoId);
      // Adds new record to the list.
      CreditMemoCouponBreakdownRecordRepository.Add(creditMemoCouponBreakdownRecord);

      UnitOfWork.CommitDefault();

      UpdateCreditMemoCouponInvoiceTotal(creditMemoCouponBreakdownRecord);

      return creditMemoCouponBreakdownRecord;
    }

    private int GetCMCouponSerialNo(Guid creditMemoRecordId)
    {
      var serialNo = 1;
      var creditMemoCouponRecord = CreditMemoCouponBreakdownRecordRepository.Get(cmCoupon => cmCoupon.CreditMemoId == creditMemoRecordId).OrderByDescending(cmCoupon => cmCoupon.SerialNo).FirstOrDefault();
      if (creditMemoCouponRecord != null)
      {
        serialNo = creditMemoCouponRecord.SerialNo + 1;
      }

      return serialNo;
    }


    /// <summary>
    /// Updates the credit memo invoice total.
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">The credit memo coupon breakdown record.</param>
    /// <param name="isCouponDelete">if set to true [is coupon delete].</param>
    private void UpdateCreditMemoCouponInvoiceTotal(CMCoupon creditMemoCouponBreakdownRecord, bool isCouponDelete = false)
    {
      //Load Strategy call
      var creditMemoRecord = CreditMemoRepository.Single(creditMemoCouponBreakdownRecord.CreditMemoId);
      // var creditMemoRecord = CreditMemoRepository.Single(cmRecord => cmRecord.Id == creditMemoCouponBreakdownRecord.CreditMemoId);
      InvoiceRepository.UpdateCMInvoiceTotal(creditMemoRecord.InvoiceId, creditMemoRecord.SourceCodeId, creditMemoCouponBreakdownRecord.CreditMemoId, creditMemoCouponBreakdownRecord.LastUpdatedBy, isCouponDelete);
    }

    /// <summary>
    /// Update specific Credit Memo Coupon Breakdown Record.
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Updated coupon breakdown record</returns>
    public CMCoupon UpdateCreditMemoCouponRecord(CMCoupon creditMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage)
    {
      var creditMemoCouponBreakdownWithDetail = CreditMemoCouponBreakdownRecordRepository.Single(creditMemoCouponBreakdownRecord.Id);

      creditMemoCouponBreakdownRecord.ISValidationFlag = creditMemoCouponBreakdownWithDetail.ISValidationFlag;
      // Validate
      duplicateErrorMessage = ValidateCreditMemoCoupon(creditMemoCouponBreakdownRecord, creditMemoCouponBreakdownWithDetail, invoiceId);

      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        creditMemoCouponBreakdownRecord.ISValidationFlag = "DU";
      }
      // update  the record
      var updatedCreditMemo = CreditMemoCouponBreakdownRecordRepository.Update(creditMemoCouponBreakdownRecord);

      var listToDelete = creditMemoCouponBreakdownWithDetail.TaxBreakdown.Where(tax => creditMemoCouponBreakdownRecord.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0).ToList();
      foreach (var tax in creditMemoCouponBreakdownRecord.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0))
      {
        tax.ParentId = creditMemoCouponBreakdownRecord.Id;
        CreditMemoCouponTaxBreakdownRepository.Add(tax);
      }
      foreach (var tax in listToDelete)
      {
        CreditMemoCouponTaxBreakdownRepository.Delete(tax);
      }

      //SCP286106 - FW: IAP ticket with missing tax record
      if (creditMemoCouponBreakdownRecord.TaxBreakdown.Count > 0)
      {
        foreach (var couponRecordTax in creditMemoCouponBreakdownRecord.TaxBreakdown)
        {
          if (creditMemoCouponBreakdownWithDetail.TaxBreakdown.Where(t => t.Id.Equals(couponRecordTax.Id)).Count() == 0)
          {
            couponRecordTax.ParentId = creditMemoCouponBreakdownRecord.Id;
            CreditMemoCouponTaxBreakdownRepository.Add(couponRecordTax);
          }
        }
      }

      var listToDeleteVat = creditMemoCouponBreakdownWithDetail.VatBreakdown.Where(vat => creditMemoCouponBreakdownRecord.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in creditMemoCouponBreakdownRecord.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        vat.ParentId = creditMemoCouponBreakdownRecord.Id;
        CreditMemoCouponVatBreakdownRepository.Add(vat);
      }
      foreach (var vat in listToDeleteVat)
      {
        CreditMemoCouponVatBreakdownRepository.Delete(vat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = creditMemoCouponBreakdownWithDetail.Attachments.Where(attachment => creditMemoCouponBreakdownRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in creditMemoCouponBreakdownRecord.Attachments
                              where creditMemoCouponBreakdownWithDetail.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var creditMemoCouponAttachmentInDb = CreditMemoCouponAttachmentRepository.Get(cmCouponAttachment => attachmentIdList.Contains(cmCouponAttachment.Id));
      foreach (var recordAttachment in creditMemoCouponAttachmentInDb)
      {
        if (IsDuplicateCreditMemoCouponAttachmentFileName(recordAttachment.OriginalFileName, creditMemoCouponBreakdownRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = creditMemoCouponBreakdownRecord.Id;
        CreditMemoCouponAttachmentRepository.Update(recordAttachment);
      }

      foreach (var couponRecordAttachment in listToDeleteAttachment)
      {
        CreditMemoCouponAttachmentRepository.Delete(couponRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      UpdateCreditMemoCouponInvoiceTotal(creditMemoCouponBreakdownRecord);

      //UpdateExpiryDate(creditMemoCouponBreakdownRecord.CreditMemoRecord, invoiceId.ToGuid(), creditMemoCouponBreakdownRecord.CreditMemoId);

      return updatedCreditMemo;
    }

    /// <summary>
    /// Deleting specific record of Credit Memo Coupon Breakdown
    /// </summary>
    /// <param name="couponBreakdownRecordId">coupon Breakdown Record Id.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public bool DeleteCreditMemoCouponRecord(string couponBreakdownRecordId, ref Guid creditMemoId, ref Guid invoiceId)
    {
      var couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();
      var creditMemo = CreditMemoCouponBreakdownRecordRepository.Single(couponBreakdownRecordGuid);

      if (creditMemo == null) return false;

      creditMemoId = creditMemo.CreditMemoId;
      invoiceId = creditMemo.CreditMemoRecord.InvoiceId;

      // Delete CM Coupon, re-sequence serial numbers of subsequent coupons and update Invoice Total.
      InvoiceRepository.DeleteCreditMemoCoupon(couponBreakdownRecordGuid);

      return true;
    }

    /// <summary>
    /// Get credit memo Coupon attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    public CMCouponAttachment GetCreditMemoCouponAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = CreditMemoCouponAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Add credit memo attachment
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    public CMCouponAttachment AddCreditMemoCouponAttachment(CMCouponAttachment attach)
    {

      CreditMemoCouponAttachmentRepository.Add(attach);

      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //Commented below code. Just return the object which is passed to function.
      //attach = CreditMemoCouponAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update parent id for credit memo Coupon attachment record with given Guid
    /// </summary>
    /// <param name="attachments">list of Guid of attachment records</param>
    /// <param name="parentId">credit memo id</param>
    /// <returns></returns>
    public IList<CMCouponAttachment> UpdateCreditMemoCouponAttachment(IList<Guid> attachments, Guid parentId)
    {
      var creditMemoCouponAttachmentInDb = CreditMemoCouponAttachmentRepository.Get(cmCouponAttachment => attachments.Contains(cmCouponAttachment.Id));
      foreach (var recordAttachment in creditMemoCouponAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        CreditMemoCouponAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return creditMemoCouponAttachmentInDb.ToList();
    }

    /// <summary>
    /// Check for duplicate filename for credit memo Coupon attachment reocrds
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="creditMemoCouponId">credit Memo Coupon Id</param>
    /// <returns></returns>
    public bool IsDuplicateCreditMemoCouponAttachmentFileName(string fileName, Guid creditMemoCouponId)
    {
      return CreditMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == creditMemoCouponId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Validates an invoice, when Validate Invoice button pressed
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <param name="isFutureSubmission"></param>
    /// <returns>
    /// True if successfully validated, false otherwise
    /// </returns>
    public override PaxInvoice ValidateInvoice(string invoiceId, out bool isFutureSubmission, int sessionUserId, string logRefId)
    {
        var log = ReferenceManager.GetDebugLog(DateTime.Now,
                                                "NonSamplingCreditNoteManager.ValidateInvoice",
                                                this.ToString(),
                                                BillingCategorys.Passenger.ToString(),
                                                "Step 3 of 12: Id: " + invoiceId + " NonSamplingCreditNoteManager.ValidateInvoice Start",
                                                sessionUserId,
                                                logRefId);

        ReferenceManager.LogDebugData(log);

      isFutureSubmission = false;
      var webValidationErrors = new List<WebValidationError>();
      bool isFutureSubmitted = false;
      var invoice = base.ValidateInvoice(invoiceId, out isFutureSubmitted, sessionUserId, logRefId);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                       "NonSamplingCreditNoteManager.ValidateInvoice",
                                       this.ToString(),
                                       BillingCategorys.Passenger.ToString(),
                                       "Step 8 of 12: Id: " + invoiceId + " after base.ValidateInvoice()",
                                       sessionUserId,
                                       logRefId);
      ReferenceManager.LogDebugData(log);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
      webValidationErrors.AddRange(invoice.ValidationErrors);

      var creditMemoRecordCount = CreditMemoRepository.GetCount(creditMemoRecord => creditMemoRecord.InvoiceId == invoice.Id);

      if (creditMemoRecordCount <= 0)
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.TransactionLineItemNotAvailable));

        //throw new ISBusinessException(ErrorCodes.TransactionLineItemNotAvailable);
      }

      var errorMessages = InvoiceRepository.ValidateMemo(invoiceId.ToGuid(), invoice.BillingCode);
      if (!string.IsNullOrEmpty(errorMessages))
      {
        errorMessages = errorMessages.Remove(errorMessages.Length - 1, 1);
        var errorMessage = errorMessages.Split(',');
        foreach (var error in errorMessage)
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), "BPAXS_10239", error));
        }
      }

      if (webValidationErrors.Count > 0)
      {
        invoice.ValidationErrors.Clear();
        invoice.ValidationErrors.AddRange(webValidationErrors);
        invoice.InvoiceStatus = InvoiceStatusType.ValidationError;
        // updating validation status to Error if there are more than 1 error, if its one then its already set to ErrorPeriod.
        if (webValidationErrors.Count > 1)
          invoice.ValidationStatus = InvoiceValidationStatus.Failed;
        invoice.ValidationDate = DateTime.UtcNow;
      }
      else
      {
        invoice.InvoiceStatus = isFutureSubmitted
                                  ? InvoiceStatusType.FutureSubmitted
                                  : InvoiceStatusType.ReadyForSubmission;
        // updating validation status to completed
        invoice.ValidationStatus = InvoiceValidationStatus.Completed;
        invoice.ValidationDate = DateTime.UtcNow;
      }

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                     "NonSamplingCreditNoteManager.ValidateInvoice",
                                     this.ToString(),
                                     BillingCategorys.Passenger.ToString(),
                                     "Step 9 of 12: Id: " + invoice.Id + " after invoice.ValidationDate = DateTime.UtcNow",
                                     sessionUserId,
                                     logRefId);
      ReferenceManager.LogDebugData(log);

      //SCP370316 - SRM: May P3 provisional invoice XML Settlement file
      if (isFutureSubmitted)
      {
        // Get Final Parent Details Clearing House.
        var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
        var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

        // Update clearing house of invoice
        var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);
        invoice.ClearingHouse = clearingHouse;
      }

      // Invoice through the validation, change invoice status to Ready for submission. 
      // invoice.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;

      //InvoiceRepository.Update(invoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      InvoiceRepository.SetInvoiceAndValidationStatus(invoice.Id, invoice.InvoiceStatusId, invoice.ValidationStatusId, isFutureSubmitted, invoice.ClearingHouse, invoice.InvoiceTotalRecord.NetBillingAmount, (int)BillingCategoryType.Pax, invoice.ExchangeRate);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                       "NonSamplingCreditNoteManager.ValidateInvoice",
                                       this.ToString(),
                                       BillingCategorys.Passenger.ToString(),
                                       "Step 10 of 12: Id: " + invoice.Id + " after InvoiceRepository.SetInvoiceAndValidationStatus()",
                                       sessionUserId,
                                       logRefId);
      ReferenceManager.LogDebugData(log);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                     "NonSamplingCreditNoteManager.ValidateInvoice",
                                     this.ToString(),
                                     BillingCategorys.Passenger.ToString(),
                                     "Step 11 of 12: Id: " + invoice.Id + " after ValidationErrorManager.UpdateValidationErrors()",
                                     sessionUserId,
                                     logRefId);
      ReferenceManager.LogDebugData(log);

      //SCP325375: File Loading & Web Response Stats ManageInvoice
      //UnitOfWork.CommitDefault();

      return invoice;
    }

    /// <summary>
    /// Validate the Invoice Header corresponding to the invoice details provided
    /// </summary>
    /// <param name="invoiceHeader"></param>
    /// <param name="invoiceHeaderInDb"></param>
    /// /// <returns></returns>
    protected override bool ValidateInvoiceHeader(PaxInvoice invoiceHeader, PaxInvoice invoiceHeaderInDb)
    {
      base.ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb);

      // Validation For Invoice Type
      if (invoiceHeader.InvoiceType != InvoiceType.CreditNote)
      {
        throw new ISBusinessException(ErrorCodes.UnexpectedInvoiceType);
      }

      return true;
    }

    /// <summary>
    /// Validate credit memo record.
    /// </summary>
    /// <param name="creditMemoRecord">credit memo record to be validated.</param>
    /// <param name="creditMemoRecordInDb">The credit memo record in db.</param>
    /// <returns></returns>
    private void ValidateCreditMemo(CreditMemo creditMemoRecord, CreditMemo creditMemoRecordInDb)
    {
      var isUpdateOperation = false;

      //If there is record in db then its a update operation
      if (creditMemoRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (creditMemoRecord.RecordSequenceWithinBatch <= 0 || creditMemoRecord.BatchSequenceNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }

      // Check whether Batch and Sequence number combination is valid and check whether Batch number is not repeated between different source codes
      int invalidBatchSequenceNumber = InvoiceRepository.IsValidBatchSequenceNo(creditMemoRecord.InvoiceId, creditMemoRecord.RecordSequenceWithinBatch, creditMemoRecord.BatchSequenceNumber, creditMemoRecord.Id, creditMemoRecord.SourceCodeId);

      // If value != 0, either Batch and Sequence number combination is invalid or Batch number is repeated between different source codes  
      if (invalidBatchSequenceNumber != 0)
      {
        // If value == 1, Batch number is repeated between different source codes, else if value == 2, Batch and Sequence number combination is invalid  
        if (invalidBatchSequenceNumber == 1)
          throw new ISBusinessException(ErrorCodes.InvalidBatchNo);
        else
          throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNo);
      }
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Commented below code ....used GetInvoiceHeader instead as only invoice header details are used 
      //var creditMemoInvoice = InvoiceRepository.Single(id: creditMemoRecord.InvoiceId);
      var creditMemoInvoice = InvoiceRepository.GetInvoiceHeader(creditMemoRecord.InvoiceId);
      // Your billing period can not be greater than or equal to the Credit Memo Invoice billing period.
      if (!((creditMemoInvoice.BillingYear > creditMemoRecord.YourInvoiceBillingYear) ||
        ((creditMemoInvoice.BillingYear == creditMemoRecord.YourInvoiceBillingYear) && (creditMemoInvoice.BillingMonth > creditMemoRecord.YourInvoiceBillingMonth)) ||
          ((creditMemoInvoice.BillingYear == creditMemoRecord.YourInvoiceBillingYear) && (creditMemoInvoice.BillingMonth == creditMemoRecord.YourInvoiceBillingMonth) && (creditMemoInvoice.BillingPeriod > creditMemoRecord.YourInvoiceBillingPeriod))))
        throw new ISBusinessException(ErrorCodes.InvalidYourBillingPeriod);

      // Check whether net amount credited is positive for credit memo, else throw an exception. 
      if (creditMemoRecord.NetAmountCredited > 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAmount);
      }

      // If no coupon breakdown exists and if Total VAT Amount Credited populated with a non-zero value, then BM-CM VAT Breakdown record needs to be present.
      var couponBreakdownCount = CreditMemoCouponBreakdownRecordRepository.GetCount(cmRecord => cmRecord.CreditMemoId == creditMemoRecord.Id);

      if (couponBreakdownCount == 0 && creditMemoRecord.VatAmount > 0)
      //if (creditMemoRecord.CouponBreakdownRecord.Count == 0 && creditMemoRecord.VatAmount > 0)
      {
        if (creditMemoRecord.VatBreakdown.Count <= 0)
        {
          throw new ISBusinessException(ErrorCodes.CreditMemoVatBreakdownRecordNotFound);
        }
      }

      // Validates whether source code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(creditMemoRecordInDb.SourceCodeId, creditMemoRecord.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(creditMemoRecord.SourceCodeId, (int)TransactionType.CreditMemo))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }

      // Validates whether reason code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(creditMemoRecordInDb.ReasonCode, creditMemoRecord.ReasonCode))
      {
        if (!ReferenceManager.IsValidReasonCode(creditMemoRecord.ReasonCode, (int)TransactionType.CreditMemo))
        {
          throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
        }
      }

      // Validate Duplicate Credit Memo.
      if (IsDuplicateCreditMemo(creditMemoRecordInDb, creditMemoRecord, isUpdateOperation, creditMemoInvoice))
      {
        throw new ISBusinessException(ErrorCodes.DuplicateCreditMemoFound);
      }

      return;
    }

    /// <summary>
    /// Any Billing memo having the same Billing memo number has been twice billed in the same invoice,
    /// or in a previous invoice to the same airline will be considered as a duplicate.
    /// </summary>
    /// <param name="creditMemoRecordInDb">The credit memo record in db.</param>
    /// <param name="creditMemoRecord">The credit memo record.</param>
    /// <param name="isUpdateOperation">if set to <c>true</c> [is update operation].</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate credit memo] [the specified credit memo record in db]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsDuplicateCreditMemo(CreditMemo creditMemoRecordInDb, CreditMemo creditMemoRecord, bool isUpdateOperation, PaxInvoice invoice)
    {
      if (!isUpdateOperation || (CompareUtil.IsDirty(creditMemoRecordInDb.CreditMemoNumber, creditMemoRecord.CreditMemoNumber)))
      {
        if (invoice != null)
        {
          var duplicateBillingMemo = CreditMemoRepository.GetCreditMemoDuplicateCount(creditMemoRecord.CreditMemoNumber, invoice.BilledMemberId, invoice.BillingMemberId, invoice.BillingMonth, invoice.BillingYear, invoice.BillingPeriod);
          if (duplicateBillingMemo > 0)
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Validate credit memo Breakdown record.
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">credit memo Breakdown record to be validated.</param>
    /// <param name="creditMemoCouponBreakdownRecordInDb">The credit memo coupon breakdown record in db.</param>
    /// <returns></returns>
    private string ValidateCreditMemoCoupon(CMCoupon creditMemoCouponBreakdownRecord, CMCoupon creditMemoCouponBreakdownRecordInDb, string invoiceId)
    {
      var isUpdateOperation = false;
      var duplicateErrorMessage = string.Empty;
      //Below code to check invalid TaxCode enter by user. if user enter invalid taxcode then system will generate error message.
      //SCP#50425 - Tax Code XT
      foreach (var tax in creditMemoCouponBreakdownRecord.TaxBreakdown)
      {
        if (!string.IsNullOrWhiteSpace(tax.TaxCode))
          tax.TaxCode = tax.TaxCode.Trim();
        if (!ReferenceManager.IsValidTaxCode(tax.TaxCode))
        {
          throw new ISBusinessException(ErrorCodes.InvalidTaxCode, tax.TaxCode);
        }
      }
      //End SCP#50425 - Tax Code XT

      //Check whether it's a update operation.
      if (creditMemoCouponBreakdownRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      // Tax Amount Billed is non-zero then Tax Breakdown record needs to be provided
      if (creditMemoCouponBreakdownRecord.TaxAmount > 0 && creditMemoCouponBreakdownRecord.TaxBreakdown.Count <= 0)
      {
        throw new ISBusinessException(ErrorCodes.CreditMemoCouponTaxBreakdownNotFound);
      }

      // Vat Amount Billed is non-zero then Tax Breakdown record needs to be provided
      if (creditMemoCouponBreakdownRecord.VatAmount > 0 && creditMemoCouponBreakdownRecord.VatBreakdown.Count <= 0)
      {
        throw new ISBusinessException(ErrorCodes.CreditMemoCouponVatBreakdownNotFound);
      }

      // Check if passed 'Currency Adjustment Indicator' is a valid currency code
      // For New coupon Record validation will be done 
      // For Update coupon Record if value CurrencyAdjustmentIndicator is updated then only validation will be done
      if (!isUpdateOperation || CompareUtil.IsDirty(creditMemoCouponBreakdownRecordInDb.CurrencyAdjustmentIndicator, creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
      {
        if (!string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator) && !ReferenceManager.IsValidCurrencyCode(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          throw new ISBusinessException(ErrorCodes.InvalidCurrencyAdjustmentInd);
        }
      }

      if (!isUpdateOperation ||
         (CompareUtil.IsDirty(creditMemoCouponBreakdownRecordInDb.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber) ||
         CompareUtil.IsDirty(creditMemoCouponBreakdownRecordInDb.TicketOrFimIssuingAirline, creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline) ||
         CompareUtil.IsDirty(creditMemoCouponBreakdownRecordInDb.TicketOrFimCouponNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber)))
      {
        // Duplicate check - Ticket Issuing Airline, Ticket/Document Number, Coupon No.: As per values provided in the dialog by the user.
        var invoiceGuid = invoiceId.ToGuid();
        var invoice = InvoiceRepository.Single(id: invoiceGuid);
        DateTime billingDate;
        var billingYearToCompare = 0;
        var billingMonthToCompare = 0;

        if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
        {
          var billingDateToCompare = billingDate.AddMonths(-12);
          billingYearToCompare = billingDateToCompare.Year;
          billingMonthToCompare = billingDateToCompare.Month;
        }
        // Duplicate check - Ticket Issuing Airline, Ticket/Document Number, Coupon No.: As per values provided in the dialog by the user.
        var duplicateCouponCount = CreditMemoCouponBreakdownRecordRepository.GetCreditMemoCouponDuplicateCount(creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber,
                                                                                                               creditMemoCouponBreakdownRecord.TicketDocOrFimNumber,
                                                                                                               creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline,
                                                                                                               invoice.BillingMemberId,
                                                                                                               invoice.BilledMemberId,
                                                                                                               billingYearToCompare,
                                                                                                               billingMonthToCompare);


        //if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0))
        if (duplicateCouponCount > 0)
        {
          duplicateErrorMessage = string.Format(Messages.CreditMemoCouponDuplicateMessage, duplicateCouponCount);
        }
      }

      return duplicateErrorMessage;
    }

    /// <summary>
    /// Gets the credit memo coupon breakdown count.
    /// </summary>
    /// <param name="memoRecordId">The memo record id.</param>
    /// <returns></returns>
    public long GetCreditMemoCouponBreakdownCount(string memoRecordId)
    {
      var memoRecordGuid = memoRecordId.ToGuid();
      return CreditMemoCouponBreakdownRecordRepository.GetCount(cmCb => cmCb.CreditMemoId == memoRecordGuid);
    }

    /// <summary>
    /// Determines whether credit memo exists for the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if credit memo exists the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    public bool IsCreditMemoExists(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      return CreditMemoRepository.GetCount(cmRecord => cmRecord.InvoiceId == invoiceGuid) > 0;
    }

    /// <summary>
    /// Validates the parsed billing memo record.
    /// </summary>
    /// <param name="creditMemoRecord">The credit memo record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="airlineFlightDesignator"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="maxAcceptableAmount"></param>
    /// <returns></returns>
    public bool ValidateParsedCreditMemoRecord(CreditMemo creditMemoRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> airlineFlightDesignator, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate, ExchangeRate exchangeRate, MaxAcceptableAmount maxAcceptableAmount)
    {
      var isValid = true;

      var creditMemoNumber = string.Empty;

      if (creditMemoRecord.CreditMemoNumber != null)
      {
        creditMemoNumber = creditMemoRecord.CreditMemoNumber;
      }

      if (creditMemoRecord.FimCouponNumber != null && creditMemoRecord.FimNumber != null)
      {
        //FIM Number and FIM Coupon Number should be blank or both fields should be captured.
        if ((creditMemoRecord.FimNumber != 0 || creditMemoRecord.FimCouponNumber != 0) && creditMemoRecord.FimNumber * creditMemoRecord.FimCouponNumber == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number", Convert.ToString(creditMemoRecord.CorrespondenceRefNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.MandatoryFimNumberAndCouponNumber, ErrorStatus.C, creditMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (creditMemoRecord.FimCouponNumber < 0 || creditMemoRecord.FimCouponNumber > 4)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Coupon Number", Convert.ToString(creditMemoRecord.FimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidFimCouponNumber, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Billing period should be 01,02,03,04
      //if (creditMemoRecord.YourInvoiceBillingPeriod < 0 || creditMemoRecord.YourInvoiceBillingPeriod > 4)
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Billing Period", string.Format("{0}{1}{2}", creditMemoRecord.YourInvoiceBillingYear, creditMemoRecord.YourInvoiceBillingMonth, creditMemoRecord.YourInvoiceBillingPeriod), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidBillingPeriod, ErrorStatus.X, creditMemoRecord);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      //Validate SourceCode 
      if (!ReferenceManager.IsValidSourceCode(invoice, creditMemoRecord.SourceCodeId, (int)TransactionType.CreditMemo))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code", Convert.ToString(creditMemoRecord.SourceCodeId), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidSourceCode, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Your Invoice Number should not be current Invoice Number
      PaxInvoice yourInvoice = null;
      //SCP122624:
      if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml)
      {
        DateTime yourInvoiceBillingDate;
        //To avoid converting year 30 into year 1930
        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.Calendar.TwoDigitYearMax = 2099;
        var yourInvoiceDateString = string.Format("{2}{1}{0}",
                                       Convert.ToString(creditMemoRecord.YourInvoiceBillingPeriod).PadLeft(2, '0'),
                                       Convert.ToString(creditMemoRecord.YourInvoiceBillingMonth).PadLeft(2, '0'),
                                       Convert.ToString(creditMemoRecord.YourInvoiceBillingYear).PadLeft(4, '0'));
        var yourInvoiceDateStringErr = yourInvoiceDateString.Substring(2, yourInvoiceDateString.Length-2);
        if ((!String.IsNullOrEmpty(yourInvoiceDateString) && (Convert.ToInt32(yourInvoiceDateString) != 0) && string.IsNullOrWhiteSpace(creditMemoRecord.YourInvoiceNumber)) || (Convert.ToInt32(yourInvoiceDateString) == 0 && !string.IsNullOrWhiteSpace(creditMemoRecord.YourInvoiceNumber)))
        {
          var validation_ExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Number", creditMemoRecord.YourInvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.MandatoryYourInvoiceNumberAndYourBillingDate, ErrorStatus.X, creditMemoRecord);
          exceptionDetailsList.Add(validation_ExceptionDetail);
          isValid = false;
        }
        if (!String.IsNullOrEmpty(yourInvoiceDateString) && (Convert.ToInt32(yourInvoiceDateString) != 0))
        {
          if (DateTime.TryParseExact(yourInvoiceDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out yourInvoiceBillingDate))
          {
            if (yourInvoiceBillingDate.Day < 1 || yourInvoiceBillingDate.Day > 4)
            {
              //Raise NonCorrectable error for invalid your invoice Date.
              var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Your Invoice Billing Date",
                                                                              yourInvoiceDateStringErr,
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelCreditMemo,
                                                                              ErrorCodes.InvalidYourInvoiceBillingDatePeriod,
                                                                              ErrorStatus.X,
                                                                              creditMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          else
          {
            //Raise NonCorrectable error for invalid your invoice Date.
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "Your Invoice Billing Date",
                                                                            yourInvoiceDateStringErr,
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelCreditMemo,
                                                                            ErrorCodes.InvalidYourInvoiceBillingDatePeriod,
                                                                            ErrorStatus.X,
                                                                            creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }
      //if (creditMemoRecord.YourInvoiceNumber != null)
      //{
      //  if (invoice.InvoiceNumber.ToUpper().Equals(creditMemoRecord.YourInvoiceNumber.ToUpper()))
      //  {
      //    var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Rejection Number",
      //                                                                    Convert.ToString(creditMemoRecord.YourInvoiceNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidYourInvoiceNumber, ErrorStatus.C,
      //                                                                    creditMemoRecord);
      //    exceptionDetailsList.Add(validationExceptionDetail);
      //    isValid = false;
      //  }
      //}

      //Validate Correspondence reference Number
      if (creditMemoRecord.ReasonCode != null)
      {

        //validate reason code 
        if (!ReferenceManager.IsValidReasonCode(invoice, creditMemoRecord.ReasonCode, (int)TransactionType.CreditMemo))
        {
          //SCP#482931: Changed the Error Status from Correctable(C) to Non Correctable(X), because Invalid Reason Code is Correctable only in 
          //            case of Sampling Form C, And we have changed Error Level From 'Billing Memo' to 'Credit Memo' as this is credit memo record.
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reason Code", creditMemoRecord.ReasonCode, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidReasonCode, ErrorStatus.X, creditMemoRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        var isCouponBreakdownMandatory = ReasonCodeRepository.GetCount(reasonCode => reasonCode.Code.ToUpper() == creditMemoRecord.ReasonCode.ToUpper() && reasonCode.TransactionTypeId == (int)TransactionType.BillingMemo && reasonCode.CouponAwbBreakdownMandatory) > 0;

        if (isCouponBreakdownMandatory)
        {
          if (creditMemoRecord.CouponBreakdownRecord.Count == 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Memo Record", Convert.ToString(creditMemoRecord.ReasonCode), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.MandatoryCouponBreakdownRecord, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }

        if (creditMemoRecord.ReasonCode.ToUpper() == "6A" || creditMemoRecord.ReasonCode.ToUpper() == "6B")
        {

          // Correspondence Ref Number should be populated for Reason Code “6A” or “6B”.
          if (creditMemoRecord.CorrespondenceRefNumber == 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Correspondence Ref Number", Convert.ToString(creditMemoRecord.CorrespondenceRefNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidReferenceCorrespondence, ErrorStatus.C, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {

            var corrBillingToBilled = CorrespondenceRepository.Single(rec => rec.CorrespondenceNumber == creditMemoRecord.CorrespondenceRefNumber && rec.FromMemberId == invoice.BillingMemberId && rec.ToMemberId == invoice.BilledMemberId);

            if (corrBillingToBilled == null)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Correspondence Ref Number", Convert.ToString(creditMemoRecord.CorrespondenceRefNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.CreditMemoReferenceCorrespondenceDoesNotExist, ErrorStatus.C, creditMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

          }

          // Correspondence Ref Number should be populated only for Reason Code “6A” or “6B”.
          if (creditMemoRecord.CorrespondenceRefNumber != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Correspondence Ref Number", Convert.ToString(creditMemoRecord.CorrespondenceRefNumber), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidReferenceCorrespondence, ErrorStatus.C, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Duplicate check in current invoice - credit memo number
      if (invoice.CreditMemoRecord.Where(memoRecord => memoRecord.CreditMemoNumber.ToUpper() == creditMemoRecord.CreditMemoNumber.ToUpper()).Count() > 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Credit Memo Number", creditMemoRecord.CreditMemoNumber, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.DuplicateCreditMemoFound, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      //Duplicate check in other invoices - credit memo number
      else if (IsDuplicateCreditMemo(null, creditMemoRecord, false, invoice))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Credit Memo Number", creditMemoRecord.CreditMemoNumber, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.DuplicateCreditMemoFound, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      ////validate reason code 
      //if (!ReferenceManager.IsValidReasonCode(invoice, creditMemoRecord.ReasonCode, (int)TransactionType.CreditMemo))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reason Code", creditMemoRecord.ReasonCode, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidReasonCode, ErrorStatus.C, creditMemoRecord, true);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      //Validate Net Reject Amount
      if (exchangeRate != null && maxAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(creditMemoRecord.NetAmountCredited), TransactionType.CreditMemo, invoice.ListingCurrencyId, invoice, exchangeRate))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Credited Amount", Convert.ToString(creditMemoRecord.NetAmountCredited),
                                                   invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.NetCreditedAmountNotInRangeOfMinMax, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Batch Sequence Number and Record Sequence Number
      //if (creditMemoRecord.RecordSequenceWithinBatch <= 0 || creditMemoRecord.BatchSequenceNumber <= 0)
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number - Record Sequence Number", string.Format("{0}-{1}",creditMemoRecord.BatchSequenceNumber,creditMemoRecord.RecordSequenceWithinBatch),
      //                                             invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidBatchSequenceNoAndRecordNo, ErrorStatus.X, creditMemoRecord);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      //Validate credit memo totals 
      ValidateParsedCreditMemoTotals(creditMemoRecord, exceptionDetailsList, invoice, fileName, fileSubmissionDate);

      // Memo level VAT breakdown should not be provided when RM/BM/CM has coupon breakdown information.
      if (creditMemoRecord.VatBreakdown.Count > 0 && creditMemoRecord.CouponBreakdownRecord.Count > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "CM Vat details", string.Empty, invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.VatPresentWhenCouponBreakdownExists, ErrorStatus.X, creditMemoRecord);
        
        exceptionDetailsList.Add(validationExceptionDetail);
        
        isValid = false;
      }

      //Validate Vat Breakdowns 
      foreach (var creditMemoVat in creditMemoRecord.VatBreakdown)
      {
        isValid = ValidateParsedVat(creditMemoVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelCreditMemoVat, fileSubmissionDate, creditMemoRecord.BatchSequenceNumber, creditMemoRecord.RecordSequenceWithinBatch, creditMemoNumber, creditMemoRecord.SourceCodeId, false, true);
      }

      //Validate CreditMemoCouponBreakdownRecord 
      MemoCouponBase previousBreakdownRecord = null;
      foreach (var creditMemoCouponBreakdownRecord in creditMemoRecord.CouponBreakdownRecord)
      {
        if (previousBreakdownRecord != null && creditMemoCouponBreakdownRecord.SerialNo != previousBreakdownRecord.SerialNo + 1)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(creditMemoCouponBreakdownRecord.SerialNo),
                                              invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, creditMemoRecord.SourceCodeId, creditMemoRecord.BatchSequenceNumber, creditMemoRecord.RecordSequenceWithinBatch, creditMemoNumber, false, string.Format("{0}-{1}-{2}", creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
        }
        else if (previousBreakdownRecord == null && creditMemoCouponBreakdownRecord.SerialNo != 1)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(creditMemoCouponBreakdownRecord.SerialNo),
                                              invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, creditMemoRecord.SourceCodeId, creditMemoRecord.BatchSequenceNumber, creditMemoRecord.RecordSequenceWithinBatch, creditMemoNumber, false, string.Format("{0}-{1}-{2}", creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
        }

        // validation for check digit- bugId- 4018
        if ((creditMemoCouponBreakdownRecord.CheckDigit < 0) || (creditMemoCouponBreakdownRecord.CheckDigit > 6 && creditMemoCouponBreakdownRecord.CheckDigit != 9))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "CheckDigit", Convert.ToString(creditMemoCouponBreakdownRecord.CheckDigit), invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidCheckDigit, ErrorStatus.C, creditMemoRecord, false, string.Format("{0}-{1}-{2}", creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
        }

        previousBreakdownRecord = creditMemoCouponBreakdownRecord;

        isValid = ValidateParsedCMCouponBreakdownRecord(creditMemoCouponBreakdownRecord, exceptionDetailsList, creditMemoRecord, invoice, fileName, yourInvoice, airlineFlightDesignator, issuingAirline, fileSubmissionDate);
      }
      
      // Update expiry period of credit memo for purging.
      //creditMemoRecord.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // SCP ID : 72923 - BGEN_00007 - TG PAX file PIDECF-2172013010320130125200007.dat
      // Reason Remark Field should be Max 4000 Char

      if (creditMemoRecord.ReasonRemarks != null)
      {
          if (creditMemoRecord.ReasonRemarks.Length > MaxReasonRemarkCharLength)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                       fileSubmissionDate,
                                                                       "Reason Remarks",
                                                                       string.Empty,
                                                                       invoice,
                                                                       fileName,
                                                                       ErrorLevels.ErrorLevelCreditMemo,
                                                                       ErrorCodes.MaxReasonRemarkCharLength,
                                                                       ErrorStatus.X, creditMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;

          }

      }



      return isValid;
    }

    /// <summary>
    /// Gets the credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CreditMemoAttachment> GetCreditMemoAttachments(List<Guid> attachmentIds)
    {
      return CreditMemoAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the credit memo coupon attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CMCouponAttachment> GetCreditMemoCouponAttachments(List<Guid> attachmentIds)
    {
      return CreditMemoCouponAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    //Validate billing memo coupon breakdown record 
    public bool ValidateParsedCMCouponTotals(CMCoupon cmCouponBreakdownRecord,
                                                                IList<IsValidationExceptionDetail> exceptionDetailsList,
                                                                CreditMemo parentCoupon, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      double totalTaxAmount = 0;
      double totalVatAmount = 0;
      var tktIsuingAirline = cmCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;
      if (Convert.ToDouble(cmCouponBreakdownRecord.VatAmount) > 0 && cmCouponBreakdownRecord.VatBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(cmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", cmCouponBreakdownRecord.VatAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, parentCoupon, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, cmCouponBreakdownRecord.TicketDocOrFimNumber, cmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (Convert.ToDouble(cmCouponBreakdownRecord.TaxAmount) != 0 && cmCouponBreakdownRecord.TaxBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(cmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount", cmCouponBreakdownRecord.TaxAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.ZeroTaxBreakdownRecords, ErrorStatus.X, parentCoupon, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, cmCouponBreakdownRecord.TicketDocOrFimNumber, cmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      totalTaxAmount = cmCouponBreakdownRecord.TaxBreakdown.Sum(currentRecord => currentRecord.Amount);

      if (invoice.Tolerance != null)
      {

        if (cmCouponBreakdownRecord.TaxBreakdown.Count() > 0 && !CompareUtil.Compare(cmCouponBreakdownRecord.TaxAmount, totalTaxAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(cmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount", Convert.ToString(cmCouponBreakdownRecord.TaxAmount),
                                                  invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, parentCoupon, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, cmCouponBreakdownRecord.TicketDocOrFimNumber, cmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      totalVatAmount = cmCouponBreakdownRecord.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);

      if (invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(cmCouponBreakdownRecord.VatAmount, totalVatAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(cmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount", Convert.ToString(cmCouponBreakdownRecord.VatAmount),
                                                  invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, parentCoupon, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, cmCouponBreakdownRecord.TicketDocOrFimNumber, cmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Total net amount credited
      if (Convert.ToDecimal(cmCouponBreakdownRecord.NetAmountCredited) != (cmCouponBreakdownRecord.GrossAmountCredited + Convert.ToDecimal(cmCouponBreakdownRecord.TaxAmount) + Convert.ToDecimal(cmCouponBreakdownRecord.IscAmountBilled) + Convert.ToDecimal(cmCouponBreakdownRecord.OtherCommissionBilled.ToString())
                  + Convert.ToDecimal(cmCouponBreakdownRecord.HandlingFeeAmount) + Convert.ToDecimal(cmCouponBreakdownRecord.UatpAmountBilled) + Convert.ToDecimal(cmCouponBreakdownRecord.VatAmount)))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(cmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount", Convert.ToString(cmCouponBreakdownRecord.NetAmountCredited),
                                            invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidNetTotal, ErrorStatus.X, parentCoupon, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, cmCouponBreakdownRecord.TicketDocOrFimNumber, cmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validate Credit Memo Total Amounts
    /// </summary>
    /// <param name="creditMemoRecord"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    public bool ValidateParsedCreditMemoTotals(CreditMemo creditMemoRecord,
                                        IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      double totalTaxAmountCredited = 0, totalVatAmountCredited = 0;
      decimal totalGrossAmountCredited = 0, netCreditedAmount = 0;
      double totalHandlingFeeAmountCredited = 0, totalIscAmountCredited = 0, totalOtherCommissionAmountCredited = 0, totalUatpAmountCredited = 0;

      var creditMemoNumber = string.Empty;

      if (creditMemoRecord.CreditMemoNumber != null)
      {
        creditMemoNumber = creditMemoRecord.CreditMemoNumber;
      }

      if (creditMemoRecord.CouponBreakdownRecord.Count == 0)
      {

        //If vat amount is > 0 and vat breakdowns is 0
        if (creditMemoRecord.VatBreakdown != null && Convert.ToDouble(creditMemoRecord.VatAmount) > 0 && creditMemoRecord.VatBreakdown.Count() == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", creditMemoRecord.VatAmount.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, creditMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        double totalVatAmount = 0;

        if (creditMemoRecord.VatBreakdown != null)
        {
          totalVatAmount = creditMemoRecord.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);
        }

        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(Convert.ToDecimal(totalVatAmount), creditMemoRecord.VatAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount",
                                                                            Convert.ToString(creditMemoRecord.VatAmount),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelCreditMemo,
                                                                            ErrorCodes.InvalidTotalVatBreakdownAmounts, ErrorStatus.X,
                                                                            creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          var netAmountBilled = creditMemoRecord.TotalGrossAmountCredited + creditMemoRecord.TaxAmount + creditMemoRecord.TotalIscAmountCredited + creditMemoRecord.TotalOtherCommissionAmountCredited + Convert.ToDecimal(creditMemoRecord.TotalHandlingFeeCredited) + creditMemoRecord.TotalUatpAmountCredited + creditMemoRecord.VatAmount;

          //Total net amount billed
          if (!CompareUtil.Compare(creditMemoRecord.NetAmountCredited, netAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount", Convert.ToString(creditMemoRecord.NetAmountCredited), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidNetBilledAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }
      else
      {
        foreach (var currentRecord in creditMemoRecord.CouponBreakdownRecord)
        {
          //Gross amount
          totalGrossAmountCredited += currentRecord.GrossAmountCredited;
          //Tax amount
          totalTaxAmountCredited += currentRecord.TaxAmount;
          //Vat amount
          totalVatAmountCredited += currentRecord.VatAmount;
          //Handling fee amount
          totalHandlingFeeAmountCredited += currentRecord.HandlingFeeAmount;
          //ISC amount
          totalIscAmountCredited += currentRecord.IscAmountBilled;
          //Other commission amount
          totalOtherCommissionAmountCredited += currentRecord.OtherCommissionBilled;
          //UATP amount
          totalUatpAmountCredited += currentRecord.UatpAmountBilled;
          netCreditedAmount += Convert.ToDecimal(currentRecord.NetAmountCredited);

        }

        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(totalGrossAmountCredited, creditMemoRecord.TotalGrossAmountCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Amount", Convert.ToString(creditMemoRecord.TotalGrossAmountCredited),
                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalGrossValue, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalTaxAmountCredited), creditMemoRecord.TaxAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount", Convert.ToString(creditMemoRecord.TaxAmount),
                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalVatAmountCredited), creditMemoRecord.VatAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", Convert.ToString(creditMemoRecord.VatAmount),
                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(totalHandlingFeeAmountCredited, creditMemoRecord.TotalHandlingFeeCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount", Convert.ToString(creditMemoRecord.TotalHandlingFeeCredited),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalHandlingFeeAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalIscAmountCredited), creditMemoRecord.TotalIscAmountCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount", Convert.ToString(creditMemoRecord.TotalIscAmountCredited),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalIscAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalOtherCommissionAmountCredited), creditMemoRecord.TotalOtherCommissionAmountCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount", Convert.ToString(creditMemoRecord.TotalOtherCommissionAmountCredited),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalOtherCommissionAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalUatpAmountCredited), creditMemoRecord.TotalUatpAmountCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount", Convert.ToString(creditMemoRecord.TotalUatpAmountCredited),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidTotalUatpAmount, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(Convert.ToDecimal(netCreditedAmount), creditMemoRecord.NetAmountCredited, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Credited Amount", Convert.ToString(creditMemoRecord.NetAmountCredited),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.InvalidNetTotal, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Credit memo net total amount should not be positive.
      if (creditMemoRecord.NetAmountCredited > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Credited Amount", Convert.ToString(creditMemoRecord.NetAmountCredited),
                                                  invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.CreditMemoCouponNetTotalAmountShouldNotBeNegative, ErrorStatus.X, creditMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validates the parsed Credit memo coupon breakdown record.
    /// </summary>
    /// <param name="creditMemoCouponBreakdownRecord">The credit memo coupon breakdown record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="parentMemo">The parent coupon.</param>
    /// <param name="invoice"></param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="yourInvoice"></param>
    /// <param name="airlineFlightDesignator"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <returns></returns>
    private bool ValidateParsedCMCouponBreakdownRecord(CMCoupon creditMemoCouponBreakdownRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, CreditMemo parentMemo, PaxInvoice invoice, string fileName, PaxInvoice yourInvoice, IDictionary<string, bool> airlineFlightDesignator, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
      var isValid = true;
      var tktIsuingAirline = creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;
      //Flight date should be YYMMDD     
      if (creditMemoCouponBreakdownRecord.FlightDate.HasValue)
      {
        DateTime resultDate;
        if (!DateTime.TryParse(creditMemoCouponBreakdownRecord.FlightDate.Value.ToString(), out resultDate))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Date", creditMemoCouponBreakdownRecord.FlightDate.Value.ToString(),
                                                    invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidFlightDate, ErrorStatus.C, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber <= 0 || creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber > 4)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Number", Convert.ToString(creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(creditMemoCouponBreakdownRecord.TicketDocOrFimNumber).Length > 10)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Ticket Documnet or FIM Number",
                                                                        Convert.ToString(creditMemoCouponBreakdownRecord.TicketDocOrFimNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelCreditMemoCoupon,
                                                                        ErrorCodes.TicketFimDocumentNoGreaterThanTenNs,
                                                                        ErrorStatus.X,
                                                                        parentMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //if (creditMemoCouponBreakdownRecord.OriginalPmi != null && creditMemoCouponBreakdownRecord.AgreementIndicatorSupplied != null && !ValidateOriginalPmi(creditMemoCouponBreakdownRecord.OriginalPmi, creditMemoCouponBreakdownRecord.AgreementIndicatorSupplied))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Original PMI", creditMemoCouponBreakdownRecord.OriginalPmi,
      //                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidOriginalPMI, ErrorStatus.C, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      // Validate Ticket issuing airline.
      ValidateTicketIssuingAirline(creditMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, parentMemo.SourceCodeId, invoice, fileName, isValid, ErrorLevels.ErrorLevelCreditMemoCoupon, issuingAirline, fileSubmissionDate);

      if (!isValid)
      {
        parentMemo.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      // ISC amount
      decimal expectedAmount = ConvertUtil.Round(Convert.ToDecimal(creditMemoCouponBreakdownRecord.IscPercent) * creditMemoCouponBreakdownRecord.GrossAmountCredited / 100, Constants.PaxDecimalPlaces);
      if (parentMemo.SourceCodeId != 90 && invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(Convert.ToDecimal(creditMemoCouponBreakdownRecord.IscAmountBilled), expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "ISC Amount Credited",
                                                                          Convert.ToString(creditMemoCouponBreakdownRecord.IscAmountBilled),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelCreditMemoCoupon,
                                                                          ErrorCodes.InvalidIscPercentage,
                                                                          ErrorStatus.X,
                                                                          parentMemo,
                                                                          false,
                                                                           string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // UATP amount
      expectedAmount = ConvertUtil.Round(Convert.ToDecimal(creditMemoCouponBreakdownRecord.UatpPercent) * creditMemoCouponBreakdownRecord.GrossAmountCredited / 100, Constants.PaxDecimalPlaces);
      if (parentMemo.SourceCodeId != 90 && invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(Convert.ToDecimal(creditMemoCouponBreakdownRecord.UatpAmountBilled), expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "UATP Amount Credited",
                                                                          Convert.ToString(creditMemoCouponBreakdownRecord.UatpAmountBilled),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelCreditMemoCoupon,
                                                                          ErrorCodes.InvalidUatpPercentage,
                                                                          ErrorStatus.X,
                                                                          parentMemo,
                                                                          false,
                                                                           string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // From airport of coupon and to airport of coupon should not be same.
      if (creditMemoCouponBreakdownRecord.FromAirportOfCoupon != null && creditMemoCouponBreakdownRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim()) && !string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()))
        {
          if (String.Equals(creditMemoCouponBreakdownRecord.FromAirportOfCoupon.ToUpper(), creditMemoCouponBreakdownRecord.ToAirportOfCoupon.ToUpper()))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From airport of coupon", Convert.ToString(creditMemoCouponBreakdownRecord.FromAirportOfCoupon), invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.FromAirportOfCouponAndToAirportOfCouponShouldNotBeSame, ErrorStatus.X, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      // Validate FromAirportOfCoupon 
      if (creditMemoCouponBreakdownRecord.FromAirportOfCoupon != null && !string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim()) && !IsValidCityAirportCode(creditMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport Of Coupon", Convert.ToString(creditMemoCouponBreakdownRecord.FromAirportOfCoupon),
                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidFromAirportCode, ErrorStatus.C, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validate ToAirportOfCoupon 
      if (creditMemoCouponBreakdownRecord.ToAirportOfCoupon != null && !string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()) && !IsValidCityAirportCode(creditMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "To Airport Of Coupon", Convert.ToString(creditMemoCouponBreakdownRecord.ToAirportOfCoupon),
                                                           invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidToAirportCode, ErrorStatus.C, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validate Currency Adjustment Indicator
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        if (string.IsNullOrWhiteSpace(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Currency Adjustment Indicator", Convert.ToString(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon,
                                                                          ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.C, parentMemo,
                                                                          true, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator != null && !string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator.Trim()) && !ReferenceManager.IsValidCurrencyCode(invoice, creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Currency Adjustment Indicator", Convert.ToString(creditMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon,
                                                                          ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.C, parentMemo,
                                                                          true, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // Validate Airline Flight Designator*
      if (creditMemoCouponBreakdownRecord.AirlineFlightDesignator != null && !string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.AirlineFlightDesignator.Trim()))
      {
        isValid = ValidateAirlineFlightDesignator(parentMemo, parentMemo.SourceCodeId, invoice, fileName, exceptionDetailsList, airlineFlightDesignator, isValid, creditMemoCouponBreakdownRecord.AirlineFlightDesignator, ErrorLevels.ErrorLevelCreditMemoCoupon, fileSubmissionDate, parentMemo.CreditMemoNumber, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        if ((!isValid) && (!parentMemo.TransactionStatus.Equals(Iata.IS.Model.Common.TransactionStatus.ErrorNonCorrectable)))
        {
          parentMemo.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.ErrorCorrectable;
        }
      }

      // Validate Reference field 1 AND Field 2
      if (parentMemo.ReasonCode != null)
      {
        if (parentMemo.ReasonCode.ToUpper() == "8P")
        {
          if (string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.ReferenceField1.Trim()))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 1", Convert.ToString(creditMemoCouponBreakdownRecord.ReferenceField1),
                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidReferenceField1, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (string.IsNullOrEmpty(creditMemoCouponBreakdownRecord.ReferenceField2.Trim()))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 2", Convert.ToString(creditMemoCouponBreakdownRecord.ReferenceField2),
                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidReferenceField2, ErrorStatus.C, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Validate credit memo totals
      ValidateParsedCMCouponTotals(creditMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, invoice, fileName, fileSubmissionDate);

      //Validate Tax Breakdowns 
      foreach (var couponRecordTax in creditMemoCouponBreakdownRecord.TaxBreakdown)
      {
        isValid = ValidateParsedTax(couponRecordTax, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCouponTax, fileSubmissionDate, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, parentMemo.CreditMemoNumber, parentMemo.SourceCodeId, string.Format("{0}-{1}-{2}", tktIsuingAirline, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber, creditMemoCouponBreakdownRecord.TicketOrFimCouponNumber));

      }

      //Validate Vat Breakdowns 
      foreach (var couponRecordVat in creditMemoCouponBreakdownRecord.VatBreakdown)
      {
        isValid = ValidateParsedVat(couponRecordVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCouponVat, fileSubmissionDate, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, creditMemoCouponBreakdownRecord.TicketDocOrFimNumber.ToString(), parentMemo.SourceCodeId, false, true);
      }

      return isValid;
    }

    /// <summary>
    /// Validates the ticket issuing airline.
    /// </summary>
    /// <param name="couponRecord">The rejection memo coupon breakdown record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="parentMemo">The parent memo.</param>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <param name="errorLevel">The error level.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <returns></returns>
    private void ValidateTicketIssuingAirline(CouponBase couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, MemoBase parentMemo, int sourceCodeId, PaxInvoice invoice, string fileName, bool isValid, string errorLevel, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
      var ticketIssuingAirline = couponRecord.TicketOrFimIssuingAirline;

      // Check ticket issuing airline is present in dictionary of issuing airlines collection.
      if (!issuingAirline.Keys.Contains(ticketIssuingAirline))
      {
        if (MemberManager.IsValidAirlineCode(ticketIssuingAirline))
        {
          issuingAirline.Add(ticketIssuingAirline, true);
        }
        else
        {
          issuingAirline.Add(ticketIssuingAirline, false);
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Or FimIssuing Airline", couponRecord.TicketOrFimIssuingAirline,
                                                               invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, sourceCodeId, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, couponRecord.TicketDocOrFimNumber.ToString());
          exceptionDetailsList.Add(validationExceptionDetail);
        }
      }
      else if (!issuingAirline[ticketIssuingAirline])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Or FimIssuing Airline", couponRecord.TicketOrFimIssuingAirline,
                                                             invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, sourceCodeId, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, couponRecord.TicketDocOrFimNumber.ToString());
        exceptionDetailsList.Add(validationExceptionDetail);
      }
    }

    /// <summary>
    /// Validates the airline flight designator.
    /// </summary>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="parentMemo">The parent memo.</param>
    /// <param name="sourceCodeId">The source code id.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="airlineFlightDesignators">The airline flight designators.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <param name="airlineFlightDesignator">The airline flight designator.</param>
    /// <param name="errorLevel"></param>
    /// <returns></returns>
    private bool ValidateAirlineFlightDesignator(MemoBase parentMemo, int sourceCodeId, PaxInvoice invoice, string fileName,
      IList<IsValidationExceptionDetail> exceptionDetailsList, IDictionary<string, bool> airlineFlightDesignators,
      bool isValid, string airlineFlightDesignator, string errorLevel, DateTime fileSubmissionDate, string documentNumber, string linkedDocNumber)
    {
      if (!airlineFlightDesignators.Keys.Contains(airlineFlightDesignator))
      {
        if (MemberManager.IsValidAirlineAlphaCode(airlineFlightDesignator))
        {
          airlineFlightDesignators.Add(airlineFlightDesignator, true);
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(parentMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(airlineFlightDesignator),
                                             invoice, fileName, errorLevel, ErrorCodes.InvalidAirlineCode,
                                             ErrorStatus.C, sourceCodeId, parentMemo.BatchSequenceNumber,
                                             parentMemo.RecordSequenceWithinBatch, documentNumber, false, linkedDocNumber);
          exceptionDetailsList.Add(validationExceptionDetail);
          airlineFlightDesignators.Add(airlineFlightDesignator, false);
          isValid = false;
        }
      }
      else if (!airlineFlightDesignators[airlineFlightDesignator])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(parentMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(airlineFlightDesignator),
                                           invoice, fileName, errorLevel, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, sourceCodeId, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, documentNumber, false, linkedDocNumber);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Checks whether the specified flight day and month is a valid combination. (considers 29 days for February).
    /// </summary>
    /// <param name="flightDay">The day of the flight.</param>
    /// <param name="flightMonth">The month of the flight.</param>
    /// <returns>True if successful, false otherwise.</returns>
    private static bool IsValidFlightDate(int? flightDay, int? flightMonth)
    {
      // Ignore the validation if neither of the values have been provided (since flight date is not mandatory).
      if (!flightDay.HasValue || !flightMonth.HasValue)
      {
        return true;
      }
      // Try to parse date string to DateTime to check whether its valid.
      DateTime resultDate;
      string flightDate = string.Format("{0}/{1}/{2}", flightMonth.ToString().PadLeft(4, '0').Substring(0, 2), flightMonth.ToString().PadLeft(4, '0').Substring(2, 2), flightDay);
      return DateTime.TryParse(flightDate, out resultDate);
    }

  }
}
