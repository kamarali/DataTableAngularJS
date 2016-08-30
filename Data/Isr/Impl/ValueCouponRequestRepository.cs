using System;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.AutoBilling;

namespace Iata.IS.Data.Isr.Impl
{
  public class ValueCouponRequestRepository : Repository<Record50LiftRequest>, IValueCouponRequestRepository
  {
    /// <summary>
    /// To check the existance of requested coupon in database.
    /// </summary>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="issuingAirline"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    public int GetValueRequestCouponCount(string ticketDocumentNo, int couponNumber, string issuingAirline,int billingMemberId)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.TicketDocNumberParameterName, typeof(string)) { Value = ticketDocumentNo };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
      parameters[2] = new ObjectParameter(ValueRequestRepositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = issuingAirline };
      parameters[3] = new ObjectParameter(ValueRequestRepositoryConstants.BillingAirlineIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[4] = new ObjectParameter(ValueRequestRepositoryConstants.CouponCountParameterName, typeof(int));

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.GetValueRequestCouponCountFunctionName, parameters);

      return int.Parse(parameters[4].Value.ToString());
    }

    /// <summary>
    /// To Update the Usage Data.
    /// </summary>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="tktIssuingAirline"></param>
    /// <param name="billingMeberId"></param>
    /// <param name="couponStatusId"></param>
    /// <param name="couponStatusdescription"></param>
    /// <param name="filename"></param>
    /// <param name="responceDate"></param>
    /// <param name="inclideInIrrRepoer"></param>
    /// <returns></returns>
    public int UpdateValueRequestCoupon(string ticketDocumentNo, int couponNumber, string tktIssuingAirline, int billingMeberId, int couponStatusId, string couponStatusdescription, string filename, DateTime responceDate, int inclideInIrrRepoer)
    {
      var parameters = new ObjectParameter[10];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.TicketDocNumberParameterName, typeof(string)) { Value = ticketDocumentNo };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
      parameters[2] = new ObjectParameter(ValueRequestRepositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = tktIssuingAirline };
      parameters[3] = new ObjectParameter(ValueRequestRepositoryConstants.BillingAirlineIdParameterName, typeof(int)) { Value = billingMeberId };
      parameters[4] = new ObjectParameter(ValueRequestRepositoryConstants.CouponStatusParameterName, typeof(int)) { Value = couponStatusId };
      parameters[5] = new ObjectParameter(ValueRequestRepositoryConstants.CouponStatusDescriptionParameterName, typeof(string)) { Value = couponStatusdescription };
      parameters[6] = new ObjectParameter(ValueRequestRepositoryConstants.ResponceFileNameParameterName, typeof(string)) { Value = filename };
      parameters[7] = new ObjectParameter(ValueRequestRepositoryConstants.ResponceDateParameterName, typeof(DateTime)) { Value = responceDate };
      parameters[8] = new ObjectParameter(ValueRequestRepositoryConstants.IrrgReportParameterName, typeof(int)) { Value = inclideInIrrRepoer };
      parameters[9] = new ObjectParameter(ValueRequestRepositoryConstants.CouponUpdateResultParameterName, typeof(int));

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.UpdateValueRequestCouponFunctionName, parameters);

      return int.Parse(parameters[9].Value.ToString());
    }

    /// <summary>
    /// To check wheather responce file needs to be send to airline or not.
    /// </summary>
    /// <param name="responcefilename"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    public bool IsIsrFileUpdationRequried(string responcefilename, int billingMemberId)
    {
      var isRequried = false;
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.ResponceFileNameParameterName, typeof(string)) { Value = responcefilename };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[2] = new ObjectParameter(ValueRequestRepositoryConstants.IsrFileRequriedParameterName, typeof(int));

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.GetIsIsrFileRequriedFunctionName, parameters);

      if (int.Parse(parameters[2].Value.ToString()) > 0) 
        isRequried = true;

      return isRequried;
    }

    /// <summary>
    /// To validate Isr coupon for Duplicate entry in DB.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="ticketDocumentNo"></param>
    /// <param name="couponNumber"></param>
    /// <param name="tktIssuingAirline"></param>
    /// <param name="billingmonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="submissionMethosId"></param>
    /// <returns></returns>
    public bool IsIsrCouponDuplicate(int billingMemberId, string ticketDocumentNo, int couponNumber, string tktIssuingAirline, int billingmonth, int billingYear, int submissionMethosId)
    {
      var isRequried = false;
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.SubmissionIdParameterName, typeof(int)) { Value = submissionMethosId };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.TicketDocNumberParameterName, typeof(string)) { Value = ticketDocumentNo };
      parameters[2] = new ObjectParameter(ValueRequestRepositoryConstants.IssuingAirlineParameterName, typeof(string)) { Value = tktIssuingAirline };
      parameters[3] = new ObjectParameter(ValueRequestRepositoryConstants.CouponNumberParameterName, typeof(int)) { Value = couponNumber };
      parameters[4] = new ObjectParameter(ValueRequestRepositoryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[5] = new ObjectParameter(ValueRequestRepositoryConstants.BillingMonthParameterName, typeof(int)) { Value = billingmonth };
      parameters[6] = new ObjectParameter(ValueRequestRepositoryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[7] = new ObjectParameter(ValueRequestRepositoryConstants.IsrCouponDupParameterName, typeof(int));

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.ChkIsIsrDuplicateCouponFunctionName, parameters);

      if (int.Parse(parameters[7].Value.ToString()) > 0)
        isRequried = true;

      return isRequried;
    }
    /// <summary>
    /// To Update the prime Coupon status and AutoBill primeCoupon status and Is_included in daily report falg.
    /// </summary>
    /// <param name="unRequestedCouponIds"></param>
    /// <param name="requestedCouponIds"></param>
    public void UpdateIncludedInIRReportFlag(string unRequestedCouponIds, string requestedCouponIds)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.UnRequestedCouponIDs, typeof(string)) { Value = unRequestedCouponIds };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.RequestedCouponIDs, typeof(string)) { Value = requestedCouponIds };

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.UpdateIncludedInIRReportFlag, parameters);
    }

    /// <summary>
    /// To Update the AutoBillingCoupon Status.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="isBadFileExists"></param>
    public void UpdateAutbillingCouponDetails(string fileName, bool isBadFileExists)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(ValueRequestRepositoryConstants.AutoBillFileName, typeof(string)) { Value = fileName };
      parameters[1] = new ObjectParameter(ValueRequestRepositoryConstants.IsBadFileExists, typeof(int)) { Value = isBadFileExists ? 1 : 0 };

      ExecuteStoredProcedure(ValueRequestRepositoryConstants.UpdateABCouponStatusFunctionName, parameters);
    }
  }
}
