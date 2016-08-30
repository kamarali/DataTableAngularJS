<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.SearchCriteria>" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/InvoiceSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/validateRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
        /*CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
        Ref: FRS Section 3.4 Table 15 Row 1 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
    function resetForm() {
      $(':input', '#InvoiceSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
      $("#BillingCode").val("-1");
      $("#InvoiceStatus").val("-1");
      $("#SettlementMethodId").val("-1");
      $("#BillingPeriod").val("-1");
      $("#OwnerId").val("-1");
      $("#BilledMemberId").val("-1");
      $("#SubmissionMethodId").val("-1");      
    }
  </script>
  <%: ScriptHelper.GeneratePaxGridEditViewValidateDeleteScript(Url, ControlIdConstants.SearchGrid,
        Url.Action("EditInvoice", "ManageInvoice"),
        Url.RouteUrl("InvoiceSearch", new { controller = "ManageInvoice", action = "ValidateInvoice" }),
        Url.RouteUrl("InvoiceSearch", new { controller = "ManageInvoice", action = "DeleteInvoice" }),
        Url.RouteUrl("InvoiceSearch", new { controller = "ManageInvoice", action = "ViewInvoice" }),
        Url.Action("DownloadPdf", "ManageInvoice", new { area = "Pax" }),
        Url.Action("DownloadZip", "ManageInvoice", new { area = "Pax" }),
        (int)ViewData[ViewDataConstants.RejectionOnValidationFlag])%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Receivables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "ManageInvoice", FormMethod.Get, new { id = "InvoiceSearchForm" }))
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
  <div class="buttonContainer">
    <input class="primaryButton" type="button" value="Submit Selected Invoices" onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("SubmitInvoices","ManageInvoice") %>');" />
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.PaxReceivables);%>
  </div>
</asp:Content>
