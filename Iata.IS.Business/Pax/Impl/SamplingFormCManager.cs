using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Enums;
using System.IO;
using Iata.IS.Core.DI;

namespace Iata.IS.Business.Pax.Impl
{
  public class SamplingFormCManager : InvoiceManagerBase, ISamplingFormCManager, IValidationSFormCManager
  {
    private const string NilFormCIndicatorY = "Y";
    private const string NilFormCIndicatorS = "S";
    private const string NilFormCIndicatorN = "N";

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ISamplingFormCRepository SamplingFormCRepository { get; set; }

    /// <summary>
    /// Source code total repository, will be injected by the container
    /// </summary>
    /// <value>The source code total repository.</value>
    public IRepository<SourceCodeTotal> SourceCodeTotalRepository { get; set; }

    /// <summary>
    /// Invoice repository, will be injected by the container
    /// </summary>
    /// <value>The invoice repository.</value>
    public ICouponRecordRepository CouponRecordRepository { get; set; }

    /// <summary>
    /// Sampling Form C repository, will be injected by the container
    /// </summary>
    /// <value>The sampling form C record repository.</value>
    public ISamplingFormCRecordRepository SamplingFormCRecordRepository { get; set; }

    /// <summary>
    /// SamplingFormCAttachmentRepository, will be injected by the container
    /// </summary>
    /// <value>The sampling form C attachment repository.</value>
    public ISamplingFormCAttachmentRepository SamplingFormCAttachmentRepository { get; set; }

    /// <summary>
    /// Sample Digit repository.
    /// </summary>
    public IRepository<SampleDigit> SampleDigitRepository { get; set; }

    public IRepository<ReasonCode> ReasonCodeRepository { get; set; }
    public IRepository<InvoiceBase> InvoiceBaseRepoeitory { get; set; }

    public IInvoiceRepository PaxInvoiceRepository { get; set; }
    private readonly ICalendarManager _calenderManager;

    //SCP:186702 - LH file in production 
    private Dictionary<string, PaxInvoice> ProvisionalInvoiceList = new Dictionary<string, PaxInvoice>();

    /* CMP #596: Length of Member Accounting Code to be Increased to 12.
     * Desc: Object created to call isTypeBMember() common method, for implementing #MW2 validation.
    */
    public IInvoiceManager InvoiceManager { get; set; }

    public SamplingFormCManager(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }

    
    public SamplingFormCManager()
    {
    }

    /// <summary>
    /// Gets invoices matching the specified search criteria
    /// </summary>
    /// <returns>Sampling form c matching to search criteria</returns>
    public IList<SamplingFormCResultSet> GetSamplingFormCList(SearchCriteria searchCriteria)
    {
      var filteredList = SamplingFormCRepository.GetSamplingFormCList(searchCriteria.BillingMonth, searchCriteria.BillingYear,
                                                                      searchCriteria.BillingMemberId, searchCriteria.InvoiceStatusId,
                                                                      searchCriteria.BilledMemberId);

      return filteredList;
    }

    /// <summary>
    /// Gets payables Form C list matching the specified search criteria
    /// </summary>
    /// <returns>Sampling form C matching to search criteria</returns>
    public IList<SamplingFormCResultSet> GetSamplingFormCPayablesList(SearchCriteria searchCriteria)
    {
      var filteredList = SamplingFormCRepository.GetSamplingFormCPayablesList(searchCriteria.BillingMonth, searchCriteria.BillingYear, searchCriteria.BillingMemberId, searchCriteria.BilledMemberId);

      return filteredList;
    }

    /// <summary>
    /// Creates the sampling form C.
    /// </summary>
    /// <param name="samplingFormCHeader">Sampling form C header to be created</param>
    /// <returns></returns>
    public SamplingFormC CreateSamplingFormC(SamplingFormC samplingFormCHeader)
    {
      // For Nil Form C, listing currency is not required.
      if (samplingFormCHeader.NilFormCIndicator == NilFormCIndicatorY) samplingFormCHeader.ListingCurrencyId = null;

      ValidateSamplingFormC(samplingFormCHeader, null);
      samplingFormCHeader.InvoiceStatus = InvoiceStatusType.Open;

      SamplingFormCRepository.Add(samplingFormCHeader);
      UnitOfWork.CommitDefault();

      return samplingFormCHeader;
    }
    //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    

    /// <summary>
    /// Updates the Sampling Form C header information.
    /// </summary>
    /// <param name="samplingFormC">Sampling form C to be updated.</param>
    /// <returns></returns>
    public SamplingFormC UpdateSamplingFormC(SamplingFormC samplingFormC)
    {
      // For Nil Form C, listing currency is not required.
      if (samplingFormC.NilFormCIndicator == NilFormCIndicatorY) samplingFormC.ListingCurrencyId = null;
      var samplingFormCInDb = SamplingFormCRepository.Single(sfc => sfc.Id == samplingFormC.Id);

      ValidateSamplingFormC(samplingFormC, samplingFormCInDb);

      var updatedSamplingFormC = SamplingFormCRepository.Update(samplingFormC);
      UnitOfWork.CommitDefault();

      return updatedSamplingFormC;
    }


    /// <summary>
    /// Deletes a sampling form C.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id to be deleted</param>
    /// <returns>True if successfully deleted, false otherwise</returns>
    public bool DeleteSamplingFormC(string samplingFormCId)
    {
      var samplingFormCGuid = samplingFormCId.ToGuid();
      var samplingFormCToBeDeleted = SamplingFormCRepository.Single(inv => inv.Id == samplingFormCGuid);
      if (samplingFormCToBeDeleted == null) return false;
      SamplingFormCRepository.Delete(samplingFormCToBeDeleted);
      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// Deletes a sampling form C.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatus">The invoice status.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>
    /// True if successfully deleted, false otherwise
    /// </returns>
    public bool DeleteSamplingFormC(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int invoiceStatus, int? listingCurrencyId)
    {
      SamplingFormC samplingFormCToBeDeleted;
      // As per business rule only one record will be available for any status except Ready for billing, (record can not be deleted in ready for billing status).
      if (listingCurrencyId == null)
        samplingFormCToBeDeleted = SamplingFormCRepository.First(sfc => sfc.ProvisionalBillingMonth == provisionalBillingMonth
                                                                           && sfc.ProvisionalBillingYear == provisionalBillingYear
                                                                           && sfc.ProvisionalBillingMemberId == provisionalBillingMemberId
                                                                           && sfc.FromMemberId == fromMemberId
                                                                           && sfc.InvoiceStatusId == invoiceStatus
                                                                           && sfc.ListingCurrencyId == null);
      else
      {
        samplingFormCToBeDeleted = SamplingFormCRepository.First(sfc => sfc.ProvisionalBillingMonth == provisionalBillingMonth
                                                                           && sfc.ProvisionalBillingYear == provisionalBillingYear
                                                                           && sfc.ProvisionalBillingMemberId == provisionalBillingMemberId
                                                                           && sfc.FromMemberId == fromMemberId
                                                                           && sfc.InvoiceStatusId == invoiceStatus
                                                                           && sfc.ListingCurrencyId == listingCurrencyId);
      }

      if (samplingFormCToBeDeleted == null) return false;

      SamplingFormCRepository.Delete(samplingFormCToBeDeleted);

      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// Validates a sampling form C.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id</param>
    /// <returns>True if successfully validated, false otherwise</returns>
    public SamplingFormC ValidateSamplingFormC(string samplingFormCId)
    {
      var webValidationErrors = new List<WebValidationError>();
      var samplingFormCGuid = samplingFormCId.ToGuid();
      var samplingFromC = SamplingFormCRepository.Single(sfc => sfc.Id == samplingFormCGuid);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(samplingFormCId);

      // Sampling form c already validated then return the sampling form c.
      if (samplingFromC.InvoiceStatus == InvoiceStatusType.ReadyForSubmission) return samplingFromC;

      if (samplingFromC.NilFormCIndicator.Equals(NilFormCIndicatorN))
      {
        // At least one transaction/line item should be present.
        var isTransactionRecordExists = SamplingFormCRecordRepository.GetCount(sfcRecord => sfcRecord.SamplingFormCId == samplingFormCGuid) > 0;
        if (!isTransactionRecordExists)
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(samplingFromC.Id, SamplingErrorCodes.TransactionLineItemNotAvailable));
        }
      }

      //// Blocked by Debtor.
      //if (CheckBlockedMember(true, samplingFromC.ProvisionalBillingMemberId, samplingFromC.FromMemberId, true))
      //{
      //  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(samplingFromC.Id, ErrorCodes.InvalidBillingToMember));
      //}

      //// Blocked by Creditor.
      //if (CheckBlockedMember(false, samplingFromC.FromMemberId, samplingFromC.ProvisionalBillingMemberId, true))
      //{
      //  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(samplingFromC.Id, ErrorCodes.InvalidBillingFromMember));
      //}

      // Update invoice status in case of error.
      if (webValidationErrors.Count > 0)
      {
        samplingFromC.InvoiceStatus = InvoiceStatusType.ValidationError;
        samplingFromC.ValidationErrors.AddRange(webValidationErrors);

        if (samplingFromC.ValidationStatus != (int)InvoiceValidationStatus.ErrorPeriod)
          samplingFromC.ValidationStatus = (int)InvoiceValidationStatus.Failed;
        // Update validation errors in db.
      }
      else
      {
        // Every validation is successful. Update invoice status as Ready for billing and invoice date as current date.
        samplingFromC.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;
      }

      ValidationErrorManager.UpdateValidationErrors(samplingFromC.Id, samplingFromC.ValidationErrors, validationErrorsInDb);


      SamplingFormCRepository.Update(samplingFromC);

      UnitOfWork.CommitDefault();

      return samplingFromC;
    }

    /// <summary>
    /// Submits the sampling form C.
    /// </summary>
    /// <param name="samplingFormDetailsList"></param>
    /// <returns></returns>
    public IList<SamplingFormCResultSet> SubmitSamplingFormC(List<SamplingFormCResultSet> samplingFormDetailsList)
    {
      var samplingFormCList = new List<SamplingFormCResultSet>();

      foreach (var samplingFormCDetail in samplingFormDetailsList)
      {
        var detail = samplingFormCDetail;
        SamplingFormC samplingFormC;
        if (detail.ListingCurrencyId == null)
          samplingFormC = SamplingFormCRepository.First(sfc => sfc.ProvisionalBillingMemberId == detail.ProvisionalBillingMemberId
                                                                  && sfc.ProvisionalBillingYear == detail.ProvisionalBillingYear
                                                                  && sfc.ProvisionalBillingMonth == detail.ProvisionalBillingMonth
                                                                  && sfc.ListingCurrencyId == null
                                                                  && sfc.FromMemberId == detail.FromMemberId
                                                                  && sfc.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission);
        else
        {
          samplingFormC = SamplingFormCRepository.First(sfc => sfc.ProvisionalBillingMemberId == detail.ProvisionalBillingMemberId
                                                                 && sfc.ProvisionalBillingYear == detail.ProvisionalBillingYear
                                                                 && sfc.ProvisionalBillingMonth == detail.ProvisionalBillingMonth
                                                                 && sfc.ListingCurrencyId == detail.ListingCurrencyId
                                                                 && sfc.FromMemberId == detail.FromMemberId
                                                                 && sfc.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission);
        }

        if (samplingFormC == null) continue;

        samplingFormC = ValidateOnSubmit(samplingFormC);
        detail.InvoiceStatusId = samplingFormC.InvoiceStatusId;

        /* Commented below code to not set Expiry date at this moment For PAX Trans only */
          // Update expiry period of sampling form C for purging.
          // DateTime expiryPeriod = GetFormCExpiryDatePeriodMethod(samplingFormC);
          //samplingFormC.ExpiryDatePeriod = expiryPeriod;

        SamplingFormCRepository.Update(samplingFormC);

        if (detail.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling)
        {
          samplingFormCList.Add(detail);
          // Execute StoredProcedure which will update FormC SourceCode total
          SamplingFormCRepository.UpdateFormCSourceCodeTotal(samplingFormC.Id);
        }
      }

      UnitOfWork.CommitDefault();

      return samplingFormCList;
    }

    /// <summary>
    /// Presents the sampling form C.
    /// </summary>
    /// <param name="samplingFormDetailsList"></param>
    /// <returns></returns>
    public IList<SamplingFormCResultSet> PresentSamplingFormC(List<SamplingFormCResultSet> samplingFormDetailsList)
    {
      var samplingFormCList = new List<SamplingFormCResultSet>();

      foreach (var samplingFormCDetail in samplingFormDetailsList)
      {
        var detail = samplingFormCDetail;
        IQueryable<SamplingFormC> samplingFormCs;
        if (detail.ListingCurrencyId == null)
          samplingFormCs = SamplingFormCRepository.Get(sfc => sfc.ProvisionalBillingMemberId == detail.ProvisionalBillingMemberId
                                                                 && sfc.ProvisionalBillingYear == detail.ProvisionalBillingYear
                                                                 && sfc.ProvisionalBillingMonth == detail.ProvisionalBillingMonth
                                                                 && sfc.ListingCurrencyId == null
                                                                 && sfc.FromMemberId == detail.FromMemberId
                                                                 && sfc.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling);
        else
        {
          samplingFormCs = SamplingFormCRepository.Get(sfc => sfc.ProvisionalBillingMemberId == detail.ProvisionalBillingMemberId
                                                                 && sfc.ProvisionalBillingYear == detail.ProvisionalBillingYear
                                                                 && sfc.ProvisionalBillingMonth == detail.ProvisionalBillingMonth
                                                                 && sfc.ListingCurrencyId == detail.ListingCurrencyId
                                                                 && sfc.FromMemberId == detail.FromMemberId
                                                                 && sfc.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling);
        }

        if (samplingFormCs == null || samplingFormCs.Count() == 0) continue;

        foreach (var samplingFormC in samplingFormCs)
        {
          samplingFormC.InvoiceStatus = InvoiceStatusType.Presented;
          SamplingFormCRepository.Update(samplingFormC);
        }
        samplingFormCList.Add(detail);
      }

      UnitOfWork.CommitDefault();

      return samplingFormCList;
    }

