using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Cargo.Base;
using Iata.IS.Model.Cargo.Enums;

namespace Iata.IS.Model.Cargo
{
  public class CMAirWayBill : AWBBase 
  {   
    public double? CreditedWeightCharge { get; set; }

    public double? CreditedValuationCharge { get; set; }

    public double CreditedOtherCharge { get; set; }

    public double CreditedAmtSubToIsc { get; set; }

    public double CreditedIscPercentage { get; set; }

    public double CreditedIscAmount { get; set; }

    public double? CreditedVatAmount { get; set; }

    public double TotalAmountCredited { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }

    public int? BilledWeight { get; set; }

    public string ProvisionalReqSpa { get; set; }

    public int? ProratePercentage { get; set; }

    public string PartShipmentIndicator { get; set; }

    public string KgLbIndicator { get; set; }

    public bool CcaIndicator { get; set; }

    public string OurReference { get; set; }

    public CargoCreditMemo CreditMemoRecord { get; set; }

    public Guid CreditMemoId { get; set; }

    public List<CMAwbOtherCharge> CMAwbOtherCharges { get; private set; }

    public List<CMAwbProrateLadderDetail> CMAwbProrateLadder { get; set; }

    public List<CMAwbVat> CMAwbVatBreakdown { get; set; }

    public List<CMAwbAttachment> Attachments { get; set; }

    public CMAirWayBill()
    {
      Attachments = new List<CMAwbAttachment>();
      CMAwbVatBreakdown = new List<CMAwbVat>();
      CMAwbOtherCharges = new List<CMAwbOtherCharge>();
      CMAwbProrateLadder = new List<CMAwbProrateLadderDetail>();       
    }

    public string AwbBillingCodeDisplay
    {
        get
        {
            return ((BillingCode)AwbBillingCode).ToString();
        }
    }

    public double OtherChargeVatSumAmount
    {
        get
        {
          return Convert.ToDouble(CMAwbOtherCharges.Sum(oc => oc.OtherChargeVatCalculatedAmount));
        }
    }

    public int OtherChargeCount
    {
        get
        {
          return Convert.ToInt32(CMAwbOtherCharges.Count());
        }
    }
   
  }
}
