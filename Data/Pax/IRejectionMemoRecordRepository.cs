using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using System;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.Pax
{
  public interface IRejectionMemoRecordRepository : IRepository<RejectionMemo>
  {
    /// <summary>
    /// Gets the rejection memo duplicate count.
    /// </summary>
    /// <param name="billedMemberId">The billed member Id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="rejectionMemoNumber">The Rejection Memo Number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>
    /// Count of records matching specific input values.
    /// </returns>
    long GetRejectionMemoDuplicateCount(int billedMemberId, int billingMemberId, string rejectionMemoNumber, int billingYear, int billingMonth, int billingPeriod);

    /// <summary>
    /// Determines whether RejectionMemo Coupon exists for given parameters.
    /// </summary>
    /// <param name="ticketIssuingAirline">The ticket issuing airline.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="couponNumber">The coupon number.</param>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="rejectionMemoNumber">The rejection memo number.</param>
    /// <param name="billingMemoNumber">The billing memo number.</param>
    /// <param name="creditMemoNumber">The credit memo number.</param>
    /// <returns>
    /// Count of records fetched against specific parameters.
    /// </returns>
    long IsRMCouponExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, string invoiceNumber, string rejectionMemoNumber, string billingMemoNumber, string creditMemoNumber);

    /// <summary>
    /// Determines whether RejectionMemo linking exists for given parameters.
    /// </summary>
    /// <param name="ticketIssuingAirline">The ticket issuing airline.</param>
    /// <param name="ticketDocNumber">The ticket doc number.</param>
    /// <param name="couponNumber">The coupon number.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="correspondenceNumber">The correspondence number.</param>
    /// <returns>Count of records fetched against specific parameters.</returns>
    long IsRMLinkingExists(int ticketIssuingAirline, long ticketDocNumber, int couponNumber, Guid invoiceId, long correspondenceNumber);

    /// <summary>
    /// Gets the RM linking details.
    /// </summary>
    /// <param name="criteria">The criteria.</param>
    /// <returns></returns>
    RMLinkingResultDetails GetRMLinkingDetails(RMLinkingCriteria criteria);

    /// <summary>
    /// Get linking details for rejection memo when multiple records are found for rejected entity then as per user selection fetch data for selected memo
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria);

    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="rejectionMemoId">RejectionMemo Id</param>
    /// <param name="correspondenceId">Correspondence Id</param>
    /// <returns>RejectionMemo</returns>
    RejectionMemo Single(Guid? rejectionMemoId = null, Guid? correspondenceId = null);

    /// <summary>
    /// Inherits the RM coupon details.
    /// </summary>
    /// <param name="rejectionMemoId">The rejection memo id.</param>
    /// <returns></returns>
    string InheritRMCouponDetails(Guid rejectionMemoId);


    /// <summary>
    /// Validates the rejection memo acceptable amount difference.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <returns></returns>
    string ValidateRejectionMemoAcceptableAmountDifference(Guid invoiceId, int billingCode);

    /// <summary>
    /// Gets the form DE linking details.
    /// </summary>
    /// <param name="criteria">Sampling rejection memo linking criteria.</param>
    /// <returns></returns>
    SamplingFormFLinkingResult GetFormDELinkingDetails(SamplingRMLinkingCriteria criteria);

    /// <summary>
    /// Gets the sampling form F linking details.
    /// </summary>
    /// <param name="criteria">Sampling rejection memo linking criteria.</param>
    /// <returns></returns>
    RMLinkingResultDetails GetSamplingFormFLinkingDetails(SamplingRMLinkingCriteria criteria);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="billedMemberId"></param>
    /// <param name="billingMemberId"></param>
    /// <param name="rejectionMemoNumber"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    string GetDuplicateRejectionMemoNumbers(int billedMemberId, int billingMemberId, string rejectionMemoNumber, int billingYear, int billingMonth, int billingPeriod);

      //CMP#674-Validation of Coupon and AWB Breakdowns in Rejections
      List<InvalidRejectionMemoDetails> IsYourRejectionCouponDropped(Guid invoiceId, Guid? rejectionMemoId = null, string YourInvoiceNo = null, string YourRejectionNo = null, int YourInvoiceYear = 0, int YourInvoiceMonth = 0, int YourInvoicePeriod = 0);
  }
}
