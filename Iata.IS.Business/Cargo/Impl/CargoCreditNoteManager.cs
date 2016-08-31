using System;

using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.AdminSystem;
using System.Collections.Generic;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using ReasonCode = Iata.IS.Model.Common.ReasonCode;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Pax.Enums;
using log4net;

using IRejectionMemoRecordRepository = Iata.IS.Data.Cargo.IRejectionMemoRecordRepository;
using RejectionMemo = Iata.IS.Model.Cargo.CargoRejectionMemo;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Cargo.BillingHistory;
using BillingCode = Iata.IS.Model.Pax.Enums.BillingCode;

//InvoiceManager
namespace Iata.IS.Business.Cargo.Impl
{
  public class CargoCreditNoteManager : CargoInvoiceManager, ICargoCreditNoteManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string DuplicateValidationFlag = "DU";

    /// <summary>
    /// CreditMemo Repository, will be injected by the container.
    /// </summary>
    /// <value>The credit memo repository.</value>
    public ICargoCreditMemoRecordRepository CargoCreditMemoRepository { get; set; }

    /// <summary>
    /// CreditMemo Attachment Repository, will be injected by the container.
    /// </summary>
    /// <value>The credit memo attachment repository.</value>
    public ICargoCreditMemoAttachmentRepository CargoCreditMemoAttachmentRepository { get; set; }

    /// <summary>
    /// Credit Memo Vat Repository.
    /// </summary>
    public IRepository<CargoCreditMemoVat> CreditMemoVatRepository { get; set; }

    /// <summary>
    /// Gets or sets the BM awb attachment repository.
    /// </summary>
    /// <value>
    /// The BM awb attachment repository.
    /// </value>
    public ICargoCreditMemoAwbAttachmentRepository CMAwbAttachmentRepository { get; set; }

    public IRepository<CMAwbProrateLadderDetail> CMAwbProrateLadderDetailRepository { get; set; }

    public IRepository<CMAwbVat> CMAwbVatRepository { get; set; }

    public IRepository<CMAwbOtherCharge> CMAwbOtherChargeRepository { get; set; }

