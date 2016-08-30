using System;
using System.Collections.Generic;
using Iata.IS.Model.ValueConfirmation;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.ValueConfirmation
{
  public interface IRequestValueConfirmationRepository
  {
      List<RequestVCFCoupon> GetRequestValueConfirmationData(BillingPeriod ICHBillingPeriod, BillingPeriod ACHBillingPeriod, int MaxAllowedCouponsPerVCF, int RegenerationFlag, Guid RequestId);

    void UpdateInvoicesAndCouponsForRequestVCF(string invoiceIds, string couponIds, string fileName, string fileLocation, int ATPCOMemberId,string VCFKey,int ExpectedResponseTime);
  }
}
