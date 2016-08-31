using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Iata.IS.AdminSystem;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Business.Common;

namespace Iata.IS.Business.Pax.Impl
{
  public class SamplingRejectionManager : InvoiceManager, ISamplingRejectionManager, IValidationSFormFManager
  {
    private const string IsValidationFlagDuplicate = "DU";
    
   /// <summary>
    /// Gets or sets the type of the transaction.
    /// </summary>
    /// <value>The type of the transaction.</value>
    protected TransactionType RejectionTransactionType { get; set; }

    /// <summary>
    /// Gets or sets the parent billing code.
    /// </summary>
    /// <value>The parent billing code.</value>
    protected BillingCode YourInvoiceBillingCode { get; set; }

    /// <summary>
    /// Gets or sets the rejection stage.
    /// </summary>
    /// <value>The rejection stage.</value>
    protected RejectionStage RejectionStage { get; set; }

    /// <summary>
    /// Gets or sets the reason billing code.
    /// </summary>
    /// <value>The reason billing code.</value>
    protected BillingCode BillingCode { get; set; }

    /// <summary>
    /// RejectionMemoRepository, will be injected by container.
    /// </summary>
    /// <value>The rejection memo repository.</value>
    public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo vat repository.
    /// </summary>
    public IRepository<RejectionMemoVat> RejectionMemoVatRepository { get; set; }

    /// <summary>
    /// RejectionMemoAttachmentRepository, will be injected by the container.
    /// </summary>
    public IRejectionMemoAttachmentRepository RejectionMemoAttachmentRepository { get; set; }

    /// <summary>
    ///  RejectionMemoCouponTaxBreakdownRepository, will be injected by the container.
    /// </summary>
    public IRepository<RMCouponTax> RejectionMemoCouponTaxBreakdownRepository { get; set; }

    /// <summary>
    /// RejectionMemoCouponAttachmentRepository,  will be injected by the container.
    /// </summary>
    public IRejectionMemoCouponAttachmentRepository RejectionMemoCouponAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo Coupon Vat Breakdown repository.
    /// </summary>
    public IRepository<RMCouponVat> RejectionMemoCouponVatBreakdownRepository { get; set; }

    private const string TimeLimitFlag = "TL";
    private const string ValidationFlagDelimeter = ",";

    /// <summary>
    /// GetRejectionMemoList
    /// </summary>
    /// <param name="invoiceId">Rejection Memo list for this invoice id.</param>
    /// <returns></returns>
    public IList<RejectionMemo> GetRejectionMemoList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var rejectionMemoList = RejectionMemoRepository.Get(rm => rm.InvoiceId == invoiceGuid);

      var reasonCodes = rejectionMemoList.Select(sfcRecord => sfcRecord.ReasonCode.ToUpper());
      var reasonCodesfromDb = ReasonCodeRepository.Get(reasonCode => reasonCodes.Contains(reasonCode.Code.ToUpper())).ToList();

      if (reasonCodesfromDb.Count() > 0)
      {
        foreach (var rejectionMemoRecord in rejectionMemoList)
        {
          var record = rejectionMemoRecord;
          rejectionMemoRecord.ReasonCodeDescription = reasonCodesfromDb.Single(rCode => rCode.Code == record.ReasonCode
            && rCode.TransactionTypeId == (int)RejectionTransactionType).Description;
        }
      }

      return rejectionMemoList.ToList();
    }

    /// <summary>
    /// GetRejectionMemoRecordDetails
    /// </summary>
    /// <param name="rejectionMemoRecordId">rejectionMemoRecordId.</param>
    /// <returns></returns>
    public RejectionMemo GetRejectionMemoRecordDetails(string rejectionMemoRecordId)
    {
      var rejectionMemoRecordGuid = rejectionMemoRecordId.ToGuid();
      //Replaced with LoadStrategy call
      var rejectionMemo = RejectionMemoRepository.Single(rejectionMemoRecordGuid);
      return rejectionMemo;
    }

