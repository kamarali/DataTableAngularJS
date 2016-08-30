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
  public class ExceptionSummarySearchResultRepository : Repository<InvoiceBase>, IExceptionSummarySearchResultRepository
  {
      public IList<ExceptionSummarySearchResult> GetExceptionSummarySearch(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, BillingCategoryType billingCategoryType)
    // public IList<SupportingDocSearchCriteria> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
    {
      //03.10.11
      var parameters = new ObjectParameter[12];

      parameters[0] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingYearParameterName, typeof(int)) { Value = billingYear };
      parameters[1] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingMonthParameterName, typeof(int)) { Value = billingMonth };
      parameters[2] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingPeriodParameterName, typeof(int)) { Value = period };
      parameters[3] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingMemberIdParameterName, typeof(int)) { Value = billingMemberId };
      parameters[4] = new ObjectParameter(ValidationExceptionSummaryConstants.BilledMemberIdParameterName, typeof(int)) { Value = billedMemberId };
      parameters[5] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingCategoryIdParameterName, typeof(int)) { Value = (int)billingCategoryType };
      parameters[6] = new ObjectParameter(ValidationExceptionSummaryConstants.InvoiceNoParameterName, typeof(string)) { Value = invoiceNo };
      parameters[7] = new ObjectParameter(ValidationExceptionSummaryConstants.ExceptionCodeParameterName, typeof(int)) { Value = exceptionCodeId };
      parameters[8] = new ObjectParameter(ValidationExceptionSummaryConstants.FileSubmissionDateParameterName, typeof(DateTime)) { Value = fileSubmmitDate };
      parameters[9] = new ObjectParameter(ValidationExceptionSummaryConstants.FileNameParameterName, typeof(string)) { Value = fileName };
      parameters[10] = new ObjectParameter(ValidationExceptionSummaryConstants.ChargeCategoryIdParameterName,typeof(int)) {Value = chargeCategoryId};
      parameters[11] = new ObjectParameter(ValidationExceptionSummaryConstants.IsFormCParameterName, typeof(int)) { Value = isFormC? 1 : 0 };
      var searchResult = ExecuteStoredFunction<ExceptionSummarySearchResult>(ValidationExceptionSummaryConstants.ExceptionSummarySearchFunctionName, parameters);

      return searchResult.ToList();
    }

      public IList<ExceptionSummarySearchResult> GetExceptionSummarySearch(ValidationErrorCorrection validationErrorCorrection, BillingCategoryType billingCategoryType)
      // public IList<SupportingDocSearchCriteria> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
      {
          //03.10.11
          var parameters = new ObjectParameter[12];

          parameters[0] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingYearParameterName, typeof(int)) { Value = validationErrorCorrection.BillingYear };
          parameters[1] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingMonthParameterName, typeof(int)) { Value = validationErrorCorrection.BillingMonth };
          parameters[2] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingPeriodParameterName, typeof(int)) { Value = validationErrorCorrection.BillingPeriod };
          parameters[3] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingMemberIdParameterName, typeof(int)) { Value = validationErrorCorrection.BillingMemberId };
          parameters[4] = new ObjectParameter(ValidationExceptionSummaryConstants.BilledMemberIdParameterName, typeof(int)) { Value = validationErrorCorrection.BilledMemberId };
          parameters[5] = new ObjectParameter(ValidationExceptionSummaryConstants.BillingCategoryIdParameterName, typeof(int)) { Value = (int)billingCategoryType };
          parameters[6] = new ObjectParameter(ValidationExceptionSummaryConstants.InvoiceNoParameterName, typeof(string)) { Value = validationErrorCorrection.InvoiceNumber };
          parameters[7] = new ObjectParameter(ValidationExceptionSummaryConstants.ExceptionCodeParameterName, typeof(int)) { Value = validationErrorCorrection.ExceptionCodeId };
          parameters[8] = new ObjectParameter(ValidationExceptionSummaryConstants.FileSubmissionDateParameterName, typeof(DateTime)) { Value = validationErrorCorrection.FileSubmissionDate };
          parameters[9] = new ObjectParameter(ValidationExceptionSummaryConstants.FileNameParameterName, typeof(string)) { Value = validationErrorCorrection.FileName };
          parameters[10] = new ObjectParameter(ValidationExceptionSummaryConstants.ChargeCategoryIdParameterName, typeof(int)) { Value = validationErrorCorrection.ChargeCategoryId };
          parameters[11] = new ObjectParameter(ValidationExceptionSummaryConstants.IsFormCParameterName, typeof(int)) { Value = validationErrorCorrection.IsFormC ? 1 : 0 };
          var searchResult = ExecuteStoredFunction<ExceptionSummarySearchResult>(ValidationExceptionSummaryConstants.ExceptionSummarySearchFunctionName, parameters);

          return searchResult.ToList();
      }

    internal static class ValidationExceptionSummaryConstants
    {
      public const string BillingYearParameterName = "BILLING_YEAR_I";
      public const string BillingMonthParameterName = "BILLING_MONTH_I";
      public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
      public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
      public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
      public const string BillingCategoryIdParameterName = "BILLING_CATEGORY_I";
      public const string InvoiceNoParameterName = "INVOICE_NO_I";
      public const string ExceptionCodeParameterName = "EXCEPTION_CODE_ID_I";
      public const string FileSubmissionDateParameterName = "FILE_SUBMISSION_DATE_I";
      public const string FileNameParameterName = "FILE_NAME_I";
      public const string ChargeCategoryIdParameterName = "CHARGE_CATEGORY_ID_I";
      public const string IsFormCParameterName = "IS_FORM_C_I";
      public const string ExceptionSummarySearchFunctionName = "GetExceptionSummarySearch";
    }
  }
}
