using System;
using System.Collections.Generic;
using System.Linq;

namespace Iata.IS.Model.Pax
{
  public class BMCoupon : MemoCouponBase
  {

    public BMCoupon()
    {
      TaxBreakdown = new List<BMCouponTax>();
      VatBreakdown = new List<BMCouponVat>();
      Attachments = new List<BMCouponAttachment>();
    }

    /// <summary>
    /// 
    /// </summary>
    public double VatAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double TaxAmount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public decimal GrossAmountBilled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double NetAmountBilled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double IscAmountBilled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double IscPercent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double OtherCommissionBilled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double OtherCommissionPercent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double UatpAmountBilled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double UatpPercent { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public double HandlingFeeAmount { get; set; }

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
    /// Gets the prorate slip details for memo reports
    /// Need to add break tag for  80 length wrap functionality on report .
    /// </summary>
    /// <value>The prorate slip details report.</value>
    public string ProrateSlipDetailsReport { get; set; }
    
      //get
      //{
      //  var nBreaks = ProrateSlipDetails.Length / 78;
      //  var prorateSlipDetailsReport = string.Empty;
      //  while (nBreaks > 0)
      //  {
      //    prorateSlipDetailsReport = ProrateSlipDetails.Insert(78 * nBreaks, "<br>");
      //  }
      //  return prorateSlipDetailsReport;
      //}
      
    


    public string ReasonCode { get; set; }

    public BillingMemo BillingMemo { get; set; }

    public Guid BillingMemoId { get; set; }

    public List<BMCouponAttachment> Attachments { get; set; }

    public List<BMCouponTax> TaxBreakdown { get; private set; }

    public List<BMCouponVat> VatBreakdown { get; private set; }

    /// <summary>
    /// Gets the total tax amount diff report.
    /// </summary>
    /// <value>The total tax amount diff report.</value>
    public double TotalTaxAmountReport
    {
      get
      {
        return (TaxBreakdown.Sum(rec => rec.Amount));
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

  }
}
