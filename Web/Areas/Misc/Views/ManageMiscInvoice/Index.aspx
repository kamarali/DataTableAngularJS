<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/validateRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Misc/MiscInvoiceSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    clearSearchUrl = '<%: Url.Action("ClearSearch", "ManageMiscInvoice") %>';
    $('input[id=3]').parents("tr").hide();
    $(document).ready(function () {
      
      InitializeSubmissionParameters('<%: Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Misc.Receivables.Invoice.Submit) %>', '<%: Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Misc.Receivables.CreditNote.Submit) %>', '<%: Convert.ToInt32(InvoiceType.CreditNote) %>');
    });
  </script>
  <%: ScriptHelper.GenerateRecMiscGridEditViewValidateDeleteScript(Url, ControlIdConstants.SearchGrid, 
    Url.Action("EditInvoice", "ManageMiscInvoice"), 
    Url.RouteUrl("MiscInvoiceSearch", new { controller = "ManageMiscInvoice", action = "ValidateInvoice" }), 
    Url.RouteUrl("MiscInvoiceSearch", new { controller = "ManageMiscInvoice", action = "DeleteInvoice" }), 
    Url.Action("ViewInvoice", "ManageMiscInvoice"),
    Url.Action("DownloadPdf", "ManageInvoice", new { area = "Pax" }), 
    Url.RouteUrl("MiscInvoiceSearch", new { controller = "ManageMiscInvoice", action = "DownloadZip" }), 
    (int)ViewData[ViewDataConstants.RejectionOnValidationFlag])%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Receivables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "ManageMiscInvoice", FormMethod.Post, new { id = "MiscInvoiceSearchForm" }))
      {
      %>

          <%: Html.AntiForgeryToken() %>
          
        <% Html.RenderPartial("~/Views/MiscUatp/InvoiceSearchControl.ascx", Model); %>
    <div class="buttonContainer">
      <input class="primaryButton" type="submit" value="Search" id="btnSearch" />
      <input class="secondaryButton" type="button" onclick="ResetSearch('#MiscInvoiceSearchForm');" value="Clear" />
      <%} 
      %>
    </div>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
  <div class="buttonContainer">
    <input  id = "SubmitInvoicesButton" class="primaryButton" type="button" value="Submit Selected Invoices" onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("SubmitInvoices","ManageMiscInvoice") %>');" />
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.MiscReceivables);%>
  </div>
</asp:Content>
