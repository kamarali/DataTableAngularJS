using System;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.Linq;

namespace Iata.IS.Business.Pax.Impl
{
  public class SamplingFormXfManager : SamplingRejectionManager, ISamplingFormXfManager
  {
    public SamplingFormXfManager()
    {
      YourInvoiceBillingCode = BillingCode.SamplingFormF;
      BillingCode = BillingCode.SamplingFormXF;
      RejectionTransactionType = TransactionType.SamplingFormXF;
      RejectionStage = RejectionStage.StageThree;
    }

    /// <summary>
    /// Validate Rejection memo record.
    /// </summary>
    /// <param name="rejectionMemoRecord">Rejection memo record to be validated.</param>
    /// <param name="rejectionMemoRecordInDb">The rejection memo record in db.</param>
    /// <param name="warningMessage">Warning Message.</param>
    /// <param name="invoice">The invoice.</param>
    protected override PaxInvoice ValidateRejectionMemo(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordInDb, PaxInvoice invoice, out string warningMessage)
    {
      var rejectionInvoice = base.ValidateRejectionMemo(rejectionMemoRecord, rejectionMemoRecordInDb, invoice, out warningMessage);

      var isUpdateOperation = false;
      if (rejectionMemoRecordInDb != null)
      {
        isUpdateOperation = true;
      }

      if (!isUpdateOperation ||
          (CompareUtil.IsDirty(rejectionMemoRecordInDb.YourRejectionNumber, rejectionMemoRecord.YourRejectionNumber) ||
           CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingPeriod, rejectionMemoRecord.YourInvoiceBillingPeriod) ||
           CompareUtil.IsDirty(rejectionMemoRecordInDb.RejectionStage, rejectionMemoRecord.RejectionStage)))
      {


        // SCP249173: XF Sample Forms
        // Sampling Form XF Already Exists Check for below combination:
        // 1. Billing & Billed Member
        // 2. Invoice Staus not equal to Error Non-Correctable and Validation status not eqaul to failed.
        // 3. Billing Code = 7
        // 4. Your Invoice
        // 5. Your RM No.
        // 6. Your Billing Period i.e. YYYYMMPP.
        // 7. Rejection Stage = 3
        var samplingFormXFAlreadyExists = InvoiceBaseRepository.Get(
          inv =>
          inv.BillingMemberId == invoice.BillingMemberId && inv.BilledMemberId == invoice.BilledMemberId &&
          !(inv.InvoiceStatusId == (int) InvoiceStatusType.ErrorNonCorrectable &&
            inv.ValidationStatusId == (int) InvoiceValidationStatus.Failed) &&
          inv.BillingCode == (int) Model.Pax.Enums.BillingCode.SamplingFormXF).Join(
            RejectionMemoRepository.Get(
              rm =>
              rm.RejectionStage == rejectionMemoRecord.RejectionStage &&
              rm.YourInvoiceBillingYear == rejectionMemoRecord.YourInvoiceBillingYear &&
              rm.YourInvoiceBillingMonth == rejectionMemoRecord.YourInvoiceBillingMonth &&
              rm.YourInvoiceBillingPeriod == rejectionMemoRecord.YourInvoiceBillingPeriod &&
              rm.YourInvoiceNumber.Trim() == rejectionMemoRecord.YourInvoiceNumber.Trim() &&
              rm.YourRejectionNumber.Trim() == rejectionMemoRecord.YourRejectionNumber.Trim()), inv => inv.Id,
            rm => rm.InvoiceId, (inv, rm) => rm).Count() > 0;


        if (samplingFormXFAlreadyExists)
        {
          throw new ISBusinessException(ErrorCodes.SamplingFormXFAlreadyExistsforYourInvRmBillingPeriod);
        }
      }

      return rejectionInvoice;
    }

    /// <summary>
    /// Determines whether billed member migrated for specified billing month and period in invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <returns>
    /// True if member migrated for the specified invoice header; otherwise, false.
    /// </returns>
    public override bool IsMemberMigrated(PaxInvoice invoiceHeader)
    {
      var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoiceHeader.BilledMemberId);
      if (passengerConfiguration == null) return false;

      var isIdecMigrationDate = passengerConfiguration.SampleFormFxfIsIdecMigratedDate;
      var isXmlMigrationDate = passengerConfiguration.SampleFormFxfIsxmlMigratedDate;

