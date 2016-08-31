using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Core;
using Iata.IS.Core.Smtp;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Calendar.Impl;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Data.SupportingDocuments;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using log4net;

namespace Iata.IS.Business.SupportingDocuments.Impl
{
  /// <summary>
  ///   Manager class for supporting documents related operations
  /// </summary>
  public class SupportingDocumentManager : ISupportingDocumentManager
  {
    #region Member Variables

    public const string SupportingDocumentFolderName = "SUPPDOCS";
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    private Dictionary<Guid, RecordType> dtRecords;
    public ICalendarManager CalendarManager { private get; set; }
    public IRepository<InvoiceOfflineCollectionMetaData> InvoiceOfflineCollectionMetaDataRepository { private get; set; }
    public IMiscInvoiceRepository MiscInvoiceRepository { private get; set; }

    public ISupportingDocumentRepository SupportingDocumentRepository { private get; set; }

    public ISupportingDocumentRepository CargoPayableSupportingDocumentRepository { private get; set; }

    public ISampleDigitManager ValidationSampleDigiteManager { private get; set; }
    public IRepository<SupportingDocSearchResult> SupportingDocSearchResultRepository { get; set; }
    public IBillingMemoAttachmentRepository BillingMemoAttachmentRepository { get; set; }
    public ICreditMemoAttachmentRepository CreditMemoAttachmentRepository { get; set; }
    public IRejectionMemoAttachmentRepository RejectionMemoAttachmentRepository { get; set; }
    public IBillingMemoCouponAttachmentRepository BillingMemoCouponAttachmentRepository { get; set; }
    public ICreditMemoCouponAttachmentRepository CreditMemoCouponAttachmentRepository { get; set; }
    public IRejectionMemoCouponAttachmentRepository RejectionMemoCouponAttachmentRepository { get; set; }
    public ICouponRecordAttachmentRepository CouponRecordAttachmentRepository { get; set; }
    public ISamplingFormCAttachmentRepository SamplingFormCAttachmentRepository { get; set; }
    public ISamplingFormDAttachmentRepository SamplingFormDAttachmentRepository { get; set; }
    public IMiscUatpInvoiceAttachmentRepository MiscUatpInvoiceAttachmentRepository { private get; set; }

    public IReferenceManager ReferenceManager { private get; set; }
    public IMemberManager MemberManager { private get; set; }

    public ICargoBillingMemoAttachmentRepository CargoBillingMemoAttachmentRepository { get; set; }
    public ICargoCreditMemoAttachmentRepository CargoCreditMemoAttachmentRepository { get; set; }
    public ICgoRejectionMemoAttachmentRepository CgoRejectionMemoAttachmentRepository { get; set; }
    public IBMAwbAttachmentRepository BMAwbAttachmentRepository { get; set; }
    public ICargoCreditMemoAwbAttachmentRepository CargoCreditMemoAwbAttachmentRepository { get; set; }
    public IRMAwbAttachmentRepository RMAwbAttachmentRepository { get; set; }
    public ICargoAwbAttachmentRepository CargoAwbAttachmentRepository { get; set; }
    /// <summary>
    /// Sampling formD repository.
    /// </summary>
    public ISamplingFormDRepository SamplingFormDRepository { get; set; }

    public IInvoiceRepository InvoiceRepository { get; set; }

    /// <summary>
    /// Get and Set Prime Coupon record Repository
    /// </summary>
    public IRepository<PrimeCoupon> PrimeCouponRecordRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing memo record repository.
    /// </summary>
    /// <value>The billing memo record repository.</value>
    public IBillingMemoRecordRepository BillingMemoRecordRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing memo coupon breakdown repository.
    /// </summary>
    /// <value>The billing memo coupon breakdown repository.</value>
    public IBillingMemoCouponBreakdownRecordRepository BillingMemoCouponBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo repository.
    /// </summary>
    /// <value>The rejection memo repository.</value>
    public Data.Pax.IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo coupon breakdown repository.
    /// </summary>
    /// <value>The rejection memo coupon breakdown repository.</value>
    public IRMCouponBreakdownRecordRepository RejectionMemoCouponBreakdownRepository { get; set; }

    /// <summary>
    /// The Credit Memo Record repository
    /// </summary>
    /// <value>The Credit Memo Record repository</value>
    public ICreditMemoRecordRepository CreditMemoRepository { get; set; }

    /// <summary>
    /// The Credit Memo Coupon Breakdown Record repository
    /// </summary>
    /// <value>The Credit Memo Coupon Breakdown Record repository.</value>
    public ICreditMemoCouponBreakdownRecordRepository CreditMemoCouponBreakdownRecordRepository { get; set; }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "supportingDocumentRepository"></param>
    public SupportingDocumentManager(ISupportingDocumentRepository supportingDocumentRepository)
    {
      SupportingDocumentRepository = supportingDocumentRepository;
      CargoPayableSupportingDocumentRepository = supportingDocumentRepository;
    }

    #endregion

    #region Public methods

    /// <summary>
    ///   This method gets all the records for given search criteria and for each record, builds the list of attachments. 
    ///   It also adds the attachments to db
    /// </summary>
    /// <param name = "recordSearchCriteria">record search criteria</param>
    /// <param name = "extractedDirectory">batch file extracted path</param>
    /// <param name="isInputFile"></param>
    /// <returns>true if success</returns>
    public bool ProcessAttachments(IEnumerable<RecordSearchCriteria> recordSearchCriteria, string extractedDirectory, IsInputFile isInputFile)
    {
      dtRecords = new Dictionary<Guid, RecordType>();

      Logger.Debug("Batch File processing started.");

      if (recordSearchCriteria.Count() <= 0)
      {
        Logger.Info("No invoice, memo or coupon records found to attach the documents.");

        //Delete temporary folder where zip file is extracted.
        DeleteDocumentsFromSourceDirectory(extractedDirectory);

        return true;
      }
     
      var skipDuplicateCheck = false;
      var billingMemberId = MemberManager.GetMemberId(recordSearchCriteria.First().BillingMemberCode.PadLeft(3, '0'));

      var fileServer = ReferenceManager.GetActiveUnlinkedDocumentsServer(); ;
      foreach (var criteria in recordSearchCriteria)
      {
        if (criteria.IsFormC)
        {
          if (criteria.BillingYear != null)
          {
            var billingYear = criteria.BillingYear.Value.ToString().Length == 2 ? "20" + criteria.BillingYear.Value.ToString() : criteria.BillingYear.Value.ToString();
            if (criteria.ClearanceMonth != null)
            {
              var billingMonth = billingYear + criteria.ClearanceMonth.Value.ToString().PadLeft(2, '0'); //"201010";

              // Check file is received before the announcement of sample digit.
              if (isInputFile != null)
              {
                var sampleDigitList = ValidationSampleDigiteManager.GetSampleDigitList(billingMonth);
                if (sampleDigitList.Count > 0 && sampleDigitList.Find(sampleDigit => sampleDigit.DigitAnnouncementDateTime < isInputFile.ReceivedDate) != null)
                {
                  Logger.InfoFormat("Attachment is ignored as,Sampling Digit is announced for given clearence month [BillingMonth: {0}]", billingMonth);
                  continue;
                }
                sampleDigitList.Clear();
                sampleDigitList = null;
              }
              else
              {
                Logger.Info("isInputFile is null");
              }
            }
          }
        }

        //Set the billing member id for corresponding member numeric code coming from search criteria.
        criteria.BillingMemberId = billingMemberId;

        //Set the BilledMemberId for corresponding member numeric code passed through folder structure of batch file
        criteria.BilledMemberId = MemberManager.GetMemberId(criteria.BilledMemberCode.PadLeft(3, '0'));

        // In case of Form D get original invoice number from Provisional invoice number.
        if (criteria.IsFormD)
        {
          // Get invoice queryable object.
          //SCP255391: FW: Supporting Documents missing for SA Form
          var invoices = InvoiceRepository.GetAll().Where(invoice=> invoice.BillingMemberId == criteria.BillingMemberId && (invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling));

          // Get sampling form D queryable object for matching search criteria.
          var formDRecords = SamplingFormDRepository.Get(
            formD => formD.ProvisionalInvoiceNumber.ToUpper() == criteria.InvoiceNumber.ToUpper()
                     && formD.BatchNumberOfProvisionalInvoice == criteria.BatchNumber &&
                     formD.RecordSeqNumberOfProvisionalInvoice == criteria.SequenceNumber);

          // Get invoice number from provisional invoice number using join.
          //SCP255391: FW: Supporting Documents missing for SA Form
          var formDEinv =
            formDRecords.Join(invoices, formD => formD.InvoiceId, inv => inv.Id, (formD, paxInvoice) => new { paxInvoice.InvoiceNumber, paxInvoice.BillingPeriod, paxInvoice.BillingMonth, paxInvoice.BillingYear }).FirstOrDefault();

          if (formDEinv != null)
          {
            DateTime eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                    formDEinv.BillingYear, formDEinv.BillingMonth,
                                    formDEinv.BillingPeriod);

            if (DateTime.UtcNow > eventTime)
            {
                Logger.Info("Supporting document deadline is over.");
            }

            //SCP255391: FW: Supporting Documents missing for SA Form
            // Use original invoice number in case of Form D invoice.
            criteria.FormDInvoiceNumber = formDEinv.InvoiceNumber;
            criteria.FormDEBillingPeriod = formDEinv.BillingPeriod;
            criteria.FormDEBillingMonth = formDEinv.BillingMonth;
            criteria.FormDEBillingYear = formDEinv.BillingYear;
          }
        }

        var supportingDocumentRecords = GetRecordListWithAttachments(criteria);

        Logger.Debug("Fetched records for given search criteria [for Invoice Number: " + criteria.InvoiceNumber + "]");

        if (supportingDocumentRecords != null && supportingDocumentRecords.Count > 0)
        {
          //check if only record w/o attachment
          if (supportingDocumentRecords.Count == 1 && string.IsNullOrEmpty(supportingDocumentRecords[0].AttachmentFileName))
          {
            skipDuplicateCheck = true;
          }

          var recordType = (RecordType)Enum.Parse(typeof(RecordType), supportingDocumentRecords[0].RecordType);
          var recordId = supportingDocumentRecords[0].RecordId;
          //SCP255391: FW: Supporting Documents missing for SA Form
          if (criteria.IsFormD)
            criteria.FormDEBillingYear = supportingDocumentRecords[0].BillingYear;
          else
          criteria.BillingYear = supportingDocumentRecords[0].BillingYear;


          foreach (var file in criteria.Files)
          {
            if (!skipDuplicateCheck && IsDuplicate(supportingDocumentRecords, Path.GetFileName(file)))
            {
              Logger.InfoFormat("Duplicate file found! Attachment is added  to Isa and deleted from source path. [File: {0}]", file);
              //continue;
              //TODO: If duplicate attachment is to be moved to unlinked doc area, please uncomment below line and uncomment above one line of code (continue)

              if (ChkDuplicateUnlinkedAttachment(criteria, skipDuplicateCheck, file))
              {
                continue;
              }
              BuildUnlinkedDocument(Path.Combine(extractedDirectory, file), criteria, fileServer);
            }
            else
            {
              //build the attachment
              if(!string.IsNullOrEmpty(BuildAttachment(recordType, recordId, Path.Combine(extractedDirectory, file), Path.GetFileName(file), criteria)))
              {
                BuildUnlinkedDocument(Path.Combine(extractedDirectory, file), criteria, fileServer);
              }
            }
          }

          // Add record details to update No of attachments column for each record in Db
          dtRecords.Add(recordId, recordType);
        }
        else
        {
          foreach (var file in criteria.Files)
          {
            if (ChkDuplicateUnlinkedAttachment(criteria, skipDuplicateCheck, file))
            {
              continue;
            }
            BuildUnlinkedDocument(Path.Combine(extractedDirectory, file), criteria, fileServer);
          }
        }
      }

      //Commit the transaction for all the attachment repositories
      UnitOfWork.CommitDefault();



      //Delete temporary folder where zip file is extracted.
      DeleteDocumentsFromSourceDirectory(extractedDirectory);

      return true;
    }

    /// <summary>
    ///   To check wheather the given document in duplicate in unlink area.
    /// </summary>
    /// <param name = "criteria">The criteria.</param>
    /// <param name = "skipDuplicateCheck">if set to <c>true</c> [skip duplicate check].</param>
    /// <param name = "file">The file.</param>
    /// <returns></returns>
    private bool ChkDuplicateUnlinkedAttachment(RecordSearchCriteria criteria, bool skipDuplicateCheck, string file)
    {
      var isDuplicate = false;
      var unlinkedSupportingDocuments =
        SupportingDocumentRepository.Get(
          unlinkedDoc =>
          unlinkedDoc.BillingMemberId == criteria.BillingMemberId && unlinkedDoc.BilledMemberId == criteria.BilledMemberId && unlinkedDoc.InvoiceNumber == criteria.InvoiceNumber &&
          unlinkedDoc.BillingYear == criteria.BillingYear && unlinkedDoc.BillingMonth == criteria.ClearanceMonth && unlinkedDoc.PeriodNumber == criteria.ClearancePeriod &&
          unlinkedDoc.BatchNumber == (criteria.BatchNumber.HasValue ? criteria.BatchNumber.Value : 0) &&
          unlinkedDoc.SequenceNumber == (criteria.SequenceNumber.HasValue ? criteria.SequenceNumber.Value : 0) &&
          unlinkedDoc.CouponBreakdownSerialNumber == (criteria.BreakdownSerialNumber.HasValue ? criteria.BreakdownSerialNumber.Value : 0));

      if (!skipDuplicateCheck && IsDuplicateUnlinked(unlinkedSupportingDocuments, Path.GetFileName(file)))
      {
        Logger.InfoFormat("Duplicate file found in unlinked documents! Document ignored and deleted from source path. [File: {0}]", file);
        isDuplicate = true;
      }
      return isDuplicate;
    }

