using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
  public class CargoRMModelBinder : DefaultModelBinder
  {
    public const string VatIdentifierId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var rejectionMemoRecord = model as CargoRejectionMemo;

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
          rejectionMemoRecord.RejectionMemoVat.Add(new CgoRejectionMemoVat
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
          rejectionMemoRecord.Attachments.Add(new CgoRejectionMemoAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          rejectionMemoRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
        }
          //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlenght for field.
        if (!string.IsNullOrEmpty(rejectionMemoRecord.ReasonRemarks))
        {
          rejectionMemoRecord.ReasonRemarks = rejectionMemoRecord.ReasonRemarks.Replace("\r\n", "\n");
            /* CMP #671: Validation of PAX CGO Stage 2 & 3 Rejection Memo Reason Text 
             * Desc: Removed below code. It use to RPad every line in ReasonRemarks field so as to make its length as 80 chars.
             * This used to cause problem for CMP#671 Validations.
             * E.g. -> User input is "1\n\n2\n3", then it will get converted to "1<79 spaces>\n\n2<79 spaces>\n3<79 spaces>"
             * So input text length = 6 will get converted to 243 chars causing the problem.
             */
            //rejectionMemoRecord.ReasonRemarks = ModelBinderHelper.PadProrateSlip(rejectionMemoRecord.ReasonRemarks);
        }
      }
      return model;
    }
  }
}
