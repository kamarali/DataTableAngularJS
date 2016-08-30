<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Payables :: Manage Supporting Documents
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/PayableSupportingDocument.js")%>"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>
  <script type="text/javascript">
      registerAutocomplete('BillingMemberText', 'BillingMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('SourceCodeId', 'SourceCodeId', '<%:Url.Action("GetEntireSourceCodeList", "Data", new { area = "" })%>', 0, false, null);
      InitializeAttachmentGrid('<%: new FileAttachmentHelper().GetValidFileExtention(Model.BillingMemberId, BillingCategoryType.Pax) %>', '<%: Url.Action("AttachmentDownload","CargoSupportingDoc") %>', '<%: Url.Action("GetPayableSupportingDocBillingYearMonth","Data", new { area = "" }) %>');
      function resetForm() {
          $(':input', '#SupportingDocSearchForm')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
          $("#BillingPeriod").val("-1");
          $("#BillingMemberText").val("");
          $("#SupportingDocumentTypeId").val("1");
          $("#InvoiceNumber").val("");
          $("#SourceCodeId").val("");
          $("#BatchSequenceNumber").val("");
          $("#RecordSequenceWithinBatch").val("");
          $("#TicketDocNumber").val("");
          $("#CouponNumber").val("");
          $("#TicketDocNumber").val("");
          $("#AWBSerialNumber").val("");
          $("#AttachmentIndicatorValidated").val("3");
          $("#DisplayBillingCode").val("-1");
      }
  </script>
   <%-- <%: ScriptHelper.GeneratePayableSupportingDocumentGridScript(Url, ControlIdConstants.CargoPayableSupportingDocSearchResultGrid,
              Url.Action("UploadAttachment", "CargoSupportingDoc", new { area = "Cargo" }))%>--%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage Supporting Documents</h1>
  <h2>
    Search Criteria</h2>
  <div>
    <%
      using (Html.BeginForm("PayableSupportingDocs", "CargoSupportingDoc", FormMethod.Post, new { id = "SupportingDocSearchForm" }))
      {
        Html.RenderPartial("PayableSupportingDocSearchControl", Model);
      } 
    %>
  </div>
  <h2>
    Search Results</h2>
  <div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CargoPayableSupportingDocSearchResultGrid]); %>
  </div>
  <%--<div class="hidden" id="divAttachment">
    <% Html.RenderPartial("PayableAttachmentControl", new Iata.IS.Model.Cargo.Common.SupportingDocumentAttachment()); %>
  </div>--%>
</asp:Content>


