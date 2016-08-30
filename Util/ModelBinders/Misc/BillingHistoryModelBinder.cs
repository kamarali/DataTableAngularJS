using System;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp.BillingHistory;

namespace Iata.IS.Web.Util.ModelBinders.Misc
{
  public class BillingHistoryModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var invoiceModel = model as InvoiceSearchCriteria;

      // Make sure we have a invoice instance.
      if (invoiceModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        if (form[ControlIdConstants.BillingYearMonthDropDown] != null)
        {
          var billingPeriodTokens = form[ControlIdConstants.BillingYearMonthDropDown].Split('-');

          if (billingPeriodTokens.Length == 2)
          {
            invoiceModel.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
            invoiceModel.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
          }
        }

      }

      return model;
    }
  }
}