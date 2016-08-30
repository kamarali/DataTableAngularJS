using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
  public class CargoCreditMemoModelBinder : DefaultModelBinder
  {
    public const string VatId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var creditMemoRecord = model as CargoCreditMemo;
      
      if (creditMemoRecord != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        var vatIds = form.AllKeys.Where(a => a.Contains(VatId));
        string id = string.Empty;

        foreach (string code in vatIds)
        {
          id = code.Substring(VatId.Length, code.Length - VatId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          creditMemoRecord.VatBreakdown.Add(new CargoCreditMemoVat()
          {
              /* SCP# 391037 - Clarification required to send Reverse VAT in Cargo IS IDEC file.
                 Desc: Allowing values 0 as valid input. */
            VatIdentifierId = Int32.Parse(form[string.Format("VatIdentifierId{0}", id)]),
            VatBaseAmount = string.IsNullOrEmpty(form[string.Format("VatBaseAmount{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatBaseAmount{0}", id)]),
            VatLabel = (form[string.Format("VatLabel{0}", id)]),
            VatText = (form[string.Format("VatText{0}", id)]),
            VatPercentage = string.IsNullOrEmpty(form[string.Format("VatPercentage{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatPercentage{0}", id)]),
            VatCalculatedAmount = string.IsNullOrEmpty(form[string.Format("VatCalculatedAmount{0}", id)]) ? 0 : Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
            Id = (form[string.Format("VatId{0}", id)] != string.Empty ? form[string.Format("VatId{0}", id)].ToGuid() : new Guid())
          });
        }

        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          creditMemoRecord.Attachments.Add(new CargoCreditMemoAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          creditMemoRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
        }
        //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlenght for field.
        if (!string.IsNullOrEmpty(creditMemoRecord.ReasonRemarks))
        {
            creditMemoRecord.ReasonRemarks = creditMemoRecord.ReasonRemarks.Replace("\r\n", "\n");
        }
      }

      return model;
    }
  }
}
