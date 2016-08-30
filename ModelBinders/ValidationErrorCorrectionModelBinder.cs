using System;
using System.Web.Mvc;
using Iata.IS.Model.Common;
using System.Globalization;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class ValidationErrorCorrectionModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var validationErrorCorrection = model as ValidationErrorCorrection;

      // Make sure we have a invoice instance.
      if (validationErrorCorrection != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        if (form[ControlIdConstants.BillingYearMonthDropDown] != null)
        {
          var billingPeriodTokens = form[ControlIdConstants.BillingYearMonthDropDown].Split('-');

          if (billingPeriodTokens.Length == 3)
          {
            validationErrorCorrection.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
            validationErrorCorrection.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
            //validationErrorCorrection.BillingMonth = DateTime.ParseExact(billingPeriodTokens[1], "MMM", CultureInfo.CurrentCulture).Month;
            validationErrorCorrection.BillingPeriod = Convert.ToInt32(billingPeriodTokens[2]);
          }
        }

      }

      return model;
    }
  }
}
