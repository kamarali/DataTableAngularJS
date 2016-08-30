<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {
      
//        $('#CurrencyId').val(840);
        $('#BillingType').val('<%=ViewData["BillingTypeId"]%>');
        $('#ToMonth').val('<%=ViewData["currentMonth"]%>');
        $('#ToYear').val('<%=ViewData["currentYear"]%>');
        $('#FromMonth').val('<%=ViewData["currentMonth"]%>');
        $('#FromYear').val('<%=ViewData["currentYear"]%>');
        $('#ToPeriod').val('<%=ViewData["PeriodNo"] %>');
        $('#FromPeriod').val('<%=ViewData["PeriodNo"] %>');
        //To display USD as default selected currency in dropdown, as per Shambhu and Robin
        $('#CurrencyId').val('840');
    });
 </script>
 <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label><span class="required">* </span>Billing Year From:</label>
               <%: Html.BillingYearLastThreeDropdownListFor(model => model.FromYear)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Month From:</label>
              <%: Html.BillingMonthDropdownListFor(model => model.FromMonth)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Period From:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.FromPeriod)%>
            </div>
        </div>
        <div>
            <div>
                <label><span class="required">* </span>Billing Year To:</label>
               <%: Html.BillingYearLastThreeDropdownListFor(model => model.ToYear)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Month To:</label>
              <%: Html.BillingMonthDropdownListFor(model => model.ToMonth)%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Period To:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.ToPeriod)%>
            </div>
        </div>
        <div>
            <div>
                <label> <%=ViewData["MemberType"]%> Member Code:</label>
                <%:Html.TextBoxFor(model => model.AirlineCode, new { @class = "autocComplete" })%>
                <%:Html.HiddenFor(model => model.AirlineId)%>
            </div>
            <div>
                <label>Settlement Method:</label>
                <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
            <div>
                <label><span class="required">* </span>Currency Code:</label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
                <%: Html.HiddenFor(model=>model.BillingType) %>
           </div>
        </div>
      </div>
<div class="clear">
</div>
</div>