<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Receivables :: Manage Supporting Documents
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/PayableSupportingDocument.js")%>"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetSourceCodeList", "Data", new { area = "" })%>', 0, false, null);
      InitializeAttachmentGrid('<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("AttachmentDownload","SupportingDoc") %>', '<%: Url.Action("GetPayableSupportingDocBillingYearMonth","Data", new { area = "" }) %>');

    });
  </script>
    <%: ScriptHelper.GeneratePayableSupportingDocumentGridScript(Url, ControlIdConstants.SupportingDocSearchResultGrid,
              Url.Action("UploadAttachment", "SupportingDoc", new { area = "Pax" }))%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage Supporting Documents</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("PayableSupportingDocs", "SupportingDoc", FormMethod.Post, new { id = "SupportingDocSearchForm" }))
      {
        Html.RenderPartial("PayableSupportingDocSearchControl", Model);
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SupportingDocSearchResultGrid]); %>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("PayableAttachmentControl", new Iata.IS.Model.Pax.Common.SupportingDocumentAttachment()); %>
  </div>
</asp:Content>


