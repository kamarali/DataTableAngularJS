using System;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Common
{
  public class ValidationErrorCorrection
  {
    public int BillingYear { set; get; }

    public int BillingMonth { set; get; }

    public int BillingPeriod { set; get; }

    public string BilledMember { get; set; }

    public int BilledMemberId { get; set; }

    public int BillingMemberId { get; set; }

    public string InvoiceNumber { get; set; }

    public DateTime? FileSubmissionDate { get; set; }

    public string FileName { get; set; }

    public int ChargeCategoryId { get; set; }

    public string ExceptionCode { get; set; }

    public string SamplingExceptionCode { get; set; }

    public string ErrorDescription { get; set; }

   

    public string SamplingErrorDescription { get; set; }

    public string ChargeCategory { get; set; }

    public int ExceptionCodeId { get; set; }

    public int ErrorCount { get; set; }

    public string BatchUpdateAllowed
    {
      get { return IsBachtUpdateAllowed ? "Yes" : "No"; }
    }

    public bool IsBachtUpdateAllowed { get; set; }

    public string InvoiceType { get; set; }

    public string ErrorLevel { get; set; }

    public string FieldName { get; set; }

    public string  SamplingFieldName { get; set; }

    public string FieldValue { get; set; }

    public string SamplingFieldValue { get; set; }

    public string NewValue { get; set; }

    public string SamplingNewValue { get; set; }
    /*    public string ClearanceMonth 
        {
          get { return string.Concat(BillingYear.ToString(), BillingMonth.ToString().PadLeft(2, '0')); }
        }

        public string BilledMemberCodeNumeric 
        {
          get 
          {
            if(BilledMember != null && BilledMember.Length > 0 && BilledMember.Split('-').Length >= 1 )
            {
              return BilledMember.Split('-')[1];
            }
            return string.Empty;
          }
        }*/

    public string UpdateFileName { get; set; }

    public string SamplingUpdateFileName { get; set; }

    public string UpdateExceptionCode { get; set; }

    public string SamplingUpdateExceptionCode { get; set; }

    public Guid ExceptionSummaryId { get; set; }

    public Guid ExceptionDetailId { get; set; }

    public int BatchUpdatedFieldCount { get; set; }

    public String YourInvoiceNo { get; set; }

    public String SamplingYourInvoiceNo { get; set; }

    public int YourInvoiceMonth { get; set; }

    public int YourInvoiceYear { get; set; }

    public int YourInvoicePeriod { get; set; }

    public string YourRejectionMemoNo { get; set; }

    public string YourBmCmNo { get; set; }

    public int BmCmIndicator { get; set; }

    public string BmCmIndicatorDisplay
    {
      get
      {
        switch (BmCmIndicator)
        {
          case (int)BMCMIndicator.None:
            return "None";
            break;
          case (int)BMCMIndicator.BMNumber:
            return "Billing Memo";
            break;
          case (int)BMCMIndicator.CMNumber:
            return "Credit Memo";
            break;
          default:
            return "";
        }
      }
    }

    public string YourInvoiceBillingDate { get; set; }

    public bool IsBmCmIndicator { get; set; }

    public string CorrectLinkingFileName { get; set; }

    public string samplingFileName { get; set; }

    public string CorrectLinkingExceptionCode { get; set; }

    public string CorrectLinkingErrorDescription { get; set; }

    public string SamplingLinkErrorDescription { get; set; }

    public string CorrespondenceRefNo { get; set; }

    public int TranscationId { get; set; }

    public string ReasonCode { get; set; }

    public int RejectionStage { get; set; }

    public Guid InvoiceID { get; set; }

    public Guid PkReferenceId { get; set; }

    public int LinkingDetail { get; set; }

    public bool Ignorevalidation { get; set; }

    public BillingCategoryType ValidationErrorCategoryType { get; set; }

    public int BatchSeqNo { get; set; }

    public int SamplingBatchSeqNo { get; set; }

    public int BatchRecordSeq { get; set; }

    public int SamplingBatchRecordSeq { get; set; }

    public string FimBmCmNo { get; set; }

    public int? FimCouponNo { get; set; }

    public string ProvisionalInvoiceNo { get; set; }

    public int FimBmCmIndicator { get; set; }

    public string FimBmCmIndicatorDisplay
    {
      get
      {
        switch (FimBmCmIndicator)
        {
          case (int)BMCMIndicator.None:
            return "None";
            break;
          case (int)BMCMIndicator.BMNumber:
            return "Billing Memo";
            break;
          case (int)BMCMIndicator.CMNumber:
            return "Credit Memo";
            break;
          default:
            return "";
        }
      }
    }

    public int CouponNo { get; set; }

    public bool IsFormC { get; set; }

    //SCP:37078 Add Source code property used for Linking purpose of FIM RM 2 and RM3
    public int SourceCodeId { get; set; }
    //SCP252342 - SRM: ICH invoice in ready for billing status
    public DateTime LastUpdatedOn { get; set; }
  }
}
