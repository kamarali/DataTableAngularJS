using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.Pax;
using Iata.IS.Core;

namespace Iata.IS.Web.Util.ModelBinders
{
  public class CouponRecordModelBinder : DefaultModelBinder
  {
    public const string TaxCode = "TaxCode";
    public const string VatId = "VatIdentifierId";
    public const string AttachmentId = "AttachmentId";
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var couponRecord = model as PrimeCoupon;

      if (couponRecord != null)
      {

        couponRecord.FromAirportOfCoupon = !string.IsNullOrEmpty(couponRecord.FromAirportOfCoupon) ? couponRecord.FromAirportOfCoupon.ToUpper() : null;
        couponRecord.ToAirportOfCoupon = !string.IsNullOrEmpty(couponRecord.ToAirportOfCoupon) ? couponRecord.ToAirportOfCoupon.ToUpper() : null;

        var form = controllerContext.HttpContext.Request.Form;
                
        if (!string.IsNullOrEmpty(form[ControlIdConstants.AttachmentIndicatorOriginal]))
        {
          var attachmentIndicator = form[ControlIdConstants.AttachmentIndicatorOriginal];
          couponRecord.AttachmentIndicatorOriginal = Convert.ToInt32(attachmentIndicator == "Yes");
        }

        var taxCodes = form.AllKeys.Where(a => a.Contains(TaxCode));
        string id = string.Empty;
        foreach (string code in taxCodes)
        {
          id = code.Substring(TaxCode.Length, code.Length - TaxCode.Length);

          if (string.IsNullOrEmpty(id))
            continue;
          couponRecord.TaxBreakdown.Add(new PrimeCouponTax()
          {
            TaxCode = form[string.Format("TaxCode{0}", id)].ToUpper(),
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
          couponRecord.VatBreakdown.Add(new PrimeCouponVat()
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
          couponRecord.Attachments.Add(new PrimeCouponAttachment()
          {
            Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
          });
        }

        couponRecord.CabinClass = string.IsNullOrEmpty(couponRecord.CabinClass) ? couponRecord.CabinClass : couponRecord.CabinClass.ToUpper();
      }
      return model;
    }
  }
}
