using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Business.Cargo.Impl
{
  public class CgoRmBmCmDetailReportUtility
  {
    public double BmAwbOtherChargeSum(CargoBillingMemoAwb awb)
    {
      return awb.OtherCharges.Sum(oc => oc.OtherChargeVatCalculatedAmount.HasValue ? oc.OtherChargeVatCalculatedAmount.Value : 0);

    }
    public double BmAwbProrateLadderDetailSum(CargoBillingMemoAwb awb)
    {
      return awb.ProrateLadder.Sum(pl => pl.Amount.HasValue ? pl.Amount.Value : 0);


    }
    public double BmAwbVatSum(CargoBillingMemoAwb awb)
    {

      return awb.AwbVat.Sum(vt => vt.VatCalculatedAmount);

    }

    public double BmVatSum(CargoBillingMemo bm)
    {
      return bm.BillingMemoVat.Sum(vt => vt.VatCalculatedAmount);
    }


    public string AwbDateString(DateTime dateTime)
    {
      return dateTime.ToString("dd MMM yyyy");
    }

    public double CmVatSum(CargoCreditMemo cm)
    {
      return cm.VatBreakdown.Sum(vt => vt.VatCalculatedAmount);
    }
    public double CmAwbOtherChargeSum(CMAirWayBill awb)
    {
      return awb.CMAwbOtherCharges.Sum(oc => oc.OtherChargeVatCalculatedAmount.HasValue ? oc.OtherChargeVatCalculatedAmount.Value : 0);

    }
    public double CmAwbVatSum(CMAirWayBill awb)
    {

      return awb.CMAwbVatBreakdown.Sum(vt => vt.VatCalculatedAmount);

    }
    public double CmAwbProrateLadderDetailSum(CMAirWayBill awb)
    {
      return awb.CMAwbProrateLadder.Sum(pl => pl.Amount.HasValue ? pl.Amount.Value : 0);


    }

    public double RmVatSum(CargoRejectionMemo bm)
    {
      return bm.RejectionMemoVat.Sum(vt => vt.VatCalculatedAmount);
    }
    public double RmAwbOtherChargeSum(RMAwb awb)
    {
      return awb.OtherCharge.Sum(oc => oc.OtherChargeVatCalculatedAmount.HasValue ? oc.OtherChargeVatCalculatedAmount.Value : 0);

    }
    public double RmAwbVatSum(RMAwb awb)
    {

      return awb.AwbVat.Sum(vt => vt.VatCalculatedAmount);
    }

    public double RmAwbProrateLadderDetailSum(RMAwb awb)
    {
      return awb.ProrateLadder.Sum(pl => pl.Amount.HasValue ? pl.Amount.Value : 0);


    }
  }
}
