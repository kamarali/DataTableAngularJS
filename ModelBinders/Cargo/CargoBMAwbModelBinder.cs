using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Core;

namespace Iata.IS.Web.Util.ModelBinders.Cargo
{
    public class CargoBMAwbModelBinder : DefaultModelBinder
    {
        public const string VatIdentifierId = "VatIdentifierId";
        public const string AttachmentId = "AttachmentId";
        public const string OtherChargeId = "OtherChargeId";
        public const string ProrateLadderId = "ProrateLadderId";
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);

            var bmAwbRecord = model as CargoBillingMemoAwb;

            if (bmAwbRecord != null)
            {
                var form = controllerContext.HttpContext.Request.Form;
                string id = string.Empty;
                var vatIds = form.AllKeys.Where(a => a.Contains(VatIdentifierId));

                foreach (string code in vatIds)
                {
                    id = code.Substring(VatIdentifierId.Length, code.Length - VatIdentifierId.Length);
                    if (string.IsNullOrEmpty(id))
                        continue;
                    bmAwbRecord.AwbVat.Add(new BMAwbVat()
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

                GetProrateLadderFields(bmAwbRecord, form);

                var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
                foreach (string code in attachmentIds)
                {
                    id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
                    if (string.IsNullOrEmpty(id))
                        continue;
                    bmAwbRecord.Attachments.Add(new BMAwbAttachment()
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
                    bmAwbRecord.OtherCharges.Add(new BMAwbOtherCharge()
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

                //var prorateLadderIds = form.AllKeys.Where(a => a.Contains(ProrateLadderId));
                //foreach (string prorateLadder in prorateLadderIds)
                //{
                //    id = prorateLadder.Substring(OtherChargeId.Length, prorateLadder.Length - OtherChargeId.Length);
                //    if (string.IsNullOrEmpty(id))
                //        continue;
                //    bmAwbRecord.ProrateLadder.Add(new BMAwbProrateLadder()
                //    {
                //        FromSector = (form[string.Format("FromSector{0}", id)]),
                //        ToSector = (form[string.Format("ToSector{0}", id)]),
                //        CarrierPrefix = (form[string.Format("CarrierPrefix{0}", id)]),
                //        ProvisoReqSpa = (form[string.Format("ProvisoReqSpa{0}", id)]),
                //        ProrateCalCurrencyId = (form[string.Format("ProrateCalCurrencyId{0}", id)]),
                //        TotalAmount = Double.Parse((form[string.Format("TotalAmount{0}", id)])),
                //        ProrateFactor = Int32.Parse((form[string.Format("ProrateFactor{0}", id)])),
                //        PercentShare = Double.Parse((form[string.Format("PercentShare{0}", id)])),
                //        Amount = Double.Parse((form[string.Format("Amount{0}", id)])),
                //        Id = (form[string.Format("ProrateLadderId{0}", id)] != string.Empty ? form[string.Format("ProrateLadderId{0}", id)].ToGuid() : new Guid())
                //    });
                //}
                if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
                {
                    var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
                    bmAwbRecord.AttachmentIndicatorOriginal = attachmentIndicator == "Yes";
                }

                string awbSerialNumberCheckDigit = form["AwbSerialNumber"];
                model = ModelBinderHelper.SplitAwbSerialNumber(bmAwbRecord, awbSerialNumberCheckDigit) as CargoBillingMemoAwb;
            }
            return model;
        }

        private static void GetProrateLadderFields(CargoBillingMemoAwb awbRecord, NameValueCollection form)
        {
          var prorateLadderIds = form.AllKeys.Where(a => a.Contains(ProrateLadderId));
          foreach (string prorateLadder in prorateLadderIds)
          {
            string id = prorateLadder.Substring(ProrateLadderId.Length, prorateLadder.Length - ProrateLadderId.Length);
            if (string.IsNullOrEmpty(id))
              continue;
            awbRecord.ProrateLadder.Add(new BMAwbProrateLadderDetail()
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

