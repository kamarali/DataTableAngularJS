using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Core;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
    public class CargoCMAwbModelBinder : DefaultModelBinder
    {
        public const string VatIdentifierId = "VatIdentifierId";
        public const string AttachmentId = "AttachmentId";
        public const string OtherChargeId = "OtherChargeId";
        public const string ProrateLadderId = "ProrateLadderId";
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);

            var cmAwbRecord = model as CMAirWayBill;

            if (cmAwbRecord != null)
            {
                var form = controllerContext.HttpContext.Request.Form;
                string id = string.Empty;
                var vatIds = form.AllKeys.Where(a => a.Contains(VatIdentifierId));

                foreach (string code in vatIds)
                {
                    id = code.Substring(VatIdentifierId.Length, code.Length - VatIdentifierId.Length);
                    if (string.IsNullOrEmpty(id))
                        continue;
                    cmAwbRecord.CMAwbVatBreakdown.Add(new CMAwbVat()
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

                GetProrateLadderFields(cmAwbRecord, form);

                var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
                foreach (string code in attachmentIds)
                {
                    id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
                    if (string.IsNullOrEmpty(id))
                        continue;
                    cmAwbRecord.Attachments.Add(new CMAwbAttachment()
                    {
                        Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
                    });
                }

                var otherchargeIds = form.AllKeys.Where(a => a.Contains(OtherChargeId));
                foreach (string otherchar in otherchargeIds)
                {
                    id = otherchar.Substring(OtherChargeId.Length, otherchar.Length - OtherChargeId.Length);
                    if (string.IsNullOrEmpty(id))
                        continue;
                    cmAwbRecord.CMAwbOtherCharges.Add(new CMAwbOtherCharge()
                    {
                        OtherChargeCode = (form[string.Format("OtherChargeCode{0}", id)]),
                        OtherChargeCodeValue = Double.Parse(form[string.Format("OtherChargeCodeValue{0}", id)]),
                        OtherChargeVatBaseAmount = string.IsNullOrEmpty(form[string.Format("OtherChargeVatBaseAmount{0}", id)]) ? null: (double?)Convert.ToDouble(form[string.Format("OtherChargeVatBaseAmount{0}", id)]),
                        OtherChargeVatLabel = string.IsNullOrEmpty(form[string.Format("OtherChargeVatLabel{0}", id)]) ? null : form[string.Format("OtherChargeVatLabel{0}", id)],
                        OtherChargeVatText = string.IsNullOrEmpty(form[string.Format("OtherChargeVatText{0}", id)]) ? null : form[string.Format("OtherChargeVatText{0}", id)],
                        OtherChargeVatCalculatedAmount = string.IsNullOrEmpty(form[string.Format("OtherChargeVatCalculatedAmount{0}", id)]) ? null: (double?)Convert.ToDouble(form[string.Format("OtherChargeVatCalculatedAmount{0}", id)]),
                        OtherChargeVatPercentage = string.IsNullOrEmpty(form[string.Format("OtherChargeVatPercentage{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("OtherChargeVatPercentage{0}", id)]),
                        Id = (form[string.Format("OtherChargeId{0}", id)] != string.Empty ? form[string.Format("OtherChargeId{0}", id)].ToGuid() : new Guid())
                    });
                }

                if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
                {
                    var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
                    cmAwbRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
                }

                string awbSerialNumberCheckDigit = form["AwbSerialNumber"];
                model = ModelBinderHelper.SplitAwbSerialNumber(cmAwbRecord, awbSerialNumberCheckDigit) as CMAirWayBill;
            }
            return model;
        }

        private static void GetProrateLadderFields(CMAirWayBill awbRecord, NameValueCollection form)
        {
          var prorateLadderIds = form.AllKeys.Where(a => a.Contains(ProrateLadderId));
          foreach (string prorateLadder in prorateLadderIds)
          {
            string id = prorateLadder.Substring(ProrateLadderId.Length, prorateLadder.Length - ProrateLadderId.Length);
            if (string.IsNullOrEmpty(id))
              continue;
            awbRecord.CMAwbProrateLadder.Add(new CMAwbProrateLadderDetail()
            {
              FromSector = string.IsNullOrEmpty(form[string.Format("FromSector{0}", id)]) ? null : form[string.Format("FromSector{0}", id)].ToUpper(),
              ToSector = string.IsNullOrEmpty(form[string.Format("ToSector{0}", id)]) ? null : form[string.Format("ToSector{0}", id)].ToUpper(),
              CarrierPrefix = string.IsNullOrEmpty(form[string.Format("CarrierPrefix{0}", id)]) ? null : form[string.Format("CarrierPrefix{0}", id)].ToUpper(),
              ProrateFactor = string.IsNullOrEmpty(form[string.Format("ProrateFactor{0}", id)]) ? null : (int?)Convert.ToInt32(form[string.Format("ProrateFactor{0}", id)]),
              ProvisoReqSpa = form[string.Format("ProvisoReqSpa{0}", id)],
              Amount = string.IsNullOrEmpty(form[string.Format("Amount{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("Amount{0}", id)]),
              PercentShare = string.IsNullOrEmpty(form[string.Format("PercentShare{0}", id)]) ? null : (double?)Convert.ToDouble(form[string.Format("PercentShare{0}", id)]),
              SequenceNumber = string.IsNullOrEmpty(form[string.Format("SequenceNumber{0}", id)]) ? null : (int?)Convert.ToInt32(form[string.Format("SequenceNumber{0}", id)]),
              Id = (form[string.Format("ProrateLadderId{0}", id)] != string.Empty ? form[string.Format("ProrateLadderId{0}", id)].ToGuid() : new Guid())
            });
          }
        }
    }
}

