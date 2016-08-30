<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.SearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<%@ Import Namespace="System.Security.Policy" %>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions --%>
  <%: ScriptHelper.GenerateScriptForCargoPayableManage(Url, ControlIdConstants.SearchGrid,
                               Url.Action("ViewInvoice", "PayablesInvoiceSearch", new { area = "CargoPayables" }),
                          Url.Action("DownloadPdf", "PayablesInvoiceSearch", new { area = "CargoPayables" }),
                          Url.Action("DownloadZip", "PayablesInvoiceSearch", new { area = "CargoPayables" }))%>
  
  <script type="text/javascript">
      $(document).ready(function () {
          //Url.RouteUrl("CGOInvoiceSearches", new { controller = "PayablesInvoiceSearch", action = "ViewInvoice" }),
          $("#CGOPayablesInvoiceSearchForm").validate({
              rules: {
                  BillingYearMonth: "required",
                  BillingPeriod: "required"
              },
              messages: {
                  BillingYearMonth: "Billing Year / Month required",
                  BillingPeriod: "Billing Period required"
              }
          });
      });

      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 28 */
      registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);

      //Set focus on BillingCode dropdown
      $("#BillingCode").focus();

      //Reset function
      function resetForm() {
          $(':input', '#CGOPayablesInvoiceSearchForm')
        .not(':button, :submit, :reset')
        .val('')
        .removeAttr('selected');
          $("#BillingCode").val("-1");
          $("#BillingPeriod").val("-1");
          $('#SettlementMethodId').val("-1");
      }
  </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Payables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
        using (Html.BeginForm("Index", "PayablesInvoiceSearch", FormMethod.Get, new { id = "CGOPayablesInvoiceSearchForm" }))
      {
        Html.RenderPartial("PayableInvoiceSearchControl", Model);
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
    <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.CargoPayables);%>
  </div>
</asp:Content>