    public SamplingFormC SubmitSamplingFormC(string samplingFormCId)
    {

      try
      {
        
        var samplingFormCGuid = samplingFormCId.ToGuid();
        var samplingFormC = SamplingFormCRepository.Single(formC => formC.Id == samplingFormCGuid);

        samplingFormC = ValidateOnSubmit(samplingFormC);

        //SCP 212028 - Missing data in SFI30 Airline 27 Oct P4 Incoming Form C
        if (samplingFormC.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
          // Call stored procedure which will update SourceCode total value
          UpdateFormCSourceCodeTotal(samplingFormCId.ToGuid());
        }

        SamplingFormCRepository.Update(samplingFormC);

        UnitOfWork.CommitDefault();
        //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
        // Update expiry period of sampling form C for purging.
        //DateTime expiryPeriod = GetFormCExpiryDatePeriodMethod(samplingFormC);

        //InvoiceRepository.UpdateExpiryDatePeriod(samplingFormC.Id, (int) TransactionType.SamplingFormC, expiryPeriod);

        return samplingFormC;
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat("Error occurred in SubmitSamplingFormC method ",exception);
        return null;

      }
    }

    /// <summary>
    /// Submits the sampling form C.
    /// </summary>
    /// <param name="samplingFormCIdList">List of sampling form c ids to be submitted</param>
    /// <returns></returns>
    public IList<SamplingFormC> SubmitSamplingFormC(List<string> samplingFormCIdList)
    {
     
       try
       {
         var samplingFomrCGuidList = samplingFormCIdList.Select(Guid.Parse);

         var samplingFormCs =
           SamplingFormCRepository.Get(
             sfc =>
             samplingFomrCGuidList.Contains(sfc.Id) && sfc.InvoiceStatusId == (int) InvoiceStatusType.ReadyForSubmission);
         var samplingFormCList =
           samplingFormCs.AsEnumerable().Select(samplingFormCdata => ValidateOnSubmit(samplingFormCdata)).Select(
             samplingFormCdata => SamplingFormCRepository.Update(samplingFormCdata)).ToList();

         foreach (var samplingFormC in samplingFormCList)
         {
           //SCP 212028 - Missing data in SFI30 Airline 27 Oct P4 Incoming Form C
           if (samplingFormC.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
           {
            // Call stored procedure which will update SourceCode total value
             UpdateFormCSourceCodeTotal(samplingFormC.Id);
           }
         }

         UnitOfWork.CommitDefault();

         return samplingFormCList;
       }
       catch (Exception exception)
       {
         Logger.ErrorFormat("Error occurred in SubmitSamplingFormC method ", exception);
         throw;
       }

    }

    /// <summary>
    /// Gets the sampling form c source code total list for given sampling form c id
    /// </summary>
    /// <param name="samplingFormCId">string of sampling form C id</param>
    /// <returns>Source code list for sampling form c id</returns>
    public IQueryable<SourceCodeTotal> GetSamplingFormCSourceCodeTotal(string samplingFormCId)
    {
      return SourceCodeTotalRepository.GetAll();
    }

    /// <summary>
    /// Gets the sampling form c source code total list for given sampling form c id
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatusId">The invoice status id.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>Source code list for sampling form c id</returns>
    public IList<SamplingFormCSourceTotal> GetSamplingFormCSourceCodeTotal(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int invoiceStatusId, int? listingCurrencyId)
    {
      return SamplingFormCRepository.GetSamplingFormCSourceTotalList(provisionalBillingMonth, provisionalBillingYear, fromMemberId, invoiceStatusId, provisionalBillingMemberId, listingCurrencyId);
    }

    /// <summary>
    /// To get the list of Sampling Form C records.
    /// </summary>
    /// <param name="samplingFormCId">string of sampling form C id</param>
    /// <returns>list of sampling form c records</returns>
    public IList<SamplingFormCRecord> GetSamplingFormCRecordList(string samplingFormCId)
    {
      var samplingFormCGuid = samplingFormCId.ToGuid();
      var samplingFormCRecordList = SamplingFormCRecordRepository.Get(samplingFormCRecord => samplingFormCRecord.SamplingFormCId == samplingFormCGuid).ToList();
      return samplingFormCRecordList;
    }

    /// <summary>
    /// To get the list of Sampling Form C records.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="statusId">The status id.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>list of sampling form c records</returns>
    public IList<SamplingFormCRecord> GetSamplingFormCRecordList(int provisionalBillingMonth, int provisionalBillingYear, int provisionalBillingMemberId, int fromMemberId, int statusId, int? listingCurrencyId)
    {
      // TODO: Need to remove member id related parameter from LINQ query.
      var samplingFormCList = SamplingFormCRepository.GetSamplingFormCDetails(sfc => sfc.ProvisionalBillingMonth == provisionalBillingMonth
                                                                                     && sfc.ProvisionalBillingYear == provisionalBillingYear
                                                                                     && sfc.ProvisionalBillingMemberId == provisionalBillingMemberId
                                                                                     && sfc.FromMemberId == fromMemberId
                                                                                     && sfc.InvoiceStatusId == statusId
                                                                                     && sfc.ListingCurrencyId == listingCurrencyId);

      //// Call replaced by Load strategy 
      //var samplingFormCList = SamplingFormCRepository.GetSamplingFormCDetails(provisionalBillingMonth, provisionalBillingYear, provisionalBillingMemberId: provisionalBillingMemberId, fromMemberId: fromMemberId,
      //invoiceStatusIds: statusId.ToString(), listingCurrencyCodeNum: listingCurrencyId);


      var samplingFormCRecordList = samplingFormCList.SelectMany(sfcItem => sfcItem.SamplingFormCDetails).ToList();

      var reasonCodes = samplingFormCRecordList.Select(sfcRecord => sfcRecord.ReasonCode.ToUpper());
      var reasonCodesfromDb = ReasonCodeRepository.Get(reasonCode => reasonCodes.Contains(reasonCode.Code.ToUpper())).ToList();

      if (reasonCodesfromDb.Count() > 0)
      {
        foreach (var samplingFormCRecord in samplingFormCRecordList)
        {
          var record = samplingFormCRecord;
          // SCP162502: Form C - AC OAR Jul P3 failure - No alert received
          var resonCodeRecord = reasonCodesfromDb.FirstOrDefault(rCode => rCode.Code == record.ReasonCode &&
                                                                          rCode.TransactionTypeId == (int) TransactionType.SamplingFormC);
          samplingFormCRecord.ReasonCodeDescription = resonCodeRecord != null ? resonCodeRecord.Description : null;
        }
      }

      return samplingFormCRecordList;
    }

    /// <summary>
    /// Gets the sampling form c record details.
    /// </summary>
    /// <param name="samplingFormCRecordId">string of sampling form C record id</param>
    /// <returns>Details of the sampling form c record matching with samplingFormCRecordId</returns>
    public SamplingFormCRecord GetSamplingFormCRecordDetails(string samplingFormCRecordId)
    {
      var samplingFormCRecordGuid = samplingFormCRecordId.ToGuid();
      //var samplingFormCRecord = SamplingFormCRecordRepository.Single(sFormCRecord => sFormCRecord.Id == samplingFormCRecordGuid);
      // Call replaced by load strategy
      var samplingFormCRecord = SamplingFormCRecordRepository.Single(samplingFormCRecordId: samplingFormCRecordGuid);
      return samplingFormCRecord;
    }

    /// <summary>
    /// Adds the sampling form C record in database.
    /// </summary>
    /// <param name="samplingFormCRecord">Details of sampling form c record to be added</param>
    /// <returns>added sampling form c record details</returns>
    public SamplingFormCRecord AddSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord)
    {
      var samplingFormCHeader = SamplingFormCRepository.Single(sfcHeader => sfcHeader.Id == samplingFormCRecord.SamplingFormCId);
      ValidateSamplingFormCRecord(samplingFormCRecord, null, samplingFormCHeader);

      // Throws an error if Form C header's NilFormCIndicator is yes 
      IsFormCWithNilIndicator(samplingFormCHeader);

      // Form C can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (samplingFormCHeader.InvoiceStatus != InvoiceStatusType.Open)
      {
        samplingFormCHeader.InvoiceStatus = InvoiceStatusType.Open;
        SamplingFormCRepository.Update(samplingFormCHeader);
      }

      SamplingFormCRecordRepository.Add(samplingFormCRecord);
      UnitOfWork.CommitDefault();

      return samplingFormCRecord;
    }

    /// <summary>
    /// Determines whether [is form C with nil indicator] [the specified sampling form C header].
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    private static void IsFormCWithNilIndicator(SamplingFormC samplingFormCHeader)
    {
      if (samplingFormCHeader.NilFormCIndicator.Equals(NilFormCIndicatorY))
      {
        throw new ISBusinessException(SamplingErrorCodes.FormCHeaderWithNilIndicator);
      }
    }

