<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {
        $('#BillingType').val('<%=ViewData["BillingTypeId"]%>');
        $('#Year').val('<%=ViewData["currentYear"]%>');
        $('#Month').val('<%=ViewData["currentMonth"]%>');
        $('#Period').val('<%=ViewData["PeriodNo"] %>');
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
                <%: Html.BillingMonthDropdownListFor(model => model.Month)%>
            </div>
            <div>
                <label>Billing Period:</label>
                <%: Html.StaticBillingPeriodDropdownListAllFor(model => model.Period)%>
            </div>
        </div>
        <div>
            <div>
                <label>Settlement Method:</label>
                <%: Html.SettlementMethodDropdownListForReport(model => model.SettlementMethodIndicatorId, -1)%>
            </div>
            <div>
                <label>Memo Type:</label>
                <%:Html.MemoTypeReportDropdownlistForReport(model => model.MemoType)%>
            </div>
            <div>
                 <label>RM/BM/CM Number:</label>
                    <%: Html.TextBoxFor(model => model.RmbmcmNumber, new { @id = "RMBMCMNo" })%>
                    <%--<%: Html.Hidden("BillingType",<%=ViewData["BillingType"]%>)%>--%>
                    <%: Html.HiddenFor(model=> model.BillingType) %>
                </div>
        </div>
        <div>
            <div>
                <label><%=ViewData["MemberType"]%> Member Code:</label>
                <%:Html.TextBoxFor(model => model.AirlineCode, new { @class = "autocComplete" })%>
                <%:Html.HiddenFor(model => model.AirlineId)%>
            </div>
            <div>
                <label>Invoice Number:</label>
                <%: Html.TextBoxFor(model => model.InvoiceNo, new { maxLength = 11, @id = "InvoiceNo" })%>
            </div>
           
           <%-- Updated 17/05/2013 Shivkumar hawale Updated for SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'  --%>
            <% if(Convert.ToInt32(ViewData["BillingTypeId"]) == 2) %>
             <%{ %>
                <div>
                    <label>Submission Method:</label>
                        <%--SCP#425722 - SRM: MISC search screen has Submission Method - AUTO BILLING--%>
                        <%: Html.SubmissionMethodDropDownListFor(model => model.SubmissionMethodId, isForCargo: true)%>
                   <%-- <%:Html.TextBoxFor(model=>model.DataSource)%>--%>
                </div>
            <% } %>
        </div>
    </div>
    <div class="clear">
    </div>
</div>