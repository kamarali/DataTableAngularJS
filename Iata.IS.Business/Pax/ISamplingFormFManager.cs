using System;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Business.Pax
{
  public interface ISamplingFormFManager : ISamplingRejectionManager
  {
    /// <summary>
    /// Used to fetch the linked Form DE sampling constant to display at Form F header.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="billedMemberId"></param>
    /// <param name="provisionalBillingMonth"></param>
    /// <param name="provisionalBillingYear"></param>
    /// <returns></returns>
    SamplingConstantDetails GetLinkedFormDESamplingConstant(int billingMemberId, int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear);

    RMLinkedCouponDetails GetSamplingCouponBreakdownSingleRecordDetails(Guid couponId, Guid rejectionMemoId, int billingMemberId, int billedMemberId);
    
    bool IsMemberMigrated(int billedMemberId, int provisionalBillingMonth, int provisionalBillingYear);

    SamplingFormFLinkingResult GetLinkedFormDEDetails(SamplingRMLinkingCriteria rmLinkingCriteria);

    RMLinkedCouponDetails GetSamplingCouponBreakdownRecordDetails(string issuingAirline,
                                                                                  int couponNo,
                                                                                  long ticketDocNo,
                                                                                  Guid rmId,
                                                                                  int billingMemberId,
                                                                                  int billedMemberId);
  }
}