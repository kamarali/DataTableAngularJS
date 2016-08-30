using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Iata.IS.Web.Util.ModelBinders
{

  public class InvoiceModelBinder : DefaultModelBinder
  {
    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    {
      var model = base.BindModel(controllerContext, bindingContext);

      var invoiceModel = model as PaxInvoice;

      // Make sure we have a invoice instance.
      if (invoiceModel != null)
      {
        var form = controllerContext.HttpContext.Request.Form;

        if (form[ControlIdConstants.BillingYearMonthPeriodDropDown] != null)
        {
          var billingPeriodTokens = form[ControlIdConstants.BillingYearMonthPeriodDropDown].Split('-');

          if (billingPeriodTokens.Length == 3)
          {
            invoiceModel.BillingYear = Convert.ToInt32(billingPeriodTokens[0]);
            invoiceModel.BillingMonth = Convert.ToInt32(billingPeriodTokens[1]);
            invoiceModel.BillingPeriod = Convert.ToInt32(billingPeriodTokens[2]);

          }
        }

        //Get data for provisional billing month and provisional billing year
        if (form[ControlIdConstants.ProvisionalBillingMonthFormDEDropdown] != null)
        {
            var provisionalBillingMonthTokens = form[ControlIdConstants.ProvisionalBillingMonthFormDEDropdown].Split('-');

            if (provisionalBillingMonthTokens.Length == 2)
            {
                invoiceModel.ProvisionalBillingYear = Convert.ToInt32(provisionalBillingMonthTokens[0]);
                invoiceModel.ProvisionalBillingMonth = Convert.ToInt32(provisionalBillingMonthTokens[1]);

            }
        }

        //Get billing and billed member location info
        if (invoiceModel.MemberLocationInformation != null)
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
      }

      return model;
    }

    /// <summary>
    /// Populates Billing member location details from hidden field if billing member location selected in dropdown is empty
    /// </summary>
    /// <param name="form"></param>
    /// <param name="invoice"></param>
    private static void GetBillingMemberLocationInfo(NameValueCollection form, PaxInvoice invoice)
    {
      var billingMember = new MemberLocationInformation();
      billingMember.IsBillingMember = true;

   //   if (!string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberLocationCode]))
   //   {
      //  billingMember.MemberLocationCode = form[ControlIdConstants.BillingMemberLocationCode];
        // This is done as Organization name has nullable = false. Hence it cannot be set to null or empty.
     //   billingMember.OrganizationName = " ";
     //   invoice.MemberLocationInformation.Add(billingMember);
     //   return;
    //  }

      billingMember.MemberLocationCode = form[ControlIdConstants.BillingMemberLocationCode];
      billingMember.OrganizationName = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberRefOrgName]) ? " " : form[ControlIdConstants.BillingMemberRefOrgName];
      billingMember.AddressLine1 = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberRefAddressLine1]) ? string.Empty : form[ControlIdConstants.BillingMemberRefAddressLine1];
      billingMember.AddressLine2 = form[ControlIdConstants.BillingMemberRefAddressLine2];
      billingMember.AddressLine3 = form[ControlIdConstants.BillingMemberRefAddressLine3];
      billingMember.CityName = form[ControlIdConstants.BillingMemberRefCity];
      billingMember.CompanyRegistrationId = form[ControlIdConstants.BillingMemberRefCompanyRegId];
      billingMember.CountryCode = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberCountryCode + "1"]) ? string.Empty : form[ControlIdConstants.BillingMemberCountryCode + "1"];
      billingMember.CountryName = string.IsNullOrEmpty(form[ControlIdConstants.BillingMemberCountryName]) ? string.Empty : form[ControlIdConstants.BillingMemberCountryName];

      billingMember.PostalCode = form[ControlIdConstants.BillingMemberRefPostalCode];
      billingMember.SubdivisionName = form[ControlIdConstants.BillingMemberRefSubdivisionName];
      billingMember.TaxRegistrationId = form[ControlIdConstants.BillingMemberRefTaxRegistrationId];
      billingMember.LegalText = form[ControlIdConstants.BillingMemberRefLegalText];
      billingMember.AdditionalTaxVatRegistrationNumber = form[ControlIdConstants.BillingMemberAdditionalTaxVatRegNumber];

      invoice.MemberLocationInformation.Add(billingMember);
    }

    /// <summary>
    /// Populates Billed member location details from hidden field if billed member location selected in dropdown is empty
    /// </summary>
    /// <param name="form"></param>
    /// <param name="invoice"></param>
    private static void GetBilledMemberLocationInfo(NameValueCollection form, PaxInvoice invoice)
    {
      var billedMember = new MemberLocationInformation();
      billedMember.IsBillingMember = false;

    //  if (!string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberLocationCode]))
   //   {
    //    billedMember.MemberLocationCode = form[ControlIdConstants.BilledMemberLocationCode];
        // This is done as Organization name has nullable = false. Hence it cannot be set to null or empty.
    //    billedMember.OrganizationName = " ";
    //    invoice.MemberLocationInformation.Add(billedMember);
   //     return;
   //   }

      billedMember.MemberLocationCode = form[ControlIdConstants.BilledMemberLocationCode];
      billedMember.OrganizationName = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberRefOrgName]) ? " " : form[ControlIdConstants.BilledMemberRefOrgName];
      billedMember.AddressLine1 = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberRefAddressLine1]) ? string.Empty : form[ControlIdConstants.BilledMemberRefAddressLine1];
      billedMember.AddressLine2 = form[ControlIdConstants.BilledMemberRefAddressLine2];
      billedMember.AddressLine3 = form[ControlIdConstants.BilledMemberRefAddressLine3];
      billedMember.CityName = form[ControlIdConstants.BilledMemberRefCity];
      billedMember.CompanyRegistrationId = form[ControlIdConstants.BilledMemberRefCompanyRegId];
      billedMember.CountryCode = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberCountryCode + "1"]) ? string.Empty : form[ControlIdConstants.BilledMemberCountryCode + "1"];
      billedMember.CountryName = string.IsNullOrEmpty(form[ControlIdConstants.BilledMemberCountryName]) ? string.Empty : form[ControlIdConstants.BilledMemberCountryName];

      billedMember.PostalCode = form[ControlIdConstants.BilledMemberRefPostalCode];
      billedMember.SubdivisionName = form[ControlIdConstants.BilledMemberRefSubdivisionName];
      billedMember.TaxRegistrationId = form[ControlIdConstants.BilledMemberRefTaxRegistrationId];
      billedMember.AdditionalTaxVatRegistrationNumber = form[ControlIdConstants.BilledMemberAdditionalTaxVatRegNumber];

      invoice.MemberLocationInformation.Add(billedMember);
    }
  }
}