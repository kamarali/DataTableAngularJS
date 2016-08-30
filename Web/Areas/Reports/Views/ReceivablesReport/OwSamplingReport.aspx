<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Reports ::
    <%=ViewData["CategoryName"] %> :: <%=ViewData["BillingTypeText"] %>
    :: Sampling Billing Analysis
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
  <%Html.BeginForm("", "", FormMethod.Post, new { id = "OwSamplingReportId" }); %>

<h1>
     <span id="headers"></span>
</h1>

      <div>
        <% Html.RenderPartial("~/Areas/Reports/Views/ReceivablesReport/OWSamplingSearch.ascx", ViewData); %>
    </div>

        <div />

    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="OwSamplingReport();" />
    </div>

     <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

 <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/ReceivablesReport.js")%>"></script>
    <script type="text/javascript">

        function OwSamplingReport() {
            ValidateOwSamplingReport("OwSamplingReportId");
        }
        $(document).ready(function () {
            var cat = '<%=ViewData["BillingTypeText"] %>';

            $('#headers').text(cat.concat(" - Passenger Sampling Billing Analysis"));
            /*CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
            Ref: FRS Section 3.4 Table 15 Row 33 and 39 */
            registerAutocomplete('BilledEntityCode', 'BilledEntityCodeId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);

            $("#BilledEntityCode").change(function () {
                if ($("#BilledEntityCode").val() == '') {
                    $("#BilledEntityCodeId").val("");
                }
            });

        });
    </script>

</asp:Content>
