using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class CMCouponModelBinder : DefaultModelBinder 
  {
    public const string TaxCode = "TaxCode";
    public const string VatId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var couponModel = model as CMCoupon;

      // Make sure we have a coupon instance.
      if (couponModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        couponModel.FromAirportOfCoupon = !string.IsNullOrEmpty(couponModel.FromAirportOfCoupon) ? couponModel.FromAirportOfCoupon.ToUpper() : null;
        couponModel.ToAirportOfCoupon = !string.IsNullOrEmpty(couponModel.ToAirportOfCoupon) ? couponModel.ToAirportOfCoupon.ToUpper() : null;
        
        //Get Flight day and month from Flight Date field.
        //if (!string.IsNullOrEmpty(form[ControlIdConstants.FlightDate]) && form[ControlIdConstants.FlightDate].Contains("-"))
        //{
        //  var flightDateTokens = form[ControlIdConstants.FlightDate].Split('-');
        //  couponModel.FlightDay = Convert.ToInt32(flightDateTokens[0]);
        //  couponModel.FlightMonth = Convert.ToInt32(flightDateTokens[1]);
        //}

        var taxCodes = form.AllKeys.Where(a => a.Contains(TaxCode));
        string id = string.Empty;
        foreach (string code in taxCodes)
        {
          id = code.Substring(TaxCode.Length, code.Length - TaxCode.Length);

          if (string.IsNullOrEmpty(id))
            continue;
          couponModel.TaxBreakdown.Add(new CMCouponTax()
          {
            TaxCode = form[string.Format("TaxCode{0}", id)].ToUpper(),
            Amount = Convert.ToDouble(form[string.Format("Amount{0}", id)]),
            Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid())
          });
        }

        var vatIds = form.AllKeys.Where(a => a.Contains(VatId));
        foreach (string code in vatIds)
        {
          id = code.Substring(VatId.Length, code.Length - VatId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          couponModel.VatBreakdown.Add(new CMCouponVat()
          {
            VatIdentifierId = Int32.Parse(form[string.Format("VatIdentifierId{0}", id)]),
            VatBaseAmount = Double.Parse(form[string.Format("VatBaseAmount{0}", id)]),
            VatLabel = (form[string.Format("VatLabel{0}", id)]),
            VatText = (form[string.Format("VatText{0}", id)]),
            VatCalculatedAmount = Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
            VatPercentage = Double.Parse(form[string.Format("VatPercentage{0}", id)]),
            Id = (form[string.Format("VatId{0}", id)] != string.Empty ? form[string.Format("VatId{0}", id)].ToGuid() : new Guid())
          });
        }

        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          couponModel.Attachments.Add(new CMCouponAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          couponModel.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }

        if (!string.IsNullOrEmpty(couponModel.ProrateSlipDetails))
        {
          couponModel.ProrateSlipDetails = ModelBinderHelper.PadProrateSlip(couponModel.ProrateSlipDetails);
        }

        couponModel.CabinClass = string.IsNullOrEmpty(couponModel.CabinClass) ? couponModel.CabinClass : couponModel.CabinClass.ToUpper();
      }


      return model;
    }
  }
}
