using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.MiscUatp;

namespace Iata.IS.Web.Util.ModelBinders.Misc
{

  public class CorrespondenceModelBinder : DefaultModelBinder
  {
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var mscCorrespondenceModel = model as MiscCorrespondence;
    
      // Make sure we have a invoice instance.
      if (mscCorrespondenceModel != null)
      {
        mscCorrespondenceModel.Attachments = new List<MiscUatpCorrespondenceAttachment>();
        var form = controllerContext.HttpContext.Request.Form;
        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        string id = string.Empty;
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;

          mscCorrespondenceModel.Attachments.Add(new MiscUatpCorrespondenceAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }
      }

      return model;
    }
  }
}