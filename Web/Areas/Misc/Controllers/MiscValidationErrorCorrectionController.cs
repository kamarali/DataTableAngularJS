using System;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Data.Common;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Areas.MU.Controllers;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
    public class MiscValidationErrorCorrectionController : MuValidationErrorCorrectionControllerBase
    {
        public MiscValidationErrorCorrectionController(ICalendarManager calenderManager, IExceptionSummarySearchResultManager validationExceptionSummaryManager, IExceptionDetailSearchResultManager validationExceptionDetail, IExceptionCodeRepository exceptionCodeRepository,
          IMiscInvoiceManager miscUatpInvoiceManager, IMiscInvoiceRepository miscInvoiceRepository, IUatpInvoiceManager uatpInvoiceManager, IMiscInvoiceRepository miscUatpInvoiceRepository, IMiscUatpInvoiceManager muInvoiceManager)
        : base(calenderManager, validationExceptionSummaryManager, validationExceptionDetail, exceptionCodeRepository, miscUatpInvoiceManager, miscInvoiceRepository, uatpInvoiceManager, miscUatpInvoiceRepository, muInvoiceManager)
        {

        }

        public override JsonResult ClearSearch()
        {
            return base.ClearSearch();
        }


        /// <summary>
        ///   
        /// </summary>
        /// <param name="valErrorCorrection"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Misc.Receivables.ValidationErrorCorrection.Correct)]
        public override ActionResult Index(ValidationErrorCorrection valErrorCorrection)
        {
            valErrorCorrection.ValidationErrorCategoryType = BillingCategoryType.Misc;
            return base.Index(valErrorCorrection);
        }

        public override JsonResult ExceptionSummaryGridData(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, int? billingCategoryType = null)
        {
            return base.ExceptionSummaryGridData(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue ? fileSubmmitDate.Value : (DateTime?)null, fileName, chargeCategoryId, isFormC, (int)BillingCategoryType.Misc);
        }

        public override JsonResult UpdateCorrectLinkingError(ValidationErrorCorrection model)
        {
            model.ValidationErrorCategoryType = BillingCategoryType.Misc;
            return base.UpdateCorrectLinkingError(model);
        }
        public override JsonResult IsDisplayLinkingButton(string exceptionCode, string billingCategoryId)
        {
          return base.IsDisplayLinkingButton(exceptionCode, billingCategoryId);
        }

        public override JsonResult BatchUpdatedCount(string summaryId, string oldvalue, string exceptionCode, string billingCategoryId)
        {
            return base.BatchUpdatedCount(summaryId, oldvalue, exceptionCode, billingCategoryId);
        }

        [RestrictInvoiceUpdate(TransactionParamName = "exceptionSummaryId", InvList = false, IsJson = true, TableName = TransactionTypeTable.VALIDATION_EXCEPTION_SUMMARY)]
        public override JsonResult UpdateValidationErrors(string filename, string ExceptionCode, string ErrorDescription, string FieldName, string FieldValue, string NewValue, string exceptionSummaryId, string exeptionDetailsId, string isBatchUpdate, string billingCat, string errorLevel, string pkReferenceId, DateTime LastUpdatedOn)
        {
            return base.UpdateValidationErrors(filename, ExceptionCode, ErrorDescription, FieldName, FieldValue, NewValue,
                                               exceptionSummaryId, exeptionDetailsId, isBatchUpdate, billingCat, errorLevel, pkReferenceId, LastUpdatedOn);
        }

        public override JsonResult ValidateError(string ExceptionCode, string NewValue, string pkReferenceId, string billingCategoryId)
        {
            return base.ValidateError(ExceptionCode, NewValue, pkReferenceId, billingCategoryId);
        }

        public override JsonResult ExceptionDetailsGridData(string rowcells, string billingCategoryType, string exceptionCode)
        {
            billingCategoryType = (Convert.ToString(Convert.ToInt32(BillingCategoryType.Misc)));
            return base.ExceptionDetailsGridData(rowcells, billingCategoryType, exceptionCode);
        }
    }
}
