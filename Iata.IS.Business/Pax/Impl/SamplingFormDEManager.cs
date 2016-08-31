using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using System.Linq;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Enums;
using Iata.IS.Core.DI;
using Iata.IS.Model;

namespace Iata.IS.Business.Pax.Impl
{
  public class SamplingFormDEManager : InvoiceManager, ISamplingFormDEManager, IValidationSFormDEManager
  {
    private const string AgreementIndicatorValidatedI = "I";
    private const string ValidatedPmiT = "T";
    private const string Delimeter = ",";
    private const string AmountGross = " Gross";
    private const string IscPercentage = " ISC Percentage";
    private const string UatpPercentage = " UATP Percentage ";
    private const string AmountTax = " Tax ";
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string HandlingFee = " Handling Fee";

    /// <summary>
    /// Sampling formD repository.
    /// </summary>
    public ISamplingFormDRepository SamplingFormDRepository { get; set; }

    /// <summary>
    /// Sampling formE Repository.
    /// </summary>
    public ISamplingFormERepository SamplingFormERepository { get; set; }

    /// <summary>
    /// ProvisionalInvoiceRecordDetail Repository.
    /// </summary>
    public IProvisionalInvoiceRepository ProvisionalInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the formD Attachment repository.
    /// </summary>
    public ISamplingFormDAttachmentRepository SamplingFormDAttachmentRepository { get; set; }

    public IRepository<SamplingFormDTax> SamplingFormDTaxRepository { get; set; }

    public IRepository<SamplingFormDVat> SamplingFormDVatRepository { get; set; }

    public IRepository<SamplingFormCRecord> SamplingFormCRecordRepository { get; set; }

    /// <summary>
    /// Gets or sets the sampling form C repository.
    /// </summary>
    /// <value>The sampling form C repository.</value>
    // public ISamplingFormCRepository SamplingFormCRepository { get; set; }

    public IRepository<BvcMatrix> BvcMatrixRepository { get; set; }

    public IRepository<PrimeCoupon> PrimeCouponRecordRepository { get; set; }

    /// <summary>
    /// SamplingFormEDetailVat Repository.
    /// </summary>
    public ISamplingFormEVatRepository SamplingFormEVatRepository { get; set; }

    public SamplingFormDRecord AddSamplingFormD(SamplingFormDRecord samplingFormD, out string duplicateErrorMessage)
    {
        //Start fix : SCPID:26171 - Lower case tax codes
        //Author: Sachin Pharande
        //Date: 25-06-2012
        //Purpose: Sometimes TaxCode was saved in lower case in database, hence it is converted to uppercase before insert/updated in database.
        foreach (var tax in samplingFormD.TaxBreakdown)
        {
            tax.TaxCode = tax.TaxCode.ToUpper();
        }
        //End fix : SCPID:26171 - Lower case tax codes

      var samplingFormDE = InvoiceRepository.Single(id: samplingFormD.InvoiceId);

      duplicateErrorMessage = ValidateFormDRecord(samplingFormD, null, samplingFormDE);
      //if (!string.IsNullOrEmpty(duplicateErrorMessage))
      //{
      //  samplingFormD.ISValidationFlag = duplicateErrorMessage;
      //}

      /* Commented below code to not set Expiry date at this moment For PAX Trans only */
      // Update expiry date for purging for Sampling Form D and previous linked transactions.
      // DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.SamplingFormF, samplingFormDE, BillingCategoryType.Pax, Constants.SamplingIndicatorYes, null);
      //samplingFormD.ExpiryDatePeriod = expiryPeriod;


      SamplingFormDRepository.Add(samplingFormD);

      // Form D/E can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (samplingFormDE.InvoiceStatus != InvoiceStatusType.Open)
      {
        samplingFormDE.InvoiceStatus = InvoiceStatusType.Open;
        samplingFormDE.ValidationStatus = InvoiceValidationStatus.Pending;
        samplingFormDE.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(samplingFormDE);
      }

      UnitOfWork.CommitDefault();

      // Update invoice total
      SamplingFormDRepository.UpdateFormDInvoiceTotal(samplingFormD.InvoiceId, samplingFormD.SourceCodeId);

      return samplingFormD;
    }

    public SamplingFormDRecord UpdateSamplingFormD(SamplingFormDRecord samplingFormD, out string duplicateErrorMessage)
    {
        //Start fix : SCPID:26171 - Lower case tax codes
        //Author: Sachin Pharande
        //Date: 25-06-2012
        //Purpose: Sometimes TaxCode was saved in lower case in database, hence it is converted to uppercase before insert/updated in database.
        foreach (var tax in samplingFormD.TaxBreakdown)
        {
            tax.TaxCode = tax.TaxCode.ToUpper();
        }
        //End fix : SCPID:26171 - Lower case tax codes

      //Replaced with LoadStrategy call
      var samplingFormDInDb = SamplingFormDRepository.Single(samplingFormD.Id);
      var invoice = InvoiceRepository.Single(id: samplingFormD.InvoiceId);

      duplicateErrorMessage = ValidateFormDRecord(samplingFormD, samplingFormDInDb, invoice);

      // Form D can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (invoice.InvoiceStatus != InvoiceStatusType.Open)
      {
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.ValidationStatus = InvoiceValidationStatus.Pending;
        invoice.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(invoice);
      }

      //   DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.SamplingFormF, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorYes, null);
      // samplingFormD.ExpiryDatePeriod = expiryPeriod;

      var updatedFormD = SamplingFormDRepository.Update(samplingFormD);

      //Changes to update tax breakdown records
      var listToDelete = samplingFormDInDb.TaxBreakdown.Where(tax => samplingFormD.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0).ToList();

      foreach (var tax in samplingFormD.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0))
      {
        SamplingFormDTaxRepository.Add(tax);
      }

      foreach (var couponRecordTax in listToDelete)
      {
        SamplingFormDTaxRepository.Delete(couponRecordTax);
      }


      // Changes to update vat breakdown records
      var listToDeleteVat = samplingFormDInDb.VatBreakdown.Where(vat => samplingFormD.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in samplingFormD.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        SamplingFormDVatRepository.Add(vat);
      }

