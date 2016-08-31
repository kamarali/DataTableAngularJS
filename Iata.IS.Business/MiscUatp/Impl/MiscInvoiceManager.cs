using System.Collections.Generic;
using System.Linq;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using System;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Core.DI;
using Iata.IS.Data.Pax;
using NVelocity;
using TransactionType = Iata.IS.Model.Enums.TransactionType;


namespace Iata.IS.Business.MiscUatp.Impl
{
  /// <summary>
  /// Business implementation for MISC invoice.
  /// </summary>
  public class MiscInvoiceManager : MiscUatpInvoiceManager, IMiscInvoiceManager, IValidationMiscInvoiceManager
  {
    private const string TimeLimitFlag = "TL";
    private const string ValidationFlagDelimeter = ",";
    private ITemplatedTextGenerator _templatedTextGenerator;
    private const string MiscBillingHistoryAuditTrailTemplateResourceName = "Iata.IS.Business.App_Data.Templates.MiscBillingHistoryAuditTrailPdf.vm";
    /// <summary>
    /// MiscInvoiceRepository, will be injected by container.
    /// </summary>
    /// <value>The misc invoice repository.</value>
    public IMiscInvoiceRepository MiscInvoiceRepository { get; set; }

    /// <summary>
    /// Misc. repository, will be injected by container.
    /// </summary>
    /// <value>The misc invoice repository.</value>
    public IRepositoryEx<MiscUatpInvoice, InvoiceBase> MiscCorrespondenceInvoiceRepository { get; set; }



    /// <summary>
    /// Initializes a new instance of the <see cref="MiscInvoiceManager"/> class.
    /// </summary>
    public MiscInvoiceManager()
      : base(new MiscErrorCodes())
    {
      InvoiceType = InvoiceType.Invoice;

      BillingCategory = BillingCategoryType.Misc;

      CorrInvoiceDABTransactionType = TransactionType.MiscCorrInvoiceDueToAuthorityToBill;

      // This property will be used to set the code for corresponding Billing Category.
      // BillingCode = BillingCode.Misc;

      // This property will be used to set the transaction type for corresponding Billing Category.
      // TransactionType = Iata.IS.Model.Pax.Enums.TransactionType
    }

    ///// <summary>
    ///// Submits invoices
    ///// </summary>
    ///// <param name="invoiceIdList">List of invoice ids to be submitted</param>
    ///// <returns></returns>
    //public IList<MiscUatpInvoice> UpdateInvoiceTransactionStatuses(List<string> invoiceIdList)
    //{
    //  var invoiceList = invoiceIdList.Select(UpdateInvoiceTransactionStatus).ToList();

    //  return invoiceList.Where(invoice => invoice != null && invoice.TransactionStatus == TransactionStatus.Accepted).ToList();
    //}

    public MiscUatpInvoice UpdateInvoiceTransactionStatus(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();

      // Replaced with LoadStrategy single call
      // var invoice = MiscInvoiceRepository.Single(invoiceObj => invoiceObj.Id == invoiceGuid);
      var invoice = MiscInvoiceRepository.Single(invoiceGuid);

      //if (invoice.TransactionStatus == TransactionStatus.Accepted)
      //  return null;

      //invoice.TransactionStatus = TransactionStatus.Accepted;

      // Update invoice to database.
      var updatedInvoice = MiscInvoiceRepository.Update(invoice);

      UnitOfWork.CommitDefault();

      return updatedInvoice;
    }

    /// <summary>
    /// Update multiple invoices status
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <param name="miscLocationCode">The misc location code.</param>
    public void UpdateInvoiceStatus(int billingYear, int billingMonth, int billingPeriod, int billedMemberId, int billingCategory, string miscLocationCode = null)
    {
        var invoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof(IInvoiceRepository));
        invoiceRepository.UpdateInvoiceStatus(billingYear, billingMonth, billingPeriod, billedMemberId, billingCategory, miscLocationCode);
    }

   

    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //protected override DateTime GetExpiryDatePeriod(MiscUatpInvoice miscUatpInvoice, BillingPeriod? billingPeriod, out TransactionType currentTransactionType)
    //{
    //  TransactionType transactionType = TransactionType.MiscRejection1;
    //  currentTransactionType = TransactionType.MiscOriginal;
    

