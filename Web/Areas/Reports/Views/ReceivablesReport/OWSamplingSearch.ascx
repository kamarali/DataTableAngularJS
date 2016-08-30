<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>

<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {

//        $('#Year').val('<%=ViewData["currentYear"]%>');
//        $('#Month').val('<%=ViewData["currentMonth"]%>');
        $('#BillingType').val('<%=ViewData["BillingTypeId"]%>');
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 33 and 39 */
        registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        registerAutocomplete('BillingEntityCode', 'BillingEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        //To display USD as default selected currency in dropdown, as per Shambhu and Robin
        $('#CurrencyId').val('840');
    });
  
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Provisonal Billing Year:
                </label>
                <%:Html.BillingYearLastThreeDropdownListFor(model => model.Year)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Provisonal Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.Month) %>
            </div>
            <div>                
                <label>
                    <%=ViewData["MemberType"] %> Member Code:</label>
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "BilledEntityCode", @class = "autocComplete" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "BilledEntityCodeId", @class = "hidden" })%>
                <%:Html.HiddenFor(model => model.BillingType, new { @id = "BillingType", @class = "hidden"})%>
            </div>
             <div>
                <label>
                    <span class="required">* </span>Currency Code:
                </label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
        </div>      
       
    </div>
    <div class="clear">
    </div>
</div>