    /// <summary>
    /// Add RejectionMemo Record.
    /// </summary>
    /// <param name="rejectionMemoRecord">RejectionMemoRecord to be added.</param>
    /// <param name="warningMessage">Warning message</param>
    /// <returns></returns>
    public RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage)
    {
      // Get invoice for rejection memo record.
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
        //Commneted below line, as only invoice header information is used. 
        //var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);

        var rejectionInvoice = InvoiceRepository.Get(i=>i.Id==rejectionMemoRecord.InvoiceId).SingleOrDefault();

      // This code of setting rejection stage was written after call of ValidateRejectionMemo method.
      // But for newly added server side validation for Rejection stage in SCP225675, rejection should be set before validation.
      // Set rejection stage for rejection memo.
      rejectionMemoRecord.RejectionStage = (int)RejectionStage;

      ValidateRejectionMemo(rejectionMemoRecord, null, rejectionInvoice, out warningMessage);

      // Carrying forward sampling constants of Invoice Header to rejection memo for further calculations
      rejectionMemoRecord.SamplingConstant = rejectionInvoice.SamplingConstant;

      // Update Invoice status to Open 
      rejectionInvoice.InvoiceStatus = InvoiceStatusType.Open;
      rejectionInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
      rejectionInvoice.ValidationDate = DateTime.MinValue;

      InvoiceRepository.Update(rejectionInvoice);

      if (rejectionInvoice.BillingCode == (int)BillingCode.SamplingFormF)
        rejectionMemoRecord.IsLinkingSuccessful = rejectionInvoice.IsFormDEViaIS;
      else if (rejectionInvoice.BillingCode == (int)BillingCode.SamplingFormXF) rejectionMemoRecord.IsLinkingSuccessful = rejectionInvoice.IsFormFViaIS;

      RejectionMemoRepository.Add(rejectionMemoRecord);

      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      try
      {
        UnitOfWork.CommitDefault();
      }
      catch (Exception exception)
      {
        // If table level constraint for Rejection Stage, throws the exception then will throw it as BusinessException.
        if (exception.InnerException.Message.Contains("CK_RM#REJ_STG"))
        {
          throw new ISBusinessException(ErrorCodes.InvalidRejectionStageAttemptedToSave);
        }
        throw;
      }

      if (rejectionMemoRecord.IsLinkingSuccessful.HasValue && rejectionMemoRecord.IsLinkingSuccessful.Value && rejectionMemoRecord.CouponBreakdownRecord.Count == 0)
      {
        if (rejectionMemoRecord.IsBreakdownAllowed.HasValue && rejectionMemoRecord.IsBreakdownAllowed.Value)
          RejectionMemoRepository.InheritRMCouponDetails(rejectionMemoRecord.Id);
        rejectionMemoRecord.IsBreakdownAllowed = true;
      }
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoRecord.Id, rejectionMemoRecord.LastUpdatedBy);
      return rejectionMemoRecord;
    }

    /// <summary>
    /// To update rejection memo record
    /// </summary>
    /// <param name="rejectionMemoRecord">Details of the rejection memo record</param>
    /// <param name="warningMessage">Warning message</param>
    /// <returns>Updated rejection memo record</returns>
    public RejectionMemo UpdateRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage)
    {
        var logRefId = Guid.NewGuid();
        var log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                                        this.GetType().ToString(), "Stage 1: Call to RejectionMemoRepository Sent ", 0, logRefId.ToString());
        ReferenceManager.LogDebugData(log);

      //Replaced with loadStrategy call
      var rejectionMemoRecordInDb = RejectionMemoRepository.Single(rejectionMemoRecord.Id);
      // Get invoice for rejection memo record.

       log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                                    this.GetType().ToString(), "Stage 1: Call to RejectionMemoRepository completed ", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);

      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);
      var rejectionInvoice = InvoiceRepository.GetInvoiceHeader(rejectionMemoRecord.InvoiceId);

      // This code of setting rejection stage was written after call of ValidateRejectionMemo method.
      // But for newly added server side validation for Rejection stage in SCP225675, rejection should be set before validation.
      // Set rejection stage for rejection memo.
      rejectionMemoRecord.RejectionStage = (int)RejectionStage;

      log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                                  this.GetType().ToString(), "Stage 2:  GetInvoiceHeader completed ", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);

      // Validate rejection memo record
      ValidateRejectionMemo(rejectionMemoRecord, rejectionMemoRecordInDb, rejectionInvoice, out warningMessage);

      log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                                this.GetType().ToString(), "Stage 3: ValidateRejectionMemo completed ", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);


      // Set sampling constants for further calculations.
      rejectionMemoRecord.SamplingConstant = rejectionInvoice.SamplingConstant;

      if (rejectionInvoice.BillingCode == (int)BillingCode.SamplingFormF)
        rejectionMemoRecord.IsLinkingSuccessful = rejectionInvoice.IsFormDEViaIS;
      else if (rejectionInvoice.BillingCode == (int)BillingCode.SamplingFormXF) rejectionMemoRecord.IsLinkingSuccessful = rejectionInvoice.IsFormFViaIS;

      log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                             this.GetType().ToString(), "Stage 4: RejectionMemoRepository.Update start", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);

        // Update the rejection memo.
      var updatedRejectionMemo = RejectionMemoRepository.Update(rejectionMemoRecord);

      log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                           this.GetType().ToString(), "Stage 4: RejectionMemoRepository.Update completed", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);


      //Changes to update tax breakdown records
      var listToDeleteVat = rejectionMemoRecordInDb.RejectionMemoVat.Where(vat => rejectionMemoRecord.RejectionMemoVat.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in rejectionMemoRecord.RejectionMemoVat.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        vat.ParentId = rejectionMemoRecord.Id;
        RejectionMemoVatRepository.Add(vat);
      }

      foreach (var rejectionMemoVat in listToDeleteVat)
      {
        RejectionMemoVatRepository.Delete(rejectionMemoVat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = rejectionMemoRecordInDb.Attachments.Where(attachment => rejectionMemoRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in rejectionMemoRecord.Attachments
                              where rejectionMemoRecordInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var rmAttachmentInDb = RejectionMemoAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));
      foreach (var recordAttachment in rmAttachmentInDb)
      {
        if (IsDuplicateRejectionMemoAttachmentFileName(recordAttachment.OriginalFileName, rejectionMemoRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = rejectionMemoRecord.Id;
        RejectionMemoAttachmentRepository.Update(recordAttachment);
      }

      foreach (var attachment in listToDeleteAttachment)
      {
        RejectionMemoAttachmentRepository.Delete(attachment);
      }

      // Update Invoice status to Open 
      rejectionInvoice.InvoiceStatus = InvoiceStatusType.Open;
      rejectionInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
      rejectionInvoice.ValidationDate = DateTime.MinValue;

      log = ReferenceManager.GetDebugLog(DateTime.Now, "UpdateRejectionMemoRecord", this.ToString(),
                           this.GetType().ToString(), "Stage 5: InvoiceRepository.Update(rejectionInvoice) start", 0, logRefId.ToString());
      ReferenceManager.LogDebugData(log);


      InvoiceRepository.Update(rejectionInvoice);

      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      try
      {
        UnitOfWork.CommitDefault();
      }
      catch (Exception exception)
      {
        // If table level constraint for Rejection Stage, throws the exception then will throw it as BusinessException.
        if (exception.InnerException.Message.Contains("CK_RM#REJ_STG"))
        {
          throw new ISBusinessException(ErrorCodes.InvalidRejectionStageAttemptedToSave);
        }
        throw;
      }

      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoRecord.Id, rejectionMemoRecord.LastUpdatedBy);

      return updatedRejectionMemo;
    }

    /// <summary>
    /// Delete RejectionMemo Record.
    /// </summary>
    /// <param name="rejectionMemoRecordId">rejectionMemoRecordId to be deleted.</param>
    /// <returns></returns>
    public bool DeleteRejectionMemoRecord(string rejectionMemoRecordId)
    {
      var rejectionMemoRecordGuid = rejectionMemoRecordId.ToGuid();

      //Replaced with LoadStrategy call
      var rejectionMemo = RejectionMemoRepository.Single(rejectionMemoRecordGuid);
      var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemo.InvoiceId);

      if (rejectionMemo == null) return false;
      RejectionMemoRepository.Delete(rejectionMemo);

      // Update Invoice status to Open 
      rejectionInvoice.InvoiceStatus = InvoiceStatusType.Open;
      rejectionInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
      rejectionInvoice.ValidationDate = DateTime.MinValue;
      InvoiceRepository.Update(rejectionInvoice);

      UnitOfWork.CommitDefault();

      // Update rejection memo invoice total.
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemo.InvoiceId, rejectionMemo.SourceCodeId, rejectionMemo.Id, rejectionMemo.LastUpdatedBy);

      return true;
    }

    /// <summary>
    /// Get rejection memo attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    public RejectionMemoAttachment GetRejectionMemoAttachmentDetails(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = RejectionMemoAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);
      return attachmentRecord;
    }

    /// <summary>
    /// Add rejection memo attachment record
    /// </summary>
    /// <param name="attach">rejection meo attachment record</param>
    /// <returns></returns>
    public RejectionMemoAttachment AddRejectionMemoAttachment(RejectionMemoAttachment attach)
    {
      RejectionMemoAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      attach = RejectionMemoAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update parent id of rejection memo attachment record for given Guids
    /// </summary>
    /// <param name="attachments">list of Guid of rejection memo attachment record</param>
    /// <param name="parentId">rejection memo id</param>
    /// <returns></returns>
    public IList<RejectionMemoAttachment> UpdateRejectionMemoAttachment(List<Guid> attachments, Guid parentId)
    {
      var rmAttachmentInDb = RejectionMemoAttachmentRepository.Get(couponAttachment => attachments.Contains(couponAttachment.Id));
      foreach (var recordAttachment in rmAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        RejectionMemoAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return rmAttachmentInDb.ToList();
    }

    /// <summary>
    /// Check for duplicate file name of rejection memo attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="rejectionMemoId">rejection Memo Id</param>
    /// <returns></returns>
    public bool IsDuplicateRejectionMemoAttachmentFileName(string fileName, Guid rejectionMemoId)
    {
      return RejectionMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == rejectionMemoId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// GetRejection MemoCoupon Breakdown List.
    /// </summary>
    /// <param name="memoRecordId">GetRejection MemoCoupon Breakdown List for this memoRecord.</param>
    /// <returns></returns>
    public IList<RMCoupon> GetRejectionMemoCouponBreakdownList(string memoRecordId)
    {
      var memoRecordGuid = memoRecordId.ToGuid();
      var rejectionMemoCouponBreakdownList = RejectionMemoCouponBreakdownRepository.Get(rmCoupon => rmCoupon.RejectionMemoId == memoRecordGuid);

      return rejectionMemoCouponBreakdownList.ToList();
    }

    /// <summary>
    /// Get Rejection MemoCoupon Details.
    /// </summary>
    /// <param name="couponBreakdownRecordId">couponBreakdownRecordId to be fetched.</param>
    /// <returns></returns>
    public RMCoupon GetRejectionMemoCouponDetails(string couponBreakdownRecordId)
    {
      var couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();
      //LoadStrategy call
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Reverted the code
       var rmCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(couponBreakdownRecordGuid);
      //var rmCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Get(i=>i.Id==couponBreakdownRecordGuid).SingleOrDefault();
      //var rmCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(rmCoupon => rmCoupon.Id == couponBreakdownRecordGuid);

      return rmCouponBreakdownRecord;
    }

    /// <summary>
    /// Add RejectionMemo CouponDetails.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">RejectionMemoCouponBreakdownRecord to be added.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    public RMCoupon AddRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, out string duplicateErrorMessage)
    {
      //Replaced with LoadStrategy call
      var rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoCouponBreakdownRecord.RejectionMemoId);

      // Get invoice for rejection memo record.
      var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);

      //SCP289215 - UA Ticket 618 729 0229461 cpn 1, validate CPN on Create and Edit
      if ((BillingCode)rejectionInvoice.BillingCode == BillingCode.SamplingFormF)
      {
        //CMP#459 : Validate Amount at coupon level.
        var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
        IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
        ValidateAmountsInRMonCouponLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, rejectionMemoCouponBreakdownRecord);
        if (exceptionDetailsList.Where(err => err.ErrorStatus == "2").Count() > 0)
        {
          var errorDesc = string.Empty;
          foreach (var err in exceptionDetailsList.Where(err => err.ErrorStatus == "2"))
          {
            errorDesc += err.ErrorDescription + " ¥ ";
          }
          throw new ISBusinessException(errorDesc);
        }
      }

      // Validation on rejection memo coupon records.
      duplicateErrorMessage = ValidateRejectionMemoCoupon(rejectionMemoCouponBreakdownRecord, null, rejectionInvoice, rejectionMemoRecord);
      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        rejectionMemoCouponBreakdownRecord.ISValidationFlag = IsValidationFlagDuplicate;
      }
      // Serial Number for RejectionMemo CouponBreakdown record
      rejectionMemoCouponBreakdownRecord.SerialNo = GetSerialNo(rejectionMemoRecord);

      RejectionMemoCouponBreakdownRepository.Add(rejectionMemoCouponBreakdownRecord);

      // Change invoice status to Open if any transaction happed in Form F/XF
      rejectionInvoice.InvoiceStatus = InvoiceStatusType.Open;
      rejectionInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
      rejectionInvoice.ValidationDate = DateTime.MinValue;
      InvoiceRepository.Update(rejectionInvoice);
      
      UnitOfWork.CommitDefault();
      
      // Update rejection memo invoice total. 
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoCouponBreakdownRecord.RejectionMemoId, rejectionMemoCouponBreakdownRecord.LastUpdatedBy);

      return rejectionMemoCouponBreakdownRecord;
    }

    /// <summary>
    /// Gets the serial no.
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <returns></returns>
    private int GetSerialNo(RejectionMemo rejectionMemoRecord)
    {
      var serialNo = 1;
      var rejectionMemoCouponRecord = RejectionMemoCouponBreakdownRepository.Get(rmCoupon => rmCoupon.RejectionMemoId == rejectionMemoRecord.Id).OrderByDescending(rm => rm.SerialNo).FirstOrDefault();
      if (rejectionMemoCouponRecord != null)
      {
        serialNo = rejectionMemoCouponRecord.SerialNo + 1;
      }

      return serialNo;
    }

    /// <summary>
    /// Update RejectionMemo Coupon Details.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">RejectionMemoCouponBreakdownRecord to be updated.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Updated coupon breakdown record</returns>
    public RMCoupon UpdateRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, out string duplicateErrorMessage)
    {
      //LoadStrategy call
      var rejectionMemoCouponBreakdownRecordWithDetail = RejectionMemoCouponBreakdownRepository.Single(rejectionMemoCouponBreakdownRecord.Id);

      //Replaced with LoadStrategy call
      var rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoCouponBreakdownRecord.RejectionMemoId);
      // Get invoice for rejection memo record.
      var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);

      // Validation on rejection memo coupon records.
      duplicateErrorMessage = ValidateRejectionMemoCoupon(rejectionMemoCouponBreakdownRecord, rejectionMemoCouponBreakdownRecordWithDetail, rejectionInvoice, rejectionMemoRecord);

      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        rejectionMemoCouponBreakdownRecord.ISValidationFlag = IsValidationFlagDuplicate;
      }
      var updatedrejectionMemoCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Update(rejectionMemoCouponBreakdownRecord);

      // Changes to update tax breakdown records along with RejectionMemoCouponBreakdownRecord.
      var listToDelete = rejectionMemoCouponBreakdownRecordWithDetail.TaxBreakdown.Where(tax => rejectionMemoCouponBreakdownRecord.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0).ToList();

      foreach (var tax in rejectionMemoCouponBreakdownRecord.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0))
      {
        RejectionMemoCouponTaxBreakdownRepository.Add(tax);
      }

      foreach (var tax in listToDelete)
      {
        RejectionMemoCouponTaxBreakdownRepository.Delete(tax);
      }

      // To store records to be deleted in another collection[Deletion of record in loop throw error].
      var listToDeleteVat = rejectionMemoCouponBreakdownRecordWithDetail.VatBreakdown.Where(vat => rejectionMemoCouponBreakdownRecord.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in rejectionMemoCouponBreakdownRecord.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        RejectionMemoCouponVatBreakdownRepository.Add(vat);
      }

      foreach (var vat in listToDeleteVat)
      {
        RejectionMemoCouponVatBreakdownRepository.Delete(vat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = rejectionMemoCouponBreakdownRecordWithDetail.Attachments.Where(attachment => rejectionMemoCouponBreakdownRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in rejectionMemoCouponBreakdownRecord.Attachments
                              where rejectionMemoCouponBreakdownRecordWithDetail.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var rmCouponAttachmentInDb = RejectionMemoCouponAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));

      foreach (var recordAttachment in rmCouponAttachmentInDb)
      {
        if (IsDuplicateRejectionMemoCouponAttachmentFileName(recordAttachment.OriginalFileName, rejectionMemoCouponBreakdownRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = rejectionMemoCouponBreakdownRecord.Id;
        RejectionMemoCouponAttachmentRepository.Update(recordAttachment);
      }

      foreach (var attachment in listToDeleteAttachment)
      {
        RejectionMemoCouponAttachmentRepository.Delete(attachment);
      }

      // Update Invoice status to Open 
      rejectionInvoice.InvoiceStatus = InvoiceStatusType.Open;
      rejectionInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
      rejectionInvoice.ValidationDate = DateTime.MinValue;
      InvoiceRepository.Update(rejectionInvoice);

      UnitOfWork.CommitDefault();

      // Update rejection memo invoice total 
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoCouponBreakdownRecord.RejectionMemoId, rejectionMemoCouponBreakdownRecord.LastUpdatedBy);

      return updatedrejectionMemoCouponBreakdownRecord;
    }

    /// <summary>
    /// Delete RejectionMemo Coupon Record.
    /// </summary>
    /// <param name="couponBreakdownRecordId">couponBreakdownRecordId to be deleted.</param>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public bool DeleteRejectionMemoCouponRecord(string couponBreakdownRecordId, ref Guid rejectionMemoId, ref Guid invoiceId)
    {
      var couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();
      //LoadStrategy call
      var couponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(couponBreakdownRecordGuid);
      //Replaced with LoadStrategy call
      var rejectionMemo = RejectionMemoRepository.Single(couponBreakdownRecord.RejectionMemoId);
      rejectionMemoId = rejectionMemo.Id;
      var rejectionInvoice = InvoiceRepository.Single(id: rejectionMemo.InvoiceId);
      invoiceId = rejectionInvoice.Id;

      // Delete RM Coupon, re-sequence serial numbers of subsequent coupons and update Invoice Total.
      InvoiceRepository.DeleteRejectionMemoCoupon(couponBreakdownRecordGuid);

      return true;
    }

    /// <summary>
    /// Get rejection memo Coupon attachment details
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    public RMCouponAttachment GetRejectionMemoCouponAttachmentDetails(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = RejectionMemoCouponAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);
      return attachmentRecord;
    }

    /// <summary>
    /// Add rejection memo Coupon attachment record
    /// </summary>
    /// <param name="attach">rejection memo Coupon attachment record</param>
    /// <returns></returns>
    public RMCouponAttachment AddRejectionMemoCouponAttachment(RMCouponAttachment attach)
    {
      RejectionMemoCouponAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Commented below line ...just return the object
      //attach = RejectionMemoCouponAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update parent id of rejection memo Coupon attachment record for given Guids
    /// </summary>
    /// <param name="attachments">list of Guid of rejection memo Coupon attachment record</param>
    /// <param name="parentId">rejection memo id</param>
    /// <returns></returns>
    public IList<RMCouponAttachment> UpdateRejectionMemoCouponAttachment(IList<Guid> attachments, Guid parentId)
    {
      var rmCouponAttachmentInDb = RejectionMemoCouponAttachmentRepository.Get(couponAttachment => attachments.Contains(couponAttachment.Id));
      foreach (var recordAttachment in rmCouponAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        RejectionMemoCouponAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return rmCouponAttachmentInDb.ToList();
    }


    /// <summary>
    /// Deletes the sampling form C attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    public bool DeleteRejectionMemoCouponAttachment(string attachmentId)
    {
      var attachGuid = attachmentId.ToGuid();
      // Get Attachment entity from the list
      var rejectionMemoCouponAttachment = RejectionMemoCouponAttachmentRepository.Single(attachment => attachment.Id == attachGuid);

      if (rejectionMemoCouponAttachment == null) return false;
      RejectionMemoCouponAttachmentRepository.Delete(rejectionMemoCouponAttachment);
      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// Check for duplicate file name of rejection memo Coupon attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="rejectionMemoCouponId">rejection Memo Coupon Id</param>
    /// <returns></returns>
    public bool IsDuplicateRejectionMemoCouponAttachmentFileName(string fileName, Guid rejectionMemoCouponId)
    {
      return RejectionMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == rejectionMemoCouponId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Determines whether transaction exists for the specified rejection memo id.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// True if transaction exists for the specified rejection memo id; otherwise, false
    /// </returns>
    public bool IsTransactionExists(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      return RejectionMemoRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoiceGuid) > 0;
    }

    /// <summary>
    /// Determines whether [is coupon exists] [the specified rejection memo id].
    /// </summary>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <returns>
    /// 	True if coupon exists for the specified rejection memo id; otherwise, false.
    /// </returns>
    public bool IsCouponExists(string rejectionMemoId)
    {
      var rejectionMemoGuid = rejectionMemoId.ToGuid();
      return RejectionMemoCouponBreakdownRepository.GetCount(rmCouponBreakdown => rmCouponBreakdown.RejectionMemoId == rejectionMemoGuid) > 0;
    }

    /// <summary>
    /// Gets the rejection memo attachment.
    /// </summary>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <returns></returns>
    public List<RejectionMemoAttachment> GetRejectionMemoAttachment(string rejectionMemoId)
    {
      var rejectionMemoGuid = rejectionMemoId.ToGuid();
      return RejectionMemoAttachmentRepository.Get(attachment => attachment.ParentId == rejectionMemoGuid).ToList();
    }

    /// <summary>
    /// Gets the rejection memo attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<RejectionMemoAttachment> GetRejectionMemoAttachments(List<Guid> attachmentIds)
    {
      return RejectionMemoAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the rejection memo coupon attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<RMCouponAttachment> GetRejectionMemoCouponAttachments(List<Guid> attachmentIds)
    {
      return RejectionMemoCouponAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Validate Rejection memo record.
    /// </summary>
    /// <param name="rejectionMemoRecord">Rejection memo record to be validated.</param>
    /// <param name="rejectionMemoRecordInDb">The rejection memo record in db.</param>
    /// <param name="rejectionInvoice">The rejection invoice.</param>
    /// <param name="warningMessage">Warning Message.</param>
    /// <returns></returns>
    protected virtual PaxInvoice ValidateRejectionMemo(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordInDb, PaxInvoice rejectionInvoice, out string warningMessage)
    {
      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      if (rejectionMemoRecord.RejectionStage < 1 || rejectionMemoRecord.RejectionStage > 3)
      {
        throw new ISBusinessException(ErrorCodes.InvalidRejectionStage);
      }

      var isUpdateOperation = false;
      if (rejectionMemoRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (rejectionMemoRecord.RecordSequenceWithinBatch <= 0 || rejectionMemoRecord.BatchSequenceNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }

      // Check whether Batch and Sequence number combination is valid and check whether Batch number is not repeated between different source codes
      int invalidBatchSequenceNumber = InvoiceRepository.IsValidBatchSequenceNo(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.RecordSequenceWithinBatch, rejectionMemoRecord.BatchSequenceNumber, rejectionMemoRecord.Id, rejectionMemoRecord.SourceCodeId);

      // If value != 0, either Batch and Sequence number combination is invalid or Batch number is repeated between different source codes  
      if (invalidBatchSequenceNumber != 0)
      {
        // If value == 1, Batch number is repeated between different source codes, else if value == 2, Batch and Sequence number combination is invalid  
        if (invalidBatchSequenceNumber == 1)
          throw new ISBusinessException(ErrorCodes.InvalidBatchNo);
        else
          throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNo);
      }

      // Transactions can not be added when invoice is in submitted status.
      // Transactions can be added only when Invoice Status is Open, ReadyForSubmission or ValidationError.
      if (rejectionInvoice.InvoiceStatus != InvoiceStatusType.Open
        && rejectionInvoice.InvoiceStatus != InvoiceStatusType.ReadyForSubmission
        && rejectionInvoice.InvoiceStatus != InvoiceStatusType.ValidationError)
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidOperationOnSubmitStatus);
      }

      //if (rejectionInvoice.InvoiceNumber.ToUpper().Equals(rejectionMemoRecord.YourInvoiceNumber.ToUpper()))
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidYourInvoiceNumber);
      //}

      if (!((rejectionInvoice.BillingYear > rejectionMemoRecord.YourInvoiceBillingYear) ||
        ((rejectionInvoice.BillingYear == rejectionMemoRecord.YourInvoiceBillingYear) && (rejectionInvoice.BillingMonth > rejectionMemoRecord.YourInvoiceBillingMonth)) ||
          ((rejectionInvoice.BillingYear == rejectionMemoRecord.YourInvoiceBillingYear) && (rejectionInvoice.BillingMonth == rejectionMemoRecord.YourInvoiceBillingMonth) && (rejectionInvoice.BillingPeriod > rejectionMemoRecord.YourInvoiceBillingPeriod))))
        throw new ISBusinessException(ErrorCodes.InvalidYourBillingPeriod);

      // Validates whether source code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoRecordInDb.SourceCodeId, rejectionMemoRecord.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(rejectionMemoRecord.SourceCodeId, (int)RejectionTransactionType))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }

      // Validates whether reason code exist in master table.
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoRecordInDb.ReasonCode, rejectionMemoRecord.ReasonCode))
      {
        if (!ReferenceManager.IsValidReasonCode(rejectionMemoRecord.ReasonCode, (int)RejectionTransactionType))
        {
          throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
        }
      }

      // “1A” cannot be used for RMs having more than one coupon breakdown record.
      // TODO: Validation required
      //var couponRecord = RejectionMemoCouponBreakdownRepository.GetCount(rmCouponRecord => rmCouponRecord.Id == rejectionMemoRecord.Id);

      //if (rejectionMemoRecord.ReasonCode.Equals(ReasonCode) && couponRecord > 0)
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
      //}

      // Should be a unique number within each Billed Airline in the Billing period.
      if (IsDuplicateRejectionMemoNumber(rejectionMemoRecord, rejectionMemoRecordInDb, isUpdateOperation, rejectionInvoice))
      {
        throw new ISBusinessException(ErrorCodes.DuplicateRejectionMemoNumber);
      }
      var transactionType = GetTransactionType(rejectionMemoRecord.RejectionStage);
      var isCouponBreakdownMandatory =
        ReasonCodeRepository.GetCount(
          reasonCode => reasonCode.Code.ToUpper() == rejectionMemoRecord.ReasonCode.ToUpper() && reasonCode.TransactionTypeId == (int)transactionType && reasonCode.CouponAwbBreakdownMandatory) > 0;

      // If linking is successful and coupon breakdown exist then validation for acceptable amount difference and Net Amount limit validation is not required.
      // as we are inheriting the RM coupons amount details from the rejected memo.
      if (IsAmountValidationRequired(rejectionMemoRecord, isCouponBreakdownMandatory))
      {

        // Validate acceptable amount difference with ReasonCode if Coupon Breakdown is not allowed while creating Or 
        // Validate it while updating if rejection memo coupon breakdown does not exists.
        if (RejectionMemoCouponBreakdownRepository.GetCount(coupon => coupon.RejectionMemoId == rejectionMemoRecord.Id) == 0)
        {
          var diffenceNotAcceptable = ValidateAcceptableDifferences(rejectionMemoRecord.ReasonCode,
                                                                    TransactionType.SamplingFormXF,
                                                                    rejectionMemoRecord.TotalGrossDifference,
                                                                    rejectionMemoRecord.TotalTaxAmountDifference,
                                                                    rejectionMemoRecord.TotalVatAmountDifference,
                                                                    rejectionMemoRecord.IscDifference,
                                                                    rejectionMemoRecord.UatpAmountDifference,
                                                                    rejectionMemoRecord.HandlingFeeAmountDifference,
                                                                    rejectionMemoRecord.OtherCommissionDifference);

          if (!string.IsNullOrEmpty(diffenceNotAcceptable))
          {
            throw new ISBusinessException(ErrorCodes.InvalidAcceptableAmountDifference, diffenceNotAcceptable);
          }
        }
      }


      double vatBreakdownTotal;
      // Check whether operation is EditRejectionMemo and CouponBreakdownRecord exists, if yes retrieve TotalVatAmountDifference from Coupon level else retrieve from VatAmount from Memo level. 
      if (isUpdateOperation && rejectionMemoRecordInDb.CouponBreakdownRecord.Count > 0)
      {
        vatBreakdownTotal = ConvertUtil.Round(rejectionMemoRecordInDb.TotalVatAmountDifference, Constants.PaxDecimalPlaces);
      }
      else
      {
        vatBreakdownTotal = ConvertUtil.Round(rejectionMemoRecord.RejectionMemoVat.Sum(vat => vat.VatCalculatedAmount), Constants.PaxDecimalPlaces);
      }

      // Check Coupon Vat Breakdown Total Amount to match Coupon Vat Amount.
      // If Not, then throw exception. 
      if (!rejectionMemoRecord.TotalVatAmountDifference.Equals(Convert.ToDouble(vatBreakdownTotal)))
      {
        throw new ISBusinessException(ErrorCodes.VatTotalAmountMismatch);
      }

      //Validate Time Limit
      if (IsTransactionOutSideTimeLimit(rejectionMemoRecord, rejectionInvoice, null))
      {
        rejectionMemoRecord.ISValidationFlag += string.IsNullOrEmpty(rejectionMemoRecord.ISValidationFlag) ? TimeLimitFlag : ValidationFlagDelimeter + TimeLimitFlag;
      }

      warningMessage = string.Empty;
      if(rejectionMemoRecord.ReasonCode == "1A")
      {
        warningMessage = Messages.ResourceManager.GetString(ErrorCodes.WarningMessageForReasonCode1A);
      }

      //CMP#641: Time Limit Validation on Third Stage PAX Rejections
      if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree && rejectionInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
          ValidatePaxStageThreeRmForTimeLimit(TransactionType.SamplingFormXF, rejectionInvoice.SettlementMethodId, rejectionMemoRecord, rejectionInvoice, isIsWeb: true);
      }

      ////CMP#459 : Validate Memo level amounts
      //if (isUpdateOperation)
      //{
      //    var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
      //    IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
      //    ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord);
      //    if (exceptionDetailsList.Where(err => err.ErrorStatus == "2").Count() > 0)
      //    {
      //        var errorDesc = string.Empty;
      //        foreach (var err in exceptionDetailsList.Where(err => err.ErrorStatus == "2"))
      //        {
      //            errorDesc += err.ErrorDescription + " ¥ ";
      //        }
      //        throw new ISBusinessException(errorDesc);
      //    }
      //}

      /* CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text */
      /* Check if validation is applicable or not */
      if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo || rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
      {
          /* CMP#671 - Validation Applicable */
          ValidateReasonTextMinLength(paxRejectionMemoRecord: rejectionMemoRecord);
      }
      //else
      //{
      //    For Logical Completion - CMP#671 - Validtion is not Applicable
      //}

      return rejectionInvoice;
    }

    /// <summary>
    /// Determines whether amount validation required for specified rejection memo record.
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="isCouponBreakdownMandatory">If coupon breakdown mandatory.</param>
    /// <returns>
    /// true if amount validation required for specified rejection memo record]; otherwise, false.
    /// </returns>
    private bool IsAmountValidationRequired(RejectionMemo rejectionMemoRecord, bool isCouponBreakdownMandatory)
    {
      // if linking is not successful, then check if coupon breakdown is mandatory for that reason code.
      // if not, then validations must be performed at RM level.
      if (!(rejectionMemoRecord.IsLinkingSuccessful.HasValue && rejectionMemoRecord.IsLinkingSuccessful.Value))
      {
        if (!isCouponBreakdownMandatory)
        {
          return true;
        }
      }
      else
      {
        // when linking is successful, then check validations are required when CM , BM or RM is rejected for which coupons are not present.
        // this is determined using FIMBMCMIndicator for Stage 1 and Stage Number for Stage 2 and 3.
        // In such cases, if rejected memo has no coupons (IsBreakAllowed value set to false), then validations are required.
        if ((rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.CMNumber || rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.BMNumber) || (rejectionMemoRecord.RejectionStage != (int)RejectionStage.StageOne))
        {
          if (rejectionMemoRecord.IsBreakdownAllowed.HasValue && !rejectionMemoRecord.IsBreakdownAllowed.Value)
          {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether duplicate rejection memo for the specified rejection memo record].
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="rejectionMemoRecordIndb">The rejection memo record indb.</param>
    /// <param name="isUpdateOperation">if set to <c>true</c> [is update operation].</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// 	True if duplicate rejection memo of the specified rejection memo record]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsDuplicateRejectionMemoNumber(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordIndb, bool isUpdateOperation, PaxInvoice invoice)
    {
      var isExistingRejectionNumber = false;

      if (!isUpdateOperation || (CompareUtil.IsDirty(rejectionMemoRecord.RejectionMemoNumber, rejectionMemoRecordIndb.RejectionMemoNumber)))
      {
        var invoices = InvoiceRepository.GetInvoicesWithRejectionMemoRecord(billingMonth: invoice.BillingMonth, billingYear: invoice.BillingYear, billingPeriod: invoice.BillingPeriod, billedMemberId: invoice.BilledMemberId, billingMemberId: invoice.BillingMemberId);
        foreach (var rminvoice in invoices)
        {
          isExistingRejectionNumber = rminvoice.RejectionMemoRecord.Count(rm => rm.RejectionMemoNumber == rejectionMemoRecord.RejectionMemoNumber) > 0;
          if (isExistingRejectionNumber) break;
        }

      }

      return isExistingRejectionNumber;
    }

    /// <summary>
    /// Validate Rejection memo Breakdown record.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
    /// <param name="rejectionMemoCouponBreakdownRecordInDb">The rejection memo coupon breakdown record in db.</param>
    /// <param name="rejectionInvoice">The rejection invoice.</param>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <returns></returns>
    private string ValidateRejectionMemoCoupon(RMCoupon rejectionMemoCouponBreakdownRecord, RMCoupon rejectionMemoCouponBreakdownRecordInDb, PaxInvoice rejectionInvoice, RejectionMemo rejectionMemoRecord)
    {
      var isUpdateOperation = false;
      var duplicateErrorMessage = string.Empty;
      //Below code to check invalid TaxCode enter by user. if user enter invalid taxcode then system will generate error message.
      //SCP#50425 - Tax Code XT
      foreach (var tax in rejectionMemoCouponBreakdownRecord.TaxBreakdown)
      {
        if (!string.IsNullOrWhiteSpace(tax.TaxCode))
          tax.TaxCode = tax.TaxCode.Trim();
        if (!ReferenceManager.IsValidTaxCode(tax.TaxCode))
        {
          throw new ISBusinessException(ErrorCodes.InvalidTaxCode, tax.TaxCode);
        }
      }
      //End SCP#50425 - Tax Code XT
      if (rejectionMemoCouponBreakdownRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      var expectedAllowedIscAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
      if (ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedIscAmount, Constants.PaxDecimalPlaces) != expectedAllowedIscAmount)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAllowedIscAmount);
      }

      var expectedAcceptedIscAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100, Constants.PaxDecimalPlaces);
      if (ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedIscAmount, Constants.PaxDecimalPlaces) != expectedAcceptedIscAmount)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAcceptedIscAmount);
      }

      var expectedAllowedUatpAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
      if (ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedUatpAmount, Constants.PaxDecimalPlaces) != expectedAllowedUatpAmount)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAllowedUatpAmount);
      }

      var expectedAcceptedUatpAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100, Constants.PaxDecimalPlaces);
      if (ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedUatpAmount, Constants.PaxDecimalPlaces) != expectedAcceptedUatpAmount)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAcceptedUatpAmount);
      }


      // If invoice has ready for billing status, then throw an exception.
      var isInvoiceStatusSubmitted = InvoiceRepository.GetCount(invoice => invoice.Id == rejectionMemoRecord.InvoiceId
        && (invoice.InvoiceStatusId != (int)InvoiceStatusType.Open
        && invoice.InvoiceStatusId != (int)InvoiceStatusType.ValidationError && invoice.InvoiceStatusId != (int)InvoiceStatusType.ReadyForSubmission)) > 0;

      if (isInvoiceStatusSubmitted)
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidOperationOnSubmitStatus);
      }

      // If VAT breakdown exists for difference amount
      if (rejectionMemoCouponBreakdownRecord.VatAmountDifference != 0 && rejectionMemoCouponBreakdownRecord.VatBreakdown.Count <= 0)
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidVatBreakdownRecord);
      }

      // If VAT amount difference is zero then breakdown record should get cleared.
      if (rejectionMemoCouponBreakdownRecord.VatAmountDifference == 0 && rejectionMemoCouponBreakdownRecord.VatBreakdown.Count > 0)
      {
        rejectionMemoCouponBreakdownRecord.VatBreakdown.Clear();
      }

      // Vat breakdown difference validation 
      IsValidVatDifference(rejectionMemoCouponBreakdownRecord, rejectionMemoRecord);

      // Duplicate rejection break down record validation
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketOrFimIssuingAirline, rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline)
        || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber)
        || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketOrFimCouponNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber))
      {
        // Duplicate check - Ticket Issuing Airline, Ticket/Document Number, Coupon No.: As per values provided in the dialog by the user.
        duplicateErrorMessage = GetDuplicateRMCouponCount(rejectionMemoCouponBreakdownRecord, isUpdateOperation, rejectionInvoice, rejectionMemoRecord, duplicateErrorMessage);

        //var duplicateRmCoupon = RejectionMemoCouponBreakdownRepository.GetRMCouponDuplicateCount(rejectionMemoRecord.RejectionStage, rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber, invoice);
        //if (duplicateRmCoupon > 0)
        //{
        //  throw new ISBusinessException(ErrorCodes.DuplicateRejectionMemoCouponBreakdownRecordFound);
        //}
      }

      // From Airport and To Airport should not be same.
      if (!string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon) &&
        !string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon) &&
        rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim().Equals(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()))
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportCombination);
      }

      // Check if passed 'From Airport Code' is a valid airport code 
      // For New coupon Record validation will be done 
      // For Update coupon Record if value FromAirportOfCoupon is updated then only validation will be done
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.FromAirportOfCoupon, rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        if (!string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidFromAirportCode);
        }
      }

      // Check if passed 'To Airport Code' is a valid airport code
      // For New coupon Record validation will be done 
      // For Update coupon Record if value ToAirportOfCoupon is updated then only validation will be done
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.ToAirportOfCoupon, rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        if (!string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
        }
      }

      var transactionType = GetTransactionType(rejectionMemoRecord.RejectionStage);
      // TODO: RejectionMemo1 need to replace with proper transaction type.
      // Validate Acceptable amount difference at RM coupon level if coupon breakdown is mandatory  
      var acceptableDifference = ValidateAcceptableDifferences(rejectionMemoRecord.ReasonCode,
                                    transactionType,
                                    rejectionMemoCouponBreakdownRecord.GrossAmountDifference,
                                    rejectionMemoCouponBreakdownRecord.TaxAmountDifference,
                                    rejectionMemoCouponBreakdownRecord.VatAmountDifference,
                                    rejectionMemoCouponBreakdownRecord.IscDifference,
                                    rejectionMemoCouponBreakdownRecord.UatpDifference,
                                    rejectionMemoCouponBreakdownRecord.HandlingDifference,
                                    rejectionMemoCouponBreakdownRecord.OtherCommissionDifference);
      if (!string.IsNullOrEmpty(acceptableDifference))
      {
        throw new ISBusinessException(ErrorCodes.InvalidAcceptableAmountDifference, acceptableDifference);
      }

      // Validation for Minimum Net Reject Amount.
      if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoCouponBreakdownRecord.NetRejectAmount), TransactionType.Coupon, rejectionInvoice.ListingCurrencyId, rejectionInvoice))
      {
        throw new ISBusinessException(ErrorCodes.InvalidRejectionMemoCouponNetRejectAmount);
      }
      //CMP#459 : Validate Amount at coupon level.
      if (isUpdateOperation)
      {
          var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
          IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
          ValidateAmountsInRMonCouponLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, rejectionMemoCouponBreakdownRecord);
          if (exceptionDetailsList.Where(err => err.ErrorStatus == "2").Count() > 0)
          {
              var errorDesc = string.Empty;
              foreach (var err in exceptionDetailsList.Where(err => err.ErrorStatus == "2"))
              {
                  errorDesc += err.ErrorDescription + " ¥ ";
              }
              throw new ISBusinessException(errorDesc);
          }

      }
      return duplicateErrorMessage;
    }

    /// <summary>
    /// Gets the type of the transaction.
    /// </summary>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <returns></returns>
    /// <remarks>Returns transaction type as Rejection Memo 1 if invalid rejection stage passed.</remarks> 
    private TransactionType GetTransactionType(int rejectionStage)
    {
      TransactionType transactionType;
      switch (rejectionStage)
      {
        case 2:
          transactionType = TransactionType.SamplingFormF;
          break;
        case 3:
          transactionType = TransactionType.SamplingFormXF;
          break;
        default:
          transactionType = TransactionType.SamplingFormF;
          break;
      }
      return transactionType;
    }
    /// <summary>
    /// Determines whether [is valid vat difference] [the specified rejection memo coupon breakdown record].
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <returns></returns>
    private static void IsValidVatDifference(RMCoupon rejectionMemoCouponBreakdownRecord, RejectionMemo rejectionMemoRecord)
    {
      var vatCalculatedAmountSum = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.VatBreakdown.Sum(vat => vat.VatCalculatedAmount), Constants.PaxDecimalPlaces);

      double difference = rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo ? ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.VatAmountAccepted - rejectionMemoCouponBreakdownRecord.VatAmountBilled, Constants.PaxDecimalPlaces)
                            : ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.VatAmountBilled - rejectionMemoCouponBreakdownRecord.VatAmountAccepted, Constants.PaxDecimalPlaces);

      if (vatCalculatedAmountSum != difference)
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidVatDifferenceAmount);
      }
    }

    /// <summary>
    /// Validates the parsed sampling form F.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    public bool ValidateParsedSamplingFormF(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Validates an invoice, when Validate Invoice button pressed
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <returns>
    /// True if successfully validated, false otherwise
    /// </returns>
    public override PaxInvoice ValidateInvoice(string invoiceId, out bool isFutureSubmission, int sessionUserId, string logRefId)
    {
        var log = ReferenceManager.GetDebugLog(DateTime.Now,
                                               "SamplingRejectionManager.ValidateInvoice",
                                               this.ToString(),
                                               BillingCategorys.Passenger.ToString(),
                                               "Step 3 of 12: Id: " + invoiceId + " NonSamplingCreditNoteManager.ValidateInvoice Start",
                                               sessionUserId,
                                               logRefId);

        ReferenceManager.LogDebugData(log);

      isFutureSubmission = false;
      var webValidationErrors = new List<WebValidationError>();

      bool isFutureSubmitted;
      var invoice = base.ValidateInvoice(invoiceId, out isFutureSubmission, sessionUserId, logRefId);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                   "SamplingRejectionManager.ValidateInvoice",
                                   this.ToString(),
                                   BillingCategorys.Passenger.ToString(),
                                   "Step 8 of 12: Id: " + invoiceId + " after base.ValidateInvoice()",
                                   sessionUserId,
                                   logRefId);
      ReferenceManager.LogDebugData(log);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
      webValidationErrors.AddRange(invoice.ValidationErrors);

      if (!IsTransactionExists(invoiceId))
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.TransactionLineItemNotAvailable));
      }

      var rejectionMemoRecordCount = RejectionMemoRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoice.Id);
      // Validate acceptable amount difference for given reason code.
      if (rejectionMemoRecordCount > 0)
      {
        var amountDifference = RejectionMemoRepository.ValidateRejectionMemoAcceptableAmountDifference(invoiceId.ToGuid(), invoice.BillingCode);
        if (!string.IsNullOrEmpty(amountDifference))
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.InvalidAcceptableAmountDifferenceValidate));
        }

        var rejectionMemoNegativeAmtCount = RejectionMemoRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoice.Id && rmRecord.TotalNetRejectAmount < 0);
        if (rejectionMemoNegativeAmtCount > 0)
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.ErrorNegativeRMNetAmount));
        }

        #region CMP-674-Validation of Coupon and AWB Breakdowns in Rejections

        /* CMP#674 validations will be applicable only to the following transactions - PAX Non-Sampling Stage 2 RMs, PAX Non-Sampling Stage 3 RMs
             and PAX Sampling Form X/Fs (Stage 3 RMs) */
        var stage2OrSatge3RMCount =
            RejectionMemoRepository.GetCount(
                rmRecord =>
                rmRecord.InvoiceId == invoice.Id && (rmRecord.RejectionStage == 2 || rmRecord.RejectionStage == 3));

        if (invoice.BillingCode == (int)BillingCode.SamplingFormXF && stage2OrSatge3RMCount > 0)
        {
            List<InvalidRejectionMemoDetails> invalidRejectionMemos =
                  RejectionMemoRepository.IsYourRejectionCouponDropped(invoice.Id);

            foreach (InvalidRejectionMemoDetails invalidRM in invalidRejectionMemos)
            {
                /* Report Error -
                 * Error Code - RejectionMemoCouponMissing = "BPAXNS_10973"
                 * Error Description - Error in RM No. <RM No.>, Batch No. <Batch No.>, Seq. No. <Seq. No.> due to mismatch in coupon <xxx-xxxxxxxxxx-x>. 
                 *                     It was billed <a> time(s) in the rejected RM; and <b> time(s) in this RM.
                 */
                var errorDescription = Messages.ResourceManager.GetString(ErrorCodes.PaxRMCouponMismatchIsWeb);

                errorDescription = string.Format(errorDescription, invalidRM.RejectionMemoNumber,
                                                 invalidRM.BatchNumber, invalidRM.SequenceNumber,
                                                 invalidRM.TicketIssuingAirline, invalidRM.TicketDocOrAwbNumber,
                                                 invalidRM.CouponNumber,
                                                 invalidRM.RejectedRMOccurrence, invalidRM.RejectingRMOccurrence);

                var webValErr = new WebValidationError
                {
                    ErrorCode = ErrorCodes.PaxRMCouponMismatchIsWeb,
                    InvoiceId = invoiceId.ToGuid(),
                    ErrorDescription = errorDescription
                };
                webValidationErrors.Add(webValErr);
            }
        }
          /* CMP#674 - Not applicable - Validation bypassed - Only for Logical Completion
          else
          {
              
          }*/

        #endregion
      }
      //CMP#459: Validate Rejection memo.
      var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
      var rejectionMemoRecords = RejectionMemoRepository.Get(rmRecord => rmRecord.InvoiceId == invoice.Id ).ToList();
      foreach (var rejectionMemoRecord in rejectionMemoRecords)
      {
          IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
          ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord);
          foreach (var isValidationExceptionDetail in exceptionDetailsList)
          {
              var webValErr = new WebValidationError { ErrorCode = isValidationExceptionDetail.ExceptionCode, ErrorDescription = isValidationExceptionDetail.ErrorDescription, InvoiceId = invoiceId.ToGuid() };
              webValidationErrors.Add(webValErr);
          }

          //CMP#641: Time Limit Validation on Third Stage PAX Rejections
          IList<IsValidationExceptionDetail> rmexceptionDetailsList = new List<IsValidationExceptionDetail>();
          if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
          {
              var transctionType = TransactionType.RejectionMemo3;
              if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
              {
                  transctionType = TransactionType.SamplingFormXF;
              }
              ValidatePaxStageThreeRmForTimeLimit(transctionType, invoice.SettlementMethodId, rejectionMemoRecord, invoice, isIsWeb: true, isManageInvoice: true, exceptionDetailsList: rmexceptionDetailsList);
              foreach (var isValidationExceptionDetail in rmexceptionDetailsList)
              {
                  var webValErr = new WebValidationError { ErrorCode = isValidationExceptionDetail.ExceptionCode, ErrorDescription = isValidationExceptionDetail.ErrorDescription, InvoiceId = invoiceId.ToGuid() };
                  webValidationErrors.Add(webValErr);
              }
          }
      }
      var errorMessages = InvoiceRepository.ValidateMemo(invoiceId.ToGuid(), invoice.BillingCode);
      if (!string.IsNullOrEmpty(errorMessages))
      {
        errorMessages = errorMessages.Remove(errorMessages.Length - 1, 1);
        var errorMessage = errorMessages.Split(',');
        foreach (var error in errorMessage)
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), "BPAXNS_10805", error));
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
        // Invoice through the validation, change invoice status to Ready for submission. 
        invoice.InvoiceStatus = isFutureSubmission
                             ? InvoiceStatusType.FutureSubmitted
                             : InvoiceStatusType.ReadyForSubmission;
        // updating validation status to completed
        invoice.ValidationStatus = InvoiceValidationStatus.Completed;
        invoice.ValidationDate = DateTime.UtcNow;
      }

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                 "SamplingRejectionManager.ValidateInvoice",
                                 this.ToString(),
                                 BillingCategorys.Passenger.ToString(),
                                 "Step 9 of 12: Id: " + invoice.Id + " after invoice.ValidationDate = DateTime.UtcNow",
                                 sessionUserId,
                                 logRefId);
      ReferenceManager.LogDebugData(log);

      //invoice.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;

      //InvoiceRepository.Update(invoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      InvoiceRepository.SetInvoiceAndValidationStatus(invoice.Id, invoice.InvoiceStatusId, invoice.ValidationStatusId, isFutureSubmission, invoice.ClearingHouse, invoice.InvoiceTotalRecord.NetBillingAmount, (int)BillingCategoryType.Pax, invoice.ExchangeRate);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                   "SamplingRejectionManager.ValidateInvoice",
                                   this.ToString(),
                                   BillingCategorys.Passenger.ToString(),
                                   "Step 10 of 12: Id: " + invoice.Id + " after InvoiceRepository.SetInvoiceAndValidationStatus()",
                                   sessionUserId,
                                   logRefId);
      ReferenceManager.LogDebugData(log);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                 "SamplingRejectionManager.ValidateInvoice",
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
    /// Validate on update Billing Year/Month/Period (so long as the changed value belongs to the same month as compared to the previously saved value).
    /// </summary>
    /// <param name="invoiceHeader">Invoice to be validated.</param>
    /// <param name="invoiceHeaderInDb">Invoice in Db.</param>
    /// <returns>
    /// Returns true on success, false otherwise.
    /// </returns>
    protected override bool ValidateInvoiceHeader(PaxInvoice invoiceHeader, PaxInvoice invoiceHeaderInDb)
    {
      base.ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb);

      if (invoiceHeaderInDb != null && IsTransactionExists(invoiceHeader.Id.Value()))
      {
        if (invoiceHeaderInDb.BillingMonth != invoiceHeader.BillingMonth || invoiceHeaderInDb.BillingYear != invoiceHeader.BillingYear)
        {
          throw new ISBusinessException(SamplingErrorCodes.InvalidUpdateOperation);
        }
      }

      return true;
    }

    private bool IsTransactionOutSideTimeLimit(RejectionMemo rejectionMemo, PaxInvoice invoice, BillingPeriod? billingPeriod)
    {
      TransactionType transactionType = 0;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.SamplingFormF:
          transactionType = TransactionType.SamplingFormF;
          break;
        case (int)BillingCode.SamplingFormXF:
          transactionType = TransactionType.SamplingFormXF;
          break;
      }

      DateTime effectiveBillingPeriod;
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyMMdd";
     
      //Validate Time Limit
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      return (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
               ? !ReferenceManager.IsTransactionInTimeLimitMethodA(transactionType, invoice.SettlementMethodId, rejectionMemo, invoice, billingPeriod)
               : ((DateTime.TryParseExact(string.Format("{0}{1}{2}", rejectionMemo.YourInvoiceBillingYear.ToString("00"), rejectionMemo.YourInvoiceBillingMonth.ToString("00"), rejectionMemo.YourInvoiceBillingPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out effectiveBillingPeriod)) ? !ReferenceManager.IsTransactionInTimeLimitMethodA2(transactionType, Convert.ToInt32(SMI.Bilateral), new PaxInvoice()
               {
                 BillingYear = invoice.BillingYear,
                 BillingMonth = invoice.BillingMonth,
                 SettlementMethodId = Convert.ToInt32(SMI.Bilateral),
                 ValidationDate = new DateTime(rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingMonth, rejectionMemo.YourInvoiceBillingPeriod)
               }) : false);
    }
  }
}
