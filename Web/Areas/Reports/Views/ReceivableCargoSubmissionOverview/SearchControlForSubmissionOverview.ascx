<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverviewUI>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<script type="text/javascript">
    $(document).ready(function () {
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 43 and 48 */
        registerAutocomplete('BillingEntity', 'EntityId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        registerAutocomplete('BilledEntity', 'EntityId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);

        $('#BillingYearFrom').val('<%=ViewData["currentYear"]%>');
        $('#BillingMonthFrom').val('<%=ViewData["currentMonth"]%>');
        $('#PeriodNoFrom').val('<%=ViewData["currentPeriod"] %>');
        $('#BillingYearTo').val('<%=ViewData["currentYear"]%>');
        $('#BillingMonthTo').val('<%=ViewData["currentMonth"]%>');
        $('#PeriodNoTo').val('<%=ViewData["currentPeriod"] %>');
    });
    
    
</script>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <div>
                <label><span class="required">* </span>Billing Year From:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.BillingYearFrom)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Year To:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.BillingYearTo)%>
            </div>
            <%if ((ViewData["BillingType"]).Equals(Iata.IS.Model.Enums.BillingType.Payables))
              { %>
            <div>
                <label>Billing Member Code:</label>
                <%:Html.TextBoxFor(model => model.BillingEntity, new {@class="autocComplete" })%>
                <%:Html.TextBoxFor(model => model.EntityId, new { @class = "hidden" })%>
            </div>
            <% } 
               else 
               { %>
            <div>
                <label>Billed Member Code:</label>
                <%:Html.TextBoxFor(model => model.BilledEntity, new { @class = "autocComplete" })%>
                <%:Html.TextBoxFor(model => model.EntityId, new { @class = "hidden" })%>
            </div>
            <% } %>
        </div>
        <div>
            <div>
                <label><span class="required">* </span>Billing Month From:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.BillingMonthFrom) %>
            </div>
            <div>
                <label><span class="required">* </span>Billing Month To:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.BillingMonthTo) %>
            </div>
            <div>
                <label>Settlement Method:</label>
                <%:Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
        </div>
        <div>
            <div>
                <label><span class="required">* </span>Billing Period From:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.PeriodNoFrom) %>
            </div>
            <div>
                <div>
                    <label><span class="required">* </span>Billing Period To:</label>
                    <%: Html.StaticBillingPeriodDropdownListFor(model => model.PeriodNoTo) %>
                </div>
            </div>
            <div>
                <label><span class="required">* </span>Output:</label>
                <%:Html.OutputDropDownSubmissionOverviewList("Output")%>
                <%= Html.Hidden("BillingType", ViewData["BillingType"])%>
            </div>
        </div>
        </div>
    </div>
<div class="clear">
</div>
</div>