<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.Cargo.ReceivablesRMBMCMSummaryUI>" %>

<h2>
    Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {

        $('#FromBillingYear').val('<%=ViewData["currentYear"]%>');
        $('#FromBillingMonth').val('<%=ViewData["currentMonth"]%>');
        $('#FromPeriod').val('<%=ViewData["Period"] %>');
    });
  
</script>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>Billing Year:
                </label>
                <%:
                Html.BillingYearDropdownListFor(model => model.BillingYear)%>
            </div>
            <div>
                <label>
                    <span class="required">* </span>Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.BillingMonth)%>
            </div>
            <div>
                <label>
                    Billing Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.PeriodNo)%>
            </div>
        </div>
        <div>
            <div>
                <label><span class="required">* </span>
                    Settlement Method
                </label>
                <%: Html.SettlementMethodStatusropdownList(ControlIdConstants.SettlementMethodStatusId,-1)%>
            </div>
            <div>
                <label>
                    Memo Type</label>
                <%:Html.MemoTypeReportDropdownlistForReport(model => model.MemoType)%>
            </div>
            <div>
                <label><span class="required">* </span>
                    Data Source</label>
                    <%: Html.SubmissionMethodDropDownListFor(model=>model.DataSource) %>
               <%-- <%:Html.TextBoxFor(model=>model.DataSource)%>--%>
            </div>
        </div>
        <div>
            <div>
                <label>
                    Billed Member Code</label>
                <%:Html.TextBoxFor(model => model.AirlineCode, new { @class = "autocComplete" })%>
                <%:Html.HiddenFor(model => model.AirlineId)%>
            </div>
            <div>
                <label>
                    Invoice Number</label>
                <%: Html.TextBoxFor(model => model.InvoiceNumber, new { maxLength = 11, @id = "InvoiceNo" })%>
            </div>
            <div>
                    <label>
                        RM/BM/CM Number</label>
                    <%: Html.TextBoxFor(model => model.RMBMCMNumber, new { @id = "RMBMCMNo" })%>
                </div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
