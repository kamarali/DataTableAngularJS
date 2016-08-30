<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Reports ::<% =ViewData["ReportPageName"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

  <%Html.BeginForm("", "", FormMethod.Post, new { id = "AutoBillingPerformanceReport" }); %>
    <h2>
        <span id="headers"><% =ViewData["ReportPageName"]%></span>
    </h2>
    <div>
        <% Html.RenderPartial("AutoBillingPerformanceSearch", ViewData); %>
    </div>
    <div />
    <div class="buttonContainer">
        <input type="submit" id="generateReport" class="primaryButton" value="Generate Report"
            onclick="CreatePaxAutoBillingPerformanceReport();" />
    </div>
    <%Html.EndForm(); %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    <script type="text/javascript" src="<%:Url.Content("~/Scripts/Reports/PaxAutoBillingPerformanceReport.js")%>"></script>
    <script type="text/javascript">


      function CreatePaxAutoBillingPerformanceReport() {
        ValidateReport("AutoBillingPerformanceReport");
      }

      $(document).ready(function () {

          $("#BilledEntityCode").change(function () {
              if ($("#BilledEntityCode").val() == '') {
                  $("#BilledEntityId").val("");
              }
          });

          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 36 */
          registerAutocomplete('BilledEntityCode', 'BilledEntityId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
      });
    </script>
</asp:Content>
