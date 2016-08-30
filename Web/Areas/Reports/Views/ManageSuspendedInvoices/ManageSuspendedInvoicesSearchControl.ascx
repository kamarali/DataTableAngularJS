<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Base.InvoiceBase>" %>
<script type="text/javascript">

  $(document).ready(function () {
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);

  });
</script>
<h2>
  Search Criteria</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          From Billing Year:</label>
        <%: Html.BillingYearDropdownListFor(searchCriteria => searchCriteria.BillingYear,new{@Id="fromBillingYear"})%>
      </div>
      <div>
        <label>
         To Billing Year:</label>
        <%: Html.BillingYearDropdownListFor(searchCriteria => searchCriteria.BillingYear,new{@Id="toBillingYear"})%>
      </div>
      <div>
        <label>
          From Billing Month:</label>
        <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonth,new{@Id="fromBillingMonth"})%>
      </div>
      <div>
        <label>
          To Billing Month:</label>
        <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.BillingMonth,new{@Id="toBillingMonth"})%>
      </div>
      <div>
        <label>
          From Billing Period:</label>
        <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.BillingPeriod, new { @Id = "fromBillingPeriod" })%>
      </div>
      <div>
        <label>
          To Billing Period:</label>
        <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.BillingPeriod, new { @Id = "toBillingPeriod" })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Settlement Method Indicator:</label>
        <%-- CMP #624: ICH Rewrite-New SMI X 
        Description: 2.25 IS-WEB Manage Suspended invoices
        Query criteria Settlement Method Indicator should include a new value, 
        i.e. description related to SMI “X” (expected to be “ICH Special Agreement”) --%>
        <%: Html.SettlementMethodDropdownList("SettlementMethodId", (Model != null)? Model.SettlementMethodId.ToString() : "0", 1, 2, 8)%>
      </div>
      <div>
        <label>
          Resubmission Status:</label>
           <%--SCP#449357 : Resubmission Status information--%>
        <%: Html.ResubmissionStatusDropdownListFor("ResubmissionStatusId", (Model != null) ? Model.ResubmissionStatusId.ToString() : "0", 1, 2, 3, 4)%>
      </div>
      <div>
        <label>
          Billed Entity Code:</label>
        <%: Html.TextBoxFor(model => model.BilledMemberText, new { @id = "BilledMemberText", @Class = "autocComplete textboxWidth" })%>
        <%: Html.HiddenFor(model => model.BilledMemberId, new { @id = "BilledMemberId" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
