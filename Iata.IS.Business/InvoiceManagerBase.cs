using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Cargo;
using Iata.IS.Ich.Interface;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax.Enums;
using System.IO;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Common;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Business.MiscUatp;

namespace Iata.IS.Business
{
  public abstract class InvoiceManagerBase
  {
    protected Dictionary<string, string> ParsedInvoiceList = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    protected Dictionary<string, string> ParsedNilFormC = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    protected Dictionary<string, PaxInvoice> AutoBillingNewInvoices = new Dictionary<string, PaxInvoice>();
    protected Dictionary<string, bool> AutoBillingInvoiceAlertTrack = new Dictionary<string, bool>();

    protected const string DefaultMemberLocationCode = "Main";
    protected const string DefaultUATPMemberLocationCode = "UATP";
    protected const int achZoneId = 3;
    
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ICurrencyManager CurrencyManager { get; set; }

    public IAchCurrencySetUpManager AchCurrencyManager { get; set; }

    /// <summary>
    /// Calendar Manager, will be injected by the container
    /// </summary>
    /// <value>The calendar manager repository.</value>
    public ICalendarManager CalendarManager { get; set; }

    /// <summary>
    /// Gets or sets the invoice repository.
    /// </summary>
    /// <value>The invoice repository.</value>
    public IInvoiceRepository InvoiceRepository { get; set; }
    /// <summary>
    /// Gets or sets the invoice repository.
    /// </summary>
    /// <value>The invoice repository.</value>
    public ICargoInvoiceRepository CGOInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the invoice repository.
    /// </summary>
    public IMiscInvoiceRepository miscUatpInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the Blocking Rule Repository.
    /// </summary>
    public IBlockingRulesRepository BlockingRulesRepository { get; set; }

    /// <summary>
    /// MaxAcceptableAmount Repository.
    /// </summary>
    public IRepository<MaxAcceptableAmount> MaxAcceptableAmountRepository { get; set; }


    /// <summary>
    /// MinAcceptableAmount Repository.
    /// </summary>
    public IRepository<MinAcceptableAmount> MinAcceptableAmountRepository { get; set; }

    /// <summary>
    /// Member manager that will be injected by the container.
    /// </summary>
    public IMemberManager MemberManager { get; set; }

    public IValidationNSInvoiceManager ValidationNSInvoiceManager { get; set; }

    protected Member BillingMember { get; set; }
    protected Member BilledMember { get; set; }

    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    public IReferenceManager ReferenceManager { get; set; }

    public IUnlocCodeManager UnlocCodeManager { get; set; }

    /// <summary>
    /// Gets or sets the location repository.
    /// </summary>
    /// <value>The location repository.</value>
    public ILocationRepository LocationRepository { get; set; }

    /// <summary>
    /// MemberLocationInfo Repository, will be injected by the container.
    /// </summary>
    public IRepository<MemberLocationInformation> MemberLocationInfoRepository { get; set; }

    /// <summary>
    /// Gets or sets the validation error manager.
    /// </summary>
    /// <value>The validation error manager.</value>
    public IValidationErrorManager ValidationErrorManager { get; set; }

    /// <summary>
    /// Gets or sets the blocking rules manager.
    /// </summary>
    /// <value>The blocking rules manager.</value>
    public IBlockingRulesManager BlockingRulesManager
    {
      get;
      set;
    }


    /// <summary>
    /// Creates the validation exception detail.
    /// </summary>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string errorDescription, string fieldName,
      string fieldValue, bool isBatchUpdateAllowed, bool isErrorCorrectable, bool isSanityCheckError, string invoiceNumber)
    {
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        InvoiceNumber = invoiceNumber,
        Id = Guid.NewGuid()
      };

