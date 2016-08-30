using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Cargo.Base;
using Iata.IS.Model.Cargo.Enums;

namespace Iata.IS.Model.Cargo
{
  public class CargoBillingMemoAwb : AWBBase
  {
    public double? BilledWeightCharge { get; set; }

    public double? BilledValuationCharge { get; set; }

    public double BilledOtherCharge { get; set; }

    public double BilledAmtSubToIsc { get; set; }

    public double BilledIscPercentage { get; set; }

    public double BilledIscAmount { get; set; }

    public double? BilledVatAmount { get; set; }

    public double TotalAmount { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }

    public int? BilledWeight { get; set; }

    public string ProvisionalReqSpa { get; set; }

    public int? PrpratePercentage { get; set; }

    public string PartShipmentIndicator { get; set; }

    public string KgLbIndicator { get; set; }

    public bool CcaIndicator { get; set; }

    public string OurReference { get; set; }

    public CargoBillingMemo BillingMemoRecord { get; set; }

    public Guid BillingMemoId { get; set; }

    public List<BMAwbOtherCharge> OtherCharges { get; private set; }

    public List<BMAwbProrateLadderDetail> ProrateLadder { get; set; }

    public List<BMAwbVat> AwbVat { get; set; }

    public List<BMAwbAttachment> Attachments { get; set; }

    public CargoBillingMemoAwb()
    {
      OtherCharges = new List<BMAwbOtherCharge>();
      ProrateLadder = new List<BMAwbProrateLadderDetail>();
      AwbVat = new List<BMAwbVat>();
      Attachments = new List<BMAwbAttachment>();
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
            return Convert.ToDouble(OtherCharges.Sum(oc => oc.OtherChargeVatCalculatedAmount));
        }
    }

    public int OtherChargeCount
    {
        get
        {
            return Convert.ToInt32(OtherCharges.Count());
        }
    }

  }
}
