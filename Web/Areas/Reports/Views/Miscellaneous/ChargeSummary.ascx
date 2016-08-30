<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Model.Enums"  %>
<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {

        $('#ToYear,#FromYear').val('<%=ViewData["PerviousYear"]%>');
        $('#ToMonth,#FromMonth').val('<%=ViewData["PerviousMonth"]%>');
        $('#ToPeriod,#FromPeriod').val('<%=ViewData["PerviousPeriod"]%>');
        $('#BillingType').val('<%=ViewData["BillingTypeId"]%>');        
        registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        registerAutocomplete('BillingEntityCode', 'BillingEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
    });
  
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
              
            <div>
            <div>
                <label>
                    <span style="color: Red">*</span>From Billing Year:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.FromYear)%>
            </div>
            <div>
                <label>
                   <span style="color: Red">*</span>From Billing Month:</label>
                <%:Html.BillingMonthDropdownListFor(model => model.FromMonth)%>
            </div>
            <div>
                <label>
                   <span style="color: Red">*</span>From Period :
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.FromPeriod)%>
            </div>
           
            <%if ((ViewData["BillingTypeText"]).Equals("Receivables"))
              { %>
            <div>
                <label>
                     Submission Method: 
                </label>
                <%--SCP#425722 - SRM: MISC search screen has Submission Method - AUTO BILLING--%>
                <%:Html.SubmissionMethodDropDownListFor(model => model.SubmissionMethod, isForMiscUatp: true)%>
            </div>
            <% } %>
            
            <div>
                <label>
                    Settlement Method Indicator:
                </label>
                <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>

           </div>
        <div>
            <div>
                <label>
                    <span style="color: Red">*</span>To Billing Year:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.ToYear)%>
            </div>
            <div>
                <label>
                   <span style="color: Red">*</span>To Billing Month:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.ToMonth)%>
            </div>
            <div>
                <label>
                   <span style="color: Red">*</span>To Period :
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.ToPeriod)%>
            </div>

            <div>                
                <label>
                  <%=ViewData["MemberType"] %> Member Code:</label>
                <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "BilledEntityCode", @class = "autocComplete textboxWidth" })%>
                <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "BilledEntityCodeId", @class = "hidden" })%>
                <%:Html.HiddenFor(model => model.BillingType, new { @id = "BillingType", @class = "hidden"})%>
            </div>
            <div>
            <%--CMP#663 - MISC Invoice Summary Reports - Add 'Transaction Type'--%>
                <label>
                    Transaction Type :
                </label>
                <%: Html.InvoiceTypeDropDownListFor(model => model.InvoiceType)%>
            </div>
        </div>
        <div>
            <%--CMP521 : Clearance Amount Info in Payables Miscellaneous Invoice Summary Report--%>
            <%--<%if ((ViewData["BillingTypeText"]).Equals("Receivables"))
              { %>--%>
              <div>
                <label>
                    Clearance Currency:
                </label>
                <%: Html.CurrencyDropdownListFor(model => model.CurrencyCode)%>
            </div>
            <%--<% } %>--%>

            <div id="chargeCategorydiv" >
                 <label for="ChargeCategory"> Charge Category:</label>
                 <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
                 <%:Html.ChargeCategoryDropdownListWithAllFor(model => model.ChargeCategory, isIncludeInactive: true)%>
            </div>
 
      
        </div>     
       
    </div>
    <div class="clear">
    </div>
</div>