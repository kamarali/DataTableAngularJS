<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Web.Reports.SuspendedInvoice.MemberSuspendedInvoice>" %>
<script type="text/javascript">

    $(document).ready(function () {
        registerAutocomplete('BilledEntityName', 'BilledEntityCode', '<%:Url.Action("GetAllMemberList", "Data", new { area = "" })%>', 0, true, null);
        $("#FromBillingYear option[value='-1']").text('Please Select');
        $("#FromBillingMonth option[value='-1']").text('Please Select');
        $("#FromBillingPeriod option[value='-1']").text('Please Select');
        $("#ToBillingYear option[value='-1']").text('Please Select');
        $("#ToBillingMonth option[value='-1']").text('Please Select');
        $("#ToBillingPeriod option[value='-1']").text('Please Select');
    });
</script>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span> Billing Year From:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(searchCriteria => searchCriteria.FromBillingYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Month From:</label>
                <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.FromBillingMonth)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Period From:</label>
                <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.FromBillingPeriod)%>
            </div>
        </div>
        <div>
            <div>
                <label>
                    <span class="required">* </span>Billing Year To:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(searchCriteria => searchCriteria.ToBillingYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Month To:</label>
                <%: Html.MonthsDropdownListFor(searchCriteria => searchCriteria.ToBillingMonth)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Period To:</label>
                <%: Html.InvoicePeriodDropdownListForProcessingDashBoard(searchCriteria => searchCriteria.ToBillingPeriod)%>
            </div>
        </div>
    <div>
        <div>
            <label><span class="required"></span>Clearing House:</label>
            <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
        </div>
        <div>
            <label>
                Suspended Member Code:</label>
            <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.5 Table 19 Row 21 -->
            <%: Html.TextBoxFor(model => model.BilledEntityName, new { @id = "BilledEntityName", @Class = "autocComplete textboxWidth" })%>
            <%: Html.HiddenFor(model => model.BilledEntityCode, new { @id = "BilledEntityCode" })%>
            <%: Html.HiddenFor(model => model.SuspendedEntityCode)%>
            <%--Hidden field for IATA & ACH Member--%>
             <%: Html.HiddenFor(model => model.IataMemberId)%>
              <%: Html.HiddenFor(model => model.AchMemberId)%>
        </div>
        <div>
            <label>
                Billing Category:</label>
            <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
        </div>
    </div>
</div>
<div class="clear">
</div>
</div> 
