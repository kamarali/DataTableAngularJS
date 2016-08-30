<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Receivables :: Manage Supporting Documents
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/SupportingDocument.js")%>"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
      $(document).ready(function () {
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 9 */
          registerAutocomplete('BilledMemberText', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetEntireSourceCodeList", "Data", new { area = "" })%>', 0, false, null);
      InitializeAttachmentGrid('<%: new FileAttachmentHelper().GetValidFileExtention(Model.BilledMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("AttachmentDownload","SupportingDoc") %>', '<%: Url.Action("GetSupportingDocBillingYearMonth","Data", new { area = "" }) %>');    
 
    });
  </script>
    <%: ScriptHelper.GenerateSupportingDocumentGridScript(Url, ControlIdConstants.SupportingDocSearchResultGrid,
              Url.Action("UploadAttachment", "SupportingDoc", new { area = "Pax" }))%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage Supporting Documents</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("Index", "SupportingDoc", FormMethod.Post, new { id = "SupportingDocSearchForm" }))
      { %>
        <%: Html.AntiForgeryToken() %>
       <% Html.RenderPartial("SupportingDocSearchControl", Model);%>
      <%} 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SupportingDocSearchResultGrid]); %>
  </div>
  <div class="hidden" id="divAttachment">
    <% Html.RenderPartial("AttachmentControl", new Iata.IS.Model.Pax.Common.SupportingDocumentAttachment()); %>
  </div>
</asp:Content>


