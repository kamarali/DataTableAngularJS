using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class RMModelBinder : DefaultModelBinder
  {
    public const string VatIdentifierId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var rejectionMemoRecord = model as RejectionMemo;

      if (rejectionMemoRecord != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        string id = string.Empty;
        var vatIds = form.AllKeys.Where(a => a.Contains(VatIdentifierId));

        foreach (string code in vatIds)
        {
          id = code.Substring(VatIdentifierId.Length, code.Length - VatIdentifierId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          rejectionMemoRecord.RejectionMemoVat.Add(new RejectionMemoVat()
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
          rejectionMemoRecord.Attachments.Add(new RejectionMemoAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          rejectionMemoRecord.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }
          //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlenght for field.
        if (!string.IsNullOrEmpty(rejectionMemoRecord.ReasonRemarks))
        {
            rejectionMemoRecord.ReasonRemarks = rejectionMemoRecord.ReasonRemarks.Replace("\r\n", "\n");
        }
      }
      return model;
    }
  }
}
