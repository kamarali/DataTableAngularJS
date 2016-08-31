using System;
using System.Web.Mvc;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class CouponModelBinder : DefaultModelBinder 
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var couponModel = model as MemoCouponBase;

      // Make sure we have a coupon instance.
      if (couponModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        couponModel.FromAirportOfCoupon = !string.IsNullOrEmpty(couponModel.FromAirportOfCoupon) ? couponModel.FromAirportOfCoupon.ToUpper() : null;
        couponModel.ToAirportOfCoupon = !string.IsNullOrEmpty(couponModel.ToAirportOfCoupon) ? couponModel.ToAirportOfCoupon.ToUpper() : null;


        /*if (!string.IsNullOrEmpty(form[ControlIdConstants.FlightDate]) && form[ControlIdConstants.FlightDate].Contains("-"))
        {
          var flightDateTokens = form[ControlIdConstants.FlightDate].Split('-');
          couponModel.FlightDay = Convert.ToInt32(flightDateTokens[0]);
          couponModel.FlightMonth = Convert.ToInt32(flightDateTokens[1]);
        }*/

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          couponModel.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }
      }

      return model;
    }
  }
}
