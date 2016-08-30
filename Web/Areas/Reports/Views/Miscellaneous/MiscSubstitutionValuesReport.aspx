<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports :: <%=ViewData["CategoryName"] %> :: Miscellaneous Substitution Values Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%Html.BeginForm("", "", FormMethod.Post, new { id = "MiscSubstitutionValuesReportId" }); %>
        <h1> <span id="headers" >Miscellaneous Substitution Values Report</span></h1>
        <div>
            <% Html.RenderPartial("~/Areas/Reports/Views/Miscellaneous/MiscSubstitutionValuesReportSearchControl.ascx", ViewData); %>
        </div>
        <div class="buttonContainer">
            <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
                onclick="MiscSubstitutionValuesReport();" />
        </div>
    <%Html.EndForm(); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/MiscellaneousReport.js")%>"></script>
    <script type="text/javascript">
        function MiscSubstitutionValuesReport() {
          ValidateMiscSubstitutionValuesReport("MiscSubstitutionValuesReportId", '<%:Url.Action("ValidateBillingMonthYearPeriodSearch","Miscellaneous")%>');
        }
        $(document).ready(function () {
            registerAutocomplete('BillingEntityCode', 'BillingEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
            registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);
        });
    </script>
    <script type="text/javascript" language="javascript">
        $("#ChargeCategory").change(function () {
            var url = '<%: Url.Content("~/")%>' + "Reports/Miscellaneous/GetChargeCodeList?chargeCategoryId=" + $("#ChargeCategory > option:selected").attr("value");
            $.getJSON(url, function (data) {
                var items = "<OPTION value=''>Please Select</OPTION>";
                $.each(data, function (i, ChargeCode) {
                    items += "<OPTION value='" + ChargeCode.Id + "'>" + ChargeCode.Name + "</OPTION>";
                });
                $("#ChargeCode").html(items);
            });
        });   
    </script>
</asp:Content>