      foreach (var couponRecordVat in listToDeleteVat)
      {
        SamplingFormDVatRepository.Delete(couponRecordVat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = samplingFormDInDb.Attachments.Where(attachment => samplingFormD.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in samplingFormD.Attachments
                              where samplingFormDInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var samplingFormDAttachmentInDb = SamplingFormDAttachmentRepository.Get(record => attachmentIdList.Contains(record.Id));
      foreach (var recordAttachment in samplingFormDAttachmentInDb)
      {
        if (IsDuplicateSamplingFormDAttachmentFileName(recordAttachment.OriginalFileName, samplingFormD.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }
        recordAttachment.ParentId = samplingFormD.Id;
        SamplingFormDAttachmentRepository.Update(recordAttachment);
      }

      foreach (var formDAttachment in listToDeleteAttachment)
      {
        SamplingFormDAttachmentRepository.Delete(formDAttachment);
      }

      UnitOfWork.CommitDefault();

      // Update expiry period of previous linked transactions.
      //  InvoiceRepository.UpdateExpiryDatePeriod(samplingFormD.Id, (int)TransactionType.SamplingFormD, expiryPeriod);

      // Update invoice total.
      SamplingFormDRepository.UpdateFormDInvoiceTotal(samplingFormD.InvoiceId, samplingFormD.SourceCodeId);
      return updatedFormD;
    }

    public SamplingFormDRecord GetSamplingFormD(string samplingFormDId)
    {
      var samplingFormDGuid = samplingFormDId.ToGuid();
      //Replaced with LoadStratgy call
      var samplingFormD = SamplingFormDRepository.Single(samplingFormDGuid);

      return samplingFormD;
    }

    public bool DeleteSamplingFormD(string samplingFormDId)
    {
      var samplingFormDGuid = samplingFormDId.ToGuid();
      //Replaced with LoadStratgy call
      var samplingFormD = SamplingFormDRepository.Single(samplingFormDGuid);

      if (samplingFormD == null)
      {
        return false;
      }

      SamplingFormDRepository.Delete(samplingFormD);
      UnitOfWork.CommitDefault();

      // Update invoice total.
      SamplingFormDRepository.UpdateFormDInvoiceTotal(samplingFormD.InvoiceId, samplingFormD.SourceCodeId);
      return true;
    }

    public IQueryable<SamplingFormDRecord> GetSamplingFormDList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var samplingFormDList = SamplingFormDRepository.Get(record => record.InvoiceId == invoiceGuid).ToList().AsQueryable();

      return samplingFormDList;
    }

    /// <summary>
    /// Get Sampling Form D Attachment details.
    /// </summary>
    /// <param name="attachmentId">Attachment Id</param>
    /// <returns></returns>
    public SamplingFormDRecordAttachment GetSamplingFormDAttachment(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = SamplingFormDAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Insert Sampling Form D Attachment
    /// </summary>
    /// <param name="attachment">Attachment record</param>
    /// <returns></returns>
    public SamplingFormDRecordAttachment AddSamplingFormDAttachment(SamplingFormDRecordAttachment attachment)
    {
      SamplingFormDAttachmentRepository.Add(attachment);

      UnitOfWork.CommitDefault();
      attachment = SamplingFormDAttachmentRepository.Single(a => a.Id == attachment.Id);
      return attachment;
    }

    /// <summary>
    /// Update Sampling Form D attachment record parent id
    /// </summary>
    /// <param name="attachments">list of attachment</param>
    /// <param name="parentId">billing memo Id</param>
    /// <returns></returns>
    public IQueryable<SamplingFormDRecordAttachment> UpdateSamplingFormDAttachment(IList<Guid> attachments, Guid parentId)
    {
      var samplingFormDAttachmentInDb = SamplingFormDAttachmentRepository.Get(record => attachments.Contains(record.Id));

      foreach (var recordAttachment in samplingFormDAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        SamplingFormDAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();

      return samplingFormDAttachmentInDb.ToList().AsQueryable();
    }

    /// <summary>
    /// Check for duplicate file name for billing memo attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="samplingFormDId">sampling Form D Id</param>
    /// <returns></returns>
    public bool IsDuplicateSamplingFormDAttachmentFileName(string fileName, Guid samplingFormDId)
    {
      return SamplingFormDAttachmentRepository.GetCount(attachment => attachment.ParentId == samplingFormDId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Creates the form E.
    /// </summary>
    /// <param name="samplingFormE"></param>
    /// <returns>Created SamplingFormE object</returns>
    public SamplingFormEDetail CreateSamplingFormE(SamplingFormEDetail samplingFormE)
    {
      SamplingFormERepository.Add(samplingFormE);
      UnitOfWork.CommitDefault();

      // TODO: Update invoice total.

      return samplingFormE;
    }

    /// <summary>
    /// Gets the sampling form E.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public SamplingFormEDetail GetSamplingFormE(string invoiceId)
    {
      var invoiceGuidId = invoiceId.ToGuid();
      // TODO: Uncomment following once db check(1-1 mapping between Invoice and Form E) is added
      //var samplingFormE = SamplingFormERepository.Single(record => record.InvoiceId == invoiceGuidId);

      // TODO: Remove following once db check(1-1 mapping between Invoice and Form E) is added
      var samplingFormE = SamplingFormERepository.First(record => record.Id == invoiceGuidId);

      return samplingFormE;
    }

    /// <summary>
    /// Update form E details.
    /// </summary>
    /// <param name="samplingFormE"></param>
    /// <returns></returns>
    public SamplingFormEDetail UpdateSamplingFormE(SamplingFormEDetail samplingFormE)
    {
      var samplingFormEDb = SamplingFormERepository.Single(formE => formE.Id == samplingFormE.Id);
      if (samplingFormE != null)
      {
        samplingFormE.TotalGrossValue = samplingFormEDb.TotalGrossValue;
        samplingFormE.TotalHandlingFee = samplingFormEDb.TotalHandlingFee;
        samplingFormE.TotalIscAmount = samplingFormEDb.TotalIscAmount;
        samplingFormE.TotalOtherCommission = samplingFormEDb.TotalOtherCommission;
        samplingFormE.TotalTaxAmount = samplingFormEDb.TotalTaxAmount;
        samplingFormE.TotalUatpAmount = samplingFormEDb.TotalUatpAmount;
        samplingFormE.TotalVatAmount = samplingFormEDb.TotalVatAmount;


        var invoice = InvoiceRepository.Single(id: samplingFormE.Id);
        if (invoice.InvoiceStatus != InvoiceStatusType.Open)
        {
          invoice.InvoiceStatus = InvoiceStatusType.Open;
          invoice.ValidationStatus = InvoiceValidationStatus.Pending;
          invoice.ValidationDate = DateTime.MinValue;
          InvoiceRepository.Update(invoice);
        }
      }


      var updatedSamplingFormE = SamplingFormERepository.Update(samplingFormE);
      UnitOfWork.CommitDefault();

      return updatedSamplingFormE;
    }

    /// <summary>
    /// Add provisional Invoice.
    /// </summary>
    /// <param name="provisionalInvoice">Provisional invoice object.</param>
    /// <returns>Created Provisional invoice object.</returns>
    public ProvisionalInvoiceRecordDetail AddProvisionalInvoice(ProvisionalInvoiceRecordDetail provisionalInvoice)
    {
      ValidateProvisionalInvoiceRecord(provisionalInvoice);

      // Any addition/deletion/modification of records should update Form E ‘Form B Total Amounts’; and any fields in Form E dependent on this field.
      UpdateFormBTotalAmount(provisionalInvoice, false);

      _logger.InfoFormat("provisionalInvoice.InvoiceDate: {0} and {1} in TZ {2}", provisionalInvoice.InvoiceDate.ToLongDateString(), provisionalInvoice.InvoiceDate.ToLongTimeString(), provisionalInvoice.InvoiceDate.Kind);
      //      if (provisionalInvoice.InvoiceDate.Month > 1)
      //      {
      //        provisionalInvoice.InvoiceDate = provisionalInvoice.InvoiceDate.ToUniversalTime();
      // provisionalInvoice.InvoiceDate = new DateTime(provisionalInvoice.InvoiceDate.Year, provisionalInvoice.InvoiceDate.Month, provisionalInvoice.InvoiceDate.Day, 0, 0, 0, DateTimeKind.Utc);
      //        _logger.InfoFormat("provisionalInvoice.InvoiceDate: {0} and {1} in TZ {2}", provisionalInvoice.InvoiceDate.ToLongDateString(), provisionalInvoice.InvoiceDate.ToLongTimeString(), provisionalInvoice.InvoiceDate.Kind);
      // 
   // }
      ProvisionalInvoiceRepository.Add(provisionalInvoice);

      // Provisional Invoice can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (provisionalInvoice.Invoice.InvoiceStatus != InvoiceStatusType.Open)
      {
        provisionalInvoice.Invoice.InvoiceStatus = InvoiceStatusType.Open;
        provisionalInvoice.Invoice.ValidationStatus = InvoiceValidationStatus.Pending;
        provisionalInvoice.Invoice.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(provisionalInvoice.Invoice);
      }

      UnitOfWork.CommitDefault();

      return provisionalInvoice;
    }

    /// <summary>
    /// Returns the list of provisional invoice records.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns>List of provisional invoice records</returns>
    public IQueryable<ProvisionalInvoiceRecordDetail> GetProvisionalInvoiceList(string invoiceId)
    {
      var invoiceIdGuid = invoiceId.ToGuid();
      var provisionalInvoiceList = ProvisionalInvoiceRepository.Get(record => record.InvoiceId == invoiceIdGuid);

      return provisionalInvoiceList;
    }

    /// <summary>
    /// Returns the provisional invoice record.
    /// </summary>
    /// <param name="provisionalInvoiceId">provisional invoice id</param>
    /// <returns></returns>
    public ProvisionalInvoiceRecordDetail GetProvisionalInvoice(string provisionalInvoiceId)
    {
      var provisionalInvoiceGuid = provisionalInvoiceId.ToGuid();
      var provisionalInvoice = ProvisionalInvoiceRepository.Single(record => record.Id == provisionalInvoiceGuid);

      return provisionalInvoice;
    }

    /// <summary>
    /// Deletes the provisional invoice record.
    /// </summary>
    /// <param name="provisionalInvoiceId">provisional invoice id</param>
    /// <returns>
    /// True if record gets deleted successfully otherwise false.
    /// </returns>
    public bool DeleteProvisionalInvoice(string provisionalInvoiceId)
    {
      var provisionalInvoiceGuid = provisionalInvoiceId.ToGuid();

      var provisionalInvoice = ProvisionalInvoiceRepository.Single(record => record.Id == provisionalInvoiceGuid);

      if (provisionalInvoice == null)
      {
        return false;
      }

      // Any addition/deletion/modification of records should update Form E ‘Form B Total Amounts’; and any fields in Form E dependent on this field.
      UpdateFormBTotalAmount(provisionalInvoice, true);

      var invoice = InvoiceRepository.Single(id: provisionalInvoice.InvoiceId);
      // Provisional invoice can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.)
      if (invoice.InvoiceStatus != InvoiceStatusType.Open)
      {
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.ValidationStatus = InvoiceValidationStatus.Pending;
        invoice.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(invoice);
      }

      ProvisionalInvoiceRepository.Delete(provisionalInvoice);

      UnitOfWork.CommitDefault();

      // TODO: Update invoice total.

      return true;
    }

    /// <summary>
    /// Add sampling Form E Vat
    /// </summary>
    /// <param name="vat">Sampling Form E record</param>
    /// <returns></returns>
    public SamplingFormEDetailVat AddSamplingFormEVat(SamplingFormEDetailVat vat)
    {
      SamplingFormEVatRepository.Add(vat);

      var invoice = InvoiceRepository.Single(id: vat.ParentId);

      // Set status of parent invoice to open if invoice level vat is added.
      if (invoice.InvoiceStatus != InvoiceStatusType.Open)
      {
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.ValidationStatus = InvoiceValidationStatus.Pending;
        invoice.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(invoice);
      }

      UnitOfWork.CommitDefault();

      return vat;
    }

    /// <summary>
    /// Get Vat list associated with Form DE
    /// </summary>
    /// <param name="invoiceId">Form DE record Id</param>
    /// <returns></returns>
    public IQueryable<SamplingFormEDetailVat> GetSamplingFormEVatList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var invoiceVatList = SamplingFormEVatRepository.Get(record => record.ParentId == invoiceGuid);

      return invoiceVatList;
    }

    /// <summary>
    /// Delete sampling FormE Vat 
    /// </summary>
    /// <param name="formEVatId">FormEVat Id</param>
    /// <returns></returns>
    public bool DeleteSamplingFormEVat(string formEVatId)
    {
      var formEVatGuid = formEVatId.ToGuid();

      var formEVat = SamplingFormEVatRepository.Single(record => record.Id == formEVatGuid);

      if (formEVat == null)
      {
        return false;
      }

      var invoice = InvoiceRepository.Single(id: formEVat.ParentId);

      // Set status of parent invoice to open if invoice level vat is added.
      if (invoice.InvoiceStatus != InvoiceStatusType.Open)
      {
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.ValidationStatus = InvoiceValidationStatus.Pending;
        invoice.ValidationDate = DateTime.MinValue;
        InvoiceRepository.Update(invoice);
      }

      SamplingFormEVatRepository.Delete(formEVat);

      UnitOfWork.CommitDefault();

      return true;
    }

    /// <summary>
    /// Retrieves list of derived VAT in Form DE invoice
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public IList<DerivedVatDetails> GetFormDInvoiceLevelDerivedVatList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      IList<DerivedVatDetails> derivedVatRecords = InvoiceRepository.GetFormDDerivedVatDetails(invoiceGuid);
      return derivedVatRecords;
    }

    /// <summary>
    /// Retrieves list of VAT types which are not applied in Form DE invoice
    /// </summary>
    /// <param name="invoiceId">invoice id for which vat list to be retrieved.</param>
    /// <returns></returns>
    public IList<NonAppliedVatDetails> GetFormDNonAppliedVatList(string invoiceId)
    {
      var nonAppliedVatList = InvoiceRepository.GetFormDNonAppliedVatDetails(invoiceId.ToGuid());
      return nonAppliedVatList;
    }

    /// <summary>
    /// Gets the sampling form D record attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<SamplingFormDRecordAttachment> GetSamplingFormDRecordAttachments(List<Guid> attachmentIds)
    {
      return SamplingFormDAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Validate the Sampling Form D/E Invoice Header corresponding to the Invoice Details provided
    /// </summary>
    /// <param name="invoiceHeader"></param>
    /// <param name="invoiceHeaderInDb"></param>
    /// <returns></returns>
    protected override bool ValidateInvoiceHeader(PaxInvoice invoiceHeader, PaxInvoice invoiceHeaderInDb)
    {
      var isUpdateOperation = false;

      ////If there is record in db then its a update operation
      if (invoiceHeaderInDb != null)
      {
        isUpdateOperation = true;
      }

      base.ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb);

      if (invoiceHeader.InvoiceType != InvoiceType.Invoice)
      {
        throw new ISBusinessException(ErrorCodes.UnexpectedInvoiceType);
      }

      // Validation for Billing code, Billing Code should be 5 for Sampling Form D.
      if (invoiceHeader.BillingCode != (int)BillingCode.SamplingFormDE)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBillingCode);
      }


      // No more than one form D/E can be billed by an airline for a provisional month to same provisional billing airline.
      if (!isUpdateOperation)
      {
        // SCP56496: Unable to send FORMD
        // Added following filter criteria:
        // 1. Invoice Status != Error Non Correctable
        // 2. Invoice Validation Status != Failed.
        var count =
          InvoiceRepository.GetCount(
            invoice =>
            invoice.ProvisionalBillingMonth == invoiceHeader.ProvisionalBillingMonth &&
            invoice.ProvisionalBillingYear == invoiceHeader.ProvisionalBillingYear &&
            invoice.BilledMemberId == invoiceHeader.BilledMemberId &&
            invoice.BillingMemberId == invoiceHeader.BillingMemberId && invoice.BillingCode == invoiceHeader.BillingCode &&
            invoice.InvoiceStatusId != (int) InvoiceStatusType.ErrorNonCorrectable &&
            invoice.ValidationStatusId != (int) InvoiceValidationStatus.Failed);

        if (count > 0)
        {
          throw new ISBusinessException(SamplingErrorCodes.DuplicateFormDeInvoiceForTheProvisionalMonth);
        }
      }
      return true;
    }

    /// <summary>
    /// Validates an Invoice
    /// </summary>
    /// <param name="invoiceId">Invoice to be validated</param>
    /// <param name="isFutureSubmission"></param>
    /// <returns>
    /// True if successfully validated, false otherwise
    /// </returns>
    public override PaxInvoice ValidateInvoice(string invoiceId, out bool isFutureSubmission, int sessionUserId, string logRefId)
    {
        var log = ReferenceManager.GetDebugLog(DateTime.Now,
                                               "SamplingFormDEManager.ValidateInvoice",
                                               this.ToString(),
                                               BillingCategorys.Passenger.ToString(),
                                               "Step 3 of 12: Id: " + invoiceId + " NonSamplingCreditNoteManager.ValidateInvoice Start",
                                               sessionUserId,
                                               logRefId);

        ReferenceManager.LogDebugData(log);

      var webValidationErrors = new List<WebValidationError>();

      isFutureSubmission = false;
      var invoice = base.ValidateInvoice(invoiceId, out isFutureSubmission, sessionUserId, logRefId);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                     "SamplingFormDEManager.ValidateInvoice",
                                     this.ToString(),
                                     BillingCategorys.Passenger.ToString(),
                                     "Step 8 of 12: Id: " + invoiceId + " after base.ValidateInvoice()",
                                     sessionUserId,
                                     logRefId);
      ReferenceManager.LogDebugData(log);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
      webValidationErrors.AddRange(invoice.ValidationErrors);

      //var invoiceGuid = invoiceId.ToGuid();
      //var invoice = InvoiceRepository.Single(inv => inv.Id == invoiceGuid);

      if (!IsTransactionExists(invoiceId))
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), SamplingErrorCodes.FormDeTransactionNotFound));
        //invoice.InvoiceStatus = InvoiceStatusType.ValidationError;
        //InvoiceRepository.Update(invoice);
        //UnitOfWork.CommitDefault();

