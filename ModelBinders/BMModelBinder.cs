using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.Pax;
using Iata.IS.Core;
namespace Iata.IS.Web.Util.ModelBinders
{
  public class BMModelBinder: DefaultModelBinder
  {
    public const string VatIdentifierId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var billingMemoRecord = model as BillingMemo;

      if (billingMemoRecord != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        string id = string.Empty;
        var vatIds = form.AllKeys.Where(a => a.Contains(VatIdentifierId));

        foreach (string code in vatIds)
        {
          id = code.Substring(VatIdentifierId.Length, code.Length - VatIdentifierId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          billingMemoRecord.VatBreakdown.Add(new BillingMemoVat()
          {
            VatIdentifierId = Int32.Parse(form[string.Format("VatIdentifierId{0}", id)]),
            VatBaseAmount = string.IsNullOrEmpty(form[string.Format("VatBaseAmount{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatBaseAmount{0}", id)]),
            VatLabel = (form[string.Format("VatLabel{0}", id)]),
            VatText = (form[string.Format("VatText{0}", id)]),
            VatCalculatedAmount = string.IsNullOrEmpty(form[string.Format("VatCalculatedAmount{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
            VatPercentage = string.IsNullOrEmpty(form[string.Format("VatPercentage{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatPercentage{0}", id)]),
            Id = (form[string.Format("VatId{0}", id)] != string.Empty ? form[string.Format("VatId{0}", id)].ToGuid() : new Guid())
          });
        }

        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          billingMemoRecord.Attachments.Add(new BillingMemoAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          billingMemoRecord.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }
        //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlenght for field.
        if(!string.IsNullOrEmpty(billingMemoRecord.ReasonRemarks))
        {
            billingMemoRecord.ReasonRemarks = billingMemoRecord.ReasonRemarks.Replace("\r\n", "\n");
        }
      }
      return model;
    }
  }
}
