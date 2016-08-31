using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class FormCRecordModelBinder : DefaultModelBinder
  {
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var formCModel = model as SamplingFormCRecord;
      
      if (formCModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
       
        string id = string.Empty;
        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          formCModel.Attachments.Add(new SamplingFormCRecordAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          formCModel.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }
      }

      return model;
    }
  }
}
