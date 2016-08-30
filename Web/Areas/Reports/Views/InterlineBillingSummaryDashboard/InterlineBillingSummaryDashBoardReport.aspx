<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Reports :: Interline Billing Summary Dashboard Report 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">


    <%Html.BeginForm("", "", FormMethod.Post, new { id = "InterlineBillingSummaryDashboardReport" }); %>
<h2> Interline Billing Summary Dashboard Report</h2>
    <div>
        <% Html.RenderPartial("InterlineBillingDashboardReportSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report" onclick="InterlineBillingSummaryDashboardReport();" />
       
    </div>
    <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

 <%--  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/InterlineBillingSummaryDashboardReport.js")%>"></script>--%>
    <script type="text/javascript">

        function InterlineBillingSummaryDashboardReport() {
            ValidateReport("InterlineBillingSummaryDashboardReport");
        }
        $(document).ready(function () {
            registerAutocomplete('BilledEntityCode', 'BilledEntityId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
</asp:Content>
