<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SearchCriteria>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <%: ScriptHelper.GeneratePaxPayableGridScript(Url, ControlIdConstants.SearchGrid,
        Url.Action("ViewInvoice", "ManagePaxPayablesInvoice", new { area = "PaxPayables" }),
        Url.Action("DownloadPdf", "ManagePaxPayablesInvoice", new { area = "PaxPayables" }),
        Url.Action("DownloadZip", "ManagePaxPayablesInvoice", new { area = "PaxPayables" }))%>
  <script type="text/javascript">
    $(document).ready(function () {

      $("#PaxPayablesInvoiceSearchForm").validate({
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
    Ref: FRS Section 3.4 Table 15 Row 16 */
    registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);

    //Set focus on BillingCode dropdown
    $("#BillingCode").focus();

    //Reset function
    function resetForm() {
      $(':input', '#PaxPayablesInvoiceSearchForm')
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
  SIS :: Pax :: Payables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "ManagePaxPayablesInvoice", FormMethod.Get, new { id = "PaxPayablesInvoiceSearchForm" }))
      {
        Html.RenderPartial("InvoiceSearchControl", Model);
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
    <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.PaxPayables);%>
  </div>
</asp:Content>
