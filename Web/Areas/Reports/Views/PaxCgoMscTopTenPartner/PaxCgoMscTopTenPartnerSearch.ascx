<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<h2>
    Search Criteria</h2>

    <script type="text/javascript">
        $(document).ready(function () {

            $('#Year').val('<%=ViewData["currentYear"]%>');
            $('#Month').val('<%=ViewData["currentMonth"]%>');
            //To display USD as default selected currency in dropdown, as per Shambhu and Robin
            $('#CurrencyId').val('840');
//            registerAutocomplete('EntityCode', 'EntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

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
                <%=Html.TextBox("IsPayableReport", ViewData["IsPayableReport"], new { @class = "hidden" })%>
            </div>
            <div>
                <label><span class="required">* </span>Billing Category:</label>
                <%:Html.BillingCategoryDropdownListFor(model => model.BillingCategory,"",null,false)%>
            </div>
            <%--<div>
                <label>
                      <%=ViewData["MemberCodeField"] %> 
                 </label>
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "EntityCode" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "EntityCodeId", @class = "hidden" })%>
               
            </div>--%>
             <div>
                <label><span class="required">* </span>Currency:</label>
                <%:Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
        </div>      
       
    </div>
    <div class="clear">
    </div>
</div>