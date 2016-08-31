using System;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Business.Pax.Impl
{
  public class SamplingFormFManager: SamplingRejectionManager, ISamplingFormFManager
  {
    public SamplingFormFManager()
    {
      YourInvoiceBillingCode = BillingCode.SamplingFormDE;
      BillingCode = BillingCode.SamplingFormF;
      RejectionTransactionType = TransactionType.SamplingFormF;
      RejectionStage = RejectionStage.StageTwo;
    }

    /// <summary>
    /// Determines whether billed member migrated for specified billing month and period in invoice header.
    /// </summary>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="provisionalBillingMonth">The provisional billing month.</param>
    /// <param name="provisionalBillingYear">The provisional billing year.</param>
    /// <returns>
    /// True if member migrated for the specified invoice header; otherwise, false.
    /// </returns>
    public bool IsMemberMigrated(int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var passengerConfiguration = MemberManager.GetPassengerConfiguration(billedMemberId);
      if(passengerConfiguration == null) return false;
      
      var isIdecMigrationDate = passengerConfiguration.SampleFormDeIsIdecMigratedDate;
      var isXmlMigrationDate = passengerConfiguration.SampleFormDeIsxmlMigratedDate;
      var isWebMigrationDate = passengerConfiguration.SampleFormDeIswebMigratedDate;
      var provisionalBillingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
      // If User migrated for both IS-Xml and IS-IDEC.
      if ((isIdecMigrationDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
        && (isXmlMigrationDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified))
      {
        var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);

        return (provisionalBillingPeriod >= isIdecMigrationPeriod) || (provisionalBillingPeriod >= isXmlMigrationPeriod);
      }

      // If User migrated for IS-IDEC.
      if (isIdecMigrationDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
      {
        var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
        return (provisionalBillingPeriod >= isIdecMigrationPeriod);
      }
      // If User migrated for IS-Xml
      if (isXmlMigrationDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified)
      {
        var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);
        return provisionalBillingPeriod >= isXmlMigrationPeriod;
      }
      // If User migrated for IS-Xml
      if (isWebMigrationDate.HasValue)
      {
        var isWebMigrationPeriod = new BillingPeriod(isWebMigrationDate.Value.Year, isWebMigrationDate.Value.Month, 1);
        return provisionalBillingPeriod >= isWebMigrationPeriod;
      }
      return false;
    }

    public override PaxInvoice CreateInvoice(PaxInvoice invoiceHeader)
    {
      //SetMigrationFlags(invoiceHeader);
      
      return base.CreateInvoice(invoiceHeader);
    }
    
    public override PaxInvoice UpdateInvoice(PaxInvoice invoiceHeader)
    {
    //  var invoiceHeaderInDb = InvoiceRepository.Single(invoice => invoice.Id == invoiceHeader.Id);
      //if(CompareUtil.IsDirty(invoiceHeader.BilledMemberId, invoiceHeaderInDb.BilledMemberId))
        //SetMigrationFlags(invoiceHeader);

      return base.UpdateInvoice(invoiceHeader);
    }
    /// <summary>
    /// Get the sampling constatnt of the linked form DE
    /// </summary>
    /// <param name="billingMemberId"> Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <param name="provisionalBillingMonth">Provisional billing Month</param>
    /// <param name="provisionalBillingYear">Provisional billing Year.</param>
    /// <returns></returns>
    public SamplingConstantDetails GetLinkedFormDESamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear)
    {
      var formDEInvoice = InvoiceRepository.GetFormDEInvoice(
          formDE =>
          formDE.BilledMemberId == billingMemberId && formDE.BillingMemberId == billedMemberId && formDE.ProvisionalBillingMonth == provisionalBillingMonth &&
          formDE.ProvisionalBillingYear == provisionalBillingYear && formDE.BillingCode == (int)BillingCode.SamplingFormDE &&
          (formDE.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || formDE.InvoiceStatusId == (int)InvoiceStatusType.Presented));

      bool isMemberMigratedForFormDE = IsMemberMigrated(billedMemberId, provisionalBillingMonth, provisionalBillingYear);

      bool isFormDEFound = formDEInvoice != null;
      var samplingConstantDetails = new SamplingConstantDetails();
      samplingConstantDetails.SamplingConstant = formDEInvoice != null ? formDEInvoice.SamplingFormEDetails.SamplingConstant : 0;
      if (formDEInvoice != null) samplingConstantDetails.IsFormDataFound = true;
      if (isMemberMigratedForFormDE && !isFormDEFound)
        samplingConstantDetails.ErrorMessage = Messages.ResourceManager.GetString(SamplingErrorCodes.MemberMigratedForFormDEButDataNotFound);

      return samplingConstantDetails;
      //invoiceHeader.IsFormDEViaIS = isFormDEFound;
    }
    
    /// <summary>
    /// Determines whether [is form D exists] [the specified rejection memo record].
    /// </summary>
    /// <param name="rejectionMemoRecord">The rejection memo record.</param>
    /// <param name="rejectionMemoInvoice">The rejection memo invoice.</param>
    /// <returns>
    /// 	<c>true</c> if [is form D exists] [the specified rejection memo record]; otherwise, <c>false</c>.
    /// </returns>
    private bool IsParentFormExists(RejectionMemo rejectionMemoRecord, PaxInvoice rejectionMemoInvoice)
    {
      return InvoiceRepository.GetCount(invoice => invoice.InvoiceNumber.ToUpper() == rejectionMemoRecord.YourInvoiceNumber.ToUpper()
                                                   && invoice.BillingMonth == rejectionMemoRecord.YourInvoiceBillingMonth
                                                   && invoice.BillingYear == rejectionMemoRecord.YourInvoiceBillingYear
                                                   && invoice.BillingPeriod == rejectionMemoRecord.YourInvoiceBillingPeriod
                                                   && invoice.BillingMemberId == rejectionMemoInvoice.BilledMemberId
                                                   && invoice.BilledMemberId == rejectionMemoInvoice.BillingMemberId
                                                   && invoice.ProvisionalBillingMonth == rejectionMemoInvoice.ProvisionalBillingMonth
                                                   && invoice.ProvisionalBillingYear == rejectionMemoInvoice.ProvisionalBillingYear
                                                   && invoice.BillingCode == (int)BillingCode.SamplingFormDE) > 0;
    }

     //protected override PaxInvoice ValidateRejectionMemo(RejectionMemo rejectionMemoRecord, RejectionMemo rejectionMemoRecordInDb, PaxInvoice rejectionInvoice)
     //{
     //  bool isUpdateOperation = false;
     //  if (rejectionMemoRecordInDb != null)
     //  {
     //    isUpdateOperation = true;
     //  }

     //  // Linking: Form D/E for the provisional billing month exists in the system.
     //  //if (rejectionInvoice.IsFormDEViaIS &&
     //  //    (!isUpdateOperation ||
     //  //     (CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceNumber) ||
     //  //      CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingMonth) ||
     //  //      CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingPeriod, rejectionMemoRecord.YourInvoiceBillingPeriod) ||
     //  //      CompareUtil.IsDirty(rejectionMemoRecordInDb.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingYear))))
     //  //{
     //  //  // Linking outcome of Form F will be considered not successful and linking errors will be ignored during the header capture if:
     //  //  // 1. If the profile of the billed member indicates that migration of Form D/E was done after the provisional billing month for which the Form F is being created. AND
     //  //  // 2. Form D/E for the provisional billing month does NOT exist in the system.
     
     //  //  if (!IsParentFormExists(rejectionMemoRecord, rejectionInvoice))
     //  //  {
     //  //    throw new ISBusinessException(ErrorCodes.LinkedInvoiceNotFound);
     //  //  }
     //  //}

     //  return base.ValidateRejectionMemo(rejectionMemoRecord, rejectionMemoRecordInDb, rejectionInvoice);
     //}

    /// <summary>
    /// Get Sampling Coupon breakdown details.
    /// </summary>
    /// <param name="issuingAirline">Issuing Airline.</param>
    /// <param name="couponNo">Coupon Number.</param>
    /// <param name="ticketDocNo">Ticket Document Number.</param>
    /// <param name="rmId">Rejection Memo ID.</param>
    /// <param name="billingMemberId">Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <param name="rejectionStage">Not used/ to make materializer common.</param>
    /// <returns></returns>
   public RMLinkedCouponDetails GetSamplingCouponBreakdownRecordDetails(string issuingAirline, int couponNo, long ticketDocNo, Guid rmId, int billingMemberId, int billedMemberId)
   {
     return RejectionMemoCouponBreakdownRepository.GetSamplingCouponLinkingDetails(issuingAirline, couponNo, ticketDocNo, rmId, billingMemberId, billedMemberId);
   }

    /// <summary>
    /// Get sampling coupon detail.
    /// </summary>
    /// <param name="couponId">Coupon ID.</param>
    /// <param name="rejectionMemoId">Rejection memo ID.</param>
    /// <param name="billingMemberId">Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <returns></returns>
    public RMLinkedCouponDetails GetSamplingCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      return RejectionMemoCouponBreakdownRepository.GetSamplingLinkedCouponAmountDetails(couponId, rejectionMemoId, billingMemberId, billedMemberId);
    }

    /// <summary>
    /// Gets the linked form DE details.
    /// </summary>
    /// <param name="rmLinkingCriteria">The RM linking criteria.</param>
    /// <returns></returns>
    public SamplingFormFLinkingResult GetLinkedFormDEDetails(SamplingRMLinkingCriteria rmLinkingCriteria)
    {
      // set IgnoreValidationOnMigrationPeriod flag from system parameter.
      if (!IsMemberMigrated(rmLinkingCriteria.BilledMemberId, rmLinkingCriteria.ProvBillingMonth, rmLinkingCriteria.ProvBillingYear))
      {
        rmLinkingCriteria.IgnoreValidationOnMigrationPeriod = true;
      }
      else
      {
        rmLinkingCriteria.IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod;
      }
      // call to stored procedure.
      var formDELinkingDetails = RejectionMemoRepository.GetFormDELinkingDetails(rmLinkingCriteria);

      return formDELinkingDetails;
    }
  }
}
