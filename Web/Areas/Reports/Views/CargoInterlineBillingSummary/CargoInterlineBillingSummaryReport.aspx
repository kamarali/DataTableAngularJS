<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Cargo :: <%=ViewData["BillingTypeWords"] %> - Interline Billing Summary
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "CargoIntelineBillingSummaryReport" }); %>
    <h1>
	
        <%=ViewData["BillingTypeWords"] %> - Interline Billing Summary
  <%-- <%=ViewData["BillingType"] %> - this is type R / P--%>
    </h1>
    <div>
        <% Html.RenderPartial("CargoIntelineBillingReportSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report" onclick="CargoIntelineBillingSummaryReport();" />
       
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/CargoInterlineBillingSummaryReport.js")%>"></script>
    <script type="text/javascript">

        function CargoIntelineBillingSummaryReport() {
            ValidateReport("CargoIntelineBillingSummaryReport");
        }
        $(document).ready(function () {

            $("#AirlineCode").change(function () {
                if ($("#AirlineCode").val() == '') {
                    $("#AirlineId").val("");
                }
            });
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 44 and 49 */
            registerAutocomplete('AirlineCode', 'AirlineId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
</asp:Content>
