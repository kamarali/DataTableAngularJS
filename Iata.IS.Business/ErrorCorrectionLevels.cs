using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business
{
  public static class ErrorCorrectionLevels
  {
    public static Dictionary<string, string> Levels;
      public static Dictionary<string, string> LevelsDisplay;

    static ErrorCorrectionLevels()
    {
      Levels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
      Levels.Add(ErrorLevels.ErrorLevelAirwayBill, "Awb");
      Levels.Add(ErrorLevels.ErrorLevelAirwayBillOtherCharge, "AwbOc");
      Levels.Add(ErrorLevels.ErrorLevelAirwayBillVat, "AwbVat");
      Levels.Add(ErrorLevels.ErrorLevelBillingCodeSubTotal, "BillingCodeSubTotal");
      Levels.Add(ErrorLevels.ErrorLevelBillingCodeSubTotalVat, "BillingCodeSubTotalVat");
      Levels.Add(ErrorLevels.ErrorLevelBillingMemo, "BillingMemo");
      Levels.Add(ErrorLevels.ErrorLevelBillingMemoCoupon, "BmCoupon");
      Levels.Add(ErrorLevels.ErrorLevelBillingMemoCouponTax, "BmCouponTax");
      Levels.Add(ErrorLevels.ErrorLevelBillingMemoCouponVat, "BmCouponVat");
      Levels.Add(ErrorLevels.ErrorLevelBillingMemoVat, "BmVat");
      Levels.Add(ErrorLevels.ErrorLevelBmAirwayBill, "BmAwb");
      Levels.Add(ErrorLevels.ErrorLevelBmAirwayBillOtherCharge, "BmAwbOc");
      Levels.Add(ErrorLevels.ErrorLevelBmAirwayBillProrateLadder, "BmAwbPr");
      Levels.Add(ErrorLevels.ErrorLevelBmAirwayBillVat, "BmAwbVat");
      Levels.Add(ErrorLevels.ErrorLevelCmAirwayBill, "CmAwb");
      Levels.Add(ErrorLevels.ErrorLevelCmAirwayBillOtherCharge, "CmAwbOc");
      Levels.Add(ErrorLevels.ErrorLevelCmAirwayBillProrateLadder, "CmAwbPr");
      Levels.Add(ErrorLevels.ErrorLevelCmAirwayBillVat, "CmAwbVat");
      Levels.Add(ErrorLevels.ErrorLevelCoupon, "Coupon");
      Levels.Add(ErrorLevels.ErrorLevelCouponTax, "CouponTax");
      Levels.Add(ErrorLevels.ErrorLevelCouponVat, "CouponVat");
      Levels.Add(ErrorLevels.ErrorLevelCreditMemo, "CreditMemo");
      Levels.Add(ErrorLevels.ErrorLevelCreditMemoCoupon, "CmCoupon");
      Levels.Add(ErrorLevels.ErrorLevelCreditMemoCouponTax, "CmCouponTax");
      Levels.Add(ErrorLevels.ErrorLevelCreditMemoCouponVat, "CmCouponVat");
      Levels.Add(ErrorLevels.ErrorLevelCreditMemoVat, "CmVat");
      Levels.Add(ErrorLevels.ErrorLevelFileTotal, "FileTotal");
      Levels.Add(ErrorLevels.ErrorLevelInvoice, "Invoice");
      Levels.Add(ErrorLevels.ErrorLevelInvoiceFooter, "InvoiceFooter");
      Levels.Add(ErrorLevels.ErrorLevelInvoiceSummaryTax, "InvoiceSummaryTax");
      Levels.Add(ErrorLevels.ErrorLevelInvoiceTotal, "InvoiceTotal");
      Levels.Add(ErrorLevels.ErrorLevelInvoiceVat, "InvoiceVat");
      Levels.Add(ErrorLevels.ErrorLevelLineItem, "LineItem");
      Levels.Add(ErrorLevels.ErrorLevelLineItemAddOnCharge, "LineItemAddOnCharge");
      Levels.Add(ErrorLevels.ErrorLevelLineItemDetail, "LineItemDetail");
      Levels.Add(ErrorLevels.ErrorLevelLineItemDetailAddOnCharge, "LIDAddOnCharge");
      Levels.Add(ErrorLevels.ErrorLevelLineItemDetailTax, "LIDTax");
      Levels.Add(ErrorLevels.ErrorLevelLineItemTax, "LineItemTax");
      Levels.Add(ErrorLevels.ErrorLevelProrateSlip, "ProrateSlip");
      Levels.Add(ErrorLevels.ErrorLevelReasonBreakdown, "ReasonBreakdown");
      Levels.Add(ErrorLevels.ErrorLevelReferenceData1, "ReferenceData1");
      Levels.Add(ErrorLevels.ErrorLevelReferenceData2, "ReferenceData2");
      Levels.Add(ErrorLevels.ErrorLevelRejectionMemo, "RejectionMemo");
      Levels.Add(ErrorLevels.ErrorLevelRejectionMemoCoupon, "RmCoupon");
      Levels.Add(ErrorLevels.ErrorLevelRejectionMemoCouponTax, "RmCouponTax");
      Levels.Add(ErrorLevels.ErrorLevelRejectionMemoCouponVat, "RmCouponVat");
      Levels.Add(ErrorLevels.ErrorLevelRejectionMemoVat, "RmVat");
      Levels.Add(ErrorLevels.ErrorLevelRmAirwayBill, "RmAwb");
      Levels.Add(ErrorLevels.ErrorLevelRmAirwayBillOtherCharge, "RmAwbOc");
      Levels.Add(ErrorLevels.ErrorLevelRmAirwayBillProrateLadder, "RmAwbPr");
      Levels.Add(ErrorLevels.ErrorLevelRmAirwayBillVat, "RmAwbVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormC, "SamplingFormC");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormCRecord, "SamplingFormCRecord");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormD, "SamplingFormD");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormDTax, "SamplingFormDTax");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormDVat, "SamplingFormDVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormE, "SamplingFormE");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormEProvisionalInvoice, "FormEProvisionalInvoice");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormEVat, "FormEVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormF, "SamplingFormF");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormFCoupon, "FormFCoupon");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormFCouponTax, "FormFCouponTax");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormFCouponVat, "FormFCouponVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormFVat, "FormFVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormXF, "SamplingFormXF");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormXFCoupon, "FormXFCoupon");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormXFCouponTax, "FormXFCouponTax");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormXFCouponVat, "FormXFCouponVat");
      Levels.Add(ErrorLevels.ErrorLevelSamplingFormXFVat, "FormXFVat");
      Levels.Add(ErrorLevels.ErrorLevelSourceCodeTotal, "SourceCodeTotal");
      Levels.Add(ErrorLevels.ErrorLevelSourceCodeVat, "SourceCodeVat");

        LevelsDisplay=new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var key in Levels.Keys)
        {
            LevelsDisplay.Add(Levels[key],key);
            
        }
    }

    /// <summary>
    /// Gets the string for Error correction
    /// </summary>
    /// <param name="errorLevel"></param>
    /// <returns></returns>
    public static string GetErrorCorrectionLevel(string errorLevel)
    {
      if (Levels.ContainsKey(errorLevel))
        return Levels[errorLevel];
      return string.Empty;
    }
  }
}
