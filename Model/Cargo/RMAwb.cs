using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo
{
  public class RMAwb : AWBBase
  {
    public double? BilledWeightCharge { get; set; }

    public double? AcceptedWeightCharge { get; set; }

    public double? WeightChargeDiff { get; set; }

    public double? BilledValuationCharge { get; set; }

    public double? AcceptedValuationCharge { get; set; }

    public double? ValuationChargeDiff { get; set; }

    public double BilledOtherCharge { get; set; }

    public double AcceptedOtherCharge { get; set; }

    public double OtherChargeDiff { get; set; }

    public double AllowedAmtSubToIsc { get; set; }

    public double AcceptedAmtSubToIsc { get; set; }

    public double AllowedIscPercentage { get; set; }

    public double AcceptedIscPercentage { get; set; }

    public double AllowedIscAmount { get; set; }

    public double AcceptedIscAmount { get; set; }

    public double IscAmountDifference { get; set; }

    public double? BilledVatAmount { get; set; }

    public double? AcceptedVatAmount { get; set; }

    public double? VatAmountDifference { get; set; }

    public double NetRejectAmount { get; set; }

    public string CurrencyAdjustmentIndicator { get; set; }

    public int? BilledWeight { get; set; }

    public string ProvisionalReqSpa { get; set; }

    public int? ProratePercentage { get; set; }

    public string PartShipmentIndicator { get; set; }

    public string KgLbIndicator { get; set; }

    public bool CcaIndicator { get; set; }

    public string OurReference { get; set; }

    public long NumberOfChildRecords { get; set; }

    public CargoRejectionMemo RejectionMemoRecord { get; set; }

    public Guid RejectionMemoId { get; set; }

    public List<RMAwbOtherCharge> OtherCharge { get; private set; }

    public List<RMAwbProrateLadderDetail> ProrateLadder { get; set; }

    public List<RMAwbVat> AwbVat { get; set; }

    public List<RMAwbAttachment> Attachments { get; set; }

    public double OtherChargeVatSumAmount
    {
      get
      {
        return Convert.ToDouble(OtherCharge.Sum(oc => oc.OtherChargeVatCalculatedAmount));
      }
    }

    public RMAwb()
    {
      OtherCharge = new List<RMAwbOtherCharge>();
      ProrateLadder = new List<RMAwbProrateLadderDetail>();
      AwbVat = new List<RMAwbVat>();
      Attachments = new List<RMAwbAttachment>();
    }
  }
}
