<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {

        $('#Year').val('<%=ViewData["LastClosedYear"]%>');
        $('#Month').val('<%=ViewData["LastClosedMonth"]%>');
        $('#PeriodNo').val('<%=ViewData["LastClosedPeriod"] %>');
        $('#CurrencyId').val('840');
        $('#ComparisonPeriod').val('1');

        registerAutocomplete('EntityCode', 'EntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

    });
  
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label><span class="required">* </span>Billing Year:</label>
                    <%:Html.BillingYearLastThreeDropdownListFor(model => model.Year)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Month:</label>
                <%:Html.BillingMonthDropdownList("Month",0) %>
            </div>              
            <div>
                <label><span class="required">* </span>Billing Period:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.PeriodNo)%>
            </div>
            <div>
                <label><span class="required">* </span>Currency Code:</label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
        </div>
        <div>
            <div>
                <label>Billing Member Code:</label>
                <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: Increasing field size by specifying in-line width
                Ref: 3.5 Table 19 Row 20 -->
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "EntityCode", @class = "textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "EntityCodeId", @class = "hidden" })%>
            </div>
            <div>
                <label>Settlement Method:</label>
                <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
            <div>
                <label><span class="required">* </span>Comparison Period:</label>
                <%: Html.ComparisonPeriodDropdownList("ComparisonPeriod", 0)%>
            </div>
            <div>
                <label>Totals Required:</label>
                <%:Html.CheckBoxFor(model => model.IsTotalsRequired)%>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
