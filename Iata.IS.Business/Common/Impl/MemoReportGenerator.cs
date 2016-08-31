using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.Business.Cargo.Impl;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.Cargo.Impl;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using log4net;

namespace Iata.IS.Business.Common.Impl
{
  public class MemoReportGenerator : IMemoReportGenerator
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    // nVelocity Embedded Resource Names 
    private const string SrmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.SamplingRejectionMemoDetailReport.vm";
    private const string NsrmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.NonSamplinghRejectionMemoDetailReport.vm";
    private const string CmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.CreditMemoDetailReport.vm";
    private const string BmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.BillingMemoDetailReport.vm";
    private const string CgoCmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.CgoCreditMemoDetailReport.vm";
    private const string CgoBmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.CgoBillingMemoDetailReport.vm";
    private const string CgoRmTemplateResosurceName = "Iata.IS.Business.App_Data.Templates.CgoRejectionMemoDetailReport.vm";
    private const string ReportGenerationDateFormat = "dd/MM/yyyy HH:mm";
    private const string CgoReportGenerationDateFormat = "dd\\/MM\\/yyyy HH:mm";

    public ITemplatedTextGenerator TemplatedTextGenerator { private get; set; }
    public IReferenceManager ReferenceManager { private get; set; }

    /// <summary>
    /// Creates the memo.
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="errors">The errors.</param>
    public void CreateMemoReports(PaxInvoice paxInvoice, string reportDirPath, StringBuilder errors)
    {
      if (paxInvoice.BillingCode != (int)BillingCode.SamplingFormF && paxInvoice.BillingCode != (int)BillingCode.SamplingFormXF)
      {
        if (paxInvoice.BillingCode == (int)BillingCode.NonSampling)
        {
          CreateBillingMemoHtmlReport(reportDirPath, paxInvoice);
          CreateCreditMemoHtmlReport(reportDirPath, paxInvoice);
          CreateNonSamplingRejectionMemoHtmlReport(reportDirPath, paxInvoice);
        }
      }
      else
      {
        CreateSamplingRejectionMemoReport(reportDirPath, paxInvoice);
      }
    }

    /// <summary>
    /// Creates the memos for Cgo Invoice
    /// </summary>
    /// <param name="cgoInvoice"></param>
    /// <param name="reportDirPath"></param>
    /// <param name="errors"></param>
    public void CreateMemoReports(CargoInvoice cgoInvoice, string reportDirPath, StringBuilder errors)
    {
      CreateCgoRejectionMemoHtmlReport(reportDirPath, cgoInvoice);
      CreateCgoBillingMemoHtmlReport(reportDirPath, cgoInvoice);
      CreateCgoCreditMemoHtmlReport(reportDirPath, cgoInvoice);

    }
    /// <summary>
    /// Creates the HTML report.
    /// </summary>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="invoice">The invoice.</param>
    private void CreateSamplingRejectionMemoReport(string reportDirPath, PaxInvoice invoice)
    {
      
      var transType = TransactionType.SamplingFormF;

      if (invoice.BillingCode != (int)BillingCode.SamplingFormF)
      {
        if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
        {
          transType = TransactionType.SamplingFormXF;
        }
      }
      else
      {
        transType = TransactionType.SamplingFormF;
      }

      //Fetch Rejection Memo details
      foreach (var rejectionMemo in invoice.RejectionMemoRecord)
      {
          var context = new NVelocity.VelocityContext();
          //SCP112277: Call logged on IS WEB
          //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
          // priority list 1: Yes 2: Pending (not for Cargo) 3: No

          var rejectionIndicator = 0;
          var couponleveldownAttachmentIndicator = 0;
          var memolevelAttachmentIndicator = rejectionMemo.AttachmentIndicatorOriginal;


          if (rejectionMemo.CouponBreakdownRecord == null || rejectionMemo.CouponBreakdownRecord.Count <= 0)
          {
            couponleveldownAttachmentIndicator = 0;
          }
          else
          {
              if (rejectionMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 1).Count() > 0)
              {
                couponleveldownAttachmentIndicator = 1;
              }
              else if (rejectionMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 2).Count() > 0)
              {
                couponleveldownAttachmentIndicator = 2;
              }
          }

         rejectionIndicator = memolevelAttachmentIndicator > couponleveldownAttachmentIndicator
                                  ? memolevelAttachmentIndicator
                                  : couponleveldownAttachmentIndicator;

