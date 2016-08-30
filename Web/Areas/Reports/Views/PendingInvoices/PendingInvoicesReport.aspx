<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Financial Controller :: Pending Invoices In Error Report
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "PendingInvoicesInErrorReport" }); %>
    <h1>
        Pending Invoices In Error Report
    </h1>
    <div>
        <% Html.RenderPartial("PendingInvoicesSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="PendingInvoicesInErrorReport();" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/PendingInvoicesInErrorReport.js")%>"></script>
    <script type="text/javascript">

        function PendingInvoicesInErrorReport() {
            ValidateReport("PendingInvoicesInErrorReport","<%:Url.Action("ValidateBillingMonthYearPeriodSearch","PendingInvoices")%>");
        }
        $(document).ready(function () {

        });
    </script>
</asp:Content>
