using System;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{

  public class InvoiceSearchModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var searchModel = model as SearchCriteria;

      // Make sure we have a invoice instance.
      if (searchModel != null)
      {
        if (controllerContext.HttpContext.Request.QueryString[ControlIdConstants.BillingYearMonthDropDown] != null)
        {
          var billingPeriodTokens = controllerContext.HttpContext.Request.QueryString[ControlIdConstants.BillingYearMonthDropDown].Split('-');

          if (billingPeriodTokens.Length == 2)
          {
            searchModel.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
            searchModel.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
          }
        }
      }

      return model;
    }
  }
}