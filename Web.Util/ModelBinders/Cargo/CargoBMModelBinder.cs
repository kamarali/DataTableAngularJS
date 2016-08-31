using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Core;
namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
  public class CargoBMModelBinder: DefaultModelBinder
  {
    public const string VatIdentifierId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public const string OtherChargeId = "OtherChargeId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var billingMemoRecord = model as CargoBillingMemo;

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
              billingMemoRecord.BillingMemoVat.Add(new CargoBillingMemoVat()
              {
                  /* SCP# 391037 - Clarification required to send Reverse VAT in Cargo IS IDEC file.
                    Desc: Allowing values 0 as valid input. */
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
              billingMemoRecord.Attachments.Add(new CargoBillingMemoAttachment()
              {
                  Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
              });
          }

          //var otherchargeIds = form.AllKeys.Where(a => a.Contains(OtherChargeId));
          //foreach (string otherchar in otherchargeIds)
          //{
          //    id = otherchar.Substring(OtherChargeId.Length, otherchar.Length - OtherChargeId.Length);
          //    if (string.IsNullOrEmpty(id))
          //        continue;
          //    billingMemoRecord.OtherCharges.Add(new CargoBillingMemoOtherCharge()
          //    {
          //        OtherChargeCode = (form[string.Format("OtherChargeCode{0}", id)]),
          //        OtherChargeCodeValue = Double.Parse(form[string.Format("OtherChargeCodeValue{0}", id)]),
          //        OtherChargeVatBaseAmount = Double.Parse(form[string.Format("VatBaseAmount{0}", id)]),
          //        OtherChargeVatLabel = (form[string.Format("VatLabel{0}", id)]),
          //        OtherChargeVatText = (form[string.Format("VatText{0}", id)]),
          //        OtherChargeVatCalculatedAmount = Double.Parse(form[string.Format("VatCalculatedAmount{0}", id)]),
          //        OtherChargeVatPercentage = Double.Parse(form[string.Format("VatPercentage{0}", id)]),
          //        Id = (form[string.Format("OtherChargeId{0}", id)] != string.Empty ? form[string.Format("OtherChargeId{0}", id)].ToGuid() : new Guid())
          //    });
          //}

          if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
          {
              var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
              billingMemoRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
          }
          //Remove \r from reasonremark fields, for new line character \r\n is added. So it exceeds DB maxlength for field.
          if (!string.IsNullOrEmpty(billingMemoRecord.ReasonRemarks))
          {
            billingMemoRecord.ReasonRemarks = billingMemoRecord.ReasonRemarks.Replace("\r\n", "\n");
            billingMemoRecord.ReasonRemarks = ModelBinderHelper.PadProrateSlip(billingMemoRecord.ReasonRemarks);
          }
      }
      return model;
    }
  }
}