    /// <summary>
    /// Updates the sampling form C record in database.
    /// </summary>
    /// <param name="samplingFormCRecord">Details of sampling form C record to be updated</param>
    /// <returns>updated sampling form c record details</returns>
    public SamplingFormCRecord UpdateSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord)
    {
      //var sFormCRecord = SamplingFormCRecordRepository.Single(sfcRecord => sfcRecord.Id == samplingFormCRecord.Id);
      // Call replaced by Load strategy
      var sFormCRecord = SamplingFormCRecordRepository.Single(samplingFormCRecordId: samplingFormCRecord.Id);

      var samplingFormCHeader = SamplingFormCRepository.Single(sfcHeader => sfcHeader.Id == samplingFormCRecord.SamplingFormCId);

      ValidateSamplingFormCRecord(samplingFormCRecord, sFormCRecord, samplingFormCHeader);

      // Throws an error if Form C header's NilFormCIndicator is yes 
      IsFormCWithNilIndicator(samplingFormCHeader);

      // Form C can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (samplingFormCHeader.InvoiceStatus != InvoiceStatusType.Open)
      {
        samplingFormCHeader.InvoiceStatus = InvoiceStatusType.Open;
        SamplingFormCRepository.Update(samplingFormCHeader);
      }

      var updatedSamplingFormCRecord = SamplingFormCRecordRepository.Update(samplingFormCRecord);

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = sFormCRecord.Attachments.Where(attachment => samplingFormCRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in samplingFormCRecord.Attachments
                              where sFormCRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var attachmentInDb = SamplingFormCAttachmentRepository.Get(sfcAttachment => attachmentIdList.Contains(sfcAttachment.Id));
      foreach (var sfcRecordAttachment in attachmentInDb)
      {
        if (IsDuplicateSamplingFormCRecordAttachmentFileName(sfcRecordAttachment.OriginalFileName, samplingFormCRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        sfcRecordAttachment.ParentId = samplingFormCRecord.Id;
        SamplingFormCAttachmentRepository.Update(sfcRecordAttachment);
      }

      foreach (var formCRecordAttachment in listToDeleteAttachment)
      {
        SamplingFormCAttachmentRepository.Delete(formCRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      return updatedSamplingFormCRecord;
    }

    /// <summary>
    /// Validates the sampling form C record.
    /// </summary>
    /// <param name="samplingFormCRecord">The sampling form C record.</param>
    /// <param name="samplingFormCRecordInDb">The sampling form C record in db.</param>
    /// <param name="samplingFormC">The sampling form C.</param>
    public void ValidateSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord, SamplingFormCRecord samplingFormCRecordInDb, SamplingFormC samplingFormC)
    {
      var isUpdateOperation = false;

      //Check whether it's a update operation.
      if (samplingFormCRecordInDb != null)
      {
        isUpdateOperation = true;
      }
     
      if (!isUpdateOperation || CompareUtil.IsDirty(samplingFormCRecordInDb.TicketIssuingAirline, samplingFormCRecord.TicketIssuingAirline) ||
       CompareUtil.IsDirty(samplingFormCRecordInDb.CouponNumber, samplingFormCRecord.CouponNumber) || CompareUtil.IsDirty(samplingFormCRecordInDb.DocumentNumber, samplingFormCRecord.DocumentNumber) ||
       CompareUtil.IsDirty(samplingFormCRecordInDb.BatchNumberOfProvisionalInvoice, samplingFormCRecord.BatchNumberOfProvisionalInvoice) ||
       CompareUtil.IsDirty(samplingFormCRecordInDb.RecordSeqNumberOfProvisionalInvoice, samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice) ||
     CompareUtil.IsDirty(samplingFormCRecordInDb.SamplingFormC.ProvisionalBillingMonth, samplingFormC.ProvisionalBillingMonth) ||
     CompareUtil.IsDirty(samplingFormCRecordInDb.SamplingFormC.ProvisionalBillingYear, samplingFormC.ProvisionalBillingYear))

      {
        if (IsDulplicateSamplingFormCRecordExists(samplingFormCRecord, samplingFormC))
        {
          throw new ISBusinessException(SamplingErrorCodes.DuplicateSamplingFormCRecordFound);
        }
      }

      if (samplingFormCRecord.DocumentNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTicketDocumnetOrFimNumber);
      }

      // Validate reason code already exist for Form C source code and billing code.
      if (!isUpdateOperation || CompareUtil.IsDirty(samplingFormCRecordInDb.ReasonCode, samplingFormCRecord.ReasonCode))
      {
        if (!ReferenceManager.IsValidReasonCode(samplingFormCRecord.ReasonCode, (int)TransactionType.SamplingFormC))
        {
          throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
        }
      }

      // Form C record field validations
      if (samplingFormC.SubmissionMethod != SubmissionMethod.IsWeb)
      {
        ValidateParsedSamplingFormCRecord(samplingFormCRecord, new List<IsValidationExceptionDetail>(), samplingFormC, string.Empty, new Dictionary<string, bool>());
      }
      else
      {
        if (IsProvisionalBillingMemberMigrated(samplingFormC))
        {
          ValidateParsedSamplingFormCRecord(samplingFormCRecord, null, samplingFormC, string.Empty, new Dictionary<string, bool>());
        }
      }

      //Validate Batch Sequence Number and Record Sequence Number
      if (samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice <= 0 || samplingFormCRecord.BatchNumberOfProvisionalInvoice <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }
    }


    /// <summary>
    /// Deletes the sampling form C record from database for given sampling form c record id
    /// </summary>
    /// <param name="samplingFormCRecordId">string of sampling form C record id</param>
    /// <returns>True if successfully deleted,false otherwise</returns>
    public bool DeleteSamplingFormCRecord(string samplingFormCRecordId)
    {
      var samplingFormCRecordGuid = samplingFormCRecordId.ToGuid();
      //var samplingFormCToBeDeleted = SamplingFormCRecordRepository.Single(sfcRecord => sfcRecord.Id == samplingFormCRecordGuid);
      //Call replaced by load strategy
      var samplingFormCToBeDeleted = SamplingFormCRecordRepository.Single(samplingFormCRecordId: samplingFormCRecordGuid);
      if (samplingFormCToBeDeleted == null) return false;

      var samplingFormCHeader = SamplingFormCRepository.Single(sfcHeader => sfcHeader.Id == samplingFormCToBeDeleted.SamplingFormCId);

      // Throws an error if Form C header's NilFormCIndicator is yes 
      IsFormCWithNilIndicator(samplingFormCHeader);

      // Form C can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (samplingFormCHeader.InvoiceStatus != InvoiceStatusType.Open)
      {
        samplingFormCHeader.InvoiceStatus = InvoiceStatusType.Open;
        SamplingFormCRepository.Update(samplingFormCHeader);
      }

      SamplingFormCRecordRepository.Delete(samplingFormCToBeDeleted);
      UnitOfWork.CommitDefault();

      return true;
    }

    /// <summary>
    /// Gets the sampling form C record attachment details.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    public SamplingFormCRecordAttachment GetSamplingFormCRecordAttachmentDetails(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = SamplingFormCAttachmentRepository.Single(sfcAttachment => sfcAttachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Adds the sampling form C record attachment.
    /// </summary>
    /// <param name="attachment">The attachment.</param>
    /// <returns></returns>
    public SamplingFormCRecordAttachment AddSamplingFormCRecordAttachment(SamplingFormCRecordAttachment attachment)
    {
      SamplingFormCAttachmentRepository.Add(attachment);
      UnitOfWork.CommitDefault();

      return attachment;
    }

    /// <summary>
    /// Updates the sampling form C record attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    public IList<SamplingFormCRecordAttachment> UpdateSamplingFormCRecordAttachment(IList<Guid> attachments, Guid parentId)
    {
      var attachmentInDb = SamplingFormCAttachmentRepository.Get(sfcAttachment => attachments.Contains(sfcAttachment.Id));
      foreach (var sfcRecordAttachment in attachmentInDb)
      {
        sfcRecordAttachment.ParentId = parentId;
        SamplingFormCAttachmentRepository.Update(sfcRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      return attachmentInDb.ToList();
    }

    /// <summary>
    /// Determines whether [is duplicate sampling form C record attachment file name] [the specified file name].
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="samplingFormCRecordId">The sampling form C record id.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate sampling form C record attachment file name] [the specified file name]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsDuplicateSamplingFormCRecordAttachmentFileName(string fileName, Guid samplingFormCRecordId)
    {
      return SamplingFormCAttachmentRepository.GetCount(attachment => attachment.ParentId == samplingFormCRecordId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Gets the rejection memo coupon attachment.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<SamplingFormCRecordAttachment> GetSamplingFormCRecordAttachments(List<Guid> attachmentIds)
    {
      return SamplingFormCAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c details</returns>
    public SamplingFormC GetSamplingFormCDetails(string samplingFormCId)
    {
      var samplingFormCGuid = samplingFormCId.ToGuid();
      var samplingFormCHeader = SamplingFormCRepository.Single(samplingFormC => samplingFormC.Id == samplingFormCGuid);

      if (samplingFormCHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        samplingFormCHeader.ValidationErrors = ValidationErrorManager.GetValidationErrors(samplingFormCId).ToList();
      }

      // Get total gross amount for sampling form c items.
      var totalGrossAmount = SamplingFormCRecordRepository.Get(sfcRecord => sfcRecord.SamplingFormCId == samplingFormCGuid).Sum(record => record.GrossAmountAlf);
      samplingFormCHeader.TotalGrossAmount = totalGrossAmount;
      samplingFormCHeader.NumberOfRecords = SamplingFormCRecordRepository.GetCount(sfcRecord => sfcRecord.SamplingFormCId == samplingFormCGuid);

      return samplingFormCHeader;
    }

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c details</returns>
    public SamplingFormC GetSamplingFormCDetailsForAttachmentUpload(string samplingFormCId)
    {
        var samplingFormCGuid = samplingFormCId.ToGuid();
        var samplingFormCHeader = SamplingFormCRepository.Get(samplingFormC => samplingFormC.Id == samplingFormCGuid).SingleOrDefault();
        return samplingFormCHeader;
    }

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <param name="provisionalBillingMemberId">The provisional billing member id.</param>
    /// <param name="fromMemberId">From member id.</param>
    /// <param name="invoiceStatus">The invoice status.</param>
    /// <param name="listingCurrencyId">The listing currency id.</param>
    /// <returns>Sampling form c details</returns>
    public SamplingFormC GetSamplingFormCDetails(int provisionalBillingMonth, int provisionalBillingYear,
                                                 int provisionalBillingMemberId, int fromMemberId, int invoiceStatus, int? listingCurrencyId)
    {
      // TODO: Need to remove member id related parameter from LINQ query.
      IQueryable<SamplingFormC> samplingFormCHeaders;
      SamplingFormCRepository = SamplingFormCRepository ?? Ioc.Resolve<ISamplingFormCRepository>(typeof(ISamplingFormCRepository));
      if (listingCurrencyId == null)
        samplingFormCHeaders = SamplingFormCRepository.GetSamplingFormCDetails(samplingFormC => samplingFormC.ProvisionalBillingMonth == provisionalBillingMonth
                                                                                                  && samplingFormC.ProvisionalBillingYear == provisionalBillingYear
                                                                                                  && samplingFormC.ProvisionalBillingMemberId == provisionalBillingMemberId
                                                                                                  && samplingFormC.FromMemberId == fromMemberId
                                                                                                  && samplingFormC.InvoiceStatusId == invoiceStatus
                                                                                                  && samplingFormC.ListingCurrencyId == null);
      else
      {
        samplingFormCHeaders = SamplingFormCRepository.GetSamplingFormCDetails(samplingFormC => samplingFormC.ProvisionalBillingMonth == provisionalBillingMonth
                                                                                                  && samplingFormC.ProvisionalBillingYear == provisionalBillingYear
                                                                                                  && samplingFormC.ProvisionalBillingMemberId == provisionalBillingMemberId
                                                                                                  && samplingFormC.FromMemberId == fromMemberId
                                                                                                  && samplingFormC.InvoiceStatusId == invoiceStatus
                                                                                                  && samplingFormC.ListingCurrencyId == listingCurrencyId);

      }
      //// Call replaced by Load strategy
      //var samplingFormCHeaders = SamplingFormCRepository.GetSamplingFormCDetails(provisionalBillingMonth: provisionalBillingMonth,
      //                                                                           provisionalBillingYear: provisionalBillingYear,
      //                                                                           provisionalBillingMemberId: provisionalBillingMemberId,
      //                                                                           fromMemberId: fromMemberId,
      //                                                                           invoiceStatusIds: invoiceStatus.ToString(),
      //                                                                           listingCurrencyCodeNum: listingCurrencyId);


      if (samplingFormCHeaders == null || samplingFormCHeaders.Count() == 0) return null;

      var defaultSamplingFormC = listingCurrencyId == null ?
        samplingFormCHeaders.FirstOrDefault(sfc => sfc.ListingCurrencyId == null && sfc.InvoiceStatusId == invoiceStatus)
        : samplingFormCHeaders.FirstOrDefault(sfc => sfc.ListingCurrencyId == listingCurrencyId && sfc.InvoiceStatusId == invoiceStatus);

      var listingCurrency = defaultSamplingFormC.ListingCurrency;
      var provisionalBillingMember = defaultSamplingFormC.ProvisionalBillingMember;
      var fromMember = defaultSamplingFormC.FromMember;
      var nilFormCIndicator = defaultSamplingFormC.NilFormCIndicator;
      var numberOfRecords = Enumerable.Sum(samplingFormCHeaders, samplingFormC => samplingFormC.SamplingFormCDetails.Count);

      //Added ProcessingComplete status
      var totalGrossAmountAlf = Enumerable.Sum(samplingFormCHeaders, sfcItem => sfcItem.SamplingFormCDetails.Sum(s => s.GrossAmountAlf));
      //SCP162502: Form C - AC OAR Jul P3 failure - No alert received
      //var samplingFormCWithOpenStatus = samplingFormCHeaders.FirstOrDefault(sfc => sfc.InvoiceStatusId == (int)InvoiceStatusType.Open
      //  || sfc.InvoiceStatusId == (int)InvoiceStatusType.ValidationError || sfc.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission
      //  || sfc.InvoiceStatusId == (int)InvoiceStatusType.Presented || sfc.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete);

      var samplingFormCWithOpenStatus = samplingFormCHeaders.FirstOrDefault();

      var samplingFormCId = Guid.Empty;
      if (samplingFormCWithOpenStatus != null)
      {
        samplingFormCId = samplingFormCWithOpenStatus.Id;
      }

      var samplingFormCHeader = new SamplingFormC
      {
        ProvisionalBillingMonth = provisionalBillingMonth,
        ProvisionalBillingYear = provisionalBillingYear,
        FromMemberId = fromMemberId,
        ProvisionalBillingMemberId = provisionalBillingMemberId,
        InvoiceStatusId = invoiceStatus,
        ListingCurrency = listingCurrency,
        ListingCurrencyId = listingCurrencyId,
        Id = samplingFormCId,
        TotalGrossAmount = totalGrossAmountAlf,
        ProvisionalBillingMember = provisionalBillingMember,
        FromMember = fromMember,
        NumberOfRecords = numberOfRecords,
        NilFormCIndicator = nilFormCIndicator
      };

      if (samplingFormCHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        samplingFormCHeader.ValidationErrors = ValidationErrorManager.GetValidationErrors(samplingFormCId.ToString()).ToList();
      }

      return samplingFormCHeader;
    }

    /// <summary>
    /// Validates the sampling form C.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <param name="samplingFormCInDb">The sampling form C in db.</param>
    /// <returns></returns>
    private void ValidateSamplingFormC(SamplingFormC samplingFormCHeader, SamplingFormC samplingFormCInDb)
    {
      var isUpdateOperation = false;

      //Check whether it's a update operation.
      if (samplingFormCInDb != null)
      {
        isUpdateOperation = true;
        if (samplingFormCInDb.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
          throw new ISBusinessException(SamplingErrorCodes.InvalidOperationOnSubmitStatus);
        }

        samplingFormCInDb.NumberOfRecords = SamplingFormCRecordRepository.GetCount(sfcRecord => sfcRecord.SamplingFormCId == samplingFormCInDb.Id);
        if (samplingFormCInDb.NumberOfRecords > 0)
        {
          throw new ISBusinessException(SamplingErrorCodes.InvalidOperationAsFormCItemsExist);
        }
      }

      // Make sure billing and billed member are not the same.
      if (samplingFormCHeader.FromMemberId == samplingFormCHeader.ProvisionalBillingMemberId)
      {
        throw new ISBusinessException(SamplingErrorCodes.InvalidFormCMemberCombination);
      }

      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: New validation #MW2. The Member should not be a Type B Member.
        Disallow Type B members to bill or be billed in Pax and Cgo billing categories.
        Ref: FRS Section 3.4 Table 16 Row 3. FRS Section 3.4 Point 22, 27.
       
        As per CMP# 596 FRS document, the term ‘Type B Members’ means  - 
        new SIS Members having an Accounting Code with one of the following attributes:
        a.The length of the code is 3, but alpha characters appear in the second and/or third position (the first position may be alpha or numeric)
        b.The length of the code is 4, but alpha character(s) appear in any position (i.e. it is not purely 4 numeric)
        c.The length of the code ranges from 5 to 12
      */

        //CMP#596: Length of Member Accounting Code to be Increased to 12
        // Adding server side validation for invalid member id
        if (samplingFormCHeader!=null && samplingFormCHeader.ProvisionalBillingMemberId == 0)
        {
            throw new ISBusinessException(ErrorCodes.InvalidMemberType);
        }

      var billingMember = (samplingFormCHeader.FromMember != null && samplingFormCHeader.FromMember.MemberCodeNumeric != null) ? samplingFormCHeader.FromMember.MemberCodeNumeric : MemberManager.GetMember(samplingFormCHeader.FromMemberId).MemberCodeNumeric;
      var billedMember = (samplingFormCHeader.ProvisionalBillingMember != null && samplingFormCHeader.ProvisionalBillingMember.MemberCodeNumeric != null) ? samplingFormCHeader.ProvisionalBillingMember.MemberCodeNumeric : MemberManager.GetMember(samplingFormCHeader.ProvisionalBillingMemberId).MemberCodeNumeric;

      if (InvoiceManager.IsTypeBMember(billingMember))
      {
          throw new ISBusinessException(ErrorCodes.InvalidMemberTypeForSampling);
      }

      if (InvoiceManager.IsTypeBMember(billedMember))
      {
          throw new ISBusinessException(ErrorCodes.InvalidMemberType);
      }

      // Used when Form C created by the same or any other owner exists in an ‘Open’, 
      // ‘Error’ or ‘Ready for Submission’ status; for the same combination of ‘Provisional Billing Month,
      // ‘Provisional Billing Airline’ and Currency of Listing/Evaluation’. 
      // The user is expected to append additional items to the existing Form C that hasn’t been submitted yet.
      if (!isUpdateOperation)
      {
        if (IsSamplingFormCExists(samplingFormCHeader))
        {
          throw new ISBusinessException(SamplingErrorCodes.InvalidSamplingFormC);
        }
      }

      // If Sampling Form C created with nil indicator as yes for provisional billing member from member in same 
      // provisional billing month then any other Form C can not be created for that provisional billing member.
      if (!isUpdateOperation || CompareUtil.IsDirty(samplingFormCHeader.NilFormCIndicator, samplingFormCInDb.NilFormCIndicator)
       || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingMonth, samplingFormCInDb.ProvisionalBillingMonth)
       || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingYear, samplingFormCInDb.ProvisionalBillingYear)
       || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingMemberId, samplingFormCInDb.ProvisionalBillingMemberId))
      {
        if (IsSamplingFormCExistsWithNilIndicator(samplingFormCHeader, samplingFormCInDb))
        {
          throw new ISBusinessException(SamplingErrorCodes.SamplingFormCExistWithNilIndicator);
        }

        if (IsNonNilSamplingFormCExists(samplingFormCHeader, samplingFormCInDb))
        {
          throw new ISBusinessException(SamplingErrorCodes.NonNilFormCExistsForSamplingFormC);
        }
      }

      //if (!isUpdateOperation || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingMonth, samplingFormCInDb.ProvisionalBillingMonth)
      //  || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingYear, samplingFormCInDb.ProvisionalBillingYear)
      //  || CompareUtil.IsDirty(samplingFormCHeader.ProvisionalBillingMemberId, samplingFormCInDb.ProvisionalBillingMemberId)
      //  || CompareUtil.IsDirty(samplingFormCHeader.ListingCurrencyId, samplingFormCInDb.ListingCurrencyId))
      //{
      if (samplingFormCHeader.NilFormCIndicator != NilFormCIndicatorY && IsProvisionalBillingMemberMigrated(samplingFormCHeader))
      {
        // If IgnoreValidationOnMigrationPeriod parameter is set to True in system parameters do not throw InvalidProvisionalListingCurrency validation exception
        // If the migration status of the provisional billing airline indicates that sampling provisional billings
        // were migrated on/before the provisional billing period indicated in the Form C header, then the ‘Currency of Listing/Evaluation’ 
        // defined in the Form C header should match at the ‘Currency of Listing/Evaluation’ of at least one of the provisional invoices.
        // Check for Provisional billing month and year required to check provisional billing member is migrated on/before provisional billing month
        if (!IsListingCurrencyExist(samplingFormCHeader))
        {
          throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalListingCurrency);
        }
        samplingFormCHeader.IsLinkingSuccessful = true;
      }
      else
      {
        samplingFormCHeader.IsLinkingSuccessful = false;
      }

      if (!isUpdateOperation || CompareUtil.IsDirty(samplingFormCHeader.NilFormCIndicator, samplingFormCInDb.NilFormCIndicator))
      {
        if (samplingFormCHeader.NilFormCIndicator.Equals(NilFormCIndicatorY))
        {
          var isTransactionExists = SamplingFormCRecordRepository.GetCount(sfcRecord => sfcRecord.SamplingFormCId == samplingFormCHeader.Id) > 0;

          if (isTransactionExists)
          {
            throw new ISBusinessException(SamplingErrorCodes.FormCHeaderWithNilIndicator);
          }
        }
      }

      //// Blocked by Debtor.
      //if (CheckBlockedMember(true, samplingFormCHeader.ProvisionalBillingMemberId, samplingFormCHeader.FromMemberId, true))
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidBillingToMember);
      //}

      //// Blocked by Creditor.
      //if (CheckBlockedMember(false, samplingFormCHeader.FromMemberId, samplingFormCHeader.ProvisionalBillingMemberId, true))
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidBillingFromMember);
      //}

      return;
    }

    /// <summary>
    /// Determines whether provisional billing member migrated for specified sampling form C header].
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns>
    /// 	True if [is provisional billing member migrated for the specified sampling form C header; otherwise, <c>false</c>.
    /// </returns>
    private bool IsProvisionalBillingMemberMigrated(SamplingFormC samplingFormCHeader)
    {
      var passengerConfiguration = MemberManager.GetPassengerConfiguration(samplingFormCHeader.ProvisionalBillingMemberId);
      if (passengerConfiguration == null) return false;

      var isIdecMigrationDate = passengerConfiguration.SamplingProvIsIdecMigratedDate;
      var isXmlMigrationDate = passengerConfiguration.SamplingProvIsxmlMigratedDate;

      // If User migrated for both IS-Xml and IS-IDEC.
      if ((isIdecMigrationDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
        && (isXmlMigrationDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified))
      {
        var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);

        return (samplingFormCHeader.ProvisionalBillingPeriod >= isIdecMigrationPeriod) || (samplingFormCHeader.ProvisionalBillingPeriod >= isXmlMigrationPeriod);
      }

      // If User migrated for IS-IDEC.
      if (isIdecMigrationDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
      {
        var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
        return (samplingFormCHeader.ProvisionalBillingPeriod >= isIdecMigrationPeriod);
      }
      // If User migrated for IS-Xml
      if (isXmlMigrationDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified)
      {
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);
        return samplingFormCHeader.ProvisionalBillingPeriod >= isXmlMigrationPeriod;
      }

      return false;
    }

    /// <summary>
    /// Determines whether listing currency exist for the specified sampling form C header].
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns>
    ///  True if listing currency exist for the specified sampling form C header; otherwise false.
    /// </returns>
    private bool IsListingCurrencyExist(SamplingFormC samplingFormCHeader)
    {
      // Sampling Form C can be created only against presented invoices.
      return InvoiceRepository.GetCount(invoice => invoice.ListingCurrencyId == samplingFormCHeader.ListingCurrencyId
                                                   && invoice.BillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                   && invoice.BillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                   && invoice.BillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                   && invoice.BilledMemberId == samplingFormCHeader.FromMemberId
                                                   && invoice.BillingCode == (int)BillingCode.SamplingFormAB
                                                   && (invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete)) > 0;
    }

    public int? GetFormABListingCurrency(SamplingFormC samplingFormCHeader)
    {
      var formAB = InvoiceRepository.First(invoice => invoice.BillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                && invoice.BillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                && invoice.BillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                && invoice.BilledMemberId == samplingFormCHeader.FromMemberId
                                                && invoice.BillingCode == (int)BillingCode.SamplingFormAB
                                                && (invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete));
      if (formAB != null) return formAB.ListingCurrencyId;

      return null;
    }

    /// <summary>
    /// Get coupon details (for pre-populating on UI) based on ticket issuing airline, document number and coupon number.
    /// </summary>
    /// <param name="fromMemberId">From member Id of Form C.</param>
    /// <param name="provisionalBillingMemberId">Provisional Billing Member Id of Form C.</param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="documentNumber"></param>
    /// <param name="couponNumber"></param>
    /// <param name="listingCurrency"></param>
    /// <returns></returns>
    public LinkedCouponDetails GetLinkedCouponDetails(int fromMemberId, int provisionalBillingMemberId, int provisionalBillingMonth, int provisionalBillingYear, string ticketIssuingAirline, long documentNumber, int couponNumber, string listingCurrency)
    {
      var linkedCouponDetails = new LinkedCouponDetails();

      //var primeCoupons =
      //  CouponRecordRepository.Get(
      //    coup =>
      //    coup.TicketOrFimIssuingAirline == ticketIssuingAirline && coup.TicketDocOrFimNumber == documentNumber && coup.TicketOrFimCouponNumber == couponNumber &&
      //    coup.Invoice.BillingMonth == provisionalBillingMonth && coup.Invoice.BillingCode == (int) BillingCode.SamplingFormAB && coup.Invoice.BillingMemberId == provisionalBillingMemberId &&
      //    coup.Invoice.BilledMemberId == fromMemberId).ToList();
      //var coupons = primeCoupons.Select(c => new LinkedCoupon
      //                        {
      //                          ProvisionalInvoiceNumber =  c.Invoice.InvoiceNumber,
      //                          BatchNumberOfProvisionalInvoice = c.BatchSequenceNumber,
      //                          RecordSeqNumberOfProvisionalInvoice = c.RecordSequenceWithinBatch,
      //                          GrossAmountAlf = c.CouponGrossValueOrApplicableLocalFare,
      //                          ElectronicTicketIndicator = c.ElectronicTicketIndicator,
      //                          AgreementIndicatorSupplied = c.AgreementIndicatorSupplied,
      //                          AgreementIndicatorValidated = c.AgreementIndicatorValidated,
      //                          OriginalPmi = c.OriginalPmi,
      //                          ValidatedPmi = c.ValidatedPmi,
      //                          ListingCurrency = c.Invoice.ListingCurrency.Code
      //                        });

      var couponList = CouponRecordRepository.GetLinkedCouponsForFormC(couponNumber,
                                                                       documentNumber,
                                                                       ticketIssuingAirline,
                                                                       provisionalBillingMemberId,
                                                                       fromMemberId,
                                                                       provisionalBillingMonth,
                                                                       provisionalBillingYear);

      //var couponList = new List<LinkedCoupon>(coupons);
      linkedCouponDetails.LinkedCoupons = couponList;

      if (couponList.Count == 0)
      {
        linkedCouponDetails.ErrorMessage = Messages.CouponDoesNotExist;
        return linkedCouponDetails;
      }

      // single occurrence with different currency.
      if (couponList.Count == 1 && couponList[0].ListingCurrency != listingCurrency)
      {
        linkedCouponDetails.ErrorMessage = Messages.CouponExistsButWithDifferentCurrency;
        return linkedCouponDetails;
      }

      if (couponList.Count == 1)
      {
        return linkedCouponDetails;
      }
      // multiple occurrences found, all with different listing currencies.
      int couponCountWithSameCurrency = couponList.Count(coup => coup.ListingCurrency == listingCurrency);
      if (couponCountWithSameCurrency == 0)
      {
        linkedCouponDetails.ErrorMessage = Messages.MultipleCouponsFoundWithDifferentCurrency;
        return linkedCouponDetails;
      }

      // show only coupons with same listing currency.
      var couponsWithSameListingCurrency = couponList.Where(coup => coup.ListingCurrency == listingCurrency);
      linkedCouponDetails.LinkedCoupons = new List<LinkedCoupon>(couponsWithSameListingCurrency);

      return linkedCouponDetails;
    }

    /// <summary>
    /// Determines whether sampling form C exists for the specified sampling form C header.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns>
    /// 	True if sampling form C exists for the specified sampling form C header; otherwise false.
    /// </returns>
    private bool IsSamplingFormCExists(SamplingFormC samplingFormCHeader)
    {
      // Nil or Non- Nil form C can not be added if Form C 
      return SamplingFormCRepository.GetCount(samplingFormC => samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                               && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId
                                                               && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                               && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                               && (samplingFormC.InvoiceStatusId == (int)InvoiceStatusType.Open
                                                                       || samplingFormC.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission
                                                                       || samplingFormC.InvoiceStatusId == (int)InvoiceStatusType.ValidationError)
                                                                       && samplingFormC.ListingCurrencyId == samplingFormCHeader.ListingCurrencyId
        //    || ((samplingFormC.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling
        //  || samplingFormC.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete)
        // && samplingFormC.NilFormCIndicator == NilFormCIndicatorY))
                                                               ) > 0;
    }

    /// <summary>
    /// If Sampling Form C created with nil indicator as yes for provisional billing member from member in same
    /// provisional billing month then any other Form C can not be created for that provisional billing member.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <param name="samplingFormCHeaderInDb">The sampling form C header in db.</param>
    /// <returns>
    /// True if sampling form C exists with nil indicator from the specified sampling form C header; otherwise, false
    /// </returns>
    private bool IsSamplingFormCExistsWithNilIndicator(SamplingFormC samplingFormCHeader, SamplingFormC samplingFormCHeaderInDb)
    {
      long count;

      if (samplingFormCHeaderInDb != null)
      {
        count = SamplingFormCRepository.GetCount(samplingFormC => (samplingFormC.NilFormCIndicator == NilFormCIndicatorY || samplingFormC.NilFormCIndicator == NilFormCIndicatorS)
                                                                  && samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                                  && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                                  && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                                  && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId
                                                                  && samplingFormC.Id != samplingFormCHeaderInDb.Id);
      }
      else
      {
        count = SamplingFormCRepository.GetCount(samplingFormC => (samplingFormC.NilFormCIndicator == NilFormCIndicatorY || samplingFormC.NilFormCIndicator == NilFormCIndicatorS)
                                                             && samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                             && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                             && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                             && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId);
      }

      return count > 0;
    }

    /// <summary>
    /// If sampling form C exists in the same IDEC file with NIL form C - To Be Done.
    /// If Sampling Form C created with nil indicator as yes for provisional billing member from member in same
    /// provisional billing month then any other Form C can not be created for that provisional billing member.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns>
    /// True if sampling form C exists with nil indicator from the specified sampling form C header; otherwise, false
    /// </returns>
    private bool IsParsedSamplingFormCExistsWithNilIndicator(SamplingFormC samplingFormCHeader)
    {
      long count = SamplingFormCRepository.GetCount(samplingFormC => (samplingFormC.NilFormCIndicator == NilFormCIndicatorY || samplingFormC.NilFormCIndicator == NilFormCIndicatorS)
                                                                     && samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                                     && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                                     && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                                     && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId);
      return count > 0;
    }

    /// <summary>
    /// Determines whether [is non nil sampling form C exists] [the specified sampling form C header].
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <param name="samplingFormCHeaderInDb">The sampling form C header in db.</param>
    /// <returns>
    /// 	<c>true</c> if [is non nil sampling form C exists] [the specified sampling form C header]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsNonNilSamplingFormCExists(SamplingFormC samplingFormCHeader, SamplingFormC samplingFormCHeaderInDb)
    {
      long count = 0;
      if (samplingFormCHeader != null && samplingFormCHeader.NilFormCIndicator != null && samplingFormCHeader.NilFormCIndicator.Equals(NilFormCIndicatorY))
      {
        if (samplingFormCHeaderInDb != null)
        {
          count = SamplingFormCRepository.GetCount(samplingFormC => samplingFormC.NilFormCIndicator == NilFormCIndicatorN
                                                                    && samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                                    && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                                    && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                                    && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId
                                                                    && samplingFormC.Id != samplingFormCHeaderInDb.Id);
        }
        else
        {
          count = SamplingFormCRepository.GetCount(samplingFormC => (samplingFormC.NilFormCIndicator == NilFormCIndicatorN)
                                                               && samplingFormC.ProvisionalBillingMemberId == samplingFormCHeader.ProvisionalBillingMemberId
                                                               && samplingFormC.ProvisionalBillingMonth == samplingFormCHeader.ProvisionalBillingMonth
                                                               && samplingFormC.ProvisionalBillingYear == samplingFormCHeader.ProvisionalBillingYear
                                                               && samplingFormC.FromMemberId == samplingFormCHeader.FromMemberId);
        }
      }

      return count > 0;
    }


    /// <summary>
    /// Validates the on submit.
    /// </summary>
    /// <param name="samplingFormCHeader">The sampling form C header.</param>
    /// <returns></returns>
    private SamplingFormC ValidateOnSubmit(SamplingFormC samplingFormCHeader)
    {
      var webValidationErrors = new List<WebValidationError>();

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(samplingFormCHeader.Id.ToString());

      // A Form C header cannot be submitted for a provisional billing month on or after the month’s Sample Digit Announcement date.
      if (IsAfterSampleDigitAnnouncementDate(samplingFormCHeader.ProvisionalBillingYear, samplingFormCHeader.ProvisionalBillingMonth))
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(samplingFormCHeader.Id, ErrorCodes.SubmissionNotAllowedAfterSampleDigitAnnouncementDate));
      }

