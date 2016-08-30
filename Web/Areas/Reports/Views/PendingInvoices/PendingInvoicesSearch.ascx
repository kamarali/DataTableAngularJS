<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {
        $('#Month').val('<%=ViewData["currentMonth"]%>');
        $('#Year').val('<%=ViewData["currentYear"]%>');
        $('#Period').val('<%=ViewData["currentPeriodNo"] %>');

    });
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Billing Year:
                </label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.Year)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.Month)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.Period)%>
            </div>
            <div>
                <label>
                    Billing Category:
                </label>
                <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategory) %>
            </div>
        </div>
        <div>
            <div>
                <label>
                    <span class="required">* </span>Settlement Method:</label>
                <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
            <div>
                <label>
                    Error Type:</label>
                <%: Html.ErrorTypeDropdownList(ControlIdConstants.ErrorTypeId, -1)%>
            </div>
            <div>
                <label>
                    Total Required:</label>
                <%: Html.CheckBoxFor(model => model.IsTotalsRequired, new { @Checked="checked"})%>
            </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
