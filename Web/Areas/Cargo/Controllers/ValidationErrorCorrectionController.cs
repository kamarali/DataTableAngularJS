using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Cargo;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Cargo.Impl;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core.ValidationErrorCorrection;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Common;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Model.Enums.BillingType;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.Globalization;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class ValidationErrorCorrectionController : ISController
  {

    private const string SearchResultGridAction = "SearchResultGridData";
    private const string ExceptionSummaryGridAction = "ExceptionSummaryGridData";
    private const string ExceptionSummaryGridData = "ExceptionSummaryGridDatas";

    private readonly ICalendarManager _calendarManager;
    //private readonly ValidationExceptionDetailBase validationExceptiondetailBase;
    public readonly IExceptionDetailSearchResultManager _validationExceptionDetail;
    private readonly IExceptionSummarySearchResultManager _validationExceptionSummaryManager;
    private readonly IExceptionCodeRepository _exceptionCodeRepository;
    private readonly ICargoBillingMemoRepository _cargoBillingMemoRepository;
    private readonly ICargoInvoiceRepository _cargoInvoiceRepository;
    private readonly ICargoInvoiceManager _cargoInvoiceManager;
    /// <summary>
    /// Gets or sets the rejection memo repository.
    /// </summary>
    /// <value>The rejection memo repository.</value>
    public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

    public ValidationErrorCorrectionController(ICalendarManager calenderManager, IExceptionDetailSearchResultManager validationExceptionDetail, IExceptionSummarySearchResultManager validationExceptionSummaryManager, IExceptionCodeRepository exceptionCodeRepository, ICargoBillingMemoRepository cargoBillingMemoRepository, ICargoInvoiceRepository cargoInvoiceRepository, ICargoInvoiceManager cargoInvoiceManager)
    {
      _calendarManager = calenderManager;
      _validationExceptionDetail = validationExceptionDetail;
      _validationExceptionSummaryManager = validationExceptionSummaryManager;
      _exceptionCodeRepository = exceptionCodeRepository;
      _cargoBillingMemoRepository = cargoBillingMemoRepository;
      _cargoInvoiceRepository = cargoInvoiceRepository;
      _cargoInvoiceManager = cargoInvoiceManager;
    }



    /// <summary>
    ///   
    /// </summary>
    /// <param name="valErrorCorrection"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.ValidationErrorCorrection.Correct)]
    public ActionResult Index(ValidationErrorCorrection valErrorCorrection)
    {
      try
      {
        if (valErrorCorrection != null)
        {
          var currentBillingPeriodDetails = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          valErrorCorrection.BillingPeriod = valErrorCorrection.BillingPeriod == 0 ? currentBillingPeriodDetails.Period : valErrorCorrection.BillingPeriod;
          valErrorCorrection.BillingMonth = valErrorCorrection.BillingMonth == 0 ? currentBillingPeriodDetails.Month : valErrorCorrection.BillingMonth;
          valErrorCorrection.BillingYear = valErrorCorrection.BillingYear == 0 ? currentBillingPeriodDetails.Year : valErrorCorrection.BillingYear;
          valErrorCorrection.ExceptionCodeId = valErrorCorrection.ExceptionCode == null ? -1 : valErrorCorrection.ExceptionCodeId;
          valErrorCorrection.BilledMemberId = valErrorCorrection.BilledMember == null ? -1 : valErrorCorrection.BilledMemberId;
          valErrorCorrection.BillingMemberId = SessionUtil.MemberId;
        }
        var billCat = (int)valErrorCorrection.ValidationErrorCategoryType;
        //string criteria = valErrorCorrection != null ? new JavaScriptSerializer().Serialize(valErrorCorrection) : string.Empty;

        var vecGrid = new ValidationErrorCorrectionGrid(ControlIdConstants.ValidationErrorCorrectionGrid, Url.Action(SearchResultGridAction, new
        {
            period = valErrorCorrection.BillingPeriod,
            billingMonth = valErrorCorrection.BillingMonth,
            billingYear = valErrorCorrection.BillingYear,
            billingMemberId = valErrorCorrection.BillingMemberId,
            billedMemberId = valErrorCorrection.BilledMemberId,
            invoiceNo = valErrorCorrection.InvoiceNumber,
            exceptionCodeId = valErrorCorrection.ExceptionCodeId,
            fileSubmmitDate = valErrorCorrection.FileSubmissionDate,
            fileName = valErrorCorrection.FileName,
            chargeCategoryId = valErrorCorrection.ChargeCategoryId,
            isFormC = valErrorCorrection.IsFormC,
            billCat
        }));
        ViewData[ViewDataConstants.ValidationErrorCorrectionGrid] = vecGrid.Instance;

        var esGrid = new ExceptionSummaryGrid(ControlIdConstants.ExceptionSummaryGrid, Url.Action("ExceptionSummaryGridDatas"));
        ViewData[ViewDataConstants.ExceptionSummaryGrid] = esGrid.Instance;
          ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);
      }

      return View(valErrorCorrection);
    }

    public virtual JsonResult ClearSearch()
    {
      var defaultPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      return Json(defaultPeriod);
    }

    public JsonResult SearchResultGridData(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId, DateTime? fileSubmmitDate, string fileName, int chargeCategoryId, bool isFormC, int? billingCategoryType = null)
    {
        //if (fileSubmmitDate == null)
        //    fileSubmmitDate = new DateTime(1, 1, 1);
       var vecGrid = new ValidationErrorCorrectionGrid(ControlIdConstants.ValidationErrorCorrectionGrid, Url.Action(SearchResultGridAction, new { period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate, fileName, chargeCategoryId, isFormC }));
        var exceptionSummarSearchData = _validationExceptionSummaryManager.GetExceptionSummarySearchResult(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue ? fileSubmmitDate.Value : (DateTime?)null, fileName, chargeCategoryId, isFormC, BillingCategoryType.Cgo).AsQueryable();


      return vecGrid.DataBind(exceptionSummarSearchData);

    }

    public JsonResult SearchDetailResult()
    {
      return null;
    }



    public JsonResult ExceptionSummaryGridDatas(string rowcells, string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(rowcells))
      {
        var validationErrorCorrection = new ValidationErrorCorrection();


        var esGrid = new ExceptionSummaryGrid(ControlIdConstants.ExceptionSummaryGrid,
                                              Url.Action(ExceptionSummaryGridAction, "ValidationErrorCorrection"));

        List<ExceptionDetailsSearchResult> invoiceSearchedData;
        Guid id = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(rowcells));

        invoiceSearchedData = _validationExceptionDetail.GetExceptionDetailData(id, 2);

        foreach (ExceptionDetailsSearchResult t in invoiceSearchedData)
        {
            switch (t.BillingCode)
            {
                case "P":
                    t.TransactionType = "Prepaid AWB";
                    break;

                case "C":
                    t.TransactionType = "Collect AWB";
                    break;

                case "B":
                    t.TransactionType = "Billing Memo";
                    break;

                case "T":
                    t.TransactionType = "Credit Memo";
                    break;

                case "R":
                    t.TransactionType = "Rejection Memo";
                    break;

                default:
                    t.TransactionType = " ";
                    break;
            }

            t.ErrorLevelDisplay = ErrorCorrectionLevels.LevelsDisplay[t.ErrorLevel];

            if (Convert.ToInt32(t.YourInvoiceYear) != 0 && Convert.ToInt32(t.YourInvoiceMonth) != 0 &&
                Convert.ToInt32(t.YourInvoicePeriod) != 0)
            {
                t.YourInvoiceBillingDate = t.YourInvoiceYear.ToString() + t.YourInvoiceMonth.ToString().PadLeft(2, '0') +
                                           t.YourInvoicePeriod.ToString().PadLeft(2, '0');
            }

            if (exceptionCode == "BCGO_10199")
            {
                //  int corrLength = t.CorrespondenceRefNo.ToString().Length;
                t.FieldValue = t.FieldValue.PadLeft(11, '0');
                t.CorrespondenceRefNo = t.FieldValue;
            }

        }

          return esGrid.DataBind(invoiceSearchedData.AsQueryable());
      }
      return null;
    }

    public JsonResult IsDisplayLinkingButton(string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(exceptionCode))
      {
        var billingCat = (int) BillingCategoryType.Cgo;
        var exceptionCodeObj = _exceptionCodeRepository.Single(i => i.Name.ToUpper().CompareTo(exceptionCode.ToUpper()) == 0 && i.BillingCategoryId == billingCat);
        if (exceptionCodeObj != null)
        {
          if (exceptionCodeObj.ErrorType == "L")
          {
            return new JsonResult() { Data = "1" };
          }

        }

      }
      return new JsonResult() { Data = "0" };
    }

    /// <summary>
    /// Converts string(i.e. FileId) to GUID and converts it to ByteArray.
    /// Iterates through ByteArray, convert it to Hexadecimal equivalent and appends each hex values to 
    /// create a string(i.e. FileId in Oracle GUID format)
    /// </summary>
    private string ConvertNetGuidToOracleGuid(string invoiceIds)
    {
      // Declare variable of type StringBuilder
      var oracleGuidBuilder = new StringBuilder();

      // Iterate through file Id's and split on ','
      foreach (var netGuid in invoiceIds.Split(','))
      {
        // Call "ConvertNetGuidToOracleGuid()" method which will return string 
        oracleGuidBuilder.Append(ToOracleGuid(netGuid));
        oracleGuidBuilder.Append(",");
      }// end foreach()

      // Declare string variable
      string oracleGuid;
      // Set string variable to Guid in Oracle format
      oracleGuid = oracleGuidBuilder.ToString();
      // Trim last ',' of comma separated GUID string
      oracleGuid = oracleGuid.TrimEnd(',');

      return oracleGuid;

    }// end ConvertNetGuidToOracleGuid()

    private string ToOracleGuid(string oracleGuid)
    {
      // Convert string to Guid
      Guid netGuid = new Guid(oracleGuid);
      // Convert Guid to ByteArray
      byte[] guidBytes = netGuid.ToByteArray();
      // Create StringBuilder
      var oracleGuidBuilder = new StringBuilder();
      // Iterate through ByteArray, get Hex equivalent of each byte and append it to string
      foreach (var singleByte in guidBytes)
      {
        // Get Hex equivalent of each byte
        var hexEqui = singleByte.ToString("X");
        // Append each Hex equivalent to construct Guid.(Pad '0' to single byte)
        oracleGuidBuilder.Append(hexEqui.ToString().PadLeft(2, '0'));
      }// end foreach()

      // Return Guid string in Oracle format
      return oracleGuidBuilder.ToString();
    }

    public JsonResult ValidateError(string ExceptionCode, string NewValue, string pkReferenceId, string errorLevel)
    {
      if (!string.IsNullOrWhiteSpace(ExceptionCode))
      {
        ErrorCorrectionExceptionCode exceptionCodeobj =
          ValidationErrorConfigReader.GetExceptionCodeDetails(ExceptionCode,(int)BillingCategoryType.Cgo);

        if (exceptionCodeobj != null)
        {
          if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("REGEX") == 0)
          {
            int result = _validationExceptionDetail.ValidateReqularExpressionValue(exceptionCodeobj.RegularExpression,
                                                                                   NewValue);
            return Json(result);
          }
          else if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("DIRECT") == 0)
          {
            Guid pkReferenceGuid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(pkReferenceId));
            int result = _cargoInvoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode, errorLevel, pkReferenceGuid);
            return Json(result);
          }
          else
          {
            int result = _validationExceptionDetail.CheckForValidation(exceptionCodeobj.ExceptionCode,
                                                                       exceptionCodeobj.ValidationType,
                                                                       exceptionCodeobj.MasterTableName,
                                                                       exceptionCodeobj.MasterColumnName, NewValue,
                                                                       exceptionCodeobj.MasterGroupColumnName,
                                                                       exceptionCodeobj.MasterGroupId);
            return Json(result);
          }
        }
      }

      return null;
    }

    [RestrictInvoiceUpdate(TransactionParamName = "exceptionSummaryId", InvList = false, IsJson = true, TableName = TransactionTypeTable.VALIDATION_EXCEPTION_SUMMARY)] 
    public JsonResult UpdateValidationErrors(string filename, string ExceptionCode, string ErrorDescription, string FieldName, string FieldValue, string NewValue, string exceptionSummaryId, string exeptionDetailsId, string isBatchUpdate, string billingCat, string errorLevel, string pkReferenceId, DateTime lastUpdatedOn)
    {

      if (!string.IsNullOrWhiteSpace(ExceptionCode))
      {
          ErrorCorrectionExceptionCode exceptionCodeobj = ValidationErrorConfigReader.GetExceptionCodeDetails(ExceptionCode, (int)BillingCategoryType.Cgo);

        if (exceptionCodeobj != null)
        {
          int result = 0;
          if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("REGEX") == 0)
          {
            result = _validationExceptionDetail.ValidateReqularExpressionValue(exceptionCodeobj.RegularExpression, NewValue);
          }
          else if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("DIRECT") == 0)
          {
            Guid pkReferenceGuid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(pkReferenceId));
            result = _cargoInvoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode,errorLevel, pkReferenceGuid);
          }
          else
          {
            result = _validationExceptionDetail.CheckForValidation(exceptionCodeobj.ExceptionCode,
                                                                   exceptionCodeobj.ValidationType,
                                                                   exceptionCodeobj.MasterTableName,
                                                                   exceptionCodeobj.MasterColumnName, NewValue,
                                                                   exceptionCodeobj.MasterGroupColumnName,
                                                                   exceptionCodeobj.MasterGroupId);
          }
          // Check for the validatation error ; if result is 1 that means validation pass
          // Update the entry in tables
          if (result == 1)
          {
            int batchUpdate;
            var childTableName = new List<string>();
            var columnName = new List<string>();
            var primaryColumnName = new List<string>();
            var errorLevels = new List<string>();
            batchUpdate = Convert.ToInt32(isBatchUpdate);

            if (batchUpdate == 1)
            {
              foreach (var error in exceptionCodeobj.ErrorLevels)
              {
                errorLevels.Add(error.ErrorLevelName);
                childTableName.Add(error.ChildTableName);
                columnName.Add(error.ColumnName);
                primaryColumnName.Add(error.PrimaryColumnName);
              }
            }
            else
            {
              var errorObj = exceptionCodeobj.ErrorLevels.Find(i => i.ErrorLevelName == errorLevel);
              if (errorObj != null)
              {
                errorLevels.Add(errorObj.ErrorLevelName);
                childTableName.Add(errorObj.ChildTableName);
                columnName.Add(errorObj.ColumnName);
                primaryColumnName.Add(errorObj.PrimaryColumnName);
              }
            }

            int billingCategory = Convert.ToInt32(billingCat);

            Guid exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(exeptionDetailsId));
           
            int isUpdated = _validationExceptionDetail.UpdateCorrectedData(filename, batchUpdate, NewValue.ToUpper(), exceptiondetailId,
                                                            string.Join(",", errorLevels.ToArray()), string.Join(",", childTableName.ToArray()), string.Join(",", columnName.ToArray()),
                                                            string.Join(",", primaryColumnName.ToArray()), billingCategory, SessionUtil.UserId, exceptionCodeobj.FieldType, lastUpdatedOn);

            return Json(isUpdated);
          }
          else if(result == 3)
          {
            return new JsonResult() { Data = "SameOriginAndDestinationValue" };
          }
          else if (result == 4)
          {
            return new JsonResult() { Data = "SameFromAndToValue" };
          }
          else
          {
            return new JsonResult() { Data = "Error" };
          }

        }
      }
      return null;
    }

    public JsonResult BatchUpdatedCount(string summaryId, string oldvalue, string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(exceptionCode))
      {
        ErrorCorrectionExceptionCode exceptionCodeobj =
            ValidationErrorConfigReader.GetExceptionCodeDetails(exceptionCode, (int)BillingCategoryType.Cgo);

        if (exceptionCodeobj != null)
        {
          var errorlevels = new List<string>();

          foreach (var error in exceptionCodeobj.ErrorLevels)
          {
            errorlevels.Add(error.ErrorLevelName);

          }// End foreach

          Guid exceptionsummaryId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(summaryId));
          int batchUpdateCount = _validationExceptionDetail.GetBatchUpdateCount(exceptionsummaryId, oldvalue, string.Join(",", errorlevels.ToArray()));

          return Json(batchUpdateCount);
        }

      }
      return null;
    }

    public JsonResult UpdateCorrectLinkingError(ValidationErrorCorrection model)
    {
      Guid exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(model.ExceptionDetailId.ToString()));

      if (model.TranscationId == (int)TransactionType.CargoBillingMemo)
      {
        var isBmValidatedString = ValidateBMLinkingError(model.PkReferenceId, long.Parse(model.CorrespondenceRefNo));
        if (isBmValidatedString.CompareTo("1") != 0)
          return Json(isBmValidatedString);
        else
        {
          int isBmUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Cgo, exceptiondetailId, model.YourInvoiceNo,
                                                                      model.YourInvoicePeriod,
                                                                      model.YourInvoiceMonth,
                                                                      model.YourInvoiceYear,
                                                                      model.YourRejectionMemoNo, model.YourBmCmNo,model.RejectionStage,
                                                                      model.BmCmIndicator,long.Parse(model.CorrespondenceRefNo),
                                                                      4, SessionUtil.UserId,model.ProvisionalInvoiceNo,model.BatchSeqNo,model.BatchRecordSeq,model.FimCouponNo,model.FimBmCmNo, model.LastUpdatedOn);

          return Json(isBmUpdated);
        }
      }
      else
      {





          var cultureInfo = new CultureInfo("en-US");
          cultureInfo.Calendar.TwoDigitYearMax = 2099;
          DateTime yourInvoiceBillingDate;

          var day = string.Empty;
          if (model.YourInvoicePeriod <= 9)
          {
              day = "0" + model.YourInvoicePeriod.ToString();
          }
          else
          {
              day = model.YourInvoicePeriod.ToString();
          }

          var month = string.Empty;
          if (model.YourInvoiceMonth <= 9)
          {
              month = "0" + model.YourInvoiceMonth.ToString();
          }
          else
          {
              month = model.YourInvoiceMonth.ToString();
          }
          string YourInvoiceBillingDate = model.YourInvoiceYear.ToString() + month.ToString() + day.ToString();
          if (DateTime.TryParseExact(YourInvoiceBillingDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out yourInvoiceBillingDate))
          {
              if (yourInvoiceBillingDate.Day > 4 || yourInvoiceBillingDate.Day <= 0)
              {
                  return Json("9");
              }
          }
          else
          {
              return Json("9");
          }


        var criteria = new CgoRMLinkingCriteria
                         {
                           ReasonCode = model.ReasonCode,
                           InvoiceNumber = model.YourInvoiceNo,
                           BillingYear = model.YourInvoiceYear,
                           BillingMonth = model.YourInvoiceMonth,
                           BillingPeriod = model.YourInvoicePeriod,
                           RejectionMemoNumber = model.YourRejectionMemoNo,
                           BMCMIndicatorId = model.BmCmIndicator,
                           RejectionStage = model.RejectionStage,
                           BillingMemberId = model.BilledMemberId, //Note : assign Billing to Billed and Billed to Billing
                           BilledMemberId = model.BillingMemberId,
                           RejectedInvoiceId = model.InvoiceID,
                           YourBillingMemoNumber = model.YourBmCmNo,
                           IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod,//model.Ignorevalidation
                           TransactionType =  model.RejectionStage == 1 ? "BM" : "RM"
                         };

        CgoRMLinkingResultDetails rmLinkingModel = RejectionMemoRepository.GetRMLinkingDetails(criteria);

        if (rmLinkingModel.IsLinkingSuccessful == false)
        {
          return Json(rmLinkingModel.ErrorMessage);
        }
        else
        {
            // CMP#650 validate changes for reason code in case of BM/RM
            var rmCurrentRecord = new CargoRejectionMemo()
                                      {
                                          InvoiceId = model.InvoiceID,
                                          BMCMIndicatorId =  model.BmCmIndicator,
                                          YourBillingMemoNumber = model.YourBmCmNo,
                                          YourInvoiceBillingPeriod = model.YourInvoicePeriod,
                                          YourInvoiceBillingMonth = model.YourInvoiceMonth,
                                          YourInvoiceBillingYear = model.YourInvoiceYear,
                                          YourInvoiceNumber = model.YourInvoiceNo,
                                          YourRejectionNumber = model.YourRejectionMemoNo,
                                          ReasonCode = model.ReasonCode,
                                          RejectionStage = model.RejectionStage
                                      };
            // CMP#650
            string errorMsg = _cargoInvoiceManager.ValidateCargoReasonCode(rmCurrentRecord, rmLinkingModel.ReasonCode, false,true);

            if(!string.IsNullOrEmpty(errorMsg))
            {
                return Json(errorMsg);
            }
          int breakdownValidation = _validationExceptionDetail.ValidateCgoLinkingFunction(exceptiondetailId,model.YourInvoiceNo,model.YourInvoicePeriod,model.YourInvoiceMonth,
            model.YourInvoiceYear, model.YourRejectionMemoNo, model.YourBmCmNo, model.BmCmIndicator, model.RejectionStage, model.BilledMemberId, model.BillingMemberId);//Note : assign Billing to Billed and Billed to Billing

          if (breakdownValidation == 0)
          {
            return Json("No matching breakdown details found in the database.");
          }
          else
          {
              //CMP#459 Validate amounts
              IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
              var isAmountsValid = _cargoInvoiceManager.ValidateAmountofRmOnValidationErrorCorrection(exceptionDetailsList, model);
              if (!isAmountsValid)
              {
                  string errMsg = string.Empty;
                  foreach (var exceptionMsg in exceptionDetailsList)
                  {
                      errMsg += exceptionMsg.ErrorDescription + "\n";
                  }
                  return Json(errMsg);
              }

              #region CMP-674-Validation of Coupon and AWB Breakdowns in Rejections

              /* CMP#674 validations will be applicable only to the following transactions - CGO Stage 2 RMs and CGO Stage 3 RMs */
              if (model.RejectionStage == 2 || model.RejectionStage == 3)
              {
                  List<InvalidRejectionMemoDetails> invalidRejectionMemos =
                      RejectionMemoRepository.IsYourRejectionCouponDropped(model.InvoiceID, model.PkReferenceId,
                                                                           model.YourInvoiceNo,
                                                                           model.YourRejectionMemoNo,
                                                                           model.YourInvoiceYear, model.YourInvoiceMonth,
                                                                           model.YourInvoicePeriod);

                  foreach (InvalidRejectionMemoDetails invalidRM in invalidRejectionMemos)
                  {
                      /* Report Error -
                      * Error Code - RejectionMemoCouponMissing = "BCGO_10402"
                      * Error Description - Mismatch in AWB <yyy-yyyyyyy>. It was billed <a> time(s) in the rejected RM; and <b> time(s) in this RM. 
                      *                     Other mismatches if any are not included in this report.
                      */
                      var errorDescription = Messages.ResourceManager.GetString(CargoErrorCodes.CargoRMCouponMismatchFileValidation);

                      errorDescription = string.Format(errorDescription,
                                                       invalidRM.TicketIssuingAirline, invalidRM.TicketDocOrAwbNumber,
                                                       invalidRM.RejectedRMOccurrence, invalidRM.RejectingRMOccurrence);

                      /* It will not be user friendly to report every mismatched coupon, so returning first error found. */
                      return Json(errorDescription);
                  }
              }
              /* CMP#674 - Not applicable - Validation bypassed - Only for Logical Completion
              else
              {
              
              }*/

              #endregion

            int isUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Cgo,exceptiondetailId, model.YourInvoiceNo,
                                                                     model.YourInvoicePeriod,
                                                                     model.YourInvoiceMonth,
                                                                     model.YourInvoiceYear,
                                                                     model.YourRejectionMemoNo, model.YourBmCmNo,model.RejectionStage,
                                                                     model.BmCmIndicator, 0,
                                                                     model.LinkingDetail, SessionUtil.UserId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq, model.FimCouponNo, model.FimBmCmNo, model.LastUpdatedOn);

            return Json(isUpdated);
          }
        }
      }// End else
      return null;
    }// End UpdateCorrectionLinkingError

    public string ValidateBMLinkingError(Guid billingMemoId, long corrReferenceNo)
    {
      var billingMemo = _cargoBillingMemoRepository.First(i => i.Id == billingMemoId);
      if (billingMemo != null)
      {
        var invoiceId = billingMemo.InvoiceId;
        var cargoInvoice = _cargoInvoiceRepository.First(i => i.Id == invoiceId);
        if (cargoInvoice != null)
        {
          billingMemo.CorrespondenceReferenceNumber = corrReferenceNo;
          try
          {
            _cargoInvoiceManager.ValidateCorrespondenceReference(billingMemo, false, cargoInvoice);
            return "1";
          }
          catch (ISBusinessException exception)
          {
            return  Messages.ResourceManager.GetString(exception.ErrorCode);
          }
        }
        else
        {
          return "Invoice not found.";
        }
      }
      return "Billing memo not found.";
    }
  }
}
