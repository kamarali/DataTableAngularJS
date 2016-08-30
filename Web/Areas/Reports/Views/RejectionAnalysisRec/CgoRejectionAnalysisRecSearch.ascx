<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {

        $('#Year').val('<%=ViewData["currentYear"]%>');
        $('#Month').val('<%=ViewData["currentMonth"]%>');

        $("#EntityCode").change(function () {
            if ($("#EntityCode").val() == '') {
                $("#EntityCodeId").val("");
            }
        });
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 45 and 50 */
        registerAutocomplete('EntityCode', 'EntityCodeId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        //To display USD as default selected currency in dropdown, as per Shambhu and Robin
        $('#CurrencyId').val('840');
    });
  
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
    <%--CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
    With this CMP, the following reports will be enhanced and Member users will be able to generate these reports for the range of Billing Months--%>
        <div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Year:
                </label>
                <%:
                Html.BillingYearLastThreeDropdownListFor(model => model.FromYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.FromMonth)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>To Billing Year:
                </label>
                <%:
                Html.BillingYearLastThreeDropdownListFor(model => model.ToYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span> To Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.ToMonth)%>
            </div>           
             
        </div>    
        <div>           
             <div>
                <label>
                      <%=ViewData["MemberCodeField"] %> 
                 </label>
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "EntityCode" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "EntityCodeId", @class = "hidden" })%>
                <%=Html.TextBox("IsPayableReport", ViewData["IsPayableReport"], new { @class = "hidden" })%>
            </div>
             <div>
                <label>
                    <span class="required">* </span>Currency:
                </label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyId)%>
            </div>
        </div>        
    </div>
    <div class="clear">
    </div>
</div>