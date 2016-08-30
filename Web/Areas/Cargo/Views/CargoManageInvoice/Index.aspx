<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.SearchCriteria>" %>
<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/InvoiceSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/deleterecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/validateRecord.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 20 */
      registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      function resetForm() {
         
          $(':input', '#CGOManageInvoiceSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
          $("#BillingCode").val("-1");
          $("#InvoiceStatus").val("-1");
          $("#SettlementMethodId").val("-1");
          $("#BillingPeriod").val("-1");
          $("#OwnerId").val("-1");
          $("#SubmissionMethodId").val("-1");
          $("#BilledMemberId").val("-1");
          
          
      }
  </script>
  <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions --%>
  <%:ScriptHelper.GenerateScriptForCargoRecManage(Url, ControlIdConstants.SearchGrid,
                                                  Url.Action("EditInvoice", "CargoManageInvoice"),
                                                  Url.RouteUrl("InvoiceSearchCGO", new { controller = "CargoManageInvoice", action = "ValidateInvoice" }),
                                                  Url.RouteUrl("InvoiceSearchCGO", new { controller = "CargoManageInvoice", action = "DeleteInvoice" }),
                                                  Url.RouteUrl("InvoiceSearchCGO", new { controller = "CargoManageInvoice", action = "ViewInvoice" }),
                                                  Url.Action("DownloadPdf", "CargoManageInvoice", new { area = "Cargo" }),
                                                  Url.Action("DownloadZip", "CargoManageInvoice", new { area = "Cargo" }),
                                                  (int)ViewData[ViewDataConstants.RejectionOnValidationFlag])%>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Receivables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <% 
        using (Html.BeginForm("Index", "CargoManageInvoice", FormMethod.Get, new { id = "CGOManageInvoiceSearchForm" }))
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
    <input class="primaryButton" type="button" value="Submit Selected Invoices" onclick="submitInvoices('#<%:ControlIdConstants.SearchGrid %>','<%:Url.Action("SubmitInvoices","CargoManageInvoice") %>');" />
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.CargoReceivables);%>
  </div>
</asp:Content>

