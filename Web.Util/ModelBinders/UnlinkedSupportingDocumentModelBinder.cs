using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.SupportingDocuments;

namespace Iata.IS.Web.Util.ModelBinders
{
    public class UnlinkedSupportingDocumentModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);

            var invoiceModel = model as UnlinkedSupportingDocumentEx;

            // Make sure we have a invoice instance.
            if (invoiceModel != null)
            {
                var form = controllerContext.HttpContext.Request.Form;

                if (form[ControlIdConstants.SupportingDocumentBillingYearMonth] != null)
                {
                    var billingPeriodTokens = form[ControlIdConstants.SupportingDocumentBillingYearMonth].Split('-');

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
