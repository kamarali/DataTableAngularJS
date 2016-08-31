using System;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.Pax
{
  public interface ISamplingFormXfManager : ISamplingRejectionManager
  {
    RMLinkingResultDetails GetLinkedFormFDetails(SamplingRMLinkingCriteria rmLinkingCriteria);
    RMLinkingResultDetails GetLinkedMemoAmountDetails(RMLinkingCriteria criteria);
    /// <summary>
    /// Get the sampling constant of the linked form F
    /// </summary>
    /// <param name="billingMemberId"> Billing Member ID.</param>
    /// <param name="billedMemberId">Billed Member ID.</param>
    /// <param name="provisionalBillingMonth">Provisional billing Month</param>
    /// <param name="provisionalBillingYear">Provisional billing Year.</param>
    /// <returns></returns>
    SamplingConstantDetails GetLinkedFormFSamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear);
    RMLinkedCouponDetails GetSamplingCouponBreakdownRecordDetails(string issuingAirline,
                                                                                  int couponNo,
                                                                                  long ticketDocNo,
                                                                                  Guid rmId,
                                                                                  int billingMemberId,
                                                                                  int billedMemberId);

    RMLinkedCouponDetails GetRMCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId);
  }
}
