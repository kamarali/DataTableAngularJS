<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.Cargo.RMBMCMDetailsModel>" %>
<script type="text/javascript">
    $(document).ready(function () {

        $('#ClearanceMonth').val('<%=ViewData["CurrentMonth"] %>');
        $('#billingYear').val('<%=ViewData["CurrentYear"] %>');
        /*$('#PeriodNo').val('<%=ViewData["CurrentPeriod"] %>');*/
       
    });
</script>
<h2>
    Search Criteria</h2>

<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label><span style="color: Red">*</span> Billing Year:</label>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.billingYear)%>
            </div>
            <div>
                <label><span style="color: Red">*</span> Billing Month:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.ClearanceMonth)%>
            </div>
            <div>
                <label>Period No:</label>
                <%: Html.StaticBillingPeriodDropdownListAllFor(model => model.PeriodNo)%>
            </div>
            <div>
                <label><span style="color: Red">*</span> Billing Type:</label>
                <%: Html.BillingTypeDropdownListFor(model => model.BillingType)%>
            </div>
        </div>
        <div>
            <div>
                <label>Settlement Method:</label>
                <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
            <div>
                <label>Memo Type:</label>
                <%:Html.MemoTypeReportDropdownlistForReport(model => model.MemoType)%>
            </div>
            <div>
                <label>Submission Method:</label>
                <%--SCP#425722 - SRM: MISC search screen has Submission Method - AUTO BILLING--%>
                <%:Html.SubmissionMethodDropDownListFor(model => model.DataSource, isForCargo: true)%>
            </div>
            <div>
                <label>Member Code:</label>
                <%:Html.TextBoxFor(model => model.AirlineCode, new { @class = "autocComplete" })%>
                <%:Html.HiddenFor(model=>model.AirlineId) %>
            </div>
        </div> 
        <div>
            <div>
                <label>Invoice Number:</label>
                <%: Html.TextBoxFor(model => model.InvoiceNumber, new { maxLength = 11, @id = "InvoiceNumber" })%>
            </div>
            <div>
                <label><span style="color: Red">*</span> Output:</label>
                <%: Html.OutputDropDownList("Output")%>
            </div>
            <div>
                <label>RM/BM/CM Number:</label>
                <%: Html.TextBoxFor(model => model.RMBMCMNumber)%>
            </div>
        </div>
    </div>
    <div class="clear">
  </div>
</div>