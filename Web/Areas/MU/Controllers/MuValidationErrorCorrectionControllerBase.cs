using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MiscUatp.Impl;
using Iata.IS.Core;
using Iata.IS.Core.ValidationErrorCorrection;
using Iata.IS.Data.Common;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Core.Exceptions;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Web.Util.BillingType;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Data.Impl;

namespace Iata.IS.Web.Areas.MU.Controllers
{
  public class MuValidationErrorCorrectionControllerBase : ISController
  {
    public const string ExceptionDetailsGridAction = "ExceptionDetailsGridData";
    public const string ExceptionSummaryGridAction = "ExceptionSummaryGridData";
    private readonly IExceptionCodeRepository _exceptionCodeRepository;
    private readonly IMiscInvoiceManager _miscInvoiceManager;

    private readonly ICalendarManager _calendarManager;

    /// <summary>
    /// Gets or sets the correspondence repository.
    /// </summary>
    /// <value>The country repository.</value>
    private readonly IExceptionSummarySearchResultManager _validationExceptionSummaryManager;

    private readonly IExceptionDetailSearchResultManager _validationExceptionDetail;

    private readonly IMiscInvoiceRepository _miscInvoiceRepository;

    private readonly IUatpInvoiceManager _uatpInvoiceManager;

    // SCP280744: MISC UATP Exchange Rate population/validation during error correction
    private readonly IMiscInvoiceRepository _miscUatpInvoiceRepository;
    private readonly IMiscUatpInvoiceManager _muInvoiceManager;

    public MuValidationErrorCorrectionControllerBase(ICalendarManager calenderManager, IExceptionSummarySearchResultManager validationExceptionSummaryManager, IExceptionDetailSearchResultManager validationExceptionDetail, IExceptionCodeRepository exceptionCodeRepository, IMiscInvoiceManager miscUatpInvoiceManager, IMiscInvoiceRepository miscInvoiceRepository, IUatpInvoiceManager uatpInvoiceManager, IMiscInvoiceRepository miscUatpInvoiceRepository, IMiscUatpInvoiceManager muInvoiceManager)
    {
        _calendarManager = calenderManager;
        _validationExceptionSummaryManager = validationExceptionSummaryManager;
        _validationExceptionDetail = validationExceptionDetail;
        _exceptionCodeRepository = exceptionCodeRepository;
        _miscInvoiceManager = miscUatpInvoiceManager;
        _miscInvoiceRepository = miscInvoiceRepository;
        _uatpInvoiceManager = uatpInvoiceManager;
        _miscUatpInvoiceRepository = miscUatpInvoiceRepository;
        _muInvoiceManager = muInvoiceManager;
    }

    public virtual JsonResult ClearSearch()
    {
      var defaultPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      return Json(defaultPeriod);
    }

    /// <summary>
    ///   
    /// </summary>
    /// <param name="valErrorCorrection"></param>
    /// <returns></returns>

    public virtual ActionResult Index(ValidationErrorCorrection valErrorCorrection)
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
          valErrorCorrection.FileSubmissionDate = valErrorCorrection.FileSubmissionDate ?? null;
          valErrorCorrection.BillingMemberId = SessionUtil.MemberId;
        }
          
        var billCat = (int)valErrorCorrection.ValidationErrorCategoryType;
        var exceptionSummaryGrid = new ExceptionSummaryGrid(ControlIdConstants.UatpExceptionSummaryGrid, Url.Action(ExceptionSummaryGridAction,
            new { period = valErrorCorrection.BillingPeriod, billingMonth = valErrorCorrection.BillingMonth, billingYear = valErrorCorrection.BillingYear, billingMemberId=valErrorCorrection.BillingMemberId,
                  billedMemberId=valErrorCorrection.BilledMemberId, invoiceNo=valErrorCorrection.InvoiceNumber, exceptionCodeId=valErrorCorrection.ExceptionCodeId,fileSubmmitDate=valErrorCorrection.FileSubmissionDate,fileName=valErrorCorrection.FileName,chargeCategoryId=valErrorCorrection.ChargeCategoryId,
                  isFormC=valErrorCorrection.IsFormC, billCat }),
                  (BillingCategoryType)billCat == BillingCategoryType.Misc ? true : false);
        ViewData[ViewDataConstants.UatpExceptionSummaryGrid] = exceptionSummaryGrid.Instance;

