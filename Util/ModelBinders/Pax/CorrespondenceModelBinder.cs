using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;

namespace Iata.IS.Web.Util.ModelBinders.Pax
{

  public class CorrespondenceModelBinder : DefaultModelBinder
  {
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var mscCorrespondenceModel = model as Correspondence;
    
      // Make sure we have a invoice instance.
      if (mscCorrespondenceModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        string id = string.Empty;
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;

          mscCorrespondenceModel.Attachments.Add(new CorrespondenceAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }
      }

      return model;
    }
  }
}