        //throw new ISBusinessException(SamplingErrorCodes.FormDeTransactionNotFound);
      }

      //SCP149711 - Incorrect Form E UA to 3M
      invoice.IsRecalculatedFormE = PaxInvoiceRepository.RecalculateFormE(invoiceId.ToGuid()) ? RecalculateFormE.Yes : RecalculateFormE.No;

      // Validate total amount form B with the sum of Billing Amounts of Provisional Invoices. (For IS-WEB we check for exact match without tolerance) 
      var provisionalInvoiceRecordDetails = ProvisionalInvoiceRepository.Get(provInvoice => provInvoice.InvoiceId == invoice.Id).ToList();
      if (provisionalInvoiceRecordDetails.Count <= 0)
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), SamplingErrorCodes.ProvisionalInvoiceNotFound));
      }

     /* var provInvoices = InvoiceRepository.GetInvoicesWithTotal(invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, null, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormAB);

        if(provInvoices != null && provInvoices.Count > 0)
        {
          decimal totalAmountFormB = Convert.ToDecimal(ConvertUtil.Round((from provisionalInvoice in provInvoices
                                                                          let currencyConversionFactor = GetCurrencyConversionFactor(provisionalInvoice.ListingCurrencyId, invoice.ListingCurrencyId, provisionalInvoice.BillingYear, provisionalInvoice.BillingMonth)
                                                                          select provisionalInvoice.InvoiceTotalRecord.NetTotal * Convert.ToDecimal(currencyConversionFactor)).Sum(), Constants.PaxDecimalPlaces));

          //SCP133697 - SIS Validation Error - Sample D/E, consider tolerance while comparing
          //Populate invoice tolerance
          if (invoice.Tolerance == null)
          {
              if (invoice.ListingCurrencyId.HasValue)
              {
                  invoice.Tolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax, invoice.ListingCurrencyId.Value, invoice, Constants.PaxDecimalPlaces);
              }
              else
              {
                  invoice.Tolerance = new Tolerance
                  {
                      ClearingHouse = CompareUtil.GetClearingHouse(invoice.SettlementMethodId),
                      BillingCategoryId = (int)BillingCategoryType.Pax,
                      RoundingTolerance = 0,
                      SummationTolerance = 0
                  };
              }
          }
          if(!CompareUtil.Compare(invoice.SamplingFormEDetails.TotalAmountFormB,totalAmountFormB,invoice.Tolerance.SummationTolerance,Constants.PaxDecimalPlaces))
          {
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(SamplingErrorCodes.FormBTotalProvisionalInvoicetotalMismatch,
                                  invoiceId.ToGuid(),
                                  invoice.SamplingFormEDetails.TotalAmountFormB.ToString(),
                                  totalAmountFormB.ToString(),
                                  (invoice.SamplingFormEDetails.TotalAmountFormB - totalAmountFormB).ToString()));
          }
        
        }																		  
      //Validate total amount form B
        //var totalAmountFormB = provisionalInvoiceRecordDetails.Aggregate<ProvisionalInvoiceRecordDetail, decimal>(0,(current,provisionalInvoiceRecordDetail)=> current + provisionalInvoiceRecordDetail.InvoiceBillingAmount);
      
      */

      if (invoice.SamplingFormEDetails.SamplingConstant <= 0)
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.NegativeSamplingConstant));
      }

      // TODO: The invoice total needs to be non-zero.
      // Invoice through the validation, change invoice status to Ready for submission 
      // invoice.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;

      if (webValidationErrors.Count > 0)
      {
        invoice.ValidationErrors.Clear();
        invoice.ValidationErrors.AddRange(webValidationErrors);
        invoice.InvoiceStatus = InvoiceStatusType.ValidationError;
        // updating validation status to Error if there are more than 1 error, if its one then its already set to ErrorPeriod.
        if (webValidationErrors.Count > 1) invoice.ValidationStatus = InvoiceValidationStatus.Failed;
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
                                   "SamplingFormDEManager.ValidateInvoice",
                                   this.ToString(),
                                   BillingCategorys.Passenger.ToString(),
                                   "Step 9 of 12: Id: " + invoice.Id + " after invoice.ValidationDate = DateTime.UtcNow",
                                   sessionUserId,
                                   logRefId);
      ReferenceManager.LogDebugData(log);

      // Update the invoice.
      //InvoiceRepository.Update(invoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      //SCP350004: Validating Form D/E
      InvoiceRepository.SetInvoiceAndValidationStatus(invoice.Id, invoice.InvoiceStatusId, invoice.ValidationStatusId, isFutureSubmission, invoice.ClearingHouse, invoice.SamplingFormEDetails.NetBilledCreditedAmount, (int)BillingCode.SamplingFormDE, invoice.ExchangeRate);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                     "SamplingFormDEManager.ValidateInvoice",
                                     this.ToString(),
                                     BillingCategorys.Passenger.ToString(),
                                     "Step 10 of 12: Id: " + invoice.Id + " after InvoiceRepository.SetInvoiceAndValidationStatus()",
                                     sessionUserId,
                                     logRefId);
      ReferenceManager.LogDebugData(log);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                   "SamplingFormDEManager.ValidateInvoice",
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
    /// Function to validate Form D Record.
    /// </summary>
    /// <param name="formDRecord">Coupon record to be updated.</param>
    /// <param name="formDRecordInDb">Existing coupon record in db.</param>
    /// <param name="samplingFormDE">The sampling form DE.</param>
    public string ValidateFormDRecord(SamplingFormDRecord formDRecord, SamplingFormDRecord formDRecordInDb, PaxInvoice samplingFormDE, bool isErrorCorrection = false)
    {
      var isUpdateOperation = false;
      var duplicateCouponErrorMessage = string.Empty;
      //Below code to check invalid TaxCode enter by user. if user enter invalid taxcode then system will generate error message.
      //SCP#50425 - Tax Code XT
      foreach (var tax in formDRecord.TaxBreakdown)
      {
        if (!string.IsNullOrWhiteSpace(tax.TaxCode))
          tax.TaxCode = tax.TaxCode.Trim();
        if (!ReferenceManager.IsValidTaxCode(tax.TaxCode))
        {
          throw new ISBusinessException(ErrorCodes.InvalidTaxCode, tax.TaxCode);
        }
      }
      //End SCP#50425 - Tax Code XT
      //If there is record in db then its a update operation
      if (formDRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (formDRecord.TicketDocNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTicketDocumnetOrFimNumber);
      }

      if (!SamplingFormDRepository.IsFormDRecordDuplicate(formDRecord.InvoiceId, formDRecord.RecordSeqNumberOfProvisionalInvoice, formDRecord.BatchNumberOfProvisionalInvoice, formDRecord.ProvisionalInvoiceNumber, formDRecord.Id))
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNo);
      }

      // Check whether net amount is positive else throw an exception
      if (formDRecord.EvaluatedNetAmount < 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAmount);
      }

      // Get the Form D Tax breakdown amount against coupon record Tax Amount.
      var couponTaxBreakdownTotal = formDRecord.TaxBreakdown.Aggregate(0.0, (current, tax) => current + Convert.ToDouble(tax.Amount));
      couponTaxBreakdownTotal = ConvertUtil.Round(couponTaxBreakdownTotal, Constants.PaxDecimalPlaces);

      // Check Coupon Tax Breakdown Total Amount to match Coupon Tax Amount.
      // If Not, then throw exception.
      if (!formDRecord.TaxAmount.Equals(Convert.ToDouble(couponTaxBreakdownTotal)))
      {
        throw new ISBusinessException(ErrorCodes.TaxTotalAmountMismatch);
      }

      // Get the Form D Vat breakdown amount against Form D Record Vat Amount.
      double couponVatBreakdownTotal = formDRecord.VatBreakdown.Aggregate(0.0, (current, vat) => current + Convert.ToDouble(vat.VatCalculatedAmount));
      couponVatBreakdownTotal = ConvertUtil.Round(couponVatBreakdownTotal, Constants.PaxDecimalPlaces);

      // Check Coupon Vat Breakdown Total Amount to match Form D Vat Amount.
      // If Not, then throw exception. 
      if (!formDRecord.VatAmount.Equals(Convert.ToDouble(couponVatBreakdownTotal)))
      {
        throw new ISBusinessException(ErrorCodes.VatTotalAmountMismatch);
      }

      //Validate Batch Sequence Number and Record Sequence Number
      if (formDRecord.RecordSeqNumberOfProvisionalInvoice <= 0 || formDRecord.BatchNumberOfProvisionalInvoice <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }

      if (!isUpdateOperation ||
        (CompareUtil.IsDirty(formDRecord.TicketDocNumber, formDRecordInDb.TicketDocNumber) ||
        CompareUtil.IsDirty(formDRecord.TicketIssuingAirline, formDRecordInDb.TicketIssuingAirline) ||
        CompareUtil.IsDirty(formDRecord.CouponNumber, formDRecordInDb.CouponNumber)))
      {
        ////Duplicate billing memo number to be considered for one year
        //DateTime billingDate;
        //var billingYearToCompare = 0;
        //var billingMonthToCompare = 0;

        //if (DateTime.TryParse(string.Format("{0}/{1}/{2}", formDRecord.BillingYear.ToString().PadLeft(2, '0'), formDRecord.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
        //{
        //  var billingDateToCompare = billingDate.AddMonths(-12);
        //  billingYearToCompare = billingDateToCompare.Year;
        //  billingMonthToCompare = billingDateToCompare.Month;
        //}

        // Validate duplicate coupon - combination Ticket/FIM number, issuing airline, coupon number exist in same invoice.
        // For New coupon Record validation will be done 
        // For Update coupon Record if any value from TicketDocOrFimNumber,TicketOrFimIssuingAirline,TicketOrFimCouponNumber is updated then only validation will be done
        // Validate duplicate coupon - combination Ticket/FIM number, issuing airline, coupon number exist in other invoice created in last 12 months has same billed member for current coupon.
        var duplicateCouponCount = SamplingFormDRepository.GetCount(formD => formD.CouponNumber == formDRecord.CouponNumber
                                                                       && formD.TicketDocNumber == formDRecord.TicketDocNumber
                                                                       && formD.TicketIssuingAirline == formDRecord.TicketIssuingAirline
                                                                       && formD.InvoiceId == formDRecord.InvoiceId);


        if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0 && !isErrorCorrection))
        {
          duplicateCouponErrorMessage = string.Format(Messages.SamplingFormDDuplicateMessage, isUpdateOperation ? duplicateCouponCount - 1 : duplicateCouponCount);
        }

        //if (SamplingFormDRepository.GetCount(formD => formD.CouponNumber == formDRecord.CouponNumber
        //                                                               && formD.TicketDocNumber == formDRecord.TicketDocNumber
        //                                                               && formD.TicketIssuingAirline == formDRecord.TicketIssuingAirline
        //                                                               && formD.InvoiceId == formDRecord.InvoiceId) > 0)
        //{
        //  throw new ISBusinessException(SamplingErrorCodes.DuplicateFormDRecordFound);
        //}
      }

      // Validates whether source code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(formDRecordInDb.SourceCodeId, formDRecord.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(formDRecord.SourceCodeId, (int)TransactionType.SamplingFormD))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }
        
      var validationError = ValidateBvcMatrix(formDRecord, samplingFormDE);

      if (!string.IsNullOrEmpty(validationError))
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidFormDBvcMatrix, validationError);
      }

      return duplicateCouponErrorMessage;
    }

    /// <summary>
    /// Determines whether transaction exists for the specified invoice id
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    public bool IsTransactionExists(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      return (SamplingFormDRepository.GetCount(samplingFormD => samplingFormD.InvoiceId == invoiceGuid) > 0);
    }

    public bool IsProvisionalInvoiceExists(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      return (ProvisionalInvoiceRepository.GetCount(provInvoice => provInvoice.InvoiceId == invoiceGuid) > 0);
    }

    /// <summary>
    /// Validate source code total fields with total of form D records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <returns></returns>
    private bool ValidateParsedSourceCodeTotalForFormDRecords(PaxInvoice invoice, SourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      if (sourceCodeTotal.VatBreakdown.Count > 0)
      {
        // Total of vat amount in all the vat records.
        double totalVat = 0;

        totalVat = sourceCodeTotal.VatBreakdown.Sum(vatdata => (vatdata.VatCalculatedAmount));

        if (invoice.Tolerance != null && !CompareUtil.Compare(sourceCodeTotal.TotalVatAmount, Convert.ToDecimal(totalVat), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code Total Vat Amount",
                                                                          sourceCodeTotal.TotalVatAmount.ToString(),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                          ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if (sourceCodeTotal.TotalVatAmount != 0)
        {

          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.VatBreakdownRecordsRequired, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      List<SamplingFormDRecord> formDSourceCodeRecords = null;
      formDSourceCodeRecords = invoice.SamplingFormDRecord.Where(formD => formD.SourceCodeId == sourceCode).ToList();
      if (formDSourceCodeRecords != null)
      {
        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(sourceCodeTotal.TotalGrossValue, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.EvaluatedGrossAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Value",
                                                                            sourceCodeTotal.TotalGrossValue.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidGrossTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalTaxAmount, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.TaxAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount",
                                                                            sourceCodeTotal.TotalTaxAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidTaxTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalVatAmount, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.VatAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidVatTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalUatpAmount, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.UatpAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount",
                                                                            sourceCodeTotal.TotalUatpAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidUatpAmountTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalIscAmount, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.IscAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount",
                                                                            sourceCodeTotal.TotalIscAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidIscTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalOtherCommission, Convert.ToDecimal(formDSourceCodeRecords.Sum(record => record.OtherCommissionAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount",
                                                                            sourceCodeTotal.TotalOtherCommission.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidOtherCommissionTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalHandlingFee, formDSourceCodeRecords.Sum(record => record.HandlingFeeAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount",
                                                                            sourceCodeTotal.TotalHandlingFee.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidHandlingFeeTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          // Validate Net Total : it should be equal to sum of Total Gross, Total Tax, Total ISC, Handling Fee Amount, Total Other Commission, Total UATP and Total Tax
          var calculatedAmount = sourceCodeTotal.TotalGrossValue + sourceCodeTotal.TotalTaxAmount +
                                 sourceCodeTotal.TotalVatAmount
                                 + sourceCodeTotal.TotalUatpAmount + sourceCodeTotal.TotalIscAmount +
                                 sourceCodeTotal.TotalOtherCommission +
                                 Convert.ToDecimal(sourceCodeTotal.TotalHandlingFee);

          if (!CompareUtil.Compare(sourceCodeTotal.TotalNetAmount, calculatedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Source Code Total Net Amount",
                                                                            sourceCodeTotal.TotalNetAmount.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              NetTotalNotEqualsToSumOfOtherAmounts,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }

        //Total number of records validation
        long totalNumberOfRecords = (formDSourceCodeRecords.Count + formDSourceCodeRecords.Sum(record => record.NumberOfChildRecords) + sourceCodeTotal.NumberOfChildRecords + 1);
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.TotalNumberOfRecords != totalNumberOfRecords)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidTotalNumberOfRecords, ErrorStatus.X, sourceCode, 99999, 99999, null, false, null, Convert.ToString(totalNumberOfRecords));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      return isValid;
    }

    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline"></param>
    /// <returns></returns>
    public bool ValidateParsedSamplingFormD(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, IDictionary<string, bool> issuingAirline)
    {
      bool isValid = true;
      bool isTransactionRecordsExistsInInvoice = false;

      // SCP56496: Unable to send FORMD
      // Get count where invoice status != non-correctable and validation status != failed.
      var count =
        InvoiceRepository.GetCount(
          formDInvoice =>
          formDInvoice.ProvisionalBillingMonth == invoice.ProvisionalBillingMonth &&
          formDInvoice.ProvisionalBillingYear == invoice.ProvisionalBillingYear &&
          formDInvoice.BilledMemberId == invoice.BilledMemberId &&
          formDInvoice.BillingMemberId == invoice.BillingMemberId && formDInvoice.BillingCode == invoice.BillingCode &&
          formDInvoice.InvoiceStatusId != (int) InvoiceStatusType.ErrorNonCorrectable &&
          formDInvoice.ValidationStatusId != (int)InvoiceValidationStatus.Failed);

      if (count > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Form D Invoice", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, SamplingErrorCodes.DuplicateFormDeInvoiceForTheProvisionalMonth, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      foreach (SamplingFormDRecord samplingFormDRecord in invoice.SamplingFormDRecord)
      {
        samplingFormDRecord.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.Validated;
        isValid = ValidateParsedSamplingFormDRecord(samplingFormDRecord, exceptionDetailsList, invoice, fileName, issuingAirline, fileSubmissionDate);
        isTransactionRecordsExistsInInvoice = true;
      }

      if (isTransactionRecordsExistsInInvoice)
      {
        //Member migration check
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormD, false) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForSamplingDeIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormD, true) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForSamplingDeIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        foreach (var sourceCodeTotal in invoice.SourceCodeTotal)
        {

          if (!ReferenceManager.IsValidSourceCode(invoice, sourceCodeTotal.SourceCodeId, (int)TransactionType.SamplingFormD))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code", Convert.ToString(sourceCodeTotal.SourceCodeId),
                                               invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidSourceCode, ErrorStatus.X, sourceCodeTotal.SourceCodeId);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {
            //For the source code
            isValid = ValidateParsedSourceCodeTotalForFormDRecords(invoice, sourceCodeTotal, sourceCodeTotal.SourceCodeId, exceptionDetailsList, fileName, fileSubmissionDate);
          }
        }
      }
      else
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sampling Form DE Invoice", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MandatoryTransactionInInvoice, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validates the parsed sampling form D record.
    /// </summary>
    /// <param name="samplingFormDRecord">The sampling form D record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline"></param>
    /// <returns></returns>
    private bool ValidateParsedSamplingFormDRecord(SamplingFormDRecord samplingFormDRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(samplingFormDRecord.TicketDocNumber).Length > 10)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Form D Coupon",
                                                                        Convert.ToString(samplingFormDRecord.TicketDocNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormD,
                                                                        SamplingErrorCodes.TicketFimDocumentNoGreaterThanTenS,
                                                                        ErrorStatus.X,
                                                                        samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Multiple entries for Provisional Invoice + Batch Number + Record Sequence No + Ticket Issuing Airline + Coupon Number + Ticket/Document Number should not exist.

      if (SamplingFormDRepository.GetCount(formD => formD.CouponNumber == samplingFormDRecord.CouponNumber
                                                                       && formD.TicketDocNumber == samplingFormDRecord.TicketDocNumber
                                                                       && formD.TicketIssuingAirline == samplingFormDRecord.TicketIssuingAirline
                                                                       && formD.BatchNumberOfProvisionalInvoice == samplingFormDRecord.BatchNumberOfProvisionalInvoice
                                                                       && formD.RecordSeqNumberOfProvisionalInvoice == samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice
                                                                       && formD.ProvisionalInvoiceNumber == samplingFormDRecord.ProvisionalInvoiceNumber
                                                                       && formD.Invoice.BillingMemberId == invoice.BillingMemberId
                                                                       && formD.Invoice.BilledMemberId == invoice.BilledMemberId) > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Form D Coupon",
                                                                 Convert.ToString(samplingFormDRecord.SourceCodeId), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD,
                                                                 SamplingErrorCodes.DuplicateFormDRecordFound, ErrorStatus.X,
                                                                 samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (invoice.SamplingFormDRecord.Where(formD => formD.CouponNumber == samplingFormDRecord.CouponNumber && formD.TicketDocNumber == samplingFormDRecord.TicketDocNumber
                                                                       && formD.TicketIssuingAirline == samplingFormDRecord.TicketIssuingAirline
                                                                       && formD.BatchNumberOfProvisionalInvoice == samplingFormDRecord.BatchNumberOfProvisionalInvoice
                                                                       && formD.RecordSeqNumberOfProvisionalInvoice == samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice
                                                                       && formD.ProvisionalInvoiceNumber == samplingFormDRecord.ProvisionalInvoiceNumber).Count() > 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Form D Coupon",
                                                                 samplingFormDRecord.TicketDocNumber.ToString(), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD,
                                                                 SamplingErrorCodes.DuplicateFormDRecordFound, ErrorStatus.X,
                                                                 samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (!ReferenceManager.IsValidSourceCode(invoice, samplingFormDRecord.SourceCodeId, (int)TransactionType.SamplingFormD))
      {

        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code",
                                                                        Convert.ToString(samplingFormDRecord.SourceCodeId),
                                                                        invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD,
                                                                        ErrorCodes.InvalidSourceCode, ErrorStatus.X,
                                                                        samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }


      // Form D coupon should not present in sampling form C coupons. Dupe is defined as a combination of:  Ticket Issuing Airline,  Ticket/Document Number,  Coupon No, Prov. Inv. No.  Batch No. of Prov. Inv.,  Seq. No. of Prov. Inv.
      if (SamplingFormCRecordRepository.GetCount(sfcRecord => sfcRecord.TicketIssuingAirline == samplingFormDRecord.TicketIssuingAirline
                                                                                         && sfcRecord.DocumentNumber == samplingFormDRecord.TicketDocNumber
                                                                                         && sfcRecord.CouponNumber == samplingFormDRecord.CouponNumber
                                                                                         && sfcRecord.BatchNumberOfProvisionalInvoice == samplingFormDRecord.BatchNumberOfProvisionalInvoice
                                                                                         && sfcRecord.RecordSeqNumberOfProvisionalInvoice == samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice
                                                                                         && sfcRecord.ProvisionalInvoiceNumber == samplingFormDRecord.ProvisionalInvoiceNumber) > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Form D Coupon",
                                                                        string.Format("{0},{1},{2}",
                                                                                      samplingFormDRecord.TicketIssuingAirline,
                                                                                      samplingFormDRecord.TicketDocNumber,
                                                                                      samplingFormDRecord.CouponNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormD,
                                                                        SamplingErrorCodes.AuditTrailFailsForFormDCoupon,
                                                                        ErrorStatus.X,
                                                                        samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      var isXmlfileType = (invoice.SubmissionMethod == SubmissionMethod.IsXml) ? true : false;


      // Validate provisional invoice number.
      var provisionalInvoice = InvoiceRepository.Single(samplingFormDRecord.ProvisionalInvoiceNumber,
                                                        invoice.ProvisionalBillingMonth,
                                                        invoice.ProvisionalBillingYear,
                                                        null,
                                                        invoice.BilledMemberId,
                                                        invoice.BillingMemberId,
                                                        (int)BillingCode.SamplingFormAB,
                                                        invoiceStatusId: (int)InvoiceStatusType.Presented
                                                        );

      if (provisionalInvoice != null)
      {
        _logger.InfoFormat("Provisional Invoice is found: {0}", provisionalInvoice.InvoiceNumber);
        var couponRecords =
          CouponRecordRepository.Get(
            record =>
            record.TicketOrFimCouponNumber == samplingFormDRecord.CouponNumber &&
            record.TicketOrFimIssuingAirline == samplingFormDRecord.TicketIssuingAirline &&
            record.TicketDocOrFimNumber == samplingFormDRecord.TicketDocNumber &&
            (invoice.SubmissionMethod != SubmissionMethod.IsIdec ||
             (record.BatchSequenceNumber == samplingFormDRecord.BatchNumberOfProvisionalInvoice &&
              record.RecordSequenceWithinBatch == samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice)) &&
            record.InvoiceId == provisionalInvoice.Id);

        var nCoupons = couponRecords.Count();
        if (nCoupons == 0)
        {
          //SCP#48125 : Linking #1. Should match with the Provisional Invoice details in the Audit Trail
          //SCP#133111: ERROR IN VALIDATION - SF Incident No. 03463835
          //SCP#112936: Implemented code to check following while linking Form D Coupon with form AB Coupon
          //            1. For IDEC: Compare Prov Invoice + Batch + Seq + Ticket Issuing Airline + Doc No + Coupon No.
          //            2. For XML : Compare Prov Invoice + Ticket Issuing Airline + Doc No + Coupon No.
          if(invoice.SubmissionMethod == SubmissionMethod.IsIdec)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Form D Coupon",
                                                                              string.Format("{0},{1},{2},{3},{4}", samplingFormDRecord.TicketIssuingAirline, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber, samplingFormDRecord.BatchNumberOfProvisionalInvoice, samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelSamplingFormD,
                                                                              SamplingErrorCodes.AuditTrailMismatchSamplingFormDCoupon,
                                                                              ErrorStatus.X,
                                                                              samplingFormDRecord);
            exceptionDetailsList.Add(validationExceptionDetail);  
          }
          else
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Form D Coupon",
                                                                              string.Format("{0},{1},{2}", samplingFormDRecord.TicketIssuingAirline, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelSamplingFormD,
                                                                              SamplingErrorCodes.AuditTrailFailForCouponNumber,
                                                                              ErrorStatus.X,
                                                                              samplingFormDRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
          }
          
              isValid = false;
        }
        else
        {
          ////SCP#48125 : Linking #2. The Ticket Issuing Airline + Coupon Number + Ticket/Document Number should not be a Form C rejected coupon
          //var samplingFormCList = SamplingFormCRepository.GetSamplingFormCDetails(invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, invoice.BillingMemberId, "6", invoice.BilledMemberId);

          //if (samplingFormCList.Count > 0)
          //{
          //  var formCcouponCount = 0;
          //  foreach (var samplingFormC in samplingFormCList)
          //  {
          //    formCcouponCount +=
          //    samplingFormC.SamplingFormCDetails.Where(
          //      record =>
          //      record.CouponNumber == samplingFormDRecord.CouponNumber && record.TicketIssuingAirline == samplingFormDRecord.TicketIssuingAirline &&
          //      record.DocumentNumber == samplingFormDRecord.TicketDocNumber).Count();
          //  }
             
          //  if (formCcouponCount > 0)
          //  {
          //    var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
          //                                                                    exceptionDetailsList.Count() + 1,
          //                                                                    fileSubmissionDate,
          //                                                                    "Form D Coupon",
          //                                                                    string.Format("{0},{1},{2}",
          //                                                                                  samplingFormDRecord.TicketIssuingAirline,
          //                                                                                  samplingFormDRecord.TicketDocNumber,
          //                                                                                  samplingFormDRecord.CouponNumber),
          //                                                                    invoice,
          //                                                                    fileName,
          //                                                                    ErrorLevels.ErrorLevelSamplingFormD,
          //                                                                    SamplingErrorCodes.FormDCouponFoundInFormC,
          //                                                                    ErrorStatus.X,
          //                                                                    samplingFormDRecord);
          //    exceptionDetailsList.Add(validationExceptionDetail);
          //    isValid = false;
          //  }
          //}
          //SCP#48125 : Linking #3. Multiple entries for Provisional Invoice + Batch Number + Record Sequence No + Ticket Issuing Airline + Coupon Number + Ticket/Document Number should not exist.
         var linkedCoupons = couponRecords.Where(cp=>cp.BatchSequenceNumber== samplingFormDRecord.BatchNumberOfProvisionalInvoice && cp.RecordSequenceWithinBatch == samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice);
         if (linkedCoupons.Count() > 1)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Form D Coupon", string.Format("{0},{1},{2}", samplingFormDRecord.TicketIssuingAirline, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD,
                                                                            SamplingErrorCodes.InvalidCouponNumberFormD, ErrorStatus.X, samplingFormDRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
         if (linkedCoupons.Count() > 0)
         {
           var couponRecord = linkedCoupons.First();

           // ID : 56824 - R2 file Sample Airline 001 Form D --- URGENT
           if (!string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorValidated))
             couponRecord.AgreementIndicatorValidated = couponRecord.AgreementIndicatorValidated.Trim();


           if (exceptionDetailsList != null && (couponRecord.BatchSequenceNumber != samplingFormDRecord.BatchNumberOfProvisionalInvoice || couponRecord.RecordSequenceWithinBatch != samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice))
           {
             //SCP#48125 : Linking
             if (couponRecord.BatchSequenceNumber != samplingFormDRecord.BatchNumberOfProvisionalInvoice)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
                                                                               exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Provisional Invoice Batch Number",
                                                                               samplingFormDRecord.BatchNumberOfProvisionalInvoice.ToString(),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               SamplingErrorCodes.InvalidProvisionalInvoiceBatchNo,
                                                                               ErrorStatus.C,
                                                                               samplingFormDRecord,
                                                                               islinkingError: true);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             if (couponRecord.RecordSequenceWithinBatch != samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
                                                                               exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Provisional Invoice Record Seq Number",
                                                                               samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice.ToString(),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               SamplingErrorCodes.InvalidProvisionalInvoiceBatchSeqNo,
                                                                               ErrorStatus.C,
                                                                               samplingFormDRecord,
                                                                               islinkingError: true);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
           }

           if (exceptionDetailsList == null && couponRecord.BatchSequenceNumber != samplingFormDRecord.BatchNumberOfProvisionalInvoice)
           {
             throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalInvoiceBatchNo);
           }

           if (exceptionDetailsList == null && couponRecord.RecordSequenceWithinBatch != samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice)
           {
             throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalInvoiceBatchSeqNo);
           }

           var currencyConversionFactor = GetCurrencyConversionFactor(provisionalInvoice.ListingCurrencyId, invoice.ListingCurrencyId, provisionalInvoice.BillingYear, provisionalInvoice.BillingMonth);
           var couponGrossValueOrAlf = ConvertUtil.Round(Convert.ToDouble(couponRecord.CouponGrossValueOrApplicableLocalFare) * currencyConversionFactor, Constants.PaxDecimalPlaces);

           if (invoice.Tolerance != null && !CompareUtil.Compare(couponGrossValueOrAlf, samplingFormDRecord.ProvisionalGrossAlfAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Provisional Gross Alf Amount",
                                                                               Convert.ToString(samplingFormDRecord.ProvisionalGrossAlfAmount),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               SamplingErrorCodes.InvalidProvisionalGrossAlfAmount,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);

               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalGrossAlfAmount);
             }
           }

           //SCP ID : 56824 - R2 file Sample Airline 001 Form D --- URGENT
           if (couponRecord.AgreementIndicatorValidated == AgreementIndicatorValidatedI)
           {
             //Evaluated values should match with provisional billing values.
             var validationError = ValidateParsedBvcMatrix(samplingFormDRecord, couponRecord, currencyConversionFactor, invoice.Tolerance == null ? 0 : invoice.Tolerance.RoundingTolerance);

             if (validationError != null && !string.IsNullOrEmpty(validationError.Trim()))
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Form D Coupon",
                                                                               string.Format("{0},{1},{2}", samplingFormDRecord.TicketIssuingAirline, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber),
                                                                               invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD,
                                                                               SamplingErrorCodes.InvalidFormDBvcMatrix, ErrorStatus.X, samplingFormDRecord);
               validationExceptionDetail.ErrorDescription = validationError + " " + validationExceptionDetail.ErrorDescription;
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
           }
           // In case of NFP Reason code is Null, assign it to empty string as in FormD contains empty string if no value provided. This is required for comparison
           if (string.IsNullOrWhiteSpace(couponRecord.NfpReasonCode))
             couponRecord.NfpReasonCode = string.Empty;

           if (string.IsNullOrWhiteSpace(samplingFormDRecord.NfpReasonCode))
             samplingFormDRecord.NfpReasonCode = string.Empty;

           if (!string.IsNullOrWhiteSpace(couponRecord.NfpReasonCode))
             couponRecord.NfpReasonCode = couponRecord.NfpReasonCode.Trim();

           if (!string.IsNullOrWhiteSpace(samplingFormDRecord.NfpReasonCode))
             samplingFormDRecord.NfpReasonCode = samplingFormDRecord.NfpReasonCode.Trim();


           if (couponRecord.NfpReasonCode != samplingFormDRecord.NfpReasonCode)
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Nfp Reason Code",
                                                                               Convert.ToString(samplingFormDRecord.NfpReasonCode),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               ErrorCodes.InvoiceNfpReasonCode,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(ErrorCodes.InvoiceNfpReasonCode);
             }
           }

           // For comparison required triming the both side values
           if (string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied))
             couponRecord.AgreementIndicatorSupplied = string.Empty;

           if (string.IsNullOrWhiteSpace(samplingFormDRecord.AgreementIndicatorSupplied))
             samplingFormDRecord.AgreementIndicatorSupplied = string.Empty;

           if (!string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied))
             couponRecord.AgreementIndicatorSupplied = couponRecord.AgreementIndicatorSupplied.Trim();

           if (!string.IsNullOrWhiteSpace(samplingFormDRecord.AgreementIndicatorSupplied))
             samplingFormDRecord.AgreementIndicatorSupplied = samplingFormDRecord.AgreementIndicatorSupplied.Trim();

           if (couponRecord.AgreementIndicatorSupplied != samplingFormDRecord.AgreementIndicatorSupplied)
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Agreement Indicator Supplied",
                                                                               Convert.ToString(samplingFormDRecord.AgreementIndicatorSupplied),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               ErrorCodes.InvalidAgreementIndicatorSupplied,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);

               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(ErrorCodes.InvalidAgreementIndicatorSupplied);
             }
           }


           // For comparison required triming the both side values
           if (string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorValidated))
             couponRecord.AgreementIndicatorValidated = string.Empty;

           if (string.IsNullOrWhiteSpace(samplingFormDRecord.AgreementIndicatorValidated))
             samplingFormDRecord.AgreementIndicatorValidated = string.Empty;

           if (!string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorValidated))
             couponRecord.AgreementIndicatorValidated = couponRecord.AgreementIndicatorValidated.Trim();

           if (!string.IsNullOrWhiteSpace(samplingFormDRecord.AgreementIndicatorValidated))
             samplingFormDRecord.AgreementIndicatorValidated = samplingFormDRecord.AgreementIndicatorValidated.Trim();

           if (couponRecord.AgreementIndicatorValidated != samplingFormDRecord.AgreementIndicatorValidated)
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Agreement Indicator Validated",
                                                                               Convert.ToString(samplingFormDRecord.AgreementIndicatorValidated),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               ErrorCodes.InvalidAgreementIndicatorValidated,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(ErrorCodes.InvalidAgreementIndicatorValidated);
             }
           }

           if (string.IsNullOrWhiteSpace(couponRecord.OriginalPmi))
             couponRecord.OriginalPmi = string.Empty;
           if (string.IsNullOrWhiteSpace(samplingFormDRecord.OriginalPmi))
             samplingFormDRecord.OriginalPmi = string.Empty;

           if (couponRecord.OriginalPmi != samplingFormDRecord.OriginalPmi)
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Original Pmi",
                                                                               Convert.ToString(samplingFormDRecord.OriginalPmi),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               ErrorCodes.InvalidOriginalPMI,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(ErrorCodes.InvalidOriginalPMI);
             }
           }

           // ID : 56824 - R2 file Sample Airline 001 Form D --- URGENT
           if (string.IsNullOrWhiteSpace(couponRecord.ValidatedPmi))
             couponRecord.ValidatedPmi = string.Empty;
           if (string.IsNullOrWhiteSpace(samplingFormDRecord.ValidatedPmi))
             samplingFormDRecord.ValidatedPmi = string.Empty;
           if (couponRecord.ValidatedPmi != samplingFormDRecord.ValidatedPmi)
           {
             if (exceptionDetailsList != null)
             {
               var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate,
                                                                               "Validated Pmi",
                                                                               Convert.ToString(samplingFormDRecord.ValidatedPmi),
                                                                               invoice,
                                                                               fileName,
                                                                               ErrorLevels.ErrorLevelSamplingFormD,
                                                                               ErrorCodes.InvalidValidatedPMI,
                                                                               ErrorStatus.X,
                                                                               samplingFormDRecord);
               exceptionDetailsList.Add(validationExceptionDetail);
               isValid = false;
             }
             else
             {
               throw new ISBusinessException(ErrorCodes.InvalidValidatedPMI);
             }
           }
           
         }
        }
      }
      else
      {
        _logger.InfoFormat("Provisional Invoice is not found: {0}", samplingFormDRecord.ProvisionalInvoiceNumber);
        var ignoreValidation = IgnoreValidationInMigrationPeriodForSamplingFormD(invoice, samplingFormDRecord, TransactionType.SamplingFormAB);
        _logger.InfoFormat("Ignore Validation In Migration Period For SamplingForm D: {0}", ignoreValidation);
        if (!ignoreValidation)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Provisional Invoice Number",
                                                                          samplingFormDRecord.ProvisionalInvoiceNumber,
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormD,
                                                                          SamplingErrorCodes.InvalidLinkedProvisionalInvoice,
                                                                          ErrorStatus.C,
                                                                          samplingFormDRecord,
                                                                          islinkingError: true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Provisional Invoice Number",
                                                                          samplingFormDRecord.ProvisionalInvoiceNumber,
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormD,
                                                                          SamplingErrorCodes.InvalidLinkedProvisionalInvoice,
                                                                          ErrorStatus.W,
                                                                          samplingFormDRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (exceptionDetailsList != null)
      {
        if (invoice.Tolerance != null && !CompareUtil.Compare((samplingFormDRecord.EvaluatedGrossAmount * samplingFormDRecord.IscPercent / 100), samplingFormDRecord.IscAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Evaluated ISC Amount", Convert.ToString(samplingFormDRecord.IscAmount), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, SamplingErrorCodes.InvalidEvaluatedIscAmount, ErrorStatus.X, samplingFormDRecord);

          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        if (invoice.Tolerance != null && !CompareUtil.Compare((samplingFormDRecord.EvaluatedGrossAmount * samplingFormDRecord.UatpPercent / 100), samplingFormDRecord.UatpAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Evaluated UATP Amount", Convert.ToString(samplingFormDRecord.UatpAmount), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, SamplingErrorCodes.InvalidEvaluatedUatpAmount, ErrorStatus.X, samplingFormDRecord);

          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidIscAmount);
      }


      // Validate ticket issuing airline
      isValid = ValidateTicketIssuingAirline(samplingFormDRecord, exceptionDetailsList, invoice, fileName, issuingAirline, isValid, fileSubmissionDate);

      if (Convert.ToDouble(samplingFormDRecord.VatAmount) > 0 && samplingFormDRecord.VatBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", samplingFormDRecord.VatAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (Convert.ToDouble(samplingFormDRecord.TaxAmount) > 0 && samplingFormDRecord.TaxBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount", samplingFormDRecord.TaxAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, ErrorCodes.ZeroTaxBreakdownRecords, ErrorStatus.X, samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validate Total Tax Breakdown amount
      double totalTaxAmount = samplingFormDRecord.TaxBreakdown.Sum(currentRecord => currentRecord.Amount);

      if (invoice.Tolerance != null && !CompareUtil.Compare(samplingFormDRecord.TaxAmount, totalTaxAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(),exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount", Convert.ToString(samplingFormDRecord.TaxAmount),
                                                                        invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      double totalVatAmount = samplingFormDRecord.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);

      if (invoice.Tolerance != null && !CompareUtil.Compare(samplingFormDRecord.VatAmount, ConvertUtil.Round(totalVatAmount, Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount", Convert.ToString(samplingFormDRecord.VatAmount),
                                                invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Tax Breakdowns 
      foreach (var samplingFormDTax in samplingFormDRecord.TaxBreakdown)
      {
        var documentNo = string.Format("{0}-{1}-{2}", samplingFormDRecord.TicketIssuingAirline ?? string.Empty, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber);
        isValid = ValidateParsedTax(samplingFormDTax, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormDTax, fileSubmissionDate, samplingFormDRecord.BatchNumberOfProvisionalInvoice, samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice, documentNo);
      }

      //Validate Vat Breakdowns 
      foreach (var samplingFormDVat in samplingFormDRecord.VatBreakdown)
      {
        var documentNo = string.Format("{0}-{1}-{2}", samplingFormDRecord.TicketIssuingAirline ?? string.Empty, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber);
        isValid = ValidateParsedVat(samplingFormDVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormDVat, fileSubmissionDate, samplingFormDRecord.BatchNumberOfProvisionalInvoice, samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice, documentNo, samplingFormDRecord.SourceCodeId, false, true);
      }

      //Validate Evaluated Net Amount
      if (samplingFormDRecord.EvaluatedNetAmount != ConvertUtil.Round((samplingFormDRecord.EvaluatedGrossAmount + samplingFormDRecord.TaxAmount + samplingFormDRecord.IscAmount + samplingFormDRecord.OtherCommissionAmount
                  + samplingFormDRecord.HandlingFeeAmount + samplingFormDRecord.UatpAmount + samplingFormDRecord.VatAmount), 2))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Evaluated Net Amount", Convert.ToString(samplingFormDRecord.EvaluatedNetAmount),
                                            invoice, fileName, ErrorLevels.ErrorLevelSamplingFormD, ErrorCodes.InvalidEvaluatedNetAmount, ErrorStatus.X, samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // set expiry period for purging.
      //samplingFormDRecord.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.SamplingFormF, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorYes, null);

      return isValid;
    }

    /// <summary>
    /// Validates the ticket issuing airline.
    /// </summary>
    /// <param name="samplingFormDRecord">The sampling form D record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <returns></returns>
    private bool ValidateTicketIssuingAirline(SamplingFormDRecord samplingFormDRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> issuingAirline, bool isValid, DateTime fileSubmissionDate)
    {
      if (!issuingAirline.Keys.Contains(samplingFormDRecord.TicketIssuingAirline))
      {
        if (MemberManager.IsValidAirlineCode(samplingFormDRecord.TicketIssuingAirline))
        {
          issuingAirline.Add(samplingFormDRecord.TicketIssuingAirline, true);
        }
        else
        {
          issuingAirline.Add(samplingFormDRecord.TicketIssuingAirline, false);
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Issuing Airline",
                                                                          samplingFormDRecord.TicketIssuingAirline,
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormD,
                                                                          ErrorCodes.InvalidTicketIssuingAirline, ErrorStatus.X,
                                                                          samplingFormDRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else if (!issuingAirline[samplingFormDRecord.TicketIssuingAirline])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormDRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Issuing Airline",
                                                                        samplingFormDRecord.TicketIssuingAirline,
                                                                        invoice, fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormD,
                                                                        ErrorCodes.InvalidTicketIssuingAirline, ErrorStatus.X,
                                                                        samplingFormDRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    public bool ValidateParsedSamplingFormE(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      if (invoice.SamplingFormEDetails != null)
      {
        //Validate sampling form E Record
        isValid = ValidateParsedSamplingFormERecord(invoice.SamplingFormEDetails, invoice, exceptionDetailsList, fileName, fileSubmissionDate);

        //SCP67958 - SFI 53 Field 'Number of Billing Records' TP Sampling Forms DE ==> missing item
        //Adding No of Billing records to form DE sampling. Copied from Source Code total number of billing records  
        var numberOfBillingRecords = invoice.SourceCodeTotal.Aggregate(0, (current, sourceCodeTotal) => current + sourceCodeTotal.NumberOfBillingRecords);
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && numberOfBillingRecords > 0)
        {
            invoice.SamplingFormEDetails.NumberOfBillingRecords = numberOfBillingRecords;
        }

        //Validate Provisional invoices)
        foreach (var provisionalInvoiceRecordDetail in invoice.ProvisionalInvoiceRecordDetails)
        {
          isValid = ValidateParsedProvisionalInvoiceRecord(provisionalInvoiceRecordDetail, invoice, exceptionDetailsList,
                                                           fileName, fileSubmissionDate);
        }

        //// Validate Groass Total of Samples at form E with Sum of ProvisionalAlfAmt at Form D
        //var totalFormDProvisionalAlfAmt = invoice.SamplingFormDRecord.Sum(samplingFormD => samplingFormD.ProvisionalGrossAlfAmount);

        //if (invoice.Tolerance != null && !CompareUtil.Compare(invoice.SamplingFormEDetails.GrossTotalOfSample, ConvertUtil.Round(Convert.ToDecimal(totalFormDProvisionalAlfAmt), Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        //{
        //  var validationExceptionDetail = CreateValidationExceptionDetail(invoice.SamplingFormEDetails.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of Sample", Convert.ToString(invoice.SamplingFormEDetails.GrossTotalOfSample), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidSampleAdjustedGrossAmount, ErrorStatus.X);
        //  exceptionDetailsList.Add(validationExceptionDetail);
        //  isValid = false;
        //}

      }

      return isValid;

    }

    /// <summary>
    /// Cross rate currency conversion  
    /// </summary>
    /// <param name="provisionalCurrencyId"></param>
    /// <param name="invoiceCurrencyId"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <returns></returns>
    private double GetCurrencyConversionFactor(int? provisionalCurrencyId, int? invoiceCurrencyId, int provisionalBillingYear, int provisionalBillingMonth)
    {
      var exchangeRate = 1.0;
      var provExchangeRate = 0.0;
      var exchangeRateFormD = 0.0;
      try
      {
        // If FormDE Invoice listing currency and provisional invoice listing currency are not same then
        if (provisionalCurrencyId.HasValue && invoiceCurrencyId.HasValue && invoiceCurrencyId.Value != provisionalCurrencyId.Value)
        {
          if (provisionalCurrencyId.Value == (int)BillingCurrency.USD ||
              provisionalCurrencyId.Value == (int)BillingCurrency.GBP ||
              provisionalCurrencyId.Value == (int)BillingCurrency.EUR)
          {
            exchangeRateFormD = ReferenceManager.GetExchangeRate(invoiceCurrencyId.Value,
                                                                 (BillingCurrency)
                                                                 provisionalCurrencyId.Value,
                                                                 provisionalBillingYear,
                                                                 provisionalBillingMonth);

            exchangeRate = ConvertUtil.Round(exchangeRateFormD, Constants.CurrencyConversionFactorDecimalPlaces);
          }
          else if (invoiceCurrencyId.Value == (int)BillingCurrency.USD ||
                   invoiceCurrencyId.Value == (int)BillingCurrency.GBP ||
                   invoiceCurrencyId.Value == (int)BillingCurrency.EUR)
          {
            provExchangeRate = ReferenceManager.GetExchangeRate(provisionalCurrencyId.Value,
                                                                  (BillingCurrency)invoiceCurrencyId.Value,
                                                                  provisionalBillingYear,
                                                                  provisionalBillingMonth);

            exchangeRate = ConvertUtil.Round(1 / provExchangeRate, Constants.CurrencyConversionFactorDecimalPlaces);
          }
          else
          {
            provExchangeRate = ReferenceManager.GetExchangeRate(provisionalCurrencyId.Value,
                                                                  BillingCurrency.USD,
                                                                  provisionalBillingYear,
                                                                  provisionalBillingMonth);

            exchangeRateFormD = ReferenceManager.GetExchangeRate(invoiceCurrencyId.Value,
                                                                 BillingCurrency.USD,
                                                                 provisionalBillingYear,
                                                                 provisionalBillingMonth);

            exchangeRate = ConvertUtil.Round(exchangeRateFormD / provExchangeRate,
                                             Constants.CurrencyConversionFactorDecimalPlaces);
          }
        }
      }
      catch (Exception ex)
      {
        return exchangeRate;
      }
      return exchangeRate;
    }

    /// <summary>
    /// Validates the parsed sampling form E record.
    /// </summary>
    /// <param name="samplingFormEDetail">The sampling form E detail.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private bool ValidateParsedSamplingFormERecord(SamplingFormEDetail samplingFormEDetail, PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;
      double grossTotalOfUniverse = 0;
      double grossTotalOfUAF = 0;
      //Validate Gross Total of Universe

      var invoices = InvoiceRepository.GetInvoicesWithTotal(invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, null, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormAB);
      if (invoices.Count() > 0)
      {
        grossTotalOfUniverse = ConvertUtil.Round((from provisionalInvoice in invoices
                                                  let currencyConversionFactor = GetCurrencyConversionFactor(provisionalInvoice.ListingCurrencyId, invoice.ListingCurrencyId, provisionalInvoice.BillingYear, provisionalInvoice.BillingMonth)
                                                  select Convert.ToDouble(provisionalInvoice.InvoiceTotalRecord.TotalGrossValue) * currencyConversionFactor).Sum(), Constants.PaxDecimalPlaces);

        if (invoice.Tolerance != null && !CompareUtil.Compare(samplingFormEDetail.GrossTotalOfUniverse, Convert.ToDecimal(grossTotalOfUniverse), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of Universe", Convert.ToString(samplingFormEDetail.GrossTotalOfUniverse), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidFormAbGrossTotalOfUniverse, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if (!invoice.IgnoreValidationInMigrationPeriodFormAb)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of Universe", string.Format("{0}-{1}", invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.NoLinkedProvisionalInvoiceForProvBillingPeriod, ErrorStatus.X, islinkingError: true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of Universe", string.Format("{0}-{1}", invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.NoLinkedProvisionalInvoiceForProvBillingPeriod, ErrorStatus.W);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      
      var samplingFormCList = SamplingFormCRepository.GetSamplingFormCList(invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, invoice.BillingMemberId,(int)InvoiceStatusType.Presented, invoice.BilledMemberId);

      if (samplingFormCList.Count > 0)
      {
        grossTotalOfUAF = ConvertUtil.Round((from samplingFormC in samplingFormCList
                                             let currencyConversionFactor = GetCurrencyConversionFactor(samplingFormC.ListingCurrencyId, invoice.ListingCurrencyId, samplingFormC.ProvisionalBillingYear, samplingFormC.ProvisionalBillingMonth)
                           select Convert.ToDouble(samplingFormC.TotalGrossAmountAlf)*currencyConversionFactor).Sum(), Constants.PaxDecimalPlaces);

        if (invoice.Tolerance != null && !CompareUtil.Compare(samplingFormEDetail.GrossTotalOfUaf, Convert.ToDecimal(grossTotalOfUAF), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of UAF", Convert.ToString(samplingFormEDetail.GrossTotalOfUaf), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidFormCGrossTotalOfUniverse, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        // Unable to validate GrossTotalOfUaf as Form AB present in SIS but Form C not present in SIS.
        if (invoices.Count() > 0 && samplingFormCList.Count() == 0 && samplingFormEDetail.GrossTotalOfUaf != 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of UAF", Convert.ToString(samplingFormEDetail.GrossTotalOfUaf), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.UnableToValidateGrossUafAsFormCNotFound, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }// End if
        //If amount is zero and no form c present in the database then there will not be any error.
        else if (samplingFormEDetail.GrossTotalOfUaf == 0)
        {
          isValid = true;
        }
        else if (!invoice.IgnoreValidationInMigrationPeriodFormAb)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of UAF", Convert.ToString(samplingFormEDetail.GrossTotalOfUaf), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidFormCGrossTotalOfUniverse, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Gross Total of UAF", Convert.ToString(samplingFormEDetail.GrossTotalOfUaf), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidFormCGrossTotalOfUniverse, ErrorStatus.W);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        

      }

      if (invoice.Tolerance != null)
      {
        //validate Universe Adjusted Gross Amount
        if (!CompareUtil.Compare(samplingFormEDetail.UniverseAdjustedGrossAmount, samplingFormEDetail.GrossTotalOfUniverse - samplingFormEDetail.GrossTotalOfUaf, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Universe Adjusted Gross Amount", Convert.ToString(samplingFormEDetail.UniverseAdjustedGrossAmount), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidUniverseAdjustedGrossAmount, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;

        }

        //validate Sample Adjusted Gross Amount
        if (!CompareUtil.Compare(samplingFormEDetail.SampleAdjustedGrossAmount, samplingFormEDetail.GrossTotalOfSample - samplingFormEDetail.GrossTotalOfUafSampleCoupon, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sample Adjusted Gross Amount", Convert.ToString(samplingFormEDetail.SampleAdjustedGrossAmount), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidSampleAdjustedGrossAmount, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //validate Sampling Constant (I/II)
        if ((samplingFormEDetail.SampleAdjustedGrossAmount == 0 && samplingFormEDetail.SamplingConstant != 0) || (samplingFormEDetail.SampleAdjustedGrossAmount != 0 && !CompareUtil.Compare(samplingFormEDetail.SamplingConstant, Convert.ToDouble(samplingFormEDetail.UniverseAdjustedGrossAmount / samplingFormEDetail.SampleAdjustedGrossAmount), invoice.Tolerance.RoundingTolerance, Constants.SamplingConstantDecimalPlaces)))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sampling Constant (I/II)", Convert.ToString(samplingFormEDetail.SamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //validate Totals of Gross Amounts x Sampling Constant
        decimal sourceCodeGross = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalGrossValue);
        if (!CompareUtil.Compare(samplingFormEDetail.TotalOfGrossAmtXSamplingConstant, sourceCodeGross * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Totals of Gross Amounts x Sampling Constant", Convert.ToString(samplingFormEDetail.TotalOfGrossAmtXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidTotalOfGrossAmtXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //validate Total of ISC Amounts x Sampling Constant
        decimal sourceCodeIscTotal = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalIscAmount);
        if (!CompareUtil.Compare(samplingFormEDetail.TotalOfIscAmtXSamplingConstant, sourceCodeIscTotal * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Totals of ISC Amounts x Sampling Constant", Convert.ToString(samplingFormEDetail.TotalOfIscAmtXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidTotalOfIscAmtXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //validate Total of Other Commission Amounts x Sampling Constant
        decimal sourceCodeOtherCommissionTotal = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalOtherCommission);
        if (!CompareUtil.Compare(samplingFormEDetail.TotalOfOtherCommissionAmtXSamplingConstant, sourceCodeOtherCommissionTotal * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total of Other Commission Amounts x Sampling Constant", Convert.ToString(samplingFormEDetail.TotalOfOtherCommissionAmtXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidTotalOfOtherCommissionAmtXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }


        //validate UATP Coupon Totals x Sampling Constant
        decimal sourceCodeUatpTotal = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalUatpAmount);

        if (!CompareUtil.Compare(samplingFormEDetail.UatpCouponTotalXSamplingConstant, sourceCodeUatpTotal * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Coupon Totals x Sampling Constant", Convert.ToString(samplingFormEDetail.UatpCouponTotalXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidUatpCouponTotalXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }


        //validate Handling Fee Totals X Sampling constant 
        double sourceHandlingFeeTotal = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, double>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalHandlingFee);

        if (!CompareUtil.Compare(samplingFormEDetail.HandlingFeeTotalAmtXSamplingConstant, sourceHandlingFeeTotal * samplingFormEDetail.SamplingConstant, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Totals X Sampling constant ", Convert.ToString(samplingFormEDetail.HandlingFeeTotalAmtXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidHandlingFeeTotalAmtXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }


        //validate Tax Coupon Totals x Sampling Constant
        decimal sourceTaxCouponTotals = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalTaxAmount);

        if (!CompareUtil.Compare(samplingFormEDetail.TaxCouponTotalsXSamplingConstant, sourceTaxCouponTotals * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Coupon Totals x Sampling Constant ", Convert.ToString(samplingFormEDetail.TaxCouponTotalsXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidTaxCouponTotalsXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //validate VAT Coupon Totals x Sampling Constant
        decimal sourcevatCouponTotals = invoice.SourceCodeTotal.Aggregate<SourceCodeTotal, decimal>(0, (current, sourceCodeTotal) => current + sourceCodeTotal.TotalVatAmount);

        if (!CompareUtil.Compare(samplingFormEDetail.VatCouponTotalsXSamplingConstant, sourcevatCouponTotals * Convert.ToDecimal(samplingFormEDetail.SamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "VAT Coupon Totals x Sampling Constant", Convert.ToString(samplingFormEDetail.VatCouponTotalsXSamplingConstant), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidVatCouponTotalsXSamplingConstant, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }


        //validate Net Amount Due 
        var netAmountDue = samplingFormEDetail.TotalOfGrossAmtXSamplingConstant + samplingFormEDetail.TotalOfIscAmtXSamplingConstant + samplingFormEDetail.TotalOfOtherCommissionAmtXSamplingConstant + samplingFormEDetail.UatpCouponTotalXSamplingConstant + Convert.ToDecimal(samplingFormEDetail.HandlingFeeTotalAmtXSamplingConstant) + samplingFormEDetail.TaxCouponTotalsXSamplingConstant + samplingFormEDetail.VatCouponTotalsXSamplingConstant;
        if (!CompareUtil.Compare(samplingFormEDetail.NetAmountDue, netAmountDue, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Amount Due", Convert.ToString(samplingFormEDetail.NetAmountDue), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidNetAmountDue, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //CMP#648: Convert Exchange rate into nullable field.
        //validate Net Amount Due in Currency of Billing
        var netAmountDueBillingCurrency = netAmountDue / (invoice.ExchangeRate.HasValue? invoice.ExchangeRate.Value : (decimal?)null);
        if (!CompareUtil.Compare(samplingFormEDetail.NetAmountDueInCurrencyOfBilling, netAmountDueBillingCurrency.HasValue ? netAmountDueBillingCurrency.Value : 0.0M, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Amount Due in Currency of Billing", Convert.ToString(samplingFormEDetail.NetAmountDueInCurrencyOfBilling), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidNetAmountDueInCurrencyOfBilling, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //Validate total amount form B

        var provInvoices = InvoiceRepository.GetInvoicesWithTotal(invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, null, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormAB);

        if(provInvoices != null && provInvoices.Count > 0)
        {
          //decimal totalAmountFormB = invoice.ProvisionalInvoiceRecordDetails.Aggregate<ProvisionalInvoiceRecordDetail, decimal>(0, (current, provisionalInvoiceRecordDetail) => current + provisionalInvoiceRecordDetail.InvoiceBillingAmount);
          decimal totalAmountFormB = Convert.ToDecimal(ConvertUtil.Round((from provisionalInvoice in provInvoices
                                                                          let currencyConversionFactor = GetCurrencyConversionFactor(provisionalInvoice.ListingCurrencyId, invoice.ListingCurrencyId, provisionalInvoice.BillingYear, provisionalInvoice.BillingMonth)
                                                                          select provisionalInvoice.InvoiceTotalRecord.NetTotal * Convert.ToDecimal(currencyConversionFactor)).Sum(), Constants.PaxDecimalPlaces));

        if (!CompareUtil.Compare(samplingFormEDetail.TotalAmountFormB, totalAmountFormB, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Amount Form B", Convert.ToString(samplingFormEDetail.TotalAmountFormB), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidTotalAmountFormB, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        }

        //Validate Net Billed/Credited Amount
        //var netBilledCreditedAmount = samplingFormEDetail.TotalAmountFormB - samplingFormEDetail.NetAmountDueInCurrencyOfBilling;
        //CMP#648: Convert Exchange rate into nullable field.
        var netBilledCreditedAmount = ConvertUtil.Round((samplingFormEDetail.TotalAmountFormB - samplingFormEDetail.NetAmountDue) / (invoice.ExchangeRate.HasValue ? invoice.ExchangeRate.Value : 0.0M), Constants.PaxDecimalPlaces);
        if (!CompareUtil.Compare(samplingFormEDetail.NetBilledCreditedAmount, netBilledCreditedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Billed/Credited Amount", Convert.ToString(samplingFormEDetail.NetBilledCreditedAmount), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.InvalidNetBilledCreditedAmount, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //SCP: 182274 - ISIDEC - Aug P4 Error.
        //Desc: samplingFormEDetail.NetBilledCreditedAmount is positive or zero then invoice type should be IV, otherwise CN.
        if (samplingFormEDetail.NetBilledCreditedAmount < 0)
        {
            if (invoice.InvoiceType != InvoiceType.CreditNote)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Type", EnumList.GetInvoiceTypeDisplayValue(invoice.InvoiceType), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.IncorrectInvoiceTypeForNetBilledOrCreditedAmount, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
        }
        else
        {
            if (invoice.InvoiceType != InvoiceType.Invoice)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Type", EnumList.GetInvoiceTypeDisplayValue(invoice.InvoiceType), invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, SamplingErrorCodes.IncorrectInvoiceTypeForNetBilledOrCreditedAmount, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
        }
      }

      //validate Net Amount Due 
      var numberOfBillingRecords = invoice.SourceCodeTotal.Aggregate(0, (current, sourceCodeTotal) => current + sourceCodeTotal.NumberOfBillingRecords);

      if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && samplingFormEDetail.NumberOfBillingRecords != numberOfBillingRecords)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Number Of Billing Records ",
                       Convert.ToString(samplingFormEDetail.NumberOfBillingRecords),
                       invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, ErrorCodes.InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && samplingFormEDetail.NumberOfBillingRecords != invoice.SourceCodeTotal.Sum(record => record.NumberOfBillingRecords))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total number of billing records", samplingFormEDetail.NumberOfBillingRecords.ToString(),
                                                  invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, ErrorCodes.InvalidTotalNoOfBillingRecordsOfInvoiceTotal, ErrorStatus.X, 0, 99999, 99999);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && samplingFormEDetail.TotalNumberOfRecords != (invoice.SourceCodeTotal.Count + invoice.SourceCodeTotal.Sum(record => record.TotalNumberOfRecords) + invoice.NumberOfChildRecords + samplingFormEDetail.NumberOfChildRecords))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", samplingFormEDetail.TotalNumberOfRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSamplingFormE, ErrorCodes.InvalidTotalNoOfRecordsOfInvoiceTotal, ErrorStatus.X, 0, 99999, 99999);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Vat Breakdowns 
      foreach (var invoiceVat in invoice.SamplingFormEDetailVat)
      {
        if (invoiceVat.VatIdentifierId == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Identifier", invoiceVat.Identifier, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormEVat, ErrorCodes.InvalidVatIdentifier, ErrorStatus.X, 0, 0, 0, string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (string.IsNullOrWhiteSpace(invoiceVat.VatText))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormEDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Text", string.Empty, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormEVat, ErrorCodes.InvalidVatText, ErrorStatus.X, 0, 0, 0, string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      // For Form DE..
      if (invoice.SubmissionMethod==SubmissionMethod.IsXml)
      {

        decimal calculatedInvoiceTotalVat = Convert.ToDecimal(invoice.SamplingFormEDetailVat.Sum(vat=>vat.VatCalculatedAmount));
        if (!CompareUtil.Compare(Convert.ToDecimal(calculatedInvoiceTotalVat), Convert.ToDecimal(samplingFormEDetail.VatCouponTotalsXSamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Summary VAT Breakdown", Convert.ToString(calculatedInvoiceTotalVat), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidInvoiceSummaryVat, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      return isValid;
    }


    /// <summary>
    /// Validates the parsed provisional invoice record.
    /// </summary>
    /// <param name="provInvoice">The prov invoice.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private bool ValidateParsedProvisionalInvoiceRecord(ProvisionalInvoiceRecordDetail provInvoice, PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      // Prov Invoice Billing Amount validation.
      decimal calculatedBillingAmount = 0;

      // Calculate billing amount if ListingToBillingRate provided is not zero.
      if (provInvoice.ListingToBillingRate != 0)
      {
        calculatedBillingAmount = provInvoice.InvoiceListingAmount / provInvoice.ListingToBillingRate;
      }// End if

      // Compare Billing amount provided & billing amount calculated and check if the difference is with in summation tolerence tobe applied.
      if (invoice.Tolerance != null && !CompareUtil.Compare(provInvoice.InvoiceBillingAmount, calculatedBillingAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Billing Amount",
                                                                        Convert.ToString(provInvoice.InvoiceBillingAmount),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice,
                                                                        SamplingErrorCodes.InvalidBillingAmount, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;

      }// End if

      //SCP141359 - Spira 8434 - Files stuck when provisional invoice listing currency is specified numeric
      bool isValidInvoiceListingCurrencyId = false;
      var currencyList = ReferenceManager.GetCurrencyList();
      var currency = (from list in currencyList
                      where list.Id == provInvoice.InvoiceListingCurrencyId
                        select list).FirstOrDefault();
      if (currency != null)
          isValidInvoiceListingCurrencyId = true;
      else
      {
          //In case of IS - IDEC  - error is already thrown, but we need to check if validCurrency or not to decide if we need call GetExchangeRate or not
          if (invoice.SubmissionMethod == SubmissionMethod.IsXml)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Provisional Invoice Listing Currency Id",
                                                                              Convert.ToString(
                                                                                  provInvoice.InvoiceListingCurrencyId),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.
                                                                                  ErrorLevelSamplingFormEProvisionalInvoice,
                                                                              SamplingErrorCodes.
                                                                                  InvalidProvListingCurrency,
                                                                              ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
          }
          isValid = false;
      }
      
      // Prov Invoice ListingToBillingRate validation.
      decimal calExchangeRate = 0;

      // If prov invoice listing and Form D billing currency are same then exchange rate should be 1.
      if (provInvoice.InvoiceListingCurrencyId == invoice.BillingCurrencyId)
      {
        calExchangeRate = 1;
      }// End if

      // if Provisional Invoice Listing Currency  and Currency of Billing of Invoice Header are different and 
      // Settlement Method of Form D = "I" or "A" or "M", the exchange rate used should be as published in the 
      // Five Day Rates Master for the Provisional Invoice Billing Month and Year.
      else if (invoice.BillingCurrency != null && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules))
      {
        //SCP141359 - Spira 8434 - Files stuck when provisional invoice listing currency is specified numeric
        if (isValidInvoiceListingCurrencyId && invoice.ProvisionalBillingYear != 0 && invoice.ProvisionalBillingMonth != 0)
            calExchangeRate = Convert.ToDecimal(ReferenceManager.GetExchangeRate(provInvoice.InvoiceListingCurrencyId, (BillingCurrency)invoice.BillingCurrency, invoice.ProvisionalBillingYear, invoice.ProvisionalBillingMonth));
      }// End if 

      if (calExchangeRate > 0)
      {
        // If calculatedExchangeRate is not equal to  ListingToBillingRate then return a valdation error.
        if (provInvoice.ListingToBillingRate != calExchangeRate)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing to Billing Rate",
                                                                          Convert.ToString(provInvoice.ListingToBillingRate),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice,
                                                                          SamplingErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;

        }// End if

      }// End if
      

      // Get provisional invoice using load strategy.
      var invoiceTobeConsidered = InvoiceRepository.Single(provInvoice.InvoiceNumber, invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, null, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormAB, invoiceStatusId: (int)InvoiceStatusType.Presented);

      if (invoiceTobeConsidered != null)
      {
        if (provInvoice.BillingPeriodNo != invoiceTobeConsidered.BillingPeriod)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Billing Period",
                                                                          Convert.ToString(provInvoice.BillingPeriodNo),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice,
                                                                          ErrorCodes.InvalidBillingPeriod, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;

        }

        if (provInvoice.InvoiceListingCurrencyId != invoiceTobeConsidered.ListingCurrencyId)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Listing Currency",
                                                                          Convert.ToString(provInvoice.InvoiceListingCurrencyId),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice,
                                                                          ErrorCodes.InvalidListingCurrency, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;

        }
      }
      else
      {
        if (!invoice.IgnoreValidationInMigrationPeriodFormAb)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Number", provInvoice.InvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice, SamplingErrorCodes.InvalidLinkedProvisionalInvoice, ErrorStatus.X, islinkingError: true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(provInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Number", provInvoice.InvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice, SamplingErrorCodes.InvalidLinkedProvisionalInvoice, ErrorStatus.W);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      return isValid;
    }


    /// <summary>
    /// Check for ListingToBillingRate for given combination of billing currency, Listing currency 
    /// and Prov billing month is inconsistent with previously captured ListingToBillingRate for same combination
    /// </summary>
    /// <param name="provInvoice"></param>
    /// <returns></returns>
    private bool ValidateProvisionalInvoiceRecord(ProvisionalInvoiceRecordDetail provInvoice)
    {
      var invoiceinDb = InvoiceRepository.Single(id: provInvoice.InvoiceId);

      // Commenting below validation for UAT bug: 3678
      //if (invoiceinDb.ProvisionalBillingYear != provInvoice.InvoiceDate.Year || invoiceinDb.ProvisionalBillingMonth != provInvoice.InvoiceDate.Month)
      //{
      //  throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalMonthYear);
      //}

      if (invoiceinDb.BillingCurrencyId == provInvoice.InvoiceListingCurrencyId)
      {
        if (provInvoice.ListingToBillingRate != 1)
          throw new ISBusinessException(SamplingErrorCodes.InvalidListingToBillingRate);
      }
      var provisionalInvoiceList = ProvisionalInvoiceRepository.Get(record => record.InvoiceId == provInvoice.InvoiceId);
      var repeatedInvoiceList = provisionalInvoiceList.Where(invoice => invoice.InvoiceNumber == provInvoice.InvoiceNumber && invoice.InvoiceDate == provInvoice.InvoiceDate && invoice.BillingPeriodNo == provInvoice.BillingPeriodNo).Select(invoice => invoice.InvoiceNumber).ToArray();

      if (repeatedInvoiceList.Contains(provInvoice.InvoiceNumber))
      {
        throw new ISBusinessException(SamplingErrorCodes.ErrorDuplicateProvisionalInvoiceRecord);
      }

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (ReferenceManager.IsSmiLikeBilateral(invoiceinDb.SettlementMethodId, true))
      {
        var exchangeRateList = provisionalInvoiceList.Where(invoice => invoice.InvoiceListingCurrencyId == provInvoice.InvoiceListingCurrencyId).Select(invoice => invoice.ListingToBillingRate).ToArray();
        if (exchangeRateList.Length == 0)
        {
          return true;
        }

        if (!exchangeRateList.Contains(provInvoice.ListingToBillingRate))
        {
          throw new ISBusinessException(SamplingErrorCodes.ErrorInconsistentListingToBillingRate);
        }
      }


      return true;
    }

    /// <summary>
    /// Updates the form B total amount.
    /// </summary>
    /// <param name="provInvoice">The provisional invoice.</param>
    /// <param name="isDeleted">if set to true [is deleted].</param>
    private void UpdateFormBTotalAmount(ProvisionalInvoiceRecordDetail provInvoice, bool isDeleted)
    {
      var samplingFormE = SamplingFormERepository.Single(formE => formE.Id == provInvoice.InvoiceId);
      if (samplingFormE == null)
      {
        throw new ISBusinessException(ErrorCodes.InvalidFormE);
      }

      // Any addition/deletion/modification of records should update Form E 'Form B Total Amounts'; and any fields in Form E dependent on this field.
      //samplingFormE.TotalAmountFormB = !isDeleted ? samplingFormE.TotalAmountFormB + provInvoice.InvoiceBillingAmount : samplingFormE.TotalAmountFormB - provInvoice.InvoiceBillingAmount;

      var provisionalDetails = ProvisionalInvoiceRepository.Get(pro => pro.InvoiceId == provInvoice.InvoiceId);
      Decimal provNetAmt = 0;

      if (provisionalDetails.Count() > 0)
      {

          foreach (var provisionalInvoiceRecordDetail in provisionalDetails)
          {
              provNetAmt += provisionalInvoiceRecordDetail.InvoiceBillingAmount;
          }

      }
      samplingFormE.TotalAmountFormB = !isDeleted
                       ? provNetAmt + provInvoice.InvoiceBillingAmount
                       : provNetAmt - provInvoice.InvoiceBillingAmount;


      // 'Net Billed/Credited Amount' = Total Amount from Form B (Element 53) - Net Amount Due in Currency of Billing (Element 37).
      samplingFormE.NetBilledCreditedAmount = samplingFormE.TotalAmountFormB - samplingFormE.NetAmountDueInCurrencyOfBilling;
    }

    public List<LinkedCoupon> GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long ticketDocNumber, string issuingAirline)
    {
      var linkedCoupons = CouponRecordRepository.GetFormDLinkedCouponDetails(invoiceId, ticketCouponNumber, ticketDocNumber, issuingAirline);

      return linkedCoupons;
    }

    /// <summary>
    /// Update Form E Details.
    /// </summary>
    /// <param name="invoiceId"> The invoice id.</param>
    public void UpdateFormEDetails(Guid invoiceId)
    {
      SamplingFormERepository.UpdateFormEDetails(invoiceId);
    }

    public IList<SamplingFormDRecord> GetSamplingFormDList(string[] rejectionIdList)
    {
      var couponList = new List<SamplingFormDRecord>();

      foreach (var couponId in rejectionIdList)
      {
        var couponGuid = couponId.ToGuid();
        // Replaced with LoadStrategy call
        couponList.Add(SamplingFormDRepository.Single(formDRecordId: couponGuid));
        // couponList.Add(CouponRecordRepository.Single(coupon => coupon.Id == couponGuid));
      }

      return couponList;
    }

      /// <summary>
      /// Validates parsed the BVC matrix.
      /// </summary>
      /// <param name="samplingFormDRecord">The sampling form D record.</param>
      /// <param name="provisionalBillingCouponRecord"></param>
      /// <param name="currencyConversionFactor"></param>
      /// <param name="roundingTolerence"></param>
      /// <returns></returns>
      private string ValidateParsedBvcMatrix(SamplingFormDRecord samplingFormDRecord, PrimeCoupon provisionalBillingCouponRecord,double currencyConversionFactor, double roundingTolerence)
    {
      if (provisionalBillingCouponRecord != null && provisionalBillingCouponRecord.ValidatedPmi != null)
      {
        var bvcMatrix = BvcMatrixRepository.Single(bvc => bvc.ValidatedPmi.ToUpper() == provisionalBillingCouponRecord.ValidatedPmi.ToUpper() && bvc.IsActive);
        if (bvcMatrix == null) return string.Empty;

        var differenceFields = new StringBuilder(string.Empty);
        //SCPID : 56824 - R2 file Sample Airline 001 Form D --- URGENT
        //if (provisionalBillingCouponRecord.AgreementIndicatorValidated.ToUpper() != AgreementIndicatorValidatedI || provisionalBillingCouponRecord.ValidatedPmi.ToUpper() != ValidatedPmiT)
        //{
        //  return string.Empty;
        //}

        const string comma = ",";

        if (bvcMatrix.IsFareAmount && !CompareUtil.Compare(ConvertUtil.Round(provisionalBillingCouponRecord.CouponGrossValueOrApplicableLocalFare * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.EvaluatedGrossAmount, roundingTolerence, Constants.PaxDecimalPlaces))
        {
          differenceFields.Append(AmountGross).Append(comma);
        }
        
        //SCPID : 94698 - R2 file-UA/016 Sep-2012 provisional
        /*  Comparism should be done after conversion of two fields at same level.*/
        if (bvcMatrix.IsIscPercentage && ConvertUtil.Round(samplingFormDRecord.IscPercent, 3) != ConvertUtil.Round(provisionalBillingCouponRecord.IscPercent, 3))
        {
          differenceFields.Append(IscPercentage).Append(comma);
        }
        if (bvcMatrix.IsUatpPercentage && ConvertUtil.Round(samplingFormDRecord.UatpPercent, 3) != ConvertUtil.Round(provisionalBillingCouponRecord.UatpPercent, 3))
        {
          differenceFields.Append(UatpPercentage).Append(comma);
        }

       if (bvcMatrix.IsHfAmount && !CompareUtil.Compare(ConvertUtil.Round(provisionalBillingCouponRecord.HandlingFeeAmount * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.HandlingFeeAmount, roundingTolerence, Constants.PaxDecimalPlaces))
        {
          differenceFields.Append(HandlingFee).Append(comma);
        }


       if (bvcMatrix.IsTaxAmount && !CompareUtil.Compare(ConvertUtil.Round(provisionalBillingCouponRecord.TaxAmount * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.TaxAmount, roundingTolerence, Constants.PaxDecimalPlaces))
          {
              differenceFields.Append(AmountTax).Append(comma);
          }

          return differenceFields.Length > 0 ? differenceFields.Remove(differenceFields.Length - 1, 1).ToString() : differenceFields.ToString();
      }
      return string.Empty;
    }

    /// <summary>
    /// Validates the BVC matrix.
    /// </summary>
    /// <param name="samplingFormDRecord">The sampling form D record.</param>
    /// <param name="samplingFormDE">The sampling form DE.</param>
    /// <returns></returns>
    private string ValidateBvcMatrix(SamplingFormDRecord samplingFormDRecord, PaxInvoice samplingFormDE)
    {


      var provisionalInvoice = InvoiceRepository.Single(samplingFormDRecord.ProvisionalInvoiceNumber, null, null, null,
                                                        samplingFormDE.BilledMemberId, samplingFormDE.BillingMemberId,
                                                        (int)BillingCode.SamplingFormAB, null,
                                                        (int)InvoiceStatusType.Presented);


      if (provisionalInvoice == null) return string.Empty;

      var couponRecord =
        PrimeCouponRecordRepository.Single(coupon =>
                                           coupon.TicketDocOrFimNumber == samplingFormDRecord.TicketDocNumber &&
                                           coupon.TicketOrFimCouponNumber == samplingFormDRecord.CouponNumber &&
                                           coupon.TicketOrFimIssuingAirline == samplingFormDRecord.TicketIssuingAirline
                                           &&
                                           coupon.BatchSequenceNumber ==
                                           samplingFormDRecord.BatchNumberOfProvisionalInvoice &&
                                           coupon.RecordSequenceWithinBatch ==
                                           samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice &&
                                           coupon.InvoiceId == provisionalInvoice.Id);

      // Check for Coupon record and its property null check. PMI ="T" comparison has been removed  with Coupn validated PMI
      if (couponRecord == null || (couponRecord != null && string.IsNullOrEmpty(couponRecord.ValidatedPmi))
                               || (couponRecord != null && string.IsNullOrEmpty(couponRecord.AgreementIndicatorValidated)) 
                               || (couponRecord.AgreementIndicatorValidated.Trim().ToUpper() != AgreementIndicatorValidatedI))
          return string.Empty;


        var bvcMatrix = BvcMatrixRepository.Single(bvc => bvc.ValidatedPmi.ToUpper() == couponRecord.ValidatedPmi.ToUpper() && bvc.IsActive);

      if (bvcMatrix == null) return string.Empty;

      var differenceFields = new StringBuilder(string.Empty);
      // delimiter


      var currencyConversionFactor = GetCurrencyConversionFactor(provisionalInvoice.ListingCurrencyId, samplingFormDE.ListingCurrencyId, provisionalInvoice.BillingYear, provisionalInvoice.BillingMonth);

      var roundingTolerence = samplingFormDE.Tolerance == null ? 0 : samplingFormDE.Tolerance.RoundingTolerance;
        

      if (bvcMatrix.IsFareAmount && !CompareUtil.Compare(ConvertUtil.Round(couponRecord.CouponGrossValueOrApplicableLocalFare * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.EvaluatedGrossAmount, roundingTolerence, Constants.PaxDecimalPlaces))
      {
        differenceFields.Append(AmountGross).Append(Delimeter);
      }

      //ID : 94698 - R2 file-UA/016 Sep-2012 provisional
      /*  Comparism should be done after conversion of two fields at same level.*/
      if (bvcMatrix.IsIscPercentage && ConvertUtil.Round(samplingFormDRecord.IscPercent, 3) != ConvertUtil.Round(couponRecord.IscPercent, 3))
      {
        differenceFields.Append(IscPercentage).Append(Delimeter);
      }
      if (bvcMatrix.IsUatpPercentage && ConvertUtil.Round(samplingFormDRecord.UatpPercent, 3) != ConvertUtil.Round(couponRecord.UatpPercent, 3))
      {
        differenceFields.Append(UatpPercentage).Append(Delimeter);
      }

      if (bvcMatrix.IsHfAmount && !CompareUtil.Compare(ConvertUtil.Round(couponRecord.HandlingFeeAmount * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.HandlingFeeAmount, roundingTolerence, Constants.PaxDecimalPlaces))
      {
        differenceFields.Append(HandlingFee).Append(Delimeter);
      }
        
      if (bvcMatrix.IsTaxAmount && !CompareUtil.Compare(ConvertUtil.Round(couponRecord.TaxAmount * currencyConversionFactor, Constants.PaxDecimalPlaces), samplingFormDRecord.TaxAmount, roundingTolerence, Constants.PaxDecimalPlaces))
      {
        differenceFields.Append(AmountTax).Append(Delimeter);
      }

      return differenceFields.Length > 0 ? differenceFields.Remove(differenceFields.Length - 1, 1).ToString() : differenceFields.ToString();
    }

    /// <summary>
    /// validate auto calculated amount and percentage like: ISC percentage and ISC amount.
    /// </summary>
    /// <param name="iscpercentage">isc percentage</param>
    /// <param name="evaluatedGrossAmount">evaluated gross amount</param>
    /// <param name="iscAmount">isc amount</param>
    public void ValidateIscPerAndAmt(double evaluatedGrossAmount, double iscpercentage, double iscAmount)
    {
      // SCP225054: Form D entry problem
      // Used the generic Round function of ConvertUtil class to match the rounding done in javascript.
      var expectedIscAmount = ConvertUtil.Round(evaluatedGrossAmount * iscpercentage / 100, Constants.PaxDecimalPlaces);
      if (!(expectedIscAmount.Equals(iscAmount)))
      {
        throw new ISBusinessException(ErrorCodes.InvalidIscAmount);
      }
    }
  }
}
