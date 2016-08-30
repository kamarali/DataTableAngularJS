using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using System.Data.Objects;
using Iata.IS.Model.ValueConfirmation;

namespace Iata.IS.Data.ValueConfirmation.Impl
{
  class RequestValueConfirmationRepository : Repository<InvoiceBase>, IRequestValueConfirmationRepository 
  {
      public List<RequestVCFCoupon> GetRequestValueConfirmationData(BillingPeriod ICHBillingPeriod, BillingPeriod ACHBillingPeriod, int maxAllowedCouponsPerVCF, int RegenerationFlag, Guid RequestId)
    {
      var parameters = new ObjectParameter[9];
      parameters[0] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ICHBillingYear, typeof(Int32)) { Value = ICHBillingPeriod.Year };
      parameters[1] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ICHBillingMonth, typeof(Int32)) { Value = ICHBillingPeriod.Month };
      parameters[2] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ICHBillingPeriod, typeof(Int32)) { Value = ICHBillingPeriod.Period };
      parameters[3] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ACHBillingYear, typeof(Int32)) { Value = ACHBillingPeriod.Year };
      parameters[4] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ACHBillingMonth, typeof(Int32)) { Value = ACHBillingPeriod.Month };
      parameters[5] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ACHBillingPeriod, typeof(Int32)) { Value = ACHBillingPeriod.Period };
      parameters[6] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.MaxAllowedCouponsPerVCF, typeof(Int32)) { Value = maxAllowedCouponsPerVCF };
      parameters[7] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.RequestId, typeof(Int32)) { Value = RequestId };
      parameters[8] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.RegenerationFlag, typeof(Int32)) { Value = RegenerationFlag };
      var list = ExecuteStoredFunction<RequestVCFCoupon>(RequestValueConfirmationRepositoryConstants.GetRequestValueConfirmationData, parameters);
      return list.ToList();
    }


    public void UpdateInvoicesAndCouponsForRequestVCF(string invoiceIds, string couponIds, string fileName,string fileLocation, int ATPCOMemberId,string VCFKey,int expectedResponseTime)
    {
      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.InvoiceIds, typeof(string)) { Value = invoiceIds };
      parameters[1] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.CouponIds, typeof(string)) { Value = couponIds };
      parameters[2] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.FileName, typeof(string)) { Value = fileName };
      parameters[3] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.FileLocation, typeof(string)) { Value = fileLocation };
      parameters[4] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ATPCOMemberId, typeof(Int32)) { Value = ATPCOMemberId };
      parameters[5] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.VCFKey, typeof(string)) { Value = VCFKey };
      parameters[6] = new ObjectParameter(RequestValueConfirmationRepositoryConstants.ExpectResponseDateTime, typeof(Int32)) { Value = expectedResponseTime };

      ExecuteStoredProcedure(RequestValueConfirmationRepositoryConstants.UpdateInvoicesAndCouponsForRequestVCF, parameters);
    }


  }
}