    //  switch (miscUatpInvoice.InvoiceType)
    //  {
    //    case InvoiceType.Invoice:
    //      transactionType = TransactionType.MiscRejection1;
    //      currentTransactionType = TransactionType.MiscOriginal;
    //      break;
    //    case InvoiceType.CreditNote:
    //      transactionType = TransactionType.MiscRejection1;
    //      currentTransactionType = TransactionType.MiscOriginal;
    //      break;
    //    case InvoiceType.RejectionInvoice:
    //      switch (miscUatpInvoice.RejectionStage)
    //      {
    //        case 1:
    //          // No further rejection for ICH/Bilateral invoices.
    //          transactionType = TransactionType.MiscCorrespondence;
    //          if (miscUatpInvoice.SettlementMethodId == (int)SMI.Ich || ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId) || miscUatpInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules)
    //          {
    //            // No further rejection for ICH/Bilateral invoices.
    //            transactionType = TransactionType.MiscCorrespondence;
    //          }
    //          else if (miscUatpInvoice.SettlementMethodId == (int)SMI.Ach)
    //          {
    //            transactionType = TransactionType.MiscRejection2;
    //          }
    //          currentTransactionType = TransactionType.MiscRejection1;
    //          break;
    //        case 2:
    //          transactionType = TransactionType.MiscCorrespondence;
    //          currentTransactionType = TransactionType.MiscRejection2;
    //          break;
    //      }

    //      break;

    //    case InvoiceType.CorrespondenceInvoice:
    //      transactionType = TransactionType.MiscRejection1;
    //      currentTransactionType = TransactionType.MiscCorrInvoiceDueToAuthorityToBill;
    //      break;

    //    default:
    //      transactionType = TransactionType.MiscRejection1;
    //      currentTransactionType = TransactionType.MiscOriginal;
    //      break;
    //  }

    //  return ReferenceManager.GetExpiryDatePeriodMethod(transactionType, miscUatpInvoice, BillingCategoryType.Misc, Constants.SamplingIndicatorNo, billingPeriod);
    //}

    ///// <summary>
    ///// Submits invoices
    ///// </summary>
    ///// <param name="invoiceIdList">List of invoice ids to be submitted</param>
    ///// <returns></returns>
    //public IList<MiscUatpInvoice> UpdateCorrespondenceTransactionStatuses(List<string> invoiceIdList)
    //{
    //  var invoiceList = invoiceIdList.Select(SubmitInvoice).ToList();

    //  return invoiceList.Where(invoice => invoice != null && invoice.TransactionStatus == TransactionStatus.Accepted).ToList();
    //}

    //public MiscUatpInvoice UpdateCorrespondenceTransactionStatuse(string correspondenceId)
    //{
    //  var correspondenceGuid = correspondenceId.ToGuid();

    //  // Replaced with LoadStrategy single call
    //  //var correspondence = MiscInvoiceRepository.Single(invoiceObj => invoiceObj.Id == correspondenceGuid);
    //  var correspondence = MiscInvoiceRepository.Single(correspondenceGuid);

    //  if (correspondence.TransactionStatus == TransactionStatus.Accepted)
    //    return null;

    //  correspondence.TransactionStatus = TransactionStatus.Accepted;

    //  // Update invoice to database.
    //  var updatedInvoice = MiscInvoiceRepository.Update(correspondence);

    //  UnitOfWork.CommitDefault();

    //  return updatedInvoice;
    //}

