<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.PayablesReport.PayablesReportModel>" %>
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
<table>
    <tr>
    <td style= "width:750px;">
    <div class="fieldContainer horizontalFlow">
        <div style= "width:750px; padding:0px;">
            <div style= "width:250px;">
                <label>
                    <span class="required">* </span>Billing Year:
                </label>
                <%:
                Html.BillingYearLastThreeDropdownListFor(model => model.FromBillingYear)%>
            </div>
            <div style="width:250px;">
                <label>
                    <span class="required">* </span>Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.FromBillingMonth)%>
            </div>
            <div style="width:250px;">
                <label>
                   Billing Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListAllFor(model => model.FromPeriod)%>
            </div>            
        </div>
        <div style="width:750px;">
            <div style="width:250px;">
                <label>
                    Settlement Method
                </label>
                <%: Html.SettlementMethodDropdownListForReport(model => model.SettlementMethod, -1)%>
            </div>
            <div style="width:250px;">
                <label>
                    Memo Type</label>
                <%:Html.MemoTypeReportDropdownlistForReport(model => model.MemoType, new { @id = "MemoType" , onChange = "MemoType_OnChange(this);"})%>
            </div>            
            <div style="width:250px;">
                <div>
                    <label>
                        RM/BM/CM Number</label>
                    <%: Html.TextBoxFor(model => model.RMBMCMNo, new { @id = "RMBMCMNo" })%>
                </div>
            </div>
        </div>
        <div style="width:750px;">
            <div style="width:250px;">
                <label>
                    Member Code</label>
                <%:Html.TextBoxFor(model => model.BillingEntityCode, new { @id = "BillingEntityCode", @class = "autocComplete" })%>
                <%:Html.TextBoxFor(model => model.BillingEntityCodeId, new { @id = "BillingEntityCodeId", @class = "hidden" })%>
            </div>
            <div style="width:250px;">
                <div>
                    <label>
                        Invoice Number</label>
                    <%: Html.TextBoxFor(model => model.InvoiceNo, new { maxLength = 11, @id = "InvoiceNo" })%>
                </div>            
        </div>
    </div>
     </td>
    <td style="width:250px; padding:0px; vertical-align:0px; padding-top:1em;">
         <%--CMP-523: Source Code in Passenger RM BM CM Summary Reports--%>
        <div>
        <label>
            Source Code(s):</label>
        <%: Html.ListBox("SourceCode", (MultiSelectList)ViewData["SourceCodeList"], new { @id = "SourceCode", size = "7" })%>
    </div>
    </td>
    </tr>
    </table>
    <div class="clear">
    </div>
</div>