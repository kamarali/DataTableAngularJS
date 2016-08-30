using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Data.Common
{
  public interface IExceptionSummarySearchResultRepository
  {
      IList<ExceptionSummarySearchResult> GetExceptionSummarySearch(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, BillingCategoryType billingCategoryType);

      IList<ExceptionSummarySearchResult> GetExceptionSummarySearch(ValidationErrorCorrection validationErrorCorrection, BillingCategoryType billingCategoryType);
  }
}
