using System.Linq;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Business.Pax;
using System.IO;
using Iata.IS.Core.DI;
using System;
using Iata.IS.Model.Common;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
namespace Iata.IS.Business.MiscUatp.Impl
{
  /// <summary>
  /// Business manager implementation for UATP invoice.
  /// </summary>
  public class UatpInvoiceManager : MiscUatpInvoiceManager, IUatpInvoiceManager, IValidationUatpInvoiceManager
  {

    public UatpInvoiceManager()
      : base(new UatpErrorCodes())
    {
      InvoiceType = InvoiceType.Invoice;

      BillingCategory = BillingCategoryType.Uatp;

      CorrInvoiceDABTransactionType = TransactionType.UatpCorrInvoiceDueToAuthorityToBill;
      // This property will be used to set the code for corresponding Billing Category.
      // BillingCode = BillingCode.Misc;

      // This property will be used to set the transaction type for corresponding Billing Category.
      // TransactionType = Iata.IS.Model.Pax.Enums.TransactionType
    }

    /// <summary>
    /// Validates the invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public override MiscUatpInvoice ValidateInvoice(string invoiceId)
    {
      var miscUatpInvoiceGuid = invoiceId.ToGuid();

      // Get ValidationErrors for invoice from DB.
      var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
      var miscUatpInvoice = base.ValidateInvoice(invoiceId);

      miscUatpInvoice.InvoiceStatus = miscUatpInvoice.ValidationErrors.Count > 0 ? InvoiceStatusType.ValidationError : InvoiceStatusType.ReadyForSubmission;
      if (miscUatpInvoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission)
      {
        // updating validation status to completed
        miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Completed;
        miscUatpInvoice.ValidationDate = DateTime.UtcNow;
      }
      else
      {
        if (miscUatpInvoice.ValidationStatus != InvoiceValidationStatus.ErrorPeriod)
          // updating validation status to Failed
          miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
        miscUatpInvoice.ValidationDate = DateTime.UtcNow;
      }

      // Update the invoice.
      //MiscUatpInvoiceRepository.Update(miscUatpInvoice);
      //SCP325375: File Loading & Web Response Stats ManageInvoice
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
    public override bool ValidateParsedInvoice(MiscUatpInvoice miscUatpInvoice, System.Collections.Generic.IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, string issuingOrganizationId)
    {
      var isValid = true;

      isValid = base.ValidateParsedInvoice(miscUatpInvoice, exceptionDetailsList, fileName, fileSubmissionDate, issuingOrganizationId);

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
    /// Determines whether billed member migrated for specified billing month and period in invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <returns>
    /// True if member migrated for the specified invoice header; otherwise, false.
    /// </returns>
    public override bool IsMemberMigrated(MiscUatpInvoice invoiceHeader)
    {
      var uatpConfiguration = MemberManager.GetUATPConfiguration(invoiceHeader.BilledMemberId,uatpMember:invoiceHeader.BilledMember);
      if (uatpConfiguration == null)
      {
        return false;
      }

      var isXmlMigrationDate = uatpConfiguration.BillingIsXmlMigrationDate;
      var isWebMigrationDate = uatpConfiguration.BillingIswebMigrationDate;
      if (!isXmlMigrationDate.HasValue && !isWebMigrationDate.HasValue) return false;

      var settlementMonthPeriod = new BillingPeriod
      {
        Year = invoiceHeader.SettlementYear,
        Month = invoiceHeader.SettlementMonth,
        Period = invoiceHeader.SettlementPeriod
      };
      var isXmlMigration = false;
      if (isXmlMigrationDate.HasValue)
      {
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month,
                                                     isXmlMigrationDate.Value.Day);

        isXmlMigration = uatpConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified &&
                         settlementMonthPeriod >= isXmlMigrationPeriod;
      }

      var isWebMigration = false;
      if (isWebMigrationDate.HasValue)
      {
        var isWebMigrationPeriod = new BillingPeriod(isWebMigrationDate.Value.Year, isWebMigrationDate.Value.Month,
                                                   isWebMigrationDate.Value.Day);

        isWebMigration = settlementMonthPeriod >= isWebMigrationPeriod;
      }

      return isXmlMigration || isWebMigration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="issuingOrganizationId"></param>
    /// <param name="invoiceBillingPeriod"></param>
    /// <returns></returns>
    public override bool IsOnBehalfBillingMemberMigrated(int issuingOrganizationId, BillingPeriod invoiceBillingPeriod)
    {
      var uatpConfiguration = MemberManager.GetUATPConfiguration(issuingOrganizationId);
      if (uatpConfiguration == null)
      {
        return false;
      }

      //if(!uatpConfiguration.IsBillingDataSubmittedByThirdPartiesRequired) return false;
      var isXmlMigrationDate = uatpConfiguration.BillingIsXmlMigrationDate;
      if (!isXmlMigrationDate.HasValue) return false;

      var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, isXmlMigrationDate.Value.Day);

      return (uatpConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified && invoiceBillingPeriod >= isXmlMigrationPeriod);
    }

    /// <summary>
    /// Determines whether member migrated for specified invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <returns>
    /// true if member migrated for specified invoice header; otherwise, false.
    /// </returns>
    public override bool IsBillingMemberMigrated(MiscUatpInvoice invoiceHeader)
    {

      var uatpConfiguration = MemberManager.GetUATPConfiguration(invoiceHeader.BillingMemberId,uatpMember: invoiceHeader.BillingMember);
      if (uatpConfiguration == null)
      {
        return false;
      }

      var isXmlMigrationDate = uatpConfiguration.BillingIsXmlMigrationDate;
      if (!isXmlMigrationDate.HasValue) return false;

      var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, isXmlMigrationDate.Value.Day);

      return (uatpConfiguration.BillingIsXmlMigrationStatus == MigrationStatus.Certified && invoiceHeader.InvoiceBillingPeriod >= isXmlMigrationPeriod);

    }
    ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //protected override DateTime GetExpiryDatePeriod(MiscUatpInvoice miscUatpInvoice, BillingPeriod? billingPeriod, out TransactionType currentTransactionType)
    //{
    //  TransactionType transactionType = TransactionType;
    //  currentTransactionType = TransactionType.UatpOriginal;
    //  switch (miscUatpInvoice.InvoiceType)
    //  {
    //    case InvoiceType.Invoice:
    //      transactionType = TransactionType.UatpRejection1;
    //      currentTransactionType = TransactionType.UatpOriginal;
    //      break;
    //    case InvoiceType.CreditNote:
    //      transactionType = TransactionType.UatpRejection1;
    //      currentTransactionType = TransactionType.UatpOriginal;
    //      break;
    //    case InvoiceType.RejectionInvoice:
    //      switch (miscUatpInvoice.RejectionStage)
    //      {
    //        case 1:
    //          transactionType = TransactionType.UatpCorrespondence;
    //          if (miscUatpInvoice.SettlementMethodId == (int)SMI.Ich || ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId) || miscUatpInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules)
    //          {
    //            // No further rejection for ICH/Bilateral invoices.
    //            transactionType = TransactionType.UatpCorrespondence;
    //          }
    //          else if (miscUatpInvoice.SettlementMethodId == (int)SMI.Ach)
    //          {
    //            transactionType = TransactionType.UatpRejection2;  
    //          }
              
    //          currentTransactionType = TransactionType.UatpRejection1;
    //          break;
    //        case 2:
    //          transactionType = TransactionType.UatpCorrespondence;
    //          currentTransactionType = TransactionType.UatpRejection2;
    //          break;
    //      }

    //      break;

    //    case InvoiceType.CorrespondenceInvoice:
    //      transactionType = TransactionType.UatpRejection1;
    //      currentTransactionType = TransactionType.UatpCorrInvoiceDueToAuthorityToBill;
    //      break;

    //    default:
    //      transactionType = TransactionType.UatpRejection1;
    //      currentTransactionType = TransactionType.UatpOriginal;
    //      break;
    //  }

    //  return ReferenceManager.GetExpiryDatePeriodMethod(transactionType, miscUatpInvoice, BillingCategoryType.Uatp, Constants.SamplingIndicatorNo, billingPeriod);
    //}
  }
}
