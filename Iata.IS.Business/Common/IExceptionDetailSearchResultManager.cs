using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common
{
  public interface IExceptionDetailSearchResultManager
  {
    List<ExceptionDetailsSearchResult> GetExceptionDetailData(Guid exceptionSummaryId, int billingCategory);

      int CheckForValidation(string exceptionCode, string validationType, string masterTableName,
                             string masterColumnName, string fieldValue, string masterGrpColumn, string masterGrpId);
      //SCP252342 - SRM: ICH invoice in ready for billing status add detailLastUpdatedOn 
      int UpdateCorrectedData(string fileName, int isBatchUpdated, string newValue, Guid exceptionDetailId,
                              string errorLevel, string childTableName, string columnName, string primaryColumn,
                              int billingCategory, int userId, string fieldValueType, DateTime detLastUpdatedOn);

      int GetBatchUpdateCount(Guid exceptionSummaryId, string oldFieldValue, string errorLevels);

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
    /// <param name="UpdateLinkErrorFunctionrejectionStage"></param>
    /// <param name="yourBillingMemberId"></param>
    /// <param name="yourBilledMemberId"></param>
    /// <returns>int</returns>
    int ValidateCgoLinkingFunction(Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod,
                                   int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber,
                                   int bmCMIndicator, int rejectionStage, int yourBillingMemberId,
                                   int yourBilledMemberId);

    /// <summary>
    /// Update Link Erroring
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
    /// <param name="bmCmIndicator"></param>
    /// <param name="corrReferenceNo"></param>
    /// <param name="linkingTypeId"></param>
    /// <param name="userId"></param>
    /// <param name="provInvoiceNo"></param>
    /// <param name="batchSeqNo"></param>
    /// <param name="batchRecordSeq"></param>
    /// <param name="fimCouponNo"></param>
    /// <param name="fimBmCmNo"></param>
    /// <param name="detLastUpdatedOn">SCP252342 - SRM: ICH invoice in ready for billing status</param>
    /// <param name="updatedExRate"> Updated Exchange Rate (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <param name="updatedClearanceAmt"> Updated Clearance Amount (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <returns></returns>
    int UpdateLinkErrorFunction(BillingCategoryType billingCategoryType, Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod,
                                int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int yourStage,
                                int bmCmIndicator, long corrReferenceNo, int linkingTypeId, int userId, string provInvoiceNo, int batchSeqNo,
                                int batchRecordSeq, int? fimCouponNo, string fimBmCmNo, DateTime detLastUpdatedOn,
                                decimal? updatedExRate = null, decimal? updatedClearanceAmt = null);

    /// <summary>
    /// To check against reqular expression
    /// </summary>
    /// <param name="reqExpression"></param>
    /// <param name="newValue"></param>
    /// <returns></returns>
    int ValidateReqularExpressionValue(string reqExpression, string newValue);

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
    int ValidatePaxLinkingFunction(Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod,
                                   int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber,
                                   int bmCMIndicator, int rejectionStage, int yourBillingMemberId,
                                   int yourBilledMemberId);

    /// <summary>
    /// To validate Pax Sampling Linking error
    /// </summary>
    /// <param name="exceptionDetailId">exceptionDetailId</param>
    /// <param name="provInvoiceNo">provInvoiceNo</param>
    /// <param name="batchSeqNo">batchSeqNo</param>
    /// <param name="batchRecordSeq">batchRecordSeq</param>
    /// <param name="isFormC">isFormC</param>
    /// <returns>int</returns>
    int ValidatePaxSamplingLinkingFunction(Guid exceptionDetailId, string provInvoiceNo, int batchSeqNo,
                                           int batchRecordSeq, bool isFormC);
  }
}
