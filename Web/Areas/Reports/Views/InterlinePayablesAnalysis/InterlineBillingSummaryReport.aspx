<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Report :: Interline Billing Summary
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Interline Billing Summary</h1>
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "InterlineBillingSummaryReportId" }); %>
    <h2>
        <span id="Span1"></span>
    </h2>
    <div>
        <% Html.RenderPartial("InterlineBillingSummaryReportSearch", ViewData); %>
    </div>

        <div />

    <div class="buttonContainer">
        <input type="submit" id="Submit1" class="primaryButton" value="Generate Report"
            onclick="InterlineBillingSummaryReport();" />
    </div>

     <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/InterlineBillingSummaryReport.js")%>"></script>
    <script type="text/javascript">
        function InterlineBillingSummaryReport() {
            ValidateInterlineBillingSummaryReport("InterlineBillingSummaryReportId");
        }
        $(document).ready(function () {

            // registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
            $("#BilledEntityCode").change(function () {
                if ($("#BilledEntityCode").val() == '') {
                    $("#BilledEntityCodeId").val("");
                }
            });

        });
    </script>
</asp:Content>
