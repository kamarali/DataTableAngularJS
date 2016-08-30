<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Report :: <%=ViewData["BillingTypeText"] %> - Passenger Rejection Analysis - Non Sampling
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1><%=ViewData["BillingTypeText"] %> - Passenger Rejection Analysis - Non Sampling</h1>
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "PaxRejectionAnalysisNonSamplingReportId" }); %>
    <h1><span id="Span1"></span></h1>
    <div>
        <% Html.RenderPartial("~/Areas/Reports/Views/ReceivablesReport/PaxRejectionAnalysisNonSamplingSearch.ascx", ViewData); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="Submit1" class="primaryButton" value="Generate Report"
            onclick="PaxRejectionAnalysisNonSamplingReport();" />
    </div>
     <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/ReceivablesReport.js")%>"></script>
<script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
<script type="text/javascript">
    function PaxRejectionAnalysisNonSamplingReport() {
      ValidatePaxRejectionAnalysisNonSamplingReport("PaxRejectionAnalysisNonSamplingReportId", '<%:Url.Action("ValidateBillingMonthYearSearch","RejectionAnalysisRec")%>');
    }
    //Resolved UAT issue 6210
    $(document).ready(function () {

        $("#BilledEntityCode").change(function () {
            if ($("#BilledEntityCode").val() == '') {
                $("#BilledEntityCodeId").val("");
            }
        });

        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 32 and 38 */
        registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
    });
    //Resolved UAT issue 6210
    </script>
</asp:Content>
