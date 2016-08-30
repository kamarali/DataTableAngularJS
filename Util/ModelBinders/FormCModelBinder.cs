using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class FormCModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var formCModel = model as SamplingFormC;
      
      if (formCModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        if (form[ControlIdConstants.ProvisionalBillingMonthDropdown] != null)
        {
          var billingYearMonthTokens = form[ControlIdConstants.ProvisionalBillingMonthDropdown].Split('-');

          if (billingYearMonthTokens.Length == 2)
          {
            formCModel.ProvisionalBillingYear = Convert.ToInt32(billingYearMonthTokens[0]);
            formCModel.ProvisionalBillingMonth = Convert.ToInt32(billingYearMonthTokens[1]);
          }
        }
      }

      return model;
    }
  }
}