      // Update invoice status in case of error.
      if (webValidationErrors.Count > 0)
      {
        samplingFormCHeader.InvoiceStatus = InvoiceStatusType.ValidationError;
        samplingFormCHeader.ValidationErrors.AddRange(webValidationErrors);

        if (samplingFormCHeader.ValidationStatus != (int)InvoiceValidationStatus.ErrorPeriod)
          samplingFormCHeader.ValidationStatus = (int)InvoiceValidationStatus.Failed;
        // Update validation errors in db.
      }
      else
      {
        // Every validation is successful. Update invoice status as Ready for billing and Validation status to Completed.
        samplingFormCHeader.InvoiceStatus = InvoiceStatusType.ReadyForBilling;
        samplingFormCHeader.ValidationStatus = (int)InvoiceValidationStatus.Completed;
        //SCP162502: Form C - AC OAR Jul P3 failure - No alert received
        samplingFormCHeader.ValidationStatusDate = DateTime.UtcNow;
      }

      ValidationErrorManager.UpdateValidationErrors(samplingFormCHeader.Id, samplingFormCHeader.ValidationErrors, validationErrorsInDb);

      return samplingFormCHeader;
    }

    private bool IsAfterSampleDigitAnnouncementDate(int provisionalBillingYear, int provisionalBillingMonth)
    {
      // A Form C header cannot be submitted for a provisional billing month on or after the month’s 
      // Sample Digit Announcement date. Refer to ‘Sample Digit Master’. This check prevents cases where some 
      // Form Cs were submitted by the airline earlier to the date; and the airline again attempts to submit another Form C on/after this date.
      var billingMonth = string.Format(@"{0:D4}{1:D2}", provisionalBillingYear, provisionalBillingMonth);
      var count = SampleDigitRepository.GetCount(c => c.DigitAnnouncementDateTime <= DateTime.UtcNow && c.ProvisionalBillingMonth.Equals(billingMonth));
      return count > 0;
    }

    /// <summary>
    /// Determines whether duplicate sampling form C exists  for specified sampling form C record.
    /// </summary>
    /// <param name="samplingFormCRecord">The sampling form C record.</param>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <returns>
    /// True is duplicate sampling form C exists for the specified sampling form C record; otherwise, false.
    /// </returns>
    private bool IsDulplicateSamplingFormCRecordExists(SamplingFormCRecord samplingFormCRecord, SamplingFormC samplingFormC)
    {

      return SamplingFormCRecordRepository.IsDuplicateSamplingFormCRecordExists(samplingFormCRecord.TicketIssuingAirline,
                                                                           samplingFormCRecord.DocumentNumber,
                                                                           samplingFormCRecord.CouponNumber,
                                                                           samplingFormCRecord.ProvisionalInvoiceNumber,
                                                                           samplingFormCRecord.BatchNumberOfProvisionalInvoice,
                                                                           samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice,
                                                                           samplingFormC.FromMemberId,
                                                                           samplingFormC.ProvisionalBillingMemberId,
                                                                           samplingFormC.ProvisionalBillingMonth,
                                                                           samplingFormC.ProvisionalBillingYear) > 0;
    }



    /// <summary>
    /// Validates the sampling invoice db.
    /// </summary>
    /// <param name="samplingFormC"></param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    public bool ValidateParsedSamplingFormC(SamplingFormC samplingFormC, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var currentPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);

      var currentPeriodYear = samplingFormC.ProvisionalBillingYear > 99 ? currentPeriod.Year : currentPeriod.Year - 2000;
      if (samplingFormC.ProvisionalBillingYear > currentPeriodYear || (samplingFormC.ProvisionalBillingYear == currentPeriodYear && samplingFormC.ProvisionalBillingMonth > currentPeriod.Month) || samplingFormC.ProvisionalBillingMonth > 12)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Provisional Billing Month", null, string.Format("{0}{1}", samplingFormC.ProvisionalBillingYear.ToString().PadLeft(4, '0').Substring(2, 2), samplingFormC.ProvisionalBillingMonth.ToString().PadLeft(2, '0')), fileName, ErrorLevels.ErrorLevelSamplingFormC, ErrorCodes.InvoiceProvisionalBillingMonthInvalid, ErrorStatus.X);

        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // A Form C header cannot be submitted for a provisional billing month on or after the month’s Sample Digit Announcement date.
      
      if (IsAfterSampleDigitAnnouncementDate(samplingFormC.ProvisionalBillingYear, samplingFormC.ProvisionalBillingMonth))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Sampling Form C", null, string.Empty, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.InvalidSamlingFormCSubmission, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (samplingFormC.NilFormCIndicator != null && string.IsNullOrEmpty(samplingFormC.NilFormCIndicator.Trim()))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Nil Form C Indicator", null, samplingFormC.NilFormCIndicator, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.InvalidNilFormCIndicatorForFormC, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (samplingFormC.NilFormCIndicator != null && samplingFormC.NilFormCIndicator.Equals("Y", StringComparison.OrdinalIgnoreCase) && samplingFormC.SamplingFormCDetails.Count > 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "SamplingFormCRecord", null, string.Empty, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.InvalidSamplingFormCRecord, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
     // Added validation on Form C if NilFormCIndicator is 'Y' then SourceCodetotal(LineItem) should not present.
      if (samplingFormC.NilFormCIndicator != null && samplingFormC.NilFormCIndicator.Equals("Y", StringComparison.OrdinalIgnoreCase) && samplingFormC.SamplingFormCSourceCodeTotals.Count > 0)
      {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "SamplingFormCRecord", null, string.Empty, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.InvalidSamplingFormCSourceCodeRecord, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
      }
      // Added validation on Form C if NilFormCIndicator is 'N' then Listing Currency is required.
      if (samplingFormC.NilFormCIndicator != null && samplingFormC.NilFormCIndicator.Equals("N", StringComparison.OrdinalIgnoreCase) && samplingFormC.ListingCurrencyId == null )
      {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Listing Currency", null, samplingFormC.ListingCurrencyId.ToString(), fileName, ErrorLevels.ErrorLevelSamplingFormC, ErrorCodes.InvalidListingCurrency, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
      }

      if (samplingFormC.NilFormCIndicator != null && samplingFormC.NilFormCIndicator.Equals("N", StringComparison.OrdinalIgnoreCase) && samplingFormC.ListingCurrencyId != null && !ReferenceManager.IsValidCurrency(samplingFormC.ListingCurrencyId))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Listing Currency", null, samplingFormC.ListingCurrencyId.ToString(), fileName, ErrorLevels.ErrorLevelSamplingFormC, ErrorCodes.InvalidListingCurrency, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }


      if (samplingFormC.FromMemberId != 0 && samplingFormC.ProvisionalBillingMemberId != 0)
      {
        // Make sure billing and billed member are not the same.
        if (samplingFormC.FromMemberId == samplingFormC.ProvisionalBillingMemberId)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "ProvisionalBillingMemberId", null, samplingFormC.ProvisionalBillingMemberText, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.SameBillingAndBilledMemberId, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
         Desc: New validation #MF1. The Member should not be a Type B Member.
         Disallow Type B members to bill or be billed in Pax and Cgo billing categories.
         Ref: FRS Section 3.7 Table 26 Row 1, 2.
       
         As per CMP# 596 FRS document, the term ‘Type B Members’ means  - 
         new SIS Members having an Accounting Code with one of the following attributes:
         a.The length of the code is 3, but alpha characters appear in the second and/or third position (the first position may be alpha or numeric)
         b.The length of the code is 4, but alpha character(s) appear in any position (i.e. it is not purely 4 numeric)
         c.The length of the code ranges from 5 to 12
      */
      var billingMemberForTypeCheck = (samplingFormC.FromMemberId == 0 ? null : MemberManager.GetMemberDetails(samplingFormC.FromMemberId));
      var billedMemberForTypeCheck = (samplingFormC.ProvisionalBillingMemberId == 0 ? null : MemberManager.GetMemberDetails(samplingFormC.ProvisionalBillingMemberId));

      if (billingMemberForTypeCheck != null && IsTypeBMember(billingMemberForTypeCheck.MemberCodeNumeric))
      {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate,
              "FormC Generating Entity", null, billingMemberForTypeCheck.MemberCodeNumeric, fileName, ErrorLevels.ErrorLevelSamplingFormC,
              ErrorCodes.InvalidMemberType, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
      }

      if (billedMemberForTypeCheck != null && IsTypeBMember(billedMemberForTypeCheck.MemberCodeNumeric))
      {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate,
              "FormC Receiving Entity", null, billedMemberForTypeCheck.MemberCodeNumeric, fileName, ErrorLevels.ErrorLevelSamplingFormC,
              ErrorCodes.InvalidMemberType, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
      }

      if (IsSamplingFormCExists(samplingFormC))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Sampling Form C", null, Convert.ToString(samplingFormC.ProvisionalBillingMemberId), fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.InvalidSamplingFormC, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;

      }

      if (IsSamplingFormCExistsWithNilIndicator(samplingFormC, null))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Sampling Form C with NIL Indicator", null, samplingFormC.NilFormCIndicator, fileName, ErrorLevels.ErrorLevelSamplingFormC, SamplingErrorCodes.SamplingFormCExistWithNilIndicator, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;

      }

      if (IsNonNilSamplingFormCExists(samplingFormC, null))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Sampling Form C with NIL Indicator",
                                                                                null,
                                                                                samplingFormC.NilFormCIndicator,
                                                                                fileName, ErrorLevels.ErrorLevelSamplingFormC,
                                                                        SamplingErrorCodes.NonNilFormCExistsForSamplingFormC, ErrorStatus.X);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;

      }

      if (exceptionDetailsList != null && (!exceptionDetailsList.Any(ed => (ed.BillingEntityCode == samplingFormC.FromMember.MemberCodeNumeric.PadLeft(4, '0') && ed.BilledEntityCode == samplingFormC.ProvisionalBillingMember.MemberCodeNumeric.PadLeft(4, '0') && ed.ExceptionCode == ErrorCodes.InvalidProvisionalInvoiceDate))))
      {
        if (!string.IsNullOrEmpty(samplingFormC.NilFormCIndicator))
        {
          var currentMonthFormCValue = fileName + "-" + samplingFormC.ProvisionalBillingYear + samplingFormC.ProvisionalBillingMonth + "-" + samplingFormC.ProvisionalBillingMemberId;
          var currentNilFormC = string.Empty;

          bool provInvoiceFound = ParsedNilFormC.ContainsKey(currentMonthFormCValue);

          if (provInvoiceFound)
          {
            ParsedNilFormC.TryGetValue(currentMonthFormCValue, out currentNilFormC);
          }
          if ((!string.IsNullOrEmpty(currentNilFormC) && !currentNilFormC.Equals(samplingFormC.NilFormCIndicator)))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            samplingFormC,
                                                                            fileSubmissionDate,
                                                                            "Sampling Form C with NIL Indicator",
                                                                            null,
                                                                            samplingFormC.NilFormCIndicator,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSamplingFormC,
                                                                            SamplingErrorCodes.FileExistWithFormCAndNilIndicator,
                                                                            ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {
            if (!provInvoiceFound)
            {
              ParsedNilFormC.Add(currentMonthFormCValue, samplingFormC.NilFormCIndicator);
            }
          }
        }

      }
      //Check for Form AB is present for prov Billing Month Year & Billing member Combination.
      if (exceptionDetailsList!=null && samplingFormC.NilFormCIndicator != NilFormCIndicatorY && IsProvisionalBillingMemberMigrated(samplingFormC))
      {
        // If the migration status of the provisional billing airline indicates that sampling provisional billings
        // were migrated on/before the provisional billing period indicated in the Form C header, then the ‘Currency of Listing/Evaluation’ 
        // defined in the Form C header should match at the ‘Currency of Listing/Evaluation’ of at least one of the provisional invoices.
        // Check for Provisional billing month and year required to check provisional billing member is migrated on/before provisional billing month
        if (!IsListingCurrencyExist(samplingFormC))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            samplingFormC,
                                                                            fileSubmissionDate,
                                                                            "Sampling Form C Listing Curency",
                                                                            null,
                                                                            Convert.ToString(samplingFormC.ListingCurrencyId),
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSamplingFormC,
                                                                            SamplingErrorCodes.InvalidProvisionalListingCurrency,
                                                                            ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
          samplingFormC.IsLinkingSuccessful = false;
        }
        else
        {
          samplingFormC.IsLinkingSuccessful = true;
        }
      }
      else
      {
        samplingFormC.IsLinkingSuccessful = false;
      }

      var issuingAirline = new Dictionary<string, bool>();
      //SCP:186702 - LH file in production 
      ProvisionalInvoiceList.Clear();
      foreach (SamplingFormCRecord samplingFormCRecord in samplingFormC.SamplingFormCDetails)
      {
        samplingFormCRecord.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.Validated;
        isValid = ValidateParsedSamplingFormCRecord(samplingFormCRecord, exceptionDetailsList, samplingFormC, fileName, issuingAirline);
      }
      //SCP:186702 - LH file in production 
      ProvisionalInvoiceList.Clear();
      // Validate Form C Source Code Total
      // vinod
      
      foreach (var sourceCodeTotal in samplingFormC.SamplingFormCSourceCodeTotals)
      {

        if (!ReferenceManager.IsValidSourceCode(sourceCodeTotal.SourceId, (int)TransactionType.SamplingFormC))
        {

          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                     samplingFormC,
                                                                     fileSubmissionDate,
                                                                     "Source Code",
                                                                     null,
                                                                     Convert.ToString(sourceCodeTotal.SourceId),
                                                                     fileName,
                                                                     ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                     ErrorCodes.InvalidSourceCode,
                                                                     ErrorStatus.X, sourceCodeTotal.SourceId);


          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          //For the source code
          isValid = ValidateParsedSourceCodeTotalForFormCRecords(samplingFormC, sourceCodeTotal, sourceCodeTotal.SourceId, exceptionDetailsList, fileName, fileSubmissionDate);
        }
      }


      var validationManager = Ioc.Resolve<IValidationErrorManager>(typeof(IValidationErrorManager));
      samplingFormC.isValidationExceptionSummary = validationManager.GetIsSummary(samplingFormC, exceptionDetailsList, fileName, fileSubmissionDate);
    
      samplingFormC.ValidationExceptionSummary = validationManager.GetIsSummaryForValidationErrorCorrection(samplingFormC, exceptionDetailsList.ToList());
  
      UpdateSamplingParsedFormCStatus(samplingFormC, fileName, fileSubmissionDate, exceptionDetailsList);

      if (samplingFormC.isValidationExceptionSummary != null)
      {
        samplingFormC.isValidationExceptionSummary.InvoiceStatus = ((int)samplingFormC.InvoiceStatus).ToString();
      }

      return isValid;
    }

    /// <summary>
    /// Update the Parsed SamplingFormC status.
    /// </summary>
    /// <param name="samplingFormC"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="exceptionDetailsList"></param>
    public void UpdateSamplingParsedFormCStatus(SamplingFormC samplingFormC, string fileName, DateTime fileSubmissionDate, IList<IsValidationExceptionDetail> exceptionDetailsList)
    {
      if (exceptionDetailsList.Count(rec => Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.Z) > 0)
      {
        samplingFormC.ValidationStatus = (int)InvoiceValidationStatus.Failed;
        samplingFormC.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
      }
      if (exceptionDetailsList.Count(rec => Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.X) > 0)
      {
        samplingFormC.ValidationStatus = (int)InvoiceValidationStatus.Failed;
        samplingFormC.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
      }
      else if (exceptionDetailsList.Count(rec => Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.C) > 0)
      {
        samplingFormC.ValidationStatus = (int)InvoiceValidationStatus.Failed;
        samplingFormC.InvoiceStatus = InvoiceStatusType.ErrorCorrectable;
      }
      else
      {
        //If the invoice is submitted with no errors then it is eligible for late submission.
        if (samplingFormC.ValidationStatus == (int)InvoiceValidationStatus.ErrorPeriod)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Billing Date", null,
                      string.Format("{0}{1}", samplingFormC.ProvisionalBillingYear, samplingFormC.ProvisionalBillingMonth), fileName, ErrorLevels.ErrorLevelSamplingFormC,
                      ErrorCodes.InvoiceValidForLateSubmission, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
        }
        else
        {
          samplingFormC.ValidationStatus = (int)InvoiceValidationStatus.Completed;
          //paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Completed;
        }

        // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
        samplingFormC.InvoiceStatusId = (int)InvoiceStatusType.OnHold;
      }
    }


    /// <summary>
    /// Validates the parsed sampling form C record.
    /// </summary>
    /// <param name="samplingFormCRecord">The sampling form C record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <returns></returns>
    private bool ValidateParsedSamplingFormCRecord(SamplingFormCRecord samplingFormCRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, SamplingFormC samplingFormC, string fileName, IDictionary<string, bool> issuingAirline)
    {
      bool isValid = true;

      DateTime fileSubmissionDate = DateTime.MinValue;
      if (!string.IsNullOrEmpty(fileName))
      {
        var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
        var isInputFile = referenceManager.GetIsInputFile(Path.GetFileName(fileName));
        if (isInputFile != null)
        {
          fileSubmissionDate = isInputFile.FileDate;
        }
      }

      if (!ReferenceManager.IsValidSourceCode(samplingFormCRecord.SourceCodeId, (int)TransactionType.SamplingFormC))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                samplingFormC,
                                                                                fileSubmissionDate,
                                                                                "Source Code",
                                                                                Convert.ToString(samplingFormCRecord.SourceCodeId),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                ErrorCodes.InvalidSourceCode,
                                                                                ErrorStatus.X,
                                                                                samplingFormCRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validate TicketDocOrFimNumber is greater than 0.
      if (samplingFormCRecord.DocumentNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Ticket or Documnet Number", Convert.ToString(samplingFormCRecord.DocumentNumber), fileName, ErrorLevels.ErrorLevelSamplingFormCRecord, ErrorCodes.InvalidTicketDocumnetOrFimNumber, ErrorStatus.X, samplingFormCRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(samplingFormCRecord.DocumentNumber).Length > 10)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormCRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        samplingFormC,
                                                                        fileSubmissionDate,
                                                                        "Ticket or Document Number",
                                                                        Convert.ToString(samplingFormCRecord.DocumentNumber),
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                        SamplingErrorCodes.TicketFimDocumentNoGreaterThanTenS,
                                                                        ErrorStatus.X,
                                                                        samplingFormCRecord);

        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Batch Sequence Number and Record Sequence Number
      if (samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice <= 0 || samplingFormCRecord.BatchNumberOfProvisionalInvoice <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        samplingFormC,
                                                                        fileSubmissionDate,
                                                                        "Batch Sequence Number-Record Sequence Number",
                                                                        string.Format("{0}-{1}",
                                                                                      samplingFormCRecord.BatchNumberOfProvisionalInvoice,
                                                                                      samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice),
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                        ErrorCodes.InvalidBatchSequenceNoAndRecordNo,
                                                                        ErrorStatus.X,
                                                                        samplingFormCRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validate ticket issuing airline.
      isValid = ValidateTicketIssuingAirline(samplingFormCRecord, exceptionDetailsList, fileName, issuingAirline, isValid, fileSubmissionDate, samplingFormC);

      //Validate ReasonCode 
      if (!string.IsNullOrEmpty(samplingFormCRecord.ReasonCode.Trim()))
      {
        if (!ReferenceManager.IsValidReasonCode(samplingFormCRecord.ReasonCode, (int)TransactionType.SamplingFormC))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                    samplingFormC,
                                                                                    fileSubmissionDate,
                                                                                    "Rejection Reason Code",
                                                                                    Convert.ToString(samplingFormCRecord.ReasonCode),
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                    ErrorCodes.InvalidReasonCode,
                                                                                    ErrorStatus.C,
                                                                                    samplingFormCRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }


      //Check duplicate of form C record
      if (samplingFormC.SubmissionMethod != SubmissionMethod.IsWeb &&
          samplingFormC.SamplingFormCDetails.Where(
            sfcRecord =>
            sfcRecord.TicketIssuingAirline == samplingFormCRecord.TicketIssuingAirline && sfcRecord.DocumentNumber == samplingFormCRecord.DocumentNumber &&
            sfcRecord.CouponNumber == samplingFormCRecord.CouponNumber && sfcRecord.BatchNumberOfProvisionalInvoice == samplingFormCRecord.BatchNumberOfProvisionalInvoice &&
            sfcRecord.RecordSeqNumberOfProvisionalInvoice == samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice &&
            sfcRecord.ProvisionalInvoiceNumber == samplingFormCRecord.ProvisionalInvoiceNumber).Count() > 1)
      {

        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                samplingFormC,
                                                                                fileSubmissionDate,
                                                                                "Form C Record",
                                                                                string.Format("{0}/{1}/{2}",
                                                                                              samplingFormCRecord.IssueAirline,
                                                                                              samplingFormCRecord.DocumentNumber,
                                                                                              samplingFormCRecord.CouponNumber),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                SamplingErrorCodes.DuplicateSamplingFormCRecordFound,
                                                                                ErrorStatus.X,
                                                                                samplingFormCRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      else if (samplingFormC.SubmissionMethod != SubmissionMethod.IsWeb && IsDulplicateSamplingFormCRecordExists(samplingFormCRecord, samplingFormC))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                samplingFormC,
                                                                                fileSubmissionDate,
                                                                                "Form C Record",
                                                                                string.Format("{0}/{1}/{2}",
                                                                                              samplingFormCRecord.IssueAirline,
                                                                                              samplingFormCRecord.DocumentNumber,
                                                                                              samplingFormCRecord.CouponNumber),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                SamplingErrorCodes.DuplicateSamplingFormCRecordFound,
                                                                                ErrorStatus.X,
                                                                                samplingFormCRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      else
      {
        //if (!IgnoreBillingHistoryValidation(samplingFormC))
        //{
        //Validate Provisional Invoice Number

        //SCP:186702 - LH file in production 
        var provInvoiceSearchCriteria = string.Format("{0}-{1}-{2}-{3}-{4}",
                                                      samplingFormCRecord.ProvisionalInvoiceNumber,
                                                      samplingFormC.ProvisionalBillingMonth,
                                                      samplingFormC.ProvisionalBillingYear,
                                                      samplingFormC.ProvisionalBillingMemberId,
                                                      samplingFormC.FromMemberId);
        PaxInvoice provisionalInvoice;
        ProvisionalInvoiceList.TryGetValue(provInvoiceSearchCriteria, out provisionalInvoice);
        if (provisionalInvoice == null && !ProvisionalInvoiceList.ContainsKey(provInvoiceSearchCriteria))
        {
          provisionalInvoice = InvoiceRepository.Single(samplingFormCRecord.ProvisionalInvoiceNumber,
                                                          samplingFormC.ProvisionalBillingMonth,
                                                          samplingFormC.ProvisionalBillingYear,
                                                          null,
                                                          samplingFormC.ProvisionalBillingMemberId,
                                                          samplingFormC.FromMemberId,
                                                          (int)BillingCode.SamplingFormAB,
                                                          invoiceStatusId: (int)InvoiceStatusType.Presented);
          ProvisionalInvoiceList.Add(provInvoiceSearchCriteria, provisionalInvoice);
        }

        if (provisionalInvoice != null)
        {
          Logger.InfoFormat("Provisional Invoice is found: {0}", provisionalInvoice.InvoiceNumber);
          if (provisionalInvoice.ListingCurrencyId != samplingFormC.ListingCurrencyId)
          {
            if (exceptionDetailsList != null)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                            samplingFormC,
                                                                                            fileSubmissionDate,
                                                                                            "Listing Currency",
                                                                                            samplingFormCRecord,
                                                                                            Convert.ToString(samplingFormC.ListingCurrencyId),
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                            SamplingErrorCodes.InvalidProvisionalListingCurrency,
                                                                                            ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            else
            {
              throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalListingCurrency);
            }
          }
          else
          {
            var couponRecords =
              CouponRecordRepository.Get(
                record =>
                record.TicketOrFimCouponNumber == samplingFormCRecord.CouponNumber && record.TicketOrFimIssuingAirline == samplingFormCRecord.TicketIssuingAirline &&
                record.TicketDocOrFimNumber == samplingFormCRecord.DocumentNumber && record.InvoiceId == provisionalInvoice.Id).ToList();
            var couponRecord =
              couponRecords.Where(
                record =>
                record.TicketOrFimCouponNumber == samplingFormCRecord.CouponNumber && record.TicketOrFimIssuingAirline == samplingFormCRecord.TicketIssuingAirline &&
                record.TicketDocOrFimNumber == samplingFormCRecord.DocumentNumber && record.InvoiceId == provisionalInvoice.Id &&
                record.BatchSequenceNumber == samplingFormCRecord.BatchNumberOfProvisionalInvoice && record.RecordSequenceWithinBatch == samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice).FirstOrDefault();
            if (couponRecords.Count > 0 && couponRecord == null) couponRecord = couponRecords.FirstOrDefault();
            if (couponRecords.Count == 0)
            {
              //SCP#48125 : RM Linking
              var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                                 samplingFormC, fileSubmissionDate,
                                                                                                 "Document Number",
                                                                                                 string.Format("{0},{1},{2}",
                                                                                                               samplingFormCRecord.TicketIssuingAirline,
                                                                                                               samplingFormCRecord.DocumentNumber,
                                                                                                               samplingFormCRecord.CouponNumber),
                                                                                                 fileName, ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                                 SamplingErrorCodes.TicketDetailsDoesNotExistsInFormAb,
                                                                                                 ErrorStatus.X, samplingFormCRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
            else
            {
              if (couponRecord.BatchSequenceNumber != samplingFormCRecord.BatchNumberOfProvisionalInvoice)
              {
                if (exceptionDetailsList != null)
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormCRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                                    samplingFormC,
                                                                                                    fileSubmissionDate,
                                                                                                    "Batch Number Of Provisional Invoice",
                                                                                                    Convert.ToString(samplingFormCRecord.BatchNumberOfProvisionalInvoice),
                                                                                                    fileName,
                                                                                                    ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                                    SamplingErrorCodes.InvalidProvisionalInvoiceBatchNo,
                                                                                                    ErrorStatus.C,
                                                                                  samplingFormCRecord, isLinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalInvoiceBatchNo);
                }
              }

              if (couponRecord.RecordSequenceWithinBatch != samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice)
              {
                if (exceptionDetailsList != null)
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormCRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                                    samplingFormC,
                                                                                                    fileSubmissionDate,
                                                                                                    "Record SeqNumber Of Provisional Invoice",
                                                                                                    Convert.ToString(samplingFormCRecord.BatchNumberOfProvisionalInvoice),
                                                                                                    fileName,
                                                                                                    ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                                    SamplingErrorCodes.InvalidProvisionalInvoiceBatchSeqNo,
                                                                                  ErrorStatus.C,
                                                                                  samplingFormCRecord, isLinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalInvoiceBatchSeqNo);
                }
              }

              //CMP#601:Removal of E-Ticket Indicator Validation in PAX Sampling Form C
              //if (couponRecord.ETicketIndicator != samplingFormCRecord.ETicketIndicator)
              //{
              //  if (exceptionDetailsList != null)
              //  {
              //    var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
              //                                                                                      samplingFormC,
              //                                                                                      fileSubmissionDate,
              //                                                                                      "E Ticket Indicator",
              //                                                                                      Convert.ToString(samplingFormCRecord.ETicketIndicator),
              //                                                                                      fileName,
              //                                                                                      ErrorLevels.ErrorLevelSamplingFormCRecord,
              //                                                                                      ErrorCodes.InvalidElectronicTicketIndicator,
              //                                                                    ErrorStatus.X,
              //                                                                                      samplingFormCRecord);
              //    exceptionDetailsList.Add(validationExceptionDetail);
              //    isValid = false;
              //  }
              //  else
              //  {
              //    throw new ISBusinessException(ErrorCodes.InvalidAgreementIndicatorValidated);
              //  }
              //}

              // SCP54940
              // The difference between gross value of Form C coupon and Form A/B coupon should not exceed a tolerence of 0.01
              // return a validation error if the difference exceeds above mentioned tolerence.
              if (Math.Abs(ConvertUtil.Round((couponRecord.CouponGrossValueOrApplicableLocalFare - samplingFormCRecord.GrossAmountAlf), Constants.SamplingConstantDecimalPlaces)) > 0.01)
              {
                if (exceptionDetailsList != null)
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                                    samplingFormC,
                                                                                                    fileSubmissionDate,
                                                                                                    "Gross Amount Alf",
                                                                                                    Convert.ToString(samplingFormCRecord.GrossAmountAlf),
                                                                                                    fileName,
                                                                                                    ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                                    SamplingErrorCodes.InvalidProvisionalGrossAlfAmount,
                                                                                                    ErrorStatus.X,
                                                                                                    samplingFormCRecord);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalGrossAlfAmount);
                }
              }
            }
          }
        }

        else
        {
          Logger.InfoFormat("Provisional Invoice is not found: {0}", samplingFormCRecord.ProvisionalInvoiceNumber);
          if (!IgnoreBillingHistoryValidation(samplingFormC))
          {
            if (exceptionDetailsList != null)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormCRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                            samplingFormC, fileSubmissionDate,
                                                                                            "Provisional Invoice Number",
                                                                                            samplingFormCRecord.ProvisionalInvoiceNumber,
                                                                                            fileName, ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                            SamplingErrorCodes.InvalidLinkedProvisionalInvoice,
                                                                              ErrorStatus.C, samplingFormCRecord, isLinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          else
          {
            if (exceptionDetailsList != null)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                            samplingFormC, fileSubmissionDate,
                                                                                            "Provisional Invoice Number",
                                                                                            samplingFormCRecord.
                                                                                                ProvisionalInvoiceNumber,
                                                                                            fileName,
                                                                                            ErrorLevels.
                                                                                                ErrorLevelSamplingFormCRecord,
                                                                                            SamplingErrorCodes.
                                                                                                InvalidLinkedProvisionalInvoice,
                                                                                            ErrorStatus.W, samplingFormCRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
          }
        }
      }
      return isValid;
    }

     /// <summary>
    /// Validate source code total fields with total of Form C Coupon records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <returns></returns>
    private static bool ValidateParsedSourceCodeTotalForFormCRecords(SamplingFormC invoice, SamplingFormCSourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      List<SamplingFormCSourceCodeTotal> formCSourceCodeTotal = null;

      formCSourceCodeTotal = invoice.SamplingFormCSourceCodeTotals.Where(c => c.SourceId == sourceCode).ToList();
        Tolerance tolerance = null;
        //Calculate tolerance if NilFormCIndicator='N' and Listing Currency is present.
        if (invoice.NilFormCIndicator.Equals("N", StringComparison.OrdinalIgnoreCase) && invoice.ListingCurrencyId.HasValue)
        tolerance = GetTolerance(BillingCategoryType.Pax, invoice.ListingCurrencyId.Value, invoice, Constants.PaxDecimalPlaces);

      if (formCSourceCodeTotal != null)
      {

        if (tolerance != null)
        {
          if (!CompareUtil.Compare(sourceCodeTotal.TotalGrossValue, (decimal)invoice.SamplingFormCDetails.Sum(record => record.GrossAmountAlf), tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {


            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                 invoice,
                                                                 fileSubmissionDate,
                                                                 "Total Gross Value",
                                                                 null,
                                                                 sourceCodeTotal.TotalGrossValue.ToString(),
                                                                 fileName,
                                                                 ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                 ErrorCodes.InvalidGrossTotalOfSourceCodeTotalRecord,
                                                                 ErrorStatus.X, sourceCode, 99999,
                                                                            99999);

            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }




        }

        //Total number of records validation
                //long totalNumberOfRecords = formCSourceCodeTotal.Count + formCSourceCodeTotal.Sum(record => record.TotalNumberOfRecords) +  sourceCodeTotal.NoOfBillingRecord + 1;

                long totalNumberOfRecords = formCSourceCodeTotal.Count + invoice.SamplingFormCDetails.Count();

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.TotalNumberOfRecords != totalNumberOfRecords)
        {
          //var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", sourceCodeTotal.TotalNumberOfRecords.ToString(),
          //                                                    null, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidTotalNumberOfRecords, ErrorStatus.X, sourceCode, 99999, 99999, null, false, null, Convert.ToString(totalNumberOfRecords));


          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1,
                                                             invoice,
                                                             fileSubmissionDate,
                                                             "Total Number of Records",
                                                             null,
                                                             sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                             fileName,
                                                             ErrorLevels.ErrorLevelSourceCodeTotal,
                                                             ErrorCodes.InvalidTotalNumberOfRecords,
                                                             ErrorStatus.X, sourceCode, 99999,
                                                                        99999);


          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.NoOfBillingRecord != invoice.SamplingFormCDetails.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1,
                                                             invoice,
                                                             fileSubmissionDate,
                                                             "Number of billing records",
                                                             null,
                                                             sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                             fileName,
                                                             ErrorLevels.ErrorLevelSourceCodeTotal,
                                                             ErrorCodes.InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord,
                                                             ErrorStatus.X, sourceCode, 99999,
                                                                        99999);

          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && sourceCodeTotal.NoOfBillingRecord != invoice.SamplingFormCDetails.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1,
                                                             invoice,
                                                             fileSubmissionDate,
                                                             "Detail Count",
                                                             null,
                                                             sourceCodeTotal.NoOfBillingRecord.ToString(),
                                                             fileName,
                                                             ErrorLevels.ErrorLevelSourceCodeTotal,
                                                             ErrorCodes.InvalidDetailCount,
                                                             ErrorStatus.X, sourceCode, 99999,
                                                                        99999);



          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
         

      }
      return isValid;
    }





    /// <summary>
    /// Ignores the billing history validation.
    /// </summary>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <returns></returns>
    private bool IgnoreBillingHistoryValidation(SamplingFormC samplingFormC)
    {
      if (!IsProvisionalBillingMemberMigrated(samplingFormC))
      {
        return true;
      }
      //if (SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
      //{
      //  return true;
      //}
      return false;
    }

    /// <summary>
    /// Validates the ticket issuing airline.
    /// </summary>
    /// <param name="samplingFormCRecord">The sampling form C record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <returns></returns>
    private bool ValidateTicketIssuingAirline(SamplingFormCRecord samplingFormCRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, IDictionary<string, bool> issuingAirline, bool isValid, DateTime fileSubmissionDate, SamplingFormC samplingFormC)
    {
      if (!issuingAirline.Keys.Contains(samplingFormCRecord.TicketIssuingAirline))
      {
        if (MemberManager.IsValidAirlineCode(samplingFormCRecord.TicketIssuingAirline))
        {
          issuingAirline.Add(samplingFormCRecord.TicketIssuingAirline, true);
        }
        else
        {
          issuingAirline.Add(samplingFormCRecord.TicketIssuingAirline, false);
          if (exceptionDetailsList != null)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Ticket Or Fim Issuing Airline",
                                                                                        Convert.ToString(samplingFormCRecord.TicketIssuingAirline),
                                                                                         fileName,
                                                                                        ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                        ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X,
                                                                                        samplingFormCRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {
            throw new ISBusinessException(ErrorCodes.InvalidTicOrFimIssuingAirline);
          }
        }
      }
      else if (!issuingAirline[samplingFormCRecord.TicketIssuingAirline])
      {
        if (exceptionDetailsList != null)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(samplingFormC.Id.Value(), exceptionDetailsList.Count() + 1, samplingFormC, fileSubmissionDate, "Ticket Or Fim Issuing Airline",
                                                                                    Convert.ToString(
                                                                                      samplingFormCRecord.TicketIssuingAirline),
                                                                                     fileName,
                                                                                    ErrorLevels.ErrorLevelSamplingFormCRecord,
                                                                                    ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X,
                                                                                    samplingFormCRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          throw new ISBusinessException(ErrorCodes.InvalidTicOrFimIssuingAirline);
        }
      }
      return isValid;
    }


    public bool DeleteSamplingFormCRecord(string samplingFormCRecordId, out SamplingFormC samplingFormC)
    {
      var samplingFormCRecordGuid = samplingFormCRecordId.ToGuid();
      //var samplingFormCToBeDeleted = SamplingFormCRecordRepository.Single(sfcRecord => sfcRecord.Id == samplingFormCRecordGuid);
      // Call replaced by load strategy 
      var samplingFormCToBeDeleted = SamplingFormCRecordRepository.Single(samplingFormCRecordId: samplingFormCRecordGuid);

      samplingFormC = SamplingFormCRepository.Single(sfcHeader => sfcHeader.Id == samplingFormCToBeDeleted.SamplingFormCId);

      if (samplingFormCToBeDeleted == null) return false;

      // Form C can be modified even if it has status “Ready for Submission”. Upon the first modification, its status will revert to “Open”.
      if (samplingFormC.InvoiceStatus != InvoiceStatusType.Open)
      {
        samplingFormC.InvoiceStatus = InvoiceStatusType.Open;
        SamplingFormCRepository.Update(samplingFormC);
      }

      SamplingFormCRecordRepository.Delete(samplingFormCToBeDeleted);
      UnitOfWork.CommitDefault();

      return true;
    }

    public IList<SamplingFormC> UpdateSamplingFormCStatuses(List<Guid> formCIdList)
    {
      var formCList = formCIdList.Select(UpdateSamplingFormCStatus).ToList();

      return formCList.Where(formC => formC != null && formC.InvoiceStatus == InvoiceStatusType.Presented).ToList();
    }

    /// <summary>
    /// Update invoice
    /// </summary>
    public SamplingFormC UpdateSamplingFormCStatus(Guid formCGuid)
    {
      var formC = SamplingFormCRepository.Single(formCObj => formCObj.Id == formCGuid);

      if (formC != null && formC.InvoiceStatus == InvoiceStatusType.Presented)
      {
        return null;

      }

      formC.InvoiceStatus = InvoiceStatusType.Presented;

      // Update FormC to database.
      var updatedFormC = SamplingFormCRepository.Update(formC);

      UnitOfWork.CommitDefault();

      return updatedFormC;
    }

    /// <summary>
    /// Update form C invoicestatus to 'Processing Complete'.
    /// </summary>
    /// <returns></returns>
    public bool UpdateFormCStatusToProcessingComplete()
    {
      SamplingFormCRepository = Ioc.Resolve<ISamplingFormCRepository>();
      SamplingFormCRecordRepository = Ioc.Resolve<ISamplingFormCRecordRepository>();
      SamplingFormCAttachmentRepository = Ioc.Resolve<ISamplingFormCAttachmentRepository>();

      var previousPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);
      var pervPeriodMonth = previousPeriod.Month.ToString();
      if (previousPeriod.Month.ToString().Length == 1)
      {
        pervPeriodMonth = "0" + previousPeriod.Month;
      }

      var prevPeriod = "0" + previousPeriod.Period;
      var processingCompleteDate = previousPeriod.Year + "-" + pervPeriodMonth + "-" + prevPeriod;
      try
      {

        var samplingFormCList = SamplingFormCRepository.Get(formC => formC.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling);
        foreach (var samplingFormC in samplingFormCList)
        {
          var c = samplingFormC;
          var formCRecordList = SamplingFormCRecordRepository.Get(samplingFormCRecord => samplingFormCRecord.SamplingFormCId == c.Id).ToList();

          samplingFormC.InvoiceStatusId = (int)InvoiceStatusType.ProcessingComplete;
          samplingFormC.ProcessingCompletedOn = DateTime.Parse(processingCompleteDate);
          samplingFormC.LastUpdatedOn = DateTime.UtcNow;
          SamplingFormCRepository.Update(samplingFormC);

        }

        var formCAttachmentList = SamplingFormCAttachmentRepository.Get(attachment => attachment.ProcessingCompletedOn == null);
        foreach (var samplingFormCRecordAttachment in formCAttachmentList)
        {
          samplingFormCRecordAttachment.ProcessingCompletedOn = DateTime.Parse(processingCompleteDate);
          samplingFormCRecordAttachment.LastUpdatedOn = DateTime.UtcNow;
          SamplingFormCAttachmentRepository.Update(samplingFormCRecordAttachment);

         }
        UnitOfWork.CommitDefault();

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// To generate Nil Form C.
    /// </summary>
    /// <param name="provisionalBillingMonth">provisional billing Month</param>
    public void GenerateNilFormC(string provisionalBillingMonth, int memberId, Boolean isReprocess)
    {
      //var result = CreateNilFormC(26, 27, 04, 2011);
      if (!String.IsNullOrEmpty(provisionalBillingMonth))
      {
        var provisionalMonth = int.Parse(provisionalBillingMonth.Substring(4, 2));
        var provisionalYear = int.Parse(provisionalBillingMonth.Substring(0, 4));

        if (isReprocess)
        {
          // system Monitor Re-process 
          if (memberId > 0)
          {
            // For Specific Member
            var provisionalInvoiceList = InvoiceBaseRepoeitory.Get(
                      invoice =>
                      invoice.BillingMonth == provisionalMonth && invoice.BillingYear == provisionalYear && invoice.BillingCode == (int)BillingCode.SamplingFormAB && invoice.BillingMemberId == (int)memberId &&
                      invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented).ToList();

            foreach (var provisionalInvoice in provisionalInvoiceList)
           {
                var billedMembers = provisionalInvoice.BilledMemberId;
              var billingMember = provisionalInvoice.BillingMemberId;

              CheckForUnProcessedFormCInvoices(billingMember, billedMembers, int.Parse(provisionalBillingMonth.Substring(4, 2)), int.Parse(provisionalBillingMonth.Substring(0, 4)), isReprocess);
 
            }

          }
          else
          {
            // For all Member
            var provisionalInvoiceList = InvoiceBaseRepoeitory.Get(
           invoice =>
           invoice.BillingMonth == provisionalMonth && invoice.BillingYear == provisionalYear && invoice.BillingCode == (int)BillingCode.SamplingFormAB &&
           invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented).ToList();

            foreach (var provisionalInvoice in provisionalInvoiceList)
            {

              var billedMembers = provisionalInvoice.BilledMemberId;
              var billingMember = provisionalInvoice.BillingMemberId;

              CheckForUnProcessedFormCInvoices(billingMember, billedMembers, int.Parse(provisionalBillingMonth.Substring(4, 2)), int.Parse(provisionalBillingMonth.Substring(0, 4)), isReprocess);

            }

          }
                 
        }
        else
        {
          // Normal Process
          var provisionalInvoiceList = InvoiceBaseRepoeitory.Get(
           invoice =>
           invoice.BillingMonth == provisionalMonth && invoice.BillingYear == provisionalYear && invoice.BillingCode == (int)BillingCode.SamplingFormAB &&
           invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented).ToList();

   foreach (var provisionalInvoice in provisionalInvoiceList)
          {

            var billedMembers = provisionalInvoice.BilledMemberId;
            var billingMember = provisionalInvoice.BillingMemberId;

            CheckForUnProcessedFormCInvoices(billingMember, billedMembers, int.Parse(provisionalBillingMonth.Substring(4, 2)), int.Parse(provisionalBillingMonth.Substring(0, 4)), isReprocess);


                    }
        }

      }
    }

    public void CheckForUnProcessedFormCInvoices(int billingMember, int billedMember, int provisionalMonth, int provosionalYear, Boolean isReprocess)
    {
      SamplingFormCRepository = Ioc.Resolve<ISamplingFormCRepository>();
      var processedFormCInvoicesCount = SamplingFormCRepository.GetCount(formC => formC.FromMemberId == billedMember && formC.ProvisionalBillingMemberId == billingMember && formC.ProvisionalBillingMonth == provisionalMonth && formC.ProvisionalBillingYear == provosionalYear && (formC.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || formC.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || formC.InvoiceStatusId == (int)InvoiceStatusType.Presented));
      if (processedFormCInvoicesCount == 0)
      {
        //below code updated for SCP#42663
        var unProcessedFormCInvoices = SamplingFormCRepository.Get(formC => formC.FromMemberId == billedMember && formC.ProvisionalBillingMemberId == billingMember && formC.ProvisionalBillingMonth == provisionalMonth && formC.ProvisionalBillingYear == provosionalYear && (formC.InvoiceStatusId == (int)InvoiceStatusType.Open || formC.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission || formC.InvoiceStatusId == (int)InvoiceStatusType.ErrorCorrectable || formC.InvoiceStatusId == (int)InvoiceStatusType.ValidationError || formC.InvoiceStatusId == (int)InvoiceStatusType.OnHold));
        if (unProcessedFormCInvoices.Count() == 0)
        {
          var nilFormCRecords =
            SamplingFormCRepository.Get(
              formC =>
              formC.FromMemberId == billedMember && formC.ProvisionalBillingMemberId == billingMember &&
              formC.ProvisionalBillingMonth == provisionalMonth && formC.ProvisionalBillingYear == provosionalYear &&
             formC.NilFormCIndicator == "S");
          if (nilFormCRecords.Count() == 0)
          {
            var result = CreateNilFormC(billedMember, billingMember, provisionalMonth, provosionalYear);
          }

        }
        else
        {
            //below code updated for SCP#42663
            //---created NilFormC-------------------------------------
                   var nilFormCRecords =
                   SamplingFormCRepository.Get(
                     formC =>
                     formC.FromMemberId == billedMember && formC.ProvisionalBillingMemberId == billingMember &&
                     formC.ProvisionalBillingMonth == provisionalMonth && formC.ProvisionalBillingYear == provosionalYear &&
                    formC.NilFormCIndicator == "S");
                   if (nilFormCRecords.Count() == 0)
                   {
                     var result = CreateNilFormC(billedMember, billingMember, provisionalMonth, provosionalYear);
                     foreach (var unProcessedFormCInvoice in unProcessedFormCInvoices)
                     {
                       unProcessedFormCInvoice.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                       SamplingFormCRepository.Update(unProcessedFormCInvoice);
                     }
                     UnitOfWork.CommitDefault();
                   }
            //---End of created NilFormC -------------------------------------
                   
        }
      }

    }

    /// <summary>
    /// Inserts the generate nil form C message in oracle queue.
    /// </summary>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    public void InsertGenerateNilFormCMessageInOracleQueue(string provisionalBillingMonth)
    {
      // Update sample digit for the provisional billing month
      UpdateSampleDigit(provisionalBillingMonth);

      UnitOfWork.CommitDefault();

      // en-queue message
      IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                              { "PROV_BILLING_MONTH", provisionalBillingMonth },
                                                                              { "MEMBER_ID", "0" },
                                                                              { "REGENERATION_FLAG", "0" }
                                                                            };
      var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["GenerateNilFormCJobQueueName"].Trim());
      queueHelper.Enqueue(messages);
    }

    public void UpdateSampleDigit(string provisionalBillingMonth)
    {
      var date = DateTime.UtcNow.Date;
      var sampleDigitRecord = SampleDigitRepository.Single(samlpleDigit => samlpleDigit.ProvisionalBillingMonth == provisionalBillingMonth);
      if (sampleDigitRecord == null)
      {
        sampleDigitRecord = new SampleDigit
        {
          IsActive = true,
          DigitAnnouncementDateTime = date,
          ProvisionalBillingMonth = provisionalBillingMonth
        };
        SampleDigitRepository.Add(sampleDigitRecord);
        return;
      }

      sampleDigitRecord.DigitAnnouncementDateTime = date;
      SampleDigitRepository.Update(sampleDigitRecord);
      UnitOfWork.CommitDefault();
    }

    private bool CreateNilFormC(int billedMember, int billingMember, int provisionalBillingMonth, int provisionalBillingYear)
    {
      //SCP238744: Form C stuck in status "Ready For Billing"
      var samplingFormC = new SamplingFormC
      {
        NilFormCIndicator = "S",
        FromMemberId = billedMember,
        ProvisionalBillingMemberId = billingMember,
        ProvisionalBillingMonth = provisionalBillingMonth,
        InvoiceStatusId = (int)InvoiceStatusType.ReadyForBilling,
        ProvisionalBillingYear = provisionalBillingYear,
        ValidationStatus = (int)InvoiceValidationStatus.Completed,
        ValidationStatusDate =  DateTime.UtcNow
      };
      SamplingFormCRepository.Add(samplingFormC);
      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// Following method updates FormC SourceCode total and is called when FormC is submitted
    /// </summary>
    /// <param name="formCId">FormC ID</param>
    public void UpdateFormCSourceCodeTotal(Guid formCId)
    {
      // Execute stored procedure which will update FormC SourceCode total
      SamplingFormCRepository.UpdateFormCSourceCodeTotal(formCId);
    }



    /// <summary>
    /// Gets the tolerance.
    /// </summary>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="currencyId">The currency id.</param>
    /// <param name="invoice"></param>
    /// <param name="allowedDecimalPlaces"></param>
    public static Tolerance GetTolerance(BillingCategoryType billingCategory, int currencyId, SamplingFormC invoice, int allowedDecimalPlaces)
    {
      if (invoice == null)
      {
        //throw new ArgumentNullException("invoice");
        return null;
      }

      var toleranceRepository = Ioc.Resolve<IRepository<Tolerance>>();
      var exchangRateRepository = Ioc.Resolve<IRepository<ExchangeRate>>();

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";
      DateTime billingDate;

      if (invoice.ProvisionalBillingYear == 0 || invoice.ProvisionalBillingMonth == 0)
      {
        return null;
      }

      // To search exchange rate for the billing month.
      var conversionSuccessful = DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.ProvisionalBillingYear, invoice.ProvisionalBillingMonth.ToString("00"), "01"), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      if (!conversionSuccessful)
      {
        return null;
      }

      var clearingHouse = GetClearingHouse((int)SMI.Ich);

      Tolerance toleranceValueCache;

      var toleranceValue = new Tolerance();

      if (ValidationCache.Instance != null && ValidationCache.Instance.ToleranceList != null)
        toleranceValueCache = ValidationCache.Instance.ToleranceList.Find(rec => rec.IsActive && rec.BillingCategoryId == (int)billingCategory && rec.ClearingHouse == clearingHouse && rec.Type.ToUpper() == "A");
      else
        toleranceValueCache = toleranceRepository.Single(rec => rec.IsActive && rec.BillingCategoryId == (int)billingCategory && rec.ClearingHouse == clearingHouse && rec.Type.ToUpper() == "A");

      if (toleranceValueCache != null)
      {
        // Get exchange rate query.
        var exchangeRates = exchangRateRepository.Get(rec => rec.IsActive && rec.CurrencyId == currencyId && rec.EffectiveFromDate <= billingDate
                 && rec.EffectiveToDate >= billingDate);

        // Get Five Day exchange rate only.
        var exchangeRateList = exchangeRates.Select(rate => new { rate.FiveDayRateUsd }).ToList();
        if (exchangeRateList.Count > 0)
        {
          var exchangeRate = exchangeRateList[0];
          toleranceValue.SummationTolerance = Math.Round(toleranceValueCache.SummationTolerance * exchangeRate.FiveDayRateUsd, allowedDecimalPlaces);
          toleranceValue.RoundingTolerance = Math.Round(toleranceValueCache.RoundingTolerance * exchangeRate.FiveDayRateUsd, allowedDecimalPlaces);
        }
        else
        {
          //Logger.Info("ExchangeRate is null for billing date = " + billingDate.ToString());
        }
      }
      else
      {
        //Logger.Info(string.Format("{0}{1}", "Tolerance is NULL for invoice", invoice.InvoiceNumber));
      }
      return toleranceValue;
    }

    /// <summary>
    /// Get clearing house from settlement method
    /// </summary>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    public static string GetClearingHouse(int settlementMethodId)
    {
      string clearingHouse = string.Empty;
      if (settlementMethodId == (int)SMI.Ach)
      {
        clearingHouse = "A";
      }
      else if (settlementMethodId == (int)SMI.Ich || settlementMethodId == (int)SMI.AchUsingIataRules || settlementMethodId == (int)SMI.Bilateral || settlementMethodId == (int)SMI.AdjustmentDueToProtest)
      {
        clearingHouse = "I";
      }
      return clearingHouse;
    }

    /// <summary>
    /// Gets details of sampling form c headers.
    /// </summary>
    /// <param name="samplingFormCId">sampling form c id </param>
    /// <returns>Sampling form c header detail</returns>
    //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
    public SamplingFormC GetSamplingFormCHeaderDetails(string samplingFormCId)
    {
      var samplingFormCGuid = samplingFormCId.ToGuid();
      var samplingFormCHeader = SamplingFormCRepository.Single(samplingFormC => samplingFormC.Id == samplingFormCGuid);
      
      return samplingFormCHeader;
    }
  }

}
