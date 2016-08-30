using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Common
{
  public class IsValidationExceptionDetail : ValidationExceptionDetailBase
  {
    public IsValidationExceptionDetail()
    {
    }

    public IsValidationExceptionDetail(IsValidationExceptionDetail isValidationExceptionDetail)
    {
      SerialNo = isValidationExceptionDetail.SerialNo;
      BilledEntityCode = isValidationExceptionDetail.BilledEntityCode;
      BillingEntityCode = isValidationExceptionDetail.BillingEntityCode;
      ClearanceMonth = isValidationExceptionDetail.ClearanceMonth;
      PeriodNumber = isValidationExceptionDetail.PeriodNumber;
      FieldName = isValidationExceptionDetail.FieldName;
      BillingFileSubmissionDate = isValidationExceptionDetail.BillingFileSubmissionDate;
      SubmissionFormat = isValidationExceptionDetail.SubmissionFormat;
      InvoiceNumber = isValidationExceptionDetail.InvoiceNumber;
      CategoryOfBilling = isValidationExceptionDetail.CategoryOfBilling;
      FileName = isValidationExceptionDetail.FileName;
      LineItemOrBatchNo = isValidationExceptionDetail.LineItemOrBatchNo;
      LineItemDetailOrSequenceNo = isValidationExceptionDetail.LineItemDetailOrSequenceNo;
     
      LinkedDocNo = isValidationExceptionDetail.LinkedDocNo;
      ChargeCategoryOrBillingCode = isValidationExceptionDetail.ChargeCategoryOrBillingCode;
      SourceCodeId = isValidationExceptionDetail.SourceCodeId;
      ErrorStatus = isValidationExceptionDetail.ErrorStatus;
      ErrorDescription = isValidationExceptionDetail.ErrorDescription;
      ExceptionCode = isValidationExceptionDetail.ExceptionCode;
      ErrorLevel = isValidationExceptionDetail.ErrorLevel;
      FieldName = isValidationExceptionDetail.FieldName;
      FieldValue = isValidationExceptionDetail.FieldValue;

      NewFieldValue = isValidationExceptionDetail.NewFieldValue;
      BatchNo = isValidationExceptionDetail.LineItemOrBatchNo;
      SequenceNo = isValidationExceptionDetail.LineItemDetailOrSequenceNo;
      Id = isValidationExceptionDetail.Id;
      ValidationExceptionId = isValidationExceptionDetail.ValidationExceptionId;
      OnlineCorrectionStaus = isValidationExceptionDetail.OnlineCorrectionStaus;
      LastUpdatedOn = isValidationExceptionDetail.LastUpdatedOn;
      LastUpdatedBy = isValidationExceptionDetail.LastUpdatedBy;
      IssuingAirline = isValidationExceptionDetail.IssuingAirline;
      PkReferenceId = isValidationExceptionDetail.PkReferenceId;
      YourInvoiceNo = isValidationExceptionDetail.YourInvoiceNo;
      YourInvoiceBillingMonth = isValidationExceptionDetail.YourInvoiceBillingMonth;
      YourInvoiceBillingYear = isValidationExceptionDetail.YourInvoiceBillingYear;
      YourInvoiceBillingPeriod = isValidationExceptionDetail.YourInvoiceBillingPeriod;
      YourRejectionMemoNo = isValidationExceptionDetail.YourRejectionMemoNo;
      YourBmCmNo = isValidationExceptionDetail.YourBmCmNo;
      YourBmCmIndicator = isValidationExceptionDetail.YourBmCmIndicator;
      TransactionId = isValidationExceptionDetail.TransactionId;
      CorrespondenceRefNo = isValidationExceptionDetail.CorrespondenceRefNo;
      ReasonCode = isValidationExceptionDetail.ReasonCode;
      RejectionStage = isValidationExceptionDetail.RejectionStage;
      ExceptionCodeId = isValidationExceptionDetail.ExceptionCodeId;
      BatchUpdateAllowed = isValidationExceptionDetail.BatchUpdateAllowed;
      ErrorCorrectionLevel = isValidationExceptionDetail.ErrorCorrectionLevel;
      FimBmCmNumber = isValidationExceptionDetail.FimBmCmNumber;
      FimCouponNumber = isValidationExceptionDetail.FimCouponNumber;
      ProvInvoiceNo = isValidationExceptionDetail.ProvInvoiceNo;
      CouponNo = isValidationExceptionDetail.CouponNo;
      FimBmCmIndicator = isValidationExceptionDetail.FimBmCmIndicator;

      if(!string.IsNullOrEmpty(isValidationExceptionDetail.DocumentNo) && isValidationExceptionDetail.DocumentNo.Split('-').Count() == 3)
      {
        DocumentNo = isValidationExceptionDetail.DocumentNo.Split('-')[1];
      }
      else
      {
        DocumentNo = isValidationExceptionDetail.DocumentNo;
      }
    }

    public string NewFieldValue { get; set; }

    public int BatchNo { get; set; }

    public int SequenceNo { get; set; }

    public Guid Id { get; set; }

    public string ValidationExceptionId { get; set; }

    /// <summary>
    /// this should be zero while parsing
    /// </summary>
    public int OnlineCorrectionStaus { get; set; }

    /// <summary>
    /// Entity Modification date. Must be UTC date and time.
    /// </summary>
    public DateTime LastUpdatedOn { get; set; }

    /// <summary>
    /// Entity Modification By.
    /// </summary>
    public int LastUpdatedBy { get; set; }

    public string IssuingAirline { get; set; }

    public string PkReferenceId { get; set; }

    public string YourInvoiceNo { get; set; }

    public int YourInvoiceBillingMonth { get; set; }

    public int YourInvoiceBillingYear { get; set; }

    public int YourInvoiceBillingPeriod { get; set; }

    public string YourRejectionMemoNo { get; set; }

    public string YourBmCmNo { get; set; }

    public int YourBmCmIndicator { get; set; }

    public int TransactionId { get; set; }

    public long CorrespondenceRefNo { get; set; }

    public string ReasonCode { get; set; }

    public int RejectionStage { get; set; }

    public int ExceptionCodeId { get; set; }

    public bool BatchUpdateAllowed { get; set; }

    public string ErrorCorrectionLevel { get; set; }

    public string FimBmCmNumber { get; set; }

    public int? FimCouponNumber { get; set; }

    public string ProvInvoiceNo { get; set; }

    public int? CouponNo { get; set; }

    public int FimBmCmIndicator { get; set; }
  }
}
