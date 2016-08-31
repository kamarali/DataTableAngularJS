using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Iata.IS.Data.Common;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common.Impl
{
  public class ExceptionDetailSearchResultManager : IExceptionDetailSearchResultManager
  {
    public IExceptionDetailSearchResultRepository ValidationExceptionrepository { get; set; }

    public List<ExceptionDetailsSearchResult> GetExceptionDetailData(Guid exceptionSummaryId, int billingCategory)
    {
      return ValidationExceptionrepository.GetExceptionDetailData(exceptionSummaryId, billingCategory);
    }

    public int CheckForValidation(string exceptionCode, string validationType, string masterTableName, string masterColumnName, string fieldValue, string masterGrpColumn, string masterGrpId)
    {
      return ValidationExceptionrepository.CheckForValidation(exceptionCode, validationType, masterTableName,
                                                       masterColumnName, fieldValue, masterGrpColumn, masterGrpId);
    }

    public int UpdateCorrectedData(string fileName, int isBatchUpdated, string newValue, Guid exceptionDetailId, string errorLevel, string childTableName, string columnName, string primaryColumn, int billingCategory, int userId, string fieldValueType, DateTime detLastUpdatedOn)
    {
      return ValidationExceptionrepository.UpdateCorrectedData(fileName, isBatchUpdated, newValue, exceptionDetailId,
                           errorLevel, childTableName, columnName, primaryColumn,
                           billingCategory, userId, fieldValueType, detLastUpdatedOn);
    }

    public int GetBatchUpdateCount(Guid exceptionSummaryId, string oldFieldValue, string errorLevels)
    {
      return ValidationExceptionrepository.GetBatchUpdateCount(exceptionSummaryId, oldFieldValue, errorLevels);
    }

    /// <summary>
    /// To validate Cargo RM Linking error
    /// </summary>
    /// <param name="exceptionDetailId">exceptionDetailId</param>
    /// <param name="yourBillingMonth"></param>
    /// <param name="yourBillingYear"></param>
    /// <param name="yourInvoiceNo"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <param name="yourRmNumber"></param>
    /// <param name="yourBmCmNumber"></param>
    /// <param name="bmCMIndicator"></param>
    /// <param name="rejectionStage"></param>
    /// <param name="yourBillingMemberId"></param>
    /// <param name="yourBilledMemberId"></param>
    /// <returns>int</returns>
    public int ValidateCgoLinkingFunction(Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int bmCMIndicator, int rejectionStage, int yourBillingMemberId, int yourBilledMemberId)
    {
      return ValidationExceptionrepository.ValidateCgoLinkingFunction(exceptionDetailId, yourInvoiceNo, yourBillingPeriod, yourBillingMonth, yourBillingYear, yourRmNumber, yourBmCmNumber, bmCMIndicator, rejectionStage, yourBillingMemberId, yourBilledMemberId);
    }


    /// <summary>
    /// To validate Pax RM Linking error
    /// </summary>
    /// <param name="exceptionDetailId">exceptionDetailId</param>
    /// <param name="yourBillingMonth"></param>
    /// <param name="yourBillingYear"></param>
    /// <param name="yourInvoiceNo"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <param name="yourRmNumber"></param>
    /// <param name="yourBmCmNumber"></param>
    /// <param name="bmCMIndicator"></param>
    /// <param name="rejectionStage"></param>
    /// <param name="yourBillingMemberId"></param>
    /// <param name="yourBilledMemberId"></param>
    /// <returns>int</returns>
    public int ValidatePaxLinkingFunction(Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int bmCMIndicator, int rejectionStage, int yourBillingMemberId, int yourBilledMemberId)
    {
      return ValidationExceptionrepository.ValidatePaxLinkingFunction(exceptionDetailId, yourInvoiceNo, yourBillingPeriod, yourBillingMonth, yourBillingYear, yourRmNumber, yourBmCmNumber, bmCMIndicator, rejectionStage, yourBillingMemberId, yourBilledMemberId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="billingCategoryType"></param>
    /// <param name="exceptionDetailId"></param>
    /// <param name="yourInvoiceNo"></param>
    /// <param name="yourBillingPeriod"></param>
    /// <param name="yourBillingMonth"></param>
    /// <param name="yourBillingYear"></param>
    /// <param name="yourRmNumber"></param>
    /// <param name="yourBmCmNumber"></param>
    /// <param name="yourStage"></param>
    /// <param name="bmCMIndicator"></param>
    /// <param name="corrReferenceNo"></param>
    /// <param name="linkingTypeId"></param>
    /// <param name="userId"></param>
    /// <param name="provInvoiceNo"></param>
    /// <param name="batchSeqNo"></param>
    /// <param name="batchRecordSeq"></param>
    /// <param name="fimCouponNo"></param>
    /// <param name="fimBmCmNo"></param>
    /// <param name="detLastUpdatedOn"></param>
    /// <param name="updatedExRate"> Updated Exchange Rate (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <param name="updatedClearanceAmt"> Updated Clearance Amount (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <returns></returns>
    public int UpdateLinkErrorFunction(BillingCategoryType billingCategoryType, Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth,
                                       int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int yourStage, int bmCMIndicator, long corrReferenceNo,
                                       int linkingTypeId, int userId, string provInvoiceNo, int batchSeqNo, int batchRecordSeq, int? fimCouponNo, string fimBmCmNo,
                                       DateTime detLastUpdatedOn, decimal? updatedExRate = null, decimal? updatedClearanceAmt = null)
    {
      return ValidationExceptionrepository.UpdateLinkErrorFunction(billingCategoryType, exceptionDetailId, yourInvoiceNo, yourBillingPeriod, yourBillingMonth,
                                                                   yourBillingYear, yourRmNumber, yourBmCmNumber, yourStage, bmCMIndicator, corrReferenceNo,
                                                                   linkingTypeId, userId, provInvoiceNo, batchSeqNo, batchRecordSeq, fimCouponNo, fimBmCmNo,
                                                                   detLastUpdatedOn, updatedExRate, updatedClearanceAmt);

    }

    /// <summary>
    /// To validate Pax Sampling Linking error
    /// </summary>
    /// <param name="exceptionDetailId">exceptionDetailId</param>
    /// <param name="provInvoiceNo">provInvoiceNo</param>
    /// <param name="batchSeqNo">batchSeqNo</param>
    /// <param name="batchRecordSeq">batchRecordSeq</param>
    /// <param name="isFormC">isFormC</param>
    /// <returns>int</returns>
    public int ValidatePaxSamplingLinkingFunction(Guid exceptionDetailId, string provInvoiceNo, int batchSeqNo,
                                           int batchRecordSeq, bool isFormC)
    {
      return ValidationExceptionrepository.ValidatePaxSamplingLinkingFunction(exceptionDetailId, provInvoiceNo,
                                                                              batchSeqNo, batchRecordSeq, isFormC);
    }

    public int ValidateReqularExpressionValue(string reqExpression, string newValue)
    {
      newValue = newValue.ToUpper();
      var regex = new Regex(reqExpression);
      return regex.IsMatch(newValue) ? 1 : 0;
    }
  }
}