    private const string PartShipMentIndicatorP = "P";

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
      return CargoCreditMemoRepository.GetCount(cmRecord => cmRecord.InvoiceId == invoiceGuid) > 0;
    }

    /// <summary>
    /// Gets the billing memo attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    public CargoCreditMemoAttachment GetCreditMemoAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = CargoCreditMemoAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);
      return attachmentRecord;
    }

    /// <summary>
    /// Adds the billing memo attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    public CargoCreditMemoAttachment AddCreditMemoAttachment(CargoCreditMemoAttachment attach)
    {
      CargoCreditMemoAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      attach = CargoCreditMemoAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Determines whether [is duplicate credit memo attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate credit memo attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDuplicateCreditMemoAttachmentFileName(string fileName, Guid creditMemoId)
    {
      return CargoCreditMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == creditMemoId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Add new record in the credit memo list
    /// </summary>
    /// <param name="creditMemo"></param>
    /// <returns></returns>
    public CargoCreditMemo AddCreditMemoRecord(CargoCreditMemo creditMemo)
    {
      creditMemo.ReasonCode = creditMemo.ReasonCode.ToUpper();
      ValidateCreditMemo(creditMemo, null);
      CargoCreditMemoRepository.Add(creditMemo);
      UnitOfWork.CommitDefault();

      // Updates Credit memo invoice total.
      CargoInvoiceRepository.UpdateCargoCMInvoiceTotal(creditMemo.InvoiceId, creditMemo.BillingCode, creditMemo.Id, creditMemo.LastUpdatedBy);

      // Update expiry date for purging.
      //UpdateExpiryDate(creditMemo, creditMemo.InvoiceId, creditMemo.Id);

      return creditMemo;
    }

    /// <summary>
    /// Any Billing memo having the same Credit memo number has been twice billed in the same invoice,
    /// or in a previous invoice to the same airline will be considered as a duplicate.
    /// </summary>
    /// <param name="creditMemoInDb">The credit memo record in db.</param>
    /// <param name="creditMemo">The credit memo record.</param>
    /// <param name="isUpdateOperation">if set to <c>true</c> [is update operation].</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate credit memo] [the specified credit memo record in db]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsDuplicateCreditMemo(CargoCreditMemo creditMemoInDb, CargoCreditMemo creditMemo, bool isUpdateOperation, CargoInvoice invoice)
    {
      if (!isUpdateOperation || (CompareUtil.IsDirty(creditMemoInDb.CreditMemoNumber, creditMemo.CreditMemoNumber)))
      {
        var duplicateCreditMemo = CargoCreditMemoRepository.GetCreditMemoDuplicateCount(creditMemo.CreditMemoNumber, invoice.BilledMemberId, invoice.BillingMemberId, invoice.BillingMonth, invoice.BillingYear, invoice.BillingPeriod);
        if (duplicateCreditMemo > 0)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Validate Credit memo record.
    /// </summary>
    /// <param name="billingMemo">Credit memo record to be validated.</param>
    /// <param name="billingMemoInDb">The credit memo record in db.</param>
    /// <returns></returns>
    private void ValidateCreditMemo(CargoCreditMemo creditMemo, CargoCreditMemo creditMemoInDb)
    {
      var isUpdateOperation = false;

      //If there is record in db then its a update operation
      if (creditMemoInDb != null)
      {
        isUpdateOperation = true;
      }

      //SCP85837: Validate Duplicate Batch Sequence No
      int invalidBatchSequenceNumber = CargoInvoiceRepository.IsValidBatchSequenceNo(creditMemo.InvoiceId, creditMemo.RecordSequenceWithinBatch, creditMemo.BatchSequenceNumber,creditMemo.BillingCode, creditMemo.Id);
     
      if (invalidBatchSequenceNumber != 0)
      {
          // If value == 1, Batch number is repeated between different source codes, else if value == 2, Batch and Sequence number combination is invalid  
          if (invalidBatchSequenceNumber == 1)
              throw new ISBusinessException(CargoErrorCodes.InvalidBatchNo);
          else
              throw new ISBusinessException(CargoErrorCodes.CgoBMInvalidBatchSequenceNo);
      }
     

      // Review: Check whether this validation is required.
      // Check whether net amount is positive for credit memo, else throw an exception. 
      if (creditMemo.NetAmountCredited > 0)
      {
        throw new ISBusinessException(CargoErrorCodes.CgoBMInvalidAmount);
      }


      // Check whether Net credited amount is equal to zero, if yes throw an exception
      if (creditMemo.NetAmountCredited == 0)
      {
        throw new ISBusinessException(CargoErrorCodes.NetCreditedAmountCannotBeZero);
      }

      //SCPID:122022 -Validate sequence and batch number
      if (creditMemo.RecordSequenceWithinBatch <= 0 || creditMemo.BatchSequenceNumber <= 0)
      {
          throw new ISBusinessException(CargoErrorCodes.BatchRecordSequenceNoReq);
      }
      //Review: Check whether this validation makes sense.
      //If no coupon breakdown exists and if Total VAT Amount Billed populated with a non-zero value, then BM-CM VAT Breakdown record needs to be present.
      var couponBreakdownCount = CMAwbRepository.GetCount(cmRecord => cmRecord.CreditMemoId == creditMemo.Id);

      if (couponBreakdownCount == 0 && creditMemo.TotalVatAmountCredited > 0)
      {
        if (creditMemo.VatBreakdown.Count <= 0)
        {
          throw new ISBusinessException(CargoErrorCodes.CgoCreditMemoVatBreakdownRecordNotFound);
        }
      }

      var creditMemoInvoice = CargoInvoiceRepository.Single(id: creditMemo.InvoiceId);

      // Your billing period can not be greater than or equal to the Billing Memo Invoice billing period.
      if (!((creditMemoInvoice.BillingYear > creditMemo.YourInvoiceBillingYear) ||
            ((creditMemoInvoice.BillingYear == creditMemo.YourInvoiceBillingYear) &&
             (creditMemoInvoice.BillingMonth > creditMemo.YourInvoiceBillingMonth)) ||
            ((creditMemoInvoice.BillingYear == creditMemo.YourInvoiceBillingYear) &&
             (creditMemoInvoice.BillingMonth == creditMemo.YourInvoiceBillingMonth) &&
             (creditMemoInvoice.BillingPeriod > creditMemo.YourInvoiceBillingPeriod))))
        throw new ISBusinessException(ErrorCodes.InvalidYourBillingPeriod);

      //// Validates whether reason code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(creditMemoInDb.ReasonCode, creditMemo.ReasonCode))
      {
        if (!ReferenceManager.IsValidReasonCode(creditMemo.ReasonCode, (int) TransactionType.CargoCreditMemo))
        {
          throw new ISBusinessException(CargoErrorCodes.CgoInvalidReasonCode);
        }
      }

      // Validates Duplicate Billing Memo.
      if (IsDuplicateCreditMemo(creditMemoInDb, creditMemo, isUpdateOperation, creditMemoInvoice))
      {
        throw new ISBusinessException(CargoErrorCodes.CgoDuplicateCreditMemoFound);
      }
    }

    /// <summary>
    /// Updates the billing memo attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    public IList<CargoCreditMemoAttachment> UpdateCreditMemoAttachment(IList<Guid> attachments, Guid parentId)
    {
      var creditMemoAttachmentInDb = CargoCreditMemoAttachmentRepository.Get(creditMemoAttachment => attachments.Contains(creditMemoAttachment.Id));
      foreach (var recordAttachment in creditMemoAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        CargoCreditMemoAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return creditMemoAttachmentInDb.ToList();
    }

    /// <summary>
    /// Gets the credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CargoCreditMemoAttachment> GetCreditMemoAttachments(List<Guid> attachmentIds)
    {
      return CargoCreditMemoAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets credit memo record details.
    /// </summary>
    /// <param name="creditMemoRecordId">credit memo record id.</param>
    /// <returns></returns>
    public CargoCreditMemo GetCreditMemoRecordDetails(string creditMemoRecordId)
    {
      var creditMemoRecordGuid = creditMemoRecordId.ToGuid();
      var creditMemoRecords = CreditMemoRepository.Single(creditMemoId: creditMemoRecordGuid);
      return creditMemoRecords;
    }

    /// <summary>
    /// Function to retrieve credit memo. 
    /// </summary>
    /// <param name="creditNoteId">Credit Memo Id.</param>
    /// <returns>creditMemoList</returns>
    public IList<CargoCreditMemo> GetCreditMemoList(string creditNoteId)
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
          var reasonCodeObj = reasonCodesfromDb.Single(rCode => rCode.Code.ToUpper() == record.ReasonCode.ToUpper() && rCode.TransactionTypeId == (int)TransactionType.CargoCreditMemo);

          creditMemoRecord.ReasonCodeDescription = reasonCodeObj != null ? reasonCodeObj.Description : string.Empty;
        }
      }
        creditMemoList.OrderBy(cm => cm.BatchSequenceNumber);
      return creditMemoList.ToList();
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
    
      if (creditMemo == null) return false;
      CreditMemoRepository.Delete(creditMemo);

      UnitOfWork.CommitDefault();

      CargoInvoiceRepository.UpdateCargoCMInvoiceTotal(creditMemo.InvoiceId, creditMemo.BillingCode, creditMemo.Id, creditMemo.LastUpdatedBy);

      return true;
    }

    /// <summary>
    /// Function to update credit memo
    /// </summary>
    /// <param name="creditMemoRecord">credit memo to be updated.</param>
    /// <returns></returns>
    public CargoCreditMemo UpdateCreditMemoRecord(CargoCreditMemo creditMemoRecord)
    {
      // LoadStrategy call
      var creditMemoRecordInDb = CreditMemoRepository.Single(creditMemoRecord.Id);
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

      var creditMemoAttachmentInDb = CargoCreditMemoAttachmentRepository.Get(cmAttachment => attachmentIdList.Contains(cmAttachment.Id));
      foreach (var recordAttachment in creditMemoAttachmentInDb)
      {
        if (IsDuplicateCreditMemoAttachmentFileName(recordAttachment.OriginalFileName, creditMemoRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = creditMemoRecord.Id;
        CargoCreditMemoAttachmentRepository.Update(recordAttachment);
      }

      foreach (var couponRecordAttachment in listToDeleteAttachment)
      {
        CargoCreditMemoAttachmentRepository.Delete(couponRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      CargoInvoiceRepository.UpdateCargoCMInvoiceTotal(creditMemoRecordInDb.InvoiceId, creditMemoRecordInDb.BillingCode, creditMemoRecordInDb.Id, creditMemoRecordInDb.LastUpdatedBy);

      //UpdateExpiryDate(creditMemoRecord, creditMemoRecord.InvoiceId, creditMemoRecord.Id);

      return updatedCreditMemo;
    }

    /// <summary>
    /// Adds the CM awb record.
    /// </summary>
    /// <param name="cmAwb">The cm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    public CMAirWayBill AddCMAwbRecord(CMAirWayBill cmAwb, string invoiceId, out string duplicateErrorMessage)
    {
      if (!string.IsNullOrEmpty(cmAwb.ReasonCode)) cmAwb.ReasonCode = cmAwb.ReasonCode.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CurrencyAdjustmentIndicator)) cmAwb.CurrencyAdjustmentIndicator = cmAwb.CurrencyAdjustmentIndicator.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.ConsignmentOriginId)) cmAwb.ConsignmentOriginId = cmAwb.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.ConsignmentDestinationId)) cmAwb.ConsignmentDestinationId = cmAwb.ConsignmentDestinationId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CarriageFromId)) cmAwb.CarriageFromId = cmAwb.CarriageFromId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CarriageToId)) cmAwb.CarriageToId = cmAwb.CarriageToId.ToUpper();

      duplicateErrorMessage = ValidateCMAwb(cmAwb, null, invoiceId);
      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        cmAwb.ISValidationFlag = DuplicateValidationFlag;
      }
      cmAwb.BdSerialNumber = GetCMAwbSerialNo(cmAwb.CreditMemoId);
      
      CMAwbRepository.Add(cmAwb);
      UnitOfWork.CommitDefault();
      // Updates credit memo invoice total.
      UpdateCreditMemoInvoiceTotal(cmAwb);

      // Update expiry date of CM for purging.
      //UpdateExpiryDate(cmAwb.CreditMemoRecord, invoiceId.ToGuid(), cmAwb.CreditMemoId);

      return cmAwb;
    }

    /// <summary>
    /// Validates the CM awb.
    /// </summary>
    /// <param name="cmAwb">The cm awb.</param>
    /// <param name="cmAwbRecordInDb">The cm awb record in db.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    private string ValidateCMAwb(CMAirWayBill cmAwb, CMAirWayBill cmAwbRecordInDb, string invoiceId)
    {

      var isUpdateOperation = false;
      var duplicateErrorMessage = string.Empty;
      //Check whether it's a update operation.
      if (cmAwbRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      // If any of the header field values are given for prorate ladder, then atleast 1 detail record should be present.
      if (cmAwb.TotalProrateAmount != null || !string.IsNullOrEmpty(cmAwb.ProrateCalCurrencyId) || cmAwb.CMAwbProrateLadder.Count > 0)
      {
        if (cmAwb.CMAwbProrateLadder.Count == 0 || cmAwb.TotalProrateAmount == null || string.IsNullOrEmpty(cmAwb.ProrateCalCurrencyId))
        {
          throw new ISBusinessException(CargoErrorCodes.ProrateLadderRequired);
        }
      }

      // Either none or both Billed Weight and KG/LB Indicator should have values.
      if ((string.IsNullOrEmpty(cmAwb.KgLbIndicator) && cmAwb.BilledWeight.HasValue && cmAwb.BilledWeight.Value != 0) || (!string.IsNullOrEmpty(cmAwb.KgLbIndicator) && (!cmAwb.BilledWeight.HasValue || (cmAwb.BilledWeight.HasValue && cmAwb.BilledWeight.Value == 0))))
      {
        throw new ISBusinessException(CargoErrorCodes.InvalidBilledWeightKGLBIndicator);
      }
      // Vat Amount Billed is non-zero then Tax Breakdown record needs to be provided
      if (cmAwb.CreditedVatAmount > 0 && cmAwb.CMAwbVatBreakdown.Count <= 0)
      {
        double? ocVatAmount = cmAwb.CMAwbOtherCharges.Sum(oc => oc.OtherChargeVatCalculatedAmount);
        double otherChargeVatCalculatedAmount = 0;
        if (ocVatAmount != null)
        {
          otherChargeVatCalculatedAmount = ConvertUtil.Round(ocVatAmount.Value, Constants.CgoDecimalPlaces);
        }
        if ((cmAwb.CreditedVatAmount - otherChargeVatCalculatedAmount) != 0)
          throw new ISBusinessException(CargoErrorCodes.CgoBMAwbVatBreakdownRecordNotFound);
      }

      // get the parent Billing Memo to get the source code.
      var creditMemo = CreditMemoRepository.Single(cmAwb.CreditMemoId);

      var transactionTypeId = creditMemo.ReasonCode == "6A"
                                   ? (int)TransactionType.CargoBillingMemoDueToAuthorityToBill
                                   : creditMemo.ReasonCode == "6B" ? (int)TransactionType.CargoBillingMemoDueToExpiry : (int)TransactionType.CargoBillingMemo;

      // Check if passed 'Currency Adjustment Indicator' is a valid currency code
      // For New coupon Record validation will be done 
      // For Update coupon Record if value CurrencyAdjustmentIndicator is updated then only validation will be done
      /* Logic Applied : 
       * Check for Currency Adjustment indicator only when
       * - entered value is not null and
       * - when value has changed during update operation
       */
      if ((!string.IsNullOrEmpty(cmAwb.CurrencyAdjustmentIndicator)) &&
          (isUpdateOperation ? CompareUtil.IsDirty(cmAwbRecordInDb.CurrencyAdjustmentIndicator, cmAwb.CurrencyAdjustmentIndicator) : true))
      {
        if (!ReferenceManager.IsValidCurrencyCode(cmAwb.CurrencyAdjustmentIndicator))
        {
          throw new ISBusinessException(ErrorCodes.InvalidCurrencyAdjustmentInd);
        }
      }

      // Validate ConsignmentOriginId 
      if (!isUpdateOperation || CompareUtil.IsDirty(cmAwbRecordInDb.ConsignmentOriginId, cmAwb.ConsignmentOriginId))
      {
        if (!string.IsNullOrEmpty(cmAwb.ConsignmentOriginId)
          && !ReferenceManager.IsValidAirportCode(cmAwb.ConsignmentOriginId))
        {
          throw new ISBusinessException(CargoErrorCodes.InvalidConsignmentOriginCode);
        }
      }

      // Validate ConsignmentDestinationId 
      if (!isUpdateOperation || CompareUtil.IsDirty(cmAwbRecordInDb.ConsignmentDestinationId, cmAwb.ConsignmentDestinationId))
      {
        if (!string.IsNullOrEmpty(cmAwb.ConsignmentDestinationId)
          && !ReferenceManager.IsValidAirportCode(cmAwb.ConsignmentDestinationId))
        {
          throw new ISBusinessException(CargoErrorCodes.InvalidConsignmentDestinationCode);
        }
      }

      // ConsignmentOriginId and ConsignmentDestinationId should not be same.
      if (!string.IsNullOrEmpty(cmAwb.ConsignmentOriginId) &&
        !string.IsNullOrEmpty(cmAwb.ConsignmentDestinationId) &&
        cmAwb.ConsignmentOriginId.Trim().Equals(cmAwb.ConsignmentDestinationId.Trim()))
      {
        throw new ISBusinessException(CargoErrorCodes.InvalidOriginDestinationCombination);
      }

      // SCP107981: to/point of transfer population error
      // From and To Point of transfer cannot be same for CM AWB Prepaid.
      if (cmAwb.AwbBillingCode == (int)Model.Cargo.Enums.BillingCode.AWBPrepaid)
      {
        if (!string.IsNullOrEmpty(cmAwb.CarriageFromId) && !string.IsNullOrEmpty(cmAwb.CarriageToId) &&
            cmAwb.CarriageFromId.Trim().Equals(cmAwb.CarriageToId.Trim()))
        {
          throw new ISBusinessException(CargoErrorCodes.InvalidCarriageCombination);
        }
      }

      if (!isUpdateOperation || CompareUtil.IsDirty(cmAwbRecordInDb.CarriageFromId, cmAwb.CarriageFromId))
      {
        if (!string.IsNullOrEmpty(cmAwb.CarriageFromId) && !ReferenceManager.IsValidAirportCode(cmAwb.CarriageFromId))
        {
          throw new ISBusinessException(CargoErrorCodes.InvalidFromCarriageCode);
        }
      }

      if (!isUpdateOperation || CompareUtil.IsDirty(cmAwbRecordInDb.CarriageToId, cmAwb.CarriageToId))
      {
        if (!string.IsNullOrEmpty(cmAwb.CarriageToId) && !ReferenceManager.IsValidAirportCode(cmAwb.CarriageToId))
        {
          throw new ISBusinessException(CargoErrorCodes.InvalidToCarriageCode);
        }
      }

      if (!isUpdateOperation ||
       (CompareUtil.IsDirty(cmAwbRecordInDb.AwbSerialNumber, cmAwb.AwbSerialNumber) ||
       CompareUtil.IsDirty(cmAwbRecordInDb.AwbIssueingAirline, cmAwb.AwbIssueingAirline)))
      {
        // Duplicate check - Ticket Issuing Airline, Ticket/Document Number, Coupon No.: As per values provided in the dialog by the user.
        var invoiceGuid = invoiceId.ToGuid();
        var invoice = CargoInvoiceRepository.Single(id: invoiceGuid);
        DateTime billingDate;
        var billingYearToCompare = 0;
        var billingMonthToCompare = 0;

        if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
        {
          var billingDateToCompare = billingDate.AddMonths(-12);
          billingYearToCompare = billingDateToCompare.Year;
          billingMonthToCompare = billingDateToCompare.Month;
        }

        long duplicateAwbCount = 0;
        if (!(cmAwb.PartShipmentIndicator == PartShipMentIndicatorP || cmAwb.CcaIndicator))
          duplicateAwbCount = CMAwbRepository.GetCMAwbRecordDuplicateCount(cmAwb.AwbSerialNumber,
                                                                            cmAwb.AwbIssueingAirline,
                                                                            invoice.BillingMemberId,
                                                                            invoice.BilledMemberId,
                                                                            billingYearToCompare,
                                                                            billingMonthToCompare, cmAwb.AwbBillingCode);

        //if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0))
        if (duplicateAwbCount > 0)
        {
          duplicateErrorMessage = string.Format(Messages.BMAwbDuplicateMessage, duplicateAwbCount);
        }
      }

      return duplicateErrorMessage;
    }

    /// <summary>
    ///  Updates expiry date of Credit memo for purging.
    /// </summary>
    /// <param name="creditMemo"></param>
    /// <param name="invoiceId"></param>
    /// <param name="creditMemoId"></param>
    //private void UpdateExpiryDate(CargoCreditMemo creditMemo, Guid invoiceId, Guid creditMemoId)
    //{
    //  CargoInvoice creditMemoInvoice;
    //  if (creditMemo == null || creditMemo.Invoice == null)
    //    creditMemoInvoice = CargoInvoiceRepository.Single(id: invoiceId);
    //  else
    //  {
    //    creditMemoInvoice = creditMemo.Invoice;
    //  }

    //  // Get expiry period.
    //  DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.CargoRejectionMemoStage1, creditMemoInvoice, BillingCategoryType.Cgo, Constants.SamplingIndicatorNo, null);

    //  // Update it in database.
    //  CargoInvoiceRepository.UpdateExpiryDatePeriod(creditMemoId, (int)TransactionType.CargoCreditMemo, expiryPeriod);
    //}

    private int GetCMAwbSerialNo(Guid creditMemoRecordId)
    {
      var serialNo = 1;
      var creditMemoAwbRecord = CMAwbRepository.Get(cmAwb => cmAwb.CreditMemoId == creditMemoRecordId).OrderByDescending(cmAwbRecord => cmAwbRecord.BdSerialNumber).FirstOrDefault();
      if (creditMemoAwbRecord != null)
      {
        serialNo = creditMemoAwbRecord.BdSerialNumber + 1;
      }

      return serialNo;
    }

    private void UpdateCreditMemoInvoiceTotal(CMAirWayBill awbRecord, bool isAwbDelete = false)
    {
      // Call replaced by Load strategy
      var creditMemoRecord = CreditMemoRepository.Single(awbRecord.CreditMemoId);
      CargoInvoiceRepository.UpdateCargoCMInvoiceTotal(creditMemoRecord.InvoiceId, creditMemoRecord.BillingCode, creditMemoRecord.Id, creditMemoRecord.LastUpdatedBy, isAwbDelete);
    }

    /// <summary>
    /// Updates the CM awb attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    public IList<CMAwbAttachment> UpdateCMAwbAttachment(IList<Guid> attachments, Guid parentId)
    {
      var cmAwbAttachmentInDb = CMAwbAttachmentRepository.Get(creditMemoAttachment => attachments.Contains(creditMemoAttachment.Id));
      foreach (var recordAttachment in cmAwbAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        CMAwbAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return cmAwbAttachmentInDb.ToList();
    }

    /// <summary>
    /// Gets the CM awb attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CMAwbAttachment> GetCMAwbAttachments(List<Guid> attachmentIds)
    {
      return CMAwbAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Adds the CM awb attachment.
    /// </summary>
    /// <param name="attach">The attach.</param>
    /// <returns></returns>
    public CMAwbAttachment AddCMAwbAttachment(CMAwbAttachment attach)
    {
      CMAwbAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      attach = CMAwbAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Gets Credit memo awb record details.
    /// </summary>
    /// <param name="cmAwbRecordId">The cm awb record id.</param>
    /// <returns></returns>
    public CMAirWayBill GetCreditMemoAwbRecordDetails(string cmAwbRecordId)
    {
      var cmAwbRecordGuid = cmAwbRecordId.ToGuid();
      var cmAwbRecords = CMAwbRepository.Single(cmAwbRecordGuid);
      return cmAwbRecords;
    }

    /// <summary>
    /// Gets the credit memo coupon breakdown count.
    /// </summary>
    /// <param name="memoRecordId">The memo record id.</param>
    /// <returns></returns>
    public long GetCreditMemoAwbBreakdownCount(string memoRecordId)
    {
      var memoRecordGuid = memoRecordId.ToGuid();
      return CMAwbRepository.GetCount(cmCb => cmCb.CreditMemoId == memoRecordGuid);
    }

    /// <summary>
    /// Gets CM awb prorate ladder detail list.
    /// </summary>
    /// <param name="cmAwbProrateLadderDetailId">The cm awb prorate ladder detail id.</param>
    /// <returns></returns>
    public IList<CMAwbProrateLadderDetail> GetCMAwbProrateLadderDetailList(Guid cmAwbProrateLadderDetailId)
    {
      var detailList = CMAwbProrateLadderDetailRepository.Get(p => p.ParentId == cmAwbProrateLadderDetailId).ToList();
      return detailList;
    }

    /// <summary>
    /// Adds the awb prorate ladder detail.
    /// </summary>
    /// <param name="cmAwbProrateLadderDetail">The cm awb prorate ladder detail.</param>
    /// <returns></returns>
    public CMAwbProrateLadderDetail AddCreditMemoAwbProrateLadderDetail(CMAwbProrateLadderDetail cmAwbProrateLadderDetail)
    {
      if (!string.IsNullOrEmpty(cmAwbProrateLadderDetail.ToSector)) cmAwbProrateLadderDetail.ToSector = cmAwbProrateLadderDetail.ToSector.ToUpper();
      if (!string.IsNullOrEmpty(cmAwbProrateLadderDetail.FromSector)) cmAwbProrateLadderDetail.FromSector = cmAwbProrateLadderDetail.FromSector.ToUpper();
      ValidateCMAwbProrateLadderDetail(cmAwbProrateLadderDetail);
      CMAwbProrateLadderDetailRepository.Add(cmAwbProrateLadderDetail);
      UnitOfWork.CommitDefault();
      return cmAwbProrateLadderDetail;
    }

    private void ValidateCMAwbProrateLadderDetail(CMAwbProrateLadderDetail cmAwbProrateLadderDetail)
    {
      // From Airport and To Airport should not be same.
      if (!string.IsNullOrEmpty(cmAwbProrateLadderDetail.FromSector) && !string.IsNullOrEmpty(cmAwbProrateLadderDetail.ToSector) &&
          cmAwbProrateLadderDetail.FromSector.Trim().Equals(cmAwbProrateLadderDetail.ToSector.Trim()))
      {
        throw new ISBusinessException(CargoErrorCodes.InvalidSectorCombination);
      }


      if (!string.IsNullOrEmpty(cmAwbProrateLadderDetail.FromSector) && !ReferenceManager.IsValidAirportCode(cmAwbProrateLadderDetail.FromSector))
      {
        throw new ISBusinessException(CargoErrorCodes.InvalidFromSectorCode);
      }


      if (!string.IsNullOrEmpty(cmAwbProrateLadderDetail.ToSector) && !ReferenceManager.IsValidAirportCode(cmAwbProrateLadderDetail.ToSector))
      {
        throw new ISBusinessException(CargoErrorCodes.InvalidToSectorCode);
      }
    }

    /// <summary>
    /// Updates CM awb record.
    /// </summary>
    /// <param name="cmAwb">The cm awb.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns></returns>
    public CMAirWayBill UpdateCMAwbRecord(CMAirWayBill cmAwb, string invoiceId, out string duplicateErrorMessage)
    {
      var creditMemoRecordInDb = CreditMemoRepository.Single(cm => cm.Id == cmAwb.Id);

      // Call replaced by load strategy
      var cmAwbRecordInDb = CMAwbRepository.Single(cmAwb.Id);
      if (!string.IsNullOrEmpty(cmAwb.ReasonCode)) cmAwb.ReasonCode = cmAwb.ReasonCode.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CurrencyAdjustmentIndicator)) cmAwb.CurrencyAdjustmentIndicator = cmAwb.CurrencyAdjustmentIndicator.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.ConsignmentOriginId)) cmAwb.ConsignmentOriginId = cmAwb.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.ConsignmentDestinationId)) cmAwb.ConsignmentDestinationId = cmAwb.ConsignmentDestinationId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CarriageFromId)) cmAwb.CarriageFromId = cmAwb.CarriageFromId.ToUpper();
      if (!string.IsNullOrEmpty(cmAwb.CarriageToId)) cmAwb.CarriageToId = cmAwb.CarriageToId.ToUpper();

      duplicateErrorMessage = ValidateCMAwb(cmAwb, cmAwbRecordInDb, invoiceId);

      var updatedCMAwb = CMAwbRepository.Update(cmAwb);

      var listToDelete = cmAwbRecordInDb.CMAwbOtherCharges.Where(awbOtherCharge => cmAwb.CMAwbOtherCharges.Count(otherCharge => otherCharge.Id == awbOtherCharge.Id) == 0).ToList();

      foreach (var otherCharge in cmAwb.CMAwbOtherCharges.Where(otherCharge => otherCharge.Id.CompareTo(new Guid()) == 0))
      {
        CMAwbOtherChargeRepository.Add(otherCharge);
      }

      foreach (var otherCharge in listToDelete)
      {
        CMAwbOtherChargeRepository.Delete(otherCharge);
      }
      //Update vat list along with Billing Memo Record
      var listToDeleteVat = cmAwbRecordInDb.CMAwbVatBreakdown.Where(vat => cmAwb.CMAwbVatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in cmAwb.CMAwbVatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        CMAwbVatRepository.Add(vat);
      }

      foreach (var cmAwbVat in listToDeleteVat)
      {
        CMAwbVatRepository.Delete(cmAwbVat);
      }

      // Update prorate ladder.
      var listToDeleteProrateLadder = cmAwbRecordInDb.CMAwbProrateLadder.Where(prorateLadderDetailRecord => cmAwb.CMAwbProrateLadder.Count(prorateLadderDetail => prorateLadderDetail.Id == prorateLadderDetailRecord.Id) == 0).ToList();

      foreach (var prorateLadderDetail in cmAwb.CMAwbProrateLadder.Where(prorateLadderDetail => prorateLadderDetail.Id.CompareTo(new Guid()) == 0))
      {
        CMAwbProrateLadderDetailRepository.Add(prorateLadderDetail);
      }

      foreach (var prorateLadderDetail in listToDeleteProrateLadder)
      {
        CMAwbProrateLadderDetailRepository.Delete(prorateLadderDetail);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = cmAwbRecordInDb.Attachments.Where(attachment => cmAwb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in cmAwb.Attachments
                              where cmAwbRecordInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var creditMemoAttachmentInDb = CMAwbAttachmentRepository.Get(billingMemoAttachment => attachmentIdList.Contains(billingMemoAttachment.Id));
      foreach (var recordAttachment in creditMemoAttachmentInDb)
      {
        if (IsDuplicateCMAwbAttachmentFileName(recordAttachment.OriginalFileName, cmAwb.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }
        recordAttachment.ParentId = cmAwb.Id;
        CMAwbAttachmentRepository.Update(recordAttachment);
      }

      foreach (var cmAwbAttachment in listToDeleteAttachment)
      {
        CMAwbAttachmentRepository.Delete(cmAwbAttachment);
      }

      UnitOfWork.CommitDefault();

      // Updates credit memo invoice total.
      UpdateCreditMemoInvoiceTotal(cmAwb);
      cmAwb.CreditMemoRecord = creditMemoRecordInDb;

      // Update credit memo expiry date for purging.
      //UpdateExpiryDate(cmAwb.CreditMemoRecord, invoiceId.ToGuid(), cmAwb.CreditMemoId);

      return updatedCMAwb;
    }


    /// <summary>
    /// Determines whether [is duplicate CM awb attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="bmAwbId"></param>
    /// <returns>
    ///   <c>true</c> if [is duplicate CM awb attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDuplicateCMAwbAttachmentFileName(string fileName, Guid bmAwbId)
    {
      return CMAwbAttachmentRepository.GetCount(attachment => attachment.ParentId == bmAwbId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Gets the CM awb list.
    /// </summary>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <returns></returns>
    public IList<CMAirWayBill> GetCMAwbList(string creditMemoId)
    {
      var creditMemoGuid = creditMemoId.ToGuid();
      var cmAwblist = CMAwbRepository.Get(awb => awb.CreditMemoId == creditMemoGuid).ToList();
      return cmAwblist;
    }

    /// <summary>
    /// Deletes credit memo awb record.
    /// </summary>
    /// <param name="awbRecordId">The awb record id.</param>
    /// <param name="creditMemoId">The credit memo id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public bool DeleteCreditMemoAwbRecord(string awbRecordId, out Guid creditMemoId, out Guid invoiceId)
    {
      var cmAwbRecordId = awbRecordId.ToGuid();
      //LoadStrategy call
      var awbRecord = CMAwbRepository.Single(cmAwbRecordId);
      if (awbRecord == null)
      {
        creditMemoId = new Guid();
        invoiceId = new Guid();
        return false;
      }

      creditMemoId = awbRecord.CreditMemoId;
      invoiceId = awbRecord.CreditMemoRecord.InvoiceId;

      // Delete the CM AWB, re-sequence subsequent serial numbers and update invoice total.
      CargoInvoiceRepository.DeleteCreditMemoAwb(cmAwbRecordId);

      return true;
    }

    /// <summary>
    /// Gets CM awb attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    public CMAwbAttachment GetCMAwbAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = CMAwbAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);
      return attachmentRecord;
    }

    /// <summary>
    /// Validates an invoice, when Validate Invoice button pressed
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <returns>
    /// True if successfully validated, false otherwise
    /// </returns>
    public override CargoInvoice ValidateInvoice(string invoiceId)
    {

      var webValidationErrors = new List<WebValidationError>();

      var invoice = base.ValidateInvoice(invoiceId);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
      webValidationErrors.AddRange(invoice.ValidationErrors);

      var creditMemoRecordCount = CreditMemoRepository.GetCount(creditMemoRecord => creditMemoRecord.InvoiceId == invoice.Id);

      if (creditMemoRecordCount <= 0)
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.TransactionLineItemNotAvailable));
      }

      var errorMessages = CargoInvoiceRepository.ValidateMemo(invoiceId.ToGuid());
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
        // Invoice through the validation, change invoice status to Ready for submission. 
        invoice.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;
        // updating validation status to completed
        invoice.ValidationStatus = InvoiceValidationStatus.Completed;
        invoice.ValidationDate = DateTime.UtcNow;
      }

      // Update the invoice.
      //InvoiceRepository.Update(invoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      InvoiceRepository.SetInvoiceAndValidationStatus(invoice.Id, invoice.InvoiceStatusId, invoice.ValidationStatusId, false, invoice.ClearingHouse, invoice.CGOInvoiceTotal.NetBillingAmount, (int)BillingCategoryType.Cgo, invoice.ExchangeRate);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

      //SCP325375: File Loading & Web Response Stats ManageInvoice
      //UnitOfWork.CommitDefault();

      return invoice;
    }
  }
}