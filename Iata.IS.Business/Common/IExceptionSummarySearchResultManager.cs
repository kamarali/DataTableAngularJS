using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common
{
  public interface IExceptionSummarySearchResultManager
  {
      IList<ExceptionSummarySearchResult> GetExceptionSummarySearchResult(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, BillingCategoryType billingCategoryType);

      IList<ExceptionSummarySearchResult> GetExceptionSummarySearchResult(ValidationErrorCorrection validationErrorCorrection, BillingCategoryType billingCategoryType);
  }
}