      return (isIdecMigrationDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified &&
              invoiceHeader.ProvisionalBillingMonth >= isIdecMigrationDate.Value.Month && invoiceHeader.ProvisionalBillingYear >= isIdecMigrationDate.Value.Year)
              || (isXmlMigrationDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified &&
              invoiceHeader.ProvisionalBillingMonth >= isXmlMigrationDate.Value.Month && invoiceHeader.ProvisionalBillingYear >= isXmlMigrationDate.Value.Year);
    }

    ///// <summary>
    ///// Determines whether billed member migrated for specified billing month and billign Year
    ///// </summary>
    ///// <param name="billedMemberId"></param>
    ///// <param name="provBillingMonth"></param>
    ///// <param name="provBillingYear"></param>
    ///// <returns></returns>
    //public bool IsMemberMigrated (int billedMemberId, int provBillingMonth, int provBillingYear)
    //{
    //  var passengerConfiguration = MemberManager.GetPassengerConfiguration(billedMemberId);
    //  if (passengerConfiguration == null) return false;

    //  var isIdecMigrationDate = passengerConfiguration.SampleFormDeIsIdecMigratedDate;
    //  var isXmlMigrationDate = passengerConfiguration.SampleFormDeIsxmlMigratedDate;
    //  var provisionalBillingPeriod = new BillingPeriod(provBillingYear, provBillingMonth, 1);

    //  return (isIdecMigrationDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified &&
    //          provBillingMonth >= isIdecMigrationDate.Value.Month && provBillingYear >= isIdecMigrationDate.Value.Year)
    //          || (isXmlMigrationDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified &&
    //          provBillingMonth >= isXmlMigrationDate.Value.Month && provBillingYear >= isXmlMigrationDate.Value.Year);

    //  return false;
    //}
    /// <summary>
    /// Get the sampling constatnt of the linked form F
    /// </summary>
    /// <param name="billingMemberId"> Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <param name="provisionalBillingMonth">Provisional billing Month</param>
    /// <param name="provisionalBillingYear">Provisional billing Year.</param>
    /// <returns></returns>
    public SamplingConstantDetails GetLinkedFormFSamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {

      return InvoiceRepository.GetFormFSamplingConstant(billingMemberId, billedMemberId, provisionalBillingMonth, provisionalBillingYear);
     
    }

    public RMLinkingResultDetails GetLinkedFormFDetails(SamplingRMLinkingCriteria rmLinkingCriteria)
    {
      // call to stored procedure.
      var formFLinkingDetails = RejectionMemoRepository.GetSamplingFormFLinkingDetails(rmLinkingCriteria);

      // compute the difference and net reject amounts.
      if (formFLinkingDetails.MemoAmount != null)
      {
        SetRMLinkedMemoAmountDifference(formFLinkingDetails.MemoAmount);
      }

      return formFLinkingDetails;
    }

    public RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria)
    {
      var result = RejectionMemoRepository.GetLinkedMemoAmountDetails(criteria);
      if (result.MemoAmount != null)
      {
        SetRMLinkedMemoAmountDifference(result.MemoAmount);
      }

      return result;
    }

    public RMLinkedCouponDetails GetSamplingCouponBreakdownRecordDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
    {
      return RejectionMemoCouponBreakdownRepository.GetRMCouponLinkingDetails(issuingAirline, couponNo, ticketDocNo, rmId, billingMemberId, billedMemberId);
    }

    private static void SetRMLinkedMemoAmountDifference(RMLinkingAmount memoAmount)
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
        
        memoAmount.TotalGrossDifference = memoAmount.TotalGrossAmountBilled - memoAmount.TotalGrossAcceptedAmount;
        memoAmount.TotalTaxAmountDifference = memoAmount.TotalTaxAmountBilled - memoAmount.TotalTaxAmountAccepted;
        memoAmount.IscDifference = memoAmount.AllowedIscAmount - memoAmount.AcceptedIscAmount;
        memoAmount.OtherCommissionDifference = memoAmount.AllowedOtherCommission - memoAmount.AcceptedOtherCommission;
        memoAmount.UatpAmountDifference = memoAmount.AllowedUatpAmount - memoAmount.AcceptedUatpAmount;
        memoAmount.HandlingFeeAmountDifference = memoAmount.AllowedHandlingFee - memoAmount.AcceptedHandlingFee;
        memoAmount.TotalVatAmountDifference = memoAmount.TotalVatAmountBilled - memoAmount.TotalVatAmountAccepted;
        
        memoAmount.TotalNetRejectAmount = memoAmount.TotalGrossDifference + memoAmount.TotalTaxAmountDifference +
                                          memoAmount.IscDifference + memoAmount.OtherCommissionDifference
                                          + memoAmount.UatpAmountDifference + memoAmount.HandlingFeeAmountDifference +
                                          memoAmount.TotalVatAmountDifference;
      }
    }

    /// <summary>
    /// Get the single record details from the list of RM coupon
    /// </summary>
    public RMLinkedCouponDetails GetRMCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      return RejectionMemoCouponBreakdownRepository.GetLinkedCouponAmountDetails(couponId, rejectionMemoId, billingMemberId, billedMemberId);
    }
  }
}