    /// <summary>
    ///   Search the records in db for given search criteria
    /// </summary>
    /// <param name = "recordSearchCriteria">record search criteria</param>
    /// <returns></returns>
    public List<SupportingDocumentRecord> GetRecordListWithAttachments(RecordSearchCriteria recordSearchCriteria)
    {
      var records = SupportingDocumentRepository.GetSupportingDocumentRecords(recordSearchCriteria);

      return records;
    }

    /// <summary>
    ///   Get list of unlinked supporting documents matching given search criteria
    /// </summary>
    /// <param name = "billingYear">The billing year.</param>
    /// <param name = "billingMonth">The billing month.</param>
    /// <param name = "billingPeriod">The billing period.</param>
    /// <param name = "billedMember">The billed member.</param>
    /// <param name = "billingMember">The billing member.</param>
    /// <param name = "invoiceNumber">The invoice number.</param>
    /// <param name = "fileName">Name of the file.</param>
    /// <param name = "submissionDate">The submission date.</param>
    /// <param name = "batchNumber">The batch number.</param>
    /// <param name = "sequenceNumber">The sequence number.</param>
    /// <param name = "breakdownSerialNumber">The breakdown serial number.</param>
    /// <param name = "billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    public List<UnlinkedSupportingDocumentEx> GetUnlinkedSupportingDocuments(int? billingYear,
                                                                             int? billingMonth,
                                                                             int? billingPeriod,
                                                                             int? billedMember,
                                                                             int billingMember,
                                                                             string invoiceNumber,
                                                                             string fileName,
                                                                             DateTime? submissionDate,
                                                                             int? batchNumber = null,
                                                                             int? sequenceNumber = null,
                                                                             int? breakdownSerialNumber = null,
                                                                             int? billingCategoryId = null)
    {
      var recordSearchCriteria = new RecordSearchCriteria();

      recordSearchCriteria.BillingYear = billingYear.HasValue && billingYear != 0 ? billingYear : null;
      recordSearchCriteria.ClearanceMonth = billingMonth.HasValue && billingMonth != 0 ? billingMonth : null;
      recordSearchCriteria.ClearancePeriod = billingPeriod.HasValue && billingPeriod != -1 ? billingPeriod : null;
      recordSearchCriteria.BilledMemberId = billedMember.HasValue && billedMember != 0 ? billedMember : null;
      recordSearchCriteria.BillingMemberId = billingMember;
      recordSearchCriteria.InvoiceNumber = invoiceNumber;
      recordSearchCriteria.OriginalFileName = fileName;
      recordSearchCriteria.SubmissionDate = submissionDate.HasValue && submissionDate != DateTime.MinValue ? submissionDate : null;
      recordSearchCriteria.BatchNumber = batchNumber;
      recordSearchCriteria.SequenceNumber = sequenceNumber;
      recordSearchCriteria.BreakdownSerialNumber = breakdownSerialNumber;
      recordSearchCriteria.BillingCategory = billingCategoryId;

      var unlinkedSupportingDocs = SupportingDocumentRepository.GetUnlinkedSupportingDocuments(recordSearchCriteria);

      return unlinkedSupportingDocs;
    }

    /// <summary>
    ///   Get details of selected unlinked supporting document by id
    /// </summary>
    /// <param name = "id"></param>
    /// <returns></returns>
    public UnlinkedSupportingDocumentEx GetSelectedUnlinkedSupportingDocumentDetails(Guid id)
    {
      var listOfUnlinkedSupportingDocument = new List<UnlinkedSupportingDocument>();
      if (id != null)
      {
        listOfUnlinkedSupportingDocument = SupportingDocumentRepository.Get(m => m.Id == id).ToList();
      }

      var listOfUnlinkedSupportingDocumentEx = new UnlinkedSupportingDocumentEx();
      if (listOfUnlinkedSupportingDocument.Count() > 0)
      {
        listOfUnlinkedSupportingDocumentEx.BatchNumber = listOfUnlinkedSupportingDocument[0].BatchNumber;
        listOfUnlinkedSupportingDocumentEx.BilledMemberId = listOfUnlinkedSupportingDocument[0].BilledMemberId;

        var billedMember = listOfUnlinkedSupportingDocument[0].BilledMemberId > 0 ? MemberManager.GetMember(listOfUnlinkedSupportingDocument[0].BilledMemberId) : null;
        var memberName = billedMember != null ? string.Format("{0}-{1}-{2}", billedMember.MemberCodeAlpha, billedMember.MemberCodeNumeric, billedMember.CommercialName) : string.Empty;
        listOfUnlinkedSupportingDocumentEx.BilledMemberName = memberName;
        listOfUnlinkedSupportingDocumentEx.BilledMemberText = memberName;

        listOfUnlinkedSupportingDocumentEx.BillingCategoryId = listOfUnlinkedSupportingDocument[0].BillingCategoryId;
        listOfUnlinkedSupportingDocumentEx.BillingMemberId = listOfUnlinkedSupportingDocument[0].BillingMemberId;
        listOfUnlinkedSupportingDocumentEx.BillingMonth = listOfUnlinkedSupportingDocument[0].BillingMonth;
        listOfUnlinkedSupportingDocumentEx.BillingYear = listOfUnlinkedSupportingDocument[0].BillingYear;
        listOfUnlinkedSupportingDocumentEx.CouponBreakdownSerialNumber = listOfUnlinkedSupportingDocument[0].CouponBreakdownSerialNumber;
        listOfUnlinkedSupportingDocumentEx.FilePath = listOfUnlinkedSupportingDocument[0].FilePath;
        listOfUnlinkedSupportingDocumentEx.FileServer = listOfUnlinkedSupportingDocument[0].FileServer;
        listOfUnlinkedSupportingDocumentEx.Id = listOfUnlinkedSupportingDocument[0].Id;
        listOfUnlinkedSupportingDocumentEx.InvoiceNumber = listOfUnlinkedSupportingDocument[0].InvoiceNumber;
        listOfUnlinkedSupportingDocumentEx.InvoiceTypeId = listOfUnlinkedSupportingDocument[0].InvoiceTypeId;
        listOfUnlinkedSupportingDocumentEx.IsFormC = listOfUnlinkedSupportingDocument[0].IsFormC;
        listOfUnlinkedSupportingDocumentEx.LastUpdatedBy = listOfUnlinkedSupportingDocument[0].LastUpdatedBy;
        listOfUnlinkedSupportingDocumentEx.LastUpdatedOn = listOfUnlinkedSupportingDocument[0].LastUpdatedOn;
        listOfUnlinkedSupportingDocumentEx.OriginalFileName = listOfUnlinkedSupportingDocument[0].OriginalFileName;
        listOfUnlinkedSupportingDocumentEx.PeriodNumber = listOfUnlinkedSupportingDocument[0].PeriodNumber;
        listOfUnlinkedSupportingDocumentEx.SequenceNumber = listOfUnlinkedSupportingDocument[0].SequenceNumber;
        listOfUnlinkedSupportingDocumentEx.ServerId = listOfUnlinkedSupportingDocument[0].ServerId;
      }

      return listOfUnlinkedSupportingDocumentEx;
    }

    #endregion

    /// <summary>
    /// Fetch search result for supporting document
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public IQueryable<SupportingDocSearchResult> GetSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
    {

      var filteredList = SupportingDocSearchResultRepository.GetAll();

      if (criteria != null)
      {
        if (!string.IsNullOrEmpty(criteria.InvoiceNumber))
        {
          filteredList = filteredList.Where(invoice => invoice.InvoiceNumber.ToUpper().Contains(criteria.InvoiceNumber.ToUpper()));
        }

        // Check if billing year and billing month is passed in search criteria.
        // if passed,then find invoices with specified billing year and billing month.
        if (criteria.BillingYear > 0 && criteria.BillingMonth > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.BillingYear == criteria.BillingYear && invoice.BillingMonth == criteria.BillingMonth);
        }

        // Check if billing period is passed in search criteria.
        // if passed,then find invoices with specified billing period.
        if (criteria.BillingPeriod > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.BillingPeriod == criteria.BillingPeriod);
        }

        // Check if billing member is passed in search criteria
        // if passed,then find invoices with specified billing member ID
        // Note: Do not check whether billing member id is greater than 0 - since IS-OPS users will have 0 member id. They should not see any member invoices.
        filteredList = filteredList.Where(invoice => invoice.BillingMemberId == criteria.BillingMemberId);

        // Check if billed member is passed in search criteria.
        // if passed,then find invoices with specified billed member ID.
        if (criteria.BilledMemberId > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.BilledMemberId == criteria.BilledMemberId);
        }

        if (criteria.SupportingDocumentTypeId > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.SupportingDocTypeId == criteria.SupportingDocumentTypeId);
        }

        if (criteria.SourceCodeId > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.SourceCodeId == criteria.SourceCodeId);
        }

        if (criteria.BatchSequenceNumber > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.BatchSequenceNumber == criteria.BatchSequenceNumber);
        }

        if (criteria.RecordSequenceWithinBatch > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.RecordSequenceWithinBatch == criteria.RecordSequenceWithinBatch);
        }

        if (!string.IsNullOrEmpty(criteria.RMBMCMNumber))
        {
          filteredList = filteredList.Where(invoice => invoice.RMBMCMNumber.ToUpper().Contains(criteria.RMBMCMNumber.ToUpper()));
        }

        if (criteria.TicketDocNumber > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.TicketDocNumber == criteria.TicketDocNumber);
        }

        if (criteria.CouponNumber > 0)
        {
          filteredList = filteredList.Where(invoice => invoice.CouponNumber == criteria.CouponNumber);
        }

        if (criteria.AttachmentIndicatorOriginal != 3)
        {
          filteredList = filteredList.Where(invoice => invoice.AttachmentIndicatorOriginal == criteria.AttachmentIndicatorOriginal);
        }

        if (criteria.ChargeCategoryId > 0)
        {

            string chargecatName = ReferenceManager.GetChargeCategoryName(criteria.ChargeCategoryId);
            filteredList = filteredList.Where(invoice => invoice.ChargeCategory.ToUpper() == chargecatName.ToUpper());
        }

        //CMP #626: TFS: 9287 Attachment indicator original 'Pending' invoices are not viewed in 'Miscellaneous supporting document mismatch' report.
        if (criteria.IsMismatchCases)
        {
          filteredList = filteredList.Where(invoice => ((invoice.AttachmentIndicatorOriginal == 1 || invoice.AttachmentIndicatorOriginal == 2)
              && invoice.AttachmentNumber == 0) || (invoice.AttachmentIndicatorOriginal != 1 && invoice.AttachmentNumber > 0));
        }
          //below code comment on 04.10.11
        //if (!string.IsNullOrEmpty(criteria.BillingCode))
        //{
        //    filteredList = filteredList.Where(i => i.BillingCode == criteria.BillingCode);
        //}

        //if (!string.IsNullOrEmpty(criteria.AWBSerialNumber))
        //{
        //    filteredList = filteredList.Where(i => i.AWBSerialNumber == criteria.AWBSerialNumber);
        //}

        if (!string.IsNullOrEmpty(criteria.CutOffDateEventName) && criteria.SupportingDocumentTypeId != 2)
        {
          filteredList = filteredList.Where(invoice => invoice.CutOffDateEventName.ToUpper().Contains(criteria.CutOffDateEventName.ToUpper()));
        }
      }

      return filteredList;

      //var filteredList = _invoiceRepository.GetAll();
      //return _supportingDocumentRepository.GetSupportingDocumentSearchResult(criteria);
    }

    /// <summary>
    /// Fetch search result for Cargo Supporting Document
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>

    public IQueryable<CargoSupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
    {
        var filteredList = SupportingDocumentRepository.GetCargoSupportingDocumentSearchResult(criteria);
        #region "old code"
        // var filteredList = SupportingDocSearchResultRepository. GetAll();

        //if (criteria != null)
        //{
        //    if (!string.IsNullOrEmpty(criteria.InvoiceNumber))
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.InvoiceNumber.ToUpper().Contains(criteria.InvoiceNumber.ToUpper()));
        //    }

        //    // Check if billing year and billing month is passed in search criteria.
        //    // if passed,then find invoices with specified billing year and billing month.
        //    if (criteria.BillingYear > 0 && criteria.BillingMonth > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.BillingYear == criteria.BillingYear && invoice.BillingMonth == criteria.BillingMonth);
        //    }

        //    // Check if billing period is passed in search criteria.
        //    // if passed,then find invoices with specified billing period.
        //    if (criteria.BillingPeriod > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.BillingPeriod == criteria.BillingPeriod);
        //    }

        //    // Check if billing member is passed in search criteria
        //    // if passed,then find invoices with specified billing member ID
        //    // Note: Do not check whether billing member id is greater than 0 - since IS-OPS users will have 0 member id. They should not see any member invoices.
        //    filteredList = filteredList.Where(invoice => invoice.BillingMemberId == criteria.BillingMemberId);

        //    // Check if billed member is passed in search criteria.
        //    // if passed,then find invoices with specified billed member ID.
        //    if (criteria.BilledMemberId > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.BilledMemberId == criteria.BilledMemberId);
        //    }

        //    if (criteria.SupportingDocumentTypeId > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.SupportingDocTypeId == criteria.SupportingDocumentTypeId);
        //    }

        //    if (!string.IsNullOrEmpty((criteria.BillingCode)))
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.BillingCode.ToUpper().Contains(criteria.BillingCode.ToUpper()));
        //    }

        //    if (criteria.BatchSequenceNumber > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.BatchSequenceNumber == criteria.BatchSequenceNumber);
        //    }

        //    if (criteria.RecordSequenceWithinBatch > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.RecordSequenceWithinBatch == criteria.RecordSequenceWithinBatch);
        //    }

        //    if (!string.IsNullOrEmpty(criteria.RMBMCMNumber))
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.RMBMCMNumber.ToUpper().Contains(criteria.RMBMCMNumber.ToUpper()));
        //    }
        //    if (!string.IsNullOrEmpty((criteria.AWBSerialNumber)))
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.AWBSerialNumber.ToUpper().Contains(criteria.AWBSerialNumber.ToUpper()));
        //    }
            
        //    if (criteria.CouponNumber > 0)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.CouponNumber == criteria.CouponNumber);
        //    }

        //    if (criteria.AttachmentIndicatorOriginal != 3)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.AttachmentIndicatorOriginal == (criteria.AttachmentIndicatorOriginal != 2));
        //    }

        //    if (criteria.IsMismatchCases)
        //        filteredList =
        //          filteredList.Where(
        //            invoice =>
        //            criteria.IsMismatchCases
        //              ? (invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber == 0) || (!invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber > 0)
        //              : (!invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber == 0) || (invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber > 0));

        //    if (!string.IsNullOrEmpty(criteria.BillingCode))
        //    {
        //        filteredList = filteredList.Where(i => i.BillingCode == criteria.BillingCode);
        //    }

        //    if (!string.IsNullOrEmpty(criteria.AWBSerialNumber))
        //    {
        //        filteredList = filteredList.Where(i => i.AWBSerialNumber == criteria.AWBSerialNumber);
        //    }

        //    if (!string.IsNullOrEmpty(criteria.CutOffDateEventName) && criteria.SupportingDocumentTypeId != 2)
        //    {
        //        filteredList = filteredList.Where(invoice => invoice.CutOffDateEventName.ToUpper().Contains(criteria.CutOffDateEventName.ToUpper()));
        //    }
        //}
        #endregion
        return filteredList.AsQueryable();

        //var filteredList = _invoiceRepository.GetAll();
        //return _supportingDocumentRepository.GetSupportingDocumentSearchResult(criteria);
    }

    /// <summary>
    ///   Fetch search result for supporting document
    /// </summary>
    /// <param name = "criteria"></param>
    /// <returns></returns>
    public IList<PayableSupportingDocSearchResult> GetPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
    {
      return SupportingDocumentRepository.GetPayableSupportingDocumentSearchResult(criteria);
    }

    /// <summary>
    ///   Fetch search result for Cargo supporting document
    /// </summary>
    /// <param name = "criteria"></param>
    /// <returns></returns>
    public IList<PayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
    {
        return CargoPayableSupportingDocumentRepository.GetCargoPayableSupportingDocumentSearchResult(criteria);
    }

    /// <summary>
    ///   Populate attachment list for supporting document search result record
    /// </summary>
    /// <param name = "invoiceId">The invoice id.</param>
    /// <param name = "transactionId">The transaction id.</param>
    /// <param name = "recordTypeId">The record type id.</param>
    /// <returns></returns>
    public IList<Attachment> GetAttachmentForSearchEntity(string invoiceId, string transactionId, int recordTypeId)
    {
      var attachments = new List<Attachment>();
      var transactionIdGuid = transactionId.ToGuid();
      var recordType = (SupportingDocRecordType)recordTypeId;
      switch (recordType)
      {
        case SupportingDocRecordType.PrimeCoupon:
          var attachmentCoupon = CouponRecordAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attachmentCoupon);
          break;
        case SupportingDocRecordType.BM:
          var attchmentBM = BillingMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentBM);
          break;
        case SupportingDocRecordType.BMCoupon:
          var attchmentBMCoupon = BillingMemoCouponAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentBMCoupon);
          break;
        case SupportingDocRecordType.RM:
          var attchmentRM = RejectionMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentRM);
          break;
        case SupportingDocRecordType.RMCoupon:
          var attchmentRMCoupon = RejectionMemoCouponAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentRMCoupon);
          break;
        case SupportingDocRecordType.CM:
          var attchmentCM = CreditMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentCM);
          break;
        case SupportingDocRecordType.CMCoupon:
          var attchmentCMCoupon = CreditMemoCouponAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentCMCoupon);
          break;
        case SupportingDocRecordType.FormC:
          var attachmentFormC = SamplingFormCAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attachmentFormC);
          break;
        case SupportingDocRecordType.FormD:
          var attachmentFormD = SamplingFormDAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attachmentFormD);
          break;
      }
      setSerialNoForAttachment(attachments);
      //Set the FileSizeInKb to file size in Kilo bytes.
      attachments = attachments.Select(attachment => { attachment.FileSizeInKb = (attachment.FileSize / 1024M); return attachment; }).ToList();
      return attachments;
    }

    /// <summary>
    /// Get Supporting document offline Collection folder path for given invoice/form c.
    /// Gets path directly using offline collectiom metadata if it exists for given invoice, 
    /// else creates the supporting document offline collection folder structure and then returns path. 
    /// </summary>
    /// <param name="invoice">InvoiceBase object</param>
    /// <returns>Supporting document offline Collection folder path</returns>
    // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
    // Used invoice billing period to get SAN Path.
    // Also modified Method Signature - removed FormC input parameter as it was not getting used.
    public string GetSupportingDocFolderPath(InvoiceBase invoice = null)
    {
      try
      {
        if(invoice != null && (invoice.InvoiceStatusId >= 3 && invoice.InvoiceStatusId <= 6)) 
        {
          var offlineMetaData =
                      InvoiceOfflineCollectionMetaDataRepository.Get(
                          filepath =>
                          filepath.InvoiceNumber ==
                          (invoice.InvoiceNumber) &&
                          filepath.BilledMemberCode == invoice.BilledMember.MemberCodeNumeric &&
                          filepath.BillingMemberCode == invoice.BillingMember.MemberCodeNumeric
                          && filepath.BillingMonth == invoice.BillingMonth
                          && filepath.PeriodNo == invoice.BillingPeriod &&
                          filepath.BillingCategoryId == invoice.BillingCategoryId &&
                          filepath.IsFormC == false && filepath.OfflineCollectionFolderTypeId == 4).
                          FirstOrDefault();

          // Return Supporting document offline Collection folder path obtained from offline collection metadata.
          if(offlineMetaData != null)
          {
            return offlineMetaData.FilePath;  
          }// End if


          // Generate Supporting document offline Collection folder path
          // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
          // To get root path for offline collection pass the invoice billing period in formation to the method.
          var rootPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR, new BillingPeriod(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod));
        
          Logger.InfoFormat("Creating first level folder path [YearMonthPeriod]...");
          var subFolderPath = Path.Combine(rootPath, string.Format("{0}{1}", GetFormattedBillingMonthYear(invoice.BillingMonth, invoice.BillingYear, PaxReportConstants.PaxReportFolderDataFormat), invoice.BillingPeriod.ToString().PadLeft(2, '0')));
          if (!Directory.Exists(subFolderPath)) Directory.CreateDirectory(subFolderPath);
        
          Logger.InfoFormat("Creating third level folder path [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
          subFolderPath = Path.Combine(subFolderPath, string.Format("{0}-{1}-{2}", Enum.GetName(typeof(BillingCategoryType), invoice.BillingCategoryId), invoice.BillingMember.MemberCodeNumeric, invoice.BilledMember.MemberCodeNumeric)).ToUpper();
          if (!Directory.Exists(subFolderPath)) Directory.CreateDirectory(subFolderPath);
        
          Logger.InfoFormat("Creating fourth level folder path[Invoice Number]");
          subFolderPath = Path.Combine(subFolderPath, string.Format("INV-{0}", invoice.InvoiceNumber)).ToUpper();
          if (!Directory.Exists(subFolderPath)) Directory.CreateDirectory(subFolderPath);

          Logger.InfoFormat("Creating fifth level folder path[SUPPDOCS]");
          subFolderPath = Path.Combine(subFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(subFolderPath)) Directory.CreateDirectory(subFolderPath);

          // Return generated Supporting document offline Collection folder path
          return subFolderPath;

        }// End if

      } // End try
      catch (Exception ex)
      {
        Logger.Error(ex); 
      }// End catch

      // Return empty string
      return string.Empty;

    }// End GetSupportingDocFolderPath()

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="basePath"></param>
    /// <param name="folderPath"></param>
    /// <param name="errors"></param>
    /// <param name="logger"></param>
    public void CopyAttachments(Attachment attachment, string basePath, string folderPath)
    {
      var fileExtension = attachment.OriginalFileName.Substring(attachment.OriginalFileName.LastIndexOf('.'));

      var sourceFile = Path.Combine(basePath, attachment.FilePath, attachment.Id.ToString());
      sourceFile = Path.ChangeExtension(sourceFile, fileExtension);
      Logger.Info("Source File:" + sourceFile);

      var destinationFile = Path.Combine(folderPath, attachment.OriginalFileName);
      Logger.Info("Destination File:" + destinationFile);

      if (File.Exists(sourceFile))
      {
        attachment.IsFullPath = true;
        attachment.FilePath = destinationFile;
        FileIo.MoveFile(sourceFile, destinationFile);
      }
      else
      {
        Logger.InfoFormat("File [{0}] not found on server", sourceFile);
      }

    }



    /// <summary>
    ///   Add supporting document
    /// </summary>
    /// <param name = "attachment">The attachment.</param>
    /// <param name = "recordTypeId">The record type id.</param>
    /// <param name = "transactionId">Transaction Id</param>
    /// <returns></returns>
    public Attachment AddSupportingDoc(SupportingDocumentAttachment attachment, int recordTypeId, Guid transactionId)
    {
      string suppDocOfflineCollectionPath = string.Empty;
      
      FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
      
      var recordType = (SupportingDocRecordType)recordTypeId;
      
      switch (recordType)
      {
        case SupportingDocRecordType.PrimeCoupon:
          var attachCoupon = new PrimeCouponAttachment();
          LoadAttachmentDetails(attachment, attachCoupon);
          
          // Get Prime coupon transaction record
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          var couponRecord= PrimeCouponRecordRepository.Get(coupon => coupon.Id == transactionId).SingleOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (couponRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && couponRecord.AttachmentIndicatorOriginal == 0)
          {
            couponRecord.AttachmentIndicatorOriginal = 1;
            PrimeCouponRecordRepository.Update(couponRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create of get supp doc offline collection folder copy them to temp path.
          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: couponRecord.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {

              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   couponRecord.BatchSequenceNumber,
                                                                                   couponRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder. 
              CopyAttachments(attachCoupon, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attachCoupon.IsFullPath;
              attachment.FilePath = attachCoupon.FilePath;
            }// End if
          }// End try
          catch(Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ",ex.Message,ex.StackTrace);
          }// End catch

          CouponRecordAttachmentRepository.Add(attachCoupon);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Commented below code to avoid the SP call. Just return the object
          //attachCoupon = CouponRecordAttachmentRepository.Single(a => a.Id == attachCoupon.Id);
          return attachCoupon;
        case SupportingDocRecordType.BM:
          var attchmentBM = new BillingMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentBM);
          
          // Get Billing memo transaction record
          var billingMemoRecord = BillingMemoRecordRepository.Get(bm => bm.Id == transactionId).FirstOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (billingMemoRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoRecord.AttachmentIndicatorOriginal == 0)
          {
            billingMemoRecord.AttachmentIndicatorOriginal = 1;
            BillingMemoRecordRepository.Update(billingMemoRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create of get supp doc offline collection folder copy them to temp path.
          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: billingMemoRecord.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   billingMemoRecord.BatchSequenceNumber,
                                                                                   billingMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder. 
              CopyAttachments(attchmentBM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentBM.IsFullPath;
              attachment.FilePath = attchmentBM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End catch

          BillingMemoAttachmentRepository.Add(attchmentBM);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Commented below code to avoid the SP call.Just return the object
         // attchmentBM = BillingMemoAttachmentRepository.Single(a => a.Id == attchmentBM.Id);
          return attchmentBM;
        case SupportingDocRecordType.BMCoupon:
          var attchmentBMCoupon = new BMCouponAttachment();
          LoadAttachmentDetails(attachment, attchmentBMCoupon);
          
          // Get Billing memo Coupon transaction record
          var billingMemoCouponRecord = BillingMemoCouponBreakdownRepository.GetBillingMemoWithCoupon(transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (billingMemoCouponRecord.BillingMemo.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoCouponRecord.AttachmentIndicatorOriginal == 0)
          {
            billingMemoCouponRecord.AttachmentIndicatorOriginal = 1;
            BillingMemoCouponBreakdownRepository.Update(billingMemoCouponRecord);
          }


          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create of get supp doc offline collection folder copy them to temp path.
          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: billingMemoCouponRecord.BillingMemo.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath =
                CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                  billingMemoCouponRecord.BillingMemo.
                                                                    BatchSequenceNumber,
                                                                  billingMemoCouponRecord.BillingMemo.
                                                                    RecordSequenceWithinBatch,
                                                                  billingMemoCouponRecord.SerialNo);

              // Move attachment to supp doc folder.
              CopyAttachments(attchmentBMCoupon, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentBMCoupon.IsFullPath;
              attachment.FilePath = attchmentBMCoupon.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End if

          BillingMemoCouponAttachmentRepository.Add(attchmentBMCoupon);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Commented below code to avoid the SP call.Just return the object
          //attchmentBMCoupon = BillingMemoCouponAttachmentRepository.Single(a => a.Id == attchmentBMCoupon.Id);
          return attchmentBMCoupon;
        case SupportingDocRecordType.RM:
          var attchmentRM = new RejectionMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentRM);
          // Get Rejection memo transaction record
          var rejectionMemoRecord = RejectionMemoRepository.Get(rm => rm.Id == transactionId).FirstOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (rejectionMemoRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoRecord.AttachmentIndicatorOriginal == 0)
          {
            rejectionMemoRecord.AttachmentIndicatorOriginal = 1;
            RejectionMemoRepository.Update(rejectionMemoRecord);
          }

          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: rejectionMemoRecord.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   rejectionMemoRecord.
                                                                                     BatchSequenceNumber,
                                                                                   rejectionMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder.
              CopyAttachments(attchmentRM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentRM.IsFullPath;
              attachment.FilePath = attchmentRM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End catch
          
          RejectionMemoAttachmentRepository.Add(attchmentRM);
          UnitOfWork.CommitDefault();

          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Commented below code to avoid the SP call.Just return the object
          //attchmentRM = RejectionMemoAttachmentRepository.Single(a => a.Id == attchmentRM.Id);
          return attchmentRM;
        case SupportingDocRecordType.RMCoupon:
          var attchmentRMCoupon = new RMCouponAttachment();
          LoadAttachmentDetails(attachment, attchmentRMCoupon);
          RejectionMemoCouponAttachmentRepository.Add(attchmentRMCoupon);
          // Get Rejection memo Coupon transaction record
          var rejectionMemoCouponRecord = RejectionMemoCouponBreakdownRepository.GetRmCouponWithRejectionMemoObject(transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (rejectionMemoCouponRecord.RejectionMemoRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoCouponRecord.AttachmentIndicatorOriginal == 0)
          {
            rejectionMemoCouponRecord.AttachmentIndicatorOriginal = 1;
            RejectionMemoCouponBreakdownRepository.Update(rejectionMemoCouponRecord);
          }

          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: rejectionMemoCouponRecord.RejectionMemoRecord.Invoice.Id);
            
            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath =
                CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                  rejectionMemoCouponRecord.RejectionMemoRecord.
                                                                    BatchSequenceNumber,
                                                                  rejectionMemoCouponRecord.RejectionMemoRecord.
                                                                    RecordSequenceWithinBatch,
                                                                  rejectionMemoCouponRecord.SerialNo);

              // Move attachment to supp doc folder.
              CopyAttachments(attchmentRMCoupon, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentRMCoupon.IsFullPath;
              attachment.FilePath = attchmentRMCoupon.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End try

          RejectionMemoCouponAttachmentRepository.Add(attchmentRMCoupon);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
         //attchmentRMCoupon = RejectionMemoCouponAttachmentRepository.Single(a => a.Id == attchmentRMCoupon.Id);
         return attchmentRMCoupon;
        case SupportingDocRecordType.CM:
          var attchmentCM = new CreditMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentCM);
          
          // Get Credit memo transaction record
          var creditMemoRecord = CreditMemoRepository.Get(cm => cm.Id == transactionId).FirstOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (creditMemoRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoRecord.AttachmentIndicatorOriginal == 0)
          {
            creditMemoRecord.AttachmentIndicatorOriginal = 1;
            CreditMemoRepository.Update(creditMemoRecord);
          }

          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: creditMemoRecord.Invoice.Id);
            
            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo folders
              suppDocOfflineCollectionPath = CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   creditMemoRecord.BatchSequenceNumber,
                                                                                   creditMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder.
              CopyAttachments(attchmentCM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentCM.IsFullPath;
              attachment.FilePath = attchmentCM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End catch

          CreditMemoAttachmentRepository.Add(attchmentCM);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //attchmentCM = CreditMemoAttachmentRepository.Single(a => a.Id == attchmentCM.Id);
          return attchmentCM;
        case SupportingDocRecordType.CMCoupon:
          var attchmentCMCoupon = new CMCouponAttachment();
          LoadAttachmentDetails(attachment, attchmentCMCoupon);
          
          // Get Credit memo Coupon transaction record
          var creditMemoCouponRecord = CreditMemoCouponBreakdownRecordRepository.GetCmCouponWithCreditMemoObject(transactionId);  
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (creditMemoCouponRecord.CreditMemoRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoCouponRecord.AttachmentIndicatorOriginal == 0)
          {
            creditMemoCouponRecord.AttachmentIndicatorOriginal = 1;
            CreditMemoCouponBreakdownRecordRepository.Update(creditMemoCouponRecord);
          }

          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: creditMemoCouponRecord.CreditMemoRecord.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath =
                CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                  creditMemoCouponRecord.CreditMemoRecord.
                                                                    BatchSequenceNumber,
                                                                  creditMemoCouponRecord.CreditMemoRecord.
                                                                    RecordSequenceWithinBatch, creditMemoCouponRecord.SerialNo);

              // Move attachment to supp doc folder.
              CopyAttachments(attchmentCMCoupon, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentCMCoupon.IsFullPath;
              attachment.FilePath = attchmentCMCoupon.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End catch

          CreditMemoCouponAttachmentRepository.Add(attchmentCMCoupon);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //attchmentCMCoupon = CreditMemoCouponAttachmentRepository.Single(a => a.Id == attchmentCMCoupon.Id);
          return attchmentCMCoupon;
        case SupportingDocRecordType.FormC:
          var attachmentFormC = new SamplingFormCRecordAttachment();
          LoadAttachmentDetails(attachment, attachmentFormC);
          SamplingFormCAttachmentRepository.Add(attachmentFormC);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //attachmentFormC = SamplingFormCAttachmentRepository.Single(a => a.Id == attachmentFormC.Id);
          return attachmentFormC;
        case SupportingDocRecordType.FormD:
          var attachmentFormD = new SamplingFormDRecordAttachment();
          LoadAttachmentDetails(attachment, attachmentFormD);
          
          // Get Form D transaction record
          var formDRecord = SamplingFormDRepository.Get(formD => formD.Id == transactionId).FirstOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (formDRecord.Invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && formDRecord.AttachmentIndicatorOriginal == 0)
          {
            formDRecord.AttachmentIndicatorOriginal = 1;
            SamplingFormDRepository.Update(formDRecord);
          }// End if

          try
          {
            // SCP128599: Attaching Supporting Documents
            // Get the invoice object with billing and billed member details.
            var invoice = InvoiceRepository.Single(id: formDRecord.Invoice.Id);

            // Get supp doc offline collection folder path.
            suppDocOfflineCollectionPath = GetSupportingDocFolderPath((InvoiceBase)invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create Prov Invoice, BatchNo, SeqNo folders
              suppDocOfflineCollectionPath =
                CreateProvInvoiceNumberBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                       formDRecord.ProvisionalInvoiceNumber,
                                                                       formDRecord.BatchNumberOfProvisionalInvoice,
                                                                       formDRecord.RecordSeqNumberOfProvisionalInvoice);

              // Move attachment to supp doc folder.
              CopyAttachments(attachmentFormD, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attachmentFormD.IsFullPath;
              attachment.FilePath = attachmentFormD.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", ex.Message, ex.StackTrace);
          }// End catch


          SamplingFormDAttachmentRepository.Add(attachmentFormD);
          UnitOfWork.CommitDefault();
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //attachmentFormD = SamplingFormDAttachmentRepository.Single(a => a.Id == attachmentFormD.Id);
          return attachmentFormD;
      }
      return null;
    }

    /// <summary>
    /// Creates suppDocOfflineCollectionPath for given batchNumber, sequenceNumber.
    /// </summary>
    /// <param name="suppDocOfflineCollectionPath">OfflineCollection Supporting Doc folder path</param>
    /// <param name="batchNumber"></param>
    /// <param name="seqNumber"></param>
    /// <returns></returns>
    public string CreateBatchNumberSequenceNumberFolder(string suppDocOfflineCollectionPath, int batchNumber, int seqNumber)
    {
      suppDocOfflineCollectionPath = Path.Combine(suppDocOfflineCollectionPath, string.Format("{0}-{1}", batchNumber.ToString().PadLeft(5, '0'),
                                                  seqNumber.ToString().PadLeft(5, '0')));

      if (!Directory.Exists(suppDocOfflineCollectionPath)) Directory.CreateDirectory(suppDocOfflineCollectionPath);

      return suppDocOfflineCollectionPath;

    }

    /// <summary>
    /// Creates suppDocOfflineCollectionPath for given batchNumber, sequenceNumber and recSeqNumber.
    /// </summary>
    /// <param name="suppDocOfflineCollectionPath">OfflineCollection Supporting Doc folder path</param>
    /// <param name="batchNumber"></param>
    /// <param name="seqNumber"></param>
    /// <param name="recSeqNumber"></param>
    /// <returns></returns>
    public string CreateBatchNumberSequenceNumberRecSeqNumberFolder(string suppDocOfflineCollectionPath, int batchNumber, int seqNumber, int recSeqNumber)
    {
      suppDocOfflineCollectionPath = CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath, batchNumber,seqNumber);

      suppDocOfflineCollectionPath = Path.Combine(suppDocOfflineCollectionPath, recSeqNumber.ToString().PadLeft(5, '0'));

      if (!Directory.Exists(suppDocOfflineCollectionPath)) Directory.CreateDirectory(suppDocOfflineCollectionPath);

      return suppDocOfflineCollectionPath;

    }

    /// <summary>
    /// Creates suppDocOfflineCollectionPath for given prov invoice number, batchNumber and sequenceNumber.
    /// </summary>
    /// <param name="suppDocOfflineCollectionPath">OfflineCollection Supporting Doc folder path</param>
    /// <param name="provInvNumber"></param>
    /// <param name="batchNumber"></param>
    /// <param name="seqNumber"></param>
    /// <returns></returns>
    public string CreateProvInvoiceNumberBatchNumberSequenceNumberFolder(string suppDocOfflineCollectionPath, string provInvNumber, int batchNumber, int seqNumber)
    {
      // 
      suppDocOfflineCollectionPath = Path.Combine(suppDocOfflineCollectionPath, string.Format("{0}-{1}-{2}", provInvNumber, batchNumber.ToString().PadLeft(5, '0'),
                                                  seqNumber.ToString().PadLeft(5, '0')));

      if (!Directory.Exists(suppDocOfflineCollectionPath)) Directory.CreateDirectory(suppDocOfflineCollectionPath);

      return suppDocOfflineCollectionPath;

    }

    /// <summary>
    /// Delete attachment from the physical path if IsFullPath flag is true.
    /// else form the path of the files at temporary location and then delete file from this location.
    /// </summary>
    /// <param name="attachment"></param>
    public void DeleteAttachement(Attachment attachment)
    {
      try
      {
        if (attachment.IsFullPath)
        {
          File.Delete(attachment.FilePath);
        }  
        else
        {
          FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
          var filePath = Path.Combine(fileServer.BasePath, attachment.FilePath);
          
          if (!string.IsNullOrWhiteSpace(filePath))
            File.Delete(filePath);
        }
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Handled Error. Error deleting [{0}]",attachment.FilePath);
        Logger.Error(ex);
      }
      
    }

    /// <summary>
    ///   Delete supporting document record as per record type
    /// </summary>
    /// <param name = "attachmentId"></param>
    /// <param name = "recordTypeId"></param>
    /// <param name = "invoice"></param>
    /// <returns></returns>
    public bool DeleteSupportingDoc(string attachmentId, int recordTypeId, InvoiceBase invoice)
    {
      var recordType = (SupportingDocRecordType)recordTypeId;
      var attachmentIdGuid = attachmentId.ToGuid();
      switch (recordType)
      {
        case SupportingDocRecordType.PrimeCoupon:

          var attachCoupon = CouponRecordAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attachCoupon == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attachCoupon);
          
          // Get prime coupon Id from attachment object
          var primeCouponId = attachCoupon.ParentId;
          CouponRecordAttachmentRepository.Delete(attachCoupon);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var primeAttachmentCount = CouponRecordAttachmentRepository.GetCount(attach => attach.ParentId == primeCouponId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && primeAttachmentCount == 0)
          {
            var transaction = PrimeCouponRecordRepository.Single(coupon => coupon.Id == primeCouponId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              PrimeCouponRecordRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.BM:
          var attchmentBM = BillingMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentBM == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentBM);

          // Get billingMemoId from attachment object
          var billingMemoId = attchmentBM.ParentId;
          BillingMemoAttachmentRepository.Delete(attchmentBM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var billingMemoAttachmentCount = BillingMemoAttachmentRepository.GetCount(attach => attach.ParentId == billingMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoAttachmentCount == 0)
          {
            var transaction = BillingMemoRecordRepository.Get(bm => bm.Id == billingMemoId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              BillingMemoRecordRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.BMCoupon:
          var attchmentBMCoupon = BillingMemoCouponAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentBMCoupon == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentBMCoupon);

          // Get billingMemoCouponId from attachment object
          var billingMemoCouponId = attchmentBMCoupon.ParentId;
          BillingMemoCouponAttachmentRepository.Delete(attchmentBMCoupon);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var billingMemoCpnAttachmentCount = BillingMemoCouponAttachmentRepository.GetCount(attach => attach.ParentId == billingMemoCouponId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoCpnAttachmentCount == 0)
          {
            var transaction = BillingMemoCouponBreakdownRepository.Get(bmCoupon => bmCoupon.Id == billingMemoCouponId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              BillingMemoCouponBreakdownRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.RM:
          var attchmentRM = RejectionMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentRM == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentRM);

          // Get rejectionMemoId from attachment object
          var rejectionMemoId = attchmentRM.ParentId;
          RejectionMemoAttachmentRepository.Delete(attchmentRM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var rejectionMemoAttachmentCount = RejectionMemoAttachmentRepository.GetCount(attach => attach.ParentId == rejectionMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoAttachmentCount == 0)
          {
            var transaction = RejectionMemoRepository.Get(rm => rm.Id == rejectionMemoId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              RejectionMemoRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.RMCoupon:
          var attchmentRMCoupon = RejectionMemoCouponAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentRMCoupon == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentRMCoupon);

          // Get rejectionMemoCouponId from attachment object
          var rejectionMemoCouponId = attchmentRMCoupon.ParentId;
          RejectionMemoCouponAttachmentRepository.Delete(attchmentRMCoupon);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var rejectionMemoCouponAttachmentCount = RejectionMemoCouponAttachmentRepository.GetCount(attach => attach.ParentId == rejectionMemoCouponId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoCouponAttachmentCount == 0)
          {
            var transaction = RejectionMemoCouponBreakdownRepository.Get(rm => rm.Id == rejectionMemoCouponId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              RejectionMemoCouponBreakdownRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.CM:
          var attchmentCM = CreditMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentCM == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentCM);

          // Get creditMemoId from attachment object
          var creditMemoId = attchmentCM.ParentId;
          CreditMemoAttachmentRepository.Delete(attchmentCM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var creditMemoAttachmentCount = CreditMemoAttachmentRepository.GetCount(attach => attach.ParentId == creditMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoAttachmentCount == 0)
          {
            var transaction = CreditMemoRepository.Get(rm => rm.Id == creditMemoId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              CreditMemoRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.CMCoupon:
          var attchmentCMCoupon = CreditMemoCouponAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentCMCoupon == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attchmentCMCoupon);

          // Get creditMemoCouponId from attachment object
          var creditMemoCouponId = attchmentCMCoupon.ParentId;
          CreditMemoCouponAttachmentRepository.Delete(attchmentCMCoupon);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var creditMemoCouponAttachmentCount = CreditMemoCouponAttachmentRepository.GetCount(attach => attach.ParentId == creditMemoCouponId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoCouponAttachmentCount == 0)
          {
            var transaction = CreditMemoCouponBreakdownRecordRepository.Get(rm => rm.Id == creditMemoCouponId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              CreditMemoCouponBreakdownRecordRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

        case SupportingDocRecordType.FormC:
          var attachmentFormC = SamplingFormCAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attachmentFormC == null)
          {
            return false;
          }

          SamplingFormCAttachmentRepository.Delete(attachmentFormC);
          UnitOfWork.CommitDefault();
          return true;

        case SupportingDocRecordType.FormD:
          var attachmentFormD = SamplingFormDAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attachmentFormD == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          DeleteAttachement(attachmentFormD);

          // Get creditMemoCouponId from attachment object
          var formDId = attachmentFormD.ParentId;
          SamplingFormDAttachmentRepository.Delete(attachmentFormD);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var formDAttachmentCount = SamplingFormDAttachmentRepository.GetCount(attach => attach.ParentId == formDId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && formDAttachmentCount == 0)
          {
            var transaction = SamplingFormDRepository.Get(rm => rm.Id == formDId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = 0;
              SamplingFormDRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;
      }

      return false;
    }

    /// <summary>
    ///   Populate attachment detail to download attachment in supporting document
    /// </summary>
    /// <param name = "transactionId">The transaction id.</param>
    /// <param name = "recordTypeId">The record type id.</param>
    /// <returns></returns>
    public Attachment GetSupportingDocumentDetail(string transactionId, int recordTypeId)
    {
      var transactionIdGuid = transactionId.ToGuid();
      var recordType = (SupportingDocRecordType)recordTypeId;
      switch (recordType)
      {
        case SupportingDocRecordType.PrimeCoupon:
          var attachmentCoupon = CouponRecordAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attachmentCoupon;
        case SupportingDocRecordType.BM:
          var attchmentBM = BillingMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentBM;
        case SupportingDocRecordType.BMCoupon:
          var attchmentBMCoupon = BillingMemoCouponAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentBMCoupon;
        case SupportingDocRecordType.RM:
          var attchmentRM = RejectionMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentRM;
        case SupportingDocRecordType.RMCoupon:
          var attchmentRMCoupon = RejectionMemoCouponAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentRMCoupon;
        case SupportingDocRecordType.CM:
          var attchmentCM = CreditMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentCM;
        case SupportingDocRecordType.CMCoupon:
          var attchmentCMCoupon = CreditMemoCouponAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentCMCoupon;
        case SupportingDocRecordType.FormC:
          var attachmentFormC = SamplingFormCAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attachmentFormC;
        case SupportingDocRecordType.FormD:
          var attachmentFormD = SamplingFormDAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attachmentFormD;

      }
      return new PrimeCouponAttachment();
    }

    /// <summary>
    ///   Populate attachment detail to download attachment in supporting document
    /// </summary>
    /// <param name = "fileName">Name of the file.</param>
    /// <param name = "transactionId">The transaction id.</param>
    /// <param name = "recordTypeId">The record type id.</param>
    /// <returns>
    ///   <c>true</c> if [is duplicate file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDuplicateFileName(string fileName, string transactionId, int recordTypeId)
    {
      var transactionIdGuid = transactionId.ToGuid();
      var recordType = (SupportingDocRecordType)recordTypeId;
      switch (recordType)
      {
        case SupportingDocRecordType.PrimeCoupon:
          return CouponRecordAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.BM:
          return BillingMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.BMCoupon:
          return BillingMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.RM:
          return RejectionMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.RMCoupon:
          return RejectionMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.CM:
          return CreditMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.CMCoupon:
          return CreditMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

        case SupportingDocRecordType.FormC:
          return SamplingFormCAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
        case SupportingDocRecordType.FormD:
          return SamplingFormDAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
        case SupportingDocRecordType.Misc:
          return MiscUatpInvoiceAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

      }
      return false;
    }

    #region Private methods

    private static void LoadAttachmentDetails(SupportingDocumentAttachment attach, Attachment attachmentTobeAdded)
    {
      attachmentTobeAdded.Id = attach.Id;
      attachmentTobeAdded.OriginalFileName = attach.OriginalFileName;
      attachmentTobeAdded.FileSize = attach.FileSize;
      attachmentTobeAdded.LastUpdatedBy = attach.LastUpdatedBy;
      attachmentTobeAdded.ServerId = attach.ServerId;
      attachmentTobeAdded.FileServer = attach.FileServer;
      attachmentTobeAdded.FileStatus = attach.FileStatus;
      attachmentTobeAdded.FilePath = attach.FilePath;
      attachmentTobeAdded.ParentId = attach.ParentId;
    }

    /// <summary>
    ///   Set serial no of attachment to display in grid
    /// </summary>
    /// <param name = "attachment"></param>
    private void setSerialNoForAttachment(List<Attachment> attachment)
    {
      var count = 1;
      foreach (var attach in attachment)
      {
        attach.SerialNo = count;
        count++;
      }
    }

    /// <summary>
    ///   Builds the search criteria for unlinked documents
    /// </summary>
    /// <param name = "unlinkedSupportingDocuments"></param>
    /// <returns></returns>
    private List<RecordSearchCriteria> BuildSearchCriteriaForUnlinkedDocuments(List<UnlinkedSupportingDocument> unlinkedSupportingDocuments)
    {
      var recordSearchCriteria = new List<RecordSearchCriteria>();
      RecordSearchCriteria criteria = null;
      //

      var criteriaEntries = new List<RecordSearchCriteria>();

      foreach (var unlinkedSupportingDocument in unlinkedSupportingDocuments)
      {
        if (criteriaEntries.Count == 0 || !IsContainsInCriteriaEntries(unlinkedSupportingDocument, criteriaEntries))
        {
          criteria = BuildCriteria(unlinkedSupportingDocument);
          criteriaEntries.Add(criteria);
          recordSearchCriteria.Add(criteria);
        }
        else
        {
          criteria.FileName = unlinkedSupportingDocument.Id.ToString();
          criteria.OriginalFileName = unlinkedSupportingDocument.OriginalFileName;
        }

        if (recordSearchCriteria.Count > 0)
        {
          recordSearchCriteria[recordSearchCriteria.Count - 1].OriginalFiles.Add(criteria.FileName, criteria.OriginalFileName);
        }
      }

      return recordSearchCriteria;
    }

    /// <summary>
    ///   Check if document already contains in search criteria
    /// </summary>
    /// <param name = "unlinkedSupportingDocument"></param>
    /// <param name = "criteriaEntries"></param>
    /// <returns></returns>
    private static bool IsContainsInCriteriaEntries(UnlinkedSupportingDocument unlinkedSupportingDocument, List<RecordSearchCriteria> criteriaEntries)
    {
      return
        criteriaEntries.Any(
          criteria =>
          criteria.BillingMemberId == unlinkedSupportingDocument.BillingMemberId && criteria.ClearanceMonth == unlinkedSupportingDocument.BillingMonth &&
          criteria.ClearancePeriod == unlinkedSupportingDocument.PeriodNumber && criteria.BilledMemberId == unlinkedSupportingDocument.BilledMemberId &&
          criteria.BillingCategory == unlinkedSupportingDocument.BillingCategoryId && criteria.InvoiceNumber == unlinkedSupportingDocument.InvoiceNumber &&
          criteria.BatchNumber == unlinkedSupportingDocument.BatchNumber && criteria.SequenceNumber == unlinkedSupportingDocument.SequenceNumber &&
          criteria.BreakdownSerialNumber == unlinkedSupportingDocument.CouponBreakdownSerialNumber);
    }

    /// <summary>
    ///   Build search criteria from list of unlinked documents
    /// </summary>
    /// <param name = "unlinkedSupportingDocument"></param>
    /// <returns></returns>
    private RecordSearchCriteria BuildCriteria(UnlinkedSupportingDocument unlinkedSupportingDocument)
    {
      var recordSearchCriteria = new RecordSearchCriteria
                                   {
                                     BillingMemberId = unlinkedSupportingDocument.BillingMemberId,
                                     BilledMemberId = unlinkedSupportingDocument.BilledMemberId,
                                     ClearanceMonth = unlinkedSupportingDocument.BillingMonth,
                                     ClearancePeriod = unlinkedSupportingDocument.PeriodNumber,
                                     BillingCategory = unlinkedSupportingDocument.BillingCategoryId,
                                     InvoiceNumber = unlinkedSupportingDocument.InvoiceNumber,
                                     BatchNumber = unlinkedSupportingDocument.BatchNumber,
                                     SequenceNumber = unlinkedSupportingDocument.SequenceNumber,
                                     BreakdownSerialNumber = unlinkedSupportingDocument.CouponBreakdownSerialNumber,
                                     FileName = unlinkedSupportingDocument.Id.ToString(),
                                     // +Path.GetExtension(unlinkedSupportingDocument.OriginalFileName);
                                     OriginalFileName = unlinkedSupportingDocument.OriginalFileName //.OriginalFileName;
                                   };
      return recordSearchCriteria;
    }

    /// <summary>
    ///   Build unlinked attachment with all necessary information from file and search criteria
    /// </summary>
    /// <param name = "fileName"></param>
    /// <param name = "recordSearchCriteria"></param>
    /// <param name="fileServer"></param>
    private void BuildUnlinkedDocument(string fileName, RecordSearchCriteria recordSearchCriteria, FileServer fileServer)
    {
      if (File.Exists(fileName))
      {
        // Read the file from folder as per record parameters and build the attachment object to add to the collection
        var unlinkedSupportingDocument = PopulateDocument(fileName, recordSearchCriteria, fileServer);
        var destinatinoFileName = unlinkedSupportingDocument.FilePath;

        SupportingDocumentRepository.Add(unlinkedSupportingDocument);

        CopyDocumentsToIsa(fileName, destinatinoFileName, fileServer);
      }
      else
      {
        Logger.InfoFormat("Attachment not found to process unlinked documents! [File: {0}]", fileName);
      }
    }

    /// <summary>
    ///   Populates attachment object with file information
    /// </summary>
    /// <param name = "fileName"></param>
    /// <param name = "criteria"></param>
    /// <param name="fileServer"></param>
    private UnlinkedSupportingDocument PopulateDocument(string fileName, RecordSearchCriteria criteria, FileServer fileServer)
    {
      var unlinkedSupportingDocument = new UnlinkedSupportingDocument();

      //var fileServer = _referenceManager.GetActiveUnlinkedDocumentsServer();

      var fileInfo = new FileInfo(fileName);

      //Get billing year and month from clearance month field
      var clearanceMonth = criteria.ClearanceMonth.ToString();
      var billingYear = 0;
      var billingMonth = 0;
      if (clearanceMonth.Length == 6)
      {
        billingYear = Convert.ToInt32(clearanceMonth.Substring(0, 4));
        billingMonth = Convert.ToInt32(clearanceMonth.Substring(4, 2));
      }
      else if (clearanceMonth.Length > 0 && clearanceMonth.Length <= 2)
      {
        billingMonth = Convert.ToInt32(clearanceMonth);
      }
      if (billingYear == 0)
      {
        billingYear = criteria.BillingYear.HasValue ? criteria.BillingYear.Value : 0;
      }

      unlinkedSupportingDocument.Id = Guid.NewGuid();
      unlinkedSupportingDocument.BillingMemberId = criteria.BillingMemberId;
      unlinkedSupportingDocument.BilledMemberId = criteria.BilledMemberId.HasValue ? criteria.BilledMemberId.Value : 0;
      unlinkedSupportingDocument.InvoiceNumber = criteria.InvoiceNumber;
      unlinkedSupportingDocument.BillingMonth = billingMonth;
      unlinkedSupportingDocument.BillingYear = billingYear;
      unlinkedSupportingDocument.PeriodNumber = criteria.ClearancePeriod.HasValue ? criteria.ClearancePeriod.Value : 0;
      unlinkedSupportingDocument.BatchNumber = criteria.BatchNumber.HasValue ? criteria.BatchNumber.Value : 0;
      unlinkedSupportingDocument.SequenceNumber = criteria.SequenceNumber.HasValue ? criteria.SequenceNumber.Value : 0;
      unlinkedSupportingDocument.CouponBreakdownSerialNumber = criteria.BreakdownSerialNumber.HasValue ? criteria.BreakdownSerialNumber.Value : 0;

      unlinkedSupportingDocument.OriginalFileName = Path.GetFileName(fileName);
      unlinkedSupportingDocument.FilePath = string.Format("{0}{1}", unlinkedSupportingDocument.Id, fileInfo.Extension);
      unlinkedSupportingDocument.ServerId = fileServer.ServerId;

      unlinkedSupportingDocument.InvoiceTypeId = (int)InvoiceType.Invoice;
      unlinkedSupportingDocument.BillingCategoryId = criteria.BillingCategory.HasValue ? criteria.BillingCategory.Value : 0;
      unlinkedSupportingDocument.IsFormC = criteria.IsFormC ? "Y" : "N";

      unlinkedSupportingDocument.LastUpdatedBy = criteria.BillingMemberId;
      unlinkedSupportingDocument.LastUpdatedOn = DateTime.UtcNow;

      return unlinkedSupportingDocument;
    }

    /// <summary>
    ///   Populates attachment object with file information
    /// </summary>
    /// <param name = "attachment"></param>
    /// <param name = "fileName"></param>
    /// <param name = "originalFileName"></param>
    /// <param name = "billingMemberId"></param>
    private void PopulateAttachment(Attachment attachment, string fileName, string originalFileName, int billingMemberId)
    {
      var fileInfo = new FileInfo(fileName);

      var fileServer = ReferenceManager.GetActiveAttachmentServer();

      attachment.Id = Guid.NewGuid();
      attachment.FileSize = fileInfo.Length;
      attachment.FileStatus = FileStatusType.Received;
      attachment.ServerId = fileServer.ServerId;

      //LastUpdatedBy value 5 is fixed for the supporting document
      attachment.LastUpdatedBy = 5;
      attachment.LastUpdatedOn = DateTime.UtcNow;

      attachment.FileTypeId = 0;
      attachment.OriginalFileName = Path.GetFileName(string.IsNullOrEmpty(originalFileName) ? fileName : originalFileName);
      attachment.IsFullPath = true;

      fileServer = null;
    }

    /// <summary>
    ///   Builds the list of attachments based of record type and adds to respective repositories
    /// </summary>
    /// <param name = "recordType"></param>
    /// <param name = "recordId"></param>
    /// <param name = "fileName"></param>
    /// <param name = "originalFileName"></param>
    /// <param name = "criteria"></param>
    // SCP282352: Problem with attachments [Added loggs in BuildAttachment method.]
    private string BuildAttachment(RecordType recordType, Guid recordId, string fileName, string originalFileName, RecordSearchCriteria criteria, bool skipSuppDocLinkingDeadlineCheck = false)
    {
      if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
      {
        //Logger.InfoFormat("Attachment not found to attach! [File: {0}]", fileName);o
        return ("Attachment file not found to attach.");
      }
      var destinationFileName = string.Empty;
      //var filePath = string.Format("{0}_{1}_{2}", criteria.BilledMemberId, criteria.BillingYear, criteria.ClearanceMonth);
      //var filePath = GetSupportingDocPathFromOfflineMetaData(criteria);
      // SCP162502  Form C - AC OAR Jul P3 failure - No alert received
      var filePath = recordType == RecordType.FormC ? GetSupportingDocPathFromOfflineMetaData(criteria, recordId, skipSuppDocLinkingDeadlineCheck) : GetSupportingDocPathFromOfflineMetaData(criteria);
	

      if (!string.IsNullOrEmpty(filePath))
      {
        if (!Directory.Exists(filePath))
        {
          try
          {
            Directory.CreateDirectory(filePath);
          }
          catch(Exception ex)
          {

            Logger.ErrorFormat(
              "Supporting Doc Linking Path: {0} not found. Attempted to create the directory threw exception. Error: {1}, Stack Trace: {2}",
              filePath, ex.Message, ex.StackTrace);

            return "Supporting documents path for invoice could not be created.";
          }
        }
        filePath = Path.Combine(filePath, Path.GetFileName(originalFileName));
        switch (recordType)
        {
          case RecordType.BillingMemo:

            

                if(criteria.BillingCategory == (int)BillingCategoryType.Pax)
                {
                     var billingMemoAttachment = new BillingMemoAttachment { ParentId = recordId, FilePath = filePath };

                     PopulateAttachment(billingMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                    destinationFileName = billingMemoAttachment.Id + Path.GetExtension(fileName);

                    if (BillingMemoAttachmentRepository == null)
                    {
                        BillingMemoAttachmentRepository = new BillingMemoAttachmentRepository();
                    }

                    BillingMemoAttachmentRepository.Add(billingMemoAttachment);
                    Logger.InfoFormat("Added entry in DB table for Passenger Billing Memo Attachment: {0}", billingMemoAttachment.OriginalFileName);
                }
                else
                {
                    var billingMemoAttachment = new CargoBillingMemoAttachment { ParentId = recordId, FilePath = filePath };

                     PopulateAttachment(billingMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                    destinationFileName = billingMemoAttachment.Id + Path.GetExtension(fileName);

                    if (CargoBillingMemoAttachmentRepository == null)
                    {
                        CargoBillingMemoAttachmentRepository = new CargoBillingMemoAttachmentRepository();
                    }

                    CargoBillingMemoAttachmentRepository.Add(billingMemoAttachment);
                    Logger.InfoFormat("Added entry in DB table for Cargo Billing Memo Attachment: {0}", billingMemoAttachment.OriginalFileName);
                }
            
            break;

          case RecordType.CreditMemo:

            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var creditMemoAttachment = new CreditMemoAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(creditMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = creditMemoAttachment.Id + Path.GetExtension(fileName);

                if (CreditMemoAttachmentRepository == null)
                {
                    CreditMemoAttachmentRepository = new CreditMemoAttachmentRepository();
                }

                CreditMemoAttachmentRepository.Add(creditMemoAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger Credit Memo Attachment: {0}", creditMemoAttachment.OriginalFileName);
            }
            else
            {
                var creditMemoAttachment = new CargoCreditMemoAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(creditMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = creditMemoAttachment.Id + Path.GetExtension(fileName);

                if (CargoCreditMemoAttachmentRepository == null)
                {
                    CargoCreditMemoAttachmentRepository = new CargoCreditMemoAttachmentRepository();
                }

                CargoCreditMemoAttachmentRepository.Add(creditMemoAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo Credit Memo Attachment: {0}", creditMemoAttachment.OriginalFileName);
            }
               

            break;

          case RecordType.RejectionMemo:

            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var rejectionMemoAttachment = new RejectionMemoAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(rejectionMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = rejectionMemoAttachment.Id + Path.GetExtension(fileName);

                if (RejectionMemoAttachmentRepository == null)
                {
                    RejectionMemoAttachmentRepository = new RejectionMemoAttachmentRepository();
                }

                RejectionMemoAttachmentRepository.Add(rejectionMemoAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger Rejection Memo Attachment: {0}", rejectionMemoAttachment.OriginalFileName);
            }
            else
            {
                var rejectionMemoAttachment = new CgoRejectionMemoAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(rejectionMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = rejectionMemoAttachment.Id + Path.GetExtension(fileName);

                if (CgoRejectionMemoAttachmentRepository == null)
                {
                    CgoRejectionMemoAttachmentRepository = new CgoRejectionMemoAttachmentRepository();
                }

                CgoRejectionMemoAttachmentRepository.Add(rejectionMemoAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo Rejection Memo Attachment: {0}", rejectionMemoAttachment.OriginalFileName);
            }
                

            break;

          case RecordType.BillingMemoCoupon:

            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var billingMemoCouponAttachment = new BMCouponAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(billingMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = billingMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (BillingMemoCouponAttachmentRepository == null)
                {
                    BillingMemoCouponAttachmentRepository = new BillingMemoCouponAttachmentRepository();
                }

                BillingMemoCouponAttachmentRepository.Add(billingMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger BM Coupon Attachment: {0}", billingMemoCouponAttachment.OriginalFileName);
            }
            else
            {
                var billingMemoCouponAttachment = new BMAwbAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(billingMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = billingMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (BMAwbAttachmentRepository == null)
                {
                    BMAwbAttachmentRepository = new BMAwbAttachmentRepository();
                }

                BMAwbAttachmentRepository.Add(billingMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo BM AWB Attachment: {0}", billingMemoCouponAttachment.OriginalFileName);
            }
                
            break;

          case RecordType.CreditMemoCoupon:

            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var creditMemoCouponAttachment = new CMCouponAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(creditMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = creditMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (CreditMemoCouponAttachmentRepository == null)
                {
                    CreditMemoCouponAttachmentRepository = new CreditMemoCouponAttachmentRepository();
                }

                CreditMemoCouponAttachmentRepository.Add(creditMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger CM Coupon Attachment: {0}", creditMemoCouponAttachment.OriginalFileName);
            }
            else
            {
                var creditMemoCouponAttachment = new CMAwbAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(creditMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = creditMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (CargoCreditMemoAwbAttachmentRepository == null)
                {
                    CargoCreditMemoAwbAttachmentRepository = new CMAwbAttachmentRepository();
                }

                CargoCreditMemoAwbAttachmentRepository.Add(creditMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo CM AWB Attachment: {0}", creditMemoCouponAttachment.OriginalFileName);
            }
               

            break;

          case RecordType.RejectionMemoCoupon:
            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var rejectionMemoCouponAttachment = new RMCouponAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(rejectionMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = rejectionMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (RejectionMemoCouponAttachmentRepository == null)
                {
                    RejectionMemoCouponAttachmentRepository = new RejectionMemoCouponAttachmentRepository();
                }

                RejectionMemoCouponAttachmentRepository.Add(rejectionMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger RM Coupon Attachment: {0}", rejectionMemoCouponAttachment.OriginalFileName);
            }
            else
            {
                var rejectionMemoCouponAttachment = new RMAwbAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(rejectionMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = rejectionMemoCouponAttachment.Id + Path.GetExtension(fileName);

                if (RMAwbAttachmentRepository == null)
                {
                    RMAwbAttachmentRepository = new RMAwbAttachmentRepository();
                }

                RMAwbAttachmentRepository.Add(rejectionMemoCouponAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo RM AWB Attachment: {0}", rejectionMemoCouponAttachment.OriginalFileName);
            }

            break;

          case RecordType.PrimeBillingCoupon:

            Logger.Info("primbillingcoupon");
            if (criteria.BillingCategory == (int)BillingCategoryType.Pax)
            {
                var couponRecordAttachment = new PrimeCouponAttachment { ParentId = recordId, FilePath = filePath };

                PopulateAttachment(couponRecordAttachment, fileName, originalFileName, criteria.BillingMemberId);

                destinationFileName = couponRecordAttachment.Id + Path.GetExtension(fileName);

                if (CouponRecordAttachmentRepository == null)
                {
                    CouponRecordAttachmentRepository = new CouponRecordAttachmentRepository();
                }

                CouponRecordAttachmentRepository.Add(couponRecordAttachment);
                Logger.InfoFormat("Added entry in DB table for Passenger Prime Coupon Attachment: {0}", couponRecordAttachment.OriginalFileName);
            }
            else
            {
                Logger.Info("filepath:" + filePath);
                var couponRecordAttachment = new AwbAttachment {ParentId = recordId, FilePath = filePath};

                Logger.Info("fileName: " + fileName);
                Logger.Info("orgfilename: " + originalFileName);
                PopulateAttachment(couponRecordAttachment, fileName, originalFileName, criteria.BillingMemberId);

               
                destinationFileName = couponRecordAttachment.Id + Path.GetExtension(fileName);
                Logger.Info("destinationFileName: " + destinationFileName);
                if (CargoAwbAttachmentRepository == null)
                {
                    Logger.Info("null");
                    CargoAwbAttachmentRepository = new CargoAwbAttachmentRepository();
                }
                Logger.Info("adding");
                CargoAwbAttachmentRepository.Add(couponRecordAttachment);
                Logger.InfoFormat("Added entry in DB table for Cargo AWB Attachment: {0}", couponRecordAttachment.OriginalFileName);
            }
                

            break;

          case RecordType.FormC:

            var samplingFormCRecordAttachment = new SamplingFormCRecordAttachment { ParentId = recordId, FilePath = filePath };

            PopulateAttachment(samplingFormCRecordAttachment, fileName, originalFileName, criteria.BillingMemberId);

            destinationFileName = samplingFormCRecordAttachment.Id + Path.GetExtension(fileName);

            if (SamplingFormCAttachmentRepository == null)
            {
              SamplingFormCAttachmentRepository = new SamplingFormCAttachmentRepository();
            }

            SamplingFormCAttachmentRepository.Add(samplingFormCRecordAttachment);
            Logger.InfoFormat("Added entry in DB table for Form C Attachment: {0}", samplingFormCRecordAttachment.OriginalFileName);
            break;

          case RecordType.MiscInvoice:

            var miscInvoiceAttachment = new MiscUatpAttachment { ParentId = recordId, FilePath = filePath };

            PopulateAttachment(miscInvoiceAttachment, fileName, originalFileName, criteria.BillingMemberId);

            destinationFileName = miscInvoiceAttachment.Id + Path.GetExtension(fileName);

            if (MiscUatpInvoiceAttachmentRepository == null)
            {
              MiscUatpInvoiceAttachmentRepository = Ioc.Resolve<IMiscUatpInvoiceAttachmentRepository>();
            }

            MiscUatpInvoiceAttachmentRepository.Add(miscInvoiceAttachment);
            Logger.InfoFormat("Added entry in DB table for MISC Invoice Attachment: {0}", miscInvoiceAttachment.OriginalFileName);
            break;

          case RecordType.UatpInvoice:

            var uatpInvoiceAttachment = new MiscUatpAttachment { ParentId = recordId, FilePath = filePath };

            PopulateAttachment(uatpInvoiceAttachment, fileName, originalFileName, criteria.BillingMemberId);

            destinationFileName = uatpInvoiceAttachment.Id + Path.GetExtension(fileName);

            if (MiscUatpInvoiceAttachmentRepository == null)
            {
                MiscUatpInvoiceAttachmentRepository = Ioc.Resolve<IMiscUatpInvoiceAttachmentRepository>();
            }

            MiscUatpInvoiceAttachmentRepository.Add(uatpInvoiceAttachment);
            Logger.InfoFormat("Added entry in DB table for UATP Invoice Attachment: {0}", uatpInvoiceAttachment.OriginalFileName);
            break;

          case RecordType.FormD:

            var samplingFormDRecordAttachment = new SamplingFormDRecordAttachment { ParentId = recordId, FilePath = filePath };

            PopulateAttachment(samplingFormDRecordAttachment, fileName, originalFileName, criteria.BillingMemberId);

            destinationFileName = samplingFormDRecordAttachment.Id + Path.GetExtension(fileName);

            if (SamplingFormDAttachmentRepository == null)
            {
              SamplingFormDAttachmentRepository = new SamplingFormDAttachmentRepository();
            }

            SamplingFormDAttachmentRepository.Add(samplingFormDRecordAttachment);
            Logger.InfoFormat("Added entry in DB table for Form D Attachment: {0}", samplingFormDRecordAttachment.OriginalFileName);
            break;
        }

        //Copy the file with attachment.Id as file name to Folder BBBBYYYYMM(Billed member id, Billing year, Billing month)
        CopyDocumentsToSfr(fileName, filePath);

        UnitOfWork.CommitDefault();
      }
      else
      {
        Logger.Info("Root path for Linked folder is not found in Invoice Offline Collection");
        return "Supporting documents path for invoice is not found or empty.";
      }

      return string.Empty;
    }

    /// <summary>
    ///   Checks if the given document is already present in the database
    /// </summary>
    /// <param name = "unlinkedSupportingDocuments"></param>
    /// <param name = "fileName"></param>
    /// <returns></returns>
    private bool IsDuplicateUnlinked(IQueryable<UnlinkedSupportingDocument> unlinkedSupportingDocuments, string fileName)
    {
      if (!string.IsNullOrEmpty(fileName))
      {
        if (unlinkedSupportingDocuments.Count() == 0)
        {
          return false;
        }

        return Enumerable.Any(unlinkedSupportingDocuments,
                              supportingDocumentRecord =>
                              supportingDocumentRecord.OriginalFileName != null && supportingDocumentRecord.OriginalFileName.Trim().ToUpper().Equals(fileName.Trim().ToUpper()));
      }
      return false;
    }

    /// <summary>
    ///   Checks if the given document is already present in the database
    /// </summary>
    /// <param name = "supportingDocumentRecords"></param>
    /// <param name = "fileName"></param>
    /// <returns></returns>
    private bool IsDuplicate(List<SupportingDocumentRecord> supportingDocumentRecords, string fileName)
    {
      if (!string.IsNullOrEmpty(fileName))
      {
        return
          supportingDocumentRecords.Any(
            supportingDocumentRecord => supportingDocumentRecord.AttachmentFileName != null && supportingDocumentRecord.AttachmentFileName.Trim().ToUpper().Equals(fileName.Trim().ToUpper()));
      }
      return false;
    }

    private RecordSearchCriteria GetCriteria(UnlinkedSupportingDocument unlinkedDocument)
    {

      var recordSearchCriteria = new RecordSearchCriteria();
      recordSearchCriteria.InvoiceNumber = unlinkedDocument.InvoiceNumber;
      recordSearchCriteria.BillingMemberId = unlinkedDocument.BillingMemberId;
      recordSearchCriteria.BillingMemberCode = MemberManager.GetMember(unlinkedDocument.BillingMemberId).MemberCodeNumeric;
      recordSearchCriteria.BilledMemberId = unlinkedDocument.BilledMemberId;
      recordSearchCriteria.BilledMemberCode = MemberManager.GetMember(unlinkedDocument.BilledMemberId).MemberCodeNumeric;
      recordSearchCriteria.BillingYear = unlinkedDocument.BillingYear;
      recordSearchCriteria.ClearanceMonth = unlinkedDocument.BillingMonth;
      recordSearchCriteria.ClearancePeriod = unlinkedDocument.PeriodNumber;
      recordSearchCriteria.BatchNumber = unlinkedDocument.BatchNumber;
      recordSearchCriteria.SequenceNumber = unlinkedDocument.SequenceNumber;
      recordSearchCriteria.FileName = unlinkedDocument.FilePath;
      recordSearchCriteria.OriginalFileName = unlinkedDocument.OriginalFileName;
      recordSearchCriteria.BillingCategory = unlinkedDocument.BillingCategoryId;
      recordSearchCriteria.BreakdownSerialNumber = unlinkedDocument.CouponBreakdownSerialNumber;

      return recordSearchCriteria;
    }

    /// <summary>
    ///   Links unlinked supporting document to the matching record/invoice
    /// </summary>
    /// <param name = "unlinkedDocument"></param>
    /// <param name="invoiceBase">Invoice Base [Defalult value is null]</param>
    /// <param name="skipSuppDocLinkingDeadlineCheck">Indicated whether called from Finalization process</param>
    /// <returns></returns>
    //SCP133627 - LP/544-Mismatch in CRSupporting File
    public string LinkDocument(UnlinkedSupportingDocument unlinkedDocument, InvoiceBase invoiceBase = null, bool skipSuppDocLinkingDeadlineCheck = false)
    {
      var skipDuplicateCheck = false;
      var recordSearchCriteria = GetCriteria(unlinkedDocument);

      // SCP62083: Supporting documents not received in SIS - SN-082
      // Use original invoice billing period and billing month in case of Form D invoice.
      if (invoiceBase != null)
      {
        if (invoiceBase.BillingCode == (int)BillingCode.SamplingFormDE)
        {
          //SCP255391: FW: Supporting Documents missing for SA Form
          recordSearchCriteria.FormDEBillingMonth = invoiceBase.BillingMonth;
          recordSearchCriteria.FormDEBillingPeriod = invoiceBase.BillingPeriod;
          recordSearchCriteria.FormDEBillingYear = invoiceBase.BillingYear;
          recordSearchCriteria.IsFormD = true;
        }
      }

      recordSearchCriteria.ChargeCategoryId = null;

      var supportingDocumentRecords = GetRecordListWithAttachments(recordSearchCriteria);
      if (supportingDocumentRecords != null && supportingDocumentRecords.Count > 0)
      {
        // SCP62083: Supporting documents not received in SIS - SN-082
        // Use original invoice billine year, billing month and billing period in case of Form D invoice to fetch eventTime.
        DateTime eventTime;

        if (invoiceBase != null && invoiceBase.BillingCode == (int)BillingCode.SamplingFormDE)
        {
          eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                                 invoiceBase.BillingYear, invoiceBase.BillingMonth,
                                                 invoiceBase.BillingPeriod);
        }
        else
        {
          //SCP162502: Form C - AC OAR Jul P3 failure - No alert received
          if (supportingDocumentRecords[0].IsFormC.Equals("Y"))
          {
            var currentOpenPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                                             currentOpenPeriod.Year, currentOpenPeriod.Month,
                                                             currentOpenPeriod.Period);
          }
          else
          {
          eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                                 unlinkedDocument.BillingYear, unlinkedDocument.BillingMonth,
                                                 unlinkedDocument.PeriodNumber);
          }
        }
        //SCP133627 - LP/544-Mismatch in CRSupporting File
        //if called from finalization, first condition will never be true, if first condition is false and not called from finalization as well, deadline is over
        if (DateTime.UtcNow <= eventTime || skipSuppDocLinkingDeadlineCheck)
          {
              // Check for sample digit announcement.
              var billingMonth = string.Format("{0}{1:D2}", unlinkedDocument.BillingYear, unlinkedDocument.BillingMonth);
              var sampleDigitList = ValidationSampleDigiteManager.GetSampleDigitList(billingMonth);
              if (supportingDocumentRecords[0].IsFormC.Equals("Y") && sampleDigitList.Count > 0 && sampleDigitList.Find(sampleDigit => sampleDigit.DigitAnnouncementDateTime < DateTime.UtcNow) != null)
              {
                  var errMessage = string.Format("Attachment is ignored as, sample digit is announced for given provisional clearance month: {0}", billingMonth);
                  Logger.InfoFormat(errMessage);
                  return errMessage;
              }

              //check if only record w/o attachment
              if (supportingDocumentRecords.Count == 1 && string.IsNullOrEmpty(supportingDocumentRecords[0].AttachmentFileName))
              {
                  skipDuplicateCheck = true;
              }

              var recordType = (RecordType)Enum.Parse(typeof(RecordType), supportingDocumentRecords[0].RecordType);
              var recordId = supportingDocumentRecords[0].RecordId;
              recordSearchCriteria.FormDInvoiceNumber = supportingDocumentRecords[0].FormDInvoiceNumber;
            
              recordSearchCriteria.IsFormC = supportingDocumentRecords[0].IsFormC.ToUpper().Equals("Y");

              if (!skipDuplicateCheck && IsDuplicate(supportingDocumentRecords, Path.GetFileName(recordSearchCriteria.OriginalFileName)))
              {
                  //Delete document from temp source path
                  if (File.Exists(recordSearchCriteria.FileName))
                  {
                      File.Delete(recordSearchCriteria.FileName);
                  }

                  //Delete from Unlinked document table
                  //DeleteUnlinkedDocument(unlinkedDocument);
                  //SCP133627 - LP/544-Mismatch in CRSupporting File
                  Logger.Info("Duplicate file found in unlinked documents! Document ignored");
                  return "Duplicate file found in unlinked documents! Document ignored"; // " and deleted from source path";
              }
              //build the attachment
              var fileServer = ReferenceManager.GetActiveUnlinkedDocumentsServer();
              var idFileName = recordSearchCriteria.FileName; // +Path.GetExtension(recordSearchCriteria.OriginalFileName);
              var idFile = Path.Combine(fileServer.BasePath, idFileName);
              var errorMesg = BuildAttachment(recordType, recordId, idFile, recordSearchCriteria.OriginalFileName, recordSearchCriteria, skipSuppDocLinkingDeadlineCheck);

              if (errorMesg != string.Empty)
              {
                  //SCP133627 - LP/544-Mismatch in CRSupporting File
                  Logger.Info(errorMesg);
                  return errorMesg;
              }

              //Delete from Unlinked document table
              DeleteUnlinkedDocuments(unlinkedDocument);
          }
        else
        {
            //SCP133627 - LP/544-Mismatch in CRSupporting File
            Logger.Info("Supporting document deadline is over.");
            return "Supporting document deadline is over.";
        }


      }
      else
      {
          //SCP133627 - LP/544-Mismatch in CRSupporting File
          Logger.Info("Invoice do not have supporting documents.");
        return "Invoice or supporting document is not present.";
      }

      recordSearchCriteria = null;
      //Logger.Info("Invoice do not have supporting documents.");
      return "";
    }

    /// <summary>
    ///   This method gets all the records for given search criteria and for each record, updates the link to invoice and transactions
    ///   and moves the attachment from unlinked document table to respective attachment table
    /// </summary>
    /// <param name = "recordSearchCriteria">record search criteria</param>
    /// <param name = "unlinkedSupportingDocuments">record search criteria</param>
    /// <returns>true if success</returns>
    public bool LinkDocuments(IEnumerable<RecordSearchCriteria> recordSearchCriteria, List<UnlinkedSupportingDocument> unlinkedSupportingDocuments)
    {
      var skipDuplicateCheck = false;

      foreach (var criteria in recordSearchCriteria)
      {
        var supportingDocumentRecords = GetRecordListWithAttachments(criteria);
        if (supportingDocumentRecords != null && supportingDocumentRecords.Count > 0)
        {
          //check if only record w/o attachment
          if (supportingDocumentRecords.Count == 1 && string.IsNullOrEmpty(supportingDocumentRecords[0].AttachmentFileName))
          {
            skipDuplicateCheck = true;
          }

          var recordType = (RecordType)Enum.Parse(typeof(RecordType), supportingDocumentRecords[0].RecordType);
          var recordId = supportingDocumentRecords[0].RecordId;

          foreach (var file in criteria.OriginalFiles)
          {
            if (!skipDuplicateCheck && IsDuplicate(supportingDocumentRecords, Path.GetFileName(file.Value)))
            {
              //Delete document from temp source path
              if (File.Exists(file.Value))
              {
                File.Delete(file.Value);
              }
              Logger.Info("Duplicate file found in unlinked documents! Document ignored and deleted from source path.");

              continue;
            }
            //build the attachment
            var fileServer = ReferenceManager.GetActiveUnlinkedDocumentsServer();
            var idFileName = file.Key + Path.GetExtension(file.Value);
            var idFile = Path.Combine(fileServer.BasePath, idFileName);
            var originalFileName = criteria.OriginalFiles[file.Key];
            BuildAttachment(recordType, recordId, idFile, originalFileName, criteria);

            //Delete from Unlinked document table
            if (unlinkedSupportingDocuments != null)
            {
              DeleteUnlinkedDocument(unlinkedSupportingDocuments.Find(docId => docId.Id == Guid.Parse(file.Key)));
            }
          }
        }
      }

      //Commit the transaction for all the attachment repositories
      UnitOfWork.CommitDefault();

      return true;
    }

    public bool DeleteUnlinkedDocuments(UnlinkedSupportingDocument unlinkedSupportingDocument)
    {
      DeleteUnlinkedDocument(unlinkedSupportingDocument);
      UnitOfWork.CommitDefault();
      return true;
    }

    private void DeleteUnlinkedDocument(UnlinkedSupportingDocument unlinkedSupportingDocument)
    {
      var unlinkedSupportingDocumentToBeDeleted = SupportingDocumentRepository.Single(doc => doc.Id == unlinkedSupportingDocument.Id);

      if (unlinkedSupportingDocumentToBeDeleted == null)
      {
        return;
      }
      SupportingDocumentRepository.Delete(unlinkedSupportingDocumentToBeDeleted);
    }

    /// <summary>
    ///   To copy the source file to the InvoiceOffline colllection attachments file repository.
    /// </summary>
    /// <param name = "sourceFilePath"></param>
    /// <param name = "targetFileName"></param>
    /// <returns></returns>
    public bool CopyDocumentsToSfr(string sourceFilePath, string targetFileName)
    {
      var supportingDocStoragePath = Path.GetDirectoryName(targetFileName);
      var isFileCopied = false;
      if (!string.IsNullOrEmpty(supportingDocStoragePath) && Directory.Exists(supportingDocStoragePath))
      {
        if (!File.Exists(targetFileName))
          File.Move(sourceFilePath, targetFileName);
        isFileCopied = true;
        Logger.Info(string.Format("{0} file copied to Linked folder", targetFileName));
      }
      else
      {
        Logger.Info("Root path for Linked folder is not found in Invoice Offline Collection");
      }

      return isFileCopied;
    }

    /// <summary>
    ///   To get the Invoice offline coolection supporting document attachment path for recordsearch criteria.
    /// </summary>
    /// <param name = "criteria"></param>
    /// <param name="formCDetailId"></param>
    /// <returns></returns>
    /// SCP162502  Form C - AC OAR Jul P3 failure - No alert received
    private string GetSupportingDocPathFromOfflineMetaData(RecordSearchCriteria criteria, Guid? formCDetailId = null, bool skipSuppDocLinkingDeadlineCheck = false)
    {
      //var offlinePathForSupportDoc = string.Empty;
      var subFolderPath = string.Empty;

      if (criteria.BillingCategory.HasValue)
      {
        InvoiceOfflineCollectionMetaData offileMetaData;
        //SCP 95393: Added Billing Year check in Search Criteria
        // In case of Form C don't use Period No.
        if (criteria.IsFormC)
        {
          offileMetaData =
            InvoiceOfflineCollectionMetaDataRepository.Get(
              filepath =>
              filepath.BilledMemberCode == criteria.BilledMemberCode &&
              filepath.BillingMemberCode == criteria.BillingMemberCode &&
              (criteria.IsFormC
                 ? filepath.ProvisionalBillingMonth == criteria.ClearanceMonth
                 : filepath.BillingMonth == criteria.ClearanceMonth) &&
              (criteria.IsFormC
                 ? filepath.ProvisionalBillingYear == criteria.BillingYear
                 : filepath.BillingYear == criteria.BillingYear) &&
              filepath.BillingCategoryId == criteria.BillingCategory.Value && filepath.IsFormC == criteria.IsFormC &&
              filepath.OfflineCollectionFolderTypeId == 4).FirstOrDefault();
        }// End if
        // For Invoice, fetch offline collection supporting doc path from InvoiceOfflineCollectionMetaData table.
        else
        {
          offileMetaData =
            InvoiceOfflineCollectionMetaDataRepository.Get(
              filepath =>
              filepath.InvoiceNumber ==
              (criteria.IsFormD ? criteria.FormDInvoiceNumber : criteria.InvoiceNumber) &&
              filepath.BilledMemberCode == criteria.BilledMemberCode &&
              filepath.BillingMemberCode == criteria.BillingMemberCode
              && filepath.BillingYear == (criteria.IsFormD ? criteria.FormDEBillingYear : criteria.BillingYear)
              && filepath.BillingMonth == (criteria.IsFormD ? criteria.FormDEBillingMonth : criteria.ClearanceMonth)
              && filepath.PeriodNo == (criteria.IsFormD ? criteria.FormDEBillingPeriod : criteria.ClearancePeriod) &&
              filepath.BillingCategoryId == criteria.BillingCategory.Value &&
              filepath.IsFormC == criteria.IsFormC && filepath.OfflineCollectionFolderTypeId == 4).
              FirstOrDefault();

        }// End else

        // If offlinecollection meta data found then use offline coll supp doc folder path. Else create it.
        if (offileMetaData != null)
        {
          Logger.InfoFormat("OfflineMetaData Found, Supp Doc Loc is : {0}",offileMetaData.FilePath);
          subFolderPath = offileMetaData.FilePath;
        }
        // Generate Offline Collection Supp Doc path if OfflineCollectionMetaData not found for the given FormC
        else if(criteria.IsFormC)
        {
          // Call "PROC_GET_FRMC_FST_FOLDER_NAME" SP to get root folder name where the Form C Offline collection needs to be created.
          var firstLevelFolderName = SupportingDocumentRepository.GetFormCFolderName((Guid)formCDetailId,
                                                                                     skipSuppDocLinkingDeadlineCheck);
          // If root folder name is null then send alert to IS Admin.
          if (string.IsNullOrEmpty(firstLevelFolderName))
          {
            Logger.InfoFormat(
              "Problem in Proc PROC_GET_FRMC_FST_FOLDER_NAME while creating First Level Folder path for Pax Form C detail Id {0}",
              formCDetailId);
            var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>(typeof(IBroadcastMessagesManager));

            broadcastMessagesManager.SendISAdminExceptionNotification(EmailTemplateId.ISAdminExceptionNotification,
                                                                      "Supporting Doc Manager",
                                                                      new Exception(
                                                                        string.Format(
                                                                          "Problem in Proc PROC_GET_FRMC_FST_FOLDER_NAME while creating First Level Folder path for Pax Form C detail Id {0}",
                                                                          formCDetailId)), false);
            Ioc.Release(broadcastMessagesManager);
          }// End if
          // Else generate the Form C Offline collection Sup doc path using the folder name fetched above.
          else
          {
            // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
            // Get root path for Form C based on billing period.
            var period = DateTime.ParseExact(firstLevelFolderName, "yyyyMMdd", CultureInfo.InvariantCulture);

            var folderPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR,
                                                                new BillingPeriod(period.Year, period.Month, period.Day))
                                            .ToUpper(), firstLevelFolderName);

            //CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich).Period.ToString().PadLeft(2, '0')));)
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            Logger.InfoFormat(
              "Creating third level folder [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
            // e.g. PAX-022-014
            folderPath =
              Path.Combine(folderPath,
                           string.Format("{0}-{1}-{2}", "PAX", criteria.BillingMemberCode, criteria.BilledMemberCode)).
                ToUpper();
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            Logger.InfoFormat("Creating fourth level folder [{0}]...", "FORMC-YYYYMMPP");
            // e.g. FORMC-20110200
            folderPath =
              Path.Combine(folderPath,
                           string.Format("FORMC-{0}",
                                         GetFormattedBillingMonthYear(criteria.ClearanceMonth.Value,
                                                                      criteria.BillingYear.Value,
                                                                      PaxReportConstants.PaxFormCReportDateFormat))).
                ToUpper();
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            subFolderPath = Path.Combine(folderPath, "SUPPDOCS");
            if (!Directory.Exists(subFolderPath)) Directory.CreateDirectory(subFolderPath);

          }// End else

        }// End if
        // Generate Offline Collection Supp Doc path if OfflineCollectionMetaData not found for the given Invoice
        else if (!criteria.IsFormC)
        {
          // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
          // Get root path for Form C based on billing period.
          var rootPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR,
                                               new BillingPeriod((criteria.IsFormD ? criteria.FormDEBillingYear : criteria.BillingYear.Value),
                                                                 (criteria.IsFormD ? criteria.FormDEBillingMonth :criteria.ClearanceMonth.Value),
                                                                 (criteria.IsFormD ? criteria.FormDEBillingPeriod :criteria.ClearancePeriod.Value)));

          Logger.InfoFormat("Creating first level folder path [YearMonthPeriod]...");

          subFolderPath = Path.Combine(rootPath, string.Format("{0}{1}", GetFormattedBillingMonthYear((criteria.IsFormD ? criteria.FormDEBillingMonth : criteria.ClearanceMonth.Value), (criteria.IsFormD ? criteria.FormDEBillingYear : criteria.BillingYear.Value), PaxReportConstants.PaxReportFolderDataFormat), (criteria.IsFormD ? criteria.FormDEBillingPeriod : criteria.ClearancePeriod.Value).ToString().PadLeft(2, '0')));
          
          Logger.InfoFormat("Creating third level folder path [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
          
          subFolderPath = Path.Combine(subFolderPath, string.Format("{0}-{1}-{2}", Enum.GetName(typeof(BillingCategoryType), criteria.BillingCategory.Value), criteria.BillingMemberCode, criteria.BilledMemberCode)).ToUpper();
          
          Logger.InfoFormat("Creating fourth level folder path[Invoice Number]");
          
          subFolderPath = Path.Combine(subFolderPath, string.Format("INV-{0}", criteria.IsFormD ? criteria.FormDInvoiceNumber : criteria.InvoiceNumber)).ToUpper();
          
          subFolderPath = Path.Combine(subFolderPath, SupportingDocumentFolderName);

          if (!Directory.Exists(subFolderPath))
          {
            Directory.CreateDirectory(subFolderPath);
          }// End if

        }// End if
     

        if (!string.IsNullOrWhiteSpace(subFolderPath) && Directory.Exists(subFolderPath))
        {
          //string subFolderPath;
          if (criteria.IsFormC || criteria.IsFormD)
          {
            subFolderPath = subFolderPath + "\\" + criteria.InvoiceNumber + "-" +
                            criteria.BatchNumber.ToString().PadLeft(5, '0') + "-" +
                            criteria.SequenceNumber.ToString().PadLeft(5, '0');
            if (!Directory.Exists(subFolderPath))
            {
              Directory.CreateDirectory(subFolderPath);
            } // End if

          } // End if
          else
          {
            if (criteria.BillingCategory.Value != (int) BillingCategoryType.Misc &&
                criteria.BillingCategory.Value != (int) BillingCategoryType.Uatp)
            {
              subFolderPath = subFolderPath + "\\" + criteria.BatchNumber.ToString().PadLeft(5, '0') + "-" +
                              criteria.SequenceNumber.ToString().PadLeft(5, '0');
              if (!Directory.Exists(subFolderPath))
              {
                Directory.CreateDirectory(subFolderPath);
              } // End if

            } // End if
           
          } // End else

          if (!criteria.IsFormC && !criteria.IsFormD && criteria.BreakdownSerialNumber != null &&
              criteria.BreakdownSerialNumber != 0)
          {
            subFolderPath = subFolderPath + "\\" + criteria.BreakdownSerialNumber.ToString().PadLeft(5, '0');
            if (!Directory.Exists(subFolderPath))
            {
              Directory.CreateDirectory(subFolderPath);
            } // End if

          } // End if

        } // End if

      }// End if

      return subFolderPath;
    }

    /// <summary>
    /// Gets the formatted billing month year.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="year">The year.</param>
    /// <param name="dateFormat">The date format.</param>
    /// <returns></returns>
    private string GetFormattedBillingMonthYear(int month, int year, string dateFormat)
    {
      try
      {
        // check if passed billingYear and billingMonth is valid,
        // and if yes, convert it to given DateFormat string
        return new DateTime(year, month, 1).ToString(dateFormat);
      }
      catch (Exception)
      {
        return string.Empty;
      }

    }

    /// <summary>
    ///   To copy the source file to the unlinked supporting documents repository.
    /// </summary>
    /// <param name = "sourceFilePath"></param>
    /// <param name = "destinationFileName"></param>
    /// <param name="fileServer"></param>
    /// <returns></returns>
    public bool CopyDocumentsToIsa(string sourceFilePath, string destinationFileName, FileServer fileServer)
    {
      var isFileCopied = false;
      if (File.Exists(sourceFilePath))
      {
        //var fileServer = _referenceManager.GetActiveUnlinkedDocumentsServer();

        if (!Directory.Exists(fileServer.BasePath))
        {
          Directory.CreateDirectory(fileServer.BasePath);
        }

        if (fileServer != null)
        {
          var destinationFilePath = Path.Combine(fileServer.BasePath, destinationFileName);
          
          if (!File.Exists(destinationFilePath))
          {
            File.Copy(sourceFilePath, destinationFilePath);
            Logger.Info(string.Format("{0} file copied to Unlinked folder", sourceFilePath));
            isFileCopied = true;
          }
        }
      }
      return isFileCopied;
    }

    /// <summary>
    /// this method run recursively retry to delete file 
    /// </summary>
    /// <param name="directoryName">file path</param>
    /// <param name="retryCount">retry count</param>
    /// <returns></returns>
    private bool DeleteDocumentsFromSourceDirectory(string directoryName, int retryCount = 1)
    {
        if (Directory.Exists(directoryName))
        {
            try
            {
                Directory.Delete(directoryName, true);
                return true;
            }
            catch (Exception exception)
            {
                Logger.InfoFormat("Exception during delete directory: " + exception.Message);

                if (retryCount <= 3)
                {
                    Logger.InfoFormat(string.Format("Retry {0} to delete directory", retryCount.ToString()));
                    retryCount++;
                    // wait thread for 1 sec, to get resource free 
                    System.Threading.Thread.Sleep(5000);
                    // retry again to delete file
                    DeleteDocumentsFromSourceDirectory(directoryName, retryCount);
                }
                else if (retryCount == 4)
                {
                    var customMessage = string.Format("Supporting document filePath: {0}, {1}", directoryName, exception.Message);

                    CommonUtil.SendEmail(exception, customMessage);
                }

                return false;
            }
        }
        return false;
    }

    #endregion
  }
}
