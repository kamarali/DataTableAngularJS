<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.BillingYearMonthDropdownForPayables(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.Payables)%>
        </div>
        <div>
          <label>
            Transaction Type :</label>
          <%:Html.InvoiceTypeDropDownListFor(m => m.InvoiceTypeId)%>
        </div>
        <div>
          <label>
            Billing Member:</label>
          <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
          <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete" })%>
        </div>
        <div>
          <label>
            Invoice/Credit Note Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
      </div>
      <div>
        <div>
          <label>
            SMI:</label>
          <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId, InvoiceType.Invoice, TransactionMode.MiscUatpInvoiceSearch)%>
        </div>
       <%-- <div>
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
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="clear">
</div>
