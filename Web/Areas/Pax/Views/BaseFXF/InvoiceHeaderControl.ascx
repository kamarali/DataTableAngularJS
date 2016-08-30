﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PaxInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>

<h2>
  Header Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Receivables)
          {%>
        <label><span>*</span> Billed Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete" })%>
        <%}
          else
          {%>
        <label>Billing Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.BillingMemberText, new { @readOnly = true })%>
          <%}%>
        <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
        <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
        
      </div>
      <div>
        <label>
          <span>*</span> Invoice Number:</label>
        <%:Html.TextBoxFor(invoice => invoice.InvoiceNumber, new { maxLength = 10, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span>*</span> Invoice Date:</label>
        <%:Html.TextBox(ControlIdConstants.InvoiceDate, Model.InvoiceDate.ToString(FormatConstants.DateFormat), new { @readonly = true })%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Billing Month:</label>
        <%:Html.ProvisionalBillingMonthFormDEDropdownList(ControlIdConstants.ProvisionalBillingMonthFormDEDropdown, Model.ProvisionalBillingMonth, Model.ProvisionalBillingYear, 4, 13)%>
      </div>
      <div>
        <label>
          <span>*</span> Sampling Constant:</label>
        <%:Html.TextBoxFor(invoice => invoice.SamplingConstant, new { min = 0.001, max = 9999.999, roundTo = 3, @class = "percentage" })%>
      </div>
    </div>
    <div>
      <div>
        <%: ScriptHelper.GenerateDialogueHtml("Billing Member's Location ID:", "Billing Member Reference Data", "BillingMemberReference", 520, 700)%>
        <% const string defaultLocation = "Main";  %>
        <%:Html.MemberLocationIdDropdownList(ControlIdConstants.BillingMemberLocationCode, Model.BillingMemberLocationCode, Model.BillingMemberId, BillingCategoryType.Pax, MemberType.Billing, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null && Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[0] : null)%>
      </div>
      <div>
        <%: ScriptHelper.GenerateDialogueHtml("Billed Member's Location ID:", "Billed Member Reference Data", "BilledMemberReference", 400, 700, "BilledMemberLocLink")%>
        <%:Html.MemberLocationIdDropdownList(ControlIdConstants.BilledMemberLocationCode, Model.BilledMemberLocationCode, Model.BilledMemberId, BillingCategoryType.Pax, MemberType.Billed, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null && Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[1] : null)%>
      </div>
      <div>
        <label>
          <span>*</span> Settlement Method:</label>
          <%--CMP#624: 2.16 : PAX Sampling IS-WEB Screens--%>
        <%:Html.SettlementMethodDropdownList(ControlIdConstants.SettlementMethodIdDropDown, Model.SettlementMethodDisplayText, Model.InvoiceType, TransactionMode.Transactions,true)%>
      </div>
      <div>
        <label>
          <span>*</span> Billing Year/Month/Period:</label>
        <%:Html.BillingYearMonthPeriodDropdown(ControlIdConstants.BillingYearMonthPeriodDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod)%>
      </div>
    </div>
    <div>
      <div>
        <label for="ListingCurrency">
          <span>*</span> Currency of Listing/Evaluation:</label>
        <%:Html.CurrencyDropdownList(ControlIdConstants.ListingCurrencyId, Model.ListingCurrencyId.HasValue ? Model.ListingCurrencyId.Value.ToString() : string.Empty)%>
      </div>
      <div>
        <label>
          <span>*</span> Currency of Billing:</label>
        <%:Html.BillingCurrencyDropdownList(ControlIdConstants.BillingCurrencyId, Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, null, Model.SettlementMethodId)%>
      </div>
      <div  class="hidden">
        <%:Html.BillingCurrencyDropdownList(ControlIdConstants.BillingCurrencyId, Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, new { id = "HiddenBillingCurrency" }, Model.SettlementMethodId)%>
        <%:Html.HiddenFor(invoice => invoice.BillingCurrencyId, new { id = "HiddenBillingCurrencyId" })%>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        <label>
          Listing/Evaluation to Billing Rate:</label>
        <%:Html.TextBox(ControlIdConstants.ListingToBillingRate, Model.ExchangeRate.HasValue? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateFormat):null, new { @readonly = true })%>
      </div>
      <div>
        <label for="DigitalSignature">
          Digital Signature:</label>
        <%:Html.DigitalSignatureDropdownList(ControlIdConstants.DigitalSignatureRequiredId, Model.DigitalSignatureRequiredId.ToString())%>
        <%:Html.Hidden("DigitalSignatureFlagId", ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId])%>
      </div>
       <%if (SystemParameters.Instance.General.IsMultilingualAllowed)
        {%>
        <div>
            <label>Invoice Template Language: </label>
            <%: Html.LanguageDropdownList(ControlIdConstants.InvTemplateLanguage,
                                                            Model.InvTemplateLanguage) %>
        </div>
        <%}%>
      <div>
        <%:Html.HiddenFor(invoice => invoice.InvoiceOwnerId)%>
        <%:Html.HiddenFor(invoice => invoice.IsFormDEViaIS)%>
        <%:Html.HiddenFor(invoice => invoice.IsFormFViaIS)%>
     </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="billingMemberReferenceContainer" class="hidden">
</div>
<div id="billedMemberReferenceContainer" class="hidden">
</div>