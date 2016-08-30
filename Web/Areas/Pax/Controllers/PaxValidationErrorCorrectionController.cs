using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Text;
using System.Linq;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core.ValidationErrorCorrection;
using Iata.IS.Data.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Model.Enums.BillingType;
using Iata.IS.Core;
using Iata.IS.Data.Pax;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Core.DI;
using System.Globalization;
namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class PaxValidationErrorCorrectionController : ISController
  {

    public const string ExceptionDetailsGridAction = "ExceptionDetailsGridData";
    public const string ExceptionSummaryGridAction = "ExceptionSummaryGridData";
    public const string SamplingExceptionDetailsGridAction = "SamplingExceptionDetailsGridData";
    public const string SamplingExceptionSummaryGridAction = "SamplingExceptionSummaryGridData";
    private readonly IExceptionCodeRepository _exceptionCodeRepository;
    private readonly IInvoiceManager _invoiceManager;
    private readonly ICalendarManager _calendarManager;
    private readonly IExceptionSummarySearchResultManager _validationExceptionSummaryManager;
    private readonly IExceptionDetailSearchResultManager _validationExceptionDetail;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBillingMemoRecordRepository _billingMemoRecordRepository;
    public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }
    private readonly ISamplingFormCRecordRepository _samplingFormCRecordRepository;
    private readonly ISamplingFormCManager _samplingFormCManager;
    private readonly ISamplingFormCRepository _samplingFormCRepository;
    private readonly ISamplingFormDEManager _samplingFormDEManager;
    private readonly ISamplingFormDRepository _samplingFormDRepository;
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;

    //private readonly  _miscInvoiceRepository;
    public PaxValidationErrorCorrectionController(ICalendarManager calenderManager, IExceptionSummarySearchResultManager validationExceptionSummaryManager, IExceptionDetailSearchResultManager validationExceptionDetail, IExceptionCodeRepository exceptionCodeRepository,
      IInvoiceManager paxInvoiceManager, IInvoiceRepository paxInvoiceRepository, IBillingMemoRecordRepository billingMemoRecordRepository, ISamplingFormCRecordRepository samplingFormCRecordRepository, ISamplingFormCManager samplingFormCManager, ISamplingFormCRepository samplingFormCRepository, ISamplingFormDEManager samplingFormDeManager, ISamplingFormDRepository samplingFormDRepository,INonSamplingInvoiceManager nonSamplingInvoiceManager)
    {
      _calendarManager = calenderManager;
      _validationExceptionSummaryManager = validationExceptionSummaryManager;
      _validationExceptionDetail = validationExceptionDetail;
      _exceptionCodeRepository = exceptionCodeRepository;
      _invoiceManager = paxInvoiceManager;
      _invoiceRepository = paxInvoiceRepository;
      _billingMemoRecordRepository = billingMemoRecordRepository;
      _samplingFormCRecordRepository = samplingFormCRecordRepository;
      _samplingFormCManager = samplingFormCManager;
      _samplingFormCRepository = samplingFormCRepository;
      _samplingFormDEManager = samplingFormDeManager;
      _samplingFormDRepository = samplingFormDRepository;
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
    }
     [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ValidationErrorCorrection.Correct)]
    public ActionResult Index()
    {
      ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      Session["helplinkurl"] = "Invoices_creditnote_validation_error_correction";
      return View();
    }

    public ActionResult SamplingValidationErrorCorrection(ValidationErrorCorrection valErrorCorrection)
    {
      try
      {
        if (valErrorCorrection != null)
        {
          var currentBillingPeriodDetails = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          valErrorCorrection.BillingPeriod = -1;
          valErrorCorrection.BillingMonth = valErrorCorrection.BillingMonth == 0 ? currentBillingPeriodDetails.Month : valErrorCorrection.BillingMonth;
          valErrorCorrection.BillingYear = valErrorCorrection.BillingYear == 0 ? currentBillingPeriodDetails.Year : valErrorCorrection.BillingYear;
          valErrorCorrection.ExceptionCodeId = valErrorCorrection.ExceptionCode == null ? -1 : valErrorCorrection.ExceptionCodeId;
          valErrorCorrection.BilledMemberId = valErrorCorrection.BilledMember == null ? -1 : valErrorCorrection.BilledMemberId;
          valErrorCorrection.BillingMemberId = SessionUtil.MemberId;
          valErrorCorrection.IsFormC = true;
        }

       // string criteria = valErrorCorrection != null ? new JavaScriptSerializer().Serialize(valErrorCorrection) : string.Empty;
        int billCat = (int)BillingCategoryType.Pax;
        var exceptionSummaryGrid = new Iata.IS.Web.UIModel.Grid.Pax.SamplingExceptionSummaryGrid(ControlIdConstants.SamplingExceptionSummaryGrid, Url.Action(SamplingExceptionSummaryGridAction, new
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
            billingCategoryType = billCat
        }), false);
        ViewData[ViewDataConstants.SamplingExceptionSummaryGrid] = exceptionSummaryGrid.Instance;

        var exceptionDetailsGrid = new Iata.IS.Web.UIModel.Grid.Pax.SamplingExceptionDetailsGrid(ControlIdConstants.SamplingExceptionDetailsGrid, Url.Action(SamplingExceptionDetailsGridAction, new { }));
        ViewData[ViewDataConstants.SamplingExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
        Session["helplinkurl"] = "Formcs_validation_error_correction";

      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);
      }

      return View(valErrorCorrection);
    }

    public ActionResult ValidationErrorCorrection(ValidationErrorCorrection valErrorCorrection)
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

        //string criteria = valErrorCorrection != null ? new JavaScriptSerializer().Serialize(valErrorCorrection) : string.Empty;
        int billCat = (int)BillingCategoryType.Pax;
        var exceptionSummaryGrid = new Iata.IS.Web.UIModel.Grid.Pax.ExceptionSummaryGrid(ControlIdConstants.UatpExceptionSummaryGrid, Url.Action(ExceptionSummaryGridAction, new { period = valErrorCorrection.BillingPeriod, billingMonth = valErrorCorrection.BillingMonth, billingYear = valErrorCorrection.BillingYear, billingMemberId=valErrorCorrection.BillingMemberId,
                  billedMemberId=valErrorCorrection.BilledMemberId, invoiceNo=valErrorCorrection.InvoiceNumber, exceptionCodeId=valErrorCorrection.ExceptionCodeId,fileSubmmitDate=valErrorCorrection.FileSubmissionDate,fileName=valErrorCorrection.FileName,chargeCategoryId=valErrorCorrection.ChargeCategoryId,
                  isFormC=valErrorCorrection.IsFormC, billingCategoryType = billCat }), false);
        ViewData[ViewDataConstants.UatpExceptionSummaryGrid] = exceptionSummaryGrid.Instance;

        var exceptionDetailsGrid = new Iata.IS.Web.UIModel.Grid.Pax.ExceptionDetailsGrid(ControlIdConstants.UatpExceptionDetailsGrid, Url.Action(ExceptionDetailsGridAction, new { }));
        ViewData[ViewDataConstants.UatpExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);
      }

      return PartialView("ValidationErrorCorrection",valErrorCorrection);

    }
    
    public JsonResult ShowSearchResult(int billingPeriod, int billingMonth, int billingYear, int? exceptionCode, int? billedMemberId, string invoiceNumber, DateTime? fileSubmissionDate, string fileName)
    {
      try
      {

        var valErrorCorrection = new ValidationErrorCorrection();
        if (valErrorCorrection != null)
        {
          var currentBillingPeriodDetails =
              _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          valErrorCorrection.BillingPeriod = billingPeriod == 0
                                                 ? currentBillingPeriodDetails.Period
                                                 : billingPeriod;
          valErrorCorrection.BillingMonth = billingMonth == 0 ? currentBillingPeriodDetails.Month : billingMonth;
          valErrorCorrection.BillingYear = billingYear == 0 ? currentBillingPeriodDetails.Year : billingYear;
          valErrorCorrection.ExceptionCodeId = !exceptionCode.HasValue ? -1 : exceptionCode.Value;
          valErrorCorrection.BilledMemberId = !billedMemberId.HasValue ? -1 : billedMemberId.Value;
          valErrorCorrection.BillingMemberId = SessionUtil.MemberId;
          valErrorCorrection.FileSubmissionDate = fileSubmissionDate;
          valErrorCorrection.FileName = fileName;
          valErrorCorrection.InvoiceNumber = invoiceNumber;
        }

      
        int billCat = (int)BillingCategoryType.Pax;
        var exceptionSummaryGrid =
            new Iata.IS.Web.UIModel.Grid.Pax.ExceptionSummaryGrid(ControlIdConstants.UatpExceptionSummaryGrid,
                                                                  Url.Action(ExceptionSummaryGridAction,
                                                                             new
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
                                                                                 billingCategoryType = billCat
                                                                             }), false);
        ViewData[ViewDataConstants.UatpExceptionSummaryGrid] = exceptionSummaryGrid.Instance;

        var exceptionDetailsGrid =
            new Iata.IS.Web.UIModel.Grid.Pax.ExceptionDetailsGrid(ControlIdConstants.UatpExceptionDetailsGrid,
                                                                  Url.Action(ExceptionDetailsGridAction, new { }));
        ViewData[ViewDataConstants.UatpExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
       

        //return Json("1");
          return ExceptionSummaryGridData(valErrorCorrection.BillingPeriod, valErrorCorrection.BillingMonth,
                                          valErrorCorrection.BillingYear, valErrorCorrection.BillingMemberId,
                                          valErrorCorrection.BilledMemberId, valErrorCorrection.InvoiceNumber,
                                          valErrorCorrection.ExceptionCodeId, valErrorCorrection.FileSubmissionDate,
                                          valErrorCorrection.FileName, valErrorCorrection.ChargeCategoryId,
                                          valErrorCorrection.IsFormC, billCat);
      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);

      }
      return null;
    }

    public JsonResult ShowSearchResultPax(int billingMonth, int billingYear, int? exceptionCode, int? billedMemberId, string fileName, DateTime? fileSubmissionDate)
    {
      try
      {

        var valErrorCorrection = new ValidationErrorCorrection();
        if (valErrorCorrection != null)
        {
          var currentBillingPeriodDetails = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          valErrorCorrection.BillingPeriod = -1;
          valErrorCorrection.BillingMonth = billingMonth == 0 ? currentBillingPeriodDetails.Month : billingMonth;
          valErrorCorrection.BillingYear = billingYear == 0 ? currentBillingPeriodDetails.Year : billingYear;
          valErrorCorrection.ExceptionCodeId = !exceptionCode.HasValue ? -1 : exceptionCode.Value;
          valErrorCorrection.BilledMemberId = !billedMemberId.HasValue ? -1 : billedMemberId.Value;
          valErrorCorrection.BillingMemberId = SessionUtil.MemberId;
          valErrorCorrection.FileName = fileName;
          valErrorCorrection.FileSubmissionDate = fileSubmissionDate;
        }

        //string criteria = valErrorCorrection != null ? new JavaScriptSerializer().Serialize(valErrorCorrection) : string.Empty;
        int billCat = (int)BillingCategoryType.Pax;
        var exceptionSummaryGrid = new Iata.IS.Web.UIModel.Grid.Pax.SamplingExceptionSummaryGrid(ControlIdConstants.SamplingExceptionSummaryGrid, Url.Action(SamplingExceptionSummaryGridAction, new
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
                                                                                 isFormC = true,
                                                                                 billingCategoryType = billCat
                                                                             }), false);
        ViewData[ViewDataConstants.SamplingExceptionSummaryGrid] = exceptionSummaryGrid.Instance;

        var exceptionDetailsGrid = new Iata.IS.Web.UIModel.Grid.Pax.SamplingExceptionDetailsGrid(ControlIdConstants.SamplingExceptionDetailsGrid, Url.Action(SamplingExceptionDetailsGridAction, new { }));
        ViewData[ViewDataConstants.SamplingExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

    //    return Json("1");
        return SamplingExceptionSummaryGridData(valErrorCorrection.BillingPeriod, valErrorCorrection.BillingMonth,
                                          valErrorCorrection.BillingYear, valErrorCorrection.BillingMemberId,
                                          valErrorCorrection.BilledMemberId, valErrorCorrection.InvoiceNumber,
                                          valErrorCorrection.ExceptionCodeId, valErrorCorrection.FileSubmissionDate,
                                          valErrorCorrection.FileName, valErrorCorrection.ChargeCategoryId,
                                          true, billCat);
      }

      catch (Exception exception)
      {
        ShowErrorMessage(exception.StackTrace);

      }

      return null;

    }
    /// <summary>
    /// To get the Exception Summary Grid Data
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public JsonResult ExceptionSummaryGridData(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId,DateTime? fileSubmmitDate,
                                                        string fileName,int chargeCategoryId,bool isFormC, int? billingCategoryType = null)
    {
      var validationErrorCorrection = new ValidationErrorCorrection();


     
      if (fileSubmmitDate.HasValue)
      {
          fileSubmmitDate = fileSubmmitDate.Value.ToLocalTime();
      }
      var exceptionSummaryGrid =
          new Iata.IS.Web.UIModel.Grid.Pax.ExceptionSummaryGrid(ControlIdConstants.UatpExceptionSummaryGrid,
                                                                Url.Action(ExceptionSummaryGridAction, new { period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate, fileName, chargeCategoryId, isFormC }),
                                                          (BillingCategoryType)billingCategoryType.Value == BillingCategoryType.Pax ? true : false);
      var exceptionSummarSearchData = _validationExceptionSummaryManager.GetExceptionSummarySearchResult(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue ? fileSubmmitDate.Value:(DateTime?)null, fileName, chargeCategoryId, isFormC, BillingCategoryType.Pax).AsQueryable();
      var rowCount = exceptionSummarSearchData.Count();
      TempData["SummaryRowCount"] = rowCount;
      return exceptionSummaryGrid.DataBind(exceptionSummarSearchData);
    }

    /// <summary>
    /// To get the exception Details Data
    /// </summary>
    /// <param name="rowcells"></param>
    /// <returns></returns>
    public JsonResult ExceptionDetailsGridData(string rowcells, string billingCategoryType, string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(rowcells))
      {
        var exceptionDetailsGrid = new Iata.IS.Web.UIModel.Grid.Pax.ExceptionDetailsGrid(ControlIdConstants.UatpExceptionDetailsGrid, Url.Action(ExceptionDetailsGridAction, new { }));
        ViewData[ViewDataConstants.UatpExceptionDetailsGrid] = exceptionDetailsGrid.Instance;
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

        var exceptionRowCount = TempData["SummaryRowCount"];
        
        var invoiceSearchedData = new List<ExceptionDetailsSearchResult>();
        Guid id = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(rowcells));

          int exceptionGridRowCount = 1;
         if (exceptionRowCount != null)
          {
              if (exceptionRowCount.ToString() == "0")
              {
                  exceptionGridRowCount = 0;
              }
          }

         if (exceptionGridRowCount != 0)
        invoiceSearchedData = _validationExceptionDetail.GetExceptionDetailData(id, (Convert.ToInt32(BillingCategoryType.Pax)));
        
        foreach (ExceptionDetailsSearchResult t in invoiceSearchedData)
        {
          string errorLevel = t.ErrorLevel;

          if (errorLevel.Equals("Invoice"))
          {
            t.TransactionName = string.Empty;
          }
          else if (errorLevel.Equals("Coupon") || errorLevel.Equals("CouponTax") || errorLevel.Equals("CouponVat"))
          {
            t.TransactionName = "Coupon ";
          }
          else if (errorLevel.Equals("SamplingFormD") || errorLevel.Equals("SamplingFormDTax") || errorLevel.Equals("SamplingFormDVat"))
          {
            t.TransactionName = "Form D Coupon";
          }
          else if (errorLevel.Equals("RejectionMemo") || errorLevel.Equals("RmCoupon") || errorLevel.Equals("RmVat") || errorLevel.Equals("RmCouponTax") || errorLevel.Equals("RmCouponVat"))
          {
            t.TransactionName = "Rejection Memo";
          }
          else if (errorLevel.Equals("BillingMemo") || errorLevel.Equals("BmCoupon") || errorLevel.Equals("BmVat") || errorLevel.Equals("BmCouponTax") || errorLevel.Equals("BmCouponVat"))
          {
            t.TransactionName = "Billing Memo";
          }
          else if (errorLevel.Equals("CreditMemo") || errorLevel.Equals("CmCoupon") || errorLevel.Equals("CmVat") || errorLevel.Equals("CmCouponTax") || errorLevel.Equals("CmCouponVat"))
          {
            t.TransactionName = "Credit Memo";
          }
          else if (errorLevel.Equals("SamplingFormF") || errorLevel.Equals("FormFCoupon") || errorLevel.Equals("FormFCouponTax") || errorLevel.Equals("FormFCouponVat") || errorLevel.Equals("FormFVat"))
          {
            t.TransactionName = "Sampling Form F";
          }
          else if (errorLevel.Equals("SamplingFormXF") || errorLevel.Equals("FormXFCoupon") || errorLevel.Equals("FormXFCouponTax") || errorLevel.Equals("FormXFCouponVat") || errorLevel.Equals("FormXFVat"))
          {
            t.TransactionName = "Sampling Form XF";
          }

          t.ErrorLevelDisplay = ErrorCorrectionLevels.LevelsDisplay[t.ErrorLevel];

          if (Convert.ToInt32(t.YourInvoiceYear) != 0 && Convert.ToInt32(t.YourInvoiceMonth) != 0 && Convert.ToInt32(t.YourInvoicePeriod) != 0)
          {
            t.YourInvoiceBillingDate = t.YourInvoiceYear.ToString() + t.YourInvoiceMonth.ToString().PadLeft(2, '0') + t.YourInvoicePeriod.ToString().PadLeft(2, '0');
          }

          if (exceptionCode == "BPAXNS_10141")
          {
              //  int corrLength = t.CorrespondenceRefNo.ToString().Length;
              t.FieldValue = t.FieldValue.PadLeft(11, '0');
              t.CorrespondenceRefNo = t.FieldValue;
          }

        }

        return exceptionDetailsGrid.DataBind(invoiceSearchedData.AsQueryable());
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ExceptionCode"></param>
    /// <param name="NewValue"></param>
    /// <param name="pkReferenceId"></param>
    /// <param name="billingCategoryId"></param>
    /// <returns></returns>
    public virtual JsonResult ValidateError(string ExceptionCode, string NewValue, string pkReferenceId)
    {
      if (!string.IsNullOrWhiteSpace(ExceptionCode))
      {
        ErrorCorrectionExceptionCode exceptionCodeobj =
            ValidationErrorConfigReader.GetExceptionCodeDetails(ExceptionCode, (int)BillingCategoryType.Pax);

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
            int result = _invoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode, pkReferenceGuid);
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
    public virtual JsonResult UpdateValidationErrors(string filename, string ExceptionCode, string ErrorDescription, string FieldName, string FieldValue, string NewValue, string exceptionSummaryId, string exeptionDetailsId, string isBatchUpdate, string billingCat, string errorLevel, string pkReferenceId, DateTime lastUpdatedOn)
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
            if (result == 1) NewValue = NewValue.Trim();

          }
          else if (exceptionCodeobj.ValidationType.ToUpper().CompareTo("DIRECT") == 0)
          {
            Guid pkReferenceGuid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(pkReferenceId));
            result = _invoiceManager.ValidateForErrorCorrection(NewValue.ToUpper(), exceptionCodeobj.ExceptionCode, pkReferenceGuid);
          }
          else
          {
            result = _validationExceptionDetail.CheckForValidation(exceptionCodeobj.ExceptionCode,
                                                                   exceptionCodeobj.ValidationType,
                                                                   exceptionCodeobj.MasterTableName,
                                                                   exceptionCodeobj.MasterColumnName, NewValue.ToUpper(),
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
                                                            string.Join(",", primaryColumnName.ToArray()), billingCategory, SessionUtil.UserId, exceptionCodeobj.FieldType,lastUpdatedOn);

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

    public JsonResult IsDisplayLinkingButton(string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(exceptionCode))
      {
        var billingCat = (int)BillingCategoryType.Pax;
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
    /// To update the linking error
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual JsonResult UpdateCorrectLinkingError(ValidationErrorCorrection model)
    {
      var exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(model.ExceptionDetailId.ToString()));

      if (model.TranscationId == (int)Model.Enums.TransactionType.PaxCorrespondence) // BM
      {
        var isCorrValidatedString = ValidateBmLinkingError(model.PkReferenceId,long.Parse(model.CorrespondenceRefNo));
        if (isCorrValidatedString.CompareTo("1") != 0)
          return Json(isCorrValidatedString);
        else
        {
          //update only : YourInvoiceNo,CorrespondenceRefNo
          int isBmUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Pax, exceptiondetailId, model.YourInvoiceNo,
                                                                      model.YourInvoicePeriod,
                                                                      model.YourInvoiceMonth,
                                                                      model.YourInvoiceYear,
                                                                      string.Empty,
                                                                      string.Empty,
                                                                      0,
                                                                      0,
                                                                      long.Parse(model.CorrespondenceRefNo),
                                                                      model.LinkingDetail, SessionUtil.UserId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq, model.FimCouponNo, model.FimBmCmNo, model.LastUpdatedOn); //for BM it is 2

          return Json(isBmUpdated);
        }
      }

      else if (model.TranscationId == (int)Model.Enums.TransactionType.RejectionMemo1)
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


        if (model.ErrorLevel == "SamplingFormF")
        {
          //Note for FormF use this block

          var invoice = _invoiceRepository.First(i => i.Id == model.InvoiceID);
          var rmLinkingCriteria = new SamplingRMLinkingCriteria
                                    {
                                      ReasonCode = model.ReasonCode.ToUpper(),
                                      BilledMemberId = model.BilledMemberId,
                                      BillingMemberId = model.BillingMemberId,
                                      BillingMonth = model.YourInvoiceMonth,
                                      BillingPeriod = model.YourInvoicePeriod,
                                      BillingYear = model.YourInvoiceYear,
                                      InvoiceNumber = model.YourInvoiceNo,
                                      ProvBillingMonth = invoice.ProvisionalBillingMonth,
                                      ProvBillingYear = invoice.ProvisionalBillingYear,
                                      RejectingInvoiceId = model.InvoiceID
                                    };
          var rejectionMemoResult = Ioc.Resolve<ISamplingFormFManager>(typeof (ISamplingFormFManager)).GetLinkedFormDEDetails(rmLinkingCriteria);
          if (!string.IsNullOrWhiteSpace(rejectionMemoResult.ErrorMessage))
          {
            return Json(rejectionMemoResult.ErrorMessage);
          }
          else
          {
           
            int breakdownValidation = _validationExceptionDetail.ValidatePaxLinkingFunction(exceptiondetailId,
                                                                                            model.YourInvoiceNo,
                                                                                            model.YourInvoicePeriod,
                                                                                            model.YourInvoiceMonth,
                                                                                            model.YourInvoiceYear,
                                                                                            model.YourRejectionMemoNo,
                                                                                            model.YourBmCmNo,
                                                                                            model.BmCmIndicator,
                                                                                            -1,
                                                                                            model.BilledMemberId,
                                                                                            model.BillingMemberId);
            //Note : assign Billing to Billed and Billed to Billing

            if (breakdownValidation == 0)
            {
              return Json("No matching breakdown details found in the database.");
            }
            else
            {
                //CMP#459 Validate amounts
                IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
                var isAmountsValid = _invoiceManager.ValidateAmountofRmOnValidationErrorCorrection(exceptionDetailsList, model);
                if (!isAmountsValid)
                {
                    string errMsg = string.Empty;
                    foreach (var exceptionMsg in exceptionDetailsList)
                    {
                        errMsg += exceptionMsg.ErrorDescription + "\n";
                    }
                    return Json(errMsg);
                }
                //CMP#641: Time Limit Validation on Third Stage PAX Rejections
                var isValidTimeLimt = _invoiceManager.ValidatePaxStageThreeRmForTimeLimit(Model.Enums.TransactionType.RejectionMemo3,0,null,null,isErrorCorrection:true,exceptionDetailsList:exceptionDetailsList,errorCorrection: model);
                if (!isValidTimeLimt)
                {
                    string errMsg = string.Empty;
                    foreach (var exceptionMsg in exceptionDetailsList)
                    {
                        errMsg += exceptionMsg.ErrorDescription + "\n";
                    }
                    return Json(errMsg);
                }
               
              int isUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Pax,
                                                                                 exceptiondetailId, model.YourInvoiceNo,
                                                                                 model.YourInvoicePeriod,
                                                                                 model.YourInvoiceMonth,
                                                                                 model.YourInvoiceYear,
                                                                                 model.YourRejectionMemoNo,
                                                                                 model.YourBmCmNo, model.RejectionStage,
                                                                                 model.BmCmIndicator, 0,
                                                                                 model.LinkingDetail,
                                                                                 SessionUtil.UserId,
                                                                                 model.ProvisionalInvoiceNo,
                                                                                 model.BatchSeqNo, model.BatchRecordSeq,
                                                                                 model.FimCouponNo, model.FimBmCmNo, model.LastUpdatedOn);

              return Json(isUpdated);
            }
          }
        }
        else
        {
            var criteria = new Iata.IS.Model.Pax.Common.RMLinkingCriteria()
                               {
                                   ReasonCode = model.ReasonCode,
                                   InvoiceNumber = model.YourInvoiceNo,
                                   BillingYear = model.YourInvoiceYear,
                                   BillingMonth = model.YourInvoiceMonth,
                                   BillingPeriod = model.YourInvoicePeriod,
                                   RejectionMemoNumber = model.YourRejectionMemoNo,
                                   RejectionStage = model.RejectionStage,
                                   BillingMemberId = model.BilledMemberId,
                                   //Note : assign Billing to Billed and Billed to Billing
                                   BilledMemberId = model.BillingMemberId,
                                   RejectedInvoiceId = model.InvoiceID,
                                   IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod,
                                   FimBMCMNumber = model.FimBmCmNo,
                                   FimCouponNumber = model.FimCouponNo,
                                   FimBmCmIndicatorId = model.FimBmCmIndicator,
                                   SourceCode = model.SourceCodeId.ToString()
                               };

          Iata.IS.Model.Pax.Common.RMLinkingResultDetails rmLinkingModel =
            RejectionMemoRepository.GetRMLinkingDetails(criteria);

          if (rmLinkingModel.IsLinkingSuccessful == false)
          {
            return Json(rmLinkingModel.ErrorMessage);
          }
          else
          {
           
            int breakdownValidation = _validationExceptionDetail.ValidatePaxLinkingFunction(exceptiondetailId,
                                                                                            model.YourInvoiceNo,
                                                                                            model.YourInvoicePeriod,
                                                                                            model.YourInvoiceMonth,
                                                                                            model.YourInvoiceYear,
                                                                                            model.YourRejectionMemoNo,
                                                                                            model.FimBmCmNo,
                                                                                            model.FimBmCmIndicator,
                                                                                            model.RejectionStage,
                                                                                            model.BilledMemberId,
                                                                                            model.BillingMemberId);
            //Note : assign Billing to Billed and Billed to Billing

            if (breakdownValidation == 0)
            {
              return Json("No matching breakdown details found in the database.");
            }
            else
            {
              //CMP #614: Source Code Validation for PAX RMs.
              var sourceCodeCriteria = new Iata.IS.Model.Pax.Common.RMSourceCodeValidationCriteria()
              {
                InvoiceNumber = model.YourInvoiceNo,
                BillingYear = model.YourInvoiceYear,
                BillingMonth = model.YourInvoiceMonth,
                BillingPeriod = model.YourInvoicePeriod,
                RejectionMemoNumber = model.YourRejectionMemoNo,
                RejectionStage = model.RejectionStage,
                BillingMemberId = model.BillingMemberId,
                BilledMemberId = model.BilledMemberId,
                FimBMCMNumber = model.FimBmCmNo,
                FimCouponNumber = model.FimCouponNo,
                SourceCode = model.SourceCodeId,
                IgnoreValidationOnRMSourceCodes = SystemParameters.Instance.ValidationParams.PaxRMSourceCodes
              };
             
              //Validated rejection memo source code. 
              var sourceCodeErrorMsg = _nonSamplingInvoiceManager.ValidateRMSourceCode(sourceCodeCriteria);
              if (!String.IsNullOrEmpty(sourceCodeErrorMsg) && !sourceCodeErrorMsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              {
                return Json(String.Format("This RM has been billed with Source Code {0}. {1}", sourceCodeCriteria .SourceCode, sourceCodeErrorMsg));
              }

                //CMP#459 Validate amounts
                IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
               var isAmountsValid = _invoiceManager.ValidateAmountofRmOnValidationErrorCorrection(exceptionDetailsList, model);
                if(!isAmountsValid)
                {
                    string errMsg = string.Empty;
                    foreach (var exceptionMsg in exceptionDetailsList)
                    {
                        errMsg += exceptionMsg.ErrorDescription + "\n";
                    }
                    return Json(errMsg);
                }
                //CMP#641: Time Limit Validation on Third Stage PAX Rejections
                var isValidTimeLimt = _invoiceManager.ValidatePaxStageThreeRmForTimeLimit(Model.Enums.TransactionType.RejectionMemo3, 0, null, null, isErrorCorrection: true, exceptionDetailsList: exceptionDetailsList, errorCorrection: model);
                if (!isValidTimeLimt)
                {
                    string errMsg = string.Empty;
                    foreach (var exceptionMsg in exceptionDetailsList)
                    {
                        errMsg += exceptionMsg.ErrorDescription + "\n";
                    }
                    return Json(errMsg);
                }

                #region CMP-674-Validation of Coupon and AWB Breakdowns in Rejections

                /* CMP#674 validations will be applicable only to the following transactions - PAX Non-Sampling Stage 2 RMs, PAX Non-Sampling Stage 3 RMs
                   and PAX Sampling Form X/Fs (Stage 3 RMs) */
               
                /* Fetch Invoice */
                var invoice = _invoiceRepository.First(i => i.Id == model.InvoiceID);

                if ((invoice.BillingCode == (int)Iata.IS.Model.Pax.Enums.BillingCode.NonSampling || invoice.BillingCode == (int)Iata.IS.Model.Pax.Enums.BillingCode.SamplingFormXF) && (model.RejectionStage == 2 || model.RejectionStage == 3))
                {
                    List<InvalidRejectionMemoDetails> invalidRejectionMemos =
                        RejectionMemoRepository.IsYourRejectionCouponDropped(model.InvoiceID, model.PkReferenceId,
                                                                             model.YourInvoiceNo,
                                                                             model.YourRejectionMemoNo,
                                                                             model.YourInvoiceYear,
                                                                             model.YourInvoiceMonth,
                                                                             model.YourInvoicePeriod);

                    foreach (InvalidRejectionMemoDetails invalidRM in invalidRejectionMemos)
                    {
                        /* Report Error -
                         * Error Code - RejectionMemoCouponMissing = "BPAXNS_10978"
                         * Error Description - Mismatch in coupon {0}-{1}-{2}. It was billed {3} time(s) in the rejected RM; and {4} time(s) in this RM. 
                         *                     Other mismatches if any are not included in this report.
                         */
                        var errorDescription = Messages.ResourceManager.GetString(ErrorCodes.PaxRMCouponMismatchFileValidation);

                        errorDescription = string.Format(errorDescription, 
                                                         invalidRM.TicketIssuingAirline, invalidRM.TicketDocOrAwbNumber,
                                                         invalidRM.CouponNumber,
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

              int isUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Pax,
                                                                                 exceptiondetailId, model.YourInvoiceNo,
                                                                                 model.YourInvoicePeriod,
                                                                                 model.YourInvoiceMonth,
                                                                                 model.YourInvoiceYear,
                                                                                 model.YourRejectionMemoNo,
                                                                                 model.YourBmCmNo, model.RejectionStage,
                                                                                 model.BmCmIndicator, 0,
                                                                                 model.LinkingDetail, SessionUtil.UserId,
                                                                                 model.ProvisionalInvoiceNo,
                                                                                 model.BatchSeqNo, model.BatchRecordSeq,
                                                                                 model.FimCouponNo == 0 ? null : model.FimCouponNo,
                                                                                 model.FimBmCmNo, model.LastUpdatedOn);

              return Json(isUpdated);
            }
          }
        }
      }
      else if (model.TranscationId == (int)Model.Enums.TransactionType.SamplingFormD)
      {

        var isCorrValidatedString = ValidateFormdLinkingError(model.PkReferenceId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq); //ValidateFormLinkingError(model.PkReferenceId, model.CorrespondenceRefNo);
        if (isCorrValidatedString.CompareTo("1") != 0)
          return Json(isCorrValidatedString);
        else
        {
          int breakdownValidation =
              _validationExceptionDetail.ValidatePaxSamplingLinkingFunction(exceptiondetailId,
                                                                            model.ProvisionalInvoiceNo,
                                                                            model.BatchSeqNo,
                                                                            model.BatchRecordSeq, false);

          if (breakdownValidation == 0)
          {
            return Json("No matching breakdown details found in the database.");
          }
          else
          {
            int isFormDUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Pax, exceptiondetailId, model.YourInvoiceNo,
                                                                     model.YourInvoicePeriod,
                                                                     model.YourInvoiceMonth,
                                                                     model.YourInvoiceYear,
                                                                     string.Empty,
                                                                     string.Empty,
                                                                     0,
                                                                     0,
                                                                     long.Parse(model.CorrespondenceRefNo),
                                                                     model.LinkingDetail, SessionUtil.UserId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq, model.FimCouponNo, model.FimBmCmNo, model.LastUpdatedOn); //for BM it is 2

            return Json(isFormDUpdated);

          }

        }



      }
      //

      return null;
    }

    /// <summary>
    /// To update the linking error for Pax Sampling Form C
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public virtual JsonResult SamplingUpdateCorrectLinkingError(ValidationErrorCorrection model)
    {
      var exceptiondetailId = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(model.ExceptionDetailId.ToString()));

      if (model.TranscationId == (int)Model.Enums.TransactionType.SamplingFormC) // BM
      {
        var isCorrValidatedString = ValidateSamplingLinkingError(model.PkReferenceId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq);
        if (isCorrValidatedString.CompareTo("1") != 0)
          return Json(isCorrValidatedString);


        int breakdownValidation = _validationExceptionDetail.ValidatePaxSamplingLinkingFunction(exceptiondetailId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq, true);//Note : assign Billing to Billed and Billed to Billing

        if (breakdownValidation == 0)
        {
          return Json("No matching breakdown details found in the database.");
        }
        else
        {

          int isUpdated = _validationExceptionDetail.UpdateLinkErrorFunction(BillingCategoryType.Pax, exceptiondetailId, model.YourInvoiceNo,
                                                                   model.YourInvoicePeriod,
                                                                   model.YourInvoiceMonth,
                                                                   model.YourInvoiceYear,
                                                                   model.YourRejectionMemoNo, model.YourBmCmNo, model.RejectionStage,
                                                                   model.BmCmIndicator, 0,
                                                                   model.LinkingDetail, SessionUtil.UserId, model.ProvisionalInvoiceNo, model.BatchSeqNo, model.BatchRecordSeq, model.FimCouponNo, model.FimBmCmNo, model.LastUpdatedOn);

          return Json(isUpdated);
        }
      }

      return null;
    }

    public string ValidateBmLinkingError(Guid billingMemoId, long corrReferenceNo)
    {
      var billingMemo = _billingMemoRecordRepository.First(i => i.Id == billingMemoId);
      if (billingMemo != null)
      {
        var invoiceId = billingMemo.InvoiceId;
        var paxInvoice = _invoiceRepository.First(i => i.Id == invoiceId);
        if (paxInvoice != null)
        {
          billingMemo.CorrespondenceRefNumber = corrReferenceNo;
          try
          {
            _invoiceManager.ValidateCorrespondenceReference(billingMemo, false, paxInvoice);
            return "1";
          }
          catch (ISBusinessException exception)
          {
            return Messages.ResourceManager.GetString(exception.ErrorCode);
          }
        }
        else
        {
          return "Invoice not found.";
        }
      }
      return "Billing memo not found.";
    }

    /// <summary>
    /// To get the Exception Summary Grid Data
    /// </summary>
    /// <param name="criteria"></param>
    /// <returns></returns>
    public JsonResult SamplingExceptionSummaryGridData(int period, int billingMonth, int billingYear, int billingMemberId, int billedMemberId, string invoiceNo, int exceptionCodeId,DateTime? fileSubmmitDate,
                                                        string fileName,int chargeCategoryId,bool isFormC, int? billingCategoryType = null)
    {
      var validationErrorCorrection = new ValidationErrorCorrection() { IsFormC = true };



        if (fileSubmmitDate.HasValue)
        {
            fileSubmmitDate = fileSubmmitDate.Value.ToLocalTime();
        }

      var exceptionSummaryGrid =
          new Iata.IS.Web.UIModel.Grid.Pax.SamplingExceptionSummaryGrid(ControlIdConstants.SamplingExceptionSummaryGrid,
                                                                Url.Action(SamplingExceptionSummaryGridAction, new { period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate, fileName, chargeCategoryId, isFormC }),
                                                          (BillingCategoryType)billingCategoryType.Value == BillingCategoryType.Pax ? true : false);
      var exceptionSummarSearchData = _validationExceptionSummaryManager.GetExceptionSummarySearchResult(period, billingMonth, billingYear, billingMemberId, billedMemberId, invoiceNo, exceptionCodeId, fileSubmmitDate.HasValue ? fileSubmmitDate.Value:(DateTime?)null, fileName, chargeCategoryId, isFormC, BillingCategoryType.Pax).AsQueryable();
      return exceptionSummaryGrid.DataBind(exceptionSummarSearchData);
    }

    /// <summary>
    /// To get the exception Details Data
    /// </summary>
    /// <param name="rowcells"></param>
    /// <returns></returns>
    public JsonResult SamplingExceptionDetailsGridData(string rowcells, string billingCategoryType, string exceptionCode)
    {
      if (!string.IsNullOrWhiteSpace(rowcells))
      {

        var esGrid = new SamplingExceptionDetailsGrid(ControlIdConstants.SamplingExceptionDetailsGrid, Url.Action(SamplingExceptionDetailsGridAction));

        List<ExceptionDetailsSearchResult> invoiceSearchedData;
        Guid id = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(rowcells));

        invoiceSearchedData = _validationExceptionDetail.GetExceptionDetailData(id, (Convert.ToInt32(BillingCategoryType.Pax)));

        foreach (ExceptionDetailsSearchResult t in invoiceSearchedData)
        {
          int billingCode = Convert.ToInt32(t.BillingCode);
          switch (billingCode)
          {
            case 1:
              t.TransactionType = "Prepaid AWB";
              break;

            case 2:
              t.TransactionType = "Collect AWB";
              break;

            case 3:
              t.TransactionType = "Billing Memo";
              break;

            case 4:
              t.TransactionType = "Credit Memo";
              break;

            case 5:
              t.TransactionType = "Rejection Memo";
              break;

            default:
              t.TransactionType = " ";
              break;
          }

          if (Convert.ToInt32(t.YourInvoiceYear) != 0 && Convert.ToInt32(t.YourInvoiceMonth) != 0 && Convert.ToInt32(t.YourInvoicePeriod) != 0)
          {
            t.YourInvoiceBillingDate = t.YourInvoiceYear.ToString() + t.YourInvoiceMonth.ToString().PadLeft(2, '0') + t.YourInvoicePeriod.ToString().PadLeft(2, '0');
          }

          if (exceptionCode == "BPAXNS_10141")
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


    public string ValidateSamplingLinkingError(Guid samplingFormcRecordId, string provisionalInvoiceNo, int batchSeqNo, int batchRecordSeq)
    {
      var samplingFormcRecord = _samplingFormCRecordRepository.First(i => i.Id == samplingFormcRecordId);
      if (samplingFormcRecord != null)
      {
        var samplingFormC = _samplingFormCRepository.First(i => i.Id == samplingFormcRecord.SamplingFormCId);
        if (samplingFormC != null)
        {
          samplingFormcRecord.BatchNumberOfProvisionalInvoice = batchSeqNo;
          samplingFormcRecord.RecordSeqNumberOfProvisionalInvoice = batchRecordSeq;
          samplingFormcRecord.ProvisionalInvoiceNumber = provisionalInvoiceNo;
          try
          {
            _samplingFormCManager.ValidateSamplingFormCRecord(samplingFormcRecord, samplingFormcRecord, samplingFormC);
            return "1";
          }
          catch (ISBusinessException exception)
          {
            return Messages.ResourceManager.GetString(exception.ErrorCode);
          }
        }
        else
        {
          return "Sampling Form C not found";
        }


      }
      else
      {
        return "Sampling Form C Record not found";
      }
    }

    public string ValidateFormdLinkingError(Guid samplingFormDRecordId, string provisionalInvoiceNo, int batchSeqNo, int batchRecordSeq)
    {
      var samplingFormDRecord = _samplingFormDRepository.Single(samplingFormDRecordId);
      if (samplingFormDRecord != null)
      {
        var paxInvoice = _invoiceRepository.First(i => i.Id == samplingFormDRecord.InvoiceId);
        if (paxInvoice != null)
        {
          samplingFormDRecord.ProvisionalInvoiceNumber = provisionalInvoiceNo;
          samplingFormDRecord.BatchNumberOfProvisionalInvoice = batchSeqNo;
          samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice = batchRecordSeq;
          try
          {
            var error = _samplingFormDEManager.ValidateFormDRecord(samplingFormDRecord, null, paxInvoice,true);
            if (!string.IsNullOrWhiteSpace(error) || error.Length > 0)
              return error;
            return "1";
          }
          catch (ISBusinessException exception)
          {
            return Messages.ResourceManager.GetString(exception.ErrorCode);
          }
        }
        else
        {
          return "Invoice not found";
        }
      }
      else
      {
        return "Form D Record not found ";
      }

    }

    [HttpPost]
    public virtual JsonResult ClearSearch()
    {
      var defaultPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      return Json(defaultPeriod);
    }
  }
}
