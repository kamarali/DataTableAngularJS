using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{

  public class CargoCorrespondenceModelBinder : DefaultModelBinder
  {
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var cgoCorrespondenceModel = model as CargoCorrespondence;
    
      // Make sure we have a invoice instance.
      if (cgoCorrespondenceModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        string id = string.Empty;
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;

          cgoCorrespondenceModel.Attachments.Add(new CargoCorrespondenceAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }
      }

      return model;
    }
  }
}