        var exceptionDetailsGrid = new ExceptionDetailsGrid(ControlIdConstants.UatpExceptionDetailsGrid, Url.Action(ExceptionDetailsGridAction));
        ViewData[ViewDataConstants.UatpExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);
      }

      return View(valErrorCorrection);
    }


    public virtual JsonResult ExceptionSummaryGridData(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId,DateTime? fileSubmmitDate,
                                                        string fileName,int chargeCategoryId,bool isFormC, int? billingCategoryType = null)
    {
      var exceptionSummaryGrid = new ExceptionSummaryGrid(ControlIdConstants.UatpExceptionSummaryGrid, Url.Action(ExceptionSummaryGridAction, new { period,billingMonth,billingYear,billingMemberId,billedMemberId,invoiceNo,exceptionCodeId,fileSubmmitDate,fileName,chargeCategoryId,isFormC }),
                                                          (BillingCategoryType)billingCategoryType.Value == BillingCategoryType.Misc ? true : false);
      var exceptionSummarSearchData = _validationExceptionSummaryManager.GetExceptionSummarySearchResult(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue ? fileSubmmitDate.Value:(DateTime?)null, fileName, chargeCategoryId, isFormC, (BillingCategoryType)billingCategoryType.Value).AsQueryable();
      return exceptionSummaryGrid.DataBind(exceptionSummarSearchData);
    }

    public virtual JsonResult ExceptionDetailsGridData(string rowcells, string billingCategoryType, string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(rowcells))
      {

        var esGrid = new ExceptionDetailsGrid(ControlIdConstants.UatpExceptionDetailsGrid, Url.Action(ExceptionDetailsGridAction));

        List<ExceptionDetailsSearchResult> invoiceSearchedData;
        Guid id = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(rowcells));

        invoiceSearchedData = _validationExceptionDetail.GetExceptionDetailData(id, (Convert.ToInt32(billingCategoryType)));

        foreach (ExceptionDetailsSearchResult t in invoiceSearchedData)
        {
          t.ErrorLevelDisplay = ErrorCorrectionLevels.LevelsDisplay[t.ErrorLevel];

          if (Convert.ToInt32(t.YourInvoiceYear) != 0 && Convert.ToInt32(t.YourInvoiceMonth) != 0 && Convert.ToInt32(t.YourInvoicePeriod) != 0)
          {
            t.YourInvoiceBillingDate = t.YourInvoiceYear.ToString() + t.YourInvoiceMonth.ToString().PadLeft(2, '0') + t.YourInvoicePeriod.ToString().PadLeft(2, '0');
          }

         if(exceptionCode == "BMISC_10193" || exceptionCode == "BUATP_10193")
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

    public virtual JsonResult ValidateError(string ExceptionCode, string NewValue, string pkReferenceId, string billingCategoryId)
    {
      if (!string.IsNullOrWhiteSpace(ExceptionCode))
      {
        ErrorCorrectionExceptionCode exceptionCodeobj =
          ValidationErrorConfigReader.GetExceptionCodeDetails(ExceptionCode, Convert.ToInt32(billingCategoryId));

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
            int result = _miscInvoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode, pkReferenceGuid);
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
    //SCP252342 - SRM: ICH invoice in ready for billing status:: Add a new parameter to fetch LastUpdatedOn date of validation detail record.
    public virtual JsonResult UpdateValidationErrors(string filename, string ExceptionCode, string ErrorDescription, string FieldName, string FieldValue, string NewValue, string exceptionSummaryId, string exeptionDetailsId, string isBatchUpdate, string billingCat, string errorLevel, string pkReferenceId, DateTime LastUpdatedOn)
    {

      if (!string.IsNullOrWhiteSpace(ExceptionCode))
      {
        int billingCategory = Convert.ToInt32(billingCat);
        ErrorCorrectionExceptionCode exceptionCodeobj = ValidationErrorConfigReader.GetExceptionCodeDetails(ExceptionCode, billingCategory);

        if (exceptionCodeobj != null)
        {
          int result = 0;
          if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("REGEX") == 0)
          {
            result = _validationExceptionDetail.ValidateReqularExpressionValue(exceptionCodeobj.RegularExpression,
                                                                                   NewValue);

          }
          else if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("DIRECT") == 0)
          {
            Guid pkReferenceGuid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(pkReferenceId));
            result = _miscInvoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode, pkReferenceGuid);
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



            Guid exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(exeptionDetailsId));

            int isUpdated = _validationExceptionDetail.UpdateCorrectedData(filename, batchUpdate, NewValue.ToUpper(), exceptiondetailId,
                                                            string.Join(",", errorLevels.ToArray()), string.Join(",", childTableName.ToArray()), string.Join(",", columnName.ToArray()),
                                                            string.Join(",", primaryColumnName.ToArray()), billingCategory, SessionUtil.UserId, exceptionCodeobj.FieldType,LastUpdatedOn);

            return Json(isUpdated);
          }
          else
          {
            return new JsonResult() { Data = "Error" };
          }

        }
      }
      return null;
    }

    public virtual JsonResult BatchUpdatedCount(string summaryId, string oldvalue, string exceptionCode, string billingCategoryId)
    {
      if (!string.IsNullOrWhiteSpace(exceptionCode))
      {
        ErrorCorrectionExceptionCode exceptionCodeobj =
            ValidationErrorConfigReader.GetExceptionCodeDetails(exceptionCode, Convert.ToInt32(billingCategoryId));

        if (exceptionCodeobj != null)
        {
          var errorlevels = new List<string>();

          foreach (var error in exceptionCodeobj.ErrorLevels)
          {
            errorlevels.Add(error.ErrorLevelName);

          }

          Guid exceptionsummaryId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(summaryId));
          int batchUpdateCount = _validationExceptionDetail.GetBatchUpdateCount(exceptionsummaryId, oldvalue, string.Join(",", errorlevels.ToArray()));

          return Json(batchUpdateCount);
        }

      }

      return null;
    }

    public virtual JsonResult IsDisplayLinkingButton(string exceptionCode, string billingCategoryId)
    {
      if (!string.IsNullOrWhiteSpace(exceptionCode))
      {
        var billingCat = Convert.ToInt32(billingCategoryId);
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
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// SCP321993: FW ICH Settlement Error - SIS Production
    /// <returns></returns>
    public virtual JsonResult UpdateCorrectLinkingError(ValidationErrorCorrection model)
    {
      // SCP321993: FW ICH Settlement Error - SIS Production
      decimal? updatedExRate = null;
      decimal? updatedClearanceAmt = null;

      var exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(model.ExceptionDetailId.ToString()));
      const int muCorrespondencevalue = 37;
      
      if (model.TranscationId == muCorrespondencevalue)
      {
        var isCorrValidatedString = ValidateCorrespondenceLinkingError(model.PkReferenceId, long.Parse(model.CorrespondenceRefNo), model.YourInvoiceNo);
        if (isCorrValidatedString.CompareTo("1") != 0)
          return Json(isCorrValidatedString);
        else
        {
            var miscUatpInvoice = _miscUatpInvoiceRepository.Single(model.InvoiceID);
            var isExchangeRateValidationResult = "Success";
            if (miscUatpInvoice.InvoiceSmi == SMI.AchUsingIataRules || miscUatpInvoice.InvoiceSmi == SMI.Ach || miscUatpInvoice.InvoiceSmi == SMI.Ich)
            {
                isExchangeRateValidationResult = _muInvoiceManager.ExchangeRateValidationsOnErrorCorrection(miscUatpInvoice, out updatedExRate, out updatedClearanceAmt);
            }

            if (isExchangeRateValidationResult.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
                //In this update only : YourInvoiceNo,CorrespondenceRefNo
                int isBmUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(model.ValidationErrorCategoryType,
                                                                                     exceptiondetailId,
                                                                                     model.YourInvoiceNo,
                                                                                     model.YourInvoicePeriod,
                                                                                     model.YourInvoiceMonth,
                                                                                     model.YourInvoiceYear,
                                                                                     string.Empty,
                                                                                     string.Empty,
                                                                                     0,
                                                                                     0,
                                                                                     long.Parse(
                                                                                         model.CorrespondenceRefNo),
                                                                                     model.LinkingDetail,
                                                                                     SessionUtil.UserId,
                                                                                     model.ProvisionalInvoiceNo,
                                                                                     model.BatchSeqNo,
                                                                                     model.BatchRecordSeq,
                                                                                     model.FimCouponNo, model.FimBmCmNo,
                                                                                     model.LastUpdatedOn,
                                                                                     updatedExRate,
                                                                                     updatedClearanceAmt);
                    //for BM it is 2

                if (isBmUpdated == 1)
                {
                    UnitOfWork.CommitDefault();
                }

                return Json(isBmUpdated);
            }
            else
            {
                return Json(isExchangeRateValidationResult);
            }
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


        //RM case
        var isRmValidatedString = ValidateRmLinkingError(model.PkReferenceId, model.YourInvoiceNo, model.YourInvoiceMonth, model.YourInvoiceYear, model.YourInvoicePeriod);
        if (isRmValidatedString.CompareTo("1") != 0)
          return Json(isRmValidatedString);
        else
        {
            var miscUatpInvoice = _miscUatpInvoiceRepository.Single(model.InvoiceID);
            var isExchangeRateValidationResult = "Success";
            if (miscUatpInvoice.InvoiceSmi == SMI.AchUsingIataRules || miscUatpInvoice.InvoiceSmi == SMI.Ach || miscUatpInvoice.InvoiceSmi == SMI.Ich)
            {
              isExchangeRateValidationResult = _muInvoiceManager.ExchangeRateValidationsOnErrorCorrection(miscUatpInvoice, out updatedExRate, out updatedClearanceAmt);
            }

            if (isExchangeRateValidationResult.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            {
                //SCP340919: Incorrect validation completed time in dashboard report
                miscUatpInvoice.ValidationDate = DateTime.UtcNow;

                //In this update only : YourInvoiceNo,settlement period
                int isRmUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(model.ValidationErrorCategoryType,
                                                                                     exceptiondetailId,
                                                                                     model.YourInvoiceNo,
                                                                                     model.YourInvoicePeriod,
                                                                                     model.YourInvoiceMonth,
                                                                                     model.YourInvoiceYear,
                                                                                     string.Empty,
                                                                                     string.Empty,
                                                                                     0,
                                                                                     0,
                                                                                     long.Parse(
                                                                                         model.CorrespondenceRefNo),
                                                                                     model.LinkingDetail,
                                                                                     SessionUtil.UserId,
                                                                                     model.ProvisionalInvoiceNo,
                                                                                     model.BatchSeqNo,
                                                                                     model.BatchRecordSeq,
                                                                                     model.FimCouponNo, model.FimBmCmNo,
                                                                                     model.LastUpdatedOn,
                                                                                     updatedExRate,
                                                                                     updatedClearanceAmt);
                    //for RM it is 1
                if (isRmUpdated == 1)
                {
                    UnitOfWork.CommitDefault();
                }
                return Json(isRmUpdated);
            }
            else
            {
                return Json(isExchangeRateValidationResult);
            }
        }
      }
      return null;
    }// End UpdateCorrectionLinkingError

    /// <summary>
    /// Assumption : in Validation_Exception_Detail YourInvoiceNo is yourRejectionInvoiceNo for Billing memo case
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="corrReferenceNo"></param>
    /// <param name="yourRejectionInvoiceNo"></param>
    /// <returns></returns>
    public string ValidateCorrespondenceLinkingError(Guid invoiceId, long corrReferenceNo, string yourRejectionInvoiceNo)
    {
      //SCP277473 - Regarding SIS Rejection of BG correspondence invoice MCO0000026
      //This code has been changed during test for this issue, invoice summary object does not come along with invoice object 
      //due this reason linking failed in amount validation. 
      var muInvoice = _miscInvoiceRepository.Single(invoiceId:invoiceId);
      if (muInvoice != null)
      {
        try
        {
          muInvoice.CorrespondenceRefNo = corrReferenceNo;
          muInvoice.RejectedInvoiceNumber = yourRejectionInvoiceNo;
          if (_miscInvoiceManager.ValidateMiscUatCorrespondenceLinking(muInvoice))
            return "1";
          return string.Empty;
        }
        catch (ISBusinessException exception)
        {
          return Messages.ResourceManager.GetString(exception.ErrorCode);
        }
      }
      return "Correspondence invoice not found.";
    }

    public string ValidateRmLinkingError(Guid invoiceId, string yourRejectionInvoiceNo, int billingMonth, int billingYear, int billingPeriod)
    {
      var muInvoice = _miscInvoiceRepository.First(i => i.Id == invoiceId);
      if (muInvoice != null)
      {
        try
        {
          muInvoice.RejectedInvoiceNumber = yourRejectionInvoiceNo;
          muInvoice.SettlementPeriod = billingPeriod;
          muInvoice.SettlementMonth = billingMonth;
          muInvoice.SettlementYear = billingYear;
          if (muInvoice.BillingCategory == BillingCategoryType.Misc)
          {
            if (_miscInvoiceManager.ValidateMiscUatpRmLinking(muInvoice))
              return "1";
          }
          else
          {
            if (_uatpInvoiceManager.ValidateMiscUatpRmLinking(muInvoice))
              return "1";
          }
          return string.Empty;
        }
        catch (ISBusinessException exception)
        {   //CMP #678: Time Limit Validation on Last Stage MISC Rejections
            return !string.IsNullOrWhiteSpace(exception.ErrorCode) ? Messages.ResourceManager.GetString(exception.ErrorCode) : exception.Message;
        }
      }
      return "Rejection invoice not found.";
    }
  }
}
