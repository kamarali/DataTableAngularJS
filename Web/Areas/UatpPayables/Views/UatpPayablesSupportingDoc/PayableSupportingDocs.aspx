<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Uatp :: Receivables :: Manage Supporting Documents
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/PayableSupportingDocument.js")%>"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
      registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
      InitializeAttachmentGrid('<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemberId, BillingCategoryType.Uatp) %>', '<%: Url.Action("AttachmentDownload","UatpPayablesSupportingDoc") %>');    
      function resetForm() {
          $(':input', '#SupportingDocSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
          $("#BillingPeriod").val("-1");
          $("#BillingMemberText").val("");
          $("#InvoiceNumber").val("");
          $("#AttachmentIndicatorValidated").val("3");          
      }
  </script>
<%: ScriptHelper.GenerateMiscSupportingDocumentGridScript(Url, ControlIdConstants.SupportingDocSearchResultGrid)%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h1>
    Manage Supporting Documents</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
        using (Html.BeginForm("PayableSupportingDocs", "UatpPayablesSupportingDoc", FormMethod.Post, new { id = "SupportingDocSearchForm" }))
      {%>
      <%: Html.AntiForgeryToken() %>
      <%
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
    <% Html.RenderPartial("PayableAttachmentControl", new Iata.IS.Model.MiscUatp.MiscUatpAttachment()); %>
  </div>
</asp:Content>


