using System;
using System.Collections.Generic;
using Iata.IS.Data.Common;
using Iata.IS.Model;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common.Impl
{
  public class ExceptionSummarySearchResultManager : IExceptionSummarySearchResultManager
  {
      /// <summary>
      /// Gets or sets the correspondence repository.
      /// </summary>
      /// <value>The country repository.</value>
      public IExceptionSummarySearchResultRepository ValidationExceptionSummaryRepository;

    public ExceptionSummarySearchResultManager(IExceptionSummarySearchResultRepository _ValidationExceptionSummaryRepository)
    {
        ValidationExceptionSummaryRepository = _ValidationExceptionSummaryRepository;
    }

    public IList<ExceptionSummarySearchResult> GetExceptionSummarySearchResult(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, BillingCategoryType billingCategoryType)
    {
        return ValidationExceptionSummaryRepository.GetExceptionSummarySearch(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue?fileSubmmitDate.Value:(DateTime?)null, fileName,chargeCategoryId,isFormC, billingCategoryType);
    }
    public IList<ExceptionSummarySearchResult> GetExceptionSummarySearchResult(ValidationErrorCorrection validationErrorCorrection, BillingCategoryType billingCategoryType)
    {
        return ValidationExceptionSummaryRepository.GetExceptionSummarySearch(validationErrorCorrection, billingCategoryType);
    }
  }
}
