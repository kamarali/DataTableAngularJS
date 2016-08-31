
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Reports.Cargo.Impl;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Cargo.Impl;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Data.SupportingDocuments;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Cargo.Enums;
//using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using log4net;
using SubmissionMethod = Iata.IS.Model.Cargo.Enums.SubmissionMethod;

namespace Iata.IS.Business.SupportingDocuments.Impl
{
  /// <summary>
  ///   Manager class for supporting documents related operations
  /// </summary>
  public class CargoSupportingDocumentManager : ICargoSupportingDocumentManager
  {
    #region Member Variables

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private Dictionary<Guid, RecordType> dtRecords;
    public ICalendarManager CalendarManager { private get; set; }
    public IRepository<InvoiceOfflineCollectionMetaData> InvoiceOfflineCollectionMetaDataRepository { private get; set; }
  

    public ICargoSupportingDocumentRepository SupportingDocumentRepository { private get; set; }

    public ICargoSupportingDocumentRepository CargoPayableSupportingDocumentRepository { private get; set; }

    public ISampleDigitManager ValidationSampleDigiteManager { private get; set; }
    public IRepository<SupportingDocSearchResult> SupportingDocSearchResultRepository { get; set; }
    

     public ICargoAwbAttachmentRepository CouponRecordAttachmentRepository { get; set; }
     public ICargoBillingMemoAttachmentRepository BillingMemoAttachmentRepository { get; set; }
     public ICgoRejectionMemoAttachmentRepository RjectionMemoAttachmentRepository { get; set; }
     public IRMAwbAttachmentRepository RmAwbAttachRepository { get; set; }
     public IBMAwbAttachmentRepository BmAwbAttachRepository { get; set; }
    // public ICMAwbAttachmentRepository CmAwbAttachRepository { get; set; }
      
     public ICargoCreditMemoAttachmentRepository CreditMemoAttachmentRepository { get; set; }
     public ICargoCreditMemoAwbAttachmentRepository CargoCreditMemoAwbAttachmentRepository { get; set; }
      public IMemberManager MemberManager { private get; set; }

      /// <summary>
      /// Gets or sets the coupon record repository.
      /// </summary>
      /// <value>The awb record repository.</value>
      public ICargoAwbRecordRepository CargoAwbRecordRepository { get; set; }

      /// <summary>
      /// Gets or sets the cargo billing memo awb repository.
      /// </summary>
      /// <value>
      /// The cargo billing memo awb repository.
      /// </value>
      public ICargoBillingMemoAwbRepository BMAwbRepository { get; set; }

      /// <summary>
      /// Gets or sets the cargo billing memo repository.
      /// </summary>
      /// <value>
      /// The cargo billing memo repository.
      /// </value>
      public ICargoBillingMemoRepository BillingMemoRepository { get; set; }

      /// <summary>
      /// Gets or sets the rejection memo repository.
      /// </summary>
      /// <value>The rejection memo repository.</value>
      public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

      /// <summary>
      /// Gets or sets the rejection memo Awb breakdown repository.
      /// </summary>
      /// <value>The rejection memo Awb breakdown repository.</value>
      public IRMAwbRepository RMAwbRepository { get; set; }

      /// <summary>
      /// CreditMemo Repository, will be injected by the container.
      /// </summary>
      /// <value>The credit memo repository.</value>
      public ICargoCreditMemoRecordRepository CargoCreditMemoRepository { get; set; }

      /// <summary>
      /// Gets or sets the cargo credit memo awb repository.
      /// </summary>
      /// <value>
      /// The cargo credit memo awb repository.
      /// </value>
      public ICargoCreditMemoAwbRepository CMAwbRepository { get; set; }

      public ISupportingDocumentManager SupportingDocumentManager { get; set; }

      public IReferenceManager ReferenceManager { private get; set; }

    #endregion

    #region Constructor

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name = "supportingDocumentRepository"></param>
    public CargoSupportingDocumentManager(ICargoSupportingDocumentRepository supportingDocumentRepository)
    {
      SupportingDocumentRepository = supportingDocumentRepository;
      CargoPayableSupportingDocumentRepository = supportingDocumentRepository;
    }

    #endregion

