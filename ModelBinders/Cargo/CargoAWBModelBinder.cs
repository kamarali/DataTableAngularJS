using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Core;
namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
  public class CargoAWBModelBinder: DefaultModelBinder
  {
    public const string VatIdentifierId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public const string OtherChargeId = "OtherChargeId";
    public const string TaxCode = "TaxCode";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var awbRecord = model as AwbRecord;

      if (awbRecord != null)
      {
        var form = controllerContext.HttpContext.Request.Form;
        string id = string.Empty;
        var vatIds = form.AllKeys.Where(a => a.Contains(VatIdentifierId));

        foreach (string code in vatIds)
        {
          id = code.Substring(VatIdentifierId.Length, code.Length - VatIdentifierId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          awbRecord.VatBreakdown.Add(new AwbVat()
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
          awbRecord.Attachments.Add(new AwbAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        var taxCodes = form.AllKeys.Where(a => a.Contains(TaxCode));
         id = string.Empty;
        foreach (string code in taxCodes)
        {
          id = code.Substring(TaxCode.Length, code.Length - TaxCode.Length);

          if (string.IsNullOrEmpty(id))
            continue;
          awbRecord.TaxBreakdown.Add(new AwbTax()
          {
            TaxCode = form[string.Format("TaxCode{0}", id)],
            Amount = Convert.ToDouble(form[string.Format("Amount{0}", id)]),
            Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid())
          });
        }
        var otherchargeIds = form.AllKeys.Where(a => a.Contains(OtherChargeId));
        foreach (string otherchar in otherchargeIds)
        {
          id = otherchar.Substring(OtherChargeId.Length, otherchar.Length - OtherChargeId.Length);
          if (string.IsNullOrEmpty(id))
            continue;
          awbRecord.OtherChargeBreakdown.Add(new AwbOtherCharge()
          {
            OtherChargeCode = (form[string.Format("OtherChargeCode{0}", id)]),
            OtherChargeCodeValue = Double.Parse(form[string.Format("OtherChargeCodeValue{0}", id)]),
            OtherChargeVatBaseAmount = string.IsNullOrEmpty(form[string.Format("OtherChargeVatBaseAmount{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("OtherChargeVatBaseAmount{0}", id)]),
            OtherChargeVatLabel = string.IsNullOrEmpty(form[string.Format("OtherChargeVatLabel{0}", id)]) ? null : form[string.Format("OtherChargeVatLabel{0}", id)],
            OtherChargeVatText = string.IsNullOrEmpty(form[string.Format("OtherChargeVatText{0}", id)]) ? null : form[string.Format("OtherChargeVatText{0}", id)],
            OtherChargeVatCalculatedAmount = string.IsNullOrEmpty(form[string.Format("OtherChargeVatCalculatedAmount{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("OtherChargeVatCalculatedAmount{0}", id)]),
            OtherChargeVatPercentage = string.IsNullOrEmpty(form[string.Format("OtherChargeVatPercentage{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("OtherChargeVatPercentage{0}", id)]),
            Id = (form[string.Format("OtherChargeId{0}", id)] != string.Empty ? form[string.Format("OtherChargeId{0}", id)].ToGuid() : new Guid())
          });
        }

        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          awbRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
        }
        //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlenght for field.
        //if (!string.IsNullOrEmpty(awbRecord.ReasonRemarks))
        //{
        //    awbRecord.ReasonRemarks = awbRecord.ReasonRemarks.Replace("\r\n", "\n");
        //}
        //model = ModelBinderHelper.SplitAwbSerialNumber(awbRecord) as AwbRecord;

        string awbSerialNumberCheckDigit = form["AwbSerialNumber"];
        model = ModelBinderHelper.SplitAwbSerialNumber(awbRecord, awbSerialNumberCheckDigit) as AwbRecord;
      }
      return model;
    }
  }
}