      return validationExceptionDetail;
    }




    /// <summary>
    /// Dictionary for Cargo BillingCode.
    /// </summary>
    private static readonly Dictionary<int, string> CargoBillingCodeToEnumMap = new Dictionary<int, string> { { 1, "P" }, { 2, "C" }, { 3, "B" }, { 4, "T" }, { 5, "R" } };

    /// <summary>
    /// To return billingCode for billingCodeId.
    /// </summary>
    /// <param name="billingCode"></param>
    /// <returns></returns>
    private static string GetBillingCode(int billingCode)
    {
      var cgoBillingCode = string.Empty;
      if (CargoBillingCodeToEnumMap.ContainsKey(billingCode))
      {
        cgoBillingCode = CargoBillingCodeToEnumMap[billingCode];
      }
      return cgoBillingCode;
    }

    # region InvoiceBase Exception

    /// <summary>
    /// Creates the validation exception detail.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <param name="invoice">The invoice.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="errorLevel">The error level.</param>
    /// <param name="exceptionCode">The exception code.</param>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="batchNo">The batch no.</param>
    /// <param name="sequenceNo">The sequence no.</param>
    /// <param name="documentNumber">The document number.</param>
    /// <param name="isBatchUpdateAllowed">if set to <c>true</c> [isBatch update allowed].</param>
    /// <param name="linkedDocumentNumber">The linked document number.</param>
    /// <param name="calculatedValue"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null, bool islinkingError = false)
    {
      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //"IS-XML";// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);

      if (!string.IsNullOrWhiteSpace(calculatedValue))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        //CouponNo = 
        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = documentNumber,
        SourceCodeId = sourceCode.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = batchNo,
        LineItemDetailOrSequenceNo = sequenceNo,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (islinkingError)
      {
        validationExceptionDetail.TransactionId = 9; //TODO : Add code for error correction : Form D

      }

      if (documentNumber != null)
      {
        string[] documentNumbers = documentNumber.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (documentNumbers.Count() == 3)
        {
          validationExceptionDetail.CouponNo = Convert.ToInt32(documentNumbers[2]);
          validationExceptionDetail.IssuingAirline = documentNumbers[0];
        }
      }

       return validationExceptionDetail;
    }




    #endregion

    # region SamplingFormC Exception

    /// <summary>
    /// Creates the validation exception detail.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="samplingFormCRecord">The sampling form C record.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="errorLevel">The error level.</param>
    /// <param name="exceptionCode">The exception code.</param>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="sourceCode">The source code.</param>
    /// <param name="batchNo">The batch no.</param>
    /// <param name="sequenceNo">The sequence no.</param>
    /// <param name="documentNumber">The document number.</param>
    /// <param name="isBatchUpdateAllowed">if set to <c>true</c> [isBatch update allowed].</param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, SamplingFormC samplingFormC, DateTime fileSubmissionDate, string fieldName, SamplingFormCRecord samplingFormCRecord, string fieldValue, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, bool islinkingError = false)
    {
      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }

      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BilledEntityCode = samplingFormC.ProvisionalBillingMember != null ? samplingFormC.ProvisionalBillingMember.MemberCodeNumeric : string.Empty,
        BillingEntityCode = samplingFormC.FromMember != null ? samplingFormC.FromMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = ((int)BillingCode.SamplingFormC).ToString(),
        CategoryOfBilling = ((int)BillingCategoryType.Pax).ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = samplingFormC.ProvisionalBillingYear + samplingFormC.ProvisionalBillingMonth.ToString().PadLeft(2, '0'),
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        InvoiceNumber = samplingFormCRecord != null ? samplingFormCRecord.ProvisionalInvoiceNumber : string.Empty,
        DocumentNo = documentNumber,
        SourceCodeId = sourceCode.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = batchNo,
        LineItemDetailOrSequenceNo = sequenceNo,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (samplingFormCRecord != null)
      {
        validationExceptionDetail.InvoiceNumber = samplingFormCRecord.ProvisionalInvoiceNumber;
        validationExceptionDetail.DocumentNo = string.Format("{0}-{1}-{2}", samplingFormCRecord.TicketIssuingAirline ?? string.Empty, samplingFormCRecord.DocumentNumber, samplingFormCRecord.CouponNumber);
        validationExceptionDetail.CouponNo = samplingFormCRecord.CouponNumber;
        validationExceptionDetail.IssuingAirline = samplingFormCRecord.TicketIssuingAirline;
      }

      return validationExceptionDetail;
    }
    #endregion

    #region PrimeCoupon Exception

    //Overload Method for PrimeCoupon
    /// <summary>
    /// Overload Validation method for PrimeCoupon.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="couponRecord"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="couponNumber"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, PrimeCoupon couponRecord, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = string.Empty,
        IssuingAirline = couponRecord.TicketOrFimIssuingAirline,
        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = string.Format("{0}-{1}-{2}", couponRecord.TicketOrFimIssuingAirline ?? string.Empty, couponRecord.TicketDocOrFimNumber, couponRecord.TicketOrFimCouponNumber), // couponRecord.TicketDocOrFimNumber.ToString(),
        SourceCodeId = couponRecord.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = couponRecord.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = couponRecord.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId,
        CouponNo = couponRecord.TicketOrFimCouponNumber > 0 ? couponRecord.TicketOrFimCouponNumber : (int?)null
      };

      if (errorStatus.Equals(ErrorStatus.X) && !couponRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        couponRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !couponRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        couponRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion


    # region RejectionMemo Exception

    /// <summary>
    /// Overload Validation method for RejectionMemo.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="rejectionMemoRecord"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="islinkingError"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="couponNumber"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, RejectionMemo rejectionMemoRecord, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, bool islinkingError = false, string ticketIssuingAirline = null, int couponNumber = 0, string exceptionDesc = null)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (String.IsNullOrEmpty(exceptionDesc))
      {
        if (!string.IsNullOrEmpty(exceptionCode))
          errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      }
      else
      {
        //CMP #614: Source Code Validation for PAX RMs.
        //Configurable exception description. 
        errorDescription = exceptionDesc; 
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = rejectionMemoRecord.RejectionMemoNumber,
        SourceCodeId = rejectionMemoRecord.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = rejectionMemoRecord.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = rejectionMemoRecord.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (!string.IsNullOrWhiteSpace(ticketIssuingAirline))
      {
        validationExceptionDetail.IssuingAirline = ticketIssuingAirline;
      }

      if (couponNumber > 0)
      {
        validationExceptionDetail.CouponNo = couponNumber;
      }

      if (islinkingError)
      {
        validationExceptionDetail.YourInvoiceNo = rejectionMemoRecord.YourInvoiceNumber;
        validationExceptionDetail.YourInvoiceBillingMonth = rejectionMemoRecord.YourInvoiceBillingMonth;
        validationExceptionDetail.YourInvoiceBillingYear = rejectionMemoRecord.YourInvoiceBillingYear;
        validationExceptionDetail.YourInvoiceBillingPeriod = rejectionMemoRecord.YourInvoiceBillingPeriod;
        validationExceptionDetail.YourRejectionMemoNo = rejectionMemoRecord.YourRejectionNumber;
        validationExceptionDetail.FimCouponNumber = rejectionMemoRecord.FimCouponNumber > 0 ? rejectionMemoRecord.FimCouponNumber : (int?)null;
        validationExceptionDetail.FimBmCmNumber = rejectionMemoRecord.FimBMCMNumber;
        validationExceptionDetail.TransactionId = 2;
        validationExceptionDetail.RejectionStage = rejectionMemoRecord.RejectionStage;
        validationExceptionDetail.ReasonCode = rejectionMemoRecord.ReasonCode;
        validationExceptionDetail.FimBmCmIndicator = rejectionMemoRecord.FIMBMCMIndicatorId;
      }
      if (errorStatus.Equals(ErrorStatus.X) && !rejectionMemoRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        rejectionMemoRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !rejectionMemoRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        rejectionMemoRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    //End Overload Method for RejectionMemo

    #endregion

    # region BillingMemo Exception

    /// <summary>
    /// Overload Validation method for BillingMemo.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingMemo"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedAmount"></param>
    /// <param name="islinkingError"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="couponNumber"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate,
      string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel, string exceptionCode,
      ErrorStatus errorStatus, BillingMemo billingMemo, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedAmount = null, bool islinkingError = false, string ticketIssuingAirline = null, int couponNumber = 0)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);

      if (!string.IsNullOrWhiteSpace(calculatedAmount))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedAmount);
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = billingMemo.BillingMemoNumber,
        SourceCodeId = billingMemo.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = billingMemo.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = billingMemo.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (!string.IsNullOrWhiteSpace(ticketIssuingAirline))
      {
        validationExceptionDetail.IssuingAirline = ticketIssuingAirline;
      }

      if (couponNumber > 0)
      {
        validationExceptionDetail.CouponNo = couponNumber;
      }

      if (islinkingError)
      {
        validationExceptionDetail.CorrespondenceRefNo = billingMemo.CorrespondenceRefNumber;
        validationExceptionDetail.ReasonCode = billingMemo.ReasonCode;
        validationExceptionDetail.TransactionId = 12;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !billingMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        billingMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !billingMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        billingMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    //End Overload Method for BillingMemo


    #endregion

    # region CreditMemo Exception

    /// <summary>
    /// Overload Validation method for CreditMemo.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="creditMemo"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="ticketIssuingAirline"></param>
    /// <param name="couponNumber"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate,
      string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel,
      string exceptionCode, ErrorStatus errorStatus, CreditMemo creditMemo,
      bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string ticketIssuingAirline = null, int couponNumber = 0)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = creditMemo.CreditMemoNumber,
        SourceCodeId = creditMemo.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = creditMemo.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = creditMemo.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (!string.IsNullOrWhiteSpace(ticketIssuingAirline))
      {
        validationExceptionDetail.IssuingAirline = ticketIssuingAirline;
      }

      if (couponNumber > 0)
      {
        validationExceptionDetail.CouponNo = couponNumber;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !creditMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        creditMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !creditMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        creditMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion

    # region SamplingFormD Exception

    /// <summary>
    /// Overload Validation method for SamplingFormD record..
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="samplingFormDRecord"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, SamplingFormDRecord samplingFormDRecord, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, bool islinkingError = false)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,
        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = string.Format("{0}-{1}-{2}", samplingFormDRecord.TicketIssuingAirline ?? string.Empty, samplingFormDRecord.TicketDocNumber, samplingFormDRecord.CouponNumber),
        CouponNo = samplingFormDRecord.CouponNumber > 0 ? samplingFormDRecord.CouponNumber : (int?)null,
        IssuingAirline = samplingFormDRecord.TicketIssuingAirline,
        SourceCodeId = samplingFormDRecord.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = samplingFormDRecord.BatchNumberOfProvisionalInvoice,
        LineItemDetailOrSequenceNo = samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (islinkingError)
      {
        validationExceptionDetail.BatchNo = samplingFormDRecord.BatchNumberOfProvisionalInvoice;
        validationExceptionDetail.SequenceNo = samplingFormDRecord.RecordSeqNumberOfProvisionalInvoice;
        validationExceptionDetail.ProvInvoiceNo = samplingFormDRecord.ProvisionalInvoiceNumber;
        validationExceptionDetail.YourInvoiceNo = samplingFormDRecord.ProvisionalInvoiceNumber;
        validationExceptionDetail.TransactionId = (int)TransactionType.SamplingFormD;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !samplingFormDRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        samplingFormDRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !samplingFormDRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        samplingFormDRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion


    # region SamplingFormC Exception

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber"></param>
    /// <param name="samplingFormC"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="samplingFormCRecord"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="isLinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, SamplingFormC samplingFormC, DateTime fileSubmissionDate, string fieldName, string fieldValue, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, SamplingFormCRecord samplingFormCRecord, bool isBatchUpdateAllowed = false, bool isLinkingError = false)
    {
      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }

      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BilledEntityCode = samplingFormC.ProvisionalBillingMember != null ? samplingFormC.ProvisionalBillingMember.MemberCodeNumeric : string.Empty,
        BillingEntityCode = samplingFormC.FromMember != null ? samplingFormC.FromMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = ((int)BillingCode.SamplingFormC).ToString(),
        CategoryOfBilling = ((int)BillingCategoryType.Pax).ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = samplingFormC.ProvisionalBillingYear + samplingFormC.ProvisionalBillingMonth.ToString().PadLeft(2, '0'),
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,
        InvoiceNumber = samplingFormCRecord != null ? samplingFormCRecord.ProvisionalInvoiceNumber : string.Empty,
        DocumentNo = string.Format("{0}-{1}-{2}", samplingFormCRecord.TicketIssuingAirline ?? string.Empty, samplingFormCRecord.DocumentNumber, samplingFormCRecord.CouponNumber),
        CouponNo = samplingFormCRecord.CouponNumber,
        IssuingAirline = samplingFormCRecord.TicketIssuingAirline,
        SourceCodeId = samplingFormCRecord.SourceCodeId.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = samplingFormCRecord.BatchNumberOfProvisionalInvoice,
        LineItemDetailOrSequenceNo = samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (errorStatus.Equals(ErrorStatus.X) && !samplingFormCRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        samplingFormCRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !samplingFormCRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        samplingFormCRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      if (isLinkingError)
      {
        validationExceptionDetail.TransactionId = (int)TransactionType.SamplingFormC;
        validationExceptionDetail.ProvInvoiceNo = samplingFormCRecord.ProvisionalInvoiceNumber;
        validationExceptionDetail.YourInvoiceNo = samplingFormCRecord.ProvisionalInvoiceNumber;
        validationExceptionDetail.BatchNo = samplingFormCRecord.BatchNumberOfProvisionalInvoice;
        validationExceptionDetail.SequenceNo = samplingFormCRecord.RecordSeqNumberOfProvisionalInvoice;

      }
      return validationExceptionDetail;
    }



    #endregion

    /// <summary>
    /// Creates the validation exception detail.
    /// </summary>
    /// <param name="pkId"></param>
    /// <param name="serialNumber">The serial number.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="fieldValue">The field value.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="errorLevel">The error level.</param>
    /// <param name="exceptionCode">The exception code.</param>
    /// <param name="errorStatus">The error status.</param>
    /// <param name="lineItemNumber">The line item number.</param>
    /// <param name="lineItemDetailNumber">The line item detail number.</param>
    /// <param name="isBatchUpdateAllowed">if set to <c>true</c> [is batch update allowed].</param>
    /// <param name="calculatedAmount"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, MiscUatpInvoice miscUatpInvoice, string fieldName, string fieldValue, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int lineItemNumber = 0, int lineItemDetailNumber = 0, bool isBatchUpdateAllowed = false, string calculatedAmount = null, bool islinkingError = false)
    {
      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      fileName = Path.GetFileName(fileName);

      var chargecodeName = string.Empty;
      if (!lineItemNumber.Equals(0))
      {
        if (miscUatpInvoice.LineItems.Count > lineItemNumber - 1)
          chargecodeName = miscUatpInvoice.LineItems[lineItemNumber - 1].ChargeCode != null ? miscUatpInvoice.LineItems[lineItemNumber - 1].ChargeCode.Name : string.Empty;
      }

      if (!string.IsNullOrWhiteSpace(calculatedAmount))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedAmount);
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = miscUatpInvoice.BillingMember != null ? miscUatpInvoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = miscUatpInvoice.BilledMember != null ? miscUatpInvoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = miscUatpInvoice.ChargeCategoryDisplayName,
        CategoryOfBilling = miscUatpInvoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = miscUatpInvoice.BillingYear + miscUatpInvoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = miscUatpInvoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = miscUatpInvoice.InvoiceNumber,
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        SourceCodeId = chargecodeName,
        FileName = Path.GetFileName(fileName),
        LineItemDetailOrSequenceNo = lineItemDetailNumber,
        LineItemOrBatchNo = lineItemNumber,
        Id = Guid.NewGuid(),
        PkReferenceId = pkId
      };

      if (islinkingError)
      {
        validationExceptionDetail.YourInvoiceNo = miscUatpInvoice.RejectedInvoiceNumber;
        validationExceptionDetail.YourInvoiceBillingYear = miscUatpInvoice.SettlementYear;
        validationExceptionDetail.YourInvoiceBillingMonth = miscUatpInvoice.SettlementMonth;
        validationExceptionDetail.YourInvoiceBillingPeriod = miscUatpInvoice.SettlementPeriod;
        validationExceptionDetail.CorrespondenceRefNo = miscUatpInvoice.CorrespondenceRefNo.HasValue
                                                            ? miscUatpInvoice.CorrespondenceRefNo.Value
                                                            : 0;

        MiscUatpErrorCodes MiscUatpErrorCodes;
        if (miscUatpInvoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          MiscUatpErrorCodes = new UatpErrorCodes();
        else
          MiscUatpErrorCodes = new MiscErrorCodes();

        if (exceptionCode.CompareTo(MiscUatpErrorCodes.RejectionInvoiceNumberNotExist) == 0) //Rm
          validationExceptionDetail.TransactionId = 38;
        else
          validationExceptionDetail.TransactionId = 37;
      }

      return validationExceptionDetail;
    }
    #region Cargo Exception

    #region CargoInvoice Exception

    /// <summary>
    /// To Validate Cargo Invoicemodel.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingCode"></param>
    /// <param name="sourceCode"></param>
    /// <param name="batchNo"></param>
    /// <param name="sequenceNo"></param>
    /// <param name="documentNumber"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedValue"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateCgoValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int billingCode, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null)
    {

      string submissionFormat;
      if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
      {
        submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //"IS-XML";// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
      }
      else
      {
        submissionFormat = ((int)SubmissionMethod.IsIdec).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
      }
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);

      if (!string.IsNullOrWhiteSpace(calculatedValue))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = documentNumber,
        SourceCodeId = sourceCode == 0 ? string.Empty : sourceCode.ToString(),
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = batchNo,
        LineItemDetailOrSequenceNo = sequenceNo,
        Id = Guid.NewGuid(),
        PkReferenceId = id
      };

      return validationExceptionDetail;
    }



    #endregion

    #region AwbRecord Exception

    /// <summary>
    /// To validate Awb record.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="awbRecord"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedValue"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateCgoAwbValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, AwbRecord awbRecord, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null)
    {
      var submissionFormat = Path.GetExtension(fileName).ToUpper().Equals(".XML") ? ((int)SubmissionMethod.IsXml).ToString() : ((int)SubmissionMethod.IsIdec).ToString();
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      if (!string.IsNullOrWhiteSpace(calculatedValue))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
      }
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = GetBillingCode(awbRecord.BillingCodeId),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = string.Empty,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = string.Format("{0}-{1}", awbRecord.AwbIssueingAirline, awbRecord.AwbSerialNumber),
        SourceCodeId = string.Empty,
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = awbRecord.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = awbRecord.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = id
      };

      if (errorStatus.Equals(ErrorStatus.X) && !awbRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        awbRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !awbRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        awbRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion

    #region CgoRejection Memo Exception

    /// <summary>
    /// To validate Cgo REjection memo.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingCode"></param>
    /// <param name="cargoRejectionMemo"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedValue"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateCgoRMValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int billingCode, CargoRejectionMemo cargoRejectionMemo, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null, bool islinkingError = false, string formatedErrorDescriptionText = null)
    {
      var submissionFormat = Path.GetExtension(fileName).ToUpper().Equals(".XML") ? ((int)SubmissionMethod.IsXml).ToString() : ((int)SubmissionMethod.IsIdec).ToString();
      var errorDescription = string.Empty;
      /* CMP#674: Validation of Coupon and AWB Breakdowns in Rejections. 
       * Desc: Added new default parameter to allow accepting formatted error description text from caller. */  
      if (string.IsNullOrWhiteSpace(formatedErrorDescriptionText))
      {
          if (!string.IsNullOrEmpty(exceptionCode))
              errorDescription = Messages.ResourceManager.GetString(exceptionCode);
          if (!string.IsNullOrWhiteSpace(calculatedValue))
          {
              errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
          } 
      }
      else
      {
          errorDescription = formatedErrorDescriptionText;
      }
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = cargoRejectionMemo.RejectionMemoNumber,
        SourceCodeId = string.Empty,
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = cargoRejectionMemo.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = cargoRejectionMemo.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = id

      };

      if (islinkingError)
      {
        validationExceptionDetail.YourInvoiceNo = cargoRejectionMemo.YourInvoiceNumber;
        validationExceptionDetail.YourInvoiceBillingMonth = cargoRejectionMemo.YourInvoiceBillingMonth;
        validationExceptionDetail.YourInvoiceBillingYear = cargoRejectionMemo.YourInvoiceBillingYear;
        validationExceptionDetail.YourInvoiceBillingPeriod = cargoRejectionMemo.YourInvoiceBillingPeriod;
        validationExceptionDetail.YourRejectionMemoNo = cargoRejectionMemo.YourRejectionNumber;
        validationExceptionDetail.YourBmCmNo = cargoRejectionMemo.YourBillingMemoNumber;
        validationExceptionDetail.YourBmCmIndicator = cargoRejectionMemo.BMCMIndicatorId;
        validationExceptionDetail.TransactionId = 16;
        validationExceptionDetail.ReasonCode = cargoRejectionMemo.ReasonCode;
        validationExceptionDetail.RejectionStage = cargoRejectionMemo.RejectionStage;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !cargoRejectionMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoRejectionMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !cargoRejectionMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoRejectionMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion

    #region CgoBilling Memo Exception

    /// <summary>
    /// To Validate Cgo Billing Memo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingCode"></param>
    /// <param name="cargoBillingMemo"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedAmount"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateCgoBMValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int billingCode, CargoBillingMemo cargoBillingMemo, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedAmount = null, bool islinkingError = false)
    {
      var submissionFormat = Path.GetExtension(fileName).ToUpper().Equals(".XML") ? ((int)SubmissionMethod.IsXml).ToString() : ((int)SubmissionMethod.IsIdec).ToString();
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);

      if (!string.IsNullOrWhiteSpace(calculatedAmount))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedAmount);
      }

      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = cargoBillingMemo.BillingMemoNumber,
        SourceCodeId = string.Empty,
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = cargoBillingMemo.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = cargoBillingMemo.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = id
      };

      if (islinkingError)
      {
        validationExceptionDetail.CorrespondenceRefNo = cargoBillingMemo.CorrespondenceReferenceNumber;
        validationExceptionDetail.TransactionId = 19;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !cargoBillingMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoBillingMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !cargoBillingMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoBillingMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion

    #region CgoCredit Memo Exception

    /// <summary>
    /// To Validate Cgo Credit memo.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingCode"></param>
    /// <param name="cargoCreditMemo"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedValue"></param>
    /// <param name="islinkingError"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateCgoCMValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int billingCode, CargoCreditMemo cargoCreditMemo, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null, bool islinkingError = false)
    {
      var submissionFormat = Path.GetExtension(fileName).ToUpper().Equals(".XML") ? ((int)SubmissionMethod.IsXml).ToString() : ((int)SubmissionMethod.IsIdec).ToString();
      var errorDescription = string.Empty;
      if (!string.IsNullOrEmpty(exceptionCode))
        errorDescription = Messages.ResourceManager.GetString(exceptionCode);
      if (!string.IsNullOrWhiteSpace(calculatedValue))
      {
        errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
      }
      var validationExceptionDetail = new IsValidationExceptionDetail
      {
        SerialNo = serialNumber,
        BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
        BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
        ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
        CategoryOfBilling = invoice.BillingCategoryId.ToString(),
        SubmissionFormat = submissionFormat,
        ErrorStatus = ((int)errorStatus).ToString(),
        ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
        PeriodNumber = invoice.BillingPeriod,
        BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
        LinkedDocNo = linkedDocumentNumber,

        ErrorDescription = errorDescription,
        FieldName = fieldName,
        FieldValue = fieldValue,
        BatchUpdateAllowed = isBatchUpdateAllowed,

        InvoiceNumber = invoice.InvoiceNumber,
        DocumentNo = cargoCreditMemo.CreditMemoNumber,
        SourceCodeId = string.Empty,
        ErrorLevel = errorLevel,
        ExceptionCode = exceptionCode,
        FileName = Path.GetFileName(fileName),
        LineItemOrBatchNo = cargoCreditMemo.BatchSequenceNumber,
        LineItemDetailOrSequenceNo = cargoCreditMemo.RecordSequenceWithinBatch,
        Id = Guid.NewGuid(),
        PkReferenceId = id
      };

      if (islinkingError)
      {
        validationExceptionDetail.CorrespondenceRefNo = cargoCreditMemo.CorrespondenceRefNumber;
        validationExceptionDetail.TransactionId = 19;
      }

      if (errorStatus.Equals(ErrorStatus.X) && !cargoCreditMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoCreditMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
      }

      if (errorStatus.Equals(ErrorStatus.C) && !cargoCreditMemo.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
      {
        cargoCreditMemo.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
      }

      return validationExceptionDetail;
    }

    #endregion

    #endregion

    protected static bool IsDirectSettlementMember(Member member)
    {
      //return (((member.IchMemberStatusId == 0 || member.IchMemberStatusId == (int) IchMemberShipStatus.NotAMember) && (member.AchMemberStatusId == 0 || member.AchMemberStatusId == (int) AchMembershipStatus.NotAMember)) || member.IchMemberStatusId == (int)IchMemberShipStatus.Terminated || member.AchMemberStatusId == (int)AchMembershipStatus.Terminated);
        return ((member.IchMemberStatusId == 0 || member.IchMemberStatusId == (int) IchMemberShipStatus.NotAMember) &&
                (member.AchMemberStatusId == 0 || member.AchMemberStatusId == (int) AchMembershipStatus.NotAMember)) ||
               (member.IchMemberStatusId == (int) IchMemberShipStatus.Terminated &&
                member.AchMemberStatusId == (int) AchMembershipStatus.Terminated) ||
               (member.IchMemberStatusId == (int) IchMemberShipStatus.Terminated &&
                (member.AchMemberStatusId == 0 || member.AchMemberStatusId == (int) AchMembershipStatus.NotAMember)) ||
               ((member.IchMemberStatusId == 0 || member.IchMemberStatusId == (int) IchMemberShipStatus.NotAMember) &&
                member.AchMemberStatusId == (int) AchMembershipStatus.Terminated);
    }

    protected static bool IsNoClearingHouseMember(Member member)
    {
      return (member.IchMemberStatusId == (int)IchMemberShipStatus.NotAMember || member.IchMemberStatusId == (int)IchMemberShipStatus.Terminated) &&
             (member.AchMemberStatusId == (int)AchMembershipStatus.NotAMember || member.AchMemberStatusId == (int)AchMembershipStatus.Terminated);
    }
    protected static bool IsDualMember(Member member)
    {
      return (member.IchMemberStatusId == (int)IchMemberShipStatus.Live || member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended) &&
             (member.AchMemberStatusId == (int)AchMembershipStatus.Live || member.AchMemberStatusId == (int)AchMembershipStatus.Suspended);
    }

    protected static bool IsAchMember(Member member)
    {
      return (member.AchMemberStatusId == (int)AchMembershipStatus.Live || member.AchMemberStatusId == (int)AchMembershipStatus.Suspended);
    }


    protected static bool IsAchOrDualMember(Member member)
    {
        return IsAchMember(member) || IsDualMember(member);
    }

    protected static bool IsIchMember(Member member)
    {
      return (member.IchMemberStatusId == (int)IchMemberShipStatus.Live || member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended);
    }

    /// <summary>
    /// Validates the settlement method.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="BillingMember">The billing member.</param>
    /// <param name="BilledMember">The billed member.</param>
    /// <returns></returns>
    protected virtual bool ValidateSettlementMethod(InvoiceBase invoiceHeader, Member BillingMember, Member BilledMember)
    {
      //BillingMember = billingMember ?? MemberManager.GetMember(invoiceHeader.BillingMemberId);
      //BilledMember = billedMember ?? MemberManager.GetMember(invoiceHeader.BilledMemberId);

      Logger.InfoFormat("invoiceHeader.InvoiceSmi: {0}", Enum.GetName(typeof(SMI), invoiceHeader.InvoiceSmi));

      if (!(invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.Ich || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules))
      {
        return true;
      }

      // Check if either of the members is a Direct Settlement Member - then the SMI has to be Bilateral.
     // if ((IsDirectSettlementMember(BillingMember) || IsDirectSettlementMember(BilledMember)))
      if (CheckIfSettlementIsBilateral(BillingMember, BilledMember))
      {
          /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
          return ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId,true);
      }

      // Billing Member and billed member both are dual member
      if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
      {

        Logger.Info("Members are Dual members.");

        // Check for ACH Exception "Settle through ICH",
        // if exception is present in both member, then SMI should be ICH.
        var achExceptionsBillingMember = MemberManager.GetExceptionMembers(BillingMember.Id, (int)invoiceHeader.BillingCategory, false).ToList();
        if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0)
        {
          Logger.Info("Exception is present in both member, so SMI should be ICH.");
          return invoiceHeader.InvoiceSmi == SMI.Ich;
        }

        //// this condition is to check if one member has other member in its exception list but not vice versa.
        //if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0)
        //{
        //  Logger.Info("One member has other member in its exception list but not vice versa.");
        //  return false;
        //}

        // if exception is not present in any of the member list for other member, then SMI should be A or M.
        if (invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules)
        {
          Logger.Info("Exception is not present in any of the member list for other member, So SMI should be A or M.");
          return true;
        }
      }

      // Billing member is Dual.
      else if (IsDualMember(BillingMember))
      {

        Logger.Info("Billing member is Dual.");

        // and Billed Member is ICH, then SMI should be ICH.
        if (IsIchMember(BilledMember) && (invoiceHeader.InvoiceSmi == SMI.Ich))
        {
          Logger.Info("Billed Member is ICH, So SMI should be ICH.");
          return true;
        }

        // and Billed Member is ACH, then SMI should be ACH or ACH using IATA rules.
        if (IsAchMember(BilledMember))
        {
          if (invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules)
          {
            Logger.Info("Billed Member is ACH, So SMI should be ACH or ACH using IATA rules.");
            return true;
          }
        }
      }

      // Billing member is only ICH, then SMI should be ICH, irrespective of BilledMember status.
      else if (IsIchMember(BillingMember))
      {
        if (invoiceHeader.InvoiceSmi == SMI.Ich)
        {
          Logger.Info("Billing member is only ICH, So SMI should be ICH, irrespective of BilledMember status.");
          return true;
        }
      }

      // Billing member is only ACH.
      else if (IsAchMember(BillingMember))
      {

        Logger.Info("Billing member is only ACH.");

        // if Billed Member is dual or only ACH, then SMI should be A or M.
        if (IsDualMember(BilledMember) || IsAchMember(BilledMember))
        {
          if (invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules)
          {
            Logger.Info("Billed Member is dual or only ACH, So SMI should be A or M.");
            return true;
          }
        }

        // if Billed Member is only ICH, then SMI should be M.
        else if (IsIchMember(BilledMember))
        {
          if (invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules)
          {
            Logger.Info("Billed Member is only ICH, So SMI should be M.");
            return true;
          }
        }
      }

      Logger.Info("Settlement Method Not found, hence return false.");

      return false;
    }


    /// <summary>
    /// Gets the member's ICH configuration.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    protected IchConfiguration GetIchConfiguration(int memberId)
    {
      var MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      var ichConfiguration = MemberManager.GetIchDetails(memberId);
      if (ichConfiguration != null)
      {
        if (ichConfiguration.IchMemberShipStatus == IchMemberShipStatus.NotAMember || ichConfiguration.IchMemberShipStatus == IchMemberShipStatus.Terminated)
        {
          return null;
        }
      }

      return ichConfiguration;
    }

    /// <summary>
    /// Gets the ACH configuration.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    protected AchConfiguration GetAchConfiguration(int memberId)
    {

      // SCP178233: AB invoices not submitted
      if(MemberManager == null)
      {
        MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
      }

      var achConfiguration = MemberManager.GetAchConfig(memberId);
      if (achConfiguration != null)
      {
        if (achConfiguration.AchMembershipStatus == AchMembershipStatus.NotAMember || achConfiguration.AchMembershipStatus == AchMembershipStatus.Terminated)
        {
          return null;
        }
      }

      return achConfiguration;
    }



    /// <summary>
    /// Validates the billing currency.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="isIchConfigRetrieved"></param>
    protected int GetBillingCurrencyForAutoBillingInvoice(InvoiceBase invoiceHeader, Member billingMember, Member billedMember, bool isIchConfigRetrieved = false)
    {
      // If Ich Configurations of the member are already retrieved don't retrieve it again.
      if (!isIchConfigRetrieved)
      {
        billingMember.IchConfiguration = GetIchConfiguration(billingMember.Id);
        billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);
      }

      if (invoiceHeader.InvoiceSmi == SMI.Ich && billingMember.IchConfiguration != null)
      {
        switch (billingMember.IchConfiguration.IchZone)
        {
          case IchZoneType.A:
            {
              // Billed airline in zone A.
              if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.A)
              {
                return (int)BillingCurrency.GBP;
              }
              else
              {
                return (int)BillingCurrency.USD;
              }
            }
            break;
          case IchZoneType.B:
          case IchZoneType.C:
            return (int)BillingCurrency.USD;
          case IchZoneType.D:
            // Billed airline in zone A, B, C.
            if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.D)
            {
              return (int)BillingCurrency.EUR;
            }
            return (int)BillingCurrency.USD;
        }
      }

      // When SMI = A/M, and if only billing member belongs to ACH, lock the dropdown on USD. 
      if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && IsAchMember(billingMember) && !IsAchMember(billedMember))
      {
        return (int)BillingCurrency.USD;
      }

      //// When SMI = A/M, and both members belong to ACH, dropdown should only offer USD/CAD. Default to USD
      //if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && IsAchMember(billingMember) && IsAchMember(billedMember))
      //{
      //  return (int)BillingCurrency.CAD;
      //}

      return (int)BillingCurrency.USD;

    }

    /// <summary>
    /// Validates the billing currency.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="isIchConfigRetrieved"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileName"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="isWeb"></param>
    /// <returns></returns>
    protected bool ValidateBillingCurrency(InvoiceBase invoiceHeader, Member billingMember, Member billedMember, bool isIchConfigRetrieved = false, IList<IsValidationExceptionDetail> exceptionDetailsList = null, string fileName = null, DateTime? fileSubmissionDate = null)
    {
       bool isValid = true;

      // If Ich Configurations of the member are already retrieved don't retrieve it again.
      if (!isIchConfigRetrieved)
      {
        billingMember.IchConfiguration = GetIchConfiguration(billingMember.Id);
        billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);
      }

      if (invoiceHeader.InvoiceSmi == SMI.Ich && billingMember.IchConfiguration != null)
      {
          switch (billingMember.IchConfiguration.IchZone)
          {
              case IchZoneType.A:
                  {
                      // Billed airline in zone A.
                      if (billedMember.IchConfiguration != null &&
                          billedMember.IchConfiguration.IchZone == IchZoneType.A)
                      {
                          if (invoiceHeader.BillingCurrency != BillingCurrency.GBP)
                          {
                              isValid = false;
                          }
                      }
                      else
                      {
                          // Billed airline in any zone other than A.
                          if (invoiceHeader.BillingCurrency != BillingCurrency.USD)
                          {
                              isValid = false;
                          }
                      }
                  }
                  break;
              case IchZoneType.B:
              case IchZoneType.C:
                  // Billed airline in any zone.
                  if (invoiceHeader.BillingCurrency != BillingCurrency.USD)
                  {
                      isValid = false;
                  }
                  break;
              case IchZoneType.D:
                  // Billed airline in zone A, B, C.
                  if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.D)
                  {
                      if (invoiceHeader.BillingCurrency != BillingCurrency.EUR)
                      {
                          isValid = false;
                      }
                  }
                  else
                  {
                      // Billed airline in any zone other than D.
                      if (invoiceHeader.BillingCurrency != BillingCurrency.USD)
                      {
                          isValid = false;
                      }
                  }
                  break;
          }
          if (!isValid && fileName != null && invoiceHeader.ValidationStatus != InvoiceValidationStatus.ErrorPeriod && invoiceHeader.SubmissionMethod != SubmissionMethod.AutoBilling && !invoiceHeader.IsFutureSubmission)
          {
              CreateValidationExceptionDetail(invoiceHeader, fileName, exceptionDetailsList, fileSubmissionDate);
          }
      }
      else
      {
          isValid = ValidateBillingCurrencyForAch(billingMember, billedMember, invoiceHeader, exceptionDetailsList,
                                                  fileSubmissionDate, fileName);
      }

      return isValid; 
    }

    /// <summary>
    /// This function is used to create validation exception detail.
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    /// </summary>
    /// <param name="invoiceHeader"></param>
    /// <param name="fileName"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileSubmissionDate"></param>
    private static void CreateValidationExceptionDetail(InvoiceBase invoiceHeader, string fileName, IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime? fileSubmissionDate)
    {
        var validationExceptionDetail = new IsValidationExceptionDetail();

        switch (invoiceHeader.BillingCategory)
        {
            case BillingCategoryType.Pax:
                validationExceptionDetail = CreateValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate.Value, "Billing Currency",
                                                                            invoiceHeader.BillingCurrencyId.Value.
                                                                                ToString(), invoiceHeader, fileName,
                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                            ErrorCodes.InvalidBillingCurrency,
                                                                            ErrorStatus.X);
                break;
            case BillingCategoryType.Cgo:

                validationExceptionDetail = CreateCgoValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                               exceptionDetailsList.Count() + 1,
                                                                               fileSubmissionDate.Value,
                                                                               "Billing Currency",
                                                                               invoiceHeader.BillingCurrencyId.Value.
                                                                                   ToString(), invoiceHeader, fileName,
                                                                               ErrorLevels.ErrorLevelInvoice,
                                                                               CargoErrorCodes.
                                                                                   InvalidBillingCurrency,
                                                                               ErrorStatus.X, invoiceHeader.BillingCode);
                break;
            case BillingCategoryType.Misc:
            case BillingCategoryType.Uatp:
                var miscUatpInvoice = (MiscUatpInvoice) invoiceHeader;

                validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate.Value, miscUatpInvoice,
                                                                            "Clearance Currency",
                                                                            miscUatpInvoice.BillingCurrencyDisplayText,
                                                                            fileName, ErrorLevels.ErrorLevelInvoice,
                                                                            new MiscUatpErrorCodes().
                                                                                InvalidClearanceCurrency,
                                                                            ErrorStatus.X);
                break;
        }
        exceptionDetailsList.Add(validationExceptionDetail);
    }

      /// <summary>
    /// This function is used to validate billing currency for ach.
    /// </summary>
    /// <param name="billingMember"></param>
    /// <param name="billedMember"></param>
    /// <param name="invoiceHeader"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    protected bool ValidateBillingCurrencyForAch(Member billingMember, Member billedMember, InvoiceBase invoiceHeader, IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime? fileSubmissionDate, string fileName)
    {
        //if submission method is autobilling then continue old validation.
        //CMP #553: ACH Requirement for Multiple Currency Handling
        //If "SMI as ACH" Or "SMI as 'M' alone with billing and billed member should be either ACH or Dual" then these validation will apply.
        //Exclusions-- Item #E1: The system will not re-validate the Currency of Clearance when the actual submission happens for Late Submission, 
        //Incremental Billing Period, Future submission
        if (!(fileName == null && (invoiceHeader.ValidationStatus == InvoiceValidationStatus.ErrorPeriod  || invoiceHeader.IsFutureSubmission)) &&
            !(invoiceHeader.SubmissionMethodId == (int)SubmissionMethod.AutoBilling || (invoiceHeader.BillingCategoryId == (int)BillingCategoryType.Pax && invoiceHeader.BillingCode != (int) BillingCode.NonSampling)) &&
            (invoiceHeader.BillingCurrencyId != (int)BillingCurrency.USD && ((invoiceHeader.InvoiceSmi == SMI.Ach) || (invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules && IsAchOrDualMember(billingMember) &&
               IsAchOrDualMember(billedMember)))))
        {
            //Validation #2: The ‘Currency of Clearance’ of the Invoice/Credit Note should exist as a valid ‘Currency of Clearance’ in new master 
            //‘Allowed ACH Currencies of Clearance Setup’ as an ACTIVE record 
            if (!AchCurrencyManager.IsValidAchCurrency(invoiceHeader.BillingCurrencyId))
            {
                //These validation message will apply only for file(is-xml, is-idec).
                if (fileName != null)
                {
                    var validationExceptionDetail = new IsValidationExceptionDetail();
                    switch (invoiceHeader.BillingCategory)
                    {
                        case BillingCategoryType.Pax:

                            validationExceptionDetail = CreateValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                                        exceptionDetailsList.
                                                                                            Count() +
                                                                                        1,
                                                                                        fileSubmissionDate.Value,
                                                                                        "Currency of Billing",
                                                                                        invoiceHeader.
                                                                                            BillingCurrencyId.
                                                                                            Value.ToString(),
                                                                                        invoiceHeader,
                                                                                        fileName,
                                                                                        ErrorLevels.
                                                                                            ErrorLevelInvoice,
                                                                                        ErrorCodes.
                                                                                            InvalidOrInactiveCurrencyOfBilling,
                                                                                        ErrorStatus.X);
                            break;
                        case BillingCategoryType.Cgo:
                            validationExceptionDetail = CreateCgoValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                                           exceptionDetailsList.Count() +
                                                                                           1,
                                                                                           fileSubmissionDate.Value,
                                                                                           "Currency of Billing",
                                                                                           invoiceHeader.
                                                                                               BillingCurrencyId.Value.
                                                                                               ToString(), invoiceHeader,
                                                                                           fileName,
                                                                                           ErrorLevels.ErrorLevelInvoice,
                                                                                           CargoErrorCodes.
                                                                                               InvalidOrInactiveCurrencyOfBilling,
                                                                                           ErrorStatus.X,
                                                                                           invoiceHeader.BillingCode);
                            break;
                        case BillingCategoryType.Misc:
                        case BillingCategoryType.Uatp:
                            var miscUatpInvoice = (MiscUatpInvoice) invoiceHeader;
                            validationExceptionDetail =
                                CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                exceptionDetailsList.Count() + 1,
                                                                fileSubmissionDate.Value,
                                                                miscUatpInvoice,
                                                                "ClearanceCurrencyCode",
                                                                miscUatpInvoice.BillingCurrencyDisplayText,
                                                                fileName,
                                                                ErrorLevels.ErrorLevelInvoice,
                                                                MiscUatpErrorCodes.
                                                                    InvalidOrInactiveCurrencyOfClearance,
                                                                ErrorStatus.X);
                            break;
                    }
                    exceptionDetailsList.Add(validationExceptionDetail);
                }
                return false;
            }

            //This validation will apply for isweb, is-xml and is-idec.
            //Validation #3: The ‘Currency of Invoice’ should be the same as the ‘Currency of Clearance’
            if (invoiceHeader.BillingCurrencyId != invoiceHeader.ListingCurrencyId)
            {
                var validationExceptionDetail = new IsValidationExceptionDetail();
                switch (invoiceHeader.BillingCategory)
                {
                    case BillingCategoryType.Pax:

                        if (fileName == null)
                        {
                            throw new ISBusinessException(ErrorCodes.CurrencyOfListingEvaluationShouldBeSameAsCurrencyOfBilling);
                        }
                        validationExceptionDetail = CreateValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                                    exceptionDetailsList.Count() +
                                                                                    1,
                                                                                    fileSubmissionDate.Value,
                                                                                    "Currency of Listing",
                                                                                    invoiceHeader.ListingCurrencyId.
                                                                                        Value.
                                                                                        ToString(), invoiceHeader,
                                                                                    fileName,
                                                                                    ErrorLevels.
                                                                                        ErrorLevelInvoice,
                                                                                    ErrorCodes.
                                                                                        CurrencyOfListingShouldBeSameAsCurrencyOfBilling,
                                                                                    ErrorStatus.X);
                        break;
                    case BillingCategoryType.Cgo:
                        if (fileName == null)
                        {
                            throw new ISBusinessException(CargoErrorCodes.CurrencyOfListingShouldBeSameAsCurrencyOfBilling);
                        }
                        validationExceptionDetail = CreateCgoValidationExceptionDetail(invoiceHeader.Id.Value(),
                                                                                       exceptionDetailsList.Count() +
                                                                                       1,
                                                                                       fileSubmissionDate.Value,
                                                                                       "Currency of Listing",
                                                                                       invoiceHeader.ListingCurrencyId.
                                                                                           Value.ToString(),
                                                                                       invoiceHeader,
                                                                                       fileName,
                                                                                       ErrorLevels.ErrorLevelInvoice,
                                                                                       CargoErrorCodes.
                                                                                           CurrencyOfListingShouldBeSameAsCurrencyOfBilling,
                                                                                       ErrorStatus.X,
                                                                                       invoiceHeader.BillingCode);
                        break;
                    case BillingCategoryType.Misc:
                    case BillingCategoryType.Uatp:
                        if (fileName == null)
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.CurrencyOfBillingShouldBeSameAsCurrencyOfClearance);
                        }
                        var miscUatpInvoice = (MiscUatpInvoice) invoiceHeader;
                        validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate.Value,
                                                                                    miscUatpInvoice,
                                                                                    "CurrencyCode",
                                                                                    miscUatpInvoice.
                                                                                        ListingCurrencyDisplayText,
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    MiscUatpErrorCodes.
                                                                                        CurrencyCodeShouldBeSameAsClearanceCurrencyCode,
                                                                                    ErrorStatus.X);
                        break;
                }
                exceptionDetailsList.Add(validationExceptionDetail);
                return false;
            }
        }
        else
        {
            // When SMI = A/M, and if only billing member belongs to ACH, lock the dropdown on USD. 
            if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) &&
                IsAchMember(billingMember) && !IsAchMember(billedMember) &&
                invoiceHeader.BillingCurrency != BillingCurrency.USD)
            {
                if (fileName != null)
                {
                    CreateValidationExceptionDetail(invoiceHeader, fileName, exceptionDetailsList, fileSubmissionDate);
                }

                return false;
            }

            // When SMI = A/M, and both members belong to ACH, dropdown should only offer USD/CAD. Default to USD
            if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) &&
                IsAchMember(billingMember) && IsAchMember(billedMember) &&
                !(invoiceHeader.BillingCurrency == BillingCurrency.USD ||
                  invoiceHeader.BillingCurrency == BillingCurrency.CAD))
            {
                if (fileName != null)
                {
                    CreateValidationExceptionDetail(invoiceHeader, fileName, exceptionDetailsList, fileSubmissionDate);
                }

                return false;
            }
        }

        return true;
    }


      /// <summary>
    /// Validates the listing currency.
    /// SCP177435 - EXCHANGE RATE 
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <returns></returns>
    protected bool ValidateListingCurrency(InvoiceBase invoiceHeader, Member billingMember, Member billedMember)
    {
      // When SMI = A/M, and both members belong to ACH, dropdown should only offer USD/CAD. Default to USD
      if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && IsAchMember(billingMember) && IsAchMember(billedMember) &&
          (invoiceHeader.BillingCurrency == BillingCurrency.CAD) &&
          invoiceHeader.ListingCurrencyId != (int)BillingCurrency.CAD)
      {
        return false;
      }

      return true;
    }

    protected bool ValidateBlockingByRule(bool isBilled, int zoneId, int memberId)
    {
      var isBlocked = false;
      var blockingRules = BlockingRulesManager.GetBlockingRuleDetailsByMemberId(memberId);
      if (blockingRules != null)
      {
        foreach (var rules in blockingRules)
        {
          var blockedgroup = rules.BlockedGroups.Where(groups => groups.IsBlockAgainst == isBilled && groups.ZoneTypeId == zoneId && groups.Pax && groups.IsDeleted == false).ToList();

          if (blockedgroup.Count > 0)
          {
            isBlocked = true;
          }
        }
      }
      return isBlocked;
    }

    /// <summary>
    /// Validates exchange rate for IS-Web invoice.
    /// </summary>
    /// <returns></returns>
    //CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.
    protected bool ValidateExchangeRate(decimal? exchangeRate, int listingCurrencyId, BillingCurrency billingCurrency, int billingYear, int billingMonth)
    {
      try
      {
        var exchangeRateInDb = ReferenceManager.GetExchangeRate(listingCurrencyId, billingCurrency, billingYear, billingMonth);

        if (ConvertUtil.Round(exchangeRateInDb, 5) != Convert.ToDouble(exchangeRate))
        {
          return false;
        }
      }
      catch (Exception exception)
      {
        //SCP345230: ICH Settlement Error - SIS Production
        return false;
      }

      return true;
    }

    /// <summary>
    /// Validates the billing IS Membership status.
    /// A member cannot create an invoice/creditNote or Form C when his IS Membership is ‘Basic’, ‘Restricted’ or ‘Terminated’.
    /// </summary>
    /// <param name="billingMember">The billing member.</param>
    protected virtual bool ValidateOnBehalfBillingMembershipStatus(Member billingMember)
    {
      if (billingMember.IsMembershipStatus == MemberStatus.Terminated)
      {
        return false;
      }

      return true;


    }

    /// <summary>
    /// Validates the billing IS Membership status.
    /// A member cannot create an invoice/creditNote or Form C when his IS Membership is ‘Basic’, ‘Restricted’ or ‘Terminated’.
    /// </summary>
    /// <param name="billingMember">The billing member.</param>
    protected virtual bool ValidateBillingMembershipStatus(Member billingMember)
    {
      if (billingMember.IsMembershipStatus == MemberStatus.Restricted
        || billingMember.IsMembershipStatus == MemberStatus.Basic
        || billingMember.IsMembershipStatus == MemberStatus.Terminated)
      {
        return false;
      }

      return true;


    }

    /// <summary>
    /// Validates the billed IS Membership status.
    /// Validation of the Billed Member will fail if the IS Membership Status of the Billed Member is ‘Terminated’.This will be a non-correctable error.
    /// </summary>
    /// <param name="billedMember">The billed member.</param>
    protected virtual bool ValidateBilledMemberStatus(Member billedMember)
    {
      if (billedMember.IsMembershipStatus == MemberStatus.Terminated || billedMember.IsMembershipStatus == MemberStatus.Pending)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Validates the billing period.
    /// TODO - Exception: 1) It can be equal to a future period if the IS calendar’s “Submissions Open (Future dated submissions)” date of that future period is equal to or less than the system date.
    /// TODO - Exception: 2) Late submissions (where the period is the current open period less 1) will be marked as validation error; even if the late submission window for the past period is open in the IS Calendar.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns></returns>
    protected virtual bool ValidateBillingPeriod(InvoiceBase invoice)
    {
      return ValidateBillingPeriod(invoice, DateTime.UtcNow);
    }

    /// <summary>
    /// Validates the billing period.
    /// TODO - Exception: 1) It can be equal to a future period if the IS calendar’s “Submissions Open (Future dated submissions)” date of that future period is equal to or less than the system date.
    /// TODO - Exception: 2) Late submissions (where the period is the current open period less 1) will be marked as validation error; even if the late submission window for the past period is open in the IS Calendar.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="inputDate">The input date.</param>
    /// <returns></returns>
    protected virtual bool ValidateBillingPeriod(InvoiceBase invoice, DateTime inputDate)
    {
      try
      {
        // Get clearing house for invoice settlement method.
        var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);

        //SCP321997: Sun Splash Aviation SH-361
        var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(clearingHouse);

        if (!ValidateBillingPeriod(invoice, currentBillingPeriod, clearingHouse)) return false;
      }
      catch (ISCalendarDataNotFoundException dataNotFoundException)
      {
        Logger.Error("Unable to get current billing period.", dataNotFoundException);
        return false;
      }

      return true;
    }

    protected bool ValidateBillingPeriod(InvoiceBase invoice, BillingPeriod billingPeriod, ClearingHouse clearingHouse)
    {
      if (invoice.InvoiceBillingPeriod != billingPeriod)
      {
        //ToDo - It can be equal to a future period if the IS calendar’s “Submissions Open (Future dated submissions)” date of that future period is equal to or less than the system date
        //var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoice.BillingMemberId);
        //if (passengerConfiguration != null && passengerConfiguration.IsFutureBillingSubmissionsAllowed)
        //{
        //  if (invoice.InvoiceBillingPeriod  != currentBillingPeriod )
        //  {
        //    return false;
        //  }
        //}
        return false;
      }

      return true;
    }

    /// <summary>
    /// Validates the billing period while saving the invoice header. This will returns true in following scenarios,
    /// case 1 -if invoice billing period set to current open period.
    /// case 2 -if invoice billing period set to previous period for which late submission window is open.
    /// case 3 -
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns></returns>
    public virtual bool ValidateBillingPeriodOnSaveHeader(InvoiceBase invoice)
    {
      // Get clearing house for invoice settlement method.
      var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);


      try
      {
        // Get current billing period for clearing house.
        var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);
        var maxFuturePeriodAllowed = currentBillingPeriod + 4;

        if (invoice.InvoiceBillingPeriod >= currentBillingPeriod && invoice.InvoiceBillingPeriod <= maxFuturePeriodAllowed)
        {
          return true;
        }

        // Check that invoice is eligible for late submission.
        var previousBillingPeriod = currentBillingPeriod - 1;
        if (invoice.InvoiceBillingPeriod == previousBillingPeriod && CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
        {
          return true;
        }
      }
      catch (ISCalendarDataNotFoundException dataNotFoundException)
      {
        Logger.Error("Unable to get current billing period.", dataNotFoundException);

        var previousBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(clearingHouse);
        var maxFuturePeriodAllowed = previousBillingPeriod + 4;

        if (invoice.InvoiceBillingPeriod >= previousBillingPeriod + 1 && invoice.InvoiceBillingPeriod <= maxFuturePeriodAllowed)
        {
          return true;
        }

        // Check that invoice is eligible for late submission.
        if (CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
        {
          return true;
        }
      }


      return false;
    }

    /// <summary>
    /// Validates the billing period.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns></returns>
    public virtual bool ValidateBillingPeriodOnValidate(InvoiceBase invoice)
    {
      // Get clearing house for invoice settlement method.
      var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);

      try
      {
        // Get current billing period for clearing house.
        var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

        if (invoice.InvoiceBillingPeriod == currentBillingPeriod)
        {
          return true;
        }

        // Check that invoice is eligible for late submission.
        var previousBillingPeriod = currentBillingPeriod - 1;
        if (invoice.InvoiceBillingPeriod == previousBillingPeriod && CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
        {
          return true;
        }
      }
      catch (ISCalendarDataNotFoundException dataNotFoundException)
      {
        Logger.Error("Unable to get current billing period.", dataNotFoundException);

        var previousBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(clearingHouse);





        // Check that invoice is eligible for late submission.
        if (CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
        {
          return true;
        }
      }

      return false;
    }



    /// <summary>
    /// Determines whether invoice is submitted late.
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <returns>
    /// true if invoice submitted late; otherwise, false.
    /// </returns>
    protected bool IsLateSubmission(InvoiceBase invoice)
    {
      // Get clearing house for invoice settlement method.
      var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);

      try
      {
        var previousBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(clearingHouse);

        if (!CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
        {
          return false;
        }

        return invoice.InvoiceBillingPeriod == previousBillingPeriod;
      }
      catch (ISCalendarDataNotFoundException dataNotFoundException)
      {
        Logger.Error("Unable to get last closed billing period.", dataNotFoundException);

        return false;
      }
    }


    protected bool IsLateSubmission(InvoiceBase invoice, DateTime inputDate)
    {
      try
      {
        // Get clearing house for invoice settlement method.
        var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);
        var previousBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(inputDate, clearingHouse);

        return IsLateSubmission(invoice, inputDate, clearingHouse, previousBillingPeriod);
      }
      catch (ISCalendarDataNotFoundException dataNotFoundException)
      {
        Logger.Error("Unable to get last closed billing period.", dataNotFoundException);

        return false;
      }
    }

    protected bool IsLateSubmission(InvoiceBase invoice, DateTime inputDate, ClearingHouse clearingHouse, BillingPeriod billingPeriod)
    {
      return invoice.InvoiceBillingPeriod == billingPeriod && CalendarManager.IsLateSubmissionWindowOpen(clearingHouse,billingPeriod, inputDate);
    }

    /// <summary>
    /// Validates the invoice number.
    /// </summary>
    /// <param name="invoiceNumber">The invoice number.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <returns>True if successful; otherwise false.</returns>
    protected bool ValidateInvoiceNumber(string invoiceNumber, int billingYear, int billingMemberId)
    {
      //The ‘Invoice Number’ provided by the Billing Member should be unique for that calendar year 
      // TODO: Stored Procedure call to find duplicate invoice number for billing member id for that calendar year. Need to uncomment once Member Profile EDMX mapping done.
      var duplicateInvoiceCount = InvoiceRepository.IsInvoiceNumberExists(invoiceNumber, billingYear, billingMemberId);

      if (duplicateInvoiceCount > 0)
      {
        return false;
      }

      return true;
    }


    /// <summary>
    /// Return default value for settlement indicator for given combination of billing and billed members
    /// </summary>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <returns></returns>
    public virtual SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId)
    {
      var settlementMethod = SMI.Bilateral;

     // if ((IsDirectSettlementMember(BillingMember) || IsDirectSettlementMember(BilledMember)) && CheckIfSettlementIsBilateral(BillingMember,BilledMember))
      if (CheckIfSettlementIsBilateral(BillingMember, BilledMember))
      {
        settlementMethod = SMI.Bilateral;
      }


      // Both the members are DUAL.
       else if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
      {
        settlementMethod = SMI.Ach;
      }
         
      // Billing member is Dual.
      else if (IsDualMember(BillingMember))
      {
        settlementMethod = IsIchMember(BilledMember) ? SMI.Ich : IsAchMember(BilledMember) ? SMI.Ach : SMI.Bilateral;
      }
      // Billing member is ICH.
      else if (IsIchMember(BillingMember))
      {
        settlementMethod = SMI.Ich;
      }

      // Billing member is only ACH.
      else if (IsAchMember(BillingMember))
      {
        if (IsDualMember(BilledMember))
        {
          settlementMethod = SMI.Ach;
        }
        else if (IsAchMember(BilledMember))
        {
          settlementMethod = SMI.Ach;
        }
        else if (IsIchMember(BilledMember))
        {
          settlementMethod = SMI.AchUsingIataRules;
        }
      }

      return settlementMethod;
    }

    /// <summary>
    /// SCP282361 - Bug to be logged
    /// </summary>
    /// <param name="billingMember">Object of billing member</param>
    /// <param name="billedMember">Object of billing member</param>
    /// <returns>true/false</returns>
    public bool CheckIfSettlementIsBilateral(Member billingMember, Member billedMember)
    {
        var returnValue = false;

        if (((billedMember.IchMemberStatusId != (int)IchMemberShipStatus.Live) &&
            (billedMember.IchMemberStatusId != (int)IchMemberShipStatus.Suspended)
            && (billedMember.AchMemberStatusId != (int)AchMembershipStatus.Live) &&
            (billedMember.AchMemberStatusId != (int)AchMembershipStatus.Suspended)
           && (billingMember.IchMemberStatusId != (int)IchMemberShipStatus.Live) &&
            (billingMember.IchMemberStatusId != (int)IchMemberShipStatus.Suspended)
            && (billingMember.AchMemberStatusId != (int)AchMembershipStatus.Live) &&
            (billingMember.AchMemberStatusId != (int)AchMembershipStatus.Suspended)))
        {
            returnValue = true;
        }
        else if (IsDirectSettlementMember(billingMember) || IsDirectSettlementMember(billedMember))
        {
            returnValue = true;
        }

        return returnValue;
    }

    /// <summary>
    /// Build IS Validation flag
    /// </summary>
    /// <param name="iSValidationFlag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public string GetIsValidationFlag(string iSValidationFlag, string value)
    {
      if (iSValidationFlag != null)
      {
        iSValidationFlag = String.Format(iSValidationFlag.Trim().Length > 0 ? "{0},{1}" : "{0}{1}", iSValidationFlag, value);
        return iSValidationFlag;
      }
      return string.Empty;
    }

    /// <summary>
    /// Gets the member numeric code.
    /// </summary>
    /// <param name="memberCode">The member code.</param>
    /// <returns></returns>
    /// <remarks></remarks>
    public string GetMemberNumericCode(int memberCode)
    {
      if (memberCode <= 999)
      {
        return memberCode.ToString().PadLeft(3, '0');
      }
      var asciiValue = Convert.ToInt32(memberCode.ToString().Substring(0, 2)) + 55;
      return Convert.ToChar(asciiValue).ToString();
    }

    /// <summary>
    /// To get the status of suspended flag.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="billingMember"></param>
    /// <param name="billedMember"></param>
    /// <returns></returns>
    protected bool ValidateSuspendedFlag(InvoiceBase invoice, Member billingMember, Member billedMember)
    {
      //TO DO- later this code will be optimized.
      bool isSuspended = false;
      if (billingMember != null && billedMember != null)
      {
        billingMember.DualMemberStatus = billingMember.AchMemberStatus && billingMember.IchMemberStatus;
        billedMember.DualMemberStatus = billedMember.AchMemberStatus && billedMember.IchMemberStatus;

        if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules) //|| invoice.InvoiceSmi == SMI.AdjustmentDueToProtest)
        {

          if (billingMember.AchMemberStatus && billingMember.AchMemberStatusId == (int)AchMembershipStatus.Suspended)
          {
            isSuspended = true;
          }

          if (billedMember.AchMemberStatus && billedMember.AchMemberStatusId == (int)AchMembershipStatus.Suspended)
          {
            isSuspended = true;
          }

          if (billedMember.IchMemberStatus && billedMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended)
          {
            isSuspended = true;
          }

          if (billingMember.DualMemberStatus && billedMember.IchMemberStatus && billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended)
          {
            isSuspended = true;
          }

        }
        // CMP#624 : 2.22-Change#1 - Update of ‘Suspended Invoice’ flag
        else if (invoice.InvoiceSmi == SMI.Ich || invoice.InvoiceSmi == SMI.IchSpecialAgreement)
        {
          if (billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended)
          {
            isSuspended = true;
          }



          if (billedMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended)
          {
            isSuspended = true;
          }

          if (billedMember.AchMemberStatus && billedMember.AchMemberStatusId == (int)AchMembershipStatus.Suspended)
          {
            isSuspended = true;
          }

          if (billingMember.DualMemberStatus && billedMember.AchMemberStatus && billingMember.AchMemberStatusId == (int)AchMembershipStatus.Suspended)
          {
            isSuspended = true;
          }

          ////check if exception already exists for the member
          //var achExceptionRepository = Ioc.Resolve<IRepository<AchException>>(typeof(IRepository<AchException>));
          //var recordForThisRelationship =
          //  achExceptionRepository.Single(
          //    existingachException =>
          //    existingachException.MemberId == invoice.BillingMemberId && existingachException.ExceptionMemberId == invoice.BilledMemberId &&
          //    existingachException.BillingCategoryId == invoice.BillingCategoryId);

          //// SCP Issue #9173-Incorrect email sent for Bilateral Settlement
          //if (billingMember.DualMemberStatus && billedMember.DualMemberStatus && recordForThisRelationship != null && billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Live && billedMember.IchMemberStatusId == (int)IchMemberShipStatus.Live)
          //{
          //  isSuspended = false;
          //}
        }
      }
      return isSuspended;
    }

    /// <summary>
    /// Determines whether digital signature required for specified invoice header.
    /// </summary>
    /// <param name="invoiceHeader">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <returns>
    /// true if digital signature required for country listed in Billing/Billed member country list in eBilling section of member profile; otherwise, false.
    /// </returns>
    protected bool IsDigitalSignatureRequired(InvoiceBase invoiceHeader, Member billingMember, Member billedMember)
    {
      if (invoiceHeader.DigitalSignatureRequired == DigitalSignatureRequired.Yes
          && (IsDigitalSignatureRequiredForTheCountry(invoiceHeader, true)
              || IsDigitalSignatureRequiredForTheCountry(invoiceHeader, false)))
      {
        return true;
      }

      return false;
    }

    private bool IsDigitalSignatureRequiredForTheCountry(IEnumerable<MemberLocationInformation> memberLocationInfo, int memberId, string billingMemberLocationCode, bool isBillingMember)
    {
      var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

      var memberRefData = memberLocationInfo.SingleOrDefault(loc => loc.IsBillingMember == isBillingMember);

      if (memberRefData != null)
      {
        var dsRequiredCountries = memberManager.GetDsRequiredCountryList(memberId, isBillingMember ? (int)BillingTypes.Billing : (int)BillingTypes.Billed, false);

        
        if (string.IsNullOrEmpty(memberRefData.CountryCode))
        {
          var locationInfo = LocationRepository.Single(loc => loc.LocationCode == billingMemberLocationCode && loc.MemberId == memberId);

          if (locationInfo != null)
          {
            if (dsRequiredCountries.Any(dsCountry => dsCountry.Id == locationInfo.Country.Id))
            {
              return true;
            }

            if (DigitalSignRequiredforBillingBilledMember(memberLocationInfo, memberId,billingMemberLocationCode, isBillingMember, dsRequiredCountries))
              {
                  return true;
              }

          }
        }
        else if (dsRequiredCountries.Any(dsCountry => dsCountry.Id == memberRefData.CountryCode))
        {
          return true;
        }
        if (DigitalSignRequiredforBillingBilledMember(memberLocationInfo, memberId,billingMemberLocationCode, isBillingMember, dsRequiredCountries))
        {
            return true;
        }

      }
      return false;
    }

    /*  UC-G3320 - Evaluate whether DS is required for the Billing Member/Billed Member:
     * Condition: Either or both of the countries from the invoice reference data (Billing Member and Billed Member) are listed in profile element of the Billing Member
                  ‘As a Billing Member, DS Required for Invoices From/To’.
     * Author : Vinod Patil
     * Date   : 10/09/2012
    */
    private bool DigitalSignRequiredforBillingBilledMember(IEnumerable<MemberLocationInformation> memberLocationInfo, int memberId, string billingMemberLocationCode, bool isBillingMember, IEnumerable<Country> dsRequiredCountries)
      {
      var memberRefData = memberLocationInfo.SingleOrDefault(loc => loc.IsBillingMember == isBillingMember);

        if (memberRefData != null)
          {
            if (string.IsNullOrEmpty(memberRefData.CountryCode))
              {
                  var locationInfo = LocationRepository.Single(loc => loc.LocationCode == billingMemberLocationCode && loc.MemberId == memberId);

                  if (locationInfo != null)
                  {
                      if (dsRequiredCountries.Any(dsCountry => dsCountry.Id == locationInfo.Country.Id))
                      {
                          return true;
                      }
                  }
              }
              else if (dsRequiredCountries.Any(dsCountry => dsCountry.Id == memberRefData.CountryCode))
              {
                  return true;
              }
          }
          return false;
      }

    /// <summary>
    /// Determines whether digital signature required for specified member and the country.
    /// </summary>
    /// <param name="invoice">The invoice header.</param>
    /// <param name="isBillingMember"></param>
    /// <returns>
    /// true if digital signature required for country listed in Billing/Billed member country list in eBilling section of member profile; otherwise, false.
    /// </returns>
    public bool IsDigitalSignatureRequiredForTheCountry(InvoiceBase invoice, bool isBillingMember)
    {
      return IsDigitalSignatureRequiredForTheCountry(invoice.MemberLocationInformation, isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId, isBillingMember ? invoice.BillingMemberLocationCode : invoice.BilledMemberLocationCode, isBillingMember);
    }

    /// <summary>
    /// Sets the digital signature required by. billedMember parameter Added to fix issue of New member created on submit invoice
    /// </summary>
    /// <param name="invoice">The invoice.</param>
    /// <param name="billingMember">The billing member.</param>
    /// /// <param name="billedMember">The billedmember.</param>
    protected void SetDigitalSignatureInfo(InvoiceBase invoice, Member billingMember, Member billedMember)
    {
      var requiredByBillingMember = false;
      var requiredByBilledMember = false;

      //if (billingMember != null && billingMember.DigitalSignApplication
      //     && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes || invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
      //{
      //  requiredByBillingMember = true;
      //}

      //if (billedMember != null && billedMember.DigitalSignApplication)
      //{
      //  requiredByBilledMember = true;
      //}
      if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes))
      {
          requiredByBillingMember = true;
      }
      else if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
      {
          if (IsDigitalSignatureRequiredForTheCountry(invoice, true))
          {
              requiredByBillingMember = true;
          }
          else if (billedMember != null && billedMember.DigitalSignApplication)
          {
              if (IsDigitalSignatureReqByEitherOrBothMember(invoice.BillingMemberId, true,
                                                            invoice.MemberLocationInformation))
              {
                  requiredByBillingMember = true;
              }
          }
      }

      if (billedMember != null && billedMember.DigitalSignApplication)
      {
          if (IsDigitalSignatureRequiredForTheCountry(invoice, false))
          {
              requiredByBilledMember = true;
          }
          else if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default || invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes))
          {
              if (IsDigitalSignatureReqByEitherOrBothMember(invoice.BilledMemberId, false,
                                                            invoice.MemberLocationInformation))
              {
                  requiredByBilledMember = true;
              }
          }
      }

      // Update DsRequiredBy of invoice based on digital signature required by billing member and billed member.
      invoice.DsRequirdBy = GetDigitalSignatureRequiredBy(requiredByBillingMember, requiredByBilledMember);
      invoice.DsStatus = GetDigitalSignatureStatus(invoice.DsRequirdBy, invoice, billedMember);
    }

    private bool IsDigitalSignatureReqByEitherOrBothMember(int memberId, bool isBillingMember, IEnumerable<MemberLocationInformation> memberLocationInfo)
      {
        try
        {

          //Either or both of the countries from the invoice reference data (Billing Member and Billed Member) are listed 
          //in profile element of the Billing/billed Member ‘As a Billing/billed Member, DS Required for Invoices From/To’.
          var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
          // Get Billed member country in case isBilling= True and vice-versa
          var dsReqCountries = memberManager.GetDsRequiredCountryList(memberId,
                                                                      isBillingMember
                                                                        ? (int)BillingTypes.Billing
                                                                        : (int)BillingTypes.Billed, false);


          Logger.InfoFormat("IsDigitalSignatureReqByEitherOrBothMember : Total DS REquired Country Found for Member ID {0} is Count {1} ", memberId, dsReqCountries.Count);

          if (memberLocationInfo == null)
          {
            Logger.Info("memberLocationInfo object is NULL");
          }

          var receivableLocData =
            memberLocationInfo.FirstOrDefault(loc => loc.IsBillingMember == isBillingMember ? true : false);

          if (receivableLocData == null)
          {
            Logger.Info("receivableLocData object is NULL");
          }

          var payableLocData =
            memberLocationInfo.FirstOrDefault(loc => loc.IsBillingMember == isBillingMember ? false : true);

          if (payableLocData == null)
          {
            Logger.Info("payableLocData object is NULL");
          }

          return
            dsReqCountries.Any(
              dsCountry => dsCountry.Id == receivableLocData.CountryCode || dsCountry.Id == payableLocData.CountryCode);
        }
        catch (Exception exception)
        {
          Logger.Error("Exception Occured at IsDigitalSignatureReqByEitherOrBothMember", exception);
          return false;
        }
      }

    public string GetDigitalSignatureStatus(string dsRequiredBy, InvoiceBase invoice, Member billedMember)
    {
      if (dsRequiredBy == "N")
      {
        return "N";
      }

      if (dsRequiredBy != "B" && dsRequiredBy != "D" && dsRequiredBy != "C")
      {
        return string.Empty;
      }

      invoice.DigitalSignatureStatus = DigitalSignatureStatus.Pending;

      if (dsRequiredBy == "C")
      {
        return "Y";
      }

      if (billedMember.DigitalSignVerification)
      {
        return "V";
      }

      return "Y";
    }

    /// <summary>
    /// Determines whether digital signature required by as per the DS Required flag of the member
    /// </summary>
    /// <returns>
    /// true if digital signature required for country listed in Billing/Billed member country list in eBilling section of member profile; otherwise, false.
    /// </returns>
    public string GetDigitalSignatureRequiredBy(bool dsRequiredByBillingMember, bool dsRequiredByBilledMember)
    {
      return dsRequiredByBillingMember && dsRequiredByBilledMember ? "B" : (dsRequiredByBillingMember ? "C" : (dsRequiredByBilledMember ? "D" : "N"));
    }

    /// <summary>
    /// Checks the blocked member.
    /// </summary>
    /// <param name="isDebtor">if set to true [is debtor].</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="isPax"></param>
    /// <param name="isMisc"></param>
    /// <param name="isUatp"></param>
    /// <param name="isCargo"></param>
    protected bool CheckBlockedMember(bool isDebtor, int billingMemberId, int billedMemberId, bool isPax = false, bool isMisc = false, bool isUatp = false, bool isCargo = false)
    {
      var isBlocked = false;

      var blockDebtorMember = BlockingRulesManager.GetBlockedMemberList(billingMemberId, isDebtor, isPax, isMisc, isUatp, isCargo);
      if (BlockingRulesManager.IsMemeberBlocked(billedMemberId, blockDebtorMember))
      {
        isBlocked = true;
      }

      return isBlocked;
    }

    /// <summary>
    /// Used to update member location information for submitted invoices. 
    /// While creating/updating invoices, member location information table stores entries only for blank location codes.
    /// </summary>
    /// <param name="updatedInvoice"></param>
    protected void UpdateMemberLocationInformation(InvoiceBase updatedInvoice)
    {
      if (!string.IsNullOrEmpty(updatedInvoice.BillingMemberLocationCode))
      {
        var billingMemberLocation = LocationRepository.Single(ml => ml.MemberId == updatedInvoice.BillingMemberId && ml.LocationCode.ToUpper() == updatedInvoice.BillingMemberLocationCode.ToUpper());
        var memberLocationInformation = GetMemberLocationInformation(billingMemberLocation, true, updatedInvoice.Id);

        // If member location information exist for given invoice, fetch it and delete before adding new member location information.
        var memlocationInfo = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == memberLocationInformation.InvoiceId && memLoc.IsBillingMember);
        
        if (memlocationInfo != null)
        {
            //SCP 107966 - SIS Member Profile - invoice footer detail, check if other field values are present, then location info is stored previously
            //memberLocationInformation.LegalText = memlocationInfo.LegalText;
            if (!string.IsNullOrEmpty(memlocationInfo.AddressLine1) && !string.IsNullOrEmpty(memlocationInfo.CountryCode) && !string.IsNullOrEmpty(memlocationInfo.CityName))
                memberLocationInformation.LegalText = memlocationInfo.LegalText;
            MemberLocationInfoRepository.Delete(memlocationInfo);
        }

        MemberLocationInfoRepository.Add(memberLocationInformation);
        string leglText = memberLocationInformation.LegalText != null ? memberLocationInformation.LegalText.Replace("\r", "").Replace("\n", "") : string.Empty;
        updatedInvoice.LegalText = leglText;

      }
      else
      {
        // If member location information exist for given invoice, i.e. member location info is added by user, copy legal text to invoice legal text
        var existingMemlocationInfo = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == updatedInvoice.Id && memLoc.IsBillingMember);
        string leglText = existingMemlocationInfo.LegalText != null ? existingMemlocationInfo.LegalText.Replace("\r", "").Replace("\n", "") : string.Empty;
        updatedInvoice.LegalText = leglText;
      }

      //If legal text in invoice is empty, i.e. location legal text is empty then assign Ebilling config legal text
      if (string.IsNullOrEmpty(updatedInvoice.LegalText))
      {
        var eBillingConfig = MemberManager.GetEbillingConfig(updatedInvoice.BillingMemberId);
        updatedInvoice.LegalText = eBillingConfig != null && eBillingConfig.LegalText != null ? eBillingConfig.LegalText.Replace("\r", "").Replace("\n", "") : string.Empty;
      }

      if (!string.IsNullOrEmpty(updatedInvoice.BilledMemberLocationCode))
      {
        var billedMemberLocation = LocationRepository.Single(ml => ml.MemberId == updatedInvoice.BilledMemberId && ml.LocationCode.ToUpper() == updatedInvoice.BilledMemberLocationCode.ToUpper());
        var billedMemberLocationInformation = GetMemberLocationInformation(billedMemberLocation, false, updatedInvoice.Id);
        //SCP85039: Changed call of Single method to Get method
        //var memlocationInfo = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == billedMemberLocationInformation.InvoiceId && memLoc.IsBillingMember == false);
        var memlocations = MemberLocationInfoRepository.Get(memLoc => memLoc.InvoiceId == billedMemberLocationInformation.InvoiceId && memLoc.IsBillingMember == false);
        var memlocationInfo = memlocations.FirstOrDefault();
        if (memlocationInfo != null)
        {
          MemberLocationInfoRepository.Delete(memlocationInfo);
        }

        MemberLocationInfoRepository.Add(billedMemberLocationInformation);
      }
    }

    private static MemberLocationInformation GetMemberLocationInformation(Location memberLocation, bool isBillingMember, Guid invoiceId)
    {
      return new MemberLocationInformation
      {
        CompanyRegistrationId = memberLocation.RegistrationId,
        AddressLine1 = memberLocation.AddressLine1,
        AddressLine2 = memberLocation.AddressLine2,
        AddressLine3 = memberLocation.AddressLine3,
        CityName = memberLocation.CityName,
        DigitalSignatureRequiredId = 0,
        SubdivisionName = memberLocation.SubDivisionName,
        SubdivisionCode = memberLocation.SubDivisionCode,
        LegalText = memberLocation.LegalText,
        OrganizationName = string.IsNullOrEmpty(memberLocation.MemberLegalName) ? " " : memberLocation.MemberLegalName,
        PostalCode = memberLocation.PostalCode,
        TaxRegistrationId = memberLocation.TaxVatRegistrationNumber,
        MemberLocationCode = memberLocation.LocationCode,
        CountryCode = memberLocation.Country != null ? memberLocation.Country.Id : string.Empty,
        AdditionalTaxVatRegistrationNumber = memberLocation.AdditionalTaxVatRegistrationNumber,
        IsBillingMember = isBillingMember,
        InvoiceId = invoiceId,
        CountryName = memberLocation.Country != null ? memberLocation.Country.Name : string.Empty
      };
    }

    protected DateTime GetBillingDate(InvoiceBase invoice)
    {
      DateTime billingDate;

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      //To search exchange rate for the billing month.
      DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString("00"), "01"), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      return billingDate;
    }


    protected DateTime GetTransactionDate(InvoiceBase invoice)
    {
      DateTime billingDate;

      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.Calendar.TwoDigitYearMax = 2099;
      const string billingDateFormat = "yyyyMMdd";

      //To search exchange rate for the billing month.
      DateTime.TryParseExact(string.Format("{0}{1}{2}", invoice.BillingYear, invoice.BillingMonth.ToString("00"), invoice.BillingPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out billingDate);
      return billingDate;
    }

    /// <summary>
    /// Validates city airport code using application level caching.
    /// </summary>
    /// <param name="cityAirportCode"></param>
    /// <returns></returns>
    protected bool IsValidCityAirportCode(string cityAirportCode)
    {
        /*  SCP# 125085: [CA-999] QUESTION ABOUT THE CXMLT FILE DOWNLOAD FROM IS-WEB.
	        Desc : Empty cityAirport code is invalid so returning false.
	        Date : 23-May-2013
         * 
         * Comment 2:
         * Date: 03-June-2013
         * Desc: reverting the change, i.e. -> if cityAirportCode is IsNullOrEmpty then returning true.
         * This was done because of QA reported problem. This is a common function used in all categories.
         * if we return false when cityAirportCode is IsNullOrEmpty --> Validation error is generated in cases like below -
         * !IsValidCityAirportCode(couponRecord.ToAirportOfCoupon) here ToAirport is not mandetory and hence the problem.
         * 
         * By reverting the change SCP# 125085 has no impact as we already check for null or whitespace before calling this function.
         * By reverting this change we can avoid existing things from breaking.
         * 
        */
        if (String.IsNullOrEmpty(cityAirportCode))
        return true;

      var isValidAirportCode = true;

      // Validate city airport code against application level cache of city airport codes.
      if (ValidationCache.Instance.ValidCityAirportCodes != null)
      {
        if (!ValidationCache.Instance.ValidCityAirportCodes.ContainsKey(cityAirportCode))
        {
          isValidAirportCode = false;
        }
      }
      else
      {
        // If ValidationCache.CityAirportCodes are null make DB call.
        isValidAirportCode = ReferenceManager.IsValidAirportCode(cityAirportCode);
      }

      return isValidAirportCode;
    }








    /// <summary>
    ///  Validates Unloc code using application level caching.
    /// </summary>
    /// <param name="unlocCode"></param>
    /// <returns></returns>
    protected bool IsValidUnlocCode(string unlocCode)
    {
      if (String.IsNullOrEmpty(unlocCode))
        return true;

      var isValidUnlocCode = true;
      // Validate city airport code against application level cache of city airport codes.
      if (ValidationCache.Instance.ValidUnlocCodes != null)
      {
        if (!ValidationCache.Instance.ValidUnlocCodes.ContainsKey(unlocCode))
        {
          isValidUnlocCode = false;
        }
      }
      else
      {
        // If ValidationCache.CityAirportCodes are null make DB call.
        isValidUnlocCode = UnlocCodeManager.IsValidUnlocCode(unlocCode);
      }

      return isValidUnlocCode;
    }

    /// <summary>
    /// Gets the Min Max acceptable amount from the application cache if it is initialized else DB call is made.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    public MaxAcceptableAmount GetMaxAcceptableAmount(InvoiceBase invoice, string clearingHouse, TransactionType transactionType)
    {
      // If invoice contains Min Max Acceptable amounts then don't make DB call.
      MaxAcceptableAmount maxAcceptableAmount;
      var transactionDate = GetTransactionDate(invoice);
      if (ValidationCache.Instance.ValidMaxAcceptableAmounts == null)
      {
        maxAcceptableAmount = MaxAcceptableAmountRepository.Single(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);
      }
      else
      {
        maxAcceptableAmount = ValidationCache.Instance.ValidMaxAcceptableAmounts.SingleOrDefault(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse && rec.EffectiveFromPeriod <= transactionDate && rec.EffectiveToPeriod >= transactionDate);
      }

      return maxAcceptableAmount;
    }

    /// <summary>
    /// Gets the Min acceptable amounts from the application cache if it is initialized else DB call is made.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="clearingHouse"></param>
    /// <param name="transactionType"></param>
    /// <returns></returns>
    public List<MinAcceptableAmount> GetMinAcceptableAmounts(InvoiceBase invoice, string clearingHouse, TransactionType transactionType)
    {
      // If invoice contains Min Max Acceptable amounts then dont make DB call.
      List<MinAcceptableAmount> minAcceptableAmounts;
      if (ValidationCache.Instance.ValidMinAcceptableAmounts == null)
      {
        minAcceptableAmounts = MinAcceptableAmountRepository.Get(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse).ToList();
      }
      else
      {
        minAcceptableAmounts = ValidationCache.Instance.ValidMinAcceptableAmounts.FindAll(rec => rec.IsActive && rec.TransactionTypeId == (int)transactionType && rec.ClearingHouse == clearingHouse);
      }

      return minAcceptableAmounts;
    }

    /// <summary>
    /// Get Settlement method for Auto billing invoices
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="BillingMember"></param>
    /// <param name="BilledMember"></param>
    /// <returns></returns>
    protected int GetSettlementMethodForAutoBillingInvoice(InvoiceBase invoice, Member BillingMember, Member BilledMember)
    {
      try
      {

        if (IsNoClearingHouseMember(BillingMember) && IsNoClearingHouseMember(BilledMember))
        {
          return (int)SMI.Bilateral;
        }

        // if Billing Member and billed member both are dual member
        if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
        {
          // Check for ACH Exception "Settle through ICH",
          // SCP109712 : ACH exceptions for settlement via ICH 
          // Exceptions related to Settlement via ICH for dual members should be only checked only from the Billing Member’s point of view
          var achExceptionsBillingMember = MemberManager.GetExceptionMembers(BillingMember.Id, (int)invoice.BillingCategory, false).ToList();
          if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0)
          {
            return (int)SMI.Ich;
          }

          // Default
          return (int)SMI.Ach;

        }
        //// Billing member is Dual.
        if (IsDualMember(BillingMember))
        {
          // and Billed Member is ICH, then SMI should be ICH.
          if (IsIchMember(BilledMember))
          {
            return (int)SMI.Ich;
          }
          if (IsAchMember(BilledMember))
          {
            return (int)SMI.Ach;
          }
        }
        // Billing member is only ICH, then SMI should be ICH, irrespective of BilledMember status.
        else if (IsIchMember(BillingMember))
        {
          if (IsIchMember(BilledMember) || IsAchMember(BilledMember))
          {
            return (int)SMI.Ich;
          }
        }
        // Billing member is only ACH.
        else if (IsAchMember(BillingMember))
        {
          // if Billed Member is dual or only ACH, then SMI should be A or M.
          if (IsDualMember(BilledMember) || IsAchMember(BilledMember))
          {
            return (int)SMI.Ach;
          }
          // if Billed Member is only ICH, then SMI should be M.
          if (IsIchMember(BilledMember))
          {
            return (int)SMI.AchUsingIataRules;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.InfoFormat(ex.Message);
      }
      // Default
      return (int)SMI.Ich;
    }
    /// <summary>
    /// Validates the settlement method.
    /// </summary>
    /// <param name="invoice">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="invoiceTypeId"></param>
    /// <returns></returns>
    protected virtual bool ValidatePaxAndCargoSettlementMethod(InvoiceBase invoice, Member billingMember, Member billedMember, int invoiceTypeId)
    {
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (invoiceTypeId == (int)InvoiceType.Invoice && (invoice.SettlementMethodId != (int)SMI.Ich && invoice.SettlementMethodId != (int)SMI.Ach &&
        !ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId,true) && invoice.SettlementMethodId != (int)SMI.AchUsingIataRules))
      {
        return false;
      }

      if (invoiceTypeId == (int)InvoiceType.CreditNote && invoice.SettlementMethodId == (int)SMI.AdjustmentDueToProtest)
      {
        return true;
      }

      BillingMember = billingMember ?? (BillingMember = MemberManager.GetMember(invoice.BillingMemberId));
      BilledMember = billedMember ?? (BilledMember = MemberManager.GetMember(invoice.BilledMemberId));

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId,true)) return true;

      // if Billing Member and billed member both are dual member
      if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
      {
        // Check for ACH Exception "Settle through ICH",
        // SCP109712 : ACH exceptions for settlement via ICH 
        // Exceptions related to Settlement via ICH for dual members should be only checked only from the Billing Member’s point of view
        var achExceptionsBillingMember = MemberManager.GetExceptionMembers(BillingMember.Id, (int)invoice.BillingCategory, false).ToList();
        if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0)
        {
          return invoice.InvoiceSmi == SMI.Ich;
        }

        // if exception is not present in any of the member list for other member, then SMI should be A or M.
        if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
        {
          return true;
        }

      }
      //// Billing member is Dual.
      else if (IsDualMember(BillingMember))
      {
        // and Billed Member is ICH, then SMI should be ICH.
        if (IsIchMember(BilledMember) && (invoice.InvoiceSmi == SMI.Ich)) return true;
        // and Billed Member is ACH, then SMI should be ACH or ACH using IATA rules.
        if (IsAchMember(BilledMember))
        {
          if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
          {
            return true;
          }
        }
      }
      // Billing member is only ICH, then SMI should be ICH, irrespective of BilledMember status.
      else if (IsIchMember(BillingMember))
      {
        if (IsIchMember(BilledMember) || IsAchMember(BilledMember))
        {
          if (invoice.InvoiceSmi == SMI.Ich) return true;
        }
      }

      // Billing member is only ACH.
      else if (IsAchMember(BillingMember))
      {
        // if Billed Member is dual or only ACH, then SMI should be A or M.
        if (IsDualMember(BilledMember) || IsAchMember(BilledMember))
        {
          if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
          {
            return true;
          }
        }
        // if Billed Member is only ICH, then SMI should be M.
        else if (IsIchMember(BilledMember))
        {
          if (invoice.InvoiceSmi == SMI.AchUsingIataRules) return true;
        }
      }

      return false;
    }

    
    /// <summary>
    /// Validates the invoice date by comparing it with file submission billing period closure date.
    /// </summary>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateParsedInvoiceDate(DateTime invoiceDate, BillingPeriod billingPeriod)
    {
      var result = true;

      // Invoice date should not be greater than billing period closure date.
      if (invoiceDate.Date > billingPeriod.EndDate.Date)
      {
        result = false;
      }

      return result;
    }

    /// <summary>
    /// Validates the invoice date by comparing it with current billing period closure date.
    /// </summary>
    /// <param name="invoiceDate">The invoice date.</param>
    /// <returns>True if successful; otherwise false.</returns>
    public bool ValidateInvoiceDate(DateTime invoiceDate)
    {
      var result = true;
      var billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);  //GetCurrentBillingPeriod();

      // Invoice date should not be greater than current billing period closure date.
      if (invoiceDate.Date > billingPeriod.EndDate.Date)
      {
        result = false;
      }

      return result;
    }

    

    #region Code fix for SCP76017
    /// <summary>
    /// Following method is used to Validation for Blocked Airline while Creating, Validating and Submitting invoice.
    /// </summary>
    /// <param name="invoiceHeader">Invoice object</param>
    public void ValidationForBlockedAirline(InvoiceBase invoiceHeader)
    {
        var isCreditorBlocked = false;
        var isDebitorBlocked = false;
        var isCGrpBlocked = false;
        var isDGrpBlocked = false;

        // Validation for Blocked Airline
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
        if (invoiceHeader.BilledMemberId != 0 && invoiceHeader.BillingMemberId != 0 &&
            (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true) &&
             invoiceHeader.InvoiceSmi != SMI.AdjustmentDueToProtest && invoiceHeader.InvoiceSmi != SMI.NoSettlement))
        {
            ValidationForBlockedAirline(invoiceHeader.BillingMemberId, invoiceHeader.BilledMemberId, (SMI)invoiceHeader.SettlementMethodId, invoiceHeader.BillingCategory,
                out isCreditorBlocked, out isDebitorBlocked, out isCGrpBlocked, out isDGrpBlocked);
        }

        // Blocked by Creditor
        if (isCreditorBlocked)
        {
            //SCP164383: Blocking Rule Failed
            //Desc: Centralized code for blocking rule validation.
            throw new ISBusinessException(ErrorCodes.InvalidBillingToMember);
        }
        // Blocked by Debtor
        if (isDebitorBlocked)
        {
            //SCP164383: Blocking Rule Failed
            //Desc: Centralized code for blocking rule validation.
            throw new ISBusinessException(ErrorCodes.InvalidBillingFromMember);
        }

        //Validate BlockBy Group Rule
        if (isCGrpBlocked) //biling block?
        {
            throw new ISBusinessException(ErrorCodes.InvalidBillingToMemberGroup);
        }

        if (isDGrpBlocked) //billed block?
        {
            throw new ISBusinessException(ErrorCodes.InvalidBillingFromMemberGroup);
        }
    }

    //SCP164383: Blocking Rule Failed
    //Desc: Centralized code for blocking rule validation.
    public void ValidationForBlockedAirline(int billingMemberId, int billedMemberId, SMI sMIndicator, BillingCategoryType billingCategory, 
        out bool isCreditorBlocked, out bool isDebitorBlocked, out bool isCGrpBlocked, out bool isDGrpBlocked, bool isCheckForZoneZoro = false)
    {  
        var smiValue = string.Empty;
        isCreditorBlocked = false;
        isDebitorBlocked = false;
        isCGrpBlocked = false;
        isDGrpBlocked = false;
        var billedZoneId = 0;

        if (billedMemberId == 0 || billingMemberId == 0)
        {
            return;
        }

        if (BlockingRulesRepository == null)
        {
            BlockingRulesRepository = Ioc.Resolve<IBlockingRulesRepository>(typeof(IBlockingRulesRepository));
        }

        /* Switch on basis of SMI */
        switch(sMIndicator)
        {
            case SMI.Ach:
                smiValue = "ACH";
                BlockingRulesRepository.ValidateBlockingRules(billingMemberId,
                                                                  billedMemberId,
                                                                  billingCategory, smiValue,
                                                                  achZoneId, achZoneId,
                                                                  out isCreditorBlocked, out isDebitorBlocked,
                                                                  out isCGrpBlocked, out isDGrpBlocked);
                break;
            case SMI.AchUsingIataRules:
                smiValue = "ACH";
                //SCP ID : 282347 - Question about CMP 467 
                var billedIchConfigUsingIata = GetIchConfiguration(billedMemberId);
                if (billedIchConfigUsingIata != null)
                {
                  if (billedIchConfigUsingIata.IchMemberShipStatusId == 1 && (billedIchConfigUsingIata.Member != null && billedIchConfigUsingIata.Member.AchMemberStatusId == 1))
                    billedZoneId = 3;
                  else
                    billedZoneId = billedIchConfigUsingIata.IchZoneId;
                }
                else
                {
                  var achRecordQuery = GetAchConfiguration(billedMemberId);
                  billedZoneId = achRecordQuery != null ? 3 : 0;
                }

                BlockingRulesRepository.ValidateBlockingRules(billingMemberId,
                                                                  billedMemberId,
                                                                  billingCategory, smiValue,
                                                                  achZoneId, billedZoneId,
                                                                  out isCreditorBlocked, out isDebitorBlocked,
                                                                  out isCGrpBlocked, out isDGrpBlocked);
                break;
            case SMI.Ich:
                smiValue = "ICH";

                var billingIchConfi = GetIchConfiguration(billingMemberId);
                var billingZoneId = 0;
                if(billingIchConfi != null)
                {
                    billingZoneId = billingIchConfi.IchZoneId;
                }
                else
                {
                    var achRecordQuery = GetAchConfiguration(billingMemberId);
                    billingZoneId = achRecordQuery != null ? 3 : 0;
                }

                var billedIchConfig = GetIchConfiguration(billedMemberId);
                billedZoneId = 0;
                if(billedIchConfig != null)
                {
                    billedZoneId = billedIchConfig.IchZoneId;
                }
                else
                {
                    var achRecordQuery = GetAchConfiguration(billedMemberId);
                    billedZoneId = achRecordQuery != null ? 3 : 0;
                }

                if (billingZoneId != 0 && billedZoneId != 0)
                {
                    BlockingRulesRepository.ValidateBlockingRules(billingMemberId,
                                                                  billedMemberId,
                                                                  billingCategory, smiValue,
                                                                  billingZoneId, billedZoneId,
                                                                  out isCreditorBlocked, out isDebitorBlocked,
                                                                  out isCGrpBlocked, out isDGrpBlocked);
                }
                else
                {
                    if(isCheckForZoneZoro)
                    {
                        BlockingRulesRepository.ValidateBlockingRules(billingMemberId,
                                                                  billedMemberId,
                                                                  billingCategory, smiValue,
                                                                  billingZoneId, billedZoneId,
                                                                  out isCreditorBlocked, out isDebitorBlocked,
                                                                  out isCGrpBlocked, out isDGrpBlocked);
                    }
                }
                break;
            default:
                // In case of any SMI listed below, need no validations.
                //SMI.Bilateral
                //SMI.AdjustmentDueToProtest
                //SMI.NoSettlement
                break;
        }
    }

    #endregion

    #region CMP602
    /// <summary>
    /// Set ViewableByClearingHouse
    /// </summary>
    /// <param name="invoice"></param>
    protected void SetViewableByClearingHouse(InvoiceBase invoice)
    {
      Logger.InfoFormat("Update ViewableByClearingHouse forInvoice Id. {0} ", invoice.Id);

      switch (invoice.InvoiceSmi)
      {
        case SMI.IchSpecialAgreement:
        case SMI.AchUsingIataRules:
        case SMI.Ach:
        case SMI.Ich:
          {
            //Get Billed Member Detail
            var ichConfigOfBilledMember = MemberManager.GetIchConfig(invoice.BilledMemberId);
            var achConfigOfBilledMember = MemberManager.GetAchConfig(invoice.BilledMemberId);           

            if(invoice.InvoiceSmi == SMI.Ich)
            {
              //Rule 1
              if (ichConfigOfBilledMember != null && (ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Live || ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Suspended))
              {
                invoice.ViewableByClearingHouse = "I";
              }
                //Rule 2
              else if (achConfigOfBilledMember != null && (achConfigOfBilledMember.AchMembershipStatus == AchMembershipStatus.Live || achConfigOfBilledMember.AchMembershipStatus == AchMembershipStatus.Suspended))
              {
                invoice.ViewableByClearingHouse = "B";
              }
            }
            if (invoice.InvoiceSmi == SMI.IchSpecialAgreement)
            {
              //Rule 3
              if (ichConfigOfBilledMember !=null && (ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Live || ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Suspended))
              {
                invoice.ViewableByClearingHouse = "I";
              }
            }
            if (invoice.InvoiceSmi == SMI.Ach)
            {
              //Rule 4
              invoice.ViewableByClearingHouse = "A";
            }
            if (invoice.InvoiceSmi == SMI.AchUsingIataRules)
            {
              //Rule 5
              if (achConfigOfBilledMember != null && (achConfigOfBilledMember.AchMembershipStatus == AchMembershipStatus.Live || achConfigOfBilledMember.AchMembershipStatus == AchMembershipStatus.Suspended))
              {
                invoice.ViewableByClearingHouse = "A";
              }
                //Rule 6
              else if (ichConfigOfBilledMember != null && (ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Live || ichConfigOfBilledMember.IchMemberShipStatus == IchMemberShipStatus.Suspended))
              {
                invoice.ViewableByClearingHouse = "B";
              }
            }
          }
          break;
        default:
          invoice.ViewableByClearingHouse = null;
          break;
      }
      Logger.InfoFormat("Updated Value of ViewableByClearingHouse : {0} ", invoice.ViewableByClearingHouse);
    }

    #endregion

    #region CMP #624: ICH Rewrite-New SMI X

    /// <summary>
    /// Validates the settlement method X.
    /// </summary>
    /// <param name="invoice">The invoice header.</param>
    /// <param name="billingMember">The billing member.</param>
    /// <param name="billedMember">The billed member.</param>
    /// <param name="invoiceTypeId">The invoice type id.</param>
    /// <param name="fieldNames">The field names.</param>
    /// <returns></returns>
    protected virtual bool ValidateSettlementMethodX(InvoiceBase invoice, Member billingMember, Member billedMember, IList<string> fieldNames = null)
    {


      BillingMember = billingMember ?? (BillingMember = MemberManager.GetMember(invoice.BillingMemberId));
      BilledMember = billedMember ?? (BilledMember = MemberManager.GetMember(invoice.BilledMemberId));

      /* CMP #624: ICH Rewrite-New SMI X 
       * Description: Validation: Both the Billing Member and the Billed Member should be ICH members. FRS Reference: 2.8,  New Validation #1:
      */
      if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
      {
        if (!IsIchMember(BillingMember))
        {
          if (fieldNames != null) fieldNames.Add("Billing Member");
        }
        if (!IsIchMember(BilledMember))
        {
          if (fieldNames != null) fieldNames.Add("Billed Member");
        }

        if (fieldNames != null && fieldNames.Count > 0)
        {
          return false;
        }
        else
        {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Method to perform invoice header level validations befor making Ich Web Service call for SMI X. 
    /// All Validations must pass before making web service call.
    /// </summary>
    /// <param name="invoice"></param>
    /// <param name="exceptionDetailsList"></param>
    /// <param name="invoiceTypeId"></param>
    /// <param name="fileDate"></param>
    /// <param name="fileName"></param>
    /// <param name="billingFinalParent"></param>
    /// <param name="billedFinalParent"></param>
    /// <param name="isCalledFromISWeb"></param>
    /// <param name="linkedInvoice"></param>
    /// <param name="batchSequenceNumber"></param>
    /// <param name="recordSequenceWithinBatch"></param>
    /// <returns></returns>
    public bool ValidationBeforeSmiXWebServiceCall(InvoiceBase invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, int invoiceTypeId, DateTime? fileDate, string fileName, Member billingFinalParent, Member billedFinalParent, bool isCalledFromISWeb, MiscUatpInvoice linkedInvoice = null, int batchSequenceNumber = 99999, int recordSequenceWithinBatch = 99999)
    {
        try
        {
            var fileSubmissionDate = fileDate.HasValue ? fileDate.Value : new DateTime();

            #region FRS Section 2.8 New Validation #1: ICH Membership Check

            /* Refer: FRS Section 2.8, New Validation #1 */
            /* This is common code for pax, cargo and MU File validations. Below existing code is enhanced and moved here as per CMP #624 requirements. */
            IList<string> fieldNames = new List<string>();
            // Validate Billing Billed Membership status for SMI X
            if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement )
            {
              if (!ValidateSettlementMethodX(invoice, billingFinalParent, billedFinalParent, fieldNames))
              {
                invoice.SetSmiXPhase1ValidationStatus(false);
                foreach (var fieldName in fieldNames)
                {
                  if (isCalledFromISWeb)
                  {
                    throw new ISBusinessException(ErrorCodes.IchMembershipCheckForSmiX);
                  }
                  else
                  {
                    var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                       exceptionDetailsList.Count() +
                                                                                       1,
                                                                                       fileSubmissionDate,
                                                                                       fieldName,
                                                                                       invoice.SettlementMethodDisplayText,
                                                                                       invoice, fileName,
                                                                                       ErrorLevels.ErrorLevelInvoice,
                                                                                       ErrorCodes.IchMembershipCheckForSmiX,
                                                                                       ErrorStatus.X,
                                                                                       invoice.BillingCode, 0,
                                                                                       batchSequenceNumber,
                                                                                       recordSequenceWithinBatch);

                    exceptionDetailsList.Add(validationExceptionDetail);
                  }
                }
              }
              else
              {
                //CMP#602
                SetViewableByClearingHouse(invoice);
              }
            }

            #endregion

            #region FRS Section 2.8 New Validation #2: CH Agreement Indicator Check

            string errorCode = "";
            bool chAgreementIndicatorValidation = ValidateChAgreementIndicator(invoice.BillingCategoryId, invoice.BillingCode, invoice.SettlementMethodId, invoice.ChAgreementIndicator, ref errorCode);
            if (!chAgreementIndicatorValidation)
            {
                invoice.SetSmiXPhase1ValidationStatus(false);

                /* Add exception details */
                if (isCalledFromISWeb)
                {
                    throw new ISBusinessException(errorCode);
                }
                else
                {
                    var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                       exceptionDetailsList.Count() +
                                                                                       1,
                                                                                       fileSubmissionDate,
                                                                                       "CH Agreement Indicator",
                                                                                       invoice.
                                                                                           ChAgreementIndicator,
                                                                                       invoice, fileName,
                                                                                       ErrorLevels.ErrorLevelInvoice,
                                                                                       errorCode,
                                                                                       ErrorStatus.X,
                                                                                       invoice.BillingCode, 0,
                                                                                       batchSequenceNumber,
                                                                                       recordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                }
            }

            invoice.SetSmiXPhase1ValidationStatus(chAgreementIndicatorValidation);

            #endregion

            #region FRS Section 2.8 New Validation #3: CH Due Date Check

            errorCode = "";
            string errorfieldName = "";
            switch (invoice.SubmissionMethodId)
            {
                case (int)SubmissionMethod.IsIdec:
                    errorfieldName = "CH Due Date";
                    break;
                case (int)SubmissionMethod.IsXml:
                    errorfieldName = "Net Due Date";
                    break;
                case (int)SubmissionMethod.IsWeb:
                    if (invoice.BillingCategoryId == (int)BillingCategoryType.Pax || invoice.BillingCategoryId == (int)BillingCategoryType.Cgo)
                    {
                        errorfieldName = "CH Due Date";
                    }
                    if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc || invoice.BillingCategoryId == (int)BillingCategoryType.Uatp)
                    {
                        errorfieldName = "Net Due Date";
                    }
                    break;
            }

            bool chDueDateValidation = ValidateChDueDate(invoice.BillingCategoryId, invoice.BillingCode, invoice.SettlementMethodId, invoice.ChDueDate, ref errorCode);
            if (!chDueDateValidation)
            {
                invoice.SetSmiXPhase1ValidationStatus(false);
                /* Add exception details */
                if (isCalledFromISWeb)
                {
                    throw new ISBusinessException(errorCode);
                }
                else
                {
                    var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                       exceptionDetailsList.Count() +
                                                                                       1,
                                                                                       fileSubmissionDate,
                                                                                       errorfieldName,
                                                                                       invoice.ChDueDate.HasValue
                                                                                           ? invoice.ChDueDate.ToString()
                                                                                           : "",
                                                                                       invoice, fileName,
                                                                                       ErrorLevels.ErrorLevelInvoice,
                                                                                       errorCode,
                                                                                       ErrorStatus.X,
                                                                                       invoice.BillingCode, 0,
                                                                                       batchSequenceNumber,
                                                                                       recordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                }
            }

            invoice.SetSmiXPhase1ValidationStatus(chDueDateValidation);

            #endregion

            #region FRS Section 2.8 New Validation #4: Pax Sampling Check

            if (invoice.BillingCategoryId == (int)BillingCategoryType.Pax && invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement && invoice.BillingCode != (int)BillingCode.NonSampling)
            {
                invoice.SetSmiXPhase1ValidationStatus(false);
                /* Validation failed - Error message - Invalid Settlement Method. Settlement Method X can be used only for Non-Sampling Invoices/Credit Notes */
                if (isCalledFromISWeb)
                {
                    throw new ISBusinessException(ErrorCodes.PaxSamplingCheckForSmiX);
                }
                else
                {
                    var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                       exceptionDetailsList.Count() +
                                                                                       1,
                                                                                       fileSubmissionDate,
                                                                                       "Settlement Method",
                                                                                       invoice.
                                                                                           SettlementMethodDisplayText,
                                                                                       invoice, fileName,
                                                                                       ErrorLevels.ErrorLevelInvoice,
                                                                                       ErrorCodes.
                                                                                           PaxSamplingCheckForSmiX,
                                                                                       ErrorStatus.X,
                                                                                       invoice.BillingCode, 0,
                                                                                       batchSequenceNumber,
                                                                                       recordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                }
            }

            #endregion

            #region FRS Section 2.8 New Validation #5 and #9: MISC/UATP Linking and SMI Check for Rejection Invoices

            if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement &&
                (invoice.BillingCategoryId == (int)BillingCategoryType.Misc || invoice.BillingCategoryId == (int)BillingCategoryType.Uatp))
            {
                if (((MiscUatpInvoice)invoice).InvoiceType == InvoiceType.RejectionInvoice)
                {
                    //CMP#624: below code commented as linked invoice get passed from caller.
                    //if (linkedInvoice == null)
                    //{
                    //    /* Get linked invoice i.e. - Original invoice/credit note using current invoice which is getting validation */
                    //    var miscUatpInvoiceManager = Ioc.Resolve<IMiscUatpInvoiceManager>(typeof(IMiscUatpInvoiceManager));
                    //    linkedInvoice = GetLinkedMUOriginalInvoice((MiscUatpInvoice)invoice);
                    //}

                    if (linkedInvoice != null)
                    {
                        /* Check type of linked invoice */
                        if (linkedInvoice.SettlementMethodId != (int)SMI.IchSpecialAgreement)
                        {
                            invoice.SetSmiXPhase1ValidationStatus(false);

                            /* FRS Section 2.8 New Validation #9 Error message  - 
                             * Rejected Invoice/Credit Note was billed using a Settlement Method (SMI) other than X. 
                             * Only Invoices/Credit Notes billed using SMI X can be rejected by an Invoice using SMI X */
                            if (isCalledFromISWeb)
                            {
                                throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiX);
                            }
                            else
                            {
                                var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                                   exceptionDetailsList.
                                                                                                       Count() +
                                                                                                   1,
                                                                                                   fileSubmissionDate,
                                                                                                   "Rejection Invoice Number",
                                                                                                   invoice.InvoiceNumber,
                                                                                                   invoice, fileName,
                                                                                                   ErrorLevels.
                                                                                                       ErrorLevelInvoice,
                                                                                                   MiscUatpErrorCodes.
                                                                                                       MuRejctionInvoiceSmiCheckForSmiX,
                                                                                                   ErrorStatus.X,
                                                                                                   invoice.BillingCode,
                                                                                                   0,
                                                                                                   batchSequenceNumber,
                                                                                                   recordSequenceWithinBatch);
                                exceptionDetailsList.Add(validationExceptionDetail);
                            }
                        }
                    }
                    else
                    {
                        invoice.SetSmiXPhase1ValidationStatus(false);

                        /* Original invoice/credit note is not found  
                         * FRS Section 2.8 New Validation #5 Error message  Original Invoice details are not found. 
                         * Successful Billing History linking is required for Rejection Invoices using Settlement Method X
                         */
                        if (isCalledFromISWeb)
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceLinkingCheckForSmiX);
                        }
                        else
                        {
                            var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                               exceptionDetailsList.
                                                                                                   Count() +
                                                                                               1,
                                                                                               fileSubmissionDate,
                                                                                               "Rejection Invoice Number",
                                                                                               invoice.InvoiceNumber,
                                                                                               invoice, fileName,
                                                                                               ErrorLevels.
                                                                                                   ErrorLevelInvoice,
                                                                                               MiscUatpErrorCodes.
                                                                                                   MuRejctionInvoiceLinkingCheckForSmiX,
                                                                                               ErrorStatus.X,
                                                                                               invoice.BillingCode, 0,
                                                                                               batchSequenceNumber,
                                                                                               recordSequenceWithinBatch);
                            exceptionDetailsList.Add(validationExceptionDetail);
                        }
                    }
                }
            }

            #endregion

            #region FRS Section 2.8 New Validation #6 and #10: MISC/UATP Linking and SMI Check for Correspondence Invoices

            if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement &&
                (invoice.BillingCategoryId == (int)BillingCategoryType.Misc || invoice.BillingCategoryId == (int)BillingCategoryType.Uatp))
            {
              if (((MiscUatpInvoice) invoice).InvoiceType == InvoiceType.CorrespondenceInvoice)
              {
                //CMP#624: below code commented as linked invoice get passed from caller.
                //if (linkedInvoice == null)
                //{
                //  /* Get linked invoice i.e. - Original invoice/credit note using current invoice which is getting validation */
                //  var miscUatpInvoiceManager = Ioc.Resolve<IMiscUatpInvoiceManager>(typeof(IMiscUatpInvoiceManager));
                //  linkedInvoice = GetLinkedMUOriginalInvoice((MiscUatpInvoice) invoice);
                //}


                if (linkedInvoice != null)
                {
                  /* Check type of linked invoice */
                  if (linkedInvoice.SettlementMethodId != (int) SMI.IchSpecialAgreement)
                  {
                    invoice.SetSmiXPhase1ValidationStatus(false);

                    /* FRS Section 2.8 New Validation #10 Error message  
                     * Rejection Invoice linked to this correspondence was billed using a Settlement Method (SMI) other than X. 
                     * Only Invoices billed using SMI X can be settled with a Correspondence Invoice using SMI X */
                    if (isCalledFromISWeb)
                    {
                      throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceSmiCheckForSmiX);
                    }
                    else
                    {
                      var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                          exceptionDetailsList.Count() + 1,
                                                                                          fileSubmissionDate,
                                                                                          "Correspondence Reference Number",
                                                                                          ((MiscUatpInvoice) invoice).CorrespondenceRefNo.HasValue
                                                                                            ? ((MiscUatpInvoice) invoice).CorrespondenceRefNo.Value.ToString()
                                                                                            : "",
                                                                                          invoice,
                                                                                          fileName,
                                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                                          MiscUatpErrorCodes.MuCorrespondenceInvoiceSmiCheckForSmiX,
                                                                                          ErrorStatus.X,
                                                                                          invoice.BillingCode,
                                                                                          0,
                                                                                          batchSequenceNumber,
                                                                                          recordSequenceWithinBatch);
                      exceptionDetailsList.Add(validationExceptionDetail);
                    }
                  }
                }
                else
                {
                  invoice.SetSmiXPhase1ValidationStatus(false);

                  /* Original invoice/credit note is not found  
                   * FRS Section 2.8 New Validation #6 Error message  Original Invoice details are not found. 
                   * Successful Billing History linking is required for Correspondence Invoices using Settlement Method X */
                  if (isCalledFromISWeb)
                  {
                    throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiX);
                  }
                  else
                  {
                    var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Correspondence Reference Number",
                                                                                        ((MiscUatpInvoice) invoice).CorrespondenceRefNo.HasValue
                                                                                          ? ((MiscUatpInvoice) invoice).CorrespondenceRefNo.Value.ToString()
                                                                                          : "",
                                                                                        invoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiX,
                                                                                        ErrorStatus.X,
                                                                                        invoice.BillingCode,
                                                                                        0,
                                                                                        batchSequenceNumber,
                                                                                        recordSequenceWithinBatch);
                    exceptionDetailsList.Add(validationExceptionDetail);
                  }
                }
              }
            }

          #endregion

            #region FRS Section 2.8 New Validation #11: Exchange Rate and Currency of Clearance Check for MISC/UATP Invoices/Credit Notes

            if (!isCalledFromISWeb && invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement &&
                (invoice.BillingCategoryId == (int)BillingCategoryType.Misc || invoice.BillingCategoryId == (int)BillingCategoryType.Uatp))
            {
                if (!((MiscUatpInvoice)invoice).IsExchangeRateProvidedInXmlFile)
                {
                    invoice.SetSmiXPhase1ValidationStatus(false);
                    /* FRS Section 2.8 New Validation #11 Error message when ExchangeRate is not provided -> 
                     * ExchangeRate is mandatory for Invoices/Credit Notes billed using Settlement Method X */
                    if (isCalledFromISWeb)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.MuExchangeRateCheckForSmiX);
                    }
                    else
                    {
                        var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                           exceptionDetailsList.
                                                                                               Count() +
                                                                                           1,
                                                                                           fileSubmissionDate,
                                                                                           "Exchange Rate",
                                                                                           "",
                                                                                           invoice, fileName,
                                                                                           ErrorLevels.
                                                                                               ErrorLevelInvoice,
                                                                                           MiscUatpErrorCodes.
                                                                                               MuExchangeRateCheckForSmiX,
                                                                                           ErrorStatus.X,
                                                                                           invoice.BillingCode,
                                                                                           0,
                                                                                           batchSequenceNumber,
                                                                                           recordSequenceWithinBatch);
                        exceptionDetailsList.Add(validationExceptionDetail);
                    }
                }

                if (!((MiscUatpInvoice)invoice).IsClearanceCurrencyInXmlFile)
                {
                    invoice.SetSmiXPhase1ValidationStatus(false);

                    /* FRS Section 2.8 New Validation #11 Error message when ClearanceCurrencyCode is not provided  -> 
                     * ClearanceCurrencyCode is mandatory for Invoices/Credit Notes billed using Settlement Method X */
                    if (isCalledFromISWeb)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.MuClearanceCurrencyCheckForSmiX);
                    }
                    else
                    {
                        var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                           exceptionDetailsList.
                                                                                               Count() +
                                                                                           1,
                                                                                           fileSubmissionDate,
                                                                                           "Clearance Currency Code",
                                                                                           "",
                                                                                           invoice, fileName,
                                                                                           ErrorLevels.
                                                                                               ErrorLevelInvoice,
                                                                                           MiscUatpErrorCodes.
                                                                                               MuClearanceCurrencyCheckForSmiX,
                                                                                           ErrorStatus.X,
                                                                                           invoice.BillingCode,
                                                                                           0,
                                                                                           batchSequenceNumber,
                                                                                           recordSequenceWithinBatch);
                        exceptionDetailsList.Add(validationExceptionDetail);
                    }
                }
            }

            #endregion
        }
        catch (Exception exception)
        {
            //if (exception is ISBusinessException)
            //    throw;
            // anyways throw as - need to re-queue member/for pass on as error message on web page
            invoice.SetSmiXPhase1ValidationStatus(false);
            Logger.Error("Exception in ValidationBeforeSmiXWebServiceCall(), Details -> ", exception);
            throw;
        }
        return invoice.GetSmiXPhase1ValidationStatus();
    }

    /// <summary>
    /// Validates the ch agreement indicator.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceSmi">The invoice smi.</param>
    /// <param name="chAgreementIndicator">The ch agreement indicator.</param>
    /// <param name="errorCode">The error code.</param>
    /// <returns></returns>
    public bool ValidateChAgreementIndicator(int billingCategoryId, int billingCode, int invoiceSmi, string chAgreementIndicator, ref string errorCode)
    {
        try
        {
            /* Refer: FRS Section 2.8, New Validation #2 */
            if (invoiceSmi == (int)SMI.IchSpecialAgreement && string.IsNullOrWhiteSpace(chAgreementIndicator))
            {
                /* Validation failed - Error message - CH Agreement Indicator is mandatory when Settlement Method is X */
                errorCode = ErrorCodes.ChAgreementIndicatorCheckForSmiX;
                return false;
            }

            /* Refer: FRS Section 2.11, New Validation #2 */
            if (billingCategoryId == (int)BillingCategoryType.Pax && billingCode != (int)BillingCode.NonSampling && !string.IsNullOrWhiteSpace(chAgreementIndicator))
            {
                /* Validation failed - Error message - CH Agreement Indicator cannot be provided for Passenger Sampling billings */
                errorCode = ErrorCodes.ChAgreementIndicatorCheckForSampling;
                return false;
            }

            /* Refer: FRS Section 2.11, New Validation #1 */
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            /* For Other than X SMI chAgreementIndicator should not be allowed.*/
            if (invoiceSmi != (int)SMI.IchSpecialAgreement && !string.IsNullOrWhiteSpace(chAgreementIndicator))
            {
                /* Validation failed - Error message - CH Agreement Indicator cannot be provided for Bilateral Settlement Methods */
                errorCode = ErrorCodes.ChAgreementIndicatorCheckForSmiOtherThanX;
                return false;
            }

            
        }
        catch (Exception exception)
        {
            Logger.Error("Exception in ValidateChAgreementIndicator(), Details -> ", exception);
            throw;
        }
        return true;
    }

    /// <summary>
    /// Validates the ch due date.
    /// </summary>
    /// <param name="billingCategoryId">The billing category id.</param>
    /// <param name="billingCode">The billing code.</param>
    /// <param name="invoiceSmi">The invoice smi.</param>
    /// <param name="chDueDate">The ch due date.</param>
    /// <param name="errorCode">The error code.</param>
    /// <returns></returns>
    public bool ValidateChDueDate(int billingCategoryId, int billingCode, int invoiceSmi, DateTime? chDueDate, ref string errorCode)
    {
        bool isValid = true;
        try
        {
            if (billingCategoryId == (int)BillingCategoryType.Pax && billingCode != (int)BillingCode.NonSampling && chDueDate.HasValue)
            {
                /* Refer: FRS Section 2.11, New Validation #4 */

                /* Validation failed - Error message - Due Date cannot be provided for Passenger Sampling billings */
                errorCode = ErrorCodes.ChDueDateCheckForSampling;
                isValid = false;
            }
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if ((billingCategoryId == (int)BillingCategoryType.Pax || billingCategoryId == (int)BillingCategoryType.Cgo)
                && invoiceSmi != (int)SMI.IchSpecialAgreement && chDueDate.HasValue)
            {
                /* Refer: FRS Section 2.11, New Validation #3 */

                /* Validation failed - Error message - Due Date cannot be provided for Bilateral Settlement Methods */
              errorCode = ErrorCodes.ChDueDateCheckForSmiOtherThanX;
                isValid = false;
            }
        }
        catch (Exception exception)
        {
            isValid = false;
            Logger.Error("Exception in ValidateChAgreementIndicator(), Details -> ", exception);
            throw;
        }
        return isValid;
    }

    /// <summary>
    /// CMP#624 : 2.8
    /// Validates the smi afterlinking.
    /// </summary>
    /// <param name="invoiceSmi">The invoice smi.</param>
    /// <param name="yourInvoiceSmi">Your invoice smi.</param>
    /// <returns></returns>
    public bool ValidateSmiAfterLinking(int invoiceSmi, int yourInvoiceSmi)
    {
      bool isSmiValid = true;
      // RM/BM Invoice SMI is X and linked your invoice SMi is should be X 
      if (invoiceSmi == (int)SMI.IchSpecialAgreement && yourInvoiceSmi != (int)SMI.IchSpecialAgreement)
      {
        isSmiValid = false;
      }
      // RM/BM Invoice SMI is NON X and linked your invoice SMi is should be NON X 
      else if (invoiceSmi != (int)SMI.IchSpecialAgreement && yourInvoiceSmi == (int)SMI.IchSpecialAgreement)
      {
        isSmiValid = false;
      }

      return isSmiValid;
    }

    /// <summary>
    /// Method to call SMI X web service and process the web service response.
    /// </summary>
    /// <param name="baseInvoice"></param>
    /// <param name="invoiceTypeId"></param>
    /// <returns></returns>
    public bool CallSmiXIchWebServiceAndHandleResponse(InvoiceBase invoice, IList<IsValidationExceptionDetail> exceptionDetailsList, int invoiceTypeId, DateTime? fileDate, string fileName, bool isCalledFromISWeb, int batchSequenceNumber = 99999, int recordSequenceWithinBatch = 99999, MiscUatpInvoice linkedInvoice = null)
    {
        try
        {
            /* Call ICH Service only when invoice SMI is X */
            if (invoice.SettlementMethodId != (int)SMI.IchSpecialAgreement || invoice.BillingCode != (int)BillingCode.NonSampling)
            {
                return true;
            }


            /* CMP #624: ICH Rewrite-New SMI X 
             * Description: As per ICH Web Service Response Message specifications 
             * Refer FRS Section 3.3 Table 9. */
            //var validationResultPass = "P"; // P when Invoice/Credit Note passes validation in ICH
            //var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
            //var validationResultError = "E"; // E when ICH receives a Bad Request from SIS
            var fileSubmissionDate = fileDate.HasValue ? fileDate.Value : new DateTime();
            if(invoice.ListingCurrency==null)
            {
              invoice.ListingCurrency = CurrencyManager.GetCurrencyDetails(Convert.ToInt32(invoice.ListingCurrencyId));
            }
            if (invoice.InvoiceBillingCurrency == null)
            {
              invoice.InvoiceBillingCurrency = CurrencyManager.GetCurrencyDetails(Convert.ToInt32(invoice.BillingCurrencyId));
            }
            
            invoice.ChValidationResult = string.Empty;
            var ichWebService = Ioc.Resolve<IICHSmiXInterfaceHandler>(typeof(IICHSmiXInterfaceHandler));
            var ichWebServiceResponse = ichWebService.SmiXwebServiceCall(invoice, invoiceTypeId, linkedInvoice);

            // P when Invoice/Credit Note passes validation in ICH
            if (ichWebServiceResponse.ValidationResult.Equals("P", StringComparison.CurrentCultureIgnoreCase))
            {
                invoice.ChValidationResult = ichWebServiceResponse.ValidationResult;
                invoice.CurrencyRateIndicator = ichWebServiceResponse.CurrencyRateIndicator;
                return true;
            }
            // F when Invoice/Credit Note fails validation in ICH
            else if (ichWebServiceResponse.ValidationResult.Equals("F", StringComparison.CurrentCultureIgnoreCase))
            {
                invoice.ChValidationResult = ichWebServiceResponse.ValidationResult;

                if (isCalledFromISWeb)
                {
                    var ichWebResponseError = ichWebServiceResponse.Errors.First();
                    var errorMessage = string.Format(
                    "This Invoice/Credit Note failed validations in ICH. See error response from ICH below.  ¥ {0} : {1}",
                    ichWebResponseError.ErrorCode, ichWebResponseError.ErrorDescription);
                    throw new ISBusinessException(ichWebResponseError.ErrorCode, errorMessage);
                }
                else
                {
                    foreach (var ichWebResponseError in ichWebServiceResponse.Errors)
                    {
                        var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                            exceptionDetailsList.Count() +
                                                                                            1,
                                                                                            fileSubmissionDate,
                                                                                            "Settlement Method",
                                                                                            invoice.
                                                                                                SettlementMethodDisplayText,
                                                                                            invoice, fileName,
                                                                                            ErrorLevels.
                                                                                                ErrorLevelInvoice,
                                                                                            ichWebResponseError.
                                                                                                ErrorCode,
                                                                                            ErrorStatus.X,
                                                                                            invoice.BillingCode, 0,
                                                                                            batchSequenceNumber,
                                                                                            recordSequenceWithinBatch,
                                                                                            null, false, null, null,
                                                                                            true,
                                                                                            ichWebResponseError.
                                                                                                ErrorDescription);
                        exceptionDetailsList.Add(validationExceptionDetail);

                    }
                }

                return false;
            }
            // E when ICH receives a Bad Request from SIS
            else if (ichWebServiceResponse.ValidationResult.Equals("E", StringComparison.CurrentCultureIgnoreCase))
            {
              invoice.ChValidationResult = ichWebServiceResponse.ValidationResult;
              
              if (isCalledFromISWeb)
              {
                //var ichWebResponseError = ichWebServiceResponse.Errors.First();
                //var errorMessage = string.Format(
                //"This Invoice/Credit Note failed validations in ICH. See error response from ICH below.  ¥ {0} : {1}",
                //ichWebResponseError.ErrorCode, ichWebResponseError.ErrorDescription);
                throw new ISBusinessException(ErrorCodes.IchWebServicefailureMessage);
              }
              else
              {
                var validationExceptionDetail = CreateSmiXValidationExceptionDetail(invoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    "Settlement Method",
                                                                                    invoice.SettlementMethodDisplayText,
                                                                                    invoice,
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    ErrorCodes.IchWebServicefailureMessage,
                                                                                    ErrorStatus.X,
                                                                                    invoice.BillingCode,
                                                                                    0,
                                                                                    batchSequenceNumber,
                                                                                    recordSequenceWithinBatch);
                exceptionDetailsList.Add(validationExceptionDetail);
              }

              return false;
            }

            /* All processing is successfully completed */
            return true;
        }
        catch (ISBusinessException exception)
        {
            //if (exception is ISBusinessException)
            //    throw;
            // anyways throw as - need to re-queue member/for pass on as error message on web page
            Logger.Error("Business Exception in CallSmiXIchWebServiceAndHandleResponse(), Details -> ", exception);
            throw;
        }
        catch (Exception exception)
        {
            //if (exception is ISBusinessException)
            //    throw;
            // anyways throw as - need to re-queue member/for pass on as error message on web page
            Logger.Error("Exception in CallSmiXIchWebServiceAndHandleResponse(), Details -> ", exception);
            throw;
        }
    }

    /// <summary>
    /// Method specifically created to serve CMP #624: ICH Rewrite-New SMI X purpose.
    /// By default -> bool isExceptionCodeNotInResource = false, string customeErrorDescription = null
    /// When bool isExceptionCodeNotInResource = true, string customeErrorDescription is expected to be not null having error description text.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="serialNumber"></param>
    /// <param name="fileSubmissionDate"></param>
    /// <param name="fieldName"></param>
    /// <param name="fieldValue"></param>
    /// <param name="invoice"></param>
    /// <param name="fileName"></param>
    /// <param name="errorLevel"></param>
    /// <param name="exceptionCode"></param>
    /// <param name="errorStatus"></param>
    /// <param name="billingCode"></param>
    /// <param name="sourceCode"></param>
    /// <param name="batchNo"></param>
    /// <param name="sequenceNo"></param>
    /// <param name="documentNumber"></param>
    /// <param name="isBatchUpdateAllowed"></param>
    /// <param name="linkedDocumentNumber"></param>
    /// <param name="calculatedValue"></param>
    /// <param name="isExceptionCodeNotInResource"></param>
    /// <param name="customeErrorDescription"></param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail CreateSmiXValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, int billingCode, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null, bool isExceptionCodeNotInResource = false, string customeErrorDescription = null)
    {

        string submissionFormat;
        if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
        {
            submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //"IS-XML";// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
        }
        else
        {
            submissionFormat = ((int)SubmissionMethod.IsIdec).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
        }

        var errorDescription = string.Empty;
        if (isExceptionCodeNotInResource)
        {
            errorDescription = customeErrorDescription;
        }
        else
        {
            if (!string.IsNullOrEmpty(exceptionCode))
                errorDescription = Messages.ResourceManager.GetString(exceptionCode);

            if (!string.IsNullOrWhiteSpace(calculatedValue))
            {
                errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
            }    
        }

        var validationExceptionDetail = new IsValidationExceptionDetail
        {
            SerialNo = serialNumber,
            BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
            BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
            ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
            CategoryOfBilling = invoice.BillingCategoryId.ToString(),
            SubmissionFormat = submissionFormat,
            ErrorStatus = ((int)errorStatus).ToString(),
            ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
            PeriodNumber = invoice.BillingPeriod,
            BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
            LinkedDocNo = linkedDocumentNumber,

            ErrorDescription = errorDescription,
            FieldName = fieldName,
            FieldValue = fieldValue,
            BatchUpdateAllowed = isBatchUpdateAllowed,

            InvoiceNumber = invoice.InvoiceNumber,
            DocumentNo = documentNumber,
            SourceCodeId = sourceCode == 0 ? string.Empty : sourceCode.ToString(),
            ErrorLevel = errorLevel,
            ExceptionCode = exceptionCode,
            FileName = Path.GetFileName(fileName),
            LineItemOrBatchNo = batchNo,
            LineItemDetailOrSequenceNo = sequenceNo,
            Id = Guid.NewGuid(),
            PkReferenceId = id
        };

        return validationExceptionDetail;
    }

    /// <summary>
    /// Get Linked Original invoice
    /// CMP #624: ICH Rewrite-New SMI X  
    /// </summary>
    /// <param name="miscUatpInvoice">The misc UATP invoice.</param>
    /// <returns>
    /// Original Invoice
    /// </returns>
    public MiscUatpInvoice GetLinkedMUOriginalInvoice(MiscUatpInvoice miscUatpInvoice, out MiscUatpInvoice linkedRm1Invoice, out  MiscUatpInvoice linkedRm2Invoice, out MiscCorrespondence linkedCorrespondence)
    {
        MiscUatpInvoice rejectedInvoiceStage1 = null;
        MiscUatpInvoice rejectedInvoiceStage2 = null;
        MiscUatpInvoice rejectedInvoice = null;
        MiscCorrespondence miscCorrespondence = null;
        linkedRm1Invoice = null;
        linkedRm2Invoice = null;
        linkedCorrespondence = null;

        switch (miscUatpInvoice.InvoiceType)
        {
            case InvoiceType.Invoice:
            case InvoiceType.CreditNote:
                /* Input itself is original invoice */
                return null;
            case InvoiceType.RejectionInvoice:
                switch (miscUatpInvoice.RejectionStage)
                {
                    case 2:
                        /* Input is Stage 2 Rejection */
                        rejectedInvoiceStage2 = miscUatpInvoice;
                        /* Get R1 for R2 */
                        rejectedInvoiceStage1 = GetMUInvoicePriviousTransanction(rejectedInvoiceStage2);
                        linkedRm1Invoice = rejectedInvoiceStage1;
                        /* Get Prime for R1 */
                        return GetMUInvoicePriviousTransanction(rejectedInvoiceStage1);
                    case 1:
                        /* Input is Stage 1 Rejection */
                        rejectedInvoiceStage1 = miscUatpInvoice;
                        /* Get Prime from R1 */
                        return GetMUInvoicePriviousTransanction(rejectedInvoiceStage1);
                }
                break;
            case InvoiceType.CorrespondenceInvoice:
                /* Input is correspondence invoice */

                /* getting First Stage correspondence */
                var miscCorrespondenceRepository = Ioc.Resolve<IMiscCorrespondenceRepository>(typeof(IMiscCorrespondenceRepository));
                //miscCorrespondence =
                //    miscCorrespondenceRepository.Get(miscUatpInvoice.CorrespondenceRefNo).
                //        OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            miscCorrespondence =
              miscCorrespondenceRepository.GetCorrespondenceWithInvoice(
                miscCorrs =>
                miscCorrs.CorrespondenceNumber == miscUatpInvoice.CorrespondenceRefNo &&
                (((miscCorrs.CorrespondenceStatusId == (int) CorrespondenceStatus.Open || miscCorrs.CorrespondenceStatusId == (int) CorrespondenceStatus.Expired)
                 /*&& miscCorrs.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Responded*/) || miscCorrs.CorrespondenceStatusId == (int) CorrespondenceStatus.Closed)).OrderByDescending(
                   miscCorr2 => miscCorr2.CorrespondenceStage).FirstOrDefault();

                linkedCorrespondence = miscCorrespondence;

                if (miscCorrespondence != null)
                {
                    /* getting Last stage rejection invoice */
                  if (miscCorrespondence.Invoice != null)
                  {
                    rejectedInvoice = miscCorrespondence.Invoice;
                  }
                  else
                  {
                    rejectedInvoice = miscUatpInvoiceRepository.Single(miscCorrespondence.InvoiceId,
                                                                       billingCategoryId:
                                                                           (int)miscUatpInvoice.BillingCategory);
                  }
                    
                }

                if (rejectedInvoice != null)
                {
                    switch (rejectedInvoice.RejectionStage)
                    {
                        case 2:
                            /* Last stage rejection is of Stage 2 */
                            rejectedInvoiceStage2 = rejectedInvoice;
                            linkedRm2Invoice = rejectedInvoiceStage2;
                            /* Get R1 for R2 */
                            rejectedInvoiceStage1 = GetMUInvoicePriviousTransanction(rejectedInvoiceStage2);
                            linkedRm1Invoice = rejectedInvoiceStage1;
                            /* Get Prime from R1 */
                            return GetMUInvoicePriviousTransanction(rejectedInvoiceStage1);
                        case 1:
                            /* Last stage rejection is of Stage 1 */
                            rejectedInvoiceStage1 = rejectedInvoice;
                            linkedRm1Invoice = rejectedInvoiceStage1;
                            /* Get Prime from R1 */
                            return GetMUInvoicePriviousTransanction(rejectedInvoiceStage1);
                    }
                }

                break;
        }

        /* Default return null - indicating that original invoice is not found */
        return null;
    }

    /// <summary>
    /// Get Privious Transaction (Used for rejection Invoice)
    /// </summary>
    /// <param name="currentTransaction"></param>
    /// <returns>Rejection Invoice</returns>
    public MiscUatpInvoice GetMUInvoicePriviousTransanction(MiscUatpInvoice currentTransaction)
    {
        if (currentTransaction != null)
        {
          var previousInvoice = miscUatpInvoiceRepository.GetMUlinkedInvoiceHeader(invoiceNumber: currentTransaction.RejectedInvoiceNumber,
                                                                                   billingMemberId: currentTransaction.BilledMemberId,
                                                                                   billedMemberId: currentTransaction.BillingMemberId,
                                                                                   billingCategoryId: currentTransaction.BillingCategoryId,
                                                                                   invoiceStatusId: (int) InvoiceStatusType.Presented,
                                                                                   billingPeriod: currentTransaction.SettlementPeriod,
                                                                                   billingMonth: currentTransaction.SettlementMonth,
                                                                                   billingYear: currentTransaction.SettlementYear);
            return previousInvoice;
        }

        return null;
    }

    #endregion

    /// <summary>
    /// CMP #596: Length of Member Accounting Code to be Increased to 12.
    /// Desc: This is a New auto-complete logic #MW1, Validation logic #MW2
    /// For more refer FRS Section 3.4 Point 20, 21.
    /// </summary>
    /// <param name="memberCodeNumeric">Member Numeric Code</param>
    /// <returns></returns>
    public bool IsTypeBMember(string memberCodeNumeric)
    {
        try
        {
            /* MemberNumericCode is expected to be at index 1 */
            if (!string.IsNullOrEmpty(memberCodeNumeric))
            {
                /* As per CMP# 596 FRS document, the term ‘Type B Members’ means  - 
                 * new SIS Members having an Accounting Code with one of the following attributes:
                 * a.The length of the code is 3, but alpha characters appear in the second and/or third position (the first position may be alpha or numeric)
                 * b.The length of the code is 4, but alpha character(s) appear in any position (i.e. it is not purely 4 numeric)
                 * c.The length of the code ranges from 5 to 12
                */
                if (memberCodeNumeric.Length >= 5 && memberCodeNumeric.Length <= 12)
                {
                    /* its a Type-B member as per definition point c. */
                    return true;
                }

                if (memberCodeNumeric.Length == 4)
                {
                    /* Check if its purely numeric */
                    var regExPurelyNumeric = new Regex("^[0-9]{0,4}$");
                    if (!regExPurelyNumeric.IsMatch(memberCodeNumeric))
                    {
                        /* Its not purely numeric code of length 4, so its a Type-B member as per definition point b. */
                        return true;
                    }
                }

                if (memberCodeNumeric.Length == 3)
                {
                    /* There is no restriction on 1st place character, it can be alphanumeric*/
                    var regExSecondPlaceAlpha = new Regex("^[a-zA-Z0-9][a-zA-Z][a-zA-Z0-9]$");
                    if (regExSecondPlaceAlpha.IsMatch(memberCodeNumeric))
                    {
                        /* 2nd place character is alpha, so its a Type-B member as per definition point a.*/
                        return true;
                    }
                    var regExThirdPlaceAlpha = new Regex("^[a-zA-Z0-9][0-9][a-zA-Z]$");
                    if (regExThirdPlaceAlpha.IsMatch(memberCodeNumeric))
                    {
                        /* 3rd place character is alpha, so its a Type-B member as per definition point a.*/
                        return true;
                    }
                }
            } //end of if i.e. - Member code numeric is obtained for further investigation.

        }
        catch (Exception exception)
        {
            Logger.Info(string.Format("Error while checking, if {0} is Type-B member or not.", memberCodeNumeric));
            Logger.Error(exception);
        }

        /* By default member will not be treated as a Type-B member */
        return false;
    }

    /// <summary>
    /// CMP#678: Time Limit Validation on Last Stage MISC Rejections
    /// </summary>
    /// <param name="pkId">unique Id</param>
    /// <param name="serialNumber">serial number</param>
    /// <param name="fileSubmissionDate">file submission date</param>
    /// <param name="miscUatpInvoice">rejection invoice</param>
    /// <param name="fileName">name of input file</param>
    /// <param name="fieldName">field name</param>
    /// <param name="fieldValue">field value</param>
    /// <param name="errorLevel">error level</param>
    /// <param name="exceptionCode">error code</param>
    /// <param name="errorDescription">error des</param>
    /// <returns></returns>
    protected static IsValidationExceptionDetail IsValidationExceptionMiscLastStageRmDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, MiscUatpInvoice miscUatpInvoice,string fileName, string fieldName, string fieldValue, string errorLevel, string exceptionCode, string errorDescription)
    {
        var validationExceptionDetail = new IsValidationExceptionDetail
        {
            SerialNo = serialNumber,
            BillingEntityCode = miscUatpInvoice.BillingMember != null ? miscUatpInvoice.BillingMember.MemberCodeNumeric : string.Empty,
            BilledEntityCode = miscUatpInvoice.BilledMember != null ? miscUatpInvoice.BilledMember.MemberCodeNumeric : string.Empty,
            ChargeCategoryOrBillingCode = miscUatpInvoice.ChargeCategoryDisplayName,
            CategoryOfBilling = miscUatpInvoice.BillingCategoryId.ToString(),
            ErrorStatus = ((int)ErrorStatus.X).ToString(),
            ClearanceMonth = miscUatpInvoice.BillingYear + miscUatpInvoice.BillingMonth.ToString().PadLeft(2, '0'),
            PeriodNumber = miscUatpInvoice.BillingPeriod,
            BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),

            ErrorDescription = errorDescription,
            FieldName = fieldName,
            FieldValue = fieldValue,
            FileName = Path.GetFileName(fileName),
            SubmissionFormat = ((int)SubmissionMethod.IsXml).ToString(),
            InvoiceNumber = miscUatpInvoice.InvoiceNumber,
            ErrorLevel = errorLevel,
            ExceptionCode = exceptionCode,
            LineItemDetailOrSequenceNo = 0,
            LineItemOrBatchNo = 0,
            Id = Guid.NewGuid(),
            PkReferenceId = pkId,
            DocumentNo = null,
            LinkedDocNo = null
        };

        return validationExceptionDetail;
    }

    #region CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text

    protected bool ValidateReasonTextMinLength(RejectionMemo paxRejectionMemoRecord = null, CargoRejectionMemo cargoRejectionMemoRecord = null, string fileName = null, InvoiceBase invoice = null, IList<IsValidationExceptionDetail> exceptionDetailsList = null, DateTime? fileSubmissionDate = null)
    {
        /* Default return value */
        bool isValid = true;

        /* Read Reason Text Minimum Length Parameter Vaalue From SIS Config File */
        var minReasonRemarkCharLength = 0; //Default Value.
        try
        {
            var strMinReasonRemarkCharLength = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("RMReasonTextMinLength");
            int.TryParse(strMinReasonRemarkCharLength, out minReasonRemarkCharLength);
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            /* Eat this exception - As a result min Reason Remark Length will be taken as 0 for further validations. */
        }

        Logger.InfoFormat("Reason Text Min Length Parameter Value read from SIS Config is: {0}", minReasonRemarkCharLength);

        /* For Pax Billing Category */
        if (paxRejectionMemoRecord != null)
        {
            /* Minimum Length Validation */
            isValid = paxRejectionMemoRecord.ReasonRemarks != null
                          ? // Reason Remark Text Provided and So Trim it to apply Length Check. Result of check applied available in isValid Variable.
                          (paxRejectionMemoRecord.ReasonRemarks.Trim().Length >= minReasonRemarkCharLength)
                          : // Reason Remark Text Not Provided, So Checking if it is a mandetory Field. Result of check applied available in isValid Variable.
                          (minReasonRemarkCharLength == 0);

            if (!isValid)
            {
                /* If file name is not provided as input, then method is called from is web */
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    /* Called From Is-Web */
                    throw new ISBusinessException(ErrorCodes.MinReasonRemarkCharLength);
                }
                else
                {
                    /* Called From File Validation */
                    if (exceptionDetailsList != null && fileSubmissionDate != null)
                    {
                        var validationExceptionDetail =
                            GetReasonTextMinLengthValidationExceptionDetail(exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate.Value, invoice, fileName,
                                                                            paxRejectionMemoRecord:
                                                                                paxRejectionMemoRecord);

                        exceptionDetailsList.Add(validationExceptionDetail);
                    }
                    //else
                    //{
                    //    For logical completion only - 
                    //    If file name is provided as input, then Exception Details List and File Submission Date is expected as well. 
                    //    Otherwize it is Not Possible to build error object, this is unexpected.
                    //}
                }
            }
        }

        /* For Cargo Billing Category */
        if (cargoRejectionMemoRecord != null)
        {
            /* Minimum Length Validation */
            isValid = cargoRejectionMemoRecord.ReasonRemarks != null
                          ? // Reason Remark Text Provided and So Trim it to apply Length Check. Result of check applied available in isValid Variable.
                          (cargoRejectionMemoRecord.ReasonRemarks.Trim().Length >= minReasonRemarkCharLength)
                          : // Reason Remark Text Not Provided, So Checking if it is a mandetory Field. Result of check applied available in isValid Variable.
                          (minReasonRemarkCharLength == 0);

            if(!isValid)
            {
                /* If file name is not provided as input, then method is called from is web */
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    /* Called From Is-Web */
                    throw new ISBusinessException(ErrorCodes.MinReasonRemarkCharLength);
                }
                else
                {
                    /* Called From File Validation */
                    if (exceptionDetailsList != null && fileSubmissionDate != null)
                    {
                        var validationExceptionDetail =
                            GetReasonTextMinLengthValidationExceptionDetail(exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate.Value, invoice, fileName,
                                                                            cargoRejectionMemoRecord:
                                                                                cargoRejectionMemoRecord);

                        exceptionDetailsList.Add(validationExceptionDetail);
                    }
                    //else
                    //{
                    //    For logical completion only - 
                    //    If file name is provided as input, then Exception Details List and File Submission Date is expected as well. 
                    //    Otherwize it is Not Possible to build error object, this is unexpected.
                    //}
                }
            }
        }

        return isValid; 
    }

    //Generate Error Description - for CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text
    private IsValidationExceptionDetail GetReasonTextMinLengthValidationExceptionDetail(int serialNumber, DateTime fileSubmissionDate, InvoiceBase invoice, string fileName, CargoRejectionMemo cargoRejectionMemoRecord = null, RejectionMemo paxRejectionMemoRecord = null)
    {
        /* Constants for building Validation Exception Deails Object */
        const string fieldName = "Reason Remarks/Description";
        const string fieldValue = "Please refer to reason remarks/description provided.";
        const string errorLevel = ErrorLevels.ErrorLevelRejectionMemo;
        const string errorCode = ErrorCodes.MinReasonRemarkCharLength;
        const ErrorStatus errorStatus = ErrorStatus.X;
        IsValidationExceptionDetail validationExceptionDetail = null;

        /* For Pax */
        if (paxRejectionMemoRecord != null)
        {
            validationExceptionDetail = CreateValidationExceptionDetail(paxRejectionMemoRecord.Id.Value(), serialNumber, fileSubmissionDate,
                                                                              fieldName, fieldValue, (PaxInvoice)invoice, fileName, errorLevel, errorCode,
                                                                              errorStatus, paxRejectionMemoRecord);
        }

        /* For Cargo */
        if (cargoRejectionMemoRecord != null)
        {
            validationExceptionDetail = CreateCgoRMValidationExceptionDetail(cargoRejectionMemoRecord.Id.Value(), serialNumber, fileSubmissionDate,
                                                                                 fieldName, fieldValue, invoice, fileName, errorLevel, errorCode,
                                                                                 errorStatus, cargoRejectionMemoRecord.BillingCode, cargoRejectionMemoRecord);
        }

        return validationExceptionDetail;

    }

    #endregion

  }
}


