<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoInvoice>" %>
<%@ Import Namespace="Iata.IS.AdminSystem" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<h2>
  Header Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
        <div>
        <%
         if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Receivables)
          {%>
        <label>
          <span>*</span> Billed Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete" })%>
        <%}
          else
          {%>
        <label>
          Billing Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.BillingMemberText, new { @readOnly = true })%>
        <%}%>
        <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
        <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
      </div>
      <div>
        <label>
          <span>*</span>
          <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
          Number:</label>
        <%:Html.TextBoxFor(invoice => invoice.InvoiceNumber, new { maxLength = 10, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span>*</span>
          <%:Model.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice"%>
          Date:</label>
        <%:Html.TextBox(ControlIdConstants.InvoiceDate, Model.InvoiceDate.ToString(FormatConstants.DateFormat), new { @readOnly = true })%>
      </div>
      <div>
        <label>
          <span>*</span> Billing Year/Month/Period:</label>
        <%:Html.BillingYearMonthPeriodDropdown(ControlIdConstants.BillingYearMonthPeriodDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod)%>
      </div>
    </div>
    <div>
      <div>
        <%: ScriptHelper.GenerateDialogueHtml("Billing Member's Location ID:", "Billing Member Reference Data", "BillingMemberReference", 520, 700)%>
        <%
          const string defaultLocation = "Main";  %>
        <%: Html.MemberLocationIdDropdownList(ControlIdConstants.BillingMemberLocationCode, Model.BillingMemberLocationCode, Model.BillingMemberId, BillingCategoryType.Cgo, MemberType.Billing, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null && Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[0] : null)%>
      </div>
      <div>
        <%: ScriptHelper.GenerateDialogueHtml("Billed Member's Location ID:", "Billed Member Reference Data", "BilledMemberReference", 450, 700, "BilledMemberLocLink")%>
        <%: Html.MemberLocationIdDropdownList(ControlIdConstants.BilledMemberLocationCode, Model.BilledMemberLocationCode, Model.BilledMemberId, BillingCategoryType.Cgo,MemberType.Billed, new { @class = "mediumTextField" }, defaultLocation, ViewData[ViewDataConstants.PageMode] != null && (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.Edit || ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View) ? true : false, (Model.MemberLocationInformation != null && Model.MemberLocationInformation.Count == 2) ? Model.MemberLocationInformation[1] : null)%>
      </div>
      <div>
        <label>
          <span>*</span> Settlement Method:</label>
           <%:Html.SettlementMethodDropdownList(ControlIdConstants.SettlementMethodIdDropDown, Model.SettlementMethodDisplayText, Model.InvoiceType, TransactionMode.Transactions)%>
        <%--<%: Html.Hidden(ControlIdConstants.SettlementMethodIdDropDown + "ForView", Model.SettlementMethodId)%>--%>       
      </div>
      <%if (ViewData[ViewDataConstants.BillingType].ToString() == Iata.IS.Web.Util.BillingType.Payables)
        {%>
      <div>
        <label>
          Suspended Flag:</label>
        <%: Html.TextBox(ControlIdConstants.SuspendedInvoiceFlag, Model.SuspendedInvoiceFlag ? "Yes" : string.Empty)%>
      </div>
      <%}%>
    </div>
    <div>
      <div>
        <label for="ListingCurrency">
          <span>*</span> Currency of Listing:</label>
        <%: Html.CurrencyDropdownList(ControlIdConstants.ListingCurrencyId, Model.ListingCurrencyId.HasValue ? Model.ListingCurrencyId.Value.ToString() : string.Empty)%>
      </div>
      <div>
        <label>
          <span>*</span> Currency of Billing:</label>
        <%:Html.BillingCurrencyDropdownList(ControlIdConstants.BillingCurrencyId, Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, null, Model.SettlementMethodId)%>
      </div>
      <div class="hidden">
        <%:Html.BillingCurrencyDropdownList("HiddenBillingCurrency", Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, new { id = "HiddenBillingCurrency" }, Model.SettlementMethodId)%>
        <%:Html.HiddenFor(invoice => invoice.BillingCurrencyId, new { id = "HiddenBillingCurrencyId" })%>
        <%--CMP #553: ACH Requirement for Multiple Currency Handling--%>
        <%:Html.BillingAchCurrencyDropdownList("HiddenAchBillingCurrency", Model.BillingCurrencyId.HasValue ? Model.BillingCurrencyId.Value.ToString() : string.Empty, new { id = "HiddenAchBillingCurrency" }, Model.SettlementMethodId)%>
      </div>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <div>
        <label>
          Listing to Billing Rate:</label> 
        <%:Html.TextBox(ControlIdConstants.ListingToBillingRate, Model.ExchangeRate.HasValue ? Model.ExchangeRate.Value.ToString(FormatConstants.ExchangeRateEditFormat) : null, new { @readOnly = true })%>
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
      <div class="Hidden">
        <%:Html.HiddenFor(invoice => invoice.InvoiceOwnerId)%>
      </div>
    </div>
    <div>
        <!-- CMP #624: ICH Rewrite-New SMI X 
        Description: FRS Section 2.12 PAX/CGO IS-WEB Screens (Part 1) 
        Change #1: New fields ‘CH Agreement Indicator’ and ‘CH Due Date’ -->
	    <div>
		    <label>CH Agreement Indicator:</label>
		    <%:Html.TextBoxFor(invoice => invoice.ChAgreementIndicator, new { maxLength = 5 })%>
	    </div>
	    <div>
		    <label>CH Due Date:</label>
             <%: Html.TextBox(ControlIdConstants.ChDueDate, (Model.ChDueDate.HasValue == true ? Model.ChDueDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
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