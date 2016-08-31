using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Isr;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Base;
using Iata.IS.Data.Common;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Base;
using System.Reflection;
using System.Diagnostics;
using NVelocity;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Data.Isr.Impl;
using IRejectionMemoRecordRepository = Iata.IS.Data.Pax.IRejectionMemoRecordRepository;
using Iata.IS.Data.Impl;
using Iata.IS.DR.Business.Validation.Pax;

namespace Iata.IS.Business.Pax.Impl
{
  public class NonSamplingInvoiceManager : InvoiceManager, INonSamplingInvoiceManager, IValidationNSInvoiceManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string TotalGrossDifferenceRejectionFlag = "G";
    private const string TotalTaxDifferenceRejectionFlag = "T";
    private const string TotalIscDifferenceRejectionFlag = "I";
    private const string TotalOtherCommisionDifferenceRejectionFlag = "O";
    private const string TotalHandlingFeeDifference = "H";
    private const string TotalUatpFeeDifference = "U";
    private const string TotalVatAmountFeeDifference = "V";
    private const string ReasonCode = "1A";
    private const string TimeLimitFlag = "TL";
    private const string ValidationFlagDelimeter = ",";
    private const string DuplicateValidationFlag = "DU";
    private const string ReasonCode6B = "6B";
    private const string ReasonCode6A = "6A";
    private const string AutoBillProrationType = "F";
    private const string HandlingFeeAggTypeAmountBased = "F";
    private const string HandlingFeeAggTypePercentBased = "P";
    private const string AbillUnavaliableInvoiceConstant = "UnavaliableInvoice";
    private const string AbillThreshouldConstant = "ThreshouldReached";
    private ITemplatedTextGenerator _templatedTextGenerator;
    private const string PaxBillingHistoryAuditTrailTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.PaxBillingHistoryAuditTrailPdf.vm";
    private const string ReasonCodeErrorMessage = "Reason Code";
    private const string ValidationErrorDelimeter = "; ";
    private const int MaxReasonRemarkCharLength = 4000;
    // CMP #672: Validation on Taxes in PAX FIM Billings
    private const int FimSourceCode14Pax = 14;
    private PaxValidationManager paxValidationManager = new PaxValidationManager();

    /// <summary>
    /// Gets or sets SourceCodevatTotal repository
    /// </summary>
    public ICargoBillingCodeSubTotalVatRepository CargoBillingCodeSubTotalVatRepository { get; set; }

      /// <summary>
      /// Dictionary object used to store whether invoice for YourInvoice detail provided in RM  exists in DB. 
      /// </summary>
    private Dictionary<string, int> IsYourInvoiceNumberPresent = new Dictionary<string, int>();
    /// <summary>
    /// Gets or sets the Ach configuration repository.
    /// </summary>
    /// <value>The Ach configuration repository.</value>
    public IRepository<AchConfiguration> AchRepository { get; set; }
   
    /// <summary>
    /// Gets or sets the coupon tax repository.
    /// </summary>
    /// <value>The coupon tax repository.</value>
    public IRepository<PrimeCouponTax> CouponTaxRepository { get; set; }


    /// <summary>
    /// Gets or sets the exchange rate repository.
    /// </summary>
    /// <value>The exchange rate repository.</value>
    public IRepository<ExchangeRate> ExchangeRateRepository { get; set; }

    /// <summary>
    /// Gets or sets the currency repository.
    /// </summary>
    /// <value>The currency repository.</value>
    public IRepository<Currency> CurrencyRepository { get; set; }

    /// <summary>
    /// Gets or sets the coupon attachment repository.
    /// </summary>
    /// <value>The coupon attachment repository.</value>
    public ICouponRecordAttachmentRepository CouponAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing memo repository.
    /// </summary>
    /// <value>The billing memo repository.</value>
    public IBillingMemoRecordRepository BillingMemoRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing memo coupon breakdown repository.
    /// </summary>
    /// <value>The billing memo coupon breakdown repository.</value>
    public IBillingMemoCouponBreakdownRecordRepository BillingMemoCouponBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the coupon record vat repository.
    /// </summary>
    /// <value>The coupon record vat repository.</value>
    public IRepository<PrimeCouponVat> CouponRecordVatRepository { get; set; }

    /// <summary>
    /// Gets or sets the member loc information repository.
    /// </summary>
    /// <value>The member loc information repository.</value>
    public IRepository<MemberLocationInformation> MemberLocInformationRepository { get; set; }

    /// <summary>
    /// Gets or sets the airport repository.
    /// </summary>
    /// <value>The airport repository.</value>
    public IRepository<CityAirport> AirportRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo repository.
    /// </summary>
    /// <value>The rejection memo repository.</value>
    public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo vat repository.
    /// </summary>
    public IRepository<RejectionMemoVat> RejectionMemoVatRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo Attachment repository.
    /// </summary>
    public IRejectionMemoAttachmentRepository RejectionMemoAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the Passenger Configuration repository.
    /// </summary>
    public IRepository<PassengerConfiguration> PassengerReository { get; set; }

    /// <summary>
    /// Gets or sets RM Reason code Acceptable Difference repository.
    /// </summary>
    public IRepository<RMReasonAcceptableDiff> RMReasonAcceptableDiffRepository { get; set; }

    /// <summary>
    ///   Gets or sets the rejection memo Coupon Tax Breakdown repository.
    /// </summary>
    public IRepository<RMCouponTax> RejectionMemoCouponTaxBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo Coupon Attachment repository.
    /// </summary>
    public IRejectionMemoCouponAttachmentRepository RejectionMemoCouponAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo Coupon Vat Breakdown repository.
    /// </summary>
    public IRepository<RMCouponVat> RejectionMemoCouponVatBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the BillingMemoVat Breakdown repository.
    /// </summary>
    public IRepository<BillingMemoVat> BillingMemoVatRepository { get; set; }

    /// <summary>
    /// Gets or sets the Billing Memo Attachment repository.
    /// </summary>
    public IBillingMemoAttachmentRepository BillingMemoAttachmentRepository { get; set; }

    /// <summary>
    ///  Gets or sets the Billing Memo TaxBreakdown repository.
    /// </summary>
    public IRepository<BMCouponTax> BillingMemoCouponTaxBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the Billing Memo Coupon VatBreakdown repository.
    /// </summary>
    public IRepository<BMCouponVat> BillingMemoCouponVatBreakdownRepository { get; set; }

    /// <summary>
    /// Gets or sets the Billing Memo Coupon Attachment repository.
    /// </summary>
    public IBillingMemoCouponAttachmentRepository BillingMemoCouponAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the non sampling credit note manager.
    /// </summary>
    /// <value>The non sampling credit note manager.</value>
    public INonSamplingCreditNoteManager NonSamplingCreditNoteManager { get; set; }

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

    /// <summary>
    /// Gets or sets the passenger repository.
    /// </summary>
    /// <value>The passenger repository.</value>
    public IRepository<PassengerConfiguration> PassengerRepository { get; set; }

    /// <summary>
    /// Gets or sets the rejection memo record repository.
    /// </summary>
    /// <value>The rejection memo record repository.</value>
    public IRejectionMemoRecordRepository RejectionMemoRecordRepository { get; set; }

    /// <summary>
    /// Gets or sets the billing memo record repository.
    /// </summary>
    /// <value>The billing memo record repository.</value>
    public IBillingMemoRecordRepository BillingMemoRecordRepository { get; set; }

    public IBlockingRulesRepository BlockingRulesRepository { get; set; }

    public IRepository<Correspondence> PaxCorrespondenceRepository { get; set; }

    public IValueCouponRequestRepository ValueRequestCouponRepository { get; set; }

    public IAutoBillingNotificationManager AutoBillingNotification { get; set; }

    public IRepository<PrimeCoupon> PrimeCouponRecordRepository { get; set; }

    public IExchangeRateManager ExchangeRateManager { get; set; }

    public IQueryAndDownloadDetailsManager QueryAndDownloadDetailsManager { get; set; }

    /// <summary>
    /// list of all coupon records
    /// </summary>
    /// <param name="invoiceId">list of all coupon records for this invoice id.</param>
    /// <returns></returns>
    /* SCP ID : 71798 - US Form D Entry - F&F
    Date: 25-02-2013
    Desc: Method return type is changed from IList<> to IQueryable<>. Use of ToList() was removed as it was identified as unnecessory performance heavy operation.
    IList<> actually executes the query on database; On the contrary IQueryable<> differe the query execution using entity framework.
    Differed query execution is advantageous as it select only required number of rows for binding to grid.
    */
    public IQueryable<PrimeCoupon> GetPrimeBillingCouponList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var primeBillingList = CouponRecordRepository.Get(cr => cr.InvoiceId == invoiceGuid);

      return primeBillingList;
    }

    public IQueryable<PrimeCoupon> GetAutoBillingPrimeCouponList(AutoBillingSearchCriteria searchCriteria)
    {
      var filteredList = CouponRecordRepository.GetAll();

      if (searchCriteria != null)
      {
        if (searchCriteria.BillingMemberId > 0)
        {
          filteredList = filteredList.Where(coupon => coupon.Invoice.BillingMemberId == searchCriteria.BillingMemberId);
        }

        if (searchCriteria.BilledMemberId > 0)
        {
          filteredList = filteredList.Where(coupon => coupon.Invoice.BilledMemberId == searchCriteria.BilledMemberId);
        }

        if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
        {
          var invoice = PaxInvoiceRepository.Single(coupon => coupon.InvoiceNumber == searchCriteria.InvoiceNumber);

          if (!string.IsNullOrEmpty(searchCriteria.InvoiceNumber))
          {
            filteredList = filteredList.Where(inv => inv.InvoiceId == invoice.Id);
          }
        }

        if (searchCriteria.SourceCode > 0)
        {
          filteredList = filteredList.Where(coupon => coupon.SourceCodeId == searchCriteria.SourceCode);
        }

        if (!string.IsNullOrEmpty(searchCriteria.ProrateMethodology))
        {
          filteredList = filteredList.Where(coupon => coupon.ProrateMethodology == searchCriteria.ProrateMethodology);
        }



        if (searchCriteria.TicketDocNumber > 0)
        {
          filteredList = filteredList.Where(coupon => coupon.TicketDocOrFimNumber == searchCriteria.TicketDocNumber);
        }

        if (searchCriteria.CouponNumber > 0)
        {
          filteredList = filteredList.Where(coupon => coupon.TicketOrFimCouponNumber == searchCriteria.CouponNumber);
        }

        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        var billingPeriod = calendarManager.GetCurrentAutoBillingPeriod(searchCriteria.BillingMemberId);
        filteredList = filteredList.Where(coupon => coupon.Invoice.BillingPeriod == billingPeriod.Period &&
                                          coupon.Invoice.BillingMonth == billingPeriod.Month &&
                                          coupon.Invoice.BillingYear == billingPeriod.Year);


        filteredList = filteredList.Where(coupon => coupon.Invoice.BillingCategoryId == (int)BillingCategoryType.Pax);
        filteredList = filteredList.Where(coupon => coupon.Invoice.SubmissionMethodId == (int)SubmissionMethod.AutoBilling);
        filteredList = filteredList.Where(coupon => coupon.Invoice.InvoiceStatusId == (int)InvoiceStatusType.Open);
      }

      return filteredList;
    }

    /// <summary>
    /// Detail of specific coupon record
    /// </summary>
    public PrimeCoupon GetCouponRecordDetails(string couponRecordId)
    {
      var couponRecordGuid = couponRecordId.ToGuid();

      // Replaced with LoadStrategy call
      var couponRecords = CouponRecordRepository.Single(couponId: couponRecordGuid);

      return couponRecords;
    }

    /// <summary>
    /// New coupon record to be added
    /// </summary>
    /// <param name="couponRecord">The coupon record.</param>
    /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
    /// <returns>Added coupon record</returns>
    public PrimeCoupon AddCouponRecord(PrimeCoupon couponRecord, out string duplicateCouponErrorMessage)
    {
        if (couponRecord.RecordSequenceWithinBatch <= 0 || couponRecord.BatchSequenceNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }

      // Check whether Batch and Sequence number combination is valid and check whether Batch number is not repeated between different source codes
      int invalidBatchSequenceNumber = InvoiceRepository.IsValidBatchSequenceNo(couponRecord.InvoiceId, couponRecord.RecordSequenceWithinBatch, couponRecord.BatchSequenceNumber, couponRecord.Id, couponRecord.SourceCodeId);

      // If value != 0, either Batch and Sequence number combination is invalid or Batch number is repeated between different source codes  
      if (invalidBatchSequenceNumber != 0)
      {
        // If value == 1, Batch number is repeated between different source codes, else if value == 2, Batch and Sequence number combination is invalid  
        if (invalidBatchSequenceNumber == 1)
          throw new ISBusinessException(ErrorCodes.InvalidBatchNo);
        else
          throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNo);
      }

      // Call validate function to validate the coupon record. 
      duplicateCouponErrorMessage = ValidateCouponRecord(couponRecord, null, SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod);
      if (!string.IsNullOrEmpty(duplicateCouponErrorMessage))
      {
        couponRecord.ISValidationFlag = DuplicateValidationFlag;
      }
      //// Calculate  UATP amount 
      //// UATP = Coupon Gross Value/Applicable Local Fare * UATP%
      //couponRecord.UatpAmount = ConvertUtil.Round((couponRecord.CouponGrossValueOrApplicableLocalFare + couponRecord.UatpPercent), Constants.PaxDecimalPlaces);

      //// Calculate  ISC amount 
      //// ISC = Coupon Gross Value/Applicable Local Fare * ISC%
      //couponRecord.IscAmount = ConvertUtil.Round((couponRecord.CouponGrossValueOrApplicableLocalFare + couponRecord.IscPercent),  Constants.PaxDecimalPlaces);

      //// Set SettlementAuthorizationCode empty in case ETicketIndicator is false
      //if (couponRecord.ElectronicTicketIndicator == false)
      //{
      //  couponRecord.SettlementAuthorizationCode = string.empty;
      //}

      //// Coupon Total Amount
      //couponRecord.CouponTotalAmount = ConvertUtil.Round((couponRecord.CouponGrossValueOrApplicableLocalFare + couponRecord.TaxAmount + couponRecord.HandlingFeeAmount + couponRecord.IscAmount + couponRecord.OtherCommissionAmount + couponRecord.UatpAmount + couponRecord.VatAmount), Constants.PaxDecimalPlaces);
      //CMP #672: Validation on Taxes in PAX FIM Billings
      if (couponRecord.SourceCodeId == FimSourceCode14Pax && couponRecord.TaxBreakdown.Count > 0)
      {
          couponRecord.TaxBreakdown = new List<PrimeCouponTax>();
          couponRecord.CouponTotalAmount = couponRecord.CouponTotalAmount - couponRecord.TaxAmount;
          couponRecord.TaxAmount = 0.0;
      }
      
      CouponRecordRepository.Add(couponRecord);
      UnitOfWork.CommitDefault();
      InvoiceRepository.UpdatePrimeInvoiceTotal(couponRecord.InvoiceId, couponRecord.SourceCodeId, couponRecord.LastUpdatedBy);

      // Get expiry period of prime coupon.
      //  DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, couponRecord.Invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // Update it in database.
      // InvoiceRepository.UpdateExpiryDatePeriod(couponRecord.Id, (int)TransactionType.Coupon, expiryPeriod);

      return couponRecord;
    }

    /// <summary>
    /// Updating an existing record.
    /// </summary>
    /// <param name="couponRecord">Coupon Record to update</param>
    /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
    /// <returns>Updated coupon record</returns>
    public PrimeCoupon UpdateCouponRecord(PrimeCoupon couponRecord, out string duplicateCouponErrorMessage, int submissionMethod = 0)
    {
      // Replaced with LoadStrategy call
      var couponRecordInDb = CouponRecordRepository.Single(couponId: couponRecord.Id);
      //var couponRecordInDb = CouponRecordRepository.Single(coupon => coupon.Id == couponRecord.Id);
      couponRecord.ISValidationFlag = couponRecordInDb.ISValidationFlag;
      duplicateCouponErrorMessage = ValidateCouponRecord(couponRecord, couponRecordInDb, SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod, submissionMethod);

      couponRecord.ISValidationFlag = !string.IsNullOrEmpty(duplicateCouponErrorMessage)
                                        ? DuplicateValidationFlag
                                        : string.Empty;
      
      var couponBreakdownRecord = CouponRecordRepository.Update(couponRecord);

      //Changes to update tax breakdown records
      var listToDelete = couponRecordInDb.TaxBreakdown.Where(tax => couponRecord.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0).ToList();

      foreach (var tax in couponRecord.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0))
      {
        CouponTaxRepository.Add(tax);
      }

      foreach (var couponRecordTax in listToDelete)
      {
        CouponTaxRepository.Delete(couponRecordTax);
      }

      //SCP286106 - FW: IAP ticket with missing tax record
      if (couponRecord.TaxBreakdown.Count > 0)
      {
        foreach (var couponRecordTax in couponRecord.TaxBreakdown)
        {
          if (couponRecordInDb.TaxBreakdown.Where(t => t.Id.Equals(couponRecordTax.Id)).Count() == 0)
            CouponTaxRepository.Add(couponRecordTax);
        }
      }

      // Changes to update tax breakdown records
      var listToDeleteVat = couponRecordInDb.VatBreakdown.Where(vat => couponRecord.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in couponRecord.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        CouponRecordVatRepository.Add(vat);
      }

      foreach (var couponRecordVat in listToDeleteVat)
      {
        CouponRecordVatRepository.Delete(couponRecordVat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = couponRecordInDb.Attachments.Where(attachment => couponRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in couponRecord.Attachments
                              where couponRecordInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var couponRecordAttachmentInDb = CouponAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));
      foreach (var recordAttachment in couponRecordAttachmentInDb)
      {
        if (IsDuplicateCouponAttachmentFileName(recordAttachment.OriginalFileName, couponBreakdownRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }

        recordAttachment.ParentId = couponBreakdownRecord.Id;
        CouponAttachmentRepository.Update(recordAttachment);
      }

      foreach (var couponRecordAttachment in listToDeleteAttachment)
      {
        CouponAttachmentRepository.Delete(couponRecordAttachment);
      }

      UnitOfWork.CommitDefault();

      // Update prime invoice total
      InvoiceRepository.UpdatePrimeInvoiceTotal(couponRecord.InvoiceId, couponRecord.SourceCodeId, couponRecord.LastUpdatedBy);

      //   var invoice = InvoiceRepository.Single(id: couponRecord.InvoiceId);

      // Get expiry period of prime coupon.
      //   DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // Update it in database.
      //  InvoiceRepository.UpdateExpiryDatePeriod(couponRecord.Id, (int)TransactionType.Coupon, expiryPeriod);

      return couponBreakdownRecord;
    }

    /// <summary>
    /// Deleting a coupon record 
    /// </summary>
    /// <param name="couponRecordId"></param>
    /// <returns></returns>
    public bool DeleteCouponRecord(string couponRecordId)
    {
      var couponRecordGuid = couponRecordId.ToGuid();
      // Replaced with LoadStrategy call
      //var primeBilling = CouponRecordRepository.Single(couponId: couponRecordGuid);
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      var primeBilling = CouponRecordRepository.Get(i => i.Id == couponRecordGuid).SingleOrDefault();
      
      // Implemented code to delete CoupanTax records if a coupon record is deleted by user.
      foreach (var coupanTax in CouponTaxRepository.Get(cpnTax => cpnTax.ParentId == couponRecordGuid).ToList())
      {
        CouponTaxRepository.Delete(coupanTax);
      }// End foreach


      //var primeBilling = CouponRecordRepository.Single(coupon => coupon.Id == couponRecordGuid);
      if (primeBilling == null) return false;
      CouponRecordRepository.Delete(primeBilling);
      UnitOfWork.CommitDefault();
      // Update prime invoice total
      InvoiceRepository.UpdatePrimeInvoiceTotal(primeBilling.InvoiceId, primeBilling.SourceCodeId, primeBilling.LastUpdatedBy);

      return true;
    }

    /// <summary>
    /// Following method is used to Get SourceCode Vat total details to be displayed on Popup
    /// </summary>
    /// <param name="billingCodeSubVatTotalId">SourceCode Vat breakdown Id</param>
    /// <returns>SourceCode Vat breakdown record</returns>
    public List<CargoBillingCodeSubTotalVat> GetBillingCodeVatTotal(string billingCodeSubVatTotalId)
    {
        // Convert billingCodeSubVatTotalId Id to Guid
        var billingCodeTotalId = billingCodeSubVatTotalId.ToGuid();
        // Query repository to get record
        var sourceCodeVatTotalRecord = CargoBillingCodeSubTotalVatRepository.Get(sct => sct.ParentId == billingCodeTotalId).ToList();
        //var sourceCodeVatTotalRecord = CgoBillingCodeSubTotalVatRepository.GetBillingCodeVatTotals(billingCodeTotalId);
        //rejectedTransactionDetails.Transactions = CargoInvoiceRepository.GetRejectedTransactionDetails(memoId, couponGuid);
        // return Record
        return sourceCodeVatTotalRecord;
    }

    /// <summary>
    /// Gets a list of all billing memo records.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public IList<BillingMemo> GetBillingMemoList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var billingMemoList = BillingMemoRepository.Get(bm => bm.InvoiceId == invoiceGuid);

      var reasonCodes = billingMemoList.Select(rmRecord => rmRecord.ReasonCode.ToUpper());
      var reasonCodesfromDb = ReasonCodeRepository.Get(reasonCode => reasonCodes.Contains(reasonCode.Code.ToUpper())).ToList();

      if (reasonCodesfromDb.Count() > 0)
      {
        foreach (var billingMemoRecord in billingMemoList)
        {
          var record = billingMemoRecord;
          var transactionTypeId = record.ReasonCode == "6A"
                                    ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                    : record.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;

          var reasonCodeObj = reasonCodesfromDb.Single(rCode => rCode.Code == record.ReasonCode && rCode.TransactionTypeId == transactionTypeId);

          billingMemoRecord.ReasonCodeDescription = reasonCodeObj != null ? reasonCodeObj.Description : string.Empty;
        }
      }

      return billingMemoList.ToList();
    }

    /// <summary>
    /// Get the Details of specific Billing Memo Record
    /// </summary>
    /// <param name="billingMemoRecordId"></param>
    /// <returns></returns>
    public BillingMemo GetBillingMemoRecordDetails(string billingMemoRecordId)
    {
      var billingMemoRecordGuid = billingMemoRecordId.ToGuid();
      //var billingMemoRecords = BillingMemoRepository.Single(memo => memo.Id == billingMemoRecordGuid);
      // call replaced by load strategy
      var billingMemoRecords = BillingMemoRepository.Single(billingMemoId: billingMemoRecordGuid);
      return billingMemoRecords;
    }

    // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
    /// <summary>
    /// Add new record in the billing memo list
    /// </summary>
    /// <param name="billingMemo"></param>
    /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
    /// <returns></returns>
    public BillingMemo AddBillingMemoRecord(BillingMemo billingMemo, bool isNullCorrRefNo = false)
    {
      billingMemo.ReasonCode = billingMemo.ReasonCode.ToUpper();
      ValidateBillingMemo(billingMemo, null);
      if (isNullCorrRefNo)
      {
        billingMemo.CorrespondenceRefNumber = -1;
      }

      // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
      if (!billingMemo.ReasonCode.Equals(ReasonCode6A) && !billingMemo.ReasonCode.Equals(ReasonCode6B) && billingMemo.CorrespondenceRefNumber >= 0)
      {
          throw new ISBusinessException(ErrorCodes.CorrRefNumberCannotBeProvidedForNon6Aor6Bbm);
      }

      BillingMemoRepository.Add(billingMemo);
      UnitOfWork.CommitDefault();

      // Updates billing memo invoice total.
      InvoiceRepository.UpdateBMInvoiceTotal(billingMemo.InvoiceId, billingMemo.SourceCodeId, billingMemo.Id, billingMemo.LastUpdatedBy);

      // Get expiry period.
      // DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, billingMemo.Invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // Update it in database.
      //InvoiceRepository.UpdateExpiryDatePeriod(billingMemo.Id, (int)TransactionType.BillingMemo, expiryPeriod);

      return billingMemo;
    }

    /// <summary>
    /// Update existing record in the list
    /// </summary>
    /// <param name="billingMemo"></param>
    /// <param name="isNullCorrRefNo"> To check user input is Null for Corr. Ref. No.</param>
    /// <returns></returns>
    public BillingMemo UpdateBillingMemoRecord(BillingMemo billingMemo, bool isNullCorrRefNo = false)
    {
      //var billingMemoRecordInDb = BillingMemoRepository.Single(bm => bm.Id == billingMemo.Id);
      // Call replaced by load strategy
      var billingMemoRecordInDb = BillingMemoRepository.Single(billingMemoId: billingMemo.Id);

      billingMemo.ReasonCode = billingMemo.ReasonCode.ToUpper();
      ValidateBillingMemo(billingMemo, billingMemoRecordInDb);
      if(isNullCorrRefNo)
      {
        billingMemo.CorrespondenceRefNumber = -1;
      }

      // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
      if (!billingMemo.ReasonCode.Equals(ReasonCode6A) && !billingMemo.ReasonCode.Equals(ReasonCode6B) && billingMemo.CorrespondenceRefNumber >= 0)
      {
          throw new ISBusinessException(ErrorCodes.CorrRefNumberCannotBeProvidedForNon6Aor6Bbm);
      }

      var updatedbillingMemo = BillingMemoRepository.Update(billingMemo);

      //Update vat list along with Billing Memo Record
      var listToDeleteVat = billingMemoRecordInDb.VatBreakdown.Where(vat => billingMemo.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in billingMemo.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        BillingMemoVatRepository.Add(vat);
      }

      foreach (var billingMemoVat in listToDeleteVat)
      {
        BillingMemoVatRepository.Delete(billingMemoVat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = billingMemoRecordInDb.Attachments.Where(attachment => billingMemo.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in billingMemo.Attachments
                              where billingMemoRecordInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var billingMemoAttachmentInDb = BillingMemoAttachmentRepository.Get(billingMemoAttachment => attachmentIdList.Contains(billingMemoAttachment.Id));
      foreach (var recordAttachment in billingMemoAttachmentInDb)
      {
        if (IsDuplicateBillingMemoAttachmentFileName(recordAttachment.OriginalFileName, billingMemo.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }
        recordAttachment.ParentId = billingMemo.Id;
        BillingMemoAttachmentRepository.Update(recordAttachment);
      }

      foreach (var billingMemoAttachment in listToDeleteAttachment)
      {
        BillingMemoAttachmentRepository.Delete(billingMemoAttachment);
      }

      UnitOfWork.CommitDefault();

      // Updates billing memo invoice total.
      InvoiceRepository.UpdateBMInvoiceTotal(billingMemo.InvoiceId, billingMemo.SourceCodeId, billingMemo.Id, billingMemo.LastUpdatedBy);

      //   var billingMemoInvoice = InvoiceRepository.Single(id: billingMemo.InvoiceId);

      // Get expiry period.
      // DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, billingMemoInvoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // Update it in database.
      //  InvoiceRepository.UpdateExpiryDatePeriod(billingMemo.Id, (int)TransactionType.BillingMemo, expiryPeriod);

      return updatedbillingMemo;
    }

    /// <summary>
    /// Delete billing Memo
    /// </summary>
    /// <param name="billingMemoRecordId"></param>
    /// <returns></returns>
    public bool DeleteBillingMemoRecord(string billingMemoRecordId)
    {
      var billingMemoRecordGuid = billingMemoRecordId.ToGuid();
      //var billingMemo = BillingMemoRepository.Single(bm => bm.Id == billingMemoRecordGuid);
      // Call replaced by load strategy
      var billingMemo = BillingMemoRepository.Single(billingMemoId: billingMemoRecordGuid);

      if (billingMemo == null) return false;
      BillingMemoRepository.Delete(billingMemo);
      UnitOfWork.CommitDefault();

      // Update Billing Memo invoice total. 
      InvoiceRepository.UpdateBMInvoiceTotal(billingMemo.InvoiceId, billingMemo.SourceCodeId, billingMemo.Id, billingMemo.LastUpdatedBy, true);

      return true;
    }

    /// <summary>
    /// Get billing memo attachment details 
    /// </summary>
    /// <param name="attachmentId">attachment Id</param>
    /// <returns></returns>
    public BillingMemoAttachment GetBillingMemoAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = BillingMemoAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Insert billing memo attachment record
    /// </summary>
    /// <param name="attach">attachment record</param>
    /// <returns></returns>
    public BillingMemoAttachment AddBillingMemoAttachment(BillingMemoAttachment attach)
    {

      BillingMemoAttachmentRepository.Add(attach);

      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //Commented below code. Just return the object which is passed to function.
     // attach = BillingMemoAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update billing memo attachment record parent id
    /// </summary>
    /// <param name="attachments">list of attachment</param>
    /// <param name="parentId">billing memo Id</param>
    /// <returns></returns>
    public IList<BillingMemoAttachment> UpdateBillingMemoAttachment(IList<Guid> attachments, Guid parentId)
    {
      //var attachmentIds = attachments.Select(att => att.Id);
      var billingMemoAttachmentInDb = BillingMemoAttachmentRepository.Get(billingMemoAttachment => attachments.Contains(billingMemoAttachment.Id));
      foreach (var recordAttachment in billingMemoAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        BillingMemoAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return billingMemoAttachmentInDb.ToList();
    }

    /// <summary>
    /// Check for duplicate file name for billing memo attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="billingMemoId">billing memo id</param>
    /// <returns></returns>
    public bool IsDuplicateBillingMemoAttachmentFileName(string fileName, Guid billingMemoId)
    {
      return BillingMemoAttachmentRepository.GetCount(attachment => attachment.ParentId == billingMemoId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// List of all Billing Memo Coupon Breakdown
    /// </summary>
    /// <param name="billingMemoId"></param>
    /// <returns></returns>
    public IList<BMCoupon> GetBillingMemoCouponList(string billingMemoId)
    {
      var billingMemoGuid = billingMemoId.ToGuid();
      var couponBreakdownList = BillingMemoCouponBreakdownRepository.Get(breakdownRecord => breakdownRecord.BillingMemoId == billingMemoGuid);

      return couponBreakdownList.ToList();
    }

    /// <summary>
    /// Detail of specific Coupon Breakdown record
    /// </summary>
    /// <param name="couponBreakdownRecordId"></param>
    /// <returns></returns>
    public BMCoupon GetBillingMemoCouponDetails(string couponBreakdownRecordId)
    {
      var billingMemoCouponGuid = couponBreakdownRecordId.ToGuid();
      var couponBreakdownRecords = BillingMemoCouponBreakdownRepository.Single(billingMemoCouponGuid);

      return couponBreakdownRecords;
    }

    /// <summary>
    /// Addition of new Billing Memo Breakdown Record
    /// </summary>
    /// <param name="billingMemoCouponBreakdownRecord">Details of the coupon breakdown record</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Add coupon breakdown record</returns>
    public BMCoupon AddBillingMemoCouponDetails(BMCoupon billingMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage)
    {
      duplicateErrorMessage = ValidateBillingMemoCoupon(billingMemoCouponBreakdownRecord, null, invoiceId);

      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        billingMemoCouponBreakdownRecord.ISValidationFlag = DuplicateValidationFlag;
      }

      var billingMemoRecord = BillingMemoRecordRepository.Single(billingMemoCouponBreakdownRecord.BillingMemoId);
      billingMemoCouponBreakdownRecord.SerialNo = GetBillingMemoSerialNo(billingMemoRecord);
      BillingMemoCouponBreakdownRepository.Add(billingMemoCouponBreakdownRecord);
      UnitOfWork.CommitDefault();
      // Update Billing Memo coupon breakdown invoice total.
      UpdateBillingMemoInvoiceTotal(billingMemoCouponBreakdownRecord);

      return billingMemoCouponBreakdownRecord;
    }

    /// <summary>
    /// Updates the billing memo invoice total.
    /// </summary>
    /// <param name="billingMemoCouponBreakdownRecord">The billing memo coupon breakdown record.</param>
    private void UpdateBillingMemoInvoiceTotal(BMCoupon billingMemoCouponBreakdownRecord, bool isCouponDelete = false)
    {
      //var billingMemoRecord = BillingMemoRepository.Single(bmRecord => bmRecord.Id == billingMemoCouponBreakdownRecord.BillingMemoId);
      // Call replaced by Load strategy
      var billingMemoRecord = BillingMemoRepository.Single(billingMemoCouponBreakdownRecord.BillingMemoId);
      InvoiceRepository.UpdateBMInvoiceTotal(billingMemoRecord.InvoiceId, billingMemoRecord.SourceCodeId, billingMemoRecord.Id, billingMemoRecord.LastUpdatedBy, isCouponDelete);
    }

    /// <summary>
    /// Update Billing Memo Coupon Details
    /// </summary>
    /// <param name="billingMemoCouponBreakdownRecord">The billing memo coupon breakdown record.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateCouponErrorMessage">The duplicate coupon error message.</param>
    /// <returns></returns>
    public BMCoupon UpdateBillingMemoCouponDetails(BMCoupon billingMemoCouponBreakdownRecord, string invoiceId, out string duplicateCouponErrorMessage)
    {
      var billingMemoCouponWithDetails = BillingMemoCouponBreakdownRepository.Single(billingMemoCouponBreakdownRecord.Id);
      billingMemoCouponBreakdownRecord.ISValidationFlag = billingMemoCouponWithDetails.ISValidationFlag;
      duplicateCouponErrorMessage = ValidateBillingMemoCoupon(billingMemoCouponBreakdownRecord, billingMemoCouponWithDetails, invoiceId);

      if (!string.IsNullOrEmpty(duplicateCouponErrorMessage))
      {
        billingMemoCouponBreakdownRecord.ISValidationFlag = DuplicateValidationFlag;
      }

      // Updates
      var updatedbillingMemoCouponBreakdown = BillingMemoCouponBreakdownRepository.Update(billingMemoCouponBreakdownRecord);

      //Update Tax Breakdown along with Coupon Breakdown Record
      var taxListToDelete = billingMemoCouponWithDetails.TaxBreakdown.Where(tax => billingMemoCouponBreakdownRecord.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0).ToList();

      foreach (var tax in billingMemoCouponBreakdownRecord.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0))
      {
        BillingMemoCouponTaxBreakdownRepository.Add(tax);
      }

      foreach (var tax in taxListToDelete)
      {
        BillingMemoCouponTaxBreakdownRepository.Delete(tax);
      }

      //SCP286106 - FW: IAP ticket with missing tax record
      if (billingMemoCouponBreakdownRecord.TaxBreakdown.Count > 0)
      {
        foreach (var couponRecordTax in billingMemoCouponBreakdownRecord.TaxBreakdown)
        {
          if (billingMemoCouponWithDetails.TaxBreakdown.Where(t => t.Id.Equals(couponRecordTax.Id)).Count() == 0)
            BillingMemoCouponTaxBreakdownRepository.Add(couponRecordTax);
        }
      }

      //Update vat Breakdown along with Coupon Breakdown Record
      var vatListToDelete = billingMemoCouponWithDetails.VatBreakdown.Where(vat => billingMemoCouponBreakdownRecord.VatBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0).ToList();

      foreach (var vat in billingMemoCouponBreakdownRecord.VatBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0))
      {
        BillingMemoCouponVatBreakdownRepository.Add(vat);
      }

      foreach (var vat in vatListToDelete)
      {
        BillingMemoCouponVatBreakdownRepository.Delete(vat);
      }

      // Changes to update attachment breakdown records
      var listToDeleteAttachment = billingMemoCouponWithDetails.Attachments.Where(attachment => billingMemoCouponBreakdownRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

      var attachmentIdList = (from attachment in billingMemoCouponBreakdownRecord.Attachments
                              where billingMemoCouponWithDetails.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                              select attachment.Id).ToList();

      var attachmentInDb = BillingMemoCouponAttachmentRepository.Get(bmCouponAttachment => attachmentIdList.Contains(bmCouponAttachment.Id));
      foreach (var recordAttachment in attachmentInDb)
      {
        if (IsDuplicateBillingMemoCouponAttachmentFileName(recordAttachment.OriginalFileName, billingMemoCouponBreakdownRecord.Id))
        {
          throw new ISBusinessException(ErrorCodes.DuplicateFileName);
        }
        recordAttachment.ParentId = billingMemoCouponBreakdownRecord.Id;
        BillingMemoCouponAttachmentRepository.Update(recordAttachment);
      }

      foreach (var bmCouponRecordAttachment in listToDeleteAttachment)
      {
        BillingMemoCouponAttachmentRepository.Delete(bmCouponRecordAttachment);
      }

      UnitOfWork.CommitDefault();
      UpdateBillingMemoInvoiceTotal(billingMemoCouponBreakdownRecord);

      Guid invoiceGuid = invoiceId.ToGuid();
      // var billingMemoInvoice = InvoiceRepository.Single(id: invoiceGuid);

      // Get expiry period.
      //DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, billingMemoInvoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

      // Update it in database.
      // InvoiceRepository.UpdateExpiryDatePeriod(billingMemoCouponBreakdownRecord.BillingMemoId, (int)TransactionType.BillingMemo, expiryPeriod);

      return updatedbillingMemoCouponBreakdown;
    }

    /// <summary>
    /// Delete the Billing Memo Coupon Breakdown record
    /// </summary>
    /// <param name="couponBreakdownRecordId"></param>
    /// <returns></returns>
    public bool DeleteBillingMemoCouponRecord(string couponBreakdownRecordId, out Guid billingMemoId, out Guid invoiceId)
    {
      Guid couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();

      // First fetch the entity from the key.
      var billingMemoCouponBreakdown = BillingMemoCouponBreakdownRepository.Single(couponBreakdownRecordGuid);

      if (billingMemoCouponBreakdown == null)
      {
        billingMemoId = new Guid();
        invoiceId = new Guid();
        return false;
      }

      billingMemoId = billingMemoCouponBreakdown.BillingMemoId;
      invoiceId = billingMemoCouponBreakdown.BillingMemo.InvoiceId;

      // Delete BM Coupon, re-sequence serial numbers of subsequent coupons and update Invoice Total.
      InvoiceRepository.DeleteBillingMemoCoupon(couponBreakdownRecordGuid);

      return true;
    }

    /// <summary>
    /// Get billing memo coupon record attachment details
    /// </summary>
    /// <param name="attachmentId">attachment id</param>
    /// <returns></returns>
    public BMCouponAttachment GetBillingMemoCouponAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = BillingMemoCouponAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Add billing memo coupon attachment record
    /// </summary>
    /// <param name="attach">attachment record</param>
    /// <returns></returns>
    public BMCouponAttachment AddBillingMemoCouponAttachment(BMCouponAttachment attach)
    {

      BillingMemoCouponAttachmentRepository.Add(attach);

      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //Commented below code. Just return the object which is passed to function.
     // attach = BillingMemoCouponAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    /// <summary>
    /// Update parent id of attachment records with given list of GUID
    /// </summary>
    /// <param name="attachments">list of GUID of attachment</param>
    /// <param name="parentId">billing memo coupon id</param>
    /// <returns></returns>
    public IList<BMCouponAttachment> UpdateBillingMemoCouponAttachment(IList<Guid> attachments, Guid parentId)
    {
      var attachmentInDb = BillingMemoCouponAttachmentRepository.Get(bmCouponAttachment => attachments.Contains(bmCouponAttachment.Id));
      foreach (var recordAttachment in attachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        BillingMemoCouponAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return attachmentInDb.ToList();
    }

    /// <summary>
    /// Check for duplicate file name for Billing memo coupon attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="billingMemoCouponId">billing Memo Coupon Id</param>
    /// <returns></returns>
    public bool IsDuplicateBillingMemoCouponAttachmentFileName(string fileName, Guid billingMemoCouponId)
    {
      return BillingMemoCouponAttachmentRepository.GetCount(attachment => attachment.ParentId == billingMemoCouponId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// GetRejectionMemoList
    /// </summary>
    /// <param name="invoiceId">Rejection Memo list for this invoice id.</param>
    /// <returns></returns>
    public IList<RejectionMemo> GetRejectionMemoList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var rejectionMemoList = RejectionMemoRepository.Get(rm => rm.InvoiceId == invoiceGuid);

      var reasonCodes = rejectionMemoList.Select(rmRecord => rmRecord.ReasonCode.ToUpper());
      var reasonCodesfromDb = ReasonCodeRepository.Get(reasonCode => reasonCodes.Contains(reasonCode.Code.ToUpper())).ToList();

      if (reasonCodesfromDb.Count() > 0)
      {
        foreach (var rejectionMemoRecord in rejectionMemoList)
        {
          var record = rejectionMemoRecord;
          var transactionType = GetTransactionType(record.RejectionStage);
          var reasonCodeObj = reasonCodesfromDb.Single(rCode => rCode.Code == record.ReasonCode && rCode.TransactionTypeId == (int)transactionType);

          rejectionMemoRecord.ReasonCodeDescription = reasonCodeObj != null ? reasonCodeObj.Description : string.Empty;
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
      var rejectionMemo = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoRecordGuid);
      //var rejectionMemo = RejectionMemoRepository.Single(rm => rm.Id == rejectionMemoRecordGuid);

      return rejectionMemo;
    }

    /// <summary>
    /// Add RejectionMemo Record.
    /// </summary>
    /// <param name="rejectionMemoRecord">RejectionMemoRecord to be added.</param>
    /// <returns></returns>
    public RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord)
    {
      string errorMessage = string.Empty, warningMessage = string.Empty;
      var rejectionMemo = AddRejectionMemoRecord(rejectionMemoRecord, out errorMessage, out warningMessage);

      return rejectionMemo;
    }

    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //private void UpdateRejectionMemoExpiryDate(RejectionMemo rejectionMemo)
    //{
    //  TransactionType transactionType;
    //  TransactionType currentTransactionType;
    //  switch (rejectionMemo.RejectionStage)
    //  {
    //    case 1:
    //      transactionType = TransactionType.RejectionMemo2;
    //      currentTransactionType = TransactionType.RejectionMemo1;
    //      break;
    //    case 2:
    //      transactionType = TransactionType.RejectionMemo3;
    //      currentTransactionType = TransactionType.RejectionMemo2;
    //      break;
    //    case 3:
    //      transactionType = TransactionType.PaxCorrespondence;
    //      currentTransactionType = TransactionType.RejectionMemo3;
    //      break;
    //    default:
    //      transactionType = TransactionType.RejectionMemo2;
    //      currentTransactionType = TransactionType.RejectionMemo1;
    //      break;
    //  }

    //  PaxInvoice rejectionMemoInvoice = rejectionMemo.Invoice;
    //  if (rejectionMemoInvoice == null)
    //    rejectionMemoInvoice = InvoiceRepository.Single(id: rejectionMemo.InvoiceId);
    //  // Get expiry period of prime coupon.
    //  DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(transactionType, rejectionMemoInvoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

    //  // Update it in database.
    //  InvoiceRepository.UpdateExpiryDatePeriod(rejectionMemo.Id, (int)currentTransactionType, expiryPeriod);
    //}
    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //private DateTime GetRejectionMemoExpiryDate(int rejectionStage, PaxInvoice invoice, BillingPeriod currentBillingPeriod)
    //{
    //  TransactionType transactionType = TransactionType.RejectionMemo2;
    //  string samplingIndicator = Constants.SamplingIndicatorNo;

    //  if (invoice.BillingCode == (int)BillingCode.NonSampling)
    //    switch (rejectionStage)
    //    {
    //      case 1:
    //        transactionType = TransactionType.RejectionMemo2;
    //        break;
    //      case 2:
    //        transactionType = TransactionType.RejectionMemo3;
    //        break;
    //      case 3:
    //        transactionType = TransactionType.PaxCorrespondence;
    //        break;
    //      default:
    //        transactionType = TransactionType.RejectionMemo2;
    //        break;
    //    }
    //  else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
    //  {
    //    transactionType = TransactionType.SamplingFormXF;
    //    samplingIndicator = Constants.SamplingIndicatorYes;
    //  }
    //  else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
    //  {
    //    transactionType = TransactionType.PaxCorrespondenceSampling;
    //    samplingIndicator = Constants.SamplingIndicatorYes;
    //  }

    //  return ReferenceManager.GetExpiryDatePeriodMethod(transactionType, invoice, BillingCategoryType.Pax, samplingIndicator, currentBillingPeriod);
    //}
    /// <summary>
    /// Add RejectionMemo Record. Used to save RM to display error message while adding RM Coupon
    /// </summary>
    /// <param name="rejectionMemoRecord">RejectionMemoRecord to be added.</param>
    /// <param name="linkingErrorMessage">Error message in RM coupon linking</param>
    /// <returns></returns>
    public RejectionMemo AddRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string linkingErrorMessage, out string warningMessage)
    {
      linkingErrorMessage = string.Empty;
      rejectionMemoRecord.ReasonCode = rejectionMemoRecord.ReasonCode.ToUpper();
      warningMessage = ValidateRejectionMemo(rejectionMemoRecord, null);

      // Get rejection flag.
      SetIsRejectionFlag(rejectionMemoRecord);

      
      // Get details of rejection memo record corresponding to the id passed.
      RejectionMemoRepository.Add(rejectionMemoRecord);

      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      try
      {
        UnitOfWork.CommitDefault();
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat("Exception Details: {0}", exception.InnerException.ToString());
        // If table level constraint for Rejection Stage, throws the exception then will throw it as BusinessException.
        if (exception.InnerException.Message.Contains("CK_RM#REJ_STG"))
        {
          throw new ISBusinessException(ErrorCodes.InvalidRejectionStageAttemptedToSave);
        }
        throw;
      }

      if (rejectionMemoRecord.IsLinkingSuccessful.HasValue && rejectionMemoRecord.IsLinkingSuccessful.Value)
      {
        //if (rejectionMemoRecord.IsBreakdownAllowed.HasValue && rejectionMemoRecord.IsBreakdownAllowed.Value)
        linkingErrorMessage = RejectionMemoRecordRepository.InheritRMCouponDetails(rejectionMemoRecord.Id);
      }
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoRecord.Id, rejectionMemoRecord.LastUpdatedBy);
      // Update expiry date of RM for purging.
      // UpdateRejectionMemoExpiryDate(rejectionMemoRecord);

      ////CMP#459 : Validate Memo level amounts
      //var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
      //IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
      //ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord);
      return rejectionMemoRecord;
    }

    /// <summary>
    /// To update rejection memo record
    /// </summary>
    /// <param name="rejectionMemoRecord">Details of the rejection memo record</param>
    /// <returns>Updated rejection memo record</returns>
    public RejectionMemo UpdateRejectionMemoRecord(RejectionMemo rejectionMemoRecord, out string warningMessage)
    {
      //Replaced with LoadStrategy call
      var rejectionMemoRecordInDb = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoRecord.Id);
      //var rejectionMemoRecordInDb = RejectionMemoRepository.Single(rm => rm.Id == rejectionMemoRecord.Id);

      rejectionMemoRecord.ReasonCode = rejectionMemoRecord.ReasonCode.ToUpper();
      warningMessage = ValidateRejectionMemo(rejectionMemoRecord, rejectionMemoRecordInDb);

      // Get rejection flag.
      SetIsRejectionFlag(rejectionMemoRecord);
      // Update the rejection memo.
      var updatedRejectionMemo = RejectionMemoRepository.Update(rejectionMemoRecord);

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
      var listToDeleteAttachment =
        rejectionMemoRecordInDb.Attachments.Where(attachment => rejectionMemoRecord.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

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

      foreach (var rmAttachment in listToDeleteAttachment)
      {
        RejectionMemoAttachmentRepository.Delete(rmAttachment);
      }

      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      try
      {
        UnitOfWork.CommitDefault();
      }
      catch (Exception exception)
      {
        Logger.ErrorFormat("Exception Details: {0}", exception.InnerException.ToString());
        // If table level constraint for Rejection Stage, throws the exception then will throw it as BusinessException.
        if (exception.InnerException.Message.Contains("CK_RM#REJ_STG"))
        {
          throw new ISBusinessException(ErrorCodes.InvalidRejectionStageAttemptedToSave);
        }
        throw;
      }

      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoRecord.Id, rejectionMemoRecord.LastUpdatedBy);
      // UpdateRejectionMemoExpiryDate(rejectionMemoRecord);

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
      var rejectionMemo = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoRecordGuid);
      //var rejectionMemo = RejectionMemoRepository.Single(rm => rm.Id == rejectionMemoRecordGuid);

      if (rejectionMemo == null) return false;
      RejectionMemoRepository.Delete(rejectionMemo);
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
      Guid attachmentGuid = attachmentId.ToGuid();

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
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //Commented below code. Just return the object which is passed to function.
     // attach = RejectionMemoAttachmentRepository.Single(a => a.Id == attach.Id);
        return attach;
    }

    /// <summary>
    /// Update parent id of rejection memo attachment record for given Guids
    /// </summary>
    /// <param name="attachments">list of Guid of rejection memo attachment record</param>
    /// <param name="parentId">rejection memo id</param>
    /// <returns></returns>
    public IList<RejectionMemoAttachment> UpdateRejectionMemoAttachment(IList<Guid> attachments, Guid parentId)
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
      var rmCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(couponBreakdownRecordGuid);
      //var rmCouponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(rmCoupon => rmCoupon.Id == couponBreakdownRecordGuid);

      return rmCouponBreakdownRecord;
    }

    /// <summary>
    /// Add RejectionMemo CouponDetails.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">RejectionMemoCouponBreakdownRecord to be added.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <param name="isFromBillingHistory">Flag indicating whether method is called from Billing History.</param>
    /// <returns>Add coupon breakdown record</returns>
    public RMCoupon AddRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage, bool isFromBillingHistory = false)
    {
        //SCP85039:Performance improvement of is-web during RMCouponCreate and return 
        //var rejectionMemoRecord = RejectionMemoRecordRepository.Single(rejectionMemoId: rejectionMemoCouponBreakdownRecord.RejectionMemoId);
        var rejectionMemoRecord = RejectionMemoRecordRepository.Get(i=>i.Id==rejectionMemoCouponBreakdownRecord.RejectionMemoId).SingleOrDefault();

      // Validate rejection memo coupon breakdown records.
      duplicateErrorMessage = ValidateRejectionMemoCoupon(rejectionMemoCouponBreakdownRecord, null, rejectionMemoRecord, invoiceId);
      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
         rejectionMemoCouponBreakdownRecord.ISValidationFlag = DuplicateValidationFlag;
      }

      if (!isFromBillingHistory)
      {
          //SCP289215 - UA Ticket 618 729 0229461 cpn 1, Validate CPN on create and Edit
          //CMP#459 : Validate Amounts.
          var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
          IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
          ValidateAmountsInRMonCouponLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, rejectionMemoCouponBreakdownRecord, isFromBillingHistory);
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


      rejectionMemoCouponBreakdownRecord.SerialNo = GetSerialNo(rejectionMemoRecord);

      RejectionMemoCouponBreakdownRepository.Add(rejectionMemoCouponBreakdownRecord);
      UnitOfWork.CommitDefault();

      // Update rejection memo invoice total. 
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoCouponBreakdownRecord.RejectionMemoId, rejectionMemoCouponBreakdownRecord.LastUpdatedBy);
      // Below method is called when RM is created from Billing History. Hence, no need to call it again when coupons are inherited.
      // if (isFromBillingHistory == false)
      // UpdateRejectionMemoExpiryDate(rejectionMemoRecord);

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
    /// Gets the serial no.
    /// </summary>
    /// <param name="billingMemoRecord">The billing memo record.</param>
    /// <returns></returns>
    private int GetBillingMemoSerialNo(BillingMemo billingMemoRecord)
    {
      var serialNo = 1;
      var billingMemoCoupon = BillingMemoCouponBreakdownRepository.Get(bmCoupon => bmCoupon.BillingMemoId == billingMemoRecord.Id).OrderByDescending(bmCoupon => bmCoupon.SerialNo).FirstOrDefault();
      if (billingMemoCoupon != null)
      {
        serialNo = billingMemoCoupon.SerialNo + 1;
      }
      return serialNo;
    }

    /// <summary>
    /// Updates the rejection memo invoice total.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
    /// <param name="isCouponDelete"></param>
    private void UpdateRejectionMemoInvoiceTotal(RMCoupon rejectionMemoCouponBreakdownRecord, bool isCouponDelete)
    {
      //Replaced with LoadStrategy call
      var rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoCouponBreakdownRecord.RejectionMemoId);
      //var rejectionMemoRecord = RejectionMemoRepository.Single(rmRecord => rmRecord.Id == rejectionMemoCouponBreakdownRecord.RejectionMemoId);
      // Update rejection memo invoice total.
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoCouponBreakdownRecord.RejectionMemoId, rejectionMemoCouponBreakdownRecord.LastUpdatedBy, isCouponDelete);
    }

    /// <summary>
    /// Update RejectionMemo Coupon Details.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">RejectionMemoCouponBreakdownRecord to be updated.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="duplicateErrorMessage">The duplicate error message.</param>
    /// <returns>Updated coupon breakdown record</returns>
    public RMCoupon UpdateRejectionMemoCouponDetails(RMCoupon rejectionMemoCouponBreakdownRecord, string invoiceId, out string duplicateErrorMessage)
    {
      //LoadStrategy call
      //SCP85039:Performance improvement of is-web during RMCreate and return
      //Reverted the code
      var rejectionMemoCouponBreakdownRecordWithDetail = RejectionMemoCouponBreakdownRepository.Single(rejectionMemoCouponBreakdownRecord.Id);
      //var rejectionMemoCouponBreakdownRecordWithDetail = RejectionMemoCouponBreakdownRepository.Get(i => i.Id == rejectionMemoCouponBreakdownRecord.Id).SingleOrDefault();
      //var rejectionMemoCouponBreakdownRecordWithDetail = RejectionMemoCouponBreakdownRepository.Single(rmCoupon => rmCoupon.Id == rejectionMemoCouponBreakdownRecord.Id);

      //var rejectionMemoRecord = RejectionMemoRecordRepository.Single(rmRecord => rmRecord.Id == rejectionMemoCouponBreakdownRecord.RejectionMemoId);
      //SCP85039:Performance improvement of is-web during RMCreate and return
      //Reverted the code
      var rejectionMemoRecord = RejectionMemoRecordRepository.Single(rejectionMemoId: rejectionMemoCouponBreakdownRecord.RejectionMemoId);
      //var rejectionMemoRecord = RejectionMemoRecordRepository.Get(i=>i.Id==rejectionMemoCouponBreakdownRecord.RejectionMemoId).SingleOrDefault();
      rejectionMemoCouponBreakdownRecord.ISValidationFlag =
        rejectionMemoCouponBreakdownRecordWithDetail.ISValidationFlag;
      // Validates rejection memo coupons 
      duplicateErrorMessage = ValidateRejectionMemoCoupon(rejectionMemoCouponBreakdownRecord, rejectionMemoCouponBreakdownRecordWithDetail, rejectionMemoRecord, invoiceId);

      if (!string.IsNullOrEmpty(duplicateErrorMessage))
      {
        rejectionMemoCouponBreakdownRecord.ISValidationFlag = DuplicateValidationFlag;
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

      //SCP286106 - FW: IAP ticket with missing tax record
      if (rejectionMemoCouponBreakdownRecord.TaxBreakdown.Count > 0)
      {
        foreach (var tax in rejectionMemoCouponBreakdownRecord.TaxBreakdown)
        {
          if (rejectionMemoCouponBreakdownRecordWithDetail.TaxBreakdown.Where(t => t.Id.Equals(tax.Id)).Count() == 0)
            RejectionMemoCouponTaxBreakdownRepository.Add(tax);
        }
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

      // Changes to update attachment breakdown records.
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
      foreach (var rmCouponRecordAttachment in listToDeleteAttachment)
      {
        RejectionMemoCouponAttachmentRepository.Delete(rmCouponRecordAttachment);
      }

      UnitOfWork.CommitDefault();

      // Update rejection memo invoice total 
      InvoiceRepository.UpdateRMInvoiceTotal(rejectionMemoRecord.InvoiceId, rejectionMemoRecord.SourceCodeId, rejectionMemoCouponBreakdownRecord.RejectionMemoId, rejectionMemoCouponBreakdownRecord.LastUpdatedBy);
      //UpdateRejectionMemoExpiryDate(rejectionMemoRecord);

      return updatedrejectionMemoCouponBreakdownRecord;
    }

    /// <summary>
    /// Delete RejectionMemo Coupon Record.
    /// </summary>
    /// <param name="couponBreakdownRecordId">couponBreakdownRecordId to be deleted.</param>
    /// <param name="rejectionMemoId"></param>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public bool DeleteRejectionMemoCouponRecord(string couponBreakdownRecordId, out Guid rejectionMemoId, out Guid invoiceId)
    {
      var couponBreakdownRecordGuid = couponBreakdownRecordId.ToGuid();
      //LoadStrategy call
      var couponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(couponBreakdownRecordGuid);
        //Reverted the code
      //var couponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Get(i=>i.Id==couponBreakdownRecordGuid).SingleOrDefault();
      //var couponBreakdownRecord = RejectionMemoCouponBreakdownRepository.Single(breakdownRecord => breakdownRecord.Id == couponBreakdownRecordGuid);
      if (couponBreakdownRecord == null)
      {
        rejectionMemoId = new Guid();
        invoiceId = new Guid();
        return false;
      }

      rejectionMemoId = couponBreakdownRecord.RejectionMemoId;
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Commented below line as it is not required
      //RejectionMemoRepository.Single(couponBreakdownRecord.RejectionMemoId);
      invoiceId = couponBreakdownRecord.RejectionMemoRecord.InvoiceId;
   
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
      Guid attachmentGuid = attachmentId.ToGuid();

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
      //Commented below code. Just return the object which is passed to function.
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
    /// Function to retrieve Attachment details
    /// </summary>
    /// <param name="attachmentId"></param>
    /// <returns></returns>
    public PrimeCouponAttachment GetCouponLevelAttachmentDetails(string attachmentId)
    {
      Guid attachmentGuid = attachmentId.ToGuid();

      var attachmentRecord = CouponAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Function to Add Attachment at coupon level
    /// </summary>
    /// <param name="attach"></param>
    /// <returns></returns>
    public PrimeCouponAttachment AddCouponLevelAttachment(PrimeCouponAttachment attach)
    {
      try
      {
        CouponAttachmentRepository.Add(attach);

        UnitOfWork.CommitDefault();
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
        //Commented below code. Just return the object which is passed to function.
      //  attach = CouponAttachmentRepository.Single(a => a.Id == attach.Id);
        return attach;
      }
      catch (Exception ex)
      {
        Logger.Error("Add Coupon Level Attachment Exception", ex);
        throw ex;
      }
    }

    /// <summary>
    /// Update attachment parent id
    /// </summary>
    /// <param name="attachments">Attachment list</param>
    /// <param name="parentId">Coupon record Id</param>
    /// <returns></returns>
    public IList<PrimeCouponAttachment> UpdateCouponRecordAttachment(IList<Guid> attachments, Guid parentId)
    {
      //var attachmentIds = attachments.Select(att => att.Id);
      var couponRecordAttachmentInDb = CouponAttachmentRepository.Get(couponAttachment => attachments.Contains(couponAttachment.Id));
      foreach (var recordAttachment in couponRecordAttachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        CouponAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return couponRecordAttachmentInDb.ToList();
    }

    /// <summary>
    /// Check whether coupon record attachment file name is duplicate
    /// </summary>
    /// <param name="fileName">File Name</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    public bool IsDuplicateCouponAttachmentFileName(string fileName, Guid couponId)
    {
      try
      {
        return CouponAttachmentRepository.GetCount(attachment => attachment.ParentId == couponId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
      }
      catch (Exception ex)
      {
        Logger.Error("Is Duplicate Coupon Exception", ex);
        throw ex;
      }
    }

    /// <summary>
    /// Function to compare Tax and VAT amount with their respective breakdown values
    /// </summary>
    /// <param name="couponRecord"></param>
    /// <returns>true or false</returns>
    private static bool BreakdownValueCompare(PrimeCoupon couponRecord)
    {
      // Get the Coupon Tax breakdown amount against coupon record
      double couponTaxBreakdownTotal = couponRecord.TaxBreakdown.Aggregate(0.0, (current, tax) => current + Convert.ToDouble(tax.Amount));
      double couponVatBreakdownTotal = couponRecord.VatBreakdown.Aggregate(0.0, (current, vat) => current + Convert.ToDouble(vat.VatBaseAmount));

      // Check Coupon Tax Breakdown Amount to match Coupon Tax Amount
      // If Yes, then set  flag to true  
      // else set  flag = false;
      return Convert.ToDouble(couponTaxBreakdownTotal) == couponRecord.TaxAmount && Convert.ToDouble(couponVatBreakdownTotal) == couponRecord.VatAmount;
    }

    /// <summary>
    /// Validate the Invoice Header corresponding to the invoice details provided
    /// </summary>
    /// <param name="invoiceHeader"></param>
    /// <param name="invoiceHeaderInDb"></param>
    /// <returns></returns>
    protected override bool ValidateInvoiceHeader(PaxInvoice invoiceHeader, PaxInvoice invoiceHeaderInDb)
    {
      base.ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb);
      if (invoiceHeader.InvoiceType != InvoiceType.Invoice)
      {
        throw new ISBusinessException(ErrorCodes.UnexpectedInvoiceType);
      }

      // Validation for Billing code, billing code should be 0 for non-sampling invoice
      if (invoiceHeader.BillingCode != 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBillingCode);
      }

      return true;
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
                                                "NonSamplingInvoiceManager.ValidateInvoice",
                                                this.ToString(),
                                                BillingCategorys.Passenger.ToString(),
                                                "Step 3 of 12: Id: " + invoiceId + " NonSamplingInvoiceManager.ValidateInvoice Start",
                                                sessionUserId,
                                                logRefId);

        ReferenceManager.LogDebugData(log);

      var webValidationErrors = new List<WebValidationError>();
      isFutureSubmission = false;
      // Call base class validate.
      var invoice = base.ValidateInvoice(invoiceId, out isFutureSubmission, sessionUserId, logRefId);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                         "NonSamplingInvoiceManager.ValidateInvoice",
                                         this.ToString(),
                                         BillingCategorys.Passenger.ToString(),
                                         "Step 8 of 12: Id: " + invoiceId + " after base.ValidateInvoice()",
                                         sessionUserId,
                                         logRefId);
      ReferenceManager.LogDebugData(log);

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);

      webValidationErrors.AddRange(invoice.ValidationErrors);
      // At least one transaction/line item should be present.
      var couponRecordCount = CouponRecordRepository.GetCount(couponRecord => couponRecord.InvoiceId == invoice.Id);
      var rejectionMemoRecordCount = RejectionMemoRecordRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoice.Id);
      var billingMemoRecordCount = BillingMemoRecordRepository.GetCount(bmRecord => bmRecord.InvoiceId == invoice.Id);

      if (couponRecordCount <= 0 && rejectionMemoRecordCount <= 0 && billingMemoRecordCount <= 0)
      {
        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.TransactionLineItemNotAvailable));
        //throw new ISBusinessException(ErrorCodes.TransactionLineItemNotAvailable);
      }

      // Validate acceptable amount difference for given reason code.
      if (rejectionMemoRecordCount > 0)
      {
        var amountDifference = RejectionMemoRecordRepository.ValidateRejectionMemoAcceptableAmountDifference(invoiceId.ToGuid(), invoice.BillingCode);
        if (!string.IsNullOrEmpty(amountDifference))
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.InvalidAcceptableAmountDifferenceValidate));
        }
        var rejectionMemoNegativeAmtCount = RejectionMemoRecordRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoice.Id && rmRecord.TotalNetRejectAmount < 0);
        if (rejectionMemoNegativeAmtCount > 0)
        {
          webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.ErrorNegativeRMNetAmount));
        }
          //CMP#459: Validate Rejection memo.
        var outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
        var rejectionMemoRecords = RejectionMemoRecordRepository.Get(rmRecord => rmRecord.InvoiceId == invoice.Id ).ToList();
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
            if(rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
            {
                var transctionType = TransactionType.RejectionMemo3;
                if(invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                {
                    transctionType = TransactionType.SamplingFormXF;
                }
                ValidatePaxStageThreeRmForTimeLimit(transctionType, invoice.SettlementMethodId, rejectionMemoRecord, invoice, isIsWeb:true,isManageInvoice: true,exceptionDetailsList:rmexceptionDetailsList);
                foreach (var isValidationExceptionDetail in rmexceptionDetailsList)
                {
                    var webValErr = new WebValidationError { ErrorCode = isValidationExceptionDetail.ExceptionCode, ErrorDescription = isValidationExceptionDetail.ErrorDescription, InvoiceId = invoiceId.ToGuid() };
                    webValidationErrors.Add(webValErr);
                }
            }
        }

        #region CMP-674-Validation of Coupon and AWB Breakdowns in Rejections

        /* CMP#674 validations will be applicable only to the following transactions - PAX Non-Sampling Stage 2 RMs, PAX Non-Sampling Stage 3 RMs
             and PAX Sampling Form X/Fs (Stage 3 RMs) */
          var stage2OrSatge3RMCount =
              RejectionMemoRecordRepository.GetCount(
                  rmRecord =>
                  rmRecord.InvoiceId == invoice.Id && (rmRecord.RejectionStage == 2 || rmRecord.RejectionStage == 3));

          if (invoice.BillingCode == (int)BillingCode.NonSampling && stage2OrSatge3RMCount > 0)
          {
              List<InvalidRejectionMemoDetails> invalidRejectionMemos =
                    RejectionMemoRecordRepository.IsYourRejectionCouponDropped(invoice.Id);

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
      
      var errorMessages = InvoiceRepository.ValidateMemo(invoiceId.ToGuid(), invoice.BillingCode);
      if (!string.IsNullOrEmpty(errorMessages))
      {
        errorMessages = errorMessages.Remove(errorMessages.Length - 1, 1);
        var errorMessage = errorMessages.Split(',');
        foreach (var error in errorMessage)
        {
          if (error.ToLower().StartsWith(ReasonCodeErrorMessage.ToLower()))
          {
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), "BPAXNS_10805", error));
          }
          else
          {
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.AmountOutsideLimit, error));
          }
        }
      }

      //SCP219674 : InvalidAmountToBeSettled Validation
      if (billingMemoRecordCount > 0)
      {
        // Linking related - correspondence reference number validations. Required only for reason codes 6A and 6B.
        var billingMemoRecords = BillingMemoRecordRepository.Get(bmRecord => bmRecord.InvoiceId == invoice.Id && (bmRecord.ReasonCode == ReasonCode6A || bmRecord.ReasonCode == ReasonCode6B));
        foreach (var billingMemoRecord in billingMemoRecords)
        {
          try
          {
            ValidateCorrespondenceReference(billingMemoRecord, false, invoice);
          }
          catch (ISBusinessException exception)
          {
            var error = string.Format(" Billing Memo Number: {0} , Batch Sequence Number: {1}", billingMemoRecord.BillingMemoNumber, billingMemoRecord.BatchSequenceNumber);
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), exception.ErrorCode, error));
          }
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
      }
      else
      {
        // Invoice through the validation, change invoice status to Ready for submission. 
        invoice.InvoiceStatus = isFutureSubmission
                                ? InvoiceStatusType.FutureSubmitted
                                : InvoiceStatusType.ReadyForSubmission;
        
        // updating validation status to completed
        invoice.ValidationStatus = InvoiceValidationStatus.Completed;
      }

      if (isFutureSubmission)
      {
        // Get Final Parent Details Clearing House.
        var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
        var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

        // Update clearing house of invoice
        var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);
        invoice.ClearingHouse = clearingHouse;
      }

      invoice.ValidationDate = DateTime.UtcNow;

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                       "NonSamplingInvoiceManager.ValidateInvoice",
                                       this.ToString(),
                                       BillingCategorys.Passenger.ToString(),
                                       "Step 9 of 12: Id: " + invoice.Id +" after invoice.ValidationDate = DateTime.UtcNow",
                                       sessionUserId,
                                       logRefId);
      ReferenceManager.LogDebugData(log);

      // Update the invoice.
      //InvoiceRepository.Update(invoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      InvoiceRepository.SetInvoiceAndValidationStatus(invoice.Id, invoice.InvoiceStatusId, invoice.ValidationStatusId, isFutureSubmission, invoice.ClearingHouse, invoice.InvoiceTotalRecord.NetBillingAmount, (int)BillingCategoryType.Pax, invoice.ExchangeRate);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                         "NonSamplingInvoiceManager.ValidateInvoice",
                                         this.ToString(),
                                         BillingCategorys.Passenger.ToString(),
                                         "Step 10 of 12: Id: " + invoice.Id + " after InvoiceRepository.SetInvoiceAndValidationStatus()",
                                         sessionUserId,
                                         logRefId);
      ReferenceManager.LogDebugData(log);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

      log = ReferenceManager.GetDebugLog(DateTime.Now,
                                       "NonSamplingInvoiceManager.ValidateInvoice",
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
    /// Gets the type of the transaction.
    /// </summary>
    /// <param name="rejectionStage">The rejection stage.</param>
    /// <returns></returns>
    /// <remarks>Returns transaction type as Rejection Memo 1 if invalid rejection stage passed.</remarks> 
    private TransactionType GetTransactionType(int rejectionStage)
    {
      var transactionType = TransactionType.RejectionMemo1;
      switch (rejectionStage)
      {
        case 1:
          transactionType = TransactionType.RejectionMemo1;
          break;
        case 2:
          transactionType = TransactionType.RejectionMemo2;
          break;
        case 3:
          transactionType = TransactionType.RejectionMemo3;
          break;
        default:
          transactionType = TransactionType.RejectionMemo1;
          break;
      }
      return transactionType;
    }

    /// <summary>
    /// Function to validate coupon record.
    /// </summary>
    /// <param name="couponRecord">Coupon record to be updated.</param>
    /// <param name="couponRecordInDb">Existing coupon record in db.</param>
    /// <returns></returns>
    private string ValidateCouponRecord(PrimeCoupon couponRecord, PrimeCoupon couponRecordInDb, bool ignoreValidationOnMigrationPeriod, int submissionMethod = 0)
    {
      var isUpdateOperation = false;
      var duplicateCouponErrorMessage = string.Empty;
      //Below code to check invalid TaxCode enter by user. if user enter invalid taxcode then system will generate error message.
      //SCP#50425 - Tax Code XT
      foreach (var tax in couponRecord.TaxBreakdown)
        {
          if (!string.IsNullOrWhiteSpace(tax.TaxCode))
            tax.TaxCode = tax.TaxCode.Trim();
          if (!ReferenceManager.IsValidTaxCode(tax.TaxCode))
          {
            throw new ISBusinessException(ErrorCodes.InvalidTaxCode,tax.TaxCode);
          }
        }
      //End SCP#50425 - Tax Code XT
      
      //If there is record in db then its a update operation
      if (couponRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (couponRecord.TicketDocOrFimNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTicketDocumnetOrFimNumber);
      }

      // Check  Tax breakdown and VAT breakdown total amount  with Tax and VAT Amount.
      // Get the Coupon Tax breakdown amount against coupon record.
      double couponTaxBreakdownTotal = couponRecord.TaxBreakdown.Aggregate(0.0, (current, tax) => current + Convert.ToDouble(tax.Amount));
      couponTaxBreakdownTotal = ConvertUtil.Round(couponTaxBreakdownTotal, Constants.PaxDecimalPlaces);
      // Get the Coupon Vat breakdown amount against coupon record.
      double couponVatBreakdownTotal = couponRecord.VatBreakdown.Aggregate(0.0, (current, vat) => current + Convert.ToDouble(vat.VatCalculatedAmount));
      couponVatBreakdownTotal = ConvertUtil.Round(couponVatBreakdownTotal, Constants.PaxDecimalPlaces);
      // Check Coupon Tax Breakdown Total Amount to match Coupon Tax Amount.
      // If Not, then throw exception.
      if (!couponRecord.TaxAmount.Equals(Convert.ToDouble(couponTaxBreakdownTotal)))
      {
        throw new ISBusinessException(ErrorCodes.TaxTotalAmountMismatch);
      }

      // Updated for AutoBilling Coupon Record validation -- by Priya R.
      if (submissionMethod != (int)SubmissionMethod.AutoBilling)
      {
        // Check Coupon Vat Breakdown Total Amount to match Coupon Vat Amount.
        // If Not, then throw exception. 
        if (!couponRecord.VatAmount.Equals(Convert.ToDouble(couponVatBreakdownTotal)))
        {
          throw new ISBusinessException(ErrorCodes.VatTotalAmountMismatch);
        }
      }

      if (couponRecord.SourceCodeId == 1 || couponRecord.SourceCodeId == 14)
      {
        if (string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidFromAirportCode);
        }
        else if (!ReferenceManager.IsValidAirportCode(couponRecord.FromAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidFromAirportCode);
        }

        if (string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
        }
        else if (!ReferenceManager.IsValidAirportCode(couponRecord.ToAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
        }
      }
      else
      {
        // Check if passed 'From Airport Code' is a valid airport code 
        // For New coupon Record validation will be done 
        // For Update coupon Record if value FromAirportOfCoupon is updated then only validation will be done
        if (!isUpdateOperation || CompareUtil.IsDirty(couponRecordInDb.FromAirportOfCoupon, couponRecord.FromAirportOfCoupon))
        {
          if (!string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(couponRecord.FromAirportOfCoupon))
          {
            throw new ISBusinessException(ErrorCodes.InvalidFromAirportCode);
          }
        }

        // Check if passed 'To Airport Code' is a valid airport code
        // For New coupon Record validation will be done 
        // For Update coupon Record if value ToAirportOfCoupon is updated then only validation will be done
        if (!isUpdateOperation || CompareUtil.IsDirty(couponRecordInDb.ToAirportOfCoupon, couponRecord.ToAirportOfCoupon))
        {
          if (!string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(couponRecord.ToAirportOfCoupon))
          {
            throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
          }
        }
      }

      // Check if passed 'Currency Adjustment Indicator' is a valid currency code
      // For New coupon Record validation will be done 
      // For Update coupon Record if value CurrencyAdjustmentIndicator is updated then only validation will be done
      if (!isUpdateOperation || CompareUtil.IsDirty(couponRecordInDb.CurrencyAdjustmentIndicator, couponRecord.CurrencyAdjustmentIndicator))
      {
        if (!string.IsNullOrEmpty(couponRecord.CurrencyAdjustmentIndicator) && !ReferenceManager.IsValidCurrencyCode(couponRecord.CurrencyAdjustmentIndicator))
        {
          throw new ISBusinessException(ErrorCodes.InvalidCurrencyAdjustmentInd);
        }
      }

      // Review: If non-zero tax amount is a requirement.
      // Tax amount is 0 or negative but tax breakdown exists, then throw an exception.
      if (couponRecord.TaxAmount <= 0)
      {
        if (couponRecord.TaxBreakdown.Count > 0)
        {
          throw new ISBusinessException(ErrorCodes.InvalidTaxAmount);
        }
      }

      var invoice = InvoiceRepository.Single(id: couponRecord.InvoiceId);

      // CMP-396: Validation of Flight Date in PAX Prime Billing process
      if (couponRecord.FlightDate.HasValue && (((couponRecord.SourceCodeId == 1 || couponRecord.SourceCodeId == 14) && invoice.BillingCode == 0) || (couponRecord.SourceCodeId == 1 && invoice.BillingCode == 3)))
      {
        if ((couponRecord.FlightDate.Value.Year > invoice.BillingYear) ||
            (couponRecord.FlightDate.Value.Year.Equals(invoice.BillingYear) &&
             couponRecord.FlightDate.Value.Month > invoice.BillingMonth))
        {
          throw new ISBusinessException(ErrorCodes.FlightDateGreaterThanBillingMonth);
        }
      }

      if (!isUpdateOperation ||
          (CompareUtil.IsDirty(couponRecordInDb.TicketDocOrFimNumber, couponRecord.TicketDocOrFimNumber) ||
           CompareUtil.IsDirty(couponRecordInDb.TicketOrFimIssuingAirline, couponRecord.TicketOrFimIssuingAirline) ||
           CompareUtil.IsDirty(couponRecordInDb.TicketOrFimCouponNumber, couponRecord.TicketOrFimCouponNumber)))
      {
        //Duplicate billing memo number to be considered for one year
        DateTime billingDate;
        var billingYearToCompare = 0;
        var billingMonthToCompare = 0;

        if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
        {
          var billingDateToCompare = billingDate.AddMonths(-12);
          billingYearToCompare = billingDateToCompare.Year;
          billingMonthToCompare = billingDateToCompare.Month;
        }

        // Validate duplicate coupon - combination Ticket/FIM number, issuing airline, coupon number exist in same invoice.
        // For New coupon Record validation will be done 
        // For Update coupon Record if any value from TicketDocOrFimNumber,TicketOrFimIssuingAirline,TicketOrFimCouponNumber is updated then only validation will be done
        // Validate duplicate coupon - combination Ticket/FIM number, issuing airline, coupon number exist in other invoice created in last 12 months has same billed member for current coupon.
        var duplicateCouponCount = CouponRecordRepository.GetCouponRecordDuplicateCount(couponRecord.TicketOrFimCouponNumber,
                                                                                           couponRecord.TicketDocOrFimNumber,
                                                                                           couponRecord.TicketOrFimIssuingAirline,
                                                                                           invoice.BillingMemberId,
                                                                                           invoice.BilledMemberId,
                                                                                           billingYearToCompare,
                                                                                           billingMonthToCompare,
                                                                                           couponRecord.SourceCodeId, invoice.Id);

        //    if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0))
        if (duplicateCouponCount > 0)
        {
          duplicateCouponErrorMessage = string.Format(Messages.PrimeCouponDuplicateMessage, duplicateCouponCount);
        }
      }
      // Check whether total amount is positive else throw an exception
      if (couponRecord.CouponTotalAmount < 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAmount);
      }

      //SCP# 293488 : Net Amount Error - Function call hooked to confirm net total amount at coupon level.
      ConfirmCouponTotalAmount(couponRecord);

      // Flight date field value validation.
      //if (!IsValidFlightDate(couponRecord.FlightDay, couponRecord.FlightMonth))
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidFlightDate);
      //}

      // Validates whether source code exist in master table.
      if (!isUpdateOperation || CompareUtil.IsDirty(couponRecordInDb.SourceCodeId, couponRecord.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(couponRecord.SourceCodeId, (int)TransactionType.Coupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }

      // From Airport and To Airport should not be same.
      if (!string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon) && !string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon) &&
          couponRecord.FromAirportOfCoupon.Trim().Equals(couponRecord.ToAirportOfCoupon.Trim()))
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportCombination);
      }

      var sourceCodeDetails = SourceCodeRepository.Get(sourceCode => sourceCode.TransactionTypeId == (int)TransactionType.Coupon && sourceCode.SourceCodeIdentifier == couponRecord.SourceCodeId).Select(scd => new { scd.IsFFIndicator }).ToList();

      if (sourceCodeDetails.Count > 0)
      {
        if (!sourceCodeDetails[0].IsFFIndicator)
        {
          double expectedIscAmount = ConvertUtil.Round(couponRecord.IscPercent * couponRecord.CouponGrossValueOrApplicableLocalFare / 100, Constants.PaxDecimalPlaces);
          if (couponRecord.IscAmount != expectedIscAmount)
          {
            throw new ISBusinessException(ErrorCodes.IscAmountIsInvalid);
          }

          double expectedUatpAmount = ConvertUtil.Round(couponRecord.UatpPercent * couponRecord.CouponGrossValueOrApplicableLocalFare / 100, Constants.PaxDecimalPlaces);
          if (couponRecord.UatpAmount != expectedUatpAmount)
          {
            throw new ISBusinessException(ErrorCodes.UatpAmountIsInvalid);
          }
        }
      }

      //Coupon Total amount should be in the range of allowed min max range for transaction type coupon
      if (couponRecord.SourceCodeId != 14 && !ReferenceManager.IsValidNetAmount(couponRecord.CouponTotalAmount, TransactionType.Coupon, invoice.ListingCurrencyId, invoice))
      {
        throw new ISBusinessException(ErrorCodes.CouponTotalAmountIsNotInAllowedRange);
      }

      //check for the reference field 1 if source code 23 is given and value should be from RFIC table
      if (couponRecord.SourceCodeId == 23 && !ReferenceManager.IsValidRficCode(couponRecord.ReferenceField1))
      {
        throw new ISBusinessException(ErrorCodes.InvalidReferenceField1ForPbCoupon);
      }

      //check for the reference field 2 if source code 23 is given and value should be from RFISC table
      if (couponRecord.SourceCodeId == 23 && !ReferenceManager.IsValidRfiscCode(couponRecord.ReferenceField2, couponRecord.ReferenceField1))
      {
        throw new ISBusinessException(ErrorCodes.InvalidReferenceField2ForPbCoupon);
      }
      // addde this code for validation of ref field values againt bug 6611
      if (couponRecord.SourceCodeId == 90 && (string.IsNullOrWhiteSpace(couponRecord.ReferenceField1) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField2) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField3) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField4) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField5)))
      {
        throw new ISBusinessException(ErrorCodes.InvalidReferenceFieldsForCoupon);
      }

      // If IgnoreValidationOnMigrationPeriod field value in System parameters is False, AgreementIndicatorSupplied field value is null and OriginalPmi field value is "N", throw business exception
      if (!ignoreValidationOnMigrationPeriod && couponRecord.AgreementIndicatorSupplied == null && couponRecord.OriginalPmi == "N")
      {
        throw new ISBusinessException(ErrorCodes.InvalidAgreementIndicatorAndOriginalPmiCombination);
      }

      if (!ReferenceManager.IsValidNetAmount(couponRecord.CouponTotalAmount, TransactionType.Coupon, invoice.ListingCurrencyId, invoice, applicableMinimumField: ApplicableMinimumField.TotalAmount))
      {
        throw new ISBusinessException(ErrorCodes.CouponTotalAmountIsNotInAllowedRange);
      }

      // TODO: Set Transaction status to Validated.
      return duplicateCouponErrorMessage;
    }

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
      string flightYear = flightMonth.ToString().PadLeft(4, '0').Substring(0, 2) == "00" ? "2000" : flightMonth.ToString().PadLeft(4, '0').Substring(0, 2);
      string flightDate = string.Format("{0}/{1}/{2}", flightYear, flightMonth.ToString().PadLeft(4, '0').Substring(2, 2), flightDay.ToString().PadLeft(2, '0'));
      return DateTime.TryParse(flightDate, out resultDate);
    }

    /// <summary>
    /// Validate Billing memo record.
    /// </summary>
    /// <param name="billingMemo">Billing memo record to be validated.</param>
    /// <param name="billingMemoInDb">The billing memo record in db.</param>
    /// <returns></returns>
    private void ValidateBillingMemo(BillingMemo billingMemo, BillingMemo billingMemoInDb)
    {
      var isUpdateOperation = false;

      if (billingMemoInDb != null)
      {
        isUpdateOperation = true;
      }

      if (billingMemo.RecordSequenceWithinBatch <= 0 || billingMemo.BatchSequenceNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNoAndRecordNo);
      }

      // Check whether Batch and Sequence number combination is valid and check whether Batch number is not repeated between different source codes
      int invalidBatchSequenceNumber = InvoiceRepository.IsValidBatchSequenceNo(billingMemo.InvoiceId, billingMemo.RecordSequenceWithinBatch, billingMemo.BatchSequenceNumber, billingMemo.Id, billingMemo.SourceCodeId);

      // If value != 0, either Batch and Sequence number combination is invalid or Batch number is repeated between different source codes  
      if (invalidBatchSequenceNumber != 0)
      {
        // If value == 1, Batch number is repeated between different source codes, else if value == 2, Batch and Sequence number combination is invalid  
        if (invalidBatchSequenceNumber == 1)
          throw new ISBusinessException(ErrorCodes.InvalidBatchNo);
        else
          throw new ISBusinessException(ErrorCodes.InvalidBatchSequenceNo);
      }

      //SCP237417 - FW: Validation 
      //Net Amount must be sum of  TotalGrossAmount,TotalVatAmount,TotalIscAmount,TotalUatpAmount,TaxAmount,TotalOtherCommissionAmount,TotalHandlingFee
      // SCP275663: SIS Billing memo Coupon error [Added roudning to 2 decimals while comparison]
      var isNetAmountValueMisMatched = ConvertUtil.Round(billingMemo.NetAmountBilled,Constants.PaxDecimalPlaces) !=
                                     ConvertUtil.Round((billingMemo.TotalGrossAmountBilled + billingMemo.TotalVatAmountBilled +
                                      billingMemo.TotalIscAmountBilled + billingMemo.TotalUatpAmountBilled +
                                      billingMemo.TaxAmountBilled + billingMemo.TotalOtherCommissionAmount +
                                      Convert.ToDecimal(billingMemo.TotalHandlingFeeBilled)), Constants.PaxDecimalPlaces);


      // Review: Check whether this validation is required.
      // Check whether net amount is positive for billing memo, else throw an exception. 
      if (billingMemo.NetAmountBilled < 0 || isNetAmountValueMisMatched)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAmount);
      }

      // Review: Check whether this validation makes sense.
      // If no coupon breakdown exists and if Total VAT Amount Billed populated with a non-zero value, then BM-CM VAT Breakdown record needs to be present.
      var couponBreakdownCount = BillingMemoCouponBreakdownRepository.GetCount(bmRecord => bmRecord.BillingMemoId == billingMemo.Id);
      if (couponBreakdownCount == 0 && billingMemo.TotalVatAmountBilled > 0)
      {
        if (billingMemo.VatBreakdown.Count <= 0)
        {
          throw new ISBusinessException(ErrorCodes.BillingMemoVatBreakdownRecordNotFound);
        }
      }

      var billingMemoInvoice = InvoiceRepository.Single(id: billingMemo.InvoiceId);

      // Your billing period can not be greater than or equal to the Billing Memo Invoice billing period.
      if (!((billingMemoInvoice.BillingYear > billingMemo.YourInvoiceBillingYear) ||
        ((billingMemoInvoice.BillingYear == billingMemo.YourInvoiceBillingYear) && (billingMemoInvoice.BillingMonth > billingMemo.YourInvoiceBillingMonth)) ||
          ((billingMemoInvoice.BillingYear == billingMemo.YourInvoiceBillingYear) && (billingMemoInvoice.BillingMonth == billingMemo.YourInvoiceBillingMonth) && (billingMemoInvoice.BillingPeriod > billingMemo.YourInvoiceBillingPeriod))))
        throw new ISBusinessException(ErrorCodes.InvalidYourBillingPeriod);

      // The ‘Correspondence Ref. No.’ has NOT been referred to in a previously successfully validated BM. 
      CheckDuplicateBillingMemoForCorr(billingMemo, billingMemoInvoice, isUpdateOperation);

      // Linking related - correspondence reference number validations. Required only for reason codes 6A and 6B.
      if (billingMemo.ReasonCode == ReasonCode6A || billingMemo.ReasonCode == ReasonCode6B)
      {
        ValidateCorrespondenceReference(billingMemo, isUpdateOperation, billingMemoInvoice);
      }

      // Validates whether source code exist in master table))
      if (!isUpdateOperation || CompareUtil.IsDirty(billingMemoInDb.SourceCodeId, billingMemo.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(billingMemo.SourceCodeId, (int)TransactionType.BillingMemo))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }

      // Validates whether reason code exist in master table
      // TODO: Reason Code validation need to be check for 6A, 6B. 
      if (!isUpdateOperation || CompareUtil.IsDirty(billingMemoInDb.ReasonCode, billingMemo.ReasonCode))
      {
        var transactionTypeId = billingMemo.ReasonCode == "6A"
                                  ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                  : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;

        if (!ReferenceManager.IsValidReasonCode(billingMemo.ReasonCode, transactionTypeId))
        {
          throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
        }
      }

      // Validates Duplicate Billing Memo.
      if (IsDuplicateBillingMemo(billingMemoInDb, billingMemo, isUpdateOperation, billingMemoInvoice))
      {
        throw new ISBusinessException(ErrorCodes.DuplicateBillingMemoFound);
      }
    }

      

      /// <summary>
    /// Checks if the specified correspondence number exists in the database and is of a valid correspondence.
    /// </summary>
    /// <param name="billingMemo">Billing Memo whose correspondence reference number is to be validated.</param>
    /// <param name="isUpdateOperation">Flag indicating whether the operation is update kind of operation.</param>
    /// <param name="billingMemoInvoice">The billing memo invoice.</param>
    private void ValidateCorrespondenceReference(BillingMemo billingMemo, bool isUpdateOperation, PaxInvoice billingMemoInvoice)
      {
          //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
          //Desc: No further action (Reply/close/init Duplicate BM) is possible as a Billing Memo refers to correspondence.
          //    In order to consider the latest stage corr. check on the substatus was removed. Prior to this change code used to 
          //    get corresp having status "Open" or "Expired" having sub status "Responded".
          var paxCorrespondence =
              PaxCorrespondenceRepository.Get(
                  correspondence =>
                  correspondence.CorrespondenceNumber == billingMemo.CorrespondenceRefNumber &&
                  (correspondence.CorrespondenceStatusId == (int) CorrespondenceStatus.Open ||
                   correspondence.CorrespondenceStatusId == (int) CorrespondenceStatus.Expired ||
                   correspondence.CorrespondenceStatusId == (int) CorrespondenceStatus.Closed)).OrderByDescending(
                       correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

          if (paxCorrespondence == null)
          {
              throw new ISBusinessException(ErrorCodes.BillingMemoReferenceCorrespondenceDoesNotExist);
          }

          if (paxCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
          {
              throw new ISBusinessException(ErrorCodes.CorrespondenceStatusIsClosed);
          }

          var transactionType = TransactionType.BillingMemo;

          if (billingMemo.ReasonCode == ReasonCode6A)
          {
              // The Billed Airline should have provided an Authority to Bill to the Billing Airline.
              if (!paxCorrespondence.AuthorityToBill)
              {
                  throw new ISBusinessException(ErrorCodes.AuthorityToBillNotSetForCorrespondence);
              }

              // The Billing Airline should be the recipient of the last correspondence and
              // the Billed Airline should be the respondent of the last correspondence.
              if (paxCorrespondence.FromMemberId != billingMemoInvoice.BilledMemberId ||
                  paxCorrespondence.ToMemberId != billingMemoInvoice.BillingMemberId
                  || paxCorrespondence.CorrespondenceStatusId != (int) CorrespondenceStatus.Open)
              {
                  throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceMembers);
              }

              transactionType = TransactionType.PasNsBillingMemoDueToAuthorityToBill;
          }
          else if (billingMemo.ReasonCode == ReasonCode6B)
          {
              // The Billing Airline should be the respondent of the last correspondence and
              // the Billed Airline should be the recipient of the last correspondence.
              if (paxCorrespondence.FromMemberId != billingMemoInvoice.BillingMemberId ||
                  paxCorrespondence.ToMemberId != billingMemoInvoice.BilledMemberId)
              {
                  throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceMembers);
              }

              // The correspondence should be expired for the Billed Airline
              if (paxCorrespondence.CorrespondenceStatus != CorrespondenceStatus.Expired)
              {
                  throw new ISBusinessException(ErrorCodes.CorrespondenceStatusNotExpired);
              }

              transactionType = TransactionType.PasNsBillingMemoDueToExpiry;
          }

          if (paxCorrespondence.BMExpiryPeriod.HasValue)
          {
              if (
                  new DateTime(billingMemoInvoice.BillingYear, billingMemoInvoice.BillingMonth,
                               billingMemoInvoice.BillingPeriod) > paxCorrespondence.BMExpiryPeriod)
              {
                  throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
              }
          }
          else
          {
              //CMP#624 : 2.10 - Change#6 : Time Limits
              /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
              if (!ReferenceManager.IsSmiLikeBilateral(billingMemoInvoice.SettlementMethodId, false))
              {
                  if (!ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType,
                                                                        billingMemoInvoice.SettlementMethodId,
                                                                        paxCorrespondence.CorrespondenceDate))
                  {
                      throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
                  }
                  else
                  {
                      if (!ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType,
                                                                             Convert.ToInt32(SMI.Bilateral),
                                                                             paxCorrespondence.CorrespondenceDate))
                      {
                          throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
                      }
                  }
              }
          }

          /* CMP #624: ICH Rewrite-New SMI X 
          *  Description: SMI X checks 
           * billingMemoInvoice is the new invoice having BM in it. 
           * paxCorrespondence is correspondence. It has last stage rejection invoice id
           * Use it to get last stage rejection invoice SMI.
           * Then perform SMI checks
           */
          var lastStageRejectionInvoice = InvoiceRepository.Single(id: paxCorrespondence.InvoiceId);

          if (!ValidateSmiAfterLinking(billingMemoInvoice.SettlementMethodId,
                                       lastStageRejectionInvoice.SettlementMethodId))
          {
              if (billingMemoInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
              {
                  /* New invoice is X but old is not. FRS Reference: 2.14,  New Validation #6 */
                  throw new ISBusinessException(ErrorCodes.PaxNSBmInvoiceLinkingCheckForSmiX);
              }
              else
              {
                  /* Old invoice is X and new is not. FRS Reference: 2.14,  New Validation #7 */
                  throw new ISBusinessException(ErrorCodes.PaxNsStandaloneBmInvLinkCheckForSmiX);
              }
          }

          //if (!(ReferenceManager.IsTransactionInTimeLimitForBm(transactionType, billingMemoInvoice.SettlementMethodId, paxCorrespondence.CorrespondenceDate, billingMemoInvoice)))
          //{
          //  throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
          //}

          //SCP219674 : InvalidAmountToBeSettled Validation

          #region Old Code for "InvalidAmountToBeSettled Validation" To be removed

          /*
      if (!ReferenceManager.IsSmiLikeBilateral(billingMemoInvoice.SettlementMethodId))
      {
        if (paxCorrespondence.CurrencyId != null && billingMemoInvoice.ListingCurrencyId != null)
        {
          // Get Listing to Billing Rate. (Same as Listing to Correspondence Rate when Billing currency and Correspondence currency are same.)
          var exchangeRate = ReferenceManager.GetExchangeRate(billingMemoInvoice.ListingCurrencyId.Value,
                                                                (BillingCurrency)billingMemoInvoice.BillingCurrencyId.Value,
                                                                billingMemoInvoice.BillingYear,
                                                                billingMemoInvoice.BillingMonth);

          var netAmountBilled = exchangeRate > 0
                                  ? billingMemo.NetAmountBilled / Convert.ToDecimal(exchangeRate)
                                  : billingMemo.NetAmountBilled;

          if (billingMemoInvoice.BillingCurrencyId == paxCorrespondence.CurrencyId)
          {
            if (ConvertUtil.Round(paxCorrespondence.AmountToBeSettled, Constants.PaxDecimalPlaces) !=
                ConvertUtil.Round(netAmountBilled, Constants.PaxDecimalPlaces))
            {
              throw new ISBusinessException(ErrorCodes.InvalidAmountToBeSettled);
            }
          }
          else
          {
            // Get Correspondence currency to Billing Currency exchange rate.
            var correspondenceToBillingRate = ReferenceManager.GetExchangeRate(billingMemoInvoice.BillingCurrencyId.Value,
                                                                (BillingCurrency)paxCorrespondence.CurrencyId.Value,
                                                                billingMemoInvoice.BillingYear,
                                                                billingMemoInvoice.BillingMonth);


            // Convert correspondence amount to billing currency using BM's billing month.
            var correspondenceAmtInBillingCurrency = correspondenceToBillingRate > 0
                                                       ? paxCorrespondence.AmountToBeSettled *
                                                         Convert.ToDecimal(correspondenceToBillingRate)
                                                       : paxCorrespondence.AmountToBeSettled;

            if (ConvertUtil.Round(correspondenceAmtInBillingCurrency, Constants.PaxDecimalPlaces) != ConvertUtil.Round(netAmountBilled, Constants.PaxDecimalPlaces))
            {
              // Get Tolerance value for given ivoice for specified period
              var tolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax, billingMemoInvoice.ListingCurrencyId.Value, billingMemoInvoice, Constants.PaxDecimalPlaces);

              // If tolerance != null get difference value and compare, else throw an exception
              if (tolerance != null)
              {
                // Get difference value from amount to be settled and Billing memo billed value
                var differenceValue = ConvertUtil.Round(correspondenceAmtInBillingCurrency, Constants.PaxDecimalPlaces) - ConvertUtil.Round(netAmountBilled, Constants.PaxDecimalPlaces);
                // If above difference value is greater than tolerance summation value raise an exception
                if (Convert.ToDouble(Math.Abs(differenceValue)) > tolerance.SummationTolerance)
                  throw new ISBusinessException(CargoErrorCodes.InvalidAmountToBeSettled);
              }
              else
              {
                throw new ISBusinessException(CargoErrorCodes.InvalidAmountToBeSettled);
              }
            }
          }
        }
      } */

          #endregion

          #region New Code for Validation of CorrespondenceAmounttobeSettled

          if (paxCorrespondence.CurrencyId != null && billingMemoInvoice.ListingCurrencyId != null)
          {
              PaxInvoice corrRmInvoice = null;
              if (billingMemoInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
              {
                corrRmInvoice = lastStageRejectionInvoice;
              }
              decimal netBilledAmount = billingMemo.NetAmountBilled;
              var isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(billingMemoInvoice,
                                                                                     ref netBilledAmount,
                                                                                     paxCorrespondence.CurrencyId.
                                                                                         Value,
                                                                                     paxCorrespondence.
                                                                                         AmountToBeSettled,
                                                                                     corrRmInvoice);
              if (!isValid)
                  throw new ISBusinessException(ErrorCodes.InvalidAmountToBeSettled);
          }

          #endregion
      }

      public IRepository<InvoiceBase> PaxInvoiceRepository
    {
      get;
      set;
    }

    /// <summary>
    /// Determines whether [is billing memo invoice out side time limit] [the specified correspondence ref number].
    /// </summary>
    /// <param name="correspondenceRefNumber">The correspondence ref number.</param>
    /// <param name="authorityToBill">if set to true [authority to bill].</param>
    /// <param name="correspondenceStatusId">The correspondence status id.</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <returns>
    /// true if [is billing memo invoice out side time limit] [the specified correspondence ref number]; otherwise, false.
    /// </returns>
    public bool IsBillingMemoInvoiceOutSideTimeLimit(string correspondenceRefNumber, bool authorityToBill, int correspondenceStatusId, DateTime correspondenceDate)
    {
      var isInsideTimeLimit = false;
      TransactionType transactionType = 0;

      var corrRefNo = Convert.ToInt64(correspondenceRefNumber);
      var paxCorrespondence =
          PaxCorrespondenceRepository.Get(corr => corr.CorrespondenceNumber == corrRefNo && corr.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Responded).OrderByDescending(correspondence => correspondence.CorrespondenceStage).
            FirstOrDefault();

      //var rejectionMemo = RejectionMemoRepository.Single(null, paxCorrespondence.Id);
      var billingMemoInvoice = PaxInvoiceRepository.Single(inv => inv.Id == paxCorrespondence.InvoiceId);
      if (correspondenceStatusId == (int)CorrespondenceStatus.Expired)
        transactionType = TransactionType.PasNsBillingMemoDueToExpiry;

      if (authorityToBill)
        transactionType = TransactionType.PasNsBillingMemoDueToAuthorityToBill;

      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (!ReferenceManager.IsSmiLikeBilateral(billingMemoInvoice.SettlementMethodId, false))
      {
        isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType, billingMemoInvoice.SettlementMethodId, correspondenceDate);
      }
      else
      {
        isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType, Convert.ToInt32(SMI.Bilateral), correspondenceDate);
      }

      return !isInsideTimeLimit;
    }

    /// <summary>
    /// Any Billing memo having the same Billing memo number has been twice billed in the same invoice,
    /// or in a previous invoice to the same airline will be considered as a duplicate.
    /// </summary>
    /// <param name="billingMemoInDb">The billing memo record in db.</param>
    /// <param name="billingMemo">The billing memo record.</param>
    /// <param name="isUpdateOperation">if set to <c>true</c> [is update operation].</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate billing memo] [the specified billing memo record in db]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsDuplicateBillingMemo(BillingMemo billingMemoInDb, BillingMemo billingMemo, bool isUpdateOperation, PaxInvoice invoice)
    {
      if (!isUpdateOperation || (CompareUtil.IsDirty(billingMemoInDb.BillingMemoNumber, billingMemo.BillingMemoNumber)))
      {
        var duplicateBillingMemo = BillingMemoRepository.GetBillingMemoDuplicateCount(billingMemo.BillingMemoNumber, invoice.BilledMemberId, invoice.BillingMemberId, invoice.BillingMonth, invoice.BillingYear, invoice.BillingPeriod);
        if (duplicateBillingMemo > 0)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Validate Billing memo Breakdown record.
    /// </summary>
    /// <param name="billingMemoCouponBreakdownRecord">Billing memo Breakdown record to be validated.</param>
    /// <param name="billingMemoCouponBreakdownRecordInDb">The billing memo coupon breakdown record in db.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    private string ValidateBillingMemoCoupon(BMCoupon billingMemoCouponBreakdownRecord, BMCoupon billingMemoCouponBreakdownRecordInDb, string invoiceId)
    {

      var isUpdateOperation = false;
      var duplicateErrorMessage = string.Empty;
      //Below code to check invalid TaxCode enter by user. if user enter invalid taxcode then system will generate error message.
      //SCP#50425 - Tax Code XT
      foreach (var tax in billingMemoCouponBreakdownRecord.TaxBreakdown)
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
      if (billingMemoCouponBreakdownRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (billingMemoCouponBreakdownRecord.TicketDocOrFimNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTicketDocumnetOrFimNumber);
      }

      // Tax Amount Billed is non-zero then Tax Breakdown record needs to be provided
      if (billingMemoCouponBreakdownRecord.TaxAmount > 0 && billingMemoCouponBreakdownRecord.TaxBreakdown.Count <= 0)
      {
        throw new ISBusinessException(ErrorCodes.BillingMemoCouponTaxBreakdownNotFound);
      }

      // Vat Amount Billed is non-zero then Tax Breakdown record needs to be provided
      if (billingMemoCouponBreakdownRecord.VatAmount > 0 && billingMemoCouponBreakdownRecord.VatBreakdown.Count <= 0)
      {
        throw new ISBusinessException(ErrorCodes.BillingMemoCouponVatBreakdownNotFound);
      }

      // get the parent Billing Memo to get the source code.
      var billingMemo = BillingMemoRepository.Single(billingMemoCouponBreakdownRecord.BillingMemoId);

      var transactionTypeId = billingMemo.ReasonCode == "6A"
                                   ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                   : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;

      var sourceCodeDetails = SourceCodeRepository.Get(sourceCode => sourceCode.TransactionTypeId == transactionTypeId && sourceCode.SourceCodeIdentifier == billingMemo.SourceCodeId).Select(scd => new { scd.IsFFIndicator }).ToList();

      if (sourceCodeDetails.Count > 0)
      {
        if (!sourceCodeDetails[0].IsFFIndicator)
        {
          decimal expectedIscAmount = ConvertUtil.Round(Convert.ToDecimal(billingMemoCouponBreakdownRecord.IscPercent) * billingMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
          if (Convert.ToDecimal(billingMemoCouponBreakdownRecord.IscAmountBilled) != expectedIscAmount)
          {
            throw new ISBusinessException(ErrorCodes.IscAmountIsInvalid);
          }

          decimal expectedUatpAmount = ConvertUtil.Round(Convert.ToDecimal(billingMemoCouponBreakdownRecord.UatpPercent) * billingMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
          if (Convert.ToDecimal(billingMemoCouponBreakdownRecord.UatpAmountBilled) != expectedUatpAmount)
          {
            throw new ISBusinessException(ErrorCodes.UatpAmountIsInvalid);
          }
        }
      }

      //SCP237417 - FW: Validation 
      //Net Amount must be sum of  TotalGrossAmount,TotalVatAmount,TotalIscAmount,TotalUatpAmount,TaxAmount,TotalOtherCommissionAmount,TotalHandlingFee
      // SCP275663: SIS Billing memo Coupon error [Added roudning to 2 decimals while comparison]
      var isNetAmountValueMisMatched = ConvertUtil.Round(billingMemoCouponBreakdownRecord.NetAmountBilled,Constants.PaxDecimalPlaces) !=
                                       ConvertUtil.Round((Convert.ToDouble(billingMemoCouponBreakdownRecord.GrossAmountBilled) + billingMemoCouponBreakdownRecord.VatAmount +
                                      billingMemoCouponBreakdownRecord.IscAmountBilled + billingMemoCouponBreakdownRecord.UatpAmountBilled +
                                      billingMemoCouponBreakdownRecord.TaxAmount + billingMemoCouponBreakdownRecord.OtherCommissionBilled +
                                      billingMemoCouponBreakdownRecord.HandlingFeeAmount),Constants.PaxDecimalPlaces);

      // Check whether net amount is positive for billing memo, else throw an exception. 
      if (billingMemoCouponBreakdownRecord.NetAmountBilled < 0 || isNetAmountValueMisMatched)
      {
        throw new ISBusinessException(ErrorCodes.InvalidAmount);
      }

      //made change to check the ref field values if reson code is 8P
      if (billingMemo.ReasonCode == "8P" && !ReferenceManager.IsValidRficCode(billingMemoCouponBreakdownRecord.ReferenceField1))
      {
        throw new ISBusinessException(ErrorCodes.InvalidReferenceField1ForBMCoupon);
      }

      //made change to check the ref field values if reson code is 8P
      if (billingMemo.ReasonCode == "8P" && !ReferenceManager.IsValidRfiscCode(billingMemoCouponBreakdownRecord.ReferenceField2, billingMemoCouponBreakdownRecord.ReferenceField1))
      {
        throw new ISBusinessException(ErrorCodes.InvalidReferenceField2ForBMCoupon);
      }


      // Check if passed 'Currency Adjustment Indicator' is a valid currency code
      // For New coupon Record validation will be done 
      // For Update coupon Record if value CurrencyAdjustmentIndicator is updated then only validation will be done
      /* Logic Applied : 
       * Check for Currency Adjustment indicator only when
       * - entered value is not null and
       * - when value has changed during update operation
       */
      if ((!string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator)) &&
          (isUpdateOperation ? CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.CurrencyAdjustmentIndicator, billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator) : true))
      {
        if (!ReferenceManager.IsValidCurrencyCode(billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          throw new ISBusinessException(ErrorCodes.InvalidCurrencyAdjustmentInd);
        }
      }

      // From Airport and To Airport should not be same.
      if (!string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.FromAirportOfCoupon) && !string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ToAirportOfCoupon) &&
          billingMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim().Equals(billingMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()))
      {
        throw new ISBusinessException(ErrorCodes.InvalidAirportCombination);
      }

      if (!isUpdateOperation || CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.FromAirportOfCoupon, billingMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        if (!string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.FromAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(billingMemoCouponBreakdownRecord.FromAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidFromAirportCode);
        }
      }

      if (!isUpdateOperation || CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.ToAirportOfCoupon, billingMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        if (!string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ToAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(billingMemoCouponBreakdownRecord.ToAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
        }
      }

      if (!isUpdateOperation ||
       (CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber) ||
       CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.TicketOrFimIssuingAirline, billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline) ||
       CompareUtil.IsDirty(billingMemoCouponBreakdownRecordInDb.TicketOrFimCouponNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber)))
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


        var duplicateCouponCount = BillingMemoCouponBreakdownRepository.GetBillingMemoCouponDuplicateCount(billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber,
                                                                                                           billingMemoCouponBreakdownRecord.TicketDocOrFimNumber,
                                                                                                           billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline,
                                                                                                           invoice.BillingMemberId,
                                                                                                           invoice.BilledMemberId,
                                                                                                           billingYearToCompare,
                                                                                                           billingMonthToCompare);

        //if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0))
        if (duplicateCouponCount > 0)
        {
          duplicateErrorMessage = string.Format(Messages.BillingMemoCouponDuplicateMessage, duplicateCouponCount);
        }
      }

      return duplicateErrorMessage;
    }

    /// <summary>
    /// Validate Rejection memo record.
    /// </summary>
    /// <param name="rejectionMemoRecord">Rejection memo record to be validated.</param>
    /// <param name="rejectionMemoRecordInDb">The rejection memo record in db.</param>
    /// <returns></returns>
    private string ValidateRejectionMemo(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordInDb)
    {
      // SCP225675: //Urgent// About the incoming XML file for SEP P4
      if (rejectionMemoRecord.RejectionStage < 1 || rejectionMemoRecord.RejectionStage > 3)
      {
        Logger.ErrorFormat("RejectionStage is: {0}", rejectionMemoRecord.RejectionStage);
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

      double vatBreakdownTotal;
      // Check whether operation is EditRejectionMemo and CouponBreakdownRecord exists, if yes retrieve TotalVatAmountDifference from Coupon level else retrieve from VatAmount from Memo level. 
      if (isUpdateOperation && rejectionMemoRecordInDb.CouponBreakdownRecord.Count > 0)
      {
        vatBreakdownTotal = rejectionMemoRecordInDb.TotalVatAmountDifference;
      }
      else
      {
        vatBreakdownTotal = rejectionMemoRecord.RejectionMemoVat.Aggregate(0.0, (current, vat) => current + Convert.ToDouble(vat.VatCalculatedAmount));
      }

      vatBreakdownTotal = ConvertUtil.Round(vatBreakdownTotal, Constants.PaxDecimalPlaces);

      // Check Coupon Vat Breakdown Total Amount to match Coupon Vat Amount.
      // If Not, then throw exception. 
      if (!rejectionMemoRecord.TotalVatAmountDifference.Equals(Convert.ToDouble(vatBreakdownTotal)))
      {
        throw new ISBusinessException(ErrorCodes.VatTotalAmountMismatch);
      }

      //TODO: Need to check if rejectionMemoRecord.Invoice.BillingCode works or not
      // Get invoice for rejection memo record.
      //SCP85039:Performance improvement of is-web during RMCreate and return
     // var rejectionMemoInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);
      var rejectionMemoInvoice = InvoiceRepository.Get(i=>i.Id==rejectionMemoRecord.InvoiceId).SingleOrDefault();

      if (!((rejectionMemoInvoice.BillingYear > rejectionMemoRecord.YourInvoiceBillingYear) ||
        ((rejectionMemoInvoice.BillingYear == rejectionMemoRecord.YourInvoiceBillingYear) && (rejectionMemoInvoice.BillingMonth > rejectionMemoRecord.YourInvoiceBillingMonth)) ||
          ((rejectionMemoInvoice.BillingYear == rejectionMemoRecord.YourInvoiceBillingYear) && (rejectionMemoInvoice.BillingMonth == rejectionMemoRecord.YourInvoiceBillingMonth) && (rejectionMemoInvoice.BillingPeriod > rejectionMemoRecord.YourInvoiceBillingPeriod))))
        throw new ISBusinessException(ErrorCodes.InvalidYourBillingPeriod);

      string errorMessage = string.Empty;
      //code when rejection stage is 1 and FimBimCM value is none.
      if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None && rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageOne)
      {
        var criteria = new RMLinkingCriteria
        {
            //SCP37078: Add source code in criteria to check wheter it is fim rejection or not.
            SourceCode=Convert.ToString(rejectionMemoRecord.SourceCodeId),
          ReasonCode = rejectionMemoRecord.ReasonCode,
          InvoiceNumber = rejectionMemoRecord.YourInvoiceNumber,
          BillingYear = rejectionMemoRecord.YourInvoiceBillingYear,
          BillingMonth = rejectionMemoRecord.YourInvoiceBillingMonth,
          BillingPeriod = rejectionMemoRecord.YourInvoiceBillingPeriod,
          FimBMCMNumber = rejectionMemoRecord.FimBMCMNumber,
          FimCouponNumber = rejectionMemoRecord.FimCouponNumber,
          RejectionMemoNumber = rejectionMemoRecord.RejectionMemoNumber,
          FimBmCmIndicatorId = rejectionMemoRecord.FIMBMCMIndicatorId,
          RejectionStage = rejectionMemoRecord.RejectionStage,

          //Note : assign Billing to Billed and Billed to Billing as SP is expecting this flip-flop
          BillingMemberId = rejectionMemoInvoice.BilledMemberId,
          BilledMemberId = rejectionMemoInvoice.BillingMemberId,

          RejectedInvoiceId = rejectionMemoRecord.InvoiceId,
          IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod
        };
        var rejectionMemoResult = GetRejectionMemoLinkingDetails(criteria);

        if (rejectionMemoResult.ErrorMessage == "")
        {
          rejectionMemoRecord.IsLinkingSuccessful = rejectionMemoResult.IsLinkingSuccessful;
          rejectionMemoRecord.IsBreakdownAllowed = rejectionMemoResult.HasBreakdown;
          rejectionMemoRecord.CurrencyConversionFactor = rejectionMemoResult.CurrencyConversionFactor;
        }
        else
        {
            /* CMP #624: ICH Rewrite-New SMI X 
             * Description: Code Fixed regarding bug #9214: CMP 624: Incorrect error on stage 1 Rejection if SMI X invoice is rejected by non X invoice from ISWEB. 
             * Instead of Exception SP is modified to return SMIException to show SMI related error message. */
            if (rejectionMemoResult.ErrorMessage.Contains("BPAXNS_10943"))
                throw new ISBusinessException(ErrorCodes.PaxNSRejctionInvoiceLinkingCheckForSmiX);

            if (rejectionMemoResult.ErrorMessage.Contains("BPAXNS_10945"))
                throw new ISBusinessException(ErrorCodes.PaxNSRejInvBHLinkingCheckForSmiX);

            if (!rejectionMemoResult.ErrorMessage.Contains("Warning"))
                throw new ISBusinessException(ErrorCodes.ErrorInvoiceNotFound);

            errorMessage = rejectionMemoResult.ErrorMessage;
        }
      }
      //if (rejectionMemoInvoice.InvoiceNumber.ToUpper().Equals(rejectionMemoRecord.YourInvoiceNumber.ToUpper()))
      //{
      //  throw new ISBusinessException(ErrorCodes.InvalidYourInvoiceNumber);
      //}

      // Validation for rejection number 
      // Your Rejection Number - 
      // 1. Should be populated if Rejection Stage (Element 11) = "2" or "3" and Billing Code = 0
      // 2. Should be populated if Billing Code = 7)
      if (((rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo || rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree) &&
           rejectionMemoInvoice.BillingCode == (int)BillingCode.NonSampling) || rejectionMemoInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        if (string.IsNullOrEmpty(rejectionMemoRecord.YourRejectionNumber))
        {
          throw new ISBusinessException(ErrorCodes.InvalidYourRejectionNumber);
        }
      }

      var transactionType = GetTransactionType(rejectionMemoRecord.RejectionStage);

      // Validates whether source code exist in master table
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoRecordInDb.SourceCodeId, rejectionMemoRecord.SourceCodeId))
      {
        if (!ReferenceManager.IsValidSourceCode(rejectionMemoRecord.SourceCodeId, (int)transactionType))
        {
          throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
        }
      }

      // Validates whether reason code exist in master table.
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoRecordInDb.ReasonCode, rejectionMemoRecord.ReasonCode))
      {
        if (!ReferenceManager.IsValidReasonCode(rejectionMemoRecord.ReasonCode, (int)transactionType))
        {
          throw new ISBusinessException(ErrorCodes.InvalidReasonCode);
        }
      }

      //  Below Code commented for the reason SCP ID :20323. doesn't required re-validate the invoice existance from database as it was validation from validate linking functionality

      //if (rejectionMemoRecord.IsLinkingSuccessful.HasValue && rejectionMemoRecord.IsLinkingSuccessful.Value == true)
      //{
      //  if (rejectionMemoRecord.YourInvoiceBillingPeriod > 0)
      //  {
      //    if (!isUpdateOperation ||
      //        (CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceNumber) ||
      //         CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingMonth) ||
      //         CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingPeriod, rejectionMemoRecord.YourInvoiceBillingPeriod) ||
      //         CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingYear)))
      //    {
      //      // Combination of fields ‘Your Invoice Number’, ‘Your Billing Year’, ‘Your Billing Month’ and ‘Your Billing Period’ 
      //      // passed in rejection memo object does not match with any other invoice in invoice table, throw an exception
      //      var invoiceCount = InvoiceRepository.IsExistingInvoice(rejectionMemoRecord.YourInvoiceNumber.ToUpper(),
      //                                                             rejectionMemoRecord.YourInvoiceBillingMonth,
      //                                                             rejectionMemoRecord.YourInvoiceBillingYear,
      //                                                             rejectionMemoRecord.YourInvoiceBillingPeriod,
      //                                                             rejectionMemoInvoice.BilledMemberId,
      //                                                             rejectionMemoInvoice.BillingMemberId,
      //                                                             (int)InvoiceStatusType.Presented);
      //      if (invoiceCount < 1)
      //      {
      //        throw new ISBusinessException(ErrorCodes.LinkedInvoiceNotFound);
      //      }
      //    }
      //  }
      //}

      // Should be a unique number within each Billed Airline in the Billing period.
      if (IsDuplicateRejectionMemoNumber(rejectionMemoRecord, rejectionMemoRecordInDb, isUpdateOperation, rejectionMemoInvoice))
      {
        throw new ISBusinessException(ErrorCodes.DuplicateRejectionMemoNumber);
      }

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
                                                                    transactionType,
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

        // If TotalNetRejectAmount == 0, do not validate Net amount limit, else validate.  
        //if (rejectionMemoRecord.TotalNetRejectAmount != 0)
        //{
        // Validates net amount limit from min/max amount table.
        //if (!ValidateNetAmountLimit(rejectionMemoRecord, rejectionMemoInvoice))
        //{
        //  throw new ISBusinessException(ErrorCodes.NetRejectAmountIsNotInAllowedRange);
        //}
        //}
      }

      //Validate Time Limit
      //CMP#624 : ICH Rewrite-New SMI X.
      //Refer FRS Section 2.14 Change #3: Update of TL flag in ‘IS Validation Flag’ for PAX/CGO RMs
      if (IsTransactionOutSideTimeLimit(rejectionMemoRecord, rejectionMemoInvoice, null))
      {
        rejectionMemoRecord.ISValidationFlag += string.IsNullOrEmpty(rejectionMemoRecord.ISValidationFlag) ? TimeLimitFlag : ValidationFlagDelimeter + TimeLimitFlag;
      }

      if (rejectionMemoRecord.ReasonCode == "1A")
      {
        string warningMessage = Messages.ResourceManager.GetString(ErrorCodes.WarningMessageForReasonCode1A);
        errorMessage = string.IsNullOrEmpty(errorMessage)
                         ? warningMessage
                         : string.Format("{0} {1}", errorMessage, warningMessage);
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

      //CMP#641: Time Limit Validation on Third Stage PAX Rejections
        if(rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
        {
            var rmtransactionType = TransactionType.RejectionMemo3;
            if(rejectionMemoInvoice != null && rejectionMemoInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
            {
                rmtransactionType = TransactionType.SamplingFormXF;
            }
            ValidatePaxStageThreeRmForTimeLimit(rmtransactionType, rejectionMemoInvoice.SettlementMethodId, rejectionMemoRecord, rejectionMemoInvoice, isIsWeb: true);
        }

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

        return errorMessage;
    }


      /// <summary>
    /// Determines whether transaction is out side time limit for specified invoice].
    /// </summary>
    /// <param name="rejectionMemo">The rejection memo.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="billingPeriod"></param>
    /// <returns>
    /// true if transaction in not out side time limit for the specified invoice; otherwise, false.
    /// </returns>
    private bool IsTransactionOutSideTimeLimit(RejectionMemo rejectionMemo, PaxInvoice invoice, BillingPeriod? billingPeriod)
    {
      TransactionType transactionType = 0;

      switch (rejectionMemo.RejectionStage)
      {
        case (int)RejectionStage.StageOne:
          transactionType = TransactionType.RejectionMemo1;
          break;
        case (int)RejectionStage.StageTwo:
          transactionType = TransactionType.RejectionMemo2;
          break;
        case (int)RejectionStage.StageThree:
          transactionType = TransactionType.RejectionMemo3;
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
    /// Validates the net amount limit from min max master table.
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="rejectionMemoInvoice">The rejection memo invoice.</param>
    /// <returns></returns>
    private bool ValidateNetAmountLimit(RejectionMemo rejectionMemoRecord, PaxInvoice rejectionMemoInvoice)
    {
      TransactionType transactionType = 0;

      switch (rejectionMemoRecord.RejectionStage)
      {
        case (int)RejectionStage.StageOne:
          transactionType = TransactionType.RejectionMemo1;
          break;
        case (int)RejectionStage.StageTwo:
          transactionType = TransactionType.RejectionMemo2;
          break;
        case (int)RejectionStage.StageThree:
          transactionType = TransactionType.RejectionMemo3;
          break;
      }

      var reasonCode = ReasonCodeRepository.First(code => code.Code == rejectionMemoRecord.ReasonCode && code.TransactionTypeId == (int)transactionType);
      // Validate RM level amounts only when coupon breakdown is not mandatory.
      if (reasonCode != null && !reasonCode.CouponAwbBreakdownMandatory)
      {
        if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalNetRejectAmount), transactionType,
                                           rejectionMemoInvoice.ListingCurrencyId, rejectionMemoInvoice,
                                           validateMaxAmount: false, iMinAcceptableAmount: null,
                                           rejectionReasonCode: rejectionMemoRecord.ReasonCode,
                                           applicableMinimumField: ApplicableMinimumField.TotalNetRejectAmount))
        {
          throw new ISBusinessException(ErrorCodes.NetRejectAmountIsNotInAllowedRange);
        }

        if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalGrossDifference), transactionType, rejectionMemoInvoice.ListingCurrencyId, rejectionMemoInvoice, validateMaxAmount: false, iMinAcceptableAmount: null, rejectionReasonCode: rejectionMemoRecord.ReasonCode, applicableMinimumField: ApplicableMinimumField.TotalGrossDifference))
        {
          throw new ISBusinessException(ErrorCodes.GrossDifferenceAmountIsNotInAllowedRange);
        }
        if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalTaxAmountDifference), transactionType, rejectionMemoInvoice.ListingCurrencyId, rejectionMemoInvoice, validateMaxAmount: false, iMinAcceptableAmount: null, rejectionReasonCode: rejectionMemoRecord.ReasonCode, applicableMinimumField: ApplicableMinimumField.TotalTaxDifference))
        {
          throw new ISBusinessException(ErrorCodes.TaxDifferenceAmountIsNotInAllowedRange);
        }
      }

      //Validate Net Reject Amount
      return ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalNetRejectAmount), transactionType, rejectionMemoInvoice.ListingCurrencyId, rejectionMemoInvoice, applicableMinimumField: ApplicableMinimumField.TotalNetRejectAmount);
    }

    /// <summary>
    /// Determines whether duplicate rejection memo for the specified rejection memo record].
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="rejectionMemoRecordIndb">The rejection memo record indb.</param>
    /// <param name="isUpdateOperation">if set to <c>true</c> [is update operation].</param>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// 	<c>true</c> if [is duplicate rejection memo] [the specified rejection memo record]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsDuplicateRejectionMemoNumber(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordIndb, bool isUpdateOperation, PaxInvoice invoice)
    {
      var isExistingRejectionNumber = false;

      if (!isUpdateOperation || (CompareUtil.IsDirty(rejectionMemoRecord.RejectionMemoNumber, rejectionMemoRecordIndb.RejectionMemoNumber)))
      {
        isExistingRejectionNumber =
          RejectionMemoRepository.GetRejectionMemoDuplicateCount(invoice.BilledMemberId,
                                                                 invoice.BillingMemberId,
                                                                 rejectionMemoRecord.RejectionMemoNumber,
                                                                 invoice.BillingYear,
                                                                 invoice.BillingMonth,
                                                                 invoice.BillingPeriod) > 0;
      }
      return isExistingRejectionNumber;
    }

    /// <summary>
    /// Forms  IS-Rejection flag.
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    private void SetIsRejectionFlag(RejectionMemo rejectionMemoRecord)
    {
      var rejectionFlags = new StringBuilder();

      if (rejectionMemoRecord.ReasonCode.Equals(ReasonCode))
      {
        if (rejectionMemoRecord.TotalGrossDifference != 0)
        {
          rejectionFlags.Append(TotalGrossDifferenceRejectionFlag).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.TotalTaxAmountDifference != 0)
        {
          rejectionFlags.Append(TotalTaxDifferenceRejectionFlag).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.IscDifference != 0)
        {
          rejectionFlags.Append(TotalIscDifferenceRejectionFlag).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.OtherCommissionDifference != 0)
        {
          rejectionFlags.Append(TotalOtherCommisionDifferenceRejectionFlag).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.HandlingFeeAmountDifference != 0)
        {
          rejectionFlags.Append(TotalHandlingFeeDifference).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.UatpAmountDifference != 0)
        {
          rejectionFlags.Append(TotalUatpFeeDifference).Append(ValidationFlagDelimeter);
        }
        if (rejectionMemoRecord.TotalVatAmountDifference != 0)
        {
          rejectionFlags.Append(TotalVatAmountFeeDifference).Append(ValidationFlagDelimeter);
        }

        if (rejectionFlags.Length > 0)
        {
          rejectionMemoRecord.IsRejectionFlag = rejectionFlags.Remove(rejectionFlags.Length - 1, 1).ToString();
        }
      }
    }

    /// <summary>
    /// Validates the rejection memo coupon.
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
    /// <param name="rejectionMemoCouponBreakdownRecordInDb">The rejection memo coupon breakdown record in db.</param>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    private string ValidateRejectionMemoCoupon(RMCoupon rejectionMemoCouponBreakdownRecord,
      RMCoupon rejectionMemoCouponBreakdownRecordInDb, RejectionMemo rejectionMemoRecord, string invoiceId)
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
          throw new ISBusinessException(ErrorCodes.InvalidTaxCode, tax.TaxCode.Trim());
        }
      }
      //End SCP#50425 - Tax Code XT
      if (rejectionMemoCouponBreakdownRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber <= 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidTicketDocumnetOrFimNumber);
      }

      var invoiceGuid = invoiceId.ToGuid();
      //SCP85039:Performance improvement of is-web during RMCreate and return
      //var invoice = InvoiceRepository.Single(id: invoiceGuid);
      var invoice = InvoiceRepository.Get(i => i.Id == invoiceGuid).SingleOrDefault();

      var transactionType = GetTransactionType(rejectionMemoRecord.RejectionStage);

      var soureCodeDetails = SourceCodeRepository.Single(sourceCode => sourceCode.TransactionTypeId == (int)transactionType && sourceCode.SourceCodeIdentifier == rejectionMemoRecord.SourceCodeId);

      // for Non Frequent flyer source code, enforce validation for Percentage and Amount in ISC and UATP
      if (!soureCodeDetails.IsFFIndicator)
      {
          Tolerance currentInvoiceTolerance = new Tolerance();
          if (invoice.Tolerance == null)
          {
              if (invoice.ListingCurrencyId.HasValue)
              {

                  currentInvoiceTolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax, invoice.ListingCurrencyId.Value, invoice, Constants.PaxDecimalPlaces);

              }
              else
              {
                  currentInvoiceTolerance = new Tolerance
                  {
                      ClearingHouse = CompareUtil.GetClearingHouse(invoice.SettlementMethodId),
                      BillingCategoryId = (int)BillingCategoryType.Pax,
                      RoundingTolerance = 0,
                      SummationTolerance = 0
                  };
              }
          }
          else
          {
              currentInvoiceTolerance = invoice.Tolerance;
          }
        double expectedAllowedIscAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
        if (currentInvoiceTolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AllowedIscAmount, expectedAllowedIscAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
            throw new ISBusinessException(ErrorCodes.InvalidAllowedIscAmount);
        }
        double expectedAcceptedIscAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100,
                                                      Constants.PaxDecimalPlaces);
        if (currentInvoiceTolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AcceptedIscAmount, expectedAcceptedIscAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
            throw new ISBusinessException(ErrorCodes.InvalidAcceptedIscAmount);
        }
        double expectedAllowedUatpAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
        if (currentInvoiceTolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AllowedUatpAmount, expectedAllowedUatpAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
            throw new ISBusinessException(ErrorCodes.InvalidAllowedUatpAmount);
        } 
        double expectedAcceptedUatpAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100,
                                                       Constants.PaxDecimalPlaces);
        if (currentInvoiceTolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AcceptedUatpAmount, expectedAcceptedUatpAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
            throw new ISBusinessException(ErrorCodes.InvalidAcceptedUatpAmount);
        } 
      }

      // Duplicate rejection break down record validation
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketOrFimIssuingAirline, rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline)
        || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber)
        || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.TicketOrFimCouponNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber))
      {
        // Duplicate check - Ticket Issuing Airline, Ticket/Document Number, Coupon No.: As per values provided in the dialog by the user.
        duplicateErrorMessage = GetDuplicateRMCouponCount(rejectionMemoCouponBreakdownRecord, isUpdateOperation, invoice, rejectionMemoRecord, duplicateErrorMessage);
      }

      // From Airport and To Airport should not be same.
      //SCP 98899: IS-Web - Outgoing Source Code 45 Issue 
      if (rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon != null && rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon) && !string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon) &&
            rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim().Equals(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()))
        {
          throw new ISBusinessException(ErrorCodes.InvalidAirportCombination);
        }
      }

      //Validate FromAirportOfCoupon 
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.FromAirportOfCoupon, rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        if (!string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon)
          && !ReferenceManager.IsValidAirportCode(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.FromAirportOfCouponIsInvalid);
        }
      }

      //Validate ToAirportOfCoupon 
      if (!isUpdateOperation || CompareUtil.IsDirty(rejectionMemoCouponBreakdownRecordInDb.ToAirportOfCoupon, rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        if (!string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon) && !ReferenceManager.IsValidAirportCode(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon))
        {
          throw new ISBusinessException(ErrorCodes.InvalidToAirportCode);
        }
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

      if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoCouponBreakdownRecord.NetRejectAmount), TransactionType.Coupon, invoice.ListingCurrencyId, invoice))
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
    /// Gets the rejection memo coupon breakdown count.
    /// </summary>
    /// <param name="memoRecordId">The memo record id.</param>
    /// <returns></returns>
    public long GetRejectionMemoCouponBreakdownCount(string memoRecordId)
    {
      var memoRecordGuid = memoRecordId.ToGuid();
      return RejectionMemoCouponBreakdownRepository.GetCount(rmCoupon => rmCoupon.RejectionMemoId == memoRecordGuid);
    }

    /// <summary>
    /// Gets the billing memo coupon count.
    /// </summary>
    /// <param name="billingMemoId">The billing memo id.</param>
    /// <returns></returns>
    public long GetBillingMemoCouponCount(string billingMemoId)
    {
      var billingMemoGuid = billingMemoId.ToGuid();
      return BillingMemoCouponBreakdownRepository.GetCount(breakdownRecord => breakdownRecord.BillingMemoId == billingMemoGuid);
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
      var isTransactionExists = (CouponRecordRepository.GetCount(couponRecord => couponRecord.InvoiceId == invoiceGuid) > 0 ||
                                 BillingMemoRepository.GetCount(bmRecord => bmRecord.InvoiceId == invoiceGuid) > 0 ||
                                 RejectionMemoRepository.GetCount(rmRecord => rmRecord.InvoiceId == invoiceGuid) > 0);

      // At least one transaction/line item should be present.
      return isTransactionExists;
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
    /// Gets the credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<PrimeCouponAttachment> GetCouponRecordAttachments(List<Guid> attachmentIds)
    {
      return CouponAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the credit memo attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<BillingMemoAttachment> GetBillingMemoAttachments(List<Guid> attachmentIds)
    {
      return BillingMemoAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the credit memo coupon attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<BMCouponAttachment> GetBillingMemoCouponAttachments(List<Guid> attachmentIds)
    {
      return BillingMemoCouponAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the credit memo coupon attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    public List<RejectionMemoAttachment> GetRejectionMemoAttachments(List<Guid> attachmentIds)
    {
      return RejectionMemoAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the credit memo coupon attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    public List<RMCouponAttachment> GetRejectionMemoCouponAttachments(List<Guid> attachmentIds)
    {
      return RejectionMemoCouponAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Validates the member blocking rule.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    private void ValidateMemberBlockingRule(PaxInvoice invoice)
    {
      //Validation for Blocked Airline
      if (invoice.BilledMemberId != 0 && invoice.BillingMemberId != 0)
      {
        //Blocked by Debtor
        if (CheckBlockedMember(true, invoice.BillingMemberId, invoice.BilledMemberId, true))
        {
          throw new ISBusinessException(ErrorCodes.InvalidBillingToMember);
        }
        //Blocked by Creditor
        if (CheckBlockedMember(false, invoice.BilledMemberId, invoice.BillingMemberId, true))
        {
          throw new ISBusinessException(ErrorCodes.InvalidBillingFromMember);
        }
      }
    }
    /// <summary>
    /// Validates the parsed non sampling invoice.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <param name="issuingAirline">The issuing airline.</param>
    /// <returns></returns>
    public bool ValidateParsedInvoice(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, IDictionary<string, bool> issuingAirline)
    {
      Logger.Info("Validate Parsed Invoice Started.");

      #region CMP #624: ICH Rewrite-New SMI X

      /* Description: Detailed Validation of IS-IDEC and IS-XML Files (Part 1) */

      // Code moved at the top as per CMP #624 requirement
      var billingMember = (invoice.BillingMemberId == 0 ? null : MemberManager.GetMemberDetails(invoice.BillingMemberId));
      var billedMember = (invoice.BilledMemberId == 0 ? null : MemberManager.GetMemberDetails(invoice.BilledMemberId));
      // Get Final Parent Details for SMI, Currency, Clearing House and Suspended Flag validations
      var billingFinalParent = (invoice.BillingMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId)));
      var billedFinalParent = (invoice.BilledMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId)));
      
      // CMP#624 : 2.22-Change#3 - Update of ‘Parent Member’ information
      // Assign final parent to invoice
      if (billingFinalParent != null && billingFinalParent.Id != billingMember.Id)
      {
          invoice.BillingParentMemberId = billingFinalParent.Id;
      }
      if (billedFinalParent != null && billedFinalParent.Id != billedMember.Id)
      {
          invoice.BilledParentMemberId = billedFinalParent.Id;
      }

      if (billingMember != null)
          billingMember.IchConfiguration = MemberManager.GetIchConfig(billingMember.Id);

      if (billedMember != null)
          billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);

      //ID : 305133 - Suspension flag not set at SIS validation process for PAX and CGO
      if ((billedMember != null) && (billingMember != null))
      {
          // Update suspended flag according to ach/Ach configuration.
          if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
          {
              invoice.SuspendedInvoiceFlag = true;
          }
      }


        /* CMP #624: ICH Rewrite-New SMI X 
        * Description: ICH Web Service is called when header is saved
        * Refer FRS Section: 2.8 Detailed Validation of IS-IDEC and IS-XML Files (Part 1). */
        bool smiXValidationsPhase1Result = ValidationBeforeSmiXWebServiceCall(invoice, exceptionDetailsList,
                                                                              invoice.InvoiceTypeId,
                                                                              fileSubmissionDate, fileName,
                                                                              billingFinalParent,
                                                                              billedFinalParent, false, null,
                                                                              invoice.BatchSequenceNumber,
                                                                              invoice.RecordSequenceWithinBatch);

        if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement && invoice.GetSmiXPhase1ValidationStatus())
        {
          /* CMP #624: ICH Rewrite-New SMI X 
           * Description: ICH Web Service is called when header is saved
           * Refer FRS Section: 2.9	Detailed Validation of IS-IDEC and IS-XML Files (Part 2). */
          CallSmiXIchWebServiceAndHandleResponse(invoice,
                                                 exceptionDetailsList,
                                                 invoice.InvoiceTypeId,
                                                 fileSubmissionDate,
                                                 fileName,
                                                 false,
                                                 invoice.BatchSequenceNumber,
                                                 invoice.RecordSequenceWithinBatch);
        }

      #endregion

      var stopWatch = new Stopwatch();
      var isValid = true;
      bool isTransactionRecordsExistsInInvoice = false;
      bool isPrimeTransactionExistsInInvoice = false;
      bool isRejectionMemoTransactionExistsInInvoice = false;
      bool isBillingMemoTransactionExistsInInvoice = false;
      bool isCreditMemoTransactionExistsInInvoice = false;
      var regEx = new Regex("^[a-zA-Z0-9]+$");
      var referenceDataRepository = Ioc.Resolve<IReferenceDataManager>(typeof(IReferenceDataManager));

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
      if (billingMember != null)
      {
        if (IsTypeBMember(billingMember.MemberCodeNumeric))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          invoice.SubmissionMethodId == (int) SubmissionMethod.IsXml ? "OrganizationID" : "Billing Member",
                                                                          billingMember.MemberCodeNumeric, invoice,
                                                                          fileName, ErrorLevels.ErrorLevelInvoice,
                                                                          ErrorCodes.InvalidMemberType,
                                                                          ErrorStatus.X, invoice.BatchSequenceNumber,
                                                                          invoice.RecordSequenceWithinBatch, 0,
                                                                          string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (billedMember != null)
      {
        if (IsTypeBMember(billedMember.MemberCodeNumeric))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          invoice.SubmissionMethodId == (int) SubmissionMethod.IsXml ? "OrganizationID" : "Billed Member",
                                                                          billedMember.MemberCodeNumeric, invoice,
                                                                          fileName, ErrorLevels.ErrorLevelInvoice,
                                                                          ErrorCodes.InvalidMemberType,
                                                                          ErrorStatus.X, invoice.BatchSequenceNumber,
                                                                          invoice.RecordSequenceWithinBatch, 0,
                                                                          string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (invoice.InvoiceNumber != null && !regEx.IsMatch(invoice.InvoiceNumber))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
          "Invoice Number", invoice.InvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.SpecialCharactersAreNotAllowedInInvoiceNumber,
          ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      bool isSystemMultilingual = ValidationCache.Instance != null ? ValidationCache.Instance.IsSystemMultilingual : SystemParameters.Instance.General.IsMultilingualAllowed;
      if (!string.IsNullOrEmpty(invoice.InvTemplateLanguage) && isSystemMultilingual)
      {
        if (ValidationCache.Instance != null && ValidationCache.Instance.Languages != null)
        {
          if (!ValidationCache.Instance.Languages.ContainsKey(invoice.InvTemplateLanguage.Trim()))
          {
            //Add exception
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
                        "Invoice Template Language", invoice.InvTemplateLanguage, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidLanguage,
                        ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);

            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        else
        {
          //Fetch from db
          var langaugesRepository = Ioc.Resolve<IRepository<Language>>();
          if (langaugesRepository.Single(i => i.Language_Code.ToLower().CompareTo(invoice.InvTemplateLanguage.ToLower()) == 0 && i.IsReqForPdf) == null)
          {
            //Add exception
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
           "Invoice Template Language", invoice.InvTemplateLanguage, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidLanguage,
           ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);

            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      // Billing period should be 01,02,03,04
      if (invoice.BillingPeriod <= 0 || invoice.BillingPeriod > 4)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Period", invoice.BillingPeriod.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingPeriod, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Validation For Invoice Type
      if ((invoice.BillingCode != (int)BillingCode.SamplingFormC) && (invoice.InvoiceTypeId != (int)InvoiceType.CreditNote && invoice.InvoiceTypeId != (int)InvoiceType.Invoice))
      {

        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Type", invoice.InvoiceType.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.UnexpectedInvoiceType, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //1. Should be 000000 in case Billing Code (Element 6) = "0" or "3" 
      //2. Incase Billing Code (Element 6) = "4" or "5", "6" or "7" the 1st two digits should be a valid year and next two digits should be a valid month and the last two digits should be 00
      Logger.InfoFormat("Provisional Billing {0} {1}-{2}", invoice.BillingCode, invoice.ProvisionalBillingYear, invoice.ProvisionalBillingMonth);
      if (invoice.BillingCode == (int)BillingCode.SamplingFormC || invoice.BillingCode == (int)BillingCode.SamplingFormDE || invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        
        var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
        var currentPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        var currentPeriodYear = invoice.ProvisionalBillingYear > 99 ? currentPeriod.Year : currentPeriod.Year - 2000;
        if (invoice.ProvisionalBillingYear > currentPeriodYear || (invoice.ProvisionalBillingYear == currentPeriodYear && invoice.ProvisionalBillingMonth > currentPeriod.Month) || invoice.ProvisionalBillingMonth > 12)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Billing Month", string.Format("{0}{1}", invoice.ProvisionalBillingYear.ToString().PadLeft(4, '0').Substring(2, 2), invoice.ProvisionalBillingMonth.ToString().PadLeft(2, '0')), invoice, fileName, ErrorLevels.ErrorLevelInvoice,
                                                                      ErrorCodes.InvoiceProvisionalBillingMonthInvalid, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.ProvisionalBillingMonth == 0 || invoice.ProvisionalBillingYear == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Month", string.Format("{0}{1}", invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingMonth), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidProvisionalInvoiceDate, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if (invoice.BillingCode == (int)BillingCode.NonSampling || invoice.BillingCode == (int)BillingCode.SamplingFormAB)
        {
          if (invoice.ProvisionalBillingMonth != 0 || invoice.ProvisionalBillingYear != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Invoice Month", string.Format("{0}{1}", invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingMonth), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidProvisionalInvoiceDate, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      
      // Get current billing period once per invoice and use it for validating all the records in invoice.
      var clearingHouseEnum = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);
      

      BillingPeriod billingPeriod;
      bool isValidBillingPeriod = false;
      var invBillingPeriod = new BillingPeriod
      {
        ClearingHouse = clearingHouseEnum,
        Month = invoice.BillingMonth,
        Year = invoice.BillingYear,
        Period = invoice.BillingPeriod
      };

      try
      {
        billingPeriod = CalendarManager.GetBillingPeriod(fileSubmissionDate, clearingHouseEnum);
        isValidBillingPeriod = ValidateBillingPeriod(invoice, billingPeriod, clearingHouseEnum);


        if (!isValidBillingPeriod)
        {
          // Check for the future submission
          // If it is future submitted invoices then do not change file submission period
          if (!(billingPeriod < invBillingPeriod))
          {
            billingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
          }
        }
      }
      catch (ISCalendarDataNotFoundException)
      {
        billingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
      }

      if (!isValidBillingPeriod)
      {

        // Check for the future submission of invoices
        // If it is future submitted update validation status of invoices to future submitted status
        // Set is future submission flag to true
        if (billingPeriod < invBillingPeriod)
        {
          // Get passenmgerconfiguration of the billing memberid
          var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoice.BillingMemberId);

          // Check whether futuresubmittion is allowed to user or not
          if (passengerConfiguration != null && passengerConfiguration.IsFutureBillingSubmissionsAllowed)
          {
            invoice.IsFutureSubmission = true;
            invoice.ValidationStatus = InvoiceValidationStatus.FutureSubmission;
            invoice.ValidationStatusId = (int)InvoiceValidationStatus.FutureSubmission;
          }
          else
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Date", string.Format("{0}{1}{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), invoice.BillingPeriod.ToString().PadLeft(2, '0')), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingMonthAndPeriod, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        else if (ReferenceManager.IsValidSmiForLateSubmission(invoice.SettlementMethodId) && IsLateSubmission(invoice, fileSubmissionDate, clearingHouseEnum, billingPeriod))
        {
          invoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
          invoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Date", string.Format("{0}{1}{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), invoice.BillingPeriod.ToString().PadLeft(2, '0')), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingMonthAndPeriod, ErrorStatus.X);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // Listing to Billing Rate should be 1 when Currency of Listing and Currency of Billing are the same.
      if (invoice.ListingCurrencyId.HasValue && invoice.BillingCurrencyId.HasValue)
      {
        if (invoice.ListingCurrencyId.Value == invoice.BillingCurrencyId.Value && Convert.ToDecimal(invoice.ExchangeRate) != 1)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing To Billing Rate", Convert.ToString(invoice.ExchangeRate), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRateForSameCurrencies, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (invoice.MemberLocationInformation.Count != 0)
      {
        stopWatch.Start();

        // Validate RefData 1,2 
        foreach (var memberLocationInfo in invoice.MemberLocationInformation)
        {
          if (!string.IsNullOrEmpty(memberLocationInfo.CountryCode))
          {
              if (!ReferenceManager.IsValidCountryCode(invoice, memberLocationInfo.CountryCode))
              {
                  var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Member Location Country code",
                                memberLocationInfo.CountryCode, invoice, fileName,
                                ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidCountryCode, ErrorStatus.X,
                                invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
              }
              else if (invoice.BilledReferenceDataSourceId == (int)ReferenceDataSource.Supplied ||
                    invoice.BillingReferenceDataSourceId == (int)ReferenceDataSource.Supplied)
              {
                  //CMP496: 1) If country code is valid then get country name from mst_country master.
                  //This validation only work when reference data is provide in file or supplied
                  //Replace it with file's country name
                  //This code will work for Both Billing and Billed
                  string countryName = ReferenceManager.GetCountryNameByCode(memberLocationInfo.CountryCode);
                  memberLocationInfo.CountryName = countryName;
              }
          }

          #region CMP496: 2) Validate Billed reference data if supplied in file.
          //This code will only work if billed location code equal to MISC. 
          if (!memberLocationInfo.IsBillingMember && invoice.BilledReferenceDataSourceId == (int)ReferenceDataSource.Supplied)
          {
              Logger.Info(string.Format("Location Code := {0}, MemberId := {1}, Billing Category := {2} ",
                                        invoice.BilledMemberLocationCode, invoice.BilledMemberId,
                                        invoice.BillingCategoryId));

              exceptionDetailsList = referenceDataRepository.ReferenceDataValidation(exceptionDetailsList,invoice.SubmissionMethod, fileName,
                                                                                     fileSubmissionDate, invoice);
              Logger.Info("Reference Data Validated");
          }
          #endregion

        }

        stopWatch.Stop();
        Logger.InfoFormat("Validating country code in member locations - time required: [{0}]", stopWatch.Elapsed);
      }

      #region CMP496: Comment, this validation not covering all possible cases.
      /*CMP496: This validations are not cover all scenarios  
       * if ((!string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode)) && (!string.IsNullOrWhiteSpace(invoice.BilledMemberLocationCode)))
       {
         var isValidBillingLocationCombination = false;

         //Either both location should be provided or location information for both billing and billed member should be provided
         if (invoice.SubmissionMethodId == (int) SubmissionMethod.IsXml && invoice.MemberLocationInformation.Count == 2 && !string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode) &&
             !string.IsNullOrEmpty(invoice.BilledMemberLocationCode))
         {
           isValidBillingLocationCombination = true;
         }

         //Either both location should be provided or location information for both billing and billed member should be provided
         if (invoice.MemberLocationInformation.Count == 2 && string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode) && string.IsNullOrWhiteSpace(invoice.BilledMemberLocationCode))
         {
           isValidBillingLocationCombination = true;
         }

         if (invoice.MemberLocationInformation.Count == 0 && !string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode) && !string.IsNullOrWhiteSpace(invoice.BilledMemberLocationCode))
         {
           isValidBillingLocationCombination = true;
         }

         if (invoice.MemberLocationInformation.Count == 0 && string.IsNullOrWhiteSpace(invoice.BillingMemberLocationCode) && string.IsNullOrWhiteSpace(invoice.BilledMemberLocationCode))
         {
           isValidBillingLocationCombination = true;
         }
         */
      #endregion

      #region CMP496: 3) Adding New validation to cover all scenarios using Matrix.
      var validBillingBilledLocationCombination =
              referenceDataRepository.IsValidBillingBilledCombination(invoice.MemberLocationInformation,
                                                                      invoice.SubmissionMethod,
                                                                      invoice.BillingMemberLocationCode,
                                                                      invoice.BilledMemberLocationCode,
                                                                      invoice.BillingReferenceDataSourceId,
                                                                      invoice.BilledReferenceDataSourceId);

       switch (validBillingBilledLocationCombination)
        {
            case ReferenceDataErrorType.General:
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Member Location Code",
                                                                        string.Empty,
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        ErrorCodes.InvalidInvoiceMemberLocationInformation,
                                                                        ErrorStatus.X,
                                                                        invoice.BatchSequenceNumber,
                                                                        invoice.RecordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                    break;
                }
            case ReferenceDataErrorType.Specific:
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Billing Member Location Code",
                                                                          string.Empty,
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                          ErrorCodes.InvalidInvoiceBillingMemberLocationInformation,
                                                                          ErrorStatus.X,
                                                                          invoice.BatchSequenceNumber,
                                                                          invoice.RecordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                    break;
                }
            case ReferenceDataErrorType.Both:
                {
                    var validationExceptionDetail1 = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                         exceptionDetailsList.Count() + 1,
                                                                         fileSubmissionDate,
                                                                         "Billing Member Location Code",
                                                                         string.Empty,
                                                                         invoice,
                                                                         fileName,
                                                                         ErrorLevels.ErrorLevelInvoice,
                                                                         ErrorCodes.InvalidInvoiceBillingMemberLocationInformation,
                                                                         ErrorStatus.X,
                                                                         invoice.BatchSequenceNumber,
                                                                         invoice.RecordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail1);
                    
                    var validationExceptionDetail2 = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Member Location Code",
                                                                        string.Empty,
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        ErrorCodes.InvalidInvoiceMemberLocationInformation,
                                                                        ErrorStatus.X,
                                                                        invoice.BatchSequenceNumber,
                                                                        invoice.RecordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail2);
                    isValid = false;

                    break;
                }
        }
           

      #endregion

      bool isValidBilledMemberLocation = true;
      bool isValidBillingMemberLocation = true;

      // Calculate time required to check whether member locations are valid.
      stopWatch.Reset();
      stopWatch.Start();

      if (invoice.BillingMemberLocationCode != null && !string.IsNullOrEmpty(invoice.BillingMemberLocationCode.Trim()) && invoice.BillingMemberId != 0)
      {
        if (!MemberManager.IsValidMemberLocation(invoice.BillingMemberLocationCode, invoice.BillingMemberId))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member Location Code", invoice.BillingMemberLocationCode,
                     invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingMemberLocation, ErrorStatus.X,
                     invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
          isValidBillingMemberLocation = false;
        }
      }
      else
      {
        isValidBillingMemberLocation = false;
      }

      if (invoice.BilledMemberLocationCode != null && !string.IsNullOrEmpty(invoice.BilledMemberLocationCode.Trim()) && invoice.BilledMemberId != 0)
      {
        
        if (!MemberManager.IsValidMemberLocation(invoice.BilledMemberLocationCode, invoice.BilledMemberId))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billed Member Location Code", invoice.BilledMemberLocationCode, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBilledMemberLocation, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
          isValidBilledMemberLocation = false;
        }
      }
      else
      {
        isValidBilledMemberLocation = false;
      }

      stopWatch.Stop();
      Logger.InfoFormat("Validating member location codes - time required: [{0}]", stopWatch.Elapsed);

      #region CMP496: 4) Populate billed member location code. Due to new scenario introduced.
      //A new scenario introduce when billing location code not supplied and billing ref data, billed location code, billed ref data are passed.
      //Due this billed member location code is not populated in member location information table's column.
      if (invoice.BilledReferenceDataSourceId == (int)ReferenceDataSource.Supplied && isValidBilledMemberLocation)
      {
          invoice.MemberLocationInformation = referenceDataRepository.PopulateBilledMemberLocationCode(invoice.MemberLocationInformation,
                                                                     invoice.BilledMemberId,
                                                                     invoice.BilledMemberLocationCode,
                                                                     invoice.BilledReferenceDataSourceId,
                                                                     isValidBilledMemberLocation);
      }
      #endregion

      // Calculate time required to validate billing and listing currencies.
      stopWatch.Reset();
      stopWatch.Start();

      #region Code moved at the top as per CMP #624 requirement
      //var billingMember = (invoice.BillingMemberId == 0 ? null : MemberManager.GetMemberDetails(invoice.BillingMemberId));
      //var billedMember = (invoice.BilledMemberId == 0 ? null : MemberManager.GetMemberDetails(invoice.BilledMemberId));
      
      //// Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
      //var billingFinalParent = (invoice.BillingMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId)));
      //var billedFinalParent = (invoice.BilledMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId)));
      
      //// Assign final parent to invoice
      //if (billingFinalParent != null && billingFinalParent.Id != billingMember.Id)
      //{
      //  invoice.BillingParentMemberId = billingFinalParent.Id;
      //}
      //if (billedFinalParent != null && billedFinalParent.Id != billedMember.Id)
      //{
      //  invoice.BilledParentMemberId = billedFinalParent.Id;
      //}

      // Retrieve Ich Configurations of the members only once.
    
      //if (billingMember != null)
      //  billingMember.IchConfiguration = MemberManager.GetIchConfig(billingMember.Id);

      //if (billedMember != null)
      //  billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);
      #endregion

      //Fixed Issue Id : 65713 
      // Retrieve Ich Configurations of the Parent members only.
      if (billingFinalParent != null)
          billingFinalParent.IchConfiguration = MemberManager.GetIchConfig(billingFinalParent.Id);
      if (billedFinalParent != null)
          billedFinalParent.IchConfiguration = GetIchConfiguration(billedFinalParent.Id);


      // Validate billing currency
      if (invoice.BillingCurrencyId != null)
      {
        if (!ReferenceManager.IsValidCurrency(invoice, invoice.BillingCurrencyId))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Currency", invoice.BillingCurrencyId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingCurrency, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          // Validate billing currency
          //CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1.doc
          if (billingMember != null && billedMember != null && !ValidateBillingCurrency(invoice, billingFinalParent, billedFinalParent, true,
                                             exceptionDetailsList, fileName, fileSubmissionDate))
          {
            isValid = false;
          }
        }
      }

      // Validation foe DS w r. to member profile.
      //Shambhu Thakur - Commented Digital Signature Validation
      //Shambhu Thakur(08Sep11) - uncommented member profile digital sign application check
      
      if (billingMember != null)
      {
        if ((!billingMember.DigitalSignApplication) && (invoice.DigitalSignatureRequired.Equals(DigitalSignatureRequired.Yes)))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Digital SignitureRequried", invoice.DigitalSignatureRequiredId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidDigitalSignatureRequired, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //  else if ((invoice.DigitalSignatureRequired.Equals(DigitalSignatureRequired.Yes)) && (billingMember.DigitalSignApplication))
        //  {
        //    if (!IsDigitalSignatureRequired(invoice, billingMember, billedMember))
        //    {
        //      var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Digital SignitureRequried", invoice.DigitalSignatureRequiredId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidDSFlagAsCountryNotSpecified, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
        //      exceptionDetailsList.Add(validationExceptionDetail);
        //      isValid = false;
        //    }
        //  }
      }

      // Validate listing currency
      if (invoice.ListingCurrencyId != null)
      {
        if (!ReferenceManager.IsValidCurrency(invoice, invoice.ListingCurrencyId))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing Currency",
            invoice.ListingCurrencyId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingCurrency, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          try
          {
            ValidateInvoiceListingCurrency(invoice);
            // SCP177435 - EXCHANGE RATE 
            if (billingMember != null && billedMember != null && !ValidateListingCurrency(invoice, billingFinalParent, billedFinalParent))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing Currency",
              invoice.ListingCurrencyId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingCurrency, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          catch (ISBusinessException)
          {

            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing Currency",
              invoice.ListingCurrencyId.ToString(), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingCurrency, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Validating billing and listing curencies - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to validate duplicate invoice number.
      stopWatch.Reset();
      stopWatch.Start();

      // Validate duplication of invoice number
      if (invoice.InvoiceNumber != null)
      {
        string currentfilename = string.Empty;
        //478879 - Loading IS-XML from IS-WEB not working Duplicate File Issue.
        var filetime = fileSubmissionDate.ToString("yyyyMMddHHMMss");


        if (ParsedInvoiceList.ContainsKey(invoice.InvoiceNumber))
        {
          ParsedInvoiceList.TryGetValue(invoice.InvoiceNumber, out currentfilename);
        }

        var msg = string.Format("FN:{0},CFN:{1},FNE:{2},Inv:{3},BY:{4},MId:{5}", fileName + filetime, currentfilename, (!string.IsNullOrEmpty(currentfilename) && currentfilename.Equals(fileName + filetime)), invoice.InvoiceNumber.Trim(), invoice.BillingYear, invoice.BillingMemberId);
        Logger.InfoFormat("Validating duplicate invoice : [{0}]", msg);

        if (!ValidateInvoiceNumber(invoice.InvoiceNumber.Trim(), invoice.BillingYear, invoice.BillingMemberId) || (!string.IsNullOrEmpty(currentfilename) && currentfilename.Equals(fileName + filetime)))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Number", invoice.InvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.DuplicateInvoiceFound, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (!ParsedInvoiceList.ContainsKey(invoice.InvoiceNumber))
        {
           ParsedInvoiceList.Add(invoice.InvoiceNumber, fileName + filetime);
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Validating duplicate invoice number - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to populate default locations.
      stopWatch.Reset();
      stopWatch.Start();

      if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec || invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml)
      {
        if (invoice.MemberLocationInformation.Count == 0)
        {
          if (invoice.BillingMemberLocationCode != null && invoice.BilledMemberLocationCode != null)
          {
            if (string.IsNullOrEmpty(invoice.BillingMemberLocationCode.Trim()) && string.IsNullOrEmpty(invoice.BilledMemberLocationCode.Trim()))
            {
              invoice.BillingMemberLocationCode = DefaultMemberLocationCode;
              invoice.BilledMemberLocationCode = DefaultMemberLocationCode;
              invoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.Default;
              invoice.BilledReferenceDataSourceId = (int)ReferenceDataSource.Default;
            }
            /*else if (isValidBillingMemberLocation && isValidBilledMemberLocation)
            {
              invoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.AsPerLocation;
              invoice.BilledReferenceDataSourceId = (int)ReferenceDataSource.AsPerLocation;
            }*/
            if (isValidBillingMemberLocation)
            {
                invoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.AsPerLocation;
            }
            else if (string.IsNullOrEmpty(invoice.BillingMemberLocationCode.Trim()))
            {
                invoice.BillingMemberLocationCode = DefaultMemberLocationCode;
                invoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.Default;
            }

            if (isValidBilledMemberLocation)
            {
                invoice.BilledReferenceDataSourceId = (int)ReferenceDataSource.AsPerLocation;
            }
            else if (string.IsNullOrEmpty(invoice.BilledMemberLocationCode.Trim()))
            {
                invoice.BilledMemberLocationCode = DefaultMemberLocationCode;
                invoice.BilledReferenceDataSourceId = (int)ReferenceDataSource.Default;
            }
            // Populate billing member location information from member's location table.
            var billingMemberLocationInformation = new MemberLocationInformation { IsBillingMember = true, Id = Guid.NewGuid() };
            if (PopulateDefaultLocation(invoice.BillingMemberId, billingMemberLocationInformation, invoice.BillingMemberLocationCode, invoice))
            {
              invoice.MemberLocationInformation.Add(billingMemberLocationInformation);
            }
            // Populate billed member location information from member's location table.
           var billedMemberLocationInformation = new MemberLocationInformation { IsBillingMember = false, Id = Guid.NewGuid() };
            if (PopulateDefaultLocation(invoice.BilledMemberId, billedMemberLocationInformation, invoice.BilledMemberLocationCode))
            {
              invoice.MemberLocationInformation.Add(billedMemberLocationInformation);
            }
          }
        }
        else if (invoice.MemberLocationInformation.Count == 2 && string.IsNullOrWhiteSpace(invoice.LegalText))
        {
          var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
          var eBillingConfig = memberManager.GetEbillingConfig(invoice.BillingMemberId);
          invoice.LegalText = eBillingConfig != null && eBillingConfig.LegalText != null ? eBillingConfig.LegalText.Trim().Replace("\r", "").Replace("\n", "") : string.Empty;
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Populating default locations - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to validate billing and and billed member.
      stopWatch.Reset();
      stopWatch.Start();

      // Validate settlement method
      if (billingMember != null && billedMember != null)
      {
        // Make sure billing and billed member are not the same.
        if (invoice.BilledMemberId == invoice.BillingMemberId)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member",
                          string.Format("{0}-{1}", invoice.BillingMember.MemberCodeNumeric, invoice.BilledMember.MemberCodeNumeric), invoice, fileName,
                          ErrorLevels.ErrorLevelInvoice, ErrorCodes.SameBillingAndBilledMember, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        #region CMP #624: ICH Rewrite-New SMI X 
        if (invoice.SettlementMethodId != (int)SMI.IchSpecialAgreement)
        {
          if (!ValidatePaxAndCargoSettlementMethod(invoice, billingFinalParent, billedFinalParent, invoice.InvoiceTypeId))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "Settlement Method",
                                                                            invoice.SettlementMethodDisplayText,
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                            ErrorCodes.InvalidSettlementMethod,
                                                                            ErrorStatus.X,
                                                                            invoice.BatchSequenceNumber,
                                                                            invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else //CMP602
          {
            SetViewableByClearingHouse(invoice);
          }
        }


        #endregion

        // Validation for Blocked Airline
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
        if (invoice.BilledMemberId != 0 && invoice.BillingMemberId != 0 && (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true) && invoice.InvoiceSmi != SMI.AdjustmentDueToProtest && invoice.InvoiceSmi != SMI.NoSettlement))
        {
          var isCreditorBlocked = false;
          var isDebitorBlocked = false;
          var isCGrpBlocked = false;
          var isDGrpBlocked = false;

          #region Old Code - Commented
          //int billingZoneId = 0;
          //int billedZoneId = 0;
          //string smiValue = string.Empty;

          //if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
          //{
          //  smiValue = "ACH";
          //}
          //else if (invoice.InvoiceSmi == SMI.Ich)
          //{
          //  smiValue = "ICH";
          //}

          //if (billingMember.IchConfiguration != null)
          //{
          //  billingZoneId = billingMember.IchConfiguration.IchZoneId;
          //}
          //else
          //{
          //  var achRecordQuery =
          //   AchRepository.Get(rec => rec.MemberId == invoice.BillingMemberId
          //     && (rec.AchMembershipStatusId == (int)AchMembershipStatus.Live || rec.AchMembershipStatusId == (int)AchMembershipStatus.Suspended));
          //  var achRecordList = achRecordQuery.Select(rec => rec.MemberId).ToList();
          //  if (achRecordList.Count > 0)
          //  {
          //    // Ach member is in zone C
          //    billingZoneId = 3;
          //  }
          //}

          //if (billedMember.IchConfiguration != null)
          //{
          //  billedZoneId = billedMember.IchConfiguration.IchZoneId;
          //}
          //else
          //{
          //  var achRecordQuery =
          //   AchRepository.Get(rec => rec.MemberId == invoice.BilledMemberId
          //     && (rec.AchMembershipStatusId == (int)AchMembershipStatus.Live || rec.AchMembershipStatusId == (int)AchMembershipStatus.Suspended));
          //  var achRecordList = achRecordQuery.Select(rec => rec.MemberId).ToList();
          //  if (achRecordList.Count > 0)
          //  {
          //    // Ach member is in zone C
          //    billedZoneId = 3;
          //  }
          //}


          //if (billingZoneId != 0 && billedZoneId != 0)
          //{

          //BlockingRulesRepository.ValidateBlockingRules(invoice.BillingMemberId, invoice.BilledMemberId, BillingCategoryType.Pax, smiValue, billingZoneId, billedZoneId, out isCreditorBlocked, out isDebitorBlocked, out isCGrpBlocked, out isDGrpBlocked);
          #endregion

          //SCP164383: Blocking Rule Failed
          //Desc: Hooking a call to centralized code for blocking rule validation
          ValidationForBlockedAirline(invoice.BillingMemberId, invoice.BilledMemberId, (SMI)invoice.InvoiceSmi,
                                      BillingCategoryType.Pax, out isCreditorBlocked, out isDebitorBlocked,
                                      out isCGrpBlocked, out isDGrpBlocked);

          // Blocked by Creditor
            if (isCreditorBlocked)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, string.Empty, string.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingToMember, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            // Blocked by Debitor
            if (isDebitorBlocked)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, string.Empty, string.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingFromMember, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            //Validate BlockBy Group Rule
            if (isCGrpBlocked)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, string.Empty, string.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingToMemberGroup, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            if (isDGrpBlocked)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, string.Empty, string.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingFromMemberGroup, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch, 0, string.Empty);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          //} end of if (billingZoneId != 0 && billedZoneId != 0)
        }


        if (!ValidateBillingMembershipStatus(billingMember))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", invoice.BillingMember.MemberCodeNumeric, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingMemberStatus, ErrorStatus.X, 0, 0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (!ValidateBilledMemberStatus(billedMember))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billed Member", invoice.BilledMember.MemberCodeNumeric, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBilledMemberStatus, ErrorStatus.X, 0, 0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      foreach (var meberLocationinfo in invoice.MemberLocationInformation)
      {
        if (meberLocationinfo.OrganizationDesignator != null)
        {
          if (meberLocationinfo.IsBillingMember)
          {
            if (billingMember != null && !meberLocationinfo.OrganizationDesignator.Equals(billingMember.MemberCodeAlpha))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member Organization Designator",
                                                                          meberLocationinfo.OrganizationDesignator, invoice, fileName, ErrorLevels.ErrorLevelInvoice,
                                                                          ErrorCodes.InvalidOrganizationDesignator, ErrorStatus.X, invoice.BatchSequenceNumber,
                                                                          invoice.RecordSequenceWithinBatch);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          else
          {
            if (billedMember != null && !meberLocationinfo.OrganizationDesignator.Equals(billedMember.MemberCodeAlpha))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billed Member Organization Designator",
                                                                          meberLocationinfo.OrganizationDesignator, invoice, fileName,
                                                                          ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidOrganizationDesignator, ErrorStatus.X,
                                                                          invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Validating billing and billed member information - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to validate invoice date.
      stopWatch.Reset();
      stopWatch.Start();

      // Validate invoice Date 
      if (!ValidateParsedInvoiceDate(invoice.InvoiceDate, billingPeriod))
      {
        if (!IsLateSubmission(invoice, invoice.InvoiceDate, clearingHouseEnum, billingPeriod))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Date",
                                                                Convert.ToString(invoice.InvoiceDate), invoice, fileName,
                                                                ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidInvoiceDate, ErrorStatus.X,
                                                                invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Validating invoice date - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to validate exchange rate.
      stopWatch.Reset();
      stopWatch.Start();

      ExchangeRate exchangeRate = null;

      if (invoice.BillingYear != 0 && invoice.BillingMonth != 0)
      {
        // This exchange rate will be used for validating net amounts in coupon.
        var invoiceBillingDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);

        exchangeRate = ExchangeRateRepository.First(
          rate => rate.CurrencyId == invoice.ListingCurrencyId &&
          rate.EffectiveFromDate <= invoiceBillingDate && rate.EffectiveToDate >= invoiceBillingDate);
      }

      // Validate billing currency rate  
      if (invoice.ListingCurrencyId != invoice.BillingCurrencyId && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AdjustmentDueToProtest))
      {
        bool invalidCurrencyRate = true;

        if (exchangeRate != null)
        {
          switch (invoice.BillingCurrencyId)
          {
            case (int)BillingCurrency.EUR:
              if (ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateEur), Constants.ExchangeRateDecimalPlaces) != Convert.ToDecimal(invoice.ExchangeRate)) invalidCurrencyRate = false;
              break;

            case (int)BillingCurrency.GBP:
              if (ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateGbp), Constants.ExchangeRateDecimalPlaces) != Convert.ToDecimal(invoice.ExchangeRate)) invalidCurrencyRate = false;
              break;

            case (int)BillingCurrency.USD:
              if (ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateUsd), Constants.ExchangeRateDecimalPlaces) !=  Convert.ToDecimal(invoice.ExchangeRate)) invalidCurrencyRate = false;
              break;
          }

          if (!invalidCurrencyRate)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Listing to Billing Rate", Convert.ToString(invoice.ExchangeRate), invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }
      stopWatch.Stop();
      Logger.InfoFormat("Validating exchange rate - time required: [{0}]", stopWatch.Elapsed);

      // Calculate time required to find duplicate coupons.
      stopWatch.Reset();
      stopWatch.Start();

      // Validate Batch Number and Record Seq number in BM and RM of Invoice
      //SCP#483217 : FW - Validation Report for MAY 15 Per 01
      var bmRmBatchseqNu = (from bmRecord in invoice.BillingMemoRecord
                            join rmRecord in invoice.RejectionMemoRecord on
                              new { bmRecord.BatchSequenceNumber, bmRecord.RecordSequenceWithinBatch } equals
                              new { rmRecord.BatchSequenceNumber, rmRecord.RecordSequenceWithinBatch }
                            group rmRecord by new { bmRecord.BillingMemoNumber, rmRecord.RejectionMemoNumber, rmRecord.BatchSequenceNumber, rmRecord.RecordSequenceWithinBatch, rmRecord.SourceCodeId }
                              into bmRmEqualBatchSeq
                              from g in bmRmEqualBatchSeq.DefaultIfEmpty()
                              where g != null
                              select bmRmEqualBatchSeq).ToList();

      if (bmRmBatchseqNu.Count > 0)
      {
        foreach (var equalBatchSeqNu in bmRmBatchseqNu)
        {
            //SCP#483217 : FW - Validation Report for MAY 15 Per 01
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Batch Number-Sequence Number with in Batch",
                                                                          Convert.ToString(equalBatchSeqNu.Key.BatchSequenceNumber).PadLeft(5, '0') + "-" +
                                                                          Convert.ToString(equalBatchSeqNu.Key.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.DuplicateBatchNoSequenceNo,
                                                                          ErrorStatus.X, equalBatchSeqNu.Key.SourceCodeId, equalBatchSeqNu.Key.BatchSequenceNumber, equalBatchSeqNu.Key.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // Validate Batch Number and Record Seq number in BM and PB of Invoice
      //SCP#483217 : FW - Validation Report for MAY 15 Per 01
      var bmPbBatchseqNu = (from bmRecord in invoice.BillingMemoRecord
                            join pbRecord in invoice.CouponDataRecord on
                              new { bmRecord.BatchSequenceNumber, bmRecord.RecordSequenceWithinBatch } equals
                              new { pbRecord.BatchSequenceNumber, pbRecord.RecordSequenceWithinBatch }
                            group pbRecord by new { bmRecord.BillingMemoNumber, pbRecord.BatchSequenceNumber, pbRecord.RecordSequenceWithinBatch, pbRecord.SourceCodeId }
                              into bmPbEqualBatchSeq
                              from g in bmPbEqualBatchSeq.DefaultIfEmpty()
                              where g != null
                              select bmPbEqualBatchSeq).ToList();

      if (bmPbBatchseqNu.Count > 0)
      {
        foreach (var equalBatchSeqNu in bmPbBatchseqNu)
        {
           //SCP#483217 : FW - Validation Report for MAY 15 Per 01
           var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Batch Number-Sequence Number with in Batch",
                                                                          Convert.ToString(equalBatchSeqNu.Key.BatchSequenceNumber).PadLeft(5, '0') + "-" +
                                                                          Convert.ToString(equalBatchSeqNu.Key.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.DuplicateBatchNoSequenceNo,
                                                                          ErrorStatus.X, equalBatchSeqNu.Key.SourceCodeId, equalBatchSeqNu.Key.BatchSequenceNumber, equalBatchSeqNu.Key.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // Validate Batch Number and Record Seq number in RM and PB of Invoice
      //SCP#483217 : FW - Validation Report for MAY 15 Per 01
      var rmPbBatchseqNu = (from rmRecord in invoice.RejectionMemoRecord
                            join pbRecord in invoice.CouponDataRecord on
                              new { rmRecord.BatchSequenceNumber, rmRecord.RecordSequenceWithinBatch } equals
                              new { pbRecord.BatchSequenceNumber, pbRecord.RecordSequenceWithinBatch }
                            group pbRecord by new { rmRecord.RejectionMemoNumber, pbRecord.BatchSequenceNumber, pbRecord.RecordSequenceWithinBatch, pbRecord.SourceCodeId }
                              into bmPbEqualBatchSeq
                              from g in bmPbEqualBatchSeq.DefaultIfEmpty()
                              where g != null
                              select bmPbEqualBatchSeq).ToList();

      if (rmPbBatchseqNu.Count > 0)
      {
        foreach (var equalBatchSeqNu in rmPbBatchseqNu)
        {
           //SCP#483217 : FW - Validation Report for MAY 15 Per 01  
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Batch Number-Sequence Number with in Batch",
                                                                          Convert.ToString(equalBatchSeqNu.Key.BatchSequenceNumber).PadLeft(5, '0') + "-" +
                                                                          Convert.ToString(equalBatchSeqNu.Key.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.DuplicateBatchNoSequenceNo,
                                                                          ErrorStatus.X, equalBatchSeqNu.Key.SourceCodeId, equalBatchSeqNu.Key.BatchSequenceNumber, equalBatchSeqNu.Key.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }


      // Coupon Record Validations
      var processedBatchNumbers = new List<int>();

      // Validate Duplicate for batch Number-Sequence with in batch pair
      var couponRecordComparer = new CouponRecordComparer();
      if (invoice.CouponDataRecord.Distinct(couponRecordComparer).Count() != invoice.CouponDataRecord.Count())
      {
        foreach (var equalCoupon in couponRecordComparer.EqualCoupons)
        {
          //SCP#483217 : FW - Validation Report for MAY 15 Per 01
          var validationExceptionDetail = CreateValidationExceptionDetail(equalCoupon.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Number-Sequence Number with in Batch", Convert.ToString(equalCoupon.BatchSequenceNumber).PadLeft(5, '0') + "-" + Convert.ToString(equalCoupon.RecordSequenceWithinBatch).PadLeft(5, '0'), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.DuplicateBatchNoSequenceNo, ErrorStatus.X, equalCoupon.SourceCodeId, equalCoupon.BatchSequenceNumber, equalCoupon.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      stopWatch.Stop();
      Logger.InfoFormat("Checking for duplicate coupon batch and sequence numbers - time required: [{0}]", stopWatch.Elapsed);

      var clearingHouse = ReferenceManager.GetClearingHouseFromSMI(invoice.SettlementMethodId);

      // Update clearing house of invoice
      // invoice.ClearingHouse = clearingHouse;
      // Get Min Max acceptable amount for Coupon.
      MaxAcceptableAmount maxCouponAcceptableAmount = null;
      if (!string.IsNullOrEmpty(clearingHouse.Trim()))
      {
        // If invoice contains Min Max Acceptable amounts then don't make DB call.
        maxCouponAcceptableAmount = GetMaxAcceptableAmount(invoice, clearingHouse, TransactionType.Coupon);
      }

      Logger.Info("Fetched the Clearing House & Max Acceptable Amount.");

      var airlineFlightDesignator = new Dictionary<string, bool>();

      Logger.Info("Validate Batch Number sequence order.");
     
      foreach (PrimeCoupon couponRecord in invoice.CouponDataRecord)
      {
        couponRecord.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.Validated;

        // Validate Batch Number sequence order
        if (processedBatchNumbers.Contains(couponRecord.BatchSequenceNumber) && processedBatchNumbers.Last() != couponRecord.BatchSequenceNumber)
        {

          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number", Convert.ToString(couponRecord.BatchSequenceNumber),
              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.DuplicateBatchNo, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (!processedBatchNumbers.Contains(couponRecord.BatchSequenceNumber))
        {
          processedBatchNumbers.Add(couponRecord.BatchSequenceNumber);
        }



        isValid = ValidateParsedCouponRecord(couponRecord, exceptionDetailsList, invoice, fileName,
                                              airlineFlightDesignator, issuingAirline, fileSubmissionDate, exchangeRate, maxCouponAcceptableAmount, SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod);
        isTransactionRecordsExistsInInvoice = true;
        isPrimeTransactionExistsInInvoice = true;
      }

      Logger.Info("Set Ignore Validation Flags.");

      // Get billing and billed member passenger configuration.
      var passengerConfigBilledMember = MemberManager.GetPassengerConfiguration(invoice.BilledMemberId);
      var passengerConfigBillingMember = MemberManager.GetPassengerConfiguration(invoice.BillingMemberId);

      var isXmlfileType = (invoice.SubmissionMethod == SubmissionMethod.IsXml) ? true : false;

      // Get IgnoreValidationInMigrationPeriod for Rm1, RM2, RM3 and Coupon and use it in RM and RM COupon validation.
      if (passengerConfigBilledMember != null)
      {
        invoice.IgnoreValidationInMigrationPeriodRm1 = IgnoreValidationInMigrationPeriod(invoice, TransactionType.RejectionMemo1, isXmlfileType, passengerConfigBilledMember);
        invoice.IgnoreValidationInMigrationPeriodCoupon = IgnoreValidationInMigrationPeriod(invoice, TransactionType.Coupon, isXmlfileType, passengerConfigBilledMember);
        invoice.IgnoreValidationInMigrationPeriodFormD = IgnoreValidationInMigrationPeriod(invoice, TransactionType.SamplingFormD, isXmlfileType, passengerConfigBilledMember);
        invoice.IgnoreValidationInMigrationPeriodFormF = IgnoreValidationInMigrationPeriod(invoice, TransactionType.SamplingFormF, isXmlfileType, passengerConfigBilledMember);
        invoice.IgnoreValidationInMigrationPeriodFormAb = IgnoreValidationInMigrationPeriod(invoice, TransactionType.SamplingFormAB, isXmlfileType, passengerConfigBilledMember);
      }
      else
      {
        // If Passenger Configuration is null Ignore Validation will be false for all the record types.
        invoice.IgnoreValidationInMigrationPeriodRm1 = invoice.IgnoreValidationInMigrationPeriodCoupon = false;
      }
      
      Logger.Info("Member Migration Check.");

      // Member Migration validations
      if (isPrimeTransactionExistsInInvoice)
      {
        if (invoice.InvoiceTypeId == (int)InvoiceType.CreditNote)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Header", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvoiceShouldNotHaveCreditNoteTransactions, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.BillingCode == (int)BillingCode.NonSampling)
        {
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.Coupon, false, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForPrimeIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.Coupon, true, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForPrimeIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        else if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
        {
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormAB, false, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForProvisionalBillingIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormAB, true, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForProvisionalBillingIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      // RM Validations 
      processedBatchNumbers.Clear();

      Logger.Info("Validate Duplicate for batch Number-Sequence with in batch pair.");

      // Validate Duplicate for batch Number-Sequence with in batch pair
      var memoRecordBaseComparer = new MemoComparer();
      if (invoice.RejectionMemoRecord.Distinct(memoRecordBaseComparer).Count() != invoice.RejectionMemoRecord.Count())
      {
        foreach (var equalCoupon in memoRecordBaseComparer.EqualCoupons)
        {
          //SCP#483217 : FW - Validation Report for MAY 15 Per 01
          var rejectionMemo = (RejectionMemo)equalCoupon;
          var validationExceptionDetail = CreateValidationExceptionDetail(equalCoupon.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Number/Sequence Number with in Batch", Convert.ToString(equalCoupon.BatchSequenceNumber).PadLeft(5, '0') + "-" + Convert.ToString(equalCoupon.RecordSequenceWithinBatch).PadLeft(5, '0'), invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.DuplicateBatchNoSequenceNo, ErrorStatus.X, rejectionMemo.SourceCodeId, equalCoupon.BatchSequenceNumber, equalCoupon.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (invoice.RejectionMemoRecord.Count > 0)
      {
        List<MinAcceptableAmount> rejectionMinAmounts = null;

        List<MaxAcceptableAmount> rejectionMaxAmounts = null;

        var transactionDate = GetTransactionDate(invoice);
        
        Logger.Info("Validate RM Min Max Amount.");

        if (invoice.ValidMinAcceptableAmounts == null)
        {
          rejectionMinAmounts =
            MinAcceptableAmountRepository.Get(
                                            record => record.IsActive && (record.TransactionTypeId == (int)TransactionType.RejectionMemo1 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo3 ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormF ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormXF)
                                            && record.ClearingHouse == clearingHouse && record.EffectiveFromPeriod <= transactionDate && record.EffectiveToPeriod >= transactionDate).ToList();
        }
        else
        {
          rejectionMinAmounts = invoice.ValidMinAcceptableAmounts.Where(
                                        record => record.IsActive && (record.TransactionTypeId == (int)TransactionType.RejectionMemo1
                                            || record.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo3
                                            || record.TransactionTypeId == (int)TransactionType.SamplingFormF ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormXF)
                                            && record.ClearingHouse == clearingHouse && record.EffectiveFromPeriod <= transactionDate && record.EffectiveToPeriod >= transactionDate).ToList();
        }


        if (invoice.ValidMaxAcceptableAmounts == null)
        {
          rejectionMaxAmounts =
            MaxAcceptableAmountRepository.Get(
                                            record => record.IsActive && (record.TransactionTypeId == (int)TransactionType.RejectionMemo1 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo3 ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormF ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormXF)
                                            && record.ClearingHouse == clearingHouse && record.EffectiveFromPeriod <= transactionDate && record.EffectiveToPeriod >= transactionDate).ToList();
        }
        else
        {
          rejectionMaxAmounts = invoice.ValidMaxAcceptableAmounts.Where(
                                        record => record.IsActive && (record.TransactionTypeId == (int)TransactionType.RejectionMemo1
                                            || record.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            record.TransactionTypeId == (int)TransactionType.RejectionMemo3
                                            || record.TransactionTypeId == (int)TransactionType.SamplingFormF ||
                                            record.TransactionTypeId == (int)TransactionType.SamplingFormXF)
                                            && record.ClearingHouse == clearingHouse && record.EffectiveFromPeriod <= transactionDate && record.EffectiveToPeriod >= transactionDate).ToList();
        }

        double samplingConstant = 0;

        Logger.Info("Validate RM Sampling Constant & Batch Number sequence order.");

       //var rejectionMemoNumbers = new StringBuilder();
        foreach (RejectionMemo rejectionMemoRecord in invoice.RejectionMemoRecord)
        {
          //rejectionMemoNumbers.Append(rejectionMemoRecord.RejectionMemoNumber);
          //rejectionMemoNumbers.Append(",");
          if (samplingConstant != 0 && samplingConstant != rejectionMemoRecord.SamplingConstant)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sampling Constant", Convert.ToString(rejectionMemoRecord.SamplingConstant),
              invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.SamplingConstantShouldBeSameForAllRejections, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          samplingConstant = rejectionMemoRecord.SamplingConstant;
          rejectionMemoRecord.TransactionStatus = Model.Common.TransactionStatus.Validated;
          //Validate Batch Number sequence order
          if (processedBatchNumbers.Contains(rejectionMemoRecord.BatchSequenceNumber) && processedBatchNumbers.Last() != rejectionMemoRecord.BatchSequenceNumber)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number", Convert.ToString(rejectionMemoRecord.BatchSequenceNumber),
              invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.DuplicateBatchNo, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (!processedBatchNumbers.Contains(rejectionMemoRecord.BatchSequenceNumber))
          {
            processedBatchNumbers.Add(rejectionMemoRecord.BatchSequenceNumber);
          }
          
          isValid = ValidateParsedRejectionMemoRecord(rejectionMemoRecord, exceptionDetailsList, invoice, fileName,
                                                      issuingAirline, fileSubmissionDate, exchangeRate, rejectionMinAmounts, rejectionMaxAmounts, maxCouponAcceptableAmount, billingPeriod, passengerConfigBilledMember);
        
         }

       
        // Perform duplicate check on all the RM records in invoice.
        //IsDuplicateRMs(exceptionDetailsList, invoice, fileName, fileSubmissionDate, rejectionMemoNumbers.ToString());

        isTransactionRecordsExistsInInvoice = true;
        isRejectionMemoTransactionExistsInInvoice = true;
      }

      if (isRejectionMemoTransactionExistsInInvoice)
      {
        if (invoice.InvoiceTypeId == (int)InvoiceType.CreditNote)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Header", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvoiceShouldNotHaveCreditNoteTransactions, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.BillingCode == (int)BillingCode.NonSampling)
        {
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.RejectionMemo1, false, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForRejectionIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.RejectionMemo1, true, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForRejectionIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        else if (invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormF, false, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForSamplingXfIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.SamplingFormF, true, passengerConfigBillingMember) == false))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForSamplingXfIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //BM Record Validations
      processedBatchNumbers.Clear();

      Logger.Info("Validate BM - Duplicate for batch Number-Sequence with in batch pair.");

      //Validate Duplicate for batch Number-Sequence with in batch pair
      memoRecordBaseComparer = new MemoComparer();
      if (invoice.BillingMemoRecord.Distinct(memoRecordBaseComparer).Count() != invoice.BillingMemoRecord.Count())
      {
        foreach (var equalCoupon in memoRecordBaseComparer.EqualCoupons)
        {
          //SCP#483217 : FW - Validation Report for MAY 15 Per 01
          var billingMemo = (BillingMemo) equalCoupon;
          var validationExceptionDetail = CreateValidationExceptionDetail(equalCoupon.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Number/Sequence Number with in Batch", Convert.ToString(equalCoupon.BatchSequenceNumber).PadLeft(5, '0') + "-" + Convert.ToString(equalCoupon.RecordSequenceWithinBatch).PadLeft(5, '0'), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.DuplicateBatchNoSequenceNo, ErrorStatus.X, billingMemo.SourceCodeId, equalCoupon.BatchSequenceNumber, equalCoupon.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      foreach (BillingMemo billingMemoRecord in invoice.BillingMemoRecord)
      {
        billingMemoRecord.TransactionStatus = Model.Common.TransactionStatus.Validated;
        //Validate Batch Number sequence order
        if (processedBatchNumbers.Contains(billingMemoRecord.BatchSequenceNumber) && processedBatchNumbers.Last() != billingMemoRecord.BatchSequenceNumber)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number", Convert.ToString(billingMemoRecord.BatchSequenceNumber),
              invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.DuplicateBatchNo, ErrorStatus.X, billingMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (!processedBatchNumbers.Contains(billingMemoRecord.BatchSequenceNumber))
        {
          processedBatchNumbers.Add(billingMemoRecord.BatchSequenceNumber);
        }
       
        isValid = ValidateParsedBillingMemoRecord(billingMemoRecord, exceptionDetailsList, invoice, fileName, airlineFlightDesignator, issuingAirline, fileSubmissionDate);
        
        isTransactionRecordsExistsInInvoice = true;
        isBillingMemoTransactionExistsInInvoice = true;
      }
     
      if (isBillingMemoTransactionExistsInInvoice)
      {
        if (invoice.InvoiceTypeId == (int)InvoiceType.CreditNote)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Header", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvoiceShouldNotHaveCreditNoteTransactions, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Member migration check
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.BillingMemo, false, passengerConfigBillingMember) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForBillingMemoIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.BillingMemo, true, passengerConfigBillingMember) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForBillingMemoIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //CM Record Validations
      processedBatchNumbers.Clear();

      Logger.Info("Validate CM - Duplicate for batch Number-Sequence with in batch pair.");

      //Validate Duplicate for batch Number-Sequence with in batch pair
      memoRecordBaseComparer = new MemoComparer();
      if (invoice.CreditMemoRecord.Distinct(memoRecordBaseComparer).Count() != invoice.CreditMemoRecord.Count())
      {
        foreach (var equalCoupon in memoRecordBaseComparer.EqualCoupons)
        {
          //SCP#483217 : FW - Validation Report for MAY 15 Per 01
          var creditMemo = (CreditMemo)equalCoupon;
          var validationExceptionDetail = CreateValidationExceptionDetail(equalCoupon.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Number/Sequence Number with in Batch", Convert.ToString(equalCoupon.BatchSequenceNumber).PadLeft(5, '0') + "-" + Convert.ToString(equalCoupon.RecordSequenceWithinBatch).PadLeft(5, '0'), invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.DuplicateBatchNoSequenceNo, ErrorStatus.X, creditMemo.SourceCodeId, equalCoupon.BatchSequenceNumber, equalCoupon.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (invoice.CreditMemoRecord.Count > 0)
      {
        MaxAcceptableAmount maxAcceptableAmount = null;
        if (!string.IsNullOrEmpty(clearingHouse.Trim()))
        {
          // Get Min Max acceptable amount for CM.
          maxAcceptableAmount = GetMaxAcceptableAmount(invoice, clearingHouse, TransactionType.CreditMemo);
        }

        foreach (CreditMemo creditMemoRecord in invoice.CreditMemoRecord)
        {
          creditMemoRecord.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.Validated;
          //Validate Batch Number sequence order
          if (processedBatchNumbers.Contains(creditMemoRecord.BatchSequenceNumber) &&
              processedBatchNumbers.Last() != creditMemoRecord.BatchSequenceNumber)
          {

            var validationExceptionDetail = CreateValidationExceptionDetail(creditMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number", Convert.ToString(creditMemoRecord.BatchSequenceNumber),
              invoice, fileName, ErrorLevels.ErrorLevelCreditMemo, ErrorCodes.DuplicateBatchNo, ErrorStatus.X, creditMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;

          }
          else if (!processedBatchNumbers.Contains(creditMemoRecord.BatchSequenceNumber))
          {
            processedBatchNumbers.Add(creditMemoRecord.BatchSequenceNumber);
          }

          
          isValid = NonSamplingCreditNoteManager.ValidateParsedCreditMemoRecord(creditMemoRecord, exceptionDetailsList, invoice, fileName, airlineFlightDesignator, issuingAirline,
                                                                                fileSubmissionDate, exchangeRate, maxAcceptableAmount);
       
        }

       
        isTransactionRecordsExistsInInvoice = true;
        isCreditMemoTransactionExistsInInvoice = true;
      }

      if (isCreditMemoTransactionExistsInInvoice)
      {
        if (invoice.BillingCode != (int)BillingCode.NonSampling || invoice.InvoiceTypeId == (int)InvoiceType.Invoice)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Header", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.CreditNotShouldNotHaveOtherTransactions, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //Member migration check
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && (IsMemberMigratedForTransaction(invoice, TransactionType.CreditMemo, false, passengerConfigBillingMember) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForCreditMemoIsIdec, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && (IsMemberMigratedForTransaction(invoice, TransactionType.CreditMemo, true, passengerConfigBillingMember) == false))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Member", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MemberIsNotMigratedForCreditMemoIsXml, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      var validationManager = Ioc.Resolve<IValidationErrorManager>(typeof(IValidationErrorManager));
      if (invoice.BillingCode == (int)BillingCode.NonSampling || invoice.BillingCode == (int)BillingCode.SamplingFormAB || invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        if (isTransactionRecordsExistsInInvoice)
        {
          Logger.Info("Validation of Source code total record with sum of transactions records.");

          //Validation of Source code total record with sum of transactions records.);
          if (ValidateParsedSourceCodeTotal(invoice, fileSubmissionDate, exceptionDetailsList, fileName) == false)
          {
            isValid = false;
          }

          Logger.Info("Validation of invoice total record with sum of sum of source code records.");

          //Validation of invoice total record with sum of sum of source code records.
          if (ValidateParsedInvoiceTotal(invoice, fileSubmissionDate, exceptionDetailsList, fileName) == false)
          {
            isValid = false;
          }
          







        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Header", String.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.MandatoryTransactionInInvoice, ErrorStatus.X, invoice.BatchSequenceNumber, invoice.RecordSequenceWithinBatch);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        // Update invoice status as ValidationCompleted, invoice status will be set to Non-Correctable/Correctable/Ready For Billing in SP.
                 
        invoice.isValidationExceptionSummary = validationManager.GetIsSummary(invoice, exceptionDetailsList, fileName, fileSubmissionDate);
        invoice.ValidationExceptionSummary = validationManager.GetIsSummaryForValidationErrorCorrection(invoice,
                                                                                                    exceptionDetailsList.ToList());
        Logger.Info("Update Non Sampling Parsed Invoice Status");
        UpdateNonSamplingParsedInvoiceStatus(invoice, fileName, fileSubmissionDate, exceptionDetailsList);

        if (invoice.isValidationExceptionSummary != null)
        {
          invoice.isValidationExceptionSummary.InvoiceStatus = ((int)invoice.InvoiceStatus).ToString();
        }

      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormDE)
      {
        var validationSFormDeManager = Ioc.Resolve<IValidationSFormDEManager>(typeof(IValidationSFormDEManager));
        validationSFormDeManager.ValidateParsedSamplingFormD(invoice, exceptionDetailsList, fileName, fileSubmissionDate, issuingAirline);
        validationSFormDeManager.ValidateParsedSamplingFormE(invoice, exceptionDetailsList, fileName, fileSubmissionDate);

        invoice.isValidationExceptionSummary = validationManager.GetIsSummary(invoice, exceptionDetailsList, fileName, fileSubmissionDate);
        invoice.ValidationExceptionSummary = validationManager.GetIsSummaryForValidationErrorCorrection(invoice,
                                                                                                   exceptionDetailsList.ToList());

        if (invoice.BillingCode == (int)BillingCode.SamplingFormDE && invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml)
        {
          invoice.InvoiceTotalRecord = null;
        }

        Logger.Info("Update Sampling Parsed Invoice Status.");

        // Update invoice status as ValidationCompleted, invoice status will be set to Non-Correctable/Correctable/Ready For Billing in SP.
        UpdateSamplingParsedInvoiceStatus(invoice, fileName, fileSubmissionDate, exceptionDetailsList);

        if (invoice.isValidationExceptionSummary != null)
        {
          invoice.isValidationExceptionSummary.InvoiceStatus = ((int)invoice.InvoiceStatus).ToString();
        }
      }
        //Clear Dictionery obects(Used for performence improvement) populated during validation of invoice  
        IsYourInvoiceNumberPresent.Clear();
        IsBilledMemberMigration.Clear();

      Logger.Info("Validate Parsed Invoice Completed.");

      return isValid;
    }

    /// <summary>
    /// Validate Invoice details and batch record sequence number. This change has implemented based on SCP#85837
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileRecordSequenceNumber"></param>
    /// <returns></returns>
    public bool ValidateParsedInvoice(PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, IDictionary<string, bool> issuingAirline, Dictionary<int, Dictionary<Guid, int>> fileRecordSequenceNumber)
    {
      
      foreach (var batchNumber in fileRecordSequenceNumber.Keys)
      {
        Dictionary<Guid, int> temp = fileRecordSequenceNumber[batchNumber];
        int expected = temp.FirstOrDefault().Value;
        foreach (var guid in temp.Keys)
        {
          if (expected != temp[guid])
          {
            if (invoice.CouponDataRecord.Count(a => a.Id == guid) > 0)
            {
              PrimeCoupon record = invoice.CouponDataRecord.FirstOrDefault(a => a.Id == guid);
              //Add Error
              var validationExceptionDetail = CreateValidationExceptionDetail(record.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Record Sequence Within Batch",
                                                                        Convert.ToString(
                                                                            record.RecordSequenceWithinBatch),
                                                                        invoice, fileName,
                                                                        ErrorLevels.ErrorLevelCoupon,
                                                                        ErrorCodes.InvalidRecordSequenceNumberOrder,
                                                                        ErrorStatus.X, record);
              validationExceptionDetail.ErrorDescription = string.Format("{0} {1}",
                                                                   validationExceptionDetail.
                                                                       ErrorDescription, expected);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
            else if (invoice.RejectionMemoRecord.Count(a => a.Id == guid) > 0)
            {
              RejectionMemo record = invoice.RejectionMemoRecord.FirstOrDefault(a => a.Id == guid);
              var validationExceptionDetail = CreateValidationExceptionDetail(record.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Record Sequence Within Batch",
                                                                          Convert.ToString(
                                                                              record.RecordSequenceWithinBatch),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelRejectionMemo,
                                                                          ErrorCodes.InvalidRecordSequenceNumberOrder,
                                                                          ErrorStatus.X, record);
              validationExceptionDetail.ErrorDescription = string.Format("{0} {1}",
                                                                  validationExceptionDetail.
                                                                      ErrorDescription, expected);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
            else if (invoice.BillingMemoRecord.Count(a => a.Id == guid) > 0)
            {
              BillingMemo record = invoice.BillingMemoRecord.FirstOrDefault(a => a.Id == guid);
              //Add Error
              var validationExceptionDetail = CreateValidationExceptionDetail(record.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Record Sequence Within Batch",
                                                                        Convert.ToString(
                                                                            record.RecordSequenceWithinBatch),
                                                                        invoice, fileName,
                                                                        ErrorLevels.ErrorLevelBillingMemo,
                                                                        ErrorCodes.InvalidRecordSequenceNumberOrder,
                                                                        ErrorStatus.X, record);
              validationExceptionDetail.ErrorDescription = string.Format("{0} {1}",
                                                                         validationExceptionDetail.
                                                                             ErrorDescription, expected);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
            else if (invoice.CreditMemoRecord.Count(a => a.Id == guid) > 0)
            {
              CreditMemo record = invoice.CreditMemoRecord.FirstOrDefault(a => a.Id == guid);
              //Add Error
              var validationExceptionDetail = CreateValidationExceptionDetail(record.Id.Value(),
                                                                         exceptionDetailsList.Count() + 1,
                                                                         fileSubmissionDate,
                                                                         "Record Sequence Within Batch",
                                                                         Convert.ToString(
                                                                             record.RecordSequenceWithinBatch),
                                                                         invoice, fileName,
                                                                         ErrorLevels.ErrorLevelCreditMemo,
                                                                         ErrorCodes.InvalidRecordSequenceNumberOrder,
                                                                         ErrorStatus.X, record);

              validationExceptionDetail.ErrorDescription = string.Format("{0} {1}",
                                                                         validationExceptionDetail.
                                                                             ErrorDescription, expected);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
          }
          expected++;
        }
      }

     

      return ValidateParsedInvoice(invoice, exceptionDetailsList, fileName,
                                   fileSubmissionDate, issuingAirline);
    }


    /// <summary>
    /// Validates the parsed billing memo record.
    /// </summary>
    /// <param name="billingMemo">The billing memo record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice"></param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    bool ValidateParsedBillingMemoRecord(BillingMemo billingMemo, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> airlineFlightDesignator, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
        var isValid = true;

        var billingMemoNumber = string.Empty;

        if (billingMemo.BillingMemoNumber != null)
        {
            billingMemoNumber = billingMemo.BillingMemoNumber;
        }

        PaxInvoice yourInvoice = null;
        //SCP122624:
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml)
        {
            DateTime yourInvoiceBillingDate;
            //To avoid converting year 30 into year 1930
            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.Calendar.TwoDigitYearMax = 2099;
            var yourInvoiceDateString = string.Format("{2}{1}{0}",
                                           Convert.ToString(billingMemo.YourInvoiceBillingPeriod).PadLeft(2, '0'),
                                           Convert.ToString(billingMemo.YourInvoiceBillingMonth).PadLeft(2, '0'),
                                           Convert.ToString(billingMemo.YourInvoiceBillingYear).PadLeft(4, '0'));
            var yourInvoiceDateStringErr = yourInvoiceDateString.Substring(2, yourInvoiceDateString.Length - 2);
            if ((!String.IsNullOrEmpty(yourInvoiceDateString) && (Convert.ToInt32(yourInvoiceDateString) != 0) && string.IsNullOrWhiteSpace(billingMemo.YourInvoiceNumber)) || (Convert.ToInt32(yourInvoiceDateString) == 0 && !string.IsNullOrWhiteSpace(billingMemo.YourInvoiceNumber)))
            {
                var validation_ExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Number", billingMemo.YourInvoiceNumber, invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.MandatoryYourInvoiceNumberAndYourBillingDate, ErrorStatus.X, billingMemo);
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
                        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Your Invoice Billing Date",
                                                                                        yourInvoiceDateStringErr,
                                                                                        invoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelBillingMemo,
                                                                                        ErrorCodes.InvalidYourInvoiceBillingDatePeriod,
                                                                                        ErrorStatus.X,
                                                                                        billingMemo);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
                else
                {
                    //Raise NonCorrectable error for invalid your invoice Date.
                    var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Your Invoice Billing Date",
                                                                                    yourInvoiceDateStringErr,
                                                                                    invoice,
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelBillingMemo,
                                                                                    ErrorCodes.InvalidYourInvoiceBillingDatePeriod,
                                                                                    ErrorStatus.X,
                                                                                    billingMemo);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
        }

        if (billingMemo.FimCouponNumber < 0 || billingMemo.FimCouponNumber > 4)
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Coupon Number", Convert.ToString(billingMemo.FimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.InvalidFimCouponNumber, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

        //FIM Number and FIM Coupon Number should be blank or both fields should be captured.
        if (billingMemo.FimCouponNumber != null && billingMemo.FimNumber != null)
        {
            if ((billingMemo.FimNumber != 0 || billingMemo.FimCouponNumber != 0) && billingMemo.FimNumber * billingMemo.FimCouponNumber == 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number and FIM Coupon Number", Convert.ToString(billingMemo.FimNumber) + "," + Convert.ToString(billingMemo.FimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.MandatoryFimNumberAndCouponNumber, ErrorStatus.X, billingMemo);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
        }

        //Duplicate check in current invoice - Billing memo number
        if (invoice.BillingMemoRecord.Where(memoRecord => memoRecord.BillingMemoNumber.ToUpper() == billingMemo.BillingMemoNumber.ToUpper()).Count() > 1)
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Memo Number", billingMemo.BillingMemoNumber, invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.DuplicateBillingMemoFound, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }
        //Validate Memo Number
        else if (IsDuplicateBillingMemo(null, billingMemo, false, invoice))
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Memo Number", billingMemo.BillingMemoNumber, invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.DuplicateBillingMemoFound, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

        //Validate Batch Sequence Number and Record Sequence Number
        if (billingMemo.RecordSequenceWithinBatch <= 0 || billingMemo.BatchSequenceNumber <= 0)
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number - Record Sequence Number", string.Format("{0}-{1}", billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch),
                                                       invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.InvalidBatchSequenceNoAndRecordNo, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

        var transactionTypeId = billingMemo.ReasonCode == "6A"
                                     ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                     : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
        //Validate SourceCode 
        if (!ReferenceManager.IsValidSourceCode(invoice, billingMemo.SourceCodeId, transactionTypeId))
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code", Convert.ToString(billingMemo.SourceCodeId),
                                                                invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.InvalidSourceCode, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

        if (billingMemo.ReasonCode != null)
        {

          //validate reason code 
          if (!ReferenceManager.IsValidReasonCode(invoice, billingMemo.ReasonCode, transactionTypeId))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Reason Code",
                                                                            billingMemo.ReasonCode, invoice, fileName,
                                                                            ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidReasonCode, ErrorStatus.X,
                                                                            billingMemo, true);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          var isCouponBreakdownMandatory = ReasonCodeRepository.GetCount(
            reasonCode =>
            reasonCode.Code.ToUpper() == billingMemo.ReasonCode.ToUpper() &&
            reasonCode.TransactionTypeId == (int) TransactionType.BillingMemo
            && reasonCode.CouponAwbBreakdownMandatory) > 0;

          if (isCouponBreakdownMandatory)
          {
            if (billingMemo.CouponBreakdownRecord.Count == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate, "Billing Memo Record",
                                                                              Convert.ToString(billingMemo.ReasonCode),
                                                                              invoice, fileName,
                                                                              ErrorLevels.ErrorLevelBillingMemo,
                                                                              ErrorCodes.MandatoryCouponBreakdownRecord,
                                                                              ErrorStatus.X, billingMemo);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }


          if (billingMemo.ReasonCode.ToUpper() == ReasonCode6A || billingMemo.ReasonCode.ToUpper() == ReasonCode6B)
          {
            //if (billingMemo.YourInvoiceNumber != null && !string.IsNullOrEmpty(billingMemo.YourInvoiceNumber.Trim()))
            //{
            //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1,
            //                              fileSubmissionDate,"Your Invoice Number",Convert.ToString(billingMemo.YourInvoiceNumber),
            //                              invoice,fileName,ErrorLevels.ErrorLevelBillingMemo,ErrorCodes.InvalidYourInvoiceNumberForReasonCode6A6B,
            //                              ErrorStatus.C,billingMemo);
            //  exceptionDetailsList.Add(validationExceptionDetail);
            //  isValid = false;
            //}

            // Correspondence Ref Number should be populated for Reason Code “6A” or “6B”.
            if (billingMemo.CorrespondenceRefNumber == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Correspondence Ref Number",
                                                                              Convert.ToString(
                                                                                billingMemo.CorrespondenceRefNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelBillingMemo,
                                                                              ErrorCodes.
                                                                                InvalidCorrespondenceRefNumber1,
                                                                              ErrorStatus.C, billingMemo);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            else
            {
              var correspondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>(typeof (IPaxCorrespondenceManager));
              var lastCorrespondence =
                correspondenceManager.GetRecentCorrespondenceDetails(billingMemo.CorrespondenceRefNumber, true);

              if (lastCorrespondence != null)
              {
                // CMP#624 : 2.8-New Validation #8 - SMI Match Check for PAX/CGO 6A/6B BMs
                yourInvoice = InvoiceRepository.GetInvoiceHeader(lastCorrespondence.InvoiceId);
                if (!ValidateSmiAfterLinking(invoice.SettlementMethodId, yourInvoice.SettlementMethodId))
                {
                    /* CMP #624: ICH Rewrite-New SMI X 
                    * Description: Code Fixed regarding CMP 624: ISWEB -  BM saved without checking the SMI of lined rejection invoice.
                    * Showing SMI related specific error message. */
                    string errorCode = "";
                    if (yourInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                    {
                        /* Old invoice is X and new is not. FRS Reference: 2.14,  New Validation #7 */
                        errorCode = ErrorCodes.PaxNsStandaloneBmInvLinkCheckForSmiX;
                    }
                    else
                    {
                        /* New invoice is X but old is not. FRS Reference: 2.14,  New Validation #6 */
                        errorCode = ErrorCodes.PaxNSBmInvoiceLinkingCheckForSmiX;
                    }

                    var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Correspondence Ref Number",
                                                                              Convert.ToString(
                                                                                billingMemo.CorrespondenceRefNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelBillingMemo,
                                                                              errorCode,
                                                                              ErrorStatus.X, billingMemo);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                TransactionType transactionType;
                if (billingMemo.ReasonCode.ToUpper() == ReasonCode6A)
                {
                  if (
                    !(lastCorrespondence.FromMemberId == invoice.BilledMemberId &&
                      lastCorrespondence.ToMemberId == invoice.BillingMemberId &&
                      lastCorrespondence.AuthorityToBill &&
                      lastCorrespondence.CorrespondenceStatusId == (int) CorrespondenceStatus.Open))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                    exceptionDetailsList.Count
                                                                                      () + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Correspondence Ref Number",
                                                                                    Convert.ToString(
                                                                                      billingMemo.
                                                                                        CorrespondenceRefNumber),
                                                                                    invoice, fileName,
                                                                                    ErrorLevels.
                                                                                      ErrorLevelBillingMemo,
                                                                                    ErrorCodes.
                                                                                      BillingMemoReferenceCorrespondenceDoesNotExist,
                                                                                    ErrorStatus.C, billingMemo,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                  transactionType = TransactionType.PasNsBillingMemoDueToAuthorityToBill;
                  isValid = ValidateTimeLimit(billingMemo, invoice, exceptionDetailsList, fileSubmissionDate,
                                              fileName, lastCorrespondence, transactionType);
                }
                else if (billingMemo.ReasonCode.ToUpper() == ReasonCode6B)
                {
                  if (
                    !(lastCorrespondence.FromMemberId == invoice.BillingMemberId &&
                      lastCorrespondence.ToMemberId == invoice.BilledMemberId &&
                      lastCorrespondence.CorrespondenceStatusId == (int) CorrespondenceStatus.Expired))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                    exceptionDetailsList.Count
                                                                                      () + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Correspondence Ref Number",
                                                                                    Convert.ToString(
                                                                                      billingMemo.
                                                                                        CorrespondenceRefNumber),
                                                                                    invoice, fileName,
                                                                                    ErrorLevels.
                                                                                      ErrorLevelBillingMemo,
                                                                                    ErrorCodes.
                                                                                      BillingMemoReferenceCorrespondenceDoesNotExist,
                                                                                    ErrorStatus.C, billingMemo,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }

                  transactionType = TransactionType.PasNsBillingMemoDueToExpiry;
                  isValid = ValidateTimeLimit(billingMemo, invoice, exceptionDetailsList, fileSubmissionDate,
                                              fileName, lastCorrespondence, transactionType);
                }

                            //SCP219674 : InvalidAmountToBeSettled Validation
                            #region Old Code For Validatation of CorrespondenceAmounttobeSettled : To be remove 
                            /*if (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId) &&
                    invoice.BillingCurrencyId != null && lastCorrespondence.CurrencyId != null &&
                    invoice.ListingCurrencyId != null)
                {
                  double exchangeRate;

                  var toleranceValue = invoice.Tolerance.RoundingTolerance;

                  // As per Business Rule, In case of Correspondence billing memo, Correspondence currency should be same as Invoice's Billing curerncy i.e.
                  // If Billing Currency is 'CAD' and correspondence currency is anyhting other than 'CAD' then there has to be a validation error 

                  if (lastCorrespondence.CurrencyId == invoice.BillingCurrencyId)
                  {
                    exchangeRate = ReferenceManager.GetExchangeRate(invoice.ListingCurrencyId.Value,
                                                                    (BillingCurrency)
                                                                    lastCorrespondence.CurrencyId.Value,
                                                                    invoice.BillingYear, invoice.BillingMonth);
                  }
                  else
                  {
                    // Exchange rate should be retrived against billing currency; whereas below it is retreived in reverse manner. 
                    // Thus value of exchange rate retrived is exact reciprocal to the one retreived at Invoice level.
                    // Which results in failure of below validation
                    exchangeRate = ReferenceManager.GetExchangeRate(invoice.BillingCurrencyId.Value,
                                                                    (BillingCurrency)
                                                                    lastCorrespondence.CurrencyId.Value,
                                                                    invoice.BillingYear, invoice.BillingMonth);
                    Tolerance tolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax,
                                                                   invoice.BillingCurrencyId.Value, invoice,
                                                                   Constants.PaxDecimalPlaces);
                    toleranceValue = tolerance != null ? tolerance.RoundingTolerance : 0;
                  }


                  var amountToBeSettled = exchangeRate > 0
                                            ? lastCorrespondence.AmountToBeSettled*
                                              Convert.ToDecimal(exchangeRate)
                                            : lastCorrespondence.AmountToBeSettled;

                  if (invoice.Tolerance != null &&
                      !CompareUtil.Compare(billingMemo.NetAmountBilled, amountToBeSettled, toleranceValue,
                                           Constants.PaxDecimalPlaces))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                    exceptionDetailsList.Count
                                                                                      () + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Net Billed Amount",
                                                                                    Convert.ToString(
                                                                                      billingMemo.
                                                                                        NetAmountBilled),
                                                                                    invoice, fileName,
                                                                                    ErrorLevels.
                                                                                      ErrorLevelBillingMemo,
                                                                                    ErrorCodes.
                                                                                      InvalidAmountToBeSettled,
                                                                                    ErrorStatus.X, billingMemo,
                                                                                    false, null,
                                                                                    Convert.ToString(
                                                                                      amountToBeSettled));
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                            } */
                            #endregion
                            #region New Code for Validatation of CorrespondenceAmounttobeSettled
                            if (lastCorrespondence.CurrencyId != null)
                            {
                              decimal netBilledAmount = billingMemo.NetAmountBilled;
                              isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(invoice,
                                                                                                 ref netBilledAmount,
                                                                                                 lastCorrespondence.CurrencyId.Value,
                                                                                                 lastCorrespondence.AmountToBeSettled, yourInvoice);
                              if (!isValid)
                              {
                                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Net Billed Amount",
                                                                                                Convert.ToString(billingMemo.NetAmountBilled),
                                                                                                invoice, fileName,
                                                                                                ErrorLevels.ErrorLevelBillingMemo,
                                                                                                ErrorCodes.InvalidAmountToBeSettled,
                                                                                                ErrorStatus.X,
                                                                                                billingMemo,
                                                                                                false, null);
                                exceptionDetailsList.Add(validationExceptionDetail);
                }
              }

                          #endregion
                        }
              else
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(),
                                                                                exceptionDetailsList.Count() + 1,
                                                                                fileSubmissionDate,
                                                                                "Correspondence Ref Number",
                                                                                Convert.ToString(
                                                                                  billingMemo.
                                                                                    CorrespondenceRefNumber),
                                                                                invoice,
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelBillingMemo,
                                                                                ErrorCodes.
                                                                                  BillingMemoReferenceCorrespondenceDoesNotExist,
                                                                                ErrorStatus.C, billingMemo,
                                                                                islinkingError: true);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            } // end of else block related to if (billingMemo.CorrespondenceRefNumber == 0)
          }
          //end of if (billingMemo.ReasonCode.ToUpper() == ReasonCode6A || billingMemo.ReasonCode.ToUpper() == ReasonCode6B)

          // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
          if (!billingMemo.ReasonCode.ToUpper().Equals(ReasonCode6A) && !billingMemo.ReasonCode.ToUpper().Equals(ReasonCode6B) && billingMemo.CorrespondenceRefNumber >= 0)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate, "Correspondence Ref Number",
                                                                              Convert.ToString(billingMemo.CorrespondenceRefNumber),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                              ErrorCodes.CorrRefNumberCannotBeProvidedForNon6Aor6BbmReasonCode,
                                                                              ErrorStatus.X, billingMemo);

              validationExceptionDetail.ErrorDescription = string.Format(validationExceptionDetail.ErrorDescription, billingMemo.ReasonCode);

              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }

        }

      // Memo level VAT breakdown should not be provided when RM/BM/CM has coupon breakdown information.
        if (billingMemo.VatBreakdown.Count > 0 && billingMemo.CouponBreakdownRecord.Count > 0)
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1,
                                  fileSubmissionDate, "BM Vat Details", string.Empty, invoice,
                                  fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.VatPresentWhenCouponBreakdownExists,
                                  ErrorStatus.X, billingMemo);

            exceptionDetailsList.Add(validationExceptionDetail);

            isValid = false;
        }

        //Validate Vat Breakdowns 
        foreach (var billingMemoVat in billingMemo.VatBreakdown)
        {
            isValid = ValidateParsedVat(billingMemoVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelBillingMemoVat, fileSubmissionDate, billingMemo.BatchSequenceNumber, billingMemo.RecordSequenceWithinBatch, billingMemoNumber, billingMemo.SourceCodeId, false, true);
        }

        //Validate billing memo total 
        ValidateParsedBillingMemoTotals(billingMemo, exceptionDetailsList, invoice, fileName, fileSubmissionDate);

        MemoCouponBase previousBreakdownRecord = null;
        //Validate BillingMemoCouponBreakdownRecord 
        foreach (var billingMemoCouponBreakdownRecord in billingMemo.CouponBreakdownRecord)
        {
            if (previousBreakdownRecord != null && billingMemoCouponBreakdownRecord.SerialNo != previousBreakdownRecord.SerialNo + 1)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(billingMemoCouponBreakdownRecord.SerialNo),
                                                    invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, billingMemo, false, string.Format("{0}-{1}-{2}", billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
                exceptionDetailsList.Add(validationExceptionDetail);
            }
            else if (previousBreakdownRecord == null && billingMemoCouponBreakdownRecord.SerialNo != 1)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(billingMemoCouponBreakdownRecord.SerialNo),
                                         invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, billingMemo, false, string.Format("{0}-{1}-{2}", billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
                exceptionDetailsList.Add(validationExceptionDetail);
            }

            //validation for checkdigit
            if ((billingMemoCouponBreakdownRecord.CheckDigit < 0) || (billingMemoCouponBreakdownRecord.CheckDigit > 6 && billingMemoCouponBreakdownRecord.CheckDigit != 9))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "CheckDigit", Convert.ToString(billingMemoCouponBreakdownRecord.CheckDigit), invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidCheckDigit, ErrorStatus.C, billingMemo, false, string.Format("{0}-{1}-{2}", billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            previousBreakdownRecord = billingMemoCouponBreakdownRecord;

            isValid = ValidateParsedBMCouponBreakdownRecord(billingMemoCouponBreakdownRecord, exceptionDetailsList, billingMemo, invoice, fileName, yourInvoice, airlineFlightDesignator, issuingAirline, fileSubmissionDate);
        }

        // SCP ID : 72923 - BGEN_00007 - TG PAX file PIDECF-2172013010320130125200007.dat
        // Reason Remark Field should be Max 4000 Char

        if (billingMemo.ReasonRemarks != null)
        {
            if (billingMemo.ReasonRemarks.Length > MaxReasonRemarkCharLength)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                         fileSubmissionDate,
                                                                         "Reason Remarks",
                                                                         string.Empty,
                                                                         invoice,
                                                                         fileName,
                                                                         ErrorLevels.ErrorLevelBillingMemo,
                                                                         ErrorCodes.MaxReasonRemarkCharLength,
                                                                         ErrorStatus.X, billingMemo);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;

            }

        }

        // Update expiry date for purging.
        // billingMemo.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

        return isValid;
    }

    private bool ValidateTimeLimit(BillingMemo billingMemo, PaxInvoice invoice, IList<IsValidationExceptionDetail> exceptionDetailsList,
                            DateTime fileSubmissionDate, string fileName,
                            Correspondence lastCorrespondence, TransactionType transactionType)
    {
      var isValid = true;
      bool isOutSideTimeLimit;
      //if (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId))
      //{
      //  isOutSideTimeLimit = !(ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType, invoice.SettlementMethodId, lastCorrespondence.CorrespondenceDate));
      //}
      //else
      //{
      //  isOutSideTimeLimit = !(ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType, Convert.ToInt32(SMI.Bilateral), lastCorrespondence.CorrespondenceDate));
      //}

      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      isOutSideTimeLimit = lastCorrespondence.BMExpiryPeriod.HasValue
                             ? new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod) >
                               lastCorrespondence.BMExpiryPeriod
                             : (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
                                 ? (!(ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType,
                                                                                      invoice.SettlementMethodId,
                                                                                      lastCorrespondence.
                                                                                        CorrespondenceDate)))
                                 : (!(ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType,
                                                                                       Convert.ToInt32(SMI.Bilateral),
                                                                                       lastCorrespondence.
                                                                                         CorrespondenceDate)));



























      if (isOutSideTimeLimit)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Correspondence Ref Number",
                                                                        Convert.ToString(billingMemo.CorrespondenceRefNumber),
                                                                        invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                        ErrorCodes.TimeLimitExpiryForCorrespondence, ErrorStatus.C, billingMemo);

        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validates the parsed billing memo coupon breakdown record.
    /// </summary>
    /// <param name="billingMemoCouponBreakdownRecord">The billing memo coupon breakdown record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="parentMemo">The parent memo.</param>
    /// <param name="invoice"></param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private bool ValidateParsedBMCouponBreakdownRecord(BMCoupon billingMemoCouponBreakdownRecord,
                                                                IList<IsValidationExceptionDetail> exceptionDetailsList,
                                                                BillingMemo parentMemo, PaxInvoice invoice, string fileName, PaxInvoice yourInvoice, IDictionary<string, bool> airlineFlightDesignators, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
      var isValid = true;

      // Validate TicketDocOrFimNumber is greater than 0.
      if (billingMemoCouponBreakdownRecord.TicketDocOrFimNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Documnet or FIM Number", Convert.ToString(billingMemoCouponBreakdownRecord.TicketDocOrFimNumber), invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidTicketDocumnetOrFimNumber, ErrorStatus.X, parentMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(billingMemoCouponBreakdownRecord.TicketDocOrFimNumber).Length > 10)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Ticket Documnet or FIM Number",
                                                                        Convert.ToString(billingMemoCouponBreakdownRecord.TicketDocOrFimNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                        ErrorCodes.TicketFimDocumentNoGreaterThanTenNs,
                                                                        ErrorStatus.X,
                                                                        parentMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      var tktIsuingAirline = billingMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;
      //Flight date should be YYMMDD

      DateTime resultDate;
      if (billingMemoCouponBreakdownRecord.FlightDate != null)
      {
        if (!DateTime.TryParse(billingMemoCouponBreakdownRecord.FlightDate.Value.ToString(), out resultDate))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Date", billingMemoCouponBreakdownRecord.FlightDate.Value.ToString(),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidFlightDate, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if (billingMemoCouponBreakdownRecord.OriginalPmi != null && billingMemoCouponBreakdownRecord.AgreementIndicatorSupplied != null && !ValidateOriginalPmi(billingMemoCouponBreakdownRecord.OriginalPmi, billingMemoCouponBreakdownRecord.AgreementIndicatorSupplied))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Original PMI", string.Empty,
                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidOriginalPMI, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber <= 0 || billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber > 4)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Number", Convert.ToString(billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline,
                                                            billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //if (billingMemoCouponBreakdownRecord.AgreementIndicatorSupplied != null && !ValidateAgreementIndicatorSupplied(billingMemoCouponBreakdownRecord.AgreementIndicatorSupplied))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Agreement Indicator Supplied", billingMemoCouponBreakdownRecord.AgreementIndicatorSupplied,
      //                                                      invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidAgreementIndicatorSupplied, ErrorStatus.C, parentMemo, false, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber.ToString());
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      // Validate ticket issuing airline using local caching.
      isValid = ValidateTicketIssuingAirline(billingMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, parentMemo.SourceCodeId, invoice, fileName, isValid, ErrorLevels.ErrorLevelBillingMemoCoupon, issuingAirline, fileSubmissionDate);


      decimal expectedAmount = 0;

      //ISC amount

      expectedAmount = ConvertUtil.Round(Convert.ToDecimal(billingMemoCouponBreakdownRecord.IscPercent) * billingMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
      if (parentMemo.SourceCodeId != 94)
      {
        if (invoice.Tolerance != null && !CompareUtil.Compare(Convert.ToDecimal(billingMemoCouponBreakdownRecord.IscAmountBilled), expectedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "ISC Amount Billed",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.IscAmountBilled),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidIscPercentage,
                                                                          ErrorStatus.X,
                                                                          parentMemo,
                                                                          false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //UATP amount
      expectedAmount = ConvertUtil.Round(Convert.ToDecimal(billingMemoCouponBreakdownRecord.UatpPercent) * billingMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
      if (parentMemo.SourceCodeId != 94)
      {
        if (invoice.Tolerance != null && !CompareUtil.Compare(Convert.ToDecimal(billingMemoCouponBreakdownRecord.UatpAmountBilled), expectedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "UATP Amount Billed",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.UatpAmountBilled),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidUatpPercentage,
                                                                          ErrorStatus.X,
                                                                          parentMemo,
                                                                          false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //From airport of coupon and to airport of coupon should not be same.
      if (billingMemoCouponBreakdownRecord.FromAirportOfCoupon != null && billingMemoCouponBreakdownRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim()) && !string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()) &&
           String.Equals(billingMemoCouponBreakdownRecord.FromAirportOfCoupon.ToUpper(), billingMemoCouponBreakdownRecord.ToAirportOfCoupon.ToUpper()))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From airport", Convert.ToString(billingMemoCouponBreakdownRecord.FromAirportOfCoupon),
                                                               invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.FromAirportOfCouponAndToAirportOfCouponShouldNotBeSame, ErrorStatus.X, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Validate FromAirportOfCoupon 
      if (billingMemoCouponBreakdownRecord.FromAirportOfCoupon != null && !string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim()) && !IsValidCityAirportCode(billingMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", Convert.ToString(billingMemoCouponBreakdownRecord.FromAirportOfCoupon),
                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.FromAirportOfCouponIsInvalid, ErrorStatus.C, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate ToAirportOfCoupon 
      if (billingMemoCouponBreakdownRecord.ToAirportOfCoupon != null && !string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()) && !IsValidCityAirportCode(billingMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "To Airport", Convert.ToString(billingMemoCouponBreakdownRecord.ToAirportOfCoupon),
                                                           invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidToAirportCode, ErrorStatus.C, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Currency Adjustment Indicator
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        if (string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Currency Adjustment Indicator",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.X, parentMemo, true,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else if (billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator != null && !ReferenceManager.IsValidCurrencyCode(invoice, billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Currency Adjustment Indicator",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.X, parentMemo, true,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Validate Airline Flight Designator*
      if (billingMemoCouponBreakdownRecord.AirlineFlightDesignator != null && !string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.AirlineFlightDesignator.Trim()))
      {
        isValid = ValidateAirlineFlightDesignator(parentMemo, parentMemo.SourceCodeId, invoice, fileName, exceptionDetailsList, airlineFlightDesignators, isValid, billingMemoCouponBreakdownRecord.AirlineFlightDesignator, ErrorLevels.ErrorLevelBillingMemoCoupon, fileSubmissionDate, parentMemo.BillingMemoNumber, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        if ((!isValid) && (!parentMemo.TransactionStatus.Equals(Iata.IS.Model.Common.TransactionStatus.ErrorNonCorrectable)))
        {
          parentMemo.TransactionStatus = Iata.IS.Model.Common.TransactionStatus.ErrorCorrectable;
        }
      }

      //Validate Reference field 1 AND Field 2
      if (parentMemo.ReasonCode != null)
      {
        if (parentMemo.ReasonCode.ToUpper() == "8P")
        {
          if (billingMemoCouponBreakdownRecord.ReferenceField1 != null && (string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ReferenceField1.Trim()) || !ReferenceManager.IsValidRficCode(billingMemoCouponBreakdownRecord.ReferenceField1.Trim())))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 1",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.ReferenceField1),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidReferenceField1ForBMCoupon, ErrorStatus.X, parentMemo, true,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;

          }
          if (billingMemoCouponBreakdownRecord.ReferenceField2 != null && (string.IsNullOrEmpty(billingMemoCouponBreakdownRecord.ReferenceField2.Trim()) || !ReferenceManager.IsValidRfiscCode(billingMemoCouponBreakdownRecord.ReferenceField2.Trim(), billingMemoCouponBreakdownRecord.ReferenceField1.Trim())))
          {

            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 2",
                                                                          Convert.ToString(billingMemoCouponBreakdownRecord.ReferenceField2),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon,
                                                                          ErrorCodes.InvalidReferenceField2ForBMCoupon, ErrorStatus.C, parentMemo, true,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Validate Tax Breakdowns 
      foreach (var couponRecordTax in billingMemoCouponBreakdownRecord.TaxBreakdown)
      {
        isValid = ValidateParsedTax(couponRecordTax, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCouponTax, fileSubmissionDate,
                                    parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch,
                                    parentMemo.BillingMemoNumber, parentMemo.SourceCodeId, string.Format("{0}-{1}-{2}", tktIsuingAirline, billingMemoCouponBreakdownRecord.TicketDocOrFimNumber, billingMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
      }

      //Validate Vat Breakdowns 
      foreach (var couponRecordVat in billingMemoCouponBreakdownRecord.VatBreakdown)
      {
        isValid = ValidateParsedVat(couponRecordVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCouponVat, fileSubmissionDate,
                                    parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch,
                                    billingMemoCouponBreakdownRecord.TicketDocOrFimNumber.ToString(), parentMemo.SourceCodeId, false, true);
      }

      // TODO: To be released in Jan 2012
      // Validate reference data for source code 94.
      //if (parentMemo.SourceCodeId == 94 && (string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.ReferenceField1) || string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.ReferenceField2) || string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.ReferenceField3) || string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.ReferenceField4) || string.IsNullOrWhiteSpace(billingMemoCouponBreakdownRecord.ReferenceField5)))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Fields", string.Empty,
      //                                     invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.ReferenceDataNotProvidedForFrequentFlyerRelatedBillings, ErrorStatus.X);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      ValidateParsedBMCouponTotals(billingMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, invoice, fileName, fileSubmissionDate);

      return isValid;
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
    /// <returns></returns>
    private bool ValidateAirlineFlightDesignator(MemoBase parentMemo, int sourceCodeId, PaxInvoice invoice, string fileName, IList<IsValidationExceptionDetail> exceptionDetailsList, IDictionary<string, bool> airlineFlightDesignators, bool isValid, string airlineFlightDesignator, string errorLevel, DateTime fileSubmissionDate, string documentNumner, string linkedDocumentNumber)
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
                                             invoice, fileName, errorLevel, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, sourceCodeId, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, documentNumner, false, linkedDocumentNumber);
          exceptionDetailsList.Add(validationExceptionDetail);
          airlineFlightDesignators.Add(airlineFlightDesignator, false);
          isValid = false;
        }
      }
      else if (!airlineFlightDesignators[airlineFlightDesignator])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(parentMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(airlineFlightDesignator),
                                           invoice, fileName, errorLevel, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, sourceCodeId, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, documentNumner, false, linkedDocumentNumber);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Validates the coupon record db.
    /// </summary>
    /// <param name="couponRecord">The coupon record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice"></param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    private bool ValidateParsedCouponRecord(PrimeCoupon couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> airlineFlightDesignators, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate, ExchangeRate exchangeRate, MaxAcceptableAmount maxAcceptableAmount, bool ignoreValidationOnMigrationPeriod)
    {
      var isValid = true;

      TransactionType transType = 0;

      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        transType = TransactionType.Coupon;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
      {
        transType = TransactionType.SamplingFormAB;
      }

      // Validate TicketDocOrFimNumber is greater than 0.
      if (couponRecord.TicketDocOrFimNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Documnet or FIM Number", Convert.ToString(couponRecord.TicketDocOrFimNumber), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidTicketOrFimNumber, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(couponRecord.TicketDocOrFimNumber).Length > 10)
      {
        var errorCodeFor = ErrorCodes.TicketFimDocumentNoGreaterThanTenNs;
        if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
        {
          errorCodeFor = SamplingErrorCodes.TicketFimDocumentNoGreaterThanTenS;
        }

        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Ticket Documnet or FIM Number",
                                                                        Convert.ToString(couponRecord.TicketDocOrFimNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelCoupon,
                                                                        errorCodeFor,
                                                                        ErrorStatus.X,
                                                                        couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP #672: Validation on Taxes in PAX FIM Billings
      if (couponRecord.SourceCodeId == FimSourceCode14Pax && couponRecord.TaxBreakdown.Count > 0)
      {
          var taxrecord = couponRecord.TaxBreakdown.FirstOrDefault();

          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Code",
                                                                          Convert.ToString(taxrecord.Amount), invoice, fileName,
                                                                          ErrorLevels.ErrorLevelCouponTax, ErrorCodes.TaxCannotbeBilledForSourceCode14Fim, ErrorStatus.X,
                                                                          couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
      }
      else
      {
          // Coupon Tax records should be present if TaxAmount > 0.
          if (couponRecord.TaxAmount > 0 && couponRecord.TaxBreakdown.Count == 0)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Tax Amount",
                                                                              Convert.ToString(couponRecord.TaxAmount), invoice, fileName,
                                                                              ErrorLevels.ErrorLevelCoupon, ErrorCodes.ZeroCouponTaxRecordsForCouponTaxAmount, ErrorStatus.X,
                                                                              couponRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;


          }
          else
          {
              // Coupon Tax records should be present if TaxAmount > 0.
              if (couponRecord.TaxAmount > 0 && couponRecord.TaxBreakdown.Count == 0)
              {
                  var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Tax Amount",
                                                                                  Convert.ToString(couponRecord.TaxAmount), invoice, fileName,
                                                                                  ErrorLevels.ErrorLevelCoupon, ErrorCodes.ZeroCouponTaxRecordsForCouponTaxAmount, ErrorStatus.X,
                                                                                  couponRecord);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
              }
              else
              {
                  // Total of Tax amount in all the Tax records.
                  double totalTax = 0;

                  if (couponRecord.TaxBreakdown != null)
                  {
                      //Parallel.ForEach(couponRecord.TaxBreakdown, taxRecord => { totalTax += taxRecord.Amount; });
                      totalTax = couponRecord.TaxBreakdown.Sum(taxRecord => taxRecord.Amount);
                  }

                  if (invoice.Tolerance != null && !CompareUtil.Compare(couponRecord.TaxAmount, totalTax, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
                  {
                      var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Tax Amount", Convert.ToString(couponRecord.TaxAmount), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCouponTaxBreakdownAmount, ErrorStatus.X, couponRecord);
                      exceptionDetailsList.Add(validationExceptionDetail);
                      isValid = false;
                  }
              }
          }
      }
      //Validate Batch Sequence Number and Record Sequence Number
      if (couponRecord.RecordSequenceWithinBatch <= 0 || couponRecord.BatchSequenceNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number-Record Sequence Number", string.Format("{0}-{1}", couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch),
                                                   invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidBatchSequenceNoAndRecordNo, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (couponRecord.VatAmount > 0 && couponRecord.VatBreakdown.Count == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Vat Amount",
                                                                       Convert.ToString(couponRecord.VatAmount), invoice, fileName,
                                                                       ErrorLevels.ErrorLevelCoupon, ErrorCodes.ZeroCouponVatRecordsForCouponVatAmount, ErrorStatus.X,
                                                                       couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      else
      {
        double totalVat = 0;
        //Parallel.ForEach(couponRecord.VatBreakdown, vatRecord => { totalVat += vatRecord.VatCalculatedAmount; });
        totalVat = couponRecord.VatBreakdown.Sum(vatRecord => vatRecord.VatCalculatedAmount);

        if (invoice.Tolerance != null && !CompareUtil.Compare(couponRecord.VatAmount, totalVat, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Vat Amount",
                                                                       Convert.ToString(couponRecord.VatAmount), invoice, fileName,
                                                                       ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCouponVatBreakdownAmount, ErrorStatus.X,
                                                                       couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      if ((couponRecord.CheckDigit < 0) || (couponRecord.CheckDigit > 6 && couponRecord.CheckDigit != 9))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Check Digit",
                                                                        Convert.ToString(couponRecord.CheckDigit), invoice, fileName,
                                                                        ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCheckDigit, ErrorStatus.C,
                                                                        couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //for FIM coupon number 
      if (couponRecord.SourceCodeId == 14)
      {
        if ((couponRecord.TicketOrFimCouponNumber <= 0 || couponRecord.TicketOrFimCouponNumber > 4) && couponRecord.TicketOrFimCouponNumber != 9)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Number", Convert.ToString(couponRecord.TicketOrFimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCouponNumber, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if ((couponRecord.TicketOrFimCouponNumber <= 0 || couponRecord.TicketOrFimCouponNumber > 4))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Number", Convert.ToString(couponRecord.TicketOrFimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCouponNumber, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Flight details are mandatory for FIM and coupon uplift
      if (couponRecord.SourceCodeId == 1 || couponRecord.SourceCodeId == 14)
      {
        if (couponRecord.FlightNumber == null || couponRecord.FlightNumber == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Number", Convert.ToString(couponRecord.FlightNumber),
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidFlightNumber, ErrorStatus.C, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (couponRecord.AirlineFlightDesignator == null || string.IsNullOrEmpty(couponRecord.AirlineFlightDesignator.Trim()))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", couponRecord.AirlineFlightDesignator,
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (string.IsNullOrWhiteSpace(couponRecord.FromAirportOfCoupon))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", couponRecord.FromAirportOfCoupon,
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidFromAirportCode, ErrorStatus.C, couponRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (string.IsNullOrWhiteSpace(couponRecord.ToAirportOfCoupon))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "To Airport", couponRecord.ToAirportOfCoupon,
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidToAirportCode, ErrorStatus.C, couponRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (couponRecord.FlightDate.HasValue == false)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Date", string.Empty,
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidFlightDate, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //if (couponRecord.SettlementAuthorizationCode != null && couponRecord.ElectronicTicketIndicator && string.IsNullOrEmpty(couponRecord.SettlementAuthorizationCode.Trim()))
        //{
        //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Settlement Authorization Code", string.Empty,
        //                                                      invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidSettlementAuthorizationCode, ErrorStatus.C, couponRecord);
        //  exceptionDetailsList.Add(validationExceptionDetail);
        //  isValid = false;
        //}
      }

      // CMP-396: Validation of Flight Date in PAX Prime Billing process
      if (couponRecord.FlightDate.HasValue && (((couponRecord.SourceCodeId == 1 || couponRecord.SourceCodeId == 14) && invoice.BillingCode == 0) || (couponRecord.SourceCodeId == 1 && invoice.BillingCode == 3)))
      {
        if ((couponRecord.FlightDate.Value.Year > invoice.BillingYear) ||
            (couponRecord.FlightDate.Value.Year.Equals(invoice.BillingYear) &&
             couponRecord.FlightDate.Value.Month > invoice.BillingMonth))
        {
          if(invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Flight Date",
                                                                          String.Format("{0:yyyyMMdd}", couponRecord.FlightDate),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelCoupon,
                                                                          ErrorCodes.FlightDateGreaterThanBillingMonth,
                                                                          ErrorStatus.X, couponRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Flight Date",
                                                                          String.Format("{0:yyyy-MM-dd}", couponRecord.FlightDate),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelCoupon,
                                                                          ErrorCodes.FlightDateGreaterThanBillingMonth,
                                                                          ErrorStatus.X, couponRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      if (
          ((!string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied)) && string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) ||
          (ignoreValidationOnMigrationPeriod ? ((!string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) && string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied) && (couponRecord.OriginalPmi.Trim() != "N")) : ((!string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) && string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied))) ||
          (couponRecord.OriginalPmi != null && couponRecord.AgreementIndicatorSupplied != null && !ValidateOriginalPmi(couponRecord.OriginalPmi, couponRecord.AgreementIndicatorSupplied))
         )
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Original PMI", couponRecord.OriginalPmi,
                                                            invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidOriginalPMI, ErrorStatus.C, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (couponRecord.AgreementIndicatorSupplied != null && !ValidateAgreementIndicatorSupplied(couponRecord.AgreementIndicatorSupplied))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Agreement Indicator Supplied", couponRecord.AgreementIndicatorSupplied,
                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidAgreementIndicatorSupplied, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //if (couponRecord.FlightMonth.HasValue && couponRecord.FlightDay.HasValue)
      if (couponRecord.FlightDate.HasValue)
      {
        // Try to parse date string to DateTime to check whether its valid.
        DateTime resultDate;
        if (!DateTime.TryParse(couponRecord.FlightDate.Value.ToString(), out resultDate))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Date", couponRecord.FlightDate.Value.ToString(),
                                                                   invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidFlightDate, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      // Duplicate check - Any prime billing transaction that has been twice billed in the same invoice, or in a previous invoice to the same airline 
      // will be considered as a duplicate. This check will be performed for invoices in the last 12 calendar months. - This is done in stored procedure

      //DateTime billingDate;
      //var billingYearToCompare = 0;
      //var billingMonthToCompare = 0;

      //if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
      //{
      //  DateTime billingDateToCompare = billingDate.AddMonths(-12);
      //  billingYearToCompare = billingDateToCompare.Year;
      //  billingMonthToCompare = billingDateToCompare.Month;
      //}

      //if (CouponRecordRepository.GetCount(coupon => coupon.TicketOrFimCouponNumber == couponRecord.TicketOrFimCouponNumber
      //                                                                 && coupon.TicketDocOrFimNumber == couponRecord.TicketDocOrFimNumber
      //                                                                 && coupon.TicketOrFimIssuingAirline == couponRecord.TicketOrFimIssuingAirline
      //                                                                 && coupon.Invoice.BilledMemberId == invoice.BilledMemberId
      //                                                                 && coupon.Invoice.BillingMemberId == invoice.BillingMemberId
      //                                                                 && coupon.Invoice.BillingYear >= billingYearToCompare
      //                                                                 && coupon.Invoice.BillingMonth >= billingMonthToCompare
      //                                                                 && coupon.Invoice.BillingCode == invoice.BillingCode) > 0)
      //{
      //  var iSValidationFlag = couponRecord.ISValidationFlag ?? string.Empty;
      //  couponRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, DuplicateValidationFlag);
      //}

      ////Duplicate check in current invoice - Ticket Issuing Airline, Ticket/Document Number, Coupon No
      //else if (invoice.CouponDataRecord.Where(coupon => coupon.TicketOrFimCouponNumber == couponRecord.TicketOrFimCouponNumber
      //                                                                 && coupon.TicketDocOrFimNumber == couponRecord.TicketDocOrFimNumber
      //                                                                 && coupon.TicketOrFimIssuingAirline == couponRecord.TicketOrFimIssuingAirline).Count() > 1)
      //{
      //  var iSValidationFlag = couponRecord.ISValidationFlag ?? string.Empty;
      //  couponRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, DuplicateValidationFlag);
      //}

      // Validate Handling Fee Type and Amount
      if (!string.IsNullOrWhiteSpace(couponRecord.HandlingFeeTypeId) && !(couponRecord.HandlingFeeTypeId.Equals("A") || couponRecord.HandlingFeeTypeId.Equals("S") || couponRecord.HandlingFeeTypeId.Equals("C")))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate, "Handling Fee Type",
                                                                        couponRecord.HandlingFeeTypeId,
                                                                        invoice, fileName,
                                                                        ErrorLevels.ErrorLevelCoupon,
                                                                        ErrorCodes.InvalidHandlingFeeType,
                                                                        ErrorStatus.X, couponRecord.SourceCodeId,
                                                                        couponRecord.BatchSequenceNumber,
                                                                        couponRecord.RecordSequenceWithinBatch,
                                                                        couponRecord.TicketDocOrFimNumber.ToString());
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }


      //if (couponRecord.HandlingFeeAmount != 0 && string.IsNullOrEmpty(couponRecord.HandlingFeeTypeId))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Type", couponRecord.HandlingFeeTypeId,
      //                                               invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidHandlingFeeTypeForValidHandFeeAmount, ErrorStatus.X, couponRecord.SourceCodeId, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch, couponRecord.TicketDocOrFimNumber.ToString());
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      if (couponRecord.HandlingFeeAmount == 0 && !string.IsNullOrEmpty(couponRecord.HandlingFeeTypeId))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Amount", Convert.ToString(couponRecord.HandlingFeeAmount),
                                                     invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidHandlingFeeAmountForValidHandFeeType, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;

      }

      //From airport of coupon and to airport of coupon should not be same.
      if (couponRecord.FromAirportOfCoupon != null && couponRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon.Trim()) && !string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon.Trim()))
        {
          if (String.Equals(couponRecord.FromAirportOfCoupon.ToUpper(), couponRecord.ToAirportOfCoupon.ToUpper()))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", Convert.ToString(couponRecord.FromAirportOfCoupon), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.FromAirportOfCouponAndToAirportOfCouponShouldNotBeSame, ErrorStatus.X, couponRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Agreement Indicator Supplied = S and Billing Code = 0 is invalid
      if (couponRecord.AgreementIndicatorSupplied != null && String.Equals(couponRecord.AgreementIndicatorSupplied.ToUpper(), "S") && (invoice.BillingCode == (int)BillingCode.NonSampling))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Agreement Indicator Supplied", Convert.ToString(couponRecord.AgreementIndicatorSupplied),
                                                             invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidAgreementIndicatorSupplied, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Ticket/FIM Issuing Airline
      isValid = ValidateTicketIssuingAirline(couponRecord, exceptionDetailsList, invoice, fileName, isValid, issuingAirline, fileSubmissionDate);

      //Validate Currency Adjustment Indicator - Mandatory for non-sampling.
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        if (string.IsNullOrWhiteSpace(couponRecord.CurrencyAdjustmentIndicator) || !ReferenceManager.IsValidCurrencyCode(invoice, couponRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Currency Adjustment Indicator", Convert.ToString(couponRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.C,
                                                                          couponRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if (!string.IsNullOrWhiteSpace(couponRecord.CurrencyAdjustmentIndicator) && !ReferenceManager.IsValidCurrencyCode(invoice, couponRecord.CurrencyAdjustmentIndicator))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Currency Adjustment Indicator", Convert.ToString(couponRecord.CurrencyAdjustmentIndicator),
                                                                          invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidCurrencyAdjustmentInd, ErrorStatus.C,
                                                                          couponRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Validate SourceCode 
      if (!ReferenceManager.IsValidSourceCode(invoice, couponRecord.SourceCodeId, (int)transType))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code", Convert.ToString(couponRecord.SourceCodeId),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidSourceCode, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Airline Flight Designator*
      if (couponRecord.AirlineFlightDesignator != null && !string.IsNullOrEmpty(couponRecord.AirlineFlightDesignator.Trim()))
      {
        isValid = ValidateAirlineFlightDesignator(couponRecord, exceptionDetailsList, invoice, fileName, airlineFlightDesignators, isValid, couponRecord.AirlineFlightDesignator, fileSubmissionDate);
      }

      //Validate FromAirportOfCoupon 
      if (!IsValidCityAirportCode(couponRecord.FromAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", Convert.ToString(couponRecord.FromAirportOfCoupon),
                                                 invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidFromAirportCode, ErrorStatus.C, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate ToAirportOfCoupon 
      if (!IsValidCityAirportCode(couponRecord.ToAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "To Airport", Convert.ToString(couponRecord.ToAirportOfCoupon),
                                                         invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidToAirportCode, ErrorStatus.C, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate ReasonCode - Dont validate reason code in coupon record.
      //if (couponRecord.ReasonCode != null && string.IsNullOrEmpty(couponRecord.ReasonCode.Trim()))
      //{
      //  if (!ReferenceManager.IsValidReasonCode(couponRecord.ReasonCode, couponRecord.SourceCodeId, (int)BillingCode.NonSampling))
      //  {
      //    var validationExceptionDetail = CreateValidationExceptionDetail("Reason Code", Convert.ToString(couponRecord.ReasonCode),
      //                                                                    invoice.InvoiceNumber, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidReasonCode,
      //                                                                    couponRecord.SourceCodeId, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch, couponRecord.TicketDocOrFimNumber);
      //    exceptionDetailsList.Add(validationExceptionDetail);
      //    isValid = false;
      //  }
      //}

      double expectedIscAmount = ConvertUtil.Round(couponRecord.IscPercent * couponRecord.CouponGrossValueOrApplicableLocalFare / 100, Constants.PaxDecimalPlaces);
      if (couponRecord.SourceCodeId != 90)
      {
        if (invoice.Tolerance != null && !CompareUtil.Compare(couponRecord.IscAmount, expectedIscAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "ISC Amount",
                                                                          Convert.ToString(couponRecord.IscAmount),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelCoupon,
                                                                          ErrorCodes.InvalidIscPercentage,
                                                                          ErrorStatus.X,
                                                                          couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      double expectedUatpAmount = ConvertUtil.Round(couponRecord.UatpPercent * couponRecord.CouponGrossValueOrApplicableLocalFare / 100, Constants.PaxDecimalPlaces);
      if (couponRecord.SourceCodeId != 90)
      {
        if (invoice.Tolerance != null && !CompareUtil.Compare(couponRecord.UatpAmount, expectedUatpAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "UATP Amount",
                                                                          Convert.ToString(couponRecord.UatpAmount),
                                                                          invoice,
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelCoupon,
                                                                          ErrorCodes.InvalidUatpPercentage,
                                                                          ErrorStatus.X,
                                                                          couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Validate if net total is sum of all the total amounts
      var netCouponTotalAmount = couponRecord.CouponGrossValueOrApplicableLocalFare + couponRecord.TaxAmount + couponRecord.VatAmount + couponRecord.IscAmount + couponRecord.HandlingFeeAmount +
                                 couponRecord.OtherCommissionAmount + couponRecord.UatpAmount;

      if (invoice.Tolerance != null && !CompareUtil.Compare(couponRecord.CouponTotalAmount, netCouponTotalAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Total amount", Convert.ToString(couponRecord.CouponTotalAmount),
                                           invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.CouponTotalAmountDoesNotMatchWithSumOfOtherAmount, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (couponRecord.CouponTotalAmount < 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Total amount", Convert.ToString(couponRecord.CouponTotalAmount),
                                           invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.PrimeBillingNetTotalAmountShouldNotBeNegative, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      else if (couponRecord.SourceCodeId != 14 && exchangeRate != null && maxAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(couponRecord.CouponTotalAmount, TransactionType.Coupon, invoice.ListingCurrencyId, invoice, exchangeRate, iMaxAcceptableAmount: maxAcceptableAmount))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Total amount", Convert.ToString(couponRecord.CouponTotalAmount),
                                           invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.CouponTotalAmountIsNotInAllowedRange, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Reference field 1 AND Field 2
      if (couponRecord.SourceCodeId == 23)
      {
          // TODO: To be released later in November 2012
          //SCP446630 : Validation of RFIC and RFISC on SC23
          // Below code uncommented as pax file get pass for SC23 even if REF1 and REF2 not provided.
          if (couponRecord.ReferenceField1 != null && (string.IsNullOrEmpty(couponRecord.ReferenceField1.Trim()) || !ReferenceManager.IsValidRficCode(couponRecord.ReferenceField1.Trim())))
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 1", Convert.ToString(couponRecord.ReferenceField1), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidReferenceField1ForCoupon, ErrorStatus.X, couponRecord);

              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;

          }
          if (couponRecord.ReferenceField2 != null && (string.IsNullOrEmpty(couponRecord.ReferenceField2.Trim()) || !ReferenceManager.IsValidRfiscCode(couponRecord.ReferenceField2.Trim(), couponRecord.ReferenceField1.Trim())))
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Field 2", Convert.ToString(couponRecord.ReferenceField2), invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidReferenceField2ForCoupon, ErrorStatus.C, couponRecord);

              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;

          }
      }

      // TODO: To be released in Nov 2012 
      // Validate reference data for source code 90.
      //if (couponRecord.SourceCodeId == 90 && (string.IsNullOrWhiteSpace(couponRecord.ReferenceField1) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField2) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField3) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField4) || string.IsNullOrWhiteSpace(couponRecord.ReferenceField5)))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Fields", string.Empty,
      //                                     invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.ReferenceDataNotProvidedForFrequentFlyerRelatedBillings, ErrorStatus.X, couponRecord);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}


      // Set main ticket document number as 'TicketIssuingAirline-TicketDocumentNumber-TicketCouponNumber'
      var mainDocumentNumber = string.Format(@"{0}-{1}-{2}", couponRecord.TicketOrFimIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber);

      //Validate Tax Breakdowns 
      foreach (var couponRecordTax in couponRecord.TaxBreakdown)
      {
        isValid = ValidateParsedTax(couponRecordTax, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelCouponTax, fileSubmissionDate, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch, mainDocumentNumber, couponRecord.SourceCodeId);
      }

      //Validate Vat Breakdowns 
      foreach (var couponRecordVat in couponRecord.VatBreakdown)
      {
        isValid = ValidateParsedVat(couponRecordVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelCouponVat, fileSubmissionDate, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch, mainDocumentNumber, couponRecord.SourceCodeId, false, true);
      }

      //Commented the code for setting the xpiry date
      //if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
      //{
      //  // Set expiry period of Form AB Coupon for purging.
      //  couponRecord.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.SamplingFormD, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorYes, null);
      //}
      //else if (invoice.BillingCode == (int)BillingCode.NonSampling)
      //{
      //  // Set expiry period of prime coupon for purging.
      //  couponRecord.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);
      //}

      return isValid;
    }

    /// <summary>
    /// Validates the airline flight designator.
    /// </summary>
    /// <param name="couponRecord">The coupon record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="airlineFlightDesignators">The valid flight designator.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <param name="airlineFlightDesignator">The airline flight designator.</param>
    /// <returns></returns>
    private bool ValidateAirlineFlightDesignator(PrimeCoupon couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> airlineFlightDesignators, bool isValid, string airlineFlightDesignator, DateTime fileSubmissionDate)
    {
      if (!airlineFlightDesignators.Keys.Contains(airlineFlightDesignator))
      {
        if (MemberManager.IsValidAirlineAlphaCode(airlineFlightDesignator))
        {
          airlineFlightDesignators.Add(airlineFlightDesignator, true);
        }
        else
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(couponRecord.AirlineFlightDesignator),
                                                              invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          airlineFlightDesignators.Add(airlineFlightDesignator, false);
          isValid = false;
        }
      }
      else if (!airlineFlightDesignators[airlineFlightDesignator])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(couponRecord.AirlineFlightDesignator),
                                                            invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidAirlineCode, ErrorStatus.C, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Validates the ticket issuing airline.
    /// </summary>
    /// <param name="couponRecord">The coupon record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="isValid">if set to <c>true</c> [is valid].</param>
    /// <returns></returns>
    private bool ValidateTicketIssuingAirline(PrimeCoupon couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, bool isValid, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
    {
      var ticketIssuingAirline = couponRecord.TicketOrFimIssuingAirline;
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
                                                               invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, couponRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else if (!issuingAirline[ticketIssuingAirline])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Or FimIssuing Airline", Convert.ToString(couponRecord.TicketOrFimIssuingAirline),
                                                             invoice, fileName, ErrorLevels.ErrorLevelCoupon, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, couponRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    //Validate billing memo coupon breakdown record 
    public bool ValidateParsedBMCouponTotals(BMCoupon bmCouponBreakdownRecord,
                                                                IList<IsValidationExceptionDetail> exceptionDetailsList,
                                                                BillingMemo parentMemo, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      var tktIsuingAirline = bmCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;
      if (Convert.ToDouble(bmCouponBreakdownRecord.VatAmount) > 0 && bmCouponBreakdownRecord.VatBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", bmCouponBreakdownRecord.VatAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (Convert.ToDouble(bmCouponBreakdownRecord.TaxAmount) != 0 && bmCouponBreakdownRecord.TaxBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount", bmCouponBreakdownRecord.TaxAmount.ToString(),
                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.ZeroTaxBreakdownRecords, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (invoice.Tolerance != null)
      {
        var totalTaxAmount = bmCouponBreakdownRecord.TaxBreakdown.Sum(currentRecord => currentRecord.Amount);

        if (!CompareUtil.Compare(bmCouponBreakdownRecord.TaxAmount, totalTaxAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount", Convert.ToString(bmCouponBreakdownRecord.TaxAmount), invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        var totalVatAmount = bmCouponBreakdownRecord.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);

        if (!CompareUtil.Compare(bmCouponBreakdownRecord.VatAmount, totalVatAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount", Convert.ToString(bmCouponBreakdownRecord.VatAmount), invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        var netAmountBilled = Convert.ToDouble(bmCouponBreakdownRecord.GrossAmountBilled) + bmCouponBreakdownRecord.TaxAmount + bmCouponBreakdownRecord.IscAmountBilled + bmCouponBreakdownRecord.OtherCommissionBilled + bmCouponBreakdownRecord.HandlingFeeAmount + bmCouponBreakdownRecord.UatpAmountBilled + bmCouponBreakdownRecord.VatAmount;

        //Total net amount billed
        if (!CompareUtil.Compare(bmCouponBreakdownRecord.NetAmountBilled, netAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount", Convert.ToString(bmCouponBreakdownRecord.NetAmountBilled), invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.InvalidNetBilledAmount, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Billing memo coupon net total amount should not be negative.
      if (bmCouponBreakdownRecord.NetAmountBilled < 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(bmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount", Convert.ToString(bmCouponBreakdownRecord.NetAmountBilled),
                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemoCoupon, ErrorCodes.BillingMemoCouponNetTotalAmountShouldNotBeNegative, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, bmCouponBreakdownRecord.TicketDocOrFimNumber, bmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validate Parsed Billing Memo Total Amounts
    /// </summary>
    /// <param name="billingMemo"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool ValidateParsedBillingMemoTotals(BillingMemo billingMemo, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      double totalTaxAmountBilled = 0, totalVatAmountBilled = 0;
      double totalHandlingFeeAmountBilled = 0, totalIscAmountBilled = 0, totalOtherCommissionAmountBilled = 0, totalUatpAmountBilled = 0;
      decimal totalGrossAmountBilled = 0, netBilledAmount = 0;

      var billingMemoNumber = string.Empty;

      if (billingMemo.BillingMemoNumber != null)
      {
        billingMemoNumber = billingMemo.BillingMemoNumber;
      }

      if (billingMemo.CouponBreakdownRecord.Count == 0)
      {
        double totalVatAmount = 0;

        //If vat amount is > 0 and vat breakdowns is 0
        if (billingMemo.VatBreakdown != null && Convert.ToDouble(billingMemo.TotalVatAmountBilled) > 0 && billingMemo.VatBreakdown.Count() == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount", billingMemo.TotalVatAmountBilled.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, billingMemo);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (billingMemo.VatBreakdown != null)
        {

          totalVatAmount = billingMemo.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);

        }
        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(billingMemo.TotalVatAmountBilled, Convert.ToDecimal(totalVatAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount", Convert.ToString(billingMemo.TotalVatAmountBilled), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.InvalidTotalVatBreakdownAmounts, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          var netAmountBilled = billingMemo.TotalGrossAmountBilled + billingMemo.TaxAmountBilled + billingMemo.TotalIscAmountBilled + billingMemo.TotalOtherCommissionAmount + Convert.ToDecimal(billingMemo.TotalHandlingFeeBilled) + billingMemo.TotalUatpAmountBilled + billingMemo.TotalVatAmountBilled;

          //Total net amount billed
          if (!CompareUtil.Compare(billingMemo.NetAmountBilled, netAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount", Convert.ToString(billingMemo.NetAmountBilled), invoice, fileName, ErrorLevels.ErrorLevelBillingMemo, ErrorCodes.InvalidNetBilledAmount, ErrorStatus.X, billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }

      }
      else
      {

        foreach (var currentRecord in billingMemo.CouponBreakdownRecord)
        {
          totalGrossAmountBilled += currentRecord.GrossAmountBilled;
          totalTaxAmountBilled += currentRecord.TaxAmount;
          totalVatAmountBilled += currentRecord.VatAmount;
          //Handling fee amount
          totalHandlingFeeAmountBilled += currentRecord.HandlingFeeAmount;
          //ISC amount
          totalIscAmountBilled += currentRecord.IscAmountBilled;
          //Other commission amount
          totalOtherCommissionAmountBilled += currentRecord.OtherCommissionBilled;
          //UATP amount
          totalUatpAmountBilled += currentRecord.UatpAmountBilled;
          netBilledAmount += Convert.ToDecimal(currentRecord.NetAmountBilled);
        }

        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(totalGrossAmountBilled, billingMemo.TotalGrossAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Amount",
                                                                            Convert.ToString(billingMemo.TotalGrossAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalGrossValue, ErrorStatus.X,
                                                                            billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(Convert.ToDecimal(totalTaxAmountBilled), billingMemo.TaxAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount",
                                                                            Convert.ToString(billingMemo.TaxAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X,
                                                                            billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(Convert.ToDecimal(totalVatAmountBilled), billingMemo.TotalVatAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount",
                                                                            Convert.ToString(billingMemo.TotalVatAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X,
                                                                            billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(totalHandlingFeeAmountBilled, billingMemo.TotalHandlingFeeBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount",
                                                                            Convert.ToString(billingMemo.TotalHandlingFeeBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalHandlingFeeAmount, ErrorStatus.X,
                                                                           billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalIscAmountBilled), billingMemo.TotalIscAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount",
                                                                            Convert.ToString(billingMemo.TotalIscAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalIscAmount, ErrorStatus.X,
                                                                            billingMemo.SourceCodeId,
                                                                            billingMemo.BatchSequenceNumber,
                                                                            billingMemo.RecordSequenceWithinBatch, billingMemoNumber);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalOtherCommissionAmountBilled), billingMemo.TotalOtherCommissionAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount",
                                                                            Convert.ToString(billingMemo.TotalOtherCommissionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalOtherCommissionAmount, ErrorStatus.X,
                                                                           billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(Convert.ToDecimal(totalUatpAmountBilled), billingMemo.TotalUatpAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount",
                                                                            Convert.ToString(billingMemo.TotalUatpAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidTotalUatpAmount, ErrorStatus.X,
                                                                           billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(netBilledAmount, billingMemo.NetAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Billed Amount",
                                                                            Convert.ToString(billingMemo.NetAmountBilled),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                            ErrorCodes.InvalidNetBilledAmount, ErrorStatus.X,
                                                                            billingMemo);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Billing memo net total amount should not be negative.
      if (billingMemo.NetAmountBilled < 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(billingMemo.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Billed Amount",
                                                                        Convert.ToString(billingMemo.NetAmountBilled),
                                                                        invoice, fileName, ErrorLevels.ErrorLevelBillingMemo,
                                                                        ErrorCodes.BillingMemoNetTotalAmountShouldNotBeNegative, ErrorStatus.X,
                                                                        billingMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// Validate Rejection Coupon Breakdown Records Total Amounts
    /// </summary>
    /// <param name="rmCouponBreakdownRecord"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="parentRecord"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool ValidateParsedRMCouponBreakdownTotal(RMCoupon rmCouponBreakdownRecord,
                                                             IList<IsValidationExceptionDetail> exceptionDetailsList,
                                                             RejectionMemo parentRecord, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;

      var errorLevel = string.Empty;
      var tktIsuingAirline = rmCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        errorLevel = ErrorLevels.ErrorLevelRejectionMemoCoupon;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormFCoupon;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormXFCoupon;
      }

      int rejectionStage = parentRecord.RejectionStage;

      if (rejectionStage == 1 || rejectionStage == 3)
      {
        //Total Gross Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.GrossAmountDifference, (rmCouponBreakdownRecord.GrossAmountBilled - rmCouponBreakdownRecord.GrossAmountAccepted), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Difference", Convert.ToString(rmCouponBreakdownRecord.GrossAmountDifference),




















































                                                        invoice, fileName, errorLevel, ErrorCodes.GrossAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Tax Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.TaxAmountDifference, (rmCouponBreakdownRecord.TaxAmountBilled - rmCouponBreakdownRecord.TaxAmountAccepted), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Difference", Convert.ToString(rmCouponBreakdownRecord.TaxAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.TaxAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Vat Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.VatAmountDifference, (rmCouponBreakdownRecord.VatAmountBilled - rmCouponBreakdownRecord.VatAmountAccepted), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Difference", Convert.ToString(rmCouponBreakdownRecord.VatAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.VatAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Handling Fee Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.HandlingDifference, (rmCouponBreakdownRecord.AllowedHandlingFee - rmCouponBreakdownRecord.AcceptedHandlingFee), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Difference", Convert.ToString(rmCouponBreakdownRecord.HandlingDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //ISC Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.IscDifference, (rmCouponBreakdownRecord.AllowedIscAmount - rmCouponBreakdownRecord.AcceptedIscAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount Difference", Convert.ToString(rmCouponBreakdownRecord.IscDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.IscAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Other Commission Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.OtherCommissionDifference, (rmCouponBreakdownRecord.AllowedOtherCommission - rmCouponBreakdownRecord.AcceptedOtherCommission), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Amount Difference", Convert.ToString(rmCouponBreakdownRecord.OtherCommissionDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //UATP Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.UatpDifference, (rmCouponBreakdownRecord.AllowedUatpAmount - rmCouponBreakdownRecord.AcceptedUatpAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Amount Difference", Convert.ToString(rmCouponBreakdownRecord.UatpDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.UatpAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        //Total gross difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.GrossAmountDifference, (rmCouponBreakdownRecord.GrossAmountAccepted - rmCouponBreakdownRecord.GrossAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Difference", Convert.ToString(rmCouponBreakdownRecord.GrossAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.GrossAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Tax Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.TaxAmountDifference, (rmCouponBreakdownRecord.TaxAmountAccepted - rmCouponBreakdownRecord.TaxAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Difference", Convert.ToString(rmCouponBreakdownRecord.TaxAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.TaxAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Vat Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.VatAmountDifference, (rmCouponBreakdownRecord.VatAmountAccepted - rmCouponBreakdownRecord.VatAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Difference", Convert.ToString(rmCouponBreakdownRecord.VatAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.VatAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Handling Fee Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.HandlingDifference, (rmCouponBreakdownRecord.AcceptedHandlingFee - rmCouponBreakdownRecord.AllowedHandlingFee), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Difference", Convert.ToString(rmCouponBreakdownRecord.HandlingDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //ISC Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.IscDifference, (rmCouponBreakdownRecord.AcceptedIscAmount - rmCouponBreakdownRecord.AllowedIscAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount Difference", Convert.ToString(rmCouponBreakdownRecord.IscDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.IscAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Other Commission Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.OtherCommissionDifference, (rmCouponBreakdownRecord.AcceptedOtherCommission - rmCouponBreakdownRecord.AllowedOtherCommission), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Amount Difference", Convert.ToString(rmCouponBreakdownRecord.OtherCommissionDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //UATP Difference
        if (!CompareUtil.Compare(rmCouponBreakdownRecord.UatpDifference, (rmCouponBreakdownRecord.AcceptedUatpAmount - rmCouponBreakdownRecord.AllowedUatpAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Amount Difference", Convert.ToString(rmCouponBreakdownRecord.UatpDifference),
                                              invoice, fileName, errorLevel, ErrorCodes.UatpAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }


      double totalTaxAmountBiled = 0;
      double totalTaxAmountAccepted = 0;
      double totalTaxAmountDifference = 0;
      double totalVatAmount = 0;

      if (Convert.ToDouble(rmCouponBreakdownRecord.VatAmountDifference) > 0 && rmCouponBreakdownRecord.VatBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Amount Difference", rmCouponBreakdownRecord.VatAmountDifference.ToString(),
                                                            invoice, fileName, errorLevel, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (Convert.ToDouble(rmCouponBreakdownRecord.TaxAmountDifference) > 0 && rmCouponBreakdownRecord.TaxBreakdown.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount Difference", rmCouponBreakdownRecord.VatAmountDifference.ToString(),
                                                            invoice, fileName, errorLevel, ErrorCodes.ZeroTaxBreakdownRecords, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }


      foreach (var currentRecord in rmCouponBreakdownRecord.TaxBreakdown)
      {
        totalTaxAmountBiled += currentRecord.Amount;
        totalTaxAmountAccepted += currentRecord.AmountAccepted;
        totalTaxAmountDifference += currentRecord.AmountDifference;
      }

      if (invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(totalTaxAmountBiled, rmCouponBreakdownRecord.TaxAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount Billed",
                                                                          Convert.ToString(rmCouponBreakdownRecord.TaxAmountBilled),
                                                                          invoice, fileName, errorLevel,
                                                                          ErrorCodes.InvalidTotalTaxBreakdownAmounts,
                                                                          ErrorStatus.X, parentRecord, false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (!CompareUtil.Compare(totalTaxAmountAccepted, rmCouponBreakdownRecord.TaxAmountAccepted, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount Accepted",
                                                                          Convert.ToString(rmCouponBreakdownRecord.TaxAmountAccepted),
                                                                          invoice, fileName, errorLevel, ErrorCodes.InvalidTotalAcceptedTaxBreakdownAmounts,
                                                                          ErrorStatus.X, parentRecord, false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (!CompareUtil.Compare(totalTaxAmountDifference, rmCouponBreakdownRecord.TaxAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Amount Difference",
                                                                          Convert.ToString(rmCouponBreakdownRecord.TaxAmountDifference),
                                                                          invoice, fileName, errorLevel, ErrorCodes.InvalidTotalDifferenceTaxBreakdownAmounts,
                                                                          ErrorStatus.X, parentRecord, false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }


        totalVatAmount = rmCouponBreakdownRecord.VatBreakdown.Sum(currentRecord => currentRecord.VatCalculatedAmount);

        if (!CompareUtil.Compare(totalVatAmount, rmCouponBreakdownRecord.VatAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Difference",
                                                                          Convert.ToString(rmCouponBreakdownRecord.VatAmountDifference),
                                                                          invoice, fileName, errorLevel,
                                                                          ErrorCodes.InvalidTotalVatBreakdownAmounts,
                                                                          ErrorStatus.X, parentRecord, false,
                                                                          string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Total net reject amount
      if (invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(Convert.ToDouble(rmCouponBreakdownRecord.NetRejectAmount), ConvertUtil.Round((rmCouponBreakdownRecord.GrossAmountDifference + rmCouponBreakdownRecord.TaxAmountDifference + rmCouponBreakdownRecord.IscDifference + rmCouponBreakdownRecord.OtherCommissionDifference
                                                                                                        + rmCouponBreakdownRecord.HandlingDifference + rmCouponBreakdownRecord.UatpDifference + rmCouponBreakdownRecord.VatAmountDifference), Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rmCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount", Convert.ToString(rmCouponBreakdownRecord.NetRejectAmount),
                                                                          invoice, fileName, errorLevel, ErrorCodes.NetRejectAmountDoesNotMatchWithSumOfDiffAmount, ErrorStatus.X, parentRecord, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rmCouponBreakdownRecord.TicketDocOrFimNumber, rmCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Total net reject amount
      //if (Convert.ToDouble(rmCouponBreakdownRecord.NetRejectAmount) <= 0)
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount", Convert.ToString(rmCouponBreakdownRecord.NetRejectAmount),
      //                                      invoice, fileName, errorLevel, ErrorCodes.RejectionMemoCouponNetTotalAmountShouldNotBeNegative, ErrorStatus.X, parentRecord, false, rmCouponBreakdownRecord.TicketDocOrFimNumber.ToString());
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      return isValid;
    }

    /// <summary>
    /// Validation of rejection memo totals with child coupon records
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    public bool ValidateParsedRejectionMemoTotals(RejectionMemo rejectionMemoRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, int billingCode, DateTime fileSubmissionDate)
    {

      bool isValid = true;

      var errorLevel = string.Empty;
      var rejectionMemoNumber = string.Empty;

      if (rejectionMemoRecord.RejectionMemoNumber != null)
      {
        rejectionMemoNumber = rejectionMemoRecord.RejectionMemoNumber;
      }

      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        errorLevel = ErrorLevels.ErrorLevelRejectionMemo;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormF;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormXF;
      }

      if (rejectionMemoRecord.RejectionStage == 1 || rejectionMemoRecord.RejectionStage == 3)
      {
        //Total Gross Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalGrossDifference, rejectionMemoRecord.TotalGrossAmountBilled - rejectionMemoRecord.TotalGrossAcceptedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Difference", Convert.ToString(rejectionMemoRecord.TotalGrossDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.GrossAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Tax Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalTaxAmountDifference, rejectionMemoRecord.TotalTaxAmountBilled - rejectionMemoRecord.TotalTaxAmountAccepted, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Difference", Convert.ToString(rejectionMemoRecord.TotalTaxAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.TaxAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Gross Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalVatAmountDifference, rejectionMemoRecord.TotalVatAmountBilled - rejectionMemoRecord.TotalVatAmountAccepted, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Difference", Convert.ToString(rejectionMemoRecord.TotalVatAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.VatAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Handling Fee Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.HandlingFeeAmountDifference, rejectionMemoRecord.AllowedHandlingFee - rejectionMemoRecord.AcceptedHandlingFee, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Difference", Convert.ToString(rejectionMemoRecord.HandlingFeeAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //ISC Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.IscDifference, rejectionMemoRecord.AllowedIscAmount - rejectionMemoRecord.AcceptedIscAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount Difference", Convert.ToString(rejectionMemoRecord.IscDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.IscAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Other Commission Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.OtherCommissionDifference, rejectionMemoRecord.AllowedOtherCommission - rejectionMemoRecord.AcceptedOtherCommission, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Amount Difference", Convert.ToString(rejectionMemoRecord.OtherCommissionDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //UATP Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.UatpAmountDifference, rejectionMemoRecord.AllowedUatpAmount - rejectionMemoRecord.AcceptedUatpAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Amount Difference", Convert.ToString(rejectionMemoRecord.UatpAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.UatpAmountDifferenceShouldMatchWithBilledMinusAccepted, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        //Total gross difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalGrossDifference, rejectionMemoRecord.TotalGrossAcceptedAmount - rejectionMemoRecord.TotalGrossAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Difference", Convert.ToString(rejectionMemoRecord.TotalGrossDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.GrossAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Tax Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalTaxAmountDifference, rejectionMemoRecord.TotalTaxAmountAccepted - rejectionMemoRecord.TotalTaxAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Difference", Convert.ToString(rejectionMemoRecord.TotalTaxAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.TaxAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Total Vat Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.TotalVatAmountDifference, rejectionMemoRecord.TotalVatAmountAccepted - rejectionMemoRecord.TotalVatAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Difference", Convert.ToString(rejectionMemoRecord.TotalVatAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.VatAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Handling Fee Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.HandlingFeeAmountDifference, rejectionMemoRecord.AcceptedHandlingFee - rejectionMemoRecord.AllowedHandlingFee, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Difference", Convert.ToString(rejectionMemoRecord.HandlingFeeAmountDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //ISC Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.IscDifference, rejectionMemoRecord.AcceptedIscAmount - rejectionMemoRecord.AllowedIscAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount Difference", Convert.ToString(rejectionMemoRecord.IscDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.IscAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Other Commission Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.OtherCommissionDifference, rejectionMemoRecord.AcceptedOtherCommission - rejectionMemoRecord.AllowedOtherCommission, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Amount Difference", Convert.ToString(rejectionMemoRecord.OtherCommissionDifference),
                                                        invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //UATP Difference
        if (!CompareUtil.Compare(rejectionMemoRecord.UatpAmountDifference, rejectionMemoRecord.AcceptedUatpAmount - rejectionMemoRecord.AllowedUatpAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Amount Difference", Convert.ToString(rejectionMemoRecord.UatpAmountDifference),
                                              invoice, fileName, errorLevel, ErrorCodes.UatpAmountDifferenceShouldMatchWithAcceptedMinusBilled, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //If vat amount is > 0 and vat breakdowns is 0
      if (rejectionMemoRecord.RejectionMemoVat != null && rejectionMemoRecord.CouponBreakdownRecord.Count == 0 && Convert.ToDouble(rejectionMemoRecord.TotalVatAmountDifference) > 0 && rejectionMemoRecord.RejectionMemoVat.Count() == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount", rejectionMemoRecord.TotalVatAmountDifference.ToString(),
                                                            invoice, fileName, errorLevel, ErrorCodes.ZeroVatBreakdownRecords, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (rejectionMemoRecord.CouponBreakdownRecord.Count > 0)
      {
        double rejectionMemoGrossAmountBilled = 0, rejectionMemoGrossAmountAccepted = 0, rejectionMemoGrossAmountDifference = 0;
        double rejectionMemoTaxAmountBilled = 0, rejectionMemoTaxAmountAccepted = 0, rejectionMemoTaxAmountDifference = 0;
        double rejectionMemoVatAmountBilled = 0, rejectionMemoVatAmountAccepted = 0, rejectionMemoVatAmountDifference = 0;
        double rejectionMemoHandlingFeeAmountAllowed = 0, rejectionMemoHandlingFeeAmountAccepted = 0, rejectionMemoHandlingFeeAmountDifference = 0;
        double rejectionMemoIscAmountAllowed = 0, rejectionMemoIscAmountAccepted = 0, rejectionMemoIscAmountDifference = 0;
        double rejectionMemoOtherCommissionAmountAllowed = 0, rejectionMemoOtherCommissionAmountAccepted = 0, rejectionMemoOtherCommissionAmountDifference = 0;
        double rejectionMemoUatpAmountAllowed = 0, rejectionMemoUatpAmountAccepted = 0, rejectionMemoUatpAmountDifference = 0;
        double netRejectAmount = 0;

        foreach (var currentRecord in rejectionMemoRecord.CouponBreakdownRecord)
        {
          //Gross amount
          rejectionMemoGrossAmountBilled += currentRecord.GrossAmountBilled;
          rejectionMemoGrossAmountAccepted += currentRecord.GrossAmountAccepted;
          rejectionMemoGrossAmountDifference += currentRecord.GrossAmountDifference;
          //Tax amount
          rejectionMemoTaxAmountBilled += currentRecord.TaxAmountBilled;
          rejectionMemoTaxAmountAccepted += currentRecord.TaxAmountAccepted;
          rejectionMemoTaxAmountDifference += currentRecord.TaxAmountDifference;
          //Vat amount
          rejectionMemoVatAmountBilled += currentRecord.VatAmountBilled;
          rejectionMemoVatAmountAccepted += currentRecord.VatAmountAccepted;
          rejectionMemoVatAmountDifference += currentRecord.VatAmountDifference;
          //Handling fee amount          
          rejectionMemoHandlingFeeAmountAllowed += currentRecord.AllowedHandlingFee;
          rejectionMemoHandlingFeeAmountAccepted += currentRecord.AcceptedHandlingFee;
          rejectionMemoHandlingFeeAmountDifference += currentRecord.HandlingDifference;
          //ISC amount
          rejectionMemoIscAmountAllowed += currentRecord.AllowedIscAmount;
          rejectionMemoIscAmountAccepted += currentRecord.AcceptedIscAmount;
          rejectionMemoIscAmountDifference += currentRecord.IscDifference;
          //Other commission amount
          rejectionMemoOtherCommissionAmountAllowed += currentRecord.AllowedOtherCommission;
          rejectionMemoOtherCommissionAmountAccepted += currentRecord.AcceptedOtherCommission;
          rejectionMemoOtherCommissionAmountDifference += currentRecord.OtherCommissionDifference;
          //UATP amount
          rejectionMemoUatpAmountAllowed += currentRecord.AllowedUatpAmount;
          rejectionMemoUatpAmountAccepted += currentRecord.AcceptedUatpAmount;
          rejectionMemoUatpAmountDifference += currentRecord.UatpDifference;
          netRejectAmount += currentRecord.NetRejectAmount;
        }

        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(rejectionMemoGrossAmountBilled, rejectionMemoRecord.TotalGrossAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Amount Billed", Convert.ToString(rejectionMemoRecord.TotalGrossAmountBilled), invoice, fileName, errorLevel, ErrorCodes.GrossAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoGrossAmountAccepted, rejectionMemoRecord.TotalGrossAcceptedAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Amount Accepted", Convert.ToString(rejectionMemoRecord.TotalGrossAcceptedAmount), invoice, fileName, errorLevel, ErrorCodes.GrossAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoGrossAmountDifference, rejectionMemoRecord.TotalGrossDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Amount Difference", Convert.ToString(rejectionMemoRecord.TotalGrossDifference), invoice, fileName, errorLevel, ErrorCodes.GrossAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          //Tax amount validations
          if (!CompareUtil.Compare(rejectionMemoTaxAmountBilled, rejectionMemoRecord.TotalTaxAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Billed", Convert.ToString(rejectionMemoRecord.TotalTaxAmountBilled), invoice, fileName, errorLevel, ErrorCodes.TaxAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoTaxAmountAccepted, rejectionMemoRecord.TotalTaxAmountAccepted, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Accepted", Convert.ToString(rejectionMemoRecord.TotalTaxAmountAccepted), invoice, fileName, errorLevel, ErrorCodes.TaxAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoTaxAmountDifference, rejectionMemoRecord.TotalTaxAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount Difference", Convert.ToString(rejectionMemoRecord.TotalTaxAmountDifference), invoice, fileName, errorLevel, ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          //Vat amount validations
          if (!CompareUtil.Compare(rejectionMemoVatAmountBilled, rejectionMemoRecord.TotalVatAmountBilled, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Billed", Convert.ToString(rejectionMemoRecord.TotalVatAmountBilled), invoice, fileName, errorLevel, ErrorCodes.VatAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoVatAmountAccepted, rejectionMemoRecord.TotalVatAmountAccepted, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Accepted", Convert.ToString(rejectionMemoRecord.TotalVatAmountAccepted), invoice, fileName, errorLevel, ErrorCodes.VatAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoVatAmountDifference, rejectionMemoRecord.TotalVatAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount Difference", Convert.ToString(rejectionMemoRecord.TotalVatAmountDifference), invoice, fileName, errorLevel, ErrorCodes.VatAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          //Handling fee amount validations  

          if (!CompareUtil.Compare(rejectionMemoHandlingFeeAmountAllowed, rejectionMemoRecord.AllowedHandlingFee, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed Handling Fee Amount", Convert.ToString(rejectionMemoRecord.AllowedHandlingFee), invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(rejectionMemoHandlingFeeAmountAccepted, rejectionMemoRecord.AcceptedHandlingFee, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Accepted Handling Fee Amount", Convert.ToString(rejectionMemoRecord.AcceptedHandlingFee), invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (!CompareUtil.Compare(rejectionMemoHandlingFeeAmountDifference, rejectionMemoRecord.HandlingFeeAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Amount Difference", Convert.ToString(rejectionMemoRecord.HandlingFeeAmountDifference), invoice, fileName, errorLevel, ErrorCodes.HandlingFeeAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //ISC amount validations
          if (!CompareUtil.Compare(rejectionMemoIscAmountAllowed, rejectionMemoRecord.AllowedIscAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed ISC Amount", Convert.ToString(rejectionMemoRecord.AllowedIscAmount), invoice, fileName, errorLevel, ErrorCodes.IscAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoIscAmountAccepted, rejectionMemoRecord.AcceptedIscAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Accepted ISC Amount", Convert.ToString(rejectionMemoRecord.AcceptedIscAmount), invoice, fileName, errorLevel, ErrorCodes.IscAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoIscAmountDifference, rejectionMemoRecord.IscDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "ISC Amount Difference", Convert.ToString(rejectionMemoRecord.IscDifference), invoice, fileName, errorLevel, ErrorCodes.IscAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          //Other commission amount validations.
          if (!CompareUtil.Compare(rejectionMemoOtherCommissionAmountAllowed, rejectionMemoRecord.AllowedOtherCommission, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed Other Commission Amount", Convert.ToString(rejectionMemoRecord.AllowedOtherCommission), invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoOtherCommissionAmountAccepted, rejectionMemoRecord.AcceptedOtherCommission, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed Other Commission Amount", Convert.ToString(rejectionMemoRecord.AcceptedOtherCommission), invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoOtherCommissionAmountDifference, rejectionMemoRecord.OtherCommissionDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Difference", Convert.ToString(rejectionMemoRecord.OtherCommissionDifference), invoice, fileName, errorLevel, ErrorCodes.OtherCommissionAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //UATP amount validations
          if (!CompareUtil.Compare(rejectionMemoUatpAmountAllowed, rejectionMemoRecord.AllowedUatpAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed UATP Amount", Convert.ToString(rejectionMemoRecord.AllowedUatpAmount), invoice, fileName, errorLevel, ErrorCodes.UatpAmountBilledIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoUatpAmountAccepted, rejectionMemoRecord.AcceptedUatpAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Accepted UATP Amount", Convert.ToString(rejectionMemoRecord.AcceptedUatpAmount), invoice, fileName, errorLevel, ErrorCodes.UatpAmountAcceptedIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(rejectionMemoUatpAmountDifference, rejectionMemoRecord.UatpAmountDifference, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Amount Difference", Convert.ToString(rejectionMemoRecord.UatpAmountDifference), invoice, fileName, errorLevel, ErrorCodes.UatpAmountDifferenceIsNotMatchingWithSumOfBreakdowns, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Total net reject amount
          if (!CompareUtil.Compare(Convert.ToDecimal(netRejectAmount), rejectionMemoRecord.TotalNetRejectAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount", Convert.ToString(rejectionMemoRecord.TotalNetRejectAmount), invoice, fileName, errorLevel, ErrorCodes.NetRejectAmountDoesNotMatchWithSumOfChildRecords, ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Total net reject amount
      if (invoice.Tolerance != null)
      {
        if (!CompareUtil.Compare(Convert.ToDouble(rejectionMemoRecord.TotalNetRejectAmount),
                                 ConvertUtil.Round((rejectionMemoRecord.TotalGrossDifference + rejectionMemoRecord.TotalTaxAmountDifference + rejectionMemoRecord.IscDifference + rejectionMemoRecord.OtherCommissionDifference +
                                             rejectionMemoRecord.HandlingFeeAmountDifference + rejectionMemoRecord.UatpAmountDifference + rejectionMemoRecord.TotalVatAmountDifference),
                                            Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount",
                                                                          Convert.ToString(rejectionMemoRecord.TotalNetRejectAmount),
                                                                          invoice, fileName, errorLevel, ErrorCodes.NetRejectAmountDoesNotMatchWithSumOfDiffAmount, ErrorStatus.X,
                                                                          rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Total net reject amount should not be 0 or negative
      if (Convert.ToDouble(rejectionMemoRecord.TotalNetRejectAmount) < 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount",
                                                                        Convert.ToString(rejectionMemoRecord.TotalNetRejectAmount),
                                                                        invoice, fileName, errorLevel, ErrorCodes.RejectionMemoNetTotalAmountShouldNotBeNegative, ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Sampling constant and Total net reject amount after sampling constant
      if (billingCode == (int)BillingCode.SamplingFormF || billingCode == (int)BillingCode.SamplingFormXF)
      {
        if (rejectionMemoRecord.SamplingConstant == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sampling Constant", Convert.ToString(rejectionMemoRecord.SamplingConstant),
                                              invoice, fileName, errorLevel, ErrorCodes.SamplingConstantShouldNotBeBlank, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.Tolerance != null && !CompareUtil.Compare(rejectionMemoRecord.TotalNetRejectAmountAfterSamplingConstant, (rejectionMemoRecord.TotalNetRejectAmount * Convert.ToDecimal(rejectionMemoRecord.SamplingConstant)), invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount After Sampling Constant", Convert.ToString(rejectionMemoRecord.TotalNetRejectAmountAfterSamplingConstant),
                                            invoice, fileName, errorLevel, SamplingErrorCodes.NetRejectAmountAfterScDoesNotMatchWithScXRejectAmount, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else
      {
        if (rejectionMemoRecord.SamplingConstant != 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Sampling Constant", Convert.ToString(rejectionMemoRecord.SamplingConstant),
                                              invoice, fileName, errorLevel, ErrorCodes.SamplingConstantShouldBeBlankForNSInvoice, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (rejectionMemoRecord.TotalNetRejectAmountAfterSamplingConstant != 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount After Sampling Constant", Convert.ToString(rejectionMemoRecord.TotalNetRejectAmountAfterSamplingConstant),
                                            invoice, fileName, errorLevel, SamplingErrorCodes.NetRejectAmountAfterScShouldBeZero, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      return isValid;
    }

    /// <summary>
    /// Validates the rejection memo record db.
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="invoice"></param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="rejectionMinAmounts"></param>

    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    private bool ValidateParsedRejectionMemoRecord(RejectionMemo rejectionMemoRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate, ExchangeRate exchangeRate, IList<MinAcceptableAmount> rejectionMinAmounts, IList<MaxAcceptableAmount> rejectionMaxAmounts, MaxAcceptableAmount couponMaxAcceptableAmount, BillingPeriod billingPeriod, PassengerConfiguration passengerConfiguration)
    {
      var isValid = true;
      var errorLevel = string.Empty;
      var errorLevelVat = string.Empty;
      var rejectionMemoNumber = string.Empty;
      TransactionType transType = 0;
      TransactionType transTypeMigration = 0;
      BillingCode billingCode;
      bool outcomeOfMismatchOnRmBilledOrAllowedAmounts = ValidationCache.Instance.PaxRmBilledAllowedAmount;
      var currDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
      var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
      
      var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == invoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
      DateTime yourInvoiceBillingDate;
      //To avoid converting year 30 into year 1930
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      bool isValidYourInvoicedate = false;
      var yourInvoiceDateString = string.Format("{2}{1}{0}",
                                     Convert.ToString(rejectionMemoRecord.YourInvoiceBillingPeriod).PadLeft(2, '0'),
                                     Convert.ToString(rejectionMemoRecord.YourInvoiceBillingMonth).PadLeft(2, '0'),
                                     Convert.ToString(rejectionMemoRecord.YourInvoiceBillingYear).PadLeft(4, '0'));
      if (DateTime.TryParseExact(yourInvoiceDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out yourInvoiceBillingDate))
      {
        if (yourInvoiceBillingDate.Day >= 1 && yourInvoiceBillingDate.Day <= 4)
        {
          isValidYourInvoicedate = true;
        }
        else
        {
          //Raise NonCorrectable error for invalid your invoice Date.
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Billing Date", yourInvoiceDateString,
                                                   invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceBillingDatePeriod, ErrorStatus.X, rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;

        }
      }
      else
      {
        //Raise NonCorrectable error for invalid your invoice Date.
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Billing Date", yourInvoiceDateString,
                                                  invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceBillingDatePeriod, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      bool ignoreValidationInMigrationPeriod = false;
      bool IsFimBmCmRmFound = false;
      if (rejectionMemoRecord.FimBMCMNumber != null)
          rejectionMemoRecord.FimBMCMNumber = rejectionMemoRecord.FimBMCMNumber.Trim();

      if (rejectionMemoRecord.RejectionMemoNumber != null)
      {
        rejectionMemoNumber = rejectionMemoRecord.RejectionMemoNumber;
      }
     
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        if (isValidYourInvoicedate)
        {
          invoice.IgnoreValidationInMigrationPeriodRm1 = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.RejectionMemo1,passengerConfiguration);
          invoice.IgnoreValidationInMigrationPeriodCoupon = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.Coupon, passengerConfiguration);
          invoice.IgnoreValidationInMigrationPeriodBm = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.BillingMemo, passengerConfiguration);
          invoice.IgnoreValidationInMigrationPeriodCm = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.CreditMemo, passengerConfiguration);
        }
        switch (rejectionMemoRecord.RejectionStage)
        {
          case (int)RejectionStage.StageOne:
            transType = TransactionType.RejectionMemo1;
            transTypeMigration = TransactionType.Coupon;

            if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber)
            {
              ignoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodBm;
            }
            else if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
            {
              ignoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodCm;
            }
            else
            {
              ignoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodCoupon;
            }

            break;
          case (int)RejectionStage.StageTwo:
            transType = TransactionType.RejectionMemo2;
            transTypeMigration = TransactionType.RejectionMemo1;
            ignoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodRm1;
            break;
          case (int)RejectionStage.StageThree:
            transType = TransactionType.RejectionMemo3;
            transTypeMigration = TransactionType.RejectionMemo1;
            ignoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodRm1;
            break;
        }
        billingCode = BillingCode.NonSampling;
        errorLevel = ErrorLevels.ErrorLevelRejectionMemo;
        errorLevelVat = ErrorLevels.ErrorLevelRejectionMemoVat;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
      {
        transType = TransactionType.SamplingFormF;
        billingCode = BillingCode.SamplingFormF;
        errorLevel = ErrorLevels.ErrorLevelSamplingFormF;
        errorLevelVat = ErrorLevels.ErrorLevelSamplingFormFVat;
        transTypeMigration = TransactionType.SamplingFormD;
        if (isValidYourInvoicedate)
        {
          ignoreValidationInMigrationPeriod = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.SamplingFormD, passengerConfiguration);
        }
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        transType = TransactionType.SamplingFormXF;
        billingCode = BillingCode.SamplingFormXF;
        errorLevel = ErrorLevels.ErrorLevelSamplingFormXF;
        errorLevelVat = ErrorLevels.ErrorLevelSamplingFormXFVat;
        transTypeMigration = TransactionType.SamplingFormF;
        if (isValidYourInvoicedate)
        {
          ignoreValidationInMigrationPeriod = IgnoreValidationInMigrationPeriod(invoice, rejectionMemoRecord, TransactionType.SamplingFormF, passengerConfiguration);
        }
      }
      string clearingHouse = string.Empty;
      if (invoice.SettlementMethodId == (int)SMI.Ach)
      {
        clearingHouse = "A";
      }
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH or Bilateral */
      else if (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules || ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true))
      {
        clearingHouse = "I";
      }

      //Validate Time Limit
      //CMP#641: Time Limit Validation on Third Stage PAX Rejections
      if (!string.IsNullOrEmpty(clearingHouse) && rejectionMemoRecord.RejectionStage != (int)RejectionStage.StageThree)
      {
        if (billingPeriod != null && IsTransactionOutSideTimeLimit(rejectionMemoRecord, invoice, billingPeriod))
        {
          var iSValidationFlag = rejectionMemoRecord.ISValidationFlag ?? string.Empty;
          rejectionMemoRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, "TL");
        }
      }

      if ((rejectionMemoRecord.FimCouponNumber < 0 || rejectionMemoRecord.FimCouponNumber > 4) && rejectionMemoRecord.FimCouponNumber != 9)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Coupon Number", Convert.ToString(rejectionMemoRecord.FimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.InvalidFimCouponNumber, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      #region SCP304023 - Source code ‘4’ have ‘FIM coupon number’ as ‘ZERO’
      //When ‘FIM BM CM Indicator’ is provided as F, and ‘FIM Coupon Number’ is provided as 0, it will lead to error
      if (rejectionMemoRecord.FimCouponNumber == 0 && rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber )
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Coupon Number", Convert.ToString(rejectionMemoRecord.FimCouponNumber), invoice, fileName, ErrorLevels.ErrorLevelRejectionMemo, ErrorCodes.InvalidFimCouponNumber, ErrorStatus.C, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      //For IS-IDEC/IS-XML submissions:
      //if ‘FIM Coupon Number’ is provided as “0”, the coupon number will be interpreted as null and no value will be inserted
      if (rejectionMemoRecord.FimCouponNumber == 0) rejectionMemoRecord.FimCouponNumber = null;

      #endregion

      //Billing period should be 01,02,03,04
      //if (rejectionMemoRecord.YourInvoiceBillingPeriod <= 0 || rejectionMemoRecord.YourInvoiceBillingPeriod > 4)
      //{  
      //  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Billing Period",
      //                                                                  string.Format("{0}{1}{2}", rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth,
      //                                                                                rejectionMemoRecord.YourInvoiceBillingPeriod), invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceBillingPeriod,
      //                                                                  ErrorStatus.C, rejectionMemoRecord);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}

      //FIM Number and FIM Coupon Number should be blank or both fields should be captured.
      //if (rejectionMemoRecord.FimCouponNumber != null && rejectionMemoRecord.FimBMCMNumber != null)
      //{
      //  long rmFimBmCmNumber = 0;
      //  if (rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber && rejectionMemoRecord.FimCouponNumber != 0 && (Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber) == false || (Int64.TryParse(rejectionMemoRecord.FimBMCMNumber, out rmFimBmCmNumber) && rmFimBmCmNumber == 0)))
      //  {
      //    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
      //                                                                    "FIM Number and FIM Coupon Number",
      //                                                                    string.Format("{0},{1}", Convert.ToString(rejectionMemoRecord.FimBMCMNumber), Convert.ToString(rejectionMemoRecord.FimCouponNumber)),
      //                                                                    invoice, fileName, errorLevel, ErrorCodes.MandatoryFimNumberAndCouponNumber, ErrorStatus.X, rejectionMemoRecord);
      //    exceptionDetailsList.Add(validationExceptionDetail);
      //    isValid = false;
      //  }
      //}

      //Validate Batch Sequence Number and Record Sequence Number
      if (rejectionMemoRecord.RecordSequenceWithinBatch <= 0 || rejectionMemoRecord.BatchSequenceNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Batch Sequence Number - Record Sequence Number", string.Format("{0}-{1}", rejectionMemoRecord.BatchSequenceNumber, rejectionMemoRecord.RecordSequenceWithinBatch),
                                                   invoice, fileName, errorLevel, ErrorCodes.InvalidBatchSequenceNoAndRecordNo, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //RM Coupon Breakdown Record is mandatory for 8C reason code.)
      if (rejectionMemoRecord.ReasonCode != null)
      {
        var isCouponBreakdownMandatory = invoice.ValidReasonCodes != null
                                           ? invoice.ValidReasonCodes.Count(
                                             reasonCode =>
                                             reasonCode.Code.Equals(rejectionMemoRecord.ReasonCode, StringComparison.OrdinalIgnoreCase) &&
                                             reasonCode.TransactionTypeId == (int)transType &&
                                             reasonCode.CouponAwbBreakdownMandatory) > 0
                                           : ReasonCodeRepository.GetCount(
                                             reasonCode =>
                                             reasonCode.Code.Equals(rejectionMemoRecord.ReasonCode, StringComparison.OrdinalIgnoreCase) &&
                                             reasonCode.TransactionTypeId == (int)transType &&
                                             reasonCode.CouponAwbBreakdownMandatory) > 0;


        if (isCouponBreakdownMandatory)
        { 
          if (rejectionMemoRecord.CouponBreakdownRecord.Count == 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
                                                                            "Rejection Memo Record", Convert.ToString(rejectionMemoRecord.ReasonCode),
                                                                            invoice, fileName, errorLevel, ErrorCodes.MandatoryCouponBreakdownRecord,
                                                                            ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }

        //Reason Code “1A” cannot be used for RMs having more than one coupon breakdown record.
        if (rejectionMemoRecord.ReasonCode.ToUpper().Equals("1A") && rejectionMemoRecord.CouponBreakdownRecord.Count > 1)
        {
            /* SCP# 115522 : Reason code - shows as Error Correctable
             * Desc: Code is corrected to treate this error as Non Correctable error (ErrorStatus.X)
             * Date: 02-05-2013
             */
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Reason Code", rejectionMemoRecord.ReasonCode,
                                                                          invoice, fileName, errorLevel, ErrorCodes.InvalidRejectionMemoRecordForReasonCode1A,
                                                                          ErrorStatus.X, rejectionMemoRecord, true);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        var diffenceNotAcceptable = ValidateAcceptableDifferences(rejectionMemoRecord.ReasonCode,
                                                                  transType,
                                                                  rejectionMemoRecord.TotalGrossDifference,
                                                                  rejectionMemoRecord.TotalTaxAmountDifference,
                                                                  rejectionMemoRecord.TotalVatAmountDifference,
                                                                  rejectionMemoRecord.IscDifference,
                                                                  rejectionMemoRecord.UatpAmountDifference,
                                                                  rejectionMemoRecord.HandlingFeeAmountDifference,
                                                                  rejectionMemoRecord.OtherCommissionDifference,
                                                                  invoice.ValidReasonCodes,
                                                                  invoice.ValidRMReasonAcceptableDiff);

        if (diffenceNotAcceptable != null && !string.IsNullOrEmpty(diffenceNotAcceptable.Trim()))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Difference Amount",
                                                                          rejectionMemoRecord.ReasonCode,
                                                                          invoice,
                                                                          fileName,
                                                                          errorLevel,
                                                                          ErrorCodes.InvalidAcceptableAmountDifference,
                                                                          ErrorStatus.X,
                                                                          rejectionMemoRecord);
          //SCP117329 : Validation report error (R2 file)
          //validationExceptionDetail.FileName = string.Format("{0} {1}", diffenceNotAcceptable, "Difference Amount");
          validationExceptionDetail.ErrorDescription = string.Format("{0} {1}", diffenceNotAcceptable, validationExceptionDetail.ErrorDescription);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //Duplicate check in current invoice - Rejection memo number
      if (invoice.RejectionMemoRecord.Where(rejection => rejection.RejectionMemoNumber == rejectionMemoRecord.RejectionMemoNumber).Count() > 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Rejection Memo Number",
                                                                        rejectionMemoRecord.RejectionMemoNumber,
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.DuplicateRejectionMemoNumber,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
      }
      // Should be a unique number within each Billed Airline in the Billing period.
      else if (IsDuplicateRejectionMemoNumber(rejectionMemoRecord, null, false, invoice))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Rejection Memo Number",
                                                                        rejectionMemoRecord.RejectionMemoNumber,
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.DuplicateRejectionMemoNumber,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
      }


      //Validate your Rejection Number
      PaxInvoice yourInvoice = null;
      RejectionMemo yourRejectionMemoRecord = null;
      BillingMemo yourBillingMemoRecord = null;
      CreditMemo yourCreditMemoRecord = null;
      PrimeCoupon yourFimRecord = null;
      var billingMemberId = invoice.BilledMemberId;

      var isXmlfileType = (invoice.SubmissionMethod == SubmissionMethod.IsXml) ? true : false;

      //Validate your Invoice Number/coupon Number/Rejection Coupon Number
      var couponSearchCriterias = new List<CouponSearchCriteria>();
      foreach (var rmCoupon in rejectionMemoRecord.CouponBreakdownRecord)
      {
        couponSearchCriterias.Add(new CouponSearchCriteria() { TicketCouponNo = rmCoupon.TicketOrFimCouponNumber, TicketDocNo = rmCoupon.TicketDocOrFimNumber, TicketIssuingAirline = rmCoupon.TicketOrFimIssuingAirline });
      }

      // YourInvoiceNumber mandatory check for Rejection Stage "1" or "2" or "3".
      if(string.IsNullOrWhiteSpace(rejectionMemoRecord.YourInvoiceNumber) && (rejectionMemoRecord.RejectionStage == 1 || rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Invoice Number", rejectionMemoRecord.YourInvoiceNumber, invoice, fileName, errorLevel, ErrorCodes.MandatoryYourInvoiceNumberForRej1Rej2Rej3, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // SCP # 56198 - Query on the file quality
      // Your Rejection Memo No. cannot be provided if PAX Non-Sampling RM stage 1 (Billing Code 0)
      // and PAX Sampling Form F (Billing Code 6)
      if (!string.IsNullOrWhiteSpace(rejectionMemoRecord.YourRejectionNumber) && ((invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoRecord.RejectionStage == 1)) || invoice.BillingCode == (int)BillingCode.SamplingFormF))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Your Rejection Memo Number",
                                                                        rejectionMemoRecord.YourRejectionNumber,
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.YourRejectionNumberNotRequired,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // YourRejectionNumber mandatory check for
      // 1. Rejection Stage "2" or "3" and Billing Code = 0.
      // 2. Billing Code = 7.
      if (string.IsNullOrWhiteSpace(rejectionMemoRecord.YourRejectionNumber) && ((invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)) || invoice.BillingCode == (int) BillingCode.SamplingFormXF))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Rejection Memo Number", rejectionMemoRecord.YourRejectionNumber, invoice, fileName, errorLevel, ErrorCodes.MandatoryYourRejectionMemoNumberForRej2Rej3, ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate your Invoice Number/coupon Number/Rejection Coupon Number
      if (rejectionMemoRecord.YourInvoiceNumber != null)
      {
          var yourInvCriteria = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                                  rejectionMemoRecord.YourInvoiceNumber,
                                                  rejectionMemoRecord.YourInvoiceBillingMonth,
                                                  rejectionMemoRecord.YourInvoiceBillingYear,
                                                  rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                  invoice.BilledMemberId,
                                                  invoice.BillingMemberId,
                                                  invoice.BillingCode);

        if (invoice.BillingCode == (int)BillingCode.NonSampling)
        {
          // Use LS to get your invoice with Coupons, RM and RM Coupons. Billing member and billed member will be exchanged.
          // Get rejection invoice with Prime Coupon only for Rejection Stage 1.
          if (rejectionMemoRecord.RejectionStage == 1)
          {
            Logger.InfoFormat("1. YourInvoice-Month-Year-Period-BilledMember-BillingMember-BillinCode: {0}-{1}-{2}-{3}-{4}-{5}-{6}", rejectionMemoRecord.YourInvoiceNumber,
                                             rejectionMemoRecord.YourInvoiceBillingMonth,
                                             rejectionMemoRecord.YourInvoiceBillingYear,
                                             rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId,
                                             invoice.BillingMemberId, (int)BillingCode.NonSampling);

            int yourInvPresent = (int)TransactionDbStatus.NotSet;

            if (IsYourInvoiceNumberPresent.TryGetValue(yourInvCriteria, out yourInvPresent) && yourInvPresent == (int)TransactionDbStatus.Found)
            {

              //Logger.InfoFormat("Fetch Your Invoice: {0} with coupons for RM1 Invoice: {1}. Start",rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

              //SCP400648 - KAL: RM linking logic in validation Service
              //Remove entity framework and ado.net for fetching invoice detail and prime coupon from database. 
              yourInvoice = paxValidationManager.GetInvoiceWithPrimeCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                         rejectionMemoRecord.YourInvoiceBillingMonth,
                                         rejectionMemoRecord.YourInvoiceBillingYear,
                                         rejectionMemoRecord.YourInvoiceBillingPeriod,
                                         invoice.BilledMemberId,
                                         invoice.BillingMemberId,
                                         (int)BillingCode.NonSampling,
                                        ((int)InvoiceStatusType.Presented).ToString(),
                                         CreateCouponSearchCriteriaString(couponSearchCriterias), rejectionMemoRecord.CouponBreakdownRecord == null ? false : rejectionMemoRecord.CouponBreakdownRecord.Count > 0 ? true : false);
    
              //Logger.InfoFormat("Fetch Your Invoice: {0} with coupons for RM1 Invoice: {1}.End",rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

            }// End if
            else if (yourInvPresent == (int)TransactionDbStatus.NotSet)
            {
              //Logger.InfoFormat("Fetch Your Invoice: {0} with coupons for RM1 Invoice: {1}. Start",rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

              //SCP400648 - KAL: RM linking logic in validation Service
              //Remove entity framework and call direct using ADO.net
              yourInvoice = paxValidationManager.GetInvoiceWithPrimeCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                          rejectionMemoRecord.YourInvoiceBillingMonth,
                                          rejectionMemoRecord.YourInvoiceBillingYear,
                                          rejectionMemoRecord.YourInvoiceBillingPeriod,
                                          invoice.BilledMemberId,
                                          invoice.BillingMemberId, 
                                          (int)BillingCode.NonSampling,
                                         ((int)InvoiceStatusType.Presented).ToString(),
                                          CreateCouponSearchCriteriaString(couponSearchCriterias),rejectionMemoRecord.CouponBreakdownRecord == null ? false : rejectionMemoRecord.CouponBreakdownRecord.Count > 0 ? true : false);
    

              //Logger.InfoFormat("Fetch Your Invoice: {0} with coupons for RM1 Invoice: {1}. End",rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);
              
              IsYourInvoiceNumberPresent.Add(yourInvCriteria, (yourInvoice == null ? (int)TransactionDbStatus.NotFound : (int)TransactionDbStatus.Found));

            }// End else if
            else
            {
                yourInvoice = null;
            }// End else
          }// End if Stage1
          else
          {
            Logger.InfoFormat("2. YourInvoice-Month-Year-Period-BilledMember-BillingMember-BillinCode-YourRejectionMemo: {0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", rejectionMemoRecord.YourInvoiceNumber,
                                               rejectionMemoRecord.YourInvoiceBillingMonth,
                                               rejectionMemoRecord.YourInvoiceBillingYear,
                                               rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId,
                                               invoice.BillingMemberId, (int)BillingCode.NonSampling, rejectionMemoRecord.YourRejectionNumber);

            // SCP51931: File stuck in Production
            // Get rejection invoice with RM and RM Coupon for Rejection Stage 2 and 3 based on following filter craterias:
            // 1. YourInvoice
            // 2. Your Month, Yearand Period
            // 3. BilledMember
            // 4. BillingMember
            // 5. BillinCode
            // 6. YourRejectionMemo: New parameter added. Earlier all rejection memos in Your Invoice were getting fetched instead of 
            //                       only the required rejection memo in Your Invoice due to which fetching your invoice with RM and 
            //                       RM Coupon was taking time, degrading the over all performance of Invoice Processor Service.

            int yourInvPresent = (int)TransactionDbStatus.NotSet;

            if (IsYourInvoiceNumberPresent.TryGetValue(yourInvCriteria, out yourInvPresent) && yourInvPresent == (int)TransactionDbStatus.Found)
            {
              Logger.InfoFormat("Fetch Your Invoice: {0} with RM coupons for RM2/3 Invoice: {1}. Start",
                                  rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

              //SCP400648 - KAL: RM linking logic in validation Service
              //Remove entity framework and use ado.net for fetching invoice detail and RM coupons from database.
              yourInvoice = paxValidationManager.GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                        rejectionMemoRecord.YourInvoiceBillingMonth,
                                                        rejectionMemoRecord.YourInvoiceBillingYear,
                                                        rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                        invoice.BilledMemberId, invoice.BillingMemberId, 
                                                        (int)BillingCode.NonSampling,
                                                        null, rejectionMemoRecord.YourRejectionNumber == null ? null : rejectionMemoRecord.YourRejectionNumber.Trim(),
                                                        ((int)InvoiceStatusType.Presented).ToString());

              Logger.InfoFormat("Fetch Your Invoice: {0} with RM coupons for RM2/3 Invoice: {1}. End",
                                  rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);
            }// End if
            else if (yourInvPresent == (int)TransactionDbStatus.NotSet)
            {
              Logger.InfoFormat("Fetch Your Invoice: {0} with RM coupons for RM2/3 Invoice: {1}. Start",
                                  rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

              //SCP400648 - KAL: RM linking logic in validation Service
              //Remove entity framework and use ado.net for fetching invoice detail and RM coupon from database.
              yourInvoice = paxValidationManager.GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                       rejectionMemoRecord.YourInvoiceBillingMonth,
                                                       rejectionMemoRecord.YourInvoiceBillingYear,
                                                       rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                       invoice.BilledMemberId, invoice.BillingMemberId, 
                                                       (int)BillingCode.NonSampling,
                                                       null, rejectionMemoRecord.YourRejectionNumber == null ? null : rejectionMemoRecord.YourRejectionNumber.Trim(),
                                                       ((int)InvoiceStatusType.Presented).ToString());

              Logger.InfoFormat("Fetch Your Invoice: {0} with RM coupons for RM2/3 Invoice: {1}. End",
                                  rejectionMemoRecord.YourRejectionNumber, invoice.InvoiceNumber);

              IsYourInvoiceNumberPresent.Add(yourInvCriteria, (yourInvoice == null ? (int)TransactionDbStatus.NotFound : (int)TransactionDbStatus.Found));
            }// End else if
            else
            {
                yourInvoice = null;
            }// End else
          }// End else Stage 2/3
        }// End if Non-Sampling
        else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
        {
            int yourInvPresent = (int)TransactionDbStatus.NotSet;
            if (IsYourInvoiceNumberPresent.TryGetValue(yourInvCriteria, out yourInvPresent) && yourInvPresent == (int)TransactionDbStatus.Found)
            {
                yourInvoice = InvoiceRepository.GetInvoiceWithFormDRecord(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormDE);
            }
            else if (yourInvPresent == (int)TransactionDbStatus.NotSet)
            {
                yourInvoice = InvoiceRepository.GetInvoiceWithFormDRecord(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormDE);
                IsYourInvoiceNumberPresent.Add(yourInvCriteria, (yourInvoice == null ? (int)TransactionDbStatus.NotFound : (int)TransactionDbStatus.Found));
            }
            else
            {
                yourInvoice = null;
            }
          
        }
        else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          // Get rejection invoice with RM and RM Coupon for Sampling For XF.
          // Get your invoice using load strategy.
            int yourInvPresent = (int)TransactionDbStatus.NotSet;
            if (IsYourInvoiceNumberPresent.TryGetValue(yourInvCriteria, out yourInvPresent) && yourInvPresent == (int)TransactionDbStatus.Found)
            {
                yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                        rejectionMemoRecord.YourInvoiceBillingMonth,
                                                        rejectionMemoRecord.YourInvoiceBillingYear,
                                                        rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                        invoice.BilledMemberId, invoice.BillingMemberId,
                                                        (int)BillingCode.SamplingFormF,
                                                        null);
            }
            else if (yourInvPresent == (int)TransactionDbStatus.NotSet)
            {
                yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                        rejectionMemoRecord.YourInvoiceBillingMonth,
                                                        rejectionMemoRecord.YourInvoiceBillingYear,
                                                        rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                        invoice.BilledMemberId, invoice.BillingMemberId,
                                                        (int)BillingCode.SamplingFormF,
                                                        null);
                IsYourInvoiceNumberPresent.Add(yourInvCriteria, (yourInvoice == null ? (int)TransactionDbStatus.NotFound : (int)TransactionDbStatus.Found));
            }
            else
            {
                yourInvoice = null;
            }
        }

        // If Your Invoice not found in SIS DB then check for member migration
        // If member migration date is grater than YourInvoice Billing date then PASS else check for IgnoreValidationFlag in system parameter
        // If IgnoreValidationFlag is true then PASS else raise Error.
        if (yourInvoice == null)
        {
          Logger.InfoFormat("Your Invoice Not found and ignoreValidationInMigrationPeriod Flag value: {0} ", ignoreValidationInMigrationPeriod);
          if (ignoreValidationInMigrationPeriod)
          {
              //CMP#641: Time Limit Validation on Third Stage PAX Rejections
              if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
              {
                  var transactionType = TransactionType.RejectionMemo3;
                  if(invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                  {
                      transactionType = TransactionType.SamplingFormXF;
                  }
                  bool rmInTimeLimt = ValidatePaxStageThreeRmForTimeLimit(transactionType,
                                                      invoice.SettlementMethodId,
                                                      rejectionMemoRecord,
                                                      invoice,
                                                      fileName: fileName,
                                                      fileSubmissionDate: fileSubmissionDate,
                                                      exceptionDetailsList: exceptionDetailsList);
                  if (!rmInTimeLimt) isValid = false;
              }
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Your Invoice Number.",
                                                                            rejectionMemoRecord.YourInvoiceNumber,
                                                                            invoice, fileName, errorLevel,
                                                                            ErrorCodes.YourInvoiceNotExists,
                                                                            ErrorStatus.W, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
          }
          else
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Your Invoice Number",
                                                                            rejectionMemoRecord.YourInvoiceNumber,
                                                                            invoice, fileName, errorLevel,
                                                                            ErrorCodes.YourInvoiceNotExists,
                                                                            ErrorStatus.C, rejectionMemoRecord,
                                                                            islinkingError: true);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        // If Your Invoice found in SIS DB then all audit trail should match.
        // regarless of member migration date & IgnoreValidationFlag in system parameter
        if (yourInvoice != null)
        {
            //CMP#624 : 2.11-New Validation #6:SMI Match Check for PAX/CGO RMs
            if (!ValidateSmiAfterLinking(invoice.SettlementMethodId, yourInvoice.SettlementMethodId))
            {
                /* CMP #624: ICH Rewrite-New SMI X 
            * Description: Code Fixed regarding bug #9214: CMP 624: Incorrect error on stage 1 Rejection if SMI X invoice is rejected by non X invoice from ISWEB. 
            * Instead of Exception SP is modified to return SMIException to show SMI related error message. */
                string errorCode = "";
                if (yourInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                {
                    /* ERROR_CODE_O := 'Exception: BPAXNS_10945 - Rejected Invoice/Credit Note was billed using Settlement Method X. 
                 * Invoices/Credit Notes billed using Settlement Method X can be rejected only by an Invoice using Settlement Method X*/
                    errorCode = ErrorCodes.PaxNSRejInvBHLinkingCheckForSmiX;
                }
                else
                {
                    /* Exception: BPAXNS_10943 - Rejected Invoice/Credit Note was billed using a Settlement Method other than X. 
                 * Only Invoices/Credit Notes billed using Settlement Method X can be rejected by an Invoice using Settlement Method X. */
                    errorCode = ErrorCodes.PaxNSRejctionInvoiceLinkingCheckForSmiX;
                }
                Logger.InfoFormat("Invoice SMI : {0} -- Your Invoice SMI {1}", invoice.SettlementMethodId,
                                  yourInvoice.SettlementMethodId);
                var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                exceptionDetailsList.Count() + 1,
                                                                                fileSubmissionDate,
                                                                                "Your Invoice Number",
                                                                                rejectionMemoRecord.YourInvoiceNumber,
                                                                                invoice, fileName, errorLevel,
                                                                                errorCode,
                                                                                ErrorStatus.X, rejectionMemoRecord,
                                                                                islinkingError: true);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            #region CMP#674: Validation of Coupon and AWB Breakdowns in Rejections

            if ((invoice.BillingCode == (int) BillingCode.NonSampling
                 || invoice.BillingCode == (int) BillingCode.SamplingFormXF)
                && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
                && yourInvoice != null && yourInvoice.RejectionMemoRecord != null &&
                yourInvoice.RejectionMemoRecord.Count() > 0)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Logger.InfoFormat("Coupon Breakdowns Rejections Validation started for Rejection Memo: [{0}]",
                                  rejectionMemoRecord.RejectionMemoNumber);

                var yourRjectionMemo =
                    yourInvoice.RejectionMemoRecord.Where(
                        rm => rm.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();

                if (yourRjectionMemo != null)
                {
                    /* Group Rejected/Your Invoice Coupons by Airline-DocNo-CpnNo */
                    IEnumerable<IGrouping<string, int>> rejectedRMCouponGroupings =
                        yourRjectionMemo.CouponBreakdownRecord.GroupBy(
                            rmCpn =>
                            string.Format("{0}-{1}-{2}", rmCpn.TicketOrFimIssuingAirline, rmCpn.TicketDocOrFimNumber,
                                          rmCpn.TicketOrFimCouponNumber), rmCpn => rmCpn.TicketOrFimCouponNumber);


                    char[] delimiter = new char[] {'-'};

                    /* Iterate over each Grouping of Airline-DocNo-CpnNo, in the Rejected RM Coupon collection. */
                    foreach (IGrouping<string, int> rmCpn in rejectedRMCouponGroupings)
                    {
                        //Rejected RM Coupon Uniqueue Key
                        var couponGroupKeyParts = rmCpn.Key.Split(delimiter);
                        var couponGroupKey_issuingAirline = couponGroupKeyParts[0];
                        var couponGroupKey_documentNumber = couponGroupKeyParts[1];
                        var couponGroupKey_couponNumber = couponGroupKeyParts[2];

                        int rejectedMemoCouponCount = rmCpn.Count();

                        /* Check if rejection invoice has that combination for same number of times. */
                        int rejectingMemoCouponCount = rejectionMemoRecord.CouponBreakdownRecord.Count(
                            cpn =>
                            cpn.TicketOrFimIssuingAirline == couponGroupKey_issuingAirline &&
                            cpn.TicketDocOrFimNumber == long.Parse(couponGroupKey_documentNumber) &&
                            cpn.TicketOrFimCouponNumber == int.Parse(couponGroupKey_couponNumber));

                        /* 
                         * This validation should not applied if below criteria meet
                         * If rejecting Invoice is FIM then rejected coupon should be less or equal to rejecting coupon
                         * Else rejected coupon should be equal to rejecting coupon.
                         */
                        var isFail = (rejectionMemoRecord.SourceCodeId == 45 || rejectionMemoRecord.SourceCodeId == 46)
                                         ? rejectedMemoCouponCount > rejectingMemoCouponCount
                                         : rejectedMemoCouponCount != rejectingMemoCouponCount;
                        if (isFail)
                        {
                            /* Report Error -
                              * Error Code - RejectionMemoCouponMissing = "BPAXNS_10978"
                              * Error Description - Mismatch in coupon {0}-{1}-{2}. It was billed {3} time(s) in the rejected RM; and {4} time(s) in this RM. 
                              *                     Other mismatches if any are not included in this report.
                              */
                            string errorDescription =
                                string.Format(
                                    Messages.ResourceManager.GetString(ErrorCodes.PaxRMCouponMismatchFileValidation),
                                    couponGroupKey_issuingAirline, couponGroupKey_documentNumber,
                                    couponGroupKey_couponNumber,
                                    rejectedMemoCouponCount, rejectingMemoCouponCount);

                            var validationExceptionDetail = CreateValidationExceptionDetail(
                                rejectionMemoRecord.Id.Value(),
                                exceptionDetailsList.Count() + 1,
                                fileSubmissionDate, "Rejection Memo",
                                rejectionMemoRecord.RejectionMemoNumber,
                                invoice, fileName, errorLevel,
                                ErrorCodes.PaxRMCouponMismatchFileValidation,
                                ErrorStatus.X, rejectionMemoRecord,
                                exceptionDesc: errorDescription);

                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;

                            /* error description should report the first mismatch identified */
                            break;
                        }
                        //else
                        //{
                        //    /* Validation Pass - For Logical Completion */
                        //}
                    }

                    stopwatch.Stop();
                    Logger.InfoFormat("Time elapsed for Coupon Breakdowns Rejections Validation: {0}", stopwatch.Elapsed);
                }
            }

          #endregion

          //Validate FIM Number is present in the original invoice 
          if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
          {
            if (rejectionMemoRecord.FimBMCMNumber != null && Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber))
            {
              if (rejectionMemoRecord.RejectionStage == 1)
              {
                // SCP77752: Implemented code to send FIM Coupon No. and FIM No. to the stored procedure so that only record matching the given FIM information is fetched from DB.
                //SCP400648 - KAL: RM linking logic in validation Service
                //Remove entity framework and use ado.net for fetching invoice detail and FIM coupon from database.
                yourInvoice = paxValidationManager.GetInvoiceWithPrimeCoupons(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.NonSampling, ((int)InvoiceStatusType.Presented).ToString(), string.Format("{0}:{1}:FIM", rejectionMemoRecord.FimCouponNumber, rejectionMemoRecord.FimBMCMNumber));

                //var yourFimRecordList = yourInvoice.CouponDataRecord.Where(couponRecord => couponRecord.TicketDocOrFimNumber == Convert.ToInt64(rejectionMemoRecord.FimBMCMNumber) && couponRecord.TicketOrFimCouponNumber == rejectionMemoRecord.FimCouponNumber).ToList();
                //var yourFimRecordList = yourInvoice.CouponDataRecord.Where(couponRecord => couponRecord.TicketDocOrFimNumber == Convert.ToInt64(rejectionMemoRecord.FimBMCMNumber.Trim()) && couponRecord.TicketOrFimCouponNumber == rejectionMemoRecord.FimCouponNumber && couponRecord.ISValidationFlag != "DU").ToList();

                //SCP 156447: FIM number with a leading zero
                //Desc: Stopping the typecasting of FimBMCMNumber field while validating to ensure exact match for linking.
                // Example - FimBMCMNumber = "0123" in file (xml node name = "LinkedFIMBillingCreditMemoNumber".
                //           Need to perform exact match in your invoice coupon for linking.
                //           Prior code use to typecast input value to ToInt64 hence FimBMCMNumber is treated as "123" instead of "0123".
                //           So validation was incorrect. Instead we now typecast TicketDocOrFimNumber comming from database into string.
                //           Correcting this now performs exact match to provide linking error if applicable.
                //Date: 25/07/2013

                //SCP 304023 - Source code ‘4’ have ‘FIM coupon number’ as ‘ZERO’
                var yourFimRecordList = new List<PrimeCoupon>();
                if (rejectionMemoRecord.FimCouponNumber == null)
                {
                  yourFimRecordList = yourInvoice.CouponDataRecord.Where(couponRecord => couponRecord.TicketDocOrFimNumber.ToString() == rejectionMemoRecord.FimBMCMNumber && (couponRecord.TicketOrFimCouponNumber == 0 || couponRecord.TicketOrFimCouponNumber == null)).ToList();
                }
                else
                {
                  yourFimRecordList = yourInvoice.CouponDataRecord.Where(couponRecord => couponRecord.TicketDocOrFimNumber.ToString() == rejectionMemoRecord.FimBMCMNumber && couponRecord.TicketOrFimCouponNumber == rejectionMemoRecord.FimCouponNumber).ToList();
                }

                if (yourFimRecordList.Count() == 0)
                {
                  Logger.InfoFormat("Your Fim Record : {0} is not found", rejectionMemoRecord.FimBMCMNumber);
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number", rejectionMemoRecord.FimBMCMNumber, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourFimNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  if (yourFimRecordList.FirstOrDefault() != null)
                  {
                    yourFimRecord = yourFimRecordList.FirstOrDefault();
                    Logger.InfoFormat("Your Fim Record: {0} is found", rejectionMemoRecord.FimBMCMNumber);
                    IsFimBmCmRmFound = true;
                    //CMP#459 : Validate Amount at memo leve in FIM R1
                    //If duplicate rejected FIMs are found in the rejected invoice, all amounts of at least one FIM coupon from the rejected invoice should match the RM level amounts of the rejecting RM
                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                      isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                  prevInvExRate,
                                                                  currInvExRate,
                                                                  rejectionMemoRecord,
                                                                  exceptionDetailsList,
                                                                  invoice,
                                                                  yourInvoice,
                                                                  null,
                                                                  yourFimRecordList.ToList(),
                                                                  null,
                                                                  fileName,
                                                                  fileSubmissionDate);
                  }
                }
              }
              else if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
              {
                var yourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber != null && rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber && rejectionRec.FimCouponNumber == rejectionMemoRecord.FimCouponNumber).ToList();

                if (yourRejectionMemoRecordList.Count() > 1)
                {
                  Logger.InfoFormat("There are multiple record found for rejection memo - " + rejectionMemoRecord.YourRejectionNumber);
                }

                if (yourRejectionMemoRecordList.Count() == 0)
                {
                  Logger.InfoFormat("Your Rejection Memo: {0} not found", rejectionMemoRecord.YourRejectionNumber);
                  var isYourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).ToList();

                  if (isYourRejectionMemoRecordList.Count() == 0)
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Your Rejection Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.
                                                                                        YourRejectionNumber), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.
                                                                                      AuditTrailFailForYourRejectionMemoNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                  }
                  else
                  {
                    //SCP146723 - Inquiry of error reason for FIM number
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Fim BM CM Number/ Fim BM CM Coupon Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.FimBMCMNumber
                                                                                        ), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.AuditTrailFailForFimNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);

                  }
                  isValid = false;
                }
                // Validate Rejection Stage.
                else if (yourRejectionMemoRecordList.FirstOrDefault() != null && yourRejectionMemoRecordList.FirstOrDefault().RejectionStage != (rejectionMemoRecord.RejectionStage - 1))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Rejection Stage", Convert.ToString(rejectionMemoRecord.RejectionStage), invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceRejectionStage, ErrorStatus.X, rejectionMemoRecord, islinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }

                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                {
                  yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                  Logger.InfoFormat("Your Rejection Memo: {0} is found", rejectionMemoRecord.YourRejectionNumber);
                  IsFimBmCmRmFound = true;
                  //CMP#459 : Validate Amount
                  //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                  var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                  var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                  isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                              prevInvExRate,
                                                              currInvExRate,
                                                                rejectionMemoRecord,
                                                                exceptionDetailsList,
                                                                invoice,
                                                                yourInvoice,
                                                                null,
                                                                null,
                                                                null,
                                                                fileName,
                                                                fileSubmissionDate);
                  //CMP#641: Time Limit Validation on Third Stage PAX Rejections
                  if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
                  {
                      var transactionType = TransactionType.RejectionMemo3;
                      if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                      {
                          transactionType = TransactionType.SamplingFormXF;
                      }
                      bool rmInTimeLimt = ValidatePaxStageThreeRmForTimeLimit(transactionType,
                                                      invoice.SettlementMethodId,
                                                      rejectionMemoRecord,
                                                      invoice,
                                                      fileName: fileName,
                                                      fileSubmissionDate: fileSubmissionDate,
                                                      exceptionDetailsList: exceptionDetailsList);
                      if (!rmInTimeLimt) isValid = false;
                  }
                }
              }
            }
            else
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number", rejectionMemoRecord.FimBMCMNumber ?? string.Empty, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourFimNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber)
          {
            if (rejectionMemoRecord.FimBMCMNumber != null && !string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber))
            {
              if (rejectionMemoRecord.RejectionStage == 1)
              {

                // Get rejection invoice with RM and RM Coupon for Rejection Stage 2 and 3.
                yourInvoice = GetInvoiceWithBMCoupons(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, billingMemberId, invoice.BillingMemberId, (int)BillingCode.NonSampling, null);

                var yourBillingMemoRecordList = yourInvoice.BillingMemoRecord.Where(billingMemo => billingMemo.BillingMemoNumber.Trim().ToUpper() == rejectionMemoRecord.FimBMCMNumber.ToUpper()).ToList();
                if (yourBillingMemoRecordList.Count() != 1)
                {
                  Logger.InfoFormat("Your Billing Memo: {0} is not found", rejectionMemoRecord.FimBMCMNumber);
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "BM Number", rejectionMemoRecord.FimBMCMNumber, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourBillingMemoNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  if (yourBillingMemoRecordList.FirstOrDefault() != null)
                  {
                    yourBillingMemoRecord = yourBillingMemoRecordList.FirstOrDefault();
                    Logger.InfoFormat("Your Billing Memo: {0} is found", rejectionMemoRecord.FimBMCMNumber);

                    //SCP219674 - Billing Memos with 6A & 6B can be rejected
                    if (yourBillingMemoRecord.ReasonCode == "6A" || yourBillingMemoRecord.ReasonCode == "6B")
                    {
                      var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "BM Number", rejectionMemoRecord.FimBMCMNumber, invoice, fileName, errorLevel, ErrorCodes.BMWithReasonCode6A6BcannotRejected, ErrorStatus.X, rejectionMemoRecord, islinkingError: true);
                      exceptionDetailsList.Add(validationExceptionDetail);
                      isValid = false;
                    }
                    else
                    {
                      IsFimBmCmRmFound = true;
                      //CMP#459 : Validate Amount
                      //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                      var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                      var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                      isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                  prevInvExRate,
                                                                  currInvExRate,
                                                                rejectionMemoRecord,
                                                                exceptionDetailsList,
                                                                invoice,
                                                                yourInvoice,
                                                                null,
                                                                null,
                                                                null,
                                                                fileName,
                                                                fileSubmissionDate);
                    }
                  }
                }
              }
              else if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
              {
                var yourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber !=null && rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber).ToList();

                if (yourRejectionMemoRecordList.Count() > 1)
                {
                  Logger.InfoFormat("There are multiple record found for rejection memo - " + rejectionMemoRecord.YourRejectionNumber);
                }

                if (yourRejectionMemoRecordList.Count() == 0)
                {
                  Logger.InfoFormat("Your Rejection Memo: {0} is not found", rejectionMemoRecord.YourRejectionNumber); 
                  var isYourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).ToList();

                  if (isYourRejectionMemoRecordList.Count() == 0)
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Your Rejection Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.
                                                                                        YourRejectionNumber), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.
                                                                                      AuditTrailFailForYourRejectionMemoNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                  else
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Fim BM CM Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.
                                                                                        YourRejectionNumber), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.AuditTrailFailForBmNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                }
                // Validate Rejection Stage.
                else if (yourRejectionMemoRecordList.FirstOrDefault() != null && yourRejectionMemoRecordList.FirstOrDefault().RejectionStage != (rejectionMemoRecord.RejectionStage - 1))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Rejection Stage", Convert.ToString(rejectionMemoRecord.RejectionStage), invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceRejectionStage, ErrorStatus.X, rejectionMemoRecord);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }

                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                {
                  yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                  Logger.InfoFormat("Your Rejection Memo: {0} is found", rejectionMemoRecord.YourRejectionNumber);
                  IsFimBmCmRmFound = true;
                  //CMP#459 : Validate Amount
                  //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                  var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                  var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                  isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                              prevInvExRate,
                                                              currInvExRate,
                                                              rejectionMemoRecord,
                                                              exceptionDetailsList,
                                                              invoice,
                                                              yourInvoice,
                                                              null,
                                                              null,
                                                              null,
                                                              fileName,
                                                              fileSubmissionDate);
                  //CMP#641: Time Limit Validation on Third Stage PAX Rejections
                  if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
                  {
                      var transactionType = TransactionType.RejectionMemo3;
                      if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                      {
                          transactionType = TransactionType.SamplingFormXF;
                      }
                      bool rmInTimeLimt = ValidatePaxStageThreeRmForTimeLimit(transactionType,
                                                      invoice.SettlementMethodId,
                                                      rejectionMemoRecord,
                                                      invoice,
                                                      fileName: fileName,
                                                      fileSubmissionDate: fileSubmissionDate,
                                                      exceptionDetailsList: exceptionDetailsList);
                      if (!rmInTimeLimt) isValid = false;
                  }
                }
              }
            }
            else
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number", rejectionMemoRecord.FimBMCMNumber ?? string.Empty, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourFimNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
          if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
          {
            if (rejectionMemoRecord.FimBMCMNumber != null && !string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber))
            {
              if (rejectionMemoRecord.RejectionStage == 1)
              {

                // Get rejection invoice with RM and RM Coupon for Rejection Stage 2 and 3.
                yourInvoice = GetInvoiceWithCMCoupons(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, billingMemberId, invoice.BillingMemberId, (int)BillingCode.NonSampling, null);


                var yourCreditMemoRecordList = yourInvoice.CreditMemoRecord.Where(creditMemo => creditMemo.CreditMemoNumber.Trim().ToUpper() == rejectionMemoRecord.FimBMCMNumber.ToUpper()).ToList();
                if (yourCreditMemoRecordList.Count() != 1)
                {
                  Logger.InfoFormat("Your Credit Memo: {0} is not found", rejectionMemoRecord.FimBMCMNumber);
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "CM Number", rejectionMemoRecord.FimBMCMNumber, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourCreditMemoNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                else
                {
                  if (yourCreditMemoRecordList.FirstOrDefault() != null)
                  {
                    yourCreditMemoRecord = yourCreditMemoRecordList.FirstOrDefault();
                    Logger.InfoFormat("Your Credit Memo: {0} is found", rejectionMemoRecord.FimBMCMNumber);
                    IsFimBmCmRmFound = true;
                      //CMP#459 : Validate Amount
                      //All amounts of the rejected CM from the rejected invoice should match with the RM level amounts of the rejecting RM
                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                prevInvExRate,
                                                                currInvExRate,
                                                              rejectionMemoRecord,
                                                              exceptionDetailsList,
                                                              invoice,
                                                              yourInvoice,
                                                              null,
                                                              null,
                                                              null,
                                                              fileName,
                                                              fileSubmissionDate);
                  }
                }
              }
              else if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
              {
                var yourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber!=null && rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber).ToList();

                if (yourRejectionMemoRecordList.Count() > 1)
                {
                  Logger.InfoFormat("There are multiple record found for rejection memo - " + rejectionMemoRecord.YourRejectionNumber);
                }

                if (yourRejectionMemoRecordList.Count() == 0)
                {
                  Logger.InfoFormat("Your Rejection Memo: {0} is not found", rejectionMemoRecord.YourRejectionNumber);
                  var isYourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).ToList();

                  if (isYourRejectionMemoRecordList.Count() == 0)
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Your Rejection Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.
                                                                                        YourRejectionNumber), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.AuditTrailFailForYourRejectionMemoNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                  else
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Fim BM CM Number",
                                                                                    Convert.ToString(
                                                                                      rejectionMemoRecord.
                                                                                        YourRejectionNumber), invoice,
                                                                                    fileName, errorLevel,
                                                                                    ErrorCodes.AuditTrailFailForCmNumber,
                                                                                    ErrorStatus.C, rejectionMemoRecord,
                                                                                    islinkingError: true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                }
                // Validate Rejection Stage.
                else if (yourRejectionMemoRecordList.FirstOrDefault() != null && yourRejectionMemoRecordList.FirstOrDefault().RejectionStage != (rejectionMemoRecord.RejectionStage - 1))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Rejection Stage", Convert.ToString(rejectionMemoRecord.RejectionStage), invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceRejectionStage, ErrorStatus.X, rejectionMemoRecord, islinkingError: true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }

                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                {
                  yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                  Logger.InfoFormat("Your Rejection Memo: {0} is found", rejectionMemoRecord.YourRejectionNumber);
                  IsFimBmCmRmFound = true;
                  //CMP#459 : Validate Amount RM
                  //All amounts of the rejected Stage 1 RM from the rejected invoice should match with the RM level amounts of the rejecting RM
                  var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                  var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                  isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                              prevInvExRate,
                                                              currInvExRate,
                                                              rejectionMemoRecord,
                                                              exceptionDetailsList,
                                                              invoice,
                                                              yourInvoice,
                                                              null,
                                                              null,
                                                              null,
                                                              fileName,
                                                              fileSubmissionDate);
                  //CMP#641: Time Limit Validation on Third Stage PAX Rejections
                  if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
                  {
                      var transactionType = TransactionType.RejectionMemo3;
                      if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                      {
                          transactionType = TransactionType.SamplingFormXF;
                      }
                      bool rmInTimeLimt = ValidatePaxStageThreeRmForTimeLimit(transactionType,
                                                      invoice.SettlementMethodId,
                                                      rejectionMemoRecord,
                                                      invoice,
                                                      fileName: fileName,
                                                      fileSubmissionDate: fileSubmissionDate,
                                                      exceptionDetailsList: exceptionDetailsList);
                      if (!rmInTimeLimt) isValid = false;
                  }
                }
              }
            }
            else
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "FIM Number", rejectionMemoRecord.FimBMCMNumber ?? string.Empty, invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourFimNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }

          //Validate rejection memo record
            if ((rejectionMemoRecord.FIMBMCMIndicatorId == 0 || rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None) && (((rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo || rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree) &&
             invoice.BillingCode == (int)BillingCode.NonSampling) || invoice.BillingCode == (int)BillingCode.SamplingFormXF))
          {
            var yourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).ToList();

            if (yourRejectionMemoRecordList.Count() > 1)
            {
              Logger.InfoFormat("There are multiple record found for rejection memo - " + rejectionMemoRecord.YourRejectionNumber);
            }

            if (yourRejectionMemoRecordList.Count() == 0)
            {
              Logger.InfoFormat("Your Rejection Memo: {0} is not found", rejectionMemoRecord.YourRejectionNumber);
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Your Rejection Number", Convert.ToString(rejectionMemoRecord.YourRejectionNumber), invoice, fileName, errorLevel, ErrorCodes.AuditTrailFailForYourRejectionMemoNumber, ErrorStatus.C, rejectionMemoRecord, islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            // Validate Rejection Stage.
            else if (yourRejectionMemoRecordList.FirstOrDefault() != null && yourRejectionMemoRecordList.FirstOrDefault().RejectionStage != (rejectionMemoRecord.RejectionStage - 1))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Rejection Stage", Convert.ToString(rejectionMemoRecord.RejectionStage), invoice, fileName, errorLevel, ErrorCodes.InvalidYourInvoiceRejectionStage, ErrorStatus.X, rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
            {
              yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
              Logger.InfoFormat("Your Rejection Memo: {0} is found", rejectionMemoRecord.YourRejectionNumber);
              IsFimBmCmRmFound = true;
              //CMP#459 : Validate Amount RM
              //All amounts of the rejected Stage 1 RM from the rejected invoice should match with the RM level amounts of the rejecting RM
              var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
              var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
              isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                          prevInvExRate,
                                                          currInvExRate,
                                                              rejectionMemoRecord,
                                                              exceptionDetailsList,
                                                              invoice,
                                                              yourInvoice,
                                                              null,
                                                              null,
                                                              null,
                                                              fileName,
                                                              fileSubmissionDate);
              //CMP#641: Time Limit Validation on Third Stage PAX Rejections
              if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
              {
                  var transactionType = TransactionType.RejectionMemo3;
                  if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                  {
                      transactionType = TransactionType.SamplingFormXF;
                  }
                  bool rmInTimeLimt = ValidatePaxStageThreeRmForTimeLimit(transactionType,
                                                      invoice.SettlementMethodId,
                                                      rejectionMemoRecord,
                                                      invoice,
                                                      fileName: fileName,
                                                      fileSubmissionDate: fileSubmissionDate,
                                                      exceptionDetailsList: exceptionDetailsList);
                  if (!rmInTimeLimt) isValid = false;
              }
            }
          }
          if ((rejectionMemoRecord.FIMBMCMIndicatorId == 0 || rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None) && (rejectionMemoRecord.RejectionStage == 1))
          {
            IsFimBmCmRmFound = true;
          }
        }
      }
      //Validate RM Coupon Breakdown Record
      RMCoupon previousBreakdownRecord = null;
      bool isCouponLink = true;
      ExchangeRate prevInvExchangeRate = null;
      if (yourInvoice != null)
      {
        var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
        prevInvExchangeRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
      }
      foreach (var rejectionMemoCouponBreakdownRecord in rejectionMemoRecord.CouponBreakdownRecord)
      {
          //Validate serial number
          ValidateRMCouponSerialNumber(rejectionMemoRecord, ref previousBreakdownRecord, rejectionMemoCouponBreakdownRecord, exceptionDetailsList, invoice, fileName, fileSubmissionDate);
          //Validate rejection memo coupon breakdown record
          isValid = ValidateParsedRMCouponBreakdownRecord(rejectionMemoCouponBreakdownRecord,
                                                          exceptionDetailsList,
                                                          rejectionMemoRecord,
                                                          yourRejectionMemoRecord,
                                                          invoice,
                                                          yourInvoice,
                                                          fileName,
                                                          issuingAirline,
                                                          fileSubmissionDate,
                                                          exchangeRate,
                                                          couponMaxAcceptableAmount,
                                                          transType,
                                                          transTypeMigration,
                                                          yourBillingMemoRecord,
                                                          yourCreditMemoRecord,
                                                          outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                          prevInvExchangeRate,
                                                          currInvExRate);
          if (!isValid) isCouponLink = isValid;
          //validation for check digit-
          if ((rejectionMemoCouponBreakdownRecord.CheckDigit < 0) || (rejectionMemoCouponBreakdownRecord.CheckDigit > 6 && rejectionMemoCouponBreakdownRecord.CheckDigit != 9))
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "CheckDigit",
                                                                              Convert.ToString(rejectionMemoCouponBreakdownRecord.CheckDigit),
                                                                              invoice,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                              ErrorCodes.InvalidCheckDigit,
                                                                              ErrorStatus.C,
                                                                              rejectionMemoRecord,
                                                                              false,
                                                                              string.Format("{0}-{1}-{2}", rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }
      }
      
      Logger.InfoFormat("Coupon Breakdowns found : {0} ", isCouponLink);
      //if your invoice and your Fim Rm BM Cm and their coupons are matched then linking is successful
      if (IsFimBmCmRmFound && isCouponLink)
      {
        rejectionMemoRecord.IsLinkingSuccessful = true;
      }

      //Rejection memo total level validations
      ValidateParsedRejectionMemoTotals(rejectionMemoRecord, exceptionDetailsList, invoice, fileName, invoice.BillingCode, fileSubmissionDate);

      if (!ReferenceManager.IsValidSourceCode(invoice, rejectionMemoRecord.SourceCodeId, (int)transType))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Source Code",
                                                                        Convert.ToString(rejectionMemoRecord.SourceCodeId),
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.InvalidSourceCode,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate ReasonCode 
      if (!ReferenceManager.IsValidReasonCode(invoice, rejectionMemoRecord.ReasonCode, (int)transType))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Reason Code",
                                                                        Convert.ToString(rejectionMemoRecord.ReasonCode),
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.InvalidReasonCode,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Net Reject Amount)
      if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalNetRejectAmount), transType, invoice.ListingCurrencyId, invoice, exchangeRate, validateMaxAmount: false, iMinAcceptableAmount: null, iMaxAcceptableAmount: null, rejectionReasonCode: rejectionMemoRecord.ReasonCode, applicableMinimumField: ApplicableMinimumField.TotalNetRejectAmount, isRejectionMemo : true))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate, "Net Reject Amount",

                                                                        Convert.ToString(rejectionMemoRecord.TotalNetRejectAmount),
                                                                        invoice, fileName, errorLevel,


                                                                        ErrorCodes.NetRejectAmountIsNotInAllowedRange,
                                                                        ErrorStatus.X, rejectionMemoRecord);

        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Gross Amount Difference
      if (Convert.ToDouble(rejectionMemoRecord.TotalGrossDifference) != 0 && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalGrossDifference), transType, invoice.ListingCurrencyId, invoice, exchangeRate, validateMaxAmount: false, iMinAcceptableAmount: null, rejectionReasonCode: rejectionMemoRecord.ReasonCode, applicableMinimumField: ApplicableMinimumField.TotalGrossDifference, isRejectionMemo : true))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate, "Gross Amount Difference",
                                                                        Convert.ToString(rejectionMemoRecord.TotalGrossDifference),
                                                                        invoice, fileName, errorLevel,
                                                                        ErrorCodes.GrossDifferenceAmountIsNotInAllowedRange,
                                                                        ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Tax Amount Difference
      if (Convert.ToDouble(rejectionMemoRecord.TotalTaxAmountDifference) != 0 && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoRecord.TotalTaxAmountDifference), transType, invoice.ListingCurrencyId, invoice, exchangeRate, validateMaxAmount: false, iMinAcceptableAmount: null, rejectionReasonCode: rejectionMemoRecord.ReasonCode, applicableMinimumField: ApplicableMinimumField.TotalTaxDifference, isRejectionMemo : true))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate, "Tax Amount Difference",
                                                                        Convert.ToString(rejectionMemoRecord.TotalTaxAmountDifference),
                                                                        invoice, fileName, errorLevel,
                                                                        ErrorCodes.TaxDifferenceAmountIsNotInAllowedRange,
                                                                        ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // FimBMCMIndicator, FimBMCMNumber and FimCouponNumber check
      #region Check for FimBMCMIndicator, FimBMCMNumber and FimCouponNumber.
      // Check done for sampling rejections
      if ((invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF) && !string.IsNullOrWhiteSpace(rejectionMemoRecord.FimBMCMNumber))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "FIMOrBMOrCM Number",
                                                                        Convert.ToString(rejectionMemoRecord.FimBMCMNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.FimNumberShouldBeBlankForSampling,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }// End if

      if ((invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF) && rejectionMemoRecord.FimCouponNumber != null && rejectionMemoRecord.FimCouponNumber != 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "FIM Coupon Number",
                                                                        Convert.ToString(rejectionMemoRecord.FimCouponNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.FimCouponNumberShouldBeBlankForSampling,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }// End if


         
      // Checks done for Source Code = "44" or "45" or "46"
      if (rejectionMemoRecord.SourceCodeId == 44 || rejectionMemoRecord.SourceCodeId == 45 || rejectionMemoRecord.SourceCodeId == 46)
      {
        // FimBMCMIndicator Check. 
        if (rejectionMemoRecord.FIMBMCMIndicator != FIMBMCMIndicator.FIMNumber)
        {
          //TODO: Create Non Correctable Validation error and add it to validation error list.
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "FIMBMCMIndicator",
                                                                        string.Empty,
                                                                        invoice,
                                                                        fileName,
                                                                        errorLevel,
                                                                        ErrorCodes.InvalidFimBmCmIndicatorforFim,
                                                                        ErrorStatus.X,
                                                                        rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }// End if

        //SCP56973  - Error message received when creating Rejection Memo - to get rejection memo record for given FIM/BM/CM no and FIM Coupon number.
        if (yourInvoice != null && yourInvoice.RejectionMemoRecord != null && (rejectionMemoRecord.SourceCodeId == 45 || rejectionMemoRecord.SourceCodeId == 46))
        {
            yourRejectionMemoRecord = yourInvoice.RejectionMemoRecord.Where(rejectionMemo => rejectionMemo.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
        }// End if

        //FIM Number Should be populated if Source Code = "44" or "45" or "46" 
        long rmFimBmCmNumber = 0;
        if ((rejectionMemoRecord.SourceCodeId == 44 || (rejectionMemoRecord.SourceCodeId != 44 && (yourInvoice == null || (yourInvoice != null && yourRejectionMemoRecord == null)) && ignoreValidationInMigrationPeriod)) && rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber && (string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) || (Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber) && Int64.TryParse(rejectionMemoRecord.FimBMCMNumber, out rmFimBmCmNumber) && rmFimBmCmNumber == 0)))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "FIM Number",
                                                                          Convert.ToString(rejectionMemoRecord.FimBMCMNumber),
                                                                          invoice,
                                                                          fileName,
                                                                          errorLevel,
                                                                          ErrorCodes.MandatoryFimNumberAndFimCouponNumber,
                                                                        ErrorStatus.X,
                                                                          rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }// End  if
        else if (rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber && !string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) && !Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                        "FIM Number",
                                                                        Convert.ToString(rejectionMemoRecord.FimBMCMNumber),
                                                                          invoice,
                                                                          fileName,
                                                                          errorLevel,
                                                                        ErrorCodes.InvalidFimNumber,
                                                                          ErrorStatus.X,
                                                                          rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }// End else if



















        //FimCouponNumber Check. 
        if ((rejectionMemoRecord.SourceCodeId == 44 || (rejectionMemoRecord.SourceCodeId != 44 && (yourInvoice == null || (yourInvoice != null && yourRejectionMemoRecord == null)) && ignoreValidationInMigrationPeriod)) && rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber && (rejectionMemoRecord.FimCouponNumber == null || rejectionMemoRecord.FimCouponNumber == 0))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "FIM Coupon Number",
                                                                          Convert.ToString(rejectionMemoRecord.FimCouponNumber),
                                                                          invoice,
                                                                          fileName,
                                                                          errorLevel,
                                                                          ErrorCodes.MandatoryFimNumberAndFimCouponNumber,
                                                                          ErrorStatus.X,
                                                                          rejectionMemoRecord);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }// End if
        }
      // Checks done for Source Code != "44" or "45" or "46"
      else
      {
        long rmFimBmCmNumber = 0;
        if (rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.FIMNumber)
        {
          if (rejectionMemoRecord.RejectionStage == 1)
          {
            // FimBMCMNumber Check. Return a Non-correctable if value is null or is not a valid number or is 0.
            if ((string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) || !Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber) || (Int64.TryParse(rejectionMemoRecord.FimBMCMNumber, out rmFimBmCmNumber) && rmFimBmCmNumber == 0)))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "FIM Number",
                                                                              Convert.ToString(
                                                                                rejectionMemoRecord.FimBMCMNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.
                                                                                InvalidFimNumber,
                                                                              ErrorStatus.X,
                                                                              rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }// End if FimBMCMNumber Check

            //FimCouponNumber Check. Return Non-correctable error if FimCouponNumber is null or 0.
            if (rejectionMemoRecord.FimCouponNumber == null || rejectionMemoRecord.FimCouponNumber == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "FIM Coupon Number",
                                                                              Convert.ToString(rejectionMemoRecord.FimCouponNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.InvalidFimCouponNumber,
                                                                              ErrorStatus.X,
                                                                              rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }// End if FimCouponNumber Check

          }// End if Rejection Stage 1

          if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
          {
            // FimBMCMNumber Check.
            // Return Non-correctable error if FimBMCMNumber is not null and is not a whole number.
            if (!string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) && !Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "FIM Number",
                                                                              Convert.ToString(
                                                                                rejectionMemoRecord.FimBMCMNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.
                                                                                InvalidFimNumber,
                                                                              ErrorStatus.X,
                                                                              rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }// End if

            // Return correctable error if FimBMCMNumber is null or 0.
            if ((string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) || (Int64.TryParse(rejectionMemoRecord.FimBMCMNumber, out rmFimBmCmNumber) && rmFimBmCmNumber == 0)))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "FIM Number",
                                                                              Convert.ToString(
                                                                                rejectionMemoRecord.FimBMCMNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.
                                                                                InvalidFimNumber,
                                                                              ErrorStatus.X,
                                                                              rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }// End if

            //FimCouponNumber Check. 
            if (rejectionMemoRecord.FimCouponNumber == null || rejectionMemoRecord.FimCouponNumber == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "FIM Coupon Number",
                                                                              Convert.ToString(rejectionMemoRecord.FimCouponNumber),
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.InvalidFimCouponNumber,
                                                                              ErrorStatus.X,
                                                                              rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }// End if FimCouponNumber Check
          }// End if Rejection Stage 2 or 3
        }// End if FimBMCMIndicator = "F"

        // FimBmCmNumber and FimCouponNumber Check for BM and CM
        if ((rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.BMNumber || rejectionMemoRecord.FIMBMCMIndicator == FIMBMCMIndicator.CMNumber)) 
        {
          // FimBmCmNumber Check. Should be provided in case of BM and CM Rejection.
          if(string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Fim BM CM Number",
                                                                            rejectionMemoRecord.FimBMCMNumber,
                                                                            invoice, fileName, errorLevel, ErrorCodes.InvalidFimBmCmNumberForValidFimbmcmIndicator,
                                                                            ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }// End if FimBmCmNumber Check

          if (!string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber) && !Validators.IsAlphaNumeric(rejectionMemoRecord.FimBMCMNumber))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                          "FIM BM CM Number",
                                                                          Convert.ToString(rejectionMemoRecord.FimBMCMNumber),
                                                                            invoice,
                                                                            fileName,
                                                                            errorLevel,
                                                                          ErrorCodes.InvalidFimNumber,
                                                                            ErrorStatus.X,
                                                                            rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          // FimCouponNumber Check. Should not be provided in case of BM and CM Rejection
          if (rejectionMemoRecord.FimCouponNumber!= null && rejectionMemoRecord.FimCouponNumber != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "FIM Coupon Number",
                                                                            Convert.ToString(rejectionMemoRecord.FimCouponNumber),
                                                                            invoice,
                                                                            fileName,
                                                                            errorLevel,
                                                                            ErrorCodes.MandatoryFimNumberAndFimCouponNumber,
                                                                            ErrorStatus.X, rejectionMemoRecord);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }// End  if FimCouponNumber Check
        }// End if FimBmCmNumber and FimCouponNumber Check for BM and CM.
      }// End else
      #endregion





      //If a Rejection Memo is raised with Reason code 1G, then the “Total Tax Difference” field should have a non zero value.

      //if (rejectionMemoRecord.ReasonCode != null && rejectionMemoRecord.ReasonCode.ToUpper() == "1G" && rejectionMemoRecord.TotalTaxAmountDifference == 0)
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1,
      //                                                                  fileSubmissionDate,
      //                                                                  "Total Tax Amount Difference",
      //                                                                  Convert.ToString(rejectionMemoRecord.TotalTaxAmountDifference),
      //                                                                  invoice, fileName, errorLevel, ErrorCodes.InvalidTotalTaxDifferenceForReasonCode1G,
      //                                                                  ErrorStatus.X, rejectionMemoRecord);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}


      // Memo level VAT breakdown should not be provided when RM/BM/CM has coupon breakdown information.
      if (rejectionMemoRecord.RejectionMemoVat.Count > 0 && rejectionMemoRecord.CouponBreakdownRecord.Count > 0 )
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "RM Vat Details",
                                                                          string.Empty,
                                                                          invoice,
                                                                          fileName,
                                                                          errorLevel,
                                                                          ErrorCodes.VatPresentWhenCouponBreakdownExists,
                                                                          ErrorStatus.X, rejectionMemoRecord);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate Vat Breakdowns 
      foreach (var rejectionMemoVat in rejectionMemoRecord.RejectionMemoVat)
      {
        isValid = ValidateParsedVat(rejectionMemoVat,
                                    exceptionDetailsList,
                                    invoice,
                                    fileName,
                                    errorLevelVat,
                                    fileSubmissionDate,
                                    rejectionMemoRecord.BatchSequenceNumber,
                                    rejectionMemoRecord.RecordSequenceWithinBatch,
                                    rejectionMemoNumber,
                                    rejectionMemoRecord.SourceCodeId, false, true);
      }

      //Set IsRejection flag depends on amount in difference
      SetIsRejectionFlag(rejectionMemoRecord);


      // SCP ID : 72923 - BGEN_00007 - TG PAX file PIDECF-2172013010320130125200007.dat
      // Reason Remark Field should be Max 4000 Char

      if (rejectionMemoRecord.ReasonRemarks != null)
      {
          if (rejectionMemoRecord.ReasonRemarks.Length > MaxReasonRemarkCharLength)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              "Reason Remarks",
                                                                              string.Empty,
                                                                              invoice,
                                                                              fileName,
                                                                              errorLevel,
                                                                              ErrorCodes.MaxReasonRemarkCharLength,
                                                                              ErrorStatus.X, rejectionMemoRecord);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }
      }

      /* CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text */
      /* Check if validation is applicable or not */
      if (rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo || rejectionMemoRecord.RejectionStage == (int)RejectionStage.StageThree)
      {
          /* CMP#671 - Validation Applicable */
          if (!ValidateReasonTextMinLength(paxRejectionMemoRecord: rejectionMemoRecord, fileName: fileName,
                                                invoice: invoice, exceptionDetailsList: exceptionDetailsList,
                                                fileSubmissionDate: fileSubmissionDate))
          {
              /* Validation has failed - Error is already populated in Exception Details List */
              isValid = false;
          }
      }
          //else
          //{
          //    For Logical Completion - CMP#671 - Validtion is not Applicable
          //}

      //CMP614: Source Code Validation for PAX RMs.
      //Desc: Validate source code for PAX rejection memo. 

      ValidateParsedSourceCodeOfRM(invoice, rejectionMemoRecord, yourInvoice, fileName, exceptionDetailsList, fileSubmissionDate, ref isValid);
         
      // Update expiry date for purging.
      //rejectionMemoRecord.ExpiryDatePeriod = GetRejectionMemoExpiryDate(rejectionMemoRecord.RejectionStage, invoice, billingPeriod);
      return isValid;
    }

      /// <summary>
    /// This function is used to validate source code of rejection memo based on below table.
    /// </summary>
    /// <param name="rejectionMemoRecord"></param>
    /// <param name="yourInvoice"></param>
    /// <param name="isValid"></param>
    //CMP #614: Source Code Validation for PAX RMs.
    /*Rule #	Stage of Non-Sampling RM Being    Type of Rejected          Source Code of Rejected   Expected Source Code of RM Being 
     *       Validated / Captured                transaction               transaction              Validated/ Captured
    1	              2                         	Non-Sampling Stage 1 RM         	4                            	5
    2         	    3                         	Non-Sampling Stage 2 RM	          5	                            6
    3	              1	                          Non-Sampling Prime Coupon	        14	                          44
    4	              2	                          Non-Sampling Stage 1 RM	          44	                          45
    5	              3	                          Non-Sampling Stage 2 RM         	45                          	46
    6	              2                         	Non-Sampling Stage 1 RM	          91                          	92
    7	              3                         	Non-Sampling Stage 2 RM	          92	                          93
    */
    private void ValidateParsedSourceCodeOfRM(PaxInvoice invoice, RejectionMemo rejectionMemoRecord, PaxInvoice yourInvoice, string fileName,
                                             IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime fileSubmissionDate, ref bool isValid)
    {
      //Source code validation only for non sampling invoices.
      if (invoice.BillingCode == (int)BillingCode.NonSampling && yourInvoice != null)
      {
        switch (rejectionMemoRecord.RejectionStage)
        {
            case 1:
                {
                    /* 313204 - question for validation report NH/205
                    * Desc: In case of RM with source code 44, linked prime coupon must have source code 14. 
                     * Because of entity framework limitation, your invoice will have all the coupons referred by the RMs in current file. 
                     * Hence below code is written to filter other coupons and consider linked coupon to current RM (in focus for which 
                     * source code validation is in progress). */

                    var primeCoupons = new List<PrimeCoupon>();
                      //SCP343865 - SRM: SIS: Admin AlertParsing and Validation failure notification - SIS
                     // rejectionMemoRecord.FIMBMCMIndicatorId == 2 (FIMNumber)
                    if (rejectionMemoRecord.FIMBMCMIndicatorId == 2)
                    {
                        
                        if (!String.IsNullOrWhiteSpace(rejectionMemoRecord.FimBMCMNumber))
                        {
                            /* In cases when FIM number is given in RM (it is RM with source code 44), linking is done at coupon level
                             * so use such criteria to get linked coupon. */
                            var linkedCouponFim = yourInvoice.CouponDataRecord.Where(pc => pc.TicketDocOrFimNumber ==
                                                                                           long.Parse(
                                                                                               rejectionMemoRecord.
                                                                                                   FimBMCMNumber)).
                                FirstOrDefault();
                            if (linkedCouponFim != null)
                            {
                                primeCoupons.Add(linkedCouponFim);
                            }
                        }
                    }
                    /* In case of RM with source code other than 44 - linking is at breakdown level and so use
                     criteria at breakdown level to get the actual linked coupon. */
                    foreach (var rmCoupon in rejectionMemoRecord.CouponBreakdownRecord)
                    {
                        var linkedCoupon = yourInvoice.CouponDataRecord.Where(pc => pc.TicketDocOrFimNumber ==
                                                                                              rmCoupon.TicketDocOrFimNumber &&
                                                                                              pc.TicketOrFimCouponNumber ==
                                                                                              rmCoupon.TicketOrFimCouponNumber &&
                                                                                              pc.TicketOrFimIssuingAirline ==
                                                                                              rmCoupon.TicketOrFimIssuingAirline).
                            FirstOrDefault();
                        if (linkedCoupon != null)
                        {
                            primeCoupons.Add(linkedCoupon);
                        }
                    }

                    /* Actual source code validation check. */
                    PrimeCoupon couponDataWithSourceCode44 = null;
                    if (primeCoupons != null && primeCoupons.Count > 0)
                    {
                        couponDataWithSourceCode44 = primeCoupons.Where(pc => pc.SourceCodeId == 14).FirstOrDefault();
                    }

                    if (couponDataWithSourceCode44 != null)
                    {
                        if (rejectionMemoRecord.SourceCodeId != 44)
                        {
                            //Add exception detail in Csv file regarding source code validation for Pax RM.
                            isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord,
                                                                                  fileName,
                                                                                  exceptionDetailsList,
                                                                                  fileSubmissionDate,
                                                                                  44, 14);
                        }
                    }
                    break;
                }
            case 2:
            {
              var yourRejectionMemoRecord = yourInvoice.RejectionMemoRecord.Where(rm=>rm.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRejectionMemoRecord != null)
              {
                switch (yourRejectionMemoRecord.SourceCodeId)
                {
                  case 4:
                    if (rejectionMemoRecord.SourceCodeId != 5)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            5, 4);
                    }
                    break;
                  case 44:
                    if (rejectionMemoRecord.SourceCodeId != 45)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            45, 44);
                    }
                    break;
                  case 91:
                    if (rejectionMemoRecord.SourceCodeId != 92)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            92, 91);
                    }
                    break;
                }
              }
            }
            break;
          case 3:
            {
              var yourRejectionMemoRecord = yourInvoice.RejectionMemoRecord.Where(rm => rm.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRejectionMemoRecord != null)
              {
                switch (yourRejectionMemoRecord.SourceCodeId)
                {
                  case 5:
                    if (rejectionMemoRecord.SourceCodeId != 6)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            6, 5);
                    }
                    break;
                  case 45:
                    if (rejectionMemoRecord.SourceCodeId != 46)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            46, 45);
                    }
                    break;
                  case 92:
                    if (rejectionMemoRecord.SourceCodeId != 93)
                    {
                      //Add exception detail in Csv file regarding source code validation for Pax RM
                      isValid = CreateAndAddExceptionDetailForRMSourceCodes(invoice, rejectionMemoRecord, fileName, exceptionDetailsList, fileSubmissionDate,
                                                                            93, 92);
                    }
                    break;
                }
              }
            }
            break;
        }
      }
    }

    /// <summary>
    /// This function is used to create and add exception detail for RM source code.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="rejectionMemoRecord"></param>
    /// <param name="fileName"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="paxRMSourceCodesFlag"></param>
    /// <returns></returns>
    //CMP614: Source Code Validation for PAX RMs.
    private static bool CreateAndAddExceptionDetailForRMSourceCodes(PaxInvoice invoice, RejectionMemo rejectionMemoRecord, string fileName,
                             IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime fileSubmissionDate,
                             int expectedSourceCode, int fileSourceCode)
    {
      //Get exception description based on error code. 
      String exceptionDesc = String.Format(Messages.ResourceManager.GetString(ErrorCodes.PaxSourceCodes), expectedSourceCode, fileSourceCode);
      var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoRecord.Id.Value(), exceptionDetailsList.Count() + 1,
                                                            fileSubmissionDate,
                                                            "Source Code",
                                                            rejectionMemoRecord.SourceCodeId.ToString(),
                                                            invoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                            ErrorCodes.PaxSourceCodes,
                                                            SystemParameters.Instance.ValidationParams.PaxRMSourceCodes == 1 ? ErrorStatus.X : ErrorStatus.W,
                                                            rejectionMemoRecord, exceptionDesc: exceptionDesc);
      exceptionDetailsList.Add(validationExceptionDetail);
      return false;
    }

    
    /// <summary>
    /// This method performs RM duplicate validation on all the RMs in input invoice.
    /// </summary>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="rejectionMemoNumbers"></param>
    private void IsDuplicateRMs(IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName, DateTime fileSubmissionDate, string rejectionMemoNumbers)
    {
      var errorLevel = string.Empty;
      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        errorLevel = ErrorLevels.ErrorLevelRejectionMemo;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormF;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormXF;
      }

      // Perform duplicate check on all the RM records in Invoice.
      // Should be a unique number within each Billed Airline in the Billing period.
      var duplicateRMNumbers = RejectionMemoRepository.GetDuplicateRejectionMemoNumbers(invoice.BilledMemberId,
                                                                invoice.BillingMemberId,
                                                                rejectionMemoNumbers,
                                                                invoice.BillingYear,
                                                                invoice.BillingMonth,
                                                                invoice.BillingPeriod);
      var duplicateRMNumberArray = duplicateRMNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      // Add validation exception details for each duplicate RM number returned from SP.
      foreach (var rmNumber in duplicateRMNumberArray)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(invoice.RejectionMemoRecord.Find(i => i.RejectionMemoNumber == rmNumber).Id.Value(), exceptionDetailsList.Count() + 1,
                                                                   fileSubmissionDate,
                                                                   "Rejection Memo Number",
                                                                   rmNumber,
                                                                   invoice,
                                                                   fileName,
                                                                   errorLevel,
                                                                   ErrorCodes.DuplicateRejectionMemoNumber,
                                                                   ErrorStatus.X,
                                                                   invoice.RejectionMemoRecord.Find(i => i.RejectionMemoNumber == rmNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
      }
    }

    private string ValidateRmCouponDifferenceAmounts(RejectionMemo rejectionMemoRecord, RMCoupon rejectionMemoCouponBreakdownRecord, TransactionType transactionType)
    {
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
      return acceptableDifference;
    }

    private string ValidateRMDifferenceAmounts(RejectionMemo rejectionMemoRecord, TransactionType transactionType)
    {
      var isCouponBreakdownMandatory = ReasonCodeRepository.GetCount(reasonCode => reasonCode.Code.ToUpper() == rejectionMemoRecord.ReasonCode.ToUpper() && reasonCode.TransactionTypeId == (int)transactionType && reasonCode.CouponAwbBreakdownMandatory) > 0;

      // If linking is successful and coupon breakdown exist then validation for acceptable amount difference and Net Amount limit validation is not required.
      // as we are inheriting the RM coupons amount details from the rejected memo.
      if (IsAmountValidationRequired(rejectionMemoRecord, isCouponBreakdownMandatory))
      {
        // Validate acceptable amount difference with ReasonCode if Coupon Breakdown is not allowed while creating Or 
        // Validate it while updating if rejection memo coupon breakdown does not exists.
        if (RejectionMemoCouponBreakdownRepository.GetCount(coupon => coupon.RejectionMemoId == rejectionMemoRecord.Id) == 0)
        {
          var diffenceNotAcceptable = ValidateAcceptableDifferences(rejectionMemoRecord.ReasonCode, transactionType, rejectionMemoRecord.TotalGrossDifference, rejectionMemoRecord.TotalTaxAmountDifference, rejectionMemoRecord.TotalVatAmountDifference, rejectionMemoRecord.IscDifference, rejectionMemoRecord.UatpAmountDifference, rejectionMemoRecord.HandlingFeeAmountDifference, rejectionMemoRecord.OtherCommissionDifference);

          return diffenceNotAcceptable;
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rejectionMemoRecord"></param>
    /// <param name="previousBreakdownRecord"></param>
    /// <param name="rejectionMemoCouponBreakdownRecord"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    private void ValidateRMCouponSerialNumber(RejectionMemo rejectionMemoRecord, ref RMCoupon previousBreakdownRecord, RMCoupon rejectionMemoCouponBreakdownRecord, IList<IsValidationExceptionDetail> exceptionDetailsList,
                                                             PaxInvoice invoice, string fileName, DateTime fileSubmissionDate)
    {
      if (previousBreakdownRecord != null && rejectionMemoCouponBreakdownRecord.SerialNo != previousBreakdownRecord.SerialNo + 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(rejectionMemoCouponBreakdownRecord.SerialNo),
                                            invoice, fileName, ErrorLevels.ErrorLevelRejectionMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, rejectionMemoRecord, false, string.Format("{0}-{1}-{2}", rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
      }
      else if (previousBreakdownRecord == null && rejectionMemoCouponBreakdownRecord.SerialNo != 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Serial No", Convert.ToString(rejectionMemoCouponBreakdownRecord.SerialNo),
                                           invoice, fileName, ErrorLevels.ErrorLevelRejectionMemoCoupon, ErrorCodes.InvalidSerialNumberSequence, ErrorStatus.X, rejectionMemoRecord, false, string.Format("{0}-{1}-{2}", rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
      }
      previousBreakdownRecord = rejectionMemoCouponBreakdownRecord;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rejectionMemoCouponBreakdownRecord"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="parentMemo"></param>
    /// <param name="yourRejectionMemoRecord"></param>
    /// <param name="invoice"></param>
    /// <param name="yourInvoice"></param>
    /// <param name="fileName"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="exchangeRate"></param>
    /// <param name="couponMaxAcceptableAmount"></param>
    /// <returns></returns>
    private bool ValidateParsedRMCouponBreakdownRecord(RMCoupon rejectionMemoCouponBreakdownRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, RejectionMemo parentMemo, RejectionMemo yourRejectionMemoRecord, PaxInvoice invoice, PaxInvoice yourInvoice, string fileName, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate, ExchangeRate exchangeRate, MaxAcceptableAmount couponMaxAcceptableAmount, TransactionType transactionType, TransactionType transTypeMigration = 0, BillingMemo yourBillingMemo = null, CreditMemo yourCreditMemo = null, bool outcomeOfMismatchOnRmBilledOrAllowedAmounts = false, ExchangeRate prevInvExRate = null, ExchangeRate currInvExRate = null)
    {
      var isValid = true;

      var errorLevel = string.Empty;
      var errorLevelVat = string.Empty;
      var errorLevelTax = string.Empty;

      if (invoice.BillingCode == (int)BillingCode.NonSampling)
      {
        errorLevel = ErrorLevels.ErrorLevelRejectionMemoCoupon;
        errorLevelVat = ErrorLevels.ErrorLevelRejectionMemoCouponVat;
        errorLevelTax = ErrorLevels.ErrorLevelRejectionMemoCouponTax;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormFCoupon;
        errorLevelVat = ErrorLevels.ErrorLevelSamplingFormFCouponVat;
        errorLevelTax = ErrorLevels.ErrorLevelSamplingFormFCouponTax;
      }
      else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        errorLevel = ErrorLevels.ErrorLevelSamplingFormXFCoupon;
        errorLevelVat = ErrorLevels.ErrorLevelSamplingFormXFCouponVat;
        errorLevelTax = ErrorLevels.ErrorLevelSamplingFormXFCouponTax;
      }

      // Validate TicketDocOrFimNumber is greater than 0.
      if (rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber <= 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Documnet or FIM Number", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), invoice, fileName, errorLevel, ErrorCodes.InvalidTicketDocumnetOrFimNumber, ErrorStatus.X, parentMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured
      // Validate TicketDocOrFimNumber is less than or equal to 10 digits
      if (Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber).Length > 10)
      {
        var errorCodeFor = ErrorCodes.TicketFimDocumentNoGreaterThanTenNs;
        if (invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          errorCodeFor = SamplingErrorCodes.TicketFimDocumentNoGreaterThanTenS;
        }

        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        "Ticket Documnet or FIM Number",
                                                                        Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber),
                                                                        invoice,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                        errorCodeFor,
                                                                        ErrorStatus.X,
                                                                        parentMemo);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //// Validate ticket issuing airline.
      ValidateTicketIssuingAirline(rejectionMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, parentMemo.SourceCodeId, invoice, fileName, isValid, errorLevel, issuingAirline, fileSubmissionDate);

      var tktIsuingAirline = rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline ?? string.Empty;

      //for FIM coupon number 

      if (rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber <= 0 || rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber > 4)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Coupon Number", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber), invoice, fileName, errorLevel, ErrorCodes.InvalidCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Duplicate check will be done in stored procedure
      // Validate rejection memo coupon breakdown records.
      //if (parentMemo.CouponBreakdownRecord.Where(coupon => coupon.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber && coupon.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && coupon.TicketOrFimIssuingAirline == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline).Count() > 1)
      //{
      //  var iSValidationFlag = rejectionMemoCouponBreakdownRecord.ISValidationFlag ?? string.Empty;
      //  rejectionMemoCouponBreakdownRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, DuplicateValidationFlag);
      //}
      //else if (invoice.RejectionMemoRecord != null)
      //{
      //  var count = invoice.RejectionMemoRecord.SelectMany(memoRecord => memoRecord.CouponBreakdownRecord).Count(rmCoupon => rmCoupon.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber && rmCoupon.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && rmCoupon.TicketOrFimIssuingAirline == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);

      //  if (count > 1)
      //  {
      //    var iSValidationFlag = rejectionMemoCouponBreakdownRecord.ISValidationFlag ?? string.Empty;
      //    rejectionMemoCouponBreakdownRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, DuplicateValidationFlag);
      //  }
      //}
      //else
      //{
      //  var duplicateErrorMessage = GetDuplicateRMCouponCount(rejectionMemoCouponBreakdownRecord, false, invoice, parentMemo, string.Empty);
      //  if (!string.IsNullOrEmpty(duplicateErrorMessage))
      //  {
      //    var iSValidationFlag = rejectionMemoCouponBreakdownRecord.ISValidationFlag ?? string.Empty;
      //    rejectionMemoCouponBreakdownRecord.ISValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, DuplicateValidationFlag);
      //  }
      //}
      if (parentMemo.SourceCodeId != 91 && parentMemo.SourceCodeId != 92 && parentMemo.SourceCodeId != 93)
      {
        double expectedAmount = 0;
        //Allowed ISC amount does not match with Allowed ISC percentage * Gross Amount
        expectedAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);

        if (invoice.Tolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AllowedIscAmount, expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "ISC Amount Allowed", Convert.ToString(rejectionMemoCouponBreakdownRecord.AllowedIscAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidAllowedIscPercentage, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Accepted ISC amount does not match with Accepted ISC percentage * Gross Amount
        expectedAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedIscPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100, Constants.PaxDecimalPlaces);
        if (invoice.Tolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AcceptedIscAmount, expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "ISC Amount Accepted", Convert.ToString(rejectionMemoCouponBreakdownRecord.AcceptedIscAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidAcceptedIscPercentage, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //Allowed UATP amount does not match with Allowed UATP percentage * Gross Amount
        expectedAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AllowedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountBilled / 100, Constants.PaxDecimalPlaces);
        if (invoice.Tolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AllowedUatpAmount, expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Allowed UATP Amount", Convert.ToString(rejectionMemoCouponBreakdownRecord.AllowedUatpAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidAllowedUatpPercentage, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Accepted ISC amount does not match with Accepted ISC percentage * Gross Amount
        expectedAmount = ConvertUtil.Round(rejectionMemoCouponBreakdownRecord.AcceptedUatpPercentage * rejectionMemoCouponBreakdownRecord.GrossAmountAccepted / 100, Constants.PaxDecimalPlaces);
        if (invoice.Tolerance != null && !CompareUtil.Compare(rejectionMemoCouponBreakdownRecord.AcceptedUatpAmount, expectedAmount, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Accepted UATP Amount", Convert.ToString(rejectionMemoCouponBreakdownRecord.AcceptedUatpAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidAcceptedUatpPercentage, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //If FIM Number/Billing Memo Number field in the parent Rejection Memo Record is zero filled, the "Net Reject Amount" should not exceed $100,000
      if (parentMemo.FimBMCMNumber != null && String.IsNullOrEmpty(parentMemo.FimBMCMNumber.Trim()))
      {
        //Validate Net Reject Amount
        if (exchangeRate != null && couponMaxAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(rejectionMemoCouponBreakdownRecord.NetRejectAmount), TransactionType.Coupon, invoice.ListingCurrencyId, invoice, exchangeRate, iMaxAcceptableAmount: couponMaxAcceptableAmount))
        {
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Reject Amount", Convert.ToString(rejectionMemoCouponBreakdownRecord.NetRejectAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidRejectionMemoCouponNetRejectAmount, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      var isXmlFileType = (invoice.SubmissionMethod == SubmissionMethod.IsXml) ? true : false;

      var isIgnoreValidationInMigrationPeriod = false;
      switch (transTypeMigration)
      {
        case TransactionType.RejectionMemo1:
          isIgnoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodRm1;
          break;

        case TransactionType.Coupon:
          isIgnoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodCoupon;
          break;

        case TransactionType.SamplingFormD:
          isIgnoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodFormD;
          break;

        case TransactionType.SamplingFormF:
          isIgnoreValidationInMigrationPeriod = invoice.IgnoreValidationInMigrationPeriodFormF;
          break;

      }
      //Your invoice is returned by ValidateRejectionMemo method on the basis of yourInvoiceNumber, yourBillingDate
      //Validate linking of coupons with the database.
      if (yourInvoice != null)
      {
        if (parentMemo.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber)
        {
          if (yourBillingMemo != null)
          {
            var yourCouponRecords = yourBillingMemo.CouponBreakdownRecord.Where(couponRecord => couponRecord.TicketOrFimIssuingAirline.Trim() == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim() && couponRecord.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && couponRecord.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);

            if (yourCouponRecords.Count() == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Number", string.Format("{0}/{1}/{2}", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber)), invoice, fileName, errorLevel, SamplingErrorCodes.AuditTrailFailForCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber), islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
        }
        else if (parentMemo.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
        {
          if (yourCreditMemo != null)
          {
            var yourCouponRecords = yourCreditMemo.CouponBreakdownRecord.Where(couponRecord => couponRecord.TicketOrFimIssuingAirline.Trim() == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim() && couponRecord.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && couponRecord.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);

            if (yourCouponRecords.Count() == 0)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Number", string.Format("{0}/{1}/{2}", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber)), invoice, fileName, errorLevel, SamplingErrorCodes.AuditTrailFailForCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
        }

        if (((parentMemo.RejectionStage == 2 || parentMemo.RejectionStage == 3) && invoice.BillingCode == (int)BillingCode.NonSampling) || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          if (yourRejectionMemoRecord != null)
          {
            var yourCouponRecords = yourRejectionMemoRecord.CouponBreakdownRecord.Where(couponRecord => couponRecord.TicketOrFimIssuingAirline.Trim() == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim() && couponRecord.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && couponRecord.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);
              bool isAllowExtraCoupon = false;
              if (invoice.BillingCode == (int)BillingCode.NonSampling)
              {
                  if(yourRejectionMemoRecord.CouponBreakdownRecord.Count==0 || (parentMemo.SourceCodeId == 45 || parentMemo.SourceCodeId == 46))
                  {
                      isAllowExtraCoupon = true;
                  }
              }
              if (yourCouponRecords.Count() == 0 && !isAllowExtraCoupon)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Number", string.Format("{0}/{1}/{2}", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber)), invoice, fileName, errorLevel, SamplingErrorCodes.AuditTrailFailForCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber), islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
        }
        else if (((parentMemo.FIMBMCMIndicatorId == 0 || parentMemo.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None) && parentMemo.RejectionStage == 1 && invoice.BillingCode == (int)BillingCode.NonSampling))
        {
          var yourCouponRecords = yourInvoice.CouponDataRecord.Where(couponRecord => couponRecord.TicketOrFimIssuingAirline.Trim() == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim() && couponRecord.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && couponRecord.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);

          if (yourCouponRecords.Count() == 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Number", string.Format("{0}/{1}/{2}", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber)), invoice, fileName, errorLevel, SamplingErrorCodes.AuditTrailFailForCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber), islinkingError: true);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {
              //CMP#459 : Validate Amount
              //If duplicate rejected FIMs are found in the rejected invoice, all amounts of at least one FIM coupon from the rejected invoice should match the RM level amounts of the rejecting RM
              isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                          prevInvExRate,
                                                          currInvExRate,
                                                                parentMemo,
                                                                exceptionDetailsList,
                                                                invoice,
                                                                yourInvoice,
                                                                rejectionMemoCouponBreakdownRecord,
                                                                yourCouponRecords.ToList(),
                                                                null,
                                                                fileName,
                                                                fileSubmissionDate);
          }
        }
        else if (invoice.BillingCode == (int)BillingCode.SamplingFormF && yourInvoice.SamplingFormDRecord != null && yourInvoice.SamplingFormDRecord.Count > 0)
        {
          var yourCouponRecords = yourInvoice.SamplingFormDRecord.Where(formDRecord => formDRecord.TicketIssuingAirline == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline && formDRecord.TicketDocNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber && formDRecord.CouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);

          if (yourCouponRecords.Count() == 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
              "Ticket Number", string.Format("{0}/{1}/{2}", Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline),
              Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber), Convert.ToString(rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber)),
              invoice, fileName, ErrorLevels.ErrorLevelSamplingFormF, SamplingErrorCodes.AuditTrailFailForCouponNumber, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {
              //CMP#459 : Validate Amount 
              //If a rejected Form D coupon is found more than once in the rejected form D/E invoice, then all amounts of at least one form D coupon from the rejected invoice should match the coupon of the rejecting RM
              isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                          prevInvExRate,
                                                          currInvExRate,
                                                            parentMemo,
                                                            exceptionDetailsList,
                                                            invoice,
                                                            yourInvoice,
                                                            rejectionMemoCouponBreakdownRecord,
                                                            null,
                                                            yourCouponRecords.ToList(),
                                                            fileName,
                                                            fileSubmissionDate);
          }
        }
      }

      //From airport of coupon and to airport of coupon should not be same.
      if (rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon != null && rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon.Trim()) && !string.IsNullOrEmpty(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon.Trim()))
        {
          if (String.Equals(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon.ToUpper(), rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon.ToUpper()))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", Convert.ToString(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon), invoice, fileName, errorLevel, ErrorCodes.FromAirportOfCouponAndToAirportOfCouponShouldNotBeSame, ErrorStatus.X, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Validate FromAirportOfCoupon 
      if (!string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon) && !IsValidCityAirportCode(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "From Airport", Convert.ToString(rejectionMemoCouponBreakdownRecord.FromAirportOfCoupon), invoice, fileName, errorLevel, ErrorCodes.FromAirportOfCouponIsInvalid, ErrorStatus.C, parentMemo, true, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Validate ToAirportOfCoupon 
      if (!string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon) && !IsValidCityAirportCode(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(rejectionMemoCouponBreakdownRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "To Airport", Convert.ToString(rejectionMemoCouponBreakdownRecord.ToAirportOfCoupon), invoice, fileName, errorLevel, ErrorCodes.InvalidToAirportCode, ErrorStatus.C, parentMemo, false, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //Rejection memo coupon total validations
      ValidateParsedRMCouponBreakdownTotal(rejectionMemoCouponBreakdownRecord, exceptionDetailsList, parentMemo, invoice, fileName, fileSubmissionDate);

      //Validate Vat Breakdowns 
      foreach (var rejectionMemoCouponVatBreakdown in rejectionMemoCouponBreakdownRecord.VatBreakdown)
      {
        //SCP204465: SIS INCOMING - Issue with the Incoming file P3 October
        //Desc: FALSE value is send for parameter isIgnoreValidation. This ensures that - VAT Calculated Amount (1) is 
        //equal to VAT Base Amount 1 multiplied by VAT Percentage 1 /100 rounded to two decimal places".
        //Date: 06/02/2014
        isValid = ValidateParsedVat(rejectionMemoCouponVatBreakdown, exceptionDetailsList, invoice, fileName, errorLevelVat, fileSubmissionDate, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, parentMemo.RejectionMemoNumber, parentMemo.SourceCodeId, true, true);
      }

      //Validate Tax Breakdowns 
      foreach (var rejectionMemoCouponTaxBreakdown in rejectionMemoCouponBreakdownRecord.TaxBreakdown)
      {
        isValid = ValidateParsedTax(rejectionMemoCouponTaxBreakdown, exceptionDetailsList, invoice, fileName, errorLevelTax, fileSubmissionDate, parentMemo.BatchSequenceNumber, parentMemo.RecordSequenceWithinBatch, parentMemo.RejectionMemoNumber, parentMemo.SourceCodeId, string.Format("{0}-{1}-{2}", tktIsuingAirline, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber));
      }

      // TODO: To be released in Jan 2012
      // Validate reference data for source code 91, 92 and 93.
      //if ((parentMemo.SourceCodeId == 91 || parentMemo.SourceCodeId == 92 || parentMemo.SourceCodeId == 93) && (string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ReferenceField1) || string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ReferenceField2) || string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ReferenceField3) || string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ReferenceField4) || string.IsNullOrWhiteSpace(rejectionMemoCouponBreakdownRecord.ReferenceField5)))
      //{
      //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Reference Fields", string.Empty, invoice, fileName, errorLevel, ErrorCodes.ReferenceDataNotProvidedForFrequentFlyerRelatedBillings, ErrorStatus.X, parentMemo, false, string.Empty);
      //  exceptionDetailsList.Add(validationExceptionDetail);
      //  isValid = false;
      //}



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
    private bool ValidateTicketIssuingAirline(CouponBase couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, RejectionMemo parentMemo,
      int sourceCodeId, PaxInvoice invoice, string fileName, bool isValid, string errorLevel, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
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
                                                               invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", couponRecord.TicketOrFimIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else if (!issuingAirline[ticketIssuingAirline])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Or FimIssuing Airline", couponRecord.TicketOrFimIssuingAirline,
                                                             invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", couponRecord.TicketOrFimIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Validates the ticket issuing airline for billing memo.
    /// </summary>
    /// <param name="couponRecord"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="parentMemo"></param>
    /// <param name="sourceCodeId"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="isValid"></param>
    /// <param name="errorLevel"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <returns></returns>
    private bool ValidateTicketIssuingAirline(CouponBase couponRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, BillingMemo parentMemo,
      int sourceCodeId, PaxInvoice invoice, string fileName, bool isValid, string errorLevel, IDictionary<string, bool> issuingAirline, DateTime fileSubmissionDate)
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
                                                               invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", ticketIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      else if (!issuingAirline[ticketIssuingAirline])
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(couponRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Ticket Or FimIssuing Airline", couponRecord.TicketOrFimIssuingAirline,
                                                             invoice, fileName, errorLevel, ErrorCodes.InvalidTicOrFimIssuingAirline, ErrorStatus.X, parentMemo, false, string.Format("{0}-{1}-{2}", ticketIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber));
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }
      return isValid;
    }

    /// <summary>
    /// Validate parsed source source code total
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected bool ValidateParsedSourceCodeTotal(PaxInvoice invoice, DateTime fileSubmissionDate, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName)
    {
      bool isValid = true;
      int sourceCode = 0;

      //Validate source code totals
      foreach (var sourceCodeTotal in invoice.SourceCodeTotal)
      {
        sourceCode = sourceCodeTotal.SourceCodeId;
        int transactionType = 0;

        //Total Amount After Sampling Constant
        if (!(invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF) && sourceCodeTotal.TotalAmountAfterSamplingConstant != 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Amount After Sampling Constant", Convert.ToString(sourceCodeTotal.TotalAmountAfterSamplingConstant),
                                             invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.TotalAmountAfterSamplingConstantForSamplingXAndXF, ErrorStatus.X, sourceCodeTotal.SourceCodeId);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (!ReferenceManager.IsValidSourceCode(invoice, sourceCode, out transactionType))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Source Code", Convert.ToString(sourceCode),
                                             invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidSourceCode, ErrorStatus.X, sourceCodeTotal.SourceCodeId);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        else
        {
          //Source code total for prime billing 
          if (transactionType == Convert.ToInt32(TransactionType.Coupon) || transactionType == Convert.ToInt32(TransactionType.SamplingFormAB))
          {
            if (invoice.CouponDataRecord != null)
            {
              //For the source code
              isValid = ValidateParsedSourceCodeTotalForCouponRecords(invoice, sourceCodeTotal, sourceCode, exceptionDetailsList, fileName, fileSubmissionDate);
            }
          }
          //Source code total for rejection
          else if (transactionType == Convert.ToInt32(TransactionType.RejectionMemo1) ||
            transactionType == Convert.ToInt32(TransactionType.RejectionMemo2) ||
            transactionType == Convert.ToInt32(TransactionType.RejectionMemo3) ||
            transactionType == Convert.ToInt32(TransactionType.SamplingFormF) ||
            transactionType == Convert.ToInt32(TransactionType.SamplingFormXF))
          {
            if (invoice.RejectionMemoRecord != null)
            {
              //For the source code
              isValid = ValidateParsedSourceCodeTotalForRejectionMemoRecords(invoice, sourceCodeTotal, sourceCode, exceptionDetailsList, fileName, fileSubmissionDate);
            }
          }
          //Source code total for billing memo
          // Ref. SCP  ID : 8151
          else if (transactionType == Convert.ToInt32(TransactionType.BillingMemo) || transactionType == Convert.ToInt32(TransactionType.PasNsBillingMemoDueToAuthorityToBill) || transactionType == Convert.ToInt32(TransactionType.PasNsBillingMemoDueToExpiry))
          {
            if (invoice.BillingMemoRecord != null)
            {
              //For the source code
              isValid = ValidateParsedSourceCodeTotalForBillingMemoRecords(invoice, sourceCodeTotal, sourceCode, exceptionDetailsList, fileName, fileSubmissionDate);
            }
          }
          //Source code total for credit memo
          else if (transactionType == Convert.ToInt32(TransactionType.CreditMemo))
          {
            if (invoice.CreditMemoRecord != null)
            {
              //For the source code
              isValid = ValidateParsedSourceCodeTotalForCreditMemoRecords(invoice, sourceCodeTotal, sourceCode, exceptionDetailsList, fileName, fileSubmissionDate);
            }
          }
        }

        // Total of Tax amount in all the Tax records.
        decimal totalNetAmount = 0;

        //totalNetAmount = ConvertUtil.Round((sourceCodeTotal.TotalGrossValue + sourceCodeTotal.TotalTaxAmount + sourceCodeTotal.TotalVatAmount + sourceCodeTotal.TotalIscAmount + sourceCodeTotal.TotalUatpAmount + sourceCodeTotal.TotalOtherCommission + Convert.ToDecimal(sourceCodeTotal.TotalHandlingFee)), 2);

        //if (invoice.Tolerance != null && !CompareUtil.Compare(sourceCodeTotal.TotalNetAmount, totalNetAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
        //{
        //  var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net total amount", Convert.ToString(sourceCodeTotal.TotalNetAmount),
        //                                     invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.NetTotalDoesNotMatchWithTotalOfIndividualAmounts, ErrorStatus.X, sourceCodeTotal.SourceCodeId);
        //  exceptionDetailsList.Add(validationExceptionDetail);
        //  isValid = false;
        //}

        //Validate Vat Breakdowns 
        foreach (var sourceCodeVat in sourceCodeTotal.VatBreakdown)
        {
            isValid = ValidateParsedVat(sourceCodeVat, exceptionDetailsList, invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, fileSubmissionDate, 0, 0, string.Empty, sourceCodeTotal.SourceCodeId, true, true);
        }

      }

      return isValid;
    }

    /// <summary>
    /// Validate source code total fields with total of coupon records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool ValidateParsedSourceCodeTotalForCouponRecords(PaxInvoice invoice, SourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
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

      List<PrimeCoupon> couponSourceCodeRecords = null;
      couponSourceCodeRecords = invoice.CouponDataRecord.Where(coupon => coupon.SourceCodeId == sourceCode).ToList();
      if (couponSourceCodeRecords != null)
      {
        if (invoice.Tolerance != null)
        {
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalGrossValue,
                                 Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.CouponGrossValueOrApplicableLocalFare)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Value",
                                                                            sourceCodeTotal.TotalGrossValue.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidGrossTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalTaxAmount, Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.TaxAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount",
                                                                            sourceCodeTotal.TotalTaxAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidTaxTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalVatAmount, Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.VatAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidVatTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalUatpAmount, Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.UatpAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount",
                                                                            sourceCodeTotal.TotalUatpAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidUatpAmountTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalIscAmount, Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.IscAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount",
                                                                            sourceCodeTotal.TotalIscAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidIscTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalOtherCommission, Convert.ToDecimal(couponSourceCodeRecords.Sum(record => record.OtherCommissionAmount)), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount",
                                                                            sourceCodeTotal.TotalOtherCommission.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidOtherCommissionTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalHandlingFee, couponSourceCodeRecords.Sum(record => record.HandlingFeeAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount",
                                                                            sourceCodeTotal.TotalHandlingFee.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidHandlingFeeTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
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

        long totalNumberOfRecords = (couponSourceCodeRecords.Count + couponSourceCodeRecords.Sum(record => record.NumberOfChildRecords) + sourceCodeTotal.NumberOfChildRecords + 1);
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.TotalNumberOfRecords != totalNumberOfRecords)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidTotalNumberOfRecords, ErrorStatus.X, sourceCode, 99999, 99999, null, false, null, Convert.ToString(totalNumberOfRecords));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.NumberOfBillingRecords != couponSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Number of billing records", sourceCodeTotal.NumberOfBillingRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && sourceCodeTotal.NumberOfBillingRecords != couponSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Detail Count", sourceCodeTotal.NumberOfBillingRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidDetailCount, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

      }
      return isValid;
    }

    /// <summary>
    /// Validate source code total fields with total of rejection memo records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool ValidateParsedSourceCodeTotalForRejectionMemoRecords(PaxInvoice invoice, SourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
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

      List<RejectionMemo> rejectionMemoSourceCodeRecords = null;
      rejectionMemoSourceCodeRecords = invoice.RejectionMemoRecord.Where(rejectionMemo => rejectionMemo.SourceCodeId == sourceCode).ToList();
      if (rejectionMemoSourceCodeRecords != null)
      {
        if (invoice.Tolerance != null)
        {
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalGrossValue,
                                 Convert.ToDecimal(
                                   rejectionMemoSourceCodeRecords.Sum(record => record.TotalGrossDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Total Gross Value",
                                                                            sourceCodeTotal.TotalGrossValue.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidGrossTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalTaxAmount,
                                 Convert.ToDecimal(
                                   rejectionMemoSourceCodeRecords.Sum(record => record.TotalTaxAmountDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Total Tax Amount",
                                                                            sourceCodeTotal.TotalTaxAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidTaxTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalVatAmount,
                                 Convert.ToDecimal(
                                   rejectionMemoSourceCodeRecords.Sum(record => record.TotalVatAmountDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidVatTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalUatpAmount,
                                 Convert.ToDecimal(
                                   rejectionMemoSourceCodeRecords.Sum(record => record.UatpAmountDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Total UATP Amount",
                                                                            sourceCodeTotal.TotalUatpAmount.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidUatpAmountTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalIscAmount,
                                 Convert.ToDecimal(rejectionMemoSourceCodeRecords.Sum(record => record.IscDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, "Total ISC Amount",
                                                                            sourceCodeTotal.TotalIscAmount.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidIscTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalOtherCommission,
                                 Convert.ToDecimal(
                                   rejectionMemoSourceCodeRecords.Sum(record => record.OtherCommissionDifference)),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "Total Other Commission Amount",
                                                                            sourceCodeTotal.TotalOtherCommission.
                                                                              ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidOtherCommissionTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (
            !CompareUtil.Compare(sourceCodeTotal.TotalHandlingFee,
                                 rejectionMemoSourceCodeRecords.Sum(record => record.HandlingFeeAmountDifference),
                                 invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "Total Handling Fee Amount",
                                                                            sourceCodeTotal.TotalHandlingFee.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidHandlingFeeTotalOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (
          !CompareUtil.Compare(sourceCodeTotal.TotalAmountAfterSamplingConstant,
                               rejectionMemoSourceCodeRecords.Sum(
                                 record => record.TotalNetRejectAmountAfterSamplingConstant),
                               invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            "Total Net Reject Amount after sampling constant",
                                                                            sourceCodeTotal.
                                                                              TotalAmountAfterSamplingConstant.ToString(),
                                                                            invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.
                                                                              InvalidTotalNetRejectAmountOfSourceCodeTotalRecord,
                                                                            ErrorStatus.X,
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
        if (invoice.SubmissionMethodId == (int) SubmissionMethod.IsIdec &&
            sourceCodeTotal.NumberOfBillingRecords != rejectionMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          "Number of billing records",
                                                                          sourceCodeTotal.TotalOtherCommission.ToString(),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                          ErrorCodes.
                                                                            InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord,
                                                                          ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        //Total number of records validation
        long totalNumberOfRecords = (rejectionMemoSourceCodeRecords.Count +
                                     rejectionMemoSourceCodeRecords.Sum(record => record.NumberOfChildRecords) +
                                     rejectionMemoSourceCodeRecords.Sum(
                                       record =>
                                       record.CouponBreakdownRecord.Sum(
                                         couponRecord => couponRecord.NumberOfChildRecords)) +
                                     sourceCodeTotal.NumberOfChildRecords + 1);
        if (invoice.SubmissionMethodId == (int) SubmissionMethod.IsIdec &&
            invoice.SubmissionMethodId == (int) SubmissionMethod.IsIdec &&
            sourceCodeTotal.TotalNumberOfRecords != totalNumberOfRecords)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Total Number of Records",
                                                                          sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                          ErrorCodes.InvalidTotalNumberOfRecords,
                                                                          ErrorStatus.X, sourceCode, 99999, 99999, null,
                                                                          false, null,
                                                                          Convert.ToString(totalNumberOfRecords));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int) SubmissionMethod.IsXml &&
            sourceCodeTotal.NumberOfBillingRecords != rejectionMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, "Detail Count",
                                                                          sourceCodeTotal.NumberOfBillingRecords.
                                                                            ToString(),
                                                                          invoice, fileName,
                                                                          ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                          ErrorCodes.InvalidDetailCount, ErrorStatus.X,
                                                                          sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      return isValid;
    }

    /// <summary>
    /// Validate source code total fields with total of billing memo records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool ValidateParsedSourceCodeTotalForBillingMemoRecords(PaxInvoice invoice, SourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
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

      List<BillingMemo> billingMemoSourceCodeRecords = null;
      billingMemoSourceCodeRecords = invoice.BillingMemoRecord.Where(billingMemo => billingMemo.SourceCodeId == sourceCode).ToList();
      if (billingMemoSourceCodeRecords != null)
      {
        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(sourceCodeTotal.TotalGrossValue, billingMemoSourceCodeRecords.Sum(record => record.TotalGrossAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Value",
                                                                            sourceCodeTotal.TotalGrossValue.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidGrossTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalTaxAmount, billingMemoSourceCodeRecords.Sum(record => record.TaxAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount",
                                                                            sourceCodeTotal.TotalTaxAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidTaxTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalVatAmount, billingMemoSourceCodeRecords.Sum(record => record.TotalVatAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidVatTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalUatpAmount, billingMemoSourceCodeRecords.Sum(record => record.TotalUatpAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount",
                                                                            sourceCodeTotal.TotalUatpAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidUatpAmountTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalIscAmount, billingMemoSourceCodeRecords.Sum(record => record.TotalIscAmountBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount",
                                                                            sourceCodeTotal.TotalIscAmount.ToString(), invoice, fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidIscTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalOtherCommission, billingMemoSourceCodeRecords.Sum(record => record.TotalOtherCommissionAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount",
                                                                            sourceCodeTotal.TotalOtherCommission.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidOtherCommissionTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalHandlingFee, billingMemoSourceCodeRecords.Sum(record => record.TotalHandlingFeeBilled), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
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
        long totalNumberOfRecords = billingMemoSourceCodeRecords.Count + billingMemoSourceCodeRecords.Sum(record => record.NumberOfChildRecords) + billingMemoSourceCodeRecords.Sum(record => record.CouponBreakdownRecord.Sum(couponRecord => couponRecord.NumberOfChildRecords)) + sourceCodeTotal.NumberOfChildRecords + 1;

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.TotalNumberOfRecords != totalNumberOfRecords)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidTotalNumberOfRecords, ErrorStatus.X, sourceCode, 99999, 99999, null, false, null, Convert.ToString(totalNumberOfRecords));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.NumberOfBillingRecords != billingMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Number of billing records", sourceCodeTotal.TotalOtherCommission.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && sourceCodeTotal.NumberOfBillingRecords != billingMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Detail Count", sourceCodeTotal.NumberOfBillingRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidDetailCount, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }
      return isValid;
    }

    /// <summary>
    /// Validate source code total fields with total of credit memo records
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="sourceCodeTotal"></param>
    /// <param name="sourceCode"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool ValidateParsedSourceCodeTotalForCreditMemoRecords(PaxInvoice invoice, SourceCodeTotal sourceCodeTotal, int sourceCode, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
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

      List<CreditMemo> creditMemoSourceCodeRecords = null;
      creditMemoSourceCodeRecords = invoice.CreditMemoRecord.Where(creditMemo => creditMemo.SourceCodeId == sourceCode).ToList();
      if (creditMemoSourceCodeRecords != null)
      {
        if (invoice.Tolerance != null)
        {
          if (!CompareUtil.Compare(sourceCodeTotal.TotalGrossValue, creditMemoSourceCodeRecords.Sum(record => record.TotalGrossAmountCredited), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Value",
                                                                            sourceCodeTotal.TotalGrossValue.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidGrossTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalTaxAmount, creditMemoSourceCodeRecords.Sum(record => record.TaxAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Amount",
                                                                            sourceCodeTotal.TotalTaxAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidTaxTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalVatAmount, creditMemoSourceCodeRecords.Sum(record => record.VatAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Amount",
                                                                            sourceCodeTotal.TotalVatAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidVatTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalUatpAmount, creditMemoSourceCodeRecords.Sum(record => record.TotalUatpAmountCredited), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Amount",
                                                                            sourceCodeTotal.TotalUatpAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidUatpAmountTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalIscAmount, creditMemoSourceCodeRecords.Sum(record => record.TotalIscAmountCredited), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Amount",
                                                                            sourceCodeTotal.TotalIscAmount.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidIscTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalOtherCommission, creditMemoSourceCodeRecords.Sum(record => record.TotalOtherCommissionAmountCredited), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission Amount",
                                                                            sourceCodeTotal.TotalOtherCommission.ToString(),
                                                                            invoice,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidOtherCommissionTotalOfSourceCodeTotalRecord, ErrorStatus.X,
                                                                            sourceCode,
                                                                            99999,
                                                                            99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          if (!CompareUtil.Compare(sourceCodeTotal.TotalHandlingFee, creditMemoSourceCodeRecords.Sum(record => record.TotalHandlingFeeCredited), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount",
                                                                            sourceCodeTotal.TotalHandlingFee.ToString(), invoice,
                                                                            fileName, ErrorLevels.ErrorLevelSourceCodeTotal,
                                                                            ErrorCodes.InvalidHandlingFeeTotalOfSourceCodeTotalRecord, ErrorStatus.X, sourceCode, 99999, 99999);
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
        var totalNumberOfRecords = sourceCodeTotal.TotalNumberOfRecords != (creditMemoSourceCodeRecords.Count + creditMemoSourceCodeRecords.Sum(record => record.NumberOfChildRecords) + creditMemoSourceCodeRecords.Sum(record => record.CouponBreakdownRecord.Sum(couponRecord => couponRecord.NumberOfChildRecords)) + sourceCodeTotal.NumberOfChildRecords + 1);
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && totalNumberOfRecords)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", sourceCodeTotal.TotalNumberOfRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidTotalNumberOfRecords, ErrorStatus.X, sourceCode, 99999, 99999, null, false, null, Convert.ToString(totalNumberOfRecords));
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //Number of billing records validation
        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && sourceCodeTotal.NumberOfBillingRecords != creditMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Number of billing records", sourceCodeTotal.TotalOtherCommission.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidNumberOfBillingRecordsOfSourceCodeTotalRecord, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsXml && sourceCodeTotal.NumberOfBillingRecords != creditMemoSourceCodeRecords.Count())
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(sourceCodeTotal.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Detail Count", sourceCodeTotal.NumberOfBillingRecords.ToString(),
                                                              invoice, fileName, ErrorLevels.ErrorLevelSourceCodeTotal, ErrorCodes.InvalidDetailCount, ErrorStatus.X, sourceCode, 99999, 99999);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
       

      }
      return isValid;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private bool ValidateParsedInvoiceTotal(PaxInvoice invoice, DateTime fileSubmissionDate, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName)
    {
      bool isValid = true;

      //Update sampling constant of invoice to invoice total.
      if (invoice.InvoiceTotalRecord != null)
      {
        invoice.InvoiceTotalRecord.SamplingConstant = invoice.SamplingConstant;

        if (invoice.InvoiceTotalVat.Count > 0)
        {
          // Total of vat amount in all the vat records.
          double totalVat = 0;

          totalVat = invoice.InvoiceTotalVat.Sum(vatdata => (vatdata.VatCalculatedAmount.HasValue) ? vatdata.VatCalculatedAmount.Value : 0);

          if (invoice.Tolerance != null && !CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalVatAmount, Convert.ToDecimal(totalVat), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Total Vat Amount",
                                                                            invoice.InvoiceTotalRecord.TotalVatAmount.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal,
                                                                            ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, 00, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
        else
        {
          if (invoice.InvoiceTotalRecord.TotalVatAmount != 0)
          {

            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Invoice Total Vat Amount",
                                                                              invoice.InvoiceTotalRecord.TotalVatAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal,
                                                                              ErrorCodes.VatBreakdownRecordsRequired, ErrorStatus.X, 00, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }

        if (invoice.SourceCodeTotal != null)
        {
          //Provisional Adjustment Rate should be 0 for Billing Code = 0 
          if (invoice.InvoiceTotalRecord.ProvAdjustmentRate != 0 && invoice.BillingCode == (int)BillingCode.NonSampling)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Adjustment Rate", Convert.ToString(invoice.InvoiceTotalRecord.ProvAdjustmentRate),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.ProvisionalAdjustmentRate, ErrorStatus.X, 00, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (invoice.BillingCode == (int)BillingCode.NonSampling && invoice.InvoiceTotalRecord.TotalVatAmountAfterSamplingConstant != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total VAT Amount After Sampling Constant", Convert.ToString(invoice.InvoiceTotalRecord.TotalVatAmountAfterSamplingConstant),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidVatAmountAfterSamplingConstant, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Fare Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.FareAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Fare Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.FareAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.FareAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Fare Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.FareAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Fare Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.FareAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.FareAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Handling Fee Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.HandlingFeeAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.HandlingFeeAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.HandlingFeeAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Handling Fee Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.HandlingFeeAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Handling Fee Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.HandlingFeeAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.HandlingFeeAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //ISC Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.IscAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "ISC Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.IscAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.IscAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //ISC Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.IscAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "ISC Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.IscAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.IscAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Other Commission Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.OtherCommissionAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.OtherCommissionAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.OtherCommissionAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Other Commission Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.OtherCommissionAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Other Commission Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.OtherCommissionAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.OtherCommissionAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Tax Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.TaxAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.TaxAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.TaxAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Tax Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.TaxAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.TaxAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.TaxAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //UATP Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.UatpAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.UatpAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.UatpAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //UATP Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.UatpAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "UATP Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.UatpAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.UatpAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Vat Absorption Amount validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.VatAbsorptionAmount != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Absorption Amount", Convert.ToString(invoice.InvoiceTotalRecord.VatAbsorptionAmount),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.VatAbsorptionAmountIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Vat Absorption Percent validation
          if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.VatAbsorptionPercent != 0)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Absorption Percent", Convert.ToString(invoice.InvoiceTotalRecord.VatAbsorptionPercent),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.VatAbsorptionPercentIsInvalid, ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          //Total provisional adjustment amount
          /*    if (invoice.BillingCode != (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount != 0)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Provisional Adjustment Amount", Convert.ToString(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.TotalProvisionalAdjustmentAmountIsInvalid, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }

              if (invoice.BillingCode == (int)BillingCode.SamplingFormAB && invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount == 0)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Provisional Adjustment Amount", Convert.ToString(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.TotalProvisionalAdjustmentAmountIsInvalid, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }*/

          if (invoice.Tolerance != null)
          {
            //Total Net Amount without VAT should be equal to Net Total – Total Vat Amount.
            if (invoice.BillingCode == (int)BillingCode.NonSampling && !CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalNetAmountWithoutVat, ConvertUtil.Round((invoice.InvoiceTotalRecord.NetTotal - invoice.InvoiceTotalRecord.TotalVatAmount), Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount Without Vat", Convert.ToString(invoice.InvoiceTotalRecord.TotalNetAmountWithoutVat),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.TotalNetAmountWithoutVatIsInvalidForNs, ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            //In case of Billing Code 6 and 7 this field should be populated. It should be equal Net Amount After Sampling Constant - Total VAT Amount after Sampling Constant
            if ((invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF))
            {
              //Total Net Amount After Sampling Constant should be zero for non sampling
              if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && !CompareUtil.Compare(invoice.InvoiceTotalRecord.NetAmountAfterSamplingConstant, invoice.SourceCodeTotal.Sum(record => record.TotalAmountAfterSamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Amount After Sampling Constant", Convert.ToString(invoice.InvoiceTotalRecord.NetAmountAfterSamplingConstant),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidNetAmountAfterSamplingConstant, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }

              if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && !CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalNetAmountWithoutVat, ConvertUtil.Round((invoice.InvoiceTotalRecord.NetAmountAfterSamplingConstant - invoice.InvoiceTotalRecord.TotalVatAmountAfterSamplingConstant), Constants.PaxDecimalPlaces), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Net Amount Without Vat", Convert.ToString(invoice.InvoiceTotalRecord.TotalNetAmountWithoutVat), invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, SamplingErrorCodes.TotalNetAmountWithoutVatIsInvalidForSampling, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }

            }

            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalGrossValue, invoice.SourceCodeTotal.Sum(record => record.TotalGrossValue), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Gross Value",
                                                                              invoice.InvoiceTotalRecord.TotalGrossValue.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalGrossValue, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalTaxAmount, invoice.SourceCodeTotal.Sum(record => record.TotalTaxAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Tax Value", invoice.InvoiceTotalRecord.TotalTaxAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalVatAmount, invoice.SourceCodeTotal.Sum(record => record.TotalVatAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Vat Value",
                                                                              invoice.InvoiceTotalRecord.TotalVatAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalUatpAmount, invoice.SourceCodeTotal.Sum(record => record.TotalUatpAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total UATP Value", invoice.InvoiceTotalRecord.TotalUatpAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal,
                                                                              ErrorCodes.InvalidTotalUatpAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalIscAmount, invoice.SourceCodeTotal.Sum(record => record.TotalIscAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total ISC Value", invoice.InvoiceTotalRecord.TotalIscAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal,
                                                                              ErrorCodes.InvalidTotalIscAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalOtherCommission, invoice.SourceCodeTotal.Sum(record => record.TotalOtherCommission), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Other Commission",
                                                                              invoice.InvoiceTotalRecord.TotalOtherCommission.ToString(),
                                                                              invoice, fileName,
                                                                              ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalOtherCommissionAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalHandlingFee, invoice.SourceCodeTotal.Sum(record => record.TotalHandlingFee), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Handling Fee Amount", invoice.InvoiceTotalRecord.TotalOtherCommission.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalHandlingFeeAmount, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            //The total provisional adjustment amount should be sum of individual amounts or total amount * adjustment rate.
            if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
            {

              if (invoice.InvoiceTotalRecord.TaxAbsorptionAmount != 0 || invoice.InvoiceTotalRecord.OtherCommissionAbsorptionAmount != 0 && invoice.InvoiceTotalRecord.IscAbsorptionAmount != 0 && invoice.InvoiceTotalRecord.UatpAbsorptionAmount != 0 && invoice.InvoiceTotalRecord.VatAbsorptionAmount != 0 && invoice.InvoiceTotalRecord.HandlingFeeAbsorptionAmount != 0)
              {
                var totalAbsorbtionAmount = invoice.InvoiceTotalRecord.FareAbsorptionAmount + invoice.InvoiceTotalRecord.TaxAbsorptionAmount + invoice.InvoiceTotalRecord.OtherCommissionAbsorptionAmount + invoice.InvoiceTotalRecord.IscAbsorptionAmount + invoice.InvoiceTotalRecord.UatpAbsorptionAmount + invoice.InvoiceTotalRecord.VatAbsorptionAmount + Convert.ToDecimal(invoice.InvoiceTotalRecord.HandlingFeeAbsorptionAmount);

                if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount, totalAbsorbtionAmount, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Provisional Adjustment Amount",
                                                                                  Convert.ToString(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount),
                                                                                  invoice, fileName,
                                                                                  ErrorLevels.ErrorLevelInvoiceTotal,
                                                                                  SamplingErrorCodes.TotalProvAdjustAmountDoesNotEqualToSumOfAbsAmount, ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }

              if (invoice.InvoiceTotalRecord.ProvAdjustmentRate != 0 && !(CompareUtil.Compare(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount,
                  (Convert.ToDecimal(invoice.InvoiceTotalRecord.ProvAdjustmentRate) * invoice.InvoiceTotalRecord.TotalGrossValue / 100), invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces)))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Provisional Adjustment Amount",
                                                                                Convert.ToString(invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount),
                                                                                invoice, fileName,
                                                                                ErrorLevels.ErrorLevelInvoiceTotal,
                                                                                SamplingErrorCodes.TotalProvisionalAdjustmentAmountIsInvalid, ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }

              if (invoice.InvoiceTotalRecord.TaxAbsorptionPercent != 0 || invoice.InvoiceTotalRecord.OtherCommissionAbsorptionPercent != 0 && invoice.InvoiceTotalRecord.IscAbsorptionPercent != 0 && invoice.InvoiceTotalRecord.UatpAbsorptionPercent != 0 && invoice.InvoiceTotalRecord.VatAbsorptionPercent != 0 && invoice.InvoiceTotalRecord.HandlingFeeAbsorptionPercent != 0)
              {
                var totalAbsorbtionPercent = invoice.InvoiceTotalRecord.FareAbsorptionPercent + invoice.InvoiceTotalRecord.TaxAbsorptionPercent + invoice.InvoiceTotalRecord.OtherCommissionAbsorptionPercent + invoice.InvoiceTotalRecord.IscAbsorptionPercent + invoice.InvoiceTotalRecord.UatpAbsorptionPercent + invoice.InvoiceTotalRecord.VatAbsorptionPercent + invoice.InvoiceTotalRecord.HandlingFeeAbsorptionPercent;

                if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.ProvAdjustmentRate, totalAbsorbtionPercent, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Provisional Adjustment Rate",
                                                                                   Convert.ToString(invoice.InvoiceTotalRecord.ProvAdjustmentRate),
                                                                                   invoice, fileName,
                                                                                   ErrorLevels.ErrorLevelInvoiceTotal,
                                                                                   ErrorCodes.ParRateDoesNotMatchWithSumOfIndAbsorbPercent, ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }

            }

            //In case of Billing Code = 3 the Net Total should be equal to the sum of Total Gross Value and Total Provisional Adjustment  Amount after taking the sign field of both the values under consideration.
            //In case of Billing Code = 6 or 7 the Net Total amount should be equal to Net Amount after Sampling Constant.For all other billing codes, this equals the sum of Net Total value of all Source Code Total records within the Invoice
            if (invoice.BillingCode == 6 || invoice.BillingCode == 7)
            {
              if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.NetTotal, invoice.SourceCodeTotal.Sum(record => record.TotalAmountAfterSamplingConstant), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Total", invoice.InvoiceTotalRecord.NetTotal.ToString(),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidNetTotalAmountForSamplingFxf, ErrorStatus.X, 0, 99999, 99999);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            }
            else if (invoice.BillingCode == 3)
            {
              var netTotalForFormAb = invoice.InvoiceTotalRecord.TotalGrossValue + invoice.InvoiceTotalRecord.TotalProvisionalAdjustmentAmount;
              if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.NetTotal, netTotalForFormAb, invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Total", invoice.InvoiceTotalRecord.NetTotal.ToString(),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidNetTotalAmountForSamplingAb, ErrorStatus.X, 0, 99999, 99999);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            }
            else
            {
              if (!CompareUtil.Compare(invoice.InvoiceTotalRecord.NetTotal, invoice.SourceCodeTotal.Sum(record => record.TotalNetAmount), invoice.Tolerance.SummationTolerance, Constants.PaxDecimalPlaces))
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Total", invoice.InvoiceTotalRecord.NetTotal.ToString(),
                                                                                invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal,
                                                                                ErrorCodes.InvalidNetTotal, ErrorStatus.X, 0, 99999, 99999);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            }

            //Net Billing Amount should be equal to Net Total / Listing to Billing Rate rounded to two decimal places
             
            if (invoice.ExchangeRate.HasValue && invoice.ExchangeRate.Value != 0 && !CompareUtil.Compare(invoice.InvoiceTotalRecord.NetBillingAmount, invoice.InvoiceTotalRecord.NetTotal / invoice.ExchangeRate.Value, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Net Billing Amount", invoice.InvoiceTotalRecord.NetBillingAmount.ToString(),
                                                                              invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.NetBillingAmountDoesNotMatchWithNetTotAndLbr, ErrorStatus.X, 0, 99999, 99999);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

          }
          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && invoice.InvoiceTotalRecord.NoOfBillingRecords != invoice.SourceCodeTotal.Sum(record => record.NumberOfBillingRecords))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total number of billing records", invoice.InvoiceTotalRecord.NoOfBillingRecords.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalNoOfBillingRecordsOfInvoiceTotal, ErrorStatus.X, 0, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }

          if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec && invoice.InvoiceTotalRecord.TotalNoOfRecords != (invoice.SourceCodeTotal.Sum(record => record.TotalNumberOfRecords) + invoice.NumberOfChildRecords + 1))
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Total Number of Records", invoice.InvoiceTotalRecord.TotalNoOfRecords.ToString(),
                                                                            invoice, fileName, ErrorLevels.ErrorLevelInvoiceTotal, ErrorCodes.InvalidTotalNoOfRecordsOfInvoiceTotal, ErrorStatus.X, 0, 99999, 99999);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
        }
      }

      //Validate Vat Breakdowns 
      foreach (var invoiceVat in invoice.InvoiceTotalVat)
      {
        if (invoiceVat.VatIdentifierId == 0)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Identifier", invoiceVat.Identifier, invoice, fileName, ErrorLevels.ErrorLevelInvoiceVat, ErrorCodes.InvalidVatIdentifier, ErrorStatus.X, 0, 0, 0, string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        if (string.IsNullOrWhiteSpace(invoiceVat.VatText))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Text", string.Empty, invoice, fileName, ErrorLevels.ErrorLevelInvoiceVat, ErrorCodes.InvalidVatText, ErrorStatus.X, 0, 0, 0, string.Empty);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        //CMP464
        else if (invoiceVat.VatText.Length>50)
        {
            var validationExceptionDetail = CreateValidationExceptionDetail(invoice.InvoiceTotalRecord.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Text", invoiceVat.VatText, invoice, fileName, ErrorLevels.ErrorLevelInvoiceVat, ErrorCodes.InvalidTaxVatLength, ErrorStatus.X, 0, 0, 0, string.Empty);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

      }

      return isValid;
    }

    /// <summary>
    /// Get linking details for rejection memo
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public RMLinkingResultDetails GetRejectionMemoLinkingDetails(RMLinkingCriteria criteria)
    {
      var result = RejectionMemoRecordRepository.GetRMLinkingDetails(criteria);
      if (result.MemoAmount != null)
      {
        SetRMLinkedMemoAmountDifference(result.MemoAmount, criteria.RejectionStage);
      }

      return result;
    }

    /// <summary>
    /// Get linking details for rejection memo when multiple records are found for rejected entity then as per user selection fetch data for selected memo
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria)
    {
      var result = RejectionMemoRecordRepository.GetLinkedMemoAmountDetails(criteria);
      if (result.MemoAmount != null)
      {
        SetRMLinkedMemoAmountDifference(result.MemoAmount, criteria.RejectionStage);
      }

      return result;
    }

    private void SetRMLinkedMemoAmountDifference(RMLinkingAmount memoAmount, int rejectionStage)
    {
      if (memoAmount != null)
      {
        memoAmount.TotalGrossAcceptedAmount = memoAmount.TotalGrossAmountBilled;
        memoAmount.TotalTaxAmountAccepted = memoAmount.TotalTaxAmountBilled;
        memoAmount.AcceptedIscAmount = memoAmount.AllowedIscAmount;
        memoAmount.AcceptedOtherCommission = memoAmount.AllowedOtherCommission;
        memoAmount.AcceptedUatpAmount = memoAmount.AllowedUatpAmount;
        memoAmount.AcceptedHandlingFee = memoAmount.AllowedHandlingFee;
        memoAmount.TotalVatAmountAccepted = memoAmount.TotalVatAmountBilled;
        if (rejectionStage == (int)RejectionStage.StageOne || rejectionStage == (int)RejectionStage.StageThree)
        {
          memoAmount.TotalGrossDifference = memoAmount.TotalGrossAmountBilled - memoAmount.TotalGrossAcceptedAmount;
          memoAmount.TotalTaxAmountDifference = memoAmount.TotalTaxAmountBilled - memoAmount.TotalTaxAmountAccepted;
          memoAmount.IscDifference = memoAmount.AllowedIscAmount - memoAmount.AcceptedIscAmount;
          memoAmount.OtherCommissionDifference = memoAmount.AllowedOtherCommission - memoAmount.AcceptedOtherCommission;
          memoAmount.UatpAmountDifference = memoAmount.AllowedUatpAmount - memoAmount.AcceptedUatpAmount;
          memoAmount.HandlingFeeAmountDifference = memoAmount.AllowedHandlingFee - memoAmount.AcceptedHandlingFee;
          memoAmount.TotalVatAmountDifference = memoAmount.TotalVatAmountBilled - memoAmount.TotalVatAmountAccepted;
        }
        else if (rejectionStage == (int)RejectionStage.StageTwo)
        {
          memoAmount.TotalGrossDifference = memoAmount.TotalGrossAcceptedAmount - memoAmount.TotalGrossAmountBilled;
          memoAmount.TotalTaxAmountDifference = memoAmount.TotalTaxAmountAccepted - memoAmount.TotalTaxAmountBilled;
          memoAmount.IscDifference = memoAmount.AcceptedIscAmount - memoAmount.AllowedIscAmount;
          memoAmount.OtherCommissionDifference = memoAmount.AcceptedOtherCommission - memoAmount.AllowedOtherCommission;
          memoAmount.UatpAmountDifference = memoAmount.AcceptedUatpAmount - memoAmount.AllowedUatpAmount;
          memoAmount.HandlingFeeAmountDifference = memoAmount.AcceptedHandlingFee - memoAmount.AllowedHandlingFee;
          memoAmount.TotalVatAmountDifference = memoAmount.TotalVatAmountAccepted - memoAmount.TotalVatAmountBilled;
        }
        memoAmount.TotalNetRejectAmount = memoAmount.TotalGrossDifference + memoAmount.TotalTaxAmountDifference +
                                          memoAmount.IscDifference + memoAmount.OtherCommissionDifference
                                          + memoAmount.UatpAmountDifference + memoAmount.HandlingFeeAmountDifference +
                                          memoAmount.TotalVatAmountDifference;

      }
    }

    public IList<PrimeCoupon> GetPrimeBillingCouponList(string[] couponIdList)
    {
      var couponList = new List<PrimeCoupon>();

      foreach (var couponId in couponIdList)
      {
        var couponGuid = couponId.ToGuid();
        // Replaced with LoadStrategy call
        couponList.Add(CouponRecordRepository.Single(couponId: couponGuid));
        // couponList.Add(CouponRecordRepository.Single(coupon => coupon.Id == couponGuid));
      }

      return couponList;
    }

    /// <summary>
    /// get RM Coupon break down record details
    /// </summary>
    public RMLinkedCouponDetails GetRMCouponBreakdownRecordDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
    {
      // return RejectionMemoCouponBreakdownRepository.GetRMCouponBreakdownRecordDetailsFake(TicketIssuingAirline, TicketNumber, CouponNumber);
      var getCouponDetails = RejectionMemoCouponBreakdownRepository.GetRMCouponLinkingDetails(issuingAirline, couponNo, ticketDocNo, rmId, billingMemberId, billedMemberId);

      return getCouponDetails;
    }

    /// <summary>
    /// Get the single record details from the list of RM coupon
    /// </summary>
    public RMLinkedCouponDetails GetRMCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      var linkedCouponDetails = RejectionMemoCouponBreakdownRepository.GetLinkedCouponAmountDetails(couponId, rejectionMemoId, billingMemberId, billedMemberId);

      if (linkedCouponDetails != null && linkedCouponDetails.Details != null)
      {
        // Round the amounts to two decimal places - otherwise comparison does not work.
        linkedCouponDetails.Details.AllowedIscAmount = ConvertUtil.Round(linkedCouponDetails.Details.AllowedIscAmount, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AcceptedIscAmount = ConvertUtil.Round(linkedCouponDetails.Details.AcceptedIscAmount, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AllowedUatpAmount = ConvertUtil.Round(linkedCouponDetails.Details.AllowedUatpAmount, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AcceptedUatpAmount = ConvertUtil.Round(linkedCouponDetails.Details.AcceptedUatpAmount, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AllowedHandlingFee = ConvertUtil.Round(linkedCouponDetails.Details.AllowedHandlingFee, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AcceptedHandlingFee = ConvertUtil.Round(linkedCouponDetails.Details.AcceptedHandlingFee, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.GrossAmountAccepted = ConvertUtil.Round(linkedCouponDetails.Details.GrossAmountAccepted, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.GrossAmountBilled = ConvertUtil.Round(linkedCouponDetails.Details.GrossAmountBilled, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AllowedOtherCommission = ConvertUtil.Round(linkedCouponDetails.Details.AllowedOtherCommission, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.AcceptedOtherCommission = ConvertUtil.Round(linkedCouponDetails.Details.AcceptedOtherCommission, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.TaxAmountAccepted = ConvertUtil.Round(linkedCouponDetails.Details.TaxAmountAccepted, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.TaxAmountBilled = ConvertUtil.Round(linkedCouponDetails.Details.TaxAmountBilled, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.VatAmountAccepted = ConvertUtil.Round(linkedCouponDetails.Details.VatAmountAccepted, Constants.PaxDecimalPlaces);
        linkedCouponDetails.Details.VatAmountBilled = ConvertUtil.Round(linkedCouponDetails.Details.VatAmountBilled, Constants.PaxDecimalPlaces);
      }

      return linkedCouponDetails;
    }

    /// <summary>
    /// Gets the billing memos for a correspondence.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    public ExistingBMTransactionDetails GetBillingMemosForCorrespondence(long correspondenceNumber, int billingMemberId)
    {
        var transactionDetails = new ExistingBMTransactionDetails();
      transactionDetails.Transactions = InvoiceRepository.GetBillingMemosForCorrespondence(correspondenceNumber, billingMemberId);
      return transactionDetails;
    }

    /// <summary>
    /// This will return the CouponSearchCriteria in the below form
    /// 'TicketIssuingAirline1-TicketCouponNo1-TicketDocNo1,TicketIssuingAirline2-TicketCouponNo2-TicketDocNo2,.....'
    /// </summary>
    /// <param name="couponSearchCriterias"></param>
    /// <returns></returns>
    private string CreateCouponSearchCriteriaString(IEnumerable<CouponSearchCriteria> couponSearchCriterias)
    {
      var couponSearchCriteriaCollection = new List<string>();
      foreach (var couponSearchCriteria in couponSearchCriterias)
      {
        var searchCriterialArray = new string[3];
        searchCriterialArray[0] = couponSearchCriteria.TicketIssuingAirline;
        searchCriterialArray[1] = couponSearchCriteria.TicketCouponNo.ToString();
        searchCriterialArray[2] = couponSearchCriteria.TicketDocNo.ToString();
        couponSearchCriteriaCollection.Add(string.Join("-", searchCriterialArray));
      }
      return string.Join(",", couponSearchCriteriaCollection);
    }

    /// <summary>
    /// Following method is used to create VelocityContext for creating AuditTrail.html file, from which we create AuditTrail.pdf file.
    /// </summary>
    /// <param name="auditTrail">AuditTrail details</param>
    /// <returns>AuditTrail.html file</returns>
    //CMP508:Audit Trail Download with Supporting Documents
    public string GeneratePaxBillingHistoryAuditTrailPdf(PaxAuditTrail auditTrail)
    {
      _templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
      
       var context = GetVelocityContext(auditTrail);
        
      // Generate Audit trail html string using .vm file and NVelocity context
      var reportContent = _templatedTextGenerator.GenerateEmbeddedTemplatedText(PaxBillingHistoryAuditTrailTemplateResosurceName, context);
      
      // return Audit trail html string)
      return reportContent;
    }

    //CMP508:Audit Trail Download with Supporting Documents
      /// <summary>
      /// Returns Html string for audit trail with supporting docs assigned with their folder numbers
      /// </summary>
      /// <param name="auditTrail">audit trail for which html is to be genereated</param>
      /// <param name="suppDocs">out parameter for Supp Docs</param>
      /// <returns>Html for audit trail</returns>
    public string GeneratePaxBillingHistoryAuditTrailPackage(PaxAuditTrail auditTrail, out Dictionary<Attachment, int> suppDocs)
    {
        _templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        var context = GetVelocityContext(auditTrail, true);
        var reportContent = _templatedTextGenerator.GenerateEmbeddedTemplatedText(PaxBillingHistoryAuditTrailTemplateResosurceName, context);

        suppDocs = (Dictionary<Attachment, int>)context.Get("suppDocs");

        // return Audit trail html string)
        return reportContent;
    }

    //CMP508:Audit Trail Download with Supporting Documents
      /// <summary>
      /// Get velocity context for pax audit trail
      /// </summary>
      /// <param name="auditTrail">audit trail</param>
      /// <param name="downloadPackage">true if request is for package download</param>
      /// <returns>Velocity context for pax audit trail</returns>
    private VelocityContext GetVelocityContext(PaxAuditTrail auditTrail,bool downloadPackage = false)
    {
        var memoList = new List<RejectionMemo>();

        // Iterate through Audit trail invoices and retrieve rejectionMemo Invoices
        foreach (var invoice in auditTrail.Invoices.Where(invoice => invoice.RejectionMemoRecord.Count > 0))
        {
            memoList.AddRange(invoice.RejectionMemoRecord);
        }

        // Sort retrieved RejectionMemo invoices in descending order
        memoList = memoList.OrderByDescending(memo => memo.RejectionStage).ToList();

        // Retrieve prime coupon list, credit memo list, billing memo list, form D list from Audit trail invoices
        var primeCouponList = auditTrail.Invoices.Where(paxInvoice => paxInvoice.CouponDataRecord.Count > 0);
        var creditMemoList = auditTrail.Invoices.Where(paxInvoice => paxInvoice.CreditMemoRecord.Count > 0);
        var billingMemoList = auditTrail.Invoices.Where(paxInvoice => paxInvoice.BillingMemoRecord.Count > 0);
        var formDList = auditTrail.Invoices.Where(paxInvoice => paxInvoice.SamplingFormDRecord.Count > 0);
        var paxPrimeInvoiceList = auditTrail.Invoices.Where(paxPbInvoice => paxPbInvoice.CouponDataRecord.Count > 0);
        var samplingFormC = (auditTrail.SamplingFormC.Count > 0 ? auditTrail.SamplingFormC[0] : null);

        // Instantiate VelocityContext
        var context = new VelocityContext();
        // Instantiate NonSamplingInvoiceManager
        var nsim = new NonSamplingInvoiceManager();

        // Add memo list, prime coupon list, credit memo list, billing memo list, form D list in NVelocity context so we can access them in .vm file
        context.Put("memoList", memoList);
        context.Put("auditTrail", auditTrail);
        context.Put("primeCouponList", primeCouponList);
        context.Put("creditMemoList", creditMemoList);
        context.Put("billingMemoList", billingMemoList);
        context.Put("formDList", formDList);
        context.Put("paxPrimeInvoiceList", paxPrimeInvoiceList);
        context.Put("samplingFormC", samplingFormC);
        context.Put("formCProvisionalPeriod", (samplingFormC != null ? samplingFormC.ProvisionalBillingPeriod.Period : 0));
        context.Put("nsim", nsim);

        //CMP508:Audit Trail Download with Supporting Documents
        context.Put("downloadPackage", downloadPackage);
        Dictionary<Attachment, int> suppDocs = new Dictionary<Attachment, int>();
        context.Put("suppDocs", suppDocs);
        return context;
    }

    /// <summary>
    /// Following method sorts Correspondence details in descending order depending on stage. Executed from .vm file 
    /// </summary>
    /// <param name="rejectionMemo">Rejection memo object</param>
    /// <returns>Rejection memo with correspondence in sorted in descending order</returns>
    public RejectionMemo GetCorrespondenceDetails(RejectionMemo rejectionMemo)
    {
      // Sort Correspondence in descending order on Stage number
      rejectionMemo.Correspondences = rejectionMemo.Correspondences.OrderByDescending(c => c.CorrespondenceStage).ToList();
      // return rejection memo
      return rejectionMemo;
    }

    /// <summary>
    /// Following method is executed from .vm file to check whether RejectionMemo is already displayed while creating .pdf file.  
    /// </summary>
    /// <param name="rejectionMemoString">String of RejectionMemo Id's</param>
    /// <param name="rejectionMemoId">RejectionMemo Id, to check whether it is displayed</param>
    /// <returns>Returns "Yes" if RejectionMemo is already displayed, else returns "No"</returns>
    public string IsRejectionMemoDisplayed(string rejectionMemoString, Guid rejectionMemoId)
    {
      // If RejectionMemo is already displayed return "Yes", else return "No"
      if (rejectionMemoString.Contains(rejectionMemoId.ToString()))
        return "Yes";
      else
        return "No";
    }

    /// <summary>
    /// Following method retrieves Stage2 RejectionMemo details
    /// </summary>
    /// <param name="memoList">RejectionMemo List</param>
    /// <param name="rejectionMemo">RejectionMemo to find in list</param>
    /// <returns>Stage2 RejectionMemo</returns>
    public RejectionMemo GetRejectionStage2MemoDetails(List<RejectionMemo> memoList, RejectionMemo rejectionMemo)
    {
      var stage2RM =
        memoList.Find(
          memo =>
          memo.RejectionMemoNumber.ToUpper() == rejectionMemo.YourRejectionNumber.ToUpper() && memo.Invoice.InvoiceNumber.ToUpper() == rejectionMemo.YourInvoiceNumber.ToUpper() &&
          memo.Invoice.BillingPeriod == rejectionMemo.YourInvoiceBillingPeriod && memo.Invoice.BillingMonth == rejectionMemo.YourInvoiceBillingMonth &&
          memo.Invoice.BillingYear == rejectionMemo.YourInvoiceBillingYear && memo.RejectionStage == 2);

      return stage2RM;
    }

    /// <summary>
    /// Following method retrieves Stage1 RejectionMemo details
    /// </summary>
    /// <param name="memoList">RejectionMemo List</param>
    /// <param name="stage2RM">Stage2 RejectionMemo to find in list</param>
    /// <returns>Stage1 RejectionMemo</returns>
    public RejectionMemo GetRejectionStage1MemoDetails(List<RejectionMemo> memoList, RejectionMemo stage2RM)
    {
      var stage1RM =
        memoList.Find(
          memo =>
          memo.RejectionMemoNumber.ToUpper() == (stage2RM.YourRejectionNumber == null ? "" : stage2RM.YourRejectionNumber.ToUpper()) && memo.Invoice.InvoiceNumber.ToUpper() == stage2RM.YourInvoiceNumber.ToUpper() &&
          memo.Invoice.BillingPeriod == stage2RM.YourInvoiceBillingPeriod && memo.Invoice.BillingMonth == stage2RM.YourInvoiceBillingMonth &&
          memo.Invoice.BillingYear == stage2RM.YourInvoiceBillingYear && memo.RejectionStage == 1);

      return stage1RM;
    }

    /// <summary>
    /// Following method is executed from PaxbillingHistoryAuditTrail.vm file to return Month name from month number
    /// </summary>
    /// <param name="monthNumber">Month number whose name is to be retireved</param>
    /// <returns>Month name</returns>
    public string GetAbbreviatedMonthName(int monthNumber)
    {
        try
        {
            return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNumber);
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Gets the reason code list.
    /// </summary>
    /// <returns></returns>
    public IList<ReasonCode> GetReasonCodeListForBillingMemo()
    {
      var reasonCodes = ReasonCodeRepository.Get(reasonCode => reasonCode.IsActive && (reasonCode.TransactionTypeId == (int)TransactionType.BillingMemo || reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill || reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToExpiry));

      return reasonCodes.ToList();
    }

    /// <summary>
    /// Following method is used to break Reason remarks text int string of 80 characters. Used in Pax pdf generation 
    /// </summary>
    /// <param name="reasonRemarks">reason remarks text</param>
    /// <returns>String broken into 80 characters</returns>
    public string CreateReasonRemarksString(string reasonRemarks)
    {
      string finalString = string.Empty;
      if (!string.IsNullOrEmpty(reasonRemarks))
      {
        char[] array = reasonRemarks.ToArray();
        int cnt = 0;
        while (true)
        {
          var str = string.Join("", array.Skip(cnt).Take(80).ToArray());
          if (string.IsNullOrEmpty(str))
          {
            break;
          }
          else
          {
            cnt = cnt + 80;
            if (cnt >= array.Length)
              finalString += str;
            else
              finalString += str + "<br />";
          }
        }
      }
      return finalString;
    }

    /// <summary>
    /// To validate Record50.
    /// </summary>
    /// <param name="record50LiftRequest"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <returns></returns>
    public bool ValidateParsedRecord50(Record50LiftRequest record50LiftRequest, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmisionDate, int autoBillingAirlineId)
    {
      return true;
    }

    /// <summary>
    /// To Validate ISR File.
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="autoBillingAirlineId"></param>
    /// <returns></returns>
    public bool ValidateParsedIsrModel(IsrAutoBillingModel isrAutoBillingModel, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmisionDate, int autoBillingAirlineId)
    {
      var isValid = true;
      var couponTobeRemoved = new ArrayList();

      if (isrAutoBillingModel.CouponCollection.Count > 0)
      { 
        foreach (var isrCoupon in isrAutoBillingModel.CouponCollection)
        {
          isValid = ValidateParsedIsrCoupon(isrAutoBillingModel, isrCoupon, fileName, fileSubmisionDate, autoBillingAirlineId);
          if (!isValid)
          {
            var uniqueCouponKey = autoBillingAirlineId + "," + isrCoupon.TicketDocOrFimNumber + "," + isrCoupon.TicketOrFimCouponNumber + "," + isrCoupon.TicketOrFimIssuingAirline;
            couponTobeRemoved.Add(uniqueCouponKey);
          }
        }
      }

      // here we start creating Invoice object as per avaliablility.
      if (isrAutoBillingModel.CouponCollection.Count > 0)
      {
        PaxInvoice paxInvoice = null;
        PaxInvoice paxAbillInvoice = null;
        string errorDescr = string.Empty;

        foreach (var pcoupon in isrAutoBillingModel.CouponCollection)
        {
          var uniqueCouponKey = autoBillingAirlineId + "," + pcoupon.TicketDocOrFimNumber + "," + pcoupon.TicketOrFimCouponNumber + "," + pcoupon.TicketOrFimIssuingAirline;
          if (!couponTobeRemoved.Contains(uniqueCouponKey))
          {
            var billedMemberId = MemberManager.GetMemberId(pcoupon.TicketOrFimIssuingAirline);
            paxInvoice = GetAutoBillingOpenInvoice(isrAutoBillingModel, pcoupon, fileSubmisionDate, autoBillingAirlineId, billedMemberId);

            Logger.InfoFormat("Pax Invoice found -: [{0}]", paxInvoice);

            if (paxInvoice != null)
            {
              if (!ValidateParsedAutoBillingCouponRecord(paxInvoice, pcoupon, SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod, out errorDescr))
              {
                Logger.InfoFormat("Error in IS- Validation for Coupon -: [{0}]", pcoupon.TicketDocOrFimNumber + "-" + pcoupon.TicketOrFimIssuingAirline + "-" + pcoupon.TicketOrFimCouponNumber);
                var cpnDescriptionver = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuVer);

                if (!string.IsNullOrWhiteSpace(cpnDescriptionver) && !string.IsNullOrWhiteSpace(errorDescr))
                {
                  cpnDescriptionver = cpnDescriptionver + ValidationErrorDelimeter + errorDescr;
                  if (cpnDescriptionver.Length > 255)
                  {
                    cpnDescriptionver = cpnDescriptionver.Substring(0, 254);
                  }
                }

                isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, pcoupon.TicketDocOrFimNumber, pcoupon.TicketOrFimIssuingAirline, pcoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.VER, cpnDescriptionver, false));
                paxInvoice = null;
                couponTobeRemoved.Add(uniqueCouponKey);
              }
              else
              {
                var autoBillUpdateSourceCodeTotalModel = new AutoBillUpdateSourceCodeTotalModel { Id = Guid.NewGuid(), InvoiceId = Guid.Parse(isrAutoBillingModel.CouponParentId), ResponseFileName = fileName };
                isrAutoBillingModel.AutoBillingUpdateSourCodeCollection.Add(autoBillUpdateSourceCodeTotalModel);
                var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRrb);
                isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, pcoupon.TicketDocOrFimNumber, pcoupon.TicketOrFimIssuingAirline, pcoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RRB, cpnDescription, false));
              }
            }
            else
            {
              // paxInvoice = new PaxInvoice { Id = Guid.NewGuid() };
              if (paxAbillInvoice == null)
              {
                paxAbillInvoice = CreateAutoBillingInvoice(isrAutoBillingModel, paxInvoice, pcoupon, fileName, fileSubmisionDate, autoBillingAirlineId, billedMemberId);

                if (paxAbillInvoice != null && !ValidateParsedAutoBillingCouponRecord(paxAbillInvoice, pcoupon, SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod, out errorDescr))
                {
                  Logger.InfoFormat("New invoice : " + paxAbillInvoice.InvoiceNumber);
                  Logger.InfoFormat("Error in IS- Validation for Coupon -: [{0}]", pcoupon.TicketDocOrFimNumber + "-" + pcoupon.TicketOrFimIssuingAirline + "-" + pcoupon.TicketOrFimCouponNumber);
                  var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuVer);

                  if (!string.IsNullOrWhiteSpace(cpnDescription) && !string.IsNullOrWhiteSpace(errorDescr))
                  {
                    cpnDescription = cpnDescription + ValidationErrorDelimeter + errorDescr;
                    if (cpnDescription.Length > 255)
                    {
                      cpnDescription = cpnDescription.Substring(0, 254);
                    }
                  }

                  isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, pcoupon.TicketDocOrFimNumber, pcoupon.TicketOrFimIssuingAirline, pcoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.VER, cpnDescription, false));
                  paxAbillInvoice = null;
                }
              }
              if (paxAbillInvoice != null)
              {
                Logger.InfoFormat("New invoice : " + paxAbillInvoice.InvoiceNumber);
                Logger.InfoFormat("IS- Validation Completed for Coupon -: [{0}]", pcoupon.TicketDocOrFimNumber + "-" + pcoupon.TicketOrFimIssuingAirline + "-" + pcoupon.TicketOrFimCouponNumber);
                pcoupon.BatchSequenceNumber = 1;
                pcoupon.RecordSequenceWithinBatch = 1;
                if (!AutoBillingNewInvoices.ContainsKey(paxAbillInvoice.Id.Value()))
                {
                  AutoBillingNewInvoices.Add(paxAbillInvoice.Id.Value(), paxAbillInvoice);
                }
                paxAbillInvoice.CouponDataRecord.Add(pcoupon);
                isrAutoBillingModel.PaxAutoBillInvoice.Add(paxAbillInvoice);

                // This entry is used to add the Sourcode codeTotal and Invoice total through SP.
                var autoBillUpdateSourceCodeTotalModel = new AutoBillUpdateSourceCodeTotalModel { Id = Guid.NewGuid(), InvoiceId = paxAbillInvoice.Id, ResponseFileName = fileName };
                isrAutoBillingModel.AutoBillingUpdateSourCodeCollection.Add(autoBillUpdateSourceCodeTotalModel);

                var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRrb);
                isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, pcoupon.TicketDocOrFimNumber, pcoupon.TicketOrFimIssuingAirline, pcoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RRB, cpnDescription, false));
              }
              couponTobeRemoved.Add(uniqueCouponKey);
            }
          }
        }
      }

      // Remove the coupon from collection having errors and
      // those which are added in the invoice.
      foreach (var uniqueCpnKey in couponTobeRemoved)
      {
        var keyList = uniqueCpnKey.ToString().Split(',');
        //var billingMemberId = Convert.ToInt64(keyList[0]);
        var ticketDocNumber = Convert.ToInt64(keyList[1]);
        var cpnNumber = Convert.ToInt32(keyList[2]);
        var ticketIssuingAirline = keyList[3];
        isrAutoBillingModel.CouponCollection.RemoveAll(pCpn => pCpn.TicketDocOrFimNumber == ticketDocNumber &&
          pCpn.TicketOrFimCouponNumber == cpnNumber &&
          pCpn.TicketOrFimIssuingAirline.Trim() == ticketIssuingAirline.Trim());
      }

      foreach (var usgData in isrAutoBillingModel.Record50RequestCollection)
      {
        var usageDataRecord = ValueRequestCouponRepository.Get(usageData => usageData.TicketDocumentNumber == usgData.TicketDocumentNumber &&
                                                            usageData.TicketIssuingAirline.Trim() == usgData.TicketIssuingAirline.Trim() &&
                                                            usageData.CouponNumber == usgData.CouponNumber &&
                                                            usageData.CouponStatusId == (int)AutoBillingCouponStatus.PEN &&
                                                            usageData.BillingAirlineId == autoBillingAirlineId).ToList();

        if (usageDataRecord.Count <= 0)
        {
          var isrUnrequestedCoupon = new IsrUnrequestedCoupon
          {
            Id = Guid.NewGuid(),
            TicketIssuingAirline = usgData.TicketIssuingAirline,
            TicketDocumentNumber = usgData.TicketDocumentNumber,
            CouponNumber = usgData.CouponNumber,
            BillingMemberId = autoBillingAirlineId,
            ResponseFileName = fileName,
            ResponceDate = fileSubmisionDate,
            IsRequriedInDailyAutoBilling = false
          };

          isrAutoBillingModel.IsrUnrequestedCouponCollection.Add(isrUnrequestedCoupon);
          isValid = false;
        }
        else
        {
          var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuErr);
          isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, usgData.TicketDocumentNumber, usgData.TicketIssuingAirline, usgData.CouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.ERR, cpnDescription, false));
        }
      }

      if (isrAutoBillingModel.CouponError != null)
      {
        var usageDataRecord = ValueRequestCouponRepository.Get(usageData => usageData.TicketDocumentNumber == isrAutoBillingModel.CouponError.TicketDocumentNumber &&
                                                            usageData.TicketIssuingAirline == isrAutoBillingModel.CouponError.TicketIssuingAirline &&
                                                            usageData.CouponStatusId == (int)AutoBillingCouponStatus.PEN &&
                                                            usageData.BillingAirlineId == autoBillingAirlineId).ToList();

        if (usageDataRecord.Count <= 0)
        {
          var isrUnrequestedCoupon = new IsrUnrequestedCoupon
          {
            Id = Guid.NewGuid(),
            TicketIssuingAirline = isrAutoBillingModel.CouponError.TicketIssuingAirline,
            TicketDocumentNumber = isrAutoBillingModel.CouponError.TicketDocumentNumber,
            CouponNumber =  null,
            BillingMemberId = autoBillingAirlineId,
            ResponseFileName = fileName,
            ResponceDate = fileSubmisionDate,
            IsRequriedInDailyAutoBilling = false
          };

          isrAutoBillingModel.IsrUnrequestedCouponCollection.Add(isrUnrequestedCoupon);
          isValid = false;
        }
        else
        {
          var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuErr);
          isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId,
                                                                                                  isrAutoBillingModel.
                                                                                                    CouponError.
                                                                                                    TicketDocumentNumber,
                                                                                                  isrAutoBillingModel.
                                                                                                    CouponError.
                                                                                                    TicketIssuingAirline,
                                                                                                  0, fileName,
                                                                                                  fileSubmisionDate,
                                                                                                  (int)
                                                                                                  AutoBillingCouponStatus
                                                                                                    .ERR, cpnDescription,
                                                                                                  false));
        }
      }
      return isValid;
    }

    /// <summary>
    /// To Validate the ISR Coupon Record.
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="isrCoupon"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="autoBillingAirlineId"></param>
    /// <returns></returns>
    private bool ValidateParsedIsrCoupon(IsrAutoBillingModel isrAutoBillingModel, PrimeCoupon isrCoupon, string fileName, DateTime fileSubmisionDate, int autoBillingAirlineId)
    {
      var isValid = true;
      var usageDataRecord = ValueRequestCouponRepository.Get(usageData => usageData.TicketDocumentNumber == isrCoupon.TicketDocOrFimNumber &&
                                                             usageData.TicketIssuingAirline == isrCoupon.TicketOrFimIssuingAirline &&
                                                             usageData.CouponNumber == isrCoupon.TicketOrFimCouponNumber &&
                                                             usageData.CouponStatusId == (int)AutoBillingCouponStatus.PEN &&
                                                             usageData.BillingAirlineId == autoBillingAirlineId).ToList();

      if (usageDataRecord.Count <= 0)
      {
        var isrUnrequestedCoupon = new IsrUnrequestedCoupon
        {
          Id = Guid.NewGuid(),
          TicketIssuingAirline = isrCoupon.TicketOrFimIssuingAirline,
          TicketDocumentNumber = isrCoupon.TicketDocOrFimNumber,
          CouponNumber = isrCoupon.TicketOrFimCouponNumber,
          BillingMemberId = autoBillingAirlineId,
          ResponseFileName = fileName,
          ResponceDate = fileSubmisionDate,
          IsRequriedInDailyAutoBilling = false
        };

        isrAutoBillingModel.IsrUnrequestedCouponCollection.Add(isrUnrequestedCoupon);
        isValid = false;
      }
      else
      {
        foreach (var usgData in usageDataRecord)
        {
          if (usgData.RequestTypeId.Equals((int)AutoBillingRequestType.AutoBill))
          {
            var billedMemberId = MemberManager.GetMemberId(isrCoupon.TicketOrFimIssuingAirline);
            var billedMember = MemberManager.GetMemberDetails(billedMemberId);
            if (billedMember != null && !ValidateBilledMemberStatus(billedMember))
            {
              var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuMem);
              isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.MEM, cpnDescription, false));
              isValid = false;
            }
            else
            {
              // P150 implementation startes from here.
              isValid = ValidateIsrCouponForAuBillingInvoice(isrAutoBillingModel, isrCoupon, usgData, fileName, fileSubmisionDate, autoBillingAirlineId);
            }
          }
          else if (usgData.RequestTypeId.Equals((int)AutoBillingRequestType.ValueRequest))
          {
            var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRer);
            isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RER, cpnDescription, false));
            isValid = false;
          }
        }
      }

      return isValid;
    }

    /// <summary>
    /// To generate Auto billing Invoice.
    /// </summary>
    /// <param name="isrCoupon"></param>
    /// <param name="usageData"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="autoBillingAirlineId"></param>
    private bool ValidateIsrCouponForAuBillingInvoice(IsrAutoBillingModel isrAutoBillingModel, PrimeCoupon isrCoupon, Record50LiftRequest usageData, string fileName, DateTime fileSubmisionDate, int autoBillingAirlineId)
    {
      var isValid = true; 
      var exchangeRate = 1.0;

      // Get crrent Auto Billing Period
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      BillingPeriod currentBillingPeriod = calendarManager.GetCurrentAutoBillingPeriod(autoBillingAirlineId);

      isrCoupon.BatchSequenceNumber = 1;
      isrCoupon.RecordSequenceWithinBatch = 1;
      isrCoupon.AutoBillingCouponStatus = (int)AutoBillingStatusType.New;
      isrCoupon.IncludeInDailyRevenueRecogn = false;
      isrCoupon.FromAirportOfCoupon = usageData.FromAirportOfCoupon;
      isrCoupon.ToAirportOfCoupon = usageData.ToAirportOfCoupon;
      isrCoupon.AirlineFlightDesignator = usageData.AirlineFlightDesignator;
      isrCoupon.FlightNumber = usageData.FlightNumber;
      isrCoupon.FlightDate = usageData.FlightDate;
      isrCoupon.SettlementAuthorizationCode = usageData.SettlementAuthorizationCode;
      isrCoupon.CabinClass = usageData.CabinClass;
      isrCoupon.ElectronicTicketIndicator = usageData.ElectronicTicketIndicatorId.Equals(1);
      isrCoupon.SourceCodeId = usageData.TransactionTypeId;
      // If Handling Fee Agreement type is neither F nor P then set amount to 0
      if (isrCoupon.HandlingFeeAgreementType != null)
      {
        isrCoupon.HandlingFeeAmount = 0;
      }
      // ISR to listing currency conversion
      if (!string.IsNullOrWhiteSpace(isrCoupon.SettlementCouponCurrencyType))
      {
        //GET listing currency from member profile
        var listingCurrency =
          MemberManager.GetPassengerConfiguration(autoBillingAirlineId) != null
            ? MemberManager.GetPassengerConfiguration(autoBillingAirlineId).ListingCurrencyId
            : 0;

        int listingCurrencyId = 0;
        if (listingCurrency != null)
        {
          listingCurrencyId = (int) listingCurrency;
        }

        var isrCurrencyId =
          CurrencyRepository.Get(currency => currency.Code == isrCoupon.SettlementCouponCurrencyType) != null
            ? CurrencyRepository.Get(currency => currency.Code == isrCoupon.SettlementCouponCurrencyType).OrderBy(
              currency => currency.Id).FirstOrDefault().Id
            : 0;

        // If ISR and Listing currencies are not same
        if (listingCurrencyId != isrCurrencyId)
        {

          if (listingCurrencyId == (int) BillingCurrency.EUR || listingCurrencyId == (int) BillingCurrency.GBP ||
              listingCurrencyId == (int)BillingCurrency.USD) // If listing currency is prime
          {
            exchangeRate = GetExchangeRateForAutoBillingCoupon(currentBillingPeriod, isrCurrencyId, listingCurrencyId);

            foreach (var tax in isrCoupon.TaxBreakdown)
            {
              isrCoupon.TaxAmount += (tax.Amount/exchangeRate);
              tax.Amount = tax.Amount/exchangeRate;
            }

            isrCoupon.CouponGrossValueOrApplicableLocalFare = isrCoupon.CouponGrossValueOrApplicableLocalFare / exchangeRate;

            // If Handling Fee is Amount based
            if (isrCoupon.HandlingFeeAgreementType != null && isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypeAmountBased))
            {
              isrCoupon.HandlingFeeAmount = ConvertUtil.Round(isrCoupon.HandlingFeeAmount/exchangeRate,
                                                              Constants.PaxDecimalPlaces);
            }

          }
          else if (isrCurrencyId == (int) BillingCurrency.EUR || isrCurrencyId == (int) BillingCurrency.GBP ||
                   isrCurrencyId == (int)BillingCurrency.USD) // If listing currency is not prime but isr currency is prime
          {
            exchangeRate = GetExchangeRateForAutoBillingCoupon(currentBillingPeriod, listingCurrencyId, isrCurrencyId);

            foreach (var tax in isrCoupon.TaxBreakdown)
            {
              isrCoupon.TaxAmount += (tax.Amount*exchangeRate);
              tax.Amount = tax.Amount * exchangeRate;
            }

            isrCoupon.CouponGrossValueOrApplicableLocalFare = isrCoupon.CouponGrossValueOrApplicableLocalFare * exchangeRate;

            // If Handling Fee is Amount based
            if (isrCoupon.HandlingFeeAgreementType != null &&
                isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypeAmountBased))
            {
              isrCoupon.HandlingFeeAmount = ConvertUtil.Round(isrCoupon.HandlingFeeAmount*exchangeRate,
                                                              Constants.PaxDecimalPlaces);
            }
          }
          else // If both listing and isr currencies are not prime
          {
            var exchangeRateAgainstListing = GetExchangeRateForAutoBillingCoupon(currentBillingPeriod,
                                                                                 listingCurrencyId);

            var exchangeRateAgainstIsr = GetExchangeRateForAutoBillingCoupon(currentBillingPeriod, isrCurrencyId);

            exchangeRate = exchangeRateAgainstListing/exchangeRateAgainstIsr;

            foreach (var tax in isrCoupon.TaxBreakdown)
            {
              isrCoupon.TaxAmount += (tax.Amount*exchangeRate);
              tax.Amount = tax.Amount * exchangeRate;
            }

            isrCoupon.CouponGrossValueOrApplicableLocalFare = isrCoupon.CouponGrossValueOrApplicableLocalFare * exchangeRate;

            // If Handling Fee is Amount based
            if (isrCoupon.HandlingFeeAgreementType != null &&
                isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypeAmountBased))
            {
              isrCoupon.HandlingFeeAmount = ConvertUtil.Round(isrCoupon.HandlingFeeAmount*exchangeRate,
                                                              Constants.PaxDecimalPlaces);
            }
          }

          if (isrCoupon.HandlingFeeAgreementType != null &&
              isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypePercentBased))
          {
            isrCoupon.HandlingFeeAmount =
              ConvertUtil.Round(
                isrCoupon.HandlingFeeChargePercentage*isrCoupon.CouponGrossValueOrApplicableLocalFare/100,
                Constants.PaxDecimalPlaces);
          }

          if (isrCoupon.HandlingFeeAmount > 0)
          {
            isrCoupon.HandlingFeeTypeId = "C";
          }
          isrCoupon.IscAmount =
            ConvertUtil.Round((isrCoupon.IscPercent*isrCoupon.CouponGrossValueOrApplicableLocalFare/100),
                              Constants.PaxDecimalPlaces);
          isrCoupon.UatpAmount =
            ConvertUtil.Round((isrCoupon.UatpPercent*isrCoupon.CouponGrossValueOrApplicableLocalFare/100),
                              Constants.PaxDecimalPlaces);
        }
        else // If listing currency == isr currency
        {
          foreach (var tax in isrCoupon.TaxBreakdown)
          {
            isrCoupon.TaxAmount += (tax.Amount);
          }

         
            // If Handling Fee is Amount based
          if (isrCoupon.HandlingFeeAgreementType != null && isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypeAmountBased))
            {
              isrCoupon.HandlingFeeAmount = ConvertUtil.Round(isrCoupon.HandlingFeeAmount, Constants.PaxDecimalPlaces);
            }

          if (isrCoupon.HandlingFeeAgreementType != null && isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypePercentBased))
            {
              isrCoupon.HandlingFeeAmount =
                ConvertUtil.Round(
                  isrCoupon.HandlingFeeChargePercentage*isrCoupon.CouponGrossValueOrApplicableLocalFare/100,
                  Constants.PaxDecimalPlaces);
            }
          


          if (isrCoupon.HandlingFeeAmount > 0)
          {
            isrCoupon.HandlingFeeTypeId = "C";
          }
          isrCoupon.IscAmount =
            ConvertUtil.Round((isrCoupon.IscPercent*isrCoupon.CouponGrossValueOrApplicableLocalFare/100),
                              Constants.PaxDecimalPlaces);
          isrCoupon.UatpAmount =
            ConvertUtil.Round((isrCoupon.UatpPercent*isrCoupon.CouponGrossValueOrApplicableLocalFare/100),
                              Constants.PaxDecimalPlaces);
          isrCoupon.CouponGrossValueOrApplicableLocalFare = isrCoupon.CouponGrossValueOrApplicableLocalFare;
        }
      }
      else
      {
        foreach (var tax in isrCoupon.TaxBreakdown)
        {
          isrCoupon.TaxAmount += (tax.Amount);
        }

        // If Handling Fee is Amount based
        if (isrCoupon.HandlingFeeAgreementType != null &&
        isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypeAmountBased))
        {
          isrCoupon.HandlingFeeAmount = ConvertUtil.Round(isrCoupon.HandlingFeeAmount, Constants.PaxDecimalPlaces);
        }

        if (isrCoupon.HandlingFeeAgreementType != null &&
            isrCoupon.HandlingFeeAgreementType.Equals(HandlingFeeAggTypePercentBased))
        {
          isrCoupon.HandlingFeeAmount =
            ConvertUtil.Round(
              isrCoupon.HandlingFeeChargePercentage * isrCoupon.CouponGrossValueOrApplicableLocalFare / 100,
              Constants.PaxDecimalPlaces);
        }

        if (isrCoupon.HandlingFeeAmount > 0)
        {
          isrCoupon.HandlingFeeTypeId = "C";
        }
        isrCoupon.IscAmount =
          ConvertUtil.Round((isrCoupon.IscPercent * isrCoupon.CouponGrossValueOrApplicableLocalFare / 100),
                            Constants.PaxDecimalPlaces);
        isrCoupon.UatpAmount =
          ConvertUtil.Round((isrCoupon.UatpPercent * isrCoupon.CouponGrossValueOrApplicableLocalFare / 100),
                            Constants.PaxDecimalPlaces);
        isrCoupon.CouponGrossValueOrApplicableLocalFare = isrCoupon.CouponGrossValueOrApplicableLocalFare;
      }

      isrCoupon.CouponTotalAmount = ConvertUtil.Round(isrCoupon.CouponGrossValueOrApplicableLocalFare + isrCoupon.TaxAmount, Constants.PaxDecimalPlaces) + isrCoupon.IscAmount + isrCoupon.HandlingFeeAmount + isrCoupon.UatpAmount;

      isrCoupon.AttachmentIndicatorOriginal = 0;

      var fdrMonth = Convert.ToInt32(isrCoupon.MonthOfIataFiveDayRate.Substring(0, 2));
      var fdrYear = Convert.ToInt32(DateTime.Now.Year.ToString().Substring(0, 2) + isrCoupon.MonthOfIataFiveDayRate.Substring(2, 2));

      // MRAT Validation : Validation for Month of IATA 5-day Rate.
      if ((fdrYear != currentBillingPeriod.Year) || (fdrYear.Equals(currentBillingPeriod.Year) && !fdrMonth.Equals(currentBillingPeriod.Month)))
      {
        var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRna);
        isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RNA, cpnDescription, false));
        isValid = false;
      }

      // Validation for Duplicate ISR coupon for lase 12 Calender Months.
      var isCoupunDuplicate = ValueRequestCouponRepository.IsIsrCouponDuplicate(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber.ToString(), isrCoupon.TicketOrFimCouponNumber, isrCoupon.TicketOrFimIssuingAirline, currentBillingPeriod.Month, currentBillingPeriod.Year, (int)SubmissionMethod.AutoBilling);
      if (isCoupunDuplicate)
      {
        var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRnb);
        isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RNB, cpnDescription, false));
        isValid = false;
      }

      // Validation for Flight date.
      if (isrCoupon.FlightDate.HasValue && ((isrCoupon.FlightDate.Value.Year > currentBillingPeriod.Year) || (isrCoupon.FlightDate.Value.Year.Equals(currentBillingPeriod.Year) && isrCoupon.FlightDate.Value.Month > currentBillingPeriod.Month)))
      {
        var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRnc);
        isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RNC, cpnDescription, false));
        isValid = false;
      }

      // Validation for SettlementCouponProrationType.
      if (isrCoupon.SettlementCouponProrationType.Equals(AutoBillProrationType))
      {
        var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuVer);

        if (!string.IsNullOrWhiteSpace(cpnDescription))
        {
          cpnDescription = cpnDescription + ValidationErrorDelimeter + "Unable to process Frequent Flyer Redemption billings";
          if (cpnDescription.Length > 255)
          {
            cpnDescription = cpnDescription.Substring(0, 254);
          }
        }

        isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(autoBillingAirlineId, isrCoupon.TicketDocOrFimNumber, isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber, fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.VER, cpnDescription, false));
        isValid = false;
      }

      return isValid;
    }

    /// <summary>
    /// IS Validation for Isr Coupon.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="couponRecord"></param>
    /// <param name="errorDescr"></param>
    /// <returns></returns>
    private bool ValidateParsedAutoBillingCouponRecord(PaxInvoice invoice, PrimeCoupon couponRecord, bool ignoreValidationOnMigrationPeriod, out string errorDescr)
    {
      var isValid = true;
      errorDescr = string.Empty;
      const TransactionType transType = TransactionType.Coupon;

      // Get MaxAccepted amount.
      var clearingHouse = ReferenceManager.GetClearingHouseFromSMI(invoice.SettlementMethodId);
      MaxAcceptableAmount maxCouponAcceptableAmount = null;
      if (!string.IsNullOrEmpty(clearingHouse.Trim()))
      {
        // If invoice contains Min Max Acceptable amounts then don't make DB call.
        maxCouponAcceptableAmount = GetMaxAcceptableAmount(invoice, clearingHouse, TransactionType.Coupon);
      }

      // Get the Exchange rate.
      ExchangeRate exchangeRate = null;
      if (invoice.BillingYear != 0 && invoice.BillingMonth != 0)
      {
        // This exchange rate will be used for validating net amounts in coupon.
        var invoiceBillingDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);

        exchangeRate = ExchangeRateRepository.First(
          rate => rate.CurrencyId == invoice.ListingCurrencyId &&
          rate.EffectiveFromDate <= invoiceBillingDate && rate.EffectiveToDate >= invoiceBillingDate);
      }

      // CheckDigit Validation.
      if ((couponRecord.CheckDigit < 0) || (couponRecord.CheckDigit > 6 && couponRecord.CheckDigit != 9))
      {
        errorDescr = "Check Digit is invalid";
        isValid = false;
      }

      // CouponNumber Validation.
      if ((couponRecord.TicketOrFimCouponNumber <= 0 || couponRecord.TicketOrFimCouponNumber > 4))
      {
        errorDescr = "Invalid Coupon Number";
        isValid = false;
      }

      // FlightNumber Validation.
      if (couponRecord.FlightNumber == null || couponRecord.FlightNumber == 0)
      {
        errorDescr = "Invalid Flight Number";
        isValid = false;
      }

      // AirlineFlightDesignator Validation.
      if (couponRecord.AirlineFlightDesignator == null || string.IsNullOrEmpty(couponRecord.AirlineFlightDesignator.Trim()))
      {
        errorDescr = "Invalid Airline Code";
        isValid = false;
      }

      // Validate FromAirportOfCoupon  and ToAirportOfCoupon.
      if (string.IsNullOrWhiteSpace(couponRecord.FromAirportOfCoupon) || string.IsNullOrWhiteSpace(couponRecord.ToAirportOfCoupon))
      {
        errorDescr = "Invalid FromAirport";
        isValid = false;
      }

      if (couponRecord.FromAirportOfCoupon != null && couponRecord.ToAirportOfCoupon != null)
      {
        if (!string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon.Trim()) && !string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon.Trim()))
        {
          if (String.Equals(couponRecord.FromAirportOfCoupon.ToUpper(), couponRecord.ToAirportOfCoupon.ToUpper()))
          {
            errorDescr = "Invalid ToAirport";
            isValid = false;
          }
        }
      }
      if (!IsValidCityAirportCode(couponRecord.FromAirportOfCoupon))
      {
        errorDescr = "Invalid FromAirport CityCode";
        isValid = false;
      }
      if (!IsValidCityAirportCode(couponRecord.ToAirportOfCoupon))
      {
        errorDescr = "Invalid ToAirport CityCode";
        isValid = false;
      }

      // Flight Date Validation.
      if (couponRecord.FlightDate.HasValue == false)
      {
        errorDescr = "Invalid Flight Date";
        isValid = false;
      }

      if (couponRecord.FlightDate.HasValue)
      {
        // Try to parse date string to DateTime to check whether its valid.
        DateTime resultDate;
        if (!DateTime.TryParse(couponRecord.FlightDate.Value.ToString(), out resultDate))
        {
          errorDescr = "Invalid Flight Date";
          isValid = false;
        }
      }

      // Validation for AgreementIndicatorSupplied.
      if (((!string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied)) && string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) ||
         (ignoreValidationOnMigrationPeriod ? 
         ((!string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) && string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied) && (couponRecord.OriginalPmi.Trim() != "N")) : 
         ((!string.IsNullOrWhiteSpace(couponRecord.OriginalPmi)) && string.IsNullOrWhiteSpace(couponRecord.AgreementIndicatorSupplied))) ||
         (couponRecord.OriginalPmi != null && couponRecord.AgreementIndicatorSupplied != null && !ValidateOriginalPmi(couponRecord.OriginalPmi, couponRecord.AgreementIndicatorSupplied)))
      {
        errorDescr = "Invalid Original PMI";
        isValid = false;
      }

      if (couponRecord.AgreementIndicatorSupplied != null && !ValidateAgreementIndicatorSupplied(couponRecord.AgreementIndicatorSupplied))
      {
        errorDescr = "Invalid Agreement Indicator Supplied";
        isValid = false;
      }

      // Agreement Indicator Supplied = S and Billing Code = 0 is invalid
      if (couponRecord.AgreementIndicatorSupplied != null && String.Equals(couponRecord.AgreementIndicatorSupplied.ToUpper(), "S"))
      {
        errorDescr = "Invalid Agreement Indicator Supplied";
        isValid = false;
      }

      // CurrencyAdjustmentIndicator Validation.
      if (string.IsNullOrWhiteSpace(couponRecord.CurrencyAdjustmentIndicator) || !ReferenceManager.IsValidCurrencyCode(invoice, couponRecord.CurrencyAdjustmentIndicator))
      {
        errorDescr = "Invalid Currency Adjustment Indicator";
        isValid = false;
      }

      if (!MemberManager.IsValidAirlineCode(couponRecord.TicketOrFimIssuingAirline))
      {
        errorDescr = "Invalid TicketOrFimIssuingAirline Code";
        isValid = false;
      }

      // Validate SourceCode 
      var validSourceCodes = ValidationCache.Instance.ValidSourceCodes;
      if (!ReferenceManager.IsValidABillSourceCode(validSourceCodes, couponRecord.SourceCodeId, (int)transType))
      {
        errorDescr = "Invalid Source Code Id";
        isValid = false;
      }

      // Validate CouponTotalAmount 
      if (couponRecord.CouponTotalAmount < 0)
      {
        errorDescr = "Invalid Total Amount";
        isValid = false;
      }
      else if (exchangeRate != null && maxCouponAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(couponRecord.CouponTotalAmount, TransactionType.Coupon, invoice.ListingCurrencyId, invoice, exchangeRate, iMaxAcceptableAmount: maxCouponAcceptableAmount))
      {
        errorDescr = "Coupon Total amount is not in the range of allowed min max amount";
        isValid = false;
      }

      // Validate couponRecord Tax.
      var validTaxCode = ValidationCache.Instance.ValidTaxCodes;
      foreach (var couponRecordTax in couponRecord.TaxBreakdown)
      {
        if (couponRecordTax.TaxCode != null && !string.IsNullOrEmpty(couponRecordTax.TaxCode.Trim()))
        {
          if (validTaxCode != null)
          {
            if (!(validTaxCode.ContainsKey(couponRecordTax.TaxCode) && validTaxCode[couponRecordTax.TaxCode]))
            {
              errorDescr = "Coupon Tax breakdown records are missing though Coupon Tax amount > 0";
              isValid = false;
            }
          }
        }
      }
      ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
      //if (couponRecord.SourceCodeId != 27)
      //{
      //  couponRecord.ExpiryDatePeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);
      //}
      return isValid;
    }

    /// <summary>
    /// To get the AutoBillUpdateCouponStatusModel object.
    /// </summary>
    /// <param name="autoBillingAirlineId"></param>
    /// <param name="ticketDocumentNumber"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="ticketOrFimCouponNumber"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="cpnStatusId"></param>
    /// <param name="cpnDescription"></param>
    /// <param name="isIncludedInIrregularityReport"></param>
    /// <returns></returns>
    private AutoBillUpdateCouponStatusModel GetAbUpdateCouponStatusObject(int autoBillingAirlineId, long ticketDocumentNumber, string ticketIssuingAirline, int ticketOrFimCouponNumber, string fileName, DateTime fileSubmisionDate, int cpnStatusId, string cpnDescription, bool isIncludedInIrregularityReport)
    {
      var isrUpdatedCoupon = new AutoBillUpdateCouponStatusModel
      {
        Id = Guid.NewGuid(),
        BillingAirlineId = autoBillingAirlineId,
        TicketDocumentNumber = ticketDocumentNumber,
        TicketIssuingAirline = ticketIssuingAirline,
        CouponNumber = ticketOrFimCouponNumber,
        ResponseFileName = fileName,
        ResponseDate = fileSubmisionDate,
        CouponStatusId = cpnStatusId,
        CouponStatusDescription = cpnDescription,
        IncludedInIrregularityReport = false
      };

      return isrUpdatedCoupon;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="settlementMethodId"></param>
    /// <returns></returns>
    private BillingPeriod GetCurrentBillingPeriod(DateTime fileSubmissionDate, int settlementMethodId)
    {
      BillingPeriod billingPeriod;

      // Get current billing period once per invoice and use it for validating all the records in invoice.
      var clearingHouseEnum = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(settlementMethodId);

      try
      {
        billingPeriod = CalendarManager.GetBillingPeriod(fileSubmissionDate, clearingHouseEnum);
      }
      catch (ISCalendarDataNotFoundException)
      {
        billingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
      }
      return billingPeriod;
    }

    /// <summary>
    /// This will be used to get auto billing open invoices
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="couponRecord"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    public PaxInvoice GetAutoBillingOpenInvoice(IsrAutoBillingModel isrAutoBillingModel, PrimeCoupon couponRecord, DateTime fileSubmissionDate, int billingMemberId, int billedMemberId)
    {
      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      BillingPeriod billingPeriod = calendarManager.GetCurrentAutoBillingPeriod(billingMemberId);

      Logger.Debug("Get Open Invoice in : " + billingPeriod.Period + "/" + billingPeriod.Month + "/" + billingPeriod.Year);

      var curInvoice = (PaxInvoice)PaxInvoiceRepository.Single(invoice => invoice.BillingCategoryId == (int)BillingCategoryType.Pax
        && invoice.SubmissionMethodId == (int)SubmissionMethod.AutoBilling
        && invoice.InvoiceStatusId == (int)InvoiceStatusType.Open
        && invoice.BillingMemberId == billingMemberId
        && invoice.BilledMemberId == billedMemberId
        && invoice.BillingYear == billingPeriod.Year
        && invoice.BillingMonth == billingPeriod.Month
        && invoice.BillingPeriod == billingPeriod.Period);


      if (curInvoice == null)
      {

        var invbase = AutoBillingNewInvoices.Where(a => a.Value.BillingYear == billingPeriod.Year && a.Value.BillingMonth == billingPeriod.Month &&
                                        a.Value.BillingPeriod == billingPeriod.Period && a.Value.BillingMemberId == billingMemberId &&
                                        a.Value.BilledMemberId == billedMemberId).FirstOrDefault();

        curInvoice = invbase.Value;

        if (curInvoice != null)
        {

          isrAutoBillingModel.CouponParentId = curInvoice.Id.Value();

          var batchSeqNo = curInvoice.CouponDataRecord.OrderByDescending(storedCpn => storedCpn.BatchSequenceNumber).FirstOrDefault().BatchSequenceNumber;
          if (batchSeqNo < 99999)
          {
            var recSeqNo = curInvoice.CouponDataRecord.Where(storedCpn => storedCpn.BatchSequenceNumber == batchSeqNo).OrderByDescending(storedCpn => storedCpn.RecordSequenceWithinBatch).FirstOrDefault().RecordSequenceWithinBatch;

            couponRecord.BatchSequenceNumber = batchSeqNo;
            couponRecord.RecordSequenceWithinBatch = recSeqNo + 1;
            curInvoice.CouponDataRecord[0].RecordSequenceWithinBatch = couponRecord.RecordSequenceWithinBatch;
          }
          else
          {
            couponRecord.BatchSequenceNumber = 1;
            couponRecord.RecordSequenceWithinBatch = 1;
          }
        }
      }
      else
      {
        var batchSeqNo = 99999;
        isrAutoBillingModel.CouponParentId = ConvertUtil.ConvertGuidToString(curInvoice.Id);
        if (PrimeCouponRecordRepository.Get(pCoupon => pCoupon.InvoiceId == curInvoice.Id).Count() > 0)
        {
          batchSeqNo =
            PrimeCouponRecordRepository.Get(pCoupon => pCoupon.InvoiceId == curInvoice.Id).OrderByDescending(
              pCoupon => pCoupon.BatchSequenceNumber).FirstOrDefault().BatchSequenceNumber;
        }
        if (batchSeqNo < 99999)
        {
          var recordSeqNo =
            PrimeCouponRecordRepository.Get(pCoupon => pCoupon.InvoiceId == curInvoice.Id && pCoupon.BatchSequenceNumber == batchSeqNo).OrderByDescending(pCoupon => pCoupon.RecordSequenceWithinBatch).
              FirstOrDefault().RecordSequenceWithinBatch;

          couponRecord.BatchSequenceNumber = batchSeqNo;
          couponRecord.RecordSequenceWithinBatch = recordSeqNo + 1;
        }
        else
        {
          couponRecord.BatchSequenceNumber = 1;
          couponRecord.RecordSequenceWithinBatch = 1;
        }
      }

      return curInvoice;
    }



    /// <summary>
    /// Track ISR responses from AIA vis-a-vis usage file sent to AIA
    /// </summary>
    public void TrackIsrResponse()
    {
      IMemberRepository memberManager = new MemberRepository();
      IValueCouponRequestRepository valueCouponRequestRepository = new ValueCouponRequestRepository();
      // Get all participants in the Value Determination process

      var memberList = memberManager.Get(i => i.IsParticipateInValueDetermination);

      foreach (Member member in memberList)
      {
        // For each participant, track ISR responses
        var record50LiftRequestList = valueCouponRequestRepository.Get(i => i.BillingAirlineId == member.Id && i.CouponStatusId == (int)AutoBillingCouponStatus.PEN && i.ExpectedResponseDate < DateTime.Now && i.IncludedInIrregularityReport == false && i.ResponseNotReceivedAsPerSLA == null);
        // var record50LiftRequestList = valueCouponRequestRepository.GetAll();

        foreach (Record50LiftRequest record50LiftRequest in record50LiftRequestList)
        {
          record50LiftRequest.ResponseNotReceivedAsPerSLA = true;
          valueCouponRequestRepository.Update(record50LiftRequest);
        }
      }

      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// To be used in Pax Auto Billing performance report
    /// </summary>
    /// <param name="members"></param>
    /// <param name="currency"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <returns></returns>
    public List<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceData(List<Member> members, Currency currency, int billingMonth, int billingYear)
    {
      var result = new List<AutoBillingPerformanceReportSearchResult>();


      //foreach (var member in members)
      //{
      //  var resultrecord = new AutoBillingPerformanceReportSearchResult();
      //  var noOfCpnsAutoBilled = new List<long>();
      //  var amountOfCpnsAutoBilled = new List<decimal>();

      //  resultrecord.Member = member;
      //  resultrecord.Currency = currency;

      //  for (var monthYearCount = 1; monthYearCount <= 6; monthYearCount++)
      //  {
      //    var invoices = InvoiceRepository.GetInvoicesWithCoupons(billingMonth: billingMonth,
      //                                                            billingYear: billingYear,
      //                                                            billingPeriod: null,
      //                                                            billingMemberId: member.Id,
      //                                                            billedMemberId: null, billingCode: null);

      //    foreach (var invoice in invoices)
      //    {
      //      noOfCpnsAutoBilled.Add(invoice.CouponDataRecord != null ? invoice.CouponDataRecord.Count : 0);
      //      amountOfCpnsAutoBilled.Add(invoice.BillingAmount);
      //    }


      //    if (billingMonth == 1)
      //    {
      //      billingMonth = 12;
      //      billingYear = billingYear - 1;
      //    }
      //    else
      //    {
      //      billingMonth = billingMonth - 1;
      //    }
      //  }

      //  resultrecord.NoOfCpnsAutoBilled = noOfCpnsAutoBilled;
      //  resultrecord.AmountOfCpnsAutoBilled = amountOfCpnsAutoBilled;
      //  result.Add(resultrecord);
      //}

      return result;
    }

    protected decimal GetExchangeRateForAutoBillingInvoice(InvoiceBase invoice, Member billingMember, Member billedMember, bool isIchConfigRetrieved = false)
    {
      ExchangeRate exchangeRate = null;

      if (invoice.BillingYear != 0 && invoice.BillingMonth != 0)
      {
        // As if Both Listing and Billing currency are same the exchage rate will be 1.
        if (invoice.ListingCurrencyId == invoice.BillingCurrencyId)
        {
          return 1;
        }
        // This exchange rate will be used for validating net amounts in coupon.
        var invoiceBillingDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);

        exchangeRate = ExchangeRateRepository.First(
          rate => rate.CurrencyId == invoice.ListingCurrencyId &&
                  rate.EffectiveFromDate <= invoiceBillingDate && rate.EffectiveToDate >= invoiceBillingDate);


        // Validate billing currency rate  
        if (invoice.ListingCurrencyId != invoice.BillingCurrencyId && exchangeRate != null)
        {
          if (invoice.SettlementMethodId == (int) SMI.Ich || invoice.SettlementMethodId == (int) SMI.Ach ||
              invoice.SettlementMethodId == (int) SMI.AchUsingIataRules)
          {
            switch (invoice.BillingCurrencyId)
            {
              case (int) BillingCurrency.EUR:
                return ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateEur),
                                         Constants.ExchangeRateDecimalPlaces);

              case (int) BillingCurrency.GBP:
                return ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateGbp),
                                         Constants.ExchangeRateDecimalPlaces);

              case (int) BillingCurrency.USD:
                return ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateUsd),
                                         Constants.ExchangeRateDecimalPlaces);
            }
          }
          else if (invoice.SettlementMethodId == (int) SMI.Bilateral)
          {
            return ConvertUtil.Round(Convert.ToDecimal(exchangeRate.FiveDayRateUsd),
                                     Constants.ExchangeRateDecimalPlaces);
          }
        }
      }
      return 0;
    }

    protected double GetExchangeRateForAutoBillingCoupon(BillingPeriod openBillingPeriod, int secondaryCurrencyId, int primaryCurrencyId = (int)BillingCurrency.USD)
    {
      var exchangeRate = 1.0;
      try
      {
        var effectiveFromDate = new DateTime(openBillingPeriod.Year, openBillingPeriod.Month, 1);
        var effectiveToDate = effectiveFromDate.AddMonths(1).AddDays(-1);

        var exchangeRateList = ExchangeRateRepository.First(
          rate => rate.CurrencyId == secondaryCurrencyId &&
                  rate.EffectiveFromDate <= effectiveFromDate && rate.EffectiveToDate >= effectiveToDate);
        

        // Validate billing currency rate  
        if (exchangeRateList != null)
        {
          switch (primaryCurrencyId)
          {
            case (int) BillingCurrency.EUR:
              exchangeRate = Convert.ToDouble(ConvertUtil.Round(Convert.ToDecimal(exchangeRateList.FiveDayRateEur),
                                                                Constants.ExchangeRateDecimalPlaces));
              break;

            case (int) BillingCurrency.GBP:
              exchangeRate = Convert.ToDouble(ConvertUtil.Round(Convert.ToDecimal(exchangeRateList.FiveDayRateGbp),
                                                                Constants.ExchangeRateDecimalPlaces));
              break;
            case (int) BillingCurrency.USD:
              exchangeRate = Convert.ToDouble(ConvertUtil.Round(Convert.ToDecimal(exchangeRateList.FiveDayRateUsd),
                                                                Constants.ExchangeRateDecimalPlaces));
              break;
          }
        }
        return exchangeRate;
      }
      catch (Exception ex)
      {
        return exchangeRate;
      }
      return exchangeRate;
    }

    /// <summary>
    /// Update auto billing invoice
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="isrCoupon"></param>
    /// <returns></returns>
    public PaxInvoice UpdateSourceCodeTotalRecord(PaxInvoice invoice, PrimeCoupon isrCoupon)
    {
      SourceCodeTotal sourceCodeTotal = invoice.SourceCodeTotal.Single(sourceCode => sourceCode.SourceCodeId == isrCoupon.SourceCodeId);
      if (sourceCodeTotal != null)
      {
        sourceCodeTotal.TotalGrossValue += Convert.ToDecimal(isrCoupon.CouponGrossValueOrApplicableLocalFare);
        sourceCodeTotal.TotalTaxAmount += Convert.ToDecimal(isrCoupon.TaxAmount);
      }
      else
      {
        sourceCodeTotal = new SourceCodeTotal();
        sourceCodeTotal.SourceCodeId = isrCoupon.SourceCodeId;
        sourceCodeTotal.TotalGrossValue = Convert.ToDecimal(isrCoupon.CouponGrossValueOrApplicableLocalFare);
        sourceCodeTotal.TotalTaxAmount = Convert.ToDecimal(isrCoupon.TaxAmount);
        invoice.SourceCodeTotal.Add(sourceCodeTotal);
      }
      return invoice;
    }

    /// <summary>
    /// Update auto billing invoice
    /// </summary>
    /// <param name="isrAutoBillingModel"></param>
    /// <param name="invoice"></param>
    /// <param name="isrCoupon"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmisionDate"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <returns></returns>
    public PaxInvoice CreateAutoBillingInvoice(IsrAutoBillingModel isrAutoBillingModel, PaxInvoice invoice, PrimeCoupon isrCoupon, string fileName, DateTime fileSubmisionDate, int billingMemberId, int billedMemberId)
    {
      invoice = new PaxInvoice { Id = Guid.NewGuid() };

      var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
      var invoiceManager = Ioc.Resolve<IInvoiceManager>(typeof(IInvoiceManager));
      BillingPeriod billingPeriod = calendarManager.GetCurrentAutoBillingPeriod(billingMemberId);

      Logger.Debug("Create Invoice Invoice in : " + billingPeriod.Period + "/" + billingPeriod.Month + "/" + billingPeriod.Year);
      if (billingPeriod != null)
      {
        invoice.BillingYear = billingPeriod.Year;
        invoice.BillingMonth = billingPeriod.Month;
        invoice.BillingPeriod = billingPeriod.Period;
      }
      
      //Populate billing member
      invoice.BillingMemberId = billingMemberId;
      invoice.BillingMember = MemberManager.GetMemberDetails(invoice.BillingMemberId);

      //Populate billed member
      invoice.BilledMemberId = billedMemberId;
      invoice.BilledMember = MemberManager.GetMemberDetails(invoice.BilledMemberId);

      // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
      var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
      var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

      // Assign final parent to invoice
      if (billingFinalParent != null && billingFinalParent.Id != invoice.BillingMemberId)
      {
        invoice.BillingParentMemberId = billingFinalParent.Id;
      }
      if (billedFinalParent != null && billedFinalParent.Id != invoice.BilledMemberId)
      {
        invoice.BilledParentMemberId = billedFinalParent.Id;
      }

      // Get passenmgerconfiguration of the billing memberid to create the bew invoice number.
      var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoice.BillingMemberId);
      if (passengerConfiguration != null)
      {
        if (passengerConfiguration.InvoiceNumberRangePrefix != null && passengerConfiguration.InvoiceNumberRangeFrom.HasValue && passengerConfiguration.InvoiceNumberRangeTo.HasValue)
        {
            var numericInvoiceNumber = invoiceManager.GetAutoBillingInvoiceNumberSeq(invoice.BillingMemberId, billingPeriod.Year);

          if (numericInvoiceNumber > passengerConfiguration.InvoiceNumberRangeTo.Value)
          {
              var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRnd);

              isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(
                GetAbUpdateCouponStatusObject(billingMemberId, isrCoupon.TicketDocOrFimNumber,
                isrCoupon.TicketOrFimIssuingAirline, isrCoupon.TicketOrFimCouponNumber,
                fileName, fileSubmisionDate, (int)AutoBillingCouponStatus.RND, cpnDescription, false));

              if (!string.IsNullOrEmpty(ValidationCache.Instance.SISOpsEmailId))
              {
                  string sisOpsMail = ValidationCache.Instance.SISOpsEmailId;

                  AutoBillingNotification.SendUnavaliableOrThresholdReachedInvoiceNotification(invoice.BillingMemberId,
                                                                                               ProcessingContactType.
                                                                                                 AutoBillingValueDeterminationAlerts,
                                                                                               sisOpsMail,
                                                                                               (int)
                                                                                               EmailTemplateId.
                                                                                                 AutoBillingInvoiceUnavalableNotification);
              }
              return null;
          }
            var threshouldValue = ValidationCache.Instance.AbillThreshouldValue;
            // Here get the threshould value from system parameter
            var noOfAvalableInvoices = passengerConfiguration.InvoiceNumberRangeTo.Value - numericInvoiceNumber;
            if (noOfAvalableInvoices <= threshouldValue)
            {
                var isNotificationSent = false;
                var mailKey = fileName + "-" + AbillThreshouldConstant;
                if (AutoBillingInvoiceAlertTrack.ContainsKey(mailKey))
                {
                    AutoBillingInvoiceAlertTrack.TryGetValue(mailKey, out isNotificationSent);
                }
                if (!isNotificationSent)
                {
                    string sisOpsMail = null;
                    if (!string.IsNullOrEmpty(ValidationCache.Instance.SISOpsEmailId))
                        sisOpsMail = ValidationCache.Instance.SISOpsEmailId;
                    AutoBillingNotification.SendUnavaliableOrThresholdReachedInvoiceNotification(
                        invoice.BillingMemberId, ProcessingContactType.AutoBillingValueDeterminationAlerts, sisOpsMail,
                        (int) EmailTemplateId.AutoBillingInvoiceThreshouldValueReachedNotification, threshouldValue,
                        noOfAvalableInvoices);
                    AutoBillingInvoiceAlertTrack.Add(mailKey, true);
                }
            }
            invoice.InvoiceNumber = passengerConfiguration.InvoiceNumberRangePrefix + numericInvoiceNumber;
          Logger.Debug("New Invoice Number  : " + invoice.InvoiceNumber);
        }
        else
        {
          var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuRnd);
          isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(billingMemberId,
                                                                                                  isrCoupon.
                                                                                                    TicketDocOrFimNumber,
                                                                                                  isrCoupon.
                                                                                                    TicketOrFimIssuingAirline,
                                                                                                  isrCoupon.
                                                                                                    TicketOrFimCouponNumber,
                                                                                                  fileName,
                                                                                                  fileSubmisionDate,
                                                                                                  (int)
                                                                                                  AutoBillingCouponStatus
                                                                                                    .RND,
                                                                                                  cpnDescription,
                                                                                                  false));

          var isNotificationSent = false;
          var mailKey = fileName + "-" + AbillUnavaliableInvoiceConstant;
          if (AutoBillingInvoiceAlertTrack.ContainsKey(mailKey))
          {
            AutoBillingInvoiceAlertTrack.TryGetValue(mailKey, out isNotificationSent);
          }

          if (!isNotificationSent)
          {
            string sisOpsMail = null;
            if (!string.IsNullOrEmpty(ValidationCache.Instance.SISOpsEmailId))
              sisOpsMail = ValidationCache.Instance.SISOpsEmailId;
            AutoBillingNotification.SendUnavaliableOrThresholdReachedInvoiceNotification(invoice.BillingMemberId,
                                                                                         ProcessingContactType.
                                                                                           AutoBillingValueDeterminationAlerts,
                                                                                         sisOpsMail,
                                                                                         (int)
                                                                                         EmailTemplateId.
                                                                                           AutoBillingInvoiceUnavalableNotification);

            AutoBillingInvoiceAlertTrack.Add(mailKey, true);
          }

          return null;
        }
        //SET listing currency as per member profile.
        invoice.ListingCurrencyId = passengerConfiguration.ListingCurrencyId;
        invoice.ListingCurrency = passengerConfiguration.ListingCurrency;
      }

      // Validate duplication of invoice number
      if (invoice.InvoiceNumber != null)
      {
        if (!ValidateInvoiceNumber(invoice.InvoiceNumber.Trim(), invoice.BillingYear, invoice.BillingMemberId))
        {
          var cpnDescription = Messages.ResourceManager.GetString(ErrorCodes.CouponStatuVer);

          if (!string.IsNullOrWhiteSpace(cpnDescription))
          {
            cpnDescription = cpnDescription + ValidationErrorDelimeter + "Duplicate Invoice Number";
            if (cpnDescription.Length > 255)
            {
              cpnDescription = cpnDescription.Substring(0, 254);
            }
          }
          isrAutoBillingModel.AutoBillingUpdateCouponCollection.Add(GetAbUpdateCouponStatusObject(billingMemberId,
                                                                                                  isrCoupon.
                                                                                                    TicketDocOrFimNumber,
                                                                                                  isrCoupon.
                                                                                                    TicketOrFimIssuingAirline,
                                                                                                  isrCoupon.
                                                                                                    TicketOrFimCouponNumber,
                                                                                                  fileName,
                                                                                                  fileSubmisionDate,
                                                                                                  (int)
                                                                                                  AutoBillingCouponStatus
                                                                                                    .VER,
                                                                                                  cpnDescription,
                                                                                                  false));
          return null;
        }
      }

      //SET Billing code as Non Sampling
      invoice.BillingCode = (int)BillingCode.NonSampling;

      invoice.InvoiceStatus = InvoiceStatusType.Open;

      invoice.InvoiceStatusId = (int)InvoiceStatusType.Open;

      invoice.SubmissionMethod = SubmissionMethod.AutoBilling;

      invoice.SubmissionMethodId = (int)SubmissionMethod.AutoBilling;

      invoice.BillingCategory = BillingCategoryType.Pax;
      invoice.InvoiceDate = DateTime.UtcNow;

        
      #region Old Code -Commented
      //Bloking Rule logic implementation
      //var smiValue = string.Empty;
      //var achZoneId = 3;
      //var isCreditorBlocked = false;
      //var isDebitorBlocked = false;
      //var isCGrpBlocked = false;
      //var isDGrpBlocked = false;

      //var smiForBlockingRule = GetSettlementMethodForAutoBillingInvoice(invoice,invoice.BillingMember, invoice.BilledMember);

      //switch (smiForBlockingRule)
      //{
      //    case (int)SMI.AchUsingIataRules:
      //    case (int)SMI.Ach:
      //        smiValue = "ACH";
      //        BlockingRulesRepository.ValidateBlockingRules(invoice.BillingMemberId,
      //                                                      invoice.BilledMemberId,
      //                                                      invoice.BillingCategory, smiValue, achZoneId,
      //                                                      achZoneId,
      //                                                      out isCreditorBlocked, out isDebitorBlocked,
      //                                                      out isCGrpBlocked, out isDGrpBlocked);
      //        break;
      //    case (int)SMI.Ich:
      //        smiValue = "ICH";
      //        BlockingRulesRepository.ValidateBlockingRules(invoice.BillingMemberId,
      //                                                      invoice.BilledMemberId,
      //                                                      invoice.BillingCategory, smiValue,
      //                                                      invoice.BillingMember.IchConfiguration != null
      //                                                          ? invoice.BillingMember.IchConfiguration.IchZoneId
      //                                                          : 0,
      //                                                      invoice.BilledMember.IchConfiguration != null
      //                                                          ? invoice.BilledMember.IchConfiguration.IchZoneId
      //                                                          : 0,
      //                                                      out isCreditorBlocked, out isDebitorBlocked,
      //                                                      out isCGrpBlocked, out isDGrpBlocked);
      //        break;
      //}

      #endregion

      var isCreditorBlocked = false;
      var isDebitorBlocked = false;
      var isCGrpBlocked = false;
      var isDGrpBlocked = false;
      //SCP164383: Blocking Rule Failed
      //Desc: Hooking a call to centralized code for blocking rule validation
      var smIndicator = GetSettlementMethodForAutoBillingInvoice(invoice, invoice.BillingMember, invoice.BilledMember);
      ValidationForBlockedAirline(invoice.BillingMemberId, invoice.BilledMemberId, (SMI)smIndicator,
                                BillingCategoryType.Pax, out isCreditorBlocked, out isDebitorBlocked,
                                out isCGrpBlocked, out isDGrpBlocked, true);

      // Blocked by Creditor/Debtor/Group Rule
      if (isCreditorBlocked || isDebitorBlocked || isCGrpBlocked || isDGrpBlocked)
      {
          //Settelment Method
          invoice.SettlementMethodId = (int)SMI.Bilateral;
          invoice.BillingCurrencyId =  (int)BillingCurrency.USD;
      }
      else
      {
          //Settelment Method
          invoice.SettlementMethodId = GetSettlementMethodForAutoBillingInvoice(invoice, billingFinalParent, billedFinalParent);
          //Currency of billing
          invoice.BillingCurrencyId = GetBillingCurrencyForAutoBillingInvoice(invoice, billingFinalParent, billedFinalParent);

      }
      
      
      //Digital Signature Flag
      invoice.DigitalSignatureRequiredId = (int)DigitalSignatureRequired.Default;

      //Exchange Rate
      invoice.ExchangeRate = GetExchangeRateForAutoBillingInvoice(invoice, invoice.BillingMember, invoice.BilledMember);

      invoice.InvoiceType = InvoiceType.Invoice;

      invoice.InvoiceTypeId = (int)InvoiceType.Invoice;

      //Populate member location information as per Main location.
      invoice.BillingMemberLocationCode = DefaultMemberLocationCode;
      invoice.BilledMemberLocationCode = DefaultMemberLocationCode;
      invoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.Default;
      invoice.BilledReferenceDataSourceId = (int)ReferenceDataSource.Default;

      // Populate billing member location information from member's location table.
      var billingMemberLocationInformation = new MemberLocationInformation { IsBillingMember = true, Id = Guid.NewGuid() };
      if (PopulateDefaultLocation(invoice.BillingMemberId, billingMemberLocationInformation,
                                  invoice.BillingMemberLocationCode, invoice))
      {
        invoice.MemberLocationInformation.Add(billingMemberLocationInformation);
      }
      // Populate billed member location information from member's location table.
      var billedMemberLocationInformation = new MemberLocationInformation { IsBillingMember = false, Id = Guid.NewGuid() };
      if (PopulateDefaultLocation(invoice.BilledMemberId, billedMemberLocationInformation,
                                  invoice.BilledMemberLocationCode))
      {
        invoice.MemberLocationInformation.Add(billedMemberLocationInformation);
      }


      //Update legal text
      var eBillingConfig = MemberManager.GetEbillingConfig(invoice.BillingMemberId);
      invoice.LegalText = eBillingConfig != null && eBillingConfig.LegalText != null ? eBillingConfig.LegalText.Trim().Replace("\r", "").Replace("\n", "") : string.Empty;

      // Invoice Owner
      invoice.InvoiceOwnerId = (int)SystemUsers.ParsingAndValidation;

      return invoice;
    }

    public static void CopyPropertyValues(object source, object destination)
    {
      var destProperties = destination.GetType().GetProperties();

      foreach (var sourceProperty in source.GetType().GetProperties())
      {
        foreach (var destProperty in destProperties)
        {
          if (destProperty.Name == sourceProperty.Name &&
      destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
          {
            destProperty.SetValue(destination, sourceProperty.GetValue(
                source, new object[] { }), new object[] { });

            break;
          }
        }
      }
    }

    /// <summary>
    /// This function is used to validate RM Source Codes
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    //CMP614: Source Code Validation for PAX RMs
    public String ValidateRMSourceCode(RMSourceCodeValidationCriteria criteria)
    {
      var result = InvoiceRepository.ValidateRMSourceCode(criteria);

      return result;
    }

    //SCP# 293488 : Net Amount Error - Function call hooked to confirm net total amount at coupon level.
    private void ConfirmCouponTotalAmount(PrimeCoupon couponRecord)
    {
        //1. Re-compute/ re-derive the net amount on server side
        var couponTotalAmountComputed = couponRecord.CouponGrossValueOrApplicableLocalFare + couponRecord.IscAmount +
                                        couponRecord.OtherCommissionAmount +
                                        couponRecord.UatpAmount + couponRecord.HandlingFeeAmount +
                                        couponRecord.TaxAmount + couponRecord.VatAmount;
        //2. Compare this derived net total amount with, net total amount received from UI.
        if (!CompareUtil.Compare(couponRecord.CouponTotalAmount, couponTotalAmountComputed, 0D, Constants.PaxDecimalPlaces))
        {
            //3. In case of mismatch, derived amount on server will be saved
            couponRecord.CouponTotalAmount = couponTotalAmountComputed;
        }
    }
  }
  }
