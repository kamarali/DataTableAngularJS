<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="System.Security.Policy" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/Misc/MiscDailyPayableInvoiceSearch.js")%>" type="text/javascript"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/ValidateDate.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/downloadZip.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Misc/MiscPayAttachmentBreakdown.js")%>" type="text/javascript"></script>
  <script type="text/javascript">
    registerAutocomplete('BilledMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
    clearSearchUrl = '<%: Url.Action("ClearSearch", "ManageMiscDailyPayablesInvoice") %>';

    //CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
    //Initialize Attachment Grid. attach URL for download the attachment. 
    InitializeAttachmentGrid('<%: Url.Action("AttachmentDownload","ManageMiscPayablesInvoice") %>'); 

  </script>
  <%--SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions --%>
  <%:ScriptHelper.GenerateMiscDailyPayablesGridViewScript(Url, ControlIdConstants.SearchGrid,
                                                          Url.Action("ViewInvoice", "ManageMiscDailyPayablesInvoice", new { area = "MiscPayables" }),
                                                          Url.Action("DownloadPdf", "ManageMiscDailyPayablesInvoice", new { area = "MiscPayables" }),
                                                          Url.Action("DownloadListing", "ManageMiscPayablesInvoice", new { area = "MiscPayables" }),
                                                          Url.Action("DownloadZip", "ManageMiscDailyPayablesInvoice", new { area = "MiscPayables" }))%>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Payables :: Manage Invoice
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Invoice Search</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "ManageMiscDailyPayablesInvoice", FormMethod.Post, new { id = "MiscInvoiceSearchForm" }))
      { %>
          <%: Html.AntiForgeryToken() %>
       <%   Html.RenderPartial("~/Views/MiscUatpPayables/DailyPayableInvoiceSearchControl.ascx", Model); %>
    <div class="buttonContainer">
      <input class="primaryButton" type="submit" value="Search" id="btnSearch" />
      <input class="secondaryButton" type="button" onclick="javascript:ResetSearch('#MiscInvoiceSearchForm');"
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
    <% Html.RenderPartial("~/Views/Shared/InvoiceDownloadOptionsControl.ascx", Iata.IS.Model.Enums.InvoiceDownloadOptions.MiscPayables);%>
  </div>
    <%--CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.10: IS-WEB MISC Daily Payables Invoice Search Screen] --%>
  <div class="hidden" id="divAttachment">
    <%
        Html.RenderPartial("MiscPayableAttachmentControl", new Iata.IS.Model.MiscUatp.MiscUatpAttachment());%>
  </div>
</asp:Content>
