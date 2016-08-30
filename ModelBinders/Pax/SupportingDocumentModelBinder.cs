using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.SupportingDocuments;

namespace Iata.IS.Web.Util.ModelBinders.Pax
{
  public class SupportingDocumentModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var searchModel = model as SupportingDocSearchCriteria;

      // Make sure we have a invoice instance.
      if (searchModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        if (form != null)
        {
          if (form[ControlIdConstants.BillingYearMonthDropDown] != null)
          {
            var billingPeriodTokens = form[ControlIdConstants.BillingYearMonthDropDown].Split('-');

            if (billingPeriodTokens.Length == 2)
            {
              searchModel.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
              searchModel.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
            }
          }
        }
      }

      return model;
    }
  }
}
