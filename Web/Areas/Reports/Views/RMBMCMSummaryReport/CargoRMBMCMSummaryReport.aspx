<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Cargo :: <%=ViewData["BillingTypeWords"] %> - RM BM CM Summary
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "CargoRMBMCMSummaryReport" }); %>
    <h1>
    
        Cargo : <%=ViewData["BillingTypeWords"] %> - RM BM CM Summary
  <%-- <%=ViewData["BillingType"] %> - this is type R / P--%>
    </h1>
    <div>
        <% Html.RenderPartial("CargoRMBMCMReportSearch", ViewData); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Download To CSV" onclick="CargoRMBMCMSummaryReport(this);" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/CargoRMBMCMSummaryReport.js")%>"></script>
    <script type="text/javascript">

        var billingTypeVal = '<%:ViewData["BillingTypeId"]%>';
        var payableRedirectUrl = '<%:Url.Action("CargoPayablesReport", "RMBMCMSummaryReport")%>';
        var receivableRedirectUrl = '<%:Url.Action("CargoReceivablesReport", "RMBMCMSummaryReport") %>';

        function CargoRMBMCMSummaryReport(e) {
            // set redirection URL depending on Billing Type.
            if (billingTypeVal == 1) {
                ValidateCargoRMBMCMSummaryReport("CargoRMBMCMSummaryReport", payableRedirectUrl, '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","RMBMCMSummaryReport")%>');
            } else {
                ValidateCargoRMBMCMSummaryReport("CargoRMBMCMSummaryReport", receivableRedirectUrl, '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","RMBMCMSummaryReport")%>');
            }
        }
        $(document).ready(function () {
            $("#AirlineCode").change(function () {
                if ($("#AirlineCode").val() == '') {
                    $("#AirlineId").val("");
                }
            });
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 46 and 51 */
            registerAutocomplete('AirlineCode', 'AirlineId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
</asp:Content>