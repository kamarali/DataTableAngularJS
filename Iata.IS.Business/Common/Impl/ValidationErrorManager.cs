using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Core;
using Iata.IS.Data;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Enums;
using System.IO;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Business.Common.Impl
{
  public class ValidationErrorManager : IValidationErrorManager
  {
    /// <summary>
    /// Gets or sets the web validation error repository.
    /// </summary>
    /// <value>The web validation error repository.</value>
    public IRepository<WebValidationError> WebValidationErrorRepository { get; set; }

    public IInvoiceRepository InvoiceRepository { get; set; }

    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    public IQueryable<WebValidationError> GetValidationErrors(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      return WebValidationErrorRepository.Get(validationErrors => validationErrors.InvoiceId == invoiceGuid);
    }

    /// <summary>
    /// Gets the web validation error.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns></returns>
    public WebValidationError GetWebValidationError(Guid invoiceId, string errorCode, string errorMessage = null)
    {
      var errorDescription = !string.IsNullOrEmpty(errorMessage) ? errorMessage : string.Empty;

      return new WebValidationError
      {
        ErrorCode = errorCode,
        ErrorDescription = Messages.ResourceManager.GetString(errorCode) + errorDescription,
        InvoiceId = invoiceId
      };
    }

    /// <summary>
    /// Gets the web validation error.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="args">input parameters for message format.</param>
    /// <returns></returns>
    public WebValidationError GetWebValidationError(string errorCode, Guid invoiceId, params string[] args)
    {

      return new WebValidationError
      {
        ErrorCode = errorCode,
        ErrorDescription = string.Format( Messages.ResourceManager.GetString(errorCode), args),
        InvoiceId = invoiceId
      };
    }

    /// <summary>
    /// Updates the validation errors.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="webValidationErrors">The web validation errors.</param>
    /// <param name="validationErrorsInDb">The validation errors in db.</param>
    public void UpdateValidationErrors(Guid invoiceId, List<WebValidationError> webValidationErrors, IEnumerable<WebValidationError> validationErrorsInDb)
    {
      var listToDeleteValidationError = validationErrorsInDb.Where(validationErrors => webValidationErrors.Count(vError => vError.Id == validationErrors.Id) == 0).ToList();

      foreach (var validationError in webValidationErrors.Where(vErr => vErr.Id.CompareTo(new Guid()) == 0))
      {
        validationError.InvoiceId = invoiceId;
          //SCP325375:File Loading & Web Response Stats ManageInvoice
        // WebValidationErrorRepository.Add(validationError);
        InvoiceRepository.AddWebValiadtionErrorEntry(validationError);

      }

      foreach (var validationError in listToDeleteValidationError)
      {
         //SCP325375:File Loading & Web Response Stats ManageInvoice
        //WebValidationErrorRepository.Delete(validationError);
          InvoiceRepository.DeleteWebValiadtionError(validationError.Id);
      }
    }

    /// <summary>
    /// To get the ValidationdetailBase report.
    /// </summary>
    /// <param name="paxExceptionDetails"></param>
    /// <returns></returns>
    public List<ValidationExceptionDetailBase> GetBaseReport(IList<IsValidationExceptionDetail> paxExceptionDetails , int errorSerialNumber = 0)
    {
      List<ValidationExceptionDetailBase> ValidationErrorReport = new List<ValidationExceptionDetailBase>();

      foreach (ValidationExceptionDetailBase validationRecord in paxExceptionDetails)
      {
        var ValidationError = new ValidationExceptionDetailBase();
        ValidationError.SerialNo = ++errorSerialNumber;
        ValidationError.BilledEntityCode = validationRecord.BilledEntityCode;
        ValidationError.BillingEntityCode = validationRecord.BillingEntityCode;
        ValidationError.ClearanceMonth = validationRecord.ClearanceMonth;
        ValidationError.PeriodNumber = validationRecord.PeriodNumber;
        ValidationError.FieldName = validationRecord.FieldName;
        if (validationRecord.BillingFileSubmissionDate != null)
          ValidationError.BillingFileSubmissionDate = validationRecord.BillingFileSubmissionDate;
        if (validationRecord.SubmissionFormat != null)
          ValidationError.SubmissionFormat = Enum.Parse(typeof(SubmissionMethod), validationRecord.SubmissionFormat).ToString().ToUpper().Replace("IS","IS-");
        ValidationError.InvoiceNumber = validationRecord.InvoiceNumber;
        ValidationError.CategoryOfBilling = Enum.Parse(typeof(BillingCategoryType), validationRecord.CategoryOfBilling).ToString().Substring(0, 1).ToUpper();
        ValidationError.FileName = validationRecord.FileName;

        ValidationError.LineItemOrBatchNo = validationRecord.LineItemOrBatchNo;
        ValidationError.LineItemDetailOrSequenceNo = validationRecord.LineItemDetailOrSequenceNo;
        ValidationError.DocumentNo = validationRecord.DocumentNo;
        ValidationError.LinkedDocNo = validationRecord.LinkedDocNo;
        ValidationError.ChargeCategoryOrBillingCode = validationRecord.ChargeCategoryOrBillingCode;
        ValidationError.SourceCodeId = validationRecord.SourceCodeId;
        ValidationError.ErrorStatus = Enum.Parse(typeof(ErrorStatus), validationRecord.ErrorStatus).ToString();
        ValidationError.ErrorDescription = validationRecord.ErrorDescription;
        ValidationError.ExceptionCode = validationRecord.ExceptionCode;
        ValidationError.ErrorLevel = validationRecord.ErrorLevel;
        ValidationError.FieldName = validationRecord.FieldName;
        ValidationError.FieldValue = validationRecord.FieldValue;
        ValidationErrorReport.Add(ValidationError);
      }

      return ValidationErrorReport;
    }

    /// <summary>
    /// To get Validation summary.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="ValidationErrorReport"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <returns></returns>
    public IsValidationExceptionSummary GetIsSummary(object invoice, IList<IsValidationExceptionDetail> ValidationErrorReport, string fileName, DateTime fileSubmissionDate)
    {
      IsValidationExceptionSummary validationSummaryReport = new IsValidationExceptionSummary();

      if (invoice == null)
      {
        return validationSummaryReport;
      }
      PaxInvoice paxInvoiceList = null;
      MiscUatpInvoice miscInvoiceList = null;
      CargoInvoice cgoInvoiceList = null;

      bool isPaxInvoice = false;
      bool isMiscInvoice = false;
      var isCgoInvoice = false;

      string submissionFormat = string.Empty;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();
      }

      if (invoice.GetType().Name.Equals("PaxInvoice"))
      {
        isPaxInvoice = true;
        paxInvoiceList = (PaxInvoice)invoice;
      }
      else if (invoice.GetType().Name.Equals("MiscUatpInvoice"))
      {
        isMiscInvoice = true;
        miscInvoiceList = (MiscUatpInvoice)invoice;
      }
      else if (invoice.GetType().Name.Equals("CargoInvoice"))
      {
        isCgoInvoice = true;
        cgoInvoiceList = (CargoInvoice) invoice;
      }
      else
      {
        return GetFormCSummaryReport((SamplingFormC)invoice, ValidationErrorReport, fileName, fileSubmissionDate, submissionFormat);
      }

      var invoiceBase = (InvoiceBase)invoice;

      validationSummaryReport.BillingFileName = Path.GetFileName(fileName);
      validationSummaryReport.SubmissionFormat = submissionFormat;
      validationSummaryReport.BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd");

      validationSummaryReport.InvoiceNumber = invoiceBase.InvoiceNumber;
      validationSummaryReport.BillingEntityCode = invoiceBase.BillingMember != null ? invoiceBase.BillingMember.MemberCodeNumeric : string.Empty;
      validationSummaryReport.ClearanceMonth = invoiceBase.BillingYear.ToString() + invoiceBase.BillingMonth.ToString().PadLeft(2, '0');
      validationSummaryReport.PeriodNumber = invoiceBase.BillingPeriod;
      validationSummaryReport.CategoryOfBilling = invoiceBase.BillingCategoryId.ToString();
      validationSummaryReport.BilledEntityCode = invoiceBase.BilledMember != null ? invoiceBase.BilledMember.MemberCodeNumeric : string.Empty;
      validationSummaryReport.InvoiceStatus = ((int)invoiceBase.InvoiceStatus).ToString();

      if (ValidationErrorReport.Count > 0)
      {

        if (ValidationErrorReport.Count(invNo => invNo.InvoiceNumber == invoiceBase.InvoiceNumber && (invNo.ErrorLevel == ErrorLevels.ErrorLevelInvoice || invNo.ErrorLevel == ErrorLevels.ErrorLevelInvoiceTotal || invNo.ErrorLevel == ErrorLevels.ErrorLevelSamplingFormE)) > 0)
        {
          validationSummaryReport.ErrorAtInvoiceLevel = "1";
        }
        else
        {
          validationSummaryReport.ErrorAtInvoiceLevel = "0";
        }
      }

      if (isPaxInvoice)
      {

        if (paxInvoiceList.BillingCurrency.HasValue)
        {
          validationSummaryReport.CurrencyOfBilling = paxInvoiceList.BillingCurrencyId.Value.ToString();
        }

      
        validationSummaryReport.TotalNoOfBillingRecords = paxInvoiceList.CouponDataRecord.Count() + paxInvoiceList.BillingMemoRecord.Count() + paxInvoiceList.CreditMemoRecord.Count() + paxInvoiceList.RejectionMemoRecord.Count() + paxInvoiceList.SamplingFormDRecord.Count();

        if (paxInvoiceList.InvoiceTotalRecord != null && paxInvoiceList.BillingCode != (int)BillingCode.SamplingFormDE)
        {
          validationSummaryReport.InvoiceAmountInBillingCurrency = ConvertUtil.Round(paxInvoiceList.InvoiceTotalRecord.NetBillingAmount, Constants.PaxDecimalPlaces);
        }
        else if (paxInvoiceList.SamplingFormEDetails != null && paxInvoiceList.BillingCode == (int)BillingCode.SamplingFormDE)
        {
            //SCPID 117972 - Validation Report showing Form D data instead of settlement amount from Form E 
            validationSummaryReport.InvoiceAmountInBillingCurrency = ConvertUtil.Round(paxInvoiceList.SamplingFormEDetails.NetBilledCreditedAmount, Constants.PaxDecimalPlaces); 
        }

        if (ValidationErrorReport.Count > 0)
        {
          var erorRecords = (from error in ValidationErrorReport where error.InvoiceNumber == paxInvoiceList.InvoiceNumber && error.DocumentNo != null && error.DocumentNo != "0" && !string.IsNullOrEmpty(error.DocumentNo.Trim()) select error.DocumentNo.Trim()).Distinct().Count();
          validationSummaryReport.RecordsInValidationError = erorRecords;// erorRecords.Select(docNo => docNo.DocumentNo).Distinct().Count();
        }

        validationSummaryReport.RecordsSuccessfullyValidated = validationSummaryReport.TotalNoOfBillingRecords - validationSummaryReport.RecordsInValidationError;

      }
      else if (isMiscInvoice && miscInvoiceList != null)
      {

        if (miscInvoiceList.ListingCurrencyId.HasValue)
        {
          validationSummaryReport.CurrencyOfBilling = miscInvoiceList.ListingCurrencyId.Value.ToString();
        }

        int totalBilingRecordCount = 0;
        foreach (var lineItem in miscInvoiceList.LineItems)
        {
          totalBilingRecordCount += lineItem.LineItemDetails.Count();
        }

        if (totalBilingRecordCount == 0)
        {
          validationSummaryReport.TotalNoOfBillingRecords = miscInvoiceList.LineItems.Count();
        }
        else
        {
          validationSummaryReport.TotalNoOfBillingRecords = totalBilingRecordCount;
        }

        if (miscInvoiceList.InvoiceSummary != null)
        {
          validationSummaryReport.InvoiceAmountInBillingCurrency = ConvertUtil.Round(miscInvoiceList.InvoiceSummary.TotalAmount, Constants.MiscDecimalPlaces); 
        }

        if (ValidationErrorReport.Count > 0)
        {
          var erorLineItemDetail = from error in ValidationErrorReport where error.InvoiceNumber == miscInvoiceList.InvoiceNumber && error.LineItemDetailOrSequenceNo != 0 select error;
          validationSummaryReport.RecordsInValidationError = erorLineItemDetail.Select(lineItemDetail => lineItemDetail.LineItemDetailOrSequenceNo).Distinct().Count();
          if (validationSummaryReport.RecordsInValidationError == 0)
          {
            var erorLineItem = from error in ValidationErrorReport where error.InvoiceNumber == miscInvoiceList.InvoiceNumber && error.LineItemOrBatchNo != 0 select error;
            validationSummaryReport.RecordsInValidationError = erorLineItem.Select(lineItem => lineItem.LineItemOrBatchNo).Distinct().Count();
          }
        }

        validationSummaryReport.RecordsSuccessfullyValidated = validationSummaryReport.TotalNoOfBillingRecords - validationSummaryReport.RecordsInValidationError;

      }
      else if (isCgoInvoice && cgoInvoiceList != null)
      {
        if (cgoInvoiceList.BillingCurrencyId.HasValue)
        {
          validationSummaryReport.CurrencyOfBilling = cgoInvoiceList.BillingCurrencyId.Value.ToString();
        }

        validationSummaryReport.TotalNoOfBillingRecords = cgoInvoiceList.AwbDataRecord.Count() + cgoInvoiceList.CGOBillingMemo.Count() + cgoInvoiceList.CGOCreditMemo.Count() + cgoInvoiceList.CGORejectionMemo.Count();

        if (cgoInvoiceList.CGOInvoiceTotal != null)
        {
          validationSummaryReport.InvoiceAmountInBillingCurrency = ConvertUtil.Round(cgoInvoiceList.CGOInvoiceTotal.NetBillingAmount, Constants.CgoDecimalPlaces);
        }

        if (ValidationErrorReport.Count > 0)
        {
          var erorRecords = (from error in ValidationErrorReport where error.InvoiceNumber == cgoInvoiceList.InvoiceNumber && error.DocumentNo != null && error.DocumentNo != "0" && !string.IsNullOrEmpty(error.DocumentNo.Trim()) select error.DocumentNo.Trim()).Distinct().Count();
          validationSummaryReport.RecordsInValidationError = erorRecords;
        }

        validationSummaryReport.RecordsSuccessfullyValidated = validationSummaryReport.TotalNoOfBillingRecords - validationSummaryReport.RecordsInValidationError;
      }

      return validationSummaryReport;
    }

    /// <summary>
    /// To get Summary for samplingFormC
    /// </summary>
    /// <param name="samplingRecord"></param>
    /// <param name="validationErrorReport"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="submissionFormat"></param>
    /// <returns></returns>
    private IsValidationExceptionSummary GetFormCSummaryReport(SamplingFormC samplingRecord, IList<IsValidationExceptionDetail> validationErrorReport, string fileName, DateTime fileSubmissionDate, string submissionFormat)
    {
      var validationSummaryReport = new IsValidationExceptionSummary();

      validationSummaryReport.BillingFileName = Path.GetFileName(fileName);
      validationSummaryReport.SubmissionFormat = submissionFormat;
      validationSummaryReport.BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd");

      validationSummaryReport.InvoiceNumber = samplingRecord.SamplingFormCDetails.Count() != 0 ? samplingRecord.SamplingFormCDetails[0].ProvisionalInvoiceNumber : string.Empty;
      validationSummaryReport.BillingEntityCode = samplingRecord.FromMember != null ? samplingRecord.FromMember.MemberCodeNumeric : string.Empty;
      validationSummaryReport.ClearanceMonth = samplingRecord.ProvisionalBillingYear.ToString() + samplingRecord.ProvisionalBillingMonth.ToString().PadLeft(2, '0');
      validationSummaryReport.CategoryOfBilling = ((int)BillingCategoryType.Pax).ToString();
      validationSummaryReport.BilledEntityCode = samplingRecord.ProvisionalBillingMember != null ? samplingRecord.ProvisionalBillingMember.MemberCodeNumeric : string.Empty;

      if (samplingRecord.ListingCurrency != null)
        validationSummaryReport.CurrencyOfBilling = samplingRecord.ListingCurrencyId.ToString();
      validationSummaryReport.InvoiceAmountInBillingCurrency = ConvertUtil.Round(samplingRecord.NetAmount, Constants.PaxDecimalPlaces); 

      validationSummaryReport.InvoiceStatus = ((int)samplingRecord.InvoiceStatus).ToString();

      if (validationErrorReport.Count > 0)
      {
        if (validationErrorReport.Count(provMonth => provMonth.ClearanceMonth == samplingRecord.ProvisionalBillingYear.ToString() + samplingRecord.ProvisionalBillingMonth.ToString().PadLeft(2, '0') && provMonth.ErrorLevel == ErrorLevels.ErrorLevelSamplingFormC) > 0)
        {
          validationSummaryReport.ErrorAtInvoiceLevel = "1";
        }
        else
        {
          validationSummaryReport.ErrorAtInvoiceLevel = "0";
        }

        var erorRecords = from error in validationErrorReport where error.ClearanceMonth == samplingRecord.ProvisionalBillingYear.ToString() + samplingRecord.ProvisionalBillingMonth.ToString().PadLeft(2, '0') && error.DocumentNo != null select error;
        validationSummaryReport.RecordsInValidationError = erorRecords.Select(docNo => docNo.DocumentNo).Distinct().Count();
      }
      validationSummaryReport.TotalNoOfBillingRecords = samplingRecord.SamplingFormCDetails.Count();

      validationSummaryReport.RecordsSuccessfullyValidated = validationSummaryReport.TotalNoOfBillingRecords - validationSummaryReport.RecordsInValidationError;

      return validationSummaryReport;
    }

    /// <summary>
    /// To get the list of ValidationException summary. 
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="isValidationExceptionDetails"></param>
    /// <returns></returns>
    public List<ValidationExceptionSummary> GetIsSummaryForValidationErrorCorrection(object invoice, List<IsValidationExceptionDetail> isValidationExceptionDetails)
    {
      PaxInvoice paxInvoice;
      MiscUatpInvoice miscUatpInvoice;
      CargoInvoice cargoInvoice;
      int chargeCategoryId = 0;
      var invoiceType = string.Empty;
      bool isFormC = false;
   
      if (invoice.GetType().Name.Equals("PaxInvoice"))
      {
       paxInvoice = (PaxInvoice)invoice;
      }
      else if (invoice.GetType().Name.Equals("MiscUatpInvoice"))
      {
        miscUatpInvoice = (MiscUatpInvoice)invoice;
        chargeCategoryId = miscUatpInvoice.ChargeCategoryId;
        invoiceType = GetInvoiceType(miscUatpInvoice.InvoiceTypeId);
      }
      else if (invoice.GetType().Name.Equals("CargoInvoice"))
      {
        cargoInvoice = (CargoInvoice)invoice;
      }
      else
      {
        isFormC = true;
      }

      var validationExceptionSummaryList = new List<ValidationExceptionSummary>();

      if (!isFormC)
      {
        var correctableErrors = isValidationExceptionDetails.FindAll(err => Convert.ToInt32(err.ErrorStatus) == (int)ErrorStatus.C && err.InvoiceNumber.CompareTo(((InvoiceBase)invoice).InvoiceNumber) == 0);


        var invoiceBase = (InvoiceBase)invoice;

        foreach (var isValidationGroupExceptionDetails in correctableErrors.GroupBy(cdr => new { cdr.ExceptionCode }))
        {
          var expDetail = isValidationGroupExceptionDetails.ToList()[0];
          var validationSummary = new ValidationExceptionSummary() { Id = Guid.NewGuid() };
          validationSummary.InvoiceId = invoiceBase.Id.Value();
          validationSummary.InvoiceNo = invoiceBase.InvoiceNumber;
          validationSummary.InvoiceType = invoiceType;
          validationSummary.FileLogId = invoiceBase.IsInputFileId.HasValue ? invoiceBase.IsInputFileId.Value.Value() : string.Empty;
          validationSummary.OnlineCorrectionStaus = 0;
          validationSummary.BilledMemberId = invoiceBase.BilledMemberId;
          validationSummary.ChargeCategoryId = chargeCategoryId;
          validationSummary.ExceptionCodeId = GetExceptionCodeId(expDetail.ExceptionCode, invoiceBase.BillingCategory);
          validationSummary.IsFormC = false; //It is non sampling case
          validationSummary.ProvInvoiceNo = expDetail.ProvInvoiceNo;
          foreach (var validationDetail in isValidationGroupExceptionDetails)
          {
            var validationDetailObj = new IsValidationExceptionDetail(validationDetail);
            validationDetailObj.ValidationExceptionId = validationSummary.Id.Value();
            validationDetailObj.ErrorCorrectionLevel = ErrorCorrectionLevels.Levels.ContainsKey(validationDetail.ErrorLevel) ? ErrorCorrectionLevels.Levels[validationDetail.ErrorLevel] : validationDetail.ErrorLevel;
            validationSummary.ValidationExceptionDetails.Add(validationDetailObj);
          }
          validationExceptionSummaryList.Add(validationSummary);
        }
      }
      else
      {
        //It is Form C
        var correctableErrors = isValidationExceptionDetails.FindAll(err => Convert.ToInt32(err.ErrorStatus) == (int) ErrorStatus.C);
        var formC = invoice as SamplingFormC;
        if (formC != null)
        {
          foreach (var isValidationGroupExceptionDetails in correctableErrors.GroupBy(cdr => new { cdr.ExceptionCode }))
          {
            var expDetail = isValidationGroupExceptionDetails.ToList()[0];
            var validationSummary = new ValidationExceptionSummary() { Id = Guid.NewGuid() };
            validationSummary.InvoiceId = formC.Id.Value();
            validationSummary.InvoiceNo = string.Empty; 
            validationSummary.InvoiceType = invoiceType;
            validationSummary.FileLogId = formC.IsInputFileDisplayId;
            validationSummary.OnlineCorrectionStaus = 0;
            validationSummary.BilledMemberId = formC.ProvisionalBillingMemberId;
            validationSummary.ChargeCategoryId = chargeCategoryId;
            validationSummary.ExceptionCodeId = GetExceptionCodeId(expDetail.ExceptionCode,BillingCategoryType.Pax);
            validationSummary.IsFormC = true; //It is non sampling case
            validationSummary.ProvInvoiceNo = expDetail.ProvInvoiceNo;
            foreach (var validationDetail in isValidationGroupExceptionDetails)
            {
              var validationDetailObj = new IsValidationExceptionDetail(validationDetail);
              validationDetailObj.ValidationExceptionId = validationSummary.Id.Value();
              validationDetailObj.ErrorCorrectionLevel = ErrorCorrectionLevels.Levels.ContainsKey(validationDetail.ErrorLevel) ? ErrorCorrectionLevels.Levels[validationDetail.ErrorLevel] : validationDetail.ErrorLevel;
              validationSummary.ValidationExceptionDetails.Add(validationDetailObj);
            }
            validationExceptionSummaryList.Add(validationSummary);
          }
        }
      }
      return validationExceptionSummaryList;
    }

    /// <summary>
    /// Gets exception code id of respective exception code
    /// </summary>
    /// <param name="exceptionCodeName"></param>
    /// <param name="billingCategoryType"></param>
    /// <returns></returns>
    private static int GetExceptionCodeId(string exceptionCodeName, BillingCategoryType billingCategoryType)
    {
      if (string.IsNullOrEmpty(exceptionCodeName))
        return 0;
      if (ValidationCache.Instance.ExceptionCodeList != null)
      {
        var exceptionCode = ValidationCache.Instance.ExceptionCodeList.SingleOrDefault(i => i.Name.ToUpper().CompareTo((exceptionCodeName.Trim().ToUpper())) == 0 && i.BillingCategoryType == billingCategoryType);
        if (exceptionCode != null)
          return exceptionCode.Id;
      }
      return 0;
    }


    /// <summary>
    /// Returns Invoice type string for Misc/Uatp invoices
    /// </summary>
    /// <param name="invoiceTypeId"></param>
    /// <returns></returns>
    private static string GetInvoiceType(int invoiceTypeId)
    {
      switch (invoiceTypeId)
      {
        case (int)InvoiceType.Invoice:
          return "Original Invoice";
        case (int)InvoiceType.CreditNote:
          return "Credit Note";
        case (int)InvoiceType.RejectionInvoice:
          return "Rejection Invoice";
        case (int)InvoiceType.CorrespondenceInvoice:
          return "Correspondence Invoice";
        default:
          return string.Empty;
      }
    }
  }
}
