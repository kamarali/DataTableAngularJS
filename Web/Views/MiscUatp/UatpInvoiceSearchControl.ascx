<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            Transaction Type :</label>
          <%:Html.InvoiceTypeDropDownListFor(m => m.InvoiceTypeId)%>
        </div>
        <div>
          <label>
            Invoice/Credit Note Status:</label>
          <%:Html.InvoiceStatusDropdownListFor(m => m.InvoiceStatus, true)%>
        </div>
        <div>
          <label>
            Billed Member:</label>
          <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
          <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete" })%>
        </div>        
      </div>
      <div>
        <div>
          <label>
            Invoice/Credit Note Owner:</label>
          <%:Html.InvoiceOwnerDropDownListFor(m => m.OwnerId, true)%>
        </div>
        <div>
          <label>
            Invoice/Credit Note Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
        <div>
          <label>
            SMI:</label>
          <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId,InvoiceType.Invoice, TransactionMode.MiscUatpInvoiceSearch)%>
        </div>
        <%--<div>
          <label>
            Charge Category:</label>
          <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, Model.BillingCategory, true) %>
        </div>--%>
        <% // Show Location only when BillingCategory is Miscellaneous
          if (Model.BillingCategory == BillingCategoryType.Misc)
          {%>
          <div>
          <label>
            Location:</label>
          <%:Html.TextBoxFor(invoice => invoice.LocationCode)%>
        </div>
        <%}%>
        <div>
          <label>
            File Name:</label>
            <%-- As per Robin's request, changed the maxLength from 30 to 100 --%>
          <%: Html.TextBox("FileName",Server.HtmlEncode(Model.FileName), new { @class = "xlargeTextField", maxLength = 100 })%>
        </div>
        <div>
          <label>
            Submission Method:</label>
          <%:Html.SubmissionMethodDropDownListFor(m => m.SubmissionMethodId, true,true)%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="clear">
</div>
