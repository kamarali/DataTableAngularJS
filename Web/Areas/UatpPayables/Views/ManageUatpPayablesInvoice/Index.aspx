<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content4" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Misc/MiscInvoiceSearch.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
      registerAutocomplete('BilledMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
      clearSearchUrl = '<%: Url.Action("ClearSearch", "ManageUatpPayablesInvoice") %>';
  </script>
<%--   CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
    Only rename the method name from GenerateMiscPayablesGridViewScript to GenerateUatpPayablesGridViewScript.
    So that CMP #665 changes should not be impact on UATP category.--%>
  <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions --%>
  <%:ScriptHelper.GenerateScriptForUatpPayableManage(Url, ControlIdConstants.SearchGrid,
                                                     Url.Action("ViewInvoice", "ManageUatpPayablesInvoice", new { area = "UatpPayables" }),
                                                     Url.Action("DownloadPdf", "ManageUatpPayablesInvoice", new { area = "UatpPayables" }),
                                                     Url.Action("DownloadZip", "ManageUatpPayablesInvoice", new { area = "UatpPayables" }))%>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Uatp :: Payables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
        using (Html.BeginForm("Index", "ManageUatpPayablesInvoice", FormMethod.Get, new { id = "UatpInvoiceSearchForm" }))
      {
        Html.RenderPartial("~/Views/MiscUatpPayables/UatpInvoiceSearchControl.ascx", Model); %>
    <div class="buttonContainer">
      <input class="primaryButton" type="submit" value="Search" />
      <input class="secondaryButton" type="button" onclick="javascript:ResetSearch('#UatpInvoiceSearchForm');"
        value="Clear" />
      <%} 
      %>
    </div>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SearchGrid]); %>
  </div>
  <div id="InvoiceDownloadOptions" class="hidden">
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.UatpPayables);%>
  </div>
</asp:Content>

