using System;
using System.Collections.Generic;
using System.Linq;

namespace Iata.IS.Model.Pax
{
  public class RMCoupon : CouponBase
  {
    public double GrossAmountAccepted { get; set; }

    public int SerialNo { get; set; }

    public double GrossAmountBilled { get; set; }

    public double GrossAmountDifference { get; set; }

    public double AllowedIscAmount { get; set; }

    public double AllowedIscPercentage { get; set; }

    public double AcceptedIscAmount { get; set; }

    public double AcceptedIscPercentage { get; set; }

    public double IscDifference { get; set; }

    public double AllowedUatpPercentage { get; set; }

    public double AllowedUatpAmount { get; set; }

    public double AcceptedUatpPercentage { get; set; }

    public double AcceptedUatpAmount { get; set; }

    public double UatpDifference { get; set; }

    public double AllowedHandlingFee { get; set; }

    public double AcceptedHandlingFee { get; set; }

    public double HandlingDifference { get; set; }

    public double AllowedOtherCommission { get; set; }

    public double AllowedOtherCommissionPercentage { get; set; }

    public double AcceptedOtherCommission { get; set; }

    public double AcceptedOtherCommissionPercentage { get; set; }

    public double OtherCommissionDifference { get; set; }

    public double TaxAmountBilled { get; set; }

    public double TaxAmountAccepted { get; set; }

    public double TaxAmountDifference { get; set; }

    public double VatAmountBilled { get; set; }

    public double VatAmountAccepted { get; set; }

    public double VatAmountDifference { get; set; }

    public double NetRejectAmount { get; set; }

    public string ReasonCode { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField1 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField2 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField3 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField4 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string ReferenceField5 { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string AirlineOwnUse { get; set; }

    public string ProrateSlipDetails { get; set; }

    /// <summary>
    /// Number of child records required in case of IDEC validations.
    /// </summary>
    public long NumberOfChildRecords { get; set; }

    public RejectionMemo RejectionMemoRecord { get; set; }

    public Guid RejectionMemoId { get; set; }

    public List<RMCouponTax> TaxBreakdown { get; private set; }

    public List<RMCouponVat> VatBreakdown { get; private set; }

    public List<RMCouponAttachment> Attachments { get; set; }

    /// <summary>
    /// Gets the prorate slip details for memo reports
    /// Need to add break tag for  80 length wrap functionality on report .
    /// </summary>
    /// <value>The prorate slip details report.</value>
    public string ProrateSlipDetailsReport{ get; set; }
    //{
      //get
      //{
      //   var prorateSlipDetailsReport = string.Empty;
      //   if (ProrateSlipDetails != null)
      //   {
      //     prorateSlipDetailsReport = ProrateSlipDetails.Replace(Environment.NewLine, string.Empty);
      //     var nBreaks = ProrateSlipDetails.Length / 80;
      //     var nPadCharCount = 0;
      //     var nCount = 1;
      //     while (nBreaks >= nCount)
      //     {

      //       prorateSlipDetailsReport = prorateSlipDetailsReport.Insert((80 * nCount) + nPadCharCount, "<br>");
      //       nPadCharCount += 3;
      //       nBreaks--;
      //       nCount++;
      //     }
      //   }
      //  return prorateSlipDetailsReport;
      //}
    //}

    /// <summary>
    /// Gets the total tax amount diff report.
    /// </summary>
    /// <value>The total tax amount diff report.</value>
    public double TotalTaxAmountDiffReport 
    { get
      {
        return (TaxBreakdown.Sum(rec => rec.AmountDifference));
      }
     }

    /// <summary>
    /// Gets the total vat amount report.
    /// </summary>
    /// <value>The total vat amount report.</value>
    public double TotalVatAmountReport
    {
      get
      {
        return (VatBreakdown.Sum(rec => rec.VatCalculatedAmount));
      }
    }
    
    public RMCoupon()
    {
      Attachments = new List<RMCouponAttachment>();
      TaxBreakdown = new List<RMCouponTax>();
      VatBreakdown = new List<RMCouponVat>();
    }
  }
}
