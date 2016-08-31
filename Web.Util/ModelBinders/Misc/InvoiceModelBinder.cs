using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Web.Util.ModelBinders.Misc
{

  public class InvoiceModelBinder : DefaultModelBinder
  {
    private const string CalculatedAmount = "CalculatedAmount";
    private const string VatSubType = "VATSubType";
    private const string NoteName = "NoteDropdown";
    public const string AttachmentId = "AttachmentId";
    
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var invoiceModel = model as MiscUatpInvoice;

      // Make sure we have a invoice instance.
      if (invoiceModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        GetBillingYearMonthPeriod(invoiceModel, form);

        // For tax breakdown.
        GetTaxFields(invoiceModel, form);
        
        // For vat breakdown.
        GetVatFields(invoiceModel, form);

        // For notes original fields.
        GetNoteFields(invoiceModel, form);

        // For additional details original fields.
        GetAdditionalDetailFields(invoiceModel, form);

        // For Add on charges.
        GetAddChargeFields(invoiceModel, form);

        // For billing member contact details.
        GetBillingContacts(invoiceModel, form);

        // For billed member contact details.
        GetBilledContacts(invoiceModel, form);

        // Bind billed in year-month for rejection invoice.
        GetSettlementMonthYear(invoiceModel, form);

        if(invoiceModel.MemberLocationInformation != null)
        {
          // clear the member information added for rendering the page.
          invoiceModel.MemberLocationInformation.Clear();
        }
        else
        {
          invoiceModel.MemberLocationInformation = new List<MemberLocationInformation>();
        }

        GetBillingMemberLocationInfo(form, invoiceModel);
        GetBilledMemberLocationInfo(form, invoiceModel);

        if (!string.IsNullOrEmpty(invoiceModel.LocationCode))
        {
          invoiceModel.LocationCode = invoiceModel.LocationCode.ToUpper();
        }
      }

      return model;
    }

    private static void GetBillingYearMonthPeriod(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      if (form[ControlIdConstants.BillingYearMonthPeriodDropDown] != null)
      {
        // For billing year/month/period dropdown.
        var billingPeriodTokens = form[ControlIdConstants.BillingYearMonthPeriodDropDown].Split('-');

        if (billingPeriodTokens.Length == 3)
        {
          invoiceModel.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
          invoiceModel.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
          invoiceModel.BillingPeriod = Convert.ToInt32(billingPeriodTokens[2]);
        }
      }
    }

    private static void GetSettlementMonthYear(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      if (form[ControlIdConstants.BilledIn] != null)
      {
        var billedInTokens = form[ControlIdConstants.BilledIn].Split('-');

        if (billedInTokens.Length == 2)
        {
          invoiceModel.SettlementYear = Convert.ToInt32(billedInTokens[0]);
          invoiceModel.SettlementMonth = Convert.ToInt32(billedInTokens[1]);
        }
      }
    }

    private static void GetAddChargeFields(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;
      var addOnChargeNameFieldIds = form.AllKeys.Where(a => a.StartsWith(ControlIdConstants.AddChargeName));
      foreach (string addOnChargeNameFieldId in addOnChargeNameFieldIds)
      {
        id = addOnChargeNameFieldId.Substring(ControlIdConstants.AddChargeName.Length, addOnChargeNameFieldId.Length - ControlIdConstants.AddChargeName.Length);
        if (string.IsNullOrEmpty(id)) continue;
          
        var nameId = string.Format(ControlIdConstants.AddChargeName+"{0}", id);
        var baseAmountId = string.Format(ControlIdConstants.ChargeableAmount + "{0}", id);
        var addChargePercentageId = string.Format(ControlIdConstants.AddChargePercentage + "{0}", id);
        var addChargeAmountId = string.Format(ControlIdConstants.AddChargeAmount + "{0}", id);
        var lineItemNumbersId = string.Format(ControlIdConstants.ChargeForLineItemNumber + "{0}", id);

        invoiceModel.AddOnCharges.Add(new InvoiceAddOnCharge
                                        {
                                          Name = form[nameId],
                                          ChargeableAmount = string.IsNullOrEmpty(form[baseAmountId])? (decimal?)null : Convert.ToDecimal(form[baseAmountId]),
                                          Percentage = string.IsNullOrEmpty(form[addChargePercentageId]) ? (double?)null : Convert.ToDouble(form[addChargePercentageId]),
                                          Amount = string.IsNullOrEmpty(form[addChargeAmountId])? 0 : Convert.ToDecimal(form[addChargeAmountId]),
                                          ChargeForLineItemNumber = form[lineItemNumbersId]
                                        });
      }
    }

    private static void GetAdditionalDetailFields(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;
      var addDetailRecordNo = 1;

      if (!string.IsNullOrEmpty(form[ControlIdConstants.AdditionalDetailDropdown]) || !string.IsNullOrEmpty(form[ControlIdConstants.AdditionalDetailDescription]))
      {
        invoiceModel.AdditionalDetails.Add(new MiscUatpInvoiceAdditionalDetail
                                             {
                                               Name = form[ControlIdConstants.AdditionalDetailDropdown],
                                               Description = form[ControlIdConstants.AdditionalDetailDescription],
                                               AdditionalDetailType = AdditionalDetailType.AdditionalDetail,
                                               RecordNumber = addDetailRecordNo
                                             });
      }

      // For additional details cloned fields.
      var addDetailFieldIds = form.AllKeys.Where(a => a.Contains(ControlIdConstants.AdditionalDetailDropdown)).ToList();
      addDetailFieldIds.Sort();
        
      foreach (var addDetailsFieldId in addDetailFieldIds)
      {
        id = addDetailsFieldId.Substring(ControlIdConstants.AdditionalDetailDropdown.Length, addDetailsFieldId.Length - ControlIdConstants.AdditionalDetailDropdown.Length);
        if (string.IsNullOrEmpty(id)) continue;

        addDetailRecordNo++;
        var additionDetailDropDownId = string.Format(ControlIdConstants.AdditionalDetailDropdown + "{0}", id);
        var additionDetailDescId = string.Format(ControlIdConstants.AdditionalDetailDescription + "{0}", id);
          
        if (!string.IsNullOrEmpty(form[additionDetailDropDownId]) || !string.IsNullOrEmpty(form[additionDetailDescId]))
        {
          invoiceModel.AdditionalDetails.Add(new MiscUatpInvoiceAdditionalDetail
                                               {
                                                 Name = form[additionDetailDropDownId],
                                                 Description = form[additionDetailDescId],
                                                 AdditionalDetailType = AdditionalDetailType.AdditionalDetail,
                                                 RecordNumber = addDetailRecordNo
                                               });
        }
      }
    }

    private void GetNoteFields(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;
      var noteRecordNo = 1;
      if (!string.IsNullOrEmpty(form[NoteName]) || !string.IsNullOrEmpty(form["txtNoteDesc"]))
      {
        invoiceModel.AdditionalDetails.Add(new MiscUatpInvoiceAdditionalDetail
                                             {
                                               Name = form[NoteName],
                                               Description = form["txtNoteDesc"],
                                               AdditionalDetailType = AdditionalDetailType.Note,
                                               RecordNumber = noteRecordNo
                                             });
      }

      // For notes cloned fields.
      var noteSubTypeFieldIds = form.AllKeys.Where(a => a.StartsWith(NoteName)).ToList();
      noteSubTypeFieldIds.Sort();

      foreach (string noteSubTypeFieldId in noteSubTypeFieldIds)
      {
        id = noteSubTypeFieldId.Substring(NoteName.Length, noteSubTypeFieldId.Length - NoteName.Length);
        if (string.IsNullOrEmpty(id)) continue;

        noteRecordNo++;
        string noteDropdownId = string.Format("NoteDropdown{0}", id);
        string noteDescriptionId = string.Format("txtNoteDesc{0}", id);
        if (!string.IsNullOrEmpty(form[noteDropdownId]) || !string.IsNullOrEmpty(form[noteDescriptionId]))
        {
          invoiceModel.AdditionalDetails.Add(new MiscUatpInvoiceAdditionalDetail
                                               {
                                                 Name = form[noteDropdownId],
                                                 Description = form[noteDescriptionId],
                                                 AdditionalDetailType = AdditionalDetailType.Note,
                                                 RecordNumber = noteRecordNo
                                               });
        }
      }
    }

    private static void GetVatFields(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;

      // For VAT breakdown.
      var vatSubTypeFieldIds = form.AllKeys.Where(a => a.Contains(VatSubType));
      foreach (string vatSubTypeFieldId in vatSubTypeFieldIds)
      {
        id = vatSubTypeFieldId.Substring(VatSubType.Length, vatSubTypeFieldId.Length - VatSubType.Length);
        if (string.IsNullOrEmpty(id)) continue;
        invoiceModel.TaxBreakdown.Add(new MiscUatpInvoiceTax
                                        {
                                          SubType = form[string.Format("VATSubType{0}", id)],
                                          Amount =
                                            string.IsNullOrEmpty(form[string.Format("VATBaseAmount{0}", id)])
                                              ? (decimal?) null
                                              : Convert.ToDecimal(form[string.Format("VATBaseAmount{0}", id)]),
                                          Percentage = string.IsNullOrEmpty(form[string.Format("VATPercent{0}", id)])  ? (double?)null : Convert.ToDouble(form[string.Format("VATPercent{0}", id)]),
                                          CalculatedAmount = GetVatCalculatedAmount(form[string.Format("VATCalculatedAmount{0}", id)]),
                                          CategoryCode = string.IsNullOrEmpty(form[string.Format("VATCategoryCode{0}", id)]) ? string.Empty : form[string.Format("VATCategoryCode{0}", id)],
                                          Description = form[string.Format("VATDescription{0}", id)],
                                          Id = string.IsNullOrEmpty(form[string.Format("VATId{0}", id)])  ? new Guid() : form[string.Format("VATId{0}", id)].ToGuid(),
                                          Type = TaxType.VAT
                                        });
      }
    }

    private static void GetTaxFields(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      var taxAmountFieldIds = form.AllKeys.Where(a => a.StartsWith(CalculatedAmount));
      string id;
      foreach (string fieldId in taxAmountFieldIds)
      {
        // get id prefix from fieldId(e.g. 1 in case of Amount1)
        id = fieldId.Substring(CalculatedAmount.Length, fieldId.Length - CalculatedAmount.Length);

        if (string.IsNullOrEmpty(id)) continue;
        invoiceModel.TaxBreakdown.Add(new MiscUatpInvoiceTax
                                        {
                                          Amount = string.IsNullOrEmpty(form[string.Format("Amount{0}", id)]) ? (decimal?)null : Convert.ToDecimal(form[string.Format("Amount{0}", id)]),
                                          Percentage =
                                            string.IsNullOrEmpty(form[string.Format("Percentage{0}", id)]) ? (double?)null : Convert.ToDouble(form[string.Format("Percentage{0}", id)]),
                                          CalculatedAmount = GetVatCalculatedAmount(form[string.Format("CalculatedAmount{0}", id)]),
                                          CategoryCode = string.IsNullOrEmpty(form[string.Format("CategoryCode{0}", id)]) ? string.Empty : form[string.Format("CategoryCode{0}", id)],
                                          Id = (form[string.Format("TaxId{0}", id)] != string.Empty ? form[string.Format("TaxId{0}", id)].ToGuid() : new Guid()),
                                          Description = string.IsNullOrEmpty(form[string.Format("TaxDescription{0}", id)]) ? string.Empty : form[string.Format("TaxDescription{0}", id)],
                                          SubType = string.IsNullOrEmpty(form[string.Format("SubType{0}", id)]) ? string.Empty : form[string.Format("SubType{0}", id)],
                                          Type = TaxType.Tax
                                        });
      }
    }

    private static void GetBilledContacts(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;
      var billedContactFieldIds = form.AllKeys.Where(a => a.StartsWith(ControlIdConstants.BilledContactType));
      foreach (string billedContactFieldId in billedContactFieldIds)
      {
        id = billedContactFieldId.Substring(ControlIdConstants.BilledContactType.Length, billedContactFieldId.Length - ControlIdConstants.BilledContactType.Length);
        if (string.IsNullOrEmpty(id)) continue;

        var contactTypeId = string.Format(ControlIdConstants.BilledContactType + "{0}", id);
        var contactDescriptionId = string.Format(ControlIdConstants.BilledContactDescription + "{0}", id);
        var contactValueId = string.Format(ControlIdConstants.BilledContactValue + "{0}", id);
        var contactId = string.Format(ControlIdConstants.BilledContactId + "{0}", id);

        invoiceModel.MemberContacts.Add(new ContactInformation
                                          {
                                            Type = form[contactTypeId],
                                            Description = form[contactDescriptionId],
                                            Value = form[contactValueId],
                                            Id = string.IsNullOrEmpty(form[contactId]) ? new Guid() : form[contactId].ToGuid(),
                                            MemberType = MemberType.Billed
                                          });
      }
    }

    private static void GetBillingContacts(MiscUatpInvoice invoiceModel, NameValueCollection form)
    {
      string id;
      var billingContactFieldIds = form.AllKeys.Where(a => a.StartsWith(ControlIdConstants.BillingContactType));
      foreach (string billingContactFieldId in billingContactFieldIds)
      {
        id = billingContactFieldId.Substring(ControlIdConstants.BillingContactType.Length, billingContactFieldId.Length - ControlIdConstants.BillingContactType.Length);
        if (string.IsNullOrEmpty(id)) continue;

        var contactTypeId = string.Format(ControlIdConstants.BillingContactType + "{0}", id);
        var contactDescriptionId = string.Format(ControlIdConstants.BillingContactDescription + "{0}", id);
        var contactValueId = string.Format(ControlIdConstants.BillingContactValue + "{0}", id);
        var contactId = string.Format(ControlIdConstants.BillingContactId + "{0}", id);

        invoiceModel.MemberContacts.Add(new ContactInformation
                                          {
                                            Type = form[contactTypeId],
                                            Description = form[contactDescriptionId],
                                            Value = form[contactValueId],
                                            Id = string.IsNullOrEmpty(form[contactId]) ? new Guid():form[contactId].ToGuid(),
                                            MemberType = MemberType.Billing 
                                          });
      }
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

    private static void GetBillingMemberLocationInfo(NameValueCollection form, MiscUatpInvoice invoice)
    {
      var billingMember = new MemberLocationInformation();
      billingMember.IsBillingMember = true;

   //   if (!string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberLocationCode]))
     // {
        //billingMember.MemberLocationCode = form[ControlIdConstants.BillingMemberLocationCode];
        // This is done as Organization name has nullable = false. Hence it cannot be set to null or empty.
        //billingMember.OrganizationName = " ";
        //invoice.MemberLocationInformation.Add(billingMember);
        //return;
      //}

      billingMember.MemberLocationCode = form[ControlIdConstants.BillingMemberLocationCode];
      billingMember.OrganizationName = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberRefOrgName]) ? " " : form[ControlIdConstants.BillingMemberRefOrgName];
      billingMember.AddressLine1 = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberRefAddressLine1]) ? string.Empty : form[ControlIdConstants.BillingMemberRefAddressLine1];
      billingMember.AddressLine2 = form[ControlIdConstants.BillingMemberRefAddressLine2];
      billingMember.AddressLine3 = form[ControlIdConstants.BillingMemberRefAddressLine3];
      billingMember.CityName = form[ControlIdConstants.BillingMemberRefCity];
      billingMember.CompanyRegistrationId = form[ControlIdConstants.BillingMemberRefCompanyRegId];
      billingMember.CountryCode = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberCountryCode + "1"]) ? string.Empty : form[ControlIdConstants.BillingMemberCountryCode + "1"];
      billingMember.CountryName = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberCountryName]) ? string.Empty : form[ControlIdConstants.BillingMemberCountryName];
      
      billingMember.AdditionalTaxVatRegistrationNumber = form[ControlIdConstants.BillingMemberAdditionalTaxVatRegNumber];
      billingMember.PostalCode = form[ControlIdConstants.BillingMemberRefPostalCode];
      billingMember.SubdivisionName = form[ControlIdConstants.BillingMemberRefSubdivisionName];
      billingMember.TaxRegistrationId = form[ControlIdConstants.BillingMemberRefTaxRegistrationId];
      billingMember.LegalText = form[ControlIdConstants.BillingMemberRefLegalText];
      
      invoice.MemberLocationInformation.Add(billingMember);
    }

    private static void GetBilledMemberLocationInfo(NameValueCollection form, MiscUatpInvoice invoice)
    {
      var billedMember = new MemberLocationInformation();
      billedMember.IsBillingMember = false;

      //if (!string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberLocationCode]))
      //{
      //  billedMember.MemberLocationCode = form[ControlIdConstants.BilledMemberLocationCode];
      //  // This is done as Organization name has nullable = false. Hence it cannot be set to null or empty.
      //  billedMember.OrganizationName = " ";
      //  invoice.MemberLocationInformation.Add(billedMember);
      //  return;
      //}

      billedMember.MemberLocationCode = form[ControlIdConstants.BilledMemberLocationCode];
      billedMember.OrganizationName = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberRefOrgName]) ? " " : form[ControlIdConstants.BilledMemberRefOrgName];
      billedMember.AddressLine1 = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberRefAddressLine1]) ? string.Empty : form[ControlIdConstants.BilledMemberRefAddressLine1];
      billedMember.AddressLine2 = form[ControlIdConstants.BilledMemberRefAddressLine2];
      billedMember.AddressLine3 = form[ControlIdConstants.BilledMemberRefAddressLine3];
      billedMember.CityName = form[ControlIdConstants.BilledMemberRefCity];
      billedMember.CompanyRegistrationId = form[ControlIdConstants.BilledMemberRefCompanyRegId];
      billedMember.CountryCode = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberCountryCode + "1"]) ? string.Empty : form[ControlIdConstants.BilledMemberCountryCode + "1"];
      billedMember.CountryName = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberCountryName]) ? string.Empty : form[ControlIdConstants.BilledMemberCountryName];
      
      billedMember.AdditionalTaxVatRegistrationNumber = form[ControlIdConstants.BilledMemberAdditionalTaxVatRegNumber];
      billedMember.PostalCode = form[ControlIdConstants.BilledMemberRefPostalCode];
      billedMember.SubdivisionName = form[ControlIdConstants.BilledMemberRefSubdivisionName];
      billedMember.TaxRegistrationId = form[ControlIdConstants.BilledMemberRefTaxRegistrationId];
      
      invoice.MemberLocationInformation.Add(billedMember);
    }
  }
}