    /// <summary>
    /// Common validations for Misc./UATP invoice.
    /// </summary>
    /// <param name="invoiceHeader">Current instance of Misc./UATP invoice.</param>
    /// <param name="invoiceHeaderInDb">Db instance of Misc./UATP invoice.</param>
    /// <returns></returns>
    protected override bool ValidateInvoiceHeader(MiscUatpInvoice invoiceHeader, MiscUatpInvoice invoiceHeaderInDb)
    {
      return base.ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb);
    }



    /// <summary>
    /// Validates the correspondence time limit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="correspondenceStatusId">The correspondence status id.</param>
    /// <param name="authorityToBill">if set to true [authority to bill].</param>
    /// <param name="correspondenceDate">The correspondence date.</param>
    /// <returns>
    /// true if [is valid correspondence time limit] [the specified invoice id]; otherwise, false.
    /// </returns>
    public bool IsCorrespondenceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate)
    {
      var isOutsideTimeLimit = false;
      var invoiceHeader = GetInvoiceDetail(invoiceId);
      var transactionType = TransactionType.MiscCorrespondence;

      if (correspondenceStatusId == (int)CorrespondenceStatus.Expired)
      {
        transactionType = TransactionType.MiscCorrInvoiceDueToExpiry;
      }
      else if (correspondenceStatusId == (int)CorrespondenceStatus.Open && authorityToBill)
      {
        transactionType = TransactionType.MiscCorrInvoiceDueToAuthorityToBill;
      }
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      isOutsideTimeLimit = (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false)) ? !ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType, invoiceHeader.SettlementMethodId, correspondenceDate) : !ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType, Convert.ToInt32(SMI.Bilateral), correspondenceDate);


      return isOutsideTimeLimit;
    }



    /// <summary>
    /// Get Billing History Search Result
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    //public List<MiscUatpInvoice> GetBillingHistoryAuditTrail(string invoiceId)
    //{
    //    var invoiceIdTrailList = MiscInvoiceRepository.GetBillingHistoryAuditTrail(invoiceId);

    //  // Replaced with LoadStrategy GetSingleInvoiceTrail call
    //  return invoiceIdTrailList.Select(trail => MiscInvoiceRepository.GetSingleInvoiceTrail(trail.Id)).ToList();
    //  // return invoiceIdTrailList.Select(trail => MiscInvoiceRepository.GetSingleInvoiceTrail(invoiceObject => invoiceObject.Id == trail.Id)).ToList();
    //}

    /// <summary>
    /// Method to generate Misc Billing history Audit trail PDF
    /// </summary>
    /// <param name="auditTrail">Audit trail object on which pdf is to be generated</param>
    /// <param name="currentMemberId">Current session member</param>
    /// <returns>Audit trail Html string</returns>
    //public string GenerateMiscBillingHistoryAuditTrailPdf(AuditTrailPdf auditTrail, int currentMemberId)
    //{
    //  _templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
    //  var orderedLineItemsList = new List<LineItem>().AsEnumerable();
    //  var thirdStageRejectionInvoiceCorrespondenceList = new List<MiscCorrespondence>().AsEnumerable();
    //  var orderedRejectionInvoiceList = new List<MiscUatpInvoice>().AsEnumerable();

    //  if (auditTrail.CorrespondenceInvoice != null)
    //  {
    //    // Ordered Line Item list
    //    orderedLineItemsList = auditTrail.CorrespondenceInvoice.LineItems.OrderBy(lineitem => lineitem.LineItemNumber);
    //  }

    //  if (auditTrail.RejectionInvoiceList.Count > 0)
    //  {
    //    if (auditTrail.CorrespondenceInvoice != null)
    //    {
    //      // Get Correspondence list from third stage Rejection invoice details
    //      thirdStageRejectionInvoiceCorrespondenceList = auditTrail.RejectionInvoiceList.Where(inv => inv.InvoiceNumber == auditTrail.CorrespondenceInvoice.RejectedInvoiceNumber).OrderByDescending(invoice => invoice.RejectionStage).ToList()[0].Correspondences.OrderByDescending(corr => corr.CorrespondenceStage);
    //    }
    //    else
    //    {
    //      // Get Correspondence list from third stage Rejection invoice details
    //      thirdStageRejectionInvoiceCorrespondenceList = auditTrail.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList()[0].Correspondences.OrderByDescending(corr => corr.CorrespondenceStage);
    //    }

    //    // Get Ordered rejection invoice list
    //    orderedRejectionInvoiceList = auditTrail.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList();
    //  }

    //  // Instantiate VelocityContext
    //  var context = new VelocityContext();
    //  // Instantiate NonSamplingInvoiceManager
    //  var miscInvoiceManager = new MiscInvoiceManager();

    //  context.Put("auditTrail", auditTrail);
    //  context.Put("orderedLineItemsList", orderedLineItemsList);
    //  context.Put("thirdStageRejectionInvoiceCorrespondenceList", thirdStageRejectionInvoiceCorrespondenceList);
    //  context.Put("currentMemberId", currentMemberId);
    //  context.Put("orderedRejectionInvoiceList", orderedRejectionInvoiceList);
    //  context.Put("miscInvoiceManager", miscInvoiceManager);

    //  // Generate Audit trail html string using .vm file and NVelocity context
    //  var reportContent = _templatedTextGenerator.GenerateEmbeddedTemplatedText(MiscBillingHistoryAuditTrailTemplateResourceName, context);
    //  // return Audit trail html string
    //  return reportContent;
    //}

    /// <summary>
    /// Method to check whether LineItem exists for given original Invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail object in which Original invoice exixts</param>
    /// <param name="lineItem">Line item to check</param>
    /// <returns>True if Lineitem exixts else false</returns>
    public bool ChechWhetherLineItemExistsForOriginalInvoice(AuditTrailPdf auditTrail, LineItem lineItem)
    {
      if (auditTrail.OriginalInvoice != null)
        return auditTrail.OriginalInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == lineItem.LineItemNumber) != null;
      else
        return false;
    }

    /// <summary>
    /// Method to get line item charge amount from Original invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail object in which Original invoice exists</param>
    /// <param name="lineItem">Lineitem model</param>
    /// <returns>line item charge amount</returns>
    public decimal GetLineItemChargeAmountFromOriginalInvoice(AuditTrailPdf auditTrail, LineItem lineItem)
    {
      return auditTrail.OriginalInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == lineItem.LineItemNumber).TotalNetAmount;
    }

    /// <summary>
    /// Method to check whether LineItem exists for given rejection Invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">Rejection invoice model</param>
    /// <param name="lineItem">Line item to check</param>
    /// <returns>True if Lineitem exixts else false</returns>
    public bool ChechWhetherLineItemExistsForRejectionInvoice(MiscUatpInvoice rejectionInvoice, LineItem lineItem)
    {
      if (rejectionInvoice != null)
        return rejectionInvoice.LineItems.Find(originalItem => originalItem.LineItemNumber == lineItem.OriginalLineItemNumber) != null;
      else return false;
    }

    /// <summary>
    /// Method to get line item charge amount from Rejection invoice. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">rejection Invoice object</param>
    /// <param name="lineItem">Lineitem model</param>
    /// <returns>line item charge amount</returns>
    public decimal GetLineItemChargeAmountFromRejectionInvoice(MiscUatpInvoice rejectionInvoice, LineItem lineItem)
    {
      return rejectionInvoice.LineItems.Find(itemO => itemO.LineItemNumber == lineItem.OriginalLineItemNumber).TotalNetAmount;
    }

    /// <summary>
    /// Method to get Ordered LineItem list. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="rejectionInvoice">Invoice in which Lineitem exists</param>
    /// <returns>Ordered Line Item list</returns>
    public List<LineItem> GetOrderedLineItemList(MiscUatpInvoice rejectionInvoice)
    {
      return rejectionInvoice.LineItems.OrderBy(lineitem => lineitem.LineItemNumber).ToList();
    }

    /// <summary>
    /// Method to get Correspondence count. Method used for Audit trail PDF generation
    /// </summary>
    /// <param name="auditTrail">Audit trail model in which Correspondence exists</param>
    /// <returns>Correspondence count</returns>
    public int GetCorrespondenceCount(AuditTrailPdf auditTrail)
    {
      return auditTrail.RejectionInvoiceList[auditTrail.RejectionInvoiceList.Count - 1].Correspondences.Count;
    }

    /// <summary>
    /// Get Billing History Search Result
    /// </summary>
    /// <param name="invoiceCriteria"></param>
    /// <returns></returns>
    //public IQueryable<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria)
    //{
    //  var invoice = MiscInvoiceRepository.GetBillingHistorySearchResult(invoiceCriteria);
    //  return invoice.AsQueryable();
    //}

    /// <summary>
    /// Gets the billing history correspondence search result.
    /// </summary>
    /// <param name="corrCriteria">The correspondence criteria.</param>
    /// <returns></returns>
    //public IQueryable<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrCriteria)
    //{
    //  var correspondences = MiscInvoiceRepository.GetBillingHistoryCorrSearchResult(corrCriteria);
    //  return correspondences.AsQueryable();
    //}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="issuingOrganizationId"></param>
    /// <param name="invoiceBillingPeriod"></param>
    /// <returns></returns>
    public override bool IsOnBehalfBillingMemberMigrated(int issuingOrganizationId, BillingPeriod invoiceBillingPeriod)
    {
      var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(issuingOrganizationId);
      if (miscellaneousConfiguration == null)
      {
        return false;
      }

      //if (!miscellaneousConfiguration.IsBillingDataSubmittedByThirdPartiesRequired) return false;
      var isXmlMigrationDate = miscellaneousConfiguration.BillingIsXmlMigrationDate;
      if (!isXmlMigrationDate.HasValue) return false;

      var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, isXmlMigrationDate.Value.Day);

      return (miscellaneousConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified && invoiceBillingPeriod >= isXmlMigrationPeriod);
    }

    /// <summary>
    /// Determines whether billed member migrated for specified billing month and period in invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <returns>
    /// True if member migrated for the specified invoice header; otherwise, false.
    /// </returns>
    public override bool IsMemberMigrated(MiscUatpInvoice invoiceHeader)
    {
      var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(invoiceHeader.BilledMemberId);
      if (miscellaneousConfiguration == null)
      {
        return false;
      }

      var isXmlMigrationDate = miscellaneousConfiguration.BillingIsXmlMigrationDate;
      var isWebMigrationDate = miscellaneousConfiguration.BillingIswebMigrationDate;
      if (!isXmlMigrationDate.HasValue && !isWebMigrationDate.HasValue) return false;

      var isXmlMigration = false;
      var settlementMonthPeriod = new BillingPeriod
      {
        Year = invoiceHeader.SettlementYear,
        Month = invoiceHeader.SettlementMonth,
        Period = invoiceHeader.SettlementPeriod
      };
      if (isXmlMigrationDate.HasValue)
      {
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, isXmlMigrationDate.Value.Day);

        isXmlMigration = miscellaneousConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified && settlementMonthPeriod >= isXmlMigrationPeriod;
      }

      var isWebMigration = false;
      if (isWebMigrationDate.HasValue)
      {
        var isWebMigrationPeriod = new BillingPeriod(isWebMigrationDate.Value.Year, isWebMigrationDate.Value.Month, isWebMigrationDate.Value.Day);

        isWebMigration = settlementMonthPeriod >= isWebMigrationPeriod;
      }

      return isXmlMigration || isWebMigration;
    }

    public override bool IsBillingMemberMigrated(MiscUatpInvoice invoiceHeader)
    {

      var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(invoiceHeader.BillingMemberId);
      if (miscellaneousConfiguration == null)
      {
        return false;
      }

      var isXmlMigrationDate = miscellaneousConfiguration.BillingIsXmlMigrationDate;
      if (!isXmlMigrationDate.HasValue) return false;

      var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, isXmlMigrationDate.Value.Day);

      //var settlementMonthPeriod = new BillingPeriod(invoiceHeader.SettlementYear, invoiceHeader.SettlementMonth, invoiceHeader.SettlementPeriod);

      return (miscellaneousConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified && invoiceHeader.InvoiceBillingPeriod >= isXmlMigrationPeriod);

    }


    //private MiscUatpInvoice GetOriginalInvoice(MiscUatpInvoice correspondenceInvoice, MiscCorrespondence miscCorrespondence)
    //{

    //  MiscUatpInvoice originalInvoice = null;
    //  MiscUatpInvoice rejectedInvoice1;

    //  if (miscCorrespondence != null)
    //  {
    //    rejectedInvoice1 = MiscUatpInvoiceRepository.Single(invoiceId: miscCorrespondence.InvoiceId,billingCategoryId: (int)BillingCategory);
    //    if (rejectedInvoice1.RejectionStage == 1)
    //    {
    //      //Fetch original invoice
    //      originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber, billingMemberId: rejectedInvoice1.BilledMemberId, billedMemberId: rejectedInvoice1.BillingMemberId, billingCategoryId: correspondenceInvoice.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented);
    //    }
    //    else
    //    {
    //      //Fetch original invoice
    //      originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber, billingMemberId: rejectedInvoice1.BilledMemberId, billedMemberId: rejectedInvoice1.BillingMemberId, billingCategoryId: (int)BillingCategory, invoiceStatusId: (int)InvoiceStatusType.Presented);
    //    }
    //  }
    //  return originalInvoice;
    //}



    /// <summary>
    /// Validates the invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public override MiscUatpInvoice ValidateInvoice(string invoiceId)
    {
      var webValidationErrors = new List<WebValidationError>();
      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);

      var miscUatpInvoice = base.ValidateInvoice(invoiceId);

      webValidationErrors.AddRange(miscUatpInvoice.ValidationErrors);

      if (webValidationErrors.Count > 0)
      {
        miscUatpInvoice.ValidationErrors.Clear();
        miscUatpInvoice.ValidationErrors.AddRange(webValidationErrors);
        miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ValidationError;
        // updating validation status to failed.
        if (webValidationErrors.Count > 1)
          miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
        miscUatpInvoice.ValidationDate = DateTime.UtcNow;
      }
      else
      {
        // Invoice through the validation, change invoice status to Ready for submission. 
        miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ReadyForSubmission;
        // updating validation status to completed
        miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Completed;
        miscUatpInvoice.ValidationDate = DateTime.UtcNow;
      }

      // Update the invoice.
      // MiscUatpInvoiceRepository.Update(miscUatpInvoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
      //SCP345230: ICH Settlement Error - SIS Production
      InvoiceRepository.SetInvoiceAndValidationStatus(miscUatpInvoice.Id, miscUatpInvoice.InvoiceStatusId, miscUatpInvoice.ValidationStatusId, miscUatpInvoice.IsFutureSubmission, miscUatpInvoice.ClearingHouse, miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency, (int)miscUatpInvoice.BillingCategory);

      // Update latest invoice status.
      ValidationErrorManager.UpdateValidationErrors(miscUatpInvoice.Id, miscUatpInvoice.ValidationErrors, validationErrorsInDb);

      //SCP325375: File Loading & Web Response Stats ManageInvoice
      //UnitOfWork.CommitDefault();

      return miscUatpInvoice;
    }

    /// <summary>
    /// Validates the parsed invoice.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <returns></returns>
    public override bool ValidateParsedInvoice(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, string issuingOrganizationId)
    {
      var isValid = true;

      //Perform common validations
      base.ValidateParsedInvoice(miscUatpInvoice, exceptionDetailsList, fileName, fileSubmissionDate, issuingOrganizationId);

      var validationManager = Ioc.Resolve<IValidationErrorManager>(typeof(IValidationErrorManager));

      miscUatpInvoice.isValidationExceptionSummary = validationManager.GetIsSummary(miscUatpInvoice, exceptionDetailsList, fileName, fileSubmissionDate);
      miscUatpInvoice.ValidationExceptionSummary = validationManager.GetIsSummaryForValidationErrorCorrection(miscUatpInvoice,
                                                                                                     exceptionDetailsList.ToList());
      //Update Status of Invoice as per the validation results
      UpdateParsedInvoiceStatus(miscUatpInvoice, exceptionDetailsList, fileSubmissionDate, fileName);

      if (miscUatpInvoice.isValidationExceptionSummary != null)
      {
        miscUatpInvoice.isValidationExceptionSummary.InvoiceStatus = ((int)miscUatpInvoice.InvoiceStatus).ToString();
      }

      return isValid;
    }

    /// <summary>
    /// Clears transaction data from MISC invoice.
    /// </summary>
    /// <param name="miscUatpInvoice"></param>
    public void ClearInvoiceTransationData(MiscUatpInvoice miscUatpInvoice)
    {
      miscUatpInvoice.LineItems.Clear();
      miscUatpInvoice.MemberContacts.Clear();
      miscUatpInvoice.OtherOrganizationInformations.Clear();
      miscUatpInvoice.MemberLocationInformation.Clear();
      miscUatpInvoice.TaxBreakdown.Clear();
      miscUatpInvoice.AdditionalDetails.Clear();
      miscUatpInvoice.AddOnCharges.Clear();
      miscUatpInvoice.Attachments.Clear();
    }

    public string GetExceptionCodeList(string filter, int billingCategoryTypeId)
    {
      return MiscInvoiceRepository.GetExceptionCodeList(filter, billingCategoryTypeId);
    }

    private bool IgnoreValidationInMigrationPeriod(MiscUatpInvoice miscUatpInvoice)
    {
      if (SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
      {
        return true;
      }

      if (!IsMemberMigrated(miscUatpInvoice))
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Updates the multiple invoices inclusion status and Generation date.
    /// </summary>
    /// <param name="invoiceIdList">The invoice ids.</param>
    /// <param name="inclusionStatusId"></param>
    /// <param name="isUpdateGenerationDate"></param>
    public void UpdateInclusionStatus(List<Guid> invoiceIdList, int inclusionStatusId, bool isUpdateGenerationDate)
    {
      string invoiceIds = string.Join(",", invoiceIdList.Select(invoice => ConvertUtil.ConvertGuidToString(invoice)).ToArray());
      MiscInvoiceRepository.UpdateInclusionStatus(invoiceIds, inclusionStatusId, isUpdateGenerationDate);
    }
  }
}
