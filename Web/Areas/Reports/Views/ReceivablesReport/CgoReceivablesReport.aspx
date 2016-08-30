<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.Cargo.ReceivablesRMBMCMSummaryUI>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Cargo :: Receivables - RM BM CM Summary
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "CargoRMBMCMSummaryReceivalbesReport" }); %>
    <h2>
        Receivables - RM BM CM Summary
    </h2>
    <div>
        <% Html.RenderPartial("CgoReceivableReportSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report" onclick="CargoRMBMCMSummaryReport();" />
       
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/CargoRMBMCMSummaryReport.js")%>"></script>
    <script type="text/javascript">

        function CargoRMBMCMSummaryReport() {
            ValidateReport("CargoRMBMCMSummaryReceivalbesReport");
        }
        $(document).ready(function () {
            registerAutocomplete('AirlineCode', 'AirlineId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
</asp:Content>
