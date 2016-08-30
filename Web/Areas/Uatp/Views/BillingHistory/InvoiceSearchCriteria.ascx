<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.BillingHistory.InvoiceSearchCriteria>" %>
<h2>
  Invoice Search Criteria</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="BillingType"><span>*</span> Billing Type:</label>
        <%:Html.BillingTypeDropdownList("BillingTypeId", Model.BillingTypeId)%>
      </div>
      <div>
        <label for="BillingYearMonth">
          <span class="required">*</span> Billing Year / Month.:</label>
        <%: Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, TransactionMode.BillingHistory)%>
      </div>
      <div>
        <label for="BillingPeriod">
          <span class="required">*</span> Billing Period:</label>
        <%: Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.InvoiceSearch)%>
      </div>
      <div>
        <label for="MemberCode">
          <span class="required">*</span> Member Code:</label>
        <%:Html.TextBoxFor(model => model.BilledMemberCode, new { @class= "autocComplete" }) %>
        <%:Html.TextBoxFor(model => model.BilledMemberId, new { @class= "hidden" }) %>
      </div>
      <div>
        <label for="InvoiceNumber">Invoice Number:</label>
        <%:Html.TextBoxFor(model => model.InvoiceNumber, new {maxLength = 10}) %>        
      </div>
  </div>
  <div>
      <div>
        <label for="ChargeCategory">Charge Category:</label>
        <%:Html.ChargeCategoryDropdownListForUatp(model => model.ChargeCategoryId)%>
      </div>
      <div>
        <label for="OurReference">Rejection Stage:</label>
        <%:Html.RejectionStageDropdownListFor(model => model.RejectionStageId, TransactionMode.MiscUatpInvoiceSearch)%>        
      </div>
      <div style="display:none;">
        <label for="TransactionStatus">Transaction Status:</label>
        <%: Html.TransactionStatusDropdownListFor(model => model.TransactionStatusId) %>  
      </div>      
    </div>
  </div>
  <div class="clear">
  </div>
</div>