    #region Public methods
#region "old code"
      /*
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
          var invoices = InvoiceRepository.GetAll();

          // Get sampling form D queryable object for matching search criteria.
          var formDRecords = SamplingFormDRepository.Get(
            formD => formD.ProvisionalInvoiceNumber.ToUpper() == criteria.InvoiceNumber.ToUpper()
                     && formD.BatchNumberOfProvisionalInvoice == criteria.BatchNumber &&
                     formD.RecordSeqNumberOfProvisionalInvoice == criteria.SequenceNumber);

          // Get invoice number from provisional invoice number using join.
          var resultList =
            formDRecords.Join(invoices, formD => formD.InvoiceId, inv => inv.Id, (formD, paxInvoice) => new { paxInvoice.InvoiceNumber, paxInvoice.BillingPeriod, paxInvoice.BillingMonth }).ToList();

          if (resultList != null && resultList.Count > 0)
          {
            var invoice = resultList[0];
            // Use original invoice number in case of Form D invoice.
            criteria.FormDInvoiceNumber = invoice.InvoiceNumber;
            criteria.ClearancePeriod = invoice.BillingPeriod;
            criteria.ClearanceMonth = invoice.BillingMonth;
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
              BuildAttachment(recordType, recordId, Path.Combine(extractedDirectory, file), Path.GetFileName(file), criteria);
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
          filteredList = filteredList.Where(invoice => invoice.AttachmentIndicatorOriginal == (criteria.AttachmentIndicatorOriginal != 2));
        }

        if (criteria.IsMismatchCases)
          filteredList =
            filteredList.Where(
              invoice =>
              criteria.IsMismatchCases
                ? (invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber == 0) || (!invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber > 0)
                : (!invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber == 0) || (invoice.AttachmentIndicatorOriginal && invoice.AttachmentNumber > 0));

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
      */
#endregion
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

       
    }

    
    /// <summary>
    ///   Fetch search result for Cargo supporting document
    /// </summary>
    /// <param name = "criteria"></param>
    /// <returns></returns>
    public IList<CargoPayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
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
       //  var recordType = (SupportingDocRecordType)recordTypeId;
      var recordType = (CargoSupportingDocRecordType)recordTypeId;
      switch (recordType)
      {
        
          case CargoSupportingDocRecordType.AWBPrepaid:
          var attachmentAWBPP = CouponRecordAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attachmentAWBPP);
          break;
          case CargoSupportingDocRecordType.AWBChargeCollect:
          var attachmentAWBCC = CouponRecordAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attachmentAWBCC);
          break;
          case CargoSupportingDocRecordType.BillingMemo:
          var attchmentBM = BillingMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentBM);
          break;
          case CargoSupportingDocRecordType.BillingMemoAWB:
          var attchmentBMAWB = BmAwbAttachRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentBMAWB);
          break;
          case CargoSupportingDocRecordType.RejectionMemo:
          var attchmentRM = RjectionMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentRM);
          break;
          case CargoSupportingDocRecordType.RejectionMemoAWB:
          var attchmentRMCoupon = RmAwbAttachRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          //var attchmentRMCoupon = RjectionMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentRMCoupon);
          break;
         case CargoSupportingDocRecordType.CreditMemo:
          var attchmentCM = CreditMemoAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentCM);
          break;
         case CargoSupportingDocRecordType.CreditMemoAWB:
          var attchmentCMCoupon = CargoCreditMemoAwbAttachmentRepository.GetDetail(attach => attach.ParentId == transactionIdGuid);
          attachments.AddRange(attchmentCMCoupon);
          break;
              
     
      }
      setSerialNoForAttachment(attachments);
      //Set the FileSizeInKb to file size in Kilo bytes.
      attachments = attachments.Select(attachment => { attachment.FileSizeInKb = (attachment.FileSize / 1024M); return attachment; }).ToList();
      return attachments;
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
      FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();
      var recordType = (CargoSupportingDocRecordType)recordTypeId;
      switch (recordType)
      {
        case CargoSupportingDocRecordType.AWBPrepaid:
          var attachCGO = new AwbAttachment();
          LoadAttachmentDetails(attachment, attachCGO);
          
          // Get Awb prepaid transaction record
          var awbPrepaidRecord = CargoAwbRecordRepository.Single(awb => awb.Id == transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (awbPrepaidRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && awbPrepaidRecord.AttachmentIndicatorOriginal == false)
          {
            awbPrepaidRecord.AttachmentIndicatorOriginal = true;
            CargoAwbRecordRepository.Update(awbPrepaidRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(awbPrepaidRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   awbPrepaidRecord.BatchSequenceNumber,
                                                                                   awbPrepaidRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder. 
              SupportingDocumentManager.CopyAttachments(attachCGO, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attachCGO.IsFullPath;
              attachment.FilePath = attachCGO.FilePath;
            }// End if
          }// End try
          catch(Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}",ex.Message,ex.StackTrace);
          }// End catch

          CouponRecordAttachmentRepository.Add(attachCGO);
          UnitOfWork.CommitDefault();
          attachCGO = CouponRecordAttachmentRepository.Single(a => a.Id == attachCGO.Id);
          return attachCGO;

        case CargoSupportingDocRecordType.AWBChargeCollect:
          var attachAWBCC = new AwbAttachment();
          LoadAttachmentDetails(attachment, attachAWBCC);
          
          // Get Awb Charge Collect transaction record
          var awbChargeCollectRecord = CargoAwbRecordRepository.Single(awb => awb.Id == transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (awbChargeCollectRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && awbChargeCollectRecord.AttachmentIndicatorOriginal == false)
          {
            awbChargeCollectRecord.AttachmentIndicatorOriginal = true;
            CargoAwbRecordRepository.Update(awbChargeCollectRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(awbChargeCollectRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {

              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   awbChargeCollectRecord.BatchSequenceNumber,
                                                                                   awbChargeCollectRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder. 
              SupportingDocumentManager.CopyAttachments(attachAWBCC, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attachAWBCC.IsFullPath;
              attachment.FilePath = attachAWBCC.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch


          CouponRecordAttachmentRepository.Add(attachAWBCC);
          UnitOfWork.CommitDefault();
          attachAWBCC = CouponRecordAttachmentRepository.Single(a => a.Id == attachAWBCC.Id);
          return attachAWBCC;

        case CargoSupportingDocRecordType.BillingMemo:
          var attchmentBM = new CargoBillingMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentBM);
          
          // Get Billing Memo transaction record
          var billingMemoRecord = BillingMemoRepository.Single(bm => bm.Id == transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (billingMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && billingMemoRecord.AttachmentIndicatorOriginal == false)
          {
            billingMemoRecord.AttachmentIndicatorOriginal = true;
            BillingMemoRepository.Update(billingMemoRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(billingMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo and SeqNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   billingMemoRecord.BatchSequenceNumber,
                                                                                   billingMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder. 
              SupportingDocumentManager.CopyAttachments(attchmentBM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentBM.IsFullPath;
              attachment.FilePath = attchmentBM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch


          BillingMemoAttachmentRepository.Add(attchmentBM);
          UnitOfWork.CommitDefault();
          attchmentBM = BillingMemoAttachmentRepository.Single(a => a.Id == attchmentBM.Id);
          return attchmentBM;

        case CargoSupportingDocRecordType.BillingMemoAWB:
          var attchmentBMAWB = new BMAwbAttachment();
          LoadAttachmentDetails(attachment, attchmentBMAWB);
          BmAwbAttachRepository.Add(attchmentBMAWB);
          // Get Billing Memo Awb transaction record
          var billingMemoAwbRecord = BMAwbRepository.GetBillingMemoWithAwb(transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (billingMemoAwbRecord.BillingMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && billingMemoAwbRecord.AttachmentIndicatorOriginal == false)
          {
            billingMemoAwbRecord.AttachmentIndicatorOriginal = true;
            BMAwbRepository.Update(billingMemoAwbRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(billingMemoAwbRecord.BillingMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                                   billingMemoAwbRecord.BillingMemoRecord.BatchSequenceNumber,
                                                                                   billingMemoAwbRecord.BillingMemoRecord.
                                                                                     RecordSequenceWithinBatch, billingMemoAwbRecord.BdSerialNumber);

              // Move attachment to supp doc folder.
              SupportingDocumentManager.CopyAttachments(attchmentBMAWB, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentBMAWB.IsFullPath;
              attachment.FilePath = attchmentBMAWB.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch


          BmAwbAttachRepository.Add(attchmentBMAWB);
          UnitOfWork.CommitDefault();
          attchmentBMAWB = BmAwbAttachRepository.Single(a => a.Id == attchmentBMAWB.Id);
          return attchmentBMAWB;

        case CargoSupportingDocRecordType.RejectionMemo:
          var attchmentRM = new CgoRejectionMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentRM);
          
          // Get Rejection Memo transaction record
          var rejectionMemoRecord = RejectionMemoRepository.Get(rm => rm.Id == transactionId).FirstOrDefault();
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (rejectionMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && rejectionMemoRecord.AttachmentIndicatorOriginal == false)
          {
            rejectionMemoRecord.AttachmentIndicatorOriginal = true;
            RejectionMemoRepository.Update(rejectionMemoRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(rejectionMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   rejectionMemoRecord.BatchSequenceNumber,
                                                                                   rejectionMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder.
              SupportingDocumentManager.CopyAttachments(attchmentRM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentRM.IsFullPath;
              attachment.FilePath = attchmentRM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch

          RjectionMemoAttachmentRepository.Add(attchmentRM);
          UnitOfWork.CommitDefault();
          attchmentRM = RjectionMemoAttachmentRepository.Single(a => a.Id == attchmentRM.Id);
          return attchmentRM;

        case CargoSupportingDocRecordType.RejectionMemoAWB:
          var attchmentRMAWB = new RMAwbAttachment();
          LoadAttachmentDetails(attachment, attchmentRMAWB);
          
          // Get Rejection Memo Awb transaction record
          var rejectionMemoAwbRecord = RMAwbRepository.GetRejectionMemoWithAwb(transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (rejectionMemoAwbRecord.RejectionMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && rejectionMemoAwbRecord.AttachmentIndicatorOriginal == false)
          {
            rejectionMemoAwbRecord.AttachmentIndicatorOriginal = true;
            RMAwbRepository.Update(rejectionMemoAwbRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {

            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(rejectionMemoAwbRecord.RejectionMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                                   rejectionMemoAwbRecord.RejectionMemoRecord.BatchSequenceNumber,
                                                                                   rejectionMemoAwbRecord.RejectionMemoRecord.
                                                                                     RecordSequenceWithinBatch, rejectionMemoAwbRecord.BdSerialNumber);

              // Move attachment to supp doc folder.
              SupportingDocumentManager.CopyAttachments(attchmentRMAWB, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentRMAWB.IsFullPath;
              attachment.FilePath = attchmentRMAWB.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch

          RmAwbAttachRepository.Add(attchmentRMAWB);
          UnitOfWork.CommitDefault();
          attchmentRMAWB = RmAwbAttachRepository.Single(a => a.Id == attchmentRMAWB.Id);
          return attchmentRMAWB;

        case CargoSupportingDocRecordType.CreditMemo:
          var attchmentCM = new CargoCreditMemoAttachment();
          LoadAttachmentDetails(attachment, attchmentCM);
          CreditMemoAttachmentRepository.Add(attchmentCM);
          // Get Credit Memo transaction record
          var creditMemoRecord = CargoCreditMemoRepository.Single(cm => cm.Id == transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (creditMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && creditMemoRecord.AttachmentIndicatorOriginal == false)
          {
            creditMemoRecord.AttachmentIndicatorOriginal = true;
            CargoCreditMemoRepository.Update(creditMemoRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(creditMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {

              // Create BatchNo, SeqNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberFolder(suppDocOfflineCollectionPath,
                                                                                   creditMemoRecord.BatchSequenceNumber,
                                                                                   creditMemoRecord.
                                                                                     RecordSequenceWithinBatch);

              // Move attachment to supp doc folder.
              SupportingDocumentManager.CopyAttachments(attchmentCM, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentCM.IsFullPath;
              attachment.FilePath = attchmentCM.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch


          UnitOfWork.CommitDefault();
          attchmentCM = CreditMemoAttachmentRepository.Single(a => a.Id == attchmentCM.Id);
          return attchmentCM;

        case CargoSupportingDocRecordType.CreditMemoAWB:
          var attchmentCMAWB = new CMAwbAttachment();
          LoadAttachmentDetails(attachment, attchmentCMAWB);
          
          // Get Credit Memo Awb transaction record
          var creditMemoAwbRecord = CMAwbRepository.GetCreditMemoWithAwb(transactionId);
          // If submission method for selected transaction is ISWeb and attachment indicator original is set to false, update it to true
          if (creditMemoAwbRecord.CreditMemoRecord.Invoice.SubmissionMethodId == (int)Model.Cargo.Enums.SubmissionMethod.IsWeb && creditMemoAwbRecord.AttachmentIndicatorOriginal == false)
          {
            creditMemoAwbRecord.AttachmentIndicatorOriginal = true;
            CMAwbRepository.Update(creditMemoAwbRecord);
          }

          // Copy supporting document to supp doc offline collection folder or 
          // if unable to create or get supp doc offline collection folder copy them to temp path.
          try
          {
            // Get supp doc offline collection folder path.
            string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(creditMemoAwbRecord.CreditMemoRecord.Invoice);

            // if supp doc offline collection folder path present.
            if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
            {
              // Create BatchNo, SeqNo and SerialNo folders
              suppDocOfflineCollectionPath = SupportingDocumentManager.CreateBatchNumberSequenceNumberRecSeqNumberFolder(suppDocOfflineCollectionPath,
                                                                                   creditMemoAwbRecord.CreditMemoRecord.BatchSequenceNumber,
                                                                                   creditMemoAwbRecord.CreditMemoRecord.
                                                                                     RecordSequenceWithinBatch, creditMemoAwbRecord.BdSerialNumber);

              // Move attachment to supp doc folder.
              SupportingDocumentManager.CopyAttachments(attchmentCMAWB, fileServer.BasePath, suppDocOfflineCollectionPath);

              // Set IsFullPath to true and set new file path.
              attachment.IsFullPath = attchmentCMAWB.IsFullPath;
              attachment.FilePath = attchmentCMAWB.FilePath;
            }// End if
          }// End try
          catch (Exception ex)
          {
            Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
          }// End catch

          CargoCreditMemoAwbAttachmentRepository.Add(attchmentCMAWB);
          UnitOfWork.CommitDefault();
          attchmentCMAWB = CargoCreditMemoAwbAttachmentRepository.Single(a => a.Id == attchmentCMAWB.Id);
          return attchmentCMAWB;
      }
      return null;
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
        var recordType = (CargoSupportingDocRecordType)recordTypeId;
      var attachmentIdGuid = attachmentId.ToGuid();
      switch (recordType)
      {
          case CargoSupportingDocRecordType.AWBPrepaid :
          var attachCoupon = CouponRecordAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attachCoupon == null)
          {
            return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attachCoupon);

          // Get awbPrepaidId from attachment object
          var awbPrepaidId = attachCoupon.ParentId;
          CouponRecordAttachmentRepository.Delete(attachCoupon);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var awbPrepaidAttachmentCount = CouponRecordAttachmentRepository.GetCount(attach => attach.ParentId == awbPrepaidId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && awbPrepaidAttachmentCount == 0)
          {
            var transaction = CargoAwbRecordRepository.Single(awb => awb.Id == awbPrepaidId);
            if(transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              CargoAwbRecordRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.AWBChargeCollect:
          var attachAWBCC = CouponRecordAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attachAWBCC == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attachAWBCC);

          // Get awbPrepaidId from attachment object
          var awbChargeCollectId = attachAWBCC.ParentId;
          CouponRecordAttachmentRepository.Delete(attachAWBCC);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var awbChargeCollectAttachmentCount = CouponRecordAttachmentRepository.GetCount(attach => attach.ParentId == awbChargeCollectId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && awbChargeCollectAttachmentCount == 0)
          {
            var transaction = CargoAwbRecordRepository.Single(awb => awb.Id == awbChargeCollectId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              CargoAwbRecordRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.BillingMemo:
          var attchmentBM = BillingMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentBM == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentBM);

          // Get billingMemoId from attachment object
          var billingMemoId = attchmentBM.ParentId;
          BillingMemoAttachmentRepository.Delete(attchmentBM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var billingMemoAttachmentCount = BillingMemoAttachmentRepository.GetCount(attach => attach.ParentId == billingMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoAttachmentCount == 0)
          {
            var transaction = BillingMemoRepository.Single(awb => awb.Id == billingMemoId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              BillingMemoRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.BillingMemoAWB:
          var attchmentBMAWB = BmAwbAttachRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentBMAWB == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentBMAWB);

          // Get billingMemoAwbId from attachment object
          var billingMemoAwbId = attchmentBMAWB.ParentId;
          BmAwbAttachRepository.Delete(attchmentBMAWB);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var billingMemoAwbAttachmentCount = BmAwbAttachRepository.GetCount(attach => attach.ParentId == billingMemoAwbId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && billingMemoAwbAttachmentCount == 0)
          {
            var transaction = BMAwbRepository.Single(awb => awb.Id == billingMemoAwbId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              BMAwbRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.CreditMemo:
          var attchmentCM = CreditMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentCM == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentCM);

          // Get creditMemoId from attachment object
          var creditMemoId = attchmentCM.ParentId;
          CreditMemoAttachmentRepository.Delete(attchmentCM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var creditMemoAttachmentCount = CreditMemoAttachmentRepository.GetCount(attach => attach.ParentId == creditMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoAttachmentCount == 0)
          {
            var transaction = CargoCreditMemoRepository.Single(awb => awb.Id == creditMemoId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              CargoCreditMemoRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.CreditMemoAWB:
          var attchmentCMAWB = CargoCreditMemoAwbAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentCMAWB == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentCMAWB);

          // Get creditMemoAwbId from attachment object
          var creditMemoAwbId = attchmentCMAWB.ParentId;
          CargoCreditMemoAwbAttachmentRepository.Delete(attchmentCMAWB);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var creditMemoAwbAttachmentCount = CargoCreditMemoAwbAttachmentRepository.GetCount(attach => attach.ParentId == creditMemoAwbId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && creditMemoAwbAttachmentCount == 0)
          {
            var transaction = CMAwbRepository.Single(awb => awb.Id == creditMemoAwbId);
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              CMAwbRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.RejectionMemo :
          var attchmentRM = RjectionMemoAttachmentRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentRM == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentRM);

          // Get RejectionMemoId from attachment object
          var rejectionMemoId = attchmentRM.ParentId;
          RjectionMemoAttachmentRepository.Delete(attchmentRM);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var rejectionMemoAttachmentCount = RjectionMemoAttachmentRepository.GetCount(attach => attach.ParentId == rejectionMemoId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoAttachmentCount == 0)
          {
            var transaction = RejectionMemoRepository.Get(rm => rm.Id == rejectionMemoId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              RejectionMemoRepository.Update(transaction);
              UnitOfWork.CommitDefault();
            }
          }

          return true;

          case CargoSupportingDocRecordType.RejectionMemoAWB:
          var attchmentRMAWB = RmAwbAttachRepository.Single(attach => attach.Id == attachmentIdGuid);
          if (attchmentRMAWB == null)
          {
              return false;
          }

          // Delete Attachment from physical path if isfullpath is true.
          SupportingDocumentManager.DeleteAttachement(attchmentRMAWB);

          // Get rejectionMemoAwbId from attachment object
          var rejectionMemoAwbId = attchmentRMAWB.ParentId;
          RmAwbAttachRepository.Delete(attchmentRMAWB);
          UnitOfWork.CommitDefault();

          // Get attachment count
          var rejectionMemoAwbAttachmentCount = RmAwbAttachRepository.GetCount(attach => attach.ParentId == rejectionMemoAwbId);
          // If invoice submission method is ISWeb and attachment count == 0, set Attachment Indicator Original flag to false
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb && rejectionMemoAwbAttachmentCount == 0)
          {
            var transaction = RMAwbRepository.Get(rm => rm.Id == rejectionMemoAwbId).FirstOrDefault();
            if (transaction != null)
            {
              transaction.AttachmentIndicatorOriginal = false;
              RMAwbRepository.Update(transaction);
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
      var recordType = (CargoSupportingDocRecordType)recordTypeId;
      switch (recordType) 
      {
          case CargoSupportingDocRecordType.AWBPrepaid:
          var attachmentCoupon = CouponRecordAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attachmentCoupon;
          case CargoSupportingDocRecordType.AWBChargeCollect:
          //var attchmentBM = BillingMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          var attchmentAWBCC = CouponRecordAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentAWBCC;
          case CargoSupportingDocRecordType.BillingMemo:
          var attchmentBM = BillingMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentBM;
          case CargoSupportingDocRecordType.BillingMemoAWB:
          var attchmentBMAWB = BmAwbAttachRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentBMAWB;
          case CargoSupportingDocRecordType.CreditMemo:
          var attchmentCMCoupon = CreditMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentCMCoupon;
          case CargoSupportingDocRecordType.CreditMemoAWB:
          var attchmentCM = CargoCreditMemoAwbAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentCM;
          case CargoSupportingDocRecordType.RejectionMemo:
          var attchmentRMCoupon = RjectionMemoAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentRMCoupon;
          case CargoSupportingDocRecordType.RejectionMemoAWB:
          var attchmentRM = RmAwbAttachRepository.Single(attach => attach.Id == transactionIdGuid);
          return attchmentRM;
        //case SupportingDocRecordType.FormD:
        //  var attachmentFormD = SamplingFormDAttachmentRepository.Single(attach => attach.Id == transactionIdGuid);
        //  return attachmentFormD;

     }
      return new AwbAttachment();
    }
      
      #region "Old Code"
      
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
        var recordType = (CargoSupportingDocRecordType)recordTypeId;
        switch (recordType)
        {
            case CargoSupportingDocRecordType.AWBPrepaid:
                return CouponRecordAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
                
            case CargoSupportingDocRecordType.AWBChargeCollect:
                return CouponRecordAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

            case CargoSupportingDocRecordType.BillingMemo:
                return BillingMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

            case CargoSupportingDocRecordType.BillingMemoAWB:
                return BmAwbAttachRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

            case CargoSupportingDocRecordType.CreditMemo:
                return CreditMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
                
            case CargoSupportingDocRecordType.CreditMemoAWB:
                return CargoCreditMemoAwbAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

            case CargoSupportingDocRecordType.RejectionMemo:
                return RjectionMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;

            case CargoSupportingDocRecordType.RejectionMemoAWB:
                return RmAwbAttachRepository.GetCount(attachment => attachment.ParentId == transactionIdGuid && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;


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
      /*
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
         }

         /// <summary>
         ///   Builds the list of attachments based of record type and adds to respective repositories
         /// </summary>
         /// <param name = "recordType"></param>
         /// <param name = "recordId"></param>
         /// <param name = "fileName"></param>
         /// <param name = "originalFileName"></param>
         /// <param name = "criteria"></param>
         private string BuildAttachment(RecordType recordType, Guid recordId, string fileName, string originalFileName, RecordSearchCriteria criteria)
         {
           if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
           {
             //Logger.InfoFormat("Attachment not found to attach! [File: {0}]", fileName);o
             return ("Attachment file not found to attach.");
           }
           var destinationFileName = string.Empty;
           //var filePath = string.Format("{0}_{1}_{2}", criteria.BilledMemberId, criteria.BillingYear, criteria.ClearanceMonth);
           var filePath = GetSupportingDocPathFromOfflineMetaData(criteria);

           if (!string.IsNullOrEmpty(filePath) && Directory.Exists(filePath))
           {
             filePath = Path.Combine(filePath, Path.GetFileName(originalFileName));
             switch (recordType)
             {
               case RecordType.BillingMemo:

                 var billingMemoAttachment = new BillingMemoAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(billingMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = billingMemoAttachment.Id + Path.GetExtension(fileName);

                 if (BillingMemoAttachmentRepository == null)
                 {
                   BillingMemoAttachmentRepository = new BillingMemoAttachmentRepository();
                 }

                 BillingMemoAttachmentRepository.Add(billingMemoAttachment);

                 break;

               case RecordType.CreditMemo:

                 var creditMemoAttachment = new CreditMemoAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(creditMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = creditMemoAttachment.Id + Path.GetExtension(fileName);

                 if (CreditMemoAttachmentRepository == null)
                 {
                   CreditMemoAttachmentRepository = new CreditMemoAttachmentRepository();
                 }

                 CreditMemoAttachmentRepository.Add(creditMemoAttachment);

                 break;

               case RecordType.RejectionMemo:

                 var rejectionMemoAttachment = new RejectionMemoAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(rejectionMemoAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = rejectionMemoAttachment.Id + Path.GetExtension(fileName);

                 if (RejectionMemoAttachmentRepository == null)
                 {
                   RejectionMemoAttachmentRepository = new RejectionMemoAttachmentRepository();
                 }

                 RejectionMemoAttachmentRepository.Add(rejectionMemoAttachment);

                 break;

               case RecordType.BillingMemoCoupon:

                 var billingMemoCouponAttachment = new BMCouponAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(billingMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = billingMemoCouponAttachment.Id + Path.GetExtension(fileName);

                 if (BillingMemoCouponAttachmentRepository == null)
                 {
                   BillingMemoCouponAttachmentRepository = new BillingMemoCouponAttachmentRepository();
                 }

                 BillingMemoCouponAttachmentRepository.Add(billingMemoCouponAttachment);

                 break;

               case RecordType.CreditMemoCoupon:

                 var creditMemoCouponAttachment = new CMCouponAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(creditMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = creditMemoCouponAttachment.Id + Path.GetExtension(fileName);

                 if (CreditMemoCouponAttachmentRepository == null)
                 {
                   CreditMemoCouponAttachmentRepository = new CreditMemoCouponAttachmentRepository();
                 }

                 CreditMemoCouponAttachmentRepository.Add(creditMemoCouponAttachment);

                 break;

               case RecordType.RejectionMemoCoupon:

                 var rejectionMemoCouponAttachment = new RMCouponAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(rejectionMemoCouponAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = rejectionMemoCouponAttachment.Id + Path.GetExtension(fileName);

                 if (RejectionMemoCouponAttachmentRepository == null)
                 {
                   RejectionMemoCouponAttachmentRepository = new RejectionMemoCouponAttachmentRepository();
                 }

                 RejectionMemoCouponAttachmentRepository.Add(rejectionMemoCouponAttachment);

                 break;

               case RecordType.PrimeBillingCoupon:

                 var couponRecordAttachment = new PrimeCouponAttachment { ParentId = recordId, FilePath = filePath };

                 PopulateAttachment(couponRecordAttachment, fileName, originalFileName, criteria.BillingMemberId);

                 destinationFileName = couponRecordAttachment.Id + Path.GetExtension(fileName);

                 if (CouponRecordAttachmentRepository == null)
                 {
                   CouponRecordAttachmentRepository = new CouponRecordAttachmentRepository();
                 }

                 CouponRecordAttachmentRepository.Add(couponRecordAttachment);

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

                 break;
             }

             //Copy the file with attachment.Id as file name to Folder BBBBYYYYMM(Billed member id, Billing year, Billing month)
             CopyDocumentsToSfr(fileName, filePath);
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
         /// <returns></returns>
         public string LinkDocument(UnlinkedSupportingDocument unlinkedDocument)
         {
           var skipDuplicateCheck = false;
           var recordSearchCriteria = GetCriteria(unlinkedDocument);
           recordSearchCriteria.ChargeCategoryId = null;

           var supportingDocumentRecords = GetRecordListWithAttachments(recordSearchCriteria);
           if (supportingDocumentRecords != null && supportingDocumentRecords.Count > 0)
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
             recordSearchCriteria.IsFormD = !string.IsNullOrEmpty(recordSearchCriteria.FormDInvoiceNumber);

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

               return "Duplicate file found in unlinked documents! Document ignored"; // " and deleted from source path";
             }
             //build the attachment
             var fileServer = ReferenceManager.GetActiveUnlinkedDocumentsServer();
             var idFileName = recordSearchCriteria.FileName; // +Path.GetExtension(recordSearchCriteria.OriginalFileName);
             var idFile = Path.Combine(fileServer.BasePath, idFileName);
             var errorMesg = BuildAttachment(recordType, recordId, idFile, recordSearchCriteria.OriginalFileName, recordSearchCriteria);

             if (errorMesg != string.Empty)
             {
               return errorMesg;
             }

             //Delete from Unlinked document table
             DeleteUnlinkedDocument(unlinkedDocument);
           }
           else
           {
             //Logger.Info("Invoice do not have supporting documents.");
             return "Invoice or supporting document is not present.";
           }

           //Commit the transaction for all the attachment repositories
           UnitOfWork.CommitDefault();

           return string.Empty;
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
         /// <returns></returns>
         private string GetSupportingDocPathFromOfflineMetaData(RecordSearchCriteria criteria)
         {
           //var offlinePathForSupportDoc = string.Empty;
           var subFolderPath = string.Empty;

           if (criteria.BillingCategory.HasValue)
           {
             InvoiceOfflineCollectionMetaData offileMetaData;

             // In case of Form C don't use Period No.
             if (criteria.IsFormC)
             {
               offileMetaData =
                 InvoiceOfflineCollectionMetaDataRepository.Get(
                   filepath =>
                   filepath.BilledMemberCode == criteria.BilledMemberCode && filepath.BillingMemberCode == criteria.BillingMemberCode &&
                   (criteria.IsFormC ? filepath.ProvisionalBillingMonth == criteria.ClearanceMonth : filepath.BillingMonth == criteria.ClearanceMonth) &&
                   filepath.BillingCategoryId == criteria.BillingCategory.Value && filepath.IsFormC == criteria.IsFormC && filepath.OfflineCollectionFolderTypeId == 4).FirstOrDefault();
             }
             else
             {
               offileMetaData =
                 InvoiceOfflineCollectionMetaDataRepository.Get(
                   filepath =>
                   filepath.InvoiceNumber == (criteria.IsFormD ? criteria.FormDInvoiceNumber : criteria.InvoiceNumber) && filepath.BilledMemberCode == criteria.BilledMemberCode && filepath.BillingMemberCode == criteria.BillingMemberCode &&
                   filepath.PeriodNo == criteria.ClearancePeriod && (criteria.IsFormC ? filepath.ProvisionalBillingMonth == criteria.ClearanceMonth : filepath.BillingMonth == criteria.ClearanceMonth) &&
                   filepath.BillingCategoryId == criteria.BillingCategory.Value && filepath.IsFormC == criteria.IsFormC && filepath.OfflineCollectionFolderTypeId == 4).FirstOrDefault();
             }

             if (offileMetaData == null && criteria.IsFormC)
             {
               var rootPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR);

               Logger.InfoFormat("Creating first level folder [YearMonthPeriod]...");
               var folderPath = Path.Combine(rootPath, string.Format("{0}", GetFormattedBillingMonthYear(criteria.ClearanceMonth.Value, criteria.BillingYear.Value, PaxReportConstants.PaxFormCReportDateFormat))).ToUpper();
               //CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich).Period.ToString().PadLeft(2, '0')));)
               if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

               Logger.InfoFormat("Creating third level folder [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
               // e.g. PAX-022-014
               folderPath = Path.Combine(folderPath, string.Format("{0}-{1}-{2}", "PAX", criteria.BillingMemberCode, criteria.BilledMemberCode)).ToUpper();
               if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

               Logger.InfoFormat("Creating fourth level folder [{0}]...", "FORMC-YYYYMMPP");
               // e.g. FORMC-20110200
               folderPath = Path.Combine(folderPath, string.Format("FORMC-{0}", GetFormattedBillingMonthYear(criteria.ClearanceMonth.Value, criteria.BillingYear.Value, PaxReportConstants.PaxFormCReportDateFormat))).ToUpper();
               if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
               folderPath = Path.Combine(folderPath, "SUPPDOCS");
               if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

               subFolderPath = folderPath + "\\" + criteria.InvoiceNumber + "-" + criteria.BatchNumber.ToString().PadLeft(5, '0') + "-" +
                                   criteria.SequenceNumber.ToString().PadLeft(5, '0');
               if (!Directory.Exists(subFolderPath))
               {
                 Directory.CreateDirectory(subFolderPath);
               }
             }

             if (offileMetaData != null)
             {
               var offlinePathForSupportDoc = offileMetaData.FilePath;
               if (!string.IsNullOrEmpty(offlinePathForSupportDoc) && Directory.Exists(offlinePathForSupportDoc))
               {
                 //string subFolderPath;
                 if (criteria.IsFormC || criteria.IsFormD)
                 {
                   subFolderPath = offlinePathForSupportDoc + "\\" + criteria.InvoiceNumber + "-" + criteria.BatchNumber.ToString().PadLeft(5, '0') + "-" +
                                   criteria.SequenceNumber.ToString().PadLeft(5, '0');
                   if (!Directory.Exists(subFolderPath))
                   {
                     Directory.CreateDirectory(subFolderPath);
                   }
                 }
                 else
                 {
                   if (criteria.BillingCategory.Value != (int)BillingCategoryType.Misc)
                   {
                     subFolderPath = offlinePathForSupportDoc + "\\" + criteria.BatchNumber.ToString().PadLeft(5, '0') + "-" + criteria.SequenceNumber.ToString().PadLeft(5, '0');
                     if (!Directory.Exists(subFolderPath))
                     {
                       Directory.CreateDirectory(subFolderPath);
                     }
                   }
                   else
                   {
                     subFolderPath = offlinePathForSupportDoc;
                   }
                 }


                 if (!criteria.IsFormC && !criteria.IsFormD && criteria.BreakdownSerialNumber != null && criteria.BreakdownSerialNumber != 0)
                 {
                   subFolderPath = subFolderPath + "\\" + criteria.BreakdownSerialNumber.ToString().PadLeft(5, '0');
                   if (!Directory.Exists(subFolderPath))
                   {
                     Directory.CreateDirectory(subFolderPath);
                   }
                 }
               }
             }
           }

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

         private bool DeleteDocumentsFromSourceDirectory(string directoryName)
         {
           if (Directory.Exists(directoryName))
           {
             Directory.Delete(directoryName, true);

             return true;
           }
           return false;
         }
           */
    #endregion
      #endregion
      #endregion
  }
}
