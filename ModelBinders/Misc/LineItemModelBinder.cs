using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Web.Util.ModelBinders.Misc
{
  public class LineItemModelBinder : DefaultModelBinder
  {
    public const string TaxCode = "TaxCode";
    private const string CalculatedAmount = "CalculatedAmount";
    private const string VATSubType = "VATSubType";
    public const string AttachmentId = "AttachmentId";

    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var lineItemModel = model as LineItem;

      // Make sure we have a line item instance.
      if (lineItemModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        var taxAmountFieldIds = form.AllKeys.Where(a => a.StartsWith(CalculatedAmount));
        var id = string.Empty;
        foreach (string fieldId in taxAmountFieldIds)
        {
          // get id prefix from fieldId(e.g. 1 in case of Amount1)
          id = fieldId.Substring(CalculatedAmount.Length, fieldId.Length - CalculatedAmount.Length);
          
          if (string.IsNullOrEmpty(id)) continue;
          lineItemModel.TaxBreakdown.Add(new LineItemTax
                                           {
                                             Amount = string.IsNullOrEmpty(form[string.Format("Amount{0}", id)]) ? (decimal?)null : Convert.ToDecimal(form[string.Format("Amount{0}", id)]),
                                             Percentage = string.IsNullOrEmpty(form[string.Format("Percentage{0}", id)]) ? (double?)null : Convert.ToDouble(form[string.Format("Percentage{0}", id)]),
                                             /*SCP257502:  VAT NODE MISSING-Upendra*/
                                             CalculatedAmount =
                                               string.IsNullOrEmpty(form[string.Format("CalculatedAmount{0}", id)])
                                                 ? (decimal?)null
                                                 : Convert.ToDecimal(form[string.Format("CalculatedAmount{0}", id)]),
                                             CategoryCode = form[string.Format("CategoryCode{0}", id)],
                                             Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid()),
                                             Description = form[string.Format("TaxDescription{0}", id)],
                                             Type = TaxType.Tax,
                                             SubType = form[string.Format("SubType{0}", id)]
                                           });
        }

        var vatSubTypeFieldIds = form.AllKeys.Where(a => a.Contains(VATSubType));
        foreach (var vatSubTypeFieldId in vatSubTypeFieldIds)
        {
          id = vatSubTypeFieldId.Substring(VATSubType.Length, vatSubTypeFieldId.Length - VATSubType.Length);
          if (string.IsNullOrEmpty(id)) continue;
          lineItemModel.TaxBreakdown.Add(new LineItemTax
                                           {
                                             SubType = form[string.Format("VATSubType{0}", id)],
                                             Amount =
                                               string.IsNullOrEmpty(form[string.Format("VATBaseAmount{0}", id)]) ? (decimal?)null : Convert.ToDecimal(form[string.Format("VATBaseAmount{0}", id)].Trim()),
                                             Percentage = string.IsNullOrEmpty(form[string.Format("VATPercent{0}", id)]) ? (double?)null : Convert.ToDouble(form[string.Format("VATPercent{0}", id)]),
                                             CalculatedAmount = GetVatCalculatedAmount(form[string.Format("VATCalculatedAmount{0}", id)]), 
                                             CategoryCode = form[string.Format("VATCategoryCode{0}", id)],
                                             Description = form[string.Format("VATDescription{0}", id)],
                                             Id = (!string.IsNullOrEmpty(form[string.Format("VATId{0}", id)])? form[string.Format("VATId{0}", id)].ToGuid() : new Guid()),
                                             Type = TaxType.VAT
                                           });
        }

        var recordNo = 1;
        if (!string.IsNullOrEmpty(form[ControlIdConstants.AdditionalDetailDropdown]) || !string.IsNullOrEmpty(form[ControlIdConstants.AdditionalDetailDescription]))
        {
            lineItemModel.LineItemAdditionalDetails.Add(new LineItemAdditionalDetail
                                                        {
                                                          Name = form[ControlIdConstants.AdditionalDetailDropdown],
                                                          Description = form[ControlIdConstants.AdditionalDetailDescription],
                                                          AdditionalDetailType = AdditionalDetailType.AdditionalDetail,
                                                          RecordNumber = recordNo
                                                        });
        }

        var addDetailFieldIds = form.AllKeys.Where(a => a.Contains(ControlIdConstants.AdditionalDetailDropdown)).ToList();
        addDetailFieldIds.Sort();

        foreach (var addDetailsFieldId in addDetailFieldIds)
        {
          id = addDetailsFieldId.Substring(ControlIdConstants.AdditionalDetailDropdown.Length, addDetailsFieldId.Length - ControlIdConstants.AdditionalDetailDropdown.Length);
          if (string.IsNullOrEmpty(id)) continue;

          recordNo++;
          var additionDetailDropDownId = string.Format(ControlIdConstants.AdditionalDetailDropdown + "{0}", id);
          var additionDetailDescId = string.Format(ControlIdConstants.AdditionalDetailDescription + "{0}", id);

          if (!string.IsNullOrEmpty(form[additionDetailDropDownId]) || !string.IsNullOrEmpty(form[additionDetailDescId]))
          {
            lineItemModel.LineItemAdditionalDetails.Add(new LineItemAdditionalDetail
                                                          {
                                                            Name = form[additionDetailDropDownId],
                                                            Description = form[additionDetailDescId],
                                                            AdditionalDetailType = AdditionalDetailType.AdditionalDetail,
                                                            RecordNumber = recordNo
                                                          });
          }
        }


        // For add on charges.
        var addOnChargeNameFieldIds = form.AllKeys.Where(a => a.StartsWith(ControlIdConstants.AddChargeName));
        foreach (string addOnChargeNameFieldId in addOnChargeNameFieldIds)
        {
          id = addOnChargeNameFieldId.Substring(ControlIdConstants.AddChargeName.Length, addOnChargeNameFieldId.Length - ControlIdConstants.AddChargeName.Length);
          if (string.IsNullOrEmpty(id)) continue;

          var nameId = string.Format(ControlIdConstants.AddChargeName + "{0}", id);
          var baseAmountId = string.Format(ControlIdConstants.ChargeableAmount + "{0}", id);
          var addChargePercentageId = string.Format(ControlIdConstants.AddChargePercentage + "{0}", id);
          var addChargeAmountId = string.Format(ControlIdConstants.AddChargeAmount + "{0}", id);

          lineItemModel.AddOnCharges.Add(new LineItemAddOnCharge
                                          {
                                            Name = form[nameId],
                                            ChargeableAmount = string.IsNullOrEmpty(form[baseAmountId]) ? (decimal?)null : Convert.ToDecimal(form[baseAmountId]),
                                            Percentage = string.IsNullOrEmpty(form[addChargePercentageId]) ? (double?)null : Convert.ToDouble(form[addChargePercentageId]),
                                            Amount = string.IsNullOrEmpty(form[addChargeAmountId]) ? 0 : Convert.ToDecimal(form[addChargeAmountId]),
                                            Id = (form[string.Format("Id{0}", id)] != string.Empty ? form[string.Format("Id{0}", id)].ToGuid() : new Guid()),
                                          });
        }

        if (!string.IsNullOrEmpty(lineItemModel.LocationCode))
          lineItemModel.LocationCode = lineItemModel.LocationCode.ToUpper();
        //if (lineItemModel.Invoice == null) lineItemModel.Invoice = new MiscUatpInvoice();
        //var attachmentIds = form.AllKeys.Where(a => a.Contains(AttachmentId));
        //foreach (string code in attachmentIds)
        //{
        //  id = code.Substring(AttachmentId.Length, code.Length - AttachmentId.Length);
        //  if (string.IsNullOrEmpty(id))
        //    continue;
        //  lineItemModel.Invoice.Attachments.Add(new MiscUatpAttachment()
        //  {
        //    Id = (form[string.Format("AttachmentId{0}", id)] != string.Empty ? form[string.Format("AttachmentId{0}", id)].ToGuid() : new Guid())
        //  });
        //}
      }
      return model;
    }

    private static decimal? GetVatCalculatedAmount(string calculatedAmountInString)
    {
      decimal? returnValue = null;
      decimal parsedValue;
      if (!string.IsNullOrEmpty(calculatedAmountInString) && decimal.TryParse(calculatedAmountInString, out parsedValue))
      {
        returnValue = parsedValue;
      }

      return returnValue;
    }
  }
}
