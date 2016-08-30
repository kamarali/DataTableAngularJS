<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Reports.Cargo.RMBMCMDetailsModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: Cargo :: RM BM CM Details
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        RM-BM-CM Details
    </h1>
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "RMBMCMDetails" }); %>
    <div>
        <% Html.RenderPartial("RMBMCMDetailsSearch", ViewData); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="RMBMCMDetailsReport();" />
    </div>
    <%Html.EndForm(); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/RMBMCMDetailsReport.js")%>"></script>
    <script type="text/javascript">

        function RMBMCMDetailsReport() {
          ValidateReceivalbesReport("RMBMCMDetails", '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","RMBMCMDetails")%>');
        }
        $(document).ready(function () {
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 52 */
            registerAutocomplete('AirlineCode', 'AirlineId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);

            $("#AirlineCode").change(function () {
                if ($("#AirlineCode").val() == '') {
                    $("#AirlineId").val("");
                }
            });
        });
    </script>
</asp:Content>
