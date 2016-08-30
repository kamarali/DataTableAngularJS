using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Core;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class FormDModelBinder : DefaultModelBinder
  {
    public const string TaxCode = "TaxCode";
    public const string VatId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";

    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var formD = model as SamplingFormDRecord;

      if (formD != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        var taxCodes = form.AllKeys.Where(a => a.Contains(TaxCode));
        string id = string.Empty;
        foreach (string code in taxCodes)
        {
          id = code.Substring(TaxCode.Length, code.Length - TaxCode.Length);

          if (string.IsNullOrEmpty(id))
            continue;
          formD.TaxBreakdown.Add(new SamplingFormDTax()
          {
            TaxCode = form[string.Format("TaxCode{0}", id)],
            Amount = Convert.ToDouble(form[string.Format("Amount{0}", id)]),
            Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid())
          });
        }

        var vatIds = form.AllKeys.Where(a => a.Contains(VatId));
        foreach (string code in vatIds)
        {
          id = code.Substring(VatId.Length, code.Length - VatId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          formD.VatBreakdown.Add(new SamplingFormDVat()
          {
            VatIdentifierId = Int32.Parse(form[string.Format("VatIdentifierId{0}", id)]),
            VatBaseAmount = Double.Parse(form[string.Format("VatBaseAmount{0}", id)]),
            VatLabel = (form[string.Format("VatLabel{0}", id)]),
            VatText = (form[string.Format("VatText{0}", id)]),
            //VatCalculatedAmount = Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
            VatPercentage = Double.Parse(form[string.Format("VatPercentage{0}", id)]),
            VatCalculatedAmount = Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
            Id = (form[string.Format("VatId{0}", id)] != string.Empty ? form[string.Format("VatId{0}", id)].ToGuid() : new Guid())
          });
        }

        var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        foreach (string code in attachmentIds)
        {
          id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          formD.Attachments.Add(new SamplingFormDRecordAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          formD.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }

        if (!string.IsNullOrEmpty(formD.ProrateSlipDetails))
        {
          formD.ProrateSlipDetails = ModelBinderHelper.PadProrateSlip(formD.ProrateSlipDetails);
        }
      }
      return model;
    }
  }
}
