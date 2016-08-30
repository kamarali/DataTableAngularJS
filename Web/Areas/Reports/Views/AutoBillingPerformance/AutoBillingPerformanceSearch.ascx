<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<h2>
  Search Criteria</h2>
<script type="text/javascript">
  $(document).ready(function () {

    $('#Year').val('<%=ViewData["currentYear"]%>');
//    $('#Month').val('<%=ViewData["currentMonth"]%>');
    //To display USD as default selected currency in dropdown, as per Shambhu and Robin
    $('#CurrencyId').val('840');
  });
  
</script>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span class="required">* </span>Clearance Year:
        </label>
        <%: Html.BillingYearTwoDropdownListFor(model => model.Year)%>
      </div>
      <div>
        <label>
          <span class="required">* </span>Clearance Month:
        </label>
        <%: Html.BillingMonthDropdownListFor(model => model.Month)%>
      </div>
    </div>
    <div>
      <div>
        <label>
          Memebr Code:</label>
        <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @class = "autocComplete" })%>
        <%:Html.HiddenFor(model => model.BilledEntityId)%>
      </div>
      <div>
        <label>
          <span class="required">* </span>Currency Code:</label>
        <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
