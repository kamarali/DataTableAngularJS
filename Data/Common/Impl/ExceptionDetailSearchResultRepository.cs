using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Data.Common.Impl
{
  public class ExceptionDetailSearchResultRepository : Repository<InvoiceBase>, IExceptionDetailSearchResultRepository
  {
    #region constants

    public const string exception_Summary_Id = "EXCEPTION_SUMMARY_I";
    public const string billing_Category = "BILLING_CAT";
    public const string GetExceptiondetails = "GetExceptiondetails";

    public const string exception_Code = "EXCEPTION_CODE_I";
    public const string validation_Type = "VALIDATION_TYPE_I";
    public const string master_Table = "MASTER_TABLE_I";
    public const string master_Column = "MASTER_COLUMN_NAME_I";
    public const string master_Grp_Column = "MASTER_GRP_COL_NAME_I";
    public const string master_Grp_Id = "MASTER_GROUP_ID_I";
    public const string field_Value = "FIELD_VALUE_I";
    public const string CheckForValidate = "ValidateUpdateErrCorrection";


    public const string updateFileName = "FILE_NAME_I";
    public const string updateBatchUpdate = "IS_BATCH_UPDATE_I";
    public const string updateFieldValue = "FIELD_VALUE_I";
    public const string updateDetailId = "EXCP_DETAIL_ID_I";
    public const string updateErrorLevel = "ERROR_LEVELS_I";
    public const string updateChildTable = "CHILD_TABLE_NAMES_I";
    public const string updateColumnName = "COLUMN_NAMES_I";
    public const string updatePimaryColumn = "PRIMARY_COL_NAMES_I";
    public const string updateBillingCat = "BILLING_CAT_ID_I";
    public const string updateUserId = "USER_ID_I";
    public const string fieldTypeName = "FIELD_TYPE_I";
    public const string updatefunction = "UpdateValidationErrorFunction";

    public const string EXCP_SUMMARY_ID_I = "EXCP_SUMMARY_ID_I";
    public const string EXCP_DETAIL_ID_I = "EXCP_DETAIL_ID_I";
    public const string OLD_FIELD_VALUE_I = "OLD_FIELD_VALUE_I";
    public const string ERROR_LEVELS_I = "ERROR_LEVELS_I";
    public const string R_RESULT_O = "R_RESULT_O";
    public const string ProcGetBatchUpdateCount = "ProcGetBatchUpdateCount";

    public const string YOUR_INVOICE_NUMBER_I = "YOUR_INVOICE_NUMBER_I";

    public const string ProcValidateCgoLinkingFunction = "ValidateCgoLinkingFunction";
    public const string ProcValidatePaxLinkingFunction = "ValidatePaxLinkingFunction";
    public const string ProcValidatePaxSamplingLinkingFunction = "ValidatePaxSamplingLinkingFunction";

    public const string YOUR_INVOICE_NO_I = "YOUR_INVOICE_NO_I";
    public const string YOUR_BILLING_PERIOD_I = "YOUR_BILLING_PERIOD_I";
    public const string YOUR_BILLING_MONTH_I = "YOUR_BILLING_MONTH_I";
    public const string YOUR_BILLING_YEAR_I = "YOUR_BILLING_YEAR_I";
    public const string YOUR_RM_NO_I = "YOUR_RM_NO_I";
    public const string YOUR_REJECTION_STAGE_I = "YOUR_REJECTION_STAGE_I";
    public const string YOUR_BM_CM_NO_I = "YOUR_BM_CM_NO_I";
    public const string YOUR_BM_CM_NUMBER_I = "YOUR_BM_CM_NUMBER_I";
    public const string YOUR_RM_NUMBER_I = "YOUR_RM_NUMBER_I";
    public const string YOUR_BM_CM_INDICATOR_I = "YOUR_BM_CM_INDICATOR_I";
    public const string YOUR_BILLING_MEMBER_ID_I = "YOUR_BILLING_MEMBER_ID_I";
    public const string YOUR_BILLED_MEMBER_ID_I = "YOUR_BILLED_MEMBER_ID_I";
    public const string BM_CM_INDICATOR_I = "BM_CM_INDICATOR_I";
    public const string CORRESPONDENCE_REF_NO_I = "CORRESPONDENCE_REF_NO_I";
    public const string LINKING_TYPE_ID_I = "LINKING_TYPE_ID_I";
    public const string USER_ID_I = "USER_ID_I";
    public const string BILLING_CAT_ID_I = "BILLING_CAT_ID_I";
    public const string ProcUpdateLinkErrorFunction = "UpdateLinkErrorFunction";
    public const string ProcUpdateLinkErrorPaxFunction = "UpdatePaxLinkErrorFunction";

    public const string PROV_INVOICE_NO_I = "PROV_INVOICE_NO_I";
    public const string BATCH_SEQ_NO_I = "BATCH_SEQ_NO_I";
    public const string BATCH_RECORD_SEQ_I = "BATCH_RECORD_SEQ_I";
    public const string FIM_COUPON_NO_I = "FIM_COUPON_NO_I";
    public const string FIM_BM_CM_NO_I = "FIM_BM_CM_NO_I";
    //SCP252342 - SRM: ICH invoice in ready for billing status
    public const string DETAIL_LAST_UPDATED_ON = "LAST_UPDATED_ON_I";

    // SCP321993: FW ICH Settlement Error - SIS Production
    public const string UpdatedExRateParameter = "UPDATED_EX_RATE_I";
    public const string UpdatedClearanceAmtParameter = "UPDATED_CLEARANCE_AMT_I";

    public const string PROV_INVOICE_NUMBER_I = "PROV_INVOICE_NUMBER_I";
    public const string BATCH_NO_I = "BATCH_NO_I";
    public const string RECORD_SEQ_NO_I = "RECORD_SEQ_NO_I";
    public const string IS_FORM_C_I = "IS_FORM_C_I";

    #endregion

    public List<ExceptionDetailsSearchResult> GetExceptionDetailData(Guid exceptionSummaryId, int billingCategory)
    {
      var parameters = new ObjectParameter[2];

      parameters[0] = new ObjectParameter(exception_Summary_Id, exceptionSummaryId);
      parameters[1] = new ObjectParameter(billing_Category, billingCategory);

      var list = ExecuteStoredFunction<ExceptionDetailsSearchResult>(GetExceptiondetails, parameters);

      return list.ToList();
    }

    public int CheckForValidation(string exceptionCode, string validationType, string masterTableName, string masterColumnName, string fieldValue, string masterGrpColumn, string masterGrpId)
    {
      var parameters = new ObjectParameter[8];
      parameters[0] = new ObjectParameter(exception_Code, exceptionCode);
      parameters[1] = new ObjectParameter(validation_Type, validationType);
      parameters[2] = new ObjectParameter(master_Table, masterTableName);
      parameters[3] = new ObjectParameter(master_Column, masterColumnName);
      parameters[4] = new ObjectParameter(master_Grp_Column, masterGrpColumn ?? string.Empty);
      parameters[5] = new ObjectParameter(master_Grp_Id, masterGrpId ?? string.Empty);
      parameters[6] = new ObjectParameter(field_Value, fieldValue);
      parameters[7] = new ObjectParameter("R_RESULT_O", typeof(int));

      ExecuteStoredProcedure(CheckForValidate, parameters);
      return Convert.ToInt32(parameters[7].Value);
    }

    public int UpdateCorrectedData(string fileName, int isBatchUpdated, string newValue, Guid exceptionDetailId,
                        string errorLevel, string childTableName, string columnName, string primaryColumn,
                        int billingCategory,int userId,string fieldValueType, DateTime detLastUpdatedOn)
    {
      var parameters = new ObjectParameter[13];
      parameters[0] = new ObjectParameter(updateFileName, fileName);
      parameters[1] = new ObjectParameter(updateBatchUpdate, isBatchUpdated);
      parameters[2] = new ObjectParameter(updateFieldValue, newValue);
      parameters[3] = new ObjectParameter(updateDetailId, exceptionDetailId);
      parameters[4] = new ObjectParameter(updateErrorLevel, errorLevel);
      parameters[5] = new ObjectParameter(updateChildTable, childTableName);
      parameters[6] = new ObjectParameter(updateColumnName, columnName);
      parameters[7] = new ObjectParameter(updatePimaryColumn, primaryColumn);
      parameters[8] = new ObjectParameter(updateBillingCat, billingCategory);
      parameters[9] = new ObjectParameter(updateUserId, userId);
      parameters[10] = new ObjectParameter(fieldTypeName, fieldValueType != null ? fieldValueType.ToUpper() : string.Empty);
      //SCP252342 - SRM: ICH invoice in ready for billing status add detailLastUpdatedOn 
      parameters[11] = new ObjectParameter(DETAIL_LAST_UPDATED_ON, detLastUpdatedOn);
      parameters[12] = new ObjectParameter("R_RESULT_O", typeof(int));

      ExecuteStoredProcedure(updatefunction, parameters);

      return Convert.ToInt32(parameters[12].Value);
    }

    /// <summary>
    /// This will return the no of rows affecting the Batch update  
    /// </summary>
    /// <param name="exceptionSummaryId">exceptionSummaryId</param>
    /// <param name="oldFieldValue">oldFieldValue</param>
    /// <param name="errorLevels">errorLevels: invoice,LineItem (Comma seperated list of error levels)</param>
    /// <returns>rows affecting the Batch update  </returns>
    public int GetBatchUpdateCount(Guid exceptionSummaryId, string oldFieldValue, string errorLevels)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter(EXCP_SUMMARY_ID_I, exceptionSummaryId);
      parameters[1] = new ObjectParameter(OLD_FIELD_VALUE_I, oldFieldValue);
      parameters[2] = new ObjectParameter(ERROR_LEVELS_I, errorLevels);
      parameters[3] = new ObjectParameter(R_RESULT_O, typeof(int));

      ExecuteStoredProcedure(ProcGetBatchUpdateCount, parameters);
      return Convert.ToInt32(parameters[3].Value);
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
    public int ValidateCgoLinkingFunction(Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int bmCMIndicator,int rejectionStage,int yourBillingMemberId,int yourBilledMemberId)
    {
      var parameters = new ObjectParameter[12];
      parameters[0] = new ObjectParameter(EXCP_DETAIL_ID_I, exceptionDetailId);
      parameters[1] = new ObjectParameter(YOUR_INVOICE_NUMBER_I, yourInvoiceNo ?? string.Empty);
      parameters[2] = new ObjectParameter(YOUR_BILLING_MONTH_I, yourBillingMonth);
      parameters[3] = new ObjectParameter(YOUR_BILLING_YEAR_I, yourBillingYear);
      parameters[4] = new ObjectParameter(YOUR_BILLING_PERIOD_I, yourBillingPeriod);
      parameters[5] = new ObjectParameter(YOUR_BM_CM_NUMBER_I, yourBmCmNumber ?? string.Empty);
      parameters[6] = new ObjectParameter(YOUR_RM_NUMBER_I, yourRmNumber ?? string.Empty);
      parameters[7] = new ObjectParameter(YOUR_BM_CM_INDICATOR_I, bmCMIndicator);
      parameters[8] = new ObjectParameter(YOUR_REJECTION_STAGE_I, rejectionStage);
      parameters[9] = new ObjectParameter(YOUR_BILLING_MEMBER_ID_I, yourBillingMemberId);
      parameters[10] = new ObjectParameter(YOUR_BILLED_MEMBER_ID_I, yourBilledMemberId);
      parameters[11] = new ObjectParameter(R_RESULT_O, typeof(int));

      ExecuteStoredProcedure(ProcValidateCgoLinkingFunction, parameters);
      return Convert.ToInt32(parameters[11].Value);
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
      var parameters = new ObjectParameter[12];
      parameters[0] = new ObjectParameter(EXCP_DETAIL_ID_I, exceptionDetailId);
      parameters[1] = new ObjectParameter(YOUR_INVOICE_NUMBER_I, yourInvoiceNo ?? string.Empty);
      parameters[2] = new ObjectParameter(YOUR_BILLING_MONTH_I, yourBillingMonth);
      parameters[3] = new ObjectParameter(YOUR_BILLING_YEAR_I, yourBillingYear);
      parameters[4] = new ObjectParameter(YOUR_BILLING_PERIOD_I, yourBillingPeriod);
      parameters[5] = new ObjectParameter(YOUR_BM_CM_NUMBER_I, yourBmCmNumber ?? string.Empty);
      parameters[6] = new ObjectParameter(YOUR_RM_NUMBER_I, yourRmNumber ?? string.Empty);
      parameters[7] = new ObjectParameter(YOUR_BM_CM_INDICATOR_I, bmCMIndicator);
      parameters[8] = new ObjectParameter(YOUR_REJECTION_STAGE_I, rejectionStage);
      parameters[9] = new ObjectParameter(YOUR_BILLING_MEMBER_ID_I, yourBillingMemberId);
      parameters[10] = new ObjectParameter(YOUR_BILLED_MEMBER_ID_I, yourBilledMemberId);
      parameters[11] = new ObjectParameter(R_RESULT_O, typeof(int));

      ExecuteStoredProcedure(ProcValidatePaxLinkingFunction, parameters);
      return Convert.ToInt32(parameters[11].Value);
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
    public int ValidatePaxSamplingLinkingFunction(Guid exceptionDetailId, string provInvoiceNo, int batchSeqNo, int batchRecordSeq, bool isFormC)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(EXCP_DETAIL_ID_I, exceptionDetailId);
      parameters[1] = new ObjectParameter(PROV_INVOICE_NUMBER_I, provInvoiceNo ?? string.Empty);
      parameters[2] = new ObjectParameter(BATCH_NO_I, batchSeqNo);
      parameters[3] = new ObjectParameter(RECORD_SEQ_NO_I, batchRecordSeq);
      parameters[4] = new ObjectParameter(IS_FORM_C_I, isFormC ? 1 : 0);
      parameters[5] = new ObjectParameter(R_RESULT_O, typeof(int));

      ExecuteStoredProcedure(ProcValidatePaxSamplingLinkingFunction, parameters);
      return Convert.ToInt32(parameters[5].Value);
    }

    /// <summary>
    /// To update all Cargo linking errors
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
    public int UpdateLinkErrorFunction(BillingCategoryType billingCategoryType, Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth,
                                       int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int yourStage, int bmCmIndicator, long corrReferenceNo,
                                       int linkingTypeId, int userId, string provInvoiceNo, int batchSeqNo, int batchRecordSeq, int? fimCouponNo, string fimBmCmNo,
                                       DateTime detLastUpdatedOn, decimal? updatedExRate = null, decimal? updatedClearanceAmt = null)
    {
      var parameters = new ObjectParameter[22];
      parameters[0] = new ObjectParameter(EXCP_DETAIL_ID_I, exceptionDetailId);
      parameters[1] = new ObjectParameter(YOUR_INVOICE_NO_I, yourInvoiceNo ?? string.Empty);
      parameters[2] = new ObjectParameter(YOUR_BILLING_PERIOD_I, yourBillingPeriod);
      parameters[3] = new ObjectParameter(YOUR_BILLING_MONTH_I, yourBillingMonth);
      parameters[4] = new ObjectParameter(YOUR_BILLING_YEAR_I, yourBillingYear);
      parameters[5] = new ObjectParameter(YOUR_RM_NO_I, yourRmNumber ?? string.Empty);
      parameters[6] = new ObjectParameter(YOUR_BM_CM_NO_I, yourBmCmNumber ?? string.Empty);
      parameters[7] = new ObjectParameter(YOUR_REJECTION_STAGE_I, yourStage);
      parameters[8] = new ObjectParameter(BM_CM_INDICATOR_I, bmCmIndicator);
      parameters[9] = new ObjectParameter(CORRESPONDENCE_REF_NO_I, corrReferenceNo);
      parameters[10] = new ObjectParameter(LINKING_TYPE_ID_I, linkingTypeId);
      parameters[11] = new ObjectParameter(USER_ID_I, userId);
      parameters[12] = new ObjectParameter(BILLING_CAT_ID_I, (int)billingCategoryType);
      parameters[13] = new ObjectParameter(PROV_INVOICE_NO_I, provInvoiceNo ?? string.Empty);
      parameters[14] = new ObjectParameter(BATCH_SEQ_NO_I, batchSeqNo);
      parameters[15] = new ObjectParameter(BATCH_RECORD_SEQ_I, batchRecordSeq);
      parameters[16] = new ObjectParameter(FIM_COUPON_NO_I, typeof(int?)) { Value = fimCouponNo };
      parameters[17] = new ObjectParameter(FIM_BM_CM_NO_I, fimBmCmNo ?? string.Empty);
      parameters[18] = new ObjectParameter(DETAIL_LAST_UPDATED_ON, detLastUpdatedOn);
      parameters[19] = new ObjectParameter(UpdatedExRateParameter, typeof (decimal?)) {Value = updatedExRate};
      parameters[20] = new ObjectParameter(UpdatedClearanceAmtParameter, typeof (decimal?)) {Value = updatedClearanceAmt};

      parameters[21] = new ObjectParameter(R_RESULT_O, typeof(int));
      ExecuteStoredProcedure(ProcUpdateLinkErrorFunction, parameters);
      return Convert.ToInt32(parameters[21].Value);
    }

    /// <summary>
    /// Update Pax linking error
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
    /// <returns></returns>
  
    public int UpdateLinkPaxErrorFunction(BillingCategoryType billingCategoryType, Guid exceptionDetailId, string yourInvoiceNo, int yourBillingPeriod, int yourBillingMonth, int yourBillingYear, string yourRmNumber, string yourBmCmNumber, int yourStage, int bmCMIndicator, long corrReferenceNo, int linkingTypeId, int userId, string provInvoiceNo, int batchSeqNo, int batchRecordSeq,int fimCouponNo,string fimBmCmNo)
    {
        var parameters = new ObjectParameter[19];
        parameters[0] = new ObjectParameter(EXCP_DETAIL_ID_I, exceptionDetailId);
        parameters[1] = new ObjectParameter(YOUR_INVOICE_NO_I, yourInvoiceNo ?? string.Empty);
        parameters[2] = new ObjectParameter(YOUR_BILLING_PERIOD_I, yourBillingPeriod);
        parameters[3] = new ObjectParameter(YOUR_BILLING_MONTH_I, yourBillingMonth);
        parameters[4] = new ObjectParameter(YOUR_BILLING_YEAR_I, yourBillingYear);
        parameters[5] = new ObjectParameter(YOUR_RM_NO_I, yourRmNumber ?? string.Empty);
        parameters[6] = new ObjectParameter(YOUR_BM_CM_NO_I, yourBmCmNumber ?? string.Empty);
        parameters[7] = new ObjectParameter(YOUR_REJECTION_STAGE_I, yourStage);
        parameters[8] = new ObjectParameter(BM_CM_INDICATOR_I, bmCMIndicator);
        parameters[9] = new ObjectParameter(CORRESPONDENCE_REF_NO_I, corrReferenceNo);
        parameters[10] = new ObjectParameter(LINKING_TYPE_ID_I, linkingTypeId);
        parameters[11] = new ObjectParameter(USER_ID_I, userId);
        parameters[12] = new ObjectParameter(BILLING_CAT_ID_I, (int)billingCategoryType);

        parameters[13] = new ObjectParameter(PROV_INVOICE_NO_I, provInvoiceNo);
        parameters[14] = new ObjectParameter(BATCH_SEQ_NO_I, batchSeqNo);
        parameters[15] = new ObjectParameter(BATCH_RECORD_SEQ_I, batchRecordSeq);
        parameters[16] = new ObjectParameter(FIM_COUPON_NO_I, fimCouponNo);
        parameters[17] = new ObjectParameter(FIM_BM_CM_NO_I, fimBmCmNo);


        parameters[18] = new ObjectParameter(R_RESULT_O, typeof(int));
        ExecuteStoredProcedure(ProcUpdateLinkErrorPaxFunction, parameters);
        return Convert.ToInt32(parameters[18].Value);
    }    
  }
}
