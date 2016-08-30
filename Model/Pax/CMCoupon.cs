using System;
using System.Collections.Generic;
using System.Linq;

namespace Iata.IS.Model.Pax
{
  public class CMCoupon : MemoCouponBase
  {

    public CMCoupon()
    {
      Attachments = new List<CMCouponAttachment>();
      TaxBreakdown = new List<CMCouponTax>();
      VatBreakdown = new List<CMCouponVat>();
    }

    public double VatAmount { get; set; }

    public double TaxAmount { get; set; }

    public decimal GrossAmountCredited { get; set; }

    public double NetAmountCredited { get; set; }

    public double IscAmountBilled { get; set; }

    public double IscPercent { get; set; }

    public double OtherCommissionBilled { get; set; }

    public double OtherCommissionPercent { get; set; }

    public double UatpAmountBilled { get; set; }

    public double UatpPercent { get; set; }

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

    public string ReasonCode { get; set; }

    public CreditMemo CreditMemoRecord { get; set; }

    public Guid CreditMemoId { get; set; }

    public List<CMCouponAttachment> Attachments { get; set; }

    public List<CMCouponTax> TaxBreakdown { get; private set; }

    public List<CMCouponVat> VatBreakdown { get; private set; }

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

    /// <summary>
    /// Gets the prorate slip details for memo reports
    /// Need to add break tag for  80 length wrap functionality on report .
    /// </summary>
    /// <value>The prorate slip details report.</value>
    public string ProrateSlipDetailsReport { get; set; }
   //{
   //   get
   //   {
   //     var nBreaks = ProrateSlipDetails.Length / 78;
   //     var prorateSlipDetailsReport = string.Empty;
   //     while (nBreaks > 0)
   //     {
   //       prorateSlipDetailsReport = ProrateSlipDetails.Insert(78 * nBreaks, "<br>");
   //     }
   //     return prorateSlipDetailsReport;
   //   }  
   // }
  }
}
