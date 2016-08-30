<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<h2>Search Criteria</h2>
<script type="text/javascript">
  $(document).ready(function () {
//        CMP#596: CIT Bug ID: 9443 System is not populating Type B Member in Financial Controller >> Interline Billing Summary.
        registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        registerAutocomplete('BillingEntityCode', 'BillingEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        $('#Year').val('<%=ViewData["PreviousClosedYear"]%>');
        $('#Month').val('<%=ViewData["PreviousClosedMonth"]%>');
        $('#PeriodNo').val('<%=ViewData["PreviousClosedPeriod"]%>');
        $('#CurrencyId').val('840');
        //alert($('#PeriodNo').val());
    });
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label> <span class="required">* </span>Billing Year:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.Year)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Month:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.Month) %>
            </div>
            <div>
                <label><span class="required">*</span>Billing Period:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.PeriodNo) %>
            </div>
            <div>
                <label>Settlement Method:</label>
                 <%:Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
        </div>
        <div>
            <div>
                <label><span class="required">* </span>Currency Code:</label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
            <div>
                <label>Member Code:</label>
                <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: Increasing field size by specifying in-line width
                Ref: 3.5 Table 19 Row 19 -->
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "BilledEntityCode", @class = "autocComplete textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "BilledEntityCodeId", @class = "hidden" })%>
                <%:Html.HiddenFor(model => model.BillingType, new { @id = "BillingType", @class = "hidden"})%>
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