        var samplingRejectionMemoDetailReport = new SamplingRejectionMemoDetailReport
                                                  {
                                                    From = invoice.BillingMemberText ?? string.Empty,
                                                    To = invoice.BilledMemberText,
                                                    RejectionMemoNumber = rejectionMemo.RejectionMemoNumber,
                                                    InvoiceNumber = invoice.InvoiceNumber,
                                                    DisplayBillingMonthYear = invoice.DisplayBillingMonthYear,
                                                    BillingPeriod = invoice.BillingPeriod,
                                                    OurRef = rejectionMemo.OurRef ?? string.Empty,
                                                    SourceCodeId = rejectionMemo.SourceCodeId,
                                                    RejectionStage = rejectionMemo.RejectionStage,
                                                    AttachmentIndicatorOriginal = rejectionIndicator,
                                                    Attachments = rejectionMemo.Attachments,
                                                    ExchangeRate = invoice.ListingToBillingRate,
                                                    CurrencyOfRm = invoice.ListingCurrencyDisplayText,
                                                    FIMBMCMIndicator = rejectionMemo.FIMBMCMIndicator,
                                                    FimBMCMNumber = rejectionMemo.FimBMCMNumber,
                                                    FimCouponNumber = rejectionMemo.FimCouponNumber,
                                                    YourInvoiceNumber = rejectionMemo.YourInvoiceNumber,
                                                    DisplayYourInvoiceBillingMonthYearReport = rejectionMemo.DisplayYourInvoiceBillingMonthYear,
                                                    YourInvoiceBillingYear = rejectionMemo.YourInvoiceBillingYear,
                                                    YourInvoiceBillingPeriod = rejectionMemo.YourInvoiceBillingPeriod,
                                                    YourRejectionNumber = rejectionMemo.YourRejectionNumber ?? string.Empty,
                                                    TotalGrossAmountBilled = rejectionMemo.TotalGrossAmountBilled,
                                                    AllowedIscAmount = rejectionMemo.AllowedIscAmount,
                                                    AllowedOtherCommission = rejectionMemo.AllowedOtherCommission,
                                                    AllowedUatpAmount = rejectionMemo.AllowedUatpAmount,
                                                    AllowedHandlingFee = rejectionMemo.AllowedHandlingFee,
                                                    TotalTaxAmountBilled = rejectionMemo.TotalTaxAmountBilled,
                                                    TotalVatAmountBilled = rejectionMemo.TotalVatAmountBilled,
                                                    TotalGrossAcceptedAmount = rejectionMemo.TotalGrossAcceptedAmount,
                                                    AcceptedIscAmount = rejectionMemo.AcceptedIscAmount,
                                                    AcceptedOtherCommission = rejectionMemo.AcceptedOtherCommission,
                                                    AcceptedUatpAmount = rejectionMemo.AcceptedUatpAmount,
                                                    AcceptedHandlingFee = rejectionMemo.AcceptedHandlingFee,
                                                    TotalTaxAmountAccepted = rejectionMemo.TotalTaxAmountAccepted,
                                                    TotalVatAmountAccepted = rejectionMemo.TotalVatAmountAccepted,
                                                    TotalGrossDifference = rejectionMemo.TotalGrossDifference,
                                                    IscDifference = rejectionMemo.IscDifference,
                                                    OtherCommissionDifference = rejectionMemo.OtherCommissionDifference,
                                                    UatpAmountDifference = rejectionMemo.UatpAmountDifference,
                                                    HandlingFeeAmountDifference = rejectionMemo.HandlingFeeAmountDifference,
                                                    TotalTaxAmountDifference = rejectionMemo.TotalTaxAmountDifference,
                                                    TotalVatAmountDifference = rejectionMemo.TotalVatAmountDifference,
                                                    RejectionMemoVat = rejectionMemo.RejectionMemoVat,
                                                    TotalVatAmountReport = rejectionMemo.RejectionMemoVat.Sum(rec => rec.VatCalculatedAmount),
                                                    ReasonCodeDescription =
                                                    rejectionMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, (int)transType) != null ? ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, (int)transType).Description : string.Empty),
                                                    ReasonRemarks = rejectionMemo.ReasonRemarks,
                                                    CouponBreakdownRecord = rejectionMemo.CouponBreakdownRecord,
                                                    ProvisionalBillingMonth = invoice.DisplayProvisionalBillingMonthYear,
                                                    SamplingConstant = rejectionMemo.SamplingConstant,
                                                    TotalNetRejectAmount = rejectionMemo.TotalNetRejectAmount,
                                                    TotalNetRejectAmountAfterSamplingConstant = rejectionMemo.TotalNetRejectAmountAfterSamplingConstant,
                                                    ReportGenerationDate = DateTime.UtcNow.ToString(ReportGenerationDateFormat)
                                                  };
     
        context.Put("Utility", new FormatUtility());
        context.Put("Message", samplingRejectionMemoDetailReport);

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(SrmTemplateResosurceName, context);

        using (var strwriter =
          new StreamWriter(Path.Combine(reportDirPath,
                                        string.Format("{0}SC{1}-{2}-{3}-{4}.HTML",
                                                      PaxReportConstants.PaxBillingCategory,
                                                      Convert.ToString(rejectionMemo.SourceCodeId).PadLeft(2, '0'),
                                                      Convert.ToString(rejectionMemo.BatchSequenceNumber).PadLeft(5, '0'),
                                                      Convert.ToString(rejectionMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                      Convert.ToString(rejectionMemo.RejectionMemoNumber)))))
        {
        strwriter.Write(reportContent);
        strwriter.Close();
        }
        context = null;
        samplingRejectionMemoDetailReport = null;
        reportContent = null;
      }
    }

    /// <summary>
    /// Creates the HTML report.
    /// </summary>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="invoice">The invoice.</param>
    private void CreateNonSamplingRejectionMemoHtmlReport(string reportDirPath, PaxInvoice invoice)
    {
      //Fetch Rejection Memo details
      foreach (var rejectionMemo in invoice.RejectionMemoRecord)
      {
        var transType = TransactionType.RejectionMemo1;
        switch (rejectionMemo.RejectionStage)
        {
          case (int)RejectionStage.StageOne:
            transType = TransactionType.RejectionMemo1;
            break;
          case (int)RejectionStage.StageTwo:
            transType = TransactionType.RejectionMemo2;
            break;
          case (int)RejectionStage.StageThree:
            transType = TransactionType.RejectionMemo3;
            break;
        }

        //SCP112277: Call logged on IS WEB
        //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
        // priority list 1: Yes 2: Pending (not for Cargo) 3: No

        var rejectionIndicator = 0;
        var couponleveldownAttachmentIndicator = 0;
        var memolevelAttachmentIndicator = rejectionMemo.AttachmentIndicatorOriginal;


        if (rejectionMemo.CouponBreakdownRecord == null || rejectionMemo.CouponBreakdownRecord.Count <= 0)
        {
          couponleveldownAttachmentIndicator = 0;
        }
        else
        {
            if (rejectionMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 1).Count() > 0)
            {
              couponleveldownAttachmentIndicator = 1;
            }
            else if (rejectionMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 2).Count() > 0)
            {
              couponleveldownAttachmentIndicator = 2;
            }
        }

        rejectionIndicator = memolevelAttachmentIndicator > couponleveldownAttachmentIndicator
                                ? memolevelAttachmentIndicator
                                : couponleveldownAttachmentIndicator;

        var samplingRejectionMemoDetailReport = new SamplingRejectionMemoDetailReport
                                                  {
                                                    From = invoice.BillingMemberText,
                                                    To = invoice.BilledMemberText,
                                                    RejectionMemoNumber = rejectionMemo.RejectionMemoNumber,
                                                    InvoiceNumber = invoice.InvoiceNumber,
                                                    DisplayBillingMonthYear = invoice.DisplayBillingMonthYear,
                                                    BillingPeriod = invoice.BillingPeriod,
                                                    FimBMCMNumber = rejectionMemo.FimBMCMNumber,
                                                    FimCouponNumber = rejectionMemo.FimCouponNumber,
                                                    FIMBMCMIndicator = rejectionMemo.FIMBMCMIndicator,
                                                    OurRef = rejectionMemo.OurRef ?? string.Empty,
                                                    SourceCodeId = rejectionMemo.SourceCodeId,
                                                    RejectionStage = rejectionMemo.RejectionStage,
                                                    AttachmentIndicatorOriginal = rejectionIndicator,
                                                    Attachments = rejectionMemo.Attachments,
                                                    ExchangeRate = invoice.ListingToBillingRate,
                                                    CurrencyOfRm = invoice.ListingCurrencyDisplayText,
                                                    YourInvoiceNumber = rejectionMemo.YourInvoiceNumber,
                                                    DisplayYourInvoiceBillingMonthYearReport = rejectionMemo.DisplayYourInvoiceBillingMonthYear,
                                                    YourInvoiceBillingPeriod = rejectionMemo.YourInvoiceBillingPeriod,
                                                    YourRejectionNumber = rejectionMemo.YourRejectionNumber ?? string.Empty,
                                                    TotalGrossAmountBilled = rejectionMemo.TotalGrossAmountBilled,
                                                    AllowedIscAmount = rejectionMemo.AllowedIscAmount,
                                                    AllowedOtherCommission = rejectionMemo.AllowedOtherCommission,
                                                    AllowedUatpAmount = rejectionMemo.AllowedUatpAmount,
                                                    AllowedHandlingFee = rejectionMemo.AllowedHandlingFee,
                                                    TotalTaxAmountBilled = rejectionMemo.TotalTaxAmountBilled,
                                                    TotalVatAmountBilled = rejectionMemo.TotalVatAmountBilled,
                                                    TotalGrossAcceptedAmount = rejectionMemo.TotalGrossAcceptedAmount,
                                                    AcceptedIscAmount = rejectionMemo.AcceptedIscAmount,
                                                    AcceptedOtherCommission = rejectionMemo.AcceptedOtherCommission,
                                                    AcceptedUatpAmount = rejectionMemo.AcceptedUatpAmount,
                                                    AcceptedHandlingFee = rejectionMemo.AcceptedHandlingFee,
                                                    TotalTaxAmountAccepted = rejectionMemo.TotalTaxAmountAccepted,
                                                    TotalVatAmountAccepted = rejectionMemo.TotalVatAmountAccepted,
                                                    TotalGrossDifference = rejectionMemo.TotalGrossDifference,
                                                    IscDifference = rejectionMemo.IscDifference,
                                                    OtherCommissionDifference = rejectionMemo.OtherCommissionDifference,
                                                    UatpAmountDifference = rejectionMemo.UatpAmountDifference,
                                                    HandlingFeeAmountDifference = rejectionMemo.HandlingFeeAmountDifference,
                                                    TotalTaxAmountDifference = rejectionMemo.TotalTaxAmountDifference,
                                                    TotalVatAmountDifference = rejectionMemo.TotalVatAmountDifference,
                                                    RejectionMemoVat = rejectionMemo.RejectionMemoVat.ToList(),
                                                    TotalVatAmountReport = rejectionMemo.RejectionMemoVat.Sum(rec => rec.VatCalculatedAmount),
                                                    ReasonCodeDescription =
                                                    rejectionMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, (int)transType) != null ? ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, (int)transType).Description : string.Empty),
                                                    ReasonRemarks = rejectionMemo.ReasonRemarks,
                                                    CouponBreakdownRecord = rejectionMemo.CouponBreakdownRecord == null ? new List<RMCoupon>() : rejectionMemo.CouponBreakdownRecord.ToList(),
                                                    SamplingConstant = rejectionMemo.SamplingConstant,
                                                    TotalNetRejectAmount = rejectionMemo.TotalNetRejectAmount,
                                                    ReportGenerationDate = DateTime.UtcNow.ToString(ReportGenerationDateFormat)
                                                  };
        var context = new NVelocity.VelocityContext();
        context.Put("Utility", new FormatUtility());
        context.Put("Message", samplingRejectionMemoDetailReport);

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(NsrmTemplateResosurceName, context);
        using (var strwriter =
          new StreamWriter(Path.Combine(reportDirPath,
                                        string.Format("{0}SC{1}-{2}-{3}-{4}.HTML",
                                                      PaxReportConstants.PaxBillingCategory,
                                                      Convert.ToString(rejectionMemo.SourceCodeId).PadLeft(2, '0'),
                                                      Convert.ToString(rejectionMemo.BatchSequenceNumber).PadLeft(5, '0'),
                                                      Convert.ToString(rejectionMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                      Convert.ToString(rejectionMemo.RejectionMemoNumber)))))
        {
        strwriter.Write(reportContent);
        strwriter.Close();
        }
        context = null;
        samplingRejectionMemoDetailReport = null;
        reportContent = null;
      }
    }

    /// <summary>
    /// Creates the HTML report.
    /// </summary>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="invoice">The invoice.</param>
    private void  CreateCreditMemoHtmlReport(string reportDirPath, PaxInvoice invoice)
    {
      //Fetch credit Memo details
      foreach (var creditMemo in invoice.CreditMemoRecord)
      {
          var context = new NVelocity.VelocityContext();
          //SCP112277: Call logged on IS WEB
          //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
          // priority list 1: Yes 2: Pending (not for Cargo) 3: No

          var creditIndicator = 0;
          var couponleveldownAttachmentIndicator = 0;
          var memolevelAttachmentIndicator = creditMemo.AttachmentIndicatorOriginal;

          if (creditMemo.CouponBreakdownRecord == null || creditMemo.CouponBreakdownRecord.Count <= 0)
          {
            couponleveldownAttachmentIndicator = 0;
          }
          else
          {
              if (creditMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 1).Count() > 0)
              {
                  couponleveldownAttachmentIndicator = 1;
              }
              else if (creditMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 2).Count() > 0)
              {
                couponleveldownAttachmentIndicator = 2;
              }
          }

          creditIndicator = memolevelAttachmentIndicator > couponleveldownAttachmentIndicator
                                ? memolevelAttachmentIndicator
                                : couponleveldownAttachmentIndicator;

        var creditMemoDetailReport = new CreditMemoDetailReport
                                       {
                                         From = invoice.BillingMemberText,
                                         To = invoice.BilledMemberText,
                                         CreditMemoNumber = creditMemo.CreditMemoNumber,
                                         InvoiceNumber = invoice.InvoiceNumber,
                                         DisplayBillingMonthYear = invoice.DisplayBillingMonthYear,
                                         BillingPeriod = invoice.BillingPeriod,
                                         OurRef = creditMemo.OurRef ?? string.Empty,
                                         SourceCodeId = creditMemo.SourceCodeId,
                                         CorrespondenceRefNumber = creditMemo.CorrespondenceRefNumber,
                                         AttachmentIndicatorOriginal = creditIndicator,
                                         ExchangeRate = invoice.ListingToBillingRate,
                                         CurrencyOfCm = invoice.ListingCurrencyDisplayText,
                                         DisplayYourInvoiceBillingMonthYearReport = creditMemo.DisplayYourInvoiceBillingMonthYear,
                                         FimNumber = creditMemo.FimNumber,
                                         FimCouponNumber = creditMemo.FimCouponNumber,
                                         YourInvoiceNumber = creditMemo.YourInvoiceNumber,
                                         YourInvoiceBillingPeriod = creditMemo.YourInvoiceBillingPeriod,
                                         TotalGrossAmountCredited = creditMemo.TotalGrossAmountCredited,
                                         TotalIscAmountCredited = creditMemo.TotalIscAmountCredited,
                                         TotalOtherCommissionAmountCredited = creditMemo.TotalOtherCommissionAmountCredited,
                                         TotalUatpAmountCredited = creditMemo.TotalUatpAmountCredited,
                                         TotalHandlingFeeCredited = creditMemo.TotalHandlingFeeCredited,
                                         TaxAmount = creditMemo.TaxAmount,
                                         VatAmount = creditMemo.VatAmount,
                                         NetAmountCredited = creditMemo.NetAmountCredited,
                                         VatBreakdown = creditMemo.VatBreakdown,
                                         TotalVatAmountReport = creditMemo.VatBreakdown.Sum(rec => rec.VatCalculatedAmount),
                                         ReasonCodeDescription =
                                         creditMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(creditMemo.ReasonCode, (int)TransactionType.CreditMemo) != null ? ReferenceManager.GetReasonCode(creditMemo.ReasonCode, (int)TransactionType.CreditMemo).Description : string.Empty),
                                         ReasonRemarks = creditMemo.ReasonRemarks,
                                         CouponBreakdownRecord = creditMemo.CouponBreakdownRecord,
                                         Attachments = creditMemo.Attachments,
                                         ReportGenerationDate = DateTime.UtcNow.ToString(ReportGenerationDateFormat)
                                       };

        
        context.Put("Utility", new FormatUtility());
        context.Put("Message", creditMemoDetailReport);
        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(CmTemplateResosurceName, context);
        using (var strwriter =
          new StreamWriter(Path.Combine(reportDirPath,
                                        string.Format("{0}SC{1}-{2}-{3}-{4}.HTML",
                                                      PaxReportConstants.PaxBillingCategory,
                                                      Convert.ToString(creditMemo.SourceCodeId).PadLeft(2, '0'),
                                                      Convert.ToString(creditMemo.BatchSequenceNumber).PadLeft(5, '0'),
                                                      Convert.ToString(creditMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                      Convert.ToString(creditMemo.CreditMemoNumber)))))
        {
        strwriter.Write(reportContent);
        strwriter.Close();
        }
        context = null;
        creditMemoDetailReport = null;
        reportContent = null;
      }
    }

    /// <summary>
    /// Creates the HTML report.
    /// </summary>
    /// <param name="reportDirPath">The report dir path.</param>
    /// <param name="invoice">The invoice.</param>
    private void CreateBillingMemoHtmlReport(string reportDirPath, PaxInvoice invoice)
    {
      //Fetch Billing Memo details
      foreach (var billingMemo in invoice.BillingMemoRecord)
      {
          var context = new NVelocity.VelocityContext();
          //SCP112277: Call logged on IS WEB
          //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
          // priority list 1: Yes 2: Pending (not for Cargo) 3: No

          var billingIndicator = 0;
          var couponleveldownAttachmentIndicator = 0;
          var memolevelAttachmentIndicator = billingMemo.AttachmentIndicatorOriginal;
         
          if (billingMemo.CouponBreakdownRecord == null || billingMemo.CouponBreakdownRecord.Count <= 0)
          {
            couponleveldownAttachmentIndicator = 0;
          }
          else
          {
              if (billingMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 1).Count() > 0)
              {
                couponleveldownAttachmentIndicator = 1;
              }
              else if (billingMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal == 2).Count() > 0)
              {
                couponleveldownAttachmentIndicator = 2;
              }
          }

         billingIndicator = memolevelAttachmentIndicator > couponleveldownAttachmentIndicator
                                 ? memolevelAttachmentIndicator
                                 : couponleveldownAttachmentIndicator;


        var billingMemoDetailReport = new BillingMemoDetailReport
                                        {
                                          From = invoice.BillingMemberText,
                                          To = invoice.BilledMemberText,
                                          BillingMemoNumber = billingMemo.BillingMemoNumber,
                                          InvoiceNumber = invoice.InvoiceNumber,
                                          DisplayBillingMonthYear = invoice.DisplayBillingMonthYear,
                                          BillingPeriod = invoice.BillingPeriod,
                                          OurRef = billingMemo.OurRef ?? string.Empty,
                                          SourceCodeId = billingMemo.SourceCodeId,
                                          CorrespondenceRefNumber = billingMemo.CorrespondenceRefNumber,
                                          AttachmentIndicatorOriginal = billingIndicator,
                                          ExchangeRate = invoice.ListingToBillingRate,
                                          CurrencyOfBm = invoice.ListingCurrencyDisplayText,
                                          DisplayYourInvoiceBillingMonthYearReport = billingMemo.DisplayYourInvoiceBillingMonthYear,
                                          FimNumber = billingMemo.FimNumber,
                                          FimCouponNumber = billingMemo.FimCouponNumber,
                                          YourInvoiceNumber = billingMemo.YourInvoiceNumber,
                                          YourInvoiceBillingPeriod = billingMemo.YourInvoiceBillingPeriod,
                                          TotalGrossAmountBilled = billingMemo.TotalGrossAmountBilled,
                                          TotalIscAmountBilled = billingMemo.TotalIscAmountBilled,
                                          TotalOtherCommissionAmount = billingMemo.TotalOtherCommissionAmount,
                                          TotalUatpAmountBilled = billingMemo.TotalUatpAmountBilled,
                                          TotalHandlingFeeBilled = billingMemo.TotalHandlingFeeBilled,
                                          TaxAmountBilled = billingMemo.TaxAmountBilled,
                                          TotalVatAmountBilled = billingMemo.TotalVatAmountBilled,
                                          NetAmountBilled = billingMemo.NetAmountBilled,
                                          VatBreakdown = billingMemo.VatBreakdown,
                                          TotalVatAmountReport = billingMemo.VatBreakdown.Sum(rec => rec.VatCalculatedAmount),
                                          ReasonCodeDescription =
                                            billingMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(billingMemo.ReasonCode, (int)TransactionType.BillingMemo) != null ? ReferenceManager.GetReasonCode(billingMemo.ReasonCode, (int)TransactionType.BillingMemo).Description : string.Empty),
                                          ReasonRemarks = billingMemo.ReasonRemarks,
                                          CouponBreakdownRecord = billingMemo.CouponBreakdownRecord,
                                          Attachments = billingMemo.Attachments,
                                          ReportGenerationDate = DateTime.UtcNow.ToString(ReportGenerationDateFormat),
                                          AirlineOwnUse = billingMemo.AirlineOwnUse
                                        };

        context.Put("Utility", new FormatUtility());
        context.Put("Message", billingMemoDetailReport);

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(BmTemplateResosurceName, context);
        using (var strwriter = new StreamWriter(Path.Combine(reportDirPath,

                                        string.Format("{0}SC{1}-{2}-{3}-{4}.HTML",
                                                      PaxReportConstants.PaxBillingCategory,
                                                      Convert.ToString(billingMemo.SourceCodeId).PadLeft(2, '0'),
                                                      Convert.ToString(billingMemo.BatchSequenceNumber).PadLeft(5, '0'),
                                                      Convert.ToString(billingMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                                                      Convert.ToString(billingMemo.BillingMemoNumber)))))
        {
        strwriter.Write(reportContent);
        strwriter.Close();
        }
        context = null;
        billingMemoDetailReport = null;
        reportContent = null;
      }
    }
    

    /// <summary>
    /// Creates Cgo Billing Memo HTML report
    /// </summary>
    /// <param name="reportDirPath"></param>
    /// <param name="cgoInvoice"></param>
    private void CreateCgoBillingMemoHtmlReport(string reportDirPath, CargoInvoice cgoInvoice)
    {

      

      //Fetch Billing Memo details
      Logger.Info("Creating Cgo billing memo Html Reort");
      foreach (var billingMemo in cgoInvoice.CGOBillingMemo)
      {
         var context = new NVelocity.VelocityContext();
         //SCP112277: Call logged on IS WEB
         //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
         // priority list 1: Yes 2: Pending (not for Cargo) 3: No
         var bmAttachmentIndicatorOriginal = false;
         var couponleveldownAttachmentIndicator = false;
         var memolevelAttachmentIndicator = billingMemo.AttachmentIndicatorOriginal;

         if (billingMemo.AwbBreakdownRecord == null || billingMemo.AwbBreakdownRecord.Count <= 0)
           couponleveldownAttachmentIndicator = false;
         else
         {
           if (billingMemo.AwbBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal).Count() > 0)
           {
             couponleveldownAttachmentIndicator = true;
           }
         }
        // billingMemo.AttachmentIndicatorOriginal || ((billingMemo.AwbBreakdownRecord == null || billingMemo.AwbBreakdownRecord.Count <= 0) ? false : billingMemo.AwbBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal).Count() > 0),
        bmAttachmentIndicatorOriginal = memolevelAttachmentIndicator || couponleveldownAttachmentIndicator
                                ? true
                                : false;

        var cgoBillingMemoDetailReport = new CgoBillingMemoDetailReport()
                                        {
                                          From = cgoInvoice.BillingMemberText,
                                          To = cgoInvoice.BilledMemberText,
                                          BillingMemoNumber = billingMemo.BillingMemoNumber,
                                          InvoiceNumber = cgoInvoice.InvoiceNumber,
                                          DisplayBillingMonthYear = cgoInvoice.DisplayBillingMonthYear,
                                          BillingPeriod = cgoInvoice.BillingPeriod,
                                          OurRef = billingMemo.OurRef ?? string.Empty,
                                          DisplayCorrespondenceReferenceNumber = billingMemo.CorrespondenceReferenceNumber.ToString("00000000000"),
                                          AttachmentIndicatorOriginal = bmAttachmentIndicatorOriginal,
                                          ExchangeRate = cgoInvoice.ListingToBillingRate,
                                          CurrencyOfBm = cgoInvoice.ListingCurrencyDisplayText,
                                          DisplayYourInvoiceBillingMonthYearReport = billingMemo.DisplayYourInvoiceBillingMonthYear,
                                          YourInvoiceNumber = billingMemo.YourInvoiceNumber,
                                          YourInvoiceBillingPeriod = billingMemo.YourInvoiceBillingPeriod,
                                          BilledTotalIscAmount = billingMemo.BilledTotalIscAmount,
                                          BilledTotalOtherChargeAmount = billingMemo.BilledTotalOtherChargeAmount,
                                          BilledTotalValuationAmount = billingMemo.BilledTotalValuationAmount,
                                          BilledTotalWeightCharge = billingMemo.BilledTotalWeightCharge,
                                          BilledTotalVatAmount = billingMemo.BilledTotalVatAmount,
                                          NetBilledAmount = billingMemo.NetBilledAmount,
                                          BillingMemoVat = billingMemo.BillingMemoVat,
                                          TotalVatAmountReport = billingMemo.BillingMemoVat.Sum(rec => rec.VatCalculatedAmount),
                                          ReasonCodeDescription = billingMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(billingMemo.ReasonCode, (int)TransactionType.CargoBillingMemo) != null ? ReferenceManager.GetReasonCode(billingMemo.ReasonCode, (int)TransactionType.CargoBillingMemo).Description : string.Empty),
                                          ReasonRemarks = billingMemo.ReasonRemarks,
                                          AwbBreakdownRecord = billingMemo.AwbBreakdownRecord != null ? billingMemo.AwbBreakdownRecord.OrderBy(awb => awb.AwbSerialNumber).ToList() : null,
                                          Attachments = billingMemo.Attachments,
                                          ReportGenerationDate = DateTime.UtcNow.ToString(CgoReportGenerationDateFormat),
                                          AirlineOwnUse = billingMemo.AirlineOwnUse
                                        };


        context.Put("Message", cgoBillingMemoDetailReport);
        context.Put("Utility", new CgoRmBmCmDetailReportUtility());

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(CgoBmTemplateResosurceName, context);



        var strwriter =
          new StreamWriter(Path.Combine(reportDirPath, string.Format("{0}BM-{1}-{2}-{3}.HTML",
                      CargoReportConstants.CgoBillingCategory,
                      Convert.ToString(billingMemo.BatchSequenceNumber).PadLeft(5, '0'),
                      Convert.ToString(billingMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                      Convert.ToString(billingMemo.BillingMemoNumber))));


        strwriter.Write(reportContent);
        strwriter.Close();
        Logger.Info("Created Cgo credit memo Html Reort");
      }
    }

    /// <summary>
    /// Creates Cgo Credit Memo HTML report
    /// </summary>
    /// <param name="reportDirPath"></param>
    /// <param name="cgoInvoice"></param>
    private void CreateCgoCreditMemoHtmlReport(string reportDirPath, CargoInvoice cgoInvoice)
    {
      

      //Fetch Billing Memo details
      Logger.Info("Creating Cgo credit memo Html Reort");
      foreach (var creditMemo in cgoInvoice.CGOCreditMemo)
      {
          var context = new NVelocity.VelocityContext();
          //SCP112277: Call logged on IS WEB
          //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
          // priority list 1: Yes 2: Pending (not for Cargo) 3: No
          var cmAttachmentIndicatorOriginal = false;
          var couponleveldownAttachmentIndicator = false;
          var memolevelAttachmentIndicator = creditMemo.AttachmentIndicatorOriginal;

          if (creditMemo.AWBBreakdownRecord == null || creditMemo.AWBBreakdownRecord.Count <= 0)
            couponleveldownAttachmentIndicator = false;
          else
          {
            if (creditMemo.AWBBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal).Count() > 0)
            {
              couponleveldownAttachmentIndicator = true;
            }
          }
          // creditMemo.AttachmentIndicatorOriginal || ((creditMemo.AWBBreakdownRecord == null || creditMemo.AWBBreakdownRecord.Count <= 0) ? false : creditMemo.AWBBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal).Count() > 0),
          cmAttachmentIndicatorOriginal = memolevelAttachmentIndicator || couponleveldownAttachmentIndicator
                                  ? true
                                  : false;

        var cgoCreditMemoDetailReport = new CgoCreditMemoDetailReport()
                                           {
                                             From = cgoInvoice.BillingMemberText,
                                             To = cgoInvoice.BilledMemberText,
                                             CreditMemoNumber = creditMemo.CreditMemoNumber,
                                             InvoiceNumber = cgoInvoice.InvoiceNumber,
                                             DisplayBillingMonthYear = cgoInvoice.DisplayBillingMonthYear,
                                             BillingPeriod = cgoInvoice.BillingPeriod,
                                             OurRef = creditMemo.OurRef ?? string.Empty,
                                             CorrespondenceRefNumber = creditMemo.CorrespondenceRefNumber,
                                             AttachmentIndicatorOriginal = cmAttachmentIndicatorOriginal,
                                             ExchangeRate = cgoInvoice.ListingToBillingRate,
                                             CurrencyOfCm = cgoInvoice.ListingCurrencyDisplayText,
                                             DisplayYourInvoiceBillingMonthYearReport = creditMemo.DisplayYourInvoiceBillingMonthYear,
                                             YourInvoiceNumber = creditMemo.YourInvoiceNumber,
                                             YourInvoiceBillingPeriod = creditMemo.YourInvoiceBillingPeriod,
                                             TotalIscAmountCredited = creditMemo.TotalIscAmountCredited,
                                             TotalOtherChargeAmt = creditMemo.TotalOtherChargeAmt,
                                             TotalValuationAmt = creditMemo.TotalValuationAmt,
                                             TotalWeightCharges = creditMemo.TotalWeightCharges,
                                             TotalVatAmountCredited = creditMemo.TotalVatAmountCredited,
                                             NetAmountCredited = creditMemo.NetAmountCredited,
                                             VatBreakdown = creditMemo.VatBreakdown,
                                             TotalVatAmountReport = creditMemo.VatBreakdown.Sum(rec => rec.VatCalculatedAmount),
                                             ReasonCodeDescription = creditMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(creditMemo.ReasonCode, (int)TransactionType.CargoCreditMemo) != null ? ReferenceManager.GetReasonCode(creditMemo.ReasonCode, (int)TransactionType.CargoCreditMemo).Description : string.Empty),
                                             ReasonRemarks = creditMemo.ReasonRemarks,
                                             AWBBreakdownRecord = creditMemo.AWBBreakdownRecord != null ? creditMemo.AWBBreakdownRecord.OrderBy(awb => awb.AwbSerialNumber).ToList() : null,
                                             Attachments = creditMemo.Attachments,
                                             ReportGenerationDate = DateTime.UtcNow.ToString(CgoReportGenerationDateFormat),
                                             AirlineOwnUse = creditMemo.AirlineOwnUse
                                           };


        context.Put("Message", cgoCreditMemoDetailReport);
        context.Put("Utility", new CgoRmBmCmDetailReportUtility());

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(CgoCmTemplateResosurceName, context);
        var strwriter =
          new StreamWriter(Path.Combine(reportDirPath, string.Format("{0}CM-{1}-{2}-{3}.HTML",
                      CargoReportConstants.CgoBillingCategory,
                      Convert.ToString(creditMemo.BatchSequenceNumber).PadLeft(5, '0'),
                      Convert.ToString(creditMemo.RecordSequenceWithinBatch).PadLeft(5, '0'),
                      Convert.ToString(creditMemo.CreditMemoNumber))));


        strwriter.Write(reportContent);
        strwriter.Close();
        Logger.Info("Created Cgo credit memo Html Reort");
      }
    }

    /// <summary>
    /// Creates Cgo rejection Memo HTML report
    /// </summary>
    /// <param name="reportDirPath"></param>
    /// <param name="cgoInvoice"></param>
    private void CreateCgoRejectionMemoHtmlReport(string reportDirPath, CargoInvoice cgoInvoice)
    {
      
      Logger.Info("Creating rejectionMemo Html Reort");
      //Fetch Rejection Memo details
      foreach (var rejectionMemo in cgoInvoice.CGORejectionMemo)
      {
        var context = new NVelocity.VelocityContext();
        var cgoRejectionMemoDetailReport = new CgoRejectionMemoDetailReport();
        
        //SCP112277: Call logged on IS WEB
        //Logic: Before this fix, system consider only breakdown's attachment indicator value, now(after fix) system get count attachment indicator of coupon level and memo level then take highest priority value for attachment indicator.
        // priority list 1: Yes 2: Pending (not for Cargo) 3: No
        var rmAttachmentIndicatorOriginal = false;
        var couponleveldownAttachmentIndicator = false;
        var memolevelAttachmentIndicator = rejectionMemo.AttachmentIndicatorOriginal;

        if (rejectionMemo.CouponBreakdownRecord == null || rejectionMemo.CouponBreakdownRecord.Count <= 0)
        {
          couponleveldownAttachmentIndicator = false;
        }
        else
        {
          if (rejectionMemo.CouponBreakdownRecord.Where(c => c.AttachmentIndicatorOriginal).Count() > 0)
          {
            couponleveldownAttachmentIndicator = true;
          }
        }

        /*
         rejectionMemo.AttachmentIndicatorOriginal ||
                                                                   ((rejectionMemo.CouponBreakdownRecord == null ||
                                                                     rejectionMemo.CouponBreakdownRecord.Count <= 0)
                                                                      ? false
                                                                      : rejectionMemo.CouponBreakdownRecord.Where(
                                                                        c => c.AttachmentIndicatorOriginal).Count() > 0);
         */

        rmAttachmentIndicatorOriginal = memolevelAttachmentIndicator || couponleveldownAttachmentIndicator
                                ? true
                                : false;

        cgoRejectionMemoDetailReport.From = cgoInvoice.BillingMemberText;
        cgoRejectionMemoDetailReport.To = cgoInvoice.BilledMemberText;
        cgoRejectionMemoDetailReport.RejectionMemoNumber = rejectionMemo.RejectionMemoNumber;
        cgoRejectionMemoDetailReport.InvoiceNumber = cgoInvoice.InvoiceNumber;
        cgoRejectionMemoDetailReport.DisplayBillingMonthYear = cgoInvoice.DisplayBillingMonthYear;
        cgoRejectionMemoDetailReport.BillingPeriod = cgoInvoice.BillingPeriod;
        cgoRejectionMemoDetailReport.OurRef = rejectionMemo.OurRef ?? string.Empty;
        cgoRejectionMemoDetailReport.AttachmentIndicatorOriginal = rmAttachmentIndicatorOriginal;
        cgoRejectionMemoDetailReport.ExchangeRate = cgoInvoice.ListingToBillingRate;
        cgoRejectionMemoDetailReport.CurrencyOfRm = cgoInvoice.ListingCurrencyDisplayText;
        cgoRejectionMemoDetailReport.DisplayYourInvoiceBillingMonthYearReport = rejectionMemo.DisplayYourInvoiceBillingMonthYear;
        cgoRejectionMemoDetailReport.YourInvoiceNumber = rejectionMemo.YourInvoiceNumber;
        cgoRejectionMemoDetailReport.YourRejectionNumber = rejectionMemo.YourRejectionNumber;
        cgoRejectionMemoDetailReport.YourBillingMemoNumber = rejectionMemo.YourBillingMemoNumber;
        cgoRejectionMemoDetailReport.YourInvoiceBillingPeriod = rejectionMemo.YourInvoiceBillingPeriod;
        cgoRejectionMemoDetailReport.BilledTotalWeightCharge = rejectionMemo.BilledTotalWeightCharge;
        cgoRejectionMemoDetailReport.BilledTotalValuationCharge = rejectionMemo.BilledTotalValuationCharge;
        cgoRejectionMemoDetailReport.AllowedTotalIscAmount = rejectionMemo.AllowedTotalIscAmount;
        cgoRejectionMemoDetailReport.BilledTotalOtherChargeAmount = rejectionMemo.BilledTotalOtherChargeAmount;
        cgoRejectionMemoDetailReport.BilledTotalVatAmount = rejectionMemo.BilledTotalVatAmount;
        cgoRejectionMemoDetailReport.AcceptedTotalWeightCharge = rejectionMemo.AcceptedTotalWeightCharge;
        cgoRejectionMemoDetailReport.AcceptedTotalValuationCharge = rejectionMemo.AcceptedTotalValuationCharge;
        cgoRejectionMemoDetailReport.AcceptedTotalIscAmount = rejectionMemo.AcceptedTotalIscAmount;
        cgoRejectionMemoDetailReport.AcceptedTotalOtherChargeAmount = rejectionMemo.AcceptedTotalOtherChargeAmount;
        cgoRejectionMemoDetailReport.AcceptedTotalVatAmount = rejectionMemo.AcceptedTotalVatAmount;
        cgoRejectionMemoDetailReport.TotalWeightChargeDifference = rejectionMemo.TotalWeightChargeDifference;
        cgoRejectionMemoDetailReport.TotalValuationChargeDifference = rejectionMemo.TotalValuationChargeDifference;
        cgoRejectionMemoDetailReport.TotalIscAmountDifference = rejectionMemo.TotalIscAmountDifference;
        cgoRejectionMemoDetailReport.TotalOtherChargeDifference = rejectionMemo.TotalOtherChargeDifference;
        cgoRejectionMemoDetailReport.TotalVatAmountDifference = rejectionMemo.TotalVatAmountDifference;
        cgoRejectionMemoDetailReport.TotalNetRejectAmount = rejectionMemo.TotalNetRejectAmount;
        cgoRejectionMemoDetailReport.RejectionMemoVat = rejectionMemo.RejectionMemoVat != null ? rejectionMemo.RejectionMemoVat.OrderBy(awb => awb.VatIdentifierId).ToList() : null;
        cgoRejectionMemoDetailReport.TotalVatAmountReport = rejectionMemo.RejectionMemoVat != null ? rejectionMemo.RejectionMemoVat.Sum(rec => rec.VatCalculatedAmount) : 0;
        cgoRejectionMemoDetailReport.TransactionTypeTemp = ((rejectionMemo.RejectionStage == 1) ? (int)TransactionType.CargoRejectionMemoStage1 : (rejectionMemo.RejectionStage == 2) ? (int)TransactionType.CargoRejectionMemoStage2 : (int)TransactionType.CargoRejectionMemoStage3);
        cgoRejectionMemoDetailReport.ReasonCodeDescription = rejectionMemo.ReasonCode + ":" + (ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, cgoRejectionMemoDetailReport.TransactionTypeTemp) != null ? ReferenceManager.GetReasonCode(rejectionMemo.ReasonCode, cgoRejectionMemoDetailReport.TransactionTypeTemp).Description : string.Empty);
        cgoRejectionMemoDetailReport.ReasonRemarks = rejectionMemo.ReasonRemarks;
        cgoRejectionMemoDetailReport.CouponBreakdownRecord = rejectionMemo.CouponBreakdownRecord != null ? rejectionMemo.CouponBreakdownRecord.OrderBy(awb => awb.AwbSerialNumber).ToList() : null;
        cgoRejectionMemoDetailReport.Attachments = rejectionMemo.Attachments;
        cgoRejectionMemoDetailReport.ReportGenerationDate = DateTime.UtcNow.ToString(CgoReportGenerationDateFormat);
        cgoRejectionMemoDetailReport.AirlineOwnUse = rejectionMemo.AirlineOwnUse;
        cgoRejectionMemoDetailReport.RejectionStage = rejectionMemo.RejectionStage;
        cgoRejectionMemoDetailReport.BmCmIndicatorReport = rejectionMemo.BMCMIndicatorId == 2 ? "BM" : rejectionMemo.BMCMIndicatorId == 3 ? "CM" : "";

      

          context.Put("Message", cgoRejectionMemoDetailReport);
        context.Put("Utility", new CgoRmBmCmDetailReportUtility());

        var reportContent = TemplatedTextGenerator.GenerateEmbeddedTemplatedText(CgoRmTemplateResosurceName, context);

        var strwriter = new StreamWriter(Path.Combine(reportDirPath, string.Format("{0}RM-{1}-{2}-{3}.HTML",
                                                                     CargoReportConstants.CgoBillingCategory,
                                                                     Convert.ToString(rejectionMemo.BatchSequenceNumber)
                                                                       .PadLeft(5, '0'),
                                                                     Convert.ToString(
                                                                       rejectionMemo.RecordSequenceWithinBatch).PadLeft(
                                                                         5, '0'),
                                                                     Convert.ToString(rejectionMemo.RejectionMemoNumber))));

        strwriter.Write(reportContent);
        strwriter.Close();
        Logger.Info("Created rejectionMemo Html Reort");
      }
    }
  }
}

