<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.BillingHistory.InvoiceSearchCriteria>" %>
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
        <%-- CMP 500 --%>
        <label for="BillingYearMonth">
          Billing Year / Month.:</label>
        <%: Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, TransactionMode.BillingHistory, true)%>
      </div>
      <div>
        <%-- CMP 500 --%>
        <label for="BillingPeriod">
          Billing Period:</label>
        <%: Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.InvoiceSearch,null, true)%>
      </div>
          <div>
        <label for="InvoiceNumber">Invoice Number:</label>
        <%:Html.TextBoxFor(model => model.InvoiceNumber, new {maxLength = 10}) %>        
      </div>
      <div>
        <%-- CMP 500 --%>
        <label for="MemberCode">
           Member Code:</label>
        <%:Html.TextBoxFor(model => model.BilledMemberCode, new { @class= "autocComplete" }) %>
        <%:Html.TextBoxFor(model => model.BilledMemberId, new { @class= "hidden" }) %>
      </div>
    </div>
    <div>
      <div>
        <label for="MemoType">Transaction Type:</label>
        <%:Html.CargoMemoTypeDropdownlistFor(model => model.MemoTypeId)%>        
      </div>
      <div>
        <label for="MemoNumber">Memo Number:</label>
        <%:Html.TextBoxFor(model => model.MemoNumber, new {maxLength = 11}) %>        
      </div>
      <div>
        <label for="OurReference">Rejection Stage:</label>
        <%:Html.RejectionStageDropdownListFor(model => model.RejectionStageId,TransactionMode.BillingHistory)%>        
      </div>
      <div>
        <label for="ReasonCode">Reason Code:</label>
        <%:Html.TextBoxFor(model => model.ReasonCodeId, new { maxLength = 3, @class = "autocComplete upperCase" })%>        
      </div>
      <div>
        <label for="IssuingAirline">Issuing Airline:</label>
        <%: Html.TextBoxFor(model => model.IssuingAirline, new { maxLength = 10, @class = "autocComplete populated" })%>  
      </div>      
  </div>
  <div>
      <div style="display:none;">
        <label for="TransactionStatus">Transaction Status:</label>
        <%: Html.TransactionStatusDropdownListFor(model => model.TransactionStatusId) %>  
      </div>   
      <div>
        <label for="AwbSerialNumber">AWB Serial Number:</label>
        <%:Html.TextBoxFor(model => model.AwbSerialNumber, new { @maxLength = 7, @minLength = 7 })%>        
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
