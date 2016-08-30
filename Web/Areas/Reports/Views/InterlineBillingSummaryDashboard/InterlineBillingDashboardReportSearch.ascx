<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {
        $('#Month').val('<%=ViewData["currentMonth"]%>');
        $('#Year').val('<%=ViewData["currentYear"]%>');
        $('#Period').val('<%=ViewData["PeriodNo"] %>');

    });
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Clearance Year
                </label>
                <%: Html.BillingYearDropdownListFor(model => model.Year)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Clearance Month
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.Month)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Period No.
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.Period)%>
            </div>
        </div>
        <div>
            <div>
                <label>
                    IS-Member Code</label>
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @class = "autocComplete" })%>
                <%:Html.HiddenFor(model => model.BilledEntityId)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Settlement Method</label>
                <%: Html.SettlementMethodStatusropdownList(ControlIdConstants.SettlementMethodStatusId,-1)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Currency Code:</label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
        </div>
        <div>
            <div>
                <label>
                    % of Varience Threshold</label>
                <%:Html.TextBoxFor(model => model.PercentVarianceThreshold)%>
                
            </div>
            <div>
                <label>
                  Amount Varience Threshold</label>
             <%:Html.TextBoxFor(model => model.AmountVarianceThreshold)%>
            </div>
           
        </div>
    </div>
    <div class="clear">
    </div>